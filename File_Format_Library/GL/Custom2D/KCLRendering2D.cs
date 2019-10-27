using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using Toolbox.Library;
using OpenTK.Graphics.OpenGL;

namespace FirstPlugin
{
    public class KCLRendering2D
    {
        // gl buffer objects
        int vbo_position;
        int ibo_elements;

        public List<KCL.KCLModel> models = new List<KCL.KCLModel>();

        private GLShaderGeneric Shader;

        public KCLRendering2D(KCL kcl)
        {
            string vertPath = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Drawing2D", "KCL.vert");
            string fragPath = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Drawing2D", "KCL.frag");

            Shader = new GLShaderGeneric();
            Shader.VertexShader = System.IO.File.ReadAllText(vertPath);
            Shader.FragmentShader = System.IO.File.ReadAllText(fragPath);
            Shader.Compile();

            models = kcl.GetKclModels();
        }

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

        public void Draw(Matrix4 modelViewMatrix)
        {
            CheckBuffers();

            if (!Runtime.OpenTKInitialized)
                return;

            GL.Disable(EnableCap.CullFace);

            Shader.Enable();
            Shader.SetMatrix("modelViewMatrix", ref modelViewMatrix);
            Shader.EnableVertexAttributes();
            foreach (KCL.KCLModel mdl in models)
                DrawModel(mdl);

            Shader.DisableVertexAttributes();

            GL.UseProgram(0);
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }

        private void DrawModel(KCL.KCLModel m, bool drawSelection = false)
        {
            if (m.faces.Count <= 3)
                return;

            SetVertexAttributes(m);

            if (Runtime.RenderModels)
            {
                GL.DrawElements(PrimitiveType.Triangles, m.displayFaceSize, DrawElementsType.UnsignedInt, m.Offset);
            }
        }

        private void SetVertexAttributes(KCL.KCLModel m)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(Shader.GetAttribute("vPosition"), 3, VertexAttribPointerType.Float, false, KCL.DisplayVertex.Size, 0);
            GL.VertexAttribPointer(Shader.GetAttribute("vNormal"), 3, VertexAttribPointerType.Float, false, KCL.DisplayVertex.Size, 12);
            GL.VertexAttribPointer(Shader.GetAttribute("vColor"), 3, VertexAttribPointerType.Float, false, KCL.DisplayVertex.Size, 24);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
        }
    }
}
