using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Bfres.Structs;
using Switch_Toolbox.Library.Forms;
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

            foreach (var item in Enum.GetValues(typeof(BoneFlagsRotation)))
                rotModeCB.Items.Add(item);

            rigidSkinningChkBox.Bind(bn, "UseRigidMatrix");
            smoothSkinningChkBox.Bind(bn, "UseSmoothMatrix");

            if (bn.BoneU != null)
            {
                var bone = bn.BoneU;
                
                userDataEditor1.LoadUserData(bone.UserData);

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
            bn.position[0] = (float)posXUD.Value;
            bn.position[1] = (float)posYUD.Value;
            bn.position[2] = (float)posZUD.Value;

            bn.rotation[0] = (float)RotXUD.Value;
            bn.rotation[1] = (float)RotYUD.Value;
            bn.rotation[2] = (float)RotZUD.Value;
            bn.rotation[3] = (float)RotWUD.Value;

            bn.scale[0] = (float)ScaXUD.Value;
            bn.scale[1] = (float)ScaYUD.Value;
            bn.scale[2] = (float)ScaZUD.Value;

            if (bn.BoneU != null)
            {
                bn.BoneU.Position = new Syroot.Maths.Vector3F(bn.position[0], bn.position[1], bn.position[2]);
                bn.BoneU.Rotation = new Syroot.Maths.Vector4F(bn.rotation[0], bn.rotation[1], bn.rotation[2], bn.rotation[3]);
                bn.BoneU.Scale = new Syroot.Maths.Vector3F(bn.scale[0], bn.scale[1], bn.scale[2]);
            }
            else
            {
                bn.Bone.Position = new Syroot.Maths.Vector3F(bn.position[0], bn.position[1], bn.position[2]);
                bn.Bone.Rotation = new Syroot.Maths.Vector4F(bn.rotation[0], bn.rotation[1], bn.rotation[2], bn.rotation[3]);
                bn.Bone.Scale = new Syroot.Maths.Vector3F(bn.scale[0], bn.scale[1], bn.scale[2]);
            }
        }

        private void GetBoneTransform(STBone bn)
        {
            posXUD.Value = (decimal)bn.position[0];
            posYUD.Value = (decimal)bn.position[1];
            posZUD.Value = (decimal)bn.position[2];
            RotXUD.Value = (decimal)bn.rotation[0];
            RotYUD.Value = (decimal)bn.rotation[1];
            RotZUD.Value = (decimal)bn.rotation[2];
            RotWUD.Value = (decimal)bn.rotation[3];
            ScaXUD.Value = (decimal)bn.scale[0];
            ScaYUD.Value = (decimal)bn.scale[1];
            ScaZUD.Value = (decimal)bn.scale[2];
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

            LibraryGUI.Instance.UpdateViewport();
        }

        private void stDropDownPanel1_Load(object sender, EventArgs e)
        {

        }

        private void rotModeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (activeBone == null || !IsLoaded)
                return;

            if ((BoneFlagsRotation)rotModeCB.SelectedItem == BoneFlagsRotation.Quaternion)
            {
                activeBone.ConvertToQuaternion();
                SetBoneTransform(activeBone);
            }
            else
            {
                activeBone.ConvertToEular();
                SetBoneTransform(activeBone);
            }

            LibraryGUI.Instance.UpdateViewport();
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
