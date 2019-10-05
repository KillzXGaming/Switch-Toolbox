using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;

namespace LayoutBXLYT
{
    public partial class BasePaneEditor : STUserControl
    {
        public BasePaneEditor()
        {
            InitializeComponent();
        }

        public void LoadPane(BasePane pane)
        {
            nameTB.Bind(pane, "Name");
        }
    }
}
