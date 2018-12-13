using System;
using System.Collections.Generic;
using System.Linq;
using Syroot.NintenTools.NSW.Bfres;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;
using Switch_Toolbox.Library.Forms;
using ResU = Syroot.NintenTools.Bfres;
using ResUGX2 = Syroot.NintenTools.Bfres.GX2;
using ResGFX = Syroot.NintenTools.NSW.Bfres.GFX;
using FirstPlugin;

namespace Bfres.Structs
{
    public class FmdlFolder : TreeNodeCustom
    {
        public FmdlFolder()
        {
            Text = "Models";
            Name = "FMDL";

            ContextMenu = new ContextMenu();
            MenuItem import = new MenuItem("Import");
            ContextMenu.MenuItems.Add(import);
            import.Click += Import;
            MenuItem exportAll = new MenuItem("Export All");
            ContextMenu.MenuItems.Add(exportAll);
            exportAll.Click += ExportAll;
            MenuItem clear = new MenuItem("Clear");
            ContextMenu.MenuItems.Add(clear);
            clear.Click += Clear;
        }
        public void Import(object sender, EventArgs args)
        {

        }
        public void ExportAll(object sender, EventArgs args)
        {

        }
        private void Clear(object sender, EventArgs args)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to remove all objects? This cannot be undone!", "", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                Nodes.Clear();
                ((BFRES)Parent).BFRESRender.models.Clear();
                ((BFRES)Parent).BFRESRender.UpdateVertexData();
            }
        }
        public override void OnClick(TreeView treeView)
        {
            FormLoader.LoadEditor(this, Text);
        }
    }

    public class FMDL : STGenericModel
    {
        public List<FSHP> shapes = new List<FSHP>();
        public Dictionary<string, FMAT> materials = new Dictionary<string, FMAT>();
        public Model Model;
        public ResU.Model ModelU;

        public ResFile GetResFile()
        {
            //ResourceFile -> FMDL -> Material Folder -> this
            return ((BFRES)Parent.Parent).resFile;
        }
        public ResU.ResFile GetResFileU()
        {
            return ((BFRES)Parent.Parent).resFileU;
        }
        public void UpdateVertexData()
        {
            ((BFRES)Parent.Parent).BFRESRender.UpdateVertexData();
        }
        public List<FMDL> GetModelList()
        {
            return ((BFRES)Parent.Parent).BFRESRender.models;
        }

        public bool IsWiiU
        {
            get
            {
                return GetResFileU() != null;
            }
        }


        public FMDL()
        {
            ImageKey = "model";
            SelectedImageKey = "model";

            Nodes.Add(new FSHPFolder());
            Nodes.Add(new FMATFolder());

            ContextMenu = new ContextMenu();
            MenuItem export = new MenuItem("Export Model");
            ContextMenu.MenuItems.Add(export);
            export.Click += Export;
            MenuItem replace = new MenuItem("Replace Model");
            ContextMenu.MenuItems.Add(replace);
            replace.Click += Replace;
            MenuItem rename = new MenuItem("Rename");
            ContextMenu.MenuItems.Add(rename);
            rename.Click += Rename;
            MenuItem calcTansBitans = new MenuItem("Calculate Tangents/Bitangents");
            ContextMenu.MenuItems.Add(calcTansBitans);
            calcTansBitans.Click += CalcTansBitansAllShapes;
            MenuItem normals = new MenuItem("Normals");
            ContextMenu.MenuItems.Add(normals);
            MenuItem smoothNormals = new MenuItem("Smooth");
            normals.MenuItems.Add(smoothNormals);
            smoothNormals.Click += SmoothNormals;
            MenuItem recalculateNormals = new MenuItem("Recalculate");
            normals.MenuItems.Add(recalculateNormals);
            recalculateNormals.Click += RecalculateNormals;
        }
        private void SmoothNormals(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            foreach (FSHP shp in shapes)
            {
                bool HasNormals = shp.vertexAttributes.Any(x => x.Name == "_n0");
                if (HasNormals)
                    shp.SmoothNormals();

                shp.SaveVertexBuffer(IsWiiU);
            }
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
        }
        private void RecalculateNormals(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            foreach (FSHP shp in shapes)
            {
                bool HasNormals = shp.vertexAttributes.Any(x => x.Name == "_n0");
                if (HasNormals)
                    shp.CalculateNormals();

                shp.SaveVertexBuffer(IsWiiU);
            }
            UpdateVertexData();
            Cursor.Current = Cursors.Default;
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
        private void CalcTansBitansAllShapes(object sender, EventArgs args)
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

                shp.CalculateTangentBitangent();
                shp.SaveVertexBuffer(IsWiiU);
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
                Viewport.Instance.UpdateViewport();
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
        public void ExportAll()
        {
            FolderSelectDialog sfd = new FolderSelectDialog();

            List<string> Formats = new List<string>();
            Formats.Add("Bfres object (.bfobj)");
            Formats.Add("CSV (.csv)");

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string folderPath = sfd.SelectedPath;

                TextureFormatExport form = new TextureFormatExport(Formats);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    foreach (FSHP shp in shapes)
                    {
                        if (form.Index == 0)
                            shp.ExportBinaryObject(folderPath + '\\' + shp.Text + ".bfobj");
                    }
                }
            }
        }
        public void Export(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.bfmdl;*.fbx;*.dae; *.obj;*.csv;|" +
             "Bfres Model|*.bfmdl|" +
             "FBX |*.fbx|" +
             "DAE |*.dae|" +
             "OBJ |*.obj|" +
             "CSV |*.csv|" +
             "All files(*.*)|*.*";
            sfd.DefaultExt = ".bfobj";
            sfd.FileName = Text;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string ext = System.IO.Path.GetExtension(sfd.FileName);
                ext = ext.ToLower();

                switch (ext)
                {
                    case ".bfmdl":
                        if (GetResFileU() != null)
                        {

                        }
                        else
                            Model.Export(sfd.FileName, GetResFile());
                        break;
                    case ".csv":
                        CsvModel csv = new CsvModel();
                        foreach (FSHP shape in shapes)
                        {
                            STGenericObject obj = new STGenericObject();
                            obj.ObjectName = shape.Text;
                            obj.vertices = shape.vertices;
                            obj.faces = shape.lodMeshes[shape.DisplayLODIndex].faces;
                            csv.objects.Add(obj);

                            int CurVtx = 0;
                            foreach (Vertex v in shape.vertices)
                            {
                                if (v.boneIds[0] != 0)
                                    obj.vertices[CurVtx].boneNames.Add(shape.GetBoneNameFromIndex(this, v.boneIds[0]));
                                if (v.boneIds[1] != 0)
                                    obj.vertices[CurVtx].boneNames.Add(shape.GetBoneNameFromIndex(this, v.boneIds[1]));
                                if (v.boneIds[2] != 0)
                                    obj.vertices[CurVtx].boneNames.Add(shape.GetBoneNameFromIndex(this, v.boneIds[2]));
                                if (v.boneIds[3] != 0)
                                    obj.vertices[CurVtx].boneNames.Add(shape.GetBoneNameFromIndex(this, v.boneIds[3]));

                                CurVtx++;
                            }
                        }
                        System.IO.File.WriteAllBytes(sfd.FileName, csv.Save());
                        break;
                    default:
                        List<STGenericTexture> Surfaces = new List<STGenericTexture>();
                        foreach (FSHP fshp in shapes)
                        {
                            foreach (var bntx in PluginRuntime.bntxContainers)
                            {
                                foreach (var tex in fshp.GetMaterial().TextureMaps)
                                {
                                    if (bntx.Textures.ContainsKey(tex.Name))
                                    {
                                        Surfaces.Add(bntx.Textures[tex.Name]);
                                    }
                                }
                            }
                            foreach (var ftex in PluginRuntime.ftexContainers)
                            {
                                foreach (var tex in fshp.GetMaterial().TextureMaps)
                                {
                                    if (ftex.Textures.ContainsKey(tex.Name))
                                    {
                                        Surfaces.Add(ftex.Textures[tex.Name]);
                                    }
                                }
                            }
                        }
                        Console.WriteLine("tex count " + Surfaces.Count);

                        AssimpData assimp = new AssimpData();
                        assimp.SaveFromModel(this, sfd.FileName, Surfaces);
                        break;
                }
            }
        }

        public void Replace(object sender, EventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfobj;*.fbx;*.dae;*.obj;*.csv;|" +
             "Bfres Object (shape/vertices) |*.bfobj|" +
             "FBX |*.fbx|" +
             "DAE |*.dae|" +
             "OBJ |*.obj|" +
             "CSV |*.csv|" +
             "All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                AddOjects(ofd.FileName);
            }

        }
        //Function addes shapes, vertices and meshes
        public void AddOjects(string FileName, bool Replace = true)
        {
            bool IsWiiU = (GetResFileU() != null);

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

                    Shape shpS = new Shape();
                    VertexBuffer vertexBuffer = new VertexBuffer();
                    shpS.Import(FileName, vertexBuffer);

                    FSHP shapeS = new FSHP();
                    shapeS.Shape = shpS;
                    BfresSwitch.ReadShapesVertices(shapeS, shpS, vertexBuffer, this);
                    shapes.Add(shapeS);
                    Nodes["FshpFolder"].Nodes.Add(shapeS);
                    Cursor.Current = Cursors.Default;
                    break;
                case ".bfmdl":
                    Cursor.Current = Cursors.WaitCursor;

                    if (Replace)
                    {
                        shapes.Clear();
                        Nodes["FshpFolder"].Nodes.Clear();
                    }

                    Model mdl = new Model();
                    mdl.Import(FileName, GetResFile());
                    mdl.Name = Text;
                    shapes.Clear();
                    foreach (Shape shp in mdl.Shapes)
                    {
                        FSHP shape = new FSHP();
                        shape.Shape = shp;
                        BfresSwitch.ReadShapesVertices(shape, shp, mdl.VertexBuffers[shp.VertexBufferIndex], this);
                        shapes.Add(shape);
                        Nodes["FshpFolder"].Nodes.Add(shape);
                    }
                    Cursor.Current = Cursors.Default;
                    break;
                case ".csv":
                    CsvModel csvModel = new CsvModel();
                    csvModel.LoadFile(FileName, true);

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
                        if (Replace)
                        {
                            shapes.Clear();
                            Nodes["FshpFolder"].Nodes.Clear();
                        }

                        Cursor.Current = Cursors.WaitCursor;

                        foreach (STGenericObject obj in csvModel.objects)
                        {
                            FSHP shape = new FSHP();
                            shape.VertexBufferIndex = shapes.Count;
                            shape.vertices = obj.vertices;
                            shape.MaterialIndex = 0;
                            shape.vertexAttributes = csvsettings.CreateNewAttributes();
                            shape.boneIndx = 0;
                            shape.Text = obj.ObjectName;
                            shape.lodMeshes = obj.lodMeshes;
                            shape.CreateNewBoundingBoxes();
                            shape.CreateBoneList(obj, this);
                            shape.CreateIndexList(obj, this);
                            shape.VertexSkinCount = obj.GetMaxSkinInfluenceCount();
                            shape.ApplyImportSettings(csvsettings, GetMaterial(shape.MaterialIndex));
                            shape.SaveShape(IsWiiU);
                            shape.SaveVertexBuffer(IsWiiU);
                            shape.BoneIndices = new List<ushort>();

                            Nodes["FshpFolder"].Nodes.Add(shape);
                            shapes.Add(shape);
                        }
                        Cursor.Current = Cursors.Default;
                    }
                    break;
                default:
                    AssimpData assimp = new AssimpData();
                    assimp.LoadFile(FileName);

                    if (assimp.objects.Count == 0)
                    {
                        MessageBox.Show("No models found!");
                        return;
                    }
                    BfresModelImportSettings settings = new BfresModelImportSettings();
                    settings.SetModelAttributes(assimp.objects[0]);
                    if (settings.ShowDialog() == DialogResult.OK)
                    {
                        if (Replace)
                        {
                            shapes.Clear();
                            Nodes["FshpFolder"].Nodes.Clear();
                        }

                        Cursor.Current = Cursors.WaitCursor;
                        if (Replace)
                        {
                            materials.Clear();
                            Nodes["FmatFolder"].Nodes.Clear();
                            MatStartIndex = 0;
                        }
                        foreach (STGenericMaterial mat in assimp.materials)
                        {
                            FMAT fmat = new FMAT();
                            if (settings.ExternalMaterialPath != string.Empty)
                            {
                                if (GetResFileU() != null)
                                {
                                    fmat.MaterialU = new ResU.Material();
                                    fmat.MaterialU.Import(settings.ExternalMaterialPath, GetResFileU());
                                    BfresWiiU.ReadMaterial(fmat, fmat.MaterialU);
                                }
                                else
                                {
                                    fmat.Material = new Material();
                                    fmat.Material.Import(settings.ExternalMaterialPath);
                                    fmat.ReadMaterial(fmat.Material);
                                }
                            }

                            fmat.Text = mat.Text;
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

                            if (PluginRuntime.bntxContainers.Count > 0)
                            {
                                foreach (var node in Parent.Parent.Nodes["EXT"].Nodes)
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

                            materials.Add(fmat.Text, fmat);
                            Nodes["FmatFolder"].Nodes.Add(fmat);

                            if (GetResFileU() != null)
                            {
                                fmat.MaterialU.Name = Text;
                                fmat.SetMaterial(fmat.MaterialU);
                            }
                            else
                            {
                                fmat.Material.Name = Text;
                                fmat.SetMaterial(fmat.Material);
                            }
                        }


                        foreach (STGenericObject obj in assimp.objects)
                        {
                            FSHP shape = new FSHP();
                            shape.VertexBufferIndex = shapes.Count;
                            shape.vertices = obj.vertices;
                            shape.VertexSkinCount = obj.MaxSkinInfluenceCount;
                            shape.vertexAttributes = settings.CreateNewAttributes();
                            shape.boneIndx = obj.BoneIndex;
                            shape.MaterialIndex = obj.MaterialIndex + MatStartIndex;

                            shape.Text = obj.ObjectName;
                            shape.lodMeshes = obj.lodMeshes;
                            shape.CreateNewBoundingBoxes();
                            shape.CreateBoneList(obj, this);
                            shape.CreateIndexList(obj, this);
                            shape.ApplyImportSettings(settings, GetMaterial(shape.MaterialIndex));
                            shape.SaveShape(IsWiiU);
                            shape.SaveVertexBuffer(IsWiiU);
                            shape.BoneIndices = new List<ushort>();

                            List<string> keyList = shapes.Select(o => o.Text).ToList();

                            shape.Text = Utils.RenameDuplicateString(keyList, shape.Text);

                            Nodes["FshpFolder"].Nodes.Add(shape);
                            shapes.Add(shape);
                        }
                        Console.WriteLine("Finshed Importing Model");

                        Cursor.Current = Cursors.Default;
                    }
                    break;
            }
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
                    mat.Material = new Material();
                    mat.Material.Import(FileName);
                    mat.ReadMaterial(mat.Material);
                    mat.Text = mat.Material.Name;

                    materials.Add(mat.Text, mat);
                    Nodes["FmatFolder"].Nodes.Add(mat);
                    break;
            }
        }
        public override void OnClick(TreeView treeView)
        {

        }
        private void CreateSkeleton()
        {

        }
        private void CreateBones(STBone bone)
        {
            Bone bn = new Bone();
            bn.BillboardIndex = (ushort)bone.BillboardIndex;
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
