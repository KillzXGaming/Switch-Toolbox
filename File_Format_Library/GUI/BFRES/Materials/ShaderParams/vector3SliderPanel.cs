using System;
using System.Drawing;
using System.Windows.Forms;
using Toolbox.Library;
using Syroot.NintenTools.NSW.Bfres;
using Bfres.Structs;

namespace FirstPlugin.Forms
{
    public partial class vector3SliderPanel : ParamValueEditorBase
    {
        public bool IsColor { get; set; }

        public Color GetColor()
        {
            if (IsColor)
                return pictureBox1.BackColor;
            else
                return Color.Empty;
        }

        public vector3SliderPanel(string UniformName, float[] values, BfresShaderParam param)
        {
            InitializeComponent();

            activeParam = param;
            stTextBox1.Bind(activeParam, "Name");

            barSlider1.DataType = typeof(float);
            barSlider2.DataType = typeof(float);
            barSlider3.DataType = typeof(float);

            barSlider1.Value = values[0];
            barSlider2.Value = values[1];
            barSlider3.Value = values[2];

            SetColor(UniformName, values);
        }

        public vector3SliderPanel(string UniformName, int[] values, BfresShaderParam param)
        {
            InitializeComponent();

            activeParam = param;
            stTextBox1.Bind(activeParam, "Name");

            barSlider1.DataType = typeof(int);
            barSlider2.DataType = typeof(int);
            barSlider3.DataType = typeof(int);

            barSlider1.Value = values[0];
            barSlider2.Value = values[1];
            barSlider3.Value = values[2];
        }

        public vector3SliderPanel(string UniformName, uint[] values, BfresShaderParam param)
        {
            InitializeComponent();

            activeParam = param;
            stTextBox1.Bind(activeParam, "Name");

            barSlider1.DataType = typeof(uint);
            barSlider2.DataType = typeof(int);
            barSlider3.DataType = typeof(int);

            barSlider1.Value = values[0];
            barSlider2.Value = values[1];
            barSlider3.Value = values[2];
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
                var SetColor = Color.FromArgb(255,
                              Utils.FloatToIntClamp(values[0]),
                              Utils.FloatToIntClamp(values[1]),
                              Utils.FloatToIntClamp(values[2]));

                pictureBox1.BackColor = SetColor;
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
            };
        }
        public void ApplyValueUint()
        {
            activeParam.ValueUint = new uint[]
            {
                (uint)barSlider1.Value,
                (uint)barSlider2.Value,
                (uint)barSlider3.Value,
            };
        }
        public void ApplyValueInt()
        {
            activeParam.ValueInt = new int[]
            {
                (int)barSlider1.Value,
                (int)barSlider2.Value,
                (int)barSlider3.Value,
            };
        }

        private void barSlider_ValueChanged(object sender, System.EventArgs e)
        {
            if (activeParam.Type == ShaderParamType.UInt3)
                ApplyValueUint();
            if (activeParam.Type == ShaderParamType.Int3)
                ApplyValueInt();
            if (activeParam.Type == ShaderParamType.Float3)
                ApplyValueSingles();

            if (OnPanelChanged != null)
                OnPanelChanged(activeParam, this);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = pictureBox1.BackColor;
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                barSlider1.Value = colorDialog.Color.R / 255f;
                barSlider2.Value = colorDialog.Color.G / 255f;
                barSlider3.Value = colorDialog.Color.B / 255f;

                ApplyValueSingles();

                if (OnPanelChanged != null)
                    OnPanelChanged(activeParam, this);
            }
        }
    }
}
