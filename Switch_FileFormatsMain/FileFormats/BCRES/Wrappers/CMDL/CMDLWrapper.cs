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

        public List<SOBJWrapper> Shapes = new List<SOBJWrapper>();

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
            var SkeletonWrapper = new CRESSkeletonWrapper();

            Nodes.Add(MeshFolder);
            Nodes.Add(MaterialFolder);
            Nodes.Add(SkeletonWrapper);

            foreach (var material in model.Materials.Values)
            {
                var matWrapper = new MTOBWrapper() { BcresParent = bcres };
                matWrapper.Load(material);
                MaterialFolder.Nodes.Add(matWrapper);
            }
            foreach (var mesh in model.Meshes)
            {
                var meshWrapper = new SOBJWrapper() { BcresParent = bcres };
                meshWrapper.Load(mesh);
                MeshFolder.Nodes.Add(meshWrapper);
                Shapes.Add(meshWrapper);
            }
            if (model.HasSkeleton)
            {
                SkeletonWrapper.Load(model.Skeleton, bcres);
            }
        }
    }
}
