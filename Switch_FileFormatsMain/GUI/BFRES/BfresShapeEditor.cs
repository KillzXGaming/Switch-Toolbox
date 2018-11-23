using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syroot.NintenTools.NSW.Bfres;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Bfres.Structs;

namespace FirstPlugin
{
    public partial class FSHPEditor : UserControl
    {
        public FSHPEditor()
        {
            InitializeComponent();
        }
        FSHP activeShape;
        FMDL activeModel;

        public void LoadObject(FMDL mdl, FSHP shape)
        {
            InitializeControls();

            activeShape = shape;
            activeModel = mdl;



            //Load all the material names unless there's alot
            if (mdl.materials.Count < 1)
            {
                ReloadMaterialList();
            }
            else
            {
                materialComboBox1.Items.Add(shape.GetMaterial().Text);
                materialComboBox1.SelectedIndex = 0;
            }
            for (int i = 0; i < shape.lodMeshes.Count; i++)
            {
                LODMeshCB.Items.Add($"mesh {i}");
            }
            LODMeshCB.SelectedIndex = activeShape.DisplayLODIndex;
        
            textBoxName.Text = shape.Text;
            textBoxBoneIndex.Text = shape.BoneIndex.ToString();
            textBoxMatIndex.Text = shape.MaterialIndex.ToString();

            
            bonesCB.Items.Add(mdl.Skeleton.bones[shape.boneIndx].Text);
            bonesCB.SelectedIndex = 0;
            textBoxVertexSkinCount.Text = shape.VertexSkinCount.ToString();

            if (BFRES.IsWiiU)
            {
       
            }
            else
            {
                if (shape.Shape.Flags == ShapeFlags.SubMeshBoundaryConsistent)
                    checkBoxUseSubMeshBoundryConsistent.Checked = true;
                if (shape.Shape.Flags == ShapeFlags.HasVertexBuffer)
                    checkBoxUseVertexBuffer.Checked = true;
            }


            shaderAttCB.Items.Add("NONE");
            foreach (FSHP.VertexAttribute att in shape.vertexAttributes)
            {
                vtxAttributesCB.Items.Add(att.Name);
                vtxFormatCB.Items.Add(att.Format);

                if (activeShape.GetMaterial().shaderassign.attributes.ContainsValue(att.Name))
                {
                    var VertexShaderAttributre = activeShape.GetMaterial().shaderassign.attributes.FirstOrDefault(x => x.Value == att.Name).Key;

                    shaderAttCB.Items.Add(VertexShaderAttributre);
                }
            }

            if (vtxAttributesCB.Items.Count > 0)
                vtxAttributesCB.SelectedIndex = 0;
            if (vtxFormatCB.Items.Count > 0)
                vtxFormatCB.SelectedIndex = 0;


            Vector3 translate = new Vector3(0);
            Vector3 scale = new Vector3(1);
            Vector4 rotate = new Vector4(0);
            translate = activeShape.boundingBoxes[0].Center;

            transXUD.Value = (decimal)translate.X;
            transYUD.Value = (decimal)translate.Y;
            transZUD.Value = (decimal)translate.Z;
            rotUDX.Value = (decimal)rotate.X;
            rotUDY.Value = (decimal)rotate.Y;
            rotUDZ.Value = (decimal)rotate.Z;
            scaleUDX.Value = (decimal)scale.X;
            scaleUDY.Value = (decimal)scale.Y;
            scaleUDZ.Value = (decimal)scale.Z;

            RenderTools.DrawCube(translate, 2);
        }
        private void InitializeControls()
        {
            IsLoaded = false;
            IsBoneListLoaded = false;
            bonesCB.Items.Clear();
            materialComboBox1.Items.Clear();
            vtxAttributesCB.Items.Clear();
            vtxFormatCB.Items.Clear();
            LODMeshCB.Items.Clear();
            rotMeasureCB.SelectedIndex = 0;
            rotModeCB.SelectedIndex = 0;
        }

        private void materialComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialComboBox1.SelectedIndex >= 0 && IsLoaded)
            {
                activeShape.MaterialIndex = materialComboBox1.SelectedIndex;
                textBoxMatIndex.Text = materialComboBox1.SelectedIndex.ToString();
            }

