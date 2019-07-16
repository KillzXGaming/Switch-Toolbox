using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class ViewportDivider : UserControl
    {
        public ViewportDivider()
        {
            InitializeComponent();

            viewportPanel.Width = Runtime.ViewportEditor.Width;
        }

        public Viewport GetActiveViewport()
        {
            foreach (var control in viewportPanel.Controls)
            {
                if (control is Viewport)
                    return (Viewport)control;
            }
            return null;
        }
    }
}
