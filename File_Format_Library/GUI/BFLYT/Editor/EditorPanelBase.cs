using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Forms;
using System.Windows.Forms;

namespace LayoutBXLYT
{
    public class EditorPanelBase : STUserControl
    {
        protected override System.Drawing.Point ScrollToControl(Control activeControl)
        {
            return DisplayRectangle.Location;
        }

        public virtual void SetUIState() { }
    }
}
