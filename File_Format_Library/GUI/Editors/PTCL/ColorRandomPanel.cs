using System;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Toolbox.Library;
using System.Text.RegularExpressions;

namespace FirstPlugin.Forms
{
    public partial class ColorRandomPanel : UserControl, IColorPanelCommon
    {
        public bool IsAlpha { get; set; }
        public STColor SelectedColor => SelectedColorKey?.MyColor;

        public event EventHandler ColorSelected;

        private ColorKey SelectedColorKey => colorKeys.Where(k => k.IsSelected).FirstOrDefault();

        private ColorKey[] colorKeys;

        public ColorRandomPanel()
        {
            InitializeComponent();
            colorKeys = Controls.OfType<Panel>().OrderBy(panel => Int32.Parse(Regex.Match(panel.Name, @"\d").Value)).Select(panel =>
            {
                var key = new ColorKey(panel);
                key.Selected += ColorKey_Selected;

                return key;
            }).ToArray();
        }

        public void UpdateSelectedColor()
        {
            SelectedColorKey?.Refresh();
        }

        public Color GetColor()
        {
            return SelectedColor.Color; // assumes this is only called while a color is selected. otherwise null ref.
        }

        public void SetColor(Color color)
        {
            SelectedColor.Color = color; // assumes this is only called while a color is selected. otherwise null ref.
            SelectedColorKey.Refresh();
        }

        public void DeselectPanel()
        {
            foreach (ColorKey key in colorKeys)
            {
                key.Deselect();
            }
        }

        public void LoadColors(STColor[] colors)
        {
            LoadColors(colors, 8);
        }

        public void LoadColors(STColor[] colors, int keyCount)
        {
            for (int i = 0; i < keyCount; i++)
            {
                colorKeys[i].LoadColor(colors[i]);
            }

            for (int i = keyCount; i < 8; i++)
            {
                colorKeys[i].Unload();
            }
        }

        private void ColorKey_Selected(object sender, EventArgs args)
        {
            foreach (ColorKey key in colorKeys)
            {
                if (key != sender)
                {
                    key.Deselect();
                }
            }

            ColorSelected?.Invoke(this, null);
        }

        private class ColorKey
        {
            public STColor MyColor { get; private set; }
            public Panel MyPanel { get; private set; }
            public bool IsSelected { get; private set; }

            public event EventHandler Selected;

            public ColorKey(Panel panel)
            {
                MyPanel = panel;
                MyPanel.Click += Panel_Click;
            }

            public void LoadColor(STColor color)
            {
                MyColor = color;
                MyPanel.BackColor = Color.FromArgb(color.Color.R, color.Color.G, color.Color.B);
                MyPanel.Visible = true;
            }

            public void Unload()
            {
                MyColor = null;
                MyPanel.Visible = false;
            }

            public void Refresh()
            {
                MyPanel.BackColor = MyColor.Color;
            }

            public void Deselect()
            {
                MyPanel.BorderStyle = BorderStyle.FixedSingle;
                IsSelected = false;
            }

            private void Panel_Click(object sender, EventArgs args)
            {
                MyPanel.BorderStyle = BorderStyle.Fixed3D;
                IsSelected = true;
                Selected?.Invoke(this, null);
            }
        }
    }
}
