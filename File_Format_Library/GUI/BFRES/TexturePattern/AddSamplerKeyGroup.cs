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
using Toolbox.Library.Animations;

namespace FirstPlugin.Forms
{
    public partial class AddSamplerKeyGroup : STForm
    {
        public AddSamplerKeyGroup()
        {
            InitializeComponent();

            CanResize = false;
        }

        public MaterialAnimation.SamplerKeyGroup GetSamplerData(MaterialAnimation materialAnimation)
        {
            MaterialAnimation.SamplerKeyGroup sampler = new MaterialAnimation.SamplerKeyGroup(materialAnimation);
            sampler.Text = samplerNameTB.Text;
            sampler.Constant = constantChkBox.Checked;
            return sampler;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

        }
    }
}
