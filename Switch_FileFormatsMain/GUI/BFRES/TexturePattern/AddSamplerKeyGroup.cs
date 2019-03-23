using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library.Animations;

namespace FirstPlugin.Forms
{
    public partial class AddSamplerKeyGroup : STForm
    {
        public AddSamplerKeyGroup()
        {
            InitializeComponent();

            CanResize = false;
        }

        public MaterialAnimation.Material.Sampler GetSamplerData()
        {
            MaterialAnimation.Material.Sampler sampler = new MaterialAnimation.Material.Sampler();
            sampler.Text = samplerNameTB.Text;
            sampler.group.Constant = constantChkBox.Checked;
            return sampler;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

        }
    }
}
