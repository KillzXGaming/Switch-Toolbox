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
    public class CRESSkeletonWrapper : STBone
    {
        internal BCRES BcresParent;
        internal Skeleton Skeleton;
        internal STSkeleton Renderable;

        public CRESSkeletonWrapper()
        {
            ImageKey = "Skeleton";
            SelectedImageKey = "Skeleton";
        }

        public CRESSkeletonWrapper(Skeleton skeleton, BCRES bcres) : base()
        {
            BcresParent = bcres;
            Load(skeleton, bcres);
        }

        public override void OnClick(TreeView treeview) {
            BcresParent.LoadEditors(this, OnPropertyChanged);
        }

        private void OnPropertyChanged()
        {

        }

        public void Load(Skeleton skeleton, BCRES bcres)
        {
            Renderable = new STSkeleton();

            BcresParent = bcres;

            Skeleton = skeleton;
            Text = "Skeleton";

            foreach (var bone in skeleton.Bones.Values)
            {
                var boneWrapper = new CRESBoneWrapper();
                boneWrapper.skeletonParent = Renderable;
                boneWrapper.Load(bone, bcres);
                Nodes.Add(boneWrapper);
                Renderable.bones.Add(boneWrapper);
            }
        }
    }
}
