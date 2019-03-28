using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Syroot.NintenTools.NSW.Bfres.GFX;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library.Rendering;
using Bfres.Structs;

namespace FirstPlugin
{
    public partial class BfresModelImportSettings : STForm
    {
        public BfresModelImportSettings()
        {
            InitializeComponent();

            CanResize = false;

            tabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;

            ExternalMaterialPath = PluginRuntime.ExternalFMATPath;
            if (System.IO.File.Exists(PluginRuntime.ExternalFMATPath))
                textBoxMaterialPath.Text = System.IO.Path.GetFileName(PluginRuntime.ExternalFMATPath);
            else
            {
                textBoxMaterialPath.BackColor = System.Drawing.Color.DarkRed;
                textBoxMaterialPath.Text = "(Select Material!)";
                ExternalMaterialPath = "";
            }
        }
        public bool EnablePositions;
        public bool EnableNormals;
        public bool EnableUV0;
        public bool EnableUV1;
        public bool EnableUV2;
        public bool EnableTangents;
        public bool EnableBitangents;
        public bool EnableWeights;
        public bool EnableIndices;
        public bool EnableVertexColors;
        public bool FlipUVsVertical;
        public bool FlipUVsHorizontal;
        public bool ImportBones;
        public bool Rotate90DegreesY;
        public bool Rotate90DegreesX;
        public bool Rotate90DegreesNegativeY;
        public bool Rotate90DegreesNegativeX;
        public bool RecalculateNormals;
        public string ExternalMaterialPath;
        public bool SetDefaultParamData;
        public int SkinCountLimit;

        public void DisableMaterialEdits()
        {
            textBoxMaterialPath.Visible = false;
            chkBoxImportMat.Checked = false;
        }
        public void EnableMaterialEdits()
        {
            textBoxMaterialPath.Visible = true;
            chkBoxImportMat.Checked = true;
        }

        public void SetModelAttributes(STGenericObject obj)
        {
            chkBoxEnablePositions.Enabled = true;
            chkBoxEnablePositions.Checked = obj.HasPos;
            chkBoxEnableNormals.Checked = obj.HasNrm;
            chkBoxEnableUVs.Checked = obj.HasUv0;
            chkBoxEnableTans.Checked = obj.HasUv0;
            chkBoxEnableBitans.Checked = obj.HasUv0;
            chkBoxEnableWeightIndices.Checked = obj.HasWeights;
            chkBoxEnableVertColors.Checked = obj.HasVertColors;
            chkBoxParamDefaults.Checked = true;
            chkBoxTransformMatrix.Checked = true;

            if (!obj.HasPos)
                DisableAttribute(chkBoxEnablePositions, comboBoxFormatPositions);
            if (!obj.HasNrm)
                DisableAttribute(chkBoxEnableNormals, comboBoxFormatPositions);
            if (!obj.HasUv0)
                DisableAttribute(chkBoxEnableUVs, comboBoxFormatUvs);
            //Note. Bitans/tans uses uvs to generate
            if (!obj.HasUv0)
                DisableAttribute(chkBoxEnableTans, comboBoxFormatTangents);
            if (!obj.HasUv0)
                DisableAttribute(chkBoxEnableBitans, comboBoxFormatBitans);
            if (!obj.HasWeights && !obj.HasIndices)
            {
                DisableAttribute(chkBoxEnableWeightIndices, comboBoxFormatWeights);
                DisableAttribute(chkBoxEnableWeightIndices, comboBoxFormatIndices);
            }
            if (!obj.HasVertColors)
                DisableAttribute(chkBoxEnableVertColors, comboBoxFormatVertexColors);

            EnableUV1 = obj.HasUv1;
            EnableUV2 = obj.HasUv2;
        }
        public List<FSHP.VertexAttribute> CreateNewAttributes()
        {
            List<FSHP.VertexAttribute> attribute = new List<FSHP.VertexAttribute>();

            if (EnablePositions)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_p0";
                att.Format = (AttribFormat)comboBoxFormatPositions.SelectedItem;
                attribute.Add(att);
            }
            if (EnableNormals)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_n0";
                att.Format = (AttribFormat)comboBoxFormatNormals.SelectedItem;
                attribute.Add(att);
            }
            if (EnableVertexColors)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_c0";
                att.Format = (AttribFormat)comboBoxFormatVertexColors.SelectedItem;
                attribute.Add(att);
            }
            if (EnableUV0)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_u0";
                att.Format = (AttribFormat)comboBoxFormatUvs.SelectedItem;
                attribute.Add(att);
            }
            if (EnableUV1 && EnableUV0)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_u1";
                att.Format = (AttribFormat)comboBoxFormatUvs.SelectedItem;
                attribute.Add(att);
            }
            if (EnableUV2 && EnableUV0)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_u2";
                att.Format = (AttribFormat)comboBoxFormatUvs.SelectedItem;
                attribute.Add(att);
            }
            if (EnableTangents)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_t0";
                att.Format = (AttribFormat)comboBoxFormatTangents.SelectedItem;
                attribute.Add(att);
            }
            if (EnableBitangents)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_b0";
                att.Format = (AttribFormat)comboBoxFormatBitans.SelectedItem;
                attribute.Add(att);
            }
            if (EnableWeights)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_w0";
                att.Format = (AttribFormat)comboBoxFormatWeights.SelectedItem;
                attribute.Add(att);
            }
            if (EnableIndices)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_i0";
                att.Format = (AttribFormat)comboBoxFormatIndices.SelectedItem;
                attribute.Add(att);
            }
            return attribute;
        }
        private string GetCmboxString(ComboBox comboBox)
        {
            return comboBox.GetItemText(comboBox.SelectedItem);
        }
        private void DisableAttribute(CheckBox checkBox, ComboBox comboBox)
        {

        }

        //Based on Wexos Toolbox since I'm not sure what formats to use for each attribute
        //Thanks Wexos!
        private void BfresModelImportSettings_Load(object sender, EventArgs e)
        {
            comboBoxFormatPositions.Items.Add(AttribFormat.Format_32_32_32_Single);
            comboBoxFormatPositions.Items.Add(AttribFormat.Format_16_16_16_16_Single);
            comboBoxFormatPositions.Items.Add(AttribFormat.Format_16_16_16_16_SNorm);
            comboBoxFormatPositions.Items.Add(AttribFormat.Format_10_10_10_2_SNorm);
            comboBoxFormatPositions.Items.Add(AttribFormat.Format_8_8_8_8_SNorm);
            comboBoxFormatPositions.SelectedIndex = 0;

            comboBoxFormatNormals.Items.Add(AttribFormat.Format_32_32_32_Single);
            comboBoxFormatNormals.Items.Add(AttribFormat.Format_16_16_16_16_Single);
            comboBoxFormatNormals.Items.Add(AttribFormat.Format_16_16_16_16_SNorm);
            comboBoxFormatNormals.Items.Add(AttribFormat.Format_10_10_10_2_SNorm);
            comboBoxFormatNormals.Items.Add(AttribFormat.Format_8_8_8_8_SNorm);
            comboBoxFormatNormals.SelectedIndex = 3;

            comboBoxFormatIndices.Items.Add(AttribFormat.Format_32_32_32_32_UInt);
            comboBoxFormatIndices.Items.Add(AttribFormat.Format_16_16_16_16_UInt);
            comboBoxFormatIndices.Items.Add(AttribFormat.Format_8_8_8_8_UInt);
            comboBoxFormatIndices.Items.Add(AttribFormat.Format_32_32_32_UInt);
            comboBoxFormatIndices.Items.Add(AttribFormat.Format_32_32_UInt);
            comboBoxFormatIndices.Items.Add(AttribFormat.Format_16_16_UInt);
            comboBoxFormatIndices.Items.Add(AttribFormat.Format_8_8_UInt);
            comboBoxFormatIndices.Items.Add(AttribFormat.Format_32_UInt);
            comboBoxFormatIndices.Items.Add(AttribFormat.Format_16_UInt);
            comboBoxFormatIndices.Items.Add(AttribFormat.Format_8_UInt);
            comboBoxFormatIndices.SelectedIndex = 2;

            comboBoxFormatWeights.Items.Add(AttribFormat.Format_32_32_32_32_Single);
            comboBoxFormatWeights.Items.Add(AttribFormat.Format_16_16_16_16_UNorm);
            comboBoxFormatWeights.Items.Add(AttribFormat.Format_8_8_8_8_UNorm);


            comboBoxFormatWeights.Items.Add(AttribFormat.Format_32_32_32_Single);
            comboBoxFormatWeights.Items.Add(AttribFormat.Format_32_32_Single);
            comboBoxFormatWeights.Items.Add(AttribFormat.Format_16_16_Single);
            comboBoxFormatWeights.Items.Add(AttribFormat.Format_16_16_UNorm);
            comboBoxFormatWeights.Items.Add(AttribFormat.Format_8_8_UNorm);
            comboBoxFormatWeights.Items.Add(AttribFormat.Format_8_UNorm);
            comboBoxFormatWeights.SelectedIndex = 2;

            comboBoxFormatTangents.Items.Add(AttribFormat.Format_32_32_32_32_Single);
            comboBoxFormatTangents.Items.Add(AttribFormat.Format_16_16_16_16_Single);
            comboBoxFormatTangents.Items.Add(AttribFormat.Format_16_16_16_16_SNorm);
            comboBoxFormatTangents.Items.Add(AttribFormat.Format_10_10_10_2_SNorm);
            comboBoxFormatTangents.Items.Add(AttribFormat.Format_8_8_8_8_SNorm);
            comboBoxFormatTangents.SelectedIndex = 4;

            comboBoxFormatBitans.Items.Add(AttribFormat.Format_32_32_32_32_Single);
            comboBoxFormatBitans.Items.Add(AttribFormat.Format_16_16_16_16_Single);
            comboBoxFormatBitans.Items.Add(AttribFormat.Format_16_16_16_16_SNorm);
            comboBoxFormatBitans.Items.Add(AttribFormat.Format_10_10_10_2_SNorm);
            comboBoxFormatBitans.Items.Add(AttribFormat.Format_8_8_8_8_SNorm);
            comboBoxFormatBitans.SelectedIndex = 4;

            comboBoxFormatUvs.Items.Add(AttribFormat.Format_32_32_Single);
            comboBoxFormatUvs.Items.Add(AttribFormat.Format_16_16_Single);
            comboBoxFormatUvs.Items.Add(AttribFormat.Format_16_16_SNorm);
            comboBoxFormatUvs.Items.Add(AttribFormat.Format_8_8_SNorm);
            comboBoxFormatUvs.Items.Add(AttribFormat.Format_8_8_UNorm);
            comboBoxFormatUvs.SelectedIndex = 1;

            comboBoxFormatVertexColors.Items.Add(AttribFormat.Format_32_32_Single);
            comboBoxFormatVertexColors.Items.Add(AttribFormat.Format_16_16_Single);
            comboBoxFormatVertexColors.Items.Add(AttribFormat.Format_16_16_SNorm);
            comboBoxFormatVertexColors.Items.Add(AttribFormat.Format_10_10_10_2_SNorm);
            comboBoxFormatVertexColors.Items.Add(AttribFormat.Format_8_8_SNorm);
            comboBoxFormatVertexColors.SelectedIndex = 3;

            comboBoxFormatFaces.Items.Add(IndexFormat.UInt16);
            comboBoxFormatFaces.Items.Add(IndexFormat.UInt32);
            comboBoxFormatFaces.SelectedIndex = 0;
        }

        private void chkBoxEnableAttribute_CheckedChanged(object sender, EventArgs e)
        {
            EnablePositions = chkBoxEnablePositions.Checked;
            EnableNormals = chkBoxEnableNormals.Checked;
            EnableUV0 = chkBoxEnableUVs.Checked;
            EnableTangents = chkBoxEnableTans.Checked;
            EnableBitangents = chkBoxEnableBitans.Checked;
            EnableWeights = chkBoxEnableWeightIndices.Checked;
            EnableIndices = chkBoxEnableWeightIndices.Checked;
            EnableVertexColors = chkBoxEnableVertColors.Checked;
        }
        private void chkBoxSettings_CheckedChanged(object sender, EventArgs e)
        {
            FlipUVsVertical = chkBoxFlipUvsY.Checked;
            Rotate90DegreesY = chkBoxRot90Y.Checked;
            Rotate90DegreesNegativeY = chkBoxRotNegative90Y.Checked;
            RecalculateNormals = chkBoxRecalcNormals.Checked;
            SetDefaultParamData = chkBoxParamDefaults.Checked;
            ImportBones = chkBoxImportBones.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Only check path if material editing is enabled
            if (!System.IO.File.Exists(ExternalMaterialPath) && textBoxMaterialPath.Visible)
            {
                this.DialogResult = DialogResult.None;
                MessageBox.Show("Please set a path from an exported material! You can export one by right clicking a material and export!", "",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxMaterialPath.BackColor = System.Drawing.Color.DarkRed;
            }
            else
                this.DialogResult = DialogResult.OK;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfmat;";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                ExternalMaterialPath = ofd.FileName;
                textBoxMaterialPath.Text = System.IO.Path.GetFileName(ofd.FileName);
                textBoxMaterialPath.BackColor = System.Drawing.Color.FromArgb(64,64,64);
            }
        }

        private void chkBoxImportMat_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBoxImportMat.Checked == false)
                DisableMaterialEdits();
            else
                EnableMaterialEdits();
        }

        private void numericUpDownInt1_ValueChanged(object sender, EventArgs e)
        {
            SkinCountLimit = (int)numericUpDownInt1.Value;
        }
    }
}
