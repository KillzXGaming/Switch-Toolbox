using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Toolbox.Library
{
    public class GLShaderGeneric : IDisposable
    {
        public bool Compiled = false;

        public int program;
        private int vertexShaderID;
        private int fragmentShaderID;

        private Dictionary<string, int> uniformBlocks = new Dictionary<string, int>();
        private Dictionary<string, int> attributes = new Dictionary<string, int>();
        private Dictionary<string, int> uniforms = new Dictionary<string, int>();
        private int activeAttributeCount;

        public void LoadShaders()
        {
            program = CompileShaders();
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

        private string vertexShader;
        private string fragShader;
        private string geomShader;

        public virtual string VertexShader
        {
            get { return vertexShader; }
            set { vertexShader = value; }
        }

        public virtual string FragmentShader
        {
            get { return fragShader; }
            set { fragShader = value; }
        }

        public virtual string GeometryShader
        {
            get { return geomShader; }
            set { geomShader = value; }
        }

        public void SetVec4(string name, Vector4 value)
        {
            if (uniforms.ContainsKey(name))
                GL.Uniform4(uniforms[name], value);
            else
                Console.WriteLine("Could not find vec4 " + name);
        }

        public void SetVec3(string name, Vector3 value)
        {
            if (uniforms.ContainsKey(name))
                GL.Uniform3(uniforms[name], value);
            else
                Console.WriteLine("Could not find vec3 " + name);
        }

        public void SetVec2(string name, Vector2 value)
        {
            if (uniforms.ContainsKey(name))
                GL.Uniform2(uniforms[name], value);
            else
                Console.WriteLine("Could not find vec2 " + name);
        }

        public void SetFloat(string name, float value)
        {
            if (uniforms.ContainsKey(name))
                GL.Uniform1(uniforms[name], value);
            else
                Console.WriteLine("Could not find float " + name);
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

        public void LoadLayout(string name)
        {
            if (!uniformBlocks.ContainsKey(name)) {
                //Get block indices
                var uniformID = GL.GetUniformBlockIndex(program, name);

                //Link them
                GL.UniformBlockBinding(program, uniformID, 0);

                int buffer;
                GL.GenBuffers(1, out buffer);
                uniformBlocks.Add(name, buffer);

                var dataValues = new Vector4[3]
                {
                new Vector4(1, 0, 0, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(0, 0, 1, 0)
                };

                var totalSize = (Vector3.SizeInBytes + 4) * dataValues.Length; //Add 4 for alignment

                GL.BindBuffer(BufferTarget.UniformBuffer, buffer);
                GL.BufferData(BufferTarget.UniformBuffer,
                    totalSize,
                    IntPtr.Zero,
                    BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.UniformBuffer, 0);

                // define the range of the buffer that links to a uniform binding point
                GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 0, buffer,
                    IntPtr.Zero, totalSize);

                GL.BindBuffer(BufferTarget.UniformBuffer, buffer);
                GL.BufferSubData<Vector4>(BufferTarget.UniformBuffer, IntPtr.Zero, (Vector3.SizeInBytes + 4) * dataValues.Length, dataValues); //Add 4 for alignment
                GL.BindBuffer(BufferTarget.UniformBuffer, 0);
            }
        }

        public void GetAttribLocation()
        {

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
            vertexShaderID = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderID, VertexShader);
            GL.CompileShader(vertexShaderID);

            fragmentShaderID = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderID, FragmentShader);
            GL.CompileShader(fragmentShaderID);

            var program = GL.CreateProgram();
            GL.AttachShader(program, vertexShaderID);
            GL.AttachShader(program, fragmentShaderID);
            GL.LinkProgram(program);

            var info = GL.GetProgramInfoLog(program);
            if (!string.IsNullOrWhiteSpace(info))
                Console.WriteLine($"GL.LinkProgram had info log: {info}");

            GL.DetachShader(program, vertexShaderID);
            GL.DetachShader(program, fragmentShaderID);
            GL.DeleteShader(vertexShaderID);
            GL.DeleteShader(fragmentShaderID);
            return program;
        }
    }
}
