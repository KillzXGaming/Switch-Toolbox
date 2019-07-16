using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class ImageFillColor : STForm
    {
        public Color FillColor = Color.White;
        public bool ResizeSmall = false;

        public ImageFillColor()
        {
            InitializeComponent();

            CanResize = false;

            stButton1.BackColor = FillColor;
        }

        private void stButton1_Click(object sender, EventArgs e)
        {
            Color NormalColor = Color.FromArgb(254, 124, 133); //A blank normal map color

            int[] ColorPresets = new int[1];
            ColorPresets[0] = NormalColor.ToArgb() & 0x00FFFFFF;

            ColorDialog colorDialog = new ColorDialog();
            colorDialog.CustomColors = ColorPresets;
            if (colorDialog.ShowDialog() == DialogResult.OK) {
                FillColor = colorDialog.Color;
                stButton1.BackColor = FillColor;
            }
        }

        private void stCheckBox1_CheckedChanged(object sender, EventArgs e) {
            ResizeSmall = stCheckBox1.Checked;
        }

        private void stButton3_Click(object sender, EventArgs e)
        {

        }

        private void stButton2_Click(object sender, EventArgs e)
        {

        }
    }
}
