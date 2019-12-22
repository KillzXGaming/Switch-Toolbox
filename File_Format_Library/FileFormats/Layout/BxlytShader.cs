using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace LayoutBXLYT
{
    public class BxlytShader : IDisposable
    {
        public bool Compiled = false;

        public int program;
        private int vertexShader;
        private int fragmentShader;

        private Dictionary<string, int> attributes = new Dictionary<string, int>();
        private Dictionary<string, int> uniforms = new Dictionary<string, int>();
        private int activeAttributeCount;

        public void LoadShaders()
        {
            Compile();
        }

        public void Enable()
        {
            GL.UseProgram(program);
        }

        public void Disable()
        {
            GL.UseProgram(0);
        }

        public void Dispose()
        {
            GL.DeleteProgram(program);
        }

        public virtual string VertexShader
        {
            get
            {
                StringBuilder vert = new StringBuilder();
                vert.AppendLine("uniform mat4 rotationMatrix;");
                vert.AppendLine("void main()");
                vert.AppendLine("{");
                {
                    vert.AppendLine("gl_FrontColor = gl_Color;");
                    vert.AppendLine("gl_Position = gl_ModelViewProjectionMatrix * rotationMatrix * gl_Vertex;");
                }
                vert.AppendLine("}");
                return vert.ToString();
            }
        }
        public virtual string FragmentShader
        {
            get
            {
                StringBuilder vert = new StringBuilder();
                vert.AppendLine("uniform vec4 color");
                vert.AppendLine("void main()");
                vert.AppendLine("{");
                {
                    vert.AppendLine("gl_FragColor = gl_Color * color;");
                }
                vert.AppendLine("}");
                return vert.ToString();
            }
        }

        //For non material panes
        public void SetBasic(BasePane pane, Color color)
        {
            var rotationMatrix = pane.GetRotationMatrix();
            SetMatrix("rotationMatrix", ref rotationMatrix);
            SetColor("color", color);
        }

        public void SetVec4(string name, Vector4 value)
        {
            if (uniforms.ContainsKey(name))
                GL.Uniform4(uniforms[name], value);
        }

        public void SetVec2(string name, Vector2 value)
        {
            if (uniforms.ContainsKey(name))
                GL.Uniform2(uniforms[name], value);
        }

        public void SetFloat(string name, float value)
        {
            if (uniforms.ContainsKey(name))
                GL.Uniform1(uniforms[name], value);
        }

        public void SetInt(string name, int value)
        {
            if (uniforms.ContainsKey(name))
                GL.Uniform1(uniforms[name], value);
        }

        public void SetBool(string name, bool value)
        {
            int intValue = value == true ? 1 : 0;

            if (uniforms.ContainsKey(name))
                GL.Uniform1(uniforms[name], intValue);
        }

        public void SetColor(string name, Color color)
        {
            if (uniforms.ContainsKey(name))
                GL.Uniform4(uniforms[name], color);
        }

        public void SetMatrix(string name, ref Matrix4 value)
        {
            if (uniforms.ContainsKey(name))
                GL.UniformMatrix4(uniforms[name], false, ref value);
        }

        public int this[string name]
        {
            get { return uniforms[name]; }
        }

        private void LoadAttributes(int program)
        {
            attributes.Clear();

            GL.GetProgram(program, GetProgramParameterName.ActiveAttributes, out activeAttributeCount);
            for (int i = 0; i < activeAttributeCount; i++)
            {
                int size = 0;
                ActiveAttribType type;

                string name = GL.GetActiveAttrib(program, i, out size, out type);
                int location = GL.GetAttribLocation(program, name);

                // Overwrite existing vertex attributes.
                attributes[name] = location;
            }
        }

        public void EnableVertexAttributes()
        {
            foreach (KeyValuePair<string, int> attrib in attributes)
                GL.EnableVertexAttribArray(attrib.Value);
        }

        public void DisableVertexAttributes()
        {
            foreach (KeyValuePair<string, int> attrib in attributes)
                GL.DisableVertexAttribArray(attrib.Value);
        }

        public int GetAttribute(string name)
        {
            if (string.IsNullOrEmpty(name) || !attributes.ContainsKey(name))
                return -1;
            else
                return attributes[name];
        }

        private void LoadUniorms(int program)
        {
            uniforms.Clear();

            GL.GetProgram(program, GetProgramParameterName.ActiveUniforms, out activeAttributeCount);
            for (int i = 0; i < activeAttributeCount; i++)
            {
                int size = 0;
                ActiveUniformType type;
                string name = GL.GetActiveUniform(program, i, out size, out type);
                int location = GL.GetUniformLocation(program, name);

                // Overwrite existing vertex attributes.
                uniforms[name] = location;
            }
        }

        public void Compile()
        {
            program = CompileShaders();

            LoadAttributes(program);
            LoadUniorms(program);
            OnCompiled();

            Compiled = true;
        }

        public virtual void OnCompiled() { }

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

            var info = GL.GetProgramInfoLog(program);
            if (!string.IsNullOrWhiteSpace(info))
            {
                if (Toolbox.Library.Runtime.DumpShadersDEBUG)
                    if (!System.IO.Directory.Exists("ShaderDump"))
                        System.IO.Directory.CreateDirectory("ShaderDump");

                    System.IO.File.WriteAllText($"ShaderDump/ShaderError_VS[{vertexShader}]_FS[{fragmentShader}].txt", 
                        info + VertexShader + FragmentShader);

                Console.WriteLine($"GL.LinkProgram had info log: {info}");
            }

            GL.DetachShader(program, vertexShader);
            GL.DetachShader(program, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            return program;
        }
    }
}
