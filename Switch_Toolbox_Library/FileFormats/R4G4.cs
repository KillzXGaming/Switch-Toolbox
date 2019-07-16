using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public class R4G4
    {
        public static byte[] Decompress(byte[] Input, int Width, int Height, bool Alpha)
        {
            byte[] Output = new byte[Width * Height * 4];

            byte[] comp = new byte[4] { 0xFF, 0xFF, 0xFF, 0xFF };

            int bpp = (int)STGenericTexture.GetBytesPerPixel(TEX_FORMAT.R4G4_UNORM);

            for (int Y = 0; Y < Height; Y++)
            {
                for (int X = 0; X < Width; X++)
                {   
                    int InputOffset = (Y * Width + X) * bpp;
                    int OutputOffset = (Y * Width + X) * 4;

                    int pixel = 0;
                    for (int i = 0; i < bpp; i++)
                        pixel |= Input[InputOffset + i] << (8 * i);

                    comp[0] = (byte)((pixel & 0xF) * 17);
                    comp[1] = (byte)(((pixel & 0xF0) >> 4) * 17);

                    Output[OutputOffset + 0] = comp[0];
                    Output[OutputOffset + 1] = comp[1];
                    Output[OutputOffset + 2] = comp[2];
                    Output[OutputOffset + 3] = comp[3];
                }
            }

            return Output;
        }
    }
}
