using System;
using System.Collections.Generic;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres;
using Syroot.NintenTools.NSW.Bfres.Helpers;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Rendering;
using Toolbox.Library.Forms;
using ResU = Syroot.NintenTools.Bfres;
using ResUGX2 = Syroot.NintenTools.Bfres.GX2;
using ResGFX = Syroot.NintenTools.NSW.Bfres.GFX;
using FirstPlugin;
using FirstPlugin.Forms;
using OpenTK;

namespace Bfres.Structs
{
    public class FSHPFolder : TreeNodeCustom, IContextMenuNode
    {
        public FSHPFolder()
        {
            Text = "Objects";
            Name = "FshpFolder";
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Import Object", null, Import, Keys.Control | Keys.I));
            Items.Add(new ToolStripSeparator());
            Items.Add(new ToolStripMenuItem("New Empty Object", null, CreateEmpty, Keys.Control | Keys.N));
            Items.Add(new ToolStripSeparator());
            Items.Add(new ToolStripMenuItem("Export All Objects", null, ExportAll, Keys.Control | Keys.A));
            Items.Add(new ToolStripMenuItem("Clear All Objects", null, Clear, Keys.Control | Keys.C));
            return Items.ToArray();
        }

        private void CreateEmpty(object sender, EventArgs args)
        {
            FMDL fmdl = (FMDL)Parent;
            FSHP mesh = new FSHP();
            mesh.Text = "EmptyShape";
            CreateEmptyMesh((FMDL)Parent, mesh);
        }

