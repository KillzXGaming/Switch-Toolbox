using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirstPlugin.CtrLibrary.Forms
{
    public partial class BCHMaterialShaderParamEditor : UserControl, IMaterialLoader
    {
        public BCHMaterialShaderParamEditor()
        {
            InitializeComponent();
        }

        public void LoadMaterial(H3DMaterialWrapper wrapper)
        {
            var matParams = wrapper.Material.MaterialParams;

        }
    }
}
