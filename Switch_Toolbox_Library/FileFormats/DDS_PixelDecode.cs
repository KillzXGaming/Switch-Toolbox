using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Switch_Toolbox.Library
{
    public class DDS_PixelDecode
    {
        public static Bitmap DecodeR8G8(Byte[] data, int width, int height)
        {
            byte[] Output = new byte[width * height * 4];

            int OOffset = 0;

            int W = (width + 3) / 4;
            int H = (height + 3) / 4;

            for (int Y = 0; Y < H; Y++)
            {
                for (int X = 0; X < W; X++)
                {
                    int IOffs = (Y * W + X) * 2;

                    Output[OOffset + 1] = data[IOffs + 1];
                    Output[OOffset + 2] = data[IOffs + 0];
                    Output[OOffset + 3] = 0xff;

                    OOffset += 4;
                }
            }

            return BitmapExtension.GetBitmap(Output, width, height);
        }
        public static Bitmap DecodeR8G8B8A8(Byte[] data, int width, int height)
        {
            byte[] Output = new byte[width * height * 4];

            int OOffset = 0;

            for (int Y = 0; Y < height; Y++)
            {
                for (int X = 0; X < width; X++)
                {
                    int IOffs = OOffset;

                    Output[OOffset + 0] = data[IOffs + 2];
                    Output[OOffset + 1] = data[IOffs + 1];
                    Output[OOffset + 2] = data[IOffs + 0];
                    Output[OOffset + 3] = data[IOffs + 3];

                    OOffset += 4;
                }
            }

            return BitmapExtension.GetBitmap(Output, width, height);
        }
        public static byte[] EncodeR8G8B8A8(Byte[] data, int width, int height, int Offset)
        {
            byte[] Output = new byte[width * height * 4];

            int OOffset = 0;

            for (int Y = 0; Y < height; Y++)
            {
                for (int X = 0; X < width; X++)
                {
                    int IOffs = (X * 4 + X + (Y * 4 + Y) * width * 4) * 4;

                    Output[OOffset + 0] = data[IOffs + 2];
                    Output[OOffset + 1] = data[IOffs + 1];
                    Output[OOffset + 2] = data[IOffs + 0];
                    Output[OOffset + 3] = data[IOffs + 3];

                    OOffset += 4;
                }
            }

            return Output;
        }
    }
}
