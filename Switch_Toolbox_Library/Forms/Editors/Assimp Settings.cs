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

namespace Switch_Toolbox.Library.Forms
{
    public partial class Assimp_Settings : STForm
    {
        public uint SkinLimitMax = 4;
        public bool UseNodeTransform = true;
        public bool RotateSkeleton90Y = false;

        public Assimp_Settings()
        {
            InitializeComponent();
        }

        public PostProcessSteps GetFlags()
        {
            RotateSkeleton90Y = rotateBonesY90.Checked;

            UseNodeTransform = useNodeTransform.Checked;

            SkinLimitMax = (uint)numericUpDown1.Value;

            var Flags = PostProcessSteps.None;

            if (generateNormalsChk.Checked)
                Flags |= PostProcessSteps.GenerateNormals;
            if (smoothNormalsChk.Checked)
                Flags |= PostProcessSteps.GenerateSmoothNormals;
            if (generateTansBitansChk.Checked)
                Flags |= PostProcessSteps.CalculateTangentSpace;
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
            RotateSkeleton90Y = rotateBonesY90.Checked;
        }
    }
}
