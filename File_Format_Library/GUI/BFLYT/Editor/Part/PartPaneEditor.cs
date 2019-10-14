using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LayoutBXLYT
{
    public partial class PartPaneEditor : EditorPanelBase
    {
        private PaneEditor ParentEditor;
        private IPartPane ActivePane;

        public PartPaneEditor()
        {
            InitializeComponent();
        }

        public void LoadPane(IPartPane pane, PaneEditor paneEditor)
        {
            ActivePane = pane;
            ParentEditor = paneEditor;

            stTextBox1.Bind(pane, "LayoutFileName");
        }
    }
}
