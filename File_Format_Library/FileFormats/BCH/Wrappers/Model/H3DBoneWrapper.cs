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
    public class H3DBoneWrapper : STBone
    {
        internal BCH BchParent;
        public H3DBone Bone;

        public H3DBoneWrapper(STSkeleton skeleton, H3DBone bone, BCH bch) : base(skeleton)
        {
            BchParent = bch;
            Load(bone, bch);
        }

        public override void OnClick(TreeView treeview) {
            var editor = BchParent.LoadEditor<CtrLibrary.Forms.BCHBoneEditor>();
            editor.LoadBone(this);
        }

        private void OnPropertyChanged()
        {

        }

        public void Load(H3DBone bone, BCH bch)
        {
            BchParent = bch;
            Checked = true;

            Bone = bone;
            Text = bone.Name;

            parentIndex = bone.ParentIndex;
            RotationType = BoneRotationType.Euler;
            position = new float[3];
            scale = new float[3];
            rotation = new float[4];
            scale[0] = bone.Scale.X;
            scale[1] = bone.Scale.Y;
            scale[2] = bone.Scale.Z;
            rotation[0] = bone.Rotation.X;
            rotation[1] = bone.Rotation.Y;
            rotation[2] = bone.Rotation.Z;
            rotation[3] = 1;
            position[0] = bone.Translation.X;
            position[1] = bone.Translation.Y;
            position[2] = bone.Translation.Z;
        }
    }
}
