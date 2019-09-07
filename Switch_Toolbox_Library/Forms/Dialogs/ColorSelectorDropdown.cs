using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class ColorSelectorDropdown : STUserControl
    {
        public STColor8 Color8
        {
            get { return colorSelector1.Color8; }
            set { colorSelector1.Color8 = value; }
        }

        public Color Color
        {
            get { return colorSelector1.Color; }
            set { colorSelector1.Color = value; }
        }

        public Color AlphaColor
        {
            get { return colorSelector1.AlphaColor; }
        }

        public int Alpha
        {
            get { return colorSelector1.Alpha; }
            set { colorSelector1.Alpha = value; }
        }

        public ColorSelectorDropdown()
        {
            InitializeComponent();
        }

        private void colorSelector1_ColorChanged(object sender, EventArgs e)
        {
            pictureBox1.BackColor = colorSelector1.Color;
            pictureBox2.BackColor = colorSelector1.AlphaColor;

            var fullColor = Color.FromArgb(colorSelector1.Alpha, colorSelector1.Color);
            pictureBoxCustom1.Image = BitmapExtension.FillColor(30,30, fullColor);
        }
    }
}
