using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using LayoutBXLYT.Cafe;
using Toolbox.Library;

namespace LayoutBXLYT
{
    public class BflytShader : BxlytShader
    {
        public BFLYT.Material material;

        public BflytShader(BFLYT.Material mat) : base()
        {
            material = mat;
            LoadShaders();
        }
        
        public override void OnCompiled()
        {
            SetColor("whiteColor", Color.FromArgb(255,255,255,255));
            SetColor("blackColor", Color.FromArgb(0, 0, 0, 0));
            SetInt("debugShading", 0);
            SetInt("hasTexture0", 0);
            SetInt("numTextureMaps", 0);

            SetVec2("uvScale0", new Vector2(1,1));
            SetFloat("uvRotate0", 0);
            SetVec2("uvTranslate0", new Vector2(0, 0));
        }

        public void SetMaterials(Dictionary<string, STGenericTexture> textures)
        {
            SetColor("whiteColor", material.WhiteColor.Color);
            SetColor("blackColor", material.BlackColor.Color);
            SetInt("debugShading", (int)Runtime.LayoutEditor.Shading);
            SetInt("numTextureMaps", material.TextureMaps.Length);

            BindTextureUniforms();

            string textureMap0 = "";
            if (material.TextureMaps.Length > 0)
                textureMap0 = material.GetTexture(0);

            if (textures.ContainsKey(textureMap0))
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                SetInt("textures0", 0);
                bool isBinded = LayoutViewer.BindGLTexture(material.TextureMaps[0], textures[textureMap0]);
                if (isBinded)
                    SetInt("hasTexture0", 1);
            }

            if (material.TextureTransforms.Length > 0)
            {
                var transform = material.TextureTransforms[0];
                float shiftX = 0;
                float shiftY = 0;
                if (transform.Scale.X < 0)
                    shiftX = 1;
                if (transform.Scale.Y < 0)
                    shiftY = 1;

                SetVec2("uvScale0",new Vector2(transform.Scale.X, transform.Scale.Y));
                SetFloat("uvRotate0", transform.Rotate);
                SetVec2("uvTranslate0",new Vector2(shiftX + transform.Translate.X, shiftY + transform.Translate.Y));
            }
        }

        private void BindTextureUniforms()
        {
            //Do uv test pattern
            GL.ActiveTexture(TextureUnit.Texture10);
            GL.Uniform1(GL.GetUniformLocation(program, "uvTestPattern"), 10);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.uvTestPattern.RenderableTex.TexID);

            if (material.TextureMaps.Length > 0)
            {
                var tex = material.TextureMaps[0];
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ConvertTextureWrap(tex.WrapModeU));
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ConvertTextureWrap(tex.WrapModeV));
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ConvertMagFilterMode(tex.MaxFilterMode));
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ConvertMinFilterMode(tex.MinFilterMode));
            }
        }

        public override string VertexShader
        {
            get
            {
                string path = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Layout", "Bflyt.vert");
                return System.IO.File.ReadAllText(path);
            }
        }

        public override string FragmentShader
        {
            get
            {
                StringBuilder frag = new StringBuilder();
                frag.AppendLine("uniform vec4 blackColor;");
                frag.AppendLine("uniform vec4 whiteColor;");
                frag.AppendLine("uniform int hasTexture0;");
                frag.AppendLine("uniform int debugShading;");
                frag.AppendLine("uniform int numTextureMaps;");
                frag.AppendFormat("uniform sampler2D textures{0};\n", 0);
                frag.AppendLine("uniform sampler2D uvTestPattern;");

                frag.AppendLine("void main()");
                frag.AppendLine("{");

                frag.AppendLine("vec4 textureMap0 = vec4(1);");
                frag.AppendLine("if (numTextureMaps > 0)");
                frag.AppendLine("{");
                frag.AppendLine("if (hasTexture0 == 1)");
                frag.AppendLine("    textureMap0 = texture2D(textures0, gl_TexCoord[0].st);");
                frag.AppendLine("}");
                frag.AppendLine("if (debugShading == 0)");
                frag.AppendLine("{");
                frag.AppendLine("vec4 colorFrag = gl_Color * textureMap0;");
                frag.AppendLine("vec4 colorBlend = colorFrag * whiteColor;");
                frag.AppendLine("gl_FragColor = colorBlend;");

                frag.AppendLine("}");
                frag.AppendLine("else if (debugShading == 1)");
                frag.AppendLine("    gl_FragColor = vec4(textureMap0.rgb, 1);");
                frag.AppendLine("else if (debugShading == 2)");
                frag.AppendLine("    gl_FragColor = whiteColor;");
                frag.AppendLine("else if (debugShading == 3)");
                frag.AppendLine("    gl_FragColor = blackColor;");
                frag.AppendLine("else if (debugShading == 4)");
                frag.AppendLine("    gl_FragColor = texture2D(uvTestPattern, gl_TexCoord[0].st);");
                frag.AppendLine("}");

                return frag.ToString();
            }
        }

        private static int ConvertTextureWrap(TextureRef.WrapMode wrapMode)
        {
            switch (wrapMode)
            {
                case TextureRef.WrapMode.Clamp: return (int)TextureWrapMode.Clamp;
                case TextureRef.WrapMode.Mirror: return (int)TextureWrapMode.MirroredRepeat;
                case TextureRef.WrapMode.Repeat: return (int)TextureWrapMode.Repeat;
                default: return (int)TextureWrapMode.Clamp;
            }
        }

        private static int ConvertMagFilterMode(TextureRef.FilterMode filterMode)
        {
            switch (filterMode)
            {
                case TextureRef.FilterMode.Linear: return (int)TextureMagFilter.Linear;
                case TextureRef.FilterMode.Near: return (int)TextureMagFilter.Nearest;
                default: return (int)TextureRef.FilterMode.Linear;
            }
        }

        private static int ConvertMinFilterMode(TextureRef.FilterMode filterMode)
        {
            switch (filterMode)
            {
                case TextureRef.FilterMode.Linear: return (int)TextureMinFilter.Linear;
                case TextureRef.FilterMode.Near: return (int)TextureMinFilter.Nearest;
                default: return (int)TextureRef.FilterMode.Linear;
            }
        }
    }
}
