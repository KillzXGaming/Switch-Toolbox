using Syroot.NintenTools.NSW.Bfres;
using Bfres.Structs;

namespace FirstPlugin.Forms
{
    public partial class vector2SliderPanel : ParamValueEditorBase
    {
        public vector2SliderPanel(float[] values, BfresShaderParam param)
        {
            InitializeComponent();

            activeParam = param;
            stTextBox1.Bind(activeParam, "Name");

            barSlider1.DataType = typeof(float);
            barSlider2.DataType = typeof(float);

            barSlider1.Value = values[0];
            barSlider2.Value = values[1];
        }

        public vector2SliderPanel(int[] values, BfresShaderParam param)
        {
            InitializeComponent();

            activeParam = param;
            stTextBox1.Bind(activeParam, "Name");

            barSlider1.DataType = typeof(int);
            barSlider2.DataType = typeof(int);

            barSlider1.Value = values[0];
            barSlider2.Value = values[1];
        }

        public vector2SliderPanel(uint[] values, BfresShaderParam param)
        {
            InitializeComponent();

            activeParam = param;
            stTextBox1.Bind(activeParam, "Name");

            barSlider1.DataType = typeof(uint);
            barSlider2.DataType = typeof(uint);

            barSlider1.Value = values[0];
            barSlider2.Value = values[1];
        }

        public void ApplyValueSingles() {
            activeParam.ValueFloat = new float[]
            {
                (float)barSlider1.Value,
                (float)barSlider2.Value,
            };
        }
        public void ApplyValueUint()
        {
            activeParam.ValueUint = new uint[]
            {
                (uint)barSlider1.Value,
                (uint)barSlider2.Value,
            };
        }
        public void ApplyValueInt()
        {
            activeParam.ValueInt = new int[]
            {
                (int)barSlider1.Value,
                (int)barSlider2.Value,
            };
        }

        private void barSlider_ValueChanged(object sender, System.EventArgs e)
        {
            if (activeParam.Type == ShaderParamType.UInt2)
                ApplyValueUint();
            if (activeParam.Type == ShaderParamType.Int2)
                ApplyValueInt();
            if (activeParam.Type == ShaderParamType.Float2)
                ApplyValueSingles();

            if (OnPanelChanged != null)
                OnPanelChanged(activeParam, this);
        }
    }
}
