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
using Switch_Toolbox.Library.Forms;
using System.Drawing;

namespace FirstPlugin
{
    public class BFFNT : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Font;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Cafe Font" };
        public string[] Extension { get; set; } = new string[] { "*.bffnt" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                return reader.CheckSignature(4, "FFNT");
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

        FFNT bffnt;
        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            bffnt = new FFNT();
            bffnt.Read(new FileReader(stream));

            TGLP tglp = bffnt.GetFontSection().tglp;

            var textureFolder = new TreeNode("Textures");
            Nodes.Add(textureFolder);
            if (tglp.SheetDataList.Count > 0)
            {
                var bntx = STFileLoader.OpenFileFormat("Sheet_0", Utils.CombineByteArray(tglp.SheetDataList.ToArray()));
                if (bntx != null) 
                {
                    textureFolder.Nodes.Add((BNTX)bntx);
                }
                else
                {
                    for (int s = 0; s < tglp.SheetDataList.Count; s++)
                    {
                        var surface = new Gx2ImageBlock();
                        surface.Text = $"Sheet_{s}";
                        surface.Load(tglp, s);
                        textureFolder.Nodes.Add(surface);
                    }
                }
            }


            int i = 0;
            foreach (byte[] texture in tglp.SheetDataList)
            {
             //   BNTX file = (BNTX)STFileLoader.OpenFileFormat("Sheet" + i++, texture);
             //  Nodes.Add(file);
            }
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            MemoryStream mem = new MemoryStream();
            bffnt.Write(new FileWriter(mem));
            return mem.ToArray();
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
        public ushort HeaderSize;
        public uint Version;

        public FINF GetFontSection()
        {
            foreach (var block in Blocks)
            {
                if (block.GetType() == typeof(FINF))
                    return (FINF)block;
            }
            return null;
        }

        public List<BFFNT_Block> Blocks = new List<BFFNT_Block>();

        public void Read(FileReader reader)
        {
            reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            string Signature = reader.ReadString(4, Encoding.ASCII);
            if (Signature != "FFNT")
                throw new Exception($"Invalid signature {Signature}! Expected FFNT.");

            BOM = reader.ReadUInt16();
            reader.CheckByteOrderMark(BOM);
            HeaderSize = reader.ReadUInt16();
            Version = reader.ReadUInt32();
            uint FileSize = reader.ReadUInt16();
            ushort BlockCount = reader.ReadUInt16();
            ushort Padding = reader.ReadUInt16();

            reader.Seek(HeaderSize, SeekOrigin.Begin);
            string SignatureCheck = CheckSignature(reader);
            switch (SignatureCheck)
            {
                case "FINF":
                    FINF finf = new FINF();
                    finf.Read(reader);
                    Blocks.Add(finf);
                    break;
                default:
                    throw new NotImplementedException("Unsupported block found! " + SignatureCheck);
            }

            reader.Close();
            reader.Dispose();
        }

        public void Write(FileWriter writer)
        {
            writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            writer.WriteSignature("FFNT");
            writer.Write(BOM);
            writer.CheckByteOrderMark(BOM);
            writer.Write(HeaderSize);
            writer.Write(Version);
            long _ofsFileSize = writer.Position;
            writer.Write(uint.MaxValue);
            writer.Write((ushort)Blocks.Count);
            writer.Write((ushort)0);
        }

        private string CheckSignature(FileReader reader)
        {
            string Signature = reader.ReadString(4, Encoding.ASCII);
            reader.Seek(-4, SeekOrigin.Current);
            return Signature;
        }
    }

    public enum Gx2ImageFormats
    {
        RGBA8_UNORM,
        RGB8_UNORM,
        RGB5A1_UNORM,
        RGB565_UNORM,
        RGBA4_UNORM,
        LA8_UNORM,
        LA4_UNORM,
        A4_UNORM,
        A8_UNORM,
        BC1_UNORM,
        BC2_UNORM,
        BC3_UNORM,
        BC4_UNORM,
        BC5_UNORM,
        RGBA8_SRGB,
        BC1_SRGB,
        BC2_SRGB,
        BC3_SRGB,
    }

    public class Gx2ImageBlock : STGenericTexture
    {
        public TGLP TextureTGLP;

        public int SheetIndex = 0;

