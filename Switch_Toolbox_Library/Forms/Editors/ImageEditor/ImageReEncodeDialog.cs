using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class ImageReEncodeDialog : STForm
    {
        public ImageReEncodeDialog()
        {
            InitializeComponent();

            Text = "Resize Image";

            CanResize = false;
        }

        private Thread Thread;

        public STGenericTexture activeTexture;
        public Bitmap activeImage;
        public Bitmap newImage;

        public int MipCount;
        public TEX_FORMAT Format;

        public void LoadImage(Image image, STGenericTexture tex)
        {
            activeTexture = tex;
            activeImage = new Bitmap(image);

            mipcountUD.Value = tex.MipCount;
            foreach (var format in tex.SupportedFormats)
                formatCB.Items.Add(format);

            formatCB.SelectedItem = tex.Format;

            UpdateImage();
        }

        private void UpdateImage()
        {
            if (activeTexture == null)
                return;

            Thread = new Thread((ThreadStart)(() =>
            {
                pictureBoxCustom1.Image = Imaging.GetLoadingImage();
                var image = activeTexture.GetBitmap(0, 0);

                var mipmaps = STGenericTexture.CompressBlock(BitmapExtension.ImageToByte(activeImage),
                                        activeImage.Width, activeImage.Height, Format, 0.5f);

                newImage = BitmapExtension.SwapBlueRedChannels(STGenericTexture.DecodeBlockGetBitmap(mipmaps, (uint)image.Width, (uint)image.Height, Format, new byte[0]));

                pictureBoxCustom1.Image = newImage;

            }));
            Thread.Start();


            pictureBoxCustom1.Image = newImage;
        }

        private void mipcountUD_ValueChanged(object sender, EventArgs e)
        {
            MipCount = (int)mipcountUD.Value;
        }

        private void formatCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (formatCB.SelectedIndex >= 0)
            {
                Format = (TEX_FORMAT)formatCB.SelectedItem;
                UpdateImage();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {

        }
    }
}
