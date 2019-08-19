using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Toolbox.Library.IO
{
    //Based on
    //https://github.com/libertyernie/brawltools/blob/40d7431b1a01ef4a0411cd69e51411bd581e93e2/BrawlLib/Imaging/PixelTypes.cs
    public class HSVPixel
    {
        public ushort H { get; set; }
        public byte   S { get; set; }
        public byte   V { get; set; }

        public HSVPixel(ushort h, byte s, byte v) { H = h; S = s; V = v; }

        public static HSVPixel FromRGBA(Color p)
        {
            HSVPixel outp = new HSVPixel(0,0,0);

            int min = Math.Min(Math.Min(p.R, p.G), p.B);
            int max = Math.Max(Math.Max(p.R, p.G), p.B);
            int diff = max - min;

            if (diff == 0)
            {
                outp.H = 0;
                outp.S = 0;
            }
            else
            {
                if (max == p.R)
                    outp.H = (ushort)((60 * ((float)(p.G - p.B) / diff) + 360) % 360);
                else if (max == p.G)
                    outp.H = (ushort)(60 * ((float)(p.B - p.R) / diff) + 120);
                else
                    outp.H = (ushort)(60 * ((float)(p.R - p.G) / diff) + 240);

                if (max == 0)
                    outp.S = 0;
                else
                    outp.S = (byte)(diff * 100 / max);
            }

            outp.V = (byte)(max * 100 / 255);

            return outp;
        }

        public Color ToRGBA()
        {
            var color = new Color();

            byte v = (byte)(V * 255 / 100);
            if (S == 0)
                color = Color.FromArgb(255,v,v,v);
            else
            {
                int h = (H / 60) % 6;
                float f = (H / 60.0f) - (H / 60);

                byte p = (byte)(V * (100 - S) * 255 / 10000);
                byte q = (byte)(V * (100 - (int)(f * S)) * 255 / 10000);
                byte t = (byte)(V * (100 - (int)((1.0f - f) * S)) * 255 / 10000);

                switch (h)
                {
                    case 0: color  = Color.FromArgb(255, v, t, p); break;
                    case 1: color  = Color.FromArgb(255, q, v, p); break;
                    case 2: color  = Color.FromArgb(255, p, v, t); break;
                    case 3: color  = Color.FromArgb(255, p, q, v); break;
                    case 4: color  = Color.FromArgb(255, t, p, v); break;
                    default: color = Color.FromArgb(255, v, p, q); break;
                }
            }

             return color;
        }
    }
}
