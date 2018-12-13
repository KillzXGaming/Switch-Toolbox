using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.BinaryData;
using System.IO;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using SFGraphics.GLObjects.Textures;
using OpenTK.Graphics.OpenGL;

namespace Switch_Toolbox.Library
{
    //Data from https://github.com/jam1garner/Smash-Forge/blob/master/Smash%20Forge/Filetypes/Textures/DDS.cs
    public class DDS
    {
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

        public static uint getFormatSize(uint fourCC)
        {
            switch (fourCC)
            {
                case 0x00000000: //RGBA
                    return 0x4;
                case FOURCC_DXT1: 
                    return 0x8;
                case FOURCC_DXT2: 
                    return 0x10;
                case FOURCC_DXT3:
                    return 0x10;
                case FOURCC_DXT4:
                    return 0x10;
                case FOURCC_DXT5:
                    return 0x10;
                case FOURCC_ATI1:
                case FOURCC_BC4U:
                    return 0x8;
                case FOURCC_ATI2: 
                case FOURCC_BC5U:
                    return 0x10;
                default:
                    return 0;
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
        public             uint imageSize;

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
            DXGI_FORMAT_FORCE_UINT = 0xFFFFFFFF
        }
        public DDS()
        {

        }
        public DDS(byte[] data)
        {
            FileReader reader = new FileReader(new MemoryStream(data));
            Load(reader);
        }
        public DDS(string FileName)
        {
            FileReader reader = new FileReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read));

