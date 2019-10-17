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
        private bool loaded = false;

        public PaneMatTextureCombiner()
        {
            InitializeComponent();
        }

        public void LoadMaterial(BxlytMaterial material, PaneEditor paneEditor)
        {
            ActiveMaterial = material;
            ParentEditor = paneEditor;

            tevStageCB.Items.Clear();
            tevColorModeCB.ResetBind();
            tevAlphaModeCB.ResetBind();

            if (material.TevStages?.Length > 0)
            {
                tevBasicPanel.Show();

                for (int i = 0; i < material.TevStages.Length; i++)
                    tevStageCB.Items.Add($"Stage [{i}]");

                tevStageCB.SelectedIndex = 0;
            }
            else
            {
                tevBasicPanel.Hide();
            }
        }

        private void tevStageCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tevStageCB.SelectedIndex >= 0)
            {
                int index = tevStageCB.SelectedIndex;
                var tevStage = ActiveMaterial.TevStages[index];
                tevColorModeCB.Bind(typeof(TevMode), tevStage, "ColorMode");
                tevAlphaModeCB.Bind(typeof(TevMode), tevStage, "AlphaMode");
                tevColorModeCB.SelectedItem = tevStage.ColorMode;
                tevAlphaModeCB.SelectedItem = tevStage.AlphaMode;
            }
        }

        private void tevColorModeCB_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateTevStage();
        }

        private void tevAlphaModeCB_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateTevStage();
        }

        private void UpdateTevStage()
        {
            if (!loaded || tevStageCB.SelectedIndex < 0) return;

            int index = tevStageCB.SelectedIndex;
            var tevStage = ActiveMaterial.TevStages[index];

            tevStage.ColorMode = (TevMode)tevColorModeCB.SelectedItem;
            tevStage.AlphaMode = (TevMode)tevColorModeCB.SelectedItem;
        }
    }
}
