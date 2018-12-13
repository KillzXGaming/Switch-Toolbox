using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GL_Core;
using GL_Core.Interfaces;

namespace Switch_Toolbox.Library
{
    public class STSkeleton : AbstractGlDrawable
    {
        public override void Prepare(GL_ControlModern control)
        {
        }
        public override void Prepare(GL_ControlLegacy control)
        {

        }
        public override void Draw(GL_ControlLegacy control)
        {
            if (Viewport.Instance.gL_ControlModern1 == null)
                return;

            control.ResetModelMatrix();

            foreach (STBone bn in bones)
            {
                bn.Render();
            }
        }
        public override void Draw(GL_ControlModern control)
        {
            foreach (STBone bn in bones)
            {
                bn.Render();
            }
        }

        public List<STBone> bones = new List<STBone>();

        public List<STBone> getBoneTreeOrder()
        {
            List<STBone> bone = new List<STBone>();
            Queue<STBone> q = new Queue<STBone>();

            q.Enqueue(bones[0]);

            while (q.Count > 0)
            {
                STBone b = q.Dequeue();
                foreach (STBone bo in b.GetChildren())
                    q.Enqueue(bo);
                bone.Add(b);
            }
            return bone;
        }

        public int boneIndex(string name)
        {
            for (int i = 0; i < bones.Count; i++)
            {
                if (bones[i].Text.Equals(name))
                {
                    return i;
                }
            }

            return -1;
        }

        public void reset(bool Main = true)
        {
            for (int i = 0; i < bones.Count; i++)
            {
                bones[i].pos = new Vector3(bones[i].position[0], bones[i].position[1], bones[i].position[2]);

                if (bones[i].boneRotationType == 1)
                {
                    bones[i].rot = (FromQuaternionAngles(bones[i].rotation[2], bones[i].rotation[1], bones[i].rotation[0], bones[i].rotation[3]));
                }
                else
                {
                    bones[i].rot = (FromEulerAngles(bones[i].rotation[2], bones[i].rotation[1], bones[i].rotation[0]));
                }
                bones[i].sca = new Vector3(bones[i].scale[0], bones[i].scale[1], bones[i].scale[2]);
            }
            update(true);
            for (int i = 0; i < bones.Count; i++)
            {
                try
                {
                    bones[i].invert = Matrix4.Invert(bones[i].transform);
                }
                catch (InvalidOperationException)
                {
                    bones[i].invert = Matrix4.Zero;
                }
            }
            update();
        }

        public STBone getBone(String name)
        {
            foreach (STBone bo in bones)
                if (bo.Text.Equals(name))
                    return bo;
            return null;
        }

        public static Quaternion FromQuaternionAngles(float z, float y, float x, float w)
        {
            {
                Quaternion q = new Quaternion();
                q.X = x;
                q.Y = y;
                q.Z = z;
                q.W = w;

                if (q.W < 0)
                    q *= -1;

                //return xRotation * yRotation * zRotation;
                return q;
            }
        }

        public static Quaternion FromEulerAngles(float z, float y, float x)
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

        private bool Updated = false;
        public void update(bool reset = false)
        {
            Updated = true;
            List<STBone> nodesToProcess = new List<STBone>();
            // Add all root nodes from the VBN
            foreach (STBone b in bones)
                if (b.Parent == null)
                    nodesToProcess.Add(b);

            // some special processing for the root bones before we start
            foreach (STBone b in nodesToProcess)
            {
                b.transform = Matrix4.CreateScale(b.sca) * Matrix4.CreateFromQuaternion(b.rot) * Matrix4.CreateTranslation(b.pos);
                // scale down the model in its entirety only when mid-animation (i.e. reset == false)
                if (!reset) b.transform *= Matrix4.CreateScale(1);
            }

            // Process as a tree from the root node's children and beyond. These
            // all use the same processing, unlike the root nodes.
            int numRootNodes = nodesToProcess.Count;
            for (int i = 0; i < numRootNodes; i++)
            {
                nodesToProcess.AddRange(nodesToProcess[0].GetChildren());
                nodesToProcess.RemoveAt(0);
            }
            while (nodesToProcess.Count > 0)
            {
                // DFS
                STBone currentBone = nodesToProcess[0];
                nodesToProcess.RemoveAt(0);
                nodesToProcess.AddRange(currentBone.GetChildren());

                // Process this node
                currentBone.transform = Matrix4.CreateScale(currentBone.sca) * Matrix4.CreateFromQuaternion(currentBone.rot) * Matrix4.CreateTranslation(currentBone.pos);
                if (currentBone.Parent != null)
                {
                    currentBone.transform = currentBone.transform * ((STBone)currentBone.Parent).transform;
                }
            }
        }
    }

    public class STBone : TreeNodeCustom
    {
        public STSkeleton skeletonParent;
        public UInt32 boneRotationType;
        public int BillboardIndex;
        public float[] position = new float[] { 0, 0, 0 };
        public float[] rotation = new float[] { 0, 0, 0 };
        public float[] scale = new float[] { 1, 1, 1 };

        public Vector3 pos = Vector3.Zero, sca = new Vector3(1f, 1f, 1f);
        public Quaternion rot = Quaternion.FromMatrix(Matrix3.Zero);
        public Matrix4 transform, invert;

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
                if (Parent == null)
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
        }

        public STBone()
        {
            ImageKey = "bone";
            SelectedImageKey = "bone";
        }

        public void Render()
        {
            Vector3 pos_c = Vector3.TransformPosition(Vector3.Zero, transform);

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
                Vector3 pos_p = Vector3.TransformPosition(Vector3.Zero, ((STBone)Parent).transform);
                GL.Vertex3(pos_c);
                GL.Color3(Color.Blue);
                GL.Vertex3(pos_p);
            }
            GL.End();
        }
    }
}