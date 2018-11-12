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
    public class BFFNT : IFileFormat
    {
        public bool CanSave { get; set; } = false;
        public bool FileIsEdited { get; set; } = false;
        public bool FileIsCompressed { get; set; } = false;
        public string[] Description { get; set; } = new string[] { "Cafe Font" };
        public string[] Extension { get; set; } = new string[] { "*.bffnt" };
        public string Magic { get; set; } = "FFNT";
        public CompressionType CompressionType { get; set; } = CompressionType.None;
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public TreeNode EditorRoot { get; set; }
        public bool IsActive { get; set; } = false;
        public bool UseEditMenu { get; set; } = false;
        public int Alignment { get; set; } = 0;
        public string FilePath { get; set; }
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
            FFNT bffnt = new FFNT();
            bffnt.Read(new FileReader(new MemoryStream(Data)));

            TGLP tglp = bffnt.finf.tglp;

            EditorRoot = new TreeNode(FileName);

            int i = 0;
            foreach (byte[] texture in tglp.SheetDataList)
            {
                SheetEntry sheet = new SheetEntry();
                sheet.data = texture;
                sheet.Text = "Sheet" + i++;

                EditorRoot.Nodes.Add(sheet);
            }
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }


        public class SheetEntry : TreeNodeCustom
        {
            public SheetEntry()
            {
                ImageKey = "fileBlank";
                SelectedImageKey = "fileBlank";

                ContextMenu = new ContextMenu();
                MenuItem export = new MenuItem("Export");
                ContextMenu.MenuItems.Add(export);
                export.Click += Export;

            }
            public byte[] data;

            public override void OnClick(TreeView treeview)
            {

            }

            private void Export(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = Text;
                sfd.DefaultExt = "bntx";
                sfd.Filter = "Supported Formats|*.bntx;|" +
                             "All files(*.*)|*.*";


                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, data);
                }
            }
        }
    }

    public class FFNT
    {
        public ushort BOM;
        public uint Version;
        public FINF finf;

        public void Read(FileReader reader)
        {
            string Signature = reader.ReadString(4, Encoding.ASCII);
            if (Signature != "FFNT")
                throw new Exception($"Invalid signature {Signature}! Expected FFNT.");

            char[] Magic = reader.ReadChars(4);
            BOM = reader.ReadUInt16();
            Version = reader.ReadUInt32();
            uint FileSize = reader.ReadUInt16();
            uint BlockCount = reader.ReadUInt16();
            uint unk = reader.ReadUInt16();

            finf = new FINF();
            finf.Read(reader);

        }
    }
    public class FINF
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
            string Signature = reader.ReadString(4, Encoding.ASCII);
            if (Signature != "FINF")
                throw new Exception($"Invalid signature {Signature}! Expected FINF.");
            Size = reader.ReadUInt32();
            Type = reader.ReadByte();
            Height = reader.ReadByte();
            Width = reader.ReadByte();
            Ascend = reader.ReadByte();
            LineFeed = reader.ReadUInt16();
            AlterCharIndex = reader.ReadUInt16();
            DefaultLeftWidth = reader.ReadByte();
            DefaultGlyphWidth = reader.ReadByte();
            DefaultCharWidth = reader.ReadByte();
            CharEncoding = reader.ReadByte();
            uint tglpOffset = reader.ReadUInt32();
            uint cwdhOffset = reader.ReadUInt32();
            uint cmapOffset = reader.ReadUInt32();

            tglp = new TGLP();
            using (reader.TemporarySeek(tglpOffset - 8, SeekOrigin.Begin))
            {
                tglp.Read(reader);
            }
        }
    }
    public class TGLP
    {
        public uint Size;
        public uint CellWidth;
        public uint CellHeight;
        public uint MaxCharWidth;
        public uint SheetSize;
        public uint BaseLinePos;
        public uint Format;
        public uint ColumnCount;
        public uint RowCount;
        public uint SheetWidth;
        public uint SheetHeight;
        public List<byte[]> SheetDataList = new List<byte[]>();

        public void Read(FileReader reader)
        {
            string Signature = reader.ReadString(4, Encoding.ASCII);
            if (Signature != "TGLP")
                throw new Exception($"Invalid signature {Signature}! Expected TGLP.");
            Size = reader.ReadUInt32();
            CellWidth = reader.ReadByte();
            CellHeight = reader.ReadByte();
            byte SheetCount = reader.ReadByte();
            MaxCharWidth = reader.ReadByte();
            SheetSize = reader.ReadUInt32();
            BaseLinePos = reader.ReadUInt16();
            Format = reader.ReadUInt16();
            ColumnCount = reader.ReadUInt16();
            RowCount = reader.ReadUInt16();
            SheetWidth = reader.ReadUInt16();
            SheetHeight = reader.ReadUInt16();
            uint sheetOffset = reader.ReadUInt32();

            using (reader.TemporarySeek(sheetOffset, SeekOrigin.Begin))
            {
                for (int i = 0; i < SheetCount; i++)
                {
                }
                SheetDataList.Add(reader.ReadBytes((int)SheetSize * SheetCount));
            }
        }
    }
}
