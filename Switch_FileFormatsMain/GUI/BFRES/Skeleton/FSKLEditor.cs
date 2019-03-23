using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bfres.Structs;
using ResU = Syroot.NintenTools.Bfres;
using Syroot.NintenTools.NSW.Bfres;
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class FSKLEditor : UserControl
    {
        public FSKLEditor()
        {
            InitializeComponent();

            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;
        }

        public FSKL activeSkeleton;

        public void LoadSkeleton(FSKL fskl)
        {
            activeSkeleton = fskl;

            if (fskl.node.SkeletonU != null)
            {
                rotationModeCB.Bind(typeof(ResU.SkeletonFlagsRotation), fskl.node.SkeletonU, "FlagsRotation");
                scalingModeCB.Bind(typeof(ResU.SkeletonFlagsScaling), fskl.node.SkeletonU, "FlagsScaling");
                rotationModeCB.SelectedItem = fskl.node.SkeletonU.FlagsRotation;
                scalingModeCB.SelectedItem = fskl.node.SkeletonU.FlagsScaling;
            }
            else
            {
                rotationModeCB.Bind(typeof(SkeletonFlagsRotation), fskl.node.Skeleton, "FlagsRotation");
                scalingModeCB.Bind(typeof(SkeletonFlagsScaling), fskl.node.Skeleton, "FlagsScaling");
                rotationModeCB.SelectedItem = fskl.node.Skeleton.FlagsRotation;
                scalingModeCB.SelectedItem = fskl.node.Skeleton.FlagsScaling;
            }
        }

        private void btnRgidIndices_Click(object sender, EventArgs e)
        {
            BoneIndexList indexViewer = new BoneIndexList("Rigid Bone Index List");

            if (activeSkeleton.node.SkeletonU != null)
            {
                var skel = activeSkeleton.node.SkeletonU;

                indexViewer.LoadMatrixToIndexIndices(skel.GetRigidIndices(), activeSkeleton);
            }
            else
            {
                var skel = activeSkeleton.node.Skeleton;

                indexViewer.LoadMatrixToIndexIndices(skel.GetRigidIndices(), activeSkeleton);
            }

            indexViewer.Show(this);
        }

        private void btnSmoothIndices_Click(object sender, EventArgs e)
        {
            BoneIndexList indexViewer = new BoneIndexList("Smooth Bone Index List");

            if (activeSkeleton.node.SkeletonU != null)
            {
                var skel = activeSkeleton.node.SkeletonU;

                indexViewer.LoadMatrixToIndexIndices(skel.GetSmoothIndices(), activeSkeleton);
            }
            else
            {
                var skel = activeSkeleton.node.Skeleton;

                indexViewer.LoadMatrixToIndexIndices(skel.GetSmoothIndices(), activeSkeleton);
            }

            indexViewer.Show(this);
        }

        private void ModeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (activeSkeleton.node.SkeletonU != null)
            {
                activeSkeleton.node.SkeletonU.FlagsRotation = (ResU.SkeletonFlagsRotation)rotationModeCB.SelectedItem;
                activeSkeleton.node.SkeletonU.FlagsScaling = (ResU.SkeletonFlagsScaling)scalingModeCB.SelectedItem;
            }
            else
            {
                activeSkeleton.node.Skeleton.FlagsRotation = (SkeletonFlagsRotation)rotationModeCB.SelectedItem;
                activeSkeleton.node.Skeleton.FlagsScaling = (SkeletonFlagsScaling)scalingModeCB.SelectedItem;
            }
        }
    }
}
