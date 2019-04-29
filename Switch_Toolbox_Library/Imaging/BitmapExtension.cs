using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            return ResizeImage(original, width, height);

            //  return new Bitmap(original, new Size(width, height));
        }

        public static Bitmap SwapBlueRedChannels(Image image)
        {
            Bitmap b = new Bitmap(image);

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
    ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width * 4;

                byte red, green, blue, alpha;

                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];
                        alpha = p[3];

                        p[0] = red;
                        p[1] = green;
                        p[2] = blue;
                        p[3] = alpha;

                        p += 4;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);

            return b;
        }

        public static Bitmap ResizeImage(Image image, int width, int height,
            InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic,
            SmoothingMode smoothingMode = SmoothingMode.HighQuality)
        {
            if (width == 0) width = 1;
            if (height == 0) height = 1;

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = interpolationMode;
                graphics.SmoothingMode = smoothingMode;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
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

        public static Bitmap SetChannel(Bitmap b,
            STChannelType channelR,
            STChannelType channelG,
            STChannelType channelB,
            STChannelType channelA)
        {
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
 ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width * 4;

                byte red, green, blue, alpha;

                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];
                        alpha = p[3];

                        p[2] = SetChannelByte(channelR, red, green, blue, alpha);
                        p[1] = SetChannelByte(channelG, red, green, blue, alpha);
                        p[0] = SetChannelByte(channelB, red, green, blue, alpha);
                        p[3] = SetChannelByte(channelA, red, green, blue, alpha);

                        p += 4;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);

            return b;
        }

        private static byte SetChannelByte(STChannelType channel, byte r, byte g, byte b, byte a)
        {
            switch (channel) {
                case STChannelType.Red: return r;
                case STChannelType.Green: return g;
                case STChannelType.Blue: return b;
                case STChannelType.Alpha: return a;
                case STChannelType.One: return 255;
                case STChannelType.Zero: return 0;
                default:
                    throw new Exception("Unknown channel type! "  + channel);
            }
        }

        public static Bitmap ShowChannel(Bitmap b, STChannelType channel)
        {
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
 ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width * 4;

                byte red, green, blue, alpha;

                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];
                        alpha = p[3];

                        if (channel == STChannelType.Red)
                        {
                            p[0] = red;
                            p[1] = red;
                            p[2] = red;
                            p[3] = 255;
                        }
                        else if (channel == STChannelType.Green)
                        {
                            p[0] = green;
                            p[1] = green;
                            p[2] = green;
                            p[3] = 255;
                        }
                        else if (channel == STChannelType.Blue)
                        {
                            p[0] = blue;
                            p[1] = blue;
                            p[2] = blue;
                            p[3] = 255;
                        }
                        else if (channel == STChannelType.Alpha)
                        {
                            p[0] = alpha;
                            p[1] = alpha;
                            p[2] = alpha;
                            p[3] = 255;
                        }

                        p += 4;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);

            return b;
        }


        public static bool SetChannels(Bitmap b, bool UseRed, bool UseBlue, bool UseGreen, bool UseAlpha)
        {
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
     ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width * 4;

                byte red, green, blue, alpha;

                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];
                        alpha = p[3];

                        if (!UseRed)
                            red = 0;
                        if (!UseGreen)
                            green = 0;
                        if (!UseBlue)
                            blue = 0;
                        if (!UseAlpha)
                            alpha = 0;
                        
                        p[2] = red;
                        p[1] = green;
                        p[0] = blue;
                        p[3] = alpha;

                        p += 4;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);

            return true;
        }
        public static Bitmap GrayScale(Image b) { return GrayScale(new Bitmap(b));
        }
        public static Bitmap GrayScale(Bitmap b)
        {
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
        ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width * 4;

                byte red, green, blue, alpha;

                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        blue = p[0];
                        green = p[1];
                        red = p[2];
                        alpha = p[3];

                        p[0] = p[1] = p[2] = (byte)(.299 * red
                            + .587 * green
                            + .114 * blue);

                        p += 4;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);

            return b;
        }
        public static bool Invert(Bitmap b)
        {
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;
            unsafe
            {
                byte* p = (byte*)(void*)Scan0;
                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width * 3;
                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        p[0] = (byte)(255 - p[0]);
                        ++p;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);

            return true;
        }


        public static Bitmap HueStaturationBrightnessScale(Bitmap image, 
            bool EditHue, bool EditSaturation, bool EditBrightness,
            float HueScale = 255, float SaturationScale = 0.5f, float BrightnessScale = 0.5f)
        {
            Bitmap b = new Bitmap(image);

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
    ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* pointer = (byte*)(void*)Scan0;

                int bytesPerPixel = 4;

                int nOffset = stride - b.Width * bytesPerPixel;

                byte red, green, blue, alpha;

                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        blue = pointer[0];
                        green = pointer[1];
                        red = pointer[2];
                        alpha = pointer[3];


                        double hue, sat, val;

                        ColorToHSV(Color.FromArgb(alpha, red, green, blue), out hue, out sat, out val);

                        var color = ColorFromHSV(hue * HueScale, sat * SaturationScale, val * BrightnessScale);

                        pointer[2] = color.R;
                        pointer[1] = color.G;
                        pointer[0] = color.B;
                        pointer[3] = alpha;

                        pointer += bytesPerPixel;
                    }
                    pointer += nOffset;
                }
            }

            b.UnlockBits(bmData);

            return b;
        }

        public static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            hue = color.GetHue();
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }

        public static void RgbToHls(int r, int g, int b,
    out double h, out double l, out double s)
        {
            // Convert RGB to a 0.0 to 1.0 range.
            double double_r = r / 255.0;
            double double_g = g / 255.0;
            double double_b = b / 255.0;

            // Get the maximum and minimum RGB components.
            double max = double_r;
            if (max < double_g) max = double_g;
            if (max < double_b) max = double_b;

            double min = double_r;
            if (min > double_g) min = double_g;
            if (min > double_b) min = double_b;

            double diff = max - min;
            l = (max + min) / 2;
            if (Math.Abs(diff) < 0.00001)
            {
                s = 0;
                h = 0;  // H is really undefined.
            }
            else
            {
                if (l <= 0.5) s = diff / (max + min);
                else s = diff / (2 - max - min);

                double r_dist = (max - double_r) / diff;
                double g_dist = (max - double_g) / diff;
                double b_dist = (max - double_b) / diff;

                if (double_r == max) h = b_dist - g_dist;
                else if (double_g == max) h = 2 + r_dist - b_dist;
                else h = 4 + g_dist - r_dist;

                h = h * 60;
                if (h < 0) h += 360;
            }
        }

        // Convert an HLS value into an RGB value.
        public static void HlsToRgb(double h, double l, double s,
            out int r, out int g, out int b)
        {
            double p2;
            if (l <= 0.5) p2 = l * (1 + s);
            else p2 = l + s - l * s;

            double p1 = 2 * l - p2;
            double double_r, double_g, double_b;
            if (s == 0)
            {
                double_r = l;
                double_g = l;
                double_b = l;
            }
            else
            {
                double_r = QqhToRgb(p1, p2, h + 120);
                double_g = QqhToRgb(p1, p2, h);
                double_b = QqhToRgb(p1, p2, h - 120);
            }

            // Convert RGB to the 0 to 255 range.
            r = (int)(double_r * 255.0);
            g = (int)(double_g * 255.0);
            b = (int)(double_b * 255.0);
        }

        private static double QqhToRgb(double q1, double q2, double hue)
        {
            if (hue > 360) hue -= 360;
            else if (hue < 0) hue += 360;

            if (hue < 60) return q1 + (q2 - q1) * hue / 60;
            if (hue < 180) return q2;
            if (hue < 240) return q1 + (q2 - q1) * (240 - hue) / 60;
            return q1;
        }

        private static void ConvertBgraToRgba(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i += 4)
            {
                var temp = bytes[i];
                bytes[i] = bytes[i + 2];
                bytes[i + 2] = temp;
            }
        }

        public static Bitmap AdjustGamma(Image image, float gamma)
        {
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetGamma(gamma);

            Point[] points =
            {
            new Point(0, 0),
            new Point(image.Width, 0),
            new Point(0, image.Height),
           };
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);

            Bitmap bm = new Bitmap(image.Width, image.Height);
            using (Graphics gr = Graphics.FromImage(bm))
            {
                gr.DrawImage(image, points, rect,
                    GraphicsUnit.Pixel, attributes);
            }
            return bm;
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
