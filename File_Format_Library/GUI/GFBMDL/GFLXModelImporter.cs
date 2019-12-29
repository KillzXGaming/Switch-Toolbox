using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;
using FirstPlugin.GFMDLStructs;

namespace FirstPlugin.Forms
{
    public partial class GFLXModelImporter : STForm
    {
        public bool ImportNewBones => !chkUseOriginalBones.Checked;

        private List<STGenericObject> Meshes;
        private List<GFLXMesh> OriginalMeshes;

        public GfbmdlImportSettings Settings = new GfbmdlImportSettings();

        private bool Loaded = false;

        public int RotationY => (int)rotateModel90YUD.Value;

        public GFLXModelImporter()
        {
            InitializeComponent();

            presetCB.Items.Add("Optimal");
            presetCB.SelectedIndex = 0;

            stPanel1.BackColor = FormThemes.BaseTheme.TextEditorBackColor;
            stPanel2.BackColor = FormThemes.BaseTheme.TextEditorBackColor;

            CanResize = false;
        }

        public void LoadMeshes(List<STGenericObject> meshes, List<STGenericMaterial> importedMats,
                       List<GFLXMaterialData> originalMaterials, List<GFLXMesh> originalMeshes)
        {
            Loaded = false;

            Meshes = meshes;
            OriginalMeshes = originalMeshes;

            foreach (var mat in originalMaterials)
            {
                materiialPresetCB.Items.Add(mat.Text);
            }

            materiialPresetCB.SelectedIndex = 0;

            listViewCustom1.Items.Clear();
            for (int i = 0; i < meshes.Count; i++)
            {
                string material = originalMaterials[0].Text;

                //Check if a material name matches one in the preset list
                //Then auto match the preset
                if (meshes[i].MaterialIndex < importedMats.Count &&
                    meshes[i].MaterialIndex > 0)
                {
                    string importedMat = importedMats[meshes[i].MaterialIndex].Text;
                    if (materiialPresetCB.Items.Contains(importedMat))
                        material = importedMat;
                }

                //Check if color 2 is used (if any vertex colors are enabled)
                bool color2Unused = meshes[i].HasVertColors && !meshes[i].HasVertColors2;
                Settings.MeshSettings.Add(new GfbmdlImportSettings.MeshSetting()
                {
                    HasColor1 = meshes[i].HasVertColors,
                    HasColor2 = meshes[i].HasVertColors,
                    HasTexCoord1 = meshes[i].HasUv0,
                    HasTexCoord2 = meshes[i].HasUv1,
                    HasTexCoord3 = meshes[i].HasUv2,
                    HasNormals = meshes[i].HasNrm,
                    HasBoneIndices = meshes[i].HasIndices,
                    HasWeights = meshes[i].HasWeights,
                    HasTangents = meshes[i].HasUv0,
                    Material = material,
                    SetNormalsToColorChannel2 = color2Unused,
                });
                listViewCustom1.Items.Add($"{meshes[i].ObjectName}");
            }

            SetDefaultFormats();

            Loaded = true;
        }

        private void SetDefaultFormats()
        {
            positionFormatCB.LoadEnum(typeof(BufferFormat));
            positionFormatCB.SelectedItem = BufferFormat.Float;

            normalFormatCB.LoadEnum(typeof(BufferFormat));
            normalFormatCB.SelectedItem = BufferFormat.HalfFloat;

            bitangentFormatCB.LoadEnum(typeof(BufferFormat));
            bitangentFormatCB.SelectedItem = BufferFormat.HalfFloat;

            uv0FormatCB.LoadEnum(typeof(BufferFormat));
            uv0FormatCB.SelectedItem = BufferFormat.Float;

            uv1FormatCB.LoadEnum(typeof(BufferFormat));
            uv1FormatCB.SelectedItem = BufferFormat.Float;

            color0FormatCB.LoadEnum(typeof(BufferFormat));
            color0FormatCB.SelectedItem = BufferFormat.Byte;

            color1FormatCB.LoadEnum(typeof(BufferFormat));
            color1FormatCB.SelectedItem = BufferFormat.Byte;

            boneFormatCB.LoadEnum(typeof(BufferFormat));
            boneFormatCB.SelectedItem = BufferFormat.Byte;

            weightFormatCB.LoadEnum(typeof(BufferFormat));
            weightFormatCB.SelectedItem = BufferFormat.BytesAsFloat;

            tangentFormatCB.LoadEnum(typeof(BufferFormat));
            tangentFormatCB.SelectedItem = BufferFormat.HalfFloat;
        }

