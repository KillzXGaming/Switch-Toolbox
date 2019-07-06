using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Switch_Toolbox.Library
{
    public enum ArchiveFileState
    {
        Empty = 0,
        Archived = 1,
        Added = 2,
        Replaced = 4,
        Renamed = 8,
        Deleted = 16
    }

    public interface IArchiveFile
    {
        bool CanAddFiles { get; } 
        bool CanRenameFiles { get; } 
        bool CanReplaceFiles { get; } 
        bool CanDeleteFiles { get; }

        IEnumerable<ArchiveFileInfo> Files { get; }

        bool AddFile(ArchiveFileInfo archiveFileInfo);
        bool DeleteFile(ArchiveFileInfo archiveFileInfo);
    }
    public class ArchiveFileInfo : INode
    {
        [Browsable(false)]
        public string ImageKey { get; set; }

        [Browsable(false)]
        public string SelectedImageKey { get; set; }

        [Browsable(false)]
        public STContextMenuStrip STContextMenuStrip;

        [Browsable(false)]
        public virtual STToolStripItem[] Menus { get; set; }

        [Browsable(false)]
        public FileType FileDataType = FileType.Default;

        //Wether or not to check the file magic to determine the type
        //This sets the icons if there's no proper extension, and may add more special operations
        //This should be disabled on larger archives!
        [Browsable(false)]
        public virtual bool CheckFileMagic { get; set; } = false;

        //Properties to show for the archive file when selected
        [Browsable(false)]
        public virtual object DisplayProperties { get; set; }

        [Browsable(false)]
        public virtual bool CanLoadFile { get; set; } = true;


        [Browsable(false)]
        public virtual IFileFormat OpenFile()
        {
            return STFileLoader.OpenFileFormat(
                IOExtensions.RemoveIllegaleFolderNameCharacters(FileName), FileData, true);
        }

        [Browsable(false)]
        public bool IsSupportedFileFormat()
        {
            if (FileData == null || FileData.Length <= 4)
                return false;

            using (var stream = new MemoryStream(FileData))
            {
                foreach (IFileFormat fileFormat in FileManager.GetFileFormats())
                {
                    fileFormat.FileName = FileName;
                    if (fileFormat.Identify(stream))
                        return true;
                }


                return false;
            }
        }

        public virtual Dictionary<string, string> ExtensionImageKeyLookup { get; }

        public virtual void Replace()
        {
            string fileName = Path.GetFileName(FileName.RemoveIllegaleFileNameCharacters());

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = fileName;
            ofd.DefaultExt = Path.GetExtension(fileName);
            ofd.Filter = "Raw Data (*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileData = File.ReadAllBytes(ofd.FileName);
            }
        }

        public virtual void Export()
        {
            string fileName = Path.GetFileName(FileName.RemoveIllegaleFileNameCharacters());

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = fileName;
            sfd.DefaultExt = Path.GetExtension(fileName);
            sfd.Filter = "Raw Data (*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(sfd.FileName, FileData);
            }
        }

        public virtual string FileSize { get {return STMath.GetFileSize(FileData.Length, 4); } }

        [Browsable(false)]
        public IFileFormat FileFormat = null; //Format attached for saving

        [Browsable(false)]
        protected byte[] _fileData = null;

        //Full File Name
        private string _fileName = string.Empty;

        [Browsable(false)]
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        public void SaveFileFormat()
        {
            if (FileFormat != null && FileFormat.CanSave)
                FileData = FileFormat.Save();
        }

        [Browsable(false)]
        public string Name { get; set; } = string.Empty; //File Name (No Path)

        [Browsable(false)]
        public virtual byte[] FileData
        {
            get { return _fileData; }
            set { _fileData = value; }
        }

        [Browsable(false)]
        public ArchiveFileState State { get; set; } = ArchiveFileState.Empty;
    }

    public class ArchiveBase : TreeNodeCustom
    {
        public IArchiveFile ArchiveFile; //The archive file being edited

        public ArchiveBase(IArchiveFile archiveFile)
        {
            ArchiveFile = archiveFile;
        }
    }

    //Wrapper for the archive file itself
    public class ArchiveRootNodeWrapper : ArchiveBase, IContextMenuNode
    {
        public virtual object PropertyDisplay { get; set; }

        public ArchiveRootNodeWrapper(string text, IArchiveFile archiveFile) 
            : base(archiveFile)
        {
            Text = text;

            PropertyDisplay = new GenericArchiveProperties(archiveFile, text);
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            return new ToolStripItem[]
            {
              new STToolStripItem("Save", SaveAction) { Enabled = ((IFileFormat)ArchiveFile).CanSave},
              new STToolStripSeparator(),
              new STToolStripItem("Repack", RepackAction){ Enabled = ArchiveFile.CanReplaceFiles},
              new STToolStripItem("Extract All", ExtractAllAction),
              new STToolStripSeparator(),
              new STToolStripItem("Preview Archive", PreviewAction),
            };
        }

        private void EnableContextMenu(ToolStripItemCollection Items, string Key, bool Enabled)
        {
            foreach (ToolStripItem item in Items)
            {
                if (item.Text == Key)
                    item.Enabled = Enabled;
            }
        }

        private void SaveAction(object sender, EventArgs args)
        {
            //Archive files are IFIleFormats
            var FileFormat = ((IFileFormat)ArchiveFile);

            Cursor.Current = Cursors.WaitCursor;
            List<IFileFormat> formats = new List<IFileFormat>();
            formats.Add(FileFormat);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(formats);
            sfd.FileName = FileFormat.FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(FileFormat, sfd.FileName);
            }
            GC.Collect();
        }

        private void ExtractAllAction(object sender, EventArgs args)
        {
            TreeNode node = this;

            var ParentPath = string.Empty;
            if (node.Parent != null) //Archive can be attached to another archive
                ParentPath = node.Parent.FullPath;

            TreeHelper.ExtractAllFiles(ParentPath, Nodes);
        }

        private void RepackAction(object sender, EventArgs args)
        {

        }

        private void PreviewAction(object sender, EventArgs args)
        {
            ArchiveListPreviewForm previewFormatList = new ArchiveListPreviewForm();
            previewFormatList.LoadArchive(ArchiveFile);
            previewFormatList.Show();
        }

        public override void OnClick(TreeView treeView)
        {
            STPropertyGrid editor = (STPropertyGrid)LibraryGUI.Instance.GetActiveContent(typeof(STPropertyGrid));
            if (editor == null)
            {
                editor = new STPropertyGrid();
                LibraryGUI.Instance.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;
            editor.LoadProperty(PropertyDisplay, OnPropertyChanged);
        }

        public virtual void OnPropertyChanged() {
            Text = Name;
        }


        public class GenericArchiveProperties
        {
            private IArchiveFile ArchiveFile;

            [Category("Archive Properties")]
            public string Name { get; set; }

            [Category("Archive Properties")]
            [DisplayName("File Count")]
            public int FileCount
            {
                get { return ArchiveFile.Files.ToList().Count; }
            }

            public GenericArchiveProperties(IArchiveFile archiveFile, string text) {
                ArchiveFile = archiveFile;
                Name = text;
            }
        }

        public void FillTreeNodes() {
            FillTreeNodes(this, ArchiveFile);
        }

        private void FillDirectory(TreeNode parent, IEnumerable<INode> Nodes, IArchiveFile archiveFile)
        {
            foreach (var node in Nodes)
            {
                if (node is IDirectoryContainer)
                {
                    var folder = new ArchiveFolderNodeWrapper(node.Name, archiveFile);
                    parent.Nodes.Add(folder);

                    if (((IDirectoryContainer)node).Nodes != null)
                        FillDirectory(folder, ((IDirectoryContainer)node).Nodes, archiveFile);
                }
                else if (node is ArchiveFileInfo)
                {
                    ArchiveFileWrapper wrapperFile = new ArchiveFileWrapper(node.Name, (ArchiveFileInfo)node, archiveFile);
                    wrapperFile.Name = node.Name;
                    parent.Nodes.Add(wrapperFile);
                }
            }
        }

        private void FillTreeNodes(TreeNode root, IArchiveFile archiveFile)
        {
            Nodes.Clear();

            var rootText = root.Text;
            var rootTextLength = rootText.Length;
            var nodeFiles = archiveFile.Files;

            if (archiveFile is IDirectoryContainer)
            {
                FillDirectory(root,((IDirectoryContainer)archiveFile).Nodes, archiveFile);
            }
            else //Else create directories by filename paths
            {

                int I = 0;
                foreach (var node in archiveFile.Files)
                {
                    if (!node.CanLoadFile)
                        continue;

                    string nodeString = node.FileName;

                    var roots = nodeString.Split(new char[] { '/' },
                        StringSplitOptions.RemoveEmptyEntries);

                    // The initial parent is the root node
                    var parentNode = root;
                    var sb = new System.Text.StringBuilder(rootText, nodeString.Length + rootTextLength);
                    for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
                    {
                        // Build the node name
                        var parentName = roots[rootIndex];
                        sb.Append("/");
                        sb.Append(parentName);
                        var nodeName = sb.ToString();

                        // Search for the node
                        var index = parentNode.Nodes.IndexOfKey(nodeName);
                        if (index == -1)
                        {
                            // Node was not found, add it

                            var folder = new ArchiveFolderNodeWrapper(parentName, archiveFile);

                            if (rootIndex == roots.Length - 1)
                            {
                                ArchiveFileWrapper wrapperFile = new ArchiveFileWrapper(parentName, node, archiveFile);
                                wrapperFile.Name = nodeName;
                                parentNode.Nodes.Add(wrapperFile);
                                parentNode = wrapperFile;
                            }
                            else
                            {
                                folder.Name = nodeName;
                                parentNode.Nodes.Add(folder);
                                parentNode = folder;
                            }
                        }
                        else
                        {
                            // Node was found, set that as parent and continue
                            parentNode = parentNode.Nodes[index];
                        }
                    }
                }
            }
        }
    }

    //Wrapper for folders
    public class ArchiveFolderNodeWrapper : ArchiveBase, IContextMenuNode
    {
        public virtual object PropertyDisplay { get; set; }

        public bool CanReplace
        {
            set
            {
                if (value)
                    ContextMenuStrip.Items[1].Enabled = true;
                else
                    ContextMenuStrip.Items[1].Enabled = false;
            }
        }
        public bool CanRename = false;
        public bool CanDelete
        {
            set
            {
                if (value)
                    ContextMenuStrip.Items[2].Enabled = true;
                else
                    ContextMenuStrip.Items[2].Enabled = false;
            }
        }

        public ArchiveFolderNodeWrapper(string text, IArchiveFile archiveFile ) : base(archiveFile)
        {
            Text = text;
            PropertyDisplay = new GenericFolderProperties();
            ((GenericFolderProperties)PropertyDisplay).Name = Text;

         //   ReloadMenus(archiveFile);
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            return new ToolStripItem[]
            {
                new STToolStripItem("Rename", RenameAction) { Enabled = ArchiveFile.CanRenameFiles },
                new STToolStripItem("Extract Folder", ExtractAction),
                new STToolStripItem("Replace Folder", ReplaceAction) { Enabled = ArchiveFile.CanReplaceFiles },
                new STToolStripItem("Delete Folder", DeleteAction) { Enabled = ArchiveFile.CanDeleteFiles },
                new STToolStripSeparator(),
               new STToolStripItem("Add File", AddFileAction) { Enabled = ArchiveFile.CanAddFiles },
            };
        }

        private void AddFileAction(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Raw Data (*.*)|*.*";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK) {
                TreeHelper.AddFiles(this, ArchiveFile, ofd.FileNames);
            }
        }

        public override void OnClick(TreeView treeView)
        {
            STPropertyGrid editor = (STPropertyGrid)LibraryGUI.Instance.GetActiveContent(typeof(STPropertyGrid));
            if (editor == null)
            {
                editor = new STPropertyGrid();
                LibraryGUI.Instance.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;
            editor.LoadProperty(PropertyDisplay, OnPropertyChanged);
        }

        public class GenericFolderProperties
        {
            [Category("Folder Properties")]
            public string Name { get; set; }
        }

        public virtual void OnPropertyChanged() {
            Text = Name;
        }

        private void ExtractAction(object sender, EventArgs args)
        {
            TreeNode node = this;

            var ParentPath = string.Empty;
            if (node.Parent != null)
                ParentPath = node.Parent.FullPath;

            TreeHelper.ExtractAllFiles(ParentPath, Nodes);
        }

        private void RenameAction(object sender, EventArgs args)
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK) { Text = dialog.textBox1.Text; }
        }

        private void ReplaceAction(object sender, EventArgs args)
        {
        }

        private void DeleteAction(object sender, EventArgs args) {
            TreeHelper.RemoveFolder(this, ArchiveFile);
        }
    }

    //Wrapper for files
    public class ArchiveFileWrapper : ArchiveBase, IContextMenuNode
    {
        public virtual ArchiveFileInfo ArchiveFileInfo { get; set; }

        public ArchiveFileWrapper(string text, ArchiveFileInfo archiveFileInfo, IArchiveFile archiveFile) : base(archiveFile)
        {
            Text = text;
        //    ReloadMenus(archiveFile);

            ArchiveFileInfo = archiveFileInfo;

            string Extension = Utils.GetExtension(text);
            if (ArchiveFileInfo.CheckFileMagic)
            {
                Extension = FindMatch(archiveFileInfo.FileData);
            }

            switch (Extension)
            {
                case ".bntx": SetImageKey("bntx"); break;
                case ".byaml": SetImageKey("byaml"); break;
                case ".byml": SetImageKey("byaml"); break;
                case ".aamp": SetImageKey("aamp"); break;
                case ".bfres": SetImageKey("bfres"); break;
                case ".sbfres": SetImageKey("sbfres"); break;
                case ".dds":
                case ".tga":
                case ".jpg":
                case ".jpeg":
                case ".tiff":
                case ".png":
                case ".gif":
                case ".astc":
                    SetImageKey("texture"); break;

                default: SetImageKey("fileBlank"); break;
            }

            if (ArchiveFileInfo.ExtensionImageKeyLookup != null)
            {
                if (ArchiveFileInfo.ExtensionImageKeyLookup.ContainsKey(Extension))
                    SetImageKey(ArchiveFileInfo.ExtensionImageKeyLookup[Extension]);
            }
        }

        private void SetImageKey(string Key) {
            ImageKey = Key;
            SelectedImageKey = Key;
        }

        private string FindMatch(byte[] f)
        {
            if (f.Matches("SARC")) return ".szs";
            else if (f.Matches("Yaz")) return ".szs";
            else if (f.Matches("YB") || f.Matches("BY")) return ".byaml";
            else if (f.Matches("FRES")) return ".bfres";
            else if (f.Matches("Gfx2")) return ".gtx";
            else if (f.Matches("FLYT")) return ".bflyt";
            else if (f.Matches("CLAN")) return ".bclan";
            else if (f.Matches("CLYT")) return ".bclyt";
            else if (f.Matches("FLIM")) return ".bclim";
            else if (f.Matches("FLAN")) return ".bflan";
            else if (f.Matches("FSEQ")) return ".bfseq";
            else if (f.Matches("VFXB")) return ".pctl";
            else if (f.Matches("AAHS")) return ".sharc";
            else if (f.Matches("BAHS")) return ".sharcb";
            else if (f.Matches("BNTX")) return ".bntx";
            else if (f.Matches("BNSH")) return ".bnsh";
            else if (f.Matches("FSHA")) return ".bfsha";
            else if (f.Matches("FFNT")) return ".bffnt";
            else if (f.Matches("CFNT")) return ".bcfnt";
            else if (f.Matches("CSTM")) return ".bcstm";
            else if (f.Matches("FSTM")) return ".bfstm";
            else if (f.Matches("STM")) return ".bfsha";
            else if (f.Matches("CWAV")) return ".bcwav";
            else if (f.Matches("FWAV")) return ".bfwav";
            else if (f.Matches("CTPK")) return ".ctpk";
            else if (f.Matches("CGFX")) return ".bcres";
            else if (f.Matches("AAMP")) return ".aamp";
            else if (f.Matches("MsgStdBn")) return ".msbt";
            else if (f.Matches("MsgPrjBn")) return ".msbp";
            else if (f.Matches(0x00000004)) return ".gfbanm";
            else if (f.Matches(0x00000014)) return ".gfbanm";
            else if (f.Matches(0x00000018)) return ".gfbanmcfg";
            else if (f.Matches(0x00000020)) return ".gfbmdl";
            else if (f.Matches(0x00000044)) return ".gfbpokecfg";
            else return "";
        }

        public static ArchiveFileWrapper FromPath(string FilePath, IArchiveFile archiveFile)
        {
            var ArchiveFileInfo = new ArchiveFileInfo();
            ArchiveFileInfo.FileName = FilePath;
            ArchiveFileInfo.FileData = File.ReadAllBytes(FilePath);
            return new ArchiveFileWrapper(Path.GetFileName(FilePath), ArchiveFileInfo, archiveFile);
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            return new ToolStripItem[]
            {
                new STToolStripItem("Rename", RenameAction) { Enabled = ArchiveFile.CanRenameFiles },
                new STToolStripItem("Export Raw Data", ExtractAction),
                new STToolStipMenuItem("Export Raw Data to File Location", null, ExportToFileLocAction, Keys.Control | Keys.F),
                new STToolStripItem("Replace Raw Data", ReplaceAction) { Enabled = ArchiveFile.CanReplaceFiles },
                new STToolStripSeparator(),
                new STToolStipMenuItem("Open With Text Editor", null, OpenTextEditorAction, Keys.Control | Keys.T),
                new STToolStripSeparator(),
                new STToolStripItem("Delete", DeleteAction) { Enabled = ArchiveFile.CanDeleteFiles },
            };
        }

        private void OpenTextEditorAction(object sender, EventArgs args)
        {
            TextEditor editor = (TextEditor)LibraryGUI.Instance.GetActiveContent(typeof(TextEditor));
            if (editor == null)
            {
                editor = new TextEditor();
                LibraryGUI.Instance.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;
            editor.FillEditor(ArchiveFileInfo.FileData);
        }

        private void ExtractAction(object sender, EventArgs args)
        {
            ArchiveFileInfo.Export();
        }

        private void ExportToFileLocAction(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            File.WriteAllBytes($"{Path.GetDirectoryName(((IFileFormat)ArchiveFile).FilePath)}/{Text}", ArchiveFileInfo.FileData);
            Cursor.Current = Cursors.Default;
        }

        private void DeleteAction(object sender, EventArgs args)
        {
            DialogResult result = MessageBox.Show($"Are your sure you want to remove {Text}? This cannot be undone!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ArchiveFile.DeleteFile(ArchiveFileInfo);
                Parent.Nodes.Remove(this);
            }
        }

        private void ReplaceAction(object sender, EventArgs args)
        {
            ArchiveFileInfo.Replace();
        }

        public override void OnDoubleMouseClick(TreeView treeview)
        {
            IFileFormat file = ArchiveFileInfo.OpenFile();
            if (file == null) //Format not supported so return
                return;

            ArchiveFileInfo.FileFormat = file;

            if (Utils.HasInterface(file.GetType(), typeof(IEditor<>)))
            {
                OpenFormDialog(file);
            }
            else if (file is TreeNode)
                ReplaceNode(this.Parent, this, (TreeNode)file);
            else if (file is IArchiveFile)
            {
                var FileRoot = new ArchiveRootNodeWrapper(file.FileName, (IArchiveFile)file);
                FileRoot.FillTreeNodes();
                ReplaceNode(this.Parent, this, FileRoot);
            }
        }

        private void OpenFormDialog(IFileFormat fileFormat)
        {
            UserControl form = GetEditorForm(fileFormat);
            form.Text = (((IFileFormat)fileFormat).FileName);

            var parentForm = LibraryGUI.Instance.GetActiveForm();

            GenericEditorForm editorForm = new GenericEditorForm(true, form);
            editorForm.FormClosing += (sender, e) => FormClosing(sender, e, fileFormat);
            if (editorForm.ShowDialog() == DialogResult.OK)
            {
                if (fileFormat.CanSave)
                {
                    ArchiveFileInfo.SaveFileFormat();
                    UpdateEditor();
                }
            }
        }

        private void FormClosing(object sender, EventArgs args, IFileFormat fileFormat)
        {
            if (((Form)sender).DialogResult != DialogResult.OK)
                return;
        }

        public UserControl GetEditorForm(IFileFormat fileFormat)
        {
            Type objectType = fileFormat.GetType();
            foreach (var inter in objectType.GetInterfaces())
            {
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IEditor<>))
                {
                    System.Reflection.MethodInfo method = objectType.GetMethod("OpenForm");
                    return (UserControl)method.Invoke(fileFormat, new object[0]);
                }
            }
            return null;
        }

        public override void OnClick(TreeView treeView) {
            UpdateEditor();
        }

        public void UpdateEditor()
        {
            ArchiveFilePanel editor = (ArchiveFilePanel)LibraryGUI.Instance.GetActiveContent(typeof(ArchiveFilePanel));
            if (editor == null)
            {
                editor = new ArchiveFilePanel();
                editor.Dock = DockStyle.Fill;
                LibraryGUI.Instance.LoadEditor(editor);
            }

            editor.LoadFile(ArchiveFileInfo);
            editor.UpdateEditor();
        }

        public static void ReplaceNode(TreeNode node, TreeNode replaceNode, TreeNode NewNode)
        {
            if (NewNode == null)
                return;

            int index = node.Nodes.IndexOf(replaceNode);
            node.Nodes.RemoveAt(index);
            node.Nodes.Insert(index, NewNode);

            NewNode.ImageKey = replaceNode.ImageKey;
            NewNode.SelectedImageKey = replaceNode.SelectedImageKey;
            NewNode.Text = replaceNode.Text;
        }

        private void RenameAction(object sender, EventArgs args)
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK) { Text = dialog.textBox1.Text; }
        }
    }
}