            Load(reader);
        }
        public void Load(BinaryDataReader reader)
        {
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

            int DX10HeaderSize = 0;
            if (header.ddspf.fourCC == FOURCC_DX10)
            {
                IsDX10 = true;

                DX10HeaderSize = 20;
                ReadDX10Header(reader);
            }

            if (IsCompressed())
            {
                imageSize = ((header.width + 3) >> 2) * ((header.height + 3) >> 2) * getFormatSize(header.ddspf.fourCC);
            }
            else
                imageSize = header.width * header.height * getFormatSize(header.ddspf.fourCC);

            reader.TemporarySeek((int)(4 + header.size + DX10HeaderSize), SeekOrigin.Begin);
            bdata = reader.ReadBytes((int)(reader.BaseStream.Length - reader.Position));

            reader.Dispose();
            reader.Close();
        }
        private void ReadDX10Header(BinaryDataReader reader)
        {
            DX10header = new DX10Header();
            DX10header.DXGI_Format = reader.ReadEnum<DXGI_FORMAT>(true);
            DX10header.ResourceDim = reader.ReadUInt32();
            DX10header.miscFlag = reader.ReadUInt32();
            DX10header.arrayFlag = reader.ReadUInt32();
            DX10header.miscFlags2 = reader.ReadUInt32();
        }
        public static TextureCubeMap CreateGLCubeMap(DDS dds)
        {
            TextureCubeMap texture = new TextureCubeMap();
            List<byte[]> cubemap = GetArrayFaces(dds.bdata, 6);
            texture.LoadImageData((int)dds.header.width,new SFGraphics.GLObjects.Textures.TextureFormats.TextureFormatUncompressed(PixelInternalFormat.Rgba,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, OpenTK.Graphics.OpenGL.PixelType.Float), cubemap[0],
                cubemap[1], cubemap[2], cubemap[3], cubemap[4], cubemap[5]);
            return texture;
        }
        public static List<byte[]> GetArrayFaces(byte[] data, uint Length)
        {
            using (FileReader reader = new FileReader(data))
            {
                List<byte[]> array = new List<byte[]>();
                for (int i = 0; i < Length; i++)
                    array.Add(reader.ReadBytes(data.Length / (int)Length));
                
                return array;
            }
        }
        public static DXGI_FORMAT GetDXGI_Format(TEX_FORMAT Format, TEX_FORMAT_TYPE type)
        {
            DXGI_FORMAT format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN;

            string formatSTR = GetFormatString(Format);
            string typeSTR = GetFormatTypeString(type, Format);

            Enum.TryParse(formatSTR + typeSTR, out format);

            return format;
        }
        public void SetFlags(TEX_FORMAT Format, TEX_FORMAT_TYPE type)
        {
            SetFlags(GetDXGI_Format(Format, type));
        }
        //Get format without type
        private static string GetFormatString(TEX_FORMAT Format)
        {
            switch (Format)
            {
                case TEX_FORMAT.BC1: return "DXGI_FORMAT_BC1_";
                case TEX_FORMAT.BC2: return "DXGI_FORMAT_BC2_";
                case TEX_FORMAT.BC3: return "DXGI_FORMAT_BC3_";
                case TEX_FORMAT.BC4: return "DXGI_FORMAT_BC4_";
                case TEX_FORMAT.BC5: return "DXGI_FORMAT_BC5_";
                case TEX_FORMAT.BC6: return "DXGI_FORMAT_BC6_";
                case TEX_FORMAT.BC7: return "DXGI_FORMAT_BC7_";

                case TEX_FORMAT.A1_B5_G5_R5: return "DXGI_FORMAT_B5G5R5A1_";
                case TEX_FORMAT.A8: return "DXGI_FORMAT_A8_";
                case TEX_FORMAT.A8P8: return "DXGI_FORMAT_A8P8";
                case TEX_FORMAT.AI44: return "DXGI_FORMAT_AI44";
                case TEX_FORMAT.AYUV: return "DXGI_FORMAT_AYUV";
                case TEX_FORMAT.B5_G6_R5: return "DXGI_FORMAT_B5G6R5_";
                case TEX_FORMAT.D16: return "DXGI_FORMAT_D16_";
                case TEX_FORMAT.D32: return "DXGI_FORMAT_D32_";
                case TEX_FORMAT.D32_S8_X24: return "DXGI_FORMAT_D32_FLOAT_S8X24_";
                case TEX_FORMAT.R1: return "DXGI_FORMAT_R1_";
                case TEX_FORMAT.R16: return "DXGI_FORMAT_R16_";
                case TEX_FORMAT.R16_G16: return "DXGI_FORMAT_R16G16_";
                case TEX_FORMAT.R10_G10_B10_A2: return "DXGI_FORMAT_R10G10B10A2_";
                case TEX_FORMAT.R11_G11_B10: return "DXGI_FORMAT_R11G11B10_";
                case TEX_FORMAT.R16_G16_B16_A16: return "DXGI_FORMAT_R16G16B16A16_";
                case TEX_FORMAT.R24_G8: return "DXGI_FORMAT_R24G8_";
                case TEX_FORMAT.R24_X8: return "DXGI_FORMAT_R24_UNORM_X8_";
                case TEX_FORMAT.R32: return "DXGI_FORMAT_R32_";
                case TEX_FORMAT.R32_G32: return "DXGI_FORMAT_R32G32_";
                case TEX_FORMAT.R32_G32_B32: return "DXGI_FORMAT_R32G32B32_";
                case TEX_FORMAT.R32_G32_B32_A32: return "DXGI_FORMAT_R32G32B32A32_";
                case TEX_FORMAT.R4_G4_B4_A4: return "DXGI_FORMAT_B4G4R4A4_";
                case TEX_FORMAT.R5_G5_B5_A1: return "DXGI_FORMAT_B5G5R5A1_";
                case TEX_FORMAT.R8: return "DXGI_FORMAT_R8_";
                case TEX_FORMAT.R8G8: return "DXGI_FORMAT_R8G8_";
                case TEX_FORMAT.R8_G8_B8_A8: return "DXGI_FORMAT_R8G8B8A8_";
                case TEX_FORMAT.R8_G8_B8_G8: return "DXGI_FORMAT_R8G8_B8G8_";
                case TEX_FORMAT.R9_G9B9E5_SHAREDEXP: return "DXGI_FORMAT_R9G9B9E5_SHAREDEXP";
                default:
                    throw new Exception($"Format not supported! {Format}");
            }
        }
        //Get only type
        private static string GetFormatTypeString(TEX_FORMAT_TYPE type, TEX_FORMAT format)
        {
            switch (type)
            {
                case TEX_FORMAT_TYPE.FLOAT:
                    if (format == TEX_FORMAT.BC6)
                        return "SF16";
                    else
                        return "FLOAT";
                case TEX_FORMAT_TYPE.UFLOAT:
                    if (format == TEX_FORMAT.BC6)
                        return "UF16";
                    else
                        return "UFLOAT";
                case TEX_FORMAT_TYPE.SINT:
                    return "SINT";
                case TEX_FORMAT_TYPE.UINT:
                    return "UINT";
                case TEX_FORMAT_TYPE.SNORM:
                    return "SNORM";
                case TEX_FORMAT_TYPE.UNORM:
                    return "UNORM";
                case TEX_FORMAT_TYPE.TYPELESS:
                    return "TYPELESS";
                case TEX_FORMAT_TYPE.SRGB:
                    return "UNORM_SRGB";
                default:
                    return "";
            }
        }

