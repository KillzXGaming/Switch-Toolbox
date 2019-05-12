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
using Switch_Toolbox.Library;
using OpenTK;

namespace FirstPlugin.Forms
{
    public partial class ShaderParamEditor : STUserControl
    {
        public ShaderParamEditor()
        {
            InitializeComponent();
        }

        public ImageList il = new ImageList();

        FMAT material;

        public void InitializeShaderParamList(FMAT mat)
        {
            material = mat;

            int CurParam = 0;

            shaderParamListView.Items.Clear();
            foreach (BfresShaderParam prm in mat.matparam.Values)
            {
                var item = new ListViewItem(prm.Name);
                ShaderParamToListItem(prm, item);

                shaderParamListView.View = View.Details;
                shaderParamListView.Items.Add(item);
                CurParam++;
            }
            il.ImageSize = new Size(10, 10);
            shaderParamListView.SmallImageList = il;
            shaderParamListView.FullRowSelect = true;
        }

        private void ShaderParamToListItem(BfresShaderParam prm, ListViewItem item )
        {
            item.SubItems.Clear();
            item.Text = prm.Name;

            string DisplayValue = "";

            switch (prm.Type)
            {
                case ShaderParamType.Float:
                case ShaderParamType.Float2:
                case ShaderParamType.Float2x2:
                case ShaderParamType.Float2x3:
                case ShaderParamType.Float2x4:
                case ShaderParamType.Float3x2:
                case ShaderParamType.Float3x3:
                case ShaderParamType.Float3x4:
                case ShaderParamType.Float4x2:
                case ShaderParamType.Float4x3:
                case ShaderParamType.Float4x4:
                    DisplayValue = SetValueToString(prm.ValueFloat);
                    break;
                case ShaderParamType.Float3:
                    DisplayValue = SetValueToString(prm.ValueFloat);
                    break;
                case ShaderParamType.Float4:
                    DisplayValue = SetValueToString(prm.ValueFloat);
                    break;
            }

            item.UseItemStyleForSubItems = false;
            item.SubItems.Add(DisplayValue);
            item.SubItems.Add("");
            item.SubItems[2].BackColor = GetColor(prm);
        }

        private Color GetColor(BfresShaderParam prm)
        {
            Vector4 col = new Vector4();

            switch (prm.Type)
            {
                case ShaderParamType.Float3:
                    col = new Vector4(prm.ValueFloat[0], prm.ValueFloat[1], prm.ValueFloat[2], 1);
                    break;
                case ShaderParamType.Float4:
                    col = new Vector4(prm.ValueFloat[0], prm.ValueFloat[1], prm.ValueFloat[2], prm.ValueFloat[3]);
                    break;
            }

            bool IsColor = prm.Name.Contains("Color") || prm.Name.Contains("color");

            Color SetColor = Color.FromArgb(40, 40, 40);

            if (IsColor)
            {
                SetColor = Color.FromArgb(
                255,
                Utils.FloatToIntClamp(col.X),
                Utils.FloatToIntClamp(col.Y),
                Utils.FloatToIntClamp(col.Z)
                );
            }

            return SetColor;
        }

        private string SetValueToString(object values)
        {
            if (values is float[])
                return string.Join(" , ", values as float[]);
            else if (values is bool[])
                return string.Join(" , ", values as bool[]);
            else if (values is int[])
                return string.Join(" , ", values as int[]);
            else if (values is uint[])
                return string.Join(" , ", values as uint[]);
            else
                return "";
        }

        STFlowLayoutPanel stFlowLayoutPanel1;
        private void LoadDropDownPanel(BfresShaderParam param)
        {
            stFlowLayoutPanel1 = new STFlowLayoutPanel();
            stFlowLayoutPanel1.Dock = DockStyle.Fill;
            stFlowLayoutPanel1.SuspendLayout();

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

            stFlowLayoutPanel1.ResumeLayout();
        }

        public bool OnValueChanged(BfresShaderParam param, UserControl sender)
        {
            if (param == null || sender.Parent == null)
                return false;

            STDropDownPanel panel = (STDropDownPanel)sender.Parent;
            panel.PanelValueName = GetValueString(param);


            panel.Refresh();

            return true;
        }

        public void LoadDialogDropPanel(ParamValueDialog control, BfresShaderParam param)
        {
            ParamValueEditorBase panel = new ParamValueEditorBase();

            switch (param.Type)
            {
                case ShaderParamType.Float: panel = new vector1SliderPanel(param.ValueFloat, param); break;
                case ShaderParamType.Float2: panel = new vector2SliderPanel(param.ValueFloat, param); break;
                case ShaderParamType.Float3: panel = new vector3SliderPanel(param.Name, param.ValueFloat, param); break;
                case ShaderParamType.Float4: panel = new vector4SliderPanel(param.Name, param.ValueFloat, param); break;
                case ShaderParamType.Int: panel = new vector1SliderPanel(param.ValueFloat, param); break;
                case ShaderParamType.Int2: panel = new vector2SliderPanel(param.ValueFloat, param); break;
                case ShaderParamType.Int3: panel = new vector3SliderPanel(param.Name, param.ValueFloat, param); break;
                case ShaderParamType.Int4: panel = new vector4SliderPanel(param.Name, param.ValueFloat, param); break;
                case ShaderParamType.UInt: panel = new vector1SliderPanel(param.ValueFloat, param); break;
                case ShaderParamType.UInt2: panel = new vector2SliderPanel(param.ValueFloat, param); break;
                case ShaderParamType.UInt3: panel = new vector3SliderPanel(param.Name, param.ValueFloat, param); break;
                case ShaderParamType.UInt4: panel = new vector4SliderPanel(param.Name, param.ValueFloat, param); break;
                case ShaderParamType.TexSrt: panel = new TexSrtPanel(param.ValueTexSrt,param); break;
                case ShaderParamType.Bool: panel = new booleanPanel(param.ValueBool, param); break;
                case ShaderParamType.Bool2: panel = new booleanPanel(param.ValueBool, param); break;
                case ShaderParamType.Bool3: panel = new booleanPanel(param.ValueBool, param); break;
                case ShaderParamType.Bool4: panel = new booleanPanel(param.ValueBool, param); break;
            }
            control.Width = panel.Width;
            control.Height = panel.Height + 70;
            control.CanResize = false;
            control.BackColor = FormThemes.BaseTheme.DropdownPanelBackColor;
            control.AddControl(panel);
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

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Material Params|*.xml;";
            sfd.DefaultExt = ".xml";
            sfd.FileName = material.Text + ".MatParams";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FMAT2XML.Save(material, sfd.FileName, true);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Material Params|*.xml;";
            ofd.DefaultExt = ".xml";
            ofd.FileName = material.Text + ".MatParams";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FMAT2XML.Read(material, ofd.FileName, true);
            }
        }

        private void shaderParamListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void shaderParamListView_Click(object sender, EventArgs e)
        {
          
        }

        private void shaderParamListView_DoubleClick(object sender, EventArgs e)
        {
            if (shaderParamListView.SelectedItems.Count > 0)
            {
                var currentItem = shaderParamListView.SelectedItems[0];

                if (material.matparam.ContainsKey(currentItem.Text))
                {
                    ParamValueDialog dialog = new ParamValueDialog();
                    LoadDialogDropPanel(dialog, material.matparam[currentItem.Text]);
                    dialog.Location = currentItem.Position;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        ShaderParamToListItem(material.matparam[currentItem.Text], shaderParamListView.SelectedItems[0]);
                    }
                }
            }
        }
    }
}
