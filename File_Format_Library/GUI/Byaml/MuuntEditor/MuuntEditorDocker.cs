using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using Toolbox.Library.Forms;

namespace FirstPlugin.MuuntEditor
{
    public class MuuntEditorDocker : DockContent
    {
        public MuuntEditorDocker()
        {
            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;
        }
    }
}