        public void SetFlags(DXGI_FORMAT Format)
        {
            header.flags = (uint)(DDSD.CAPS | DDSD.HEIGHT | DDSD.WIDTH | DDSD.PIXELFORMAT | DDSD.MIPMAPCOUNT | DDSD.LINEARSIZE);
            header.caps = (uint)DDSCAPS.TEXTURE;
            if (header.mipmapCount > 1)
                header.caps |= (uint)(DDSCAPS.COMPLEX | DDSCAPS.MIPMAP);

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
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM_SRGB:
                case DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM:
                    header.ddspf.flags = (uint)DDPF.FOURCC;
                    header.ddspf.fourCC = FOURCC_DXT1;
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM_SRGB:
                case DXGI_FORMAT.DXGI_FORMAT_BC2_UNORM:
                    header.ddspf.flags = (uint)DDPF.FOURCC;
                    header.ddspf.fourCC = FOURCC_DXT3;
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM_SRGB:
                case DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM:
                    header.ddspf.flags = (uint)DDPF.FOURCC;
                    header.ddspf.fourCC = FOURCC_DXT5;
                    break;
                case DXGI_FORMAT.DXGI_FORMAT_BC4_UNORM:
                case DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
                case DXGI_FORMAT.DXGI_FORMAT_BC5_UNORM:
                case DXGI_FORMAT.DXGI_FORMAT_BC5_SNORM:
                case DXGI_FORMAT.DXGI_FORMAT_BC6H_UF16:
                case DXGI_FORMAT.DXGI_FORMAT_BC6H_SF16:
                case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM:
                case DXGI_FORMAT.DXGI_FORMAT_BC7_UNORM_SRGB:
                    header.ddspf.flags = (uint)DDPF.FOURCC;
                    header.ddspf.fourCC = FOURCC_DX10;
                    if (DX10header == null)
                        DX10header = new DX10Header();

                    IsDX10 = true;
                    DX10header.DXGI_Format = Format;
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
                        return true;
                    default:
                        return false;
                }
            }
        }
        public Tuple<TEX_FORMAT, TEX_FORMAT_TYPE> GetFormat()
        {
            TEX_FORMAT format = TEX_FORMAT.UNKNOWN;
            TEX_FORMAT_TYPE type = TEX_FORMAT_TYPE.UNORM;

            if (DX10header != null)
            {
                string DXGIFormatSTR = DX10header.DXGI_Format.ToString();

                //Set the type. 
                if (DXGIFormatSTR.Contains("SRGB"))
                    type = TEX_FORMAT_TYPE.SRGB;
                else if (DXGIFormatSTR.Contains("SNORM"))
                    type = TEX_FORMAT_TYPE.SNORM;
                else if (DXGIFormatSTR.Contains("UNORM"))
                    type = TEX_FORMAT_TYPE.UNORM;
                else if (DXGIFormatSTR.Contains("UF16"))
                    type = TEX_FORMAT_TYPE.UFLOAT;
                else if (DXGIFormatSTR.Contains("SF16"))
                    type = TEX_FORMAT_TYPE.FLOAT;
                else if (DXGIFormatSTR.Contains("FLOAT"))
                    type = TEX_FORMAT_TYPE.FLOAT;
                else if (DXGIFormatSTR.Contains("UFLOAT"))
                    type = TEX_FORMAT_TYPE.UFLOAT;
                else if (DXGIFormatSTR.Contains("TYPELESS"))
                    type = TEX_FORMAT_TYPE.TYPELESS;

                //Set the format. 
                if (DXGIFormatSTR.Contains("DXGI_FORMAT_BC1"))
                    format = TEX_FORMAT.BC1;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_BC2"))
                    format = TEX_FORMAT.BC2;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_BC3"))
                    format = TEX_FORMAT.BC3;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_BC3"))
                    format = TEX_FORMAT.BC3;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_BC4"))
                    format = TEX_FORMAT.BC4;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_BC4"))
                    format = TEX_FORMAT.BC4;
                else if(DXGIFormatSTR.Contains("DXGI_FORMAT_BC5"))
                    format = TEX_FORMAT.BC5;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_BC6"))
                    format = TEX_FORMAT.BC6;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_BC7"))
                    format = TEX_FORMAT.BC7;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_R8G8B8A8"))
                    format = TEX_FORMAT.R8_G8_B8_A8;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_R16"))
                    format = TEX_FORMAT.R16;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_R8G8"))
                    format = TEX_FORMAT.R8G8;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_A8"))
                    format = TEX_FORMAT.A8;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_A8"))
                    format = TEX_FORMAT.A8;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_R1"))
                    format = TEX_FORMAT.R1;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_R16G16"))
                    format = TEX_FORMAT.R16_G16;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_G8R8_G8B8"))
                    format = TEX_FORMAT.G8_R8_G8_B8;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_D32_FLOAT_S8X24"))
                    format = TEX_FORMAT.D32_S8_X24;
                else if (DXGIFormatSTR.Contains("DXGI_FORMAT_D32"))
                    format = TEX_FORMAT.D32;
            }
            switch (header.ddspf.fourCC)
            {
                case FOURCC_DXT1:
                    format = TEX_FORMAT.BC1; break;
                case FOURCC_DXT2:
                    format = TEX_FORMAT.BC2; break;
                case FOURCC_DXT3:
                    format = TEX_FORMAT.BC2; break;
                case FOURCC_DXT4:
                    format = TEX_FORMAT.BC3; break;
                case FOURCC_DXT5:
                    format = TEX_FORMAT.BC3; break;
                case FOURCC_ATI1:
                    format = TEX_FORMAT.BC4; break;
                case FOURCC_BC4U:
                    format = TEX_FORMAT.BC4; break;
                case FOURCC_ATI2:
                    format = TEX_FORMAT.BC5; break;
                case FOURCC_BC5U:
                    format = TEX_FORMAT.BC5; break;
                default:
                    format = TEX_FORMAT.UNKNOWN; break;
            }

            return Tuple.Create(format, type);
        }
        public void Save(DDS dds, string FileName, List<STGenericTexture.Surface> data = null)
        {
            FileWriter writer = new FileWriter(new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.Write));
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
                writer.Write(data[0].mipmaps[0]);
            }
            else
            {
                writer.Write(bdata);
            }

            writer.Flush();
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
