using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public class STLabel : Label
    {
        public STLabel()
        {
            BackColor = FormThemes.BaseTheme.TextBackColor;
            ForeColor = FormThemes.BaseTheme.TextForeColor;
            AutoSize = true;
        }
    }
}
