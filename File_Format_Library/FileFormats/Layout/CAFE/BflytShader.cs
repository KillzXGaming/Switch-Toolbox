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
            SetVec2("uvScale0", new Vector2(1, 1));
            SetFloat("uvRotate0", 0);
            SetVec2("uvTranslate0", new Vector2(0, 0));

            BindTextureUniforms();

            string textureMap0 = "";
            if (material.TextureMaps.Length > 0)
                textureMap0 = material.GetTexture(0);

            if (textures.ContainsKey(textureMap0))
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                SetInt("textures0", 0);
                bool isBinded = BxlytToGL.BindGLTexture(material.TextureMaps[0], textures[textureMap0]);
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
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, BxlytToGL.ConvertTextureWrap(tex.WrapModeU));
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, BxlytToGL.ConvertTextureWrap(tex.WrapModeV));
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, BxlytToGL.ConvertMagFilterMode(tex.MaxFilterMode));
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, BxlytToGL.ConvertMinFilterMode(tex.MinFilterMode));
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
                string path = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Layout", "Bflyt.frag");
                return System.IO.File.ReadAllText(path);
            }
        }
    }
}
