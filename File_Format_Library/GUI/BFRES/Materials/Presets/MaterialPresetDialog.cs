using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class MaterialPresetDialog : STForm
    {
        public MaterialPresetDialog()
        {
            InitializeComponent();
        }

        private void MaterialPresetDialog_Load(object sender, EventArgs e) {
            ReloadMaterials();
        }

        public void ReloadMaterials()
        {
            string PresetPath = Path.Combine(Runtime.ExecutableDir, "Presets");
            string MaterialPath = Path.Combine(PresetPath, "Materials");

            foreach (var directory in Directory.GetDirectories(MaterialPath))
            {
                TreeNode GameFolder = new TreeNode(Path.GetFileName(directory));
                treeViewCustom1.Nodes.Add(GameFolder);

                foreach (var file in Directory.GetFiles(directory))
                {
                    TreeNode MaterailPreset = new TreeNode(Path.GetFileName(file));
                    MaterailPreset.ImageKey = "material";
                    MaterailPreset.SelectedImageKey = "material";

                    MaterailPreset.Tag = file;
                    GameFolder.Nodes.Add(MaterailPreset);
                }
            }
        }
    }
}
