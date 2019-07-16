using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class APAKFileInfo : ArchiveFileInfo
    {

    }

    public class APAK : TreeNodeFile
    {
        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "APAK" };
        public string[] Extension { get; set; } = new string[] { "*.apak" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "APAK");
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

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            CanDelete = true;

            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                reader.ReadSignature(4, "APAK");
                uint Version = reader.ReadUInt32();
                uint FileCount = reader.ReadUInt32();
                uint unk = reader.ReadUInt32();
                uint unk2 = reader.ReadUInt32();
                uint DataTotalSize = reader.ReadUInt32();
                uint unk3 = reader.ReadUInt32();

                for (int i = 0; i < FileCount; i++)
                {
                    var info = new FileInfo(reader);

                    APAKFileInfo archive = new APAKFileInfo();
                    archive.FileData = info.Data;
                    archive.Name = info.Text;

                    Nodes.Add(info);
                }
            }
        }

        public class FileInfo : TreeNodeCustom
        {
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

            public override void OnClick(TreeView treeview)
            {
                HexEditor editor = (HexEditor)LibraryGUI.GetActiveContent(typeof(HexEditor));
                if (editor == null)
                {
                    editor = new HexEditor();
                    LibraryGUI.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.LoadData(Data);
            }

            public FileInfo(FileReader reader)
            {
                long pos = reader.Position;

                uint dataOffset = reader.ReadUInt32();
                uint uncompressedSize = reader.ReadUInt32();
                uint compressedSize = reader.ReadUInt32();
                uint unk = reader.ReadUInt32();
                uint unk2 = reader.ReadUInt32();
                uint unk3 = reader.ReadUInt32();
                uint unk4 = reader.ReadUInt32();

                uint FileOffset = reader.ReadUInt32();
                uint FileSize = reader.ReadUInt32();
                uint padding = reader.ReadUInt32();

            //    reader.Seek(NameOffset, System.IO.SeekOrigin.Begin);
                Text = reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);

                reader.Seek(dataOffset, System.IO.SeekOrigin.Begin);
                Data = reader.ReadBytes((int)compressedSize);

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
            return null;
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
