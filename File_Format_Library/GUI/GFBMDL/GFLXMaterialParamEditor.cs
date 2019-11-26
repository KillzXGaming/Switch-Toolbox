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
using Toolbox.Library.IO;

namespace FirstPlugin.Forms
{
    public partial class GFLXMaterialParamEditor : UserControl
    {
        private GFLXMaterialData ActiveMaterial;

        public GFLXMaterialParamEditor()
        {
            InitializeComponent();

            stDropDownPanel1.ResetColors();
            stDropDownPanel2.ResetColors();
            stDropDownPanel3.ResetColors();
        }

        public void LoadParams(GFLXMaterialData mat) {
            ActiveMaterial = mat;
            ReloadParams();
        }

        private void ReloadParams()
        {
            switchParamCB.Items.Clear();
            valueParamCB.Items.Clear();
            colorParamCB.Items.Clear();

            foreach (var param in ActiveMaterial.SwitchParams.Values) {
                switchParamCB.Items.Add($"{param.Name} {param.Value}");
            }

            foreach (var param in ActiveMaterial.ValueParams.Values) {
                valueParamCB.Items.Add($"{param.Name} {param.Value}");
            }

            foreach (var param in ActiveMaterial.ColorParams.Values) {
                colorParamCB.Items.Add($"{param.Name} {param.Value}");
            }

            if (switchParamCB.Items.Count > 0) switchParamCB.SelectedIndex = 0;
            if (valueParamCB.Items.Count > 0)  valueParamCB.SelectedIndex = 0;
            if (colorParamCB.Items.Count > 0)  colorParamCB.SelectedIndex = 0;
        }

        private void switchParamCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = switchParamCB.SelectedIndex;
            if (index >= 0)
            {
                var param = ActiveMaterial.SwitchParams.ElementAtOrDefault(index).Value;
                stCheckBox1.Checked = param.Value;
            }
        }

        private void valueParamCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = valueParamCB.SelectedIndex;
            if (index >= 0)
            {
                var param = ActiveMaterial.ValueParams.ElementAtOrDefault(index).Value;
                barSlider1.Value = param.Value;
            }
        }

        private void colorParamCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = colorParamCB.SelectedIndex;
            if (index >= 0)
            {
                var param = ActiveMaterial.ColorParams.ElementAtOrDefault(index).Value;
                barSlider2.Value = param.Value.X;
                barSlider3.Value = param.Value.X;
                barSlider4.Value = param.Value.X;

                pictureBox1.BackColor = Color.FromArgb(
                    Utils.FloatToIntClamp(param.Value.X),
                    Utils.FloatToIntClamp(param.Value.Y),
                    Utils.FloatToIntClamp(param.Value.Z));
            }
            else
                pictureBox1.BackColor = Color.White;
        }
    }
}
