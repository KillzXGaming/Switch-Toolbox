using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using BcresLibrary;

namespace FirstPlugin
{
    public class CMDLWrapper : STGenericModel
    {
        internal BCRES BcresParent;
        internal Model Model;

        public CRESSkeletonWrapper Skeleton;

        public List<SOBJWrapper> Shapes = new List<SOBJWrapper>();
        public List<MTOBWrapper> Materials = new List<MTOBWrapper>();

        public CMDLWrapper()
        {
            ImageKey = "Model";
            SelectedImageKey = "Model";
        }

        public CMDLWrapper(Model model, BCRES bcres) : base()
        {
            BcresParent = bcres;
            LoadModel(model, bcres);
        }

        public override void OnClick(TreeView treeview) {
            BcresParent.LoadEditors(this, OnPropertyChanged);
        }

        private void OnPropertyChanged()
        {

        }

        public void LoadModel(Model model, BCRES bcres)
        {
            BcresParent = bcres;

            Model = model;
            Text = model.Name;

            var MaterialFolder = new TreeNode("Materials");
            var MeshFolder = new TreeNode("Meshes");
            Skeleton = new CRESSkeletonWrapper();
            Skeleton.Text = "Skeleton";
            Checked = true;

            Nodes.Add(MeshFolder);
            Nodes.Add(MaterialFolder);
            Nodes.Add(Skeleton);

            if (model.HasSkeleton)
            {
                Skeleton.Load(model.Skeleton, bcres);
            }

            int Index = 0;
            foreach (var material in model.Materials.Values)
            {
                var matWrapper = new MTOBWrapper();
                matWrapper.Load(bcres, material);
                if (matWrapper.Text == string.Empty)
                    matWrapper.Text = $"Material {Index++}";
                MaterialFolder.Nodes.Add(matWrapper);
                Materials.Add(matWrapper);
            }

            Index = 0;
            foreach (var mesh in model.Meshes)
            {
                var meshWrapper = new SOBJWrapper(this, mesh) { BcresParent = bcres };
                MeshFolder.Nodes.Add(meshWrapper);
                if (meshWrapper.Text == string.Empty)
                    meshWrapper.Text = $"Mesh {Index++}";
                Shapes.Add(meshWrapper);
            }
        }
    }
}
