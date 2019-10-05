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
    public partial class PaneEditor : LayoutDocked
    {
        public PaneEditor()
        {
            InitializeComponent();
        }

        public void LoadPane(BasePane pane)
        {
            LoadBasePane(pane);
        }

        private void LoadBasePane(BasePane pane)
        {
            var page = new TabPage() { Text = "Base Pane" };
            var basePaneEditor = new BasePaneEditor();
            basePaneEditor.Dock = DockStyle.Fill;
            basePaneEditor.LoadPane(pane);
            page.Controls.Add(basePaneEditor);
            tabControl1.TabPages.Add(page);
        }
    }
}
