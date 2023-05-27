using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using SELib;
using Toolbox.Library.NodeWrappers;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;

namespace Toolbox.Library.Animations
{
    public enum InterpolationType
    {
        LINEAR = 0,
        CONSTANT,
        HERMITE,
        STEP,
        STEPBOOL,
    };

    public class Animation : STGenericWrapper
    {
        public bool CanLoop { get; set; }

        //Use to load data when clicked on. Can potentially speed up load times
        public virtual void OpenAnimationData() { }

        //Used to apply back and calculate some necessary data
        public virtual void UpdateAnimationData() { }

        public override void Export(string FileName) {}
        public override void Replace(string FileName) { }

        public bool IsEdited { get; set; }

        public bool IsBaked { get; set; }

        public float Frame = 0;
        public int FrameCount { get; set; } = 0;

        public List<KeyNode> Bones = new List<KeyNode>();

        public List<object> Children = new List<object>();

        public Animation()
        {
            ImageKey = "anim";
            SelectedImageKey = "anim";
        }

        public Animation(string Name)
        {
            Text = Name;
            ImageKey = "anim";
            SelectedImageKey = "anim";
        }

        public enum RotationType
        {
            EULER = 0,
            QUATERNION
        }

 

        public class KeyNode : TreeNodeCustom
        {
            public int Hash = -1;

            public bool UseSegmentScaleCompensate = false;

            public KeyGroup XPOS = new KeyGroup() { Text = "XPOS" };
            public KeyGroup YPOS = new KeyGroup() { Text = "YPOS" };
            public KeyGroup ZPOS = new KeyGroup() { Text = "ZPOS" };

            public RotationType RotType = RotationType.QUATERNION;
            public KeyGroup XROT = new KeyGroup() { Text = "XROT" };
            public KeyGroup YROT = new KeyGroup() { Text = "YROT" };
            public KeyGroup ZROT = new KeyGroup() { Text = "ZROT" };
            public KeyGroup WROT = new KeyGroup() { Text = "WROT" };

            public KeyGroup XSCA = new KeyGroup() { Text = "XSCA" };
            public KeyGroup YSCA = new KeyGroup() { Text = "YSCA" };
            public KeyGroup ZSCA = new KeyGroup() { Text = "ZSCA" };

            public KeyNode(string bname, bool LoadContextMenus = true)
            {
                Text = bname;
               // Text = SearchDuplicateNames(bname);
                if (bname != null && bname.Equals("")) Text = Hash.ToString("x");
                ImageKey = "bone";
                SelectedImageKey = "bone";

                if (LoadContextMenus)
                    LoadMenus();
            }

            public void LoadMenus()
            {
                //File Operations
                ContextMenuStrip = new STContextMenuStrip();
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Rename", null, RenameAction, Keys.Control | Keys.N));
                ContextMenuStrip.Items.Add(new ToolStripSeparator());
                ContextMenuStrip.Items.Add(new ToolStripMenuItem("Delete", null, DeleteAction, Keys.Control | Keys.Delete));
            }

            protected void DeleteAction(object sender, EventArgs e) { Delete(); Unload(); }
            protected void RenameAction(object sender, EventArgs e) { Rename(); }

            public virtual void Delete()
            {
                if (Parent != null)
                {
                    Remove();
                }
            }

            public virtual void Unload()
            {
                foreach (var node in Nodes)
                    if (node is STGenericWrapper)
                        ((STGenericWrapper)node).Unload();

                Nodes.Clear();
            }

            public virtual void Rename()
            {
                RenameDialog dialog = new RenameDialog();
                dialog.SetString(Text);

                if (dialog.ShowDialog() == DialogResult.OK) { Text = dialog.textBox1.Text; }
            }

