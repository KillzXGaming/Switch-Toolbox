using Syroot.NintenTools.NSW.Bfres;
using Bfres.Structs;
using System.Collections.Generic;

namespace FirstPlugin.Forms
{
    public partial class booleanPanel : ParamValueEditorBase
    {
        public booleanPanel(bool[] values, BfresShaderParam param)
        {
            InitializeComponent();

            activeParam = param;
            stTextBox1.Bind(activeParam, "Name");

            if (values.Length >= 1)
                stCheckBox1.Checked = values[0];
            if (values.Length >= 2)
                stCheckBox1.Checked = values[1];
            if (values.Length >= 3)
                stCheckBox1.Checked = values[2];
            if (values.Length >= 4)
                stCheckBox1.Checked = values[3];
        }

        private void barSlider1_ValueChanged(object sender, System.EventArgs e)
        {
            List<bool> values = new List<bool>();

            if (activeParam.Type == ShaderParamType.Bool)
            {
                values.Add(stCheckBox1.Checked);
            }
            if (activeParam.Type == ShaderParamType.Bool2)
            {
                values.Add(stCheckBox1.Checked);
                values.Add(stCheckBox2.Checked);
            }
            if (activeParam.Type == ShaderParamType.Bool3)
            {
                values.Add(stCheckBox1.Checked);
                values.Add(stCheckBox2.Checked);
                values.Add(stCheckBox3.Checked);
            }
            if (activeParam.Type == ShaderParamType.Bool4)
            {
                values.Add(stCheckBox1.Checked);
                values.Add(stCheckBox2.Checked);
                values.Add(stCheckBox3.Checked);
                values.Add(stCheckBox4.Checked);
            }

            activeParam.ValueBool = values.ToArray();

            if (OnPanelChanged != null)
                OnPanelChanged(activeParam, this);
        }
    }
}
