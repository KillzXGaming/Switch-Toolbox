using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using SPICA.Formats.CtrH3D.Model;
using SPICA.Formats.CtrH3D.Model.Material;
using SPICA.Formats.CtrH3D.Model.Mesh;
using SPICA.Formats.CtrH3D.Texture;
using FirstPlugin.CtrLibrary.Forms;

namespace FirstPlugin.CtrLibrary
{
    public class H3DModelWrapper : STGenericModel
    {
        internal BCH BchParent;
        internal H3DModel Model;

        public H3DSkeletonWrapper Skeleton;

        public List<H3DMeshWrapper> Meshes = new List<H3DMeshWrapper>();
        public List<H3DMaterialWrapper> Materials = new List<H3DMaterialWrapper>();

        public override void Export(string FileName)
        {
            string ext = Utils.GetExtension(FileName);
            switch (ext)
            {
                case ".dae":
                    ExportModelSettings exportDlg = new ExportModelSettings();
                    if (exportDlg.ShowDialog() == DialogResult.OK)
                        ExportModel(FileName, exportDlg.Settings);
                    break;
            }
        }

        public void ExportModel(string fileName, DAE.ExportSettings settings)
        {
            var model = new STGenericModel();
            model.Materials = Materials;
            model.Objects = Meshes;

            DAE.Export(fileName, settings, model, BchParent.Textures, Skeleton.Renderable);
        }


        public override string ExportFilter => FileFilters.CMDL;

        public H3DModelWrapper(H3DModel model, BCH bch) : base()
        {
            BchParent = bch;
            LoadModel(model, bch);
        }

        public override void OnClick(TreeView treeview)
        {
            var editor = BchParent.LoadEditor<BCHModelEditor>();
            editor.LoadModel(this);
        }

        private void OnPropertyChanged()
        {

        }

        public void LoadModel(H3DModel model, BCH bch)
        {
            BchParent = bch;

            ImageKey = "model";
            SelectedImageKey = "model";

            Model = model;
            Text = model.Name;

            var MaterialFolder = new TreeNode("Materials");
            var MeshFolder = new TreeNode("Meshes");
            Skeleton = new H3DSkeletonWrapper();
            Skeleton.Text = "Skeleton";
            Checked = true;

            Nodes.Add(MeshFolder);
            Nodes.Add(MaterialFolder);
            Nodes.Add(Skeleton);

            if (model.Skeleton != null) {
                Skeleton.Load(model, bch);
            }

            int Index = 0;
            foreach (var material in model.Materials)
            {
                var matWrapper = new H3DMaterialWrapper(bch, this, material);
                matWrapper.Text = material.Name;
                MaterialFolder.Nodes.Add(matWrapper);
                Materials.Add(matWrapper);
            }

            Index = 0;
            foreach (var mesh in model.Meshes)
            {
                var meshWrapper = new H3DMeshWrapper(bch, this, mesh);

                MeshFolder.Nodes.Add(meshWrapper);
                bch.Renderer.Meshes.Add(meshWrapper);

                if (meshWrapper.Text == string.Empty)
                    meshWrapper.Text = $"mesh_{model.Materials[mesh.MaterialIndex].Name}";
                Meshes.Add(meshWrapper);
            }
        }
    }
}
