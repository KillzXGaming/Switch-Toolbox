using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bfres.Structs;

namespace FirstPlugin
{
    public partial class BfresLODMeshEditor : Form
    {
        public BfresLODMeshEditor()
        {
            InitializeComponent();
        }
        FSHP ActiveShape;
        public void LoadLODS(FSHP fshp)
        {
            meshListView.Items.Clear();
            subMeshListView.Items.Clear();
            subMeshListView.FullRowSelect = true;
            meshListView.FullRowSelect = true;

            ActiveShape = fshp;

            for (int i = 0; i < fshp.lodMeshes.Count; i++)
            {
                meshListView.Items.Add($"mesh {i}");
            }
        }

        private void meshListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (meshListView.SelectedItems.Count > 0)
            {
                var lod = ActiveShape.lodMeshes[meshListView.SelectedIndices[0]];
                radiusUD.Value = (decimal)ActiveShape.boundingRadius[meshListView.SelectedIndices[0]];


                subMeshListView.Items.Clear();
                for (int i = 0; i < lod.subMeshes.Count; i++)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = lod.subMeshes[i].offset.ToString();
                    item.SubItems.Add(lod.subMeshes[i].size.ToString());
                    subMeshListView.Items.Add(item);
                }
                if (subMeshListView.Items.Count > 0)
                {
                    subMeshListView.Items[0].Selected = true;
                    subMeshListView.Select();
                }
            }
        }

        private void subMeshListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (subMeshListView.SelectedItems.Count > 0)
            {
                int MshIndx = meshListView.SelectedIndices[0];
                int SubIndx = subMeshListView.SelectedIndices[0];

                var center = ActiveShape.boundingBoxes[MshIndx + SubIndx].Center;
                var extend = ActiveShape.boundingBoxes[MshIndx + SubIndx].Extend;

                centXUD.Value = (decimal)center.X;
                centYUD.Value = (decimal)center.Y;
                centZUD.Value = (decimal)center.Z;
                extXUD.Value = (decimal)extend.X;
                extYUD.Value = (decimal)extend.Y;
                extZUD.Value = (decimal)extend.Z;
            }
        }
    }
}
