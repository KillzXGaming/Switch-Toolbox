using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library.IO;

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

    public class ArchiveNodeWrapper : TreeNodeCustom
    {
        public ArchiveNodeWrapper(string text)
        {
            Text = text;

            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new STToolStripItem("Extract", ExtractAction));
            ContextMenuStrip.Items.Add(new STToolStripItem("Replace", ReplaceAction));
        }

        public virtual ArchiveFileInfo ArchiveFileInfo { get; set; }

        private void ExtractAction(object sender, EventArgs args)
        {
            ArchiveFileInfo.Export();
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
