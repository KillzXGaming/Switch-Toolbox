using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public class STPanel : Panel
    {
        public STPanel()
        {
            ForeColor = FormThemes.BaseTheme.FormForeColor;
            BackColor = FormThemes.BaseTheme.FormBackColor;
        }

        public void SetDoubleBuffer()
        {
            this.SetStyle(
             ControlStyles.AllPaintingInWmPaint |
             ControlStyles.UserPaint |
             ControlStyles.DoubleBuffer,
             true);
        }
    }
}
