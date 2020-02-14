using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using Toolbox.Library;

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
                vert.AppendLine("uniform vec4 color;");
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
                {
                    if (!System.IO.Directory.Exists("ShaderDump"))
                        System.IO.Directory.CreateDirectory("ShaderDump");

                    System.IO.File.WriteAllText($"ShaderDump/ShaderError_VS[{vertexShader}]_FS[{fragmentShader}].txt",
                        info + VertexShader + FragmentShader);
                }


                Console.WriteLine($"GL.LinkProgram had info log: {info}");
            }

            GL.DetachShader(program, vertexShader);
            GL.DetachShader(program, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            return program;
        }

        public static void LoadDefaultBlending()
        {
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Always, 0f);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.Disable(EnableCap.ColorLogicOp);
            GL.LogicOp(LogicOp.Noop);
        }

        public static void LoadTextureUniforms(BxlytShader shader, BxlytMaterial material,
             Dictionary<string, STGenericTexture> textures)
        {
            shader.SetInt("hasTexture0", 0);
            shader.SetInt("hasTexture1", 0);
            shader.SetInt("hasTexture2", 0);
            shader.SetInt("textures0", 0);
            shader.SetInt("textures1", 0);
            shader.SetInt("textures2", 0);

            BindTextureUniforms(shader, material);

            if (material.TextureMaps.Length > 0 || Runtime.LayoutEditor.Shading == Runtime.LayoutEditor.DebugShading.UVTestPattern)
                GL.Enable(EnableCap.Texture2D);

            for (int i = 0; i < 3; i++)
            {
                //Default UVs as centered
                var matTranslate = Matrix4.CreateTranslation(0 / 1 - 0.5f, 0 / 1 - 0.5f, 0);
                shader.SetMatrix(String.Format("textureTransforms[{0}]", i), ref matTranslate);
            }

            int id = 1;
            for (int i = 0; i < material.TextureMaps.Length; i++)
            {
                string TexName = material.TextureMaps[i].Name;

                if (material.animController.TexturePatterns.ContainsKey((LTPTarget)i))
                    TexName = material.animController.TexturePatterns[(LTPTarget)i];

                if (textures.ContainsKey(TexName))
                {
                    GL.ActiveTexture(TextureUnit.Texture0 + id);
                    shader.SetInt($"textures{i}", id);
                    bool isBinded = BxlytToGL.BindGLTexture(material.TextureMaps[i], textures[TexName]);
                    if (isBinded)
                        shader.SetInt($"hasTexture{i}", 1);

                    var scale = new Syroot.Maths.Vector2F(1, 1);
                    float rotate = 0;
                    var translate = new Syroot.Maths.Vector2F(0, 0);

                    int index = i;

                    if (material.TextureTransforms.Length > index)
                    {
                        var transform = material.TextureTransforms[index];
                        scale = transform.Scale;
                        rotate = transform.Rotate;
                        translate = transform.Translate;

                        foreach (var animItem in material.animController.TextureSRTS)
                        {
                            switch (animItem.Key)
                            {
                                case LTSTarget.ScaleS: scale.X = animItem.Value; break;
                                case LTSTarget.ScaleT: scale.Y = animItem.Value; break;
                                case LTSTarget.Rotate: rotate = animItem.Value; break;
                                case LTSTarget.TranslateS: translate.X = animItem.Value; break;
                                case LTSTarget.TranslateT: translate.Y = animItem.Value; break;
                            }
                        }
                    }


                    var matScale = Matrix4.CreateScale(scale.X, scale.Y, 1.0f);
                    var matRotate = Matrix4.CreateFromAxisAngle(new Vector3(0, 0, 1), MathHelper.DegreesToRadians(rotate));
                    var matTranslate = Matrix4.CreateTranslation(
                        translate.X / scale.X - 0.5f,
                        translate.Y / scale.Y - 0.5f, 0);

                    Matrix4 matTransform = matRotate * matTranslate * matScale;
                    shader.SetMatrix(String.Format("textureTransforms[{0}]", i), ref matTransform);

                    id++;
                }
            }
        }

        private static void BindTextureUniforms(BxlytShader shader, BxlytMaterial material)
        {
            //Do uv test pattern
            GL.ActiveTexture(TextureUnit.Texture10);
            GL.Uniform1(GL.GetUniformLocation(shader.program, "uvTestPattern"), 10);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.uvTestPattern.RenderableTex.TexID);

            if (material.TextureMaps.Length > 0)
            {
                var tex = material.TextureMaps[0];
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, BxlytToGL.ConvertTextureWrap(tex.WrapModeU));
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, BxlytToGL.ConvertTextureWrap(tex.WrapModeV));
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, BxlytToGL.ConvertMagFilterMode(tex.MaxFilterMode));
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, BxlytToGL.ConvertMinFilterMode(tex.MinFilterMode));
            }
            else
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureParameterName.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureParameterName.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            }
        }
    }
}
