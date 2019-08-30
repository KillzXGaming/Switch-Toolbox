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
        public LayoutProperties()
        {
            InitializeComponent();
        }

        public void Reset()
        {
            stPropertyGrid1.LoadProperty(null);
        }

        public void LoadProperties(BasePane prop, Action propChanged)
        {
            LoadPropertyTab("Pane", prop, propChanged);
        }

        private void LoadPropertyTab(string text, object prop, Action propChanged)
        {
            DoubleBufferedTabPage page = new DoubleBufferedTabPage();
            page.Enabled = false;
            page.Text = text;
            stPropertyGrid1.LoadProperty(prop, propChanged);
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
