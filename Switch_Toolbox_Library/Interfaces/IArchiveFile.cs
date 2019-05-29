using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library.IO;
using System.Text.RegularExpressions;

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
    public class ArchiveFileInfo
    {
        public virtual STToolStripItem[] Menus { get; set; }

        public FileType FileDataType = FileType.Default;

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

        public string GetSize()
        {
            return STMath.GetFileSize(FileData.Length, 4);
        }

        public IFileFormat FileFormat = null; //Format attached for saving

        protected byte[] _fileData = null;

        public string FileName { get; set; } = string.Empty;  //Full File Name
        public string Name { get; set; } = string.Empty; //File Name (No Path)
        public virtual byte[] FileData
        {
            get
            {
                if (FileFormat != null && FileFormat.CanSave)
                    return FileFormat.Save();
                else
                    return _fileData;
            }
            set { _fileData = value; }
        }

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
    public class ArchiveRootNodeWrapper : ArchiveBase
    {
        public virtual object PropertyDisplay { get; set; }

        public ArchiveRootNodeWrapper(string text, IArchiveFile archiveFile) 
            : base(archiveFile)
        {
            Text = text;

            ReloadMenus();

            PropertyDisplay = new GenericArchiveProperties(archiveFile, text);

            if (!((IFileFormat)archiveFile).CanSave) {
                EnableContextMenu(ContextMenuStrip.Items, "Save", false);
            }
            if (!archiveFile.CanReplaceFiles) {
                EnableContextMenu(ContextMenuStrip.Items, "Repack", false);
            }

            EnableContextMenu(ContextMenuStrip.Items, "Preview Assets", false);
        }

        public void ReloadMenus(bool IsNewInstance = true)
        {
            if (IsNewInstance)
                ContextMenuStrip = new STContextMenuStrip();

            ContextMenuStrip.Items.Add(new STToolStripItem("Save", SaveAction));
            ContextMenuStrip.Items.Add(new STToolStripSeparator());
            ContextMenuStrip.Items.Add(new STToolStripItem("Repack", RepackAction));
            ContextMenuStrip.Items.Add(new STToolStripItem("Extract All", ExtractAllAction));
            ContextMenuStrip.Items.Add(new STToolStripSeparator());
            ContextMenuStrip.Items.Add(new STToolStripItem("Preview Assets", PreviewAction));
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
    }

    //Wrapper for folders
    public class ArchiveFolderNodeWrapper : ArchiveBase
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

            ReloadMenus(archiveFile);
        }

        private void ReloadMenus(IArchiveFile archiveFile)
        {
            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new STToolStripItem("Rename", RenameAction) { Enabled = archiveFile.CanRenameFiles });
            ContextMenuStrip.Items.Add(new STToolStripItem("Extract Folder", ExtractAction));
            ContextMenuStrip.Items.Add(new STToolStripItem("Replace Folder", ReplaceAction) { Enabled = archiveFile.CanReplaceFiles });
            ContextMenuStrip.Items.Add(new STToolStripItem("Delete Folder", DeleteAction) { Enabled = archiveFile.CanDeleteFiles });
            ContextMenuStrip.Items.Add(new STToolStripSeparator());
            ContextMenuStrip.Items.Add(new STToolStripItem("Add File", AddFileAction) { Enabled = archiveFile.CanAddFiles });
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
    public class ArchiveFileWrapper : ArchiveBase
    {
        public ArchiveFileWrapper(string text, IArchiveFile archiveFile) : base(archiveFile)
        {
            Text = text;
            ReloadMenus(archiveFile);
        }

        public static ArchiveFileWrapper FromPath(string FilePath, IArchiveFile archiveFile)
        {
            var wrapper = new ArchiveFileWrapper(Path.GetFileName(FilePath), archiveFile);
            wrapper.ArchiveFileInfo = new ArchiveFileInfo();
            wrapper.ArchiveFileInfo.FileName = FilePath;
            wrapper.ArchiveFileInfo.FileData = File.ReadAllBytes(FilePath);
            return wrapper;
        }

        private void ReloadMenus(IArchiveFile archiveFile)
        {
            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new STToolStripItem("Rename", RenameAction) { Enabled = archiveFile.CanRenameFiles });
            ContextMenuStrip.Items.Add(new STToolStripItem("Extract", ExtractAction));
            ContextMenuStrip.Items.Add(new STToolStripItem("Replace", ReplaceAction) { Enabled = archiveFile.CanReplaceFiles });
            ContextMenuStrip.Items.Add(new STToolStripItem("Delete", DeleteAction) { Enabled = archiveFile.CanDeleteFiles });
        }

        public virtual ArchiveFileInfo ArchiveFileInfo { get; set; }

        private void ExtractAction(object sender, EventArgs args)
        {
            ArchiveFileInfo.Export();
        }

        private void DeleteAction(object sender, EventArgs args)
        {
        }

        private void ReplaceAction(object sender, EventArgs args)
        {
            ArchiveFileInfo.Replace();
        }

        public override void OnDoubleMouseClick(TreeView treeview)
        {
            IFileFormat file = STFileLoader.OpenFileFormat(Text, ArchiveFileInfo.FileData, true);
            if (file == null) //Format not supported so return
                return;

            ArchiveFileInfo.FileFormat = file;

            if (Utils.HasInterface(file.GetType(), typeof(IEditor<>)))
            {
                OpenFormDialog(file);
            }
            else if (file != null)
                ReplaceNode(this.Parent, this, (TreeNode)file);
        }

        private void OpenFormDialog(IFileFormat fileFormat)
        {
            STForm form = GetEditorForm(fileFormat);
            var parentForm = LibraryGUI.Instance.GetActiveForm();

            form.Text = (((IFileFormat)fileFormat).FileName);
            form.FormClosing += (sender, e) => FormClosing(sender, e, fileFormat);
            form.Show(parentForm);
        }

        private void FormClosing(object sender, EventArgs args, IFileFormat fileFormat)
        {
            if (((Form)sender).DialogResult != DialogResult.OK)
                return;

            if (fileFormat.CanSave)
            {
                ArchiveFileInfo.FileData = fileFormat.Save();
                UpdateHexView();
            }
        }

        private STForm GetEditorForm(IFileFormat fileFormat)
        {
            Type objectType = fileFormat.GetType();
            foreach (var inter in objectType.GetInterfaces())
            {
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IEditor<>))
                {
                    System.Reflection.MethodInfo method = objectType.GetMethod("OpenForm");
                    return (STForm)method.Invoke(fileFormat, new object[0]);
                }
            }
            return null;
        }

        public override void OnClick(TreeView treeView) {
            UpdateHexView();
        }

        private void UpdateHexView()
        {
            HexEditor editor = (HexEditor)LibraryGUI.Instance.GetActiveContent(typeof(HexEditor));
            if (editor == null)
            {
                editor = new HexEditor();
                LibraryGUI.Instance.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;
            editor.LoadData(ArchiveFileInfo.FileData);
        }

        public static void ReplaceNode(TreeNode node, TreeNode replaceNode, TreeNode NewNode)
        {
            if (NewNode == null)
                return;

            int index = node.Nodes.IndexOf(replaceNode);
            node.Nodes.RemoveAt(index);
            node.Nodes.Insert(index, NewNode);
        }

        private void RenameAction(object sender, EventArgs args)
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK) { Text = dialog.textBox1.Text; }
        }
    }
}
