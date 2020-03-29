using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Bfres.Structs;
using Toolbox.Library.Forms;
using Syroot.NintenTools.NSW.Bfres;
using ResU = Syroot.NintenTools.Bfres;

namespace FirstPlugin
{
    public partial class BfresBoneEditor : UserControl
    {
        public BfresBoneEditor()
        {
            InitializeComponent();

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;

            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;

            posXUD.ValueChanged += new EventHandler(valueUD_ValueChanged);
            posYUD.ValueChanged += new EventHandler(valueUD_ValueChanged);
            posZUD.ValueChanged += new EventHandler(valueUD_ValueChanged);
            RotXUD.ValueChanged += new EventHandler(valueUD_ValueChanged);
            RotYUD.ValueChanged += new EventHandler(valueUD_ValueChanged);
            RotZUD.ValueChanged += new EventHandler(valueUD_ValueChanged);
            RotWUD.ValueChanged += new EventHandler(valueUD_ValueChanged);
            ScaXUD.ValueChanged += new EventHandler(valueUD_ValueChanged);
            ScaYUD.ValueChanged += new EventHandler(valueUD_ValueChanged);
            ScaZUD.ValueChanged += new EventHandler(valueUD_ValueChanged);
        }
        BfresBone activeBone;
        bool IsLoaded = false;

        public void LoadBone(BfresBone bn)
        {
            IsLoaded = false;

            rotModeCB.Items.Clear();
            billboardModeCB.Items.Clear();

            activeBone = bn;


            boneInfoPanel1.LoadBone(bn);

            rigidSkinningChkBox.Bind(bn, "UseRigidMatrix");
            smoothSkinningChkBox.Bind(bn, "UseSmoothMatrix");

            if (bn.BoneU != null)
            {
                var bone = bn.BoneU;
                
                userDataEditor1.LoadUserData(bone.UserData);

                foreach (var item in Enum.GetValues(typeof(ResU.BoneFlagsRotation)))
                    rotModeCB.Items.Add(item);

                foreach (var item in Enum.GetValues(typeof(ResU.BoneFlagsBillboard)))
                    billboardModeCB.Items.Add(item);

                rotModeCB.SelectedItem = bone.FlagsRotation;
                billboardModeCB.SelectedItem = bone.FlagsBillboard;

                billboardIndexUD.Value = bone.BillboardIndex;
                smoothIndexUD.Value = bone.SmoothMatrixIndex;
                rigidIndexUD.Value = bone.RigidMatrixIndex;

                chkTransformIdentity.Bind(bone, "TransformIdentity");
                chkTransformRotateTranslateZero.Bind(bone, "TransformRotateTranslateZero");
                chkTransformRotateZero.Bind(bone, "TransformRotateZero");
                chkTransformScaleOne.Bind(bone, "TransformScaleOne");
                chkTransformScaleUniform.Bind(bone, "TransformScaleUniform");
                chkTransformScaleVolumeOne.Bind(bone, "TransformScaleVolumeOne");
                chkTransformTranslateZero.Bind(bone, "TransformTranslateZero");

                chkTransformCuIdenity.Bind(bone, "TransformCumulativeIdentity");
                chkTransformCuRotateTrnaslateZero.Bind(bone, "TransformCumulativeRotateTranslateZero");
                chkTransformCuRotateZero.Bind(bone, "TransformCumulativeRotateZero");
                chkTransformCuScaleOne.Bind(bone, "TransformCumulativeScaleOne");
                chkTransformCuScaleUniform.Bind(bone, "TransformCumulativeScaleUniform");
                chkTransformCuScaleVolumeOne.Bind(bone, "TransformCumulativeScaleVolumeOne");
                chkTransformCuTranslateZero.Bind(bone, "TransformCumulativeTranslateZero");
            }
            else
            {
                var bone = bn.Bone;
                userDataEditor1.LoadUserData(bone.UserData.ToList());

                foreach (var item in Enum.GetValues(typeof(BoneFlagsBillboard)))
                    billboardModeCB.Items.Add(item);

                foreach (var item in Enum.GetValues(typeof(BoneFlagsRotation)))
                    rotModeCB.Items.Add(item);

                rotModeCB.SelectedItem = bone.FlagsRotation;
                billboardModeCB.SelectedItem = bone.FlagsBillboard;

                billboardIndexUD.Bind(bone, "BillboardIndex");
                smoothIndexUD.Bind(bone, "SmoothMatrixIndex");
                rigidIndexUD.Bind(bone, "RigidMatrixIndex");

          
                chkTransformIdentity.Bind(bone, "TransformIdentity");
                chkTransformRotateTranslateZero.Bind(bone, "TransformRotateTranslateZero");
                chkTransformRotateZero.Bind(bone, "TransformRotateZero");
                chkTransformScaleOne.Bind(bone, "TransformScaleOne");
                chkTransformScaleUniform.Bind(bone, "TransformScaleUniform");
                chkTransformScaleVolumeOne.Bind(bone, "TransformScaleVolumeOne");
                chkTransformTranslateZero.Bind(bone, "TransformTranslateZero");

                chkTransformCuIdenity.Bind(bone, "TransformCumulativeIdentity");
                chkTransformCuRotateTrnaslateZero.Bind(bone, "TransformCumulativeRotateTranslateZero");
                chkTransformCuRotateZero.Bind(bone, "TransformCumulativeRotateZero");
                chkTransformCuScaleOne.Bind(bone, "TransformCumulativeScaleOne");
                chkTransformCuScaleUniform.Bind(bone, "TransformCumulativeScaleUniform");
                chkTransformCuScaleVolumeOne.Bind(bone, "TransformCumulativeScaleVolumeOne");
                chkTransformCuTranslateZero.Bind(bone, "TransformCumulativeTranslateZero");
            }


            GetBoneTransform(bn);

            IsLoaded = true;
        }

