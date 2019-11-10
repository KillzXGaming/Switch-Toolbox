using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.Rendering;

namespace FirstPlugin.PunchOutWii
{
    public class RenderableMeshWrapper : GenericRenderedObject
    {
        public STGenericMaterial Material = new STGenericMaterial();

        public override STGenericMaterial GetMaterial()
        {
            return Material;
        }
    }

    public class PO_Model : TreeNodeCustom, IContextMenuNode
    {
        public PO_DICT ParentDictionary;

        public uint NumMeshes;
        public uint HashID { get; set; }

        public List<PO_Mesh> Meshes = new List<PO_Mesh>();

        public List<RenderableMeshWrapper> RenderedMeshes = new List<RenderableMeshWrapper>();

        Viewport viewport
        {
            get
            {
                var editor = LibraryGUI.GetObjectEditor();
                return editor.GetViewport();
            }
            set
            {
                var editor = LibraryGUI.GetObjectEditor();
                editor.LoadViewport(value);
            }
        }

        private bool loaded = false;
        public override void OnClick(TreeView treeView)
        {
            if (!loaded)
                UpdateVertexData();

            if (Runtime.UseOpenGL)
            {
                if (viewport == null)
                {
                    viewport = new Viewport(ObjectEditor.GetDrawableContainers());
                    viewport.Dock = DockStyle.Fill;
                }

                Viewport editor = (Viewport)LibraryGUI.GetActiveContent(typeof(Viewport));
                if (editor == null)
                {
                    editor = viewport;
                    LibraryGUI.LoadEditor(viewport);
                }

                viewport.ReloadDrawables(ParentDictionary.DrawableContainer);
                viewport.Text = Text;
            }
        }

        private void UpdateVertexData()
        {
            ParentDictionary.Renderer.UpdateVertexData();
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Export", null, ExportModelAction, Keys.Control | Keys.E));
            return Items.ToArray();
        }

        private void ExportModelAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.dae;";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ExportModel(sfd.FileName);
            }
        }

        private void ExportModel(string FileName)
        {
            AssimpSaver assimp = new AssimpSaver();
            ExportModelSettings settings = new ExportModelSettings();

            List<STGenericMaterial> Materials = new List<STGenericMaterial>();
            //  foreach (var msh in DataDictionary.Renderer.Meshes)
            //    Materials.Add(msh.GetMaterial());

            var model = new STGenericModel();
            model.Materials = Materials;
            model.Objects = RenderedMeshes;

            assimp.SaveFromModel(model, FileName, new List<STGenericTexture>(), new STSkeleton());
        }
    }
}
