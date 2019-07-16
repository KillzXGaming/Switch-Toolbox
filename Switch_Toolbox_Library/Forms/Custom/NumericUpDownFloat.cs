using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Toolbox.Library.Forms
{
    public class NumericUpDownFloat : STNumbericUpDown
    {
        public NumericUpDownFloat()
        {
            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;

            Maximum = 1000000000;
            Minimum = -100000000;
            DecimalPlaces = 5;
            Increment = 0.005m;
        }
    }
}
