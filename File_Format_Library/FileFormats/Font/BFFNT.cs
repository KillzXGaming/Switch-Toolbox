using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using System.IO;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Drawing;
using FirstPlugin.Forms;

namespace FirstPlugin
{
    public class BFFNT : IFileFormat, IEditor<BffntEditor>, IConvertableTextFormat
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

        public FFNT bffnt;

        public BffntEditor OpenForm()
        {
            BffntEditor form = new BffntEditor();
            form.Text = "BFFNT Editor";
            form.Dock = DockStyle.Fill;
            form.LoadFontFile(this);
            return form;
        }

        public void FillEditor(UserControl control)
        {
            ((BffntEditor)control).LoadFontFile(this);
        }


        #region Text Converter Interface
        public TextFileType TextFileType => TextFileType.Xml;
        public bool CanConvertBack => false;

        public string ConvertToString()
        {
            return BffntCharSet2Xlor.ToXlor(this);
        }

        public void ConvertFromString(string text)
        {
        }

        #endregion

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            bffnt = new FFNT();
            bffnt.Read(new FileReader(stream));

            TGLP tglp = bffnt.FontSection.TextureGlyph;

            if (tglp.SheetDataList.Count > 0)
            {
                var bntx = STFileLoader.OpenFileFormat("Sheet_0", Utils.CombineByteArray(tglp.SheetDataList.ToArray()));
                if (bntx != null)
                {
                    tglp.BinaryTextureFile = (BNTX)bntx;
                }
                else
                {
                    for (int s = 0; s < tglp.SheetDataList.Count; s++)
                    {
                        var surface = new Gx2ImageBlock();
                        surface.Text = $"Sheet_{s}";
                        surface.Load(tglp, s);
                        tglp.Gx2Textures.Add(surface);
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
        public uint Version { get; set; }

        public FINF FontSection { get; set; }
        public FontKerningTable KerningTable { get; set; }

        public List<BFFNT_Block> Blocks = new List<BFFNT_Block>();

        public PlatformType Platform { get; set; } = PlatformType.Cafe;

        public enum PlatformType
        {
            Cafe,
            NX,
            Ctr
        }

        public void Read(FileReader reader)
        {
            reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            string Signature = reader.ReadString(4, Encoding.ASCII);
            if (Signature != "FFNT" && Signature != "CFNT")
                throw new Exception($"Invalid signature {Signature}! Expected FFNT or CFNT.");

            BOM = reader.ReadUInt16();
            reader.CheckByteOrderMark(BOM);
            HeaderSize = reader.ReadUInt16();
            Version = reader.ReadUInt32();
            uint FileSize = reader.ReadUInt16();
            ushort BlockCount = reader.ReadUInt16();
            ushort Padding = reader.ReadUInt16();

            if (reader.ByteOrder == Syroot.BinaryData.ByteOrder.LittleEndian)
            {
                if (Version > 0x3000000 || Version > 0x00000103)
                    Platform = PlatformType.NX;
                else
                    Platform = PlatformType.Ctr;
            }
            else
                Platform = PlatformType.Cafe;

            if (Signature == "CFNT")
                Platform = PlatformType.Ctr;

            reader.Seek(HeaderSize, SeekOrigin.Begin);
            FontSection = new FINF();
            FontSection.Read(reader, this);
            Blocks.Add(FontSection);

            //Check for any unread blocks
            reader.Seek(HeaderSize, SeekOrigin.Begin);
            while (!reader.EndOfStream)
            {
                long BlockStart = reader.Position;

                string BlockSignature = reader.ReadString(4, Encoding.ASCII);
                uint BlockSize = reader.ReadUInt32();

                switch (BlockSignature)
                {
                    case "FFNT":
                    case "FFNA":
                    case "FCPX":
                    case "CWDH":
                    case "CGLP":
                    case "CMAP":
                    case "TGLP":
                    case "FINF":
                        break;
                    case "KRNG":
                        KerningTable = new FontKerningTable();
                        KerningTable.Read(reader, this);
                        break;
                    case "GLGR":
                    case "HTGL":
                        break;
                    default:
                        throw new Exception("Unknown block found! " + BlockSignature);
                }

                reader.SeekBegin(BlockStart + BlockSize);
            }




            reader.Close();
            reader.Dispose();
        }

        internal int BlockCounter = 0;
        public void Write(FileWriter writer)
        {
            writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            BlockCounter = 1;

            writer.WriteSignature("FFNT");
            writer.Write(BOM);
            writer.CheckByteOrderMark(BOM);
            writer.Write(HeaderSize);
            writer.Write(Version);
            long _ofsFileSize = writer.Position;
            writer.Write(uint.MaxValue);
            long _ofsBlockNum = writer.Position;
            writer.Write((ushort)0); //BlockCount
            writer.Write((ushort)0);

            writer.SeekBegin(HeaderSize);
            FontSection.Write(writer, this);

            //Save Block Count
            using (writer.TemporarySeek(_ofsBlockNum, SeekOrigin.Begin))
            {
                writer.Write((ushort)(BlockCounter + 1));
            }

            //Save File size
            using (writer.TemporarySeek(_ofsFileSize, SeekOrigin.Begin))
            {
                writer.Write((uint)(writer.BaseStream.Length));
            }
        }

        private string CheckSignature(FileReader reader)
        {
            string Signature = reader.ReadString(4, Encoding.ASCII);
            reader.Seek(-4, SeekOrigin.Current);
            return Signature;
        }
    }

    //Kerning Table
    //https://github.com/dnasdw/3dsfont/blob/4ead538d225d5d05929dce9d736bec91a6158052/src/bffnt/ResourceFormat.h
    public class FontKerningTable
    {
        private byte[] Data;

        public KerningFirstTable FirstTable { get; set; }

        public void Read(FileReader reader, FFNT Header)
        {
            if (Header.Platform == FFNT.PlatformType.NX)
            {
                ushort FirstWordCount = reader.ReadUInt16();
                ushort padding = reader.ReadUInt16();

                FirstTable = new KerningFirstTable();
                FirstTable.Read(reader, Header);
            }
        }
    }

    public class KerningFirstTable
    {
        public uint FirstWordCount { get; set; }
        public uint Offset { get; set; }

        public void Read(FileReader reader, FFNT Header)
        {
            if (Header.Platform == FFNT.PlatformType.NX)
            {
                uint FirstWordCount = reader.ReadUInt16();
                uint Offset = reader.ReadUInt16();
            }
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
            Bfres.Structs.FTEX ftex = new Bfres.Structs.FTEX();
            ftex.ReplaceTexture(FileName, Format, 1, SupportedFormats, true, true, false);
            if (ftex.texture != null)
            {
                TextureTGLP.Format = (ushort)ConvertToGx2(ftex.Format);
                TextureTGLP.SheetHeight = (ushort)ftex.texture.Height;
                TextureTGLP.SheetWidth = (ushort)ftex.texture.Width;
                TextureTGLP.SheetDataList[SheetIndex] = ftex.texture.Data;
                Format = ftex.Format;
                Width = ftex.texture.Width;
                Height = ftex.texture.Height;

                UpdateEditor();
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
                var data = GenerateMipsAndCompress(bitmap, MipCount, Format);

                //Swizzle and create surface
                var surface = GX2.CreateGx2Texture(data, Text,
                 (uint)4,
                 (uint)0,
                 (uint)Width,
                 (uint)Height,
                 (uint)1,
                 (uint)Gx2Format,
                 (uint)SwizzlePattern,
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

        private const uint SwizzleBase = 0x00000000;

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

        private uint SwizzlePattern
        {
            get
            {
                return (uint)(SheetIndex * 2);
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

            return GX2.Decode(surf, ArrayLevel, MipLevel);
        }

        public override void OnClick(TreeView treeview)
        {
            UpdateEditor();
        }

        private void UpdateEditor()
        {
            ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
            if (editor == null)
            {
                editor = new ImageEditorBase();
                editor.Dock = DockStyle.Fill;
                LibraryGUI.LoadEditor(editor);
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
        public Dictionary<char, int> CodeMapDictionary = new Dictionary<char, int>();

        public uint Size;
        public FontType Type { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public byte Ascent { get; set; }
        public ushort LineFeed { get; set; }
        public ushort AlterCharIndex { get; set; }
        public byte DefaultLeftWidth { get; set; }
        public byte DefaultGlyphWidth { get; set; }
        public byte DefaultCharWidth { get; set; }
        public CharacterCode CharEncoding { get; set; }
        public TGLP TextureGlyph;
        public CMAP CodeMap;
        public CWDH CharacterWidth;

        public List<CWDH> CharacterWidths { get; set; }
        public List<CMAP> CodeMaps { get; set; }

        public enum FontType : byte
        {
            Glyph = 1,
            Texture = 2,
            PackedTexture = 3,
        }

        public enum CharacterCode : byte
        {
            Unicode = 1,
            ShiftJIS = 2,
            CP1252 = 3,
        }

        public void Read(FileReader reader, FFNT Header)
        {
            CharacterWidths = new List<CWDH>();
            CodeMaps = new List<CMAP>();

            string Signature = reader.ReadString(4, Encoding.ASCII);
            if (Signature != "FINF")
                throw new Exception($"Invalid signature {Signature}! Expected FINF.");
            Size = reader.ReadUInt32();

            if (Header.Platform == FFNT.PlatformType.Ctr)
            {
                Type = reader.ReadEnum<FontType>(true);
                LineFeed = reader.ReadUInt16();
                AlterCharIndex = reader.ReadUInt16();
                DefaultLeftWidth = reader.ReadByte();
                DefaultGlyphWidth = reader.ReadByte();
                DefaultCharWidth = reader.ReadByte();
                CharEncoding = reader.ReadEnum<CharacterCode>(true);
                uint tglpOffset = reader.ReadUInt32();
                uint cwdhOffset = reader.ReadUInt32();
                uint cmapOffset = reader.ReadUInt32();

                Height = reader.ReadByte();
                Width = reader.ReadByte();
                Ascent = reader.ReadByte();
                reader.ReadByte(); //Padding


                //Add counter for TGLP
                //Note the other counters are inside sections due to recusive setup
                Header.BlockCounter += 1;

                TextureGlyph = new TGLP();
                using (reader.TemporarySeek(tglpOffset - 8, SeekOrigin.Begin))
                    TextureGlyph.Read(reader);

                CharacterWidth = new CWDH();
                CharacterWidths.Add(CharacterWidth);
                using (reader.TemporarySeek(cwdhOffset - 8, SeekOrigin.Begin))
                    CharacterWidth.Read(reader, Header, CharacterWidths);

                CodeMap = new CMAP();
                CodeMaps.Add(CodeMap);
                using (reader.TemporarySeek(cmapOffset - 8, SeekOrigin.Begin))
                    CodeMap.Read(reader, Header, CodeMaps);

            }
            else
            {

                Type = reader.ReadEnum<FontType>(true);
                Height = reader.ReadByte();
                Width = reader.ReadByte();
                Ascent = reader.ReadByte();
                LineFeed = reader.ReadUInt16();
                AlterCharIndex = reader.ReadUInt16();
                DefaultLeftWidth = reader.ReadByte();
                DefaultGlyphWidth = reader.ReadByte();
                DefaultCharWidth = reader.ReadByte();
                CharEncoding = reader.ReadEnum<CharacterCode>(true);
                uint tglpOffset = reader.ReadUInt32();
                uint cwdhOffset = reader.ReadUInt32();
                uint cmapOffset = reader.ReadUInt32();

                //Add counter for TGLP
                //Note the other counters are inside sections due to recusive setup
                Header.BlockCounter += 1;

                TextureGlyph = new TGLP();
                using (reader.TemporarySeek(tglpOffset - 8, SeekOrigin.Begin))
                    TextureGlyph.Read(reader);

                CharacterWidth = new CWDH();
                CharacterWidths.Add(CharacterWidth);
                using (reader.TemporarySeek(cwdhOffset - 8, SeekOrigin.Begin))
                    CharacterWidth.Read(reader, Header, CharacterWidths);

                CodeMap = new CMAP();
                CodeMaps.Add(CodeMap);
                using (reader.TemporarySeek(cmapOffset - 8, SeekOrigin.Begin))
                    CodeMap.Read(reader, Header, CodeMaps);

            }
        }

        public void Write(FileWriter writer, FFNT header)
        {
            long pos = writer.Position;

            writer.WriteSignature("FINF");
            writer.Write(uint.MaxValue);
            writer.Write(Type, true);
            writer.Write(Height);
            writer.Write(Width);
            writer.Write(Ascent);
            writer.Write(LineFeed);
            writer.Write(AlterCharIndex);
            writer.Write(DefaultLeftWidth);
            writer.Write(DefaultGlyphWidth);
            writer.Write(DefaultCharWidth);
            writer.Write(CharEncoding, true);

            long _ofsTGLP = writer.Position;
            writer.Write(uint.MaxValue);
            long _ofsCWDH = writer.Position;
            writer.Write(uint.MaxValue);
            long _ofsCMAP = writer.Position;
            writer.Write(uint.MaxValue);


            //Save section size
            long endPos = writer.Position;
            using (writer.TemporarySeek(pos + 4, SeekOrigin.Begin))
            {
                writer.Write((uint)(endPos - pos));
            }

            //Save Texture Glyph
            writer.WriteUint32Offset(_ofsTGLP, -8);
            TextureGlyph.Write(writer, header);

            //Save Character Widths
            writer.WriteUint32Offset(_ofsCWDH, -8);
            CharacterWidth.Write(writer, header);

            //Save Code Maps
            writer.WriteUint32Offset(_ofsCMAP, -8);
            CodeMap.Write(writer, header);
        }

        public CWDH GetCharacterWidth(int index)
        {
            if (index == -1)
                return null;

            for (int i = 0; i < CharacterWidths.Count; i++)
            {
                if (CharacterWidths[i].StartIndex <= index && CharacterWidths[i].EndIndex >= index)
                {
                    int CharaIndex = index - CharacterWidths[i].StartIndex;
                    return CharacterWidths[CharaIndex];
                }
            }

            throw new Exception("Failed to get valid character index!");
        }
    }
    public class TGLP
    {
        public BNTX BinaryTextureFile;
        public List<Gx2ImageBlock> Gx2Textures = new List<Gx2ImageBlock>();

        public uint SectionSize;
        public byte CellWidth { get; set; }
        public byte CellHeight { get; set; }
        public byte MaxCharWidth { get; set; }
        public byte SheetCount { get; private set; }
        public uint SheetSize { get; set; }
        public ushort BaseLinePos { get; set; }
        public ushort Format { get; set; }
        public ushort ColumnCount { get; set; }
        public ushort RowCount { get; set; }
        public ushort SheetWidth { get; set; }
        public ushort SheetHeight { get; set; }
        public List<byte[]> SheetDataList = new List<byte[]>();

        public void Read(FileReader reader)
        {
            string Signature = reader.ReadString(4, Encoding.ASCII);
            if (Signature != "TGLP")
                throw new Exception($"Invalid signature {Signature}! Expected TGLP.");
            SectionSize = reader.ReadUInt32();
            CellWidth = reader.ReadByte();
            CellHeight = reader.ReadByte();
            SheetCount = reader.ReadByte();
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

        public void Write(FileWriter writer, FFNT Header)
        {
            long pos = writer.Position;

            writer.WriteSignature("TGLP");
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

            if (Header.Platform == FFNT.PlatformType.NX)
                writer.Align(4096);
            else
                writer.Align(8192);

            if (BinaryTextureFile != null)
                SheetDataList[0] = BinaryTextureFile.Save();

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
            using (writer.TemporarySeek(pos + 4, SeekOrigin.Begin))
            {
                writer.Write((uint)(SectionEndPosition - pos));
            }
        }

        public STGenericTexture GetImageSheet(int Index)
        {
            if (BinaryTextureFile != null) //BNTX uses only one image with multiple arrays
                return BinaryTextureFile.Textures.ElementAt(0).Value;
            else
                return Gx2Textures[Index];
        }
    }

    public interface CharMapping { }

    public class CMAPIndexTable : CharMapping
    {
        public short[] Table { get; set; }
    }

    public class CMAPDirect : CharMapping
    {
        public ushort Offset { get; set; }
    }

    public class CMAPScanMapping : CharMapping
    {
        public uint[] Codes { get; set; }
        public short[] Indexes { get; set; }
    }

    public class CMAP
    {
        public uint SectionSize;

        public char CharacterCodeBegin { get; set; }
        public char CharacterCodeEnd { get; set; }

        public Mapping MappingMethod { get; set; }

        private ushort Padding;

        public CharMapping MappingData;

        public enum Mapping : ushort
        {
            Direct,
            Table,
            Scan,
        }

        public CMAP NextCodeMapSection { get; set; }

        public void Read(FileReader reader, FFNT header, List<CMAP> CodeMaps)
        {
            uint CodeBegin = 0;
            uint CodeEnd = 0;

            long pos = reader.Position;

            reader.ReadSignature(4, "CMAP");
            SectionSize = reader.ReadUInt32();
            if (header.Platform == FFNT.PlatformType.NX)
            {
                CodeBegin = reader.ReadUInt32();
                CodeEnd = reader.ReadUInt32();
                MappingMethod = reader.ReadEnum<Mapping>(true);
                Padding = reader.ReadUInt16();
            }
            else
            {
                CodeBegin = reader.ReadUInt16();
                CodeEnd = reader.ReadUInt16();
                MappingMethod = reader.ReadEnum<Mapping>(true);
                Padding = reader.ReadUInt16();
            }

            CharacterCodeBegin = (char)CodeBegin;
            CharacterCodeEnd = (char)CodeEnd;

            uint NextMapOffset = reader.ReadUInt32();

            //Mapping methods from
            https://github.com/IcySon55/Kuriimu/blob/f670c2719affc1eaef8b4c40e40985881247acc7/src/Cetera/Font/BFFNT.cs#L211
            switch (MappingMethod)
            {
                case Mapping.Direct:
                    var charOffset = reader.ReadUInt16();
                    for (char i = CharacterCodeBegin; i <= CharacterCodeEnd; i++)
                    {
                        int idx = i - CharacterCodeBegin + charOffset;
                        header.FontSection.CodeMapDictionary[i] = idx < ushort.MaxValue ? idx : 0;
                    }

                    MappingData = new CMAPDirect();
                    ((CMAPDirect)MappingData).Offset = charOffset;
                    break;
                case Mapping.Table:
                    List<short> table = new List<short>();
                    for (char i = CharacterCodeBegin; i <= CharacterCodeEnd; i++)
                    {
                        short idx = reader.ReadInt16();
                        if (idx != -1) header.FontSection.CodeMapDictionary[i] = idx;

                        table.Add(idx);
                    }

                    MappingData = new CMAPIndexTable();
                    ((CMAPIndexTable)MappingData).Table = table.ToArray();
                    break;
                case Mapping.Scan:
                    var CharEntryCount = reader.ReadUInt16();

                    if (header.Platform == FFNT.PlatformType.NX)
                        reader.ReadUInt16(); //Padding

                    uint[] codes = new uint[CharEntryCount];
                    short[] indexes = new short[CharEntryCount];

                    for (int i = 0; i < CharEntryCount; i++)
                    {
                        if (header.Platform == FFNT.PlatformType.NX)
                        {
                            uint charCode = reader.ReadUInt32();
                            short index = reader.ReadInt16();
                            short padding = reader.ReadInt16();
                            if (index != -1) header.FontSection.CodeMapDictionary[(char)charCode] = index;

                            codes[i] = charCode;
                            indexes[i] = index;
                        }
                        else
                        {
                            ushort charCode = reader.ReadUInt16();
                            short index = reader.ReadInt16();
                            if (index != -1) header.FontSection.CodeMapDictionary[(char)charCode] = index;

                            codes[i] = charCode;
                            indexes[i] = index;
                        }
                    }

                    MappingData = new CMAPScanMapping();
                    ((CMAPScanMapping)MappingData).Codes = codes;
                    ((CMAPScanMapping)MappingData).Indexes = indexes;
                    break;
            }

            if (NextMapOffset != 0)
            {
                reader.SeekBegin(NextMapOffset - 8);
                NextCodeMapSection = new CMAP();
                NextCodeMapSection.Read(reader, header, CodeMaps);
                CodeMaps.Add(NextCodeMapSection);
            }
            else
                reader.SeekBegin(pos + SectionSize);
        }

        public void Write(FileWriter writer, FFNT Header)
        {
            Header.BlockCounter += 1;

            long pos = writer.Position;

            writer.WriteSignature("CMAP");
            writer.Write(uint.MaxValue); //Section Size
            if (Header.Platform == FFNT.PlatformType.NX)
            {
                writer.Write((uint)CharacterCodeBegin);
                writer.Write((uint)CharacterCodeEnd);
            }
            else
            {
                writer.Write((ushort)CharacterCodeBegin);
                writer.Write((ushort)CharacterCodeEnd);
            }

            writer.Write(MappingMethod, true);
            writer.Seek(2);

            long DataPos = writer.Position;
            writer.Write(0); //Next Section Offset

            //Write the data
            switch (MappingMethod)
            {
                case Mapping.Direct:
                    writer.Write(((CMAPDirect)MappingData).Offset);
                    break;
                case Mapping.Table:
                    for (int i = 0; i < ((CMAPIndexTable)MappingData).Table.Length; i++)
                    {
                        writer.Write(((CMAPIndexTable)MappingData).Table[i]);
                    }
                    break;
                case Mapping.Scan:
                    writer.Write((ushort)((CMAPScanMapping)MappingData).Codes.Length);
                    if (Header.Platform == FFNT.PlatformType.NX)
                        writer.Seek(2); //Padding

                    for (int i = 0; i < ((CMAPScanMapping)MappingData).Codes.Length; i++)
                    {
                        if (Header.Platform == FFNT.PlatformType.NX)
                        {
                            writer.Write((uint)((CMAPScanMapping)MappingData).Codes[i]);
                            writer.Write(((CMAPScanMapping)MappingData).Indexes[i]);
                            writer.Write((ushort)0); //Padding
                        }
                        else
                        {
                            writer.Write((ushort)((CMAPScanMapping)MappingData).Codes[i]);
                            writer.Write(((CMAPScanMapping)MappingData).Indexes[i]);
                        }
                    }
                    break;
            }
            writer.Align(4); //Padding


            //Save section size
            long endPos = writer.Position;
            using (writer.TemporarySeek(pos + 4, SeekOrigin.Begin))
            {
                writer.Write((uint)(endPos - pos));
            }

            if (NextCodeMapSection != null)
            {
                writer.WriteUint32Offset(DataPos, -8);
                NextCodeMapSection.Write(writer, Header);
            }
        }

        //From https://github.com/dnasdw/3dsfont/blob/79e6f4ab6676d82fdcd6c0f79d9b0d7a343f82b5/src/bcfnt2charset/bcfnt2charset.cpp#L3
        //Todo add the rest of the encoding types
        public char CodeToU16Code(FINF.CharacterCode characterCode, ushort code)
        {
            if (code < 0x20)
            {
                return (char)0;
            }

            switch (characterCode)
            {
                case FINF.CharacterCode.Unicode:
                    return (char)code;
            }

            return (char)code;
        }
    }

    public class CWDH
    {
        public ushort StartIndex { get; set; }
        public ushort EndIndex { get; set; }

        public List<CharacterWidthEntry> WidthEntries = new List<CharacterWidthEntry>();

        public CWDH NextWidthSection { get; set; }

        public ushort EntryCount
        {
            get { return (ushort)(EndIndex - StartIndex + 1); }
        }

        public uint SectionSize;

        public void Read(FileReader reader, FFNT header, List<CWDH> CharacterWidths)
        {
            long pos = reader.Position;

            reader.ReadSignature(4, "CWDH");
            SectionSize = reader.ReadUInt32();
            StartIndex = reader.ReadUInt16();
            EndIndex = reader.ReadUInt16();
            uint NextWidthSectionOffset = reader.ReadUInt32();

            for (ushort i = StartIndex; i <= EndIndex; i++)
            {
                var entry = new CharacterWidthEntry();
                entry.LeftWidth = reader.ReadSByte();
                entry.GlyphWidth = reader.ReadByte();
                entry.Width = reader.ReadByte();
                WidthEntries.Add(entry);
            }

            if (NextWidthSectionOffset != 0)
            {
                reader.SeekBegin((int)NextWidthSectionOffset - 8);
                NextWidthSection = new CWDH();
                NextWidthSection.Read(reader, header, CharacterWidths);
                CharacterWidths.Add(NextWidthSection);
            }
            else
                reader.SeekBegin(pos + SectionSize);
        }

        public void Write(FileWriter writer, FFNT Header)
        {
            Header.BlockCounter += 1;

            long pos = writer.Position;

            writer.WriteSignature("CWDH");
            writer.Write(uint.MaxValue); //Section Size
            writer.Write(StartIndex);
            writer.Write(EndIndex);

            long DataPos = writer.Position;
            writer.Write(0); //NextOffset

            for (int i = 0; i < WidthEntries.Count; i++)
            {
                writer.Write(WidthEntries[i].LeftWidth);
                writer.Write(WidthEntries[i].GlyphWidth);
                writer.Write(WidthEntries[i].Width);
            }

            writer.Align(4);

            if (NextWidthSection != null)
            {
                writer.WriteUint32Offset(DataPos, -8);
                NextWidthSection.Write(writer, Header);
            }

            //Save section size
            long endPos = writer.Position;
            using (writer.TemporarySeek(pos + 4, SeekOrigin.Begin))
            {
                writer.Write((uint)(endPos - pos));
            }
        }
    }

    public class CharacterWidthEntry
    {
        public sbyte LeftWidth { get; set; }
        public byte GlyphWidth { get; set; }
        public byte Width { get; set; }
    }
}
