using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class GenericEditorForm : STForm
    {
        public GenericEditorForm(bool UseSaveDialog, UserControl control)
        {
            control.Dock = DockStyle.Fill;
            InitializeComponent();
            stPanel1.Controls.Add(control);
            SetSaveDialog(UseSaveDialog);
        }

        public Control GetControl()
        {
            return stPanel1.Controls[0];
        }

        public void SetSaveDialog(bool UseSaveDialog)
        {
            if (!UseSaveDialog)
            {
                stButton1.Visible = false;
                stButton2.Visible = false;
                stPanel1.Dock = DockStyle.Fill;
            }
        }

        private void GenericEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var control in stPanel1.Controls)
            {
                if (control is STUserControl)
                    ((STUserControl)control).OnControlClosing();
            }
        }
    }
}
