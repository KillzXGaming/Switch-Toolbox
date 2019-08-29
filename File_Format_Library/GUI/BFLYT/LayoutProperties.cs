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

namespace FirstPlugin.Forms
{
    public partial class LayoutProperties : UserControl
    {
        public LayoutProperties()
        {
            InitializeComponent();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
        }

        public void LoadProperties(BFLYT.BasePane prop, Action propChanged)
        {
            stTabControl1.Controls.Clear();

            if (prop is BFLYT.PIC1)
            {
                LoadPropertyTab("Pane", prop, propChanged);
                LoadPropertyTab("Materials", ((BFLYT.PIC1)prop).GetMaterial(), propChanged);
            }
            else
                LoadPropertyTab("Pane", prop, propChanged);
        }

        private void LoadPropertyTab(string text, object prop, Action propChanged)
        {
            TabPage page = new TabPage();
            page.Text = text;
            var propGrid = new STPropertyGrid();
            propGrid.Dock = DockStyle.Fill;
            propGrid.LoadProperty(prop, propChanged);
            page.Controls.Add(propGrid);
            stTabControl1.Controls.Add(page);
        }
    }
}
