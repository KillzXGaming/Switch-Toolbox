using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Toolbox.Library
{
    public class STBone : TreeNodeCustom
    {
        private bool visbile = true;
        public bool Visible
        {
            get
            {
                return visbile;
            }
            set
            {
                visbile = value;
            }
        }

        public bool UseSegmentScaleCompensate;

        public STSkeleton skeletonParent;
        public BoneRotationType RotationType;
        public short BillboardIndex;
        public short RigidMatrixIndex;
        public short SmoothMatrixIndex;

        public float[] position = new float[] { 0, 0, 0 };
        public float[] rotation = new float[] { 0, 0, 0 };
        public float[] scale = new float[] { 1, 1, 1 };

        public Vector3 pos = Vector3.Zero, sca = new Vector3(1f, 1f, 1f);
        public Quaternion rot = Quaternion.FromMatrix(Matrix3.Zero);
        public Matrix4 Transform, invert;


        public Matrix4 GetTransform()
        {
            Vector3 mPos = new Vector3(position[0], position[1],position[2]);

            Quaternion mRot;
            if (RotationType == STBone.BoneRotationType.Quaternion)
                mRot = (STSkeleton.FromQuaternionAngles(rotation[2], rotation[1],rotation[0], rotation[3]));
            else
                mRot = (STSkeleton.FromEulerAngles(rotation[2],rotation[1],rotation[0]));

            Vector3 mSca = new Vector3(scale[0], scale[1],scale[2]);

            return Matrix4.CreateScale(mSca) * Matrix4.CreateFromQuaternion(mRot) * Matrix4.CreateTranslation(mPos);
        }

        //Used for shifting models with the bones in the shader
        public Matrix4 ModelMatrix = Matrix4.Identity;

        public Vector3 GetPosition()
        {
            return pos;
        }

        public Quaternion GetRotation()
        {
            return rot;
        }

        public Vector3 GetScale()
        {
            return sca;
        }

        public void FromTransform(Matrix4 Transform)
        {
            var pos = Transform.ExtractTranslation();
            var quat = Transform.ExtractRotation();
            var scale = Transform.ExtractScale();

            position[0] = pos.X;
            position[1] = pos.X;
            position[2] = pos.Z;

            var eul = Toolbox.Library.Animations.ANIM.quattoeul(quat);
            rotation = new float[] { eul.X, eul.Y, eul.Z, 1 };

            scale[0] = scale.X;
            scale[1] = scale.X;
            scale[2] = scale.Z;
        }

        private void ApplyTransforms()
        {
            position = new float[] { pos.X, pos .Y, pos .Z};
            if (RotationType == BoneRotationType.Quaternion)
                rotation = new float[] { rot.X, rot.Y, rot.Z, rot.W };
            else
            {
                var eul = Toolbox.Library.Animations.ANIM.quattoeul(rot);
                rotation = new float[] { eul.X, eul.Y, eul.Z, 1 };
            }

            scale = new float[] { sca.X, sca.Y, sca.Z };
        }

        public int GetIndex()
        {
            if (skeletonParent != null)
                return skeletonParent.bones.IndexOf(this);
            else
                return -1;
        }

        public void ConvertToQuaternion()
        {
            if (RotationType == BoneRotationType.Quaternion)
                return;

            RotationType = STBone.BoneRotationType.Quaternion;

            ApplyTransforms();

            //Update matrices
            skeletonParent.reset();
            skeletonParent.update();
        }

        public void ConvertToEular()
        {
            if (RotationType == BoneRotationType.Euler)
                return;

            RotationType = STBone.BoneRotationType.Euler;

            ApplyTransforms();

            //Update matrices
            skeletonParent.reset();
            skeletonParent.update();
        }

        public override void OnClick(TreeView treeView)
        {

        }

        public enum BoneRotationType
        {
            Euler,
            Quaternion,
        }

        public int parentIndex
        {
            set
            {
                if (Parent != null) Parent.Nodes.Remove(this);
                if (value > -1 && value < skeletonParent.bones.Count)
                {
                    skeletonParent.bones[value].Nodes.Add(this);
                }
            }

            get
            {
                if (Parent == null || !(Parent is STBone))
                    return -1;


                return skeletonParent.bones.IndexOf((STBone)Parent);
            }
        }

        public List<STBone> GetChildren()
        {
            List<STBone> l = new List<STBone>();
            foreach (STBone b in skeletonParent.bones)
                if (b.Parent == this)
                    l.Add(b);
            return l;
        }

        public STBone(STSkeleton skl)
        {
            skeletonParent = skl;
            ImageKey = "bone";
            SelectedImageKey = "bone";

            Checked = true;
        }

        public STBone()
        {
            ImageKey = "bone";
            SelectedImageKey = "bone";
        }

        public void RenderLegacy()
        {
            if (!Runtime.OpenTKInitialized || !Runtime.renderBones)
                return;

            Vector3 pos_c = Vector3.TransformPosition(Vector3.Zero, Transform);

            if (IsSelected)
            {
                GL.Color3(Color.Red);
            }
            else
                GL.Color3(Color.GreenYellow);

            RenderTools.DrawCube(pos_c, 0.1f);

            // now draw line between parent
            GL.Color3(Color.LightBlue);
            GL.LineWidth(2f);

            GL.Begin(PrimitiveType.Lines);
            if (Parent != null && Parent is STBone)
            {
                Vector3 pos_p = Vector3.TransformPosition(Vector3.Zero, ((STBone)Parent).Transform);
                GL.Vertex3(pos_c);
                GL.Color3(Color.Blue);
                GL.Vertex3(pos_p);
            }
            GL.End();
        }
    }
}
