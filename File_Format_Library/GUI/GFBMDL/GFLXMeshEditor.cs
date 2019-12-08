using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirstPlugin.Forms
{
    public partial class GFLXMeshEditor : UserControl
    {
        private bool Loaded = false;
        private GFLXMesh ActiveMesh;

        public GFLXMeshEditor()
        {
            InitializeComponent();

            stDropDownPanel1.ResetColors();
        }

        public void LoadMesh(GFLXMesh mesh)
        {
            polyGroupCB.Items.Clear();
            materialCB.Items.Clear();

            ActiveMesh = mesh;
            Loaded = false;

            var materials = mesh.ParentModel.GenericMaterials;
            for (int i = 0; i < materials.Count; i++)
                materialCB.Items.Add(materials[i].Text);

            for (int i = 0; i < mesh.PolygonGroups.Count; i++) {
                polyGroupCB.Items.Add($"{i}");
            }

            polyGroupCB.SelectedIndex = 0;

            Loaded = true;
        }

        private void polyGroupCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = polyGroupCB.SelectedIndex;
            if (index >= 0)
            {
                var poly = ActiveMesh.PolygonGroups[index];
                var mat = ActiveMesh.ParentModel.GenericMaterials[poly.MaterialIndex];
                materialCB.SelectedItem = mat.Text;
            }
        }

        private void materialCB_SelectedIndexChanged(object sender, EventArgs e) {
            if (!Loaded) return;

            int index = polyGroupCB.SelectedIndex;
            if (index >= 0)
            {
                var poly = ActiveMesh.PolygonGroups[index];
                var materials = ActiveMesh.ParentModel.GenericMaterials;
                var mappedMat = materials.First(x => x.Text == materialCB.GetSelectedText());
                if (mappedMat != null) {
                    var matIndex = materials.IndexOf(mappedMat);
                    poly.MaterialIndex = matIndex;
                    ActiveMesh.MeshData.Polygons[index].MaterialIndex = (uint)matIndex;

                    Toolbox.Library.LibraryGUI.UpdateViewport();
                }
            }
        }
    }
}
