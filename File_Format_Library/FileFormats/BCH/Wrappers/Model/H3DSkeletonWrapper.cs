using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using SPICA.Formats.CtrH3D.Model;

namespace FirstPlugin.CtrLibrary
{
    public class H3DSkeletonWrapper : TreeNodeCustom
    {
        internal BCH BcresParent;
        internal H3DModel ModelParent;
        internal STSkeleton Renderable;

        public H3DSkeletonWrapper()
        {
            ImageKey = "Skeleton";
            SelectedImageKey = "Skeleton";
        }

        public H3DSkeletonWrapper(H3DModel model, BCH bch) : base()
        {
            BcresParent = bch;
            Load(model, bch);
        }

        public override void OnClick(TreeView treeview)
        {

        }

        private void OnPropertyChanged()
        {

        }

        public void Load(H3DModel model, BCH bch)
        {
            Renderable = new STSkeleton();
            bch.DrawableContainer.Drawables.Add(Renderable);

            BcresParent = bch;
            ModelParent = model;

            Text = "Skeleton";
            Checked = true;

            foreach (var bone in model.Skeleton)
                Renderable.bones.Add(new H3DBoneWrapper(Renderable, bone, bch));

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
