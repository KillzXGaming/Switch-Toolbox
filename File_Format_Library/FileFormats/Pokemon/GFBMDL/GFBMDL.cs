using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using Toolbox.Library.Rendering;
using OpenTK;
using FirstPlugin.Forms;

namespace FirstPlugin
{
    public class GFBMDL : TreeNodeFile, IContextMenuNode, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Graphic Model" };
        public string[] Extension { get; set; } = new string[] { "*.gfbmdl" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                bool IsMatch = reader.ReadUInt32() == 0x20000000;
                reader.Position = 0;

                return IsMatch;
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
        public override void OnClick(TreeView treeView) {
            LoadEditor<STPropertyGrid>();
        }

        public T LoadEditor<T>() where T : UserControl, new()
        {
            T control = new T();
            control.Dock = DockStyle.Fill;

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

            editor.LoadEditor(control);

            return control;
        }

        public GFBMDL_Render Renderer;

        public DrawableContainer DrawableContainer = new DrawableContainer();

        public GFLXModel Model;

        public void Load(System.IO.Stream stream)
        {
            CanSave = false;

            Text = FileName;
            DrawableContainer.Name = FileName;
            Renderer = new GFBMDL_Render();
            DrawableContainer.Drawables.Add(Renderer);

            Model = new GFLXModel();
            Model.LoadFile(FlatBuffers.Gfbmdl.Model.GetRootAsModel(
                new FlatBuffers.ByteBuffer(stream.ToBytes())),this, Renderer);

            TreeNode SkeletonWrapper = new TreeNode("Skeleton");
            TreeNode MaterialFolderWrapper = new TreeNode("Materials");
            TreeNode VisualGroupWrapper = new TreeNode("Visual Groups");
            TreeNode Textures = new TreeNode("Textures");

            if (Model.Skeleton.bones.Count > 0)
            {
                Nodes.Add(SkeletonWrapper);
                DrawableContainer.Drawables.Add(Model.Skeleton);

                foreach (var bone in Model.Skeleton.bones) {
                    if (bone.Parent == null)
                        SkeletonWrapper.Nodes.Add(bone);
                }
            }


            List<string> loadedTextures = new List<string>();
            for (int i = 0; i < Model.Textures.Count; i++)
            {
                foreach (var bntx in PluginRuntime.bntxContainers)
                {
                    if (bntx.Textures.ContainsKey(Model.Textures[i]) &&
                        !loadedTextures.Contains(Model.Textures[i]))
                    {
                        TreeNode tex = new TreeNode(Model.Textures[i]);
                        tex.ImageKey = "texture";
                        tex.SelectedImageKey = "texture";

                        tex.Tag = bntx.Textures[Model.Textures[i]];
                        Textures.Nodes.Add(tex);
                        loadedTextures.Add(Model.Textures[i]);
                    }
                }
            }

            loadedTextures.Clear();

            Nodes.Add(MaterialFolderWrapper);
            Nodes.Add(VisualGroupWrapper);
            if (Textures.Nodes.Count > 0)
                Nodes.Add(Textures);

            for (int i = 0; i < Model.GenericMaterials.Count; i++)
                MaterialFolderWrapper.Nodes.Add(Model.GenericMaterials[i]);

            for (int i = 0; i < Model.GenericMeshes.Count; i++)
                VisualGroupWrapper.Nodes.Add(Model.GenericMeshes[i]);
        }

        public void Save(System.IO.Stream stream)
        {
            Model.SaveFile(stream);
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Export Model", null, ExportAction, Keys.Control | Keys.E));
            return Items.ToArray();
        }

        private void ExportAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.dae;";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ExportModelSettings exportDlg = new ExportModelSettings();
                if (exportDlg.ShowDialog() == DialogResult.OK)
                    ExportModel(sfd.FileName, exportDlg.Settings);
            }
        }

        public void ExportModel(string fileName, DAE.ExportSettings settings)
        {
            var model = new STGenericModel();
            model.Materials = Model.GenericMaterials;
            model.Objects = Model.GenericMeshes;
            var textures = new List<STGenericTexture>();
            foreach (var bntx in PluginRuntime.bntxContainers)
                foreach (var tex in bntx.Textures.Values)
                    textures.Add(tex);

            DAE.Export(fileName, settings, model, textures, Model.Skeleton);
        }

        public void Unload()
        {

        }
    }
}
