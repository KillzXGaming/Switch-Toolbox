using Toolbox.Library.NodeWrappers;
using Toolbox.Library;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System;
using FirstPlugin;
using ResU = Syroot.NintenTools.Bfres;
using ResNX = Syroot.NintenTools.NSW.Bfres;
using Toolbox.Library.Animations;
using Toolbox.Library.Forms;

namespace Bfres.Structs
{
    public enum BRESGroupType
    {
        None,
        Models,
        Textures,
        SkeletalAnim,
        MaterialAnim,
        ShaderParamAnim,
        ColorAnim,
        TexSrtAnim,
        TexPatAnim,
        BoneVisAnim,
        MatVisAnim,
        ShapeAnim,
        SceneAnim,
        Embedded,
    }

    public class BFRESGroupNode : STGenericWrapper, IContextMenuNode
    {
        public bool ShowNewContextMenu = true;

        public bool IsWiiU;

        public override void OnClick(TreeView treeview)
        {
            if (Parent is BFRES)
                ((BFRES)Parent).LoadEditors(this);
            else if (Parent.Parent is BFRES) {
                ((BFRES)Parent.Parent).LoadEditors(this);
            }else {
                ((BFRES)Parent.Parent.Parent).LoadEditors(this);
            }
        }

        public BFRESGroupNode(bool isWiiU) : base()
        {
            ImageKey = "folder";

            IsWiiU = isWiiU;

            CanExport = false;
            CanReplace = false;
            CanRename = false;
            CanDelete = false;
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();

            Items.Add(new STToolStipMenuItem("New", null, NewAction, Keys.Control | Keys.N) { Enabled = ShowNewContextMenu });
            Items.Add(new STToolStipMenuItem("Import", null, ImportAction, Keys.Control | Keys.I));
            Items.Add(new ToolStripMenuItem("Export All", null, ExportAllAction, Keys.Control | Keys.E));
            Items.Add(new ToolStripMenuItem("Replace (From Folder)", null, ReplaceAllAction, Keys.Control | Keys.R));
            Items.Add(new STToolStripSeparator());
            Items.Add(new STToolStipMenuItem("Sort", null, SortAction, Keys.Control | Keys.S));
            Items.Add(new STToolStipMenuItem("Clear", null, ClearAction, Keys.Control | Keys.C));

            if (Type == BRESGroupType.Textures)
            {
                Items.Add(new STToolStripSeparator());
                Items.Add(new STToolStipMenuItem("Batch Generate Mipmaps", null, BatchGenerateMipmapsAction, Keys.Control | Keys.M));
            }
            if (Type == BRESGroupType.Models)
            {
                Items.Add(new STToolStripSeparator());
                Items.Add(new STToolStipMenuItem("Show All Models", null, ShowAllModelsAction, Keys.Control | Keys.A));
                Items.Add(new STToolStipMenuItem("Hide All Models", null, HideAllModelsAction, Keys.Control | Keys.H));
            }
            if (Type == BRESGroupType.SkeletalAnim)
            {
                Items.Add(new STToolStripSeparator());
                Items.Add(new STToolStipMenuItem("Batch Edit Base Data", null, BatchEditBaseAnimDataAction, Keys.Control | Keys.A));
            }

            return Items.ToArray();
        }

        public override string ExportFilter { get { return GetSubfileExtensions(true); } }
        public override string ImportFilter { get { return GetSubfileExtensions(false); } }

        protected void NewAction(object sender, EventArgs e) { NewSubFile(); }
        protected void BatchGenerateMipmapsAction(object sender, EventArgs e) { BatchGenerateMipmaps(); }

        protected void HideAllModelsAction(object sender, EventArgs e) { HideAllModels(); }
        protected void ShowAllModelsAction(object sender, EventArgs e) { ShowAllModels(); }

        protected void BatchEditBaseAnimDataAction(object sender, EventArgs e) { BatchEditBaseAnimData(); }

