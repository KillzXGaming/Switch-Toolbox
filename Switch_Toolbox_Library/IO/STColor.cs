using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Toolbox.Library
{
    //Class that contains colors and useful color related methods
    public class STColor
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public Color Color
        {
            get
            {
                int red = Utils.FloatToIntClamp(R);
                int green = Utils.FloatToIntClamp(G);
                int blue = Utils.FloatToIntClamp(B);
                int alpha = Utils.FloatToIntClamp(A);

                return Color.FromArgb(alpha, red, green, blue);
            }
            set
            {
                var color = value;
                R = color.R / 255f;
                G = color.G / 255f;
                B = color.B / 255f;
                A = color.A / 255f;
            }
        }
            

        public STColor()
        {
            R = 1;
            G = 1;
            B = 1;
            A = 1;
        }
    }
}
