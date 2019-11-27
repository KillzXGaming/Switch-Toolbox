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
    public partial class UVEditorAdvanced : UserControl
    {
        public UVViewport Viewport;

        public UVEditorAdvanced()
        {
            InitializeComponent();

            Viewport = new UVViewport();
            Viewport.Dock = DockStyle.Fill;
            Controls.Add(Viewport);
        }
    }
}
