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
using LayoutBXLYT.Revolution;

namespace LayoutBXLYT
{
    public partial class PaneMatRevTevSwapTableEditor : EditorPanelBase
    {
        private PaneEditor ParentEditor;
        private BxlytMaterial ActiveMaterial;

        public SwapTableEntryControl[] SwapControls;

        public PaneMatRevTevSwapTableEditor()
        {
            InitializeComponent();

            SwapControls = new SwapTableEntryControl[4];
            for (int i = 0; i < 4; i++)
            {
                SwapControls[i] = new SwapTableEntryControl(new SwapMode());
                SwapControls[i].Dock = DockStyle.Fill;

                var stDropDownPanel1 = new STDropDownPanel();
                stDropDownPanel1.PanelName = $"SwapMode {i}";
                stDropDownPanel1.Height = SwapControls[i].Height;
                stDropDownPanel1.Controls.Add(SwapControls[i]);
                stFlowLayoutPanel1.Controls.Add(stDropDownPanel1);
            }
        }

        public void LoadMaterial(Revolution.Material material, PaneEditor paneEditor)
        {
            ParentEditor = paneEditor;
            ActiveMaterial = material;

            if (material.TevSwapModeTable != null)
            {
                var swapModes = material.TevSwapModeTable.SwapModes;
                //Note these are always 4 in length for RGBA
                for (int i = 0; i < swapModes?.Length; i++) {
                    SwapControls[i].LoadSwapMode(swapModes[i]);
                }
            }
        }

        private void CreatePanel(string name, int index, SwapMode swapMode)
        {
      
        }
    }
}
