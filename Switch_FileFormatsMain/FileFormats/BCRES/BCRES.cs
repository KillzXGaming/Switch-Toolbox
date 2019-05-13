using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Forms;
using BcresLibrary;
using FirstPlugin.Forms;
using GL_EditorFramework.Interfaces;

namespace FirstPlugin
{
    public class BCRES : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "BCRES" };
        public string[] Extension { get; set; } = new string[] { "*.bcres" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "CGFX");
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

        public BcresFile BcresFile;
        public BCRES_Render RenderedBcres;

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;
            BcresFile = new BcresFile(stream);
            RenderedBcres = new BCRES_Render();

            AddNodeGroup(BcresFile.Data.Models, new BCRESGroupNode(BCRESGroupType.Models));
            AddNodeGroup(BcresFile.Data.Textures, new BCRESGroupNode(BCRESGroupType.Textures));
            AddNodeGroup(BcresFile.Data.Lookups, new BCRESGroupNode(BCRESGroupType.Lookups));
            AddNodeGroup(BcresFile.Data.Shaders, new BCRESGroupNode(BCRESGroupType.Shaders));
            AddNodeGroup(BcresFile.Data.Cameras, new BCRESGroupNode(BCRESGroupType.Cameras));
            AddNodeGroup(BcresFile.Data.Lights, new BCRESGroupNode(BCRESGroupType.Lights));
            AddNodeGroup(BcresFile.Data.Fogs, new BCRESGroupNode(BCRESGroupType.Fogs));
            AddNodeGroup(BcresFile.Data.Scenes, new BCRESGroupNode(BCRESGroupType.Scenes));
            AddNodeGroup(BcresFile.Data.SkeletalAnims, new BCRESGroupNode(BCRESGroupType.SkeletalAnim));
            AddNodeGroup(BcresFile.Data.MaterialAnims, new BCRESGroupNode(BCRESGroupType.MaterialAnim));
            AddNodeGroup(BcresFile.Data.VisibiltyAnims, new BCRESGroupNode(BCRESGroupType.VisibiltyAnim));
            AddNodeGroup(BcresFile.Data.CameraAnims, new BCRESGroupNode(BCRESGroupType.CameraAnim));
            AddNodeGroup(BcresFile.Data.LightAnims, new BCRESGroupNode(BCRESGroupType.LightAnim));
            AddNodeGroup(BcresFile.Data.EmitterAnims, new BCRESGroupNode(BCRESGroupType.EmitterAnim));
            AddNodeGroup(BcresFile.Data.Particles, new BCRESGroupNode(BCRESGroupType.Particles));
        }

        public override void OnClick(TreeView treeview) {
            LoadEditors(this, null);
        }

        private void AddNodeGroup<T>(ResDict<T> SubSections, BCRESGroupNode Folder)
            where T : CtrObject, new()
        {
            if (SubSections == null || SubSections.Count == 0)
                return;

            Nodes.Add(Folder);

            foreach (CtrObject section in SubSections.Values)
            {
                switch (Folder.Type)
                {
                    case BCRESGroupType.Models:
                        var CMDLWrapper = new CMDLWrapper((Model)section, this);
                        Folder.AddNode(CMDLWrapper);
                        RenderedBcres.Models.Add(CMDLWrapper);
                        break;
                    case BCRESGroupType.Textures:
                        Folder.AddNode(new TXOBWrapper((Texture)section, this));
                        PluginRuntime.bcresTexContainers.Add(Folder);
                        break;
                }
            }
        }

        List<AbstractGlDrawable> drawables = new List<AbstractGlDrawable>();

        public void LoadEditors(TreeNode Wrapper, Action OnPropertyChanged)
        {
            BcresEditor bcresEditor = (BcresEditor)LibraryGUI.Instance.GetActiveContent(typeof(BcresEditor));
            bool HasModels = RenderedBcres.Models.Count > 0;
            if (bcresEditor == null)
            {
                bcresEditor = new BcresEditor(HasModels);
                bcresEditor.Dock = DockStyle.Fill;
                LibraryGUI.Instance.LoadEditor(bcresEditor);
            }

            if (drawables.Count <= 0)
            {
                //Add drawables
                drawables.Add(RenderedBcres);

                for (int m = 0; m < RenderedBcres.Models.Count; m++)
                {
                    if (RenderedBcres.Models[m].Skeleton.Renderable != null)
                        drawables.Add(RenderedBcres.Models[m].Skeleton.Renderable);
                }
            }

            if (Runtime.UseOpenGL)
                bcresEditor.LoadViewport(drawables);

            if (Wrapper is BcresTextureMapWrapper)
            {
                BcresSamplerEditorSimple editor = (BcresSamplerEditorSimple)bcresEditor.GetActiveEditor(typeof(BcresSamplerEditorSimple));
                if (editor == null)
                {
                    editor = new BcresSamplerEditorSimple();
                    editor.Dock = DockStyle.Fill;
                    bcresEditor.LoadEditor(editor);
                }
                editor.LoadTexture(((BcresTextureMapWrapper)Wrapper));
            }
            else if (Wrapper is BCRES) {
                LoadPropertyGrid(((BCRES)Wrapper).BcresFile, bcresEditor, OnPropertyChanged);
            }
            else if (Wrapper is MTOBWrapper) {
                LoadPropertyGrid(((MTOBWrapper)Wrapper).Material, bcresEditor, OnPropertyChanged);
            }
            else if (Wrapper is CMDLWrapper) {
                LoadPropertyGrid(((CMDLWrapper)Wrapper).Model, bcresEditor, OnPropertyChanged);
            }
            else if (Wrapper is CRESBoneWrapper) {
                LoadPropertyGrid(((CRESBoneWrapper)Wrapper).Bone, bcresEditor, OnPropertyChanged);
            }
            else if (Wrapper is CRESSkeletonWrapper) {
                LoadPropertyGrid(((CRESSkeletonWrapper)Wrapper).Skeleton, bcresEditor, OnPropertyChanged);
            }
        }

        private void LoadPropertyGrid(object property, BcresEditor bcresEditor, Action OnPropertyChanged)
        {
            STPropertyGrid editor = (STPropertyGrid)bcresEditor.GetActiveEditor(typeof(STPropertyGrid));
            if (editor == null)
            {
                editor = new STPropertyGrid();
                editor.Dock = DockStyle.Fill;
                bcresEditor.LoadEditor(editor);
            }
            editor.LoadProperty(property, OnPropertyChanged);
        }

        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }
    }
}