            Viewport.Instance.UpdateViewport();
        }

        bool IsLoaded = false;
        bool IsBoneListLoaded = false;
        private void materialComboBox1_Click(object sender, EventArgs e)
        {
        }

        private void materialComboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            ReloadMaterialList();
        }
        private void bonesCB_KeyDown(object sender, KeyEventArgs e)
        {
            ReloadBoneList();
        }
        private void materialComboBox1_MouseDown(object sender, MouseEventArgs e)
        {
            ReloadMaterialList();
        }
        private void bonesCB_MouseDown(object sender, MouseEventArgs e)
        {
            ReloadBoneList();
        }
        private void ReloadMaterialList()
        {
            //For optmization purposes. Load a list when used instead
            if (!IsLoaded)
            {
                materialComboBox1.Items.Clear();
                foreach (FMAT mat in activeModel.materials.Values)
                    materialComboBox1.Items.Add(mat.Text);

                materialComboBox1.SelectedIndex = activeShape.MaterialIndex;
                IsLoaded = true;
            }
        }
        private void ReloadBoneList()
        {
            if (!IsBoneListLoaded)
            {
                bonesCB.Items.Clear();
                foreach (var bn in activeModel.Skeleton.bones)
                    bonesCB.Items.Add(bn.Text);

                bonesCB.SelectedIndex = activeShape.BoneIndex;
                IsBoneListLoaded = true;
            }
        }
        private void bonesCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bonesCB.SelectedIndex >= 0 && IsBoneListLoaded)
            {
                activeShape.boneIndx = bonesCB.SelectedIndex;
                textBoxBoneIndex.Text = bonesCB.SelectedIndex.ToString();
            }

            Viewport.Instance.UpdateViewport();
        }

        bool IsSet = false;
        private void numericUD_ValueChanged(object sender, EventArgs e)
        {
            if (IsSet)
            {
                Vector3 translate = new Vector3(0);
                Vector3 scale = new Vector3(0);
                Vector4 rotate = new Vector4(0);
                translate.X = (float)transXUD.Value;
                translate.Y = (float)transYUD.Value;
                translate.Z = (float)transZUD.Value;
                rotate.X = (float)rotUDX.Value;
                rotate.Y = (float)rotUDY.Value;
                rotate.Z = (float)rotUDZ.Value;
                scale.X = (float)scaleUDX.Value;
                scale.Y = (float)scaleUDY.Value;
                scale.Z = (float)scaleUDZ.Value;

                activeShape.TransformPosition(translate, rotate.Xyz, scale);
            }
        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void vtxAttributesCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (vtxAttributesCB.SelectedIndex != -1)
            {
                vtxFormatCB.SelectedIndex = vtxAttributesCB.SelectedIndex;

                string SelectedText = vtxAttributesCB.GetItemText(vtxAttributesCB.SelectedItem);
                if (activeShape.GetMaterial().shaderassign.attributes.ContainsValue(SelectedText))
                {
                    var VertexShaderAttributre = activeShape.GetMaterial().shaderassign.attributes.FirstOrDefault(x => x.Value == SelectedText).Key;

                    shaderAttCB.SelectedIndex = shaderAttCB.FindStringExact(VertexShaderAttributre);
                }
                else
                    shaderAttCB.SelectedIndex = shaderAttCB.FindStringExact("NONE");

            }
        }

        private void btnVertexBuffData_Click(object sender, EventArgs e)
        {
            if (vtxAttributesCB.SelectedIndex != -1)
            {
                string SelectedText = vtxAttributesCB.GetItemText(vtxAttributesCB.SelectedItem);
                LoadAttribute(SelectedText);
            }
        }
        private void LoadAttribute(string attribute)
        {
            VertexAttributeDataList list = new VertexAttributeDataList();
            foreach (Vertex vtx in activeShape.vertices)
            {
                switch (attribute)
                {
                    case "_p0":
                        list.AddVector3(vtx.pos);
                        break;
                    case "_n0":
                        list.AddVector3(vtx.nrm);
                        break;
                    case "_u0":
                        list.AddVector2(vtx.uv0);
                        break;
                    case "_u1":
                        list.AddVector2(vtx.uv1);
                        break;
                    case "_u2":
                        list.AddVector2(vtx.uv2);
                        break;
                    case "_c0":
                        list.AddColor(vtx.col);
                        break;
                    case "_t0":
                        list.AddVector4(vtx.tan);
                        break;
                    case "_b0":
                        list.AddVector4(vtx.bitan);
                        break;
                    case "_w0":
                        list.AddWeights(vtx.weights);
                        break;
                    case "_i0":
                        List<string> boneNames = new List<string>();
                        foreach (int id in vtx.boneIds)
                            boneNames.Add(activeShape.GetBoneNameFromIndex(activeModel, id));
                        list.AddBoneName(boneNames);
                        boneNames = null;
                        break;
                    case "_w1":
                        list.AddWeights(vtx.weights);
                        break;
                    case "_i1":
                        List<string> boneNames2 = new List<string>();
                        foreach (int id in vtx.boneIds)
                            boneNames2.Add(activeShape.GetBoneNameFromIndex(activeModel, id));
                        list.AddBoneName(boneNames2);
                        boneNames2 = null;
                        break;
                }
            }
            list.Show();
        }

        private void textBoxBoneName_TextChanged(object sender, EventArgs e)
        {

        }

        private void shaderAttCB_KeyDown(object sender, KeyEventArgs e)
        {
            //Disable the combo box from being used for now
            e.SuppressKeyPress = true;
        }

        private void btnLODMeshes_Click(object sender, EventArgs e)
        {
            BfresLODMeshEditor bfresLODMeshEditor = new BfresLODMeshEditor();
            bfresLODMeshEditor.LoadLODS(activeShape);
            bfresLODMeshEditor.Show();
        }

        private void LODMeshCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LODMeshCB.SelectedIndex != -1)
            {
                activeShape.DisplayLODIndex = LODMeshCB.SelectedIndex;
                activeShape.UpdateVertexData();
                Viewport.Instance.UpdateViewport();
            }
        }
    }
}