        public static void CreateEmptyMesh(FMDL fmdl, FSHP mesh)
        {
            List<string> ShapeKeys = fmdl.shapes.Select(i => i.Text).ToList();
            mesh.Text = Utils.RenameDuplicateString(ShapeKeys, mesh.Text);

            if (fmdl.GetResFileU() != null)
            {
                var Shape = new ResU.Shape();
                Shape.CreateEmptyMesh();
                Shape.Name = mesh.Text;

                var VertexBuffer = new ResU.VertexBuffer();
                VertexBuffer.CreateEmptyVertexBuffer();

                fmdl.ModelU.VertexBuffers.Add(VertexBuffer);
                fmdl.ModelU.Shapes.Add(Shape.Name, Shape);

                BfresWiiU.ReadShapesVertices(mesh, Shape, VertexBuffer, fmdl);
                mesh.MaterialIndex = 0;

                fmdl.Nodes["FshpFolder"].Nodes.Add(mesh);
                fmdl.shapes.Add(mesh);
            }
            else
            {
                var Shape = new Shape();
                Shape.CreateEmptyMesh();
                Shape.Name = mesh.Text;

                var VertexBuffer = new VertexBuffer();
                VertexBuffer.CreateEmptyVertexBuffer();

                fmdl.Model.VertexBuffers.Add(VertexBuffer);
                fmdl.Model.Shapes.Add(Shape);
                fmdl.Model.ShapeDict.Add(mesh.Text);

                BfresSwitch.ReadShapesVertices(mesh, Shape, VertexBuffer, fmdl);
                mesh.MaterialIndex = 0;

                fmdl.Nodes["FshpFolder"].Nodes.Add(mesh);
                fmdl.shapes.Add(mesh);
            }
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
    public class FSHP : STGenericObject, IContextMenuNode
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
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();

            Items.Add(new ToolStripMenuItem("Export", null, Export, Keys.Control | Keys.E));
            Items.Add(new ToolStripMenuItem("Replace", null, Replace, Keys.Control | Keys.R));
            Items.Add(new ToolStripSeparator());
            Items.Add(new ToolStripMenuItem("Rename", null, Rename, Keys.Control | Keys.N));
            Items.Add(new ToolStripSeparator());

            ToolStripMenuItem lodMenu = new ToolStripMenuItem("Level Of Detail");
            lodMenu.DropDownItems.Add(new ToolStripMenuItem("Clear LOD Meshes", null, ClearLODMeshes));
            lodMenu.DropDownItems.Add(new ToolStripMenuItem("Add dummy LOD Meshes", null, GenerateDummyLODMeshesAction));
            Items.Add(lodMenu);

            ToolStripMenuItem boundingsMenu = new ToolStripMenuItem("Boundings");
            boundingsMenu.DropDownItems.Add(new ToolStripMenuItem("Regenerate Bounding Boxes/Radius", null, GenerateBoundingBoxes));
            Items.Add(boundingsMenu);

            ToolStripMenuItem uvMenu = new ToolStripMenuItem("UVs");
            uvMenu.DropDownItems.Add(new ToolStripMenuItem("Flip (Vertical)", null, FlipUvsVertical));
            uvMenu.DropDownItems.Add(new ToolStripMenuItem("Flip (Horizontal)", null, FlipUvsHorizontal));
            uvMenu.DropDownItems.Add(new ToolStripMenuItem("Copy Channel", null, CopyUVChannelAction));
            //  uvMenu.DropDownItems.Add(new ToolStripMenuItem("Unwrap By Position", null, UVUnwrapPosition));

            Items.Add(uvMenu);

            ToolStripMenuItem normalsMenu = new ToolStripMenuItem("Normals");
            normalsMenu.DropDownItems.Add(new ToolStripMenuItem("Smooth (Multiple Meshes)", null, MultiMeshSmoothNormals));
            normalsMenu.DropDownItems.Add(new ToolStripMenuItem("Smooth", null, SmoothNormals));
            normalsMenu.DropDownItems.Add(new ToolStripMenuItem("Invert", null, InvertNormals));
            normalsMenu.DropDownItems.Add(new ToolStripMenuItem("Recalculate", null, RecalculateNormals));
            Items.Add(normalsMenu);
			
            ToolStripMenuItem colorMenu = new ToolStripMenuItem("Colors");
            colorMenu.DropDownItems.Add(new ToolStripMenuItem("Set Color", null, SetVertexColorDialog));
            colorMenu.DropDownItems.Add(new ToolStripMenuItem("Set As White", null, SetVertexColorWhite));
            Items.Add(colorMenu);

            Items.Add(new ToolStripMenuItem("Recalulate Tangents/Bitangents", null, CalcTansBitans, Keys.Control | Keys.T));
            Items.Add(new ToolStripMenuItem("Fill Tangent Space with constant", null, FillTangentsAction, Keys.Control | Keys.W));
            Items.Add(new ToolStripMenuItem("Fill Bitangent Space with constant", null, FillBitangentsAction, Keys.Control | Keys.B));

            Items.Add(new ToolStripMenuItem("Open Material Editor", null, OpenMaterialEditor, Keys.Control | Keys.M));

            Items.Add(new ToolStripMenuItem("Delete", null, Remove, Keys.Control | Keys.Delete));
            return Items.ToArray();
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
        public override void UpdateVertexData()
        {
            ((FMDL)Parent.Parent).UpdateVertexData();
        }
        public FMDL GetParentModel()
        {
           return ((FMDL)Parent.Parent);
        }

        public FMAT GetFMAT()
        {
            return (FMAT)GetMaterial();
        }

        public override STGenericMaterial GetMaterial()
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

        public void TransformBindedBone(string BoneName)
        {
            if (Parent == null || VertexSkinCount > 1) return;

            //Check if bone index obtains the right bone
            if (VertexSkinCount == 0)
            {
                if (GetParentModel().Skeleton.bones[BoneIndex].Text != BoneName)
                    return;
            }

            TransformBindedBone();
        }

        //Transforms vertices given the bone
        //Used when a bone is updated in the editor
        public void TransformBindedBone()
        {
            if (Parent == null || VertexSkinCount > 1) return;

            FMDL model = GetParentModel();

            Matrix4 SingleBind = model.Skeleton.bones[BoneIndex].Transform;

            for (int v = 0; v < vertices.Count; v++)
            {
                if (VertexSkinCount == 0)
                {
                    vertices[v].pos = Vector3.TransformPosition(vertices[v].pos, SingleBind);
                    vertices[v].nrm = Vector3.TransformNormal(vertices[v].nrm, SingleBind);
                }
                else if (VertexSkinCount == 1)
                {
                    int boneIndex = BoneIndex;
                    if (vertices[v].boneIds.Count > 0)
                        boneIndex = model.Skeleton.Node_Array[vertices[v].boneIds[0]];

                    Matrix4 SingleBindLocal = model.Skeleton.bones[boneIndex].Transform;

                    vertices[v].pos = Vector3.TransformPosition(vertices[v].pos, SingleBindLocal);
                    vertices[v].nrm = Vector3.TransformNormal(vertices[v].nrm, SingleBindLocal);
                }
            }

            SaveVertexBuffer(GetResFileU() != null);
            UpdateVertexData();
        }

        private void MultiMeshSmoothNormals(object sender, EventArgs args)
        {
            SmoothNormalsMultiMeshForm form = new SmoothNormalsMultiMeshForm();
            form.LoadMeshes(GetParentModel().GetModelList());
            if (form.ShowDialog() == DialogResult.OK)
            {
                var SelectedMeshes = form.GetSelectedMeshes();

                Cursor.Current = Cursors.WaitCursor;
                SmoothNormals(SelectedMeshes);
                Cursor.Current = Cursors.Default;

                SaveVertexBuffer(GetResFileU() != null);
                UpdateVertexData();
            }
        }

        private void UVUnwrapPosition(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            UVUnwrapPosition();
            SaveVertexBuffer(GetResFileU() != null);
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }

        private void SmoothNormals(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            SmoothNormals();
            SaveVertexBuffer(GetResFileU() != null);
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }

        private void InvertNormals(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            InvertNormals();
            SaveVertexBuffer(GetResFileU() != null);
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }

        private void GenerateBoundingBoxes(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            CreateNewBoundingBoxes(GetParentModel().Skeleton);
            SaveShape(GetResFileU() != null);
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }

        private void GenerateDummyLODMeshesAction(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            GenerateDummyLODMeshes();
            SaveShape(GetResFileU() != null);
            UpdateVertexData();
            GenerateBoundingNodes();
            Cursor.Current = Cursors.Default;
        }

        public void GenerateDummyLODMeshes(int count = 2)
        {
            var mesh = lodMeshes.FirstOrDefault();
            while (true)
            {
                if (lodMeshes.Count >= count + 1)
                    break;

                LOD_Mesh lod = new LOD_Mesh();
                lod.faces.AddRange(mesh.faces);
                lod.IndexFormat = mesh.IndexFormat;
                lodMeshes.Add(lod);

                var subMesh = new LOD_Mesh.SubMesh();
                subMesh.offset = mesh.subMeshes[0].offset;
                subMesh.size = mesh.subMeshes[0].size;
                lod.subMeshes.Add(subMesh);
            }

            CreateNewBoundingBoxes(GetParentModel().Skeleton);
        }

        private void GenerateLODMeshes(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;

            //Todo add lod generating

            CreateNewBoundingBoxes(GetParentModel().Skeleton);
            SaveShape(GetResFileU() != null);
            UpdateVertexData();
            GenerateBoundingNodes();

            Cursor.Current = Cursors.Default;
        }

        private void ClearLODMeshes(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;

            DisplayLODIndex = 0; //Force index to prevent errors

            //Clear all but first base mesh
            var meshes = lodMeshes.ToList();
            for (int i = 0; i < meshes.Count; i++)
            {
                if (i != 0)
                    lodMeshes.Remove(meshes[i]);
            }

            CreateNewBoundingBoxes(GetParentModel().Skeleton);
            SaveShape(GetResFileU() != null);
            UpdateVertexData();
            GenerateBoundingNodes();

            Cursor.Current = Cursors.Default;
        }

        private void GenerateBoundingNodes()
        {
            if (ShapeU != null)
            {
                ShapeU.SubMeshBoundingIndices = new List<ushort>();
                ShapeU.SubMeshBoundingIndices.Add(0);
                ShapeU.SubMeshBoundingNodes = new List<ResU.BoundingNode>();
                ShapeU.SubMeshBoundingNodes.Add(new ResU.BoundingNode()
                {
                    LeftChildIndex = 0,
                    NextSibling = 0,
                    SubMeshIndex = 0,
                    RightChildIndex = 0,
                    Unknown = 0,
                    SubMeshCount = 1,
                });
            }
        }

        private void RecalculateNormals(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            CalculateNormals();
            SaveVertexBuffer(GetResFileU() != null);
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }

        private void FillBitangentsAction(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            FillBitangentSpace(1);
            SaveVertexBuffer(GetResFileU() != null);
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }

        private void FillTangentsAction(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            FillTangentSpace(1);
            SaveVertexBuffer(GetResFileU() != null);
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
                    SaveVertexBuffer(GetResFileU() != null);
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

        public VertexAttribute GetWeightAttribute(int index)
        {
            foreach (var attribute in vertexAttributes)
            {
                if (attribute.Name == $"_w{index}")
                    return attribute;
            }

            return null;
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


                SaveVertexBuffer(GetResFileU() != null);
                UpdateVertexData();
            }
        }
        private void SetVertexColorWhite(object sender, EventArgs args)
        {
            CheckVertexColors();

            if (!vertexAttributes.Any(x => x.Name == "_c0"))
                return;

            SetVertexColor(new Vector4(1,1,1,1));

            SaveVertexBuffer(GetResFileU() != null);
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
                    v.uv1 = new Vector2(v.uv1.X, 1 - v.uv1.Y);
                    v.uv2 = new Vector2(v.uv2.X, 1 - v.uv2.Y);
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

                    //check second UV layer
                    if (Parent != null) {
                        UseUVLayer2 = GetFMAT().IsNormalMapTexCoord2();
                    }

                    CalculateTangentBitangent(UseUVLayer2);
                }
                catch (Exception ex)
                {
                    STErrorDialog.Show($"Failed to generate tangents for mesh {Text}", "Tangent Calculation", ex.ToString());
                }
            }

