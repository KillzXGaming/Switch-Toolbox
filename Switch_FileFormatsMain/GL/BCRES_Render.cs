using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.Rendering;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using BcresLibrary;

namespace FirstPlugin
{
    public class BCRES_Render : AbstractGlDrawable
    {
        public Matrix4 ModelTransform = Matrix4.Identity;

        public List<CMDLWrapper> Models = new List<CMDLWrapper>();

        // gl buffer objects
        int vbo_position;
        int ibo_elements;

        private void GenerateBuffers()
        {
            GL.GenBuffers(1, out vbo_position);
            GL.GenBuffers(1, out ibo_elements);

            UpdateVertexData();
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

            SOBJWrapper.DisplayVertex[] Vertices;
            int[] Faces;

            int poffset = 0;
            int voffset = 0;
            List<SOBJWrapper.DisplayVertex> Vs = new List<SOBJWrapper.DisplayVertex>();
            List<int> Ds = new List<int>();
            foreach (CMDLWrapper m in Models)
            {
                foreach (SOBJWrapper shape in m.Shapes)
                {
                    shape.Offset = poffset * 4;
                    List<SOBJWrapper.DisplayVertex> pv = shape.CreateDisplayVertices();
                    Vs.AddRange(pv);

                    for (int i = 0; i < shape.lodMeshes[shape.DisplayLODIndex].displayFaceSize; i++)
                    {
                        Ds.Add(shape.display[i] + voffset);
                    }
                    poffset += shape.lodMeshes[shape.DisplayLODIndex].displayFaceSize;
                    voffset += pv.Count;
                }
            }

            // Binds
            Vertices = Vs.ToArray();
            Faces = Ds.ToArray();

            // Bind only once!
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<SOBJWrapper.DisplayVertex>(BufferTarget.ArrayBuffer, (IntPtr)(Vertices.Length * SOBJWrapper.DisplayVertex.Size), Vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            GL.BufferData<int>(BufferTarget.ElementArrayBuffer, (IntPtr)(Faces.Length * sizeof(int)), Faces, BufferUsageHint.StaticDraw);

            LibraryGUI.Instance.UpdateViewport();
        }

        public ShaderProgram defaultShaderProgram;

        public override void Prepare(GL_ControlModern control)
        {
            string pathFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bcres") + "\\BCRES.frag";
            string pathVert = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bcres") + "\\BCRES.vert";

            var defaultFrag = new FragmentShader(File.ReadAllText(pathFrag));
            var defaultVert = new VertexShader(File.ReadAllText(pathVert));

            defaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert);
        }

        public override void Prepare(GL_ControlLegacy control)
        {
            string pathFrag = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bcres") + "\\BCRES.frag";
            string pathVert = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Bcres") + "\\BCRES.vert";

            var defaultFrag = new FragmentShader(File.ReadAllText(pathFrag));
            var defaultVert = new VertexShader(File.ReadAllText(pathVert));

            defaultShaderProgram = new ShaderProgram(defaultFrag, defaultVert);
        }

        public override void Draw(GL_ControlLegacy control, Pass pass)
        {
            if (!Runtime.OpenTKInitialized)
                return;
        }

        public override void Draw(GL_ControlModern control, Pass pass)
        {
            if (!Runtime.OpenTKInitialized)
                return;

            bool buffersWereInitialized = ibo_elements != 0 && vbo_position != 0;
            if (!buffersWereInitialized)
                GenerateBuffers();

            ShaderProgram shader = defaultShaderProgram;
            control.CurrentShader = shader;
            control.UpdateModelMatrix(Matrix4.CreateScale(Runtime.previewScale) * ModelTransform);

            SetRenderSettings(shader);

            DrawModels(shader, control);
        }

        private void SetRenderSettings(ShaderProgram shader)
        {
            shader.SetInt("renderType", (int)Runtime.viewportShading);
        }

        private void DrawModels(ShaderProgram shader, GL_ControlModern control)
        {
            shader.EnableVertexAttributes();
            foreach (CMDLWrapper mdl in Models)
            {
                if (mdl.Checked)
                {
                    foreach (SOBJWrapper shp in mdl.Shapes)
                    {
                        DrawModel(shp, mdl, shader, mdl.IsSelected);
                    }
                }
            }
            shader.DisableVertexAttributes();
        }

        private void SetVertexAttributes(SOBJWrapper m, ShaderProgram shader)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(shader.GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 0);
            GL.VertexAttribPointer(shader.GetAttribute("vNormal"), 3, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 12);
            GL.VertexAttribPointer(shader.GetAttribute("vTangent"), 3, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 24);
            GL.VertexAttribPointer(shader.GetAttribute("vUV0"), 2, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 36);
            GL.VertexAttribPointer(shader.GetAttribute("vColor"), 4, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 48);
            GL.VertexAttribIPointer(shader.GetAttribute("vBone"), 4, VertexAttribIntegerType.Int, SOBJWrapper.DisplayVertex.Size, new IntPtr(56));
            GL.VertexAttribPointer(shader.GetAttribute("vWeight"), 4, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 72);
            GL.VertexAttribPointer(shader.GetAttribute("vUV1"), 2, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 88);
            GL.VertexAttribPointer(shader.GetAttribute("vUV2"), 2, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 104);
            GL.VertexAttribPointer(shader.GetAttribute("vPosition2"), 3, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 112);
            GL.VertexAttribPointer(shader.GetAttribute("vPosition3"), 3, VertexAttribPointerType.Float, false, SOBJWrapper.DisplayVertex.Size, 124);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
        }

        private void DrawModel(SOBJWrapper m, CMDLWrapper mdl, ShaderProgram shader, bool drawSelection)
        {
            if (m.lodMeshes[m.DisplayLODIndex].faces.Count <= 3)
                return;

            SetVertexAttributes(m, shader);

            if ((m.IsSelected))
            {
                DrawModelSelection(m, shader);
            }
            else
            {
                if (Runtime.RenderModels)
                {
                    GL.DrawElements(PrimitiveType.Triangles, m.lodMeshes[m.DisplayLODIndex].displayFaceSize, DrawElementsType.UnsignedInt, m.Offset);
                }
            }
        }

        private static void DrawModelSelection(STGenericObject p, ShaderProgram shader)
        {

        }
    }
}
