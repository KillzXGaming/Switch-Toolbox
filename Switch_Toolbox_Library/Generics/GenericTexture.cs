using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.IO;
using OpenTK.Graphics.OpenGL;
using Switch_Toolbox.Library.Rendering;

namespace Switch_Toolbox.Library
{
    public enum STChannelType
    {
        Red,
        Green,
        Blue,
        Alpha,
        One,
        Zero
    }

    public class STGenericTexture : TreeNodeCustom
    {
        public STGenericTexture()
        {
        }

        public STChannelType RedChannel;
        public STChannelType GreenChannel;
        public STChannelType BlueChannel;
        public STChannelType AlphaChannel;

        /// <summary>
        /// Stores the <see cref="Surface"/> instances for arrays of images. Cubemaps use 6 surfaces.
        /// </summary>
        public List<Surface> Surfaces = new List<Surface>();

        /// <summary>
        /// The total amount of surfaces for the texture.
        /// </summary>
        public int ArrayCount
        {
            get
            {
                return Surfaces.Count;
            }
        }

        /// <summary>
        /// The width of the image in pixels.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// The height of the image in pixels.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// The <see cref="TEX_FORMAT"/> Format of the image. 
        /// </summary>
        public TEX_FORMAT Format { get; set; }

        /// <summary>
        /// The <see cref="TEX_FORMAT_TYPE"/> Format of the image. 
        /// </summary>
        public TEX_FORMAT_TYPE FormatType { get; set; }

        public uint MipmapCount { get; set; }

        public RenderableTex RenderableTex { get; set; }

        public int GetBytesPerPixel(TEX_FORMAT Format)
        {
            return FormatTable[Format].BytesPerPixel;
        }

        public int GetBlockHeight(TEX_FORMAT Format)
        {
            return FormatTable[Format].BlockHeight;
        }

        public int GetBlockWidth(TEX_FORMAT Format)
        {
            return FormatTable[Format].BlockWidth;
        }

        // Based on Ryujinx's image table 
        // https://github.com/Ryujinx/Ryujinx/blob/c86aacde76b5f8e503e2b412385c8491ecc86b3b/Ryujinx.Graphics/Graphics3d/Texture/ImageUtils.cs
        // A nice way to get bpp, block data, and buffer types for formats

