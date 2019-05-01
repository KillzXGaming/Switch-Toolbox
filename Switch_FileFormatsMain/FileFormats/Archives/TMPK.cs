using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class TMPK : TreeNodeFile, IArchiveFile
    {
        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "TMPK" };
        public string[] Extension { get; set; } = new string[] { "*.pack" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "TMPK");
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public IEnumerable<ArchiveFileInfo> Files { get; }

        public bool CanAddFiles { get; set; } = true;
        public bool CanRenameFiles { get; set; } = true;
        public bool CanReplaceFiles { get; set; } = true;
        public bool CanDeleteFiles { get; set; } = true;

        public List<FileInfo> files = new List<FileInfo>();
        public Dictionary<long, byte[]> SavedDataEntries = new Dictionary<long, byte[]>();
        public Dictionary<long, string> SavedStringEntries = new Dictionary<long, string>();

        public uint Alignment;
        public static readonly uint DefaultAlignment = 4;

        public void Load(System.IO.Stream stream)
        {
            TPFileSizeTable table = new TPFileSizeTable();
            table.Read(new FileReader($"{Runtime.TpGamePath}/FileSizeList.txt"));
            table.Write(new FileWriter($"{Runtime.TpGamePath}/FileSizeListTEST.txt"));

            Text = FileName;

            CanSave = true;

            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                reader.ReadSignature(4, "TMPK");
                uint FileCount = reader.ReadUInt32();
                Alignment = reader.ReadUInt32();
                uint padding = reader.ReadUInt32();
                for (int i = 0; i < FileCount; i++)
                {
                    var info = new FileInfo(reader);
                    Nodes.Add(info);
                    files.Add(info);
                }
            }

            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Save", null, Save, Keys.Control | Keys.E));
        }

        private void Save(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "pack";
            sfd.Filter = "Supported Formats|*.pack;";
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        private void SaveFile(FileWriter writer)
        {
            writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            writer.WriteSignature("TMPK");
            writer.Write(files.Count);
            writer.Write(Alignment);
            writer.Write(0);
            for (int i = 0; i < files.Count; i++)
            {
                files[i]._posHeader = writer.Position;
                writer.Write(uint.MaxValue);
                writer.Write(uint.MaxValue);
                writer.Write(files[i].Data.Length); //Padding
                writer.Write(0); //Padding
            }
            for (int i = 0; i < files.Count; i++)
            {
                writer.WriteUint32Offset(files[i]._posHeader);
                writer.Write(files[i].Text, Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);
            }
            for (int i = 0; i < files.Count; i++)
            {
                SetAlignment(writer, files[i].Text);
                writer.WriteUint32Offset(files[i]._posHeader + 4);
                writer.Write(files[i].Data);
            }
        }

        private void SetAlignment(FileWriter writer, string FileName)
        {
            if (FileName.EndsWith(".gmx"))
                writer.Align(0x40);
            else if (FileName.EndsWith(".gtx"))
                writer.Align(0x2000);
            else
                writer.Write(DefaultAlignment);
        }

        public class FileInfo : TreeNodeCustom
        {
            internal long _posHeader;

            public byte[] Data;

            public FileInfo()
            {
                ContextMenu = new ContextMenu();
                MenuItem export = new MenuItem("Export Raw Data");
                ContextMenu.MenuItems.Add(export);
                export.Click += Export;
            }

            private void Export(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = Text;
                sfd.DefaultExt = Path.GetExtension(Text);
                sfd.Filter = "Raw Data (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, Data);
                }
            }

            public override void OnDoubleMouseClick(TreeView treeView)
            {
                if (Data.Length <= 0)
                    return;

                IFileFormat file = STFileLoader.OpenFileFormat(Text, Data, false, true, this);
                if (file == null) //File returns null if no supported format is found
                    return;

                ReplaceNode(this.Parent, this, (TreeNode)file);
            }

            public static void ReplaceNode(TreeNode node, TreeNode replaceNode, TreeNode NewNode)
            {
                if (NewNode == null)
                    return;

                int index = node.Nodes.IndexOf(replaceNode);
                node.Nodes.RemoveAt(index);
                node.Nodes.Insert(index, NewNode);
            }

            public override void OnClick(TreeView treeview)
            {
                HexEditor editor = (HexEditor)LibraryGUI.Instance.GetActiveContent(typeof(HexEditor));
                if (editor == null)
                {
                    editor = new HexEditor();
                    LibraryGUI.Instance.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.LoadData(Data);
            }

            public FileInfo(FileReader reader)
            {
                long pos = reader.Position;

                uint NameOffset = reader.ReadUInt32();
                uint FileOffset = reader.ReadUInt32();
                uint FileSize = reader.ReadUInt32();
                uint padding = reader.ReadUInt32();

                reader.Seek(NameOffset, System.IO.SeekOrigin.Begin);
                Text = reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);

                reader.Seek(FileOffset, System.IO.SeekOrigin.Begin);
                Data = reader.ReadBytes((int)FileSize);

                reader.Seek(pos + 16, System.IO.SeekOrigin.Begin);

                ContextMenu = new ContextMenu();
                MenuItem export = new MenuItem("Export Raw Data");
                ContextMenu.MenuItems.Add(export);
                export.Click += Export;
            }
        }    

        public void Unload()
        {

        }
        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
            SaveFile(new FileWriter(mem));
            return mem.ToArray();
        }


        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }
    }
}
