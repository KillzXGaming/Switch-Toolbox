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


        }
    }
}
