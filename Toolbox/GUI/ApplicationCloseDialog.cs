using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox
{
    public partial class ApplicationCloseDialog : Form
    {
        public ApplicationCloseDialog()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void chkHideDialog_CheckedChanged(object sender, EventArgs e)
        {
            Toolbox.Library.Runtime.ShowCloseDialog = !chkHideDialog.Checked;
        }
    }
}
