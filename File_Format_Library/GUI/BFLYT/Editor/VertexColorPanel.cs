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

namespace LayoutBXLYT
{
    public partial class VertexColorPanel : EditorPanelBase
    {
        public EventHandler ColorChanged;

        public STColor8 ColorTopLeft { get; set; }
        public STColor8 ColorTopRight { get; set; }
        public STColor8 ColorBottomLeft { get; set; }
        public STColor8 ColorBottomRight { get; set; }

        public bool IsAnimationMode = false;

        public VertexColorPanel()
        {
            InitializeComponent();

            vertexColorBox1.OnColorChanged += OnColorChanged;
        }

        public void LoadColors(Color[] colors)
        {
            SetValue(TLRUD, colors[0].R, false);
            SetValue(TLGUD, colors[0].G, false);
            SetValue(TLBUD, colors[0].B, false);
            SetValue(TLAUD, colors[0].A, false);

            SetValue(TRRUD, colors[1].R, false);
            SetValue(TRGUD, colors[1].G, false);
            SetValue(TRBUD, colors[1].B, false);
            SetValue(TRAUD, colors[1].A, false);

            SetValue(BLRUD, colors[2].R, false);
            SetValue(BLGUD, colors[2].G, false);
            SetValue(BLBUD, colors[2].B, false);
            SetValue(BLAUD, colors[2].A, false);

            SetValue(BRRUD, colors[3].R, false);
            SetValue(BRGUD, colors[3].G, false);
            SetValue(BRBUD, colors[3].B, false);
            SetValue(BRAUD, colors[3].A, false);
        }

        private void UpdateColors()
        {
            vertexColorBox1.TopLeftColor = Color.FromArgb((byte)TLAUD.Value, (byte)TLRUD.Value, (byte)TLGUD.Value, (byte)TLBUD.Value);
            vertexColorBox1.TopRightColor = Color.FromArgb((byte)TRAUD.Value, (byte)TRRUD.Value, (byte)TRGUD.Value, (byte)TRBUD.Value);
            vertexColorBox1.BottomLeftColor = Color.FromArgb((byte)BLAUD.Value, (byte)BLRUD.Value, (byte)BLGUD.Value, (byte)BLBUD.Value);
            vertexColorBox1.BottomRightColor = Color.FromArgb((byte)BRAUD.Value, (byte)BRRUD.Value, (byte)BRGUD.Value, (byte)BRBUD.Value);
        }

        private void OnColorChanged(object sender, EventArgs e)
        {
            SetKeyedValue(TLRUD, vertexColorBox1.TopLeftColor.R);
            SetKeyedValue(TLGUD, vertexColorBox1.TopLeftColor.G);
            SetKeyedValue(TLBUD, vertexColorBox1.TopLeftColor.B);
            SetKeyedValue(TLAUD, vertexColorBox1.TopLeftColor.A);

            SetKeyedValue(TRRUD, vertexColorBox1.TopRightColor.R);
            SetKeyedValue(TRGUD, vertexColorBox1.TopRightColor.G);
            SetKeyedValue(TRBUD, vertexColorBox1.TopRightColor.B);
            SetKeyedValue(TRAUD, vertexColorBox1.TopRightColor.A);

            SetKeyedValue(BLRUD, vertexColorBox1.BottomLeftColor.R);
            SetKeyedValue(BLGUD, vertexColorBox1.BottomLeftColor.G);
            SetKeyedValue(BLBUD, vertexColorBox1.BottomLeftColor.B);
            SetKeyedValue(BLAUD, vertexColorBox1.BottomLeftColor.A);

            SetKeyedValue(BRRUD, vertexColorBox1.BottomRightColor.R);
            SetKeyedValue(BRGUD, vertexColorBox1.BottomRightColor.G);
            SetKeyedValue(BRBUD, vertexColorBox1.BottomRightColor.B);
            SetKeyedValue(BRAUD, vertexColorBox1.BottomRightColor.A);

            ColorChanged?.Invoke(sender, e);
        }

        private void SetKeyedValue(BarSlider.BarSlider barSlider, float value)
        {
            bool changed = barSlider.Value != value;
            if (!changed)
                return;

            SetValue(barSlider, value, IsAnimationMode);
        }

        private void SetValue(BarSlider.BarSlider barSlider, float value, bool keyed)
        {
            barSlider.Maximum = 255;
            barSlider.Minimum = 0;
            barSlider.Value = value;
        }
    }
}
