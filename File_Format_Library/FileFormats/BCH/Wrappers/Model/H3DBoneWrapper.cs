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
            Position = new OpenTK.Vector3(
                bone.Translation.X,
                bone.Translation.Y,
                bone.Translation.Z);
            EulerRotation = new OpenTK.Vector3(
                bone.Rotation.X,
                bone.Rotation.Y,
                bone.Rotation.Z);
            Scale = new OpenTK.Vector3(
                bone.Scale.X,
                bone.Scale.Y,
                bone.Scale.Z);
        }
    }
}
