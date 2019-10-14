using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Toolbox.Library.IO
{
    public static class ColorExtensions
    {
        public static Color GrayScale(this Color color, bool removeAlpha = false)
        {
            int grayScale = (int)((color.R * 0.3) + (color.G * 0.59) + (color.B * 0.11));

            if (removeAlpha)
                return Color.FromArgb(255, grayScale, grayScale, grayScale);
            else
                return Color.FromArgb(color.A, grayScale, grayScale, grayScale);
        }

        public static Color Inverse(this Color color)
        {
            return Color.FromArgb(color.A, (byte)(255 - color.R), (byte)(255 - color.G), (byte)(255 - color.B));
        }

        public static Color WhiteAlpha(this Color color)
        {
            return Color.FromArgb(255, color.R, color.G, color.B);
        }

        public static Color Lighten(this Color color, int amount)
        {
            return Color.FromArgb(color.A, (byte)Math.Min(color.R + amount, 255), (byte)Math.Min(color.G + amount, 255), (byte)Math.Min(color.B + amount, 255));
        }

        public static Color Darken(this Color color, int amount)
        {
            return Color.FromArgb(color.A, (byte)Math.Max(color.R - amount, 0), (byte)Math.Max(color.G - amount, 0), (byte)Math.Max(color.B - amount, 0));
        }
    }
}
