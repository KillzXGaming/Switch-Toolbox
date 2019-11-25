using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public class RGBAPixelDecoder
    {
        private static byte[] GetComponentsFromPixel(TEX_FORMAT format, int pixel, byte[] comp)
        {
            switch (format)
            {
                case TEX_FORMAT.L8:
                    comp[0] = (byte)(pixel & 0xFF);
                    break;
                case TEX_FORMAT.LA8:
                    comp[0] = (byte)(pixel & 0xFF);
                    comp[1] = (byte)((pixel & 0xFF00) >> 8);
                    break;
                case TEX_FORMAT.LA4:
                    comp[0] = (byte)((pixel & 0xF) * 17);
                    comp[1] = (byte)(((pixel & 0xF0) >> 4) * 17);
                    break;
                case TEX_FORMAT.B5G6R5_UNORM:
                    comp[0] = (byte)(((pixel & 0xF800) >> 11) / 0x1F * 0xFF);
                    comp[1] = (byte)(((pixel & 0x7E0) >> 5) / 0x3F * 0xFF);
                    comp[2] = (byte)((pixel & 0x1F) / 0x1F * 0xFF);
                    break;
                case TEX_FORMAT.R5G5B5_UNORM:
                    {
                        int R = ((pixel >> 0) & 0x1f) << 3;
                        int G = ((pixel >> 5) & 0x1f) << 3;
                        int B = ((pixel >> 10) & 0x1f) << 3;

                        comp[0] = (byte)(R | (R >> 5));
                        comp[1] = (byte)(G | (G >> 5));
                        comp[2] = (byte)(B | (B >> 5));
                    }
                    break;
                case TEX_FORMAT.R5G5B5A1_UNORM:
                    {
                        int R = ((pixel >> 0) & 0x1f) << 3;
                        int G = ((pixel >> 5) & 0x1f) << 3;
                        int B = ((pixel >> 10) & 0x1f) << 3;
                        int A = ((pixel & 0x8000) >> 15) * 0xFF;

                        comp[0] = (byte)(R | (R >> 5));
                        comp[1] = (byte)(G | (G >> 5));
                        comp[2] = (byte)(B | (B >> 5));
                        comp[3] = (byte)A;
                    }
                    break;
                case TEX_FORMAT.R8G8B8A8_UNORM:
                case TEX_FORMAT.R8G8B8A8_UNORM_SRGB:
                    comp[0] = (byte)(pixel & 0xFF);
                    comp[1] = (byte)((pixel & 0xFF00) >> 8);
                    comp[2] = (byte)((pixel & 0xFF0000) >> 16);
                    comp[3] = (byte)((pixel & 0xFF000000) >> 24);
                    break;
            }


            return comp;
        }

        //Method from https://github.com/aboood40091/BNTX-Editor/blob/master/formConv.py
        public static byte[] Decode(byte[] data, int width, int height, TEX_FORMAT format)
        {
            uint bpp = STGenericTexture.GetBytesPerPixel(format);
            int size = width * height * 4;

            bpp = (uint)(data.Length / (width * height));

            byte[] output = new byte[size];

            int inPos = 0;
            int outPos = 0;

            byte[] comp = new byte[] { 0, 0, 0, 0xFF, 0, 0xFF };
            byte[] compSel = new byte[4] {0,1,2,3 };

            if (format == TEX_FORMAT.LA8)
            {
                compSel = new byte[4] { 0, 0, 0, 1 };
                bpp = 2;
            }
            else if (format == TEX_FORMAT.L8)
                compSel = new byte[4] { 0, 0, 0, 5 };
            else if (format == TEX_FORMAT.R5G5B5A1_UNORM)
            {
                bpp = 2;
                return DDSCompressor.DecodePixelBlock(data, (int)width, (int)height, DDS.DXGI_FORMAT.DXGI_FORMAT_B5G5R5A1_UNORM);
            }

            for (int Y = 0; Y < height; Y++)
            {
                for (int X = 0; X < width; X++)
                {
                    inPos = (Y * width + X) * (int)bpp;
                    outPos = (Y * width + X) * 4;

                    int pixel = 0;
                    for (int i = 0; i < bpp; i++)
                        pixel |= data[inPos + i] << (8 * i);

                    comp = GetComponentsFromPixel(format, pixel, comp);

                    output[outPos + 3] = comp[compSel[3]];
                    output[outPos + 2] = comp[compSel[2]];
                    output[outPos + 1] = comp[compSel[1]];
                    output[outPos + 0] = comp[compSel[0]];
                }
            }

            return output;
        }
    }
}
