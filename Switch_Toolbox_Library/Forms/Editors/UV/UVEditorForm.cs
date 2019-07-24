using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class UVEditorForm : STForm
    {
        public UVEditorForm()
        {
            InitializeComponent();
        }

        public void LoadEditor(List<STGenericObject> Meshes)
        {
            List<STGenericMaterial> materials = new List<STGenericMaterial>();
            for (int i =0; i < Meshes.Count; i++)
            {
                if (Meshes[i].GetMaterial() != null)
                {
                    materials.Add(Meshes[i].GetMaterial());
                }
            }

            uvEditor1.ActiveObjects = Meshes;
            uvEditor1.ActiveMaterials = materials;
            uvEditor1.Textures.Clear();
            uvEditor1.Reset();
            uvEditor1.Refresh();
        }
    }
}
