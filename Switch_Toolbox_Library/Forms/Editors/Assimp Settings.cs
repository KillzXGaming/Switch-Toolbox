using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Assimp;

namespace Toolbox.Library.Forms
{
    public partial class Assimp_Settings : STForm
    {
        public uint SkinLimitMax = 4;
        public bool UseNodeTransform = true;
        public bool RotateSkeleton = false;
        public float RotateSkeletonAmount = 90;

        public Assimp_Settings()
        {
            InitializeComponent();
        }

        public PostProcessSteps GetFlags()
        {
            RotateSkeleton = rotateBones.Checked;
            RotateSkeletonAmount = (float)rotateBonesUD.Value;

            UseNodeTransform = useNodeTransform.Checked;

            SkinLimitMax = (uint)numericUpDown1.Value;

            var Flags = PostProcessSteps.None;
         //   Flags |= PostProcessSteps.GlobalScale;

            if (generateNormalsChk.Checked)
                Flags |= PostProcessSteps.GenerateNormals;
            if (smoothNormalsChk.Checked)
                Flags |= PostProcessSteps.GenerateSmoothNormals;
            if (flipUVsChk.Checked)
                Flags |= PostProcessSteps.FlipUVs;
            if (limtBoneWeightChk.Checked)
                Flags |= PostProcessSteps.LimitBoneWeights;
            if (joinDupedVertsSk.Checked)
                Flags |= PostProcessSteps.JoinIdenticalVertices;
            if (preTransformVerticesChk.Checked)
                Flags |= PostProcessSteps.PreTransformVertices;
            if (leftHandedChk.Checked)
                Flags |= PostProcessSteps.MakeLeftHanded;
            if (triangulateChk.Checked)
                Flags |= PostProcessSteps.Triangulate;

            return Flags;
        }
            
        private void limtBoneWeightChk_CheckedChanged(object sender, EventArgs e) {
            numericUpDown1.Enabled = limtBoneWeightChk.Checked;
        }

        private void rotateBonesY90_CheckedChanged(object sender, EventArgs e)
        {
            RotateSkeleton = rotateBones.Checked;
            rotateBonesUD.Enabled = RotateSkeleton;
        }

        private void rotateBonesUD_ValueChanged(object sender, EventArgs e)
        {
            RotateSkeletonAmount = (float)rotateBonesUD.Value;
        }
    }
}
