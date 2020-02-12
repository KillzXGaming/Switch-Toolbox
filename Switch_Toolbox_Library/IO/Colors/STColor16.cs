using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace Toolbox.Library
{
    //Class that contains colors and useful color related methods
    [Editor(typeof(Toolbox.Library.IO.ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class STColor16 : Toolbox.Library.IO.ColorEditor
    {
        public ushort R { get; set; }
        public ushort G { get; set; }
        public ushort B { get; set; }
        public ushort A { get; set; }

        public Color Color
        {
            get
            {
                return Color.FromArgb(A, R, G, B);
            }
            set
            {
                var color = value;
                R = color.R;
                G = color.G;
                B = color.B;
                A = color.A;
            }
        }

        public STColor8 ToColor8()
        {
            return STColor8.FromBytes(new byte[4] { (byte)R, (byte)G, (byte)B, (byte)A });
        }

        public ushort[] ToUShorts() {
            return new ushort[4] { R, G, B, A };
        }

        public static STColor16 FromShorts(ushort[] color)
        {
            STColor16 col = new STColor16();
            col.R = color[0];
            col.G = color[1];
            col.B = color[2];
            col.A = color[3];
            return col;
        }

        public STColor16()
        {
            R = 255;
            G = 255;
            B = 255;
            A = 255;
        }

        public STColor16(Color color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }

        public STColor16(ushort r, ushort g, ushort b, ushort a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public override string ToString()
        {
            return String.Format("R:{0} G:{1} B:{2} A:{3}", R, G, B, A);
        }

        public string ToHexString()
        {
            return String.Format("R:{0:X2} G:{1:X2} B:{2:X2} A:{3:X2}", R, G, B, A);
        }

        public static STColor8 White
        {
            get { return new STColor8(255, 255, 255, 255); }
        }
    }
}
