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
            uvEditor1.Materials.Clear();
            uvEditor1.Textures.Clear();
            uvEditor1.Objects.Clear();

            for (int i = 0; i < Meshes.Count; i++)
            {
                if (Meshes[i].GetMaterial() != null)
                {
                    var mat = Meshes[i].GetMaterial();
                    if (!uvEditor1.Materials.Contains(mat))
                    {
                        uvEditor1.Materials.Add(mat);
                    }
                }
            }

            uvEditor1.Objects = Meshes;
            uvEditor1.Reset();
            uvEditor1.Refresh();
        }
    }
}