        public void Load(TGLP texture, int Index)
        {
            CanReplace = true;

            SheetIndex = Index;
            TextureTGLP = texture;
            Height = TextureTGLP.SheetHeight;
            Width = TextureTGLP.SheetWidth;
            var BFNTFormat = (Gx2ImageFormats)TextureTGLP.Format;
            Format = ConvertToGeneric(BFNTFormat);

            ImageKey = "Texture";
            SelectedImageKey = "Texture";
        }

        public override bool CanEdit { get; set; } = true;
        public override string ExportFilter => FileFilters.GTX;
        public override string ReplaceFilter => FileFilters.GTX;

        public override void Replace(string FileName)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = FileFilters.GTX;
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Bfres.Structs.FTEX ftex = new Bfres.Structs.FTEX();
                ftex.ReplaceTexture(ofd.FileName, 1, SupportedFormats, true, true, false, Format);
                if (ftex.texture != null)
                {
                    TextureTGLP.Format = (ushort)ConvertToGx2(Format);
                    TextureTGLP.SheetHeight = (ushort)ftex.texture.Height;
                    TextureTGLP.SheetWidth = (ushort)ftex.texture.Width;
                    TextureTGLP.SheetDataList[SheetIndex] = ftex.texture.Data;

                    UpdateEditor();
                }
            }
        }

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[] {
                        TEX_FORMAT.R8_UNORM,
                        TEX_FORMAT.BC1_UNORM_SRGB,
                        TEX_FORMAT.BC1_UNORM,
                        TEX_FORMAT.BC2_UNORM,
                        TEX_FORMAT.BC2_UNORM_SRGB,
                        TEX_FORMAT.BC3_UNORM,
                        TEX_FORMAT.BC3_UNORM_SRGB,
                        TEX_FORMAT.BC4_UNORM,
                        TEX_FORMAT.BC5_UNORM,
                        TEX_FORMAT.R8G8_UNORM,
                        TEX_FORMAT.B5G6R5_UNORM,
                        TEX_FORMAT.B5G5R5A1_UNORM,
                        TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                        TEX_FORMAT.R8G8B8A8_UNORM,
                };
            }
        }

        public TEX_FORMAT ConvertToGeneric(Gx2ImageFormats Format)
        {
            switch (Format)
            {
                case Gx2ImageFormats.A8_UNORM: return TEX_FORMAT.R8_UNORM;
                case Gx2ImageFormats.BC1_SRGB: return TEX_FORMAT.BC1_UNORM_SRGB;
                case Gx2ImageFormats.BC1_UNORM: return TEX_FORMAT.BC1_UNORM;
                case Gx2ImageFormats.BC2_UNORM: return TEX_FORMAT.BC2_UNORM;
                case Gx2ImageFormats.BC2_SRGB: return TEX_FORMAT.BC2_UNORM_SRGB;
                case Gx2ImageFormats.BC3_UNORM: return TEX_FORMAT.BC3_UNORM;
                case Gx2ImageFormats.BC3_SRGB: return TEX_FORMAT.BC3_UNORM_SRGB;
                case Gx2ImageFormats.BC4_UNORM: return TEX_FORMAT.BC4_UNORM;
                case Gx2ImageFormats.BC5_UNORM: return TEX_FORMAT.BC5_UNORM;
                case Gx2ImageFormats.LA4_UNORM: return TEX_FORMAT.R4G4_UNORM;
                case Gx2ImageFormats.LA8_UNORM: return TEX_FORMAT.R8G8_UNORM;
                case Gx2ImageFormats.RGB565_UNORM: return TEX_FORMAT.B5G6R5_UNORM;
                case Gx2ImageFormats.RGB5A1_UNORM: return TEX_FORMAT.B5G5R5A1_UNORM;
                case Gx2ImageFormats.RGB8_UNORM: return TEX_FORMAT.R8G8_UNORM;
                case Gx2ImageFormats.RGBA8_SRGB: return TEX_FORMAT.R8G8B8A8_UNORM_SRGB;
                case Gx2ImageFormats.RGBA8_UNORM: return TEX_FORMAT.R8G8B8A8_UNORM;
                default:
                    throw new NotImplementedException("Unsupported format " + Format);
            }
        }

        public Gx2ImageFormats ConvertToGx2(TEX_FORMAT Format)
        {
            switch (Format)
            {
                case TEX_FORMAT.R8_UNORM: return Gx2ImageFormats.A8_UNORM;
                case TEX_FORMAT.BC1_UNORM_SRGB: return Gx2ImageFormats.BC1_SRGB;
                case TEX_FORMAT.BC1_UNORM: return Gx2ImageFormats.BC1_UNORM;
                case TEX_FORMAT.BC2_UNORM_SRGB: return Gx2ImageFormats.BC2_SRGB;
                case TEX_FORMAT.BC2_UNORM: return Gx2ImageFormats.BC2_UNORM;
                case TEX_FORMAT.BC3_UNORM_SRGB: return Gx2ImageFormats.BC3_SRGB;
                case TEX_FORMAT.BC3_UNORM: return Gx2ImageFormats.BC3_UNORM;
                case TEX_FORMAT.BC4_UNORM: return Gx2ImageFormats.BC4_UNORM;
                case TEX_FORMAT.BC5_UNORM: return Gx2ImageFormats.BC5_UNORM;
                case TEX_FORMAT.R4G4_UNORM: return Gx2ImageFormats.LA4_UNORM;
                case TEX_FORMAT.R8G8_UNORM: return Gx2ImageFormats.RGB8_UNORM;
                case TEX_FORMAT.B5G6R5_UNORM: return Gx2ImageFormats.RGB565_UNORM;
                case TEX_FORMAT.B5G5R5A1_UNORM: return Gx2ImageFormats.RGB5A1_UNORM;
                case TEX_FORMAT.R8G8B8A8_UNORM_SRGB: return Gx2ImageFormats.RGBA8_SRGB;
                case TEX_FORMAT.R8G8B8A8_UNORM: return Gx2ImageFormats.RGBA8_UNORM;
                default:
                    throw new NotImplementedException("Unsupported format " + Format);
            }
        }

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            if (bitmap == null)
                return; //Image is likely disposed and not needed to be applied

            uint Gx2Format = (uint)Bfres.Structs.FTEX.ConvertToGx2Format(Format);
            Width = (uint)bitmap.Width;
            Height = (uint)bitmap.Height;

            MipCount = 1;
            uint[] MipOffsets = new uint[MipCount];

            try
            {
                //Create image block from bitmap first
                var data = GenerateMipsAndCompress(bitmap, Format);

                //Swizzle and create surface
                var surface = GX2.CreateGx2Texture(data, Text,
                 (uint)2,
                 (uint)0,
                 (uint)Width,
                 (uint)Height,
                 (uint)1,
                 (uint)Gx2Format,
                 (uint)0,
                 (uint)1,
                 (uint)MipCount
                 );

                TextureTGLP.Format = (ushort)ConvertToGx2(Format);
                TextureTGLP.SheetHeight = (ushort)surface.height;
                TextureTGLP.SheetWidth = (ushort)surface.width;
                TextureTGLP.SheetDataList[SheetIndex] = surface.data;

                IsEdited = true;
                UpdateEditor();
            }
            catch (Exception ex)
            {
                STErrorDialog.Show("Failed to swizzle and compress image " + Text, "Error", ex.ToString());
            }
        }

        private const uint SwizzleBase = 0x000D0000;

        private uint swizzle;
        private uint Swizzle
        {
            get
            {
                swizzle = SwizzleBase;
                swizzle |= (uint)(SheetIndex * 2) << 8;
                return swizzle;
            }
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            uint bpp = GetBytesPerPixel(Format);

            GX2.GX2Surface surf = new GX2.GX2Surface();
            surf.bpp = bpp;
            surf.height = Height;
            surf.width = Width;
            surf.aa = (uint)GX2.GX2AAMode.GX2_AA_MODE_1X;
            surf.alignment = 0;
            surf.depth = 1;
            surf.dim = (uint)GX2.GX2SurfaceDimension.DIM_2D;
            surf.format = (uint)Bfres.Structs.FTEX.ConvertToGx2Format(Format);
            surf.use = (uint)GX2.GX2SurfaceUse.USE_COLOR_BUFFER;
            surf.pitch = 0;
            surf.data = TextureTGLP.SheetDataList[SheetIndex];
            surf.numMips = 1;
            surf.mipOffset = new uint[0];
            surf.mipData = null;
            surf.tileMode = (uint)GX2.GX2TileMode.MODE_2D_TILED_THIN1;
            surf.swizzle = Swizzle;
            surf.numArray = 1;

            var surfaces = GX2.Decode(surf);

            return surfaces[ArrayLevel][MipLevel];
        }

        public override void OnClick(TreeView treeview)
        {
            UpdateEditor();
        }

        private void UpdateEditor()
        {
            ImageEditorBase editor = (ImageEditorBase)LibraryGUI.Instance.GetActiveContent(typeof(ImageEditorBase));
            if (editor == null)
            {
                editor = new ImageEditorBase();
                editor.Dock = DockStyle.Fill;
                LibraryGUI.Instance.LoadEditor(editor);
            }

            Properties prop = new Properties();
            prop.Width = Width;
            prop.Height = Height;
            prop.Depth = Depth;
            prop.MipCount = MipCount;
            prop.ArrayCount = ArrayCount;
            prop.ImageSize = (uint)TextureTGLP.SheetDataList[SheetIndex].Length;
            prop.Format = Format;
            prop.Swizzle = Swizzle;


            editor.Text = Text;
            editor.LoadProperties(prop);
            editor.LoadImage(this);
        }
    }

    public class BFFNT_Block
    {

    }

    public class FINF : BFFNT_Block
    {
        public uint Size;
        public byte Type;
        public byte Width;
        public byte Height;
        public byte Ascend;
        public ushort LineFeed;
        public ushort AlterCharIndex;
        public byte DefaultLeftWidth;
        public byte DefaultGlyphWidth;
        public byte DefaultCharWidth;
        public byte CharEncoding;
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

        public void Write(FileWriter writer)
        {
            writer.WriteSignature("FINF");
            writer.Write(uint.MaxValue);
            writer.Write(Type);
            writer.Write(Height);
            writer.Write(Width);
            writer.Write(Ascend);
            writer.Write(LineFeed);
            writer.Write(AlterCharIndex);
            writer.Write(DefaultLeftWidth);
            writer.Write(DefaultGlyphWidth);
            writer.Write(DefaultCharWidth);
            writer.Write(CharEncoding);

            long _ofsTGLP = writer.Position;
            writer.Write(uint.MaxValue);
            long _ofsCWDH = writer.Position;
            writer.Write(uint.MaxValue);
            long _ofsCMAP = writer.Position;
            writer.Write(uint.MaxValue);
        }
    }
    public class TGLP
    {
        public uint Size;
        public byte CellWidth;
        public byte CellHeight;
        public byte MaxCharWidth;
        public uint SheetSize;
        public ushort BaseLinePos;
        public ushort Format;
        public ushort ColumnCount;
        public ushort RowCount;
        public ushort SheetWidth;
        public ushort SheetHeight;
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
                    SheetDataList.Add(reader.ReadBytes((int)SheetSize));
                }
            }
        }

        public void Write(FileWriter writer)
        {
            writer.WriteSignature("TGLP");
            long _ofsSectionSize = writer.Position;
            writer.Write(uint.MaxValue);
            writer.Write(CellWidth);
            writer.Write(CellHeight);
            writer.Write((byte)SheetDataList.Count);
            writer.Write(MaxCharWidth);
            writer.Write(SheetDataList[0].Length);
            writer.Write(BaseLinePos);
            writer.Write(Format);
            writer.Write(ColumnCount);
            writer.Write(RowCount);
            writer.Write(SheetWidth);
            writer.Write(SheetHeight);
            long _ofsSheetBlocks = writer.Position;
            writer.Write(uint.MaxValue);
            writer.Align(8192);

            long DataPosition = writer.Position;
            using (writer.TemporarySeek(_ofsSheetBlocks, SeekOrigin.Begin))
            {
                writer.Write((uint)DataPosition);
            }

            for (int i = 0; i < SheetDataList.Count; i++)
            {
                writer.Write(SheetDataList[i]);
            }

            long SectionEndPosition = writer.Position;

            //End of section. Set the size

            using (writer.TemporarySeek(_ofsSectionSize, SeekOrigin.Begin))
            {
                writer.Write((uint)(SectionEndPosition - _ofsSectionSize - 4));
            }
        }
    }
}
