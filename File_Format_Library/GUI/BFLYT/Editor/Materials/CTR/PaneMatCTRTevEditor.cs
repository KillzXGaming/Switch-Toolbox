using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LayoutBXLYT.Revolution;

namespace LayoutBXLYT.CTR
{
    public partial class PaneMatCTRTevEditor : EditorPanelBase
    {
        private PaneEditor ParentEditor;
        private Material ActiveMaterial;

        private bool loaded = false;

        public PaneMatCTRTevEditor()
        {
            InitializeComponent();
        }

        public void LoadMaterial(Material material, PaneEditor paneEditor)
        {
            ParentEditor = paneEditor;
            ActiveMaterial = material;

            tevStageCB.Items.Clear();
            stPropertyGrid1.LoadProperty(null);

            stageCounterLbl.Text = $"Stage 0 of {material.TevStages.Length}";
            for (int i = 0; i < material.TevStages.Length; i++)
                tevStageCB.Items.Add($"Stage[[{i}]");

            if (tevStageCB.Items.Count > 0)
                tevStageCB.SelectedIndex = 0;
        }

        private void tevStageCB_SelectedIndexChanged(object sender, EventArgs e) {
            if (tevStageCB.SelectedIndex == -1)
                return;

            int index = tevStageCB.SelectedIndex;
            stageCounterLbl.Text = $"Stage {index + 1} of {ActiveMaterial.TevStages.Length}";

            LoadTevStage((TevStage)ActiveMaterial.TevStages[index]);
        }

        private void LoadTevStage(TevStage stage) {
            stPropertyGrid1.LoadProperty(stage, OnPropertyChanged);
        }

        private void OnPropertyChanged() {
            ParentEditor.PropertyChanged?.Invoke(EventArgs.Empty, null);
        }
    }
}
