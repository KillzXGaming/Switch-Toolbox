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
    public partial class STColorDialog : Form
    {
        public EventHandler ColorChanged;

        public STColorDialog(Color color)
        {
            InitializeComponent();

            Text = "Color Dialog";
            colorSelector1.Color = color;
            colorSelector1.DisplayAlpha = true;
            colorSelector1.Alpha = color.A;
        }

        public Color NewColor
        {
            get
            {
                return colorSelector1.Color;
            }
            set
            {
                colorSelector1.Color = value;
            }
        }

        private void colorSelector1_ColorChanged(object sender, EventArgs e)
        {
            colorPB.BackColor = NewColor;
            alphaPB.BackColor = colorSelector1.AlphaColor;

            if (ColorChanged != null)
                ColorChanged.Invoke(sender, e);
        }
    }
}
