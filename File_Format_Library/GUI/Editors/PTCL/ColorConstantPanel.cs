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

        public ColorConstantPanel()
        {
            InitializeComponent();
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

        public void SelectPanel()
        {
            this.BorderStyle = BorderStyle.Fixed3D;
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

        private void color0PB_Click(object sender, EventArgs e)
        {
            if (ColorSelected != null)
                ColorSelected(this, null);
        }
    }
}
