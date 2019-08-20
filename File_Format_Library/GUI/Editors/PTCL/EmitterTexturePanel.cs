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
    public partial class EmitterTexturePanel : STUserControl
    {
        public EmitterTexturePanel()
        {
            InitializeComponent();
        }

        Thread Thread;

        private PTCL.Emitter ActiveEmitter;
        public void LoadTextures(PTCL.Emitter emitter)
        {
            ActiveEmitter = emitter;

            pictureBoxCustom1.Visible = false;
            pictureBoxCustom2.Visible = false;
            pictureBoxCustom3.Visible = false;

            pictureBoxCustom1.Image = Toolbox.Library.Properties.Resources.LoadingImage;
            pictureBoxCustom2.Image = Toolbox.Library.Properties.Resources.LoadingImage;
            pictureBoxCustom3.Image = Toolbox.Library.Properties.Resources.LoadingImage;

            if (Thread != null && Thread.IsAlive)
                Thread.Abort();

            Thread = new Thread((ThreadStart)(() =>
            {
                for (int i = 0; i < emitter.DrawableTex.Count; i++)
                {
                    var image = emitter.DrawableTex[i].GetBitmap();
                    image = emitter.DrawableTex[i].GetComponentBitmap(image, showAlphaChk.Checked);

                    if (pictureBoxCustom1.InvokeRequired)
                    {
                        pictureBoxCustom1.Invoke((MethodInvoker)delegate
                        {
                            UpdatePicturebox(i,image);
                        });
                    }
                    else
                        UpdatePicturebox(i, image);
                }
            }));
            Thread.Start();
        }

        private void UpdatePicturebox(int index, Bitmap image)
        {
            if (index == 0)
            {
                pictureBoxCustom1.Visible = true;
                pictureBoxCustom1.Image = image;
            }
            if (index == 1)
            {
                pictureBoxCustom2.Visible = true;
                pictureBoxCustom2.Image = image;
            }
            if (index == 2)
            {
                pictureBoxCustom3.Visible = true;
                pictureBoxCustom3.Image = image;
            }
        }

        private void showAlphaChk_CheckedChanged(object sender, EventArgs e) {
            LoadTextures(ActiveEmitter);
        }
    }
}
