using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Toolbox.Library.Forms
{
    public class STTabControl : FlatTabControl.FlatTabControl
    {
        public STTabControl() : base()
        {
            myBackColor = FormThemes.BaseTheme.FormBackColor;
            InitializeComponent();
        }

        private const int WM_SETREDRAW = 11;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);



        private void tabForms_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (TabPage tpCheck in TabPages)
            {
                tpCheck.BackColor = FormThemes.BaseTheme.TabPageInactive;
                tpCheck.ForeColor = FormThemes.BaseTheme.DisabledItemColor;
            }

            if (SelectedTab != null)
            {
                SelectedTab.BackColor = FormThemes.BaseTheme.TabPageActive;
                SelectedTab.ForeColor = FormThemes.BaseTheme.TextForeColor;
            }

            if ((SelectedTab != null) &&
                (SelectedTab.Tag != null))
            {
                SendMessage(this.Handle, WM_SETREDRAW, false, 0);
                (SelectedTab.Tag as Form).Select();
                SendMessage(this.Handle, WM_SETREDRAW, true, 0);
                this.Refresh();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // STTabControl
            // 
            this.SelectedIndexChanged += new System.EventHandler(this.tabForms_SelectedIndexChanged);
            this.Selected += new System.Windows.Forms.TabControlEventHandler(this.STTabControl_Selected);
            this.ResumeLayout(false);

        }

        private void STTabControl_Selected(object sender, TabControlEventArgs e)
        {
      
        }
    }
}
