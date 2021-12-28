using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;
using System.Text.RegularExpressions;

namespace Toolbox.Library
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

    /// <summary>
    /// A common archive format interface used to edit archive file formats
    /// </summary>
    public interface IArchiveFile
    {
        bool CanAddFiles { get; } 
        bool CanRenameFiles { get; } 
        bool CanReplaceFiles { get; } 
        bool CanDeleteFiles { get; }

        IEnumerable<ArchiveFileInfo> Files { get; }

        void ClearFiles();
        bool AddFile(ArchiveFileInfo archiveFileInfo);
        bool DeleteFile(ArchiveFileInfo archiveFileInfo);
    }

    public class ArchiveFileInfo : INode
    {
        // Opens the file format automatically (may take longer to open the archive file)
        [Browsable(false)]
        public virtual bool OpenFileFormatOnLoad { get; set; }

        [Browsable(false)]
        // The source file. If an archive is in another archive, this is necessary to get the original path
        public string SourceFile { get; internal set; }

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
        public ArchiveFileWrapper FileWrapper;

        public void UpdateWrapper()
        {
            if (FileWrapper == null) return;

            FileWrapper.Text = Path.GetFileName(FileName);
        }

        [Browsable(false)]
        public virtual IFileFormat OpenFile()
        {
            if (FileFormat != null)
                return FileFormat;

            if (FileDataStream != null)
            {
                return STFileLoader.OpenFileFormat(FileDataStream,
                IOExtensions.RemoveIllegaleFolderNameCharacters(FileName), true, true);
            }
            else
            {
                return STFileLoader.OpenFileFormat(new MemoryStream(FileData),
                IOExtensions.RemoveIllegaleFolderNameCharacters(FileName), false, true);
            }
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

        [Browsable(false)]
        public virtual Dictionary<string, string> ExtensionImageKeyLookup { get; }

        public virtual bool Replace()
        {
            string fileName = Path.GetFileName(FileName.RemoveIllegaleFileNameCharacters());

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = fileName;
            ofd.DefaultExt = Path.GetExtension(fileName);
            ofd.Filter = "Raw Data (*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (FileDataStream != null)
                    FileDataStream = new MemoryStream(File.ReadAllBytes(ofd.FileName));
                else
                    FileData = File.ReadAllBytes(ofd.FileName);
                return true;
            }
            return false;
        }

        public virtual void Export()
        {
            string fileName = Path.GetFileName(FileName.RemoveIllegaleFolderNameCharacters());

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = fileName;
            sfd.DefaultExt = Path.GetExtension(fileName);
            sfd.Filter = "Raw Data (*.*)|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (FileDataStream != null)
                    FileDataStream.ExportToFile(sfd.FileName);
                else
                    File.WriteAllBytes(sfd.FileName, FileData);
            }
        }

        public virtual string FileSize { get {return STMath.GetFileSize(
            FileDataStream != null ? FileDataStream.Length : FileData.Length, 4); } }

        [Browsable(false)]
        public IFileFormat FileFormat = null; //Format attached for saving

        [Browsable(false)]
        private byte[] _fileData = null;

        //Full File Name
        private string _fileName = string.Empty;

        [Browsable(false)]
        public virtual string FileName
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

        public static void SaveFileFormat(ArchiveFileInfo archiveFile, IFileFormat fileFormat)
        {
            if (fileFormat != null && fileFormat.CanSave)
            {
                if (archiveFile.FileDataStream != null)
                {
                    var mem = new System.IO.MemoryStream();
                    fileFormat.Save(mem);
                    archiveFile.FileDataStream = mem;
                    //Reload file data
                    fileFormat.Load(archiveFile.FileDataStream);
                }
                else
                {
                    var mem = new System.IO.MemoryStream();
                    fileFormat.Save(mem);
                    archiveFile.FileData = STLibraryCompression.CompressFile(mem.ToArray(), fileFormat);
                }
            }
        }

        public void SaveFileFormat()
        {
            if (FileFormat != null && FileFormat.CanSave)
            {
                if (FileDataStream != null)
                {
                    Console.WriteLine($"Updating FileDataStream " + (FileDataStream is FileStream));
                    if (FileDataStream is FileStream)
                        FileDataStream.Close();

                    var mem = new System.IO.MemoryStream();
                    FileFormat.Save(mem);
                    FileDataStream = mem;
                    //Reload file data
                    FileFormat.Load(FileDataStream);
                }
                else
                {
                    var mem = new System.IO.MemoryStream();
                    FileFormat.Save(mem);
                    FileData = STLibraryCompression.CompressFile(mem.ToArray(), FileFormat);
                }
            }
        }

        [Browsable(false)]
        public string Name { get; set; } = string.Empty; //File Name (No Path)

        [Browsable(false)]
        public virtual byte[] FileData
        {
            get { return _fileData; }
            set { _fileData = value; }
        }

        public virtual Stream FileDataStream
        {
            get
            {
                if (_fileStream != null)
                    _fileStream.Position = 0;

                return _fileStream;
            }
            set { _fileStream = value; }
        }

        protected Stream _fileStream = null;

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
        //A list that links archive file infos to treenodes of varying types
        public List<Tuple<ArchiveFileInfo, TreeNode>> FileNodes = new List<Tuple<ArchiveFileInfo, TreeNode>>();

        public virtual object PropertyDisplay { get; set; }

        public ArchiveRootNodeWrapper(string text, IArchiveFile archiveFile) 
            : base(archiveFile)
        {
            Text = text;

            if (archiveFile is IPropertyContainer)
                PropertyDisplay = ((IPropertyContainer)archiveFile).Property;
            else
                PropertyDisplay = new GenericArchiveProperties(archiveFile, text, this);
        }

        public void AddFileNode(ArchiveFileWrapper fileWrapper)
        {
            FileNodes.Add(Tuple.Create(fileWrapper.ArchiveFileInfo, (TreeNode)fileWrapper));
            fileWrapper.ArchiveFileInfo.FileWrapper = fileWrapper;

            string FullName = SetFullPath(fileWrapper, this);
            if (FullName != string.Empty)
            {
                Console.WriteLine($"Updating info {FullName}");
                fileWrapper.ArchiveFileInfo.FileName = FullName;
            }
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            var ToolStrips = new ToolStripItem[]
{
                    new STToolStripItem("Save", SaveAction) { Enabled = ((IFileFormat)ArchiveFile).CanSave},
                    new STToolStripSeparator(),
                    new STToolStripItem("Repack", RepackAction){ Enabled = ArchiveFile.CanAddFiles },
                    new STToolStripItem("Extract All", ExtractAllAction),
                    new STToolStripSeparator(),
                    new STToolStripItem("Preview Archive", PreviewAction),
                    new STToolStripSeparator(),
                    new STToolStripItem("Add Folder", AddFolderAction) { Enabled = ArchiveFile.CanAddFiles},
                    new STToolStripItem("Add File", AddFileAction) { Enabled = ArchiveFile.CanAddFiles},
                    new STToolStripItem("Clear Files", ClearAction) { Enabled = ArchiveFile.CanDeleteFiles},
            };

            var toolStripList = ToolStrips.ToList();
            if (ArchiveFile is IContextMenuNode)
            {
                toolStripList.AddRange(((IContextMenuNode)ArchiveFile).GetContextMenuItems());
            }

            return toolStripList.ToArray();
        }

        private void ClearAction(object sender, EventArgs args)
        {
            //Clear all nodes
            for (int i = 0; i < FileNodes.Count; i++)
                ArchiveFile.DeleteFile(FileNodes[i].Item1);

            Nodes.Clear();
            FileNodes.Clear();
        }

        private void AddFolderAction(object sender, EventArgs args)
        {
            Nodes.Add(new ArchiveFolderNodeWrapper("NewFolder", ArchiveFile, this));
        }

        private void AddFileAction(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Raw Data (*.*)|*.*";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TreeHelper.AddFiles(this, this, ofd.FileNames);
            }
        }

        public void UpdateFileNames()
        {
            if (!ArchiveFile.CanRenameFiles)
                return;

            for (int i = 0; i < FileNodes.Count; i++)
            {
                string NewName = SetFullPath(FileNodes[i].Item2, this);
                if (NewName != string.Empty)
                {
                    FileNodes[i].Item1.Name = FileNodes[i].Item2.Text;
                    FileNodes[i].Item1.FileName = NewName;
                }
            }
        }

        private static string SetFullPath(TreeNode node, TreeNode root)
        {
            if (node.TreeView == null) {
                return string.Empty;
            }

            string nodePath = node.FullPath;
            int startIndex = nodePath.IndexOf(root.Text);
            if (startIndex > 0)
                nodePath = nodePath.Substring(startIndex);

            string slash = Path.DirectorySeparatorChar.ToString();
            string slashAlt = Path.AltDirectorySeparatorChar.ToString();

            string SetPath = nodePath.Replace(root.Text + slash, string.Empty).Replace(slash ?? "", slashAlt);

            Console.WriteLine($"FullPath { node.FullPath}");
            Console.WriteLine($"SetPath {SetPath}");

            return SetPath;
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
            Save("", true);
        }

        private void Save(string FileName, bool UseDialog)
        {
            UpdateFileNames();

            //Archive files are IFIleFormats
            var FileFormat = ((IFileFormat)ArchiveFile);

            Cursor.Current = Cursors.WaitCursor;
            if (UseDialog)
            {
                List<IFileFormat> formats = new List<IFileFormat>();
                formats.Add(FileFormat);

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = Utils.GetAllFilters(formats);
                sfd.FileName = FileFormat.FileName;

                if (sfd.ShowDialog() == DialogResult.OK)
                    FileName = sfd.FileName;
                else
                    return;
            }

            STFileSaver.SaveFileFormat(FileFormat, FileName);

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
            FolderSelectDialog dlg = new FolderSelectDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string FolderPath = dlg.SelectedPath;

                STProgressBar progressBar = new STProgressBar();
                progressBar.Task = "Reading Directory...";
                progressBar.Value = 0;
                progressBar.StartPosition = FormStartPosition.CenterScreen;
                progressBar.Show();
                progressBar.Refresh();

                var ProccessedFiles = TreeHelper.ReadFiles(FolderPath);

                progressBar.Task = "Repacking Files...";
                progressBar.Refresh();

                ArchiveFile.ClearFiles();

                for (int i = 0; i < ProccessedFiles.Count; i++)
                {
                    progressBar.Value = (i * 100) / ProccessedFiles.Count;
                    progressBar.Task = $"Packing {ProccessedFiles[i].Item1}";
                    progressBar.Refresh();

                    ArchiveFile.AddFile(new ArchiveFileInfo()
                    {
                        FileName = ProccessedFiles[i].Item1,
                        FileData = File.ReadAllBytes(ProccessedFiles[i].Item2),
                    });
                }

                progressBar.Close();
                progressBar.Dispose();
                ProccessedFiles.Clear();

                GC.Collect();

                FillTreeNodes();
            }
        }

        private void PreviewAction(object sender, EventArgs args)
        {
            ArchiveListPreviewForm previewFormatList = new ArchiveListPreviewForm();
            previewFormatList.LoadArchive(ArchiveFile);
            previewFormatList.Show();
        }

        public override void OnClick(TreeView treeView)
        {
            STPropertyGrid editor = (STPropertyGrid)LibraryGUI.GetActiveContent(typeof(STPropertyGrid));
            if (editor == null)
            {
                editor = new STPropertyGrid();
                LibraryGUI.LoadEditor(editor);
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
            private ArchiveRootNodeWrapper rootNode;

            private IArchiveFile ArchiveFile;

            [Category("Archive Properties")]
            public string Name { get; set; }

            [Category("Archive Properties")]
            [DisplayName("File Count")]
            public int FileCount
            {
                get
                {
                    return rootNode.FileNodes != null ? rootNode.FileNodes.Count : 0;
                }
            }

            public GenericArchiveProperties(IArchiveFile archiveFile, string text, ArchiveRootNodeWrapper root) {
                ArchiveFile = archiveFile;
                Name = text;
                rootNode = root;
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
                    var folder = new ArchiveFolderNodeWrapper(node.Name, archiveFile, this);
                    folder.DirectoryContainer = (IDirectoryContainer)node;
                    parent.Nodes.Add(folder);

                    if (((IDirectoryContainer)node).Nodes != null)
                        FillDirectory(folder, ((IDirectoryContainer)node).Nodes, archiveFile);
                }
                else if (node is ArchiveFileInfo)
                {
                    ArchiveFileWrapper wrapperFile = new ArchiveFileWrapper(node.Name, (ArchiveFileInfo)node, this);
                    wrapperFile.Name = node.Name;
                    parent.Nodes.Add(wrapperFile);
                    AddFileNode(wrapperFile);
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
                foreach (var node in archiveFile.Files)
                {
                    if (!node.CanLoadFile)
                        continue;

                    if (!((IFileFormat)archiveFile).IFileInfo.InArchive && File.Exists(((IFileFormat)archiveFile).FilePath))
                        node.SourceFile = ((IFileFormat)archiveFile).FilePath;

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

                            var folder = new ArchiveFolderNodeWrapper(parentName, archiveFile, this);

                            if (rootIndex == roots.Length - 1)
                            {
                                ArchiveFileWrapper wrapperFile = new ArchiveFileWrapper(parentName, node, this);
                                wrapperFile.Name = nodeName;
                                parentNode.Nodes.Add(wrapperFile);
                                parentNode = wrapperFile;
                                AddFileNode(wrapperFile);
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
        public ArchiveRootNodeWrapper RootNode;

        public virtual object PropertyDisplay { get; set; }

        public IDirectoryContainer DirectoryContainer { get; set; }

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

        public ArchiveFolderNodeWrapper(string text, IArchiveFile archiveFile, ArchiveRootNodeWrapper root ) : base(archiveFile)
        {
            RootNode = root;
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
                new STToolStripItem("Replace Folder", ReplaceAction) { Enabled = ArchiveFile.CanReplaceFiles && ArchiveFile.CanAddFiles },
                new STToolStripItem("Delete Folder", DeleteAction) { Enabled = ArchiveFile.CanDeleteFiles },
                new STToolStripSeparator(),
                new STToolStripItem("Add Folder", AddFolderAction) { Enabled = ArchiveFile.CanAddFiles },
                new STToolStripItem("Add File", AddFileAction) { Enabled = ArchiveFile.CanAddFiles },
                new STToolStripItem("Clear Files", ClearAction) { Enabled = ArchiveFile.CanDeleteFiles },
            };
        }

        private void AddFolderAction(object sender, EventArgs args)
        {
            Nodes.Add(new ArchiveFolderNodeWrapper("NewFolder", ArchiveFile, RootNode));
        }

        private void AddFileAction(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Raw Data (*.*)|*.*";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK) {
                TreeHelper.AddFiles(this, RootNode, ofd.FileNames);
            }
        }

        private void ClearAction(object sender, EventArgs args)
        {
            foreach (var node in TreeViewExtensions.Collect(Nodes))
            {
                if (node is ArchiveFileWrapper)
                    ArchiveFile.DeleteFile(((ArchiveFileWrapper)node).ArchiveFileInfo);
            }

            Nodes.Clear();
        }

        public override void OnClick(TreeView treeView)
        {
            STPropertyGrid editor = (STPropertyGrid)LibraryGUI.GetActiveContent(typeof(STPropertyGrid));
            if (editor == null)
            {
                editor = new STPropertyGrid();
                LibraryGUI.LoadEditor(editor);
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

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Text = dialog.textBox1.Text;
                if (DirectoryContainer != null)
                    DirectoryContainer.Name = Text;
            }
        }

        private void ReplaceAction(object sender, EventArgs args) {
            //Add folders and files from selected path
            FolderSelectDialog ofd = new FolderSelectDialog();
            if (ofd.ShowDialog() == DialogResult.OK) {
                //Clear all nodes
                foreach (var node in TreeViewExtensions.Collect(Nodes))
                {
                    if (node is ArchiveFileWrapper)
                        ArchiveFile.DeleteFile(((ArchiveFileWrapper)node).ArchiveFileInfo);
                }

                Nodes.Clear();

                var proccessedFiles = TreeHelper.ReadFiles(ofd.SelectedPath);

                string folderPath = TreeHelper.GetFolderAbsoultePath(this, RootNode);
                for (int i = 0; i < proccessedFiles.Count; i++)
                {
                    ArchiveFile.AddFile(new ArchiveFileInfo()
                    {
                        FileName = $"{folderPath}/{proccessedFiles[i].Item1}",
                        FileData = File.ReadAllBytes(proccessedFiles[i].Item2),
                    });
                }
                RootNode.FillTreeNodes();
            }
        }

        private void DeleteAction(object sender, EventArgs args) {
            TreeHelper.RemoveFolder(this, ArchiveFile);
        }
    }

    //Wrapper for files
    public class ArchiveFileWrapper : ArchiveBase, IContextMenuNode
    {
        public ArchiveRootNodeWrapper RootNode;

        public virtual ArchiveFileInfo ArchiveFileInfo { get; set; }

        public ArchiveFileWrapper(string text, ArchiveFileInfo archiveFileInfo, ArchiveRootNodeWrapper rootNode) : base(rootNode.ArchiveFile)
        {
            Text = text;
            RootNode = rootNode;
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
                case ".gfbmdl": SetImageKey("model"); break;
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
            else if (f.Matches("VFXB")) return ".ptcl";
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

        public static ArchiveFileWrapper FromPath(string FilePath, ArchiveRootNodeWrapper rootNode)
        {
            var ArchiveFileInfo = new ArchiveFileInfo();
            ArchiveFileInfo.FileName = FilePath;
            ArchiveFileInfo.FileData = File.ReadAllBytes(FilePath);
            return new ArchiveFileWrapper(Path.GetFileName(FilePath), ArchiveFileInfo, rootNode);
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
            TextEditor editor = (TextEditor)LibraryGUI.GetActiveContent(typeof(TextEditor));
            if (editor == null)
            {
                editor = new TextEditor();
                LibraryGUI.LoadEditor(editor);
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
            string path = FileManager.GetSourcePath(((IFileFormat)ArchiveFile));
            string folder = Path.GetDirectoryName(path);
            string filePath = Path.Combine(folder, Text);

            Cursor.Current = Cursors.WaitCursor;
            if (ArchiveFileInfo.FileDataStream != null)
                ArchiveFileInfo.FileDataStream.ExportToFile(filePath);
            else
                File.WriteAllBytes(filePath, ArchiveFileInfo.FileData);

            Cursor.Current = Cursors.Default;
        }

        private void DeleteAction(object sender, EventArgs args)
        {
            DialogResult result = MessageBox.Show($"Are your sure you want to remove {Text}? This cannot be undone!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                bool isRemoved = ArchiveFile.DeleteFile(ArchiveFileInfo);
                if (!isRemoved) return;

                if (Parent != null)
                    Parent.Nodes.Remove(this);
                else if (ArchiveFileInfo.FileFormat != null && 
                         ArchiveFileInfo.FileFormat is TreeNode)
                {
                    var prevNode = (TreeNode)ArchiveFileInfo.FileFormat;
                    var parent = prevNode.Parent;

                    var index = parent.Nodes.IndexOf(prevNode);
                    parent.Nodes.RemoveAt(index);
                }
            }
        }

        private void ReplaceAction(object sender, EventArgs args)
        {
            if (!ArchiveFile.CanReplaceFiles) return;

            bool IsReplaced = ArchiveFileInfo.Replace();
            if (!IsReplaced) return;

            if (ArchiveFileInfo.FileFormat != null)
            {
                if (ArchiveFileInfo.FileFormat is IArchiveFile)
                {
                 
                }
                if (ArchiveFileInfo.FileFormat is TreeNode)
                {
                    var prevNode = (TreeNode)ArchiveFileInfo.FileFormat;
                    var parent = prevNode.Parent;
                    if (parent != null)
                    {
                        var index = parent.Nodes.IndexOf(prevNode);
                        parent.Nodes.RemoveAt(index);
                        parent.Nodes.Insert(index, this);
                    }
                }

                ArchiveFileInfo.FileFormat.Unload();
                ArchiveFileInfo.FileFormat = null;
                Nodes.Clear();
                OpenFileFormat(TreeView);
            }

            UpdateEditor();
        }

        public override void OnDoubleMouseClick(TreeView treeview) {
            OpenFileFormat(treeview);
        }

        public void OpenFileFormat(TreeView treeview)
        {
            IFileFormat file = ArchiveFileInfo.OpenFile();
            if (file == null) //Format not supported so return
                return;

            if (file.IFileInfo != null)
                file.IFileInfo.ArchiveParent = ArchiveFile;

            if (Utils.HasInterface(file.GetType(), typeof(IEditor<>)))
                OpenControlDialog(file);
            else if (Utils.HasInterface(file.GetType(), typeof(IEditorForm<>)))
                OpenFormDialog(file);
            else if (file is IArchiveFile)
            {
                var FileRoot = new ArchiveRootNodeWrapper(file.FileName, (IArchiveFile)file);
                FileRoot.FillTreeNodes();

                if (file is TreeNode) //It can still be both, so add all it's nodes
                {
                    foreach (TreeNode n in ((TreeNode)file).Nodes)
                        FileRoot.Nodes.Add(n);
                }

                ReplaceNode(this.Parent, treeview, this, FileRoot, RootNode);
            }
            else if (file is TreeNode)
            {
                ReplaceNode(this.Parent, treeview, this, (TreeNode)file, RootNode);
            }

            ArchiveFileInfo.FileFormat = file;
        }

        private static Form activeForm;
        public void OpenFormDialog(IFileFormat fileFormat)
        {
            if (activeForm != null && !activeForm.IsDisposed && !activeForm.Disposing)
            {
                activeForm.Text = (((IFileFormat)fileFormat).FileName);
                System.Reflection.MethodInfo methodFill = fileFormat.GetType().GetMethod("FillEditor");
                methodFill.Invoke(fileFormat, new object[1] { activeForm });
            }
            else
            {
                activeForm = GetEditorForm(fileFormat);
                activeForm.Text = (((IFileFormat)fileFormat).FileName);
                if (activeForm is IArchiveEditor)
                    ((IArchiveEditor)activeForm).UpdateArchiveFile += OnFormSaved;

                activeForm.Show();
            }
        }

        private void OnFormSaved(object sender, EventArgs args)
        {
            if (ArchiveFileInfo.FileFormat == null) return;

            Console.WriteLine("OnFormSaved");
            if (ArchiveFileInfo.FileFormat.CanSave)
            {
                Console.WriteLine("SaveFileFormat");

                ArchiveFileInfo.SaveFileFormat();
                UpdateEditor();
            }
        }

        private void OnFormClosed(object sender, EventArgs args)
        {
            if (ArchiveFileInfo.FileFormat == null) return;

            if (activeForm.DialogResult == DialogResult.OK)
            {
                if (ArchiveFileInfo.FileFormat.CanSave)
                {
                    ArchiveFileInfo.SaveFileFormat();
                    UpdateEditor();
                }
            }
        }

        private void OpenControlDialog(IFileFormat fileFormat)
        {
            UserControl form = GetEditorControl(fileFormat);
            
            form.Text = (((IFileFormat)fileFormat).FileName);

            var parentForm = LibraryGUI.GetActiveForm();

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

        public Form GetEditorForm(IFileFormat fileFormat)
        {
            Type objectType = fileFormat.GetType();
            foreach (var inter in objectType.GetInterfaces())
            {
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IEditorForm<>))
                {
                    System.Reflection.MethodInfo method = objectType.GetMethod("OpenForm");
                    return (Form)method.Invoke(fileFormat, new object[0]);
                }
            }
            return null;
        }

        public UserControl GetEditorControl(IFileFormat fileFormat)
        {
            Type objectType = fileFormat.GetType();
            foreach (var inter in objectType.GetInterfaces())
            {
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IEditor<>))
                {
                    System.Reflection.MethodInfo method = objectType.GetMethod("OpenForm");
                    System.Reflection.MethodInfo methodFill = fileFormat.GetType().GetMethod("FillEditor");

                    var control = (UserControl)method.Invoke(fileFormat, new object[0]);
                    methodFill.Invoke(fileFormat, new object[1] { control });
                    return control;
                }
            }
            return null;
        }

        public override void OnClick(TreeView treeView) {
            UpdateEditor();
        }

        public void UpdateEditor()
        {
            if (ArchiveFileInfo.FileFormat != null)
                Console.WriteLine($"UpdateEditor {ArchiveFileInfo.FileFormat.FileName}");

            ArchiveFilePanel editor = (ArchiveFilePanel)LibraryGUI.GetActiveContent(typeof(ArchiveFilePanel));
            if (editor == null)
            {
                editor = new ArchiveFilePanel();
                editor.Dock = DockStyle.Fill;
                LibraryGUI.LoadEditor(editor);
            }

            editor.LoadFile(ArchiveFileInfo, ArchiveFile);
            editor.UpdateEditor();
        }

        public static void ReplaceNode(TreeNode node, TreeView treeview, ArchiveFileWrapper replaceNode, TreeNode NewNode, ArchiveRootNodeWrapper rootNode)
        {
            if (NewNode == null)
                return;

            var fileInfo = replaceNode.ArchiveFileInfo;

            int index = 0;
            if (node == null)
            {
                index = treeview.Nodes.IndexOf(replaceNode);
                treeview.Nodes.RemoveAt(index);
                treeview.Nodes.Insert(index, NewNode);
            }
            else
            {
                index = node.Nodes.IndexOf(replaceNode);
                node.Nodes.RemoveAt(index);
                node.Nodes.Insert(index, NewNode);
            }

            NewNode.ImageKey = replaceNode.ImageKey;
            NewNode.SelectedImageKey = replaceNode.SelectedImageKey;
            NewNode.Text = replaceNode.Text;
            NewNode.Tag = fileInfo;

            rootNode.FileNodes.RemoveAt(index);
            rootNode.FileNodes.Insert(index, Tuple.Create(fileInfo, NewNode));

            if (NewNode is ISingleTextureIconLoader)
            {
                ObjectEditor editor = LibraryGUI.GetObjectEditor();
                if (editor != null) //The editor isn't always in object editor so check
                {
                    editor.UpdateTextureIcon((ISingleTextureIconLoader)NewNode);
                }
            }
        }

        private void RenameAction(object sender, EventArgs args)
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK) { Text = dialog.textBox1.Text; }
        }
    }
}
