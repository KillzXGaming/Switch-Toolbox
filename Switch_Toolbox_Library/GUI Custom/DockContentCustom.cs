using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeifenLuo.WinFormsUI.Docking;
using WeifenLuo.WinFormsUI.ThemeVS2015;

namespace Switch_Toolbox.Library.Forms
{
    public class DockPanelCustom : DockPanel
    {
        public DockPanelCustom()
        {
            var theme = new VS2015DarkTheme();
            this.Theme = theme;
        }
    }
}
