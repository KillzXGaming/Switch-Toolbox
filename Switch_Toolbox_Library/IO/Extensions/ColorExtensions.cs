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
    }
}
