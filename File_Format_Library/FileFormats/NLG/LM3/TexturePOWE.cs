using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Windows.Forms;

namespace FirstPlugin.LuigisMansion3
{
    public class TexturePOWE : STGenericTexture
    {
        public static readonly uint Identifier = 0xE977D350;

        public uint Index { get; set; }

        public uint ID { get; set; }
        public uint ID2 { get; set; }

        public byte[] ImageData { get; set; }

        private POWEProperties properties;

        public class POWEProperties
        {
            [Browsable(false)]
            public uint ID { get; set; }

            public string HashID
            {
                get
                {
                    return ID.ToString("x");
                }
            }

            [ReadOnly(true)]
            public uint Width { get; set; }
            [ReadOnly(true)]
            public uint Height { get; set; }
            [ReadOnly(true)]
            public byte NumMips { get; set; }
            [ReadOnly(true)]
            public TEX_FORMAT Format { get; set; }

        }

        public Dictionary<byte, TEX_FORMAT> FormatTable = new Dictionary<byte, TEX_FORMAT>()
        {
            { 0x00, TEX_FORMAT.R8G8B8A8_UNORM },
            { 0x01, TEX_FORMAT.R8G8B8A8_UNORM_SRGB },
            { 0x11, TEX_FORMAT.BC1_UNORM },
            { 0x12, TEX_FORMAT.BC1_UNORM_SRGB },
            { 0x13, TEX_FORMAT.BC2_UNORM },
            { 0x14, TEX_FORMAT.BC3_UNORM },
            { 0x15, TEX_FORMAT.BC4_UNORM },
            { 0x16, TEX_FORMAT.BC5_SNORM },
            { 0x17, TEX_FORMAT.BC6H_UF16 },
            { 0x18, TEX_FORMAT.BC7_UNORM },
            { 0x19, TEX_FORMAT.ASTC_4x4_UNORM },
            { 0x1A, TEX_FORMAT.ASTC_5x4_UNORM },
            { 0x1B, TEX_FORMAT.ASTC_5x5_UNORM },
            { 0x1C, TEX_FORMAT.ASTC_6x5_UNORM },
            { 0x1D, TEX_FORMAT.ASTC_6x6_UNORM },
            { 0x1E, TEX_FORMAT.ASTC_8x5_UNORM },
            { 0x1F, TEX_FORMAT.ASTC_8x6_UNORM },
            { 0x20, TEX_FORMAT.ASTC_8x8_UNORM },
        };

        public byte TexFormat;
        public byte Unknown;
        public byte Unknown2;
        public ushort Unknown3;

        public void Read(FileReader reader)
        {
            //Magic and ID not pointed to for sub entries so just skip them for now
            //     uint magic = reader.ReadUInt32();
            //   if (magic != Identifier)
            //         throw new Exception($"Invalid texture header magic! Expected {Identifier.ToString("x")}. Got {Identifier.ToString("x")}");
            //     ID = reader.ReadUInt32();

            ID2 = reader.ReadUInt32();
            Width = reader.ReadUInt16();
            Height = reader.ReadUInt16();
            var numMips = reader.ReadByte();
            var unk = reader.ReadByte(); //padding?
            var numArray = reader.ReadByte();
            Unknown = reader.ReadByte();
            TexFormat = reader.ReadByte();
            Unknown2 = reader.ReadByte();
            Unknown3 = reader.ReadUInt16();

            if (FormatTable.ContainsKey(TexFormat))
                Format = FormatTable[TexFormat];
            else
            {
                Format = TEX_FORMAT.ASTC_8x8_UNORM;
                Console.WriteLine("Unknown Format!" + TexFormat.ToString("X"));
            }

            MipCount = 1;
            ArrayCount = numArray;

            properties = new POWEProperties();
            properties.ID = ID2;
            properties.Width = Width;
            properties.Height = Height;
            properties.NumMips = numMips;
            properties.Format = Format;
        }

        public override void OnClick(TreeView treeview)
        {
            ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
            if (editor == null)
            {
                editor = new ImageEditorBase();
                editor.Dock = DockStyle.Fill;

                LibraryGUI.LoadEditor(editor);
            }
            editor.Text = Text;
            editor.LoadProperties(properties);
            editor.LoadImage(this);
        }

        public override bool CanEdit { get; set; } = false;

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            throw new NotImplementedException();
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            uint blkHeight = STGenericTexture.GetBlockHeight(Format);
            uint blkDepth = STGenericTexture.GetBlockDepth(Format);
            uint  blockHeight = TegraX1Swizzle.GetBlockHeight(TegraX1Swizzle.DIV_ROUND_UP(Height, blkHeight));
            uint BlockHeightLog2 = (uint)Convert.ToString(blockHeight, 2).Length;

            //    if (Format == TEX_FORMAT.ASTC_4x4_UNORM || Format == TEX_FORMAT.ASTC_6x6_UNORM || Format == TEX_FORMAT.BC5_SNORM)

            if (Format != TEX_FORMAT.ASTC_8x5_UNORM)
                BlockHeightLog2 -= 1;

            Console.WriteLine("blkHeight " + blkHeight);
            Console.WriteLine("blockHeight " + blockHeight);
            Console.WriteLine("BlockHeightLog2 " + BlockHeightLog2);

            return TegraX1Swizzle.GetImageData(this, ImageData, ArrayLevel, MipLevel, BlockHeightLog2, 1);
        }

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[]
                {
                    TEX_FORMAT.B5G6R5_UNORM,
                    TEX_FORMAT.R8G8_UNORM,
                    TEX_FORMAT.B5G5R5A1_UNORM,
                    TEX_FORMAT.B4G4R4A4_UNORM,
                    TEX_FORMAT.LA8,
                    TEX_FORMAT.HIL08,
                    TEX_FORMAT.L8,
                    TEX_FORMAT.A8_UNORM,
                    TEX_FORMAT.LA4,
                    TEX_FORMAT.A4,
                    TEX_FORMAT.ETC1_UNORM,
                    TEX_FORMAT.ETC1_A4,
            };
            }
        }
    }
}