            public bool HasKeyedFrames(float frame)
            {
                return (XPOS.HasAnimation() && XPOS.GetKeyFrame(frame).IsKeyed ||
                        YPOS.HasAnimation() && YPOS.GetKeyFrame(frame).IsKeyed ||
                        ZPOS.HasAnimation() && ZPOS.GetKeyFrame(frame).IsKeyed ||
                        XROT.HasAnimation() && XROT.GetKeyFrame(frame).IsKeyed ||
                        YROT.HasAnimation() && YROT.GetKeyFrame(frame).IsKeyed ||
                        ZROT.HasAnimation() && ZROT.GetKeyFrame(frame).IsKeyed ||
                        WROT.HasAnimation() && WROT.GetKeyFrame(frame).IsKeyed ||
                        XSCA.HasAnimation() && XSCA.GetKeyFrame(frame).IsKeyed ||
                        YSCA.HasAnimation() && YSCA.GetKeyFrame(frame).IsKeyed ||
                        ZSCA.HasAnimation() && ZSCA.GetKeyFrame(frame).IsKeyed);
            }

            public Vector3 GetPosition(float frame)
            {
                Vector3 pos = new Vector3(0);

                if (XPOS.HasAnimation()) pos.X = XPOS.GetValue(frame);
                if (YPOS.HasAnimation()) pos.Y = YPOS.GetValue(frame);
                if (ZPOS.HasAnimation()) pos.Z = ZPOS.GetValue(frame);

                return pos;
            }

            public Matrix4 Transform(float Frame)
            {
                Matrix4 Translaton = Matrix4.CreateTranslation(GetPosition(Frame));
                Matrix4 Rotation = Matrix4.CreateFromQuaternion(GetRotation(Frame));
                Matrix4 Scale = Matrix4.CreateScale(GetScale(Frame));

                return (Scale * Rotation * Translaton);
            }

            public Quaternion GetRotation(float frame)
            {
                Quaternion rot = new Quaternion();

                if (XROT.HasAnimation()) rot.X = XROT.GetValue(frame);
                if (YROT.HasAnimation()) rot.Y = YROT.GetValue(frame);
                if (ZROT.HasAnimation()) rot.Z = ZROT.GetValue(frame);
                if (WROT.HasAnimation()) rot.W = WROT.GetValue(frame);

                return rot;
            }

            public Vector3 GetEulerRotation(float frame)
            {
                Vector3 rot = new Vector3(0);

                float x = XROT.HasAnimation() ? XROT.GetValue(frame) : 0;
                float y = YROT.HasAnimation() ? YROT.GetValue(frame) : 0;
                float z = ZROT.HasAnimation() ? ZROT.GetValue(frame) : 0;

                return rot;
            }

            public Vector3 GetScale(float frame)
            {
                Vector3 sca = new Vector3(1);

                if (XSCA.HasAnimation()) sca.X = XSCA.GetValue(frame);
                if (YSCA.HasAnimation()) sca.Y = YSCA.GetValue(frame);
                if (ZSCA.HasAnimation()) sca.Z = ZSCA.GetValue(frame);

                return sca;
            }
        }

        public void ReplaceMe(Animation a)
        {
            Tag = null;
            Nodes.Clear();
            Bones.Clear();
            Children.Clear();

            Bones = a.Bones;

            FrameCount = a.FrameCount;
        }

        public class KeyGroup : TreeNode
        {
            public InterpolationType InterpolationType = InterpolationType.HERMITE;

            //Determines which data to use. Offset will shift for params.
            //Some values detmine the type ie (SRT) or (XYZW)
            public uint AnimDataOffset;

            public bool Constant = false;

            public float Scale { get; set; }

            public float Offset { get; set; }

            public float StartFrame { get; set; }

            public float EndFrame { get; set; }

            public float Delta { get; set; }

            public float CalculateDelta()
            {
                if (Keys.Count >= 2)
                {
                    float startValue = GetValue(0);
                    float endValue = GetValue(FrameCount);
                    return endValue - startValue;
                }
                else
                    return 0;
            }

            public bool HasAnimation()
            {
                return Keys.Count > 0;
            }

            public List<KeyFrame> Keys = new List<KeyFrame>();
            public float FrameCount
            {
                get
                {
                    float fc = 0;
                    foreach (KeyFrame k in Keys)
                        if (k.Frame > fc) fc = k.Frame;
                    return fc;
                }
            }