            if (settings.ResetColorParams)
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

            if (settings.ResetUVParams)
            {
                foreach (var param in mat.matparam.Values)
                {
                    switch (param.Name)
                    {
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

            if (!HasAttributeUV0())
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
            if (GetFMAT().shaderassign.options.ContainsKey("uking_texture2_texcoord"))
            {
                float value = float.Parse(GetFMAT().shaderassign.options["uking_texture2_texcoord"]);

                if (value == 1)
                    UseUVLayer2 = true;
            }

            CalculateTangentBitangent(UseUVLayer2);
            SaveVertexBuffer(GetResFileU() != null);
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }
        public bool HasAttributeUV0()
        {
            return vertexAttributes.Any(x => x.Name == "_u0");
        }
        public bool HasAttributeUV1()
        {
            return vertexAttributes.Any(x => x.Name == "_u1");
        }
        public bool HasAttributeUV2()
        {
            return vertexAttributes.Any(x => x.Name == "_u2");
        }
        public void FlipUvsVertical(object sender, EventArgs args)
        {
            if (!HasAttributeUV0())
            {
                MessageBox.Show($"Error! {Text} does not have UVs!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FlipUvsVertical();
            SaveVertexBuffer(GetResFileU() != null);
            UpdateVertexData();
        }
        public void FlipUvsHorizontal(object sender, EventArgs args)
        {
            if (!HasAttributeUV0())
            {
                MessageBox.Show($"Error! {Text} does not have UVs!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FlipUvsHorizontal();
            SaveVertexBuffer(GetResFileU() != null);
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
                GetFMAT().Material.Export(sfd.FileName, GetResFile());
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
                GetFMAT().Material.Import(ofd.FileName);
            }
        }
        public void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = FileFilters.FSHP; 
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
                    case ".obj":
                        OBJ.ExportMesh(sfd.FileName, this);
                        break;
                    default:
                        DAE.Export(sfd.FileName, new DAE.ExportSettings(), this);
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

                            byte ForceSkinInfluenceMax = VertexSkinCount;

                            var originalAttributes = vertexAttributes;

                            BfresModelImportSettings settings = new BfresModelImportSettings();
                            settings.SetModelAttributes(assimp.objects);
                            if (settings.ShowDialog() == DialogResult.OK)
                            {
                                STGenericObject obj = selector.GetSelectedMesh();

                                Cursor.Current = Cursors.WaitCursor;
                                VertexBufferIndex = obj.VertexBufferIndex;
                                vertices = obj.vertices;

                                if (settings.UseOriginalAttributeFormats)
                                    vertexAttributes = originalAttributes;
                                else
                                    vertexAttributes = settings.CreateNewAttributes();

                                if (settings.LimitSkinCount)
                                    obj.VertexSkinCount = ForceSkinInfluenceMax;
                                else
                                    VertexSkinCount = obj.GetMaxSkinInfluenceCount();

                                lodMeshes = obj.lodMeshes;
                                CreateBoneList(obj, (FMDL)Parent.Parent, settings.LimitSkinCount, ForceSkinInfluenceMax);
                                CreateIndexList(obj, (FMDL)Parent.Parent, settings.LimitSkinCount, ForceSkinInfluenceMax);
                                BoneIndices = GetIndices(GetParentModel().Skeleton);

                                ApplyImportSettings(settings, GetFMAT());

                                CreateNewBoundingBoxes(GetParentModel().Skeleton);
                                OptmizeAttributeFormats();
                                SaveShape(IsWiiU);
                                SaveVertexBuffer(IsWiiU);

                                Cursor.Current = Cursors.Default;
                            }
                        }
                        break;
                }
                UpdateVertexData();
            }
        }
        public void CreateIndexList(STGenericObject ob, FMDL mdl = null, bool ForceSkinLimt = false, int LimitAmount = 4)
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

            if (ForceSkinLimt && LimitAmount > 0)
                BoneIndices.Add((ushort)0);

            foreach (STBone bone in mdl.Skeleton.bones)
            {
                foreach (string bnam in boneNames)
                {
                    if (bone.Text == bnam)
                    {
                        int index = boneNames.IndexOf(bone.Text);
                        STConsole.WriteLine($"Adding bone to shape index list! {bone.Text} {index}");

                        if (!BoneIndices.Contains((ushort)index))
                            BoneIndices.Add((ushort)index);
                    }
                }
            }
        }
        public void CreateBoneList(STGenericObject ob, FMDL mdl, bool ForceSkinCount, int ForcedSkinAmount = 4)
        {
            if (ForceSkinCount && !ob.HasIndices && VertexSkinCount != 0 && !vertexAttributes.Any(x => x.Name == "_i0"))
            {
                var attributeIndex = new FSHP.VertexAttribute();
                attributeIndex.Format = ResGFX.AttribFormat.Format_8_8_8_8_UInt;
                attributeIndex.Name = "_i0";
                vertexAttributes.Add(attributeIndex);
            }

            //Check weights. If they are all 1. If they are then they aren't necessary
            if (ob.VertexSkinCount == 1 || ForceSkinCount && ForcedSkinAmount == 1)
            {
                bool UseWeights = ob.vertices.Any(o => o.boneWeights.Count > 0);
                if (!UseWeights)
                {
                    for (int v = 0; v < ob.vertices.Count; v++)
                    {
                        ob.vertices[v].boneWeights.Clear();
                    }

                    var weightAttribute = vertexAttributes.Where(att => att.Name == "_w0");
                    if (weightAttribute != null && weightAttribute.ToList().Count > 0)
                    {
                        vertexAttributes.Remove(weightAttribute.First());
                    }
                }
            }

            bool UseRigidSkinning = ob.VertexSkinCount == 1;

            if (mdl.Skeleton.Node_Array == null || mdl.Skeleton.Node_Array.Length == 0)
                return;

            var bones = new Dictionary<string, STBone>();
            foreach (var index in mdl.Skeleton.Node_Array) {
                if (index < mdl.Skeleton.bones.Count) {
                    var bone = mdl.Skeleton.bones[index];
                    if (bones.ContainsKey(bone.Text))
                    {
                        STConsole.WriteLine($"There are multiple bones named {bone.Text}. Using the first one.", System.Drawing.Color.Red);
                    }
                    else
                    {
                        bones.Add(bone.Text, bone);
                    }
                }
            }


            List<string> BonesNotMatched = new List<string>();
            foreach (Vertex v in ob.vertices)
            {
                List<int> RigidIds = new List<int>();
                foreach (string bn in v.boneNames)
                {
                    STBone bone;
                    bool hasMatch = bones.TryGetValue(bn, out bone);
                    if (hasMatch)
                    {
                        if (!UseRigidSkinning && bone.SmoothMatrixIndex != -1)
                        {
                            if (v.boneIds.Count < ForcedSkinAmount)
                            {
                                var index = Array.FindIndex(mdl.Skeleton.Node_Array, boneIndex => mdl.Skeleton.bones[boneIndex].Text == bn);
                                v.boneIds.Add(index);
                            }
                        }
                        else if (bone.RigidMatrixIndex != -1)
                        {
                            v.boneIds.Add(bone.RigidMatrixIndex);
                        }
                        else if (bone.SmoothMatrixIndex != -1)
                        {
                            STConsole.WriteLine(bone.SmoothMatrixIndex + " mesh " + Text + " bone " + bn);
                            v.boneIds.Add(bone.SmoothMatrixIndex);
                        }
                        else
                        {
                            // last-ditch effort if no rigid or smooth index
                            var index = Array.FindIndex(mdl.Skeleton.Node_Array, boneIndex => mdl.Skeleton.bones[boneIndex].Text == bn);
                            if (index != -1)
                            {
                                v.boneIds.Add(index);
                            }
                            else
                            {
                                BonesNotMatched.Add(bn);
                                STConsole.WriteLine($"Bone {bn} is not skinnable. Vertices will remain unmapped for this bone!", System.Drawing.Color.Red);
                            }
                        }
                    }
                    else if (!BonesNotMatched.Contains(bn))
                    {
                        BonesNotMatched.Add(bn);
                        STConsole.WriteLine($"No bone matches {bn}. Vertices will remain unmapped for this bone!", System.Drawing.Color.Red);
                    } // else bone not matched but already generated warning for it
                }

                //Sort smooth bones
                if (v.boneWeights.Count > 0)
                {
                    int j = 0;

                    List<Tuple<int, float>> envelopes = new List<Tuple<int, float>>();
                    for (j = 0; j < v.boneIds.Count; j++)
                        envelopes.Add(Tuple.Create(v.boneIds[j], v.boneWeights[j]));

                    envelopes.Sort((x, y) => y.Item1.CompareTo(x.Item1));

                    j = 0;
                    foreach (var envelope in envelopes.OrderBy(x => x.Item1))
                    {
                        v.boneIds[j] = envelope.Item1;
                        v.boneWeights[j] = envelope.Item2;
                        j++;
                    }

                    envelopes.Clear();
                }

                if (RigidIds.Count > 0)
                {
                    foreach (int id in RigidIds)
                    {
                        if (v.boneIds.Count < ob.VertexSkinCount)
                            v.boneIds.Add(id);
                    }


                    //Use only rigid ids for rigid skinning
                    if (ob.VertexSkinCount == 1)
                    {
                        v.boneIds.Clear();
                        v.boneIds.Add(RigidIds[0]);
                    }
                }

                if (ForceSkinCount)
                {
                    for (int i = 0; i < ForcedSkinAmount; i++)
                    {
                        if (v.boneIds.Count < ob.VertexSkinCount)
                        {
                            v.boneIds.Add(0);

                            if (v.boneWeights.Count > 0)
                                v.boneWeights.Add(0);
                        }
                    }
                }
            }
        }

        public void CreateNewBoundingBoxes(STSkeleton skeleton)
        {
            boundingBoxes.Clear();
            boundingRadius.Clear();
            foreach (LOD_Mesh mesh in lodMeshes)
            {
                BoundingBox box = CalculateBoundingBox(skeleton);
                boundingBoxes.Add(box);
                boundingRadius.Add(box.Radius);
                foreach (LOD_Mesh.SubMesh sub in mesh.subMeshes)
                    boundingBoxes.Add(box);
            }
        }

        private BoundingBox CalculateBoundingBox(STSkeleton skeleton)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            if (VertexSkinCount > 0)
            {
                var aabb = CalculateSkinnedBoundings(skeleton, vertices);
                //Failed to get bounding data for some reason, calculate normally
                //This shouldn't happen but it's a fail safe option.
                if (aabb.Count == 0)
                {
                    min = CalculateBBMin(vertices);
                    max = CalculateBBMax(vertices);
                }

                //Get the largest values in local space to create the largest bounding box
                foreach (var bounding in aabb)
                {
                    min.X = Math.Min(min.X, bounding.Min.X);
                    min.Y = Math.Min(min.Y, bounding.Min.Y);
                    min.Z = Math.Min(min.Z, bounding.Min.Z);
                    max.X = Math.Max(max.X, bounding.Max.X);
                    max.Y = Math.Max(max.Y, bounding.Max.Y);
                    max.Z = Math.Max(max.Z, bounding.Max.Z);
                }
            }
            else
            {
                min = CalculateBBMin(vertices);
                max = CalculateBBMax(vertices);
            }

