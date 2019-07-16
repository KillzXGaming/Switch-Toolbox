using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class ParamValueDialog : STForm
    {
        public ParamValueDialog()
        {
            InitializeComponent();
        }

        public void AddControl(UserControl control)
        {
            stPanel1.Controls.Clear();
            stPanel1.Controls.Add(control);
        }
    }
}