            public KeyFrame GetKeyFrame(float frame, bool InsertNewKey = true)
            {
                if (Keys.Count == 0) return null;
                KeyFrame k1 = (KeyFrame)Keys[0], k2 = (KeyFrame)Keys[0];
                foreach (KeyFrame k in Keys)
                {
                    if (k.Frame < frame)
                    {
                        k1 = k;
                    }
                    else
                    {
                        k2 = k;
                        break;
                    }
                }

                return k1;
            }

            public bool SetValue(float Value, int frame)
            {
                for (int i = 0; i < Keys.Count; i++)
                {
                    KeyFrame key = Keys[i];
                    if (key.Frame == frame)
                    {
                        key.Value = Value;
                        return true;
                    }   
                }
                return false;
            }

            private float GetWrapFrame(float frame)
            {
                var lastFrame = Keys.Last().Frame;
                if (frame < 0 || lastFrame < 0)
                    return frame;

                return frame;
            }

            public virtual float GetValue(float frame, float startFrame = 0)
            {
                if (Keys.Count == 0) return 0;
                if (Keys.Count == 1) return Keys[0].Value;
                KeyFrame LK = Keys.First();
                KeyFrame RK = Keys.Last();

                float Frame = GetWrapFrame(frame - startFrame);
                foreach (KeyFrame keyFrame in Keys)
                {
                    if (keyFrame.Frame <= Frame) LK = keyFrame;
                    if (keyFrame.Frame >= Frame && keyFrame.Frame < RK.Frame) RK = keyFrame;
                }
                if (LK == RK)
                    return LK.Value;

                return GetInterpolatedValue(Frame, LK, RK);
            }

            private float GetInterpolatedValue(float Frame, KeyFrame LK, KeyFrame RK)
            {
                float FrameDiff = Frame - LK.Frame;
                float Weight = FrameDiff / (RK.Frame - LK.Frame);

                switch (InterpolationType)
                {
                    case InterpolationType.CONSTANT: return LK.Value;
                    case InterpolationType.STEP: return LK.Value;
                    case InterpolationType.LINEAR: return InterpolationHelper.Lerp(LK.Value, RK.Value, Weight);
                    case InterpolationType.HERMITE:
                        float length = RK.Frame - LK.Frame;

                        return InterpolationHelper.HermiteInterpolate(Frame,
                         LK.Frame,
                         RK.Frame,
                         LK.Value,
                         RK.Value,
                         LK.Out * length,
                         RK.In * length);
                }

                return LK.Value;
            }

            public KeyFrame[] GetFrame(float frame)
            {
                if (Keys.Count == 0) return null;
                KeyFrame k1 = (KeyFrame)Keys[0], k2 = (KeyFrame)Keys[0];
                foreach (KeyFrame k in Keys)
                {
                    if (k.Frame < frame)
                    {
                        k1 = k;
                    }
                    else
                    {
                        k2 = k;
                        break;
                    }
                }

                return new KeyFrame[] { k1, k2 };
            }

            public void ExpandNodes()
            {
                Nodes.Clear();
                foreach (KeyFrame v in Keys)
                {
                    Nodes.Add(v.GetNode());
                }
            }
        }

        public class KeyFrame
        {
            public bool IsKeyed = false;

            public float Value1, Delta;

            public float Slope1;
            public float Slope2;

            public float Value
            {
                get { if (Degrees) return _value * 180 / (float)Math.PI; else return _value; }
                set { _value = value; }
            }
            public float _value;
            public float Frame
            {
                get { return _frame; }
                set { _frame = value; }
            }
            public String Text;
            public float _frame;

            public float In = 0;
            public float Out = -1;

            //     public float In = 0, Out = -1;
            public bool Weighted = false;
            public bool Degrees = false; // Use Degrees
            public InterpolationType InterType = InterpolationType.LINEAR;

            public KeyFrame(float value, float frame)
            {
                Value = value;
                Frame = frame;
            }

            public KeyFrame()
            {

            }

            public TreeNode GetNode()
            {
                TreeNode t = new TreeNode();
                t.Text = Frame + " : " + Value + (In != 0 ? " " + In.ToString() : "");
                t.Tag = this;
                return t;
            }

