using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;
using Bfres.Structs;
using ResU = Syroot.NintenTools.Bfres;
using Syroot.NintenTools.NSW.Bfres;
using Toolbox.Library.Animations;
using Toolbox.Library.NodeWrappers;
using GL_EditorFramework.Interfaces;
using FirstPlugin.Forms;
using FirstPlugin.NodeWrappers;
using OpenTK;

namespace FirstPlugin
{
    public class BFRES : BFRESWrapper, IFileFormat, ITextureContainer, IExportableModelContainer, IDisposable
    {
        public FileType FileType { get; set; } = FileType.Resource;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "BFRES" };
        public string[] Extension { get; set; } = new string[] {
            "*.bfres", "*.sbfres", "*.sbmapopen", "*.sbstftex", "*.sbitemico", "*.sbmaptex", "*.sbreviewtex" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                return reader.CheckSignature(4, "FRES");
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                types.Add(typeof(MenuExt));
                return types.ToArray();
            }
        }

        //Determines if the binary is in a PTCL binary file.
        public bool IsParticlePrimitive = false;

        public bool DisplayIcons => false;

        public List<STGenericTexture> TextureList
        {
            get { return GetTextures(); }
            set { }
        }

        public IEnumerable<STGenericModel> ExportableModels
        {
            get { return BFRESRender.models; }
        }

        public IEnumerable<STGenericTexture> ExportableTextures => TextureList;

        public override string ExportFilter => Utils.GetAllFilters(this);

        //Stores the skeleton and models in this
        public DrawableContainer DrawableContainer = new DrawableContainer();

        class MenuExt : IFileMenuExtension
        {
            public STToolStripItem[] NewFileMenuExtensions => newFileExt;
            public STToolStripItem[] NewFromFileMenuExtensions => null;
            public STToolStripItem[] EditMenuExtensions => editExt;
            public STToolStripItem[] ToolsMenuExtensions => toolExt;
            public STToolStripItem[] TitleBarExtensions => null;
            public STToolStripItem[] CompressionMenuExtensions => null;
            public STToolStripItem[] ExperimentalMenuExtensions => null;
            public ToolStripButton[] IconButtonMenuExtensions => null;

            STToolStripItem[] toolExt = new STToolStripItem[1];
            STToolStripItem[] newFileExt = new STToolStripItem[2];
            STToolStripItem[] editExt = new STToolStripItem[1];

            public MenuExt()
            {
             //   toolExt[0] = new STToolStripItem("Models");
            //    toolExt[0].DropDownItems.Add(new STToolStripItem("Batch Export (BFRES)", Export));

                editExt[0] = new STToolStripItem("Use Advanced Editor As Default", AdvancedEditor);
                newFileExt[0] = new STToolStripItem("BFRES (Switch)", NewSwitchBfres);
                newFileExt[1] = new STToolStripItem("BFRES (Wii U)", NewWiiUBfres);
                editExt[0].Checked = !PluginRuntime.UseSimpleBfresEditor;
            }

            private void AdvancedEditor(object sender, EventArgs args)
            {
                BFRES file = null;

                ObjectEditor editor = (ObjectEditor)LibraryGUI.GetActiveForm();
                if (editor != null)
                {
                    file = (BFRES)editor.GetActiveFile();
                }

                if (editExt[0].Checked)
                {
                    editExt[0].Checked = false;
                    PluginRuntime.UseSimpleBfresEditor = true;

                    if (file != null)
                        file.LoadSimpleMode();
                }
                else
                {
                    editExt[0].Checked = true;
                    PluginRuntime.UseSimpleBfresEditor = false;

                    if (file != null)
                        file.LoadAdvancedMode();
                }
            }
            private void NewWiiUBfres(object sender, EventArgs args)
            {
                BFRES bfres = new BFRES();
                bfres.IFileInfo = new IFileInfo();
                bfres.FileName = "Untitled.bfres";

                bfres.Load(new MemoryStream(BfresWiiU.CreateNewBFRES("Untitled.bfres")));

                ObjectEditor editor = new ObjectEditor(bfres);
                editor.Text = "Untitled-" + 0;
                LibraryGUI.CreateMdiWindow(editor);
            }
            private void NewSwitchBfres(object sender, EventArgs args)
            {
                BFRES bfres = new BFRES();
                bfres.IFileInfo = new IFileInfo();
                bfres.FileName = "Untitled.bfres";

                bfres.Load(new MemoryStream(CreateNewBFRESSwitch("Untitled.bfres")));

                ObjectEditor editor = new ObjectEditor(bfres);
                editor.Text = "Untitled-" + 0;
                LibraryGUI.CreateMdiWindow(editor);
            }
            private void DebugInfo(object sender, EventArgs args)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = Utils.GetAllFilters(new BFRES());

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                var debugInfo = new DebugInfoBox();
                debugInfo.Show();
                debugInfo.PrintDebugInfo(ofd.FileName);
            }

            private void Export(object sender, EventArgs args)
            {
                string formats = FileFilters.FMDL_EXPORT;

                string[] forms = formats.Split('|');

                List<string> Formats = new List<string>();
                for (int i = 0; i < forms.Length; i++)
                {
                    if (i > 1 || i == (forms.Length - 1)) //Skip lines with all extensions
                    {
                        if (!forms[i].StartsWith("*"))
                            Formats.Add(forms[i]);
                    }
                }

                BatchFormatExport form = new BatchFormatExport(Formats);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    string Extension = form.GetSelectedExtension();

                    OpenFileDialog ofd = new OpenFileDialog();
                    ofd.Multiselect = true;
                    ofd.Filter = Utils.GetAllFilters(new Type[] { typeof(BFRES), typeof(SARC) });

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        FolderSelectDialog folderDialog = new FolderSelectDialog();
                        if (folderDialog.ShowDialog() == DialogResult.OK)
                        {
                            foreach (string file in ofd.FileNames)
                            {
                                var FileFormat = STFileLoader.OpenFileFormat(file, new Type[] { typeof(BFRES), typeof(SARC) });
                                if (FileFormat == null)
                                    continue;

                                SearchBinary(FileFormat, folderDialog.SelectedPath, Extension);
                            }
                        }
                    }
                }
            }

            private void SearchBinary(IFileFormat FileFormat, string Folder, string Extension)
            {
                if (FileFormat is SARC)
                {
                    foreach (var file in ((SARC)FileFormat).Files)
                    {
                        var archiveFile = STFileLoader.OpenFileFormat(file.FileName, new Type[] { typeof(BFRES), typeof(SARC) }, file.FileData);
                        if (archiveFile == null)
                            continue;

                        SearchBinary(archiveFile, Folder, Extension);
                    }
                }
                if (FileFormat is BFRES)
                {
                 //   ((BFRES)FileFormat).Export(Path.Combine(Folder, $"{FileFormat.FileName}{Extension}"));
                }

                FileFormat.Unload();
            }
        }

        private static byte[] CreateNewBFRESSwitch(string Name)
        {
            MemoryStream mem = new MemoryStream();

            ResFile resFile = new ResFile();
            resFile.Name = Name;

            resFile.Save(mem);
            var data = mem.ToArray();

            mem.Close();
            mem.Dispose();
            return data;
        }

        public BoundingBox GetBoundingBox()
            {
            Vector3 Min = new Vector3(0);
            Vector3 Max = new Vector3(0);

            var Models = GetModels();
            if (Models == null)
                return new BoundingBox();

            foreach (FMDL model in Models)
            {
                foreach (var shape in model.shapes)
                {
                    foreach (var vertex in shape.vertices)
                    {
                        Min.X = Math.Min(Min.X, vertex.pos.X);
                        Min.Y = Math.Min(Min.Y, vertex.pos.Y);
                        Min.Z = Math.Min(Min.Z, vertex.pos.Z);
                        Max.X = Math.Max(Max.X, vertex.pos.X);
                        Max.Y = Math.Max(Max.Y, vertex.pos.Y);
                        Max.Z = Math.Max(Max.Z, vertex.pos.Z);
                    }
                }
            }

            return new BoundingBox()
            {
                Max = Max,
                Min = Min,
            };
        }

        public void OnPropertyChanged()
        {
            if (resFile != null)
            {
                Text = resFile.Name;
            }
            else
            {
                Text = resFileU.Name;
            }
            STPropertyGrid editor = (STPropertyGrid)LibraryGUI.GetActiveContent(typeof(STPropertyGrid));

            if (editor != null)
                editor.Refresh();
        }

        public void LoadAdvancedMode()
        {
            foreach (var model in BFRESRender.models)
            {
                foreach (var mat in model.materials.Values)
                {
                    foreach (var tex in mat.TextureMaps)
                    {
                        mat.Nodes.RemoveByKey(tex.Name);
                    }
                }
            }
        }

        public void LoadSimpleMode()
        {
            ObjectEditor editor = (ObjectEditor)LibraryGUI.GetActiveForm();
            if (editor == null)
                return;

            editor.BeginUpdate();

            foreach (var model in BFRESRender.models)
            {
                foreach (var mat in model.materials.Values)
                {
                    mat.Nodes.Clear();

                    foreach (MatTexture tex in mat.TextureMaps)
                    {
                        mat.Nodes.Add(new MatTextureWrapper(tex.Name, tex.Name, tex));
                    }
                }
            }

            foreach (TreeNode node in Nodes)
            {
               if (node is BFRESAnimFolder)
                {
                    foreach (BFRESGroupNode animFolder in node.Nodes)
                    {
                        if (animFolder.Type == BRESGroupType.SkeletalAnim)
                        {
                            foreach (FSKA anim in animFolder.Nodes)
                            {
                                foreach (FSKA.BoneAnimNode bone in ((FSKA)anim).Bones)
                                {
                                    int index = 0;
                                    if (bone.BoneAnimU != null)
                                    {
                                        foreach (var curve in bone.BoneAnimU.Curves)
                                            GetSkeletonAnimCurveOffset(curve.AnimDataOffset);
                                    }
                                    else
                                    {
                                        foreach (var curve in bone.BoneAnim.Curves)
                                            GetSkeletonAnimCurveOffset(curve.AnimDataOffset);
                                    }
                                }
                            }
                        }
                        if (animFolder.Type == BRESGroupType.ColorAnim)
                        {
                            foreach (var anim in animFolder.Nodes)
                            {
                                if (anim is FMAA)
                                {
                                    foreach (FMAA.MaterialAnimEntry mat in ((FMAA)anim).Nodes)
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
            }

            editor.EndUpdate();
        }

        private string GetSkeletonAnimCurveOffset(uint offset)
        {
            switch ((FSKA.TrackType)offset)
            {
                case FSKA.TrackType.XPOS: return "Translate_X";
                case FSKA.TrackType.YPOS: return "Translate_Y";
                case FSKA.TrackType.ZPOS: return "Translate_Z";
                case FSKA.TrackType.XROT: return "Rotate_X";
                case FSKA.TrackType.YROT: return "Rotate_Y";
                case FSKA.TrackType.ZROT: return "Rotate_Z";
                case FSKA.TrackType.WROT: return "Rotate_W";
                case FSKA.TrackType.XSCA: return "Scale_X";
                case FSKA.TrackType.YSCA: return "Scale_Y";
                case FSKA.TrackType.ZSCA: return "Scale_Z";
                default:  offset.ToString(); break;
            }

            return "";
        }

        public bool HasShapes()
        {
            BFRESRender.UpdateModelList();
            for (int i = 0; i < BFRESRender.models.Count; i++)
            {
                if (BFRESRender.models[i].shapes.Count > 0)
                    return true;
            }
            return false;
        }

        public STForm ActiveFormEditor; //For active form windows as editors

        private bool DrawablesLoaded = false;
        public void LoadEditors(object SelectedSection)
        {
            Console.WriteLine($"SelectedSection {SelectedSection}");

            BfresEditor bfresEditor = (BfresEditor)LibraryGUI.GetActiveContent(typeof(BfresEditor));
            bool HasModels = false;
            bool hasShapes = HasShapes();

            if (bfresEditor == null)
            {
                HasModels = BFRESRender.models.Count > 0;

                bfresEditor = new BfresEditor(HasModels);
                bfresEditor.Dock = DockStyle.Fill;
                LibraryGUI.LoadEditor(bfresEditor);
            }

            bool ViewportToggled = bfresEditor.DisplayViewport;

            if (SelectedSection is FTEX)
            {
                ImageEditorBase editorFtex = (ImageEditorBase)bfresEditor.GetActiveEditor(typeof(ImageEditorBase));
                if (editorFtex == null)
                {
                    editorFtex = new ImageEditorBase();
                    editorFtex.Dock = DockStyle.Fill;

                    bfresEditor.LoadEditor(editorFtex);
                }
                editorFtex.Text = Text;
                editorFtex.LoadProperties(((FTEX)SelectedSection).texture);
                editorFtex.LoadImage((FTEX)SelectedSection);
                if (Runtime.DisplayViewport && ViewportToggled)
                    editorFtex.SetEditorOrientation(true);

                if (((FTEX)SelectedSection).texture.UserData != null)
                {
                    UserDataEditor userEditor = (UserDataEditor)editorFtex.GetActiveTabEditor(typeof(UserDataEditor));
                    if (userEditor == null)
                    {
                        userEditor = new UserDataEditor();
                        userEditor.Name = "User Data";
                        editorFtex.AddCustomControl(userEditor, typeof(UserDataEditor));
                    }
                    userEditor.LoadUserData(((FTEX)SelectedSection).texture.UserData);
                }
                return;
            }

            if (SelectedSection is TextureData)
            {
                ImageEditorBase editor = (ImageEditorBase)bfresEditor.GetActiveEditor(typeof(ImageEditorBase));
                if (editor == null)
                {
                    editor = new ImageEditorBase();
                    editor.Dock = DockStyle.Fill;
                    bfresEditor.LoadEditor(editor);
                }
                if (((TextureData)SelectedSection).Texture.UserData != null)
                {
                    UserDataEditor userEditor = (UserDataEditor)editor.GetActiveTabEditor(typeof(UserDataEditor));
                    if (userEditor == null)
                    {
                        userEditor = new UserDataEditor();
                        userEditor.Name = "User Data";
                        editor.AddCustomControl(userEditor, typeof(UserDataEditor));
                    }
                    userEditor.LoadUserData(((TextureData)SelectedSection).Texture.UserData.ToList());
                }

                editor.Text = Text;
                if (Runtime.DisplayViewport && ViewportToggled)
                    editor.SetEditorOrientation(true);

                editor.LoadProperties(((TextureData)SelectedSection).Texture);
                editor.LoadImage((TextureData)SelectedSection);
                return;
            }

            if (SelectedSection is BNTX)
            {
                STPropertyGrid editor = (STPropertyGrid)bfresEditor.GetActiveEditor(typeof(STPropertyGrid));
                if (editor == null)
                {
                    editor = new STPropertyGrid();
                    editor.Dock = DockStyle.Fill;
                    bfresEditor.LoadEditor(editor);
                }
                editor.LoadProperty(((BNTX)SelectedSection).BinaryTexFile, OnPropertyChanged);
                return;
            }

            if (SelectedSection is ExternalFileData)
            {
                ArchiveFilePanel editor = (ArchiveFilePanel)LibraryGUI.GetActiveContent(typeof(ArchiveFilePanel));
                if (editor == null)
                {
                    editor = new ArchiveFilePanel();
                    editor.Dock = DockStyle.Fill;
                    LibraryGUI.LoadEditor(editor);
                }

                editor.LoadFile(((ExternalFileData)SelectedSection).ArchiveFileInfo);
                editor.UpdateEditor();
                return;
            }

            if (!DrawablesLoaded)
            {
                ObjectEditor.AddContainer(DrawableContainer);
                DrawablesLoaded = true;
            }

            if (Runtime.UseOpenGL)
                bfresEditor.LoadViewport(this, hasShapes, DrawableContainer);

            if (SelectedSection is BFRES && hasShapes)
                bfresEditor.FrameCamera(BFRESRender);

            bool IsSimpleEditor = PluginRuntime.UseSimpleBfresEditor;
            if (IsSimpleEditor)
            {
                if (SelectedSection is MatTextureWrapper)
                {
                    SamplerEditorSimple editorT = (SamplerEditorSimple)bfresEditor.GetActiveEditor(typeof(SamplerEditorSimple));
                    if (editorT == null)
                    {
                        editorT = new SamplerEditorSimple();
                        editorT.Dock = DockStyle.Fill;
                        bfresEditor.LoadEditor(editorT);
                    }
                    editorT.Text = Text;
                    editorT.LoadTexture(((MatTextureWrapper)SelectedSection).textureMap);
                    return;
                }
           

                STPropertyGrid editor = (STPropertyGrid)bfresEditor.GetActiveEditor(typeof(STPropertyGrid));
                if (editor == null)
                {
                    editor = new STPropertyGrid();
                    editor.Dock = DockStyle.Fill;
                    bfresEditor.LoadEditor(editor);
                }
                editor.Text = Text;

                if (SelectedSection is BFRES)
                {
                    if (resFile != null)
                        editor.LoadProperty(resFile, OnPropertyChanged);
                    else
                        editor.LoadProperty(resFileU, OnPropertyChanged);
                }
                else if (SelectedSection is FMDL)
                {
                    if (((FMDL)SelectedSection).ModelU != null)
                    {
                        editor.LoadProperty(((FMDL)SelectedSection).ModelU, OnPropertyChanged);
                    }
                    else
                        editor.LoadProperty(((FMDL)SelectedSection).Model, OnPropertyChanged);
                }
                else if (SelectedSection is FSHP)
                {
                    if (((FSHP)SelectedSection).ShapeU != null)
                        editor.LoadProperty(((FSHP)SelectedSection).ShapeU, OnPropertyChanged);
                    else
                        editor.LoadProperty(((FSHP)SelectedSection).Shape, OnPropertyChanged);
                }
                else if (SelectedSection is FMAT)
                {
                    if (((FMAT)SelectedSection).MaterialU != null)
                        editor.LoadProperty(((FMAT)SelectedSection).MaterialU, OnPropertyChanged);
                    else
                        editor.LoadProperty(((FMAT)SelectedSection).Material, OnPropertyChanged);
                }
                else if (SelectedSection is BfresBone)
                {
                    if (((BfresBone)SelectedSection).BoneU != null)
                        editor.LoadProperty(((BfresBone)SelectedSection).BoneU, OnPropertyChanged);
                    else
                        editor.LoadProperty(((BfresBone)SelectedSection).Bone, OnPropertyChanged);
                }
                else if (SelectedSection is FSKL.fsklNode)
                {
                    if (((FSKL.fsklNode)SelectedSection).SkeletonU != null)
                        editor.LoadProperty(((FSKL.fsklNode)SelectedSection).SkeletonU, OnPropertyChanged);
                    else
                        editor.LoadProperty(((FSKL.fsklNode)SelectedSection).Skeleton, OnPropertyChanged);
                }
                else if (SelectedSection is FSKA)
                {
                    if (((FSKA)SelectedSection).SkeletalAnimU != null)
                        editor.LoadProperty(((FSKA)SelectedSection).SkeletalAnimU, OnPropertyChanged);
                    else
                        editor.LoadProperty(((FSKA)SelectedSection).SkeletalAnim, OnPropertyChanged);
                }
                else if (SelectedSection is FMAA)
                {
                    editor.LoadProperty(((FMAA)SelectedSection).MaterialAnim, OnPropertyChanged);
                }
                else
                    editor.LoadProperty(null, OnPropertyChanged);
            }
            else
            {
                if (SelectedSection is BFRES)
                {
                    STPropertyGrid editor = (STPropertyGrid)bfresEditor.GetActiveEditor(typeof(STPropertyGrid));
                    if (editor == null)
                    {
                        editor = new STPropertyGrid();
                        editor.Dock = DockStyle.Fill;
                        bfresEditor.LoadEditor(editor);
                    }
                    editor.Text = Text;

                    if (resFile != null)
                        editor.LoadProperty(resFile, OnPropertyChanged);
                    else
                        editor.LoadProperty(resFileU, OnPropertyChanged);
                }
                else if (SelectedSection is BFRESGroupNode)
                {
                    STPropertyGrid editor = (STPropertyGrid)bfresEditor.GetActiveEditor(typeof(STPropertyGrid));
                    if (editor == null)
                    {
                        editor = new STPropertyGrid();
                        editor.Dock = DockStyle.Fill;
                        bfresEditor.LoadEditor(editor);
                    }
                    editor.Text = Text;
                    editor.LoadProperty(null, null);
                }
                else if (SelectedSection is FSKL.fsklNode)
                {
                    FSKLEditor editor = (FSKLEditor)bfresEditor.GetActiveEditor(typeof(FSKLEditor));
                    if (editor == null)
                    {
                        editor = new FSKLEditor();
                        editor.Text = Text;
                        editor.Dock = DockStyle.Fill;
                        bfresEditor.LoadEditor(editor);
                    }
                    editor.LoadSkeleton(((FSKL.fsklNode)SelectedSection).fskl);
                }
                else if (SelectedSection is BfresBone)
                {
                    BfresBoneEditor editor = (BfresBoneEditor)bfresEditor.GetActiveEditor(typeof(BfresBoneEditor));
                    if (editor == null)
                    {
                        editor = new BfresBoneEditor();
                        editor.Text = Text;
                        editor.Dock = DockStyle.Fill;
                        bfresEditor.LoadEditor(editor);
                    }
                    editor.LoadBone((BfresBone)SelectedSection);
                }
                else if (SelectedSection is FSHP)
                {
                    BfresShapeEditor editor = (BfresShapeEditor)bfresEditor.GetActiveEditor(typeof(BfresShapeEditor));
                    if (editor == null)
                    {
                        editor = new BfresShapeEditor();
                        editor.Text = Text;
                        editor.Dock = DockStyle.Fill;
                        bfresEditor.LoadEditor(editor);
                    }
                    editor.LoadShape((FSHP)SelectedSection);
                }
                else if (SelectedSection is FMAT)
                {
                    FMATEditor editor = (FMATEditor)bfresEditor.GetActiveEditor(typeof(FMATEditor));
                    if (editor == null)
                    {
                        editor = new FMATEditor();
                        editor.Text = Text;
                        editor.Dock = DockStyle.Fill;
                        bfresEditor.LoadEditor(editor);
                    }
                    editor.LoadMaterial((FMAT)SelectedSection);
                }
                else if (SelectedSection is FSKA.BoneAnimNode)
                {
                    BoneAnimEditor editor = (BoneAnimEditor)bfresEditor.GetActiveEditor(typeof(BoneAnimEditor));
                    if (editor == null)
                    {
                        editor = new BoneAnimEditor();
                        editor.Text = Text;
                        editor.Dock = DockStyle.Fill;
                        bfresEditor.LoadEditor(editor);
                    }
                    editor.LoadBoneAnim((FSKA.BoneAnimNode)SelectedSection);
                }
                else if (SelectedSection is FSCN.BfresCameraAnim)
                {
                    SceneAnimEditor editor = (SceneAnimEditor)bfresEditor.GetActiveEditor(typeof(SceneAnimEditor));
                    if (editor == null)
                    {
                        editor = new SceneAnimEditor();
                        editor.Text = Text;
                        editor.Dock = DockStyle.Fill;
                        bfresEditor.LoadEditor(editor);
                    }
                    editor.LoadCameraAnim((FSCN.BfresCameraAnim)SelectedSection);
                }
                else if (SelectedSection is FSCN.BfresLightAnim)
                {
                    SceneAnimEditor editor = (SceneAnimEditor)bfresEditor.GetActiveEditor(typeof(SceneAnimEditor));
                    if (editor == null)
                    {
                        editor = new SceneAnimEditor();
                        editor.Text = Text;
                        editor.Dock = DockStyle.Fill;
                        bfresEditor.LoadEditor(editor);
                    }
                    editor.LoadLightAnim((FSCN.BfresLightAnim)SelectedSection);
                }
                else if (SelectedSection is FSCN.BfresFogAnim)
                {
                    SceneAnimEditor editor = (SceneAnimEditor)bfresEditor.GetActiveEditor(typeof(SceneAnimEditor));
                    if (editor == null)
                    {
                        editor = new SceneAnimEditor();
                        editor.Text = Text;
                        editor.Dock = DockStyle.Fill;
                        bfresEditor.LoadEditor(editor);
                    }
                    editor.LoadFogAnim((FSCN.BfresFogAnim)SelectedSection);
                }
                else if (SelectedSection is FMDL)
                    OpenSubFileEditor<FMDL>(SelectedSection, bfresEditor);
                else if (SelectedSection is FSKA)
                    OpenSubFileEditor<FSKA>(SelectedSection, bfresEditor);
                else if (SelectedSection is FSHU)
                    OpenSubFileEditor<FSHU>(SelectedSection, bfresEditor);
                else if (SelectedSection is FSCN)
                    OpenSubFileEditor<FSCN>(SelectedSection, bfresEditor);
                else if (SelectedSection is FSHA)
                    OpenSubFileEditor<FSHA>(SelectedSection, bfresEditor);
                else if (SelectedSection is FTXP)
                    OpenSubFileEditor<FTXP>(SelectedSection, bfresEditor);
                else if (SelectedSection is FMAA)
                    OpenSubFileEditor<FMAA>(SelectedSection, bfresEditor);
                else if (SelectedSection is FVIS)
                    OpenSubFileEditor<FVIS>(SelectedSection, bfresEditor);
            }
        }

        private SubFileEditor OpenSubFileEditor<T>(object node, BfresEditor bfresEditor) where T : STGenericWrapper
        {
            SubFileEditor editor = (SubFileEditor)bfresEditor.GetActiveEditor(typeof(SubFileEditor));
            if (editor == null)
            {
                editor = new SubFileEditor();
                editor.Dock = DockStyle.Fill;
                bfresEditor.LoadEditor(editor);
            }
            editor.LoadSubFile<T>(node);

            return editor;
        }

        public static bool CheckWiiU(Stream stream)
        {
            bool IsWiiU = false;
            using (FileReader reader = new FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                reader.Position = 4;

                if (reader.ReadInt32() != 0x20202020)
                    IsWiiU = true;

                reader.Position = 0;
            }
            return IsWiiU;
        }

        public BFRESRenderBase BFRESRender;

        private MeshCodec MeshCodec;

        public void Dispose() { MeshCodec.Dispose(); }

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            ImageKey = "bfres";
            SelectedImageKey = "bfres";

            IsWiiU = CheckWiiU(stream);
            LoadMenus(IsWiiU);
       
            BFRESRender = new BFRESRender();
            DrawableContainer.Name = FileName;

            BFRESRender.ModelTransform = MarioCostumeEditor.SetTransform(FileName);
            BFRESRender.ResFileNode = this;

            MeshCodec = new MeshCodec();

            var externalFlags = MeshCodec.GetExternalFlags(stream);
            //External flags used
            if (externalFlags.HasFlag(MeshCodec.ExternalFlags.HasExternalGPU) || this.FileName.EndsWith(".mc"))
            {
                //Ensure it uses mc compressor for save
                this.IFileInfo.FileIsCompressed = true;
                if (this.IFileInfo.FileCompression == null)
                {
                    this.IFileInfo.FileCompression = new MeshCodecFormat();
                    if (!this.FileName.EndsWith(".mc"))
                        this.FileName += ".mc";
                    if (!this.FilePath.EndsWith(".mc"))
                        this.FilePath += ".mc";
                }
            }
            if (externalFlags.HasFlag(MeshCodec.ExternalFlags.HasExternalString))
                MeshCodec.Prepare();

            if (IsWiiU)
            {
                LoadFile(new Syroot.NintenTools.Bfres.ResFile(stream));
            }
            else
            {
                LoadFile(new Syroot.NintenTools.NSW.Bfres.ResFile(stream));
            }

            if (resFileU != null) {
                if (resFileU.VersioFull == "3,0,0,1")
                {
                    //Todo check for valid sharc files to parse as programs via materials to map

                 //   Console.WriteLine("RedPro_Renderer!");
                  /*  BFRESRender = new RedPro_Renderer();
                    BFRESRender.ModelTransform = Matrix4.Identity;
                    BFRESRender.ResFileNode = this;*/
                }
            }

            //Mesh codec type of bfres, load textures externallly
            if (externalFlags != (MeshCodec.ExternalFlags)0)
            {
                MeshCodec.PrepareTexToGo(resFile);
                MeshCodec.TextureFolder = new TexToGoFolder(MeshCodec);
                this.Nodes.Add(MeshCodec.TextureFolder);
            }

            DrawableContainer.Drawables.Add(BFRESRender);

            var Models = GetModels();
            if (Models != null)
            {
                foreach (FMDL mdl in Models)
                {
                    BFRESRender.models.Add(mdl);
                    DrawableContainer.Drawables.Add(mdl.Skeleton);
                }
            }
        }

        public void Unload()
        {
            BFRESRender.Destroy();
            DrawableContainer.Drawables.Clear();

            ObjectEditor.RemoveContainer(DrawableContainer);

            if (resFile != null)
            {
                resFile.Models.Clear();
                resFile.SkeletalAnims.Clear();
                resFile.MaterialAnims.Clear();
                resFile.SceneAnims.Clear();
                resFile.ShapeAnims.Clear();
                resFile.BoneVisibilityAnims.Clear();
                resFile.ModelDict.Clear();
                resFile.SkeletalAnimDict.Clear();
                resFile.MaterialAnimDict.Clear();
                resFile.SceneAnimDict.Clear();
                resFile.ShapeAnimDict.Clear();
                resFile.BoneVisibilityAnimDict.Clear();
                resFile.ExternalFiles.Clear();
                resFile.ExternalFileDict.Clear();
            }
            else if (resFileU != null)
            {
                resFileU.Models.Clear();
                resFileU.Textures.Clear();
                resFileU.SkeletalAnims.Clear();
                resFileU.ShaderParamAnims.Clear();
                resFileU.ColorAnims.Clear();
                resFileU.TexSrtAnims.Clear();
                resFileU.TexPatternAnims.Clear();
                resFileU.BoneVisibilityAnims.Clear();
                resFileU.MatVisibilityAnims.Clear();
                resFileU.ShapeAnims.Clear();
                resFileU.SceneAnims.Clear();
                resFileU.ExternalFiles.Clear();
            }

            foreach (var node in TreeViewExtensions.Collect(BFRESRender.ResFileNode.Nodes))
            {
                if (node is BFRESGroupNode)
                {
                    if (((BFRESGroupNode)node).Type == BRESGroupType.Models)
                    {
                        for (int i = 0; i < ((BFRESGroupNode)node).Nodes.Count; i++)
                        {
                            FMDL model = ((FMDL)((BFRESGroupNode)node).Nodes[i]);
                            for (int shp = 0; shp < model.shapes.Count; shp++)
                            {
                                model.shapes[shp].vertices.Clear();
                                model.shapes[shp].faces.Clear();
                                foreach (var lod in model.shapes[shp].lodMeshes)
                                    lod.faces.Clear();
                            }
                        }
                    }
                    if (((BFRESGroupNode)node).Type == BRESGroupType.Textures)
                    {
                        for (int i = 0; i < ((BFRESGroupNode)node).Nodes.Count; i++)
                            ((FTEX)((BFRESGroupNode)node).Nodes[i]).Unload();
                        
                        if (PluginRuntime.ftexContainers.Contains(((BFRESGroupNode)node)))
                            PluginRuntime.ftexContainers.Remove(((BFRESGroupNode)node));
                    }

                    ((BFRESGroupNode)node).ResourceNodes.Clear();
                    ((BFRESGroupNode)node).Nodes.Clear();
                }

                if (node is BNTX)
                    ((BNTX)node).Unload();
            }
            Nodes.Clear();

            GC.SuppressFinalize(this);
        }

        public void Save(Stream stream)
        {
            //Force mesh codec compression on save 
            if (this.FilePath != null && this.FilePath.EndsWith(".mc"))
            {
                this.IFileInfo.FileCompression = new MeshCodecFormat();
                this.IFileInfo.FileIsCompressed = true;
            }

            var Models = GetModels();
            if (Models != null && !IsParticlePrimitive)
            {
                foreach (FMDL mdl in Models)
                {
                    for (int s = 0; s < mdl.shapes.Count; s++)
                    {
                        SetShaderAssignAttributes(mdl.shapes[s]);
                    }
                }
            }


            if (IsWiiU)
                SaveWiiU(stream);
            else
                SaveSwitch(stream);

            if (MeshCodec.TextureList.Count > 0)
                MeshCodec.SaveTexToGo();
        }

        public TreeNodeCollection GetModels()
        {
            foreach (var folder in Nodes)
            {
                if (folder is BFRESGroupNode && ((BFRESGroupNode)folder).Type == BRESGroupType.Models)
                    return ((BFRESGroupNode)folder).Nodes;
            }

            return null;
        }

        public List<STGenericTexture> GetTextures()
        {
            List<STGenericTexture> textures = new List<STGenericTexture>();
            var bntx = GetBNTX;
            if (bntx != null)
                return bntx.TextureList;

            foreach (TreeNode folder in Nodes)
            {
                if (folder is BFRESGroupNode && ((BFRESGroupNode)folder).Type == BRESGroupType.Textures)
                {
                    foreach (STGenericTexture node in folder.Nodes)
                        textures.Add(node);
                }
            }

            return textures;
        }

        public ResFile resFile = null;
        public ResU.ResFile resFileU = null;

        public override void OnClick(TreeView treeView)
        {
            LoadEditors(this);
        }

        public BFRESGroupNode GetFTEXContainer
        {
            get
            {
                foreach (TreeNode folder in Nodes)
                {
                    if (folder is BFRESGroupNode)
                    {
                        if (((BFRESGroupNode)folder).Type == BRESGroupType.Textures)
                            return (BFRESGroupNode)folder;
                    }
                }
                return null;
            }
        }

        public BNTX GetBNTX
        {
            get
            {
                foreach (TreeNode folder in Nodes)
                {
                    if (folder is BNTX)
                        return (BNTX)folder;
                }
                return null;
            }
        }

        public bool HasTextures
        {
            get
            {
                foreach (TreeNode folder in Nodes)
                {
                    if (folder is BFRESGroupNode)
                    {
                        bool hasTextures = (((BFRESGroupNode)folder).Type == BRESGroupType.Textures &&
                                folder.Nodes.Count > 0);
                        if (hasTextures) return true;
                    }
                    if (folder is BNTX)
                    {
                        return ((BNTX)folder).Textures.Count > 0;
                    }
                }
                return false;
            }
        }

        public void LoadFile(ResU.ResFile res)
        {
            resFileU = res;

            Text = resFileU.Name;

            var modelFolder = new BFRESGroupNode(BRESGroupType.Models, true);
            var texturesFolder = new BFRESGroupNode(BRESGroupType.Textures, true);
            var animFolder = new BFRESAnimFolder();
            var externalFilesFolder = new BFRESGroupNode(BRESGroupType.Embedded, true);

            texturesFolder.ShowNewContextMenu = false;

            Nodes.Add(modelFolder);
            Nodes.Add(texturesFolder);
            Nodes.Add(animFolder);
            Nodes.Add(externalFilesFolder);

            animFolder.LoadMenus(IsWiiU);
            PluginRuntime.ftexContainers.Add(texturesFolder);

            if (resFileU.Models.Count > 0)
            {
                for (int i = 0; i < resFileU.Models.Count; i++)
                {
                    var fmdl = new FMDL();
                    BfresWiiU.ReadModel(fmdl, resFileU.Models[i]);
                    modelFolder.AddNode(fmdl);
                }
            }
            if (resFileU.Textures.Count > 0)
            {
                for (int i = 0; i < resFileU.Textures.Count; i++)
                {
                    var ftex = new FTEX(resFileU.Textures[i]);
                    texturesFolder.AddNode(ftex);
                    ftex.UpdateMipMaps();
                }
            }
            if (resFileU.SkeletalAnims.Count > 0)
            {
                var group = new BFRESGroupNode(BRESGroupType.SkeletalAnim, true);
                animFolder.Nodes.Add(group);

                for (int i = 0; i < resFileU.SkeletalAnims.Count; i++)
                    group.AddNode(new FSKA(resFileU.SkeletalAnims[i]));
            }
            if (resFileU.ShaderParamAnims.Count > 0)
            {
                var group = new BFRESGroupNode(BRESGroupType.ShaderParamAnim, true);
                animFolder.Nodes.Add(group);

                for (int i = 0; i < resFileU.ShaderParamAnims.Count; i++)
                    group.AddNode(new FSHU(resFileU.ShaderParamAnims[i], MaterialAnimation.AnimationType.ShaderParam));
            }
            if (resFileU.ColorAnims.Count > 0)
            {
                var group = new BFRESGroupNode(BRESGroupType.ColorAnim, true);
                animFolder.Nodes.Add(group);

                for (int i = 0; i < resFileU.ColorAnims.Count; i++)
                    group.AddNode(new FSHU(resFileU.ColorAnims[i], MaterialAnimation.AnimationType.Color));
            }
            if (resFileU.TexSrtAnims.Count > 0)
            {
                var group = new BFRESGroupNode(BRESGroupType.TexSrtAnim, true);
                animFolder.Nodes.Add(group);

                for (int i = 0; i < resFileU.TexSrtAnims.Count; i++)
                    group.AddNode(new FSHU(resFileU.TexSrtAnims[i], MaterialAnimation.AnimationType.TextureSrt));
            }
            if (resFileU.TexPatternAnims.Count > 0)
            {
                var group = new BFRESGroupNode(BRESGroupType.TexPatAnim, true);
                animFolder.Nodes.Add(group);

                for (int i = 0; i < resFileU.TexPatternAnims.Count; i++)
                    group.AddNode(new FTXP(resFileU.TexPatternAnims[i]));
            }
            if (resFileU.ShapeAnims.Count > 0)
            {
                var group = new BFRESGroupNode(BRESGroupType.ShapeAnim, true);
                animFolder.Nodes.Add(group);

                for (int i = 0; i < resFileU.ShapeAnims.Count; i++)
                    group.AddNode(new FSHA(resFileU.ShapeAnims[i]));
            }
            if (resFileU.BoneVisibilityAnims.Count > 0)
            {
                var group = new BFRESGroupNode(BRESGroupType.BoneVisAnim, true);
                animFolder.Nodes.Add(group);

                for (int i = 0; i < resFileU.BoneVisibilityAnims.Count; i++)
                    group.AddNode(new FVIS(resFileU.BoneVisibilityAnims[i]));
            }
            if (resFileU.MatVisibilityAnims.Count > 0)
            {
                var group = new BFRESGroupNode(BRESGroupType.MatVisAnim, true);
                animFolder.Nodes.Add(group);

                for (int i = 0; i < resFileU.MatVisibilityAnims.Count; i++)
                    group.AddNode(new FVIS(resFileU.MatVisibilityAnims[i]));
            }
            if (resFileU.SceneAnims.Count > 0)
            {
                var group = new BFRESGroupNode(BRESGroupType.SceneAnim, true);
                animFolder.Nodes.Add(group);

                for (int i = 0; i < resFileU.SceneAnims.Count; i++)
                    group.AddNode(new FSCN(resFileU.SceneAnims[i]));
            }
            if (resFileU.ExternalFiles.Count > 0)
            {
                foreach (var anim in resFileU.ExternalFiles)
                {
                    externalFilesFolder.AddNode(new ExternalFileData(anim.Key, anim.Value.Data));
                }
            }

            if (PluginRuntime.UseSimpleBfresEditor)
                LoadSimpleMode();
        }
        public void LoadFile(ResFile res)
        {
            resFile = res;

            Text = resFile.Name;

            var modelFolder = new BFRESGroupNode(BRESGroupType.Models);
            var texturesFolder = new BNTX() { ImageKey = "folder", SelectedImageKey = "folder", Text = "Textures" };
            var animFolder = new BFRESAnimFolder();
            var externalFilesFolder = new BFRESGroupNode(BRESGroupType.Embedded);

            //Texture folder acts like a bntx for saving back
            //This will only save if the user adds textures to it or the file has a bntx already
            texturesFolder.IFileInfo = new IFileInfo();
            texturesFolder.FileName = "Textures";
            texturesFolder.Load(new MemoryStream(BNTX.CreateNewBNTX("Textures")));

            animFolder.LoadMenus(IsWiiU);

            Nodes.Add(modelFolder);
            Nodes.Add(texturesFolder);
            Nodes.Add(animFolder);
            Nodes.Add(externalFilesFolder);

            if (resFile.Models.Count > 0)
            {
                for (int i = 0; i < resFile.Models.Count; i++)
                {
                    var fmdl = new FMDL();
                    BfresSwitch.ReadModel(fmdl, resFile.Models[i]);
                    modelFolder.AddNode(fmdl);
                }
            }
            if (resFile.SkeletalAnims.Count > 0)
            {
                var group = new BFRESGroupNode(BRESGroupType.SkeletalAnim);
                animFolder.Nodes.Add(group);

                for (int i = 0; i < resFile.SkeletalAnims.Count; i++)
                    group.AddNode(new FSKA(resFile.SkeletalAnims[i]));
            }
            if (resFile.MaterialAnims.Count > 0)
            {
                var group = new BFRESGroupNode(BRESGroupType.ShaderParamAnim);
                var group2 = new BFRESGroupNode(BRESGroupType.TexSrtAnim);
                var group3 = new BFRESGroupNode(BRESGroupType.TexPatAnim);
                var group4 = new BFRESGroupNode(BRESGroupType.ColorAnim);
                var group5 = new BFRESGroupNode(BRESGroupType.MatVisAnim);
                var group6 = new BFRESGroupNode(BRESGroupType.MaterialAnim);

                bool HasShaderParamsAnim = false;
                bool HasTextureSrtAnim = false;
                bool HasTexturePatternAnim = false;
                bool HasColorAnim = false;
                bool HasMatVisAnim = false;
                bool HasMaterialAnim = false;

                for (int i = 0; i < resFile.MaterialAnims.Count; i++)
                {
                    var anim = resFile.MaterialAnims[i];
                    var fmaa = new FMAA(anim);
                    if (fmaa.AnimType == MaterialAnimation.AnimationType.ShaderParam)
                    {
                        group.AddNode(fmaa);
                        HasShaderParamsAnim = true;
                    }
                    else if (fmaa.AnimType == MaterialAnimation.AnimationType.TextureSrt)
                    {
                        group2.AddNode(fmaa);
                        HasTextureSrtAnim = true;
                    }
                    else if (fmaa.AnimType == MaterialAnimation.AnimationType.TexturePattern)
                    {
                        group3.AddNode(fmaa);
                        HasTexturePatternAnim = true;
                    }
                    else if (fmaa.AnimType == MaterialAnimation.AnimationType.Color)
                    {
                        group4.AddNode(fmaa);
                        HasColorAnim = true;
                    }
                    else if (fmaa.AnimType == MaterialAnimation.AnimationType.Visibilty)
                    {
                        group5.AddNode(fmaa);
                        HasMatVisAnim = true;
                    }
                    else
                    {
                        group.AddNode(fmaa);
                        HasMaterialAnim = true;
                    }
                }

                if (HasShaderParamsAnim)
                    animFolder.Nodes.Add(group);
                if (HasTextureSrtAnim)
                    animFolder.Nodes.Add(group2);
                if (HasTexturePatternAnim)
                    animFolder.Nodes.Add(group3);
                if (HasColorAnim)
                    animFolder.Nodes.Add(group4);
                if (HasMatVisAnim)
                    animFolder.Nodes.Add(group5);
                if (HasMaterialAnim)
                    animFolder.Nodes.Add(group6);
            }
            if (resFile.BoneVisibilityAnims.Count > 0)
            {
                var group = new BFRESGroupNode(BRESGroupType.BoneVisAnim);
                animFolder.Nodes.Add(group);

                for (int i = 0; i < resFile.BoneVisibilityAnims.Count; i++)
                    group.AddNode(new FVIS(resFile.BoneVisibilityAnims[i]));
            }
            if (resFile.SceneAnims.Count > 0)
            {
                var group = new BFRESGroupNode(BRESGroupType.SceneAnim);
                animFolder.Nodes.Add(group);

                for (int i = 0; i < resFile.SceneAnims.Count; i++)
                    group.AddNode(new FSCN(resFile.SceneAnims[i]));
            }
            if (resFile.ShapeAnims.Count > 0)
            {
                var group = new BFRESGroupNode(BRESGroupType.ShapeAnim);
                animFolder.Nodes.Add(group);

                for (int i = 0; i < resFile.ShapeAnims.Count; i++)
                    group.AddNode(new FSHA(resFile.ShapeAnims[i]));
            }
            if (resFile.ExternalFiles.Count > 0)
            {
                int index = 0;

                //This also can be set to true if we don't want to rebuild the bntx for debug purposes
                bool IsTexturesReplaced = false;

                foreach (var anim in resFile.ExternalFiles)
                {
                    if (anim.Data == null) anim.Data = new byte[0];

                    // group.AddNode(new ExternalFileData(Name, anim.Data) { FileFormat = file });

                    string Name = resFile.ExternalFileDict.GetKey(index++);

                    //Bfsha changes versions alot so ignore these for now
                    if (Utils.GetExtension(Name) == ".bfsha")
                    {
                        externalFilesFolder.AddNode(new ExternalFileData(Name, anim.Data));
                        continue;
                    }

                    var file = STFileLoader.OpenFileFormat(new MemoryStream(anim.Data), Name, false, true);

                    //Only do once. There's usually one bntx embedded but incase there are multiple
                    if (file is BNTX && !IsTexturesReplaced)
                    {
                        ((TreeNode)file).Text = texturesFolder.Text;
                        ((TreeNode)file).ImageKey = texturesFolder.ImageKey;
                        ((TreeNode)file).SelectedImageKey = texturesFolder.SelectedImageKey;

                        //Remove temporary bntx file with file one
                        PluginRuntime.bntxContainers.Remove(texturesFolder);

                        ReplaceNode(texturesFolder.Parent, texturesFolder, (TreeNode)file);

                        IsTexturesReplaced = true;
                    }
                    else
                    {
                        externalFilesFolder.AddNode(new ExternalFileData(Name, anim.Data));
                    }
                }
            }

            if (PluginRuntime.UseSimpleBfresEditor)
                LoadSimpleMode();
        }

        public static void ReplaceNode(TreeNode node, TreeNode replaceNode, TreeNode NewNode)
        {
            if (NewNode == null)
                return;

            int index = node.Nodes.IndexOf(replaceNode);
            node.Nodes.RemoveAt(index);
            node.Nodes.Insert(index, NewNode);
        }

        public override void Export(string FileName)
        {
            bool IsTex1 = FileName.Contains("Tex1");
            bool HasTextures = false;

            foreach (TreeNode group in Nodes)
            {
                if (group is BFRESGroupNode)
                {
                    if (((BFRESGroupNode)group).Type == BRESGroupType.Textures && group.Nodes.Count > 0)
                        HasTextures = true;
                }
            }

            if (IsTex1 && HasTextures)
            {
                STFileSaver.SaveFileFormat(this, FileName);

                byte[] Tex2 = GenerateTex2();

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = Path.GetFileName(FileName.Replace("Tex1", "Tex2"));
                sfd.DefaultExt = ".sbfres";

                List<IFileFormat> formats = new List<IFileFormat>();
                formats.Add(this);
                sfd.Filter = Utils.GetAllFilters(formats);

                if (sfd.ShowDialog() == DialogResult.OK)
                    STFileSaver.SaveFileFormat(Tex2, true,new Yaz0(), 0, sfd.FileName);
            }
            else
                STFileSaver.SaveFileFormat(this, FileName);
        }

        private byte[] GenerateTex2()
        {
            STProgressBar progressBar = new STProgressBar();
            progressBar.Task = "Generating Tex2...";
            progressBar.Value = 0;
            progressBar.StartPosition = FormStartPosition.CenterScreen;
            progressBar.Show();
            progressBar.Refresh();

            var mem = new MemoryStream();

            var resFileU = BFRESRender.ResFileNode.resFileU;

            //Create a tex2 file
            ResU.ResFile resFileTex2 = new ResU.ResFile();
            resFileTex2.Alignment = resFileU.Alignment;
            resFileTex2.Name = resFileU.Name.Replace("Tex1", "Tex2");
            resFileTex2.VersionMajor = resFileU.VersionMajor;
            resFileTex2.VersionMajor2 = resFileU.VersionMajor2;
            resFileTex2.VersionMinor = resFileU.VersionMinor;
            resFileTex2.VersionMinor2 = resFileU.VersionMinor2;
            resFileTex2.Textures = resFileU.Textures;

            int curTex = 0;
            foreach (var group in Nodes)
            {
                if (group is BFRESGroupNode)
                {
                    if (((BFRESGroupNode)group).Type != BRESGroupType.Textures)
                        continue;

                    foreach (FTEX tex in ((BFRESGroupNode)group).Nodes)
                    {
                        Console.WriteLine("tex " + tex.Text + (resFileTex2.Textures.ContainsKey(tex.Text))) ;

                        if (resFileTex2.Textures.ContainsKey(tex.Text))
                        {
                            Console.WriteLine("NoMips " + tex.texture.MipData == null || tex.texture.MipData.Length <= 0);

                            if (tex.texture.MipData == null || tex.texture.MipData.Length <= 0)
                            {
                                progressBar.Task = $"Generating Mipmaps for {tex.Text}";
                                progressBar.Value = ((curTex * 100) / resFileTex2.Textures.Count);
                                progressBar.Refresh();

                                FTEX.GenerateMipmaps(tex.texture.MipCount, tex.Format, tex.GetBitmap(), resFileTex2.Textures[tex.Text]);
                            }
                            else
                            {
                                resFileTex2.Textures[tex.Text].MipData = tex.texture.MipData;
                                resFileTex2.Textures[tex.Text].MipOffsets = tex.texture.MipOffsets;
                                resFileTex2.Textures[tex.Text].MipCount = tex.texture.MipCount;
                                resFileTex2.Textures[tex.Text].Swizzle = tex.Tex2Swizzle;
                            }

                            curTex++;
                        }
                    }
                }
            }

            progressBar.Task = $"Saving File";
            progressBar.Value = 90;

            resFileTex2.Save(mem);

            progressBar.Value = 100;
            progressBar.Close();
            progressBar.Dispose();

            return mem.ToArray();
        }

        private void SaveTex2(string fileName)
        {
            bool Compressed = fileName.EndsWith("sbfres");

            byte[] data;
            if (Compressed)
                data = EveryFileExplorer.YAZ0.Decompress(fileName);
            else
                data = File.ReadAllBytes(fileName);

            ResU.ResFile resFileTex2 = new ResU.ResFile(new MemoryStream(data));

        
            foreach (BFRESGroupNode group in Nodes)
            {
                if (group.Type != BRESGroupType.Textures)
                    return;

                foreach (FTEX tex in group.Nodes)
                {
                    if (resFileTex2.Textures.ContainsKey(tex.Text))
                    {
                        resFileTex2.Textures[tex.Text].MipData = tex.texture.MipData;
                        resFileTex2.Textures[tex.Text].MipOffsets = tex.texture.MipOffsets;
                        resFileTex2.Textures[tex.Text].MipCount = tex.texture.MipCount;
                    }
                }
            }
            MemoryStream mem2 = new MemoryStream();
            resFileTex2.Save(mem2);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = FileName + "NewTex2.sbfres";

            List<IFileFormat> formats = new List<IFileFormat>();
            formats.Add(this);
            sfd.Filter = Utils.GetAllFilters(formats);

            if (sfd.ShowDialog() == DialogResult.OK)
                STFileSaver.SaveFileFormat(mem2.ToArray(), Compressed,new Yaz0(), 0, sfd.FileName);
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
        private void Remove(object sender, EventArgs args)
        {
            Unload();
        }
        private void SaveSwitch(Stream stream)
        {
            var resFile = BFRESRender.ResFileNode.resFile;

            resFile.Models.Clear();
            resFile.SkeletalAnims.Clear();
            resFile.MaterialAnims.Clear();
            resFile.SceneAnims.Clear();
            resFile.ShapeAnims.Clear();
            resFile.BoneVisibilityAnims.Clear();
            resFile.ModelDict.Clear();
            resFile.SkeletalAnimDict.Clear();
            resFile.MaterialAnimDict.Clear();
            resFile.SceneAnimDict.Clear();
            resFile.ShapeAnimDict.Clear();
            resFile.BoneVisibilityAnimDict.Clear();
            resFile.ExternalFiles.Clear();
            resFile.ExternalFileDict.Clear();

            foreach (TreeNode node in Nodes)
            {
                if (node is BFRESGroupNode)
                    SaveBfresSwitchGroup((BFRESGroupNode)node, resFile);
                
                if (node is BFRESAnimFolder)
                {
                    foreach (var animGroup in node.Nodes)
                        SaveBfresSwitchGroup((BFRESGroupNode)animGroup, resFile);
                }
                if (node is BNTX)
                {
                    if (SettingRemoveUnusedTextures)
                        RemoveUnusedTextures((BNTX)node);

                    if (((BNTX)node).Textures.Count > 0)
                    {
                        var mem = new System.IO.MemoryStream();
                        ((BNTX)node).Save(mem);

                        resFile.ExternalFiles.Add(new ExternalFile() { Data = mem.ToArray() });
                        resFile.ExternalFileDict.Add(((BNTX)node).FileName);
                    }
                }
            }

            ErrorCheck();

            resFile.Save(stream);
        }

        private void RemoveUnusedTextures(BFRESGroupNode ftexGroup)
        {
            var models = GetModels();
            if (models == null)
                return;

            List<string> Keys = ftexGroup.ResourceNodes.Select(x => x.Key).ToList();

            var AllTextures = GetAllTextures();
            for (int i = 0; i < Keys.Count; i++)
            {
                //If nowhere in the bfres contains the key, we can remove it
                if (!AllTextures.Contains(Keys[i]))
                {
                    ftexGroup.RemoveChild(ftexGroup.ResourceNodes[Keys[i]]);
                }
            }
        }

        private void RemoveUnusedTextures(BNTX bntx)
        {
            var models = GetModels();
            if (models == null)
                return;

            List<string> Keys = bntx.Textures.Select(x => x.Key).ToList();

            var AllTextures = GetAllTextures();
            for (int i = 0; i < Keys.Count; i++)
            {
                //If nowhere in the bfres contains the key, we can remove it
                if (!AllTextures.Contains(Keys[i]))
                {
                    bntx.RemoveTexture(bntx.Textures[Keys[i]]);
                }
            }
        }

        private List<string> GetAllTextures()
        {
            List<string> AllTextures = new List<string>();

            foreach (TreeNode group in Nodes)
            {
                if (group is BFRESGroupNode && ((BFRESGroupNode)group).Type == BRESGroupType.Models)
                {
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        foreach (var material in ((FMDL)group.Nodes[i]).materials.Values)
                        {
                            foreach (var tex in material.TextureMaps)
                            {
                                if (!AllTextures.Contains(tex.Name))
                                    AllTextures.Add(tex.Name);
                            }
                        }
                    }
                }
                if (group is BFRESAnimFolder)
                {
                    foreach (BFRESGroupNode animGroup in group.Nodes)
                    {
                        if (animGroup.Type == BRESGroupType.TexPatAnim)
                        {
                            for (int i = 0; i < group.Nodes.Count; i++)
                            {
                                foreach (var tex in ((MaterialAnimation)group.Nodes[i]).Textures)
                                {
                                    if (!AllTextures.Contains(tex))
                                        AllTextures.Add(tex);
                                }
                            }
                        }
                    }
                }
            }

            return AllTextures;
        }

        private static void SaveBfresSwitchGroup(BFRESGroupNode group, ResFile resFile)
        {
            switch (group.Type)
            {
                case BRESGroupType.Models:
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FMDL)group.Nodes[i]).Model.Name = ((FMDL)group.Nodes[i]).Text;
                        resFile.Models.Add(BfresSwitch.SetModel((FMDL)group.Nodes[i]));
                    }
                    break;
                case BRESGroupType.SkeletalAnim:
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FSKA)group.Nodes[i]).SkeletalAnim.BoneAnims.Clear();
                        ((FSKA)group.Nodes[i]).SkeletalAnim.Name = ((FSKA)group.Nodes[i]).Text;
                        for (int b = 0; b < ((FSKA)group.Nodes[i]).Bones.Count; b++)
                            ((FSKA)group.Nodes[i]).SkeletalAnim.BoneAnims.Add(
                                ((FSKA.BoneAnimNode)((FSKA)group.Nodes[i]).Bones[b]).SaveData(((FSKA)group.Nodes[i]).IsEdited));

                        resFile.SkeletalAnims.Add(((FSKA)group.Nodes[i]).SkeletalAnim);
                    }
                    break;
                case BRESGroupType.TexPatAnim:
                case BRESGroupType.ShaderParamAnim:
                case BRESGroupType.ColorAnim:
                case BRESGroupType.TexSrtAnim:
                case BRESGroupType.MatVisAnim:
                case BRESGroupType.MaterialAnim:
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FMAA)group.Nodes[i]).SaveAnimData();
                        resFile.MaterialAnims.Add(((FMAA)group.Nodes[i]).MaterialAnim);
                    }
                    break;
                case BRESGroupType.BoneVisAnim:
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FVIS)group.Nodes[i]).SaveAnimData();
                        resFile.BoneVisibilityAnims.Add(((FVIS)group.Nodes[i]).VisibilityAnim);
                    }
                    break;
                case BRESGroupType.ShapeAnim:
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FSHA)group.Nodes[i]).SaveAnimData();
                        resFile.ShapeAnims.Add(((FSHA)group.Nodes[i]).ShapeAnim);
                    }
                    break;
                case BRESGroupType.SceneAnim:
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FSCN)group.Nodes[i]).SaveAnimData();
                        resFile.SceneAnims.Add(((FSCN)group.Nodes[i]).SceneAnim);
                    }
                    break;
                case BRESGroupType.Embedded:
                    foreach (var ext in group.Nodes)
                    {
                        if (ext is ExternalFileData)
                        {
                            resFile.ExternalFiles.Add(new ExternalFile() { Data = ((ExternalFileData)ext).Data });
                            resFile.ExternalFileDict.Add(((ExternalFileData)ext).Text);
                        }
                        else if (ext is TreeNodeFile)
                        {
                            var mem = new System.IO.MemoryStream();
                            ((IFileFormat)ext).Save(mem);

                            resFile.ExternalFiles.Add(new ExternalFile()
                            {
                                Data = mem.ToArray(),
                            });
                            resFile.ExternalFileDict.Add(((TreeNodeFile)ext).Text);
                        }

                    }
                    break;
            }
        }

        private void SaveMaterialAnims(TreeNodeCollection nodes)
        {

        }
            

        private void SaveWiiU(Stream mem)
        {
            var resFileU = BFRESRender.ResFileNode.resFileU;

            resFileU.Models.Clear();
            resFileU.Textures.Clear();
            resFileU.SkeletalAnims.Clear();
            resFileU.ShaderParamAnims.Clear();
            resFileU.ColorAnims.Clear();
            resFileU.TexSrtAnims.Clear();
            resFileU.TexPatternAnims.Clear();
            resFileU.BoneVisibilityAnims.Clear();
            resFileU.MatVisibilityAnims.Clear();
            resFileU.ShapeAnims.Clear();
            resFileU.SceneAnims.Clear();
            resFileU.ExternalFiles.Clear();

            bool IsTex1 = FilePath.Contains("Tex1");

            foreach (TreeNode node in Nodes)
            {
                if (node is BFRESGroupNode)
                    SaveBfresWiiUGroup(this, (BFRESGroupNode)node, resFileU);

                if (node is BFRESAnimFolder)
                {
                    foreach (var animGroup in node.Nodes)
                        SaveBfresWiiUGroup(this, (BFRESGroupNode)animGroup, resFileU);
                }
            }

            //     ErrorCheck();
            resFileU.Save(mem);
        }

        private static void SaveBfresWiiUGroup(BFRES bfres, BFRESGroupNode group, ResU.ResFile resFileU)
        {
            switch (group.Type)
            {
                case BRESGroupType.Models:
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FMDL)group.Nodes[i]).ModelU.Name = group.Nodes[i].Text;
                        resFileU.Models.Add(group.Nodes[i].Text, BfresWiiU.SetModel((FMDL)group.Nodes[i]));
                    }
                    break;
                case BRESGroupType.Textures:
                    if (bfres.SettingRemoveUnusedTextures)
                        bfres.RemoveUnusedTextures(group);

                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FTEX)group.Nodes[i]).texture.Name = group.Nodes[i].Text;
                        resFileU.Textures.Add(group.Nodes[i].Text, ((FTEX)group.Nodes[i]).texture);
                    }
                    break;
                case BRESGroupType.SkeletalAnim:
                for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FSKA)group.Nodes[i]).SkeletalAnimU.BoneAnims.Clear();
                        ((FSKA)group.Nodes[i]).SkeletalAnimU.Name = ((FSKA)group.Nodes[i]).Text;
                        for (int b = 0; b < ((FSKA)group.Nodes[i]).Bones.Count; b++)
                            ((FSKA)group.Nodes[i]).SkeletalAnimU.BoneAnims.Add(
                                ((FSKA.BoneAnimNode)((FSKA)group.Nodes[i]).Bones[b]).SaveDataU(((FSKA)group.Nodes[i]).IsEdited));

                        resFileU.SkeletalAnims.Add(group.Nodes[i].Text, ((FSKA)group.Nodes[i]).SkeletalAnimU);
                    }
                    break;
                case BRESGroupType.ShaderParamAnim:
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FSHU)group.Nodes[i]).SaveAnimData();
                        resFileU.ShaderParamAnims.Add(group.Nodes[i].Text, ((FSHU)group.Nodes[i]).ShaderParamAnim);
                    }
                    break;
                case BRESGroupType.ColorAnim:
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FSHU)group.Nodes[i]).SaveAnimData();
                        resFileU.ColorAnims.Add(group.Nodes[i].Text, ((FSHU)group.Nodes[i]).ShaderParamAnim);
                    }
                    break;
                case BRESGroupType.TexSrtAnim:
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FSHU)group.Nodes[i]).SaveAnimData();
                        resFileU.TexSrtAnims.Add(group.Nodes[i].Text, ((FSHU)group.Nodes[i]).ShaderParamAnim);
                    }
                    break;
                case BRESGroupType.TexPatAnim:
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FTXP)group.Nodes[i]).SaveAnimData();
                        resFileU.TexPatternAnims.Add(group.Nodes[i].Text, ((FTXP)group.Nodes[i]).TexPatternAnim);
                    }
                    break;
                case BRESGroupType.MatVisAnim:
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FVIS)group.Nodes[i]).SaveAnimData();
                        resFileU.MatVisibilityAnims.Add(group.Nodes[i].Text, ((FVIS)group.Nodes[i]).VisibilityAnimU);
                    }
                    break;
                case BRESGroupType.BoneVisAnim:
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FVIS)group.Nodes[i]).SaveAnimData();
                        resFileU.BoneVisibilityAnims.Add(group.Nodes[i].Text, ((FVIS)group.Nodes[i]).VisibilityAnimU);
                    }
                    break;
                case BRESGroupType.ShapeAnim:
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FSHA)group.Nodes[i]).SaveAnimData();
                        resFileU.ShapeAnims.Add(group.Nodes[i].Text, ((FSHA)group.Nodes[i]).ShapeAnimU);
                    }
                    break;
                case BRESGroupType.SceneAnim:
                    for (int i = 0; i < group.Nodes.Count; i++)
                    {
                        ((FSCN)group.Nodes[i]).SaveAnimData();
                        resFileU.SceneAnims.Add(group.Nodes[i].Text, ((FSCN)group.Nodes[i]).SceneAnimU);
                    }
                    break;
                case BRESGroupType.Embedded:
                    foreach (TreeNode ext in group.Nodes)
                    {
                        if (ext is ExternalFileData)
                        {
                            resFileU.ExternalFiles.Add(ext.Text, new ResU.ExternalFile() { Data = ((ExternalFileData)ext).Data });
                        }
                    }
                    break;
            }
        }

        public static void SetShaderAssignAttributes(FSHP shape)
        {
            var shd = shape.GetFMAT().shaderassign;

            foreach (var att in shape.vertexAttributes)
            {
                if (!shd.attributes.ContainsValue(att.Name))
                {
                    try
                    {
                        shd.attributes.Add(att.Name, att.Name);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Attribute link failed! \n " + ex);
                    }
                }
            }
            foreach (var tex in shape.GetMaterial().TextureMaps)
            {
                if (!shd.samplers.ContainsValue(tex.SamplerName))
                {
                    try
                    {
                        shd.samplers.Add(tex.SamplerName, tex.SamplerName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Sampler link failed! \n " + ex);
                    }
                }
            }
        }


        private void SetDuplicateShapeName(FSHP shape)
        {
            DialogResult dialogResult = MessageBox.Show($"A shape {shape.Text} already exists with that name", "", MessageBoxButtons.OK);

            if (dialogResult == DialogResult.OK)
            {
                RenameDialog renameDialog = new RenameDialog();
                renameDialog.Text = "Rename Texture";
                if (renameDialog.ShowDialog() == DialogResult.OK)
                {
                    shape.Text = renameDialog.textBox1.Text;
                }
            }
        }

        static bool ImportMissingTextures = false;
        public static void CheckMissingTextures(FSHP shape)
        {
            // FSHP > Objects > FMDL > Models > BFRES
            BFRES root = (BFRES)shape.Parent.Parent.Parent.Parent;

            foreach (var node in root.Nodes)
            {
                if (node is BNTX)
                {
                    BNTX bntx = (BNTX)node;
                    if (bntx.Textures.Count == 0)
                        return;

                    List<string> textureList = new List<string>();
                    foreach (MatTexture tex in shape.GetMaterial().TextureMaps)
                    {
                        if (!bntx.Textures.ContainsKey(tex.Name) && !textureList.Contains(tex.Name))
                            textureList.Add(tex.Name);
                    }

                    foreach (var tex in textureList)
                    {
                        if (!bntx.Textures.ContainsKey(tex))
                        {
                            if (!ImportMissingTextures)
                            {
                                string textureDetails = string.Join("\n",textureList);
                                DialogResult result = MessageBox.Show($"Missing textures found! Would you like to use placeholders?\nTextures:\n{textureDetails}", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                if (result == DialogResult.Yes)
                                {
                                    ImportMissingTextures = true;
                                }
                                else
                                    return;
                            }

                            if (ImportMissingTextures)
                            {
                                foreach (var texture in textureList)
                                    bntx.ImportPlaceholderTexture(texture);
                            }
                        }
                    }
                }
            }

            foreach (var node in root.Nodes)
            {
                if (node is BFRESGroupNode && ((BFRESGroupNode)node).Type == BRESGroupType.Textures)
                {
                    if (((BFRESGroupNode)node).ResourceNodes.Count <= 0)
                        return;

                    foreach (MatTexture tex in shape.GetMaterial().TextureMaps)
                    {
                        if (!((BFRESGroupNode)node).ResourceNodes.ContainsKey(tex.Name))
                        {
                            if (!ImportMissingTextures)
                            {
                                DialogResult result = MessageBox.Show("Missing textures found! Would you like to use placeholders?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                                if (result == DialogResult.Yes)
                                {
                                    ImportMissingTextures = true;
                                }
                                else
                                {
                                    return;
                                }
                            }

                            if (ImportMissingTextures)
                                ((BFRESGroupNode)node).ImportPlaceholderTexture(tex.Name);
                        }
                    }
                }
            }
        }

        public void ErrorCheck()
        {
            if (BFRESRender != null)
            {
                List<Errors> Errors = new List<Errors>();
                foreach (FMDL model in BFRESRender.models)
                {
                    foreach (FSHP shp in model.shapes)
                    {
                        if (!IsWiiU)
                        {
                            Syroot.NintenTools.NSW.Bfres.VertexBuffer vtx = shp.VertexBuffer;
                            Syroot.NintenTools.NSW.Bfres.Material mat = shp.GetFMAT().Material;
                            Syroot.NintenTools.NSW.Bfres.ShaderAssign shdr = mat.ShaderAssign;

                            for (int att = 0; att < vtx.Attributes.Count; att++)
                            {
                                if (!shdr.AttribAssigns.Contains(vtx.Attributes[att].Name))
                                    STConsole.WriteLine($"Error! Attribute {vtx.Attributes[att].Name} is unlinked!");
                            }
                            for (int att = 0; att < mat.Samplers.Count; att++)
                            {
                                if (!shdr.SamplerAssigns.Contains(mat.SamplerDict.GetKey(att))) //mat.SamplerDict[att]
                                    STConsole.WriteLine($"Error! Sampler {mat.Samplers[att].Name} is unlinked!");
                            }
                        }
                        else
                        {
                            Syroot.NintenTools.Bfres.VertexBuffer vtx = shp.VertexBufferU;
                            Syroot.NintenTools.Bfres.Material mat = shp.GetFMAT().MaterialU;
                            Syroot.NintenTools.Bfres.ShaderAssign shdr = mat.ShaderAssign;

                            for (int att = 0; att < vtx.Attributes.Count; att++)
                            {
                                 ResU.ResString str = new ResU.ResString();
                                str.String = vtx.Attributes[att].Name;
                                if (!shdr.AttribAssigns.ContainsValue(str))
                                    STConsole.WriteLine($"Error! Attribute {vtx.Attributes[att].Name} is unlinked!");
                            }

                            for (int att = 0; att < mat.Samplers.Count; att++)
                            {
                                ResU.ResString str2 = new ResU.ResString();
                                str2.String = mat.Samplers[att].Name;
                                if (!shdr.SamplerAssigns.ContainsValue(str2)) //mat.SamplerDict[att]
                                    STConsole.WriteLine($"Error! Sampler {mat.Samplers[att].Name} is unlinked!");
                            }
                        }
                    }
                }
             //   ErrorList errorList = new ErrorList();
             //   errorList.LoadList(Errors);
            //    errorList.Show();
            }
        }
        public class Errors
        {
            public string Section = "None";
            public string Section2 = "None";
            public string Message = "";
            public string Type = "Unkown";
        }
    }
}
