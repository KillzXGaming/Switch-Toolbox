using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class HueSaturationAdjuster : STForm
    {
        public HueSaturationAdjuster()
        {
            InitializeComponent();
        }
        Bitmap bitmapOriginalCached;
        Bitmap ActiveImage;
        Bitmap NewImage;

        PictureBox pictureBox;

        private Thread Thread;

        public void LoadBitmap(PictureBox pictureBx)
        {
            ActiveImage = new Bitmap(pictureBx.Image);

            pictureBox = pictureBx;

            hueTrackBar.Value = (int)(hue * 100f);
            saturationTrackBar.Value = (int)(saturation * 100f);
            brightnessTrackBar.Value = (int)(brightness * 100f);
        }

        private float hue = 0.5f;
        private float saturation = 0.5f;
        private float brightness = 0.5f;

        private void UpdateImage()
        {
            Console.WriteLine(hue);
            Console.WriteLine(saturation);
            Console.WriteLine(brightness);

            Thread = new Thread((ThreadStart)(() =>
            {
                NewImage = BitmapExtension.HueStaturationBrightnessScale(
                    ActiveImage, true, true, true, hue * 2, saturation, brightness);

                pictureBox.Image = NewImage;
            }));
            Thread.Start();
        }

        private void hueTrackBar_ValueChanged(object sender, EventArgs e)
        {
            hue = (hueTrackBar.Value / 100f);

            UpdateImage();
        }

        private void saturationTrackBar_ValueChanged(object sender, EventArgs e)
        {
            saturation = (saturationTrackBar.Value / 100f);

            UpdateImage();
        }

        private void brightnessTrackBar_ValueChanged(object sender, EventArgs e)
        {
            brightness =  (brightnessTrackBar.Value / 100f);

            UpdateImage();
        }

        private void hueTrackBar_Scroll(object sender, EventArgs e)
        {

        }

        private void saturationTrackBar_Scroll(object sender, EventArgs e)
        {

        }

        private void brightnessTrackBar_Scroll(object sender, EventArgs e)
        {

        }
    }
}
