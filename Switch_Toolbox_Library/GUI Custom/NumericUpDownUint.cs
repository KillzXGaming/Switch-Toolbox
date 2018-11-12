using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switch_Toolbox.Library.Forms
{
    public class NumericUpDownUint : NumericUpDown
    {
        public NumericUpDownUint()
        {
            Maximum = 2147483647;
            Minimum = 0;
        }
    }
}
