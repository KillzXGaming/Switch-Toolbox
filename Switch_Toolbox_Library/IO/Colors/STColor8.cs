using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Toolbox.Library
{
    //Class that contains colors and useful color related methods
    [Editor(typeof(Toolbox.Library.IO.ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class STColor8
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

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

        public STColor16 ToColor16() {
            return STColor16.FromShorts(new ushort[4] { R, G, B, A });
        }

        public static STColor8 FromBytes(byte[] color)
        {
            STColor8 col = new STColor8();
            col.R = color[0];
            col.G = color[1];
            col.B = color[2];
            col.A = color[3];
            return col;
        }

        public STColor8()
        {
            R = 255;
            G = 255;
            B = 255;
            A = 255;
        }

        public STColor8(Color color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }

        public STColor8(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public byte[] ToBytes()
        {
            return new byte[] { R, G, B, A, };
        }

        public override string ToString()
        {
            return String.Format("R:{0} G:{1} B:{2} A:{3}", R, G, B, A);
        }

        public string ToHexString()
        {
            return String.Format("R:{0:X2} G:{1:X2} B:{2:X2} A:{3:X2}", R, G, B, A);
        }


        public static STColor8 Black
        {
            get { return new STColor8(0, 0, 0, 255); }
        }

        public static STColor8 White
        {
            get { return new STColor8(255, 255, 255, 255); }
        }
    }
}
