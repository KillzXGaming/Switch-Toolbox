using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library;
using Toolbox.Library.Rendering;

namespace FirstPlugin
{
    public class KCLRendering : AbstractGlDrawable
    {
        public bool DrawGlobalOctrees = false;

        public Vector3 Max = new Vector3(0);
        public Vector3 Min = new Vector3(0);

        public List<ushort> SelectedTypes = new List<ushort>();

        public Vector3 position = new Vector3(0, 0, 0);

        public bool UseOverlay = false;

        protected bool Selected = false;
        protected bool Hovered = false;

        // public override bool IsSelected() => Selected;
        //  public override bool IsSelected(int partIndex) => Selected;

        public bool IsHovered() => Selected;

        // gl buffer objects
        int vbo_position;
        int ibo_elements;

        //Set the game's material list
        public KCL.GameSet GameMaterialSet = KCL.GameSet.MarioKart8D;
        public List<KCL.KCLModel> models = new List<KCL.KCLModel>();

        private void GenerateBuffers()
        {
            GL.GenBuffers(1, out vbo_position);
            GL.GenBuffers(1, out ibo_elements);
        }

        public void Destroy()
        {
            GL.DeleteBuffer(vbo_position);
            GL.DeleteBuffer(ibo_elements);
        }

        public void UpdateVertexData()
        {
            if (!Runtime.OpenTKInitialized)
                return;

            KCL.DisplayVertex[] Vertices;
            int[] Faces;

            int poffset = 0;
            int voffset = 0;
            List<KCL.DisplayVertex> Vs = new List<KCL.DisplayVertex>();
            List<int> Ds = new List<int>();
            foreach (KCL.KCLModel m in models)
            {
                m.Offset = poffset * 4;
                List<KCL.DisplayVertex> pv = m.CreateDisplayVertices();
                Vs.AddRange(pv);

                for (int i = 0; i < m.displayFaceSize; i++)
                {
                    Ds.Add(m.display[i] + voffset);
                }
                poffset += m.displayFaceSize;
                voffset += pv.Count;
            }

            // Binds
            Vertices = Vs.ToArray();
            Faces = Ds.ToArray();

            // Bind only once!
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<KCL.DisplayVertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * KCL.DisplayVertex.Size), Vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            GL.BufferData<int>(BufferTarget.ElementArrayBuffer, (IntPtr)(Faces.Length * sizeof(int)), Faces, BufferUsageHint.StaticDraw);

            LibraryGUI.UpdateViewport();
        }

        public GLShaderGeneric Shader;

        public ShaderProgram defaultShaderProgram;
        public ShaderProgram solidColorShaderProgram;

        public override void Prepare(GL_ControlModern control)
        {
            string pathFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader") + "\\KCL.frag";
            string pathVert = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader") + "\\KCL.vert";

            var defaultFrag = new FragmentShader(File.ReadAllText(pathFrag));
            var defaultVert = new VertexShader(File.ReadAllText(pathVert));

            var solidColorFrag = new FragmentShader(
           @"#version 330
				uniform vec4 color;
				out vec4 FragColor;

				void main(){
					FragColor = color;
				}");

            var solidColorVert = new VertexShader(
          @"#version 330
                in vec3 vPosition;
                in vec3 vNormal;
                in vec3 vColor;

                out vec3 normal;
                out vec3 color;
                out vec3 position;

	            uniform mat4 mtxMdl;
				uniform mat4 mtxCam;

				void main(){
                    normal = vNormal;
                    color = vColor;
	                position = vPosition;

                    gl_Position = mtxMdl * mtxCam  * vec4(vPosition.xyz, 1.0);
				}");

            Shader = new GLShaderGeneric()
            {
                FragmentShader = File.ReadAllText(pathFrag),
                VertexShader = File.ReadAllText(pathVert),
            };

            defaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert, control);
            solidColorShaderProgram = new ShaderProgram(solidColorFrag, solidColorVert, control);

        }

        public override void Prepare(GL_ControlLegacy control)
        {

        }

        private void CheckBuffers()
        {
            if (!Runtime.OpenTKInitialized)
                return;

            bool buffersWereInitialized = ibo_elements != 0 && vbo_position != 0;
            if (!buffersWereInitialized)
            {
                GenerateBuffers();
                UpdateVertexData();
            }
        }
        public override void Draw(GL_ControlLegacy control, Pass pass)
        {
            if (!Runtime.OpenTKInitialized || defaultShaderProgram == null || !Visible)
                return;

            Matrix4 mvpMat = control.ModelMatrix * control.CameraMatrix * control.ProjectionMatrix;

            Matrix4 invertedCamera = Matrix4.Identity;
            if (invertedCamera.Determinant != 0)
                invertedCamera = mvpMat.Inverted();

            Vector3 lightDirection = new Vector3(0f, 0f, -1f);
            Vector3 difLightDirection = Vector3.TransformNormal(lightDirection, invertedCamera).Normalized();

            GL.Disable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);

            foreach (var model in models)
            {
                if (Runtime.RenderModels && model.Checked && model.Checked)
                {
                    List<int> faces = model.getDisplayFace();

                    GL.Begin(PrimitiveType.Triangles);
                    foreach (var index in faces)
                    {
                        Vertex vert = model.vertices[index];
                        float normal = Vector3.Dot(difLightDirection, vert.nrm) * 0.5f + 0.5f;
                        GL.Color3(new Vector3(normal));
                        GL.Vertex3(vert.pos);
                    }
                    GL.End();
                }
            }

            GL.Enable(EnableCap.Texture2D);
        }

        public override void Draw(GL_ControlModern control, Pass pass)
        {
            if (!Runtime.OpenTKInitialized || pass == Pass.TRANSPARENT || defaultShaderProgram == null || !Visible)
                return;

            if (UseOverlay && pass == Pass.PICKING)
                return;

            CheckBuffers();

            Matrix4 camMat = control.ModelMatrix * control.CameraMatrix * control.ProjectionMatrix;
            control.CurrentShader = defaultShaderProgram;

            if (UseOverlay)
            {
                control.UpdateModelMatrix(
                  Matrix4.CreateScale(1.0002f));
            }
            else
            {
                control.UpdateModelMatrix(
                  Matrix4.CreateScale(Runtime.previewScale));
            }
          

            SetRenderSettings(defaultShaderProgram);


            GL.Disable(EnableCap.CullFace);

            GL.Uniform3(defaultShaderProgram["difLightDirection"], Vector3.TransformNormal(new Vector3(0f, 0f, -1f), camMat.Inverted()).Normalized());
            GL.Uniform3(defaultShaderProgram["difLightColor"], new Vector3(1));
            GL.Uniform3(defaultShaderProgram["ambLightColor"], new Vector3(1));

            defaultShaderProgram.EnableVertexAttributes();

            foreach (KCL.KCLModel mdl in models)
            {
                DrawModel(mdl, defaultShaderProgram);
            }

            defaultShaderProgram.DisableVertexAttributes();

            GL.UseProgram(0);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }

        private void SetRenderSettings(ShaderProgram shader)
        {
            shader.SetBoolToInt("renderVertColor", Runtime.renderVertColor);
            GL.Uniform1(defaultShaderProgram["renderType"], (int)Runtime.viewportShading);

        }

        private void DrawModel(KCL.KCLModel m, ShaderProgram shader, bool drawSelection = false)
        {
            if (m.faces.Count <= 3)
                return;

            SetVertexAttributes(m, shader);

            if (m.Checked)
            {
                if (UseOverlay)
                {
                    DrawOverlayWireframe(m, shader);
                }
                else if ((m.IsSelected))
                {
                    DrawModelSelection(m, shader);
                }
                else
                {
                    if (Runtime.RenderModelWireframe)
                    {
                        DrawModelWireframe(m, shader);
                    }

                    if (Runtime.RenderModels)
                    {
                        GL.DrawElements(PrimitiveType.Triangles, m.displayFaceSize, DrawElementsType.UnsignedInt, m.Offset);
                    }
                }
            }
        }

        private static void DrawModelSelection(KCL.KCLModel p, ShaderProgram shader)
        {
            //This part needs to be reworked for proper outline. Currently would make model disappear

            GL.DrawElements(PrimitiveType.Triangles, p.displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);

            GL.Enable(EnableCap.StencilTest);
            // use vertex color for wireframe color
            GL.Uniform1(shader["colorOverride"], 1);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(1.5f);
            GL.DrawElements(PrimitiveType.Triangles, p.displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Uniform1(shader["colorOverride"], 0);

            GL.Enable(EnableCap.DepthTest);
        }
        private void SetVertexAttributes(KCL.KCLModel m, ShaderProgram shader)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(shader.GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, KCL.DisplayVertex.Size, 0);
            GL.VertexAttribPointer(shader.GetAttribute("vNormal"), 3, VertexAttribPointerType.Float, false, KCL.DisplayVertex.Size, 12);
            GL.VertexAttribPointer(shader.GetAttribute("vColor"), 3, VertexAttribPointerType.Float, false, KCL.DisplayVertex.Size, 24);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
        }

        private static void DrawOverlayWireframe(KCL.KCLModel p, ShaderProgram shader)
        {
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(0, -1);

            // use vertex color for wireframe color
            GL.Uniform1(shader["colorOverride"], 1);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(1.2f);
            GL.DrawElements(PrimitiveType.Triangles, p.displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Uniform1(shader["colorOverride"], 0);

            GL.Disable(EnableCap.PolygonOffsetFill);
        }

        private static void DrawModelWireframe(KCL.KCLModel p, ShaderProgram shader)
        {
            // use vertex color for wireframe color
            GL.Uniform1(shader["colorOverride"], 1);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);
            GL.Enable(EnableCap.LineSmooth);
            GL.LineWidth(1.5f);
            GL.DrawElements(PrimitiveType.Triangles, p.displayFaceSize, DrawElementsType.UnsignedInt, p.Offset);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Uniform1(shader["colorOverride"], 0);
        }

        /*     public override BoundingBox GetSelectionBox()
             {
                 Vector3 Min = new Vector3(0);
                 Vector3 Max = new Vector3(0);

                 foreach (var model in models)
                 {
                     foreach (var vertex in model.vertices)
                     {
                         Min.X = Math.Min(Min.X, vertex.pos.X);
                         Min.Y = Math.Min(Min.Y, vertex.pos.Y);
                         Min.Z = Math.Min(Min.Z, vertex.pos.Z);
                         Max.X = Math.Max(Max.X, vertex.pos.X);
                         Max.Y = Math.Max(Max.Y, vertex.pos.Y);
                         Max.Z = Math.Max(Max.Z, vertex.pos.Z);
                     }
                 }

                 return new BoundingBox()
                 {
                     minX = Min.X,
                     minY = Min.Y,
                     minZ = Min.Z,
                     maxX = Max.X,
                     maxY = Max.Y,
                     maxZ = Max.Z,
                 };
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
                 range = 20000; //Make the range large for now. Todo go back to this

                 BoundingBox box = GetSelectionBox();

                 if (pos.X < box.maxX + range && pos.X > box.minX - range &&
                   pos.Y < box.maxY + range && pos.Y > box.minY - range &&
                   pos.Z < box.maxZ + range && pos.Z > box.minZ - range)
                     return true;

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
             }*/
    }

}
