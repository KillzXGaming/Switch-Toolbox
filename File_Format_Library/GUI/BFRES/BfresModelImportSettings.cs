using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Syroot.NintenTools.NSW.Bfres.GFX;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.Rendering;
using Bfres.Structs;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres;
using static OpenTK.Graphics.OpenGL.GL;

namespace FirstPlugin
{
    public partial class BfresModelImportSettings : STForm
    {
        public List<STGenericObject> NewMeshlist = new List<STGenericObject>();

        public BfresModelImportSettings()
        {
            InitializeComponent();

            CanResize = false;
            ogSkinCountChkBox.Checked = false;
            chkMapOriginalMaterials.Checked = true;

            lodCountUD.Value = 2;

            tabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;

            foreach (GamePreset val in Enum.GetValues(typeof(GamePreset))) {
                gamePresetCB.Items.Add(val);
            }

            gamePresetCB.SelectedIndex = 0;

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
        public bool ResetUVParams;
        public bool ResetColorParams;

        public bool CombineUVs => combineUVs.Checked;

        public bool LimitSkinCount => ogSkinCountChkBox.Checked;
        public bool MapOriginalMaterials
        {
            get
            {
               return chkMapOriginalMaterials.Checked && !chkBoxImportMat.Checked;
            }
        }

        public bool CreateDummyLODs => chkCreateDummyLODs.Checked;
        public int DummyLODCount => (int)lodCountUD.Value;

        public bool UseOriginalAttributes => chkOriginalAttributesFormats.Checked;
        public bool UseOriginalAttributeFormats => chkOriginalAttributesFormats.Checked;

        public bool GeneratePlaceholderTextures = true;

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

        public enum GamePreset
        {
            Default = 0,
            BreathOfTheWild = 1,
            WindWakerHD,
            SuperMario3DWorld,
        }

        public void LoadNewMeshData(List<STGenericObject> Shapes)
        {
            NewMeshlist = Shapes;

            assimpMeshListView.BeginUpdate();
            for (int i = 0; i < NewMeshlist.Count; i++)
                assimpMeshListView.Items.Add(NewMeshlist[i].ObjectName);
            assimpMeshListView.EndUpdate();
        }

        public void LoadOriginalMeshData(List<FSHP> Shapes)
        {
            originalMeshListView.BeginUpdate();
            for (int i = 0; i < Shapes.Count; i++)
                originalMeshListView.Items.Add(Shapes[i].Text);
            originalMeshListView.EndUpdate();
        }

        public void SetModelAttributes(List<STGenericObject> objects)
        {
            chkBoxEnablePositions.Enabled = true;
            chkBoxEnablePositions.Checked = objects.Any(o => o.HasPos);
            chkBoxEnableNormals.Checked = objects.Any(o => o.HasNrm);
            chkBoxEnableUVs.Checked = objects.Any(o => o.HasUv0);
            chkBoxEnableTans.Checked = objects.Any(o => o.HasUv0);
            chkBoxEnableBitans.Checked = objects.Any(o => o.HasUv0);
            chkBoxEnableWeightIndices.Checked = objects.Any(o => o.HasWeights);
            chkBoxEnableVertColors.Checked = objects.Any(o => o.HasVertColors);
            chkResetUVParams.Checked = true;
            chkBoxTransformMatrix.Checked = true;

            if (!objects.Any(o => o.HasPos))
                DisableAttribute(chkBoxEnablePositions, comboBoxFormatPositions);
            if (!objects.Any(o => o.HasNrm))
                DisableAttribute(chkBoxEnableNormals, comboBoxFormatPositions);
            if (!objects.Any(o => o.HasUv0))
                DisableAttribute(chkBoxEnableUVs, comboBoxFormatUvs);
            //Note. Bitans/tans uses uvs to generate
            if (!objects.Any(o => o.HasUv0))
                DisableAttribute(chkBoxEnableTans, comboBoxFormatTangents);
            if (!objects.Any(o => o.HasUv0))
                DisableAttribute(chkBoxEnableBitans, comboBoxFormatBitans);
            if (!objects.Any(o => o.HasWeights) && !objects.Any(o => o.HasIndices))
            {
                DisableAttribute(chkBoxEnableWeightIndices, comboBoxFormatWeights);
                DisableAttribute(chkBoxEnableWeightIndices, comboBoxFormatIndices);
            }
            if (!objects.Any(o => o.HasVertColors))
                DisableAttribute(chkBoxEnableVertColors, comboBoxFormatVertexColors);

            EnableUV1 = objects.Any(o => o.HasUv1);
            EnableUV2 = objects.Any(o => o.HasUv2);
        }

        public List<FSHP.VertexAttribute> CreateNewAttributes(List<FSHP.VertexAttribute> Attributes)
        {
            for (int i = 0; i < Attributes.Count; i++)
            {
                if (!UseOriginalAttributeFormats)
                {
                    if (Attributes[i].Name == "_p0")
                        Attributes[i].Format = (AttribFormat)comboBoxFormatPositions.SelectedItem;
                    if (Attributes[i].Name == "_n0")
                        Attributes[i].Format = (AttribFormat)comboBoxFormatNormals.SelectedItem;
                    if (Attributes[i].Name == "_u0")
                        Attributes[i].Format = (AttribFormat)comboBoxFormatUvs.SelectedItem;
                    if (Attributes[i].Name == "_u1")
                        Attributes[i].Format = (AttribFormat)comboBoxFormatUvs.SelectedItem;
                    if (Attributes[i].Name == "_u2")
                        Attributes[i].Format = (AttribFormat)comboBoxFormatUvs.SelectedItem;
                    if (Attributes[i].Name == "_c0")
                        Attributes[i].Format = (AttribFormat)comboBoxFormatVertexColors.SelectedItem;
                    if (Attributes[i].Name == "_t0")
                        Attributes[i].Format = (AttribFormat)comboBoxFormatTangents.SelectedItem;
                    if (Attributes[i].Name == "_b0")
                        Attributes[i].Format = (AttribFormat)comboBoxFormatBitans.SelectedItem;
                    if (Attributes[i].Name == "_w0")
                        Attributes[i].Format = (AttribFormat)comboBoxFormatWeights.SelectedItem;
                    if (Attributes[i].Name == "_i0")
                        Attributes[i].Format = (AttribFormat)comboBoxFormatIndices.SelectedItem;

                    if (CombineUVs && Attributes[i].Name == "_u0")
                        Attributes[i].Format = AttribFormat.Format_16_16_16_16_Single;


                }
            }

            return Attributes;
        }

        public List<FSHP.VertexAttribute> CreateNewAttributes(FMAT material = null)
        {
            Dictionary<string, FSHP.VertexAttribute> attribute = new Dictionary<string, FSHP.VertexAttribute>();

            Console.WriteLine($"EnablePositions {EnablePositions}");
            Console.WriteLine($"EnableNormals {EnableNormals}");
            Console.WriteLine($"EnableVertexColors {EnableVertexColors}");
            Console.WriteLine($"EnableUV0 {EnableUV0}");
            Console.WriteLine($"EnableUV1 {EnableUV1}");
            Console.WriteLine($"EnableUV2 {EnableUV2}");
            Console.WriteLine($"EnableUV2 {EnableUV2}");
            Console.WriteLine($"EnableTangents {EnableTangents}");
            Console.WriteLine($"EnableTangents {EnableTangents}");
            Console.WriteLine($"EnableBitangents {EnableBitangents}");
            Console.WriteLine($"EnableWeights {EnableWeights}");
            Console.WriteLine($"EnableIndices {EnableIndices}");

            if (EnablePositions)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_p0";
                att.Format = (AttribFormat)comboBoxFormatPositions.SelectedItem;
                attribute.Add(att.Name, att);
            }
            if (EnableNormals)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_n0";
                att.Format = (AttribFormat)comboBoxFormatNormals.SelectedItem;
                attribute.Add(att.Name, att);
            }
            if (EnableVertexColors)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_c0";
                att.Format = (AttribFormat)comboBoxFormatVertexColors.SelectedItem;
                attribute.Add(att.Name, att);
            }
            if (EnableUV0)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_u0";
                att.Format = (AttribFormat)comboBoxFormatUvs.SelectedItem;

