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

            Bone = bone;
            Text = bone.Name;
        }
    }
}
