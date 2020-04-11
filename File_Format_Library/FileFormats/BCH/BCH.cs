using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Windows.Forms;
using FirstPlugin.Forms;
using SPICA.Formats.CtrH3D;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Texture;
using SPICA.Formats.CtrH3D.Shader;
using SPICA.Formats.CtrH3D.Animation;
using SPICA.Formats.CtrH3D.Camera;
using SPICA.Formats.CtrH3D.Fog;
using SPICA.Formats.CtrH3D.Light;
using SPICA.Formats.CtrH3D.LUT;
using SPICA.WinForms.Formats;

namespace FirstPlugin.CtrLibrary
{
    public class BCH : TreeNodeFile, IContextMenuNode, IFileFormat, ITextureContainer
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "BCH", "BCRES" };
        public string[] Extension { get; set; } = new string[] { "*.bch", "*.bcres" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true)) {
                return (reader.CheckSignature(3, "BCH") ||
                        reader.CheckSignature(4, "CGFX"));
                     // (GFPackage.IsValidPackage(stream));
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }


        bool DrawablesLoaded = false;
        public override void OnClick(TreeView treeView)
        {
            var propertyGrid = LoadEditor<STPropertyGrid>();
            propertyGrid.LoadProperty(new FileSettings(H3DFile));
        }

        public bool DisplayIcons => true;

        public List<STGenericTexture> TextureList
        {
            get { return Textures; }
            set { }
        }

        public class FileSettings
        {
            private H3D H3DFile;

            public enum VersionList : ushort
            {
                LatestVersion = 44943,
            }

            public VersionList VersionPreset
            {
                get { return (VersionList)H3DFile.ConverterVersion; }
                set { H3DFile.ConverterVersion = (ushort)value; }
            }

            public ushort Version
            {
                get { return H3DFile.ConverterVersion; }
                set { H3DFile.ConverterVersion = value; }
            }

            public byte BackwardCompatibility
            {
                get { return H3DFile.BackwardCompatibility; }
                set { H3DFile.BackwardCompatibility = value; }
            }

            public byte ForwardCompatibility
            {
                get { return H3DFile.ForwardCompatibility; }
                set { H3DFile.ForwardCompatibility = value; }
            }

            public FileSettings(H3D h3d) {
                H3DFile = h3d;
            }
        }

        public T LoadEditor<T>() where T : UserControl, new()
        {
            ViewportEditor editor = (ViewportEditor)LibraryGUI.GetActiveContent(typeof(ViewportEditor));
            if (editor == null)
            {
                editor = new ViewportEditor(true);
                editor.Dock = DockStyle.Fill;
                LibraryGUI.LoadEditor(editor);
            }
            if (!DrawablesLoaded)
            {
                ObjectEditor.AddContainer(DrawableContainer);
                DrawablesLoaded = true;
            }
            if (Runtime.UseOpenGL)
                editor.LoadViewport(DrawableContainer);

            foreach (var subControl in editor.GetEditorPanel().Controls)
                if (subControl.GetType() == typeof(T))
                    return subControl as T;

            T control = new T();
            control.Dock = DockStyle.Fill;
            editor.LoadEditor(control);
            return control;
        }

        public BCH_Renderer Renderer;
        public DrawableContainer DrawableContainer = new DrawableContainer();

        public List<STGenericTexture> Textures = new List<STGenericTexture>();
        public H3D H3DFile;

        public FileFormatType FormatType = FileFormatType.BCH;

        public enum FileFormatType
        {
            BCH,
            BCRES,
            MBN,
            GFModel,
            GFTexture,
            GFModelPack,
            GFMotion,
            GFPackedTexture,
            GFL2OverWorld,
            GFBtlSklAnim,
            GFCharaModel,
            GFOWMapModel,
            GFOWCharaModel,
            GFPkmnModel,
            GFPkmnSklAnim,
            MTModel,
        }

        private void ConvertH3D(System.IO.Stream stream)
        {
            CanSave = true;

            System.IO.BinaryReader Reader = new System.IO.BinaryReader(stream);
            using (FileReader reader = new FileReader(stream, true))
            {
                uint magicNumber = reader.ReadUInt32();

                switch (magicNumber)
                {
                    case 0x15122117:
                        FormatType = FileFormatType.GFModel;
                        H3DFile = new H3D();
                        H3DFile.Models.Add(new SPICA.Formats.GFL2.Model.GFModel(Reader, "Model").ToH3DModel());
                        break;
                }

                string mbnPath = FilePath.Replace("bch", "mbn");

                if (reader.CheckSignature(3, "BCH"))
                {
                    H3DFile = H3D.Open(stream.ToBytes());
                    FormatType = FileFormatType.BCH;
                    return;
                }
                else if (reader.CheckSignature(4, "CGFX"))
                {
                    H3DFile = SPICA.Formats.CtrGfx.Gfx.Open(stream);
                    FormatType = FileFormatType.BCRES;
                }
                else if (GFPackage.IsValidPackage(stream))
                {
                    GFPackage.Header PackHeader = GFPackage.GetPackageHeader(stream);
                    switch (PackHeader.Magic)
                    {
                        case "PC": H3DFile = GFPkmnModel.OpenAsH3D(stream, PackHeader, null); break;
                    }
                }
                else  if (System.IO.File.Exists(mbnPath))
                {
                    var ModelBinary = new SPICA.Formats.ModelBinary.MBn(new System.IO.BinaryReader(
                        System.IO.File.OpenRead(mbnPath)), H3DFile);

                    H3DFile = ModelBinary.ToH3D();
                    FormatType = FileFormatType.MBN;
                }
                else
                    H3DFile = H3D.Open(stream.ToBytes());
            }
        }

        public void UpdateViewport() {
            LibraryGUI.UpdateViewport();
        }

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            ConvertH3D(stream);

            Renderer = new BCH_Renderer();
            DrawableContainer.Name = FileName;
            DrawableContainer.Drawables.Add(Renderer);

            AddNodeGroup(H3DFile.Models, new BCHGroupNode(BCHGroupType.Models));
            AddNodeGroup(H3DFile.Textures, new TextureFolder(BCHGroupType.Textures));
            AddNodeGroup(H3DFile.LUTs, new BCHGroupNode(BCHGroupType.Lookups));
            AddNodeGroup(H3DFile.Shaders, new BCHGroupNode(BCHGroupType.Shaders));
            AddNodeGroup(H3DFile.Cameras, new BCHGroupNode(BCHGroupType.Cameras));
            AddNodeGroup(H3DFile.Lights, new BCHGroupNode(BCHGroupType.Lights));
            AddNodeGroup(H3DFile.Fogs, new BCHGroupNode(BCHGroupType.Fogs));
            AddNodeGroup(H3DFile.Scenes, new BCHGroupNode(BCHGroupType.Scenes));
            AddNodeGroup(H3DFile.SkeletalAnimations, new BCHGroupNode(BCHGroupType.SkeletalAnim));
            AddNodeGroup(H3DFile.MaterialAnimations, new BCHGroupNode(BCHGroupType.MaterialAnim));
            AddNodeGroup(H3DFile.VisibilityAnimations, new BCHGroupNode(BCHGroupType.VisibiltyAnim));
            AddNodeGroup(H3DFile.CameraAnimations, new BCHGroupNode(BCHGroupType.CameraAnim));
            AddNodeGroup(H3DFile.LightAnimations, new BCHGroupNode(BCHGroupType.LightAnim));
            AddNodeGroup(H3DFile.FogAnimations, new BCHGroupNode(BCHGroupType.EmitterAnim));
        }

        private void AddNodeGroup<T>(H3DDict<T> SubSections, BCHGroupNode Folder)
       where T : SPICA.Formats.Common.INamed
        {
            if (SubSections == null || SubSections.Count == 0)
                return;

            Nodes.Add(Folder);
            if (Folder.Type == BCHGroupType.Textures)
                PluginRuntime.bchTexContainers.Add(Folder);

            for (int i = 0; i < SubSections?.Count; i++)
            {
                var section = SubSections[i] as SPICA.Formats.Common.INamed;
                switch (Folder.Type)
                {
                    case BCHGroupType.Models:
                        var CMDLWrapper = new H3DModelWrapper((H3DModel)section, this);
                        Folder.AddNode(CMDLWrapper);
                         break;
                    case BCHGroupType.SkeletalAnim:
                        var CSKAWrapper = new H3DSkeletalAnimWrapper((H3DAnimation)section, this);
                        Folder.AddNode(CSKAWrapper);
                        break;
                    case BCHGroupType.Textures:
                        {
                            var wrapper = new H3DTextureWrapper((H3DTexture)section, this);
                            wrapper.ImageKey = "texture";
                            wrapper.SelectedImageKey = "texture";
                            Folder.AddNode(wrapper);
                            Textures.Add(wrapper);
                        }
                        break;
                    default:
                        Folder.AddNode(new Toolbox.Library.NodeWrappers.STGenericWrapper(SubSections[i].Name));
                        break;
                }
            }
        }

        public void Save(System.IO.Stream stream) {
            switch (FormatType)
            {
                case FileFormatType.BCH:
                    H3D.Save(stream, H3DFile);
                    break;
                default:
                    throw new Exception($"File format cannot be saved yet! {FormatType}");
            }
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
          //  Items.Add(new ToolStripMenuItem("Export Model", null, ExportAction, Keys.Control | Keys.E));
            return Items.ToArray();
        }

        private void ExportAction(object  sender, EventArgs e)
        {
            
        }

        public void Unload()
        {
            ObjectEditor.RemoveContainer(DrawableContainer);

            foreach (var node in Nodes)
            {
                if (PluginRuntime.bchTexContainers.Contains(node))
                    PluginRuntime.bchTexContainers.Remove((BCHGroupNode)node);
            }

            foreach (var tex in Textures)
                tex?.DisposeRenderable();
            Textures.Clear();
        }


        public class TextureFolder : BCHGroupNode, ITextureContainer
        {
            public bool DisplayIcons => true;

            public TextureFolder(BCHGroupType type) : base(type) { }

            public List<STGenericTexture> TextureList
            {
                get
                {
                    List<STGenericTexture> textures = new List<STGenericTexture>();
                    foreach (STGenericTexture node in Nodes)
                        textures.Add(node);

                    return textures;
                }
                set { }
            }
        }
    }
}
