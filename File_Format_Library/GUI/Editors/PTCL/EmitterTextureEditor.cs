using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class EmitterTextureEditor : STUserControl
    {
        private Thread Thread;
        private EFTB.EMTR.SamplerInfo ActiveTexture;
        private IEnumerable<EFTB.TEXR> LookupTextures;

        public EmitterTextureEditor()
        {
            InitializeComponent();
        }

        public void LoadTexture(EFTB.EMTR.SamplerInfo textureInfo, IEnumerable<EFTB.TEXR> textures)
        {
            ActiveTexture = textureInfo;
            LookupTextures = textures;

            enabledCheckBox.Checked = textureInfo.Enabled;
            textureIdTextBox.Text = textureInfo.TextureId.ToString("X8");

            LoadTextureImage(textureInfo.TextureId);
        }

        private void LoadTextureImage(uint textureId)
        {
            pictureBoxCustom1.Visible = false;
            pictureBoxCustom1.Image = Toolbox.Library.Properties.Resources.LoadingImage;

            if (Thread != null && Thread.IsAlive)
                Thread.Abort();

            Thread = new Thread((ThreadStart)(() =>
            {
                EFTB.TEXR texture = LookupTextures.Where(t => t.TextureId == textureId).FirstOrDefault();
                if (texture != null)
                {
                    var image = texture.GetBitmap();
                    // image = texture.GetComponentBitmap(image, showAlphaChk.Checked);

                    if (pictureBoxCustom1.InvokeRequired)
                    {
                        pictureBoxCustom1.Invoke((MethodInvoker)delegate
                        {
                            UpdatePicturebox(image);
                        });
                    }
                    else
                        UpdatePicturebox(image);
                }
            }));
            Thread.Start();
        }

        private void UpdatePicturebox(Bitmap image)
        {
            pictureBoxCustom1.Visible = true;
            pictureBoxCustom1.Image = image;
        }

        private void EnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            ActiveTexture.Enabled = enabledCheckBox.Checked;
        }

        private void TextureIdTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                ActiveTexture.TextureId = (uint)Convert.ToInt32(textureIdTextBox.Text, 16);
                LoadTextureImage(ActiveTexture.TextureId);
            }
            catch (FormatException)
            {
                // revert
                textureIdTextBox.Text = ActiveTexture.TextureId.ToString("X8");
            }
        }
    }
}
