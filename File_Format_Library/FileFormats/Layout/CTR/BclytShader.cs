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
    public class BclytShader : BxlytShader
    {
        public BCLYT.Material material;

        public BclytShader(BCLYT.Material mat) : base()
        {
            material = mat;
            LoadShaders();
        }
        
        public override void OnCompiled()
        {
            SetColor("whiteColor", Color.FromArgb(255, 255, 255, 255));
            SetColor("blackColor", Color.FromArgb(0, 0, 0, 0));
            SetInt("debugShading", 0);
            SetInt("hasTexture0", 0);
            SetInt("numTextureMaps", 0);
            SetInt("flipTexture", 0);
            SetInt("textures0", 0);
            SetInt("textures1", 0);
            SetInt("textures2", 0);

            SetVec2("uvScale0", new Vector2(1, 1));
            SetFloat("uvRotate0", 0);
            SetVec2("uvTranslate0", new Vector2(0, 0));
        }

        public void SetMaterials(Dictionary<string, STGenericTexture> textures)
        {
            SetColor("whiteColor", material.TevConstantColors[0].Color);
            SetColor("blackColor", material.TevColor.Color);
            SetInt("debugShading", (int)Runtime.LayoutEditor.Shading);
            SetInt("numTextureMaps", material.TextureMaps.Count);
            SetVec2("uvScale0", new Vector2(1, 1));
            SetFloat("uvRotate0", 0);
            SetVec2("uvTranslate0", new Vector2(0, 0));
            SetInt("flipTexture", 0);

            BindTextureUniforms();

            string textureMap0 = "";
            if (material.TextureMaps.Count > 0)
                textureMap0 = material.GetTexture(0);

            if (textures.ContainsKey(textureMap0))
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                SetInt("textures0", 0);
                bool isBinded = BxlytToGL.BindGLTexture(material.TextureMaps[0], textures[textureMap0]);
                if (isBinded)
                    SetInt("hasTexture0", 1);
            }

            if (material.TextureTransforms.Count > 0)
            {
                var transform = material.TextureTransforms[0];
                SetVec2("uvScale0", new Vector2(transform.Scale.X, transform.Scale.Y));
                SetFloat("uvRotate0", transform.Rotate);
                SetVec2("uvTranslate0", new Vector2(transform.Translate.X, transform.Translate.Y));
            }
        }

        private void BindTextureUniforms()
        {
            //Do uv test pattern
            GL.ActiveTexture(TextureUnit.Texture10);
            GL.Uniform1(GL.GetUniformLocation(program, "uvTestPattern"), 10);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.uvTestPattern.RenderableTex.TexID);

            if (material.TextureMaps.Count > 0)
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
                string legacyPath = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Layout", "Legacy", "Bflyt.vert");

                if (LayoutEditor.UseLegacyGL)
                    return System.IO.File.ReadAllText(legacyPath);
                else
                    return System.IO.File.ReadAllText(path);
            }
        }

        public override string FragmentShader
        {
            get
            {
                string path = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Layout", "Bflyt.frag");
                string legacyPath = System.IO.Path.Combine(Runtime.ExecutableDir, "Shader", "Layout", "Legacy", "Bflyt.frag");

                if (LayoutEditor.UseLegacyGL)
                    return System.IO.File.ReadAllText(legacyPath);
                else
                    return System.IO.File.ReadAllText(path);
            }
        }
    }
}