            Vector3 center = max + min;

            float xxMax = GetExtent(max.X, min.X);
            float yyMax = GetExtent(max.Y, min.Y);
            float zzMax = GetExtent(max.Z, min.Z);
            Vector3 extend = new Vector3(xxMax, yyMax, zzMax);

            float radius = (float)(center.Length + extend.Length);

            return new BoundingBox() { Radius = radius, Center = center, Extend = extend };
        }

        private List<AABB> CalculateSkinnedBoundings(STSkeleton skeleton, List<Vertex> vertices)
        {
            Dictionary<int, AABB> skinnedBoundings = new Dictionary<int, AABB>();
            for (int i = 0; i < vertices.Count; i++)
            {
                foreach (var boneID in vertices[i].boneIds)
                {
                    if (!skinnedBoundings.ContainsKey(boneID))
                        skinnedBoundings.Add(boneID, new AABB());

                    //Get the skinned bone transform
                    var transform = skeleton.bones[boneID].Transform;
                    var inverted = transform.Inverted();

                    //Get the position in local coordinates
                    var position = vertices[i].pos;
                    position = OpenTK.Vector3.TransformPosition(position, inverted);

                    var bounding = skinnedBoundings[boneID];
                    //Set the min and max values
                    bounding.Min.X = Math.Min(bounding.Min.X, position.X);
                    bounding.Min.Y = Math.Min(bounding.Min.Y, position.Y);
                    bounding.Min.Z = Math.Min(bounding.Min.Z, position.Z);
                    bounding.Max.X = Math.Max(bounding.Max.X, position.X);
                    bounding.Max.Y = Math.Max(bounding.Max.Y, position.Y);
                    bounding.Max.Z = Math.Max(bounding.Max.Z, position.Z);
                }
            }
            return skinnedBoundings.Values.ToList();
        }

