using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Windows.Forms;
using Syroot.NintenTools.NSW.Bntx.GFX;

namespace FirstPlugin.LuigisMansion3
{
    public class TexturePOWE : STGenericTexture
    {
        public long HeaderOffset;
        public long DataOffset;

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

            public uint HashIDUint
            {
                get
                {
                    return ID;
                }
            }

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
        public ushort Unknown;
        public byte Unknown2;
        public byte Unknown3;
        public ushort Unknown4;

        public void Read(FileReader reader)
        {
            //Magic and ID not pointed to for sub entries so just skip them for now
            //     uint magic = reader.ReadUInt32();
            //   if (magic != Identifier)
            //         throw new Exception($"Invalid texture header magic! Expected {Identifier.ToString("x")}. Got {Identifier.ToString("x")}");
            //     ID = reader.ReadUInt32();

            CanReplace = true;
            CanRename = false;
            CanDelete = false;

            ID2 = reader.ReadUInt32();
            Width = reader.ReadUInt16();
            Height = reader.ReadUInt16();
            Unknown = reader.ReadUInt16();
            var numArray = reader.ReadByte();
            Unknown2 = reader.ReadByte();
            TexFormat = reader.ReadByte();
            Unknown3 = reader.ReadByte();
            Unknown4 = reader.ReadUInt16();

            Console.WriteLine(ID2);

            if (FormatTable.ContainsKey(TexFormat))
                Format = FormatTable[TexFormat];
            else
            {
                Format = TEX_FORMAT.ASTC_8x8_UNORM;
                Console.WriteLine("Unknown Format!" + TexFormat.ToString("X"));
            }

            MipCount = 1;
            ArrayCount = numArray;

            UpdateProperties();
        }

        private void UpdateProperties()
        {
            properties = new POWEProperties();
            properties.ID = ID2;
            properties.Width = Width;
            properties.Height = Height;
            properties.NumMips = (byte)MipCount;
            properties.Format = Format;
        }

        public override void OnClick(TreeView treeview) {
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
            editor.Text = Text;
            editor.LoadProperties(properties);
            editor.LoadImage(this);
        }

        public override void Replace(string FileName)
        {
            var bntxFile = new BNTX();
            var tex = new TextureData();
            tex.Replace(FileName, MipCount, 0, Format);

            //If it's null, the operation is cancelled
            if (tex.Texture == null)
                return;

            var surfacesNew = tex.GetSurfaces();
            var surfaces = GetSurfaces();

            if (surfaces[0].mipmaps[0].Length > surfacesNew[0].mipmaps[0].Length)
                throw new Exception($"Image must be the same size or smaller! {surfaces[0].mipmaps[0].Length}");

            ImageData = tex.Texture.TextureData[0][0];

            Width = tex.Texture.Width;
            Height = tex.Texture.Height;
            MipCount = tex.Texture.MipCount;
            ArrayCount = tex.Texture.ArrayLength;
            Format = tex.Format;
            TexFormat = FormatTable.FirstOrDefault(x => x.Value == tex.Format).Key;
            UpdateProperties();

            Console.WriteLine($"TexFormat {TexFormat.ToString("X")}");

            surfacesNew.Clear();
            surfaces.Clear();

            UpdateEditor();
        }

        public override bool CanEdit { get; set; } = true;

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            var tex = new Syroot.NintenTools.NSW.Bntx.Texture();
            tex.Height = (uint)bitmap.Height;
            tex.Width = (uint)bitmap.Width;
            tex.Format = TextureData.GenericToBntxSurfaceFormat(Format);
            tex.Name = Text;
            tex.Path = "";
            tex.TextureData = new List<List<byte[]>>();

            STChannelType[] channels = SetChannelsByFormat(Format);
            tex.sparseBinding = 0; //false
            tex.sparseResidency = 0; //false
            tex.Flags = 0;
            tex.Swizzle = 0;
            tex.textureLayout = 0;
            tex.Regs = new uint[0];
            tex.AccessFlags = AccessFlags.Texture;
            tex.ArrayLength = (uint)ArrayLevel;
            tex.MipCount = MipCount;
            tex.Depth = Depth;
            tex.Dim = Syroot.NintenTools.NSW.Bntx.GFX.Dim.Dim2D;
            tex.TileMode = Syroot.NintenTools.NSW.Bntx.GFX.TileMode.Default;
            tex.textureLayout2 = 0x010007;
            tex.SurfaceDim = Syroot.NintenTools.NSW.Bntx.GFX.SurfaceDim.Dim2D;
            tex.SampleCount = 1;
            tex.Pitch = 32;

            tex.MipOffsets = new long[tex.MipCount];

            var mipmaps = TextureImporterSettings.SwizzleSurfaceMipMaps(tex,
                GenerateMipsAndCompress(bitmap, MipCount, Format), MipCount);

            ImageData = Utils.CombineByteArray(mipmaps.ToArray());
            ArrayCount = tex.ArrayLength;
            TexFormat = FormatTable.FirstOrDefault(x => x.Value == Format).Key;
            UpdateProperties();
        }

        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
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

            return TegraX1Swizzle.GetImageData(this, ImageData, ArrayLevel, MipLevel, DepthLevel, BlockHeightLog2, 1);
        }

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[]
                {
                TEX_FORMAT.R8G8B8A8_UNORM,
                TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                TEX_FORMAT.BC1_UNORM,
                TEX_FORMAT.BC1_UNORM_SRGB,
                TEX_FORMAT.BC2_UNORM,
                TEX_FORMAT.BC3_UNORM,
                TEX_FORMAT.BC4_UNORM,
                TEX_FORMAT.BC5_SNORM,
                TEX_FORMAT.BC6H_UF16,
                TEX_FORMAT.BC7_UNORM,
                TEX_FORMAT.ASTC_4x4_UNORM,
                TEX_FORMAT.ASTC_5x4_UNORM,
                TEX_FORMAT.ASTC_5x5_UNORM,
                TEX_FORMAT.ASTC_6x5_UNORM,
                TEX_FORMAT.ASTC_6x6_UNORM,
                TEX_FORMAT.ASTC_8x5_UNORM,
                TEX_FORMAT.ASTC_8x6_UNORM,
                TEX_FORMAT.ASTC_8x8_UNORM,
                };
            }
        }
    }
}
