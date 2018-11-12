using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Switch_Toolbox.Library.Forms
{
    public class NumericUpDownFloat : NumericUpDown
    {
        public NumericUpDownFloat()
        {
            Maximum = 1000000000;
            Minimum = -100000000;
            DecimalPlaces = 5;
            Increment = 0.005m;

            BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            ForeColor = Color.White;
        }
    }
}