        private void BatchEditBaseAnimData()
        {
            if (Nodes.Count <= 0)
                return;

            BatchEditBaseAnimDataForm form = new BatchEditBaseAnimDataForm();
            form.LoadAnim((Animation)Nodes[0]);

            if (form.ShowDialog()== DialogResult.OK)
            {
                foreach (FSKA animation in Nodes)
                {
                    if (animation.SkeletalAnimU != null)
                    {
                        foreach (var boneAnim in animation.SkeletalAnimU.BoneAnims)
                        {
                            if (boneAnim.Name == form.TargetBone)
                            {
                                var baseData = boneAnim.BaseData;
                                if (form.HasCustomScale)
                                {
                                    boneAnim.FlagsBase |= ResU.BoneAnimFlagsBase.Scale;
                                    boneAnim.ApplyScaleOne = false;
                                    boneAnim.ApplyScaleUniform = false;
                                    boneAnim.ApplySegmentScaleCompensate = form.SegmentScaleCompensate;
                                    baseData.Scale = new Syroot.Maths.Vector3F(form.ScaleX, form.ScaleY, form.ScaleZ);
                                }

                                boneAnim.BaseData = baseData;
                            }
                        }
                    }
                    else
                    {
                        foreach (var boneAnim in animation.SkeletalAnim.BoneAnims)
                        {
                            if (boneAnim.Name == form.TargetBone)
                            {
                                var baseData = boneAnim.BaseData;
                                if (form.HasCustomScale)
                                {
                                    boneAnim.FlagsBase |= ResNX.BoneAnimFlagsBase.Scale;
                                    boneAnim.ApplyScaleOne = false;
                                    boneAnim.ApplyScaleUniform = false;
                                    boneAnim.ApplySegmentScaleCompensate = form.SegmentScaleCompensate;
                                    baseData.Scale = new Syroot.Maths.Vector3F(form.ScaleX, form.ScaleY, form.ScaleZ);
                                }

                                boneAnim.BaseData = baseData;
                            }
                        }
                       }

                    animation.OpenAnimationData();
                }
            }
        }

