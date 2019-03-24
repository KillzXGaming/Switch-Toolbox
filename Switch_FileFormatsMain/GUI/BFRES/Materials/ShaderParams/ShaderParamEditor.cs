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
    public partial class ShaderParamEditor : STUserControl
    {
        public ShaderParamEditor()
        {
            InitializeComponent();
        }

        FMAT material;

        public void InitializeShaderParamList(FMAT mat)
        {
            material = mat;

            stFlowLayoutPanel1.SuspendLayout();
            stFlowLayoutPanel1.Controls.Clear();


            foreach (BfresShaderParam param in mat.matparam.Values)
            {
                if (param.Type == ShaderParamType.Bool ||
                    param.Type == ShaderParamType.Bool2 ||
                    param.Type == ShaderParamType.Bool3 ||
                    param.Type == ShaderParamType.Bool4)
                {
                    booleanPanel panel = new booleanPanel(param.ValueBool, param);
                    LoadDropPanel(panel, param);
                }
                if (param.Type == ShaderParamType.TexSrt)
                {
                    TexSrtPanel panel = new TexSrtPanel(param.ValueTexSrt, param);
                    LoadDropPanel(panel, param);
                }
                if (param.Type == ShaderParamType.Float)
                {
                    vector1SliderPanel panel = new vector1SliderPanel(param.ValueFloat, param);
                    LoadDropPanel(panel, param);
                }
                if (param.Type == ShaderParamType.Float2)
                {
                    vector2SliderPanel panel = new vector2SliderPanel(param.ValueFloat, param);
                    LoadDropPanel(panel, param);
                }
                if (param.Type == ShaderParamType.Float3)
                {
                    vector3SliderPanel panel = new vector3SliderPanel(param.Name, param.ValueFloat, param);
                    LoadDropPanel(panel, param);
                }
                if (param.Type == ShaderParamType.Float4)
                {
                    vector4SliderPanel panel = new vector4SliderPanel(param.Name, param.ValueFloat, param);
                    LoadDropPanel(panel, param);
                }
                if (param.Type == ShaderParamType.UInt)
                {
                    vector1SliderPanel panel = new vector1SliderPanel(param.ValueUint, param);
                    LoadDropPanel(panel, param);
                }
                if (param.Type == ShaderParamType.UInt2)
                {
                    vector2SliderPanel panel = new vector2SliderPanel(param.ValueUint, param);
                    LoadDropPanel(panel, param);
                }
                if (param.Type == ShaderParamType.UInt3)
                {
                    vector3SliderPanel panel = new vector3SliderPanel(param.Name, param.ValueUint, param);
                    LoadDropPanel(panel, param);
                }
                if (param.Type == ShaderParamType.UInt4)
                {
                    vector4SliderPanel panel = new vector4SliderPanel(param.Name, param.ValueUint, param);
                    LoadDropPanel(panel, param);
                }
                if (param.Type == ShaderParamType.Int)
                {
                    vector1SliderPanel panel = new vector1SliderPanel(param.ValueInt, param);
                    LoadDropPanel(panel, param);
                }
                if (param.Type == ShaderParamType.Int2)
                {
                    vector2SliderPanel panel = new vector2SliderPanel(param.ValueInt, param);
                    LoadDropPanel(panel, param);
                }
                if (param.Type == ShaderParamType.Int3)
                {
                    vector3SliderPanel panel = new vector3SliderPanel(param.Name, param.ValueInt, param);
                    LoadDropPanel(panel, param);
                }
                if (param.Type == ShaderParamType.Int4)
                {
                    vector4SliderPanel panel = new vector4SliderPanel(param.Name, param.ValueInt, param);
                    LoadDropPanel(panel, param);
                }
            }

            stFlowLayoutPanel1.ResumeLayout();
        }

        public bool OnValueChanged(BfresShaderParam param, UserControl sender)
        {
            if (param == null || sender.Parent == null)
                return false;

            STDropDownPanel panel = (STDropDownPanel)sender.Parent;

            panel.PanelValueName = GetValueString(param);
            return true;
        }

        public void LoadDropPanel(ParamValueEditorBase control, BfresShaderParam param)
        {
            STDropDownPanel panel = new STDropDownPanel();
            panel.SuspendLayout();
            panel.PanelName = param.Name;
            panel.PanelValueName = GetValueString(param);
            panel.Controls.Add(control);
            panel.Height = control.Height;
            panel.IsExpanded = false;

            control.BackColor = FormThemes.BaseTheme.DropdownPanelBackColor;
            control.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left;
            control.Width = panel.Width;
            control.LoadAction(OnValueChanged); //To update value test

            if (control is vector4SliderPanel)
                panel.SetIconColor = ((vector4SliderPanel)control).GetColor();
            if (control is vector4SliderPanel)
                panel.SetIconAlphaColor = ((vector4SliderPanel)control).GetAlphaColor();
            if (control is vector3SliderPanel)
                panel.SetIconColor = ((vector3SliderPanel)control).GetColor();

            panel.ResumeLayout();

            stFlowLayoutPanel1.Controls.Add(panel);
        }

        private string GetValueString(BfresShaderParam param)
        {
            string Values = "";

            switch (param.Type)
            {

                case ShaderParamType.Float:
                    Values = $"[ {RoundParam(param.ValueFloat[0])} ]";
                    break;
                case ShaderParamType.Float2:
                    Values = $"[ {RoundParam(param.ValueFloat[0])} ," +
                              $" {RoundParam(param.ValueFloat[1])} ]";
                    break;
                case ShaderParamType.Float3:
                    Values = $"[ {RoundParam(param.ValueFloat[0])} ," +
                              $" {RoundParam(param.ValueFloat[1])} ," +
                             $"  {RoundParam(param.ValueFloat[2])} ,]";
                    break;
                case ShaderParamType.Float4:
                    Values = $"[ {RoundParam(param.ValueFloat[0])} ," +
                              $" {RoundParam(param.ValueFloat[1])} ," +
                             $"  {RoundParam(param.ValueFloat[2])} ," +
                             $"  {RoundParam(param.ValueFloat[3])} ]";
                    break;
                case ShaderParamType.TexSrt:
                    Values = $"[ {param.ValueTexSrt.Mode} ," +
                             $" {RoundParam(param.ValueTexSrt.Scaling.X)} ," +
                             $" {RoundParam(param.ValueTexSrt.Scaling.Y)} ," +
                             $" {RoundParam(param.ValueTexSrt.Rotation)}, " +
                             $" {RoundParam(param.ValueTexSrt.Translation.X)}," +
                             $" {RoundParam(param.ValueTexSrt.Translation.X)} ]";
                    break;
            }

            Console.WriteLine(String.Format("{0,-30} {1,-30}", param.Name, Values));

            return Values;
        }

        private float RoundParam(float Value) {
            return (float)Math.Round(Value, 2);
        }
    }
}