        class AABB
        {
            public Vector3 Min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            public Vector3 Max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        }

        private float CalculateBoundingRadius(Vector3 min, Vector3 max)
        {
            Vector3 length = max - min;
           return CalculateRadius(length.X / 2.0f, length.Y / 2.0f);
        }

        private static float CalculateRadius(float horizontalLeg, float verticalLeg) {
            return (float)Math.Sqrt((horizontalLeg * horizontalLeg) + (verticalLeg * verticalLeg));
        }

        private float GetExtent(float max, float min)
        {
            return (float)Math.Max(Math.Sqrt(max * max), Math.Sqrt(min * min));
        }

        private Vector3 CalculateBBMin(List<Vertex> positionVectors)
        {
            Vector3 minimum = new Vector3(float.MaxValue);
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
            Vector3 maximum = new Vector3(float.MinValue);
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
        public int TargetAttribCount;

        public List<float> boundingRadius = new List<float>();
        public List<BoundingBox> boundingBoxes = new List<BoundingBox>();
        public class BoundingBox
        {
            public Vector3 Center;
            public Vector3 Extend;
            public float Radius;
        }

        public List<VertexAttribute> vertexAttributes = new List<VertexAttribute>();
        public class VertexAttribute
        {
            public enum AttributeType
            {
                Unknown,
                Position,
                Normal,
                UV,
                Color,
                Tangent,
                Bitangent,
                Weight,
                Index,
            }

            public string Name;
            public ResGFX.AttribFormat Format;

            public byte BufferIndex { get; set; }

            public override string ToString()
            {
                return Name;
            }

            public AttributeType GetAttributeType()
            {
                var index = string.Concat(Name.ToArray().Reverse().TakeWhile(char.IsNumber).Reverse());

                if (Name == $"_p{index}")
                    return AttributeType.Position;
                if (Name == $"_n{index}")
                    return AttributeType.Normal;
                if (Name == $"_u{index}")
                    return AttributeType.UV;
                if (Name == $"_c{index}")
                    return AttributeType.Color;
                if (Name == $"_t{index}")
                    return AttributeType.Tangent;
                if (Name == $"_b{index}")
                    return AttributeType.Bitangent;
                if (Name == $"_w{index}")
                    return AttributeType.Weight;
                if (Name == $"_i{index}")
                    return AttributeType.Index;

                return AttributeType.Unknown;
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

            indices.Sort();

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
                    bool IsRigidIndex = skel.IsIndexRigid(BoneIndex);
                    if (!IsRigidIndex)
                        return position;

                    if (BoneIndex >= skel.Node_Array.Length || BoneIndex == -1)
                    {
                        STConsole.WriteLine($"Invalid bone index to bind bone to mesh {Text} {BoneIndex} ", System.Drawing.Color.Red);
                        return position;
                    }

                    var bone = skel.bones[skel.Node_Array[BoneIndex]];
                    trans = bone.invert;

                    if (trans.Determinant == 0)
                    {
                        STConsole.WriteLine($"Determinant for bone transform is 0 to bind bone to mesh {Text} {BoneIndex} ", System.Drawing.Color.Red);
                        return position;
                    }
                }
                else
                {
                    var bone = skel.bones[BoneIndex];
                    trans = bone.invert;

                    if (trans.Determinant == 0)
                    {
                        STConsole.WriteLine($"Determinant for bone transform is 0 to bind bone to mesh {Text} {BoneIndex} ", System.Drawing.Color.Red);
                        return position;
                    }
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

        public override void SaveVertexBuffer()
        {
            SaveVertexBuffer(GetResFileU() != null);
        }

        public void SaveVertexBuffer(bool IsWiiU)
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

                    for (int i = 0; i < weights.Count; i++)
                    {
                        Console.WriteLine($"w {i} {weights[i]}");
                    }

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

        public void OptmizeAttributeFormats()
        {
            for (int i = 0; i < vertexAttributes.Count; i++)
            {
                if (vertexAttributes[i].GetAttributeType() == VertexAttribute.AttributeType.Weight ||
                    vertexAttributes[i].GetAttributeType() == VertexAttribute.AttributeType.Index)
                {
                    OptmizeIndicesWeights(vertexAttributes[i]);
                }
            }
        }

        private void OptmizeIndicesWeights(VertexAttribute attribute)
        {
            if (VertexSkinCount == 1)
            {
                switch (attribute.Format)
                {
                    case ResGFX.AttribFormat.Format_32_32_32_32_Single:
                    case ResGFX.AttribFormat.Format_32_32_32_Single:
                    case ResGFX.AttribFormat.Format_32_32_Single:
                        attribute.Format = ResGFX.AttribFormat.Format_32_Single;
                        break;
                    case ResGFX.AttribFormat.Format_32_32_32_32_SInt:
                    case ResGFX.AttribFormat.Format_32_32_32_SInt:
                    case ResGFX.AttribFormat.Format_32_32_SInt:
                        attribute.Format = ResGFX.AttribFormat.Format_32_SInt;
                        break;
                    case ResGFX.AttribFormat.Format_32_32_32_32_UInt:
                    case ResGFX.AttribFormat.Format_32_32_32_UInt:
                    case ResGFX.AttribFormat.Format_32_32_UInt:
                        attribute.Format = ResGFX.AttribFormat.Format_32_UInt;
                        break;
                    case ResGFX.AttribFormat.Format_16_16_16_16_Single:
                    case ResGFX.AttribFormat.Format_16_16_Single:
                        attribute.Format = ResGFX.AttribFormat.Format_16_Single;
                        break;
                    case ResGFX.AttribFormat.Format_16_16_16_16_SInt:
                    case ResGFX.AttribFormat.Format_16_16_SInt:
                        attribute.Format = ResGFX.AttribFormat.Format_16_SInt;
                        break;
                    case ResGFX.AttribFormat.Format_16_16_16_16_UInt:
                    case ResGFX.AttribFormat.Format_16_16_UInt:
                        attribute.Format = ResGFX.AttribFormat.Format_16_UInt;
                        break;
                    case ResGFX.AttribFormat.Format_8_8_8_8_UInt:
                    case ResGFX.AttribFormat.Format_8_8_UInt:
                        attribute.Format = ResGFX.AttribFormat.Format_8_UInt;
                        break;
                    case ResGFX.AttribFormat.Format_8_8_8_8_SInt:
                    case ResGFX.AttribFormat.Format_8_8_SInt:
                        attribute.Format = ResGFX.AttribFormat.Format_8_SInt;
                        break;
                    case ResGFX.AttribFormat.Format_8_8_8_8_SNorm:
                    case ResGFX.AttribFormat.Format_8_8_SNorm:
                        attribute.Format = ResGFX.AttribFormat.Format_8_SNorm;
                        break;
                    case ResGFX.AttribFormat.Format_8_8_8_8_UNorm:
                    case ResGFX.AttribFormat.Format_8_8_UNorm:
                        attribute.Format = ResGFX.AttribFormat.Format_8_UNorm;
                        break;
                    case ResGFX.AttribFormat.Format_8_8_8_8_SIntToSingle:
                    case ResGFX.AttribFormat.Format_8_8_SIntToSingle:
                        attribute.Format = ResGFX.AttribFormat.Format_8_SIntToSingle;
                        break;
                    case ResGFX.AttribFormat.Format_8_8_8_8_UIntToSingle:
                    case ResGFX.AttribFormat.Format_8_8_UIntToSingle:
                        attribute.Format = ResGFX.AttribFormat.Format_8_UIntToSingle;
                        break;
                }
            }
            if (VertexSkinCount == 2)
            {
                switch (attribute.Format)
                {
                    case ResGFX.AttribFormat.Format_32_32_32_32_Single:
                    case ResGFX.AttribFormat.Format_32_32_32_Single:
                        attribute.Format = ResGFX.AttribFormat.Format_32_32_Single;
                        break;
                    case ResGFX.AttribFormat.Format_32_32_32_32_SInt:
                    case ResGFX.AttribFormat.Format_32_32_32_SInt:
                        attribute.Format = ResGFX.AttribFormat.Format_32_32_SInt;
                        break;
                    case ResGFX.AttribFormat.Format_32_32_32_32_UInt:
                    case ResGFX.AttribFormat.Format_32_32_32_UInt:
                        attribute.Format = ResGFX.AttribFormat.Format_32_32_UInt;
                        break;
                    case ResGFX.AttribFormat.Format_16_16_16_16_Single:
                        attribute.Format = ResGFX.AttribFormat.Format_16_16_Single;
                        break;
                    case ResGFX.AttribFormat.Format_16_16_16_16_SInt:
                        attribute.Format = ResGFX.AttribFormat.Format_16_16_SInt;
                        break;
                    case ResGFX.AttribFormat.Format_16_16_16_16_UInt:
                        attribute.Format = ResGFX.AttribFormat.Format_16_16_UInt;
                        break;
                    case ResGFX.AttribFormat.Format_8_8_8_8_UInt:
                        attribute.Format = ResGFX.AttribFormat.Format_8_8_UInt;
                        break;
                    case ResGFX.AttribFormat.Format_8_8_8_8_SInt:
                        attribute.Format = ResGFX.AttribFormat.Format_8_8_SInt;
                        break;
                    case ResGFX.AttribFormat.Format_8_8_8_8_SNorm:
                        attribute.Format = ResGFX.AttribFormat.Format_8_8_SNorm;
                        break;
                    case ResGFX.AttribFormat.Format_8_8_8_8_UNorm:
                        attribute.Format = ResGFX.AttribFormat.Format_8_8_UNorm;
                        break;
                    case ResGFX.AttribFormat.Format_8_8_8_8_SIntToSingle:
                        attribute.Format = ResGFX.AttribFormat.Format_8_8_SIntToSingle;
                        break;
                    case ResGFX.AttribFormat.Format_8_8_8_8_UIntToSingle:
                        attribute.Format = ResGFX.AttribFormat.Format_8_8_UIntToSingle;
                        break;
                }
            }
            if (VertexSkinCount == 3)
            {
                switch (attribute.Format)
                {
                    case ResGFX.AttribFormat.Format_32_32_32_32_Single:
                        attribute.Format = ResGFX.AttribFormat.Format_32_32_32_Single;
                        break;
                    case ResGFX.AttribFormat.Format_32_32_32_32_SInt:
                        attribute.Format = ResGFX.AttribFormat.Format_32_32_32_SInt;
                        break;
                    case ResGFX.AttribFormat.Format_32_32_32_32_UInt:
                        attribute.Format = ResGFX.AttribFormat.Format_32_32_32_UInt;
                        break;
                }
            }
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

                var WeightAttribute = GetWeightAttribute(0);

                //Produce identical results for the weight output as BFRES_Vertex.py
                //This should prevent encoding back and exploding
                int MaxWeight = 255;
                for (int i = 0; i < 4; i++)
                {
                    if (VertexSkinCount < i + 1 || vtx.boneWeights.Count < i + 1)
                    {
                        weightsA[i] = 0;
                        MaxWeight = 0;
                    }
                    else
                    {
                        int weight = (int)(vtx.boneWeights[i] * 255);
                        if (vtx.boneWeights.Count == i + 1)
                            weight = MaxWeight;

                        if (weight >= MaxWeight)
                        {
                            weight = MaxWeight;
                            MaxWeight = 0;
                        }
                        else
                            MaxWeight -= weight;

                        weightsA[i] = weight / 255f;
                    }
                }

                for (int i = 0; i < VertexSkinCount; i++) {
                    if (vtx.boneIds.Count > i)
                        indicesA[i] = vtx.boneIds[i];
                }

                weights.Add(new Syroot.Maths.Vector4F(weightsA[0], weightsA[1], weightsA[2], weightsA[3]));
                boneInd.Add(new Syroot.Maths.Vector4F(indicesA[0], indicesA[1], indicesA[2], indicesA[3]));
            }
        }

        public List<DisplayVertex> CreateDisplayVertices(FMDL model)
        {
            // rearrange faces
            display = lodMeshes[DisplayLODIndex].getDisplayFace().ToArray();

            List<DisplayVertex> displayVertList = new List<DisplayVertex>();

            if (lodMeshes[DisplayLODIndex].faces.Count <= 3)
                return displayVertList;

            foreach (Vertex v in vertices)
            {
                model.MaxPosition = OpenGLUtils.GetMax(model.MaxPosition, v.pos);
                model.MinPosition = OpenGLUtils.GetMin(model.MinPosition, v.pos);

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
