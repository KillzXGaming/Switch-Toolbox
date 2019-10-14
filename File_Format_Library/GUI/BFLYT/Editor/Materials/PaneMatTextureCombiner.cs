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
    public partial class PaneMatTextureCombiner : EditorPanelBase
    {
        private PaneEditor ParentEditor;
        private BxlytMaterial ActiveMaterial;

        public PaneMatTextureCombiner()
        {
            InitializeComponent();
        }

        public void LoadMaterial(BxlytMaterial material, PaneEditor paneEditor)
        {
            ActiveMaterial = material;
            ParentEditor = paneEditor;
        }
    }
}
