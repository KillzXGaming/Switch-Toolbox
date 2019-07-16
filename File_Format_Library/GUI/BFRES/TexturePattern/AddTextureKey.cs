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

namespace FirstPlugin.Forms
{
    public partial class AddTextureKey : STForm
    {
        public AddTextureKey()
        {
            InitializeComponent();

            CanResize = false;
        }

        public void LoadAnim(uint FrameCount)
        {
            maxFrameCountUD.Value = FrameCount;
            currentFrameCountUD.Maximum = FrameCount;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Texture_Selector selector = new Texture_Selector();
            if (selector.ShowDialog() == DialogResult.OK)
            {
                textureNameTB.Text = selector.GetSelectedTexture();
            }
        }
    }
}
