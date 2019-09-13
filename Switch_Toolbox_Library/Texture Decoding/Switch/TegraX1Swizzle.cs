using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public class TegraX1Swizzle
    {
        public static List<uint[]> GenerateMipSizes(TEX_FORMAT Format, uint Width, uint Height, uint Depth, uint SurfaceCount, uint MipCount, uint ImageSize)
        {
            List<uint[]> MipMapSizes = new List<uint[]>();

            uint bpp = STGenericTexture.GetBytesPerPixel(Format);
            uint blkWidth = STGenericTexture.GetBlockWidth(Format);
            uint blkHeight = STGenericTexture.GetBlockHeight(Format);
            uint blkDepth = STGenericTexture.GetBlockDepth(Format);

            uint blockHeight = TegraX1Swizzle.GetBlockHeight(TegraX1Swizzle.DIV_ROUND_UP(Height, blkHeight));
            uint BlockHeightLog2 = (uint)Convert.ToString(blockHeight, 2).Length - 1;

            uint Pitch = 0;
            uint DataAlignment = 512;

            int linesPerBlockHeight = (1 << (int)BlockHeightLog2) * 8;

            uint ArrayCount = SurfaceCount;

            uint ArrayOffset = 0;
            for (int arrayLevel = 0; arrayLevel < ArrayCount; arrayLevel++)
            {
                uint SurfaceSize = 0;
                int blockHeightShift = 0;

                uint[] MipOffsets = new uint[MipCount];

                for (int mipLevel = 0; mipLevel < MipCount; mipLevel++)
                {
                    uint width = (uint)Math.Max(1, Width >> mipLevel);
                    uint height = (uint)Math.Max(1, Height >> mipLevel);
                    uint depth = (uint)Math.Max(1, Depth >> mipLevel);

                    uint size = TegraX1Swizzle.DIV_ROUND_UP(width, blkWidth) * TegraX1Swizzle.DIV_ROUND_UP(height, blkHeight) * bpp;

                    if (TegraX1Swizzle.pow2_round_up(TegraX1Swizzle.DIV_ROUND_UP(height, blkWidth)) < linesPerBlockHeight)
                        blockHeightShift += 1;

                    uint width__ = TegraX1Swizzle.DIV_ROUND_UP(width, blkWidth);
                    uint height__ = TegraX1Swizzle.DIV_ROUND_UP(height, blkHeight);

                    //Calculate the mip size instead
                    byte[] AlignedData = new byte[(TegraX1Swizzle.round_up(SurfaceSize, DataAlignment) - SurfaceSize)];
                    SurfaceSize += (uint)AlignedData.Length;
                    MipOffsets[mipLevel] = (SurfaceSize);

                    //Get the first mip offset and current one and the total image size
                    int msize = (int)((MipOffsets[0] + ImageSize - MipOffsets[mipLevel]) / ArrayCount);

                    Pitch = TegraX1Swizzle.round_up(width__ * bpp, 64);
                    SurfaceSize += Pitch * TegraX1Swizzle.round_up(height__, Math.Max(1, blockHeight >> blockHeightShift) * 8);
                }
                ArrayOffset += (uint)(ImageSize / ArrayCount);

                MipMapSizes.Add(MipOffsets);
            }

            return MipMapSizes;
        }

        public static byte[] GetImageData(STGenericTexture texture, byte[] ImageData, int ArrayLevel, int MipLevel, int target = 1, bool LinearTileMode = false)
        {
            uint blkHeight = STGenericTexture.GetBlockHeight(texture.Format);
            uint blkDepth = STGenericTexture.GetBlockDepth(texture.Format);
            uint blockHeight = TegraX1Swizzle.GetBlockHeight(TegraX1Swizzle.DIV_ROUND_UP(texture.Height, blkHeight));
            uint BlockHeightLog2 = (uint)Convert.ToString(blockHeight, 2).Length - 1;
            return GetImageData(texture, ImageData, ArrayLevel, MipLevel, BlockHeightLog2, target, LinearTileMode);
        }

        public static byte[] GetImageData(STGenericTexture texture, byte[] ImageData, int ArrayLevel, int MipLevel, uint BlockHeightLog2, int target = 1, bool LinearTileMode = false)
        {
            uint bpp = STGenericTexture.GetBytesPerPixel(texture.Format);
            uint blkWidth = STGenericTexture.GetBlockWidth(texture.Format);
            uint blkHeight = STGenericTexture.GetBlockHeight(texture.Format);
            uint blkDepth = STGenericTexture.GetBlockDepth(texture.Format);
            uint blockHeight = TegraX1Swizzle.GetBlockHeight(TegraX1Swizzle.DIV_ROUND_UP(texture.Height, blkHeight));

            uint Pitch = 0;
            uint DataAlignment = 512;
            uint TileMode = 0;
            if (LinearTileMode)
                TileMode = 1;

            int linesPerBlockHeight = (1 << (int)BlockHeightLog2) * 8;

            uint ArrayOffset = 0;
            for (int arrayLevel = 0; arrayLevel < texture.ArrayCount; arrayLevel++)
            {
                uint SurfaceSize = 0;
                int blockHeightShift = 0;

                List<uint> MipOffsets = new List<uint>();

                for (int mipLevel = 0; mipLevel < texture.MipCount; mipLevel++)
                {
                    uint width = (uint)Math.Max(1, texture.Width >> mipLevel);
                    uint height = (uint)Math.Max(1, texture.Height >> mipLevel);
                    uint depth = (uint)Math.Max(1, texture.Depth >> mipLevel);

                    uint size = TegraX1Swizzle.DIV_ROUND_UP(width, blkWidth) * TegraX1Swizzle.DIV_ROUND_UP(height, blkHeight) * bpp;

                    Console.WriteLine($"size " + size);

                    if (TegraX1Swizzle.pow2_round_up(TegraX1Swizzle.DIV_ROUND_UP(height, blkWidth)) < linesPerBlockHeight)
                        blockHeightShift += 1;


                    uint width__ = TegraX1Swizzle.DIV_ROUND_UP(width, blkWidth);
                    uint height__ = TegraX1Swizzle.DIV_ROUND_UP(height, blkHeight);

                    //Calculate the mip size instead
                    byte[] AlignedData = new byte[(TegraX1Swizzle.round_up(SurfaceSize, DataAlignment) - SurfaceSize)];
                    SurfaceSize += (uint)AlignedData.Length;
                    MipOffsets.Add(SurfaceSize);

                    //Get the first mip offset and current one and the total image size
                    int msize = (int)((MipOffsets[0] + ImageData.Length - MipOffsets[mipLevel]) / texture.ArrayCount);

                    byte[] data_ = Utils.SubArray(ImageData, ArrayOffset + MipOffsets[mipLevel], (uint)msize);

                    try
                    {
                        Pitch = TegraX1Swizzle.round_up(width__ * bpp, 64);
                        SurfaceSize += Pitch * TegraX1Swizzle.round_up(height__, Math.Max(1, blockHeight >> blockHeightShift) * 8);

                        byte[] result = TegraX1Swizzle.deswizzle(width, height, depth, blkWidth, blkHeight, blkDepth, target, bpp, TileMode, (int)Math.Max(0, BlockHeightLog2 - blockHeightShift), data_);
                        //Create a copy and use that to remove uneeded data
                        byte[] result_ = new byte[size];
                        Array.Copy(result, 0, result_, 0, size);
                        result = null;

                        if (ArrayLevel == arrayLevel && MipLevel == mipLevel)
                            return result_;
                    }
                    catch (Exception e)
                    {
                        System.Windows.Forms.MessageBox.Show($"Failed to swizzle texture {texture.Text}!");
                        Console.WriteLine(e);

                        return new byte[0];
                    }
                }

                ArrayOffset += (uint)(ImageData.Length / texture.ArrayCount);
            }
            return new byte[0];
        }


        /*---------------------------------------
         * 
         * Code ported from AboodXD's BNTX Extractor https://github.com/aboood40091/BNTX-Extractor/blob/master/swizzle.py
         * 
         *---------------------------------------*/

        public static uint GetBlockHeight(uint height)
        {
            uint blockHeight = pow2_round_up(height / 8);
            if (blockHeight > 16)
                blockHeight = 16;

            return blockHeight;
        }

        public static uint DIV_ROUND_UP(uint n, uint d)
        {
            return (n + d - 1) / d;
        }
        public static uint round_up(uint x, uint y)
        {
            return ((x - 1) | (y - 1)) + 1;
        }
        public static uint pow2_round_up(uint x)
        {
            x -= 1;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x + 1;
        }

        private static byte[] _swizzle(uint width, uint height, uint depth, uint blkWidth, uint blkHeight, uint blkDepth, int roundPitch, uint bpp, uint tileMode, int blockHeightLog2, byte[] data, int toSwizzle)
        {
            uint block_height = (uint)(1 << blockHeightLog2);

            Console.WriteLine($"Swizzle {width} {height} {blkWidth} {blkHeight} {roundPitch} {bpp} {tileMode} {blockHeightLog2} {data.Length} {toSwizzle}");

            width = DIV_ROUND_UP(width, blkWidth);
            height = DIV_ROUND_UP(height, blkHeight);
            depth = DIV_ROUND_UP(depth, blkDepth);

            uint pitch;
            uint surfSize;
            if (tileMode == 1)
            {
                pitch = width * bpp;

                if (roundPitch == 1)
                    pitch = round_up(pitch, 32);

                surfSize = pitch * height;
            }
            else
            {
                pitch = round_up(width * bpp, 64);
                surfSize = pitch * round_up(height, block_height * 8);
            }

            byte[] result = new byte[surfSize];

            for (uint y = 0; y < height; y++)
            {
                for (uint x = 0; x < width;     x++)
                {
                    uint pos;
                    uint pos_;

                    if (tileMode == 1)
                        pos = y * pitch + x * bpp;
                    else
                        pos = getAddrBlockLinear(x, y, width, bpp, 0, block_height);

                    pos_ = (y * width + x) * bpp;

                    if (pos + bpp <= surfSize)
                    {
                        if (toSwizzle == 0)
                            Array.Copy(data, pos, result, pos_, bpp);
                        else
                            Array.Copy(data, pos_, result, pos, bpp);
                    }
                }
            }
            return result;
        }

        public static byte[] deswizzle(uint width, uint height, uint depth, uint blkWidth, uint blkHeight, uint blkDepth, int roundPitch, uint bpp, uint tileMode, int size_range, byte[] data)
        {
            return _swizzle(width, height, depth, blkWidth, blkHeight, blkDepth, roundPitch, bpp, tileMode, size_range, data, 0);
        }

        public static byte[] swizzle(uint width, uint height, uint depth, uint blkWidth, uint blkHeight,uint blkDepth, int roundPitch, uint bpp, uint tileMode, int size_range, byte[] data)
        {
            return _swizzle(width, height, depth, blkWidth, blkHeight, blkDepth, roundPitch, bpp, tileMode, size_range, data, 1);
        }

        static uint getAddrBlockLinear(uint x, uint y, uint width, uint bytes_per_pixel, uint base_address, uint block_height)
        {
            /*
              From Tega X1 TRM 
                               */
            uint image_width_in_gobs = DIV_ROUND_UP(width * bytes_per_pixel, 64);


            uint GOB_address = (base_address
                                + (y / (8 * block_height)) * 512 * block_height * image_width_in_gobs
                                + (x * bytes_per_pixel / 64) * 512 * block_height
                                + (y % (8 * block_height) / 8) * 512);

            x *= bytes_per_pixel;

            uint Address = (GOB_address + ((x % 64) / 32) * 256 + ((y % 8) / 2) * 64
                            + ((x % 32) / 16) * 32 + (y % 2) * 16 + (x % 16));
            return Address;
        }
    }
}