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
            if (BcresParent != null)
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
            Checked = true;

            foreach (var bone in skeleton.Bones.Values)
            {
                var boneWrapper = new CRESBoneWrapper();
                boneWrapper.skeletonParent = Renderable;
                boneWrapper.Load(bone, bcres);
                Renderable.bones.Add(boneWrapper);
            }

            Nodes.Clear();
            foreach (var bone in Renderable.bones)
            {
                if (bone.Parent == null)
                {
                    Nodes.Add(bone);
                }
            }

            Renderable.update();
            Renderable.reset();
        }
    }
}
