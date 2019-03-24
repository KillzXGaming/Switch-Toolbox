using Switch_Toolbox.Library.NodeWrappers;
using Switch_Toolbox.Library;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System;
using FirstPlugin;
using ResU = Syroot.NintenTools.Bfres;
using ResNX = Syroot.NintenTools.NSW.Bfres;
using Switch_Toolbox.Library.Animations;
using Switch_Toolbox.Library.Forms;

namespace Bfres.Structs
{
    public enum BRESGroupType
    {
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

    public class BFRESGroupNode : STGenericWrapper
    {
        public bool IsWiiU
        {
            get
            {
                return ((BFRES)Parent).IsWiiU;
            }
        }

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

        public BFRESGroupNode() : base()
        {
            ImageKey = "folder";

            LoadContextMenus();
        }

        public override void LoadContextMenus()
        {
            ContextMenuStrip = new STContextMenuStrip();

            CanExport = false;
            CanReplace = false;
            CanRename = false;
            CanDelete = false;

            //Folder Operations
            ContextMenuStrip.Items.Add(new STToolStipMenuItem("New", null, NewAction, Keys.Control | Keys.N));
            ContextMenuStrip.Items.Add(new STToolStipMenuItem("Import", null, ImportAction, Keys.Control | Keys.I));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Export All", null, ExportAllAction, Keys.Control | Keys.E));
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Replace All", null, ReplaceAllAction, Keys.Control | Keys.R));
            ContextMenuStrip.Items.Add(new STToolStripSeparator());
            ContextMenuStrip.Items.Add(new STToolStipMenuItem("Sort", null, SortAction, Keys.Control | Keys.S));
            ContextMenuStrip.Items.Add(new STToolStipMenuItem("Clear", null, ClearAction, Keys.Control | Keys.C));
        }

        public override string ExportFilter { get { return GetSubfileExtensions(); } }
        public override string ImportFilter { get { return GetSubfileExtensions(); } }

        protected void NewAction(object sender, EventArgs e) { NewSubFile(); }


        public BFRESGroupNode(string name) : base() { Text = name; }
        public BFRESGroupNode(BRESGroupType type) : base() { Type = type; SetNameByType(); }

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

        public void NewModel()
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
                fmdl.ModelU.Materials.Add("NewMaterial", new ResU.Material() { Name = "NewMaterial" });

                BfresWiiU.ReadModel(fmdl, fmdl.ModelU);
                ((BFRES)Parent).AddSkeletonDrawable(fmdl.Skeleton);
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
                ((BFRES)Parent).AddSkeletonDrawable(fmdl.Skeleton);
            }

            AddNode(fmdl, "NewModel");
        }

        public void NewExternalFile()
        {
            ExternalFileData externalFileData = new ExternalFileData("NewExternalFile", new byte[0]);
            AddNode(externalFileData, "NewExternalFile");
        }


        private string GetSubfileExtensions()
        {
            switch (Type)
            {
                case BRESGroupType.Models: return FileFilters.GetFilter(typeof(FMDL));
                case BRESGroupType.Textures: return FileFilters.GetFilter(typeof(FTEX));
                case BRESGroupType.SkeletalAnim: return FileFilters.GetFilter(typeof(FSKA));
                case BRESGroupType.MaterialAnim: return FileFilters.GetFilter(typeof(FMAA));
                case BRESGroupType.ShaderParamAnim: return FileFilters.GetFilter(typeof(FSHU));
                case BRESGroupType.ColorAnim: return FileFilters.GetFilter(typeof(FSHU));
                case BRESGroupType.TexSrtAnim: return FileFilters.GetFilter(typeof(FSHU));
                case BRESGroupType.TexPatAnim: return FileFilters.GetFilter(typeof(FTXP));
                case BRESGroupType.BoneVisAnim: return FileFilters.GetFilter(typeof(FVIS));
                case BRESGroupType.MatVisAnim: return FileFilters.GetFilter(typeof(FVIS));
                case BRESGroupType.ShapeAnim: return FileFilters.GetFilter(typeof(FSHA));
                case BRESGroupType.SceneAnim: return FileFilters.GetFilter(typeof(FSCN));
                case BRESGroupType.Embedded: return FileFilters.GetFilter(typeof(ExternalFileData));
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
                string extension = Path.GetExtension(FileName);

                switch (Type)
                {
                    case BRESGroupType.Models:
                        FMDL fmdl = new FMDL();
                        fmdl.Text = ResourceName;

                        if (IsWiiU)
                        {
                            fmdl.ModelU = new ResU.Model();
                            fmdl.ModelU.Name = ResourceName;

                            var skeleton = new ResU.Skeleton();

                            //Create skeleton with empty bone
                            skeleton.Bones.Add("Root", new ResU.Bone() { Name = "Root" });

                            fmdl.ModelU.Skeleton = skeleton;
                            fmdl.ModelU.Shapes.Add("NeShape", new ResU.Shape() { Name = "NeShape" });
                            fmdl.ModelU.VertexBuffers.Add(new ResU.VertexBuffer() { });

                            BfresWiiU.ReadModel(fmdl, fmdl.ModelU);
                            ((BFRES)Parent).AddSkeletonDrawable(fmdl.Skeleton);
                        }
                        else
                        {
                            fmdl.Model = new ResNX.Model();
                            fmdl.Model.Name = ResourceName;

                            //Create skeleton with empty bone
                            var skeleton = new ResNX.Skeleton();
                            skeleton.Bones.Add(new ResNX.Bone() { Name = "Root" });

                            fmdl.Model.Skeleton = skeleton;
                            fmdl.Model.Shapes.Add(new ResNX.Shape() { Name = "NeShape" });
                            fmdl.Model.VertexBuffers.Add(new ResNX.VertexBuffer() { });

                            BfresSwitch.ReadModel(fmdl, fmdl.Model);
                            ((BFRES)Parent).AddSkeletonDrawable(fmdl.Skeleton);
                        }
                        fmdl.Replace(FileName, resFileNX, resFileU);

                        Nodes.Add(fmdl);


                        fmdl.UpdateVertexData();
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
                        FSHU fshu = new FSHU(new ResU.ShaderParamAnim(), MaterialAnimation.AnimationType.ShaderParam);
                        fshu.Text = ResourceName;
                        fshu.Replace(FileName, resFileU);
                        Nodes.Add(fshu);
                        break;
                    case BRESGroupType.ColorAnim:
                        FSHU fclh = new FSHU(new ResU.ShaderParamAnim(), MaterialAnimation.AnimationType.Color);
                        fclh.Text = ResourceName;
                        fclh.Replace(FileName, resFileU);
                        Nodes.Add(fclh);
                        break;
                    case BRESGroupType.TexSrtAnim:
                        FSHU fsth = new FSHU(new ResU.ShaderParamAnim(), MaterialAnimation.AnimationType.TextureSrt);
                        fsth.Text = ResourceName;
                        fsth.Replace(FileName, resFileU);
                        Nodes.Add(fsth);
                        break;
                    case BRESGroupType.TexPatAnim:
                        FTXP ftxp = new FTXP(new ResU.TexPatternAnim());
                        ftxp.Text = ResourceName;
                        ftxp.Replace(FileName, resFileU);
                        Nodes.Add(ftxp);
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
                        FVIS fmtv = new FVIS(new ResU.VisibilityAnim() { Type = ResU.VisibilityAnimType.Material });
                        fmtv.Text = ResourceName;
                        fmtv.Replace(FileName, resFileNX, resFileU);
                        Nodes.Add(fmtv);
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
                        ExternalFileData ext = new ExternalFileData(ResourceName, File.ReadAllBytes(FileName));
                        ext.Replace(FileName);
                        Nodes.Add(ext);
                        break;
                    case BRESGroupType.MaterialAnim:
                        FMAA fmaa = new FMAA(new ResNX.MaterialAnim(), MaterialAnimation.AnimationType.ShaderParam);
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
                foreach (var node in Nodes)
                {
                    if (node is STGenericWrapper)
                    {
                        ((STGenericWrapper)node).Unload();
                        RemoveChild(((STGenericWrapper)node));
                    }
                }
                
                Nodes.Clear();
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
                    return;
                }
                else if (ext == ".dds" || ext == ".dds2")
                {
                    FTEX ftex = new FTEX();
                    ftex.texture = new ResU.Texture();

                    GTXImporterSettings setting = FTEX.SetImporterSettings(name);

                    if (setting.DataBlockOutput != null)
                    {
                        var surface = setting.CreateGx2Texture(setting.DataBlockOutput[0]);
                        var tex = ftex.FromGx2Surface(surface, setting);
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
                return;
            }

            importer.LoadSettings(settings);
            if (importer.ShowDialog() == DialogResult.OK)
            {
                ImportTexture(settings);
            }
            settings.Clear();
            GC.Collect();
            Cursor.Current = Cursors.Default;
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
                    var surface = setting.CreateGx2Texture(setting.DataBlockOutput[0]);
                    var tex = ftex.FromGx2Surface(surface, setting);
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

            var surface = setting.CreateGx2Texture(setting.DataBlockOutput[0]);
            FTEX ftex = new FTEX();
            ftex.texture = new ResU.Texture();
            ftex.texture = ftex.FromGx2Surface(surface, setting);
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
