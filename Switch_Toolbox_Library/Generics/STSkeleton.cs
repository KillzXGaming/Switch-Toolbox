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
using Switch_Toolbox.Library.Rendering;

using static GL_EditorFramework.EditorDrawables.EditorSceneBase;

namespace Switch_Toolbox.Library
{
    public class STSkeleton : EditableObject
    {
        public Vector3 position = new Vector3(0, 0, 0);

        protected bool Selected = false;
        protected bool Hovered = false;

        public override bool IsSelected() => Selected;
        public override bool IsSelected(int partIndex) => Selected;

        public bool IsHovered() => Selected;

        public ShaderProgram solidColorShaderProgram;
        public override void Prepare(GL_ControlModern control)
        {
            var solidColorFrag = new FragmentShader(
                      @"#version 330
				uniform vec4 boneColor;

                out vec4 FragColor;

				void main(){
	                FragColor = boneColor;
				}");
            var solidColorVert = new VertexShader(
              @"#version 330

				in vec4 point;

                uniform mat4 mtxCam;
                uniform mat4 mtxMdl;

                uniform mat4 bone;
                uniform mat4 parent;
                uniform mat4 rotation;
                uniform int hasParent;
                uniform float scale;

				void main(){
                    vec4 position = bone * rotation * vec4(point.xyz * scale, 1);
                    if (hasParent == 1)
                    {
                        if (point.w == 0)
                            position = parent * rotation * vec4(point.xyz * scale, 1);
                        else
                            position = bone * rotation * vec4((point.xyz - vec3(0, 1, 0)) * scale, 1);
                    }
					gl_Position =  mtxCam  * mtxMdl * vec4(position.xyz, 1);

				}");

            solidColorShaderProgram = new ShaderProgram(solidColorFrag, solidColorVert);
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

            foreach (STBone bn in bones)
            {
                bn.Render();
            }
        }

        public override void Draw(GL_ControlLegacy control, Pass pass)
        {
            if (!Runtime.OpenTKInitialized || pass == Pass.TRANSPARENT)
                return;

            foreach (STBone bn in bones)
            {
                bn.Render();
            }
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
            var boundings = GetSelectionBox();

            DrawableBoundingBox.DrawBoundingBox(
                new Vector3(boundings.minX, boundings.minY, boundings.minZ),
                new Vector3(boundings.maxX, boundings.maxY, boundings.maxZ)
                );

            return;
        }

        public override void Draw(GL_ControlModern control, Pass pass, EditorSceneBase editorScene)
        {
            CheckBuffers();

            if (!Runtime.OpenTKInitialized || !Runtime.renderBones)
                return;

            GL.UseProgram(0);
            GL.Disable(EnableCap.CullFace);

            if (Runtime.boneXrayDisplay)
                GL.Disable(EnableCap.DepthTest);

            if (Runtime.renderBoundingBoxes)
                DrawBoundingBoxes();

            control.CurrentShader = solidColorShaderProgram;

            control.UpdateModelMatrix(
            Matrix4.CreateScale(Runtime.previewScale) *
            Matrix4.CreateTranslation(Selected ? editorScene.CurrentAction.NewPos(position) : position));


            solidColorShaderProgram.EnableVertexAttributes();
            solidColorShaderProgram.SetMatrix4x4("rotation", ref prismRotation);

            foreach (STBone bn in bones)
            {
                solidColorShaderProgram.SetVector4("boneColor", ColorUtility.ToVector4(boneColor));
                solidColorShaderProgram.SetFloat("scale", Runtime.bonePointSize);

                Matrix4 transform = bn.Transform;

                solidColorShaderProgram.SetMatrix4x4("bone", ref transform);
                solidColorShaderProgram.SetInt("hasParent", bn.parentIndex != -1 ? 1 : 0);

                if (bn.parentIndex != -1)
                {
                    var transformParent = ((STBone)bn.Parent).Transform;
                    solidColorShaderProgram.SetMatrix4x4("parent", ref transformParent);
                }

                Draw(solidColorShaderProgram);

                if (Runtime.SelectedBoneIndex == bn.GetIndex())
                    solidColorShaderProgram.SetVector4("boneColor", ColorUtility.ToVector4(selectedBoneColor));

                solidColorShaderProgram.SetInt("hasParent", 0);
                Draw(solidColorShaderProgram);
            }

            solidColorShaderProgram.DisableVertexAttributes();

            GL.UseProgram(0);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
        }

        private void Attributes(ShaderProgram shader)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(shader.GetAttribute("point"), 4, VertexAttribPointerType.Float, false, 16, 0);
        }
        private void Draw(ShaderProgram shader)
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

                if (bones[i].RotationType == STBone.BoneRotationType.Quaternion)
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

        public override BoundingBox GetSelectionBox()
        {
            BoundingBox box = BoundingBox.Default;

            for (int i = 1; i < bones.Count; i++)
            {
                box.Include(bones[i - 1].pos);
                box.Include(bones[i].pos);
            }

            return box;
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

        public override uint SelectAll(GL_ControlBase control)
        {
            Selected = true;
            return REDRAW;
        }

        public override uint SelectDefault(GL_ControlBase control)
        {
            Selected = true;
            return REDRAW;
        }

        public override uint Select(int partIndex, GL_ControlBase control)
        {
            Selected = true;
            return REDRAW;
        }

        public override uint Deselect(int partIndex, GL_ControlBase control)
        {
            Selected = false;
            return REDRAW;
        }

        public override uint DeselectAll(GL_ControlBase control)
        {
            Selected = false;
            return REDRAW;
        }

        public override Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
    }
}