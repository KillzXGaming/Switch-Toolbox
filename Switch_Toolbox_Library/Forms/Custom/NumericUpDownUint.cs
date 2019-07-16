using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public class NumericUpDownUint : STNumbericUpDown
    {
        public NumericUpDownUint()
        {
            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;

            Maximum = 2147483647;
            Minimum = 0;
        }
    }
}
