using Syroot.NintenTools.NSW.Bfres;
using Bfres.Structs;

namespace FirstPlugin.Forms
{
    public partial class vector1SliderPanel : ParamValueEditorBase
    {
        public vector1SliderPanel(float[] values, BfresShaderParam param) {
            InitializeComponent();

            activeParam = param;
            stTextBox1.Bind(activeParam, "Name");
            barSlider1.DataType = typeof(float);

            barSlider1.Value = values[0];
        }

        public vector1SliderPanel(int[] values, BfresShaderParam param)
        {
            InitializeComponent();

            activeParam = param;
            stTextBox1.Bind(activeParam, "Name");
            barSlider1.DataType = typeof(int);

            barSlider1.Value = values[0];
        }

        public vector1SliderPanel(uint[] values, BfresShaderParam param)
        {
            InitializeComponent();

            activeParam = param;
            stTextBox1.Bind(activeParam, "Name");
            barSlider1.DataType = typeof(uint);

            barSlider1.Value = values[0];
        }

        public void ApplyValueSingles() {
            activeParam.ValueFloat = new float[] { (float)barSlider1.Value };
        }
        public void ApplyValueUint() {
            activeParam.ValueUint = new uint[] { (uint)barSlider1.Value };
        }
        public void ApplyValueInt() {
            activeParam.ValueInt = new int[] { (int)barSlider1.Value };
        }

        private void barSlider1_ValueChanged(object sender, System.EventArgs e)
        {
            if (activeParam.Type == ShaderParamType.UInt)
                ApplyValueUint();
            if (activeParam.Type == ShaderParamType.Int)
                ApplyValueInt();
            if (activeParam.Type == ShaderParamType.Float)
                ApplyValueSingles();

            if (OnPanelChanged != null)
                OnPanelChanged(activeParam, this);
        }
    }
}
