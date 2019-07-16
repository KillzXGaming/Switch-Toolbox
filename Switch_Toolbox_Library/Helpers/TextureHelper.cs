using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public class TextureHelper
    {
        public static Tuple<uint, uint> GetCurrentMipSize(uint width, uint height, uint blkWidth, uint blkHeight, uint bpp, int CurLevel)
        {
            uint offset = 0;
            uint width_ = 0;
            uint height_ = 0;

            for (int mipLevel = 0; mipLevel < CurLevel; mipLevel++)
            {
                width_ = DIV_ROUND_UP(Math.Max(1, width >> mipLevel), blkWidth);
                height_ = DIV_ROUND_UP(Math.Max(1, height >> mipLevel), blkHeight);

                offset += width_ * height_ * bpp;
            }

            width_ = DIV_ROUND_UP(Math.Max(1, width >> CurLevel), blkWidth);
            height_ = DIV_ROUND_UP(Math.Max(1, height >> CurLevel), blkHeight);

            uint size = width_ * height_ * bpp;
            return Tuple.Create(offset, size);

        }
        private static uint DIV_ROUND_UP(uint n, uint d)
        {
            return (n + d - 1) / d;
        }
    }
}
