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
using Toolbox.Library.IO;

namespace LayoutBXLYT.CTR
{
    public partial class PaneMatCTRTevEditor : EditorPanelBase
    {
        private PaneEditor ParentEditor;
        private Material ActiveMaterial;

        public PaneMatCTRTevEditor()
        {
            InitializeComponent();
        }

        public void LoadMaterial(Material material, PaneEditor paneEditor)
        {
            ParentEditor = paneEditor;
            ActiveMaterial = material;

            stPropertyGrid1.LoadProperty(null);
            UpdateMaterial(0);
        }

        private void UpdateMaterial(int stageIndex)
        {
            tevStageCB.Items.Clear();

            stageCounterLbl.Text = $"Stage 0 of {ActiveMaterial.TevStages.Length}";
            for (int i = 0; i < ActiveMaterial.TevStages.Length; i++)
                tevStageCB.Items.Add($"Stage[[{i}]");

            if (tevStageCB.Items.Count > stageIndex)
                tevStageCB.SelectedIndex = stageIndex;
        }

        private void tevStageCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tevStageCB.SelectedIndex == -1)
                return;

            int index = tevStageCB.SelectedIndex;
            stageCounterLbl.Text = $"Stage {index + 1} of {ActiveMaterial.TevStages.Length}";

            LoadTevStage((TevStage)ActiveMaterial.TevStages[index]);
        }

        private void LoadTevStage(TevStage stage)
        {
            stPropertyGrid1.LoadProperty(stage, OnPropertyChanged);
        }

        private void OnPropertyChanged()
        {
            ParentEditor.PropertyChanged?.Invoke(EventArgs.Empty, null);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (ActiveMaterial.TevStages.Length < 3)
            {
                ActiveMaterial.TevStages = ActiveMaterial.TevStages.AddToArray(new TevStage());
                UpdateMaterial(ActiveMaterial.TevStages.Length - 1);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            int index = tevStageCB.SelectedIndex;
            if (index != -1)
            {
                ActiveMaterial.TevStages = ActiveMaterial.TevStages.RemoveAt(index);
                UpdateMaterial(ActiveMaterial.TevStages.Length - 1);

                if (ActiveMaterial.TevStages.Length == 0)
                {
                    tevStageCB.Items.Clear();
                    tevStageCB.SelectedIndex = -1;
                    stPropertyGrid1.LoadProperty(null);
                }
            }
        }
    }
}
