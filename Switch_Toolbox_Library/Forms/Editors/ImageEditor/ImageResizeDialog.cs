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

namespace Toolbox.Library.Forms
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

        private bool IsUpdating = false;
        private void UpdateImage()
        {
            if (activeImage == null)
                return;

            IsUpdating = true;

            int ImageWidth = (int)widthUD.Value;
            int ImageHeight = (int)heightUD.Value;

            if (chkKeepAspectRatio.Checked)
                ApplyRatio((int)widthUD.Value, (int)heightUD.Value, out ImageWidth, out ImageHeight);

            if (ImageHeight <= 0) ImageHeight = 1;
            if (ImageWidth <= 0) ImageWidth = 1;

            widthUD.Value = ImageWidth;
            heightUD.Value = ImageHeight;

            newImage = BitmapExtension.ResizeImage(
                activeImage, ImageWidth, ImageHeight,
                (InterpolationMode)resampleCB.SelectedItem);

            pictureBoxCustom1.Image = newImage;

            IsUpdating = false;
        }

        private void resampleCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (resampleCB.SelectedIndex >= 0)
            {
                UpdateImage();
            }
        }

        private void widthUD_ValueChanged(object sender, EventArgs e) {
            if (!IsUpdating)
            UpdateImage();
        }

        private void heightUD_ValueChanged(object sender, EventArgs e) {
            if (!IsUpdating)
                UpdateImage();
        }

        private void ApplyRatio(int Width, int Height, out int NewWidth, out int NewHeight)
        {
            double ratioX = (double)Width / (double)activeImage.Width;
            double ratioY = (double)Height / (double)activeImage.Height;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            NewHeight = Convert.ToInt32(activeImage.Height* ratio);
            NewWidth = Convert.ToInt32(activeImage.Width* ratio);
        }

        private void chkKeepAspectRatio_CheckedChanged(object sender, EventArgs e) {
            UpdateImage();
        }
    }
}
