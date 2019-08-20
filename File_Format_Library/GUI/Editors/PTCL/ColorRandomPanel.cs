using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class ColorRandomPanel : UserControl, IColorPanelCommon
    {
        public bool IsAlpha { get; set; }

        public ColorRandomPanel()
        {
            InitializeComponent();
        }

        private int SelectedIndex = 0;

        public event EventHandler ColorSelected;

        public Color GetColor()
        {
            var panel = GetColor(SelectedIndex);
            return panel.BackColor;
        }

        public void SetColor(Color color)
        {
            var panel = GetColor(SelectedIndex);
            panel.BackColor = color;
            activeColors[SelectedIndex].Color = color;
        }

        public void SelectPanel()
        {
            var panel = GetColor(SelectedIndex);
            panel.BorderStyle = BorderStyle.Fixed3D;
        }

        public void DeselectPanel()
        {
            foreach (var panel in Controls)
                ((Panel)panel).BorderStyle = BorderStyle.None;
        }

        STColor[] activeColors;

        public void LoadColors(STColor[] colors)
        {
            activeColors = colors;
            for (int i = 0; i < colors.Length; i++)
            {
                var panel = GetColor(i);
                Color c = colors[i].Color;
                panel.BackColor = Color.FromArgb(c.R, c.G, c.B);
            }
        }

        public Panel GetColor(int index)
        {
            foreach (Control control in Controls)
            {
                if (control.Name == $"color{index}PB")
                    return (Panel)control;
            }
            return null;
        }

        private void colorPB_Click(object sender, EventArgs e)
        {
            var panel = sender as Panel;
            if (panel == null) return;

            for (int i =0; i < 8; i++)
            {
                if (panel.Name == $"color{i}PB")
                    SelectedIndex = i;
            }

            if (ColorSelected != null)
                ColorSelected(this, null);
        }
    }
}
