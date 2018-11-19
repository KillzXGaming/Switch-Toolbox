using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using System.IO;
using Switch_Toolbox.Library.IO;

namespace FirstPlugin
{
    public class GFPAK : IFileFormat
    {
        public bool CanSave { get; set; } = false;
        public bool FileIsEdited { get; set; } = false;
        public bool FileIsCompressed { get; set; } = false;
        public string[] Description { get; set; } = new string[] { "Graphic Package" };
        public string[] Extension { get; set; } = new string[] { "*.gfpak" };
        public string Magic { get; set; } = "GFLX";
        public CompressionType CompressionType { get; set; } = CompressionType.None;
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public TreeNodeFile EditorRoot { get; set; }
        public bool IsActive { get; set; } = false;
        public bool UseEditMenu { get; set; } = false;
        public int Alignment { get; set; } = 0;
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }
        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public void Load()
        {
            IsActive = true;
            EditorRoot = new TreeNodeFile(FileName, this);

            GFLXPACK gflx = new GFLXPACK();
            gflx.Read(new FileReader(new MemoryStream(Data)), EditorRoot);

        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }


     
    }

    public class GFLXPACK
    {
        public ushort BOM;
        public uint Version;
        public List<FileEntry> files = new List<FileEntry>();
        public List<UInt64> hashes = new List<UInt64>();

        public void Read(FileReader reader, TreeNode root)
        {
            string Signature = reader.ReadString(8, Encoding.ASCII);
            if (Signature != "GFLXPACK")
                throw new Exception($"Invalid signature {Signature}! Expected GFLXPACK.");

            uint unk = reader.ReadUInt32();
            uint padding = reader.ReadUInt32();
            uint FileCount = reader.ReadUInt32();
            uint unk2 = reader.ReadUInt32();
            ulong FileInfoOffset = reader.ReadUInt64();
            ulong hashArrayOffset = reader.ReadUInt64();
            ulong UnkOffset = reader.ReadUInt64();
            ulong Unk2Offset = reader.ReadUInt64();
            ulong Unk3Offset = reader.ReadUInt64();

            reader.Seek((long)hashArrayOffset, SeekOrigin.Begin);
            for (int i = 0; i < FileCount; i++)
            {
                ulong hash = reader.ReadUInt64();
                hashes.Add(hash);
            }

            reader.Seek((long)FileInfoOffset, SeekOrigin.Begin);
            for (int i = 0; i < FileCount; i++)
            {
                FileEntry fileEntry = new FileEntry();
                fileEntry.Read(reader);
                fileEntry.Text = hashes[i].ToString();
                files.Add(fileEntry);

                root.Nodes.Add(fileEntry);
            }
        }

        public class FileEntry : TreeNodeCustom
        {
            public FileEntry()
            {
                ImageKey = "fileBlank";
                SelectedImageKey = "fileBlank";

                ContextMenu = new ContextMenu();
                MenuItem export = new MenuItem("Export");
                ContextMenu.MenuItems.Add(export);
                export.Click += Export;

            }
            public byte[] data;

            public void Read(FileReader reader)
            {
                uint unk = reader.ReadUInt16();
                uint unk2 = reader.ReadUInt16();
                uint unk3 = reader.ReadUInt32();
                uint FileSize = reader.ReadUInt32();
                uint unk4 = reader.ReadUInt32();
                ulong FileOffset = reader.ReadUInt64();

                using (reader.TemporarySeek((long)FileOffset, SeekOrigin.Begin))
                {
                    byte type = reader.ReadByte();
                    data = reader.ReadBytes((int)FileSize - 1);

                    string ext = SARCExt.SARC.GuessFileExtension(data);

                    if (ext == ".bntx")
                    {
                        ImageKey = "bntx";
                        SelectedImageKey = "bntx";
                    }
                    if (ext == ".byaml")
                    {
                        ImageKey = "byaml";
                        SelectedImageKey = "byaml";
                    }
                    if (ext == ".aamp")
                    {
                        ImageKey = "aamp";
                        SelectedImageKey = "aamp";
                    }
                    if (ext == ".lua")
                    {

                    }
                }
            }
            public override void OnClick(TreeView treeview)
            {

            }

            private void Export(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = Text;
                sfd.Filter = "All files(*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, STLibraryCompression.Type_LZ4.Decompress(data));
                }
            }
        }
    }
    public class FileUnk
    {
        public uint Size;
        public uint Type;
        public uint Width;
        public uint Height;
        public uint Ascend;
        public uint LineFeed;
        public uint AlterCharIndex;
        public uint DefaultLeftWidth;
        public uint DefaultGlyphWidth;
        public uint DefaultCharWidth;
        public uint CharEncoding;
        public TGLP tglp;

        public void Read(FileReader reader)
        {
           
        }
    }
}
