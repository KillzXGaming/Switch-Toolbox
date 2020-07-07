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
        private bool loaded = false;

        private bool CanCloseOnLostFocus= false;

        public EventHandler ColorChanged;

        public STColorDialog(Color color)
        {
            InitializeComponent();

            Text = "Color Dialog";
            colorSelector1.Color = color;
            colorSelector1.DisplayAlpha = true;
            colorSelector1.Alpha = color.A;

            //Wait until the user can lose focus for closing control
            //This prevents window from popping up and closing instantly
            Utils.DelayAction(100,delegate
            {
                CanCloseOnLostFocus = true;
            });
        }

        public int Alpha
        {
            get { return colorSelector1.Alpha; }
            set { colorSelector1.Alpha = value; }
        }

        public Color NewColor
        {
            get
            {
                return Color.FromArgb(Alpha, colorSelector1.Color);
            }
            set
            {
                colorSelector1.Alpha = value.A;
                colorSelector1.Color = Color.FromArgb(255, value);
            }
        }

        public Color ColorRGB
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
            colorPB.BackColor = ColorRGB;
            alphaPB.BackColor = colorSelector1.AlphaColor;

            loaded = false;

            redUD.Value = ColorRGB.R;
            greenUD.Value = ColorRGB.G;
            blueUD.Value = ColorRGB.B;
            alphaUD.Value = Alpha;

            //Only adjust from other color changes
            //This will keep the textbox in the same current state it's being edited in.
            if (!hexTextChanged)
                hexTB.Text = Utils.ColorToHex(NewColor);

            loaded = true;

            if (ColorChanged != null)
                ColorChanged.Invoke(sender, e);
        }

        private void STColorDialog_Deactivate(object sender, EventArgs e)
        {
            if (CanCloseOnLostFocus)
                this.Close();
        }

        private void STColorDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void STColorDialog_Load(object sender, EventArgs e)
        {

        }

        private void UD_ValueChanged(object sender, EventArgs e)
        {
            if (!loaded) return;
            NewColor = Color.FromArgb((byte)alphaUD.Value, (byte)redUD.Value, (byte)greenUD.Value, (byte)blueUD.Value);
        }

        bool hexTextChanged = false;
        private void stTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox && loaded)
            {
                ((TextBox)sender).MaxLength = 8;

                if (((TextBox)sender).Text.Length != 8)
                    return;

                hexTextChanged = true;

                NewColor = Utils.HexToColor(((TextBox)sender).Text);


                if (ColorChanged != null)
                    ColorChanged.Invoke(sender, e);

                hexTextChanged = false;
            }
        }
    }
}
