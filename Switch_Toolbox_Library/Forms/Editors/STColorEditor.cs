using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms.Editors
{
    public partial class STColorEditor : UserControl
    {
        public event EventHandler<STColor> ColorChanged;

        private STColor activeColor;

        public STColorEditor()
        {
            InitializeComponent();
            colorSelector.ColorChanged += ColorSelector_ColorChanged;
        }

        public void LoadColor(STColor color, bool isAlpha, bool isTimed)
        {
            activeColor = color;

            colorSelector.DisplayColor = !isAlpha;
            colorSelector.DisplayAlpha = isAlpha;
            colorSelector.Color = color.Color;
            if (isAlpha)
            {
                colorSelector.Alpha = color.Color.R;
            }

            ShowTimeInput(isTimed);
            timeUpDown.Value = (decimal)color.Time;
        }

        public void ShowTimeInput(bool showTimeInput)
        {
            timeUpDown.Visible = timeLabel.Visible = showTimeInput;
        }

        private void ColorSelector_ColorChanged(object sender, EventArgs e)
        {
            Color color = colorSelector.Color;
            string hex = Utils.ColorToHex(color);
            if (hexTextBox.Text != hex)
            {
                hexTextBox.Text = hex;
            }

            previewPictureBox.BackColor = color;

            if (activeColor.Color != color)
            {
                activeColor.Color = color;
            }

            ColorChanged?.Invoke(this, activeColor);
        }

        private void HexTextBox_TextChanged(object sender, EventArgs e)
        {
            string text = hexTextBox.Text;
            if (text.Length != 8)
            {
                return;
            }

            Color color = Utils.HexToColor(hexTextBox.Text);
            if (colorSelector.Color != color)
            {
                colorSelector.Color = color;
            }
            // color selector will update the rest
        }

        private void TimeUpDown_ValueChanged(object sender, EventArgs e)
        {
            float time = (float)timeUpDown.Value;
            if (activeColor.Time != time)
            {
                activeColor.Time = time;
            }

            ColorChanged?.Invoke(this, activeColor);
        }
    }
}
