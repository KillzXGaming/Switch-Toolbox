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
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using SFGraphics.GLObjects.Textures;
using OpenTK.Graphics.OpenGL;

namespace Switch_Toolbox.Library
{
    //Data from https://github.com/jam1garner/Smash-Forge/blob/master/Smash%20Forge/Filetypes/Textures/DDS.cs
    public class DDS : STGenericTexture, IEditor<ImageEditorForm>, IFileFormat
    {
        public override TEX_FORMAT[] SupportedFormats
        {
            get
            {
                return Enum.GetValues(typeof(TEX_FORMAT)).Cast<TEX_FORMAT>().ToArray();
            }
        }

        public override bool CanEdit { get; set; } = false;

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
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "DDS ");
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

            FileReader reader = new FileReader(stream);
            reader.ByteOrder = ByteOrder.LittleEndian;
            Load(reader);
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
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
                case 0x00000000: //RGBA
                    return false;
                case FOURCC_DXT1:
                case FOURCC_DXT2:
                case FOURCC_DXT3:
                case FOURCC_DXT4:
                case FOURCC_DXT5:
                case FOURCC_ATI1:
                case FOURCC_ATI2:
                case FOURCC_BC5U:
                    return true;
                default:
                    return false;
            }
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
        public uint imageSize;

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

        public ImageEditorForm OpenForm()
        {
            ImageEditorForm form = new ImageEditorForm();
            form.editorBase.Text = Text;
            form.editorBase.Dock = DockStyle.Fill;
            form.editorBase.LoadImage(this);
            form.editorBase.LoadProperties(GenericProperties);
            return form;
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
            bdata = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));

            reader.Dispose();
            reader.Close();

            ArrayCount = 1;
            MipCount = header.mipmapCount;
            Width = header.width;
            Height = header.height;
            Format = GetFormat();
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

        public bool Swizzle = false;
        public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
        {
            if (Swizzle)
                return TegraX1Swizzle.GetImageData(this, bdata, ArrayLevel, MipLevel);

            return GetArrayFaces(this, 1)[ArrayLevel].mipmaps[MipLevel];
        }

        public override void SetImageData(Bitmap bitmap, int ArrayLevel)
        {
            throw new NotImplementedException("Cannot set image data! Operation not implemented!");
        }

        public static TextureCubeMap CreateGLCubeMap(DDS dds)
        {
            TextureCubeMap texture = new TextureCubeMap();
            var cubemap = GetArrayFaces(dds, 6);

            bool Compressed = dds.IsCompressed();

            if (Compressed)
            {
                texture.LoadImageData((int)dds.header.width,
                    (InternalFormat)PixelInternalFormat.CompressedRgbaS3tcDxt1Ext,
                    cubemap[0].mipmaps,
                    cubemap[1].mipmaps,
                    cubemap[2].mipmaps,
                    cubemap[3].mipmaps,
                    cubemap[4].mipmaps,
                    cubemap[5].mipmaps);
            }
            else
            {
                texture.LoadImageData((int)dds.header.width, new SFGraphics.GLObjects.Textures.TextureFormats.TextureFormatUncompressed(PixelInternalFormat.Rgba,
                OpenTK.Graphics.OpenGL.PixelFormat.Rgba, OpenTK.Graphics.OpenGL.PixelType.UnsignedByte),
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
        public static List<byte[]> GetArrayFacesBytes(DDS dds, int Length)
        {
            int Offset = 0;
            List<byte[]> surfaces = new List<byte[]>();
            for (byte i = 0; i < Length; ++i)
            {
                int size = dds.bdata.Length / Length;

                surfaces.Add(Utils.SubArray(dds.bdata, (uint)Offset, (uint)size));

                Offset += size;
            }
            return surfaces;
        }

        public static List<Surface> GetArrayFaces(DDS dds, uint Length)
        {
            using (FileReader reader = new FileReader(dds.bdata))
            {
                var Surfaces = new List<STGenericTexture.Surface>();

                /*               Rendering.RenderableTex tex = new Rendering.RenderableTex();
                                tex.height = (int)dds.header.height;
                                tex.width = (int)dds.header.width;
                                byte surfaceCount = 1;
                                bool isCubemap = (dds.header.caps2 & (uint)DDSCAPS2.CUBEMAP) == (uint)DDSCAPS2.CUBEMAP;
                                if (isCubemap)
                                {
                                    if ((dds.header.caps2 & (uint)DDSCAPS2.CUBEMAP_ALLFACES) == (uint)DDSCAPS2.CUBEMAP_ALLFACES)
                                        surfaceCount = 6;
                                    else
                                        throw new NotImplementedException($"Unsupported cubemap face amount for texture. Six faces are required.");
                                }

                                bool isBlock = true;

                                switch (dds.header.ddspf.fourCC)
                                {
                                    case 0x00000000: //RGBA
                                        isBlock = false;
                                        tex.pixelInternalFormat = PixelInternalFormat.Rgba;
                                        tex.pixelFormat = OpenTK.Graphics.OpenGL.PixelFormat.Rgba;
                                        break;
                                    case 0x31545844: //DXT1
                                        tex.pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                                        break;
                                    case 0x33545844: //DXT3
                                        tex.pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                                        break;
                                    case 0x35545844: //DXT5
                                        tex.pixelInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                                        break;
                                    case 0x31495441: //ATI1
                                    case 0x55344342: //BC4U
                                        tex.pixelInternalFormat = PixelInternalFormat.CompressedRedRgtc1;
                                        break;
                                    case 0x32495441: //ATI2
                                    case 0x55354342: //BC5U
                                        tex.pixelInternalFormat = PixelInternalFormat.CompressedRgRgtc2;
                                        break;
                                    default:
                                        MessageBox.Show("Unsupported DDS format - 0x" + dds.header.ddspf.fourCC.ToString("x"));
                                        break;
                                }

                                uint formatSize = getFormatSize(dds.header.ddspf.fourCC);

                                FileReader d = new FileReader(dds.bdata);
                                if (dds.header.mipmapCount == 0)
                                    dds.header.mipmapCount = 1;

                                List<Surface> surfaces = new List<Surface>();

                                uint off = 0;
                                for (byte i = 0; i < surfaceCount; ++i)
                                {
                                    Surface surface = new Surface();
                                    uint w = dds.header.width, h = dds.header.height;
                                    for (int j = 0; j < dds.header.mipmapCount; ++j)
                                    {
                                        //If texture is DXT5 and isn't square, limit the mipmaps to an amount such that width and height are each always >= 4
                                        if (tex.pixelInternalFormat == PixelInternalFormat.CompressedRgbaS3tcDxt5Ext && tex.width != tex.height && (w < 4 || h < 4))
                                            break;

                                        uint s = (w * h); //Total pixels
                                        if (isBlock)
                                        {
                                            s = (uint)(s * ((float)formatSize / 0x10)); //Bytes per 16 pixels
                                            if (s < formatSize) //Make sure it's at least one block
                                                s = formatSize;
                                        }
                                        else
                                        {
                                            s = (uint)(s * (formatSize)); //Bytes per pixel
                                        }

                                        w /= 2;
                                        h /= 2;
                                        surface.mipmaps.Add(d.getSection((int)off, (int)s));
                                        off += s;
                                    }
                                    surfaces.Add(surface);
                                }*/


                uint formatSize = getFormatSize(dds.header.ddspf.fourCC);
                bool isBlock = getFormatBlock(dds.header.ddspf.fourCC);
                if (dds.header.mipmapCount == 0)
                    dds.header.mipmapCount = 1;

                uint Offset = 0;
                for (byte i = 0; i < Length; ++i)
                {
                    var Surface = new STGenericTexture.Surface();

                    uint MipWidth = dds.header.width, MipHeight = dds.header.height;
                    for (int j = 0; j < dds.header.mipmapCount; ++j)
                    {
                        uint size = (MipWidth * MipHeight); //Total pixels
                        if (isBlock)
                        {
                            size = (uint)(size * ((float)formatSize / 0x10)); //Bytes per 16 pixels
                            if (size < formatSize) //Make sure it's at least one block
                                size = formatSize;
                        }
                        else
                        {
                            size = (uint)(size * (GetBytesPerPixel(dds.Format))); //Bytes per pixel
                        }

                        MipWidth /= 2;
                        MipHeight /= 2;
                        Surface.mipmaps.Add(reader.getSection((int)Offset, (int)size));
                        Offset += size;
                    }
                    Surfaces.Add(Surface);
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
                    return TEX_FORMAT.BC2_UNORM;
                case FOURCC_DXT3:
                    return TEX_FORMAT.BC2_UNORM;
                case FOURCC_DXT4:
                    return TEX_FORMAT.BC3_UNORM;
                case FOURCC_DXT5:
                    return TEX_FORMAT.BC3_UNORM;
                case FOURCC_ATI1:
                    return TEX_FORMAT.BC4_UNORM;
                case FOURCC_BC4U:
                    return TEX_FORMAT.BC4_UNORM;
                case FOURCC_ATI2:
                    return TEX_FORMAT.BC5_UNORM;
                case FOURCC_BC5U:
                    return TEX_FORMAT.BC5_UNORM;
                case FOURCC_RXGB:
                    return TEX_FORMAT.R8G8B8A8_UNORM;
                default:
                    return TEX_FORMAT.R8G8B8A8_UNORM;
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
                case DXGI_FORMAT.DXGI_FORMAT_BC4_SNORM:
                    pixelInternalFormat = PixelInternalFormat.CompressedRedRgtc1;
                    break;
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