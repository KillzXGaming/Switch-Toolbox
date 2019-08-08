using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Chadsoft.CTools.Image;
using SuperBMDLib.Util;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Toolbox.Library
{
    public class Decode_Gamecube
    {
        //Code from https://github.com/Sage-of-Mirrors/SuperBMD/blob/ce1061e9b5f57de112f1d12f6459b938594664a0/SuperBMDLib/source/Materials/BinaryTextureImage.cs
        //Adjusted for proper editing in ST

        #region Data Types

        public static TEX_FORMAT ToGenericFormat(TextureFormats Format)
        {
            switch (Format)
            {
                case TextureFormats.C14X2: return TEX_FORMAT.C14X2;
                case TextureFormats.C4: return TEX_FORMAT.C4;
                case TextureFormats.C8: return TEX_FORMAT.C8;
                case TextureFormats.CMPR: return TEX_FORMAT.CMPR;
                case TextureFormats.I4: return TEX_FORMAT.I4;
                case TextureFormats.I8: return TEX_FORMAT.I8;
                case TextureFormats.IA4: return TEX_FORMAT.IA4;
                case TextureFormats.IA8: return TEX_FORMAT.IA8;
                case TextureFormats.RGB565: return TEX_FORMAT.RGB565;
                case TextureFormats.RGB5A3: return TEX_FORMAT.RGB5A3;
                case TextureFormats.RGBA32: return TEX_FORMAT.RGBA32;
                default:
                    throw new Exception("Unknown Format " + Format);
            }
        }

        public static PALETTE_FORMAT ToGenericPaletteFormat(PaletteFormats Format)
        {
            switch (Format)
            {
                case PaletteFormats.IA8: return PALETTE_FORMAT.IA8;
                case PaletteFormats.RGB565: return PALETTE_FORMAT.RGB565;
                case PaletteFormats.RGB5A3: return PALETTE_FORMAT.RGB5A3;
                default:
                    throw new Exception("Unknown Palette Format " + Format);
            }
        }

        public static PaletteFormats FromGenericPaletteFormat(PALETTE_FORMAT Format)
        {
            switch (Format)
            {
                case PALETTE_FORMAT.None: return PaletteFormats.IA8;
                case PALETTE_FORMAT.IA8: return PaletteFormats.IA8;
                case PALETTE_FORMAT.RGB565: return PaletteFormats.RGB565;
                case PALETTE_FORMAT.RGB5A3: return PaletteFormats.RGB5A3;
                default:
                    throw new Exception("Unknown Palette Format " + Format);
            }
        }

        public static TextureFormats FromGenericFormat(TEX_FORMAT Format)
        {
            switch (Format)
            {
                case TEX_FORMAT.C14X2: return TextureFormats.C14X2;
                case TEX_FORMAT.C4: return TextureFormats.C4;
                case TEX_FORMAT.C8: return TextureFormats.C8;
                case TEX_FORMAT.CMPR: return TextureFormats.CMPR;
                case TEX_FORMAT.I4: return TextureFormats.I4;
                case TEX_FORMAT.I8: return TextureFormats.I8;
                case TEX_FORMAT.IA4: return TextureFormats.IA4;
                case TEX_FORMAT.IA8: return TextureFormats.IA8;
                case TEX_FORMAT.RGB565: return TextureFormats.RGB565;
                case TEX_FORMAT.RGB5A3: return TextureFormats.RGB5A3;
                case TEX_FORMAT.RGBA32: return TextureFormats.RGBA32;
                default:
                    throw new Exception("Unknown Format " + Format);
            }
        }

        /// <summary>
        /// ImageFormat specifies how the data within the image is encoded.
        /// Included is a chart of how many bits per pixel there are, 
        /// the width/height of each block, how many bytes long the
        /// actual block is, and a description of the type of data stored.
        /// </summary>
        public enum TextureFormats
        {
            //Bits per Pixel | Block Width | Block Height | Block Size | Type / Description
            I4 = 0x00,      //  4 | 8 | 8 | 32 | grey
            I8 = 0x01,      //  8 | 8 | 8 | 32 | grey
            IA4 = 0x02,     //  8 | 8 | 4 | 32 | grey + alpha
            IA8 = 0x03,     // 16 | 4 | 4 | 32 | grey + alpha
            RGB565 = 0x04,  // 16 | 4 | 4 | 32 | color
            RGB5A3 = 0x05,  // 16 | 4 | 4 | 32 | color + alpha
            RGBA32 = 0x06,  // 32 | 4 | 4 | 64 | color + alpha
            C4 = 0x08,      //  4 | 8 | 8 | 32 | palette choices (IA8, RGB565, RGB5A3)
            C8 = 0x09,      //  8 | 8 | 4 | 32 | palette choices (IA8, RGB565, RGB5A3)
            C14X2 = 0x0a,   // 16 | 4 | 4 | 32 | palette (IA8, RGB565, RGB5A3) NOTE: only 14 bits are used per pixel
            CMPR = 0x0e,    //  4 | 8 | 8 | 32 | mini palettes in each block, RGB565 or transparent.
        }

        /// <summary>
        /// Defines how textures handle going out of [0..1] range for texcoords.
        /// </summary>
        public enum WrapModes
        {
            ClampToEdge = 0,
            Repeat = 1,
            MirroredRepeat = 2,
        }

        /// <summary>
        /// PaletteFormat specifies how the data within the palette is stored. An
        /// image uses a single palette (except CMPR which defines its own
        /// mini-palettes within the Image data). Only C4, C8, and C14X2 use
        /// palettes. For all other formats the type and count is zero.
        /// </summary>
        public enum PaletteFormats
        {
            IA8 = 0x00,
            RGB565 = 0x01,
            RGB5A3 = 0x02,
        }

        /// <summary>
        /// FilterMode specifies what type of filtering the file should use for min/mag.
        /// </summary>
        public enum FilterMode
        {
            /* Valid in both Min and Mag Filter */
            Nearest = 0x0,                  // Point Sampling, No Mipmap
            Linear = 0x1,                   // Bilinear Filtering, No Mipmap

            /* Valid in only Min Filter */
            NearestMipmapNearest = 0x2,     // Point Sampling, Discrete Mipmap
            NearestMipmapLinear = 0x3,      // Bilinear Filtering, Discrete Mipmap
            LinearMipmapNearest = 0x4,      // Point Sampling, Linear MipMap
            LinearMipmapLinear = 0x5,       // Trilinear Filtering
        }

        /// <summary>
        /// The Palette simply stores the color data as loaded from the file.
        /// It does not convert the files based on the Palette type to RGBA8.
        /// </summary>
        private sealed class Palette
        {
            private byte[] _paletteData;

            public void Load(byte[] paletteData)
            {
                _paletteData = paletteData;
            }

            public void Load(ushort[] paletteData)
            {
                var mem = new System.IO.MemoryStream();
                using (var writer = new FileWriter(mem))
                {
                    writer.Write(paletteData);
                }

                _paletteData = mem.ToArray();
            }

            public void Load(FileReader reader, uint paletteEntryCount)
            {
                //Files that don't have palettes have an entry count of zero.
                if (paletteEntryCount == 0)
                {
                    _paletteData = new byte[0];
                    return;
                }

                //All palette formats are 2 bytes per entry.
                _paletteData = reader.ReadBytes((int)paletteEntryCount * 2);
            }

            public byte[] GetBytes()
            {
                return _paletteData;
            }
        }
        #endregion

        #region MethodsHelpers

        public static byte[] GetMipLevel(byte[] ImageData, uint Width, uint Height, uint MipCount, uint MipLevel, TEX_FORMAT format)
        {
            return GetMipLevel(ImageData, Width, Height, MipCount, MipLevel, FromGenericFormat(format));
        }

        public static byte[] GetMipLevel(byte[] ImageData, uint Width, uint Height, uint MipCount, uint MipLevel, TextureFormats format)
        {
            uint offset = 0;
            for (int m = 0; m < MipCount; m++)
            {
                uint width = (uint)Math.Max(1, Width >> m);
                uint height = (uint)Math.Max(1, Height >> m);

                uint size = (uint)Decode_Gamecube.GetDataSize(format, (int)width, (int)height);

                if (MipLevel == m)
                    return Utils.SubArray(ImageData, offset, size);

                offset += size;
            }

            return ImageData;
        }

        #endregion

        #region Decoding

        private static readonly int[] Bpp = { 4, 8, 8, 16, 16, 16, 32, 0, 4, 8, 16, 0, 0, 0, 4 };

        public static int GetBpp(TextureFormats Format) { return Bpp[(uint)Format]; }

        private static readonly int[] TileSizeW = { 8, 8, 8, 4, 4, 4, 4, 0, 8, 8, 4, 0, 0, 0, 8 };
        private static readonly int[] TileSizeH = { 8, 4, 4, 4, 4, 4, 4, 0, 8, 4, 4, 0, 0, 0, 8 };

        public static int GetDataSizeWithMips(TextureFormats format, uint Width, uint Height, uint MipCount)
        {
            return GetDataSizeWithMips((uint)format, Width, Height, MipCount);
        }

        public static int GetDataSizeWithMips(uint format, uint Width, uint Height, uint MipCount)
        {
            int size = 0;
            for (int m = 0; m < MipCount; m++)
            {
                uint width = (uint)Math.Max(1, Width >> m);
                uint height = (uint)Math.Max(1, Height >> m);

                size =+ Decode_Gamecube.GetDataSize(format, width, height);
            }

            return size;
        }

        public static int GetDataSize(uint Format, uint Width, uint Height)
        {
            return GetDataSize((TextureFormats)Format, (int)Width, (int)Height);
        }

        public static int GetDataSize(TextureFormats Format, int Width, int Height)
        {
            while ((Width % TileSizeW[(uint)Format]) != 0) Width++;
            while ((Height % TileSizeH[(uint)Format]) != 0) Height++;
            return Width * Height * GetBpp(Format) / 8;
        }

        public static System.Drawing.Bitmap DecodeDataToBitmap(byte[] ImageData, ushort[] PaletteData, uint width, uint height, TextureFormats format, PaletteFormats palleteFormat)
        {
            return BitmapExtension.GetBitmap(DecodeData(ImageData, PaletteData, width, height, format, palleteFormat),
               (int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        public static System.Drawing.Bitmap DecodeDataToBitmap(byte[] ImageData, byte[] PaletteData, uint width, uint height, TEX_FORMAT format, PALETTE_FORMAT palleteFormat)
        {
            var FormatGC = FromGenericFormat(format);
            var PalleteFormatGC = FromGenericPaletteFormat(palleteFormat);
            return BitmapExtension.GetBitmap(DecodeData(ImageData, PaletteData, width, height, FormatGC, PalleteFormatGC),
               (int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        public static byte[] DecodeData(byte[] ImageData, byte[] PaletteData, uint width, uint height, TEX_FORMAT format, PALETTE_FORMAT palleteFormat) {
            var FormatGC = FromGenericFormat(format);
            var PalleteFormatGC = FromGenericPaletteFormat(palleteFormat);
            return DecodeData(ImageData, PaletteData, width, height, FormatGC, PalleteFormatGC);
        }

        public static byte[] DecodeData(byte[] ImageData, ushort[] PaletteData, uint width, uint height, TextureFormats format, PaletteFormats palleteFormat) {
            Palette Palette = new Palette();
            Palette.Load(PaletteData);

            return DecodeData(new FileReader(ImageData), width, height, format, Palette, palleteFormat);
        }

        public static byte[] DecodeData(byte[] ImageData, byte[] PaletteData, uint width, uint height, TextureFormats format, PaletteFormats palleteFormat)
        {
            Palette Palette = new Palette();
            Palette.Load(PaletteData);

            return DecodeData(new FileReader(ImageData), width, height, format, Palette, palleteFormat);
        }

        private static byte[] DecodeData(FileReader stream, uint width, uint height, TextureFormats format, Palette imagePalette, PaletteFormats paletteFormat)
        {
            stream.SetByteOrder(true);

            switch (format)
            {
                case TextureFormats.I4:
                    return DecodeI4(stream, width, height);
                case TextureFormats.I8:
                    return DecodeI8(stream, width, height);
                case TextureFormats.IA4:
                    return DecodeIA4(stream, width, height);
                case TextureFormats.IA8:
                    return DecodeIA8(stream, width, height);
                case TextureFormats.RGB565:
                    return DecodeRgb565(stream, width, height);
                case TextureFormats.RGB5A3:
                    return DecodeRgb5A3(stream, width, height);
                case TextureFormats.RGBA32:
                    return DecodeRgba32(stream, width, height);
                case TextureFormats.C4:
                    return DecodeC4(stream, width, height, imagePalette, paletteFormat);
                case TextureFormats.C8:
                    return DecodeC8(stream, width, height, imagePalette, paletteFormat);
                case TextureFormats.CMPR:
                    return DecodeCmpr(stream, width, height);
                case TextureFormats.C14X2:
                default:
                    Console.WriteLine("Unsupported Binary Texture Image format {0}, unable to decode!", format);
                    return new byte[0];
            }
        }

        private static byte[] DecodeRgba32(FileReader stream, uint width, uint height)
        {
            uint numBlocksW = width / 4; //4 byte block width
            uint numBlocksH = height / 4; //4 byte block height 

            byte[] decodedData = new byte[width * height * 4];

            for (int yBlock = 0; yBlock < numBlocksH; yBlock++)
            {
                for (int xBlock = 0; xBlock < numBlocksW; xBlock++)
                {
                    //For each block, we're going to examine block width / block height number of 'pixels'
                    for (int pY = 0; pY < 4; pY++)
                    {
                        for (int pX = 0; pX < 4; pX++)
                        {
                            //Ensure the pixel we're checking is within bounds of the image.
                            if ((xBlock * 4 + pX >= width) || (yBlock * 4 + pY >= height))
                                continue;

                            //Now we're looping through each pixel in a block, but a pixel is four bytes long. 
                            uint destIndex = (uint)(4 * (width * ((yBlock * 4) + pY) + (xBlock * 4) + pX));
                            decodedData[destIndex + 3] = stream.ReadByte(); //Alpha
                            decodedData[destIndex + 2] = stream.ReadByte(); //Red
                        }
                    }

                    //...but we have to do it twice, because RGBA32 stores two sub-blocks per block. (AR, and GB)
                    for (int pY = 0; pY < 4; pY++)
                    {
                        for (int pX = 0; pX < 4; pX++)
                        {
                            //Ensure the pixel we're checking is within bounds of the image.
                            if ((xBlock * 4 + pX >= width) || (yBlock * 4 + pY >= height))
                                continue;

                            //Now we're looping through each pixel in a block, but a pixel is four bytes long. 
                            uint destIndex = (uint)(4 * (width * ((yBlock * 4) + pY) + (xBlock * 4) + pX));
                            decodedData[destIndex + 1] = stream.ReadByte(); //Green
                            decodedData[destIndex + 0] = stream.ReadByte(); //Blue
                        }
                    }

                }
            }

            return decodedData;
        }

        private static byte[] DecodeC4(FileReader stream, uint width, uint height, Palette imagePalette, PaletteFormats paletteFormat)
        {
            //4 bpp, 8 block width/height, block size 32 bytes, possible palettes (IA8, RGB565, RGB5A3)
            uint numBlocksW = width / 8;
            uint numBlocksH = height / 8;

            byte[] decodedData = new byte[width * height * 8];

            //Read the indexes from the file
            for (int yBlock = 0; yBlock < numBlocksH; yBlock++)
            {
                for (int xBlock = 0; xBlock < numBlocksW; xBlock++)
                {
                    //Inner Loop for pixels
                    for (int pY = 0; pY < 8; pY++)
                    {
                        for (int pX = 0; pX < 8; pX += 2)
                        {
                            //Ensure we're not reading past the end of the image.
                            if ((xBlock * 8 + pX >= width) || (yBlock * 8 + pY >= height))
                                continue;

                            byte data = stream.ReadByte();
                            byte t = (byte)(data & 0xF0);
                            byte t2 = (byte)(data & 0x0F);

                            decodedData[width * ((yBlock * 8) + pY) + (xBlock * 8) + pX + 0] = (byte)(t >> 4);
                            decodedData[width * ((yBlock * 8) + pY) + (xBlock * 8) + pX + 1] = t2;
                        }
                    }
                }
            }

            //Now look them up in the palette and turn them into actual colors.
            byte[] finalDest = new byte[decodedData.Length / 2];

            int pixelSize = paletteFormat == PaletteFormats.IA8 ? 2 : 4;
            int destOffset = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    UnpackPixelFromPalette(decodedData[y * width + x], ref finalDest, destOffset, imagePalette.GetBytes(), paletteFormat);
                    destOffset += pixelSize;
                }
            }

            return finalDest;
        }

        private static byte[] DecodeC8(FileReader stream, uint width, uint height, Palette imagePalette, PaletteFormats paletteFormat)
        {
            //4 bpp, 8 block width/4 block height, block size 32 bytes, possible palettes (IA8, RGB565, RGB5A3)
            uint numBlocksW = width / 8;
            uint numBlocksH = height / 4;

            byte[] decodedData = new byte[width * height * 8];

            //Read the indexes from the file
            for (int yBlock = 0; yBlock < numBlocksH; yBlock++)
            {
                for (int xBlock = 0; xBlock < numBlocksW; xBlock++)
                {
                    //Inner Loop for pixels
                    for (int pY = 0; pY < 4; pY++)
                    {
                        for (int pX = 0; pX < 8; pX++)
                        {
                            //Ensure we're not reading past the end of the image.
                            if ((xBlock * 8 + pX >= width) || (yBlock * 4 + pY >= height))
                                continue;


                            byte data = stream.ReadByte();
                            decodedData[width * ((yBlock * 4) + pY) + (xBlock * 8) + pX] = data;
                        }
                    }
                }
            }

            //Now look them up in the palette and turn them into actual colors.
            byte[] finalDest = new byte[decodedData.Length / 2];

            int pixelSize = paletteFormat == PaletteFormats.IA8 ? 2 : 4;
            int destOffset = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    UnpackPixelFromPalette(decodedData[y * width + x], ref finalDest, destOffset, imagePalette.GetBytes(), paletteFormat);
                    destOffset += pixelSize;
                }
            }

            return finalDest;
        }

        private static byte[] DecodeRgb565(FileReader stream, uint width, uint height)
        {
            //16 bpp, 4 block width/height, block size 32 bytes, color.
            uint numBlocksW = width / 4;
            uint numBlocksH = height / 4;

            byte[] decodedData = new byte[width * height * 4];

            //Read the indexes from the file
            for (int yBlock = 0; yBlock < numBlocksH; yBlock++)
            {
                for (int xBlock = 0; xBlock < numBlocksW; xBlock++)
                {
                    //Inner Loop for pixels
                    for (int pY = 0; pY < 4; pY++)
                    {
                        for (int pX = 0; pX < 4; pX++)
                        {
                            //Ensure we're not reading past the end of the image.
                            if ((xBlock * 4 + pX >= width) || (yBlock * 4 + pY >= height))
                                continue;

                            ushort sourcePixel = stream.ReadUInt16();
                            RGB565ToRGBA8(sourcePixel, ref decodedData,
                                (int)(4 * (width * ((yBlock * 4) + pY) + (xBlock * 4) + pX)));
                        }
                    }
                }
            }

            return decodedData;
        }

        private static byte[] DecodeCmpr(FileReader stream, uint width, uint height)
        {
            //Decode S3TC1
            byte[] buffer = new byte[width * height * 4];

            for (int y = 0; y < height / 4; y += 2)
            {
                for (int x = 0; x < width / 4; x += 2)
                {
                    for (int dy = 0; dy < 2; ++dy)
                    {
                        for (int dx = 0; dx < 2; ++dx)
                        {
                            if (4 * (x + dx) < width && 4 * (y + dy) < height)
                            {
                                byte[] fileData = stream.ReadBytes(8);
                                Buffer.BlockCopy(fileData, 0, buffer, (int)(8 * ((y + dy) * width / 4 + x + dx)), 8);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < width * height / 2; i += 8)
            {
                // Micro swap routine needed
                Swap(ref buffer[i], ref buffer[i + 1]);
                Swap(ref buffer[i + 2], ref buffer[i + 3]);

                buffer[i + 4] = S3TC1ReverseByte(buffer[i + 4]);
                buffer[i + 5] = S3TC1ReverseByte(buffer[i + 5]);
                buffer[i + 6] = S3TC1ReverseByte(buffer[i + 6]);
                buffer[i + 7] = S3TC1ReverseByte(buffer[i + 7]);
            }

            //Now decompress the DXT1 data within it.
            return DecompressDxt1(buffer, width, height);
        }

        private static void Swap(ref byte b1, ref byte b2)
        {
            byte tmp = b1; b1 = b2; b2 = tmp;
        }

        private static ushort Read16Swap(byte[] data, uint offset)
        {
            return (ushort)((Buffer.GetByte(data, (int)offset + 1) << 8) | Buffer.GetByte(data, (int)offset));
        }

        private static uint Read32Swap(byte[] data, uint offset)
        {
            return (uint)((Buffer.GetByte(data, (int)offset + 3) << 24) | (Buffer.GetByte(data, (int)offset + 2) << 16) | (Buffer.GetByte(data, (int)offset + 1) << 8) | Buffer.GetByte(data, (int)offset));
        }

        private static byte S3TC1ReverseByte(byte b)
        {
            byte b1 = (byte)(b & 0x3);
            byte b2 = (byte)(b & 0xC);
            byte b3 = (byte)(b & 0x30);
            byte b4 = (byte)(b & 0xC0);

            return (byte)((b1 << 6) | (b2 << 2) | (b3 >> 2) | (b4 >> 6));
        }

        private static byte[] DecompressDxt1(byte[] src, uint width, uint height)
        {
            uint dataOffset = 0;
            byte[] finalData = new byte[width * height * 4];

            for (int y = 0; y < height; y += 4)
            {
                for (int x = 0; x < width; x += 4)
                {
                    // Haha this is in little-endian (DXT1) so we have to swap the already swapped bytes.
                    ushort color1 = Read16Swap(src, dataOffset);
                    ushort color2 = Read16Swap(src, dataOffset + 2);
                    uint bits = Read32Swap(src, dataOffset + 4);
                    dataOffset += 8;

                    byte[][] ColorTable = new byte[4][];
                    for (int i = 0; i < 4; i++)
                        ColorTable[i] = new byte[4];

                    RGB565ToRGBA8(color1, ref ColorTable[0], 0);
                    RGB565ToRGBA8(color2, ref ColorTable[1], 0);

                    if (color1 > color2)
                    {
                        ColorTable[2][0] = (byte)((2 * ColorTable[0][0] + ColorTable[1][0] + 1) / 3);
                        ColorTable[2][1] = (byte)((2 * ColorTable[0][1] + ColorTable[1][1] + 1) / 3);
                        ColorTable[2][2] = (byte)((2 * ColorTable[0][2] + ColorTable[1][2] + 1) / 3);
                        ColorTable[2][3] = 0xFF;

                        ColorTable[3][0] = (byte)((ColorTable[0][0] + 2 * ColorTable[1][0] + 1) / 3);
                        ColorTable[3][1] = (byte)((ColorTable[0][1] + 2 * ColorTable[1][1] + 1) / 3);
                        ColorTable[3][2] = (byte)((ColorTable[0][2] + 2 * ColorTable[1][2] + 1) / 3);
                        ColorTable[3][3] = 0xFF;
                    }
                    else
                    {
                        ColorTable[2][0] = (byte)((ColorTable[0][0] + ColorTable[1][0] + 1) / 2);
                        ColorTable[2][1] = (byte)((ColorTable[0][1] + ColorTable[1][1] + 1) / 2);
                        ColorTable[2][2] = (byte)((ColorTable[0][2] + ColorTable[1][2] + 1) / 2);
                        ColorTable[2][3] = 0xFF;

                        ColorTable[3][0] = (byte)((ColorTable[0][0] + 2 * ColorTable[1][0] + 1) / 3);
                        ColorTable[3][1] = (byte)((ColorTable[0][1] + 2 * ColorTable[1][1] + 1) / 3);
                        ColorTable[3][2] = (byte)((ColorTable[0][2] + 2 * ColorTable[1][2] + 1) / 3);
                        ColorTable[3][3] = 0x00;
                    }

                    for (int iy = 0; iy < 4; ++iy)
                    {
                        for (int ix = 0; ix < 4; ++ix)
                        {
                            if (((x + ix) < width) && ((y + iy) < height))
                            {
                                int di = (int)(4 * ((y + iy) * width + x + ix));
                                int si = (int)(bits & 0x3);
                                finalData[di + 0] = ColorTable[si][0];
                                finalData[di + 1] = ColorTable[si][1];
                                finalData[di + 2] = ColorTable[si][2];
                                finalData[di + 3] = ColorTable[si][3];
                            }
                            bits >>= 2;
                        }
                    }
                }
            }

            return finalData;
        }

        private static byte[] DecodeIA8(FileReader stream, uint width, uint height)
        {
            uint numBlocksW = width / 4; //4 byte block width
            uint numBlocksH = height / 4; //4 byte block height 

            byte[] decodedData = new byte[width * height * 4];

            for (int yBlock = 0; yBlock < numBlocksH; yBlock++)
            {
                for (int xBlock = 0; xBlock < numBlocksW; xBlock++)
                {
                    //For each block, we're going to examine block width / block height number of 'pixels'
                    for (int pY = 0; pY < 4; pY++)
                    {
                        for (int pX = 0; pX < 4; pX++)
                        {
                            //Ensure the pixel we're checking is within bounds of the image.
                            if ((xBlock * 4 + pX >= width) || (yBlock * 4 + pY >= height))
                                continue;

                            //Now we're looping through each pixel in a block, but a pixel is four bytes long. 
                            uint destIndex = (uint)(4 * (width * ((yBlock * 4) + pY) + (xBlock * 4) + pX));
                            byte byte0 = stream.ReadByte();
                            byte byte1 = stream.ReadByte();
                            decodedData[destIndex + 3] = byte0;
                            decodedData[destIndex + 2] = byte1;
                            decodedData[destIndex + 1] = byte1;
                            decodedData[destIndex + 0] = byte1;
                        }
                    }
                }
            }

            return decodedData;
        }

        private static byte[] DecodeIA4(FileReader stream, uint width, uint height)
        {
            uint numBlocksW = width / 8;
            uint numBlocksH = height / 4;

            byte[] decodedData = new byte[width * height * 4];

            for (int yBlock = 0; yBlock < height; yBlock++)
            {
                for (int xBlock = 0; xBlock < width; xBlock++)
                {
                    //For each block, we're going to examine block width / block height number of 'pixels'
                    for (int pY = 0; pY < 4; pY++)
                    {
                        for (int pX = 0; pX < 8; pX++)
                        {
                            //Ensure the pixel we're checking is within bounds of the image.
                            if ((xBlock * 8 + pX >= width) || (yBlock * 4 + pY >= height))
                                continue;


                            byte value = stream.ReadByte();

                            byte alpha = (byte)((value & 0xF0) >> 4);
                            byte lum = (byte)(value & 0x0F);

                            uint destIndex = (uint)(4 * (width * ((yBlock * 4) + pY) + (xBlock * 8) + pX));

                            decodedData[destIndex + 0] = (byte)(lum * 0x11);
                            decodedData[destIndex + 1] = (byte)(lum * 0x11);
                            decodedData[destIndex + 2] = (byte)(lum * 0x11);
                            decodedData[destIndex + 3] = (byte)(alpha * 0x11);
                        }
                    }
                }
            }

            return decodedData;
        }

        private static byte[] DecodeI4(FileReader stream, uint width, uint height)
        {
            uint numBlocksW = width / 8; //8 byte block width
            uint numBlocksH = height / 8; //8 byte block height 

            byte[] decodedData = new byte[width * height * 4];

            for (int yBlock = 0; yBlock < numBlocksH; yBlock++)
            {
                for (int xBlock = 0; xBlock < numBlocksW; xBlock++)
                {
                    //For each block, we're going to examine block width / block height number of 'pixels'
                    for (int pY = 0; pY < 8; pY++)
                    {
                        for (int pX = 0; pX < 8; pX += 2)
                        {
                            //Ensure the pixel we're checking is within bounds of the image.
                            if ((xBlock * 8 + pX >= width) || (yBlock * 8 + pY >= height))
                                continue;

                            byte data = stream.ReadByte();
                            byte t = (byte)((data & 0xF0) >> 4);
                            byte t2 = (byte)(data & 0x0F);
                            uint destIndex = (uint)(4 * (width * ((yBlock * 8) + pY) + (xBlock * 8) + pX));

                            decodedData[destIndex + 0] = (byte)(t * 0x11);
                            decodedData[destIndex + 1] = (byte)(t * 0x11);
                            decodedData[destIndex + 2] = (byte)(t * 0x11);
                            decodedData[destIndex + 3] = (byte)(t * 0x11);

                            decodedData[destIndex + 4] = (byte)(t2 * 0x11);
                            decodedData[destIndex + 5] = (byte)(t2 * 0x11);
                            decodedData[destIndex + 6] = (byte)(t2 * 0x11);
                            decodedData[destIndex + 7] = (byte)(t2 * 0x11);
                        }
                    }
                }
            }

            return decodedData;
        }

        private static byte[] DecodeI8(FileReader stream, uint width, uint height)
        {
            uint numBlocksW = width / 8; //8 pixel block width
            uint numBlocksH = height / 4; //4 pixel block height 

            byte[] decodedData = new byte[width * height * 4];

            for (int yBlock = 0; yBlock < numBlocksH; yBlock++)
            {
                for (int xBlock = 0; xBlock < numBlocksW; xBlock++)
                {
                    //For each block, we're going to examine block width / block height number of 'pixels'
                    for (int pY = 0; pY < 4; pY++)
                    {
                        for (int pX = 0; pX < 8; pX++)
                        {
                            //Ensure the pixel we're checking is within bounds of the image.
                            if ((xBlock * 8 + pX >= width) || (yBlock * 4 + pY >= height))
                                continue;

                            byte data = stream.ReadByte();
                            uint destIndex = (uint)(4 * (width * ((yBlock * 4) + pY) + (xBlock * 8) + pX));

                            decodedData[destIndex + 0] = data;
                            decodedData[destIndex + 1] = data;
                            decodedData[destIndex + 2] = data;
                            decodedData[destIndex + 3] = data;
                        }
                    }
                }
            }

            return decodedData;
        }

        private static byte[] DecodeRgb5A3(FileReader stream, uint width, uint height)
        {
            uint numBlocksW = width / 4; //4 byte block width
            uint numBlocksH = height / 4; //4 byte block height 

            byte[] decodedData = new byte[width * height * 4];

            for (int yBlock = 0; yBlock < numBlocksH; yBlock++)
            {
                for (int xBlock = 0; xBlock < numBlocksW; xBlock++)
                {
                    //For each block, we're going to examine block width / block height number of 'pixels'
                    for (int pY = 0; pY < 4; pY++)
                    {
                        for (int pX = 0; pX < 4; pX++)
                        {
                            //Ensure the pixel we're checking is within bounds of the image.
                            if ((xBlock * 4 + pX >= width) || (yBlock * 4 + pY >= height))
                                continue;

                            ushort sourcePixel = stream.ReadUInt16();
                            RGB5A3ToRGBA8(sourcePixel, ref decodedData,
                                (int)(4 * (width * ((yBlock * 4) + pY) + (xBlock * 4) + pX)));
                        }
                    }
                }
            }

            return decodedData;
        }

        private static void UnpackPixelFromPalette(int paletteIndex, ref byte[] dest, int offset, byte[] paletteData, PaletteFormats format)
        {
            switch (format)
            {
                case PaletteFormats.IA8:
                    dest[0] = paletteData[2 * paletteIndex + 1];
                    dest[1] = paletteData[2 * paletteIndex + 0];
                    break;
                case PaletteFormats.RGB565:
                    {
                        ushort palettePixelData = (ushort)((Buffer.GetByte(paletteData, 2 * paletteIndex) << 8) | Buffer.GetByte(paletteData, 2 * paletteIndex + 1));
                        RGB565ToRGBA8(palettePixelData, ref dest, offset);
                    }
                    break;
                case PaletteFormats.RGB5A3:
                    {
                        ushort palettePixelData = (ushort)((Buffer.GetByte(paletteData, 2 * paletteIndex) << 8) | Buffer.GetByte(paletteData, 2 * paletteIndex + 1));
                        RGB5A3ToRGBA8(palettePixelData, ref dest, offset);
                    }
                    break;
            }
        }



        /// <summary>
        /// Convert a RGB565 encoded pixel (two bytes in length) to a RGBA (4 byte in length)
        /// pixel.
        /// </summary>
        /// <param name="sourcePixel">RGB565 encoded pixel.</param>
        /// <param name="dest">Destination array for RGBA pixel.</param>
        /// <param name="destOffset">Offset into destination array to write RGBA pixel.</param>
        private static void RGB565ToRGBA8(ushort sourcePixel, ref byte[] dest, int destOffset)
        {
            byte r, g, b;
            r = (byte)((sourcePixel & 0xF100) >> 11);
            g = (byte)((sourcePixel & 0x7E0) >> 5);
            b = (byte)((sourcePixel & 0x1F));

            r = (byte)((r << (8 - 5)) | (r >> (10 - 8)));
            g = (byte)((g << (8 - 6)) | (g >> (12 - 8)));
            b = (byte)((b << (8 - 5)) | (b >> (10 - 8)));

            dest[destOffset] = b;
            dest[destOffset + 1] = g;
            dest[destOffset + 2] = r;
            dest[destOffset + 3] = 0xFF; //Set alpha to 1
        }

        /// <summary>
        /// Convert a RGB5A3 encoded pixel (two bytes in length) to an RGBA (4 byte in length)
        /// pixel.
        /// </summary>
        /// <param name="sourcePixel">RGB5A3 encoded pixel.</param>
        /// <param name="dest">Destination array for RGBA pixel.</param>
        /// <param name="destOffset">Offset into destination array to write RGBA pixel.</param>
        private static void RGB5A3ToRGBA8(ushort sourcePixel, ref byte[] dest, int destOffset)
        {
            byte r, g, b, a;

            //No alpha bits
            if ((sourcePixel & 0x8000) == 0x8000)
            {
                a = 0xFF;
                r = (byte)((sourcePixel & 0x7C00) >> 10);
                g = (byte)((sourcePixel & 0x3E0) >> 5);
                b = (byte)(sourcePixel & 0x1F);

                r = (byte)((r << (8 - 5)) | (r >> (10 - 8)));
                g = (byte)((g << (8 - 5)) | (g >> (10 - 8)));
                b = (byte)((b << (8 - 5)) | (b >> (10 - 8)));
            }
            //Alpha bits
            else
            {
                a = (byte)((sourcePixel & 0x7000) >> 12);
                r = (byte)((sourcePixel & 0xF00) >> 8);
                g = (byte)((sourcePixel & 0xF0) >> 4);
                b = (byte)(sourcePixel & 0xF);

                a = (byte)((a << (8 - 3)) | (a << (8 - 6)) | (a >> (9 - 8)));
                r = (byte)((r << (8 - 4)) | r);
                g = (byte)((g << (8 - 4)) | g);
                b = (byte)((b << (8 - 4)) | b);
            }

            dest[destOffset + 0] = b;
            dest[destOffset + 1] = g;
            dest[destOffset + 2] = r;
            dest[destOffset + 3] = a;
        }
        #endregion

        public static Tuple<byte[], ushort[]> EncodeFromBitmap(System.Drawing.Bitmap bitmap, TextureFormats Format, PaletteFormats PaletteFormat = PaletteFormats.RGB565)
        {
            byte[] m_rgbaImageData = new byte[bitmap.Width * bitmap.Height * 4];

            int width = bitmap.Width;
            int height = bitmap.Height;

            BitmapData dat = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy(dat.Scan0, m_rgbaImageData, 0, m_rgbaImageData.Length);
            bitmap.UnlockBits(dat);
            bitmap.Dispose();

            return EncodeData(m_rgbaImageData, Format, PaletteFormat, width, height);
        }

        #region Encoding
        public static Tuple<byte[], ushort[]> EncodeData(byte[] m_rgbaImageData, TextureFormats Format, PaletteFormats PaletteFormat, int Width, int Height)
        {
            switch (Format)
            {
                case TextureFormats.I4:
                    return new Tuple<byte[], ushort[]>(ImageDataFormat.I4.ConvertTo(m_rgbaImageData, Width, Height, null), new ushort[0]);
                case TextureFormats.I8:
                    return new Tuple<byte[], ushort[]>(ImageDataFormat.I8.ConvertTo(m_rgbaImageData, Width, Height, null), new ushort[0]);
                case TextureFormats.IA4:
                    return new Tuple<byte[], ushort[]>(ImageDataFormat.IA4.ConvertTo(m_rgbaImageData, Width, Height, null), new ushort[0]);
                case TextureFormats.IA8:
                    return new Tuple<byte[], ushort[]>(ImageDataFormat.IA8.ConvertTo(m_rgbaImageData, Width, Height, null), new ushort[0]);
                case TextureFormats.RGB565:
                    return new Tuple<byte[], ushort[]>(ImageDataFormat.RGB565.ConvertTo(m_rgbaImageData, Width, Height, null), new ushort[0]);
                case TextureFormats.RGB5A3:
                    return new Tuple<byte[], ushort[]>(ImageDataFormat.RGB5A3.ConvertTo(m_rgbaImageData, Width, Height, null), new ushort[0]);
                case TextureFormats.RGBA32:
                    return new Tuple<byte[], ushort[]>(ImageDataFormat.Rgba32.ConvertTo(m_rgbaImageData, Width, Height, null), new ushort[0]);
                case TextureFormats.C4:
                    return EncodeC4(PaletteFormat, m_rgbaImageData, Width, Height);
                case TextureFormats.C8:
                    return EncodeC8(PaletteFormat, m_rgbaImageData, Width, Height);
                case TextureFormats.CMPR:
                    return new Tuple<byte[], ushort[]>(ImageDataFormat.Cmpr.ConvertTo(m_rgbaImageData, Width, Height, null), new ushort[0]);
                default:
                    return new Tuple<byte[], ushort[]>(new byte[0], new ushort[0]);
            }
        }

        private static Tuple<byte[], ushort[]> EncodeC4(PaletteFormats PaletteFormat, byte[] m_rgbaImageData, int Width, int Height)
        {
            List<Color32> palColors = new List<Color32>();

            uint numBlocksW = (uint)Width / 8;
            uint numBlocksH = (uint)Height / 8;

            byte[] pixIndices = new byte[numBlocksH * numBlocksW * 8 * 8];

            for (int i = 0; i < (Width * Height) * 4; i += 4)
                palColors.Add(new Color32(m_rgbaImageData[i + 2], m_rgbaImageData[i + 1], m_rgbaImageData[i + 0], m_rgbaImageData[i + 3]));

            List<ushort> rawColorData = new List<ushort>();
            Dictionary<Color32, byte> pixelColorIndexes = new Dictionary<Color32, byte>();
            foreach (Color32 col in palColors)
            {
                EncodeColor(PaletteFormat, col, rawColorData, pixelColorIndexes);
            }

            int pixIndex = 0;
            for (int yBlock = 0; yBlock < numBlocksH; yBlock++)
            {
                for (int xBlock = 0; xBlock < numBlocksW; xBlock++)
                {
                    for (int pY = 0; pY < 8; pY++)
                    {
                        for (int pX = 0; pX < 8; pX += 2)
                        {
                            byte color1 = (byte)(pixelColorIndexes[palColors[Width * ((yBlock * 8) + pY) + (xBlock * 8) + pX]] & 0xF);
                            byte color2 = (byte)(pixelColorIndexes[palColors[Width * ((yBlock * 8) + pY) + (xBlock * 8) + pX + 1]] & 0xF);
                            pixIndices[pixIndex] = (byte)(color1 << 4);
                            pixIndices[pixIndex++] |= color2;
                        }
                    }
                }
            }

         //   PaletteCount = (ushort)rawColorData.Count;
          //  PalettesEnabled = true;

            return new Tuple<byte[], ushort[]>(pixIndices, rawColorData.ToArray());
        }

        private static Tuple<byte[], ushort[]> EncodeC8(PaletteFormats PaletteFormat, byte[] m_rgbaImageData, int Width, int Height)
        {
            List<Color32> palColors = new List<Color32>();

            uint numBlocksW = (uint)Width / 8;
            uint numBlocksH = (uint)Height / 4;

            byte[] pixIndices = new byte[numBlocksH * numBlocksW * 8 * 4];

            for (int i = 0; i < (Width * Height) * 4; i += 4)
                palColors.Add(new Color32(m_rgbaImageData[i + 2], m_rgbaImageData[i + 1], m_rgbaImageData[i + 0], m_rgbaImageData[i + 3]));

            List<ushort> rawColorData = new List<ushort>();
            Dictionary<Color32, byte> pixelColorIndexes = new Dictionary<Color32, byte>();
            foreach (Color32 col in palColors)
            {
                EncodeColor(PaletteFormat, col, rawColorData, pixelColorIndexes);
            }

            int pixIndex = 0;
            for (int yBlock = 0; yBlock < numBlocksH; yBlock++)
            {
                for (int xBlock = 0; xBlock < numBlocksW; xBlock++)
                {
                    for (int pY = 0; pY < 4; pY++)
                    {
                        for (int pX = 0; pX < 8; pX++)
                        {
                            pixIndices[pixIndex++] = pixelColorIndexes[palColors[Width * ((yBlock * 4) + pY) + (xBlock * 8) + pX]];
                        }
                    }
                }
            }

         //   PaletteCount = (ushort)rawColorData.Count;
         //   PalettesEnabled = true;

            return new Tuple<byte[], ushort[]>(pixIndices, rawColorData.ToArray());
        }

        private static void EncodeColor(PaletteFormats PaletteFormat, Color32 col, List<ushort> rawColorData, Dictionary<Color32, byte> pixelColorIndexes)
        {
            switch (PaletteFormat)
            {
                case PaletteFormats.IA8:
                    byte i = (byte)((col.R * 0.2126) + (col.G * 0.7152) + (col.B * 0.0722));

                    ushort fullIA8 = (ushort)((i << 8) | (col.A));
                    if (!rawColorData.Contains(fullIA8))
                        rawColorData.Add(fullIA8);
                    if (!pixelColorIndexes.ContainsKey(col))
                        pixelColorIndexes.Add(col, (byte)rawColorData.IndexOf(fullIA8));
                    break;
                case PaletteFormats.RGB565:
                    ushort r_565 = (ushort)(col.R >> 3);
                    ushort g_565 = (ushort)(col.G >> 2);
                    ushort b_565 = (ushort)(col.B >> 3);

                    ushort fullColor565 = 0;
                    fullColor565 |= b_565;
                    fullColor565 |= (ushort)(g_565 << 5);
                    fullColor565 |= (ushort)(r_565 << 11);

                    if (!rawColorData.Contains(fullColor565))
                        rawColorData.Add(fullColor565);
                    if (!pixelColorIndexes.ContainsKey(col))
                        pixelColorIndexes.Add(col, (byte)rawColorData.IndexOf(fullColor565));
                    break;
                case PaletteFormats.RGB5A3:
                    ushort r_53 = (ushort)(col.R >> 4);
                    ushort g_53 = (ushort)(col.G >> 4);
                    ushort b_53 = (ushort)(col.B >> 4);
                    ushort a_53 = (ushort)(col.A >> 5);

                    ushort fullColor53 = 0;
                    fullColor53 |= b_53;
                    fullColor53 |= (ushort)(g_53 << 4);
                    fullColor53 |= (ushort)(r_53 << 8);
                    fullColor53 |= (ushort)(a_53 << 12);

                    if (!rawColorData.Contains(fullColor53))
                        rawColorData.Add(fullColor53);
                    if (!pixelColorIndexes.ContainsKey(col))
                        pixelColorIndexes.Add(col, (byte)rawColorData.IndexOf(fullColor53));
                    break;
            }
        }
        #endregion
    }
}
