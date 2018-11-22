using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Switch_Toolbox.Library
{
    public class BitmapExtension
    {
        public BitmapExtension()
        {

        }
        public static List<byte[]> GenerateMipMaps(Bitmap bitmap)
        {
            List<byte[]> datas = new List<byte[]>();

            datas.Add(ImageToByte(bitmap));
            while (bitmap.Width / 2 > 0 && bitmap.Height / 2 > 0)
            {
                bitmap = Resize(bitmap, bitmap.Width / 2, bitmap.Height / 2);
                datas.Add(ImageToByte(bitmap));
            }
            return datas;
        }
        public static Bitmap Resize(Image original, int width, int height)
        {
            return new Bitmap(original, new Size(width, height));
        }
        public static Bitmap GetBitmap(byte[] Buffer, int Width, int Height, PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
        {

            Rectangle Rect = new Rectangle(0, 0, Width, Height);

            Bitmap Img = new Bitmap(Width, Height, pixelFormat);

            BitmapData ImgData = Img.LockBits(Rect, ImageLockMode.WriteOnly, Img.PixelFormat);

            if (Buffer.Length > ImgData.Stride * Img.Height)
                throw new Exception($"Invalid Buffer Length ({Buffer.Length})!!!");

                Marshal.Copy(Buffer, 0, ImgData.Scan0, Buffer.Length);

            Img.UnlockBits(ImgData);

            return Img;
        }
        public class ColorSwapFilter
        {
            private ColorSwapType swapType = ColorSwapType.FixDDS;
            public ColorSwapType SwapType
            {
                get { return swapType; }
                set { swapType = value; }
            }

            private Red compRed = Red.Red;
            public Red CompRed
            {
                get { return compRed; }
                set { compRed = value; }
            }
            private Green comGreen = Green.Green;
            public Green CompGreen
            {
                get { return comGreen; }
                set { comGreen = value; }
            }
            private Blue compBlue = Blue.Blue;
            public Blue CompBlue
            {
                get { return compBlue; }
                set { compBlue = value; }
            }
            private Alpha compAlpha = Alpha.Alpha;
            public Alpha CompAlpha
            {
                get { return compAlpha; }
                set { compAlpha = value; }
            }

            private bool swapHalfColorValues = false;
            public bool SwapHalfColorValues
            {
                get { return swapHalfColorValues; }
                set { swapHalfColorValues = value; }
            }


            private bool invertColorsWhenSwapping = false;
            public bool InvertColorsWhenSwapping
            {
                get { return invertColorsWhenSwapping; }
                set { invertColorsWhenSwapping = value; }
            }

            public enum Red
            {
                Red,
                Green,
                Blue,
                Alpha,
                One,
                Zero,
            }
            public enum Green
            {
                Red,
                Green,
                Blue,
                Alpha,
                One,
                Zero,
            }
            public enum Blue
            {
                Red,
                Green,
                Blue,
                Alpha,
                One,
                Zero,
            }
            public enum Alpha
            {
                Red,
                Green,
                Blue,
                Alpha,
                One,
                Zero,
            }

            public enum ColorSwapType
            {
                FixDDS,
            }
        }
        public static Bitmap SwapRGB(Bitmap bitmap, ColorSwapFilter swapFilterData)
        {
            BitmapData sourceData = bitmap.LockBits
                                    (new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];
            Marshal.Copy(sourceData.Scan0, resultBuffer, 0, resultBuffer.Length);
            bitmap.UnlockBits(sourceData);

            byte sourceBlue = 0, resultBlue = 0,
                    sourceGreen = 0, resultGreen = 0,
                    sourceRed = 0, resultRed = 0,
                    sourceAlpha = 0, resultAlpha = 0;
            byte byte2 = 2, maxValue = 255;

            for (int k = 0; k < resultBuffer.Length; k += 4)
            {
                sourceRed = resultBuffer[k];
                sourceGreen = resultBuffer[k + 1];
                sourceBlue = resultBuffer[k + 2];
                sourceAlpha = resultBuffer[k + 3];



                switch (swapFilterData.SwapType)
                {
                    case ColorSwapFilter.ColorSwapType.FixDDS:
                        {
                            resultBlue = sourceRed;
                            resultRed = sourceBlue;
                            break;
                        }
                }

                switch (swapFilterData.CompRed)
                {
                    case ColorSwapFilter.Red.Red:
                        resultRed = sourceRed;
                        break;
                    case ColorSwapFilter.Red.Green:
                        resultRed = sourceGreen;
                        break;
                    case ColorSwapFilter.Red.Blue:
                        resultRed = sourceBlue;
                        break;
                    case ColorSwapFilter.Red.Alpha:
                        resultRed = sourceAlpha;
                        break;
                    case ColorSwapFilter.Red.One:
                        resultRed = 255;
                        break;
                    case ColorSwapFilter.Red.Zero:
                        resultRed = 0;
                        break;
                }
                switch (swapFilterData.CompGreen)
                {
                    case ColorSwapFilter.Green.Red:
                        resultGreen = sourceRed;
                        break;
                    case ColorSwapFilter.Green.Green:
                        resultGreen = sourceGreen;
                        break;
                    case ColorSwapFilter.Green.Blue:
                        resultGreen = sourceBlue;
                        break;
                    case ColorSwapFilter.Green.Alpha:
                        resultGreen = sourceAlpha;
                        break;
                    case ColorSwapFilter.Green.One:
                        resultGreen = 255;
                        break;
                    case ColorSwapFilter.Green.Zero:
                        resultGreen = 0;
                        break;
                }
                switch (swapFilterData.CompBlue)
                {
                    case ColorSwapFilter.Blue.Red:
                        resultBlue = sourceRed;
                        break;
                    case ColorSwapFilter.Blue.Green:
                        resultBlue = sourceGreen;
                        break;
                    case ColorSwapFilter.Blue.Blue:
                        resultBlue = sourceBlue;
                        break;
                    case ColorSwapFilter.Blue.Alpha:
                        resultBlue = sourceAlpha;
                        break;
                    case ColorSwapFilter.Blue.One:
                        resultBlue = 255;
                        break;
                    case ColorSwapFilter.Blue.Zero:
                        resultBlue = 0;
                        break;
                }
                switch (swapFilterData.CompAlpha)
                {
                    case ColorSwapFilter.Alpha.Red:
                        resultAlpha = sourceRed;
                        break;
                    case ColorSwapFilter.Alpha.Green:
                        resultAlpha = sourceGreen;
                        break;
                    case ColorSwapFilter.Alpha.Blue:
                        resultAlpha = sourceBlue;
                        break;
                    case ColorSwapFilter.Alpha.Alpha:
                        resultAlpha = sourceAlpha;
                        break;
                    case ColorSwapFilter.Alpha.One:
                        resultAlpha = 255;
                        break;
                    case ColorSwapFilter.Alpha.Zero:
                        resultAlpha = 0;
                        break;
                }

                resultBuffer[k] = resultRed;
                resultBuffer[k + 1] = resultGreen;
                resultBuffer[k + 2] = resultBlue;
                resultBuffer[k + 3] = resultAlpha;
            }


            Bitmap resultBitmap = new Bitmap(bitmap.Width, bitmap.Height,
                                                PixelFormat.Format32bppArgb);

            BitmapData resultData = resultBitmap.LockBits
                                    (new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height),
                                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);


            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            bitmap.Dispose();
            return resultBitmap;

        }
        public static byte[] ImageToByte(Bitmap bitmap)
        {
            BitmapData bmpdata = null;

            try
            {
                bmpdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int numbytes = bmpdata.Stride * bitmap.Height;
                byte[] bytedata = new byte[numbytes];
                IntPtr ptr = bmpdata.Scan0;

                Marshal.Copy(ptr, bytedata, 0, numbytes);

                return bytedata;
            }
            finally
            {
                if (bmpdata != null)
                    bitmap.UnlockBits(bmpdata);
            }
        }
    }
}