                if (material.shaderassign.attributes.ContainsValue("_g3d_02_u0_u1"))
                {
                    att.Format = AttribFormat.Format_16_16_16_16_Single;
                    att.Name = "_g3d_02_u0_u1";
                }

                attribute.Add(att.Name, att);
            }
            if (EnableUV1 && EnableUV0 && !attribute.ContainsKey("_g3d_02_u0_u1"))
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_u1";
                att.Format = (AttribFormat)comboBoxFormatUvs.SelectedItem;
                attribute.Add(att.Name, att);
            }

            if (EnableUV2 && EnableUV0)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_u2";
                att.Format = (AttribFormat)comboBoxFormatUvs.SelectedItem;

                if (material.shaderassign.attributes.ContainsValue("_g3d_02_u2_u3"))
                {
                    att.Format = AttribFormat.Format_16_16_16_16_Single;
                    att.Name = "_g3d_02_u2_u3";
                }

                attribute.Add(att.Name, att);
            }
            if (EnableTangents)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_t0";
                att.Format = (AttribFormat)comboBoxFormatTangents.SelectedItem;
                attribute.Add(att.Name, att);
            }
            if (EnableBitangents)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_b0";
                att.Format = (AttribFormat)comboBoxFormatBitans.SelectedItem;
                attribute.Add(att.Name, att);
            }
            if (EnableWeights)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_w0";
                att.Format = (AttribFormat)comboBoxFormatWeights.SelectedItem;
                attribute.Add(att.Name, att);
            }
            if (EnableIndices)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_i0";
                att.Format = (AttribFormat)comboBoxFormatIndices.SelectedItem;
                attribute.Add(att.Name, att);
            }

            if (material.shaderassign.attributes.ContainsValue("_c0") && !EnableVertexColors)
            {
                FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                att.Name = "_c0";
                att.Format = (AttribFormat)comboBoxFormatVertexColors.SelectedItem;
                attribute.Add(att.Name, att);
            }

            for (int i = 1; i < 6; i++)
            {
                if (material.shaderassign.attributes.ContainsValue($"_c{i}"))
                {
                    FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                    att.Name = $"_c{i}";
                    att.Format = (AttribFormat)comboBoxFormatVertexColors.SelectedItem;
                    attribute.Add(att.Name, att);
                }
            }

            switch ((GamePreset)gamePresetCB.SelectedItem)
            {
                //Use single buffer
                case GamePreset.WindWakerHD:
                case GamePreset.SuperMario3DWorld:
                    foreach (var att in attribute.Values)
                        att.BufferIndex = 0;
                    break;
                case GamePreset.BreathOfTheWild:
                    //A bit hacky. The position uses first buffer
                    //Weight data uses 1
                    //The rest besides bitangents use 2, bitans using 3
                    //If no weight data, then it shifts by 1
                    byte posIndex = 0;
                    byte weightIndex = 1;
                    byte dataIndex = 1;
                    if (attribute.ContainsKey("_w0") || attribute.ContainsKey("_i0"))
                        dataIndex += 1;
                    byte bitanIndex = (byte)(dataIndex + 1);

                    if (attribute.ContainsKey("_p0")) attribute["_p0"].BufferIndex = posIndex;
                    if (attribute.ContainsKey("_i0")) attribute["_i0"].BufferIndex = weightIndex;
                    if (attribute.ContainsKey("_w0")) attribute["_w0"].BufferIndex = weightIndex; //Same buffer as indices
                    if (attribute.ContainsKey("_n0")) attribute["_n0"].BufferIndex = dataIndex;
                    if (attribute.ContainsKey("_u0")) attribute["_u0"].BufferIndex = dataIndex;
                    if (attribute.ContainsKey("_u1")) attribute["_u1"].BufferIndex = dataIndex;
                    if (attribute.ContainsKey("_u2")) attribute["_u2"].BufferIndex = dataIndex;
                    if (attribute.ContainsKey("_t0")) attribute["_t0"].BufferIndex = dataIndex;
                    if (attribute.ContainsKey("_c0")) attribute["_c0"].BufferIndex = dataIndex;
                    if (attribute.ContainsKey("_c1")) attribute["_c1"].BufferIndex = dataIndex;
                    if (attribute.ContainsKey("_b0")) attribute["_b0"].BufferIndex = bitanIndex;
                    break;
                default:
                    var attirbutes = attribute.Values.ToList();
                    for (int i = 0; i < attirbutes.Count; i++)
                        attirbutes[i].BufferIndex = (byte)i;
                    return attirbutes;
            }

            return attribute.Values.ToList();
        }
        private string GetCmboxString(ComboBox comboBox)
        {
            return comboBox.GetItemText(comboBox.SelectedItem);
        }

        private void DisableAttribute(CheckBox checkBox, ComboBox comboBox)
        {

        }

        public void UpdateTexturePlaceholderSetting(bool UsePlaceholder)
        {
            GeneratePlaceholderTextures = UsePlaceholder;
            chkPlaceHolderTextures.Checked = GeneratePlaceholderTextures;
        }

        //Based on Wexos Toolbox since I'm not sure what formats to use for each attribute
        //Thanks Wexos!
        private void BfresModelImportSettings_Load(object sender, EventArgs e)
        {
            chkPlaceHolderTextures.Checked = GeneratePlaceholderTextures;

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
            comboBoxFormatUvs.Items.Add(AttribFormat.Format_16_16_UNorm);
            comboBoxFormatUvs.Items.Add(AttribFormat.Format_8_8_SNorm);
            comboBoxFormatUvs.Items.Add(AttribFormat.Format_8_8_UNorm);
            comboBoxFormatUvs.SelectedIndex = 1;

            comboBoxFormatVertexColors.Items.Add(AttribFormat.Format_32_32_32_32_Single);
            comboBoxFormatVertexColors.Items.Add(AttribFormat.Format_16_16_16_16_Single);
            comboBoxFormatVertexColors.Items.Add(AttribFormat.Format_16_16_16_16_SNorm);
            comboBoxFormatVertexColors.Items.Add(AttribFormat.Format_8_8_8_8_UNorm);
            comboBoxFormatVertexColors.Items.Add(AttribFormat.Format_8_8_8_8_SNorm);
            comboBoxFormatVertexColors.SelectedIndex = 1;

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
            ResetUVParams = chkResetUVParams.Checked;
            ResetColorParams = chkResetColorParams.Checked;
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

        private void chkPlaceHolderTextures_CheckedChanged(object sender, EventArgs e)
        {
            GeneratePlaceholderTextures = chkPlaceHolderTextures.Checked;
        }

        private void assimpMeshListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (assimpMeshListView.SelectedIndices.Count == 0)
                return;

            int assimpIndex = assimpMeshListView.SelectedIndices[0];
            objectNameTB.Text = NewMeshlist[assimpIndex].ObjectName;
        }

        private void stTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (assimpMeshListView.SelectedIndices.Count == 0 || objectNameTB.Text == string.Empty)
                return;

            int assimpIndex = assimpMeshListView.SelectedIndices[0];

            if (objectNameTB.Text == originalMeshListView.Items[assimpIndex].Text)
                objectNameTB.BackColor = System.Drawing.Color.Green;
            else
                objectNameTB.BackColor = System.Drawing.Color.DarkRed;

            NewMeshlist[assimpIndex].ObjectName = objectNameTB.Text;
        }

        private void gamePresetCB_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateFormatList((GamePreset)gamePresetCB.SelectedItem);
        }

        private void UpdateFormatList(GamePreset preset)
        {
            if (comboBoxFormatFaces.Items.Count == 0)
                return;

            switch (preset)
            {
                case GamePreset.BreathOfTheWild:
                    comboBoxFormatPositions.SelectedItem = AttribFormat.Format_16_16_16_16_Single;
                    comboBoxFormatNormals.SelectedItem = AttribFormat.Format_10_10_10_2_SNorm;
                    comboBoxFormatTangents.SelectedItem = AttribFormat.Format_8_8_8_8_SNorm;
                    comboBoxFormatBitans.SelectedItem = AttribFormat.Format_8_8_8_8_SNorm;
                    comboBoxFormatVertexColors.SelectedItem = AttribFormat.Format_8_8_8_8_UNorm;
                    comboBoxFormatUvs.SelectedItem = AttribFormat.Format_16_16_UNorm;
                    comboBoxFormatIndices.SelectedItem = AttribFormat.Format_8_8_8_8_UInt;
                    comboBoxFormatWeights.SelectedItem = AttribFormat.Format_8_8_8_8_UNorm;
                    break;
                case GamePreset.WindWakerHD:
                    comboBoxFormatPositions.SelectedItem = AttribFormat.Format_32_32_32_Single;
                    comboBoxFormatNormals.SelectedItem = AttribFormat.Format_32_32_32_Single;
                    comboBoxFormatTangents.SelectedItem = AttribFormat.Format_32_32_32_32_Single;
                    comboBoxFormatBitans.SelectedItem = AttribFormat.Format_32_32_32_32_Single;
                    comboBoxFormatVertexColors.SelectedItem = AttribFormat.Format_32_32_32_32_Single;
                    comboBoxFormatUvs.SelectedItem = AttribFormat.Format_32_32_Single;
                    comboBoxFormatIndices.SelectedItem = AttribFormat.Format_32_32_32_32_UInt;
                    comboBoxFormatWeights.SelectedItem = AttribFormat.Format_32_32_32_32_Single;
                    break;
                default:
                    comboBoxFormatPositions.SelectedItem = AttribFormat.Format_32_32_32_32_Single;
                    comboBoxFormatNormals.SelectedItem = AttribFormat.Format_10_10_10_2_SNorm;
                    comboBoxFormatTangents.SelectedItem = AttribFormat.Format_8_8_8_8_SNorm;
                    comboBoxFormatBitans.SelectedItem = AttribFormat.Format_8_8_8_8_SNorm;
                    comboBoxFormatVertexColors.SelectedItem = AttribFormat.Format_8_8_8_8_UNorm;
                    comboBoxFormatUvs.SelectedItem = AttribFormat.Format_16_16_UNorm;
                    comboBoxFormatIndices.SelectedItem = AttribFormat.Format_8_8_8_8_UInt;
                    comboBoxFormatWeights.SelectedItem = AttribFormat.Format_8_8_8_8_UNorm;
                    break;
            }
        }
    }
}
