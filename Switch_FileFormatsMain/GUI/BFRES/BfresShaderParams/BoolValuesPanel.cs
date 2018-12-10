using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Bfres.Structs;
using Syroot.NintenTools.NSW.Bfres;

namespace FirstPlugin
{
    public partial class BoolValuesPanel : UserControl
    {
        public BoolValuesPanel()
        {
            InitializeComponent();
        }
        public void SetValues(BfresShaderParam param)
        {
            switch (param.Type)
            {
                case ShaderParamType.Bool:
                    param.ValueBool = new bool[] { bool1.Visible};
                    break;
                case ShaderParamType.Bool2:
                    param.ValueBool = new bool[] { bool1.Visible, bool2.Visible };
                    break;
                case ShaderParamType.Bool3:
                    param.ValueBool = new bool[] { bool1.Visible, bool2.Visible,
                                                   bool3.Visible };
                    break;
                case ShaderParamType.Bool4:
                    param.ValueBool = new bool[] { bool1.Visible, bool2.Visible,
                                                   bool3.Visible , bool4.Visible };
                    break;
            }
        }
        public void LoadValues(bool[] values)
        {
            bool1.Visible = false;
            bool2.Visible = false;
            bool3.Visible = false;
            bool4.Visible = false;

            if (values.Length >= 1)
            {
                bool1.Visible = true;
                bool1.Checked = values[0];
            }
            if (values.Length >= 2)
            {
                bool2.Visible = true;
                bool2.Checked = values[1];
            }
            if (values.Length >= 3)
            {
                bool3.Visible = true;
                bool3.Checked = values[2];
            }
            if (values.Length >= 4)
            {
                bool4.Visible = true;
                bool4.Checked = values[3];
            }
        }

        private void BoolValuesPanel_Load(object sender, EventArgs e)
        {

        }

        private void bool_CheckedChanged(object sender, EventArgs e)
        {
            Viewport.Instance.UpdateViewport();
        }
    }
}