        private void SetBoneTransform(BfresBone bn)
        {
            bn.Position = new OpenTK.Vector3(
                 (float)posXUD.Value,
                 (float)posYUD.Value,
                 (float)posZUD.Value
            );

            if ((BoneFlagsRotation)rotModeCB.SelectedItem == BoneFlagsRotation.Quaternion)
            {
                bn.Rotation = new OpenTK.Quaternion(
                     (float)RotXUD.Value,
                     (float)RotYUD.Value,
                     (float)RotZUD.Value,
                     (float)RotWUD.Value
                );
            }
            else
            {
                bn.EulerRotation = new OpenTK.Vector3(
                     (float)RotXUD.Value,
                     (float)RotYUD.Value,
                     (float)RotZUD.Value
                );
            }

            bn.Scale = new OpenTK.Vector3(
                 (float)ScaXUD.Value,
                 (float)ScaYUD.Value,
                 (float)ScaZUD.Value
            );

            bn.GenericToBfresBone();
        }

        private void GetBoneTransform(STBone bn)
        {
            posXUD.Value = (decimal)bn.Position.X;
            posYUD.Value = (decimal)bn.Position.Y;
            posZUD.Value = (decimal)bn.Position.Z;
            if (bn.RotationType == STBone.BoneRotationType.Quaternion) {
                RotXUD.Value = (decimal)bn.Rotation.X;
                RotYUD.Value = (decimal)bn.Rotation.Y;
                RotZUD.Value = (decimal)bn.Rotation.Z;
                RotWUD.Value = (decimal)bn.Rotation.W;
            }
            else
            {
                RotXUD.Value = (decimal)bn.EulerRotation.X;
                RotYUD.Value = (decimal)bn.EulerRotation.Y;
                RotZUD.Value = (decimal)bn.EulerRotation.Z;
                RotWUD.Value = (decimal)1.0f;
            }

            ScaXUD.Value = (decimal)bn.Scale.X;
            ScaYUD.Value = (decimal)bn.Scale.Y;
            ScaZUD.Value = (decimal)bn.Scale.Z;
        }

        private void rotMeasureCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (activeBone == null || !IsLoaded)
                return;
        }

        private void valueUD_ValueChanged(object sender, EventArgs e)
        {
            if (activeBone == null || !IsLoaded)
                return;

            SetBoneTransform(activeBone);
            activeBone.skeletonParent.reset();
            activeBone.skeletonParent.update(true);

         //   var Model = ((FSKL)activeBone.skeletonParent).GetModelParent();

        //    GetBoneTransform(activeBone);
        //    for (int s = 0; s < Model.shapes.Count; s++)
             //   Model.shapes[s].TransformBindedBone(activeBone.Text);

            LibraryGUI.UpdateViewport();
        }

        private void stDropDownPanel1_Load(object sender, EventArgs e)
        {

        }

        private void rotModeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (activeBone == null || !IsLoaded)
                return;

            SetBoneTransform(activeBone);
            LibraryGUI.UpdateViewport();
        }

        private void boneInfoPanel1_Load(object sender, EventArgs e)
        {

        }

        private void billboardModeCB_SelectedIndexChanged(object sender, EventArgs e) {
            if (activeBone == null || !IsLoaded)
                return;

            if (activeBone.BoneU != null)
            {
                activeBone.BoneU.FlagsBillboard = (ResU.BoneFlagsBillboard)billboardModeCB.SelectedItem;
            }
            else
            {
                activeBone.Bone.FlagsBillboard = (BoneFlagsBillboard)billboardModeCB.SelectedItem;
            }
        }

        private void chkTransformIdentity_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rigidSkinningChkBox_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