        private static readonly Dictionary<TEX_FORMAT, FormatInfo> FormatTable =
                         new Dictionary<TEX_FORMAT, FormatInfo>()
     {
            { TEX_FORMAT.R32_G32_B32_A32,      new FormatInfo(16, 1,  1,  TargetBuffer.Color) },
            { TEX_FORMAT.R16_G16_B16_A16,      new FormatInfo(8,  1,  1,  TargetBuffer.Color) },
            { TEX_FORMAT.R32_G32,              new FormatInfo(8,  1,  1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8_G8_B8_X8,          new FormatInfo(4,  1,  1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8_G8_B8_A8,          new FormatInfo(4,  1,  1,  TargetBuffer.Color) },
            { TEX_FORMAT.R10_G10_B10_A2,       new FormatInfo(4,  1,  1,  TargetBuffer.Color) },
            { TEX_FORMAT.R32,                  new FormatInfo(4,  1,  1,  TargetBuffer.Color) },
            { TEX_FORMAT.R4_G4_B4_A4,          new FormatInfo(2,  1,  1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC6,                  new FormatInfo(16, 4,  4,  TargetBuffer.Color) },
            { TEX_FORMAT.BC7,                  new FormatInfo(16, 4,  4,  TargetBuffer.Color) },
            { TEX_FORMAT.R16_G16,              new FormatInfo(4,  1,  1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8G8,                 new FormatInfo(2,  1,  1,  TargetBuffer.Color) },
            { TEX_FORMAT.R16,                  new FormatInfo(2,  1,  1,  TargetBuffer.Color) },
            { TEX_FORMAT.R8,                   new FormatInfo(1,  1,  1,  TargetBuffer.Color) },
            { TEX_FORMAT.R11_G11_B10,          new FormatInfo(4,  1,  1,  TargetBuffer.Color) },
            { TEX_FORMAT.BC1,                  new FormatInfo(8,  4,  4,  TargetBuffer.Color) },
            { TEX_FORMAT.BC2,                  new FormatInfo(16, 4,  4,  TargetBuffer.Color) },
            { TEX_FORMAT.BC3,                  new FormatInfo(16, 4,  4,  TargetBuffer.Color) },
            { TEX_FORMAT.BC4,                  new FormatInfo(8,  4,  4,  TargetBuffer.Color) },
            { TEX_FORMAT.BC5,                  new FormatInfo(16, 4,  4,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC4x4,              new FormatInfo(16, 4,  4,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC5x5,              new FormatInfo(16, 5,  5,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC6x6,              new FormatInfo(16, 6,  6,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC8x8,              new FormatInfo(16, 8,  8,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC10x10,            new FormatInfo(16, 10, 10, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC12x12,            new FormatInfo(16, 12, 12, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC5x4,              new FormatInfo(16, 5,  4,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC6x5,              new FormatInfo(16, 6,  5,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC8x6,              new FormatInfo(16, 8,  6,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC10x8,             new FormatInfo(16, 10, 8,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC12x10,            new FormatInfo(16, 12, 10, TargetBuffer.Color) },
            { TEX_FORMAT.ASTC8x5,              new FormatInfo(16, 8,  5,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC10x5,             new FormatInfo(16, 10, 5,  TargetBuffer.Color) },
            { TEX_FORMAT.ASTC10x6,             new FormatInfo(16, 10, 6,  TargetBuffer.Color) },

            { TEX_FORMAT.D16,   new FormatInfo(2, 1, 1, TargetBuffer.Depth)        },
            { TEX_FORMAT.D24,   new FormatInfo(4, 1, 1, TargetBuffer.Depth)        },
            { TEX_FORMAT.D24S8, new FormatInfo(4, 1, 1, TargetBuffer.DepthStencil) },
            { TEX_FORMAT.D32,   new FormatInfo(4, 1, 1, TargetBuffer.Depth)        },
            { TEX_FORMAT.D32S8, new FormatInfo(8, 1, 1, TargetBuffer.DepthStencil) }
     };

        /// <summary>
        /// A Surface contains mip levels of compressed/uncompressed texture data
        /// </summary>
        public class Surface
        {
            public List<byte[]> mipmaps = new List<byte[]>();
        }

        public void CreateGenericTexture(uint width, uint height, List<Surface> surfaces, TEX_FORMAT format )
        {
            Width = width;
            Height = height;
            Surfaces = surfaces;
            Surfaces = surfaces;
            Format = format;
        }
        private enum TargetBuffer
        {
            Color = 1,
            Depth = 2,
            Stencil = 3,
            DepthStencil = 4,
        }

        private class FormatInfo
        {
            public int BytesPerPixel { get; private set; }
            public int BlockWidth { get; private set; }
            public int BlockHeight { get; private set; }
            public TargetBuffer TargetBuffer;

            public FormatInfo(int bytesPerPixel, int blockWidth, int blockHeight, TargetBuffer targetBuffer)
            {
                BytesPerPixel = bytesPerPixel;
                BlockWidth = blockWidth;
                BlockHeight = blockHeight;
                TargetBuffer = targetBuffer;
            }
        }

        /// <summary>
        /// Gets a <see cref="Bitmap"/> given an array and mip index.
        /// </summary>
        /// <param name="ArrayIndex">The index of the surface/array. Cubemaps will have 6</param>
        /// <param name="MipLevel">The index of the mip level.</param>
        /// <returns></returns>
        public Bitmap GetBitmap(int ArrayIndex = 0, int MipLevel = 0)
        {
            if (Surfaces.Count == 0)
                throw new Exception($"Surfaces are empty on texture {Text}! Failed to get bitmap!");

            Bitmap bitmap = BitmapExtension.GetBitmap(DecodeBlock(Surfaces[ArrayIndex].mipmaps[MipLevel], Width, Height, Format),
                (int)Width, (int)Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);


            return bitmap;
        }
        public static Bitmap DecodeBlockGetBitmap(byte[] data, uint Width, uint Height, TEX_FORMAT Format)
        {
            Bitmap bitmap = BitmapExtension.GetBitmap(DecodeBlock(data, Width, Height, Format),
               (int)Width, (int)Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            return bitmap;
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
            if (Format <= 0)      throw new Exception($"Invalid Format!");
            if (data.Length <= 0) throw new Exception($"Data is empty!");
            if (Width <= 0)       throw new Exception($"Invalid width size {Width}!");
            if (Height <= 0)      throw new Exception($"Invalid height size {Height}!");

            if (Format == TEX_FORMAT.BC5)
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

        public static byte[] CompressBlock(byte[] data, int width, int height, TEX_FORMAT format, TEX_FORMAT_TYPE type, float alphaRef)
        {
            if (IsCompressed(format))
                return DDSCompressor.CompressBlock(data, width, height, DDS.GetDXGI_Format(format, type), alphaRef);
            else if (IsAtscFormat(format))
                return null;
            else
                return DDSCompressor.EncodePixelBlock(data, width, height, DDS.GetDXGI_Format(format, type));
        }

        public void LoadDDS(string path)
        {
            SetNameFromPath(path);

            DDS dds = new DDS();
            LoadDDS(path);

            Width = dds.header.width;
            Height = dds.header.height;
            var formats = dds.GetFormat();
            Format = formats.Item1;
            FormatType = formats.Item2;

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
            ASTC atsc = new ASTC();
            atsc.BlockDimX = (byte)GetBlockHeight(Format);
            atsc.BlockDimY = (byte)GetBlockWidth(Format);
            atsc.BlockDimZ = (byte)1;
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
            dds.header.mipmapCount = (uint)Surfaces[SurfaceLevel].mipmaps.Count;
            dds.header.pitchOrLinearSize = (uint)Surfaces[SurfaceLevel].mipmaps[MipLevel].Length;
            dds.SetFlags(Format, FormatType);

            dds.Save(dds, FileName, Surfaces);
        }

        public void LoadOpenGLTexture()
        {
            RenderableTex.LoadOpenGLTexture(this);
        }
        public static bool IsAtscFormat(TEX_FORMAT Format)
        {
            if (Format.ToString().Contains("ASTC"))
                return true;
            else
                return false;
        }

        public static bool IsCompressed(TEX_FORMAT Format)
        {
            switch (Format)
            {
                case TEX_FORMAT.BC1:
                case TEX_FORMAT.BC2:
                case TEX_FORMAT.BC3:
                case TEX_FORMAT.BC4:
                case TEX_FORMAT.BC5:
                case TEX_FORMAT.BC6:
                case TEX_FORMAT.BC7:
                    return true;
                default:
                    return false;
            }
        }
        public static STChannelType[] SetChannelsByFormat(TEX_FORMAT Format)
        {
            STChannelType[] channels = new STChannelType[4];

            switch (Format)
            {
                case TEX_FORMAT.BC5:
                    channels[0] = STChannelType.Red;
                    channels[1] = STChannelType.Green;
                    channels[2] = STChannelType.Zero;
                    channels[3] = STChannelType.One;
                    break;
                case TEX_FORMAT.BC4:
                    channels[0] = STChannelType.Red;
                    channels[1] = STChannelType.Red;
                    channels[2] = STChannelType.Red;
                    channels[3] = STChannelType.Red;
                    break;
                default:
                    channels[0] = STChannelType.Red;
                    channels[1] = STChannelType.Green;
                    channels[2] = STChannelType.Blue;
                    channels[3] = STChannelType.Alpha;
                    break;
            }
            return channels;
        }

        public static Bitmap SwapBlueRedChannels(Bitmap bitmap)
        {
            return ColorComponentSelector(bitmap, STChannelType.Blue, STChannelType.Green, STChannelType.Red, STChannelType.Alpha);
        }

        public static Bitmap ColorComponentSelector(Bitmap image, STChannelType R, STChannelType G, STChannelType B, STChannelType A)
        {
            BitmapExtension.ColorSwapFilter color = new BitmapExtension.ColorSwapFilter();
            if (R == STChannelType.Red)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.Red;
            if (R == STChannelType.Green)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.Green;
            if (R == STChannelType.Blue)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.Blue;
            if (R == STChannelType.Alpha)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.Alpha;
            if (R == STChannelType.One)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.One;
            if (R == STChannelType.Zero)
                color.CompRed = BitmapExtension.ColorSwapFilter.Red.Zero;

            if (G == STChannelType.Red)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.Red;
            if (G == STChannelType.Green)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.Green;
            if (G == STChannelType.Blue)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.Blue;
            if (G == STChannelType.Alpha)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.Alpha;
            if (G == STChannelType.One)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.One;
            if (G == STChannelType.Zero)
                color.CompGreen = BitmapExtension.ColorSwapFilter.Green.Zero;

            if (B == STChannelType.Red)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.Red;
            if (B == STChannelType.Green)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.Green;
            if (B == STChannelType.Blue)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.Blue;
            if (B == STChannelType.Alpha)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.Alpha;
            if (B == STChannelType.One)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.One;
            if (B == STChannelType.Zero)
                color.CompBlue = BitmapExtension.ColorSwapFilter.Blue.Zero;

            if (A == STChannelType.Red)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.Red;
            if (A == STChannelType.Green)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.Green;
            if (A == STChannelType.Blue)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.Blue;
            if (A == STChannelType.Alpha)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.Alpha;
            if (A == STChannelType.One)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.One;
            if (A == STChannelType.Zero)
                color.CompAlpha = BitmapExtension.ColorSwapFilter.Alpha.Zero;

            return BitmapExtension.SwapRGB(image, color);
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
