using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bfres.Structs;
using Syroot.NintenTools.NSW.Bfres;
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class TexSrtPanel : ParamValueEditorBase
    {
        public TexSrtPanel(TexSrt TexSrt, BfresShaderParam param)
        {
            InitializeComponent();

            activeParam = param;
            stTextBox1.Bind(activeParam, "Name");

            scalingModeCN.Bind(typeof(TexSrtMode), TexSrt, "Mode");
            scalingModeCN.SelectedItem = TexSrt.Mode;

            scaXUD.DataType = typeof(float);
            scaYUD.DataType = typeof(float);
            rotXUD.DataType = typeof(float);
            transXUD.DataType = typeof(float);
            transYUD.DataType = typeof(float);

            scaXUD.Value = TexSrt.Scaling.X;
            scaYUD.Value = TexSrt.Scaling.Y;

            rotXUD.Value = TexSrt.Rotation;

            transXUD.Value = TexSrt.Translation.X;
            transYUD.Value = TexSrt.Translation.Y;
        }

        public void ApplyValues()
        {
            activeParam.ValueTexSrt = new TexSrt
            {
                Mode = (TexSrtMode)scalingModeCN.SelectedItem,
                Scaling = new Syroot.Maths.Vector2F(scaXUD.Value, scaYUD.Value),
                Rotation = rotXUD.Value,
                Translation = new Syroot.Maths.Vector2F(transXUD.Value, transYUD.Value),
            };

            if (OnPanelChanged != null)
                OnPanelChanged(activeParam, this);
        }

        private void barSlider_ValueChanged(object sender, EventArgs e)
        {
            ApplyValues();
        }
    }
}