        private void TryMatchOriginal()
        {
            if (OriginalMeshes == null || OriginalMeshes?.Count == 0)
                return;

            Loaded = false;
            foreach (var mesh in Meshes)
            {
                var matchedMesh = (GFLXMesh)OriginalMeshes.FirstOrDefault(x => x.Text == mesh.ObjectName);
                if (matchedMesh != null)
                {
                    int index = Meshes.IndexOf(mesh);
                    var setting = Settings.MeshSettings[index];

                    foreach (var attribute in matchedMesh.MeshData.Attributes)
                    {
                        switch ((VertexType)attribute.VertexType)
                        {
                            case VertexType.Position:
                                setting.PositionFormat = (BufferFormat)attribute.BufferFormat;
                                break;
                            case VertexType.Normal:
                                setting.NormalFormat = (BufferFormat)attribute.BufferFormat;
                                setting.HasNormals = true;
                                break;
                            case VertexType.Color1:
                                setting.Color1Format = (BufferFormat)attribute.BufferFormat;
                                setting.HasColor1 = true;
                                break;
                            case VertexType.Color2:
                                setting.Color2Format = (BufferFormat)attribute.BufferFormat;
                                setting.HasColor2 = true;

                                bool hasColors = mesh.vertices.Any(x => x.col2 != OpenTK.Vector4.Zero);
                                if (!hasColors)
                                    setting.SetNormalsToColorChannel2 = true;
                                break;
                            case VertexType.UV1:
                                setting.TexCoord1Format = (BufferFormat)attribute.BufferFormat;
                                setting.HasTexCoord1 = true;
                                break;
                            case VertexType.UV2:
                                setting.TexCoord2Format = (BufferFormat)attribute.BufferFormat;
                                setting.HasTexCoord2 = true;
                                break;
                            case VertexType.UV3:
                                setting.TexCoord3Format = (BufferFormat)attribute.BufferFormat;
                                setting.HasTexCoord3 = true;
                                break;
                            case VertexType.UV4:
                                setting.TexCoord4Format = (BufferFormat)attribute.BufferFormat;
                                setting.HasTexCoord4 = true;
                                break;
                            case VertexType.BoneWeight:
                                setting.BoneWeightFormat = (BufferFormat)attribute.BufferFormat;
                                if (mesh.vertices.FirstOrDefault().boneWeights.Count == 0)
                                {
                                    //Fill weights with 1s
                                    for (int v = 0; v < mesh.vertices.Count; v++)
                                    {
                                        for (int j = 0; j < 1; j++)
                                            mesh.vertices[v].boneWeights.Add(1);
                                    }
                                }
                                setting.HasWeights = true;
                                break;
                            case VertexType.BoneID:
                                setting.BoneIndexFormat = (BufferFormat)attribute.BufferFormat;
                                if (mesh.vertices.FirstOrDefault().boneNames.Count == 0)
                                {
                                    //Fill indices with 1s
                                    for (int v = 0; v < mesh.vertices.Count; v++)
                                    {
                                        for (int j = 0; j < 1; j++)
                                            mesh.vertices[v].boneIds.Add(1);
                                    }
                                }
                                setting.HasBoneIndices = true;
                                break;
                            case VertexType.Tangents:
                                setting.TangentsFormat = (BufferFormat)attribute.BufferFormat;
                                setting.HasTangents = true;
                                break;
                            case VertexType.Bitangent:
                                setting.BitangentnFormat = (BufferFormat)attribute.BufferFormat;
                                setting.HasBitangents = true;
                                break;
                        }
                    }
                }
            }

            Loaded = true;
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e) {
            if (!Loaded) return;

            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                Loaded = false;

                int index = listViewCustom1.SelectedIndices[0];
                var mesh = Meshes[index];

                var settings = Settings.MeshSettings[index];
                materiialPresetCB.SelectedItem = settings.Material;

                UpdateSelectedMesh(settings);

                Loaded = true;
            }
        }

