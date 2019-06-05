using System;
using System.Collections.Generic;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.NodeWrappers;
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
    public class FMDL : STGenericModel
    {
        //These get updated on UpdateVertexData()
        public Vector3 MaxPosition = new Vector3(0);
        public Vector3 MinPosition = new Vector3(0);

        public bool IsEdited { get; set; }

        public List<FSHP> shapes = new List<FSHP>();
        public List<FSHP> depthSortedMeshes = new List<FSHP>();

        public Dictionary<string, FMAT> materials = new Dictionary<string, FMAT>();
        public Model Model;
        public ResU.Model ModelU;

        public ResFile GetResFile()
        {
            if (Parent == null || Parent.Parent == null)
                return null;

            //ResourceFile -> FMDL -> Model Folder -> this
            return ((BFRES)Parent.Parent).resFile;
        }
        public ResU.ResFile GetResFileU()
        {
            if (Parent == null || Parent.Parent == null)
                return null;

            return ((BFRES)Parent.Parent).resFileU;
        }
        public void UpdateVertexData()
        {
            if (Parent != null)
                GetRenderer().UpdateVertexData();
        }
        public BFRESRender GetRenderer()
        {
            return ((BFRES)Parent.Parent).BFRESRender;
        }
        public List<FMDL> GetModelList()
        {
            return GetRenderer().models;
        }

        public FMDL()
        {
            ImageKey = "model";
            SelectedImageKey = "model";
            Checked = true;
            CanReplace = true;
            CanDelete = true;

            Nodes.Add(new FSHPFolder());
            Nodes.Add(new FMATFolder());
            Nodes.Add(new FSKL.fsklNode());

            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Transform", null, TransformToolAction, Keys.Control | Keys.T));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Calculate Tangents/Bitangents", null, CalcTansBitansAllShapesAction, Keys.Control | Keys.C));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Normals", null,
              new ToolStripMenuItem("Smooth", null, SmoothNormalsAction),
              new ToolStripMenuItem("Recalculate", null, RecalculateNormalsAction)
            ));

            ContextMenuStrip.Items.Add(new ToolStripMenuItem("UVs", null,
              new ToolStripMenuItem("Flip Vertical", null, FlipUvsVerticalAction),
              new ToolStripMenuItem("Flip Horizontal", null, FlipUvsHorizontalAction),
              new ToolStripMenuItem("Copy UV Channel", null, CopyUVChannels)
            ));

            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Colors", null,
              new ToolStripMenuItem("  Set Color", null, SetVertexColorDialogAction),
              new ToolStripMenuItem("Set White Color", null, SetVertexColorWhiteAction)
            ));
        }

        protected void TransformToolAction(object sender, EventArgs e) { TransformTool(); }
        protected void CalcTansBitansAllShapesAction(object sender, EventArgs e) { CalcTansBitansAllShapes(); }
        protected void SmoothNormalsAction(object sender, EventArgs e) { SmoothNormals(); }
        protected void RecalculateNormalsAction(object sender, EventArgs e) { RecalculateNormals(); }
        protected void FlipUvsVerticalAction(object sender, EventArgs e) { FlipUvsVertical(); }
        protected void FlipUvsHorizontalAction(object sender, EventArgs e) { FlipUvsHorizontal(); }
        protected void CopyUVChannels(object sender, EventArgs e) { CopyUVChannels(); }
        protected void SetVertexColorDialogAction(object sender, EventArgs e) { SetVertexColorDialog(); }
        protected void SetVertexColorWhiteAction(object sender, EventArgs e) { SetVertexColorWhite(); }

        public override void Delete()
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove this model? This cannot be undone!", "", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                ((BFRES)Parent.Parent).RemoveSkeletonDrawable(Skeleton);

                shapes.Clear();
                materials.Clear();

                UpdateVertexData();

                Nodes.Clear();
                ((BFRESGroupNode)Parent).RemoveChild(this);
            }
        }

        public void CopyUVChannels()
        {
            CopyUVChannelDialog dialog = new CopyUVChannelDialog();
            dialog.LoadUVAttributes();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int dest = dialog.DestIndex;
                int source = dialog.SourceIndex;

                bool CanCreateUV = false;
                bool HasDestUV = false;
                bool ShownDialog = false;

                foreach (var shape in shapes)
                {
                    //If no source, there's nothing to copy from
                    if (!shape.vertexAttributes.Any(x => x.Name == $"_u{source}"))
                        continue;

                    if ((!shape.vertexAttributes.Any(x => x.Name == $"_u{dest}")))
                    {
                        //Only show the dialog once for creating UV channels
                        if (!CanCreateUV && !ShownDialog)
                        {
                            DialogResult dialogResult = MessageBox.Show($"Some of the objects are missing the destenation uv channel ({dest}) to copy to. Create one?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (dialogResult == DialogResult.Yes)
                            {
                                CanCreateUV = true;
                                ShownDialog = true;
                            }
                        }

                        if (CanCreateUV)
                        {
                            FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                            att.Name = $"_u{dest}";
                            att.Format = ResGFX.AttribFormat.Format_16_16_Single;
                            shape.vertexAttributes.Add(att);

                            HasDestUV = true;
                        }
                    }
                    else
                        HasDestUV = true;

                    if (HasDestUV)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        shape.CopyUVChannel(dialog.SourceIndex, dialog.DestIndex);
                        shape.SaveVertexBuffer(GetResFileU() != null);
                        Cursor.Current = Cursors.Default;
                    }

                    shape.CopyUVChannel(dialog.SourceIndex, dialog.DestIndex);
                    shape.SaveVertexBuffer(GetResFileU() != null);
                }
            }

            UpdateVertexData();
        }

        public void FlipUvsVertical()
        {
            foreach (var shape in shapes)
            {
                if (!shape.HasUV0())
                {
                    MessageBox.Show($"Error! {Text} does not have UVs!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                shape.FlipUvsVertical();
                shape.SaveVertexBuffer(GetResFileU() != null);
            }
 
            UpdateVertexData();
        }
        public void FlipUvsHorizontal()
        {
            foreach (var shape in shapes)
            {
                if (!shape.HasUV0())
                {
                    MessageBox.Show($"Error! {Text} does not have UVs!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                shape.FlipUvsHorizontal();
                shape.SaveVertexBuffer(GetResFileU() != null);
            }

            UpdateVertexData();
        }
        private void BatchAttributeEdit()
        {
            AttributeEditor editor = new AttributeEditor();
            editor.LoadObjects(this);

            if (editor.ShowDialog() == DialogResult.OK)
            {

            }
        }
        private void SetVertexColorDialog()
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                foreach (FSHP shp in shapes)
                {
                    shp.CheckVertexColors();

                    shp.SetVertexColor(new OpenTK.Vector4(
                        dlg.Color.R / 255.0f,
                        dlg.Color.G / 255.0f,
                        dlg.Color.B / 255.0f,
                        dlg.Color.A / 255.0f));

                    shp.SaveVertexBuffer(GetResFileU() != null);
                }
            }
            UpdateVertexData();
        }
        private void SetVertexColorWhite()
        {
            foreach (FSHP shp in shapes)
            {
                shp.CheckVertexColors();
                shp.SetVertexColor(new OpenTK.Vector4(1, 1, 1, 1));
                shp.SaveVertexBuffer(GetResFileU() != null);
            }
            UpdateVertexData();
        }
        private void SmoothNormals()
        {
            Cursor.Current = Cursors.WaitCursor;
            foreach (FSHP shp in shapes)
            {
                bool HasNormals = shp.vertexAttributes.Any(x => x.Name == "_n0");
                if (HasNormals)
                    shp.SmoothNormals();

                shp.SaveVertexBuffer(GetResFileU() != null);
            }
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }
        private void RecalculateNormals()
        {
            Cursor.Current = Cursors.WaitCursor;
            foreach (FSHP shp in shapes)
            {
                bool HasNormals = shp.vertexAttributes.Any(x => x.Name == "_n0");
                if (HasNormals)
                    shp.CalculateNormals();

                shp.SaveVertexBuffer(GetResFileU() != null);
            }
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }
        public override void Rename()
        {
            RenameDialog dialog = new RenameDialog();
            dialog.SetString(Text);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Text = dialog.textBox1.Text;
            }
        }
        private void CalcTansBitansAllShapes()
        {
            Cursor.Current = Cursors.WaitCursor;
            foreach (FSHP shp in shapes)
            {
                bool HasTans = shp.vertexAttributes.Any(x => x.Name == "_t0");
                bool HasBiTans = shp.vertexAttributes.Any(x => x.Name == "_b0");

                if (!shp.HasUV0())
                {
                    MessageBox.Show($"Error! {Text} does not have UVs!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!HasBiTans)
                {
                    DialogResult dialogResult2 = MessageBox.Show("Mesh does not have bitangents. Do you want to create them? (will make file size bigger)", "", MessageBoxButtons.YesNo);

                    FSHP.VertexAttribute att2 = new FSHP.VertexAttribute();
                    att2.Name = "_b0";
                    att2.Format = ResGFX.AttribFormat.Format_10_10_10_2_SNorm;

                    if (dialogResult2 == DialogResult.Yes)
                    {
                        if (!HasBiTans)
                            shp.vertexAttributes.Add(att2);
                    }
                }

                if (!HasTans)
                {
                    DialogResult dialogResult = MessageBox.Show("Mesh does not have tangets. Do you want to create them? (will make file size bigger)", "", MessageBoxButtons.YesNo);

                    FSHP.VertexAttribute att = new FSHP.VertexAttribute();
                    att.Name = "_t0";
                    att.Format = ResGFX.AttribFormat.Format_10_10_10_2_SNorm;


                    if (dialogResult == DialogResult.Yes)
                    {
                        if (!HasTans)
                            shp.vertexAttributes.Add(att);
                    }
                }

                bool UseUVLayer2 = false;

                //for BOTW if it uses UV layer 2 for normal maps use second UV map
                if (shp.GetMaterial().shaderassign.options.ContainsKey("uking_texture2_texcoord")) {
                    float value = float.Parse(shp.GetMaterial().shaderassign.options["uking_texture2_texcoord"]);

                    if (value == 1)
                        UseUVLayer2 = true;
                }

                shp.CalculateTangentBitangent(UseUVLayer2);
                shp.SaveVertexBuffer(GetResFileU() != null);
            }

            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }
        public void CopyMaterial(FMAT selectedMaterial)
        {
            CopyMaterialMenu menu = new CopyMaterialMenu();
            menu.LoadMaterials(selectedMaterial.Text, GetModelList());
            if (menu.ShowDialog() == DialogResult.OK)
            {
                foreach (TreeNode mdl in menu.materialTreeView.Nodes)
                {
                    foreach (TreeNode n in mdl.Nodes)
                    {
                        if (n.Checked)
                        {
                            if (materials.ContainsKey(n.Text))
                                SetCopiedMaterialData(menu, selectedMaterial, materials[n.Text]);
                        }
                    }
                }
                LibraryGUI.Instance.UpdateViewport();
            }
        }
        private void SetCopiedMaterialData(CopyMaterialMenu menu,
            FMAT selectedMaterial, FMAT targetMaterial)
        {
            targetMaterial.Material.Flags = selectedMaterial.Material.Flags;
            targetMaterial.Material.UserDatas = selectedMaterial.Material.UserDatas;
            targetMaterial.Material.UserDataDict = selectedMaterial.Material.UserDataDict;

            if (menu.chkBoxRenderInfo.Checked)
            {
                targetMaterial.Material.RenderInfoDict = selectedMaterial.Material.RenderInfoDict;
                targetMaterial.Material.RenderInfos = selectedMaterial.Material.RenderInfos;
            }
            if (menu.chkBoxShaderOptions.Checked)
            {
                targetMaterial.Material.ShaderAssign = selectedMaterial.Material.ShaderAssign;
            }
            if (menu.chkBoxShaderParams.Checked)
            {
                targetMaterial.Material.ShaderParamData = selectedMaterial.Material.ShaderParamData;
                targetMaterial.Material.ShaderParamDict = selectedMaterial.Material.ShaderParamDict;
                targetMaterial.Material.ShaderParams = selectedMaterial.Material.ShaderParams;
                targetMaterial.Material.VolatileFlags = selectedMaterial.Material.VolatileFlags;
            }
            if (menu.chkBoxTextures.Checked)
            {
                targetMaterial.Material.SamplerDict = selectedMaterial.Material.SamplerDict;
                targetMaterial.Material.Samplers = selectedMaterial.Material.Samplers;
                targetMaterial.Material.SamplerSlotArray = selectedMaterial.Material.SamplerSlotArray;
                targetMaterial.Material.TextureSlotArray = selectedMaterial.Material.TextureSlotArray;
                targetMaterial.Material.TextureRefs = selectedMaterial.Material.TextureRefs;
            }
            targetMaterial.ReadMaterial(targetMaterial.Material);
        }
        private void TransformTool()
        {
            TransformMeshTool transform = new TransformMeshTool();
            foreach (var mesh in shapes)
            {
                transform.LoadGenericMesh(mesh, UpdateVertexData);
            }
            transform.CacheList();

            if (transform.ShowDialog() != DialogResult.OK)
            {
                int curShp = 0;
                foreach (var mesh in shapes)
                {
                    mesh.vertices = transform.cachedBackups[curShp].vertices;

                    curShp++;
                }
                UpdateVertexData();
            }
            else
            {
                foreach (var shape in shapes) {
                    shape.SaveVertexBuffer(GetResFileU() != null);
                }
                UpdateVertexData();
            }
        }

        public override void Unload()
        {
            if (Parent != null)
                ((BFRES)Parent.Parent).RemoveSkeletonDrawable(Skeleton);

            shapes.Clear();
            materials.Clear();
            Nodes.Clear();
        }

        public override string ExportFilter => FileFilters.GetFilter(typeof(FMDL), null, true);
        public override string ImportFilter => FileFilters.GetFilter(typeof(FMDL));

        public override void Export(string FileName)
        {
            string ext = System.IO.Path.GetExtension(FileName);
            ext = ext.ToLower();

            switch (ext)
            {
                case ".bfmdl":
                    if (GetResFileU() != null)
                        ModelU.Export(FileName, GetResFileU());
                    else
                        Model.Export(FileName, GetResFile());
                    break;
                case ".obj":
                    OBJ.ExportModel(FileName, this, GetTextures());
                    break;
                default:
                    AssimpSaver assimp = new AssimpSaver();
                    ExportModelSettings settings = new ExportModelSettings();
                    if (settings.ShowDialog() == DialogResult.OK)
                    {
                        if (settings.ExportTextures)
                            assimp.SaveFromModel(this, FileName, GetTextures(), Skeleton, Skeleton.Node_Array.ToList());
                        else
                            assimp.SaveFromModel(this, FileName, new List<STGenericTexture>(), Skeleton, Skeleton.Node_Array.ToList());
                    }
                    break;
            }
        }

        private List<STGenericTexture> GetTextures()
        {
            List<STGenericTexture> textures = new List<STGenericTexture>();
            foreach (var mat in materials)
            {
                foreach (var texref in mat.Value.TextureMaps)
                {
                    foreach (var bntx in PluginRuntime.bntxContainers)
                    {
                        if (bntx.Textures.ContainsKey(texref.Name))
                            textures.Add(bntx.Textures[texref.Name]);
                    }
                    foreach (var ftexCont in PluginRuntime.ftexContainers)
                    {
                        if (ftexCont.ResourceNodes.ContainsKey(texref.Name))
                            textures.Add((FTEX)ftexCont.ResourceNodes[texref.Name]);
                    }
                }
            }

            return textures;
        }

        public override void Replace(string FileName) {
            AddOjects(FileName, GetResFile(), GetResFileU());
        }

        public void Replace(string FileName, ResFile resFileNX, ResU.ResFile resFileU) {
            AddOjects(FileName, resFileNX, resFileU);
        }

        //Function addes shapes, vertices and meshes
        public void AddOjects(string FileName, ResFile resFileNX, ResU.ResFile resFileU,  bool Replace = true)
        {
            bool IsWiiU = (resFileU != null);

            int MatStartIndex = materials.Count;
            string ext = System.IO.Path.GetExtension(FileName);
            ext = ext.ToLower();

            switch (ext)
            {
                case ".bfobj":
                    Cursor.Current = Cursors.WaitCursor;

                    if (Replace)
                    {
                        shapes.Clear();
                        Nodes["FshpFolder"].Nodes.Clear();
                    }
                    if (IsWiiU)
                    {
                        var shpS = new ResU.Shape();
                        var vertexBufferU = new ResU.VertexBuffer();
                        shpS.Import(FileName, vertexBufferU, resFileU);

                        FSHP shapeS = new FSHP();
                        shapeS.ShapeU = shpS;
                        BfresWiiU.ReadShapesVertices(shapeS, shpS, vertexBufferU, this);
                        shapes.Add(shapeS);
                        Nodes["FshpFolder"].Nodes.Add(shapeS);
                    }
                    else
                    {
                        Shape shpS = new Shape();
                        VertexBuffer vertexBuffer = new VertexBuffer();
                        shpS.Import(FileName, vertexBuffer);

                        FSHP shapeS = new FSHP();
                        shapeS.Shape = shpS;
                        BfresSwitch.ReadShapesVertices(shapeS, shpS, vertexBuffer, this);
                        shapes.Add(shapeS);
                        Nodes["FshpFolder"].Nodes.Add(shapeS);
                    }

                    IsEdited = true;

                    Cursor.Current = Cursors.Default;
                    break;
                case ".bfmdl":
                    Cursor.Current = Cursors.WaitCursor;

                    if (Replace)
                    {
                        shapes.Clear();
                        Nodes["FshpFolder"].Nodes.Clear();
                        materials.Clear();
                        Nodes["FmatFolder"].Nodes.Clear();
                    }

                    if (IsWiiU)
                    {
                        var mdlU = new ResU.Model();
                        mdlU.Import(FileName, resFileU);
                        mdlU.Name = Text;
                        BfresWiiU.ReadModel(this, mdlU);
                    }
                    else
                    {
                        Model mdl = new Model();
                        mdl.Import(FileName, resFileNX);
                        mdl.Name = Text;

                        Console.WriteLine(mdl.ShapeCount);
                        Console.WriteLine(mdl.MaterialCount);
                        Console.WriteLine(mdl.VertexBufferCount);
                        Console.WriteLine(mdl.Skeleton.Bones.Count);

                        BfresSwitch.ReadModel(this, mdl);
                    }
                    IsEdited = true;

                    Cursor.Current = Cursors.Default;
                    break;
                case ".csv":
                    CsvModel csvModel = new CsvModel();
                    csvModel.LoadFile(new System.IO.FileStream(FileName, System.IO.FileMode.Open), true);

                    if (csvModel.objects.Count == 0)
                    {
                        MessageBox.Show("No models found!");
                        return;
                    }
                    BfresModelImportSettings csvsettings = new BfresModelImportSettings();
                    csvsettings.DisableMaterialEdits();
                    csvsettings.SetModelAttributes(csvModel.objects[0]);
                    if (csvsettings.ShowDialog() == DialogResult.OK)
                    {
                        if (csvsettings.LimitSkinCount) {
                            for (int i = 0; i < csvModel.objects.Count; i++)
                            {
                                List<FSHP> Matches = shapes.Where(p => String.Equals(p.Text,
                                    csvModel.objects[i].ObjectName, StringComparison.CurrentCulture)).ToList();

                                if (Matches != null && Matches.Count > 0)
                                {
                                    //Match the skin count setting if names match
                                    //Only one match should be found as shapes can't have duped names
                                    csvModel.objects[i].VertexSkinCount = ((FSHP)Matches[0]).VertexSkinCount;
                                }
                                else
                                {
                                    //Else just match the first object
                                    csvModel.objects[i].VertexSkinCount = shapes[0].VertexSkinCount;
                                }
                            }
                        }

                        if (Replace)
                        {
                            shapes.Clear();
                            Nodes["FshpFolder"].Nodes.Clear();
                        }

                        Cursor.Current = Cursors.WaitCursor;

                        bool ForceSkinInfluence = csvsettings.LimitSkinCount;

                        foreach (STGenericObject obj in csvModel.objects)
                        {
                            int ForceSkinInfluenceMax = obj.VertexSkinCount;

                            FSHP shape = new FSHP();
                            Nodes["FshpFolder"].Nodes.Add(shape);
                            shapes.Add(shape);

                            shape.VertexBufferIndex = shapes.Count;
                            shape.vertices = obj.vertices;
                            shape.MaterialIndex = 0;
                            shape.vertexAttributes = csvsettings.CreateNewAttributes();
                            shape.BoneIndex = 0;
                            shape.Text = obj.ObjectName;
                            shape.lodMeshes = obj.lodMeshes;
                            shape.CreateNewBoundingBoxes();
                            shape.CreateBoneList(obj, this, ForceSkinInfluence, ForceSkinInfluenceMax);
                            shape.CreateIndexList(obj, this);
                            shape.ApplyImportSettings(csvsettings, GetMaterial(shape.MaterialIndex));
                            shape.BoneIndices = shape.GetIndices(Skeleton);

                            if (!ForceSkinInfluence)
                                shape.VertexSkinCount = obj.GetMaxSkinInfluenceCount();
                            else
                                shape.VertexSkinCount = (byte)ForceSkinInfluenceMax;


                            if (shape.VertexSkinCount == 1)
                            {
                                int boneIndex = shape.BoneIndices[0];
                                shape.BoneIndex = boneIndex;
                            }

                            shape.OptmizeAttributeFormats();
                            shape.SaveShape(IsWiiU);
                            shape.SaveVertexBuffer(IsWiiU);

                            if (IsWiiU)
                            {
                                shape.ShapeU.SubMeshBoundingIndices = new List<ushort>();
                                shape.ShapeU.SubMeshBoundingIndices.Add(0);
                                shape.ShapeU.SubMeshBoundingNodes = new List<ResU.BoundingNode>();
                                shape.ShapeU.SubMeshBoundingNodes.Add(new ResU.BoundingNode()
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
                        Cursor.Current = Cursors.Default;
                    }
                    IsEdited = true;

                    break;
                default:
                    AssimpData assimp = new AssimpData();
                    bool IsLoaded = assimp.LoadFile(FileName);

                    if (!IsLoaded)
                        return;

                    if (assimp.objects.Count == 0)
                    {
                        MessageBox.Show("No models found!");
                        return;
                    }

                    BfresModelImportSettings settings = new BfresModelImportSettings();

                    if (Parent != null)
                    {
                        bool HasTextures = ((BFRES)Parent.Parent).HasTextures;
                        settings.UpdateTexturePlaceholderSetting(HasTextures);
                    }


                    settings.SetModelAttributes(assimp.objects[0]);
                    if (settings.ShowDialog() == DialogResult.OK)
                    {
                        STProgressBar progressBar = new STProgressBar();
                        progressBar.Text = "Model Importing";
                        progressBar.Task = "Importing DAE...";
                        progressBar.Value = 0;
                        progressBar.StartPosition = FormStartPosition.CenterScreen;
                        progressBar.Show();
                        progressBar.Refresh();

                        bool UseMats = settings.ExternalMaterialPath != string.Empty;

                        if (settings.LimitSkinCount)
                        {
                            for (int i = 0; i < assimp.objects.Count; i++)
                            {
                                List<FSHP> Matches = shapes.Where(p => String.Equals(p.Name,
                                    assimp.objects[i].ObjectName, StringComparison.CurrentCulture)).ToList();

                                if (Matches != null && Matches.Count > 0)
                                {
                                    //Match the skin count setting if names match
                                    //Only one match should be found as shapes can't have duped names
                                    assimp.objects[i].VertexSkinCount = ((FSHP)Matches[0]).VertexSkinCount;
                                }
                                else
                                {
                                    //Else just match the first object
                                    assimp.objects[i].VertexSkinCount = shapes[0].VertexSkinCount;
                                }
                            }
                        }

                        if (Replace)
                        {
                            shapes.Clear();
                            Nodes["FshpFolder"].Nodes.Clear();
                        }

                        Cursor.Current = Cursors.WaitCursor;
                        if (Replace && UseMats)
                        {
                            materials.Clear();
                            Nodes["FmatFolder"].Nodes.Clear();
                            MatStartIndex = 0;
                        }
                        if (UseMats)
                        {
                            int curMat = 0;
                            foreach (STGenericMaterial mat in assimp.materials)
                            {
                                progressBar.Task = $"Generating material { mat.Text } {curMat} / {assimp.materials.Count}";
                                progressBar.Value = ((curMat++ * 100) / assimp.materials.Count);
                                progressBar.Refresh();

                                FMAT fmat = new FMAT();

                                if (IsWiiU)
                                {
                                    fmat.MaterialU = new ResU.Material();
                                    fmat.MaterialU.Import(settings.ExternalMaterialPath, resFileU);
                                    BfresWiiU.ReadMaterial(fmat, fmat.MaterialU);
                                }
                                else
                                {
                                    fmat.Material = new Material();
                                    fmat.Material.Import(settings.ExternalMaterialPath);
                                    fmat.ReadMaterial(fmat.Material);
                                }

                                fmat.Text = mat.Text;

                                if (settings.GeneratePlaceholderTextures)
                                {

                                    //Setup placeholder textures
                                    //Note we can't add/remove samplers so we must fill these slots
                                    foreach (var t in fmat.TextureMaps)
                                    {
                                        t.wrapModeS = 0;
                                        t.wrapModeT = 0;

                                        switch (t.Type)
                                        {
                                            case STGenericMatTexture.TextureType.Diffuse:
                                                t.Name = "Basic_Alb";
                                                break;
                                            case STGenericMatTexture.TextureType.Emission:
                                                t.Name = "Basic_Emm";
                                                break;
                                            case STGenericMatTexture.TextureType.Normal:
                                                t.Name = "Basic_Nrm";
                                                break;
                                            case STGenericMatTexture.TextureType.Specular:
                                                t.Name = "Basic_Spm";
                                                break;
                                            case STGenericMatTexture.TextureType.SphereMap:
                                                t.Name = "Basic_Sphere";
                                                break;
                                            case STGenericMatTexture.TextureType.Metalness:
                                                t.Name = "Basic_Mtl";
                                                break;
                                            case STGenericMatTexture.TextureType.Roughness:
                                                t.Name = "Basic_Rgh";
                                                break;
                                            case STGenericMatTexture.TextureType.MRA:
                                                t.Name = "Basic_MRA";
                                                break;
                                            case STGenericMatTexture.TextureType.Shadow:
                                                t.Name = "Basic_Bake_st0";
                                                break;
                                            case STGenericMatTexture.TextureType.Light:
                                                t.Name = "Basic_Bake_st1";
                                                break;
                                        }
                                    }

                                    if (PluginRuntime.bntxContainers.Count > 0 && Parent != null)
                                    {
                                        foreach (var node in Parent.Parent.Nodes)
                                        {
                                            if (node is BNTX)
                                            {
                                                var bntx = (BNTX)node;

                                                bntx.ImportBasicTextures("Basic_Alb");
                                                bntx.ImportBasicTextures("Basic_Nrm");
                                                bntx.ImportBasicTextures("Basic_Spm");
                                                bntx.ImportBasicTextures("Basic_Sphere");
                                                bntx.ImportBasicTextures("Basic_Mtl");
                                                bntx.ImportBasicTextures("Basic_Rgh");
                                                bntx.ImportBasicTextures("Basic_MRA");
                                                bntx.ImportBasicTextures("Basic_Bake_st0");
                                                bntx.ImportBasicTextures("Basic_Bake_st1");
                                                bntx.ImportBasicTextures("Basic_Emm");
                                            }
                                        }
                                    }
                                    if (PluginRuntime.ftexContainers.Count > 0 && Parent != null)
                                    {
                                        foreach (var node in Parent.Parent.Nodes)
                                        {
                                            if (node is BFRESGroupNode)
                                            {
                                                if (((BFRESGroupNode)node).Type == BRESGroupType.Textures)
                                                {
                                                    var ftexCont = (BFRESGroupNode)node;

                                                    ftexCont.ImportBasicTextures("Basic_Alb");
                                                    ftexCont.ImportBasicTextures("Basic_Nrm");
                                                    ftexCont.ImportBasicTextures("Basic_Spm");
                                                    ftexCont.ImportBasicTextures("Basic_Sphere");
                                                    ftexCont.ImportBasicTextures("Basic_Mtl");
                                                    ftexCont.ImportBasicTextures("Basic_Rgh");
                                                    ftexCont.ImportBasicTextures("Basic_MRA");
                                                    ftexCont.ImportBasicTextures("Basic_Bake_st0");
                                                    ftexCont.ImportBasicTextures("Basic_Bake_st1");
                                                    ftexCont.ImportBasicTextures("Basic_Emm");
                                                }
                                            }
                                        }
                                    }

                                }

                                foreach (var tex in mat.TextureMaps)
                                {
                                    foreach (var t in fmat.TextureMaps)
                                    {
                                        if (t.Type == tex.Type)
                                        {
                                            t.Name = tex.Name;
                                            t.wrapModeS = tex.wrapModeS;
                                            t.wrapModeT = tex.wrapModeT;
                                            t.wrapModeW = tex.wrapModeW;
                                            t.Type = tex.Type;
                                        }
                                    }
                                }

                                List<string> keyList = new List<string>(materials.Keys);
                                fmat.Text = Utils.RenameDuplicateString(keyList, fmat.Text);

                                if (IsWiiU)
                                {
                                    fmat.MaterialU.Name = Text;
                                    fmat.SetMaterial(fmat.MaterialU, resFileU);
                                }
                                else
                                {
                                    fmat.Material.Name = Text;
                                    fmat.SetMaterial(fmat.Material);
                                }

                                materials.Add(fmat.Text, fmat);
                                Nodes["FmatFolder"].Nodes.Add(fmat);
                            }

                        }

                        progressBar.Task = "Importing Bones... ";
                        progressBar.Refresh();

                        if (settings.ImportBones)
                        {
                            if (assimp.skeleton.bones.Count > 0)
                            {
                                Skeleton.bones.Clear();
                                Skeleton.node.Nodes.Clear();

                                if (IsWiiU)
                                    BfresWiiU.SaveSkeleton(Skeleton, assimp.skeleton.bones);
                                else
                                    BfresSwitch.SaveSkeleton(Skeleton, assimp.skeleton.bones);
                            }
                        }


                        //Genericate indices
                        //Check for rigged bones
                        for (int ob = 0; ob < assimp.objects.Count; ob++)
                        {
                            foreach (string NewBone in assimp.objects[ob].boneList)
                            {
                                foreach (var bones in Skeleton.bones)
                                {
                                    if (bones.Text == NewBone)
                                    {
                                        bones.SmoothMatrixIndex += 1;
                                    }
                                }
                            }
                        }

                        Skeleton.CalculateIndices();

                        if (materials.Count <= 0)
                        {
                            //Force material creation if there is none present
                            FMAT fmat = new FMAT();
                            fmat.Text = "NewMaterial";
                            materials.Add(fmat.Text, fmat);
                            Nodes["FmatFolder"].Nodes.Add(fmat);

                            if (IsWiiU)
                            {
                                fmat.MaterialU = new ResU.Material();
                                fmat.MaterialU.Name = "NewMaterial";
                                BfresWiiU.ReadMaterial(fmat, fmat.MaterialU);
                            }
                            else
                            {
                                fmat.Material = new Material();
                                fmat.Material.Name = "NewMaterial";
                                fmat.ReadMaterial(fmat.Material);
                            }
                        }

                        Console.WriteLine("Processing Data. Object count " + assimp.objects.Count);

                        bool ForceSkinInfluence = settings.LimitSkinCount;


                        int curShp = 0;
                        foreach (STGenericObject obj in assimp.objects)
                        {
                            int ForceSkinInfluenceMax = obj.VertexSkinCount;

                            if (obj.ObjectName == "")
                                obj.ObjectName = $"Mesh {curShp}";

                            progressBar.Task = $"Generating shape {obj.ObjectName} { curShp} / { assimp.materials.Count}";
                            progressBar.Value = ((curShp++ * 100) / assimp.materials.Count);
                            progressBar.Refresh();


                            FSHP shape = new FSHP();
                            List<string> keyList = shapes.Select(o => o.Text).ToList();
                            shape.Text = Utils.RenameDuplicateString(keyList, obj.ObjectName);

                            Nodes["FshpFolder"].Nodes.Add(shape);
                            shapes.Add(shape);

                            shape.VertexBufferIndex = shapes.Count;
                            shape.vertices = obj.vertices;

                            progressBar.Task = $"Creating Attributes. Mesh: {obj.ObjectName}";
                            progressBar.Refresh();

                            shape.vertexAttributes = settings.CreateNewAttributes();
                            shape.BoneIndex = obj.BoneIndex;

                            if (UseMats)
                                shape.MaterialIndex = obj.MaterialIndex + MatStartIndex;
                            else
                                shape.MaterialIndex = 0;

                            if (shape.MaterialIndex >= materials.Count)
                                shape.MaterialIndex = 0;

                            shape.lodMeshes = obj.lodMeshes;

                            progressBar.Task = $"Creating Bounding Boxes. Mesh: {obj.ObjectName}";
                            progressBar.Refresh();

                            shape.CreateNewBoundingBoxes();

                            progressBar.Task = $"Creating Bone list. Mesh: {obj.ObjectName}";
                            progressBar.Refresh();

                            shape.CreateBoneList(obj, this, ForceSkinInfluence, ForceSkinInfluenceMax);

                            progressBar.Task = $"Creating Index list. Mesh: {obj.ObjectName}";
                            progressBar.Refresh();

                            shape.CreateIndexList(obj, this, ForceSkinInfluence, ForceSkinInfluenceMax);

                            progressBar.Task = $"Applying Settings. Mesh: {obj.ObjectName}";
                            progressBar.Refresh();

                            shape.ApplyImportSettings(settings, GetMaterial(shape.MaterialIndex));

                            progressBar.Task = $"Setting bone indices. Mesh: {obj.ObjectName}";
                            progressBar.Refresh();

                            shape.BoneIndices = shape.GetIndices(Skeleton);

                            progressBar.Task = $"Generating Max Skin Influence. Mesh: {obj.ObjectName}";
                            progressBar.Refresh();

                            if (!ForceSkinInfluence)
                                shape.VertexSkinCount = obj.GetMaxSkinInfluenceCount();
                            else
                                shape.VertexSkinCount = (byte)ForceSkinInfluenceMax;


                            if (shape.VertexSkinCount == 1 && shape.BoneIndices.Count > 0)
                            {
                                int boneIndex = shape.BoneIndices[0];
                                shape.BoneIndex = boneIndex;
                            }

                            Console.WriteLine($"VertexSkinCount 1 {shape.VertexSkinCount}");

                            shape.OptmizeAttributeFormats();

                            progressBar.Task = $"Saving shape. Mesh: {obj.ObjectName}";
                            progressBar.Refresh();

                            shape.SaveShape(IsWiiU);

                            progressBar.Task = $"Saving vertex buffer. Mesh: {obj.ObjectName}";
                            progressBar.Refresh();

                            shape.SaveVertexBuffer(IsWiiU);

                            if (IsWiiU)
                            {
                                shape.ShapeU.SubMeshBoundingIndices = new List<ushort>();
                                shape.ShapeU.SubMeshBoundingIndices.Add(0);
                                shape.ShapeU.SubMeshBoundingNodes = new List<ResU.BoundingNode>();
                                shape.ShapeU.SubMeshBoundingNodes.Add(new ResU.BoundingNode()
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

                        progressBar.Value = 100;
                        progressBar.Close();

                        IsEdited = true;

                        Cursor.Current = Cursors.Default;
                    }
                    break;
            }

            Console.WriteLine("Updating Vertex Data");

            if (IsEdited)
                UpdateVertexData();
        }

        public FMAT GetMaterial(int index)
        {
            return materials.Values.ElementAt(index);
        }

        public void AddMaterials(string FileName, bool Replace = true)
        {
            string ext = System.IO.Path.GetExtension(FileName);
            ext = ext.ToLower();

            switch (ext)
            {
                case ".bfmat":
                    Cursor.Current = Cursors.WaitCursor;
                    if (Replace)
                    {
                        materials.Clear();
                        Nodes["FmatFolder"].Nodes.Clear();
                    }

                    FMAT mat = new FMAT();

                    List<string> keyList = new List<string>(this.materials.Keys);

                    if (GetResFileU() != null)
                    {
                        mat.MaterialU = new ResU.Material();
                        mat.MaterialU.Import(FileName, GetResFileU());
                        mat.MaterialU.Name = Utils.RenameDuplicateString(keyList, mat.MaterialU.Name);
                        mat.Text = mat.MaterialU.Name;

                        mat.ReadMaterial(mat.MaterialU);
                    }
                    else
                    {
                        mat.Material = new Material();
                        mat.Material.Import(FileName);
                        mat.Material.Name = Utils.RenameDuplicateString(keyList, mat.Material.Name);
                        mat.Text = mat.Material.Name;

                        mat.ReadMaterial(mat.Material);
                    }

                    keyList.Clear();


                    materials.Add(mat.Text, mat);
                    Nodes["FmatFolder"].Nodes.Add(mat);
                    break;
            }
        }

        public override void OnClick(TreeView treeView)
        {
            UpdateEditor();
        }

        public void UpdateEditor(){
            if (Parent != null)
                ((BFRES)Parent.Parent).LoadEditors(this);
        }

        private void CreateSkeleton()
        {

        }
        private void CreateBones(STBone bone)
        {
            Bone bn = new Bone();
            bn.BillboardIndex = (short)bone.BillboardIndex;
            bn.Flags = BoneFlags.Visible;
            bn.FlagsRotation = BoneFlagsRotation.EulerXYZ;
            bn.FlagsTransform = BoneFlagsTransform.None;
            bn.FlagsTransformCumulative = BoneFlagsTransformCumulative.None;
            bn.Name = bone.Text;
            bn.RigidMatrixIndex = 0;
            bn.Rotation = new Syroot.Maths.Vector4F(bone.rotation[0],
                bone.rotation[1], bone.rotation[2], bone.rotation[3]);
            bn.Position = new Syroot.Maths.Vector3F(bone.position[0],
                bone.position[1], bone.position[2]);
            bn.Scale = new Syroot.Maths.Vector3F(bone.scale[0],
               bone.scale[1], bone.scale[2]);
            bn.UserData = new List<UserData>();
            bn.UserDataDict = new ResDict();
        }

        public FSKL Skeleton
        {
            get
            {
                return skeleton;
            }
            set
            {
                skeleton = value;
            }
        }
        private FSKL skeleton = new FSKL();
    }
}
