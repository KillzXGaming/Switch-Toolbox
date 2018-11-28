using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPlugin
{
    public class TegraX1Swizzle
    {
        public struct Surface
        {
            public sbyte dim;
            public int depth;
            public ushort numMips;
            public uint format;
            public int imageSize;
            public sbyte tileMode;
            public ushort swizzle;
            public int alignment;
            public int sizeRange;

            public List<byte[]> data;

            public int[] mipOffset;


            public int width
            {
                set;
                get;
            }
            public int height
            {
                set;
                get;
            }
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

        public static byte[] _swizzle(uint width, uint height, uint blkWidth, uint blkHeight, int roundPitch, uint bpp, uint tileMode, int blockHeightLog2, byte[] data, int toSwizzle)
        {
            uint block_height = (uint)(1 << blockHeightLog2);

            Console.WriteLine($"Swizzle {width} {height} {blkWidth} {blkHeight} {roundPitch} {bpp} {tileMode} {blockHeightLog2} {data.Length} {toSwizzle}");

            width = DIV_ROUND_UP(width, blkWidth);
            height = DIV_ROUND_UP(height, blkHeight);

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
                for (uint x = 0; x < width; x++)
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

        public static byte[] deswizzle(uint width, uint height, uint blkWidth, uint blkHeight, int roundPitch, uint bpp, uint tileMode, int size_range, byte[] data)
        {
            return _swizzle(width, height, blkWidth, blkHeight, roundPitch, bpp, tileMode, size_range, data, 0);
        }

        public static byte[] swizzle(uint width, uint height, uint blkWidth, uint blkHeight, int roundPitch, uint bpp, uint tileMode, int size_range, byte[] data)
        {
            return _swizzle(width, height, blkWidth, blkHeight, roundPitch, bpp, tileMode, size_range, data, 1);
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