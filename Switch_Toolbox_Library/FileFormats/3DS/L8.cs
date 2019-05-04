using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Switch_Toolbox.Library
{
    public class L8
    {
        public static byte[] Decompress(byte[] Input, int Width, int Height, bool Alpha)
        {
            byte[] Output = new byte[Width * Height * 4];

            int bpp = 16;
            int Increment = 16 / 8;

            int IOffset = 0;

            for (int TY = 0; TY < Height; TY += 8)
            {
                for (int TX = 0; TX < Width; TX += 8)
                {
                    for (int Px = 0; Px < 64; Px++)
                    {
                        int X = Swizzle_3DS.SwizzleLUT[Px] & 7;
                        int Y = (Swizzle_3DS.SwizzleLUT[Px] - X) >> 3;

                        int OOffet = (TX + X + ((Height - 1 - (TY + Y)) * Width)) * 4;

                        Output[OOffet + 0] = Input[IOffset];
                        Output[OOffet + 1] = Input[IOffset];
                        Output[OOffet + 2] = Input[IOffset];
                        Output[OOffet + 3] = 0xff;

                        IOffset += Increment;
                    }
                }
            }

            return Output;
        }
    }
}
