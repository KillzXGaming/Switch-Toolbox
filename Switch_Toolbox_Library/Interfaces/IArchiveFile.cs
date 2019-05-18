using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public virtual void Replace() { }
        public virtual void Export() { }

        public string GetSize()
        {
            return STMath.GetFileSize(FileData.Length, 4);
        }

        IFileFormat FileFormat = null; //Format attached for saving

        protected byte[] _fileData = null;

        public string FileName { get; set; } = string.Empty;  //Full File Name
        public string Name { get; set; } = string.Empty; //File Name (No Path)
        public virtual byte[] FileData
        {
            get
            {
                return _fileData;
            }
            set { _fileData = value; }
        }

        public ArchiveFileState State { get; set; } = ArchiveFileState.Empty;
    }

    //Wrapper for the archive file itself
    public class ArchiveRootNodeWrapper : TreeNodeCustom
    {
        IArchiveFile ArchiveFile;

        public ArchiveRootNodeWrapper(string text, IArchiveFile archiveFile)
        {
            Text = text;
            ArchiveFile = archiveFile;

            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new STToolStripItem("Save", SaveAction));
            if (!((IFileFormat)archiveFile).CanSave)
                ContextMenuStrip.Items[0].Enabled = false;
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
    }

    //Wrapper for folders
    public class ArchiveFolderNodeWrapper : TreeNodeCustom
    {
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

        public ArchiveFolderNodeWrapper(string text)
        {
            Text = text;

            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new STToolStripItem("Extract Folder", ExtractAction));
            ContextMenuStrip.Items.Add(new STToolStripItem("Replace Folder", ReplaceAction));
            ContextMenuStrip.Items.Add(new STToolStripItem("Delete Folder", DeleteAction));
        }

        private void ExtractAction(object sender, EventArgs args)
        {
            TreeNode node = this;
            var ParentPath = string.Empty;

            if (node.Parent != null)
            {
                ParentPath = node.Parent.FullPath;
            }


            FolderSelectDialog folderDialog = new FolderSelectDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                STProgressBar progressBar = new STProgressBar();
                progressBar.Task = "Extracing Files...";
                progressBar.Refresh();
                progressBar.Value = 0;
                progressBar.StartPosition = FormStartPosition.CenterScreen;
                progressBar.Show();

                var Collection = TreeViewExtensions.Collect(Nodes);

                int Curfile = 0;
                foreach (TreeNode file in Collection)
                {
                    string FilePath = ((ArchiveNodeWrapper)file).ArchiveFileInfo.FileName;
                    FilePath = FilePath.Replace(ParentPath, string.Empty);

                    Console.WriteLine($"FilePath " + FilePath);
                    var path = Path.Combine(folderDialog.SelectedPath, FilePath);

                    progressBar.Value = (Curfile++ * 100) / Collection.Count();
                    progressBar.Refresh();
                    CreateDirectoryIfExists($"{path}");

                    if (file is ArchiveNodeWrapper)
                    {
                        File.WriteAllBytes($"{path}",
                            ((ArchiveNodeWrapper)file).ArchiveFileInfo.FileData);
                    }
                }

                progressBar.Value = 100;
                progressBar.Refresh();
                progressBar.Close();
            }
        }

        private void ReplaceAction(object sender, EventArgs args)
        {

        }

        private void DeleteAction(object sender, EventArgs args)
        {

        }

        private void CreateDirectoryIfExists(string Dir)
        {
            if (!String.IsNullOrWhiteSpace(Path.GetDirectoryName(Dir)))
            {
                //Make sure no file names use the same name to prevent errors
                if (!File.Exists(Dir))
                {
                    if (!Directory.Exists(Dir))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(Dir));
                    }
                }
            }
        }
    }

    //Wrapper for files
    public class ArchiveNodeWrapper : TreeNodeCustom
    {
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

        public ArchiveNodeWrapper(string text)
        {
            Text = text;

            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new STToolStripItem("Extract", ExtractAction));
            ContextMenuStrip.Items.Add(new STToolStripItem("Replace", ReplaceAction));
            ContextMenuStrip.Items.Add(new STToolStripItem("Delete", DeleteAction));
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
            TreeNode node = STFileLoader.GetNodeFileFormat(Text, ArchiveFileInfo.FileData, true, this);
            if (node != null)
                ReplaceNode(this.Parent, this, node);
        }

        public override void OnClick(TreeView treeView)
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
    }
}
