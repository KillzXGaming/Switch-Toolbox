using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.IO;

namespace Switch_Toolbox.Library
{
    public class STGenericTexture : TreeNodeCustom
    {
        /// <summary>
        /// Stores the <see cref="Surface"/> instances for arrays of images. Cubemaps use 6 surfaces.
        /// </summary>
        public List<Surface> surfaces = new List<Surface>();

        /// <summary>
        /// The total amount of surfaces for the texture.
        /// </summary>
        public int ArrayCount
        {
            get
            {
                return surfaces.Count;
            }
        }

        /// <summary>
        /// The width of the image in pixels
        /// </summary>
        public uint Width;

        /// <summary>
        /// The height of the image in pixels
        /// </summary>
        public uint Height;

        /// <summary>
        /// The <see cref="TEX_FORMAT"/> Format of the image. 
        /// </summary>
        public TEX_FORMAT Format;

        /// <summary>
        /// The total amount of mip maps for the texture.
        /// </summary>
        public uint MipmapCount;

        /// <summary>
        /// A Surface contains mip levels of compressed/uncompressed texture data
        /// </summary>
        public class Surface
        {
            public List<byte[]> mipmaps = new List<byte[]>();
        }

        /// <summary>
        /// Gets a <see cref="Bitmap"/> given an array and mip index.
        /// </summary>
        /// <param name="ArrayIndex">The index of the surface/array. Cubemaps will have 6</param>
        /// <param name="MipLevel">The index of the mip level.</param>
        /// <returns></returns>
        public Bitmap GetBitmap(int ArrayIndex = 0, int MipLevel = 0)
        {
            if (surfaces.Count == 0)
                throw new Exception($"Surfaces are empty on texture {Text}! Failed to get bitmap!");

            return BitmapExtension.GetBitmap(DecodeBlock(surfaces[ArrayIndex].mipmaps[MipLevel], Height, Width, Format),
                (int)Width, (int)Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }
        /// <summary>
        /// Decodes a byte array of image data given the source image in bytes, width, height, and DXGI format.
        /// </summary>
        /// <param name="byte[]">The byte array of the image</param>
        /// <param name="Width">The width of the image in pixels.</param>
        /// <param name="Height">The height of the image in pixels.</param>
        /// <param name=" DDS.DXGI_FORMAT">The image format.</param>
        /// <returns>Returns a byte array of decoded data. </returns>
        public static byte[] DecodeBlock(byte[] data, uint Width, uint Height, TEX_FORMAT Format)
        {
            if (data.Length <= 0)
                throw new Exception($"Data is empty!");
            if (Width <= 0)
                throw new Exception($"Invalid width size {Width}!");
            if (Height <= 0)
                throw new Exception($"Invalid height size {Height}!");

            if (Format == TEX_FORMAT.BC5_SNORM)
                return ConvertBgraToRgba(DDSCompressor.DecompressBC5(data, (int)Width, (int)Height, true, true));

            if (IsCompressed(Format))
                return ConvertBgraToRgba(DDSCompressor.DecompressBlock(data, (int)Width, (int)Height, (DDS.DXGI_FORMAT)Format));
            else
            {
                //If blue channel becomes first, do not swap them
                if (Format.ToString().Contains("FORMAT_B"))
                    return DDSCompressor.DecodePixelBlock(data, (int)Width, (int)Height, (DDS.DXGI_FORMAT)Format);
                else
                    return ConvertBgraToRgba(DDSCompressor.DecodePixelBlock(data, (int)Width, (int)Height, (DDS.DXGI_FORMAT)Format));
            }

        }

        public void LoadDDS(string path)
        {
            SetNameFromPath(path);

            DDS dds = new DDS();
            LoadDDS(path);

            Width = dds.header.width;
            Height = dds.header.height;
            Format = dds.GetFormat();
            MipmapCount = dds.header.mipmapCount;
        }
        public void LoadTGA(string path)
        {
            SetNameFromPath(path);
            Bitmap tga = Paloma.TargaImage.LoadTargaImage(path);

        }
        public void LoadBitmap(string path)
        {
            SetNameFromPath(path);

        }
        public void SaveASTC(string FileName, int SurfaceLevel = 0, int MipLevel = 0)
        {
            var blockDims = GetDims(Format);

            ASTC atsc = new ASTC();
            atsc.BlockDimX = (byte)blockDims.Item1;
            atsc.BlockDimY = (byte)blockDims.Item2;
            atsc.BlockDimZ = (byte)blockDims.Item3;
        }
        public void SaveTGA(string FileName, int SurfaceLevel = 0, int MipLevel = 0)
        {
           
        }
        public void SaveBitMap(string FileName, int SurfaceLevel = 0, int MipLevel = 0)
        {
            Bitmap bitMap = GetBitmap(MipLevel, SurfaceLevel);
            bitMap.Save(FileName);
            bitMap.Dispose();
        }
        public void SaveDDS(string FileName, int SurfaceLevel = 0, int MipLevel = 0)
        {
            DDS dds = new DDS();
            dds.header = new DDS.Header();
            dds.header.width = Width;
            dds.header.height = Height;
            dds.header.mipmapCount = (uint)surfaces[SurfaceLevel].mipmaps.Count;
            dds.header.pitchOrLinearSize = (uint)surfaces[SurfaceLevel].mipmaps[MipLevel].Length;
            dds.SetFlags((DDS.DXGI_FORMAT)Format);

            dds.Save(dds, FileName, surfaces);
        }
        public static Tuple<int, int, int> GetDims(TEX_FORMAT Format)
        {
            switch (Format)
            {
                case TEX_FORMAT.BC1_TYPELESS:
                    return Tuple.Create(4,4,4);
            }
            return Tuple.Create(4, 4, 4);
        }
        public static bool IsCompressed(TEX_FORMAT Format)
        {
            switch (Format)
            {
                case TEX_FORMAT.BC1_UNORM:
                case TEX_FORMAT.BC1_UNORM_SRGB:
                case TEX_FORMAT.BC1_TYPELESS:
                case TEX_FORMAT.BC2_UNORM_SRGB:
                case TEX_FORMAT.BC2_UNORM:
                case TEX_FORMAT.BC2_TYPELESS:
                case TEX_FORMAT.BC3_UNORM_SRGB:
                case TEX_FORMAT.BC3_UNORM:
                case TEX_FORMAT.BC3_TYPELESS:
                case TEX_FORMAT.BC4_UNORM:
                case TEX_FORMAT.BC4_TYPELESS:
                case TEX_FORMAT.BC4_SNORM:
                case TEX_FORMAT.BC5_UNORM:
                case TEX_FORMAT.BC5_TYPELESS:
                case TEX_FORMAT.BC5_SNORM:
                    return true;
                default:
                    return false;
            }
        }
        //DXGI formats are first, then ASTC formats after
        public enum TEX_FORMAT : uint
        {
            UNKNOWN = 0,
            R32G32B32A32_TYPELESS = 1,
            R32G32B32A32_FLOAT = 2,
            R32G32B32A32_UINT = 3,
            R32G32B32A32_SINT = 4,
            R32G32B32_TYPELESS = 5,
            R32G32B32_FLOAT = 6,
            R32G32B32_UINT = 7,
            R32G32B32_SINT = 8,
            R16G16B16A16_TYPELESS = 9,
            R16G16B16A16_FLOAT = 10,
            R16G16B16A16_UNORM = 11,
            R16G16B16A16_UINT = 12,
            R16G16B16A16_SNORM = 13,
            R16G16B16A16_SINT = 14,
            R32G32_TYPELESS = 15,
            R32G32_FLOAT = 16,
            R32G32_UINT = 17,
            R32G32_SINT = 18,
            R32G8X24_TYPELESS = 19,
            D32_FLOAT_S8X24_UINT = 20,
            R32_FLOAT_X8X24_TYPELESS = 21,
            X32_TYPELESS_G8X24_UINT = 22,
            R10G10B10A2_TYPELESS = 23,
            R10G10B10A2_UNORM = 24,
            R10G10B10A2_UINT = 25,
            R11G11B10_FLOAT = 26,
            R8G8B8A8_TYPELESS = 27,
            R8G8B8A8_UNORM = 28,
            R8G8B8A8_UNORM_SRGB = 29,
            R8G8B8A8_UINT = 30,
            R8G8B8A8_SNORM = 31,
            R8G8B8A8_SINT = 32,
            R16G16_TYPELESS = 33,
            R16G16_FLOAT = 34,
            R16G16_UNORM = 35,
            R16G16_UINT = 36,
            R16G16_SNORM = 37,
            R16G16_SINT = 38,
            R32_TYPELESS = 39,
            D32_FLOAT = 40,
            R32_FLOAT = 41,
            R32_UINT = 42,
            R32_SINT = 43,
            R24G8_TYPELESS = 44,
            D24_UNORM_S8_UINT = 45,
            R24_UNORM_X8_TYPELESS = 46,
            X24_TYPELESS_G8_UINT = 47,
            R8G8_TYPELESS = 48,
            R8G8_UNORM = 49,
            R8G8_UINT = 50,
            R8G8_SNORM = 51,
            R8G8_SINT = 52,
            R16_TYPELESS = 53,
            R16_FLOAT = 54,
            D16_UNORM = 55,
            R16_UNORM = 56,
            R16_UINT = 57,
            R16_SNORM = 58,
            R16_SINT = 59,
            R8_TYPELESS = 60,
            R8_UNORM = 61,
            R8_UINT = 62,
            R8_SNORM = 63,
            R8_SINT = 64,
            A8_UNORM = 65,
            R1_UNORM = 66,
            R9G9B9E5_SHAREDEXP = 67,
            R8G8_B8G8_UNORM = 68,
            G8R8_G8B8_UNORM = 69,
            BC1_TYPELESS = 70,
            BC1_UNORM = 71,
            BC1_UNORM_SRGB = 72,
            BC2_TYPELESS = 73,
            BC2_UNORM = 74,
            BC2_UNORM_SRGB = 75,
            BC3_TYPELESS = 76,
            BC3_UNORM = 77,
            BC3_UNORM_SRGB = 78,
            BC4_TYPELESS = 79,
            BC4_UNORM = 80,
            BC4_SNORM = 81,
            BC5_TYPELESS = 82,
            BC5_UNORM = 83,
            BC5_SNORM = 84,
            B5G6R5_UNORM = 85,
            B5G5R5A1_UNORM = 86,
            B8G8R8A8_UNORM = 87,
            B8G8R8X8_UNORM = 88,
            R10G10B10_XR_BIAS_A2_UNORM = 89,
            B8G8R8A8_TYPELESS = 90,
            B8G8R8A8_UNORM_SRGB = 91,
            B8G8R8X8_TYPELESS = 92,
            B8G8R8X8_UNORM_SRGB = 93,
            BC6H_TYPELESS = 94,
            BC6H_UF16 = 95,
            BC6H_SF16 = 96,
            BC7_TYPELESS = 97,
            BC7_UNORM = 98,
            BC7_UNORM_SRGB = 99,
            AYUV = 100,
            Y410 = 101,
            Y416 = 102,
            NV12 = 103,
            P010 = 104,
            P016 = 105,
            Format_420_OPAQUE = 106,
            YUY2 = 107,
            Y210 = 108,
            Y216 = 109,
            NV11 = 110,
            AI44 = 111,
            IA44 = 112,
            P8 = 113,
            A8P8 = 114,
            B4G4R4A4_UNORM = 115,
            P208 = 130,
            V208 = 131,
            V408 = 132,

            ASTC4x4 = 200,
            ASTC5x4 = 201,
            ASTC5x5 = 202,
            ASTC6x5 = 203,
            ASTC6x6 = 204,
            ASTC8x5 = 205,
            ASTC8x6 = 206,
            ASTC8x8 = 207,
            ASTC10x5 = 208,
            ASTC10x6 = 209,
            ASTC10x8 = 210,
            ASTC10x10 = 211,
            ASTC12x10 = 212,
            ASTC12x12 = 213,
            FORCE_UINT = 0xFFFFFFFF
        }
        public STGenericTexture()
        {
        }
        public override void OnClick(TreeView treeView)
        {

        }
        private void SetNameFromPath(string path)
        {
            //Replace extensions manually. This is because using the
            //GetFileNameWithoutExtension function can remove .0, .1, texture names.
            var name = Path.GetFileName(path);
            name.Replace(".tga",  string.Empty);
            name.Replace(".png",  string.Empty);
            name.Replace(".jpg",  string.Empty);
            name.Replace(".dds",  string.Empty);
            name.Replace(".jpeg", string.Empty);
            name.Replace(".tiff", string.Empty);
            name.Replace(".gif",  string.Empty);
            name.Replace(".dds2", string.Empty);
            name.Replace(".jpe",  string.Empty);
            name.Replace(".jfif", string.Empty);
            name.Replace(".bmp",  string.Empty);
            name.Replace(".pdn",  string.Empty);
            name.Replace(".psd",  string.Empty);
            name.Replace(".hdr",  string.Empty);

            Text = name;
        }
        private static byte[] ConvertBgraToRgba(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i += 4)
            {
                var temp = bytes[i];
                bytes[i] = bytes[i + 2];
                bytes[i + 2] = temp;
            }
            return bytes;
        }
    }
}
