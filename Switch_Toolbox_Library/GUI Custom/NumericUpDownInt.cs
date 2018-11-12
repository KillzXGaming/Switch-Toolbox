using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switch_Toolbox.Library.Forms
{
    public class NumericUpDownInt : NumericUpDown
    {
        public NumericUpDownInt()
        {
            Maximum = 2147483647;
            Minimum = -2147483648;
        }
    }
}