        public override void ReplaceAll(string ReplacePath = "")
        {
            FolderSelectDialog sfd = new FolderSelectDialog();
            if (ReplacePath != "" || sfd.ShowDialog() == DialogResult.OK)
            {
                if (Type == BRESGroupType.Textures)
                {
                    GTXTextureImporter importer = new GTXTextureImporter();
                    List<GTXImporterSettings> settings = new List<GTXImporterSettings>();

                    foreach (string file in System.IO.Directory.GetFiles(ReplacePath != "" ? ReplacePath : sfd.SelectedPath))
                    {
                        string FileName = System.IO.Path.GetFileNameWithoutExtension(file);

                        foreach (FTEX ftex in Nodes)
                        {
                            if (FileName == ftex.Text)
                            {
                                string ext = Path.GetExtension(file);
                                ext = ext.ToLower();

                                if (ext == ".bftex")
                                {
                                    ftex.texture = new ResU.Texture();
                                    ftex.texture.Import(file, GetResFileU());
                                    ftex.IsEdited = true;
                                    ftex.Read(ftex.texture);
                                }
                                else if (ext == ".dds" || ext == ".dds2")
                                {
                                    GTXImporterSettings setting = FTEX.SetImporterSettings(file);
                                    if (setting.DataBlockOutput != null)
                                    {
                                        var surface = GTXSwizzle.CreateGx2Texture(setting.DataBlockOutput[0], setting);
                                        var tex = FTEX.FromGx2Surface(surface, setting.TexName);
                                        ftex.UpdateTex(tex);
                                        ftex.IsReplaced = true;
                                        ftex.Read(ftex.texture);
                                        ftex.LoadOpenGLTexture();
                                    }
                                }
                                else
                                {
                                    var setting = FTEX.SetImporterSettings(file);
                                    setting.MipCountOriginal = (int)ftex.MipCount;
                                    setting.Format = (GX2.GX2SurfaceFormat)FTEX.ConvertToGx2Format(ftex.Format);
                                    settings.Add(setting);
                                }
                            }
                        }
                    }

                    if (settings.Count == 0)
                    {
                        importer.Dispose();
                    }
                    else
                    {
                        importer.LoadSettings(settings);
                        if (importer.ShowDialog() == DialogResult.OK)
                        {
                            foreach (var setting in settings)
                            {
                                foreach (FTEX ftex in Nodes)
                                {
                                    if (setting.TexName == ftex.Text)
                                    {
                                        if (setting.GenerateMipmaps && !setting.IsFinishedCompressing)
                                        {
                                            setting.DataBlockOutput.Clear();
                                            setting.DataBlockOutput.Add(setting.GenerateMips());
                                        }

                                        if (setting.DataBlockOutput != null)
                                        {
                                            var surface = GTXSwizzle.CreateGx2Texture(setting.DataBlockOutput[0], setting);
                                            var tex = FTEX.FromGx2Surface(surface, setting.TexName);
                                            ftex.UpdateTex(tex);
                                            tex.Name = ftex.Text;
                                            ftex.IsEdited = true;
                                            ftex.Read(ftex.texture);
                                            ftex.LoadOpenGLTexture();
                                        }
                                        else
                                        {
                                            MessageBox.Show("Something went wrong???");
                                        }
                                    }
                                }
                            }

                            settings.Clear();

                            GC.Collect();
                            Cursor.Current = Cursors.Default;
                        }
                    }
                }
                else
                {
                    foreach (string file in System.IO.Directory.GetFiles(ReplacePath != "" ? ReplacePath : sfd.SelectedPath))
                    {
                        string FileName = System.IO.Path.GetFileNameWithoutExtension(file);

                        foreach (TreeNode node in Nodes)
                        {
                            if (node is STGenericWrapper)
                            {
                                if (FileName == node.Text)
                                {
                                    ((STGenericWrapper)node).Replace(file);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void BatchGenerateMipmaps()
        {
            foreach (FTEX texture in Nodes)
            {
                texture.SetImageData(texture.GetBitmap(), 0);
            }
        }

        public void ShowAllModels()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Checked = true;
            }
        }

        public void HideAllModels()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Checked = false;
            }
        }

        public BFRESGroupNode(string name, bool isWiiU = false) : base() { Text = name; IsWiiU = isWiiU; }
        public BFRESGroupNode(BRESGroupType type, bool isWiiU = false) : base() { Type = type; SetNameByType(); IsWiiU = isWiiU; }

        public BRESGroupType Type { get; set; }

        public Dictionary<string, STGenericWrapper> ResourceNodes = new Dictionary<string, STGenericWrapper>(); //To get instance of classes

        public ResNX.ResFile GetResFile() {
            if (Parent is BFRES)
                return ((BFRES)Parent).resFile;
            else
                return ((BFRES)Parent.Parent).resFile;
        }

        public ResU.ResFile GetResFileU() {
            if (Parent is BFRES)
                return ((BFRES)Parent).resFileU;
            else
                return ((BFRES)Parent.Parent).resFileU;
        }


        public void NewSubFile()
        {
            switch (Type)
            {
                case BRESGroupType.Models: NewModel(); break;
                case BRESGroupType.SkeletalAnim: ((BFRESAnimFolder)Parent).NewSkeletalAnim(); break;
                case BRESGroupType.ShaderParamAnim: ((BFRESAnimFolder)Parent).ImportShaderParamAnim(); break;
                case BRESGroupType.ColorAnim: ((BFRESAnimFolder)Parent).NewColorAnim(); break;
                case BRESGroupType.TexSrtAnim: ((BFRESAnimFolder)Parent).NewTexSrtAnim(); break;
                case BRESGroupType.TexPatAnim: ((BFRESAnimFolder)Parent).NewTexPatAnim(); break;
                case BRESGroupType.BoneVisAnim: ((BFRESAnimFolder)Parent).NewBoneVisAnim(); break;
                case BRESGroupType.MatVisAnim: ((BFRESAnimFolder)Parent).NewMatVisAnim(); break;
                case BRESGroupType.ShapeAnim: ((BFRESAnimFolder)Parent).NewShapeAnim(); break;
                case BRESGroupType.SceneAnim: ((BFRESAnimFolder)Parent).NewSceneAnim(); break;
                case BRESGroupType.Embedded: NewExternalFile(); break;
            }
        }

        public FMDL NewModel(bool AddTreeNode = true)
        {
            FMDL fmdl = new FMDL();

            if (IsWiiU)
            {
                fmdl.ModelU = new ResU.Model();

                //Create skeleton with empty bone
                var skeleton = new ResU.Skeleton();

                //Create skeleton with empty bone
                skeleton.Bones.Add("Root", new ResU.Bone() { Name = "Root" });

                fmdl.ModelU.Skeleton = skeleton;

                var shape = new ResU.Shape() { Name = "NewShape" };
                shape.CreateEmptyMesh();

                var VertexBuffer = new ResU.VertexBuffer();
                VertexBuffer.CreateEmptyVertexBuffer();

                fmdl.ModelU.VertexBuffers.Add(VertexBuffer);
                fmdl.ModelU.Shapes.Add("NewShape", shape);
                fmdl.ModelU.Materials.Add("NewMaterial", new ResU.Material() { Name = "NewMaterial", RenderState = new ResU.RenderState(), });

                BfresWiiU.ReadModel(fmdl, fmdl.ModelU);
                ((BFRES)Parent).DrawableContainer.Drawables.Add(fmdl.Skeleton);
            }
            else
            {
                fmdl.Model = new ResNX.Model();

                //Create skeleton with empty bone
                var skeleton = new ResNX.Skeleton();

                //Create skeleton with empty bone
                skeleton.Bones.Add(new ResNX.Bone() { Name = "Root" });

                fmdl.Model.Skeleton = skeleton;

                var shape = new ResNX.Shape() { Name = "NewShape" };
                shape.CreateEmptyMesh();

                fmdl.Model.Shapes.Add(shape);
                fmdl.Model.Materials.Add(new ResNX.Material() { Name = "NewMaterial" });

                var VertexBuffer = new ResNX.VertexBuffer();
                VertexBuffer.CreateEmptyVertexBuffer();

                fmdl.Model.VertexBuffers.Add(VertexBuffer);

                BfresSwitch.ReadModel(fmdl, fmdl.Model);
                ((BFRES)Parent).DrawableContainer.Drawables.Add(fmdl.Skeleton);
            }

            if (AddTreeNode)
                AddNode(fmdl, "NewModel");

            return fmdl;
        }

        public void NewExternalFile()
        {
            ExternalFileData externalFileData = new ExternalFileData("NewExternalFile", new byte[0]);
            AddNode(externalFileData, "NewExternalFile");
        }


        private string GetSubfileExtensions(bool IsExporting)
        {
            switch (Type)
            {
                case BRESGroupType.Models: return FileFilters.GetFilter(typeof(FMDL), null, IsExporting);
                case BRESGroupType.Textures: return FileFilters.GetFilter(typeof(FTEX), null, IsExporting);
                case BRESGroupType.SkeletalAnim: return FileFilters.GetFilter(typeof(FSKA), null, IsExporting);
                case BRESGroupType.MaterialAnim: return FileFilters.GetFilter(typeof(FMAA), null, IsExporting);
                case BRESGroupType.ShaderParamAnim: return FileFilters.GetFilter(typeof(FSHU), null, IsExporting);
                case BRESGroupType.ColorAnim: return FileFilters.GetFilter(typeof(FSHU), MaterialAnimation.AnimationType.Color, IsExporting);
                case BRESGroupType.TexSrtAnim: return FileFilters.GetFilter(typeof(FSHU), MaterialAnimation.AnimationType.TextureSrt, IsExporting);
                case BRESGroupType.TexPatAnim: return FileFilters.GetFilter(typeof(FTXP), MaterialAnimation.AnimationType.ShaderParam, IsExporting);
                case BRESGroupType.BoneVisAnim: return FileFilters.GetFilter(typeof(FVIS), null, IsExporting);
                case BRESGroupType.MatVisAnim: return FileFilters.GetFilter(typeof(FVIS), null, IsExporting);
                case BRESGroupType.ShapeAnim: return FileFilters.GetFilter(typeof(FSHA), null, IsExporting);
                case BRESGroupType.SceneAnim: return FileFilters.GetFilter(typeof(FSCN), null, IsExporting);
                case BRESGroupType.Embedded: return FileFilters.GetFilter(typeof(ExternalFileData), null, IsExporting);
                default: return "All files(*.*)|*.*";
            }
        }

        public override void Import(string[] FileNames) {
            Import(FileNames, GetResFile(), GetResFileU());
        }

        public void Import(string[] FileNames, ResNX.ResFile resFileNX, ResU.ResFile resFileU)
        {
            if (Type == BRESGroupType.Textures)
            {
                ImportTexture(FileNames);
                return;
            }

            foreach (string FileName in FileNames)
            {
                string ResourceName = Path.GetFileNameWithoutExtension(FileName);
                string ResourceNameExt = Path.GetFileName(FileName);

                string extension = Path.GetExtension(FileName);

                switch (Type)
                {
                    case BRESGroupType.Models:
                        FMDL fmdl = NewModel(false);
                        fmdl.Text = ResourceName;
                        fmdl.Replace(FileName, resFileNX, resFileU);
                        fmdl.UpdateVertexData();
                        AddNode(fmdl);
                        break;
                    case BRESGroupType.SkeletalAnim:
                        FSKA fska = new FSKA();
                        fska.Text = ResourceName;
                        if (IsWiiU)
                            fska.SkeletalAnimU = new ResU.SkeletalAnim();
                        else
                            fska.SkeletalAnim = new ResNX.SkeletalAnim();

                        fska.Replace(FileName, resFileNX, resFileU);
                        Nodes.Add(fska);
                        break;
                    case BRESGroupType.ShaderParamAnim:
                        if (IsWiiU)
                        {
                            FSHU fshu = new FSHU(new ResU.ShaderParamAnim(), MaterialAnimation.AnimationType.ShaderParam);
                            fshu.Text = ResourceName;
                            fshu.Replace(FileName, resFileU);
                            Nodes.Add(fshu);
                        }
                        else
                        {
                            FMAA fmaaPrm = new FMAA(new ResNX.MaterialAnim());
                            fmaaPrm.Text = ResourceName;
                            fmaaPrm.Replace(FileName);
                            Nodes.Add(fmaaPrm);
                        }
                        break;
                    case BRESGroupType.ColorAnim:
                        if (IsWiiU)
                        {
                            FSHU fclh = new FSHU(new ResU.ShaderParamAnim(), MaterialAnimation.AnimationType.Color);
                            fclh.Text = ResourceName;
                            fclh.Replace(FileName, resFileU);
                            Nodes.Add(fclh);
                        }
                        else
                        {
                            FMAA fmaaClr = new FMAA(new ResNX.MaterialAnim());
                            fmaaClr.Text = ResourceName;
                            fmaaClr.Replace(FileName);
                            Nodes.Add(fmaaClr);
                        }
                        break;
                    case BRESGroupType.TexSrtAnim:
                        if (IsWiiU)
                        {
                            FSHU fsth = new FSHU(new ResU.ShaderParamAnim(), MaterialAnimation.AnimationType.TextureSrt);
                            fsth.Text = ResourceName;
                            fsth.Replace(FileName, resFileU);
                            Nodes.Add(fsth);
                        }
                        else
                        {
                            FMAA fmaaSrt = new FMAA(new ResNX.MaterialAnim());
                            fmaaSrt.Text = ResourceName;
                            fmaaSrt.Replace(FileName);
                            Nodes.Add(fmaaSrt);
                        }
                        break;
                    case BRESGroupType.TexPatAnim:
                        if (IsWiiU)
                        {
                            FTXP ftxp = new FTXP(new ResU.TexPatternAnim());
                            ftxp.Text = ResourceName;
                            ftxp.Replace(FileName, resFileU);
                            Nodes.Add(ftxp);
                        }
                        else
                        {
                            FMAA fmaaTxp = new FMAA(new ResNX.MaterialAnim());
                            fmaaTxp.Text = ResourceName;
                            fmaaTxp.Replace(FileName);
                            Nodes.Add(fmaaTxp);
                        }

                     
                        break;
                    case BRESGroupType.BoneVisAnim:
                        FVIS fbnv = new FVIS();
                        fbnv.Text = ResourceName;
                        if (IsWiiU)
                            fbnv.VisibilityAnimU = new ResU.VisibilityAnim() { Type = ResU.VisibilityAnimType.Bone };
                        else
                            fbnv.VisibilityAnim = new ResNX.VisibilityAnim();

                        fbnv.Replace(FileName, resFileNX, resFileU);
                        Nodes.Add(fbnv);
                        break;
                    case BRESGroupType.MatVisAnim:
                        if (IsWiiU)
                        {
                            FVIS fmtv = new FVIS(new ResU.VisibilityAnim() { Type = ResU.VisibilityAnimType.Material });
                            fmtv.Text = ResourceName;
                            fmtv.Replace(FileName, resFileNX, resFileU);
                            Nodes.Add(fmtv);
                        }
                        else
                        {
                            FMAA fmaaVis = new FMAA(new ResNX.MaterialAnim());
                            fmaaVis.Text = ResourceName;
                            fmaaVis.Replace(FileName);
                            Nodes.Add(fmaaVis);
                        }
                        break;
                    case BRESGroupType.ShapeAnim:
                        FSHA fsha = new FSHA();
                        fsha.Text = ResourceName;
                        if (IsWiiU)
                            fsha.ShapeAnimU = new ResU.ShapeAnim();
                        else
                            fsha.ShapeAnim = new ResNX.ShapeAnim();

                        fsha.Replace(FileName, resFileNX, resFileU);
                        Nodes.Add(fsha);
                        break;
                    case BRESGroupType.SceneAnim:
                        FSCN fscn = new FSCN();
                        fscn.Text = ResourceName;
                        if (IsWiiU)
                            fscn.SceneAnimU = new ResU.SceneAnim();
                        else
                            fscn.SceneAnim = new ResNX.SceneAnim();

                        fscn.Replace(FileName, resFileNX, resFileU);
                        Nodes.Add(fscn);
                        break;
                    case BRESGroupType.Embedded:
                        ExternalFileData ext = new ExternalFileData(ResourceNameExt, File.ReadAllBytes(FileName));
                        ext.Replace(FileName);
                        Nodes.Add(ext);
                        break;
                    case BRESGroupType.MaterialAnim:
                        FMAA fmaa = new FMAA(new ResNX.MaterialAnim());
                        fmaa.Replace(FileName);
                        Nodes.Add(fmaa);
                        break;
                }
            }
        }

        public void AddNode(STGenericWrapper node, string Name)
        {
            node.Text = SearchDuplicateName(Name);

            AddNode(node);
        }

        public void AddNode(STGenericWrapper node)
        {
            if (node.Text == string.Empty)
                throw new System.Exception("Text invalid. Must not be empty! ");

            Nodes.Add(node);
            ResourceNodes.Add(node.Text, node);
        }

        public void RemoveChild(STGenericWrapper node)
        {
            Nodes.Remove(node);
            ResourceNodes.Remove(node.Text);
        }


        public override void Clear()
        {
            var result = MessageBox.Show("Are you sure you want to clear this section? This cannot be undone!",
                "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                foreach (TreeNode node in Nodes)
                {
                    if (node is STGenericWrapper)
                    {
                        ((STGenericWrapper)node).Unload();
                        RemoveChild(((STGenericWrapper)node));
                    }
                }
   
                ResourceNodes.Clear();
                Nodes.Clear();

                if (Type == BRESGroupType.Models)
                {
                    ((BFRES)Parent).BFRESRender.UpdateModelList();
                    LibraryGUI.UpdateViewport();
                }
            }
        }

        public STGenericWrapper GetFirstChild()
        {
            if (Nodes.Count > 0 && Nodes[0] is STGenericWrapper)
                return (STGenericWrapper)Nodes[0];
            else
                return new STGenericWrapper();
        }

        public void SetNameByType()
        {
            Text = SetName();
        }

        private string SetName()
        {
            switch (Type)
            {
                case BRESGroupType.Models: return "Models";
                case BRESGroupType.Textures: return "Textures";
                case BRESGroupType.SkeletalAnim: return "Skeletal Animations";
                case BRESGroupType.ShaderParamAnim: return "Shader Param Animations";
                case BRESGroupType.ColorAnim: return "Color Animations";
                case BRESGroupType.TexSrtAnim: return "Texture SRT Animations";
                case BRESGroupType.TexPatAnim: return "Texture Pattern Animations";
                case BRESGroupType.BoneVisAnim: return "Bone Visibilty Animations";
                case BRESGroupType.MatVisAnim: return "Material Visibilty Animations";
                case BRESGroupType.ShapeAnim: return "Shape Animations";
                case BRESGroupType.SceneAnim: return "Scene Animations";
                case BRESGroupType.Embedded: return "Embedded Files";
                case BRESGroupType.MaterialAnim: return "Material Animations";
                default:
                    throw new System.Exception("Unknown type? " + Type);
            }
        }

        public void GetFileType()
        {
            var child = GetFirstChild();

            if (Text == "Models" || child is FMDL)
                Type = BRESGroupType.Models;
            if (Text == "Textures" || child is FTEX)
                Type = BRESGroupType.Textures;
            if (Text == "Skeleton Animations" || child is FSKA)
                Type = BRESGroupType.SkeletalAnim;
            if (Text == "Material Animations" || child is FMAA)
                Type = BRESGroupType.MaterialAnim;
            if (Text == "Shader Param Animations" || child is FSHU)
                Type = BRESGroupType.ShaderParamAnim;
            if (Text == "Color Animations" || child is FSHU)
                Type = BRESGroupType.ColorAnim;
            if (Text == "Texture Srt Animations" || child is FSHU)
                Type = BRESGroupType.TexSrtAnim;
            if (Text == "Texture Pattern Animations" || child is FTXP)
                Type = BRESGroupType.TexPatAnim;
            if (Text == "Bone Visibilty Animations" || child is FVIS)
                Type = BRESGroupType.BoneVisAnim;
            if (Text == "Material Visibilty Animations" || child is FVIS)
                Type = BRESGroupType.MatVisAnim;
            if (Text == "Embedded Files" || child is ExternalFileData)
                Type = BRESGroupType.Embedded;
        }

        int IndexStr = 0;
        public string SearchDuplicateName(string Name)
        {
            if (ResourceNodes.ContainsKey(Name))
                return SearchDuplicateName($"{Name}{IndexStr++}");
            else
                return Name;
        }

        public void ImportTexture(string[] FileNames)
        {
            GTXTextureImporter importer = new GTXTextureImporter();
            List<GTXImporterSettings> settings = new List<GTXImporterSettings>();

            foreach (string name in FileNames)
            {
                string TextureName = Path.GetFileNameWithoutExtension(name);


                string ext = Path.GetExtension(name);
                ext = ext.ToLower();

                if (ext == ".bftex")
                {
                    FTEX ftex = new FTEX();
                    ftex.texture = new ResU.Texture();
                    ftex.texture.Import(name, GetResFileU());
                    ftex.IsEdited = true;
                    ftex.Read(ftex.texture);
                    AddNode(ftex);
                }
                else if (ext == ".dds" || ext == ".dds2")
                {
                    FTEX ftex = new FTEX();
                    ftex.texture = new ResU.Texture();

                    GTXImporterSettings setting = FTEX.SetImporterSettings(name);

                    if (setting.DataBlockOutput != null)
                    {
                        var surface = GTXSwizzle.CreateGx2Texture(setting.DataBlockOutput[0], setting);
                        var tex = FTEX.FromGx2Surface(surface, setting.TexName);
                        ftex.UpdateTex(tex);

                        ftex.IsEdited = true;
                        ftex.Read(ftex.texture);
                        ftex.LoadOpenGLTexture();
                        AddNode(ftex);
                    }
                }
                else
                {
                    settings.Add(FTEX.SetImporterSettings(name));
                }
            }
            if (settings.Count == 0)
            {
                importer.Dispose();
            }
            else
            {
                importer.LoadSettings(settings);
                if (importer.ShowDialog() == DialogResult.OK)
                {
                    ImportTexture(settings);

                    settings.Clear();

                    GC.Collect();
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        public GTXImporterSettings LoadSettings(System.Drawing.Image image, string Name)
        {
            var importer = new GTXImporterSettings();
            importer.LoadBitMap(image, Name);
            return importer;
        }

        private void ImportTexture(List<GTXImporterSettings> settings)
        {
            Cursor.Current = Cursors.WaitCursor;
            foreach (var setting in settings)
            {
                if (setting.GenerateMipmaps)
                {
                    setting.DataBlockOutput.Clear();
                    setting.DataBlockOutput.Add(setting.GenerateMips());
                }

                if (setting.DataBlockOutput != null)
                {
                    FTEX ftex = new FTEX();
                    ftex.texture = new ResU.Texture();
                    var surface = GTXSwizzle.CreateGx2Texture(setting.DataBlockOutput[0], setting);
                    var tex = FTEX.FromGx2Surface(surface, setting.TexName);
                    ftex.UpdateTex(tex);
                    ftex.IsEdited = true;

                    ftex.Read(ftex.texture);
                    ftex.LoadOpenGLTexture();
                    AddNode(ftex);
                }
                else
                {
                    MessageBox.Show("Something went wrong???");
                }
            }
        }

        public void ImportTexture(ImageKeyFrame[] Keys, string TextureName)
        {
            if (ResourceNodes.ContainsKey(TextureName) || Type != BRESGroupType.Textures)
                return;

            GTXTextureImporter importer = new GTXTextureImporter();
            List<GTXImporterSettings> settings = new List<GTXImporterSettings>();

            foreach (var key in Keys) {
                settings.Add(FTEX.SetImporterSettings(key.Image, $"{TextureName}{key.Frame}"));
            }

            importer.LoadSettings(settings);

            if (importer.ShowDialog() == DialogResult.OK) {
                ImportTexture(settings);
            }

            settings.Clear();
            GC.Collect();
            Cursor.Current = Cursors.Default;
        }

        public void ImportPlaceholderTexture(string TextureName)
        {
            if (ResourceNodes.ContainsKey(TextureName) || Type != BRESGroupType.Textures)
                return;

            if (TextureName == "Basic_Alb")
                ImportBasicTextures("Basic_Alb");
            else if (TextureName == "Basic_Nrm")
                ImportBasicTextures("Basic_Nrm");
            else if (TextureName == "Basic_Spm")
                ImportBasicTextures("Basic_Spm");
            else if (TextureName == "Basic_Sphere")
                ImportBasicTextures("Basic_Sphere");
            else if (TextureName == "Basic_Mtl")
                ImportBasicTextures("Basic_Mtl");
            else if (TextureName == "Basic_Rgh")
                ImportBasicTextures("Basic_Rgh");
            else if (TextureName == "Basic_MRA")
                ImportBasicTextures("Basic_MRA");
            else if (TextureName == "Basic_Bake_st0")
                ImportBasicTextures("Basic_Bake_st0");
            else if (TextureName == "Basic_Bake_st1")
                ImportBasicTextures("Basic_Bake_st1");
            else if (TextureName == "Basic_Emm")
                ImportBasicTextures("Basic_Emm");
            else
            {
                ImportPlaceholderTexture(FirstPlugin.Properties.Resources.InjectTexErrored, TextureName);
            }
        }
        private void ImportPlaceholderTexture(byte[] data, string TextureName)
        {
            GTXImporterSettings setting = new GTXImporterSettings();
            setting.LoadDDS(TextureName, data);

            var surface = GTXSwizzle.CreateGx2Texture(setting.DataBlockOutput[0], setting);
            FTEX ftex = new FTEX();
            ftex.texture = new ResU.Texture();
            ftex.texture = FTEX.FromGx2Surface(surface, setting.TexName);
            ftex.IsEdited = true;
            ftex.Read(ftex.texture);

            AddNode(ftex);
            ftex.LoadOpenGLTexture();
        }
        public void ImportBasicTextures(string TextureName, bool BC5Nrm = true)
        {
            if (ResourceNodes.ContainsKey(TextureName) || Type != BRESGroupType.Textures)
                return;

            if (TextureName == "Basic_Alb")
                ImportPlaceholderTexture(FirstPlugin.Properties.Resources.InjectTexErrored, TextureName);
            if (TextureName == "Basic_Nrm" && BC5Nrm)
                ImportPlaceholderTexture(FirstPlugin.Properties.Resources.Basic_NrmBC5, TextureName);
            if (TextureName == "Basic_Nrm" && BC5Nrm == false)
                ImportPlaceholderTexture(FirstPlugin.Properties.Resources.Basic_Nrm, TextureName);
            if (TextureName == "Basic_Spm")
                ImportPlaceholderTexture(FirstPlugin.Properties.Resources.Black, TextureName);
            if (TextureName == "Basic_Sphere")
                ImportPlaceholderTexture(FirstPlugin.Properties.Resources.Black, TextureName);
            if (TextureName == "Basic_Mtl")
                ImportPlaceholderTexture(FirstPlugin.Properties.Resources.Black, TextureName);
            if (TextureName == "Basic_Rgh")
                ImportPlaceholderTexture(FirstPlugin.Properties.Resources.White, TextureName);
            if (TextureName == "Basic_MRA")
                ImportPlaceholderTexture(FirstPlugin.Properties.Resources.Black, TextureName);
            if (TextureName == "Basic_Bake_st0")
                ImportPlaceholderTexture(FirstPlugin.Properties.Resources.Basic_Bake_st0, TextureName);
            if (TextureName == "Basic_Bake_st1")
                ImportPlaceholderTexture(FirstPlugin.Properties.Resources.Basic_Bake_st1, TextureName);
        }
    }
}
