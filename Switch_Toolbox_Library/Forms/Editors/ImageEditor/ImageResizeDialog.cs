using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switch_Toolbox.Library.Forms
{
    public partial class ImageResizeDialog : STForm
    {
        public ImageResizeDialog()
        {
            InitializeComponent();

            Text = "Resize Image";

            CanResize = false;

            resampleCB.Items.Add(InterpolationMode.HighQualityBicubic);
            resampleCB.Items.Add(InterpolationMode.HighQualityBilinear);
            resampleCB.Items.Add(InterpolationMode.NearestNeighbor);

            resampleCB.SelectedIndex = 0;
        }

        public Bitmap activeImage;
        public Bitmap newImage;

        public void LoadImage(Image image)
        {
            widthUD.Value = image.Width;
            heightUD.Value = image.Height;

            activeImage = new Bitmap(image);

            UpdateImage();
        }

        private void UpdateImage()
        {
            if (activeImage == null)
                return;

            newImage = BitmapExtension.ResizeImage(
                activeImage, (int)widthUD.Value, (int)heightUD.Value,
                (InterpolationMode)resampleCB.SelectedItem);

            pictureBoxCustom1.Image = newImage;
        }

        private void resampleCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (resampleCB.SelectedIndex >= 0)
            {
                UpdateImage();
            }
        }

        private void widthUD_ValueChanged(object sender, EventArgs e) {
            UpdateImage();
        }

        private void heightUD_ValueChanged(object sender, EventArgs e) {
            UpdateImage();
        }
    }
}
