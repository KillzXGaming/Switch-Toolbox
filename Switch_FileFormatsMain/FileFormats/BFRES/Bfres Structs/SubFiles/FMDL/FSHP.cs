using System;
using System.Collections.Generic;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres;
using Syroot.NintenTools.NSW.Bfres.Helpers;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;
using Switch_Toolbox.Library.Forms;
using ResU = Syroot.NintenTools.Bfres;
using ResUGX2 = Syroot.NintenTools.Bfres.GX2;
using ResGFX = Syroot.NintenTools.NSW.Bfres.GFX;
using FirstPlugin;
using FirstPlugin.Forms;
using OpenTK;

namespace Bfres.Structs
{
    public class FSHPFolder : TreeNodeCustom
    {
        public FSHPFolder()
        {
            Text = "Objects";
            Name = "FshpFolder";

            ContextMenuStrip = new STContextMenuStrip();

            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Import Object", null, Import, Keys.Control | Keys.I));
            ContextMenuStrip.Items.Add(new ToolStripSeparator());
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export All Objects", null, ExportAll, Keys.Control | Keys.A));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Clear All Objects", null, Clear, Keys.Control | Keys.C));

        }
        private void Clear(object sender, EventArgs args)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all objects? This cannot be undone!", "", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                Nodes.Clear();
                ((FMDL)Parent).shapes.Clear();
                ((FMDL)Parent).UpdateVertexData();
            }
        }
        private void ExportAll(object sender, EventArgs args)
        {
        }
        private void Import(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfobj;*.fbx;*.dae; *.obj;*.csv;|" +
             "Bfres Object (shape/vertices) |*.bfobj|" +
             "FBX |*.fbx|" +
             "DAE |*.dae|" +
             "OBJ |*.obj|" +
             "CSV |*.csv|" +
             "All files(*.*)|*.*";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in ofd.FileNames)
                    ((FMDL)Parent).AddOjects(file, ((FMDL)Parent).GetResFile(), ((FMDL)Parent).GetResFileU(), false);
            }
        }
    
        public override void OnClick(TreeView treeView)
        {

        }
    }
    public struct DisplayVertex
    {
        // Used for rendering.
        public Vector3 pos;
        public Vector3 nrm;
        public Vector3 tan;
        public Vector3 bit;
        public Vector2 uv;
        public Vector4 col;
        public Vector4 node;
        public Vector4 weight;
        public Vector2 uv2;
        public Vector2 uv3;
        public Vector3 pos1;
        public Vector3 pos2;

        public static int Size = 4 * (3 + 3 + 3 + 3 + 2 + 4 + 4 + 4 + 2 + 2 + 3 + 3);
    }
    public class FSHP : STGenericObject
    {
        public bool IsWiiU
        {
            get
            {
                return GetResFileU() != null;
            }
        }

        public FSHP()
        {
            Checked = true;
            ImageKey = "mesh";
            SelectedImageKey = "mesh";

            ContextMenuStrip = new STContextMenuStrip();

            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export", null, Export, Keys.Control | Keys.E));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Replace", null, Replace, Keys.Control | Keys.R));

            ContextMenuStrip.Items.Add(new ToolStripSeparator());
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Rename", null, Rename, Keys.Control | Keys.N));
            ContextMenuStrip.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem uvMenu = new ToolStripMenuItem("UVs");
            uvMenu.DropDownItems.Add(new ToolStripMenuItem("Flip (Vertical)", null, FlipUvsVertical));
            uvMenu.DropDownItems.Add(new ToolStripMenuItem("Flip (Horizontal)", null, FlipUvsHorizontal));
            uvMenu.DropDownItems.Add(new ToolStripMenuItem("Copy Channel", null, CopyUVChannelAction));
            ContextMenuStrip.Items.Add(uvMenu);

            ToolStripMenuItem normalsMenu = new ToolStripMenuItem("Normals");
            normalsMenu.DropDownItems.Add(new ToolStripMenuItem("Smooth", null, SmoothNormals));
            normalsMenu.DropDownItems.Add(new ToolStripMenuItem("Recalculate", null, RecalculateNormals));
            ContextMenuStrip.Items.Add(normalsMenu);

            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Recalulate Tangents/Bitangents", null, CalcTansBitans, Keys.Control | Keys.T));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Fill Tangent Space with constant", null, FillTangentsAction, Keys.Control | Keys.W));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Fill Bitangent Space with constant", null, FillBitangentsAction, Keys.Control | Keys.B));

            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Open Material Editor", null, OpenMaterialEditor, Keys.Control | Keys.M));

            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Delete", null, Remove, Keys.Control | Keys.Delete));
        }

        public VertexBuffer VertexBuffer;
        public Shape Shape;
        public ResU.VertexBuffer VertexBufferU;
        public ResU.Shape ShapeU;

        public ResFile GetResFile()
        {
            //ResourceFile -> FMDL -> Material Folder -> this
            return ((FMDL)Parent.Parent).GetResFile();
        }
        public ResU.ResFile GetResFileU()
        {
            return ((FMDL)Parent.Parent).GetResFileU();
        }
        public void UpdateVertexData()
        {
            ((FMDL)Parent.Parent).UpdateVertexData();
        }
        public FMDL GetParentModel()
        {
           return ((FMDL)Parent.Parent);
        }

        public FMAT GetMaterial()
        {
            if (Parent == null)
                STErrorDialog.Show($"Error! Shape {Text} has no parent node!", "GetMaterial", "");

            return ((FMDL)Parent.Parent).materials.Values.ElementAt(MaterialIndex);
        }
        public void SetMaterial(FMAT material)
        {
            ((FMDL)Parent.Parent).materials[material.Text] = material;
        }

        public override void OnClick(TreeView treeView)
        {
            UpdateEditor();
        }
        public void UpdateEditor()
        {
            ((BFRES)Parent.Parent.Parent.Parent).LoadEditors(this);
        }
        private void SmoothNormals(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            SmoothNormals();
            SaveVertexBuffer();
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }
        private void RecalculateNormals(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            CalculateNormals();
            SaveVertexBuffer();
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }
        private void FillBitangentsAction(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            FillBitangentSpace(1);
            SaveVertexBuffer();
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }
        private void FillTangentsAction(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            FillTangentSpace(1);
            SaveVertexBuffer();
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }
        private void CopyUVChannelAction(object sender, EventArgs args)
        {
            if (!vertexAttributes.Any(x => x.Name == "_u0"))
                return;

            if (!vertexAttributes.Any(x => x.Name == "_u1"))
            {
                DialogResult dialogResult = MessageBox.Show("This model has no second uv channel to copy to. Create one?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    VertexAttribute att = new VertexAttribute();
                    att.Name = "_u1";
                    att.Format = ResGFX.AttribFormat.Format_16_16_Single;
                    vertexAttributes.Add(att);
                }
            }

            if (GetUVAttributes().Count > 1)
            {
                CopyUVChannelDialog dialog = new CopyUVChannelDialog();
                dialog.LoadUVAttributes();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    CopyUVChannel(dialog.SourceIndex, dialog.DestIndex);
                    SaveVertexBuffer();
                    UpdateVertexData();
                    Cursor.Current = Cursors.Default;
                }
            }
            else
            {
                throw new Exception("Only one UV found. No destination UV can be applied");
            }
        }

        private List<VertexAttribute> GetUVAttributes()
        {
            var uvMaps = new List<VertexAttribute>();

            foreach (var attribute in vertexAttributes)
            {
                var index = string.Concat(attribute.Name.ToArray().Reverse().TakeWhile(char.IsNumber).Reverse());

                if (attribute.Name == $"_u{index}")
                    uvMaps.Add(attribute);
            }

            return uvMaps;
        }

        private void Rename(object sender, EventArgs args)
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Text = dialog.textBox1.Text;
            }
        }
        private void SetVertexColorDialog(object sender, EventArgs args)
        {
            CheckVertexColors();

            if (!vertexAttributes.Any(x => x.Name == "_c0"))
                return;

            ColorDialog dlg = new ColorDialog();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SetVertexColor(new Vector4(
                    dlg.Color.R / 255.0f,
                    dlg.Color.G / 255.0f,
                    dlg.Color.B / 255.0f,
                    dlg.Color.A / 255.0f));


                SaveVertexBuffer();
                UpdateVertexData();
            }
        }
        private void SetVertexColorWhite(object sender, EventArgs args)
        {
            CheckVertexColors();

            if (!vertexAttributes.Any(x => x.Name == "_c0"))
                return;

            SetVertexColor(new Vector4(1,1,1,1));

            SaveVertexBuffer();
            UpdateVertexData();
        }
        private void Remove(object sender, EventArgs args)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove this object? This cannot be undone!", "", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                ((FMDL)Parent.Parent).shapes.Remove(this);
                ((FMDL)Parent.Parent).UpdateVertexData();
                Parent.Nodes.Remove(this);
            }
        }
        public void CheckVertexColors()
        {
            if (!vertexAttributes.Any(x => x.Name == "_c0"))
            {
                DialogResult dialogResult2 = MessageBox.Show($"Mesh {Text} does not have vertex colors. Do you want to create them? (will make file size bigger)", "", MessageBoxButtons.YesNo);

                if (dialogResult2 == DialogResult.Yes)
                {
                    VertexAttribute att = new VertexAttribute();
                    att.Name = "_c0";
                    att.Format = ResGFX.AttribFormat.Format_8_8_8_8_UNorm;
                    vertexAttributes.Add(att);

                }
            }
        }

        public void CopyUVChannel(int SourceIndex, int DestIndex)
        {
            foreach (Vertex v in vertices)
            {
                Vector2 source = new Vector2(0);

                if (SourceIndex == 0)
                    source = v.uv0;
                if (SourceIndex == 1)
                    source = v.uv1;
                if (SourceIndex == 2)
                    source = v.uv2;

                if (DestIndex == 0)
                    v.uv0 = source;
                if (DestIndex == 1)
                    v.uv1 = source;
                if (DestIndex == 2)
                    v.uv2 = source;
            }
        }

        public void FillBitangentSpace(float Value)
        {
            foreach (Vertex v in vertices)
            {
                v.bitan = new Vector4(0.5f);
            }
        }

        public void FillTangentSpace(float Value)
        {
            foreach (Vertex v in vertices)
            {
                v.tan = new Vector4(0.5f);
            }
        }

        public void ApplyImportSettings(BfresModelImportSettings settings, FMAT mat)
        {
            if (settings.FlipUVsVertical)
            {
                foreach (Vertex v in vertices)
                {
                    v.uv0 = new Vector2(v.uv0.X, 1 - v.uv0.Y);
                }
            }
            if (settings.RecalculateNormals)
            {
                CalculateNormals();
            }
            if (settings.Rotate90DegreesY)
            {
                TransformPosition(Vector3.Zero, new Vector3(90, 0, 0), new Vector3(1));
            }
            if (settings.Rotate90DegreesNegativeY)
            {
                TransformPosition(Vector3.Zero, new Vector3(-90, 0, 0), new Vector3(1));
            }
            if (settings.EnableTangents)
            {
                try
                {
                    bool UseUVLayer2 = false;

                    //for BOTW if it uses UV layer 2 for normal maps use second UV map
                    if (Parent != null && GetMaterial().shaderassign.options.ContainsKey("uking_texture2_texcoord"))
                    {
                        float value = float.Parse(GetMaterial().shaderassign.options["uking_texture2_texcoord"]);
                        UseUVLayer2 = (value == 1);
                    }

                    CalculateTangentBitangent(UseUVLayer2);
                }
                catch (Exception ex)
                {
                    STErrorDialog.Show($"Failed to generate tangents for mesh {Text}", "Tangent Calculation", ex.ToString());
                }
            }
            if (settings.SetDefaultParamData)
            {
                foreach (var param in mat.matparam.Values)
                {
                    switch (param.Name)
                    {
                        case "const_color0":
                        case "const_color1":
                        case "const_color2":
                        case "const_color3":
                        case "base_color_mul_color":
                        case "uniform0_mul_color":
                        case "uniform1_mul_color":
                        case "uniform2_mul_color":
                        case "uniform3_mul_color":
                        case "uniform4_mul_color":
                        case "proc_texture_2d_mul_color":
                        case "proc_texture_3d_mul_color":
                        case "displacement1_color":
                        case "ripple_emission_color":
                        case "hack_color":
                        case "stain_color":
                        case "displacement_color":
                            param.ValueFloat = new float[] { 1, 1, 1, 1 };
                            break;
                        case "gsys_bake_st0":
                        case "gsys_bake_st1":
                            param.ValueFloat = new float[] { 1, 1, 0, 0 };
                            break;
                    }
                }
            }
        }
        private void OpenMaterialEditor(object sender, EventArgs args)
        {
            TreeView.SelectedNode = GetMaterial();
        }
        private void CalcTansBitans(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;

            bool HasTans = vertexAttributes.Any(x => x.Name == "_t0");
            bool HasBiTans = vertexAttributes.Any(x => x.Name == "_b0");

            if (!HasUV0())
            {
                MessageBox.Show($"Error! {Text} does not have UVs!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!HasBiTans)
            {
                DialogResult dialogResult2 = MessageBox.Show($"Mesh {Text} does not have bitangents. Do you want to create them? (will make file size bigger)", "", MessageBoxButtons.YesNo);

                VertexAttribute att2 = new VertexAttribute();
                att2.Name = "_b0";
                att2.Format = ResGFX.AttribFormat.Format_10_10_10_2_SNorm;

                if (dialogResult2 == DialogResult.Yes)
                {
                    if (!HasBiTans)
                        vertexAttributes.Add(att2);
                }
            }

            if (!HasTans)
            {
                DialogResult dialogResult = MessageBox.Show($"Mesh {Text} does not have tangets. Do you want to create them? (will make file size bigger)", "", MessageBoxButtons.YesNo);

                VertexAttribute att = new VertexAttribute();
                att.Name = "_t0";
                att.Format = ResGFX.AttribFormat.Format_10_10_10_2_SNorm;


                if (dialogResult == DialogResult.Yes)
                {
                    if (!HasTans)
                        vertexAttributes.Add(att);
                }
            }

            bool UseUVLayer2 = false;

            //for BOTW if it uses UV layer 2 for normal maps use second UV map
            if (GetMaterial().shaderassign.options.ContainsKey("uking_texture2_texcoord"))
            {
                float value = float.Parse(GetMaterial().shaderassign.options["uking_texture2_texcoord"]);

                if (value == 1)
                    UseUVLayer2 = true;
            }

            CalculateTangentBitangent(UseUVLayer2);
            SaveVertexBuffer();
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }
        public bool HasUV0()
        {
            return vertexAttributes.Any(x => x.Name == "_u0");
        }
        public bool HasUV1()
        {
            return vertexAttributes.Any(x => x.Name == "_u1");
        }
        public bool HasUV2()
        {
            return vertexAttributes.Any(x => x.Name == "_u2");
        }
        public void FlipUvsVertical(object sender, EventArgs args)
        {
            if (!HasUV0())
            {
                MessageBox.Show($"Error! {Text} does not have UVs!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FlipUvsVertical();
            SaveVertexBuffer();
            UpdateVertexData();
        }
        public void FlipUvsHorizontal(object sender, EventArgs args)
        {
            if (!HasUV0())
            {
                MessageBox.Show($"Error! {Text} does not have UVs!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FlipUvsHorizontal();
            SaveVertexBuffer();
            UpdateVertexData();
        }
        public void ExportMaterials(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Materials|*.bfmat;";
            sfd.DefaultExt = ".bfmat";
            sfd.FileName = GetMaterial().Text;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                GetMaterial().Material.Export(sfd.FileName, GetResFile());
            }
        }
        public void ReplaceMaterials(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Materials|*.bfmat;";
            ofd.DefaultExt = ".bfmat";
            ofd.FileName = GetMaterial().Text;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                GetMaterial().Material.Import(ofd.FileName);
            }
        }
        public void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.bfobj;*.fbx;*.dae; *.obj;|" +
             "Bfres Object (shape/vertices) |*.bfobj|" +
             "FBX |*.fbx|" +
             "DAE |*.dae|" +
             "OBJ |*.obj|" +
             "All files(*.*)|*.*";
            sfd.DefaultExt = ".bfobj";
            sfd.FileName = Text;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(sfd.FileName);
                ext = ext.ToLower();

                switch (ext)
                {
                    case ".bfobj":
                        ExportBinaryObject(sfd.FileName);
                        break;
                    default:
                        AssimpSaver assimp = new AssimpSaver();
                        assimp.SaveFromObject(vertices, lodMeshes[DisplayLODIndex].faces, Text, sfd.FileName);
                        break;
                }
            }
        }
        public void ExportBinaryObject(string FileName)
        {
            if (IsWiiU)
                ShapeU.Export(FileName,VertexBufferU, GetResFileU());
            else
                Shape.Export(FileName, GetResFile());
        }
        public void Replace(object sender, EventArgs args)
        {
            bool IsWiiU = (GetResFileU() != null);

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfobj;*.fbx;*.dae; *.obj;|" +
             "Bfres Object (shape/vertices) |*.bfobj|" +
             "FBX |*.fbx|" +
             "DAE |*.dae|" +
             "OBJ |*.obj|" +
             "All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(ofd.FileName);
                ext = ext.ToLower();

                switch (ext)
                {
                    case ".bfobj":
                        if (IsWiiU)
                        {
                            var shp = new ResU.Shape();
                            shp.Import(ofd.FileName, VertexBufferU, GetResFileU());
                            shp.Name = Text;
                            shp.MaterialIndex = (ushort)MaterialIndex;
                            BfresWiiU.ReadShapesVertices(this, shp, VertexBufferU, GetParentModel());
                        }
                        else
                        {
                            Shape shp = new Shape();
                            shp.Import(ofd.FileName, VertexBuffer);
                            shp.Name = Text;
                            shp.MaterialIndex = (ushort)MaterialIndex;
                            BfresSwitch.ReadShapesVertices(this, shp, VertexBuffer, GetParentModel());
                        }
       
                        break;
                    default:
                        AssimpData assimp = new AssimpData();
                        assimp.LoadFile(ofd.FileName);
                        AssimpMeshSelector selector = new AssimpMeshSelector();
                        selector.LoadMeshes(assimp, Index);

                        if (selector.ShowDialog() == DialogResult.OK)
                        {
                            if (assimp.objects.Count == 0)
                            {
                                MessageBox.Show("No models found!");
                                return;
                            }
                            BfresModelImportSettings settings = new BfresModelImportSettings();
                            settings.SetModelAttributes(assimp.objects[0]);
                            if (settings.ShowDialog() == DialogResult.OK)
                            {
                                STGenericObject obj = selector.GetSelectedMesh();

                                Cursor.Current = Cursors.WaitCursor;
                                VertexBufferIndex = obj.VertexBufferIndex;
                                vertices = obj.vertices;
                                CreateBoneList(obj, (FMDL)Parent.Parent);
                                CreateIndexList(obj, (FMDL)Parent.Parent);
                                VertexSkinCount = obj.GetMaxSkinInfluenceCount();
                                vertexAttributes = settings.CreateNewAttributes();
                                ApplyImportSettings(settings, GetMaterial());
                                lodMeshes = obj.lodMeshes;
                                CreateNewBoundingBoxes();

                                SaveShape(IsWiiU);
                                SaveVertexBuffer();

                                Cursor.Current = Cursors.Default;
                            }
                        }
                        break;
                }
                UpdateVertexData();
            }
        }
        public void CreateIndexList(STGenericObject ob, FMDL mdl = null)
        {
            BoneIndices = new List<ushort>();

            List<string> boneNames = new List<string>();
            foreach (Vertex v in ob.vertices)
            {
                foreach (string bn in v.boneNames)
                {
                    if (!boneNames.Contains(bn))
                    {
                        boneNames.Add(bn);
                    }
                }
            }

            foreach (STBone bone in mdl.Skeleton.bones)
            {
                foreach (string bnam in boneNames)
                {
                    if (bone.Text == bnam)
                    {
                        int index = boneNames.IndexOf(bone.Text);
                        STConsole.WriteLine($"Adding bone to shape index list! {bone.Text} {index}");

                        BoneIndices.Add((ushort)index);
                    }
                }
            }
        }
        public void CreateBoneList(STGenericObject ob, FMDL mdl)
        {
            if (mdl.Skeleton.Node_Array == null)
                mdl.Skeleton.Node_Array = new int[0];

            string[] nodeArrStrings = new string[mdl.Skeleton.Node_Array.Length];
            short[] nodeRigidIndex = new short[mdl.Skeleton.Node_Array.Length];
                
            int CurNode = 0;
            foreach (int thing in mdl.Skeleton.Node_Array)
            {
                nodeArrStrings[CurNode] = mdl.Skeleton.bones[thing].Text;
                nodeRigidIndex[CurNode] = mdl.Skeleton.bones[thing].RigidMatrixIndex;
                CurNode++;
            }


            foreach (Vertex v in ob.vertices)
            {
                List<int> RigidIds = new List<int>();
                foreach (string bn in v.boneNames)
                {
                    int i = 0;
                    foreach (var defBn in nodeArrStrings.Select((Value, Index) => new { Value, Index }))
                    {
                        if (bn == defBn.Value)
                        {
                            //Add these after smooth matrices
                            if (nodeRigidIndex[i] != -1)
                            {
                                RigidIds.Add(defBn.Index);
                            }
                            else
                            {
                                if (v.boneIds.Count < 4)
                                {
                                    STConsole.WriteLine(defBn.Index + " mesh " + Text + " bone " + bn);
                                    v.boneIds.Add(defBn.Index);
                                }
                            }
                        }
                        i++;
                    }
                }
                if (RigidIds.Count > 0)
                {
                    foreach (int id in RigidIds)
                    {
                        if (v.boneIds.Count < 4)
                            v.boneIds.Add(id);
                    }
                }
            }
        }
        public void CreateNewBoundingBoxes()
        {
            boundingBoxes.Clear();
            boundingRadius.Clear();
            foreach (LOD_Mesh mesh in lodMeshes)
            {
                BoundingBox box = CalculateBoundingBox();
                boundingBoxes.Add(box);
                boundingRadius.Add((float)(box.Center.Length + box.Extend.Length));
                foreach (LOD_Mesh.SubMesh sub in mesh.subMeshes)
                    boundingBoxes.Add(box);
            }
        }
        private BoundingBox CalculateBoundingBox()
        {
            Vector3 Max = new Vector3();
            Vector3 Min = new Vector3();

            Min = Max = vertices[0].pos;

            Min = CalculateBBMin(vertices);
            Max = CalculateBBMax(vertices);
            Vector3 center = (Max + Min);
            Vector3 extend = Max - Min;

            return new BoundingBox() { Center = center, Extend = extend };
        }
        private Vector3 CalculateBBMin(List<Vertex> positionVectors)
        {
            Vector3 minimum = new Vector3();
            foreach (Vertex vtx in positionVectors)
            {
                if (vtx.pos.X < minimum.X) minimum.X = vtx.pos.X;
                if (vtx.pos.Y < minimum.Y) minimum.Y = vtx.pos.Y;
                if (vtx.pos.Z < minimum.Z) minimum.Z = vtx.pos.Z;
            }

            return minimum;
        }

        private Vector3 CalculateBBMax(List<Vertex> positionVectors)
        {
            Vector3 maximum = new Vector3();
            foreach (Vertex vtx in positionVectors)
            {
                if (vtx.pos.X > maximum.X) maximum.X = vtx.pos.X;
                if (vtx.pos.Y > maximum.Y) maximum.Y = vtx.pos.Y;
                if (vtx.pos.Z > maximum.Z) maximum.Z = vtx.pos.Z;
            }

            return maximum;
        }

        private void UpdateShaderAssignAttributes(FMAT material)
        {
            material.shaderassign.attributes.Clear();
            foreach (VertexAttribute att in vertexAttributes)
            {
                material.shaderassign.attributes.Add(att.Name, att.Name);
            }
        }

        public float sortingDistance = 0;

        public float CalculateSortingDistance(Vector3 cameraPosition)
        {
            BoundingBox box = new BoundingBox();

            Vector3 distanceVector = new Vector3(cameraPosition - box.Center);
            return distanceVector.Length + boundingRadius[0];
        }

        public int[] Faces;
        public List<ushort> BoneIndices = new List<ushort>();

        // for drawing
        public int[] display;
         
        public int DisplayId;
        public int VertexBufferIndex;
        public int TargetAttribCount;

        public List<float> boundingRadius = new List<float>();
        public List<BoundingBox> boundingBoxes = new List<BoundingBox>();
        public class BoundingBox
        {
            public Vector3 Center;
            public Vector3 Extend;
        }
        public int DisplayLODIndex = 0;

        public List<VertexAttribute> vertexAttributes = new List<VertexAttribute>();
        public class VertexAttribute
        {
            public string Name;
            public ResGFX.AttribFormat Format;

            public override string ToString()
            {
                return Name;
            }

            public ResGFX.AttribFormat GetTypeWiiU(ResUGX2.GX2AttribFormat type)
            {
                return (ResGFX.AttribFormat)System.Enum.Parse(typeof(ResGFX.AttribFormat), $"{type.ToString()}");
            }
            public ResUGX2.GX2AttribFormat SetTypeWiiU(ResGFX.AttribFormat type)
            {
                return (ResUGX2.GX2AttribFormat)System.Enum.Parse(typeof(ResUGX2.GX2AttribFormat), type.ToString());
            }
        }
        public void SaveShape(bool IsWiiU)
        {
            if (IsWiiU)
                ShapeU = BfresWiiU.SaveShape(this);
            else
                Shape = BfresSwitch.SaveShape(this);
        }
        public List<ushort> GetIndices(FSKL fskl)
        {
            List<ushort> indices = new List<ushort>();

            List<string> BoneNodes = new List<string>();
            foreach (Vertex vtx in vertices)
            {
                foreach (int index in vtx.boneIds)
                {
                    var bone = fskl.bones[fskl.Node_Array[index]];

                    ushort ind = (ushort)fskl.bones.IndexOf(bone);
                    if (!indices.Contains(ind))
                    {
                        STConsole.WriteLine($"Saving bone index {bone.Name} {index}");
                        indices.Add(ind);
                    }

                }
            }
            STConsole.WriteLine($"Total Indices for {Text}  {indices.Count}");

            return indices;
        }
        public Vector3 TransformLocal(Vector3 position, int BoneIndex,bool IsSingleBind,  bool IsPos = true)
        {
            try
            {
                var skel = GetParentModel().Skeleton;
                Matrix4 trans = Matrix4.Identity;
                if (IsSingleBind)
                {
                    if (BoneIndex >= skel.Node_Array.Length || BoneIndex == -1)
                    {
                        STConsole.WriteLine($"Invalid bone index to bind bone to mesh {Text} {BoneIndex} ", System.Drawing.Color.Red);
                        return position;
                    }

                    var bone = skel.bones[skel.Node_Array[BoneIndex]];

                    if (trans.Determinant != 0)
                        trans = bone.invert;
                    else
                        STConsole.WriteLine($"Determinant for bone transform is 0 to bind bone to mesh {Text} {BoneIndex} ", System.Drawing.Color.Red);
                }
                else
                {
                    var bone = skel.bones[BoneIndex];

                    if (trans.Determinant != 0)
                        trans = bone.invert;
                    else
                        STConsole.WriteLine($"Determinant for bone transform is 0 to bind bone to mesh {Text} {BoneIndex} ", System.Drawing.Color.Red);
                }

                if (IsPos)
                    return Vector3.TransformPosition(position, trans);
                else
                    return Vector3.TransformNormal(position, trans);
            }
            catch
            {
                STConsole.WriteLine("Failed to bind bone to mesh " + Text, System.Drawing.Color.Red);
                return position;
            }
        }
        public static Vector3 transform(Vector3 input, Matrix4 matrix)
        {   
            Vector3 output = new Vector3();
            output.X = input.X * matrix.M11 + input.Y * matrix.M21 + input.Z * matrix.M31 + matrix.M41;
            output.Y = input.X * matrix.M12 + input.Y * matrix.M22 + input.Z * matrix.M32 + matrix.M42;
            output.Z = input.X * matrix.M13 + input.Y * matrix.M23 + input.Z * matrix.M33 + matrix.M43;
            return output;      
        }
        public void SaveVertexBuffer()
        {
            if (IsWiiU)
            {
                BfresWiiU.SaveVertexBuffer(this);
                BfresWiiU.ReadShapesVertices(this, ShapeU, VertexBufferU, GetParentModel());
                return;
            }

            VertexBufferHelper helpernx = new VertexBufferHelper(new VertexBuffer(), Syroot.BinaryData.ByteOrder.LittleEndian);
            List<VertexBufferHelperAttrib> atrib = new List<VertexBufferHelperAttrib>();

            UpdateVertices();


            foreach (VertexAttribute att in vertexAttributes)
            {
                if (att.Name == "_p0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = verts.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_n0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = norms.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_u0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = uv0.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_u1")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = uv1.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_u2")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = uv2.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_w0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = weights.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_i0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = boneInd.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_b0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = bitans.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_t0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = tans.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
                if (att.Name == "_c0")
                {
                    VertexBufferHelperAttrib vert = new VertexBufferHelperAttrib();
                    vert.Name = att.Name;
                    vert.Data = colors.ToArray();
                    vert.Format = att.Format;
                    atrib.Add(vert);
                }
            }
            if (atrib.Count == 0)
            {
                MessageBox.Show("Attributes are empty?");
                return;
            }
            helpernx.Attributes = atrib;
            VertexBuffer = helpernx.ToVertexBuffer();
            VertexBuffer.VertexSkinCount = VertexSkinCount;

            BfresSwitch.ReadShapesVertices(this, Shape, VertexBuffer, GetParentModel());
        }

        internal List<Syroot.Maths.Vector4F> verts = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> norms = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> uv0 = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> uv1 = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> uv2 = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> tans = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> bitans = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> weights = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> boneInd = new List<Syroot.Maths.Vector4F>();
        internal List<Syroot.Maths.Vector4F> colors = new List<Syroot.Maths.Vector4F>();

        public string GetBoneNameFromIndex(FMDL mdl, int index)
        {
            if (index == 0)
                return "";

            return mdl.Skeleton.bones[mdl.Skeleton.Node_Array[index]].Text;
        }

        public void UpdateVertices()
        {
            verts.Clear();
            norms.Clear();
            uv0.Clear();
            uv1.Clear();
            uv2.Clear();
            tans.Clear();
            bitans.Clear();
            colors.Clear();
            weights.Clear();
            boneInd.Clear();

            foreach (Vertex vtx in vertices)
            {
                if (VertexSkinCount == 0 || VertexSkinCount == 1)
                {
                    int boneId = BoneIndex;
                    if (VertexSkinCount == 1 && vtx.boneIds.Count > 0)
                        boneId = vtx.boneIds[0];

                    vtx.pos = TransformLocal(vtx.pos, boneId, VertexSkinCount == 1);
                    vtx.nrm = TransformLocal(vtx.nrm, boneId, VertexSkinCount == 1, false);
                }

                verts.Add(new Syroot.Maths.Vector4F(vtx.pos.X, vtx.pos.Y, vtx.pos.Z, 1.0f));
                norms.Add(new Syroot.Maths.Vector4F(vtx.nrm.X, vtx.nrm.Y, vtx.nrm.Z, 0));
                uv0.Add(new Syroot.Maths.Vector4F(vtx.uv0.X, vtx.uv0.Y, 0, 0));
                uv1.Add(new Syroot.Maths.Vector4F(vtx.uv1.X, vtx.uv1.Y, 0, 0));
                uv2.Add(new Syroot.Maths.Vector4F(vtx.uv2.X, vtx.uv2.Y, 0, 0));
                tans.Add(new Syroot.Maths.Vector4F(vtx.tan.X, vtx.tan.Y, vtx.tan.Z, vtx.tan.W));
                bitans.Add(new Syroot.Maths.Vector4F(vtx.bitan.X, vtx.bitan.Y, vtx.bitan.Z, vtx.bitan.W));
                colors.Add(new Syroot.Maths.Vector4F(vtx.col.X, vtx.col.Y, vtx.col.Z, vtx.col.W));

                float[] weightsA = new float[4];
                int[] indicesA = new int[4];

                if (vtx.boneWeights.Count >= 1)
                    weightsA[0] = vtx.boneWeights[0];
                if (vtx.boneWeights.Count >= 2)
                    weightsA[1] = vtx.boneWeights[1];
                if (vtx.boneWeights.Count >= 3)
                    weightsA[2] = vtx.boneWeights[2];
                if (vtx.boneWeights.Count >= 4)
                    weightsA[3] = vtx.boneWeights[3];

                if (vtx.boneIds.Count >= 1)
                    indicesA[0] = vtx.boneIds[0];
                if (vtx.boneIds.Count >= 2)
                    indicesA[1] = vtx.boneIds[1];
                if (vtx.boneIds.Count >= 3)
                    indicesA[2] = vtx.boneIds[2];
                if (vtx.boneIds.Count >= 4)
                    indicesA[3] = vtx.boneIds[3];

                weights.Add(new Syroot.Maths.Vector4F(weightsA[0], weightsA[1], weightsA[2], weightsA[3]));
                boneInd.Add(new Syroot.Maths.Vector4F(indicesA[0], indicesA[1], indicesA[2], indicesA[3]));
            }
        }

        public List<DisplayVertex> CreateDisplayVertices()
        {
            // rearrange faces
            display = lodMeshes[DisplayLODIndex].getDisplayFace().ToArray();

            List<DisplayVertex> displayVertList = new List<DisplayVertex>();

            if (lodMeshes[DisplayLODIndex].faces.Count <= 3)
                return displayVertList;

            foreach (Vertex v in vertices)
            {
                DisplayVertex displayVert = new DisplayVertex()
                {
                    pos = v.pos,
                    nrm = v.nrm,
                    tan = v.tan.Xyz,
                    bit = v.bitan.Xyz,
                    col = v.col,
                    uv = v.uv0,
                    uv2 = v.uv1,
                    uv3 = v.uv2,
                    node = new Vector4(
                         v.boneIds.Count > 0 ? v.boneIds[0] : -1,
                         v.boneIds.Count > 1 ? v.boneIds[1] : -1,
                         v.boneIds.Count > 2 ? v.boneIds[2] : -1,
                         v.boneIds.Count > 3 ? v.boneIds[3] : -1),
                    weight = new Vector4(
                         v.boneWeights.Count > 0 ? v.boneWeights[0] : 0,
                         v.boneWeights.Count > 1 ? v.boneWeights[1] : 0,
                         v.boneWeights.Count > 2 ? v.boneWeights[2] : 0,
                         v.boneWeights.Count > 3 ? v.boneWeights[3] : 0),
                };

                displayVertList.Add(displayVert);


                /*   Console.WriteLine($"---------------------------------------------------------------------------------------");
                   Console.WriteLine($"Position   {displayVert.pos.X} {displayVert.pos.Y} {displayVert.pos.Z}");
                   Console.WriteLine($"Normal     {displayVert.nrm.X} {displayVert.nrm.Y} {displayVert.nrm.Z}");
                   Console.WriteLine($"Binormal   {displayVert.bit.X} {displayVert.bit.Y} {displayVert.bit.Z}");
                   Console.WriteLine($"Tanget     {displayVert.tan.X} {displayVert.tan.Y} {displayVert.tan.Z}");
                   Console.WriteLine($"Color      {displayVert.col.X} {displayVert.col.Y} {displayVert.col.Z} {displayVert.col.W}");
                   Console.WriteLine($"UV Layer 1 {displayVert.uv.X} {displayVert.uv.Y}");
                   Console.WriteLine($"UV Layer 2 {displayVert.uv2.X} {displayVert.uv2.Y}");
                   Console.WriteLine($"UV Layer 3 {displayVert.uv3.X} {displayVert.uv3.Y}");
                   Console.WriteLine($"Bone Index {displayVert.node.X} {displayVert.node.Y} {displayVert.node.Z} {displayVert.node.W}");
                   Console.WriteLine($"Weights    {displayVert.weight.X} {displayVert.weight.Y} {displayVert.weight.Z} {displayVert.weight.W}");
                   Console.WriteLine($"---------------------------------------------------------------------------------------");*/
            }

            return displayVertList;
        }
    }
}
