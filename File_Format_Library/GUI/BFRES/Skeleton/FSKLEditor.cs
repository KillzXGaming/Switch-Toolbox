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
using Toolbox.Library.Forms;
using Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class FSKLEditor : UserControl
    {
        public FSKLEditor()
        {
            InitializeComponent();

            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;

            textBox1.BackColor = FormThemes.BaseTheme.ListViewBackColor;
            textBox1.ForeColor = FormThemes.BaseTheme.FormForeColor;
        }

        public FSKL activeSkeleton;
        bool IsLoaded = false;

        public void LoadSkeleton(FSKL fskl)
        {
            IsLoaded = false;

            rotationModeCB.DataSource = null;
            scalingModeCB.DataSource = null;
            rotationModeCB.Items.Clear();
            scalingModeCB.Items.Clear();

            activeSkeleton = fskl;

            if (fskl.node.SkeletonU != null)
            {
                foreach (var item in Enum.GetValues(typeof(ResU.SkeletonFlagsRotation)))
                    rotationModeCB.Items.Add(item);
                foreach (var item in Enum.GetValues(typeof(ResU.SkeletonFlagsScaling)))
                    scalingModeCB.Items.Add(item);

                rotationModeCB.SelectedItem = fskl.node.SkeletonU.FlagsRotation;
                scalingModeCB.SelectedItem = fskl.node.SkeletonU.FlagsScaling;
                Console.WriteLine("FlagsScaling " + fskl.node.SkeletonU.FlagsScaling);
            }
            else
            {
                foreach (var item in Enum.GetValues(typeof(SkeletonFlagsRotation)))
                    rotationModeCB.Items.Add(item);
                foreach (var item in Enum.GetValues(typeof(SkeletonFlagsScaling)))
                    scalingModeCB.Items.Add(item);

                rotationModeCB.SelectedItem = fskl.node.Skeleton.FlagsRotation;
                scalingModeCB.SelectedItem = fskl.node.Skeleton.FlagsScaling;
            }

            if (fskl.node.Skeleton != null && fskl.node.Skeleton.userIndices != null)
            {
                var indices = string.Join(",", fskl.node.Skeleton.userIndices);
                this.textBox1.Text = indices;
            }

            IsLoaded = true;
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
            if (!IsLoaded)
                return;

            if (activeSkeleton.node.SkeletonU != null)
            {
                activeSkeleton.node.SkeletonU.FlagsRotation = (ResU.SkeletonFlagsRotation)rotationModeCB.SelectedItem;
                activeSkeleton.node.SkeletonU.FlagsScaling = (ResU.SkeletonFlagsScaling)scalingModeCB.SelectedItem;

                foreach (var bone in activeSkeleton.bones)
                    bone.UseSegmentScaleCompensate = activeSkeleton.node.SkeletonU.FlagsScaling.HasFlag(SkeletonFlagsScaling.Maya);
                activeSkeleton.update();
            }
            else
            {
                activeSkeleton.node.Skeleton.FlagsRotation = (SkeletonFlagsRotation)rotationModeCB.SelectedItem;
                activeSkeleton.node.Skeleton.FlagsScaling = (SkeletonFlagsScaling)scalingModeCB.SelectedItem;

                foreach (var bone in activeSkeleton.bones)
                    bone.UseSegmentScaleCompensate = activeSkeleton.node.Skeleton.FlagsScaling.HasFlag(SkeletonFlagsScaling.Maya);
                activeSkeleton.update();
            }
            LibraryGUI.UpdateViewport();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (activeSkeleton.node.Skeleton != null)
            {
                List<ushort> indices = new List<ushort>();

                foreach (var line in textBox1.Text.Split(','))
                {
                    if (string.IsNullOrEmpty(line))
                        continue;

                    if (ushort.TryParse(line, out ushort id))
                        indices.Add(id);
                }
                activeSkeleton.node.Skeleton.userIndices = indices.ToArray();
            }
        }
    }
}
