using System;
using System.Drawing;
using Switch_Toolbox.Library;
using Syroot.NintenTools.NSW.Bfres;
using Bfres.Structs;
using System.Windows.Forms;

namespace FirstPlugin.Forms
{
    public partial class vector4SliderPanel : ParamValueEditorBase
    {
        public bool IsColor { get; set; }

        public Color GetColor()
        {
            if (IsColor)
            {
                SetColor(activeParam.Name, activeParam.ValueFloat);
                return colorPB.BackColor;
            }
            else
                return Color.Empty;
        }

        public Color GetAlphaColor()
        {
            if (IsColor)
                return alphaPB.BackColor;
            else
                return Color.Empty;
        }

        public vector4SliderPanel(string UniformName, float[] values, BfresShaderParam param)
        {
            activeParam = param;

            InitializeComponent();

            barSlider1.DataType = typeof(float);
            barSlider2.DataType = typeof(float);
            barSlider3.DataType = typeof(float);
            barSlider4.DataType = typeof(float);

            barSlider1.Value = values[0];
            barSlider2.Value = values[1];
            barSlider3.Value = values[2];
            barSlider4.Value = values[3];

            SetColor(UniformName, values);
        }

        public vector4SliderPanel(string UniformName, uint[] values, BfresShaderParam param)
        {
            activeParam = param;

            InitializeComponent();

            barSlider1.DataType = typeof(uint);
            barSlider2.DataType = typeof(uint);
            barSlider3.DataType = typeof(uint);
            barSlider4.DataType = typeof(uint);

            barSlider1.Value = values[0];
            barSlider2.Value = values[1];
            barSlider3.Value = values[2];
            barSlider4.Value = values[3];
        }

        public vector4SliderPanel(string UniformName, int[] values, BfresShaderParam param)
        {
            activeParam = param;

            InitializeComponent();

            barSlider1.DataType = typeof(int);
            barSlider2.DataType = typeof(int);
            barSlider3.DataType = typeof(int);
            barSlider4.DataType = typeof(int);

            barSlider1.Value = values[0];
            barSlider2.Value = values[1];
            barSlider3.Value = values[2];
            barSlider4.Value = values[3];
        }

        public void SetColor(string UniformName, float[] values)
        {

            IsColor = UniformName.Contains("Color") ||
                                     UniformName.Contains("color") ||
                                     UniformName.Contains("konst0") ||
                                     UniformName.Contains("konst1") ||
                                     UniformName.Contains("konst2") ||
                                     UniformName.Contains("konst3");

            if (IsColor)
            {
                colorPB.BackColor = Color.FromArgb(
                    Utils.FloatToIntClamp(255),
                    Utils.FloatToIntClamp(values[0]),
                    Utils.FloatToIntClamp(values[1]),
                    Utils.FloatToIntClamp(values[2])
                    );

                alphaPB.BackColor = Color.FromArgb(
                Utils.FloatToIntClamp(255),
                Utils.FloatToIntClamp(values[3]),
                Utils.FloatToIntClamp(values[3]),
                Utils.FloatToIntClamp(values[3])
                );
            }
            else
            {
                colorPB.Visible = false;
                alphaPB.Visible = false;

                Height -= 18;
            }
        }

        public void ApplyValueSingles()
        {
            SetColor(activeParam.Name, activeParam.ValueFloat);

            activeParam.ValueFloat = new float[]
            {
                (float)barSlider1.Value,
                (float)barSlider2.Value,
                (float)barSlider3.Value,
                (float)barSlider4.Value,
            };
        }

        public void ApplyValueUint()
        {
            activeParam.ValueUint = new uint[]
            {
                (uint)barSlider1.Value,
                (uint)barSlider2.Value,
                (uint)barSlider3.Value,
                (uint)barSlider4.Value,
            };
        }
        public void ApplyValueInt()
        {
            activeParam.ValueInt = new int[]
            {
                (int)barSlider1.Value,
                (int)barSlider2.Value,
                (int)barSlider3.Value,
                (int)barSlider4.Value,
            };
        }

        private void barSlider_ValueChanged(object sender, System.EventArgs e)
        {
            if (activeParam.Type == ShaderParamType.UInt4)
                ApplyValueUint();
            if (activeParam.Type == ShaderParamType.Int4)
                ApplyValueInt();
            if (activeParam.Type == ShaderParamType.Float4)
                ApplyValueSingles();

            if (OnPanelChanged != null)
                OnPanelChanged(activeParam, this);
        }

        private void colorPB_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = colorPB.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                barSlider1.Value = colorDialog.Color.R / 255;
                barSlider2.Value = colorDialog.Color.G / 255;
                barSlider3.Value = colorDialog.Color.B / 255;
            }
        }

        private void alphaPB_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = alphaPB.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                barSlider4.Value = colorDialog.Color.R / 255;
            }
        }

        private void barSlider1_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
        {

        }
    }
}
