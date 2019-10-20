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
        public BclytShader() : base()
        {
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
            SetBool("ThresholdingAlphaInterpolation", false);

            SetVec2("uvScale0", new Vector2(1, 1));
            SetFloat("uvRotate0", 0);
            SetVec2("uvTranslate0", new Vector2(0, 0));

            var rotationMatrix = Matrix4.Identity;
            SetMatrix("rotationMatrix", ref rotationMatrix);
            SetInt($"texCoords0GenType", 0);
            SetInt($"texCoords0Source", 0);
        }

        public static void SetMaterials(BxlytShader shader, BCLYT.Material material, BasePane pane, Dictionary<string, STGenericTexture> textures)
        {
            Matrix4 rotationX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(pane.Rotate.X));
            Matrix4 rotationY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(pane.Rotate.Y));
            Matrix4 rotationZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(pane.Rotate.Z));
            var rotationMatrix = rotationX * rotationY * rotationZ;

            shader.SetMatrix("rotationMatrix", ref rotationMatrix);

            shader.SetColor("whiteColor", material.TevConstantColors[0].Color);
            shader.SetColor("blackColor", material.TevColor.Color);
            shader.SetInt("debugShading", (int)Runtime.LayoutEditor.Shading);
            shader.SetInt("numTextureMaps", material.TextureMaps.Length);
            shader.SetVec2("uvScale0", new Vector2(1, 1));
            shader.SetFloat("uvRotate0", 0);
            shader.SetVec2("uvTranslate0", new Vector2(0, 0));
            shader.SetInt("flipTexture", 0);
            shader.SetBool("ThresholdingAlphaInterpolation", material.ThresholdingAlphaInterpolation);
            shader.SetInt($"texCoords0GenType", 0);
            shader.SetInt($"texCoords0Source", 0);

            BindTextureUniforms(shader, material);

            string textureMap0 = "";
            if (material.TextureMaps.Length > 0)
                textureMap0 = material.GetTexture(0);

            if (textures.ContainsKey(textureMap0))
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                shader.SetInt("textures0", 0);
                bool isBinded = BxlytToGL.BindGLTexture(material.TextureMaps[0], textures[textureMap0]);
                if (isBinded)
                    shader.SetInt("hasTexture0", 1);
            }

            if (material.TextureTransforms.Length > 0)
            {
                var transform = material.TextureTransforms[0];
                shader.SetVec2("uvScale0", new Vector2(transform.Scale.X, transform.Scale.Y));
                shader.SetFloat("uvRotate0", transform.Rotate);
                shader.SetVec2("uvTranslate0", new Vector2(transform.Translate.X, transform.Translate.Y));
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
