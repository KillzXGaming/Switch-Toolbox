using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using BcresLibrary;

namespace FirstPlugin
{
    public class CRESBoneWrapper : STBone
    {
        internal BCRES BcresParent;
        internal Bone Bone;

        public CRESBoneWrapper()
        {
            ImageKey = "Bone";
            SelectedImageKey = "Bone";
        }

        public CRESBoneWrapper(Bone bone, BCRES bcres) : base()
        {
            BcresParent = bcres;
            Load(bone, bcres);
        }

        public override void OnClick(TreeView treeview) {
            BcresParent.LoadEditors(this, OnPropertyChanged);
        }

        private void OnPropertyChanged()
        {

        }

        public void Load(Bone bone, BCRES bcres)
        {
            BcresParent = bcres;
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
            position[0] = bone.Position.X;
            position[1] = bone.Position.Y;
            position[2] = bone.Position.Z;

        }
    }
}
