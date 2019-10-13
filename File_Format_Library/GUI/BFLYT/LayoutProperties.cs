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
    public partial class LayoutProperties : LayoutDocked
    {
        private UserDataEditor userDataEditor;

        public LayoutProperties()
        {
            InitializeComponent();
            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;

            userDataEditor = new UserDataEditor();
            userDataEditor.Dock = DockStyle.Fill;
            tabPage2.Controls.Add(userDataEditor);

            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;
        }

        public void Reset()
        {
            stPropertyGrid1.LoadProperty(null);
        }

        public void UpdateProperties()
        {
            stPropertyGrid1.UpdateProperties();
        }

        public void LoadProperties(object prop, Action propChanged)
        {
            LoadPropertyTab("Properties", prop, propChanged);
        }

        public void LoadProperties(BasePane prop, Action propChanged)
        {
            LoadPropertyTab("Pane", prop, propChanged);
        }

        private void LoadPropertyTab(string text, object prop, Action propChanged)
        {
            userDataEditor.Reset();
            if (prop is IUserDataContainer)
                LoadUserData((IUserDataContainer)prop);

            DoubleBufferedTabPage page = new DoubleBufferedTabPage();
            page.Enabled = false;
            page.Text = text;
            stPropertyGrid1.LoadProperty(prop, propChanged);
        }

        private void LoadUserData(IUserDataContainer container)
        {
            if (container.UserData != null && container.UserData.Entries != null)
                userDataEditor.LoadUserData(container as BasePane, container.UserData);
        }

        class DoubleBufferedTabPage : System.Windows.Forms.TabPage
        {
            public DoubleBufferedTabPage()
            {
                this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            }
        }

        private void stTabControl1_TabIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
