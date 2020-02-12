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
            SetBool("AlphaInterpolation", false);

            SetVec2("uvScale0", new Vector2(1, 1));
            SetFloat("uvRotate0", 0);
            SetVec2("uvTranslate0", new Vector2(0, 0));

            var rotationMatrix = Matrix4.Identity;
            SetMatrix("rotationMatrix", ref rotationMatrix);
            SetInt($"texCoords0GenType", 0);
            SetInt($"texCoords0Source", 0);
        }

        public static void SetMaterials(BxlytShader shader, CTR.Material material, BasePane pane, Dictionary<string, STGenericTexture> textures)
        {
            Matrix4 rotationX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(pane.Rotate.X));
            Matrix4 rotationY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(pane.Rotate.Y));
            Matrix4 rotationZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(pane.Rotate.Z));
            var rotationMatrix = rotationX * rotationY * rotationZ;

            shader.SetMatrix("rotationMatrix", ref rotationMatrix);

            shader.SetColor("whiteColor", material.WhiteColor.Color);
            shader.SetColor("blackColor", material.BlackColor.Color);
            shader.SetInt("debugShading", (int)Runtime.LayoutEditor.Shading);
            shader.SetInt("numTextureMaps", material.TextureMaps.Length);
            shader.SetVec2("uvScale0", new Vector2(1, 1));
            shader.SetFloat("uvRotate0", 0);
            shader.SetVec2("uvTranslate0", new Vector2(0, 0));
            shader.SetInt("flipTexture", 0);
            shader.SetBool("AlphaInterpolation", material.AlphaInterpolation);
            shader.SetInt($"texCoords0GenType", 0);
            shader.SetInt($"texCoords0Source", 0);

            BindTextureUniforms(shader, material);

            int id = 1;
            for (int i = 0; i < material.TextureMaps.Length; i++)
            {
                string TexName = material.TextureMaps[i].Name;
                if (material.animController.TexturePatterns.ContainsKey((LTPTarget)i))
                    TexName = material.animController.TexturePatterns[(LTPTarget)i];

                shader.SetInt($"hasTexture{i}", 0);
                if (textures.ContainsKey(TexName))
                {
                    GL.ActiveTexture(TextureUnit.Texture0 + id);
                    shader.SetInt($"textures{i}", id);
                    bool binded = BxlytToGL.BindGLTexture(material.TextureMaps[i], textures[TexName]);
                    shader.SetInt($"hasTexture{i}", binded ? 1 : 0);
                    id++;
                }
            }

            for (int i = 0; i < 3; i++) {
                Matrix4 matTransform = Matrix4.Identity;
                shader.SetMatrix(String.Format("textureTransforms[{0}]", i), ref matTransform);
            }

            for (int i = 0; i < material.TextureMaps.Length; i++) {
                var scale = new Syroot.Maths.Vector2F(1, 1);
                float rotate = 0;
                var translate = new Syroot.Maths.Vector2F(0, 0);

                int index = i;
               // if (material.TexCoordGens?.Length > i)
               //     index = (int)material.TexCoordGens[i].Source / 3 - 10;

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
