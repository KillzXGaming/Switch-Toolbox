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

        private Matrix4 transform;
        private Quaternion rotation = Quaternion.Identity;
        private Vector3 eulerRotation;

        /// <summary>
        /// Gets or sets the transformation of the bone.
        /// Setting this will adjust the 
        /// <see cref="Scale"/>, 
        /// <see cref="Rotation"/>, and 
        /// <see cref="Position"/> properties.
        /// </summary>
        public Matrix4 Transform
        {
            set
            {
             //   Scale = value.ExtractScale();
              //  Rotation = value.ExtractRotation();
             //   Position = value.ExtractTranslation();
                transform = value;
            }
            get
            {
                return transform;
            }
        }

        /// <summary>
        /// Gets or sets the position of the bone in world space.
        /// </summary>
        public Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the scale of the bone in world space.
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.One;

        /// <summary>
        /// Gets or sets the rotation of the bone in world space.
        /// </summary>
        public Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                if (RotationType == BoneRotationType.Euler)
                {
                    eulerRotation = new Vector3(value.X, value.Y, value.Z);
                    rotation = STMath.FromEulerAngles(eulerRotation);
                }
                else
                {
                    eulerRotation = STMath.ToEulerAngles(Rotation);
                    rotation = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Rotation"/> using euler method. 
        /// </summary>
        public Vector3 EulerRotation
        {
            get { return eulerRotation; }
            set {
                eulerRotation = value;
                rotation = STMath.FromEulerAngles(value);
                if (rotation.W == 0)
                    rotation.W = 1;
            }
        }

        public Vector3 pos = Vector3.Zero, sca = new Vector3(1f, 1f, 1f);
        public Quaternion rot = Quaternion.Identity;
        public Matrix4 invert;

        public Matrix4 GetTransform()
        {
            return Matrix4.CreateScale(Scale) *
                   Matrix4.CreateFromQuaternion(Rotation) * 
                   Matrix4.CreateTranslation(Position);
        }

        public void FromTransform(Matrix4 transform)
        {
            Scale = transform.ExtractScale();
            Position = transform.ExtractTranslation();
            rotation = transform.ExtractRotation();
            eulerRotation = STMath.ToEulerAngles(rotation);
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

        public int GetIndex()
        {
            if (skeletonParent != null)
                return skeletonParent.bones.IndexOf(this);
            else
                return -1;
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
            Checked = true;
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