        private void ApplySettings(object sender, EventArgs e)
        {
            if (!Loaded) return;

            Settings.ResetUVTransform = chkResetTexTransforms.Checked;
            Settings.FlipUVsVertical = chkFlipUVsVertical.Checked;

            var indices = listViewCustom1.SelectedIndices;
            for (int i = 0; i < indices?.Count; i++)
            {
                int index = indices[i];
                var mesh = Meshes[index];
                var settings = Settings.MeshSettings[index];

                settings.Material = materiialPresetCB.GetSelectedText();
                settings.HasNormals = chkUseNormals.Checked;
                settings.HasBitangents = chkBitangents.Checked;
                settings.HasTexCoord1 = chkHasUv1.Checked;
                settings.HasTexCoord2 = chkHasUv2.Checked;
                settings.SetNormalsToColorChannel2 = chkSetNormalsToColorChannel.Checked;
                if (settings.SetNormalsToColorChannel2)
                    chkUseColor2.Checked = true;
                settings.HasColor1 = chkUseColor1.Checked;
                settings.HasColor2 = chkUseColor2.Checked;
                settings.HasBoneIndices = chkUseBoneIndex.Checked;
                settings.HasWeights = chkUseBoneWeights.Checked;
                settings.HasTangents = chkTangents.Checked;

                settings.PositionFormat = (BufferFormat)positionFormatCB.SelectedItem;
                settings.NormalFormat = (BufferFormat)normalFormatCB.SelectedItem;
                settings.BitangentnFormat = (BufferFormat)bitangentFormatCB.SelectedItem;
                settings.TexCoord1Format = (BufferFormat)uv0FormatCB.SelectedItem;
                settings.TexCoord2Format = (BufferFormat)uv1FormatCB.SelectedItem;
                settings.Color1Format = (BufferFormat)color0FormatCB.SelectedItem;
                settings.Color2Format = (BufferFormat)color1FormatCB.SelectedItem;
                settings.BoneIndexFormat = (BufferFormat)boneFormatCB.SelectedItem;
                settings.BoneWeightFormat = (BufferFormat)weightFormatCB.SelectedItem;
                settings.TangentsFormat = (BufferFormat)tangentFormatCB.SelectedItem;
            }
        }

        private void UpdateSelectedMesh(GfbmdlImportSettings.MeshSetting settings) {
            chkUseNormals.Checked = settings.HasNormals;
            chkBitangents.Checked = settings.HasBitangents;
            chkHasUv1.Checked = settings.HasTexCoord1;
            chkHasUv2.Checked = settings.HasTexCoord2;
            chkUseColor1.Checked = settings.HasColor1;
            chkUseColor2.Checked = settings.HasColor2 || settings.SetNormalsToColorChannel2;
            chkUseBoneIndex.Checked = settings.HasBoneIndices;
            chkUseBoneWeights.Checked = settings.HasWeights;
            chkTangents.Checked = settings.HasTangents;
            chkSetNormalsToColorChannel.Checked = settings.SetNormalsToColorChannel2;

            positionFormatCB.SelectedItem = settings.PositionFormat;
            normalFormatCB.SelectedItem = settings.NormalFormat;
            bitangentFormatCB.SelectedItem = settings.BitangentnFormat;
            uv0FormatCB.SelectedItem = settings.TexCoord1Format;
            uv1FormatCB.SelectedItem = settings.TexCoord2Format;
            color0FormatCB.SelectedItem = settings.Color1Format;
            color1FormatCB.SelectedItem = settings.Color2Format;
            boneFormatCB.SelectedItem = settings.BoneIndexFormat;
            weightFormatCB.SelectedItem = settings.BoneWeightFormat;
            tangentFormatCB.SelectedItem = settings.TangentsFormat;
        }

        private void stButton1_Click(object sender, EventArgs e) {
            if (!Loaded) return;

            if (listViewCustom1.SelectedIndices.Count == 0) return;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.json;";
            ofd.DefaultExt = "json";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var mat = Newtonsoft.Json.JsonConvert.DeserializeObject<Material>(
                                    System.IO.File.ReadAllText(ofd.FileName));

                if (mat == null) {
                    MessageBox.Show("Invalid material file!", "GFBMDL Importer");
                    return;
                }

                string name = System.IO.Path.GetFileNameWithoutExtension(ofd.FileName);
                materiialPresetCB.Items.Add(name);

                var indices = listViewCustom1.SelectedIndices;
                for (int i = 0; i < indices?.Count; i++)
                {
                    var settings = Settings.MeshSettings[indices[i]];
                    settings.MaterialFile = ofd.FileName;
                    settings.Material = name;
                }

                materiialPresetCB.SelectedItem = name;
            }
        }

        private void chkMatchAttributes_CheckedChanged(object sender, EventArgs e) {
            if (chkMatchAttributes.Checked)
            {
                TryMatchOriginal();
                if (listViewCustom1.SelectedIndices.Count > 0)
                {
                    int index = listViewCustom1.SelectedIndices[0];
                    UpdateSelectedMesh(Settings.MeshSettings[index]);
                }
            }
        }

        private void numericUpDownFloat1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
