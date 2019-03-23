using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library;
using Syroot.NintenTools.Bfres.GX2;

namespace FirstPlugin
{
    public class GTXSwizzle
    {
        public static GTX.GX2Surface CreateGx2Texture(byte[] imageData, GTXImporterSettings setting, uint tileMode, uint AAMode)
        {
            var Format = (GTX.GX2SurfaceFormat)setting.Format;

            Console.WriteLine("Format " + Format + " " + setting.TexName);

            var surfOut = GTX.getSurfaceInfo(Format, setting.TexWidth, setting.TexHeight, 1, 1, tileMode, 0, 0);
            uint imageSize = (uint)surfOut.surfSize;
            uint alignment = surfOut.baseAlign;
            uint pitch = surfOut.pitch;
            uint mipSize = 0;
            uint dataSize = (uint)imageData.Length;
            uint bpp = GTX.surfaceGetBitsPerPixel((uint)setting.Format) >> 3;

            if (dataSize <= 0)
                throw new Exception($"Image is empty!!");

            if (surfOut.depth != 1)
                throw new Exception($"Unsupported Depth {surfOut.depth}!");

            uint s = 0;
            switch (tileMode)
            {
                case 1:
                case 2:
                case 3:
                case 16:
                    s = 0;
                    break;
                default:
                    s = 0xd0000 | setting.swizzle << 8;
                    break;
            }
            uint blkWidth, blkHeight;
            if (GTX.IsFormatBCN(Format))
            {
                blkWidth = 4;
                blkHeight = 4;
            }
            else
            {
                blkWidth = 1;
                blkHeight = 1;
            }
            List<uint> mipOffsets = new List<uint>();
            List<byte[]> Swizzled = new List<byte[]>();

            for (int mipLevel = 0; mipLevel < setting.MipCount; mipLevel++)
            {
                var result = TextureHelper.GetCurrentMipSize(setting.TexWidth, setting.TexHeight, blkWidth, blkHeight, bpp, mipLevel);

                uint offset = result.Item1;
                uint size = result.Item2;

                Console.WriteLine("Swizzle Size " + size);
                Console.WriteLine("Swizzle offset " + offset);
                Console.WriteLine("bpp " + bpp);
                Console.WriteLine("TexWidth " + setting.TexWidth);
                Console.WriteLine("TexHeight " + setting.TexHeight);
                Console.WriteLine("blkWidth " + blkWidth);
                Console.WriteLine("blkHeight " + blkHeight);
                Console.WriteLine("mipLevel " + mipLevel);

                byte[] data_ = new byte[size];
                Array.Copy(imageData, offset, data_, 0, size);

                uint width_ = Math.Max(1, setting.TexWidth >> mipLevel);
                uint height_ = Math.Max(1, setting.TexHeight >> mipLevel);

                if (mipLevel != 0)
                {
                    surfOut = GTX.getSurfaceInfo(Format, setting.TexWidth, setting.TexHeight, 1, 1, tileMode, 0, mipLevel);

                    if (mipLevel == 1)
                        mipOffsets.Add(imageSize);
                    else
                        mipOffsets.Add(mipSize);
                }

                data_ = Utils.CombineByteArray(data_, new byte[surfOut.surfSize - size]);
                byte[] dataAlignBytes = new byte[RoundUp(mipSize, surfOut.baseAlign) - mipSize];

                if (mipLevel != 0)
                    mipSize += (uint)(surfOut.surfSize + dataAlignBytes.Length);

                byte[] SwizzledData = GTX.swizzle(width_, height_, surfOut.height, (uint)Format, surfOut.tileMode, s,
                        surfOut.pitch, surfOut.bpp, data_);

                Swizzled.Add(dataAlignBytes.Concat(SwizzledData).ToArray());
            }

            GTX.GX2Surface surf = new GTX.GX2Surface();
            surf.depth = setting.Depth;
            surf.width = setting.TexWidth;
            surf.height = setting.TexHeight;
            surf.depth = 1;
            surf.use = 1;
            surf.dim = (uint)setting.SurfaceDim;
            surf.tileMode = tileMode;
            surf.swizzle = s;
            surf.resourceFlags = 0;
            surf.pitch = pitch;
            surf.bpp = bpp;
            surf.format = (uint)setting.Format;
            surf.numMips = setting.MipCount;
            surf.aa = AAMode;
            surf.mipOffset = mipOffsets.ToArray();
            surf.numMips = (uint)Swizzled.Count;
            surf.alignment = alignment;
            surf.mipSize = mipSize;
            surf.imageSize = imageSize;
            surf.data = Swizzled[0];

            List<byte[]> mips = new List<byte[]>();
            for (int mipLevel = 1; mipLevel < Swizzled.Count; mipLevel++)
            {
                mips.Add(Swizzled[mipLevel]);
                Console.WriteLine(Swizzled[mipLevel].Length);
            }
            surf.mipData = Utils.CombineByteArray(mips.ToArray());
            mips.Clear();


            Console.WriteLine("");
            Console.WriteLine("// ----- GX2Surface Swizzled Info ----- ");
            Console.WriteLine("  dim             = 1");
            Console.WriteLine("  width           = " + surf.width);
            Console.WriteLine("  height          = " + surf.height);
            Console.WriteLine("  depth           = 1");
            Console.WriteLine("  numMips         = " + surf.numMips);
            Console.WriteLine("  format          = " + surf.format);
            Console.WriteLine("  aa              = 0");
            Console.WriteLine("  use             = 1");
            Console.WriteLine("  imageSize       = " + surf.imageSize);
            Console.WriteLine("  mipSize         = " + surf.mipSize);
            Console.WriteLine("  tileMode        = " + surf.tileMode);
            Console.WriteLine("  swizzle         = " + surf.swizzle);
            Console.WriteLine("  alignment       = " + surf.alignment);
            Console.WriteLine("  pitch           = " + surf.pitch);
            Console.WriteLine("  data            = " + surf.data.Length);
            Console.WriteLine("  mipData         = " + surf.mipData.Length);
            Console.WriteLine("");
            Console.WriteLine("  GX2 Component Selector:");
            Console.WriteLine("");
            Console.WriteLine("  bits per pixel  = " + (surf.bpp << 3));
            Console.WriteLine("  bytes per pixel = " + surf.bpp);
            Console.WriteLine("  realSize        = " + imageData.Length);

            return surf;
        }
        private static int RoundUp(int X, int Y)
        {
            return ((X - 1) | (Y - 1)) + 1;
        }
        private static uint RoundUp(uint X, uint Y)
        {
            return ((X - 1) | (Y - 1)) + 1;
        }
    }
}
