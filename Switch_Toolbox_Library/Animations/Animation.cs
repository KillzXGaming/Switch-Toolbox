using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;

namespace Switch_Toolbox.Library
{
    public class Animation : TreeNode
    {
        public float Frame = 0;
        public int FrameCount = 0;

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

        public enum InterpolationType
        {
            LINEAR = 0,
            CONSTANT,
            HERMITE,
            STEP,
        };

        public class KeyNode : TreeNode
        {
            public int Hash = -1;

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

            public KeyNode(String bname)
            {
                Text = bname;
                if (bname != null && bname.Equals("")) Text = Hash.ToString("x");
                ImageKey = "bone";
                SelectedImageKey = "bone";
            }

            public void SetKeyFromBone(float frame, STBone bone)
            {
                Vector3 rot = ANIM.quattoeul(bone.rot);
                if (rot.X != bone.rotation[0] || rot.Y != bone.rotation[1] || rot.Z != bone.rotation[2])
                {
                    XROT.GetKeyFrame(frame).Value = bone.rot.X;
                    YROT.GetKeyFrame(frame).Value = bone.rot.Y;
                    ZROT.GetKeyFrame(frame).Value = bone.rot.Z;
                    WROT.GetKeyFrame(frame).Value = bone.rot.W;
                }
                if (bone.pos.X != bone.position[0] || bone.pos.Y != bone.position[1] || bone.pos.Z != bone.position[2])
                {
                    XPOS.GetKeyFrame(frame).Value = bone.pos.X;
                    YPOS.GetKeyFrame(frame).Value = bone.pos.Y;
                    ZPOS.GetKeyFrame(frame).Value = bone.pos.Z;
                }
                if (bone.sca.X != bone.scale[0] || bone.sca.Y != bone.scale[1] || bone.sca.Z != bone.scale[2])
                {
                    XSCA.GetKeyFrame(frame).Value = bone.sca.X;
                    YSCA.GetKeyFrame(frame).Value = bone.sca.Y;
                    ZSCA.GetKeyFrame(frame).Value = bone.sca.Z;
                }
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

            public KeyFrame GetKeyFrame(float frame)
            {
                KeyFrame key = null;
                int i;
                for (i = 0; i < Keys.Count; i++)
                {
                    if (Keys[i].Frame == frame)
                    {
                        key = Keys[i];
                        break;
                    }
                    if (Keys[i].Frame > frame)
                    {
                        break;
                    }
                }

                if (key == null)
                {
                    key = new KeyFrame();
                    key.Frame = frame;
                    Keys.Insert(i, key);
                }

                return key;
            }

            int LastFound = 0;
            float LastFrame;
            public float GetValue(float frame)
            {
                KeyFrame k1 = (KeyFrame)Keys[0], k2 = (KeyFrame)Keys[0];
                int i = 0;
                if (frame < LastFrame)
                    LastFound = 0;
                for (i = LastFound; i < Keys.Count; i++)
                {
                    LastFound = i % (Keys.Count);
                    KeyFrame k = Keys[LastFound];
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
                LastFound -= 1;
                if (LastFound < 0)
                    LastFound = 0;
                if (LastFound >= Keys.Count - 2)
                    LastFound = 0;
                LastFrame = frame;

                if (k1.InterType == InterpolationType.CONSTANT)
                    return k1.Value;
                if (k1.InterType == InterpolationType.STEP)
                    return k1.Value;
                if (k1.InterType == InterpolationType.LINEAR)
                {
                    return Lerp(k1.Value, k2.Value, k1.Frame, k2.Frame, frame);
                }
                if (k1.InterType == InterpolationType.HERMITE)
                {
                    float val = Hermite(frame, k1.Frame, k2.Frame, k1.In, k1.Out != -1 ? k1.Out : k2.In, k1.Value, k2.Value) * (k1.Degrees ? (float)Math.PI / 180 : 1);
                    if (Parent != null && Text.Equals("XROT"))
                        Console.WriteLine(Text + " " + k1.Value + " " + k2.Value + " " + k1.Frame + " " + k2.Frame + " " + (val * 180 / (float)Math.PI));
                    if (float.IsNaN(val)) val = k1._value;

                    return val;//k1.Out != -1 ? k1.Out : 
                }

                return k1.Value;
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
            public float Value
            {
                get { if (Degrees) return _value * 180 / (float)Math.PI; else return _value; }
                set { _value = value; }//Text = _frame + " : " + _value; }
            }
            public float _value;
            public float Frame
            {
                get { return _frame; }
                set { _frame = value; }//Text = _frame + " : " + _value; }
            }
            public String Text;
            public float _frame;
            public float In = 0, Out = -1;
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

        public void NextFrame(STSkeleton skeleton, bool isChild = false)
        {
            if (Frame >= FrameCount) return;

            if (Frame == 0 && !isChild)
                skeleton.reset();

            foreach (object child in Children)
            {
                if (child is Animation)
                {
                    ((Animation)child).SetFrame(Frame);
                    ((Animation)child).NextFrame(skeleton, isChild: true);
                }
            }

            bool Updated = false; // no need to update skeleton of animations that didn't change
            foreach (KeyNode node in Bones)
            {
                // Get Skeleton Node
                STBone b = null;
                b = skeleton.getBone(node.Text);
                if (b == null) continue;
                Updated = true;

                b.pos.X = node.XPOS.GetValue(Frame);
                b.pos.Y = node.YPOS.GetValue(Frame);
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
                        float x = node.XROT.HasAnimation() ? node.XROT.GetValue(Frame) : b.rotation[0];
                        float y = node.YROT.HasAnimation() ? node.YROT.GetValue(Frame) : b.rotation[1];
                        float z = node.ZROT.HasAnimation() ? node.ZROT.GetValue(Frame) : b.rotation[2];
                        b.rot = EulerToQuat(z, y, x);
                    }
                }
            }
            Frame += 1f;
            if (Frame >= FrameCount)
            {
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


        public static float Hermite(float frame, float frame1, float frame2, float outslope, float inslope, float val1, float val2)
        {
            /*float offset = frame - frame1;
            float span = frame2 - frame1;
            if (offset == 0) return val1;
            if (offset == span) return val2;
            float diff = val2 - val1;
            float time = offset / span;
            
            //bool prevDouble = prevframe1 >= 0 && prevframe1 == frame1 - 1;
            //bool nextDouble = next._next._index >= 0 && next._next._index == next._index + 1;
            bool oneApart = frame2 == frame1 + 1;
            
            float tan = outslope, nextTan = inslope;
            if (oneApart)
                tan = (val2 - val1) / (frame2 - frame1);
            //if (oneApart)
                nextTan = (val2 - val1) / (frame2 - frame1);
            float inv = time - 1.0f; //-1 to 0
            return val1
                + (offset * inv * ((inv * tan) + (time * nextTan)))
                + ((time * time) * (3.0f - 2.0f * time) * diff);*/

            if (frame == frame1) return val1;
            if (frame == frame2) return val2;

            float distance = frame - frame1;
            float invDuration = 1f / (frame2 - frame1);
            float t = distance * invDuration;
            float t1 = t - 1f;
            return (val1 + ((((val1 - val2) * ((2f * t) - 3f)) * t) * t)) + ((distance * t1) * ((t1 * outslope) + (t * inslope)));
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
