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
using Bfres.Structs;

namespace FirstPlugin.Forms
{
    public partial class SubMeshEditor : STForm
    {
        public SubMeshEditor()
        {
            InitializeComponent();
        }

        FSHP.LOD_Mesh activeMesh;
        FSHP.LOD_Mesh.SubMesh activeSubMesh;
        FSHP activeShape;

        public void LoadMesh(FSHP.LOD_Mesh mesh, FSHP fshp)
        {
            activeShape = fshp;
            activeMesh = mesh;

            int index = 0;
            foreach (var subMesh in mesh.subMeshes)
                listViewCustom1.Items.Add($"Sub Mesh{index}");

        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0 && activeMesh != null)
            {
                int index = listViewCustom1.SelectedIndices[0];

                activeSubMesh = activeMesh.subMeshes[index];

                FaceCountUD.Value = activeSubMesh.size;
                offsetUD.Value = activeSubMesh.offset;

                int MeshIndex = activeShape.lodMeshes.IndexOf(activeMesh);

                int boundIndex = MeshIndex;

                centerXUD.Value = (decimal)activeShape.boundingBoxes[boundIndex].Center.X;
                centerYUD.Value = (decimal)activeShape.boundingBoxes[boundIndex].Center.Y;
                centerZUD.Value = (decimal)activeShape.boundingBoxes[boundIndex].Center.Z;

                extendXUD.Value = (decimal)activeShape.boundingBoxes[boundIndex].Extend.X;
                extendYUD.Value = (decimal)activeShape.boundingBoxes[boundIndex].Extend.Y;
                extendZUD.Value = (decimal)activeShape.boundingBoxes[boundIndex].Extend.Z;

                if (activeShape.boundingRadius.Count > 1)
                    radiusUD.Value = (decimal)activeShape.boundingRadius[MeshIndex];
                else
                    radiusUD.Value = (decimal)activeShape.boundingRadius[0];
            }
            else
            {
                activeSubMesh = null;
                FaceCountUD.Value = 0;
                offsetUD.Value = 0;
                centerXUD.Value = 0;
                centerYUD.Value = 0;
                centerZUD.Value = 0;
                extendXUD.Value = 0 ;
                extendYUD.Value = 0;
                extendZUD.Value = 0;

                radiusUD.Value = 0;
            }
        }
    }
}