            public override string ToString()
            {
                return Frame + " " + Value;
            }
        }

        public void SetFrame(float frame)
        {
            Frame = frame;
        }

        public int Size()
        {
            return FrameCount;
        }

        public void NextFrame(STSkeleton skeleton, bool isChild = false, bool AdancedNextFrame = false) 
        {
            if (Frame > FrameCount) return;

            if (Frame == 0 && !isChild)
                skeleton.reset();

            foreach (object child in Children)
            {
                if (child is Animation)
                {
                    ((Animation)child).SetFrame(Frame);
                    ((Animation)child).NextFrame(skeleton, isChild, AdancedNextFrame);
                }
            }

            bool Updated = false; // no need to update skeleton of animations that didn't change
            foreach (KeyNode node in Bones)
            {
                // Get Skeleton Node
                STBone b = null;
                b = skeleton.GetBone(node.Text);
                if (b == null) continue;

                b.UseSegmentScaleCompensate = node.UseSegmentScaleCompensate;

                Updated = true;

                if (node.XPOS.HasAnimation())
                    b.pos.X = node.XPOS.GetValue(Frame);
                if (node.YPOS.HasAnimation())
                    b.pos.Y = node.YPOS.GetValue(Frame);
                if (node.ZPOS.HasAnimation())
                    b.pos.Z = node.ZPOS.GetValue(Frame);

                if (node.XSCA.HasAnimation())
                    b.sca.X = node.XSCA.GetValue(Frame);
                else b.sca.X = 1;
                if (node.YSCA.HasAnimation())
                    b.sca.Y = node.YSCA.GetValue(Frame);
                else b.sca.Y = 1;
                if (node.ZSCA.HasAnimation())
                    b.sca.Z = node.ZSCA.GetValue(Frame);
                else b.sca.Z = 1;


                if (node.XROT.HasAnimation() || node.YROT.HasAnimation() || node.ZROT.HasAnimation())
                {
                    if (node.RotType == RotationType.QUATERNION)
                    {
                        KeyFrame[] x = node.XROT.GetFrame(Frame);
                        KeyFrame[] y = node.YROT.GetFrame(Frame);
                        KeyFrame[] z = node.ZROT.GetFrame(Frame);
                        KeyFrame[] w = node.WROT.GetFrame(Frame);
                        Quaternion q1 = new Quaternion(x[0].Value, y[0].Value, z[0].Value, w[0].Value);
                        Quaternion q2 = new Quaternion(x[1].Value, y[1].Value, z[1].Value, w[1].Value);
                        if (x[0].Frame == Frame)
                            b.rot = q1;
                        else
                        if (x[1].Frame == Frame)
                            b.rot = q2;
                        else
                            b.rot = Quaternion.Slerp(q1, q2, (Frame - x[0].Frame) / (x[1].Frame - x[0].Frame));
                    }
                    else
                    if (node.RotType == RotationType.EULER)
                    {
                        float x = node.XROT.HasAnimation() ? node.XROT.GetValue(Frame) : b.EulerRotation.X;
                        float y = node.YROT.HasAnimation() ? node.YROT.GetValue(Frame) : b.EulerRotation.Y;
                        float z = node.ZROT.HasAnimation() ? node.ZROT.GetValue(Frame) : b.EulerRotation.Z;
                        b.rot = STMath.FromEulerAngles(new Vector3(x, y, z));
                    }
                }
            }

            if (AdancedNextFrame)
            {
                Frame++;
                if (Frame >= FrameCount)
                    Frame = 0;
            }

            if (!isChild && Updated)
            {
                skeleton.update();
            }
        }

        public void ExpandBones()
        {
            Nodes.Clear();
            foreach (var v in Bones)
                Nodes.Add(v);
        }

        public bool HasBone(String name)
        {
            foreach (var v in Bones)
                if (v.Text.Equals(name))
                    return true;
            return false;
        }

        public KeyNode GetBone(String name)
        {
            foreach (var v in Bones)
                if (v.Text.Equals(name))
                    return v;
            return null;
        }

        #region  Interpolation

        public static float CubicEval(float cf0, float cf1, float cf2, float cf3, float t)
        {
            return ((cf3 * t + cf2) * t) * t + (cf1 * t + cf0);

           // return (((cf0 * t + cf1) * t + cf2) * t + cf3);
        }

        public static float HermiteInterpolate(float frame, float frame1, float frame2,
        float inSlope, float outSlope, float p0, float p1)
        {
            if (frame == frame1) return p0;
            if (frame == frame2) return p1;

            float t = (frame - frame1) / (frame2 - frame1);
            return GetPointHermite(p0, p1, outSlope, inSlope, t);
        }

        private static float GetPointHermite(float p0, float p1, float s0, float s1, float t)
        {
            float cf0 = (p0 * 2) + (p1 * -2) + (s0 * 1) + (s1 * 1);
            float cf1 = (p0 * -3) + (p1 * 3) + (s0 * -2) + (s1 * -1);
            float cf2 = (p0 * 0) + (p1 * 0) + (s0 * 1) + (s1 * 0);
            float cf3 = (p0 * 1) + (p1 * 0) + (s0 * 0) + (s1 * 0);
            return GetPointCubic(cf0, cf1, cf2, cf3, t);
        }

        private static float GetPointBezier(float p0, float p1, float p2, float p3, float t)
        {
            float cf0 = (p0 * -1) + (p1 * 3) + (p2 * -3) + (p3 * 1);
            float cf1 = (p0 * 3) + (p1 * -6) + (p2 * 3) + (p3 * 0);
            float cf2 = (p0 * -3) + (p1 * 3) + (p2 * 0) + (p3 * 0);
            float cf3 = (p0 * 1) + (p1 * 0) + (p2 * 0) + (p3 * 0);
            return GetPointCubic(cf0, cf1, cf2, cf3, t);
        }

        private static float GetPointCubic(float cf0, float cf1, float cf2, float cf3, float t)
        {
            return (((cf0 * t + cf1) * t + cf2) * t + cf3);
        }

        public static float Lerp(float av, float bv, float v0, float v1, float t)
        {
            if (v0 == v1) return av;

            if (t == v0) return av;
            if (t == v1) return bv;


            float mu = (t - v0) / (v1 - v0);
            return ((av * (1 - mu)) + (bv * mu));
        }

        public static Quaternion Slerp(Vector4 v0, Vector4 v1, double t)
        {
            v0.Normalize();
            v1.Normalize();

            double dot = Vector4.Dot(v0, v1);

            const double DOT_THRESHOLD = 0.9995;
            if (Math.Abs(dot) > DOT_THRESHOLD)
            {
                Vector4 result = v0 + new Vector4((float)t) * (v1 - v0);
                result.Normalize();
                return new Quaternion(result.Xyz, result.W);
            }
            if (dot < 0.0f)
            {
                v1 = -v1;
                dot = -dot;
            }

            if (dot < -1) dot = -1;
            if (dot > 1) dot = 1;
            double theta_0 = Math.Acos(dot);  // theta_0 = angle between input vectors
            double theta = theta_0 * t;    // theta = angle between v0 and result 

            Vector4 v2 = v1 - v0 * new Vector4((float)dot);
            v2.Normalize();              // { v0, v2 } is now an orthonormal basis

            Vector4 res = v0 * new Vector4((float)Math.Cos(theta)) + v2 * new Vector4((float)Math.Sign(theta));
            return new Quaternion(res.Xyz, res.W);
        }

        public static Quaternion EulerToQuat(float z, float y, float x)
        {
            {
                Quaternion xRotation = Quaternion.FromAxisAngle(Vector3.UnitX, x);
                Quaternion yRotation = Quaternion.FromAxisAngle(Vector3.UnitY, y);
                Quaternion zRotation = Quaternion.FromAxisAngle(Vector3.UnitZ, z);

                Quaternion q = (zRotation * yRotation * xRotation);

                if (q.W < 0)
                    q *= -1;

                //return xRotation * yRotation * zRotation;
                return q;
            }
        }
        #endregion
    }
}
