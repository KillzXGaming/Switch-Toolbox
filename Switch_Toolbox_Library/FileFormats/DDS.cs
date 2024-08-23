using System;
using System.Data;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Syroot.BinaryData;
using System.IO;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using SFGraphics.GLObjects.Textures;
using OpenTK.Graphics.OpenGL;

namespace Toolbox.Library
{
    //Data from https://github.com/jam1garner/Smash-Forge/blob/master/Smash%20Forge/Filetypes/Textures/DDS.cs
    public class DDS : STGenericTexture, IFileFormat, IContextMenuNode
    {
        public STGenericTexture IconTexture { get { return this; } }

        public FileType FileType { get; set; } = FileType.Image;

        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return new TEX_FORMAT[]
               {
                    TEX_FORMAT.BC1_UNORM,
                    TEX_FORMAT.BC1_UNORM_SRGB,
                    TEX_FORMAT.BC2_UNORM,
                    TEX_FORMAT.BC2_UNORM_SRGB,
                    TEX_FORMAT.BC3_UNORM,
                    TEX_FORMAT.BC3_UNORM_SRGB,
                    TEX_FORMAT.BC4_UNORM,
                    TEX_FORMAT.BC4_SNORM,
                    TEX_FORMAT.BC5_UNORM,
                    TEX_FORMAT.BC5_SNORM,
                    TEX_FORMAT.BC6H_UF16,
                    TEX_FORMAT.BC6H_SF16,
                    TEX_FORMAT.BC7_UNORM,
                    TEX_FORMAT.BC7_UNORM_SRGB,
                    TEX_FORMAT.B5G5R5A1_UNORM,
                    TEX_FORMAT.B5G6R5_UNORM,
                    TEX_FORMAT.B8G8R8A8_UNORM_SRGB,
                    TEX_FORMAT.B8G8R8A8_UNORM,
                    TEX_FORMAT.R10G10B10A2_UNORM,
                    TEX_FORMAT.R16_UNORM,
                    TEX_FORMAT.B4G4R4A4_UNORM,
                    TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                    TEX_FORMAT.R8G8B8A8_UNORM,
                    TEX_FORMAT.R8_UNORM,
                    TEX_FORMAT.R8G8_UNORM,
                    TEX_FORMAT.R32G8X24_FLOAT,
                 };
            }
        }

        public override bool CanEdit { get; set; } = true;

        public bool CanSave { get; set; } = false;
        public bool FileIsEdited { get; set; } = false;
        public bool FileIsCompressed { get; set; } = false;
        public string[] Description { get; set; } = new string[] { "Microsoft DDS" };
        public string[] Extension { get; set; } = new string[] { "*.dds" };
        public CompressionType CompressionType { get; set; } = CompressionType.None;
        public string FileName { get; set; }
        public bool IsActive { get; set; } = false;
        public bool UseEditMenu { get; set; } = false;
        public int Alignment { get; set; } = 0;
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "DDS ");
            }
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Save", null, SaveAction, Keys.Control | Keys.S));
            Items.AddRange(base.GetContextMenuItems());
            return Items.ToArray();
        }

        private void SaveAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(this);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
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

        public void Load(System.IO.Stream stream)
        {
            IsActive = true;
            CanSave = true;
            CanReplace = true;

            FileReader reader = new FileReader(stream);
            reader.ByteOrder = ByteOrder.LittleEndian;
            Load(reader);
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream) {
            Save(this, stream, GetSurfaces());
        }


        public PixelInternalFormat pixelInternalFormat;
        public OpenTK.Graphics.OpenGL.PixelFormat pixelFormat;
        public OpenTK.Graphics.OpenGL.PixelType pixelType;

        public const uint FOURCC_DXT1 = 0x31545844;
        public const uint FOURCC_DXT2 = 0x32545844;
        public const uint FOURCC_DXT3 = 0x33545844;
        public const uint FOURCC_DXT4 = 0x34545844;
        public const uint FOURCC_DXT5 = 0x35545844;
        public const uint FOURCC_ATI1 = 0x31495441;
        public const uint FOURCC_BC4U = 0x55344342;
        public const uint FOURCC_BC4S = 0x53344342;
        public const uint FOURCC_BC5U = 0x55354342;
        public const uint FOURCC_BC5S = 0x53354342;
        public const uint FOURCC_DX10 = 0x30315844;

        public const uint FOURCC_ATI2 = 0x32495441;
        public const uint FOURCC_RXGB = 0x42475852;

        // RGBA Masks
        private static int[] A1R5G5B5_MASKS = { 0x7C00, 0x03E0, 0x001F, 0x8000 };
        private static int[] X1R5G5B5_MASKS = { 0x7C00, 0x03E0, 0x001F, 0x0000 };
        private static int[] A4R4G4B4_MASKS = { 0x0F00, 0x00F0, 0x000F, 0xF000 };
        private static int[] X4R4G4B4_MASKS = { 0x0F00, 0x00F0, 0x000F, 0x0000 };
        private static int[] R5G6B5_MASKS = { 0xF800, 0x07E0, 0x001F, 0x0000 };
        private static int[] R8G8B8_MASKS = { 0xFF0000, 0x00FF00, 0x0000FF, 0x000000 };
        private static uint[] A8B8G8R8_MASKS = { 0x000000FF, 0x0000FF00, 0x00FF0000, 0xFF000000 };
        private static int[] X8B8G8R8_MASKS = { 0x000000FF, 0x0000FF00, 0x00FF0000, 0x00000000 };
        private static uint[] A8R8G8B8_MASKS = { 0x00FF0000, 0x0000FF00, 0x000000FF, 0xFF000000 };
        private static int[] X8R8G8B8_MASKS = { 0x00FF0000, 0x0000FF00, 0x000000FF, 0x00000000 };

        private static int[] L8_MASKS = { 0x000000FF, 0x0000 ,};
        private static int[] A8L8_MASKS = { 0x000000FF, 0x0F00, };

        public enum CubemapFace
        {
            PosX,
            NegX,
            PosY,
            NegY,
            PosZ,
            NegZ
        }

        [Flags]
        public enum DDSD : uint
        {
            CAPS = 0x00000001,
            HEIGHT = 0x00000002,
            WIDTH = 0x00000004,
            PITCH = 0x00000008,
            PIXELFORMAT = 0x00001000,
            MIPMAPCOUNT = 0x00020000,
            LINEARSIZE = 0x00080000,
            DEPTH = 0x00800000
        }
        [Flags]
        public enum DDPF : uint
        {
            ALPHAPIXELS = 0x00000001,
            ALPHA = 0x00000002,
            FOURCC = 0x00000004,
            RGB = 0x00000040,
            YUV = 0x00000200,
            LUMINANCE = 0x00020000,
        }
        [Flags]
        public enum DDSCAPS : uint
        {
            COMPLEX = 0x00000008,
            TEXTURE = 0x00001000,
            MIPMAP = 0x00400000,
        }
        [Flags]
        public enum DDSCAPS2 : uint
        {
            CUBEMAP = 0x00000200,
            CUBEMAP_POSITIVEX = 0x00000400 | CUBEMAP,
            CUBEMAP_NEGATIVEX = 0x00000800 | CUBEMAP,
            CUBEMAP_POSITIVEY = 0x00001000 | CUBEMAP,
            CUBEMAP_NEGATIVEY = 0x00002000 | CUBEMAP,
            CUBEMAP_POSITIVEZ = 0x00004000 | CUBEMAP,
            CUBEMAP_NEGATIVEZ = 0x00008000 | CUBEMAP,
            CUBEMAP_ALLFACES = (CUBEMAP_POSITIVEX | CUBEMAP_NEGATIVEX |
                                  CUBEMAP_POSITIVEY | CUBEMAP_NEGATIVEY |
                                  CUBEMAP_POSITIVEZ | CUBEMAP_NEGATIVEZ),
            VOLUME = 0x00200000
        }

        public static bool getFormatBlock(uint fourCC)
        {
            switch (fourCC)
            {
                case FOURCC_DXT1:
                case FOURCC_DXT2:
                case FOURCC_DXT3:
                case FOURCC_DXT4:
                case FOURCC_DXT5:
                case FOURCC_ATI1:
                case FOURCC_ATI2:
                case FOURCC_BC4U:
                case FOURCC_BC4S:
                case FOURCC_BC5U:
                case FOURCC_BC5S:
                    return true;
                default:
                    return false;
            }
        }

        public void SetFourCC(DXGI_FORMAT Format)
        {
            switch (Format)
            {
                case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
                case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
                    header.ddspf.fourCC = FOURCC_DXT1;
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
                case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB:
                    header.ddspf.fourCC = FOURCC_DXT3;
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
                case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
                    header.ddspf.fourCC = FOURCC_DXT5;
                    break;
            }
        }
        public bool IsDX10;

        public Header header;
        public DX10Header DX10header;
        public class Header
        {
            public uint size = 0x7C;
            public uint flags = 0x00000000;
            public uint height = 0;
            public uint width = 0;
            public uint pitchOrLinearSize = 0;
            public uint depth = 0;
            public uint mipmapCount = 0;
            public uint[] reserved1 = new uint[11];
            public DDS_PixelFormat ddspf = new DDS_PixelFormat();
            public class DDS_PixelFormat
            {
                public uint size = 0x20;
                public uint flags = 0x00000000;
                public uint fourCC;
                public uint RGBBitCount = 0;
                public uint RBitMask = 0x00000000;
                public uint GBitMask = 0x00000000;
                public uint BBitMask = 0x00000000;
                public uint ABitMask = 0x00000000;
            }
            public uint caps = 0;
            public uint caps2 = 0;
            public uint caps3 = 0;
            public uint caps4 = 0;
            public uint reserved2 = 0;
        }
        public class DX10Header
        {
            public DXGI_FORMAT DXGI_Format;
            public uint ResourceDim;
            public uint miscFlag;
            public uint arrayFlag;
            public uint miscFlags2;

        }
        public byte[] bdata;
        public List<byte[]> mipmaps = new List<byte[]>();

        public enum DXGI_FORMAT : uint
        {
            DXGI_FORMAT_UNKNOWN = 0,
            DXGI_FORMAT_R32G32B32A32_TYPELESS = 1,
            DXGI_FORMAT_R32G32B32A32_FLOAT = 2,
            DXGI_FORMAT_R32G32B32A32_UINT = 3,
            DXGI_FORMAT_R32G32B32A32_SINT = 4,
            DXGI_FORMAT_R32G32B32_TYPELESS = 5,
            DXGI_FORMAT_R32G32B32_FLOAT = 6,
            DXGI_FORMAT_R32G32B32_UINT = 7,
            DXGI_FORMAT_R32G32B32_SINT = 8,
            DXGI_FORMAT_R16G16B16A16_TYPELESS = 9,
            DXGI_FORMAT_R16G16B16A16_FLOAT = 10,
            DXGI_FORMAT_R16G16B16A16_UNORM = 11,
            DXGI_FORMAT_R16G16B16A16_UINT = 12,
            DXGI_FORMAT_R16G16B16A16_SNORM = 13,
            DXGI_FORMAT_R16G16B16A16_SINT = 14,
            DXGI_FORMAT_R32G32_TYPELESS = 15,
            DXGI_FORMAT_R32G32_FLOAT = 16,
            DXGI_FORMAT_R32G32_UINT = 17,
            DXGI_FORMAT_R32G32_SINT = 18,
            DXGI_FORMAT_R32G8X24_TYPELESS = 19,
            DXGI_FORMAT_D32_FLOAT_S8X24_UINT = 20,
            DXGI_FORMAT_R32_FLOAT_X8X24_TYPELESS = 21,
            DXGI_FORMAT_X32_TYPELESS_G8X24_UINT = 22,
            DXGI_FORMAT_R10G10B10A2_TYPELESS = 23,
            DXGI_FORMAT_R10G10B10A2_UNORM = 24,
            DXGI_FORMAT_R10G10B10A2_UINT = 25,
            DXGI_FORMAT_R11G11B10_FLOAT = 26,
            DXGI_FORMAT_R8G8B8A8_TYPELESS = 27,
            DXGI_FORMAT_R8G8B8A8_UNORM = 28,
            DXGI_FORMAT_R8G8B8A8_UNORM_SRGB = 29,
            DXGI_FORMAT_R8G8B8A8_UINT = 30,
            DXGI_FORMAT_R8G8B8A8_SNORM = 31,
            DXGI_FORMAT_R8G8B8A8_SINT = 32,
            DXGI_FORMAT_R16G16_TYPELESS = 33,
            DXGI_FORMAT_R16G16_FLOAT = 34,
            DXGI_FORMAT_R16G16_UNORM = 35,
            DXGI_FORMAT_R16G16_UINT = 36,
            DXGI_FORMAT_R16G16_SNORM = 37,
            DXGI_FORMAT_R16G16_SINT = 38,
            DXGI_FORMAT_R32_TYPELESS = 39,
            DXGI_FORMAT_D32_FLOAT = 40,
            DXGI_FORMAT_R32_FLOAT = 41,
            DXGI_FORMAT_R32_UINT = 42,
            DXGI_FORMAT_R32_SINT = 43,
            DXGI_FORMAT_R24G8_TYPELESS = 44,
            DXGI_FORMAT_D24_UNORM_S8_UINT = 45,
            DXGI_FORMAT_R24_UNORM_X8_TYPELESS = 46,
            DXGI_FORMAT_X24_TYPELESS_G8_UINT = 47,
            DXGI_FORMAT_R8G8_TYPELESS = 48,
            DXGI_FORMAT_R8G8_UNORM = 49,
            DXGI_FORMAT_R8G8_UINT = 50,
            DXGI_FORMAT_R8G8_SNORM = 51,
            DXGI_FORMAT_R8G8_SINT = 52,
            DXGI_FORMAT_R16_TYPELESS = 53,
            DXGI_FORMAT_R16_FLOAT = 54,
            DXGI_FORMAT_D16_UNORM = 55,
            DXGI_FORMAT_R16_UNORM = 56,
            DXGI_FORMAT_R16_UINT = 57,
            DXGI_FORMAT_R16_SNORM = 58,
            DXGI_FORMAT_R16_SINT = 59,
            DXGI_FORMAT_R8_TYPELESS = 60,
            DXGI_FORMAT_R8_UNORM = 61,
            DXGI_FORMAT_R8_UINT = 62,
            DXGI_FORMAT_R8_SNORM = 63,
            DXGI_FORMAT_R8_SINT = 64,
            DXGI_FORMAT_A8_UNORM = 65,
            DXGI_FORMAT_R1_UNORM = 66,
            DXGI_FORMAT_R9G9B9E5_SHAREDEXP = 67,
            DXGI_FORMAT_R8G8_B8G8_UNORM = 68,
            DXGI_FORMAT_G8R8_G8B8_UNORM = 69,
            DXGI_FORMAT_BC1_TYPELESS = 70,
            DXGI_FORMAT_BC1_UNORM = 71,
            DXGI_FORMAT_BC1_UNORM_SRGB = 72,
            DXGI_FORMAT_BC2_TYPELESS = 73,
            DXGI_FORMAT_BC2_UNORM = 74,
            DXGI_FORMAT_BC2_UNORM_SRGB = 75,
            DXGI_FORMAT_BC3_TYPELESS = 76,
            DXGI_FORMAT_BC3_UNORM = 77,
            DXGI_FORMAT_BC3_UNORM_SRGB = 78,
            DXGI_FORMAT_BC4_TYPELESS = 79,
            DXGI_FORMAT_BC4_UNORM = 80,
            DXGI_FORMAT_BC4_SNORM = 81,
            DXGI_FORMAT_BC5_TYPELESS = 82,
            DXGI_FORMAT_BC5_UNORM = 83,
            DXGI_FORMAT_BC5_SNORM = 84,
            DXGI_FORMAT_B5G6R5_UNORM = 85,
            DXGI_FORMAT_B5G5R5A1_UNORM = 86,
            DXGI_FORMAT_B8G8R8A8_UNORM = 87,
            DXGI_FORMAT_B8G8R8X8_UNORM = 88,
            DXGI_FORMAT_R10G10B10_XR_BIAS_A2_UNORM = 89,
            DXGI_FORMAT_B8G8R8A8_TYPELESS = 90,
            DXGI_FORMAT_B8G8R8A8_UNORM_SRGB = 91,
            DXGI_FORMAT_B8G8R8X8_TYPELESS = 92,
            DXGI_FORMAT_B8G8R8X8_UNORM_SRGB = 93,
            DXGI_FORMAT_BC6H_TYPELESS = 94,
            DXGI_FORMAT_BC6H_UF16 = 95,
            DXGI_FORMAT_BC6H_SF16 = 96,
            DXGI_FORMAT_BC7_TYPELESS = 97,
            DXGI_FORMAT_BC7_UNORM = 98,
            DXGI_FORMAT_BC7_UNORM_SRGB = 99,
            DXGI_FORMAT_AYUV = 100,
            DXGI_FORMAT_Y410 = 101,
            DXGI_FORMAT_Y416 = 102,
            DXGI_FORMAT_NV12 = 103,
            DXGI_FORMAT_P010 = 104,
            DXGI_FORMAT_P016 = 105,
            DXGI_FORMAT_420_OPAQUE = 106,
            DXGI_FORMAT_YUY2 = 107,
            DXGI_FORMAT_Y210 = 108,
            DXGI_FORMAT_Y216 = 109,
            DXGI_FORMAT_NV11 = 110,
            DXGI_FORMAT_AI44 = 111,
            DXGI_FORMAT_IA44 = 112,
            DXGI_FORMAT_P8 = 113,
            DXGI_FORMAT_A8P8 = 114,
            DXGI_FORMAT_B4G4R4A4_UNORM = 115,
            DXGI_FORMAT_P208 = 130,
            DXGI_FORMAT_V208 = 131,
            DXGI_FORMAT_V408 = 132,


            DXGI_FORMAT_ASTC_4X4_UNORM = 134,
            DXGI_FORMAT_ASTC_4X4_UNORM_SRGB = 135,
            DXGI_FORMAT_ASTC_5X4_TYPELESS = 137,
            DXGI_FORMAT_ASTC_5X4_UNORM = 138,
            DXGI_FORMAT_ASTC_5X4_UNORM_SRGB = 139,
            DXGI_FORMAT_ASTC_5X5_TYPELESS = 141,
            DXGI_FORMAT_ASTC_5X5_UNORM = 142,
            DXGI_FORMAT_ASTC_5X5_UNORM_SRGB = 143,
            DXGI_FORMAT_ASTC_6X5_TYPELESS = 145,
            DXGI_FORMAT_ASTC_6X5_UNORM = 146,
            DXGI_FORMAT_ASTC_6X5_UNORM_SRGB = 147,
            DXGI_FORMAT_ASTC_6X6_TYPELESS = 149,
            DXGI_FORMAT_ASTC_6X6_UNORM = 150,
            DXGI_FORMAT_ASTC_6X6_UNORM_SRGB = 151,
            DXGI_FORMAT_ASTC_8X5_TYPELESS = 153,
            DXGI_FORMAT_ASTC_8X5_UNORM = 154,
            DXGI_FORMAT_ASTC_8X5_UNORM_SRGB = 155,
            DXGI_FORMAT_ASTC_8X6_TYPELESS = 157,
            DXGI_FORMAT_ASTC_8X6_UNORM = 158,
            DXGI_FORMAT_ASTC_8X6_UNORM_SRGB = 159,
            DXGI_FORMAT_ASTC_8X8_TYPELESS = 161,
            DXGI_FORMAT_ASTC_8X8_UNORM = 162,
            DXGI_FORMAT_ASTC_8X8_UNORM_SRGB = 163,
            DXGI_FORMAT_ASTC_10X5_TYPELESS = 165,
            DXGI_FORMAT_ASTC_10X5_UNORM = 166,
            DXGI_FORMAT_ASTC_10X5_UNORM_SRGB = 167,
            DXGI_FORMAT_ASTC_10X6_TYPELESS = 169,
            DXGI_FORMAT_ASTC_10X6_UNORM = 170,
            DXGI_FORMAT_ASTC_10X6_UNORM_SRGB = 171,
            DXGI_FORMAT_ASTC_10X8_TYPELESS = 173,
            DXGI_FORMAT_ASTC_10X8_UNORM = 174,
            DXGI_FORMAT_ASTC_10X8_UNORM_SRGB = 175,
            DXGI_FORMAT_ASTC_10X10_TYPELESS = 177,
            DXGI_FORMAT_ASTC_10X10_UNORM = 178,
            DXGI_FORMAT_ASTC_10X10_UNORM_SRGB = 179,
            DXGI_FORMAT_ASTC_12X10_TYPELESS = 181,
            DXGI_FORMAT_ASTC_12X10_UNORM = 182,
            DXGI_FORMAT_ASTC_12X10_UNORM_SRGB = 183,
            DXGI_FORMAT_ASTC_12X12_TYPELESS = 185,
            DXGI_FORMAT_ASTC_12X12_UNORM = 186,
            DXGI_FORMAT_ASTC_12X12_UNORM_SRGB = 187,

            DXGI_FORMAT_FORCE_UINT = 0xFFFFFFFF
        }

        public enum DXGI_ASTC_FORMAT
        {
          
        }

        public DDS()
        {

        }
        public DDS(byte[] data)
        {
            FileReader reader = new FileReader(new MemoryStream(data));
            reader.ByteOrder = ByteOrder.LittleEndian;
            Load(reader);
        }
        public DDS(string FileName)
        {
            FileReader reader = new FileReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read));

            Load(reader);
        }



        public void Load(BinaryDataReader reader)
        {
            Text = Path.GetFileNameWithoutExtension(FileName);

            reader.Seek(0);
            string Magic = reader.ReadString(4);
            Console.WriteLine(Magic);
            if (Magic != "DDS ")
            {
                MessageBox.Show("The file does not appear to be a valid DDS file.");
            }

            header = new Header();
            header.size = reader.ReadUInt32();
            header.flags = reader.ReadUInt32();
            header.height = reader.ReadUInt32();
            header.width = reader.ReadUInt32();
            header.pitchOrLinearSize = reader.ReadUInt32();
            header.depth = reader.ReadUInt32();
            header.mipmapCount = reader.ReadUInt32();
            header.reserved1 = new uint[11];
            for (int i = 0; i < 11; ++i)
                header.reserved1[i] = reader.ReadUInt32();

            header.ddspf.size = reader.ReadUInt32();
            header.ddspf.flags = reader.ReadUInt32();
            header.ddspf.fourCC = reader.ReadUInt32();
            header.ddspf.RGBBitCount = reader.ReadUInt32();
            header.ddspf.RBitMask = reader.ReadUInt32();
            header.ddspf.GBitMask = reader.ReadUInt32();
            header.ddspf.BBitMask = reader.ReadUInt32();
            header.ddspf.ABitMask = reader.ReadUInt32();

            header.caps = reader.ReadUInt32();
            header.caps2 = reader.ReadUInt32();
            header.caps3 = reader.ReadUInt32();
            header.caps4 = reader.ReadUInt32();
            header.reserved2 = reader.ReadUInt32();

            if (header.reserved1[9] == 1414813262)
                WiiUSwizzle = true;

            ArrayCount = 1;

            int DX10HeaderSize = 0;
            if (header.ddspf.fourCC == FOURCC_DX10)
            {
                IsDX10 = true;

                DX10HeaderSize = 20;
                ReadDX10Header(reader);
            }

            if (header.caps2 == (uint)DDS.DDSCAPS2.CUBEMAP_ALLFACES)
            {
                ArrayCount = 6;
            }

            bool Compressed = false;
            bool HasLuminance = false;
            bool HasAlpha = false;
            bool IsRGB = false;

            if (header.ddspf.flags == 4)
                Compressed = true;
            else if (header.ddspf.flags == (uint)DDPF.LUMINANCE || header.ddspf.flags == 2)
                HasLuminance = true;
            else if (header.ddspf.flags == 0x20001)
            {
                HasLuminance = true;
                HasAlpha = true;
            }
            else if (header.ddspf.flags == (uint)DDPF.RGB)
            {
                IsRGB = true;
            }
            else if (header.ddspf.flags == 0x41)
            {
                IsRGB = true;
                HasAlpha = true;
                HasAlpha = true;
            }

            reader.TemporarySeek((int)(4 + header.size + DX10HeaderSize), SeekOrigin.Begin);
            var UbiExtraData = reader.ReadUInt16();
            reader.TemporarySeek(-2, SeekOrigin.Current);
            if (UbiExtraData == 12816 || UbiExtraData == 1331 && IsDX10) //me when ubisoft | for some reason theres some extra data on some mario rabbids textures god knows what it is
            {
                if (header.width == 1024 && header.height == 1024)
                {
                    reader.TemporarySeek((int)(4 + 30 + header.size + DX10HeaderSize), SeekOrigin.Begin);
                }
                if (header.width == 512 && header.height == 512)
                {
                    reader.TemporarySeek((int)(4 + 26 + header.size + DX10HeaderSize), SeekOrigin.Begin);
                }
            }
            bdata = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));

            Format = GetFormat();
            Width = header.width;
            Height = header.height;
            MipCount = header.mipmapCount;
            Depth = header.depth;
            if (Depth == 0)
                Depth = 1;

            byte[] Components = new byte[4] { 0, 1, 2, 3 };

            if (!IsDX10 && !IsCompressed()) {
                Format = GetUncompressedType(this, Components, IsRGB, HasAlpha, HasLuminance, header.ddspf);
            }

            RedChannel = (STChannelType)Components[0];
            GreenChannel = (STChannelType)Components[1];
            BlueChannel = (STChannelType)Components[2];
            AlphaChannel = (STChannelType)Components[3];

            reader.Dispose();
            reader.Close();



        }
        private TEX_FORMAT GetUncompressedType(DDS dds, byte[] Components, bool IsRGB, bool HasAlpha, bool HasLuminance, Header.DDS_PixelFormat header)
        {
            uint bpp = header.RGBBitCount;
            uint RedMask = header.RBitMask;
            uint GreenMask = header.GBitMask;
            uint BlueMask = header.BBitMask;
            uint AlphaMask = HasAlpha ? header.ABitMask : 0;

            if (HasLuminance)
            {
                throw new Exception("Luminance not supported!");
            }
            else if (IsRGB)
            {
                if (bpp == 16)
                {
                    if (RedMask == A1R5G5B5_MASKS[0] && GreenMask == A1R5G5B5_MASKS[1] && BlueMask == A1R5G5B5_MASKS[2] && AlphaMask == A1R5G5B5_MASKS[3])
                    {
                        return TEX_FORMAT.B5G5R5A1_UNORM;
                    }
                    else if (RedMask == X1R5G5B5_MASKS[0] && GreenMask == X1R5G5B5_MASKS[1] && BlueMask == X1R5G5B5_MASKS[2] && AlphaMask == X1R5G5B5_MASKS[3])
                    {
                        return TEX_FORMAT.B5G6R5_UNORM;
                    }
                    else if (RedMask == A4R4G4B4_MASKS[0] && GreenMask == A4R4G4B4_MASKS[1] && BlueMask == A4R4G4B4_MASKS[2] && AlphaMask == A4R4G4B4_MASKS[3])
                    {
                        return TEX_FORMAT.B4G4R4A4_UNORM;
                    }
                    else if (RedMask == X4R4G4B4_MASKS[0] && GreenMask == X4R4G4B4_MASKS[1] && BlueMask == X4R4G4B4_MASKS[2] && AlphaMask == X4R4G4B4_MASKS[3])
                    {
                        return TEX_FORMAT.B4G4R4A4_UNORM;
                    }
                    else if (RedMask == R5G6B5_MASKS[0] && GreenMask == R5G6B5_MASKS[1] && BlueMask == R5G6B5_MASKS[2] && AlphaMask == R5G6B5_MASKS[3])
                    {
                        return TEX_FORMAT.B5G6R5_UNORM;
                    }
                    else
                    {
                        throw new Exception("Unsupported 16 bit image!");
                    }
                }
                else if (bpp == 24)
                {
                    if (RedMask == R8G8B8_MASKS[0] && GreenMask == R8G8B8_MASKS[1] && BlueMask == R8G8B8_MASKS[2] && AlphaMask == R8G8B8_MASKS[3])
                    {
                        dds.bdata = ConvertToRgba(this, "RGB8", 3,  new byte[4] { 2, 1, 0, 3 });
                        return TEX_FORMAT.R8G8B8A8_UNORM;
                    }
                    else
                    {
                        throw new Exception("Unsupported 24 bit image!");
                    }
                }
                else if (bpp == 32)
                {
                    if (RedMask == A8B8G8R8_MASKS[0] && GreenMask == A8B8G8R8_MASKS[1] && BlueMask == A8B8G8R8_MASKS[2] && AlphaMask == A8B8G8R8_MASKS[3])
                    {
                        return TEX_FORMAT.R8G8B8A8_UNORM;
                    }
                    else if (RedMask == X8B8G8R8_MASKS[0] && GreenMask == X8B8G8R8_MASKS[1] && BlueMask == X8B8G8R8_MASKS[2] && AlphaMask == X8B8G8R8_MASKS[3])
                    {
                        dds.bdata = ConvertToRgba(this, "RGB8X", 4, new byte[4] { 2, 1, 0, 3 });
                        return TEX_FORMAT.B8G8R8X8_UNORM;
                    }
                    else if (RedMask == A8R8G8B8_MASKS[0] && GreenMask == A8R8G8B8_MASKS[1] && BlueMask == A8R8G8B8_MASKS[2] && AlphaMask == A8R8G8B8_MASKS[3])
                    {
                        dds.bdata = ConvertBgraToRgba(dds.bdata);
                        return TEX_FORMAT.R8G8B8A8_UNORM;
                    }
                    else if (RedMask == X8R8G8B8_MASKS[0] && GreenMask == X8R8G8B8_MASKS[1] && BlueMask == X8R8G8B8_MASKS[2] && AlphaMask == X8R8G8B8_MASKS[3])
                    {
                        dds.bdata = ConvertToRgba(this, "RGB8X", 4, new byte[4] { 0, 1, 2, 3 });
                        return TEX_FORMAT.B8G8R8X8_UNORM;
                    }
                    else
                    {
                        throw new Exception("Unsupported 32 bit image!");
                    }
                }
            }
            else
            {
                throw new Exception("Unknown type!");
            }
            return TEX_FORMAT.UNKNOWN;
        }

        private static byte[] ConvertRgb8ToRgbx8(byte[] bytes)
        {
            int size = bytes.Length / 3;
            byte[] NewData = new byte[size];

            for (int i = 0; i < size; i ++)
            {
                NewData[4 * i + 0] = bytes[3 * i + 0];
                NewData[4 * i + 1] = bytes[3 * i + 1];
                NewData[4 * i + 2] = bytes[3 * i + 2];
                NewData[4 * i + 3] = 0xFF;
            }
            return NewData;
        }

        //Thanks abood. Based on https://github.com/aboood40091/BNTX-Editor/blob/master/formConv.py
        private static byte[] ConvertToRgba(DDS dds, string Format, int bpp, byte[] compSel)
        {
            byte[] bytes = dds.bdata;

            if (bytes == null)
                throw new Exception("Data block returned null. Make sure the parameters and image properties are correct!");

            List<byte[]> mipmaps = new List<byte[]>();

            uint Offset = 0;

            for (byte a = 0; a < dds.ArrayCount; ++a)
            {
                for (byte m = 0; m < dds.MipCount; ++m)
                {
                    uint MipWidth = Math.Max(1, dds.Width >> m);
                    uint MipHeight = Math.Max(1, dds.Height >> m);

                    uint NewSize = (MipWidth * MipHeight) * 4;
                    uint OldSize = (MipWidth * MipHeight) * (uint)bpp;

                    byte[] NewImageData = new byte[NewSize];
                    mipmaps.Add(NewImageData);

                    byte[] comp = new byte[4] { 0, 0, 0, 0xFF };

                    for (int j = 0; j < MipHeight * MipWidth; j++)
                    {
                        var pos = Offset + (j * bpp);
                        var pos_ = (j * 4);

                       int pixel = 0;
                       for (int i = 0; i < bpp; i += 1)
                            pixel |= bytes[pos + i] << (8 * i);

                        comp = GetComponentsFromPixel(Format, pixel, comp);
                        NewImageData[pos_ + 3] = comp[compSel[3]];
                        NewImageData[pos_ + 2] = comp[compSel[2]];
                        NewImageData[pos_ + 1] = comp[compSel[1]];
                        NewImageData[pos_ + 0] = comp[compSel[0]];
                    }

                    Offset += OldSize;
                }
            }

            return Utils.CombineByteArray(mipmaps.ToArray());
        }

        private static byte[] GetComponentsFromPixel(string Format, int pixel, byte[] comp)
        {
            switch (Format)
            {
                case "RGB8X":
                comp[0] = (byte)(pixel & 0xFF);
                comp[1] = (byte)((pixel & 0xFF00) >> 8);
                comp[2] = (byte)((pixel & 0xFF0000) >> 16);
                comp[3] = (byte)(0xFF);
                    break;
                case "RGB8":
                comp[0] = (byte)(pixel & 0xFF);
                comp[1] = (byte)((pixel & 0xFF00) >> 8);
                comp[2] = (byte)((pixel & 0xFF0000) >> 16);
                comp[3] = (byte)(0xFF);
                    break;
                case "RGBA4":
                comp[0] = (byte)((pixel & 0xF) * 17);
                comp[1] = (byte)(((pixel & 0xF0) >> 4) * 17);
                comp[2] = (byte)(((pixel & 0xF00) >> 8) * 17);
                comp[3] = (byte)(((pixel & 0xF000) >> 12) * 17);
                    break;
                case "RGBA5":
                comp[0] = (byte)(((pixel & 0xF800) >> 11) / 0x1F * 0xFF);
                comp[1] = (byte)(((pixel & 0x7E0) >> 5) / 0x3F * 0xFF);
                comp[2] = (byte)((pixel & 0x1F) / 0x1F * 0xFF);
                    break;
            }

            return comp;
        }

        private static byte[] ConvertBgraToRgba(byte[] bytes)
        {
            if (bytes == null)
                throw new Exception("Data block returned null. Make sure the parameters and image properties are correct!");

            for (int i = 0; i < bytes.Length; i += 4)
            {
                var temp = bytes[i];
                bytes[i] = bytes[i + 2];
                bytes[i + 2] = temp;
            }
            return bytes;
        }

        private void ReadDX10Header(BinaryDataReader reader)
        {
            DX10header = new DX10Header();
            DX10header.DXGI_Format = reader.ReadEnum<DXGI_FORMAT>(true);
            DX10header.ResourceDim = reader.ReadUInt32();
            DX10header.miscFlag = reader.ReadUInt32();
            DX10header.arrayFlag = reader.ReadUInt32();
            DX10header.miscFlags2 = reader.ReadUInt32();

            ArrayCount = DX10header.arrayFlag;
        }

        public bool SwitchSwizzle = false;
        public bool WiiUSwizzle = false;
        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0, int DepthLevel = 0)
        {
            if (IsAtscFormat(Format))
                SwitchSwizzle = true;

            if (SwitchSwizzle)
                return TegraX1Swizzle.GetImageData(this, bdata, ArrayLevel, MipLevel, DepthLevel);
            else if (WiiUSwizzle)
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
                surf.format = (uint)GX2.ConvertToGx2Format(Format);
                surf.use = (uint)GX2.GX2SurfaceUse.USE_COLOR_BUFFER;
                surf.pitch = 0;
                surf.data = bdata;
                surf.numMips = MipCount;
                surf.mipOffset = new uint[0];
                surf.mipData = bdata;
                surf.tileMode = (uint)GX2.GX2TileMode.MODE_2D_TILED_THIN1;
                surf.swizzle = 0;
                surf.numArray = 1;

                return GX2.Decode(surf, ArrayLevel, MipLevel);
            }

            return GetArrayFaces(this, ArrayCount, DepthLevel)[ArrayLevel].mipmaps[MipLevel];
        }

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            SetArrayLevel(GenerateMipsAndCompress(bitmap, MipCount, Format), ArrayLevel);
        }

        //Todo create actual cube map conversion with Renderable Texture from generic one
        public static TextureCubeMap CreateGLCubeMap(DDS dds)
        {
            TextureCubeMap texture = new TextureCubeMap();
            var cubemap = GetArrayFaces(dds, 6);

            bool Compressed = dds.IsCompressed();

           
            if (Compressed)
            {
                PixelInternalFormat pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                switch (dds.Format)
                {
                    case TEX_FORMAT.BC1_UNORM:
                    case TEX_FORMAT.BC1_UNORM_SRGB:
                        pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                        break;
                    case TEX_FORMAT.BC2_UNORM:
                    case TEX_FORMAT.BC2_UNORM_SRGB:
                        pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                        break;
                    case TEX_FORMAT.BC3_UNORM:
                    case TEX_FORMAT.BC3_UNORM_SRGB:
                        pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                        break;
                    case TEX_FORMAT.BC6H_UF16:
                        pixelInternalFormat = PixelInternalFormat.CompressedRgbBptcUnsignedFloat;
                        break;
                    case TEX_FORMAT.BC6H_SF16:
                        pixelInternalFormat = PixelInternalFormat.CompressedRgbBptcUnsignedFloat;
                        break;
                    default:
                        throw new Exception("Unsupported format! " + dds.Format);
                }

                texture.LoadImageData((int)dds.header.width,
                    (InternalFormat)pixelInternalFormat,
                    cubemap[0].mipmaps,
                    cubemap[1].mipmaps,
                    cubemap[2].mipmaps,
                    cubemap[3].mipmaps, 
                    cubemap[4].mipmaps,
                    cubemap[5].mipmaps);
            }
            else
            {
                PixelInternalFormat pixelInternalFormat = PixelInternalFormat.Rgba;
                PixelType pixelType = PixelType.UnsignedByte;
                PixelFormat pixelFormat = PixelFormat.Rgba;

                switch (dds.Format)
                {
                    case TEX_FORMAT.R32G32B32A32_FLOAT:
                        pixelInternalFormat = PixelInternalFormat.Rgba32f;
                        pixelType = PixelType.Float;
                        break;
                    case TEX_FORMAT.R8G8B8A8_UNORM:
                    case TEX_FORMAT.R8G8B8A8_UNORM_SRGB:
                        pixelInternalFormat = PixelInternalFormat.Rgba;
                        break;
                    default:
                        throw new Exception("Unsupported format! " + dds.Format);
                }

                texture.LoadImageData((int)dds.header.width, new SFGraphics.GLObjects.Textures.TextureFormats.TextureFormatUncompressed(pixelInternalFormat,
                pixelFormat, pixelType),
                cubemap[0].mipmaps[0],
                cubemap[1].mipmaps[0],
                cubemap[2].mipmaps[0],
                cubemap[3].mipmaps[0],
                cubemap[4].mipmaps[0],
                cubemap[5].mipmaps[0]);
            }

            return texture;
        }
        /*       public STGenericTexture ToGenericTexture()
               {
                   STGenericTexture texture = new STGenericTexture();
                   texture.Width = header.width;
                   texture.Height = header.height;
                   texture.Format = GetFormat();
                   bool IsCubemap = false;

                   if (IsCubemap)
                       texture.Surfaces = GetArrayFaces(this, 6);
                   else
                       texture.Surfaces = GetArrayFaces(this, 1);

                   return texture;
               }*/
        public static List<byte[]> GetArrayFacesBytes(byte[] data, int Length)
        {
            int Offset = 0;
            List<byte[]> surfaces = new List<byte[]>();
            for (byte i = 0; i < Length; ++i)
            {
                int size = data.Length / Length;

                surfaces.Add(Utils.SubArray(data, (uint)Offset, (uint)size));

                Offset += size;
            }
            return surfaces;
        }

        public void SetArrayLevel(byte[] data, int ArrayIndex, int DepthIndex = 0)
        {
            uint formatSize = GetBytesPerPixel(Format);

            uint Offset = 0;
            for (byte d = 0; d < Depth; ++d)
            {
                for (byte i = 0; i < ArrayCount; ++i)
                {
                    if (i == ArrayIndex)
                    {
                        Array.Copy(data, 0, bdata, Offset, data.Length);
                    }

                    uint MipWidth = Width, MipHeight = Height;
                    for (int j = 0; j < MipCount; ++j)
                    {
                        MipWidth = (uint)Math.Max(1, Width >> j);
                        MipHeight = (uint)Math.Max(1, Height >> j);

                        uint size = (MipWidth * MipHeight); //Total pixels
                        if (IsCompressed(Format))
                        {
                            size = ((MipWidth + 3) >> 2) * ((MipHeight + 3) >> 2) * formatSize;
                            if (size < formatSize)
                                size = formatSize;
                        }
                        else
                        {
                            size = (uint)(size * GetBytesPerPixel(Format)); //Bytes per pixel
                        }

                        Offset += size;
                    }
                }
            }
        }

        public static List<Surface> GetArrayFaces(STGenericTexture tex, byte[] ImageData, uint Length)
        {
            using (FileReader reader = new FileReader(ImageData))
            {
                var Surfaces = new List<STGenericTexture.Surface>();

                uint formatSize = GetBytesPerPixel(tex.Format);

                uint numDepth = 1;
                if (tex.Depth > 1)
                    numDepth = tex.Depth;

                uint Offset = 0;
                for (byte d = 0; d < numDepth; ++d)
                {
                    for (byte i = 0; i < Length; ++i)
                    {
                        var Surface = new STGenericTexture.Surface();

                        uint MipWidth = tex.Width, MipHeight = tex.Height;
                        for (int j = 0; j < tex.MipCount; ++j)
                        {
                            MipWidth = (uint)Math.Max(1, tex.Width >> j);
                            MipHeight = (uint)Math.Max(1, tex.Height >> j);

                            uint size = (MipWidth * MipHeight); //Total pixels
                            if (IsCompressed(tex.Format))
                            {
                                size = ((MipWidth + 3) >> 2) * ((MipHeight + 3) >> 2) * formatSize;
                                if (size < formatSize)
                                    size = formatSize;
                            }
                            else
                            {
                                size = (uint)(size * GetBytesPerPixel(tex.Format)); //Bytes per pixel
                            }

                            Surface.mipmaps.Add(reader.getSection((int)Offset, (int)size));
                            Offset += size;
                        }
                        Surfaces.Add(Surface);
                    }
                }

                return Surfaces;
            }
        }

        public static List<Surface> GetArrayFaces(DDS dds, uint Length, int DepthLevel = 0)
        {
            using (FileReader reader = new FileReader(dds.bdata))
            {
                var Surfaces = new List<STGenericTexture.Surface>();
              
                uint formatSize = GetBytesPerPixel(dds.Format);

                bool isBlock = dds.IsCompressed();
                if (dds.header.mipmapCount == 0)
                    dds.header.mipmapCount = 1;

                uint Offset = 0;

                if (dds.Depth > 1 && dds.header.mipmapCount > 1)
                {
                    var Surface = new Surface();

                    uint MipWidth = dds.header.width, MipHeight = dds.header.height;
                    for (int j = 0; j < dds.header.mipmapCount; ++j)
                    {
                        MipWidth = (uint)Math.Max(1, dds.header.width >> j);
                        MipHeight = (uint)Math.Max(1, dds.header.height >> j);
                        for (byte d = 0; d < dds.Depth; ++d)
                        {
                            uint size = (MipWidth * MipHeight); //Total pixels
                            if (isBlock)
                            {
                                size = ((MipWidth + 3) >> 2) * ((MipHeight + 3) >> 2) * formatSize;
                                if (size < formatSize)
                                    size = formatSize;
                            }
                            else
                            {
                                size = (uint)(size * (GetBytesPerPixel(dds.Format))); //Bytes per pixel
                            }


                            //Only add mips to the depth level needed
                            if (d == DepthLevel)
                                Surface.mipmaps.Add(reader.getSection((int)Offset, (int)size));

                            Offset += size;

                            //Add the current depth level and only once
                            if (d == DepthLevel && j == 0)
                                Surfaces.Add(Surface);
                        }
                    }
                }
                else
                {
                    for (byte d = 0; d < dds.Depth; ++d)
                    {
                        for (byte i = 0; i < Length; ++i)
                        {
                            var Surface = new STGenericTexture.Surface();

                            uint MipWidth = dds.header.width, MipHeight = dds.header.height;
                            for (int j = 0; j < dds.header.mipmapCount; ++j)
                            {
                                MipWidth = (uint)Math.Max(1, dds.header.width >> j);
                                MipHeight = (uint)Math.Max(1, dds.header.height >> j);

                                uint size = (MipWidth * MipHeight); //Total pixels
                                if (isBlock)
                                {
                                    size = ((MipWidth + 3) >> 2) * ((MipHeight + 3) >> 2) * formatSize;
                                    if (size < formatSize)
                                        size = formatSize;
                                }
                                else
                                {
                                    size = (uint)(size * (GetBytesPerPixel(dds.Format))); //Bytes per pixel
                                }

                                Surface.mipmaps.Add(reader.getSection((int)Offset, (int)size));
                                Offset += size;
                            }

                            if (d == DepthLevel)
                                Surfaces.Add(Surface);
                        }
                    }
                }

                return Surfaces;
            }
        }

        public TEX_FORMAT GetFormat()
        {
            if (DX10header != null)
                return (TEX_FORMAT)DX10header.DXGI_Format;

            switch (header.ddspf.fourCC)
            {
                case FOURCC_DXT1:
                    return TEX_FORMAT.BC1_UNORM;
                case FOURCC_DXT2:
                case FOURCC_DXT3:
                    return TEX_FORMAT.BC2_UNORM;
                case FOURCC_DXT4:
                case FOURCC_DXT5:
                    return TEX_FORMAT.BC3_UNORM;
                case FOURCC_ATI1:
                case FOURCC_BC4U:
                    return TEX_FORMAT.BC4_UNORM;
                case FOURCC_BC4S:
                    return TEX_FORMAT.BC4_SNORM;
                case FOURCC_ATI2:
                case FOURCC_BC5U:
                    return TEX_FORMAT.BC5_UNORM;
                case FOURCC_BC5S:
                    return TEX_FORMAT.BC5_SNORM;

                case FOURCC_RXGB:
                    return TEX_FORMAT.R8G8B8A8_UNORM;
                default:
                    return TEX_FORMAT.R8G8B8A8_UNORM;
            }
        }
        public void SetFlags(DXGI_FORMAT Format, bool UseDX10 = false, bool isCubeMap = false)
        {
            header.flags = (uint)(DDSD.CAPS | DDSD.HEIGHT | DDSD.WIDTH | DDSD.PIXELFORMAT | DDSD.MIPMAPCOUNT | DDSD.LINEARSIZE);
            header.caps = (uint)DDSCAPS.TEXTURE;
            if (header.mipmapCount > 1)
                header.caps |= (uint)(DDSCAPS.COMPLEX | DDSCAPS.MIPMAP);

            if (isCubeMap)
            {
                header.caps2 |= (uint)(DDSCAPS2.CUBEMAP | DDSCAPS2.CUBEMAP_POSITIVEX | DDSCAPS2.CUBEMAP_NEGATIVEX |
                                      DDSCAPS2.CUBEMAP_POSITIVEY | DDSCAPS2.CUBEMAP_NEGATIVEY |
                                      DDSCAPS2.CUBEMAP_POSITIVEZ | DDSCAPS2.CUBEMAP_NEGATIVEZ);
            }

            if (UseDX10)
            {
                header.ddspf.flags = (uint)DDPF.FOURCC;
                header.ddspf.fourCC = FOURCC_DX10;
                if (DX10header == null)
                    DX10header = new DX10Header();

                IsDX10 = true;
                DX10header.DXGI_Format = Format;
                if (isCubeMap)
                {
                    DX10header.arrayFlag = (ArrayCount / 6);
                    DX10header.miscFlag = 0x4;
                }
                return;
            }

            switch (Format)
            {
                case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM:
                case DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM_SRGB:
                    header.ddspf.flags = (uint)(DDPF.RGB | DDPF.ALPHAPIXELS);
                    header.ddspf.RGBBitCount = 0x8 * 4;
                    header.ddspf.RBitMask = 0x000000FF;
                    header.ddspf.GBitMask = 0x0000FF00;
                    header.ddspf.BBitMask = 0x00FF0000;
                    header.ddspf.ABitMask = 0xFF000000;
                    pixelInternalFormat = PixelInternalFormat.SrgbAlpha;
                    pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_R8G8_UNORM:
                    header.ddspf.flags = (uint)(DDPF.RGB | DDPF.ALPHAPIXELS);
                    header.ddspf.RGBBitCount = 24;
                    header.ddspf.RBitMask = (uint)R8G8B8_MASKS[0];
                    header.ddspf.GBitMask = (uint)R8G8B8_MASKS[1];
                    header.ddspf.BBitMask = (uint)R8G8B8_MASKS[2];
                    header.ddspf.ABitMask = (uint)R8G8B8_MASKS[3];
                    pixelInternalFormat = PixelInternalFormat.SrgbAlpha;
                    pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
                case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
                    header.ddspf.flags = (uint)DDPF.FOURCC;
                    header.ddspf.fourCC = FOURCC_DXT1;
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB:
                case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
                    header.ddspf.flags = (uint)DDPF.FOURCC;
                    header.ddspf.fourCC = FOURCC_DXT3;
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
                case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
                    header.ddspf.flags = (uint)DDPF.FOURCC;
                    header.ddspf.fourCC = FOURCC_DXT5;
                    pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
                    header.ddspf.flags = (uint)DDPF.FOURCC;
                    header.ddspf.fourCC = FOURCC_BC4U;
                    pixelInternalFormat = PixelInternalFormat.CompressedRedRgtc1;
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
                    header.ddspf.flags = (uint)DDPF.FOURCC;
                    header.ddspf.fourCC = FOURCC_BC4S;
                    pixelInternalFormat = PixelInternalFormat.CompressedSignedRedRgtc1;
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
                    header.ddspf.flags = (uint)DDPF.FOURCC;
                    header.ddspf.fourCC = FOURCC_BC5U;
                    pixelInternalFormat = PixelInternalFormat.CompressedRgRgtc2;
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
                    header.ddspf.flags = (uint)DDPF.FOURCC;
                    header.ddspf.fourCC = FOURCC_BC5S;
                    pixelInternalFormat = PixelInternalFormat.CompressedSignedRgRgtc2;
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16:
                case DXGI_FORMAT.DXGI_FORMAT_BC6H_SF16:
                case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
                    header.ddspf.flags = (uint)DDPF.FOURCC;
                    header.ddspf.fourCC = FOURCC_DX10;
                    if (DX10header == null)
                        DX10header = new DX10Header();

                    IsDX10 = true;
                    DX10header.DXGI_Format = Format;
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
                    header.ddspf.flags = (uint)DDPF.FOURCC;
                    header.ddspf.fourCC = FOURCC_DX10;
                    if (DX10header == null)
                        DX10header = new DX10Header();

                    IsDX10 = true;
                    DX10header.DXGI_Format = DDS.DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM;
                    break;
            }
        }
        public bool IsCompressed()
        {
            if (header == null)
                return false;

            if (DX10header != null)
            {
                switch (DX10header.DXGI_Format)
                {
                    case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
                    case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
                    case DXGI_FORMAT.DXGI_FORMAT_BC1_TYPELESS:
                    case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB:
                    case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
                    case DXGI_FORMAT.DXGI_FORMAT_BC2_TYPELESS:
                    case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
                    case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
                    case DXGI_FORMAT.DXGI_FORMAT_BC3_TYPELESS:
                    case DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
                    case DXGI_FORMAT.DXGI_FORMAT_BC4_TYPELESS:
                    case DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
                    case DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
                    case DXGI_FORMAT.DXGI_FORMAT_BC5_TYPELESS:
                    case DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
                    case DXGI_FORMAT.DXGI_FORMAT_BC6H_TYPELESS:
                    case DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16:
                    case DXGI_FORMAT.DXGI_FORMAT_BC6H_SF16:
                    case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
                    case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
                    case DXGI_FORMAT.DXGI_FORMAT_BC7_TYPELESS:
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                switch (header.ddspf.fourCC)
                {
                    case FOURCC_DXT1:
                    case FOURCC_DXT2:
                    case FOURCC_DXT3:
                    case FOURCC_DXT4:
                    case FOURCC_DXT5:
                    case FOURCC_ATI1:
                    case FOURCC_BC4U:
                    case FOURCC_ATI2:
                    case FOURCC_BC5U:
                    case FOURCC_BC5S:
                        return true;
                    default:
                        return false;
                }
            }
        }

        public void Save(DDS dds, string FileName, List<Surface> data = null)
        {
            Save(dds, new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.Write), data);
        }

        public void Save(DDS dds, Stream stream, List<Surface> data = null)
        {
            FileWriter writer = new FileWriter(stream);
            var header = dds.header;
            writer.Write(Encoding.ASCII.GetBytes("DDS "));
            writer.Write(header.size);
            writer.Write(header.flags);
            writer.Write(header.height);
            writer.Write(header.width);
            writer.Write(header.pitchOrLinearSize);
            writer.Write(header.depth);
            writer.Write(header.mipmapCount);
            for (int i = 0; i < 11; ++i)
                writer.Write(header.reserved1[i]);

            writer.Write(header.ddspf.size);
            writer.Write(header.ddspf.flags);
            writer.Write(header.ddspf.fourCC);
            writer.Write(header.ddspf.RGBBitCount);
            writer.Write(header.ddspf.RBitMask);
            writer.Write(header.ddspf.GBitMask);
            writer.Write(header.ddspf.BBitMask);
            writer.Write(header.ddspf.ABitMask);
            writer.Write(header.caps);
            writer.Write(header.caps2);
            writer.Write(header.caps3);
            writer.Write(header.caps4);
            writer.Write(header.reserved2);

            if (IsDX10)
            {
                WriteDX10Header(writer);
            }

            if (data != null)
            {
                foreach (var surface in data)
                {
                    writer.Write(Utils.CombineByteArray(surface.mipmaps.ToArray()));
                }
            }
            else
            {
                writer.Write(bdata);
            }

            writer.Flush();
            writer.Close();
            writer.Dispose();
        }

        private void WriteDX10Header(BinaryDataWriter writer)
        {
            if (DX10header == null)
                DX10header = new DX10Header();

            writer.Write((uint)DX10header.DXGI_Format);
            writer.Write(DX10header.ResourceDim);
            writer.Write(DX10header.miscFlag);
            writer.Write(DX10header.arrayFlag);
            writer.Write(DX10header.miscFlags2);
        }

        public override void Replace(string FileName)
        {
            GenericTextureImporterList importer = new GenericTextureImporterList(SupportedFormats);
            GenericTextureImporterSettings settings = new GenericTextureImporterSettings();

            if (Utils.GetExtension(FileName) == ".dds" ||
                Utils.GetExtension(FileName) == ".dds2")
            {
                settings.LoadDDS(FileName);
                importer.LoadSettings(new List<GenericTextureImporterSettings>() { settings, });
                ApplySettings(settings);
                UpdateEditor();
            }
            else
            {
                settings.LoadBitMap(FileName);
                importer.LoadSettings(new List<GenericTextureImporterSettings>() { settings, });

                if (importer.ShowDialog() == DialogResult.OK)
                {
                    if (settings.GenerateMipmaps && !settings.IsFinishedCompressing)
                    {
                        settings.DataBlockOutput.Clear();
                        settings.DataBlockOutput.Add(settings.GenerateMips(importer.CompressionMode, importer.MultiThreading));
                    }

                    ApplySettings(settings);
                    UpdateEditor();
                }
            }
        }

        public override void OnClick(TreeView treeView) {
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

            editor.LoadProperties(GenericProperties);
            editor.LoadImage(this);
        }

        public override UserControl GetEditor()
        {
            ImageEditorBase editor = new ImageEditorBase();
            editor.Text = Text;
            editor.Dock = DockStyle.Fill;
            return editor;
        }

        public override void FillEditor(UserControl control)
        {
            ((ImageEditorBase)control).LoadProperties(GenericProperties);
            ((ImageEditorBase)control).LoadImage(this);
        }

        private void ApplySettings(GenericTextureImporterSettings settings)
        {
            //Combine all arrays
            this.bdata = Utils.CombineByteArray( settings.DataBlockOutput.ToArray());
            this.Width = settings.TexWidth;
            this.Height = settings.TexHeight;
            this.Format = settings.Format;
            this.MipCount = settings.MipCount;
            this.Depth = settings.Depth;
            this.ArrayCount = (uint)settings.DataBlockOutput.Count;

            this.header.width = Width;
            this.header.height = Height;
            this.header.depth = Depth;
            this.header.mipmapCount = (uint)MipCount;
            this.header.pitchOrLinearSize = (uint)bdata.Length / ArrayCount;

            if (this.ArrayCount > 0) //Use DX10 format for array surfaces as it can do custom amounts
                this.SetFlags((DDS.DXGI_FORMAT)Format, true);
            else
                this.SetFlags((DDS.DXGI_FORMAT)Format);
        }

        public static byte[] CompressBC1Block(byte[] data, int Width, int Height)
        {
            byte[] image = new byte[0];

            return image;
        }

        public static void ToRGBA(byte[] data, int Width, int Height, int bpp, int compSel)
        {
            int Size = Width * Height * 4;

            byte[] result = new byte[Size];

            for (int Y = 0; Y < Height; Y++)
            {
                for (int X = 0; X < Width; X++)
                {
                    int pos = (Y * Width + X) * bpp;
                    int pos_ = (Y * Width + X) * 4;

                    int pixel = 0;
                }
            }
        }
    }
}