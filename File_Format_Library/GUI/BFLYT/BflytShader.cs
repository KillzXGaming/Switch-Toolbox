using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace LayoutBXLYT
{
    public class BflytShader : IDisposable
    {
        private int program;
        private int vertexShader;
        private int fragmentShader;

        public BflytShader()
        {
            program = CompileShaders();
        }

        public void Dispose()
        {
            GL.DeleteProgram(program);
        }

        public string VertexShader
        {
            get
            {
                StringBuilder vert = new StringBuilder();

                return vert.ToString();
            }
        }

        public string FragmentShader
        {
            get
            {
                StringBuilder frag = new StringBuilder();

                return frag.ToString();
            }
        }

        private int CompileShaders()
        {
            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, VertexShader);
            GL.CompileShader(vertexShader);

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, FragmentShader);
            GL.CompileShader(fragmentShader);

            var program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            GL.DetachShader(program, vertexShader);
            GL.DetachShader(program, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            return program;
        }
    }
}
