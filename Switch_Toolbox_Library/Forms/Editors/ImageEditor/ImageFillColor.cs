using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switch_Toolbox.Library.Forms
{
    public partial class ImageFillColor : STForm
    {
        public Color FillColor = Color.White;
        public bool ResizeSmall = false;

        public ImageFillColor()
        {
            InitializeComponent();

            CanResize = true;

            stCheckBox1.BackColor = FillColor;
        }

        private void stButton1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK) {
                FillColor = colorDialog.Color;
                stCheckBox1.BackColor = FillColor;
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
