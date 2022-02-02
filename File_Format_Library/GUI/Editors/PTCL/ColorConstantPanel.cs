using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class ColorConstantPanel : UserControl, IColorPanelCommon
    {
        public bool IsAlpha { get; set; }
        public STColor SelectedColor
        {
            get
            {
                return ActiveColor;
            }
        }

        public ColorConstantPanel()
        {
            InitializeComponent();
        }

        public void UpdateSelectedColor()
        {
            color0PB.BackColor = ActiveColor.Color;
        }

        public Color GetColor()
        {
            return color0PB.BackColor;
        }

        public void SetColor(Color color)
        {
            color0PB.BackColor = color;
            ActiveColor.Color = color;
        }

        public void DeselectPanel()
        {
            this.BorderStyle = BorderStyle.None;
        }

        public event EventHandler ColorSelected;

        private STColor ActiveColor;

        public void LoadColor(STColor color)
        {
            ActiveColor = color;
            color0PB.BackColor = Color.FromArgb(ActiveColor.Color.R, ActiveColor.Color.G, ActiveColor.Color.B);
        }

        public void LoadColors(STColor[] colors, int keyCount)
        {
            throw new InvalidOperationException("ColorConstantPanel doesn't support multiple colors. Use LoadColor(STColor) instead.");
        }

        private void color0PB_Click(object sender, EventArgs e)
        {
            if (ColorSelected != null)
                ColorSelected(this, null);
        }
    }
}
