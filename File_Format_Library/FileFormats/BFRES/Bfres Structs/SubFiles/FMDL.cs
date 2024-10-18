using System;
using System.Collections.Generic;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.NodeWrappers;
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
    public class FMDL : STGenericModel, IContextMenuNode
    {
        //These get updated on UpdateVertexData()
        public Vector3 MaxPosition = new Vector3(0);
        public Vector3 MinPosition = new Vector3(0);

        public bool IsEdited { get; set; }

        public List<FSHP> shapes = new List<FSHP>();
        public List<FSHP> depthSortedMeshes = new List<FSHP>();

        public override IEnumerable<STGenericObject> Objects => shapes;
        public override IEnumerable<STGenericMaterial> Materials => materials.Values.ToList();

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

        public BFRESRenderBase GetRenderer()
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
            IsFolder = false;

            Nodes.Add(new FSHPFolder());
            Nodes.Add(new FMATFolder());
            Nodes.Add(new FSKL.fsklNode());
        }

        public override ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.AddRange(base.GetContextMenuItems());
            Items.Add(new ToolStripMenuItem("Transform", null, TransformToolAction, Keys.Control | Keys.T));
            Items.Add(new ToolStripMenuItem("Calculate Tangents/Bitangents", null, CalcTansBitansAllShapesAction, Keys.Control | Keys.C));
            Items.Add(new ToolStripMenuItem("Normals", null,
             new ToolStripMenuItem("Smooth (Multiple Meshes)", null, MultiMeshSmoothNormals),
             new ToolStripMenuItem("Smooth", null, SmoothNormalsAction),
             new ToolStripMenuItem("Recalculate", null, RecalculateNormalsAction)
            ));

            Items.Add(new ToolStripMenuItem("UVs", null,
              new ToolStripMenuItem("Flip Vertical", null, FlipUvsVerticalAction),
              new ToolStripMenuItem("Flip Horizontal", null, FlipUvsHorizontalAction),
              new ToolStripMenuItem("Copy UV Channel", null, CopyUVChannels)
            ));

            Items.Add(new ToolStripMenuItem("Colors", null,
              new ToolStripMenuItem("  Set Color", null, SetVertexColorDialogAction),
              new ToolStripMenuItem("Set White Color", null, SetVertexColorWhiteAction)
            ));
            return Items.ToArray();
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
                Skeleton.bones.Clear();
                ((BFRES)Parent.Parent).DrawableContainer.Drawables.Remove(Skeleton);

                shapes.Clear();
                materials.Clear();

                UpdateVertexData();

                Nodes.Clear();
                ((BFRESGroupNode)Parent).RemoveChild(this);

                LibraryGUI.UpdateViewport();

                Unload();
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

        public FSHP GetShape(string Name)
        {
            for (int fshp = 0; fshp < shapes.Count; fshp++)
            {
                if (shapes[fshp].Text == Name)
                    return shapes[fshp];
            }

            return null;
        }

        public void FlipUvsVertical()
        {
            foreach (var shape in shapes)
            {
                if (!shape.HasAttributeUV0())
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
                if (!shape.HasAttributeUV0())
                {
                    MessageBox.Show($"Error! {Text} does not have UVs!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                shape.FlipUvsHorizontal();
                shape.SaveVertexBuffer(GetResFileU() != null);
            }

            UpdateVertexData();
        }

        private void MultiMeshSmoothNormals(object sender, EventArgs args)
        {
            SmoothNormalsMultiMeshForm form = new SmoothNormalsMultiMeshForm();
            form.LoadMeshes(GetModelList());
            if (form.ShowDialog() == DialogResult.OK)
            {
                var SelectedMeshes = form.GetSelectedMeshes();

                Cursor.Current = Cursors.WaitCursor;
                STGenericObject.SmoothNormals(SelectedMeshes);
                Cursor.Current = Cursors.Default;

                foreach (var shp in shapes)
                    shp.SaveVertexBuffer(GetResFileU() != null);

                UpdateVertexData();
            }
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

                if (!shp.HasAttributeUV0())
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

                int UseUVIndex = 0;

                //for BOTW if it uses UV layer 2 for normal maps use second UV map
                if (shp.GetFMAT().shaderassign.options.ContainsKey("uking_texture2_texcoord"))
                {
                    float value = float.Parse(shp.GetFMAT().shaderassign.options["uking_texture2_texcoord"]);

                    if (value == 1)
                        UseUVIndex = 1;
                }

                //for TOTK use o_texture2_texcoord to find required uv layer for tangents
                if (shp.GetFMAT().shaderassign.options.ContainsKey("o_texture2_texcoord"))
                {
                    UseUVIndex = int.TryParse(shp.GetFMAT().shaderassign.options["o_texture2_texcoord"], out UseUVIndex) ? UseUVIndex : 0;
                }

                shp.CalculateTangentBitangent(UseUVIndex);
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
                LibraryGUI.UpdateViewport();
            }
        }
        private void SetCopiedMaterialData(CopyMaterialMenu menu,
            FMAT selectedMaterial, FMAT targetMaterial)
        {
            if (targetMaterial.Material != null)
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
            else
            {
                targetMaterial.MaterialU.Flags = selectedMaterial.MaterialU.Flags;
                targetMaterial.MaterialU.UserData = selectedMaterial.MaterialU.UserData;

                if (menu.chkBoxRenderInfo.Checked)
                {
                    targetMaterial.MaterialU.RenderState = selectedMaterial.MaterialU.RenderState;
                    targetMaterial.MaterialU.RenderInfos = selectedMaterial.MaterialU.RenderInfos;
                }
                if (menu.chkBoxShaderOptions.Checked)
                {
                    targetMaterial.MaterialU.ShaderAssign = selectedMaterial.MaterialU.ShaderAssign;
                }
                if (menu.chkBoxShaderParams.Checked)
                {
                    targetMaterial.MaterialU.ShaderParamData = selectedMaterial.MaterialU.ShaderParamData;
                    targetMaterial.MaterialU.ShaderParams = selectedMaterial.MaterialU.ShaderParams;
                    targetMaterial.MaterialU.VolatileFlags = selectedMaterial.MaterialU.VolatileFlags;
                }
                if (menu.chkBoxTextures.Checked)
                {
                    targetMaterial.MaterialU.Samplers = selectedMaterial.MaterialU.Samplers;
                    targetMaterial.MaterialU.TextureRefs = selectedMaterial.MaterialU.TextureRefs;
                }
                targetMaterial.ReadMaterial(targetMaterial.MaterialU);
            }
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
                foreach (var shape in shapes)
                {
                    shape.SaveVertexBuffer(GetResFileU() != null);
                }
                UpdateVertexData();
            }
        }

        public override void Unload()
        {
            if (Parent != null)
                ((BFRES)Parent.Parent).DrawableContainer.Drawables.Remove(Skeleton);

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

            if (ModelU != null)
                BfresWiiU.SetModel(this);
            else
                BfresSwitch.SetModel(this);

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

                    ExportModelSettings settings = new ExportModelSettings();
                    if (settings.ShowDialog() == DialogResult.OK)
                        DAE.Export(FileName, settings.Settings, this, GetTextures(),
                            Skeleton, Skeleton.Node_Array.ToList());

                    break;
            }
        }

        private List<STGenericTexture> GetTextures()
        {
            List<string> textureRefs = new List<string>();
            foreach (var mat in materials) {
                foreach (var texref in mat.Value.TextureMaps) {
                    if (!textureRefs.Contains(texref.Name)) {
                        textureRefs.Add(texref.Name);
                        if (texref.Name.EndsWith(".0"))
                        {
                            for (int i = 1; i < 100; i++)
                                textureRefs.Add(texref.Name.Replace(".0", $".{i}"));
                        }
                    }
                }
            }

            List<STGenericTexture> textures = new List<STGenericTexture>();
            foreach (var texref in textureRefs)
            {
                foreach (var bntx in PluginRuntime.bntxContainers)
                {
                    if (bntx.Textures.ContainsKey(texref))
                        textures.Add(bntx.Textures[texref]);
                }
                foreach (var ftexCont in PluginRuntime.ftexContainers)
                {
                    if (ftexCont.ResourceNodes.ContainsKey(texref))
                        textures.Add((FTEX)ftexCont.ResourceNodes[texref]);
                }
                if (PluginRuntime.TextureCache.ContainsKey(texref))
                    textures.Add(PluginRuntime.TextureCache[texref]);
            }

            return textures;
        }

        public override void Replace(string FileName)
        {
            AddOjects(FileName, GetResFile(), GetResFileU());
        }

        public void Replace(string FileName, ResFile resFileNX, ResU.ResFile resFileU)
        {
            AddOjects(FileName, resFileNX, resFileU);
        }

        //Function addes shapes, vertices and meshes
        public void AddOjects(string FileName, ResFile resFileNX, ResU.ResFile resFileU, bool Replace = true)
        {
            //If using original attributes, this to look them up
            Dictionary<string, List<FSHP.VertexAttribute>> AttributeMatcher = new Dictionary<string, List<FSHP.VertexAttribute>>();

            bool IsWiiU = (resFileU != null);
            var boneMappings = Model != null ? Model.Skeleton.userIndices : new ushort[0];

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
                    csvsettings.SetModelAttributes(csvModel.objects);
                    if (csvsettings.ShowDialog() == DialogResult.OK)
                    {
                        if (csvsettings.LimitSkinCount ||
                            csvsettings.MapOriginalMaterials ||
                            csvsettings.UseOriginalAttributes)
                        {
                            for (int i = 0; i < csvModel.objects.Count; i++)
                            {
                                //Only one match should be found as shapes can't have duped names
                                FSHP match = shapes.Where(p => string.Equals(p.Text,
                                    csvModel.objects[i].ObjectName, StringComparison.CurrentCulture)).FirstOrDefault();

                                if (csvsettings.LimitSkinCount)
                                {
                                    //Match the skin count setting if names match
                                    //Else just match the first object
                                    csvModel.objects[i].VertexSkinCount = (match ?? shapes.First()).VertexSkinCount;
                                }

                                if (csvsettings.MapOriginalMaterials && match != null)
                                {
                                    csvModel.objects[i].MaterialIndex = match.MaterialIndex;
                                }

                                if (csvsettings.UseOriginalAttributes && match != null)
                                {
                                    AttributeMatcher.Add(csvModel.objects[i].ObjectName, match.vertexAttributes);
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
                            if (obj.vertices.Count <= 0)
                            {
                                STConsole.WriteLine($"0 Vertices found on mesh {obj.ObjectName}! Will add an empty mesh to prevent issues.", 0);

                                FSHP emptyShape = new FSHP();
                                emptyShape.Text = obj.ObjectName;
                                if (obj.MaterialIndex < materials.Count)
                                    emptyShape.MaterialIndex = obj.MaterialIndex;

                                FSHPFolder.CreateEmptyMesh(this, emptyShape);
                                continue;
                            }

                            int ForceSkinInfluenceMax = obj.VertexSkinCount;

                            FSHP shape = new FSHP();
                            Nodes["FshpFolder"].Nodes.Add(shape);
                            shapes.Add(shape);

                            shape.VertexBufferIndex = shapes.Count;
                            shape.vertices = obj.vertices;
                            shape.MaterialIndex = obj.MaterialIndex;

                            if (AttributeMatcher.ContainsKey(obj.ObjectName))
                                shape.vertexAttributes = csvsettings.CreateNewAttributes(AttributeMatcher[obj.ObjectName]);
                            else
                                shape.vertexAttributes = csvsettings.CreateNewAttributes(GetMaterial(shape.MaterialIndex));

                            shape.BoneIndex = 0;
                            shape.Text = obj.ObjectName;
                            shape.lodMeshes = obj.lodMeshes;
                            shape.CreateBoneList(obj, this, ForceSkinInfluence, ForceSkinInfluenceMax);
                            shape.CreateIndexList(obj, this);
                            shape.ApplyImportSettings(csvsettings, GetMaterial(shape.MaterialIndex));
                            shape.BoneIndices = shape.GetIndices(Skeleton);

                            if (csvsettings.CreateDummyLODs)
                                shape.GenerateDummyLODMeshes();

                            Console.WriteLine($"ForceSkinInfluence {ForceSkinInfluence}");

                            if (!ForceSkinInfluence)
                                shape.VertexSkinCount = obj.GetMaxSkinInfluenceCount();
                            else
                                shape.VertexSkinCount = (byte)ForceSkinInfluenceMax;

                            Console.WriteLine($"VertexSkinCount { shape.VertexSkinCount}");

                            if (shape.VertexSkinCount == 1)
                            {
                                int boneIndex = shape.BoneIndices[0];
                                shape.BoneIndex = boneIndex;
                            }

                            shape.CreateNewBoundingBoxes(this);
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
                    List<STGenericObject> ImportedObjects = new List<STGenericObject>();
                    List<STGenericMaterial> ImportedMaterials = new List<STGenericMaterial>();
                    STSkeleton ImportedSkeleton = new STSkeleton();

                    if (ext == ".semodel")
                    {
                        SEModel seModel = new SEModel();
                        seModel.CreateGenericModel(FileName);
                        ImportedObjects = seModel.objects;
                        ImportedMaterials = seModel.materials;
                        ImportedSkeleton = seModel.skeleton;
                    }
                    else
                    {
                        AssimpData assimp = new AssimpData();
                        bool IsLoaded = assimp.LoadFile(FileName);

                        if (!IsLoaded)
                            return;

                        ImportedObjects = assimp.objects;
                        ImportedMaterials = assimp.materials;
                        ImportedSkeleton = assimp.skeleton;
                    }


                    string[] shapeSortCheck = shapes.Select(o => o.Text).ToArray();
                    //  assimp.objects = assimp.objects.SortBy(shapeSortCheck, c => c.ObjectName).ToList();

                    if (ImportedObjects.Count == 0)
                    {
                        MessageBox.Show("No models found!");
                        return;
                    }

                    BfresModelImportSettings settings = new BfresModelImportSettings();
                    settings.LoadOriginalMeshData(shapes);
                    settings.LoadNewMeshData(ImportedObjects);

                    if (Parent != null)
                    {
                        bool HasTextures = ((BFRES)Parent.Parent).HasTextures;
                        settings.UpdateTexturePlaceholderSetting(HasTextures);
                    }

                    settings.SetModelAttributes(ImportedObjects);
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

                        for (int i = 0; i < ImportedObjects.Count; i++)
                        {
                            List<FSHP> Matches = shapes.Where(p => String.Equals(p.Text,
                            ImportedObjects[i].ObjectName, StringComparison.CurrentCulture)).ToList();
                            ImportedObjects[i].BoneIndex = 0;

                            if (Matches != null && Matches.Count > 0)
                            {
                                //Match the skin count setting if names match
                                //Only one match should be found as shapes can't have duped names

                                if (settings.MapOriginalMaterials)
                                    ImportedObjects[i].MaterialIndex = ((FSHP)Matches[0]).MaterialIndex;

                                if (settings.LimitSkinCount)
                                    ImportedObjects[i].VertexSkinCount = ((FSHP)Matches[0]).VertexSkinCount;

                                //Keep original bone mapping by default
                                //Only do this for original boneset for now
                                if (!settings.ImportBones)
                                    ImportedObjects[i].BoneIndex = ((FSHP)Matches[0]).BoneIndex;

                                if (settings.UseOriginalAttributes)
                                {
                                    AttributeMatcher.Add(ImportedObjects[i].ObjectName, Matches[0].vertexAttributes);
                                }
                            }
                        }

                        if (settings.LimitSkinCount)
                        {
                            for (int i = 0; i < ImportedObjects.Count; i++)
                            {
                                List<FSHP> Matches = shapes.Where(p => String.Equals(p.Text,
                                ImportedObjects[i].ObjectName, StringComparison.CurrentCulture)).ToList();

                                if (Matches != null && Matches.Count > 0)
                                {
                                    //Match the skin count setting if names match
                                    //Only one match should be found as shapes can't have duped names
                                    ImportedObjects[i].VertexSkinCount = ((FSHP)Matches[0]).VertexSkinCount;
                                }
                                else
                                {
                                    //Else just match the first object
                                    ImportedObjects[i].VertexSkinCount = shapes[0].VertexSkinCount;
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
                        else if (UseMats)
                            MatStartIndex = materials.Count;
                        else
                            MatStartIndex = 0;

                        if (UseMats)
                        {
                            int curMat = 0;
                            foreach (STGenericMaterial mat in ImportedMaterials)
                            {
                                progressBar.Task = $"Generating material { mat.Text } {curMat} / {ImportedMaterials.Count}";
                                progressBar.Value = ((curMat++ * 100) / ImportedMaterials.Count);
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
                                        t.WrapModeS = STTextureWrapMode.Repeat;
                                        t.WrapModeT = STTextureWrapMode.Repeat;

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
                                            t.WrapModeS = tex.WrapModeS;
                                            t.WrapModeT = tex.WrapModeT;
                                            t.WrapModeW = tex.WrapModeW;
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
                            if (ImportedSkeleton.bones.Count > 0)
                            {
                                Skeleton.bones.Clear();
                                Skeleton.node.Nodes.Clear();

                                if (IsWiiU)
                                    BfresWiiU.SaveSkeleton(Skeleton, ImportedSkeleton.bones);
                                else
                                    BfresSwitch.SaveSkeleton(Skeleton, ImportedSkeleton.bones);
                            }
                        }

                        //Genericate indices
                        //Check for rigged bones
                        for (int ob = 0; ob < ImportedObjects.Count; ob++)
                        {
                            foreach (string NewBone in ImportedObjects[ob].boneList) {
                                foreach (var bones in Skeleton.bones) {
                                    if (bones.Text == NewBone)
                                    {
                                        bones.SmoothMatrixIndex += 1;
                                    }
                                }
                            }
                        }

                        List<int> smoothSkinningIndices = new List<int>();
                        List<int> rigidSkinningIndices = new List<int>();

                        foreach (BfresBone bone in Skeleton.bones)
                        {
                            bone.SmoothMatrixIndex = -1;
                            bone.RigidMatrixIndex = -1;
                            if (bone.BoneU != null)
                            {
                                bone.BoneU.SmoothMatrixIndex = -1;
                                bone.BoneU.RigidMatrixIndex = -1;
                            }
                            if (bone.Bone != null)
                            {
                                bone.Bone.SmoothMatrixIndex = -1;
                                bone.Bone.RigidMatrixIndex = -1;
                            }
                        }

                        //Determine the rigid and smooth bone skinning
                        foreach (var mesh in ImportedObjects)
                        {
                            int numSkinning = 0;
                            if (settings.LimitSkinCount)
                                numSkinning = (byte)mesh.VertexSkinCount;
                            else
                                numSkinning = mesh.vertices.Max(t => t.boneNames.Count);

                            //First create index lists for all the rigid and smooth skinning bone indices
                            foreach (var vertex in mesh.vertices) {
                                foreach (var bone in vertex.boneNames) {
                                    var bn = Skeleton.bones.Where(x => x.Text == bone).FirstOrDefault();
                                    if (bn != null)
                                    {
                                        int index = Skeleton.bones.IndexOf(bn);

                                        //Rigid skinning
                                        if (numSkinning == 1)
                                        {
                                            if (!rigidSkinningIndices.Contains(index))
                                                rigidSkinningIndices.Add(index);
                                        }
                                        else
                                        {
                                            if (!smoothSkinningIndices.Contains(index))
                                                smoothSkinningIndices.Add(index);
                                        }
                                    }
                                }
                            }
                        }

                        //Combine these lists into one global list
                        smoothSkinningIndices.Sort();
                        rigidSkinningIndices.Sort();

                        List<int> skinningIndices = new List<int>();
                        skinningIndices.AddRange(smoothSkinningIndices);
                        skinningIndices.AddRange(rigidSkinningIndices);
                        Skeleton.Node_Array = skinningIndices.ToArray();

                        //Next update the bone's skinning index value
                        foreach (var index in smoothSkinningIndices) {
                            var bone = Skeleton.bones[index];
                            bone.SmoothMatrixIndex = (short)smoothSkinningIndices.IndexOf(index);
                        }
                        //Rigid indices go after smooth indices
                        //Here we do not index the global iist as the global list can include the same index in both smooth/rigid
                        foreach (var index in rigidSkinningIndices) {
                            var bone = Skeleton.bones[index];
                            bone.RigidMatrixIndex = (short)(smoothSkinningIndices.Count + rigidSkinningIndices.IndexOf(index));
                        }

                        //Update all the bfres data directly from the bones
                        foreach (BfresBone bn in skeleton.bones)
                        {
                            if (bn.BoneU != null)
                            {
                                bn.BoneU.SmoothMatrixIndex = bn.SmoothMatrixIndex;
                                bn.BoneU.RigidMatrixIndex = bn.RigidMatrixIndex;
                            }
                            if (bn.Bone != null)
                            {
                                bn.Bone.SmoothMatrixIndex = bn.SmoothMatrixIndex;
                                bn.Bone.RigidMatrixIndex = bn.RigidMatrixIndex;
                            }
                        }

                        if (Skeleton.node.SkeletonU != null)
                        {
                            Skeleton.node.SkeletonU.MatrixToBoneList = new List<ushort>();
                            for (int i = 0; i < skinningIndices.Count; i++)
                                Skeleton.node.SkeletonU.MatrixToBoneList.Add((ushort)skinningIndices[i]);
                        }
                        else
                        {
                            Skeleton.node.Skeleton.MatrixToBoneList = new List<ushort>();
                            for (int i = 0; i < skinningIndices.Count; i++)
                                Skeleton.node.Skeleton.MatrixToBoneList.Add((ushort)skinningIndices[i]);
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

                        Console.WriteLine("Processing Data. Object count " + ImportedObjects.Count);

                        bool ForceSkinInfluence = settings.LimitSkinCount;


                        int curShp = 0;
                        foreach (STGenericObject obj in ImportedObjects)
                        {
                            if (obj.vertices.Count <= 0)
                            {
                                STConsole.WriteLine($"0 Vertices found on mesh {obj.ObjectName}! Will add an empty mesh to prevent issues.", 0);

                                FSHP emptyShape = new FSHP();
                                emptyShape.Text = obj.ObjectName;
                                if (obj.MaterialIndex < materials.Count)
                                    emptyShape.MaterialIndex = obj.MaterialIndex;

                                FSHPFolder.CreateEmptyMesh(this, emptyShape);
                                continue;
                            }

                            int ForceSkinInfluenceMax = obj.VertexSkinCount;

                            if (obj.ObjectName == "")
                                obj.ObjectName = $"Mesh {curShp}";

                            progressBar.Task = $"Generating shape {obj.ObjectName} { curShp} / { ImportedObjects.Count}";
                            progressBar.Value = ((curShp++ * 100) / ImportedObjects.Count);
                            progressBar.Refresh();


                            FSHP shape = new FSHP();
                            List<string> keyList = shapes.Select(o => o.Text).ToList();
                            shape.Text = Utils.RenameDuplicateString(keyList, obj.ObjectName);

                            Nodes["FshpFolder"].Nodes.Add(shape);
                            shapes.Add(shape);

                            shape.VertexBufferIndex = shapes.Count;
                            shape.vertices = obj.vertices;
                            shape.BoneIndex = obj.BoneIndex;

                            if (obj.MaterialIndex + MatStartIndex < materials.Count && obj.MaterialIndex > 0)
                                shape.MaterialIndex = obj.MaterialIndex + MatStartIndex;
                            else
                                shape.MaterialIndex = 0;

                            if (AttributeMatcher.ContainsKey(obj.ObjectName))
                                shape.vertexAttributes = settings.CreateNewAttributes(AttributeMatcher[obj.ObjectName]);
                            else
                                shape.vertexAttributes = settings.CreateNewAttributes(GetMaterial(shape.MaterialIndex));

                            shape.lodMeshes = obj.lodMeshes;
                            shape.CreateBoneList(obj, this, ForceSkinInfluence, ForceSkinInfluenceMax);
                            shape.CreateIndexList(obj, this, ForceSkinInfluence, ForceSkinInfluenceMax);
                            shape.ApplyImportSettings(settings, GetMaterial(shape.MaterialIndex));
                            shape.BoneIndices = shape.GetIndices(Skeleton);

                            if (settings.CreateDummyLODs)
                                shape.GenerateDummyLODMeshes(settings.DummyLODCount);

                            if (ForceSkinInfluence)
                                shape.VertexSkinCount = (byte)ForceSkinInfluenceMax;
                            else
                                shape.VertexSkinCount = obj.GetMaxSkinInfluenceCount();

                            shape.CreateNewBoundingBoxes(this);
                            shape.OptmizeAttributeFormats();
                            shape.SaveShape(IsWiiU);
                            shape.SaveVertexBuffer(IsWiiU);

                            if (shape.Shape != null)
                                shape.Shape.BoneIndex = (ushort)shape.BoneIndex;

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

                        AttributeMatcher.Clear();

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

            if (Model != null)
                Model.Skeleton.userIndices = boneMappings;
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

        public void UpdateEditor()
        {
            ((BFRES)Parent?.Parent)?.LoadEditors(this);
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
            bn.Rotation = new Syroot.Maths.Vector4F(
                       bone.Rotation.X,
                       bone.Rotation.Y,
                       bone.Rotation.Z,
                       bone.Rotation.W);
            bn.Position = new Syroot.Maths.Vector3F(
                bone.Position.X,
                bone.Position.Y,
                bone.Position.Z);
            bn.Scale = new Syroot.Maths.Vector3F(
                bone.Scale.X,
                bone.Scale.Y,
                bone.Scale.Z);
            bn.UserData = new List<UserData>();
            bn.UserDataDict = new ResDict();
        }

        public override STSkeleton GenericSkeleton => Skeleton;

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