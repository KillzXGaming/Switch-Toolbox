using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.EditorDrawables;
using Toolbox.Library.Rendering;
using SF = SFGraphics.GLObjects.Shaders;

using static GL_EditorFramework.EditorDrawables.EditorSceneBase;

namespace Toolbox.Library
{
    public class STSkeleton : EditableObject
    {
        /// <summary>
        /// A list of bone indices used to remap bones from an exported model
        /// </summary>
        public virtual int[] BoneIndices { get; }

        public virtual float PreviewScale { get; set; } = 1.0f;

        public virtual float BonePointScale { get; set; } = 1.0f;

        public Vector3 position = new Vector3(0, 0, 0);

        protected bool Selected = false;
        protected bool Hovered = false;

        public override bool IsSelected() => Selected;
        public override bool IsSelected(int partIndex) => Selected;

        public bool IsHovered() => Selected;

        public override void Prepare(GL_ControlModern control)
        {

        }
        public override void Prepare(GL_ControlLegacy control)
        {

        }

        int vbo_position;
        public void Destroy()
        {
            bool buffersWereInitialized = vbo_position != 0;
            if (!buffersWereInitialized)
                return;

            GL.DeleteBuffer(vbo_position);
        }

        public override void Draw(GL_ControlLegacy control, Pass pass, EditorSceneBase editorScene)
        {
            if (!Runtime.OpenTKInitialized || pass == Pass.TRANSPARENT)
                return;

            if (Runtime.boneXrayDisplay)
                GL.Disable(EnableCap.DepthTest);

            GL.Disable(EnableCap.Texture2D);
            foreach (STBone bn in bones)
            {
                bn.RenderLegacy();
            }
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
        }

        public override void Draw(GL_ControlLegacy control, Pass pass)
        {
            if (!Runtime.OpenTKInitialized || pass == Pass.TRANSPARENT)
                return;

            if (Runtime.boneXrayDisplay)
                GL.Disable(EnableCap.DepthTest);

            GL.Disable(EnableCap.Texture2D);
            foreach (STBone bn in bones)
            {
                bn.RenderLegacy();
            }
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
        }

        private static List<Vector4> screenPositions = new List<Vector4>()
        {
            // cube
            new Vector4(0f, 0f, -1f, 0),
            new Vector4(1f, 0f, 0f, 0),
            new Vector4(1f, 0f, 0f, 0),
            new Vector4(0f, 0f, 1f, 0),
            new Vector4(0f, 0f, 1f, 0),
            new Vector4(-1f, 0f, 0f, 0),
            new Vector4(-1f, 0f, 0f, 0),
            new Vector4(0f, 0f, -1f, 0),

            //point top parentless
            new Vector4(0f, 0f, -1f, 0),
            new Vector4(0f, 1f, 0f, 0),
            new Vector4(0f, 0f, 1f, 0),
            new Vector4(0f, 1f, 0f, 0),
            new Vector4(1f, 0f, 0f, 0),
            new Vector4(0f, 1f, 0f, 0),
            new Vector4(-1f, 0f, 0f, 0),
            new Vector4(0f, 1f, 0f, 0),

            //point top
            new Vector4(0f, 0f, -1f, 0),
            new Vector4(0f, 1f, 0f, 1),
            new Vector4(0f, 0f, 1f, 0),
            new Vector4(0f, 1f, 0f, 1),
            new Vector4(1f, 0f, 0f, 0),
            new Vector4(0f, 1f, 0f, 1),
            new Vector4(-1f, 0f, 0f, 0),
            new Vector4(0f, 1f, 0f, 1),

            //point bottom
            new Vector4(0f, 0f, -1f, 0),
            new Vector4(0f, -1f, 0f, 0),
            new Vector4(0f, 0f, 1f, 0),
            new Vector4(0f, -1f, 0f, 0),
            new Vector4(1f, 0f, 0f, 0),
            new Vector4(0f, -1f, 0f, 0),
            new Vector4(-1f, 0f, 0f, 0),
            new Vector4(0f, -1f, 0f, 0),
        };

        Vector4[] Vertices
        {
            get
            {
                return screenPositions.ToArray();
            }
        }

        public void UpdateVertexData()
        {
            GL.GenBuffers(1, out vbo_position);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<Vector4>(BufferTarget.ArrayBuffer,
                                   new IntPtr(Vertices.Length * Vector4.SizeInBytes),
                                   Vertices, BufferUsageHint.StaticDraw);
        }


        private static Matrix4 prismRotation = Matrix4.CreateFromAxisAngle(new Vector3(0, 0, 1), 1.5708f);

        private void CheckBuffers()
        {
            if (!Runtime.OpenTKInitialized)
                return;

            bool buffersWereInitialized = vbo_position != 0;
            if (!buffersWereInitialized)
            {
                GL.GenBuffers(1, out vbo_position);
                UpdateVertexData();
            }
        }

        public override void Draw(GL_ControlModern control, Pass pass)
        {

        }

        Color boneColor = Color.FromArgb(255, 240, 240, 0);
        Color selectedBoneColor = Color.FromArgb(255, 240, 240, 240);

        private void DrawBoundingBoxes()
        {
         /*   var boundings = GetSelectionBox();

            DrawableBoundingBox.DrawBoundingBox(
                new Vector3(boundings.minX, boundings.minY, boundings.minZ),
                new Vector3(boundings.maxX, boundings.maxY, boundings.maxZ),
                new Vector3(0)
                );*/

            return;
        }

        public bool HideSkeleton;

        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
        {
            CheckBuffers();

            if (!Runtime.OpenTKInitialized || !Runtime.renderBones || !Visible || HideSkeleton)
                return;

            SF.Shader shader = OpenTKSharedResources.shaders["BONE"];
            shader.UseProgram();

            GL.Disable(EnableCap.CullFace);

            if (Runtime.boneXrayDisplay)
                GL.Disable(EnableCap.DepthTest);

            if (Runtime.renderBoundingBoxes)
                DrawBoundingBoxes();

            control.UpdateModelMatrix(
            Matrix4.CreateScale(Runtime.previewScale * PreviewScale) *
            Matrix4.CreateTranslation(Selected ? editorScene.CurrentAction.NewPos(position) : position));


            shader.EnableVertexAttributes();
            shader.SetMatrix4x4("rotation", ref prismRotation);

            Matrix4 camMat = control.CameraMatrix;
            Matrix4 mdlMat = control.ModelMatrix;
            Matrix4 projMat = control.ProjectionMatrix;
            Matrix4 computedCamMtx = camMat * projMat;

            shader.SetMatrix4x4("mtxCam", ref computedCamMtx);
            shader.SetMatrix4x4("mtxMdl", ref mdlMat);

            foreach (STBone bn in bones)
            {
                if (!bn.Checked)
                    continue;

                shader.SetVector4("boneColor", ColorUtility.ToVector4(boneColor));
                shader.SetFloat("scale", Runtime.bonePointSize * BonePointScale);
                shader.SetMatrix4x4("ModelMatrix", ref bn.ModelMatrix);


                Matrix4 transform = bn.Transform;

                shader.SetMatrix4x4("bone", ref transform);
                shader.SetInt("hasParent", bn.parentIndex != -1 ? 1 : 0);

                if (bn.parentIndex != -1)
                {
                    var transformParent = ((STBone)bn.Parent).Transform;
                    shader.SetMatrix4x4("parent", ref transformParent);
                }

                Draw(shader);

                if (Runtime.SelectedBoneIndex == bn.GetIndex())
                    shader.SetVector4("boneColor", ColorUtility.ToVector4(selectedBoneColor));

                shader.SetInt("hasParent", 0);
                Draw(shader);
            }

            shader.DisableVertexAttributes();

            GL.UseProgram(0);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
        }

        private void Attributes(SF.Shader shader)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(shader.GetAttribLocation("point"), 4, VertexAttribPointerType.Float, false, 16, 0);
        }
        private void Draw(SF.Shader shader)
        {
            Attributes(shader);

            GL.DrawArrays(PrimitiveType.Lines, 0, Vertices.Length);
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

        public Matrix4 GetBoneTransform(int index) {
            return GetBoneTransform(bones[index]);
        }

        public Matrix4 GetBoneTransform(STBone Bone)
        {
            if (Bone == null)
                return Matrix4.Identity;
            if (Bone.parentIndex == -1)
                return Bone.GetTransform();
            else
                return Bone.GetTransform() * GetBoneTransform(bones[Bone.parentIndex]);
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
                bones[i].pos = new Vector3(
                    bones[i].Position.X,
                    bones[i].Position.Y,
                    bones[i].Position.Z);
                bones[i].rot = new Quaternion(
                    bones[i].Rotation.X, 
                    bones[i].Rotation.Y, 
                    bones[i].Rotation.Z, 
                    bones[i].Rotation.W);
                bones[i].sca = new Vector3(
                    bones[i].Scale.X, 
                    bones[i].Scale.Y, 
                    bones[i].Scale.Z);
            }
            update(true);
            for (int i = 0; i < bones.Count; i++)
            {
                try
                {
                    bones[i].invert = Matrix4.Invert(bones[i].Transform);
                }
                catch (InvalidOperationException)
                {
                    bones[i].invert = Matrix4.Zero;
                }
            }
            update();
        }

        public STBone GetBone(String name)
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
                if (b.parentIndex == -1)
                    nodesToProcess.Add(b);

            // some special processing for the root bones before we start
            foreach (STBone b in nodesToProcess)
            {
                b.Transform = Matrix4.CreateScale(b.sca) * Matrix4.CreateFromQuaternion(b.rot) * Matrix4.CreateTranslation(b.pos);

                // scale down the model in its entirety only when mid-animation (i.e. reset == false)
                if (!reset) b.Transform *= Matrix4.CreateScale(1);
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
                STBone Bone = nodesToProcess[0];
                nodesToProcess.RemoveAt(0);
                nodesToProcess.AddRange(Bone.GetChildren());

                // Process this node
                Bone.Transform = Matrix4.CreateScale(Bone.sca) * Matrix4.CreateFromQuaternion(Bone.rot) * Matrix4.CreateTranslation(Bone.pos);
                if (Bone.parentIndex != -1)
                {
                    if (Bone.UseSegmentScaleCompensate && Bone.Parent != null
                        && Bone.Parent is STBone)
                    {
                        Bone.Transform *= Matrix4.CreateScale(
                              1f / ((STBone)Bone.Parent).GetScale().X,
                              1f / ((STBone)Bone.Parent).GetScale().Y,
                              1f / ((STBone)Bone.Parent).GetScale().Z);

                        Bone.Transform *= ((STBone)Bone.Parent).Transform;
                    }
                    else
                    {
                        Bone.Transform = Bone.Transform * ((STBone)Bone.Parent).Transform;
                    }
                }
            }
        }

        public override void GetSelectionBox(ref BoundingBox boundingBox)
        {
            for (int i = 1; i < bones.Count; i++)
            {
                boundingBox.Include(bones[i - 1].pos);
                boundingBox.Include(bones[i].pos);
            }
        }

        public override LocalOrientation GetLocalOrientation(int partIndex)
        {
            return new LocalOrientation(position);
        }

        public override bool TryStartDragging(DragActionType actionType, int hoveredPart, out LocalOrientation localOrientation, out bool dragExclusively)
        {
            localOrientation = new LocalOrientation(position);
            dragExclusively = false;
            return Selected;
        }

        public override bool IsInRange(float range, float rangeSquared, Vector3 pos)
        {
            return true;

            BoundingBox box;
            for (int i = 1; i < bones.Count; i++)
            {
                box = BoundingBox.Default;
                box.Include(bones[i - 1].pos);
                box.Include(bones[i].pos);

                if (pos.X < box.maxX + range && pos.X > box.minX - range &&
                    pos.Y < box.maxY + range && pos.Y > box.minY - range &&
                    pos.Z < box.maxZ + range && pos.Z > box.minZ - range)
                    return true;
            }
            return false;
        }

        public override uint SelectAll(GL_ControlBase control, ISet<object> selectedObjects)
        {
            Selected = true;
            return REDRAW;
        }

        public override uint SelectDefault(GL_ControlBase control, ISet<object> selectedObjects)
        {
            Selected = true;
            return REDRAW;
        }

        public override uint Select(int partIndex, GL_ControlBase control, ISet<object> selectedObjects)
        {
            Selected = true;
            return REDRAW;
        }

        public override uint Deselect(int partIndex, GL_ControlBase control, ISet<object> selectedObjects)
        {
            Selected = false;
            return REDRAW;
        }

        public override uint DeselectAll(GL_ControlBase control, ISet<object> selectedObjects)
        {
            Selected = false;
            return REDRAW;
        }
    }
}