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
    public class BflytShader : BxlytShader
    {
        public BflytShader() : base() {
            LoadShaders();
        }
        
        public override void OnCompiled()
        {
            SetColor("whiteColor", Color.FromArgb(255,255,255,255));
            SetColor("blackColor", Color.FromArgb(0, 0, 0, 0));
            SetInt("debugShading", 0);
            SetInt("hasTexture0", 0);
            SetInt("numTextureMaps", 0);
            SetInt("flipTexture", 0);
            SetInt("textures0", 0);
            SetInt("textures1", 0);
            SetInt("textures2", 0);
            SetBool("AlphaInterpolation", false);
            var rotationMatrix = Matrix4.Identity;
            SetMatrix("rotationMatrix", ref rotationMatrix);
            SetInt("numTevStages", 0);

            SetVec2("uvScale0", new Vector2(1,1));
            SetFloat("uvRotate0", 0);
            SetVec2("uvTranslate0", new Vector2(0, 0));
            SetVec4("IndirectMat0", new Vector4(1, 1, 0, 0));
            SetVec4("IndirectMat1", new Vector4(1, 1, 0, 0));
            SetInt($"texCoords0GenType", 0);
            SetInt($"texCoords0Source", 0);
        }

        public static void SetMaterials(BxlytShader shader, Cafe.Material material, BasePane pane, Dictionary<string, STGenericTexture> textures)
        {
            var rotationMatrix = pane.GetRotationMatrix();
            shader.SetMatrix("rotationMatrix", ref rotationMatrix);

            STColor8 WhiteColor = material.WhiteColor;
            STColor8 BlackColor = material.BlackColor;

            foreach (var animItem in material.animController.MaterialColors)
            {
                switch (animItem.Key)
                {
                    case LMCTarget.WhiteColorRed:
                        WhiteColor.R = (byte)animItem.Value; break;
                    case LMCTarget.WhiteColorGreen:
                        WhiteColor.G = (byte)animItem.Value; break;
                    case LMCTarget.WhiteColorBlue:
                        WhiteColor.B = (byte)animItem.Value; break;
                    case LMCTarget.WhiteColorAlpha:
                        WhiteColor.A = (byte)animItem.Value; break;
                    case LMCTarget.BlackColorRed:
                        BlackColor.R = (byte)animItem.Value; break;
                    case LMCTarget.BlackColorGreen:
                        BlackColor.G = (byte)animItem.Value; break;
                    case LMCTarget.BlackColorBlue:
                        BlackColor.B = (byte)animItem.Value; break;
                    case LMCTarget.BlackColorAlpha:
                        BlackColor.A = (byte)animItem.Value; break;
                }
            }

            shader.SetColor("whiteColor", WhiteColor.Color);
            shader.SetColor("blackColor", BlackColor.Color);
            shader.SetInt("debugShading", (int)Runtime.LayoutEditor.Shading);
            shader.SetInt("numTextureMaps", material.TextureMaps.Length);
            shader.SetVec2("uvScale0", new Vector2(1, 1));
            shader.SetFloat("uvRotate0", 0);
            shader.SetVec2("uvTranslate0", new Vector2(0, 0));
            shader.SetInt("flipTexture", 0);
            shader.SetInt("numTevStages", material.TevStages.Length);
            shader.SetBool("AlphaInterpolation", material.AlphaInterpolation);
            shader.SetVec4("IndirectMat0", new Vector4(1, 1, 0, 0));
            shader.SetVec4("IndirectMat1", new Vector4(1, 1, 0, 0));
            shader.SetInt("tevTexMode", 0);
            shader.SetInt($"texCoords0GenType", 0);
            shader.SetInt($"texCoords0Source", 0);
            shader.SetInt("hasTexture0", 0);
            shader.SetInt("hasTexture1", 0);
            shader.SetInt("hasTexture2", 0);
            shader.SetInt("textures0", 0);
            shader.SetInt("textures1", 0);
            shader.SetInt("textures2", 0);

            BindTextureUniforms(shader, material);

            if (material.TextureMaps.Length > 0 || Runtime.LayoutEditor.Shading == Runtime.LayoutEditor.DebugShading.UVTestPattern)
                GL.Enable(EnableCap.Texture2D);

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

                    id++;
                }
            }

            for (int i = 0; i < material.TexCoordGens?.Length; i++)
            {
                shader.SetInt($"texCoords{i}GenType", (int)material.TexCoordGens[i].Matrix);
                shader.SetInt($"texCoords{i}Source", (int)material.TexCoordGens[i].Source);
            }

            for (int i = 0; i < material.TevStages?.Length; i++)
            {
                shader.SetInt($"tevStage{i}RGB", (int)material.TevStages[i].ColorMode);
                shader.SetInt($"tevStage{i}A",   (int)material.TevStages[i].AlphaMode);
            }

            LoadTextureUniforms(shader, material, textures);
            LoadDefaultBlending();

            if (material.BlendMode != null && material.EnableBlend)
            {
                var srcFactor = BxlytToGL.ConvertBlendFactor(material.BlendMode.SourceFactor);
                var destFactor = BxlytToGL.ConvertBlendFactor(material.BlendMode.DestFactor);
                var blendOp = BxlytToGL.ConvertBlendOperation(material.BlendMode.BlendOp);
                var logicOp = BxlytToGL.ConvertLogicOperation(material.BlendMode.LogicOp);
                if (logicOp != LogicOp.Noop)
                    GL.Enable(EnableCap.ColorLogicOp);

                GL.BlendFunc(srcFactor, destFactor);
                GL.BlendEquation(blendOp);
                GL.LogicOp(logicOp);
            }
            if (material.BlendModeLogic != null && material.EnableBlendLogic)
            {
                var srcFactor = BxlytToGL.ConvertBlendFactor(material.BlendModeLogic.SourceFactor);
                var destFactor = BxlytToGL.ConvertBlendFactor(material.BlendModeLogic.DestFactor);
                var blendOp = BxlytToGL.ConvertBlendOperation(material.BlendModeLogic.BlendOp);
                var logicOp = BxlytToGL.ConvertLogicOperation(material.BlendModeLogic.LogicOp);
                if (logicOp != LogicOp.Noop)
                    GL.Enable(EnableCap.ColorLogicOp);

                GL.BlendFunc(srcFactor, destFactor);
                GL.BlendEquation(blendOp);
                GL.LogicOp(logicOp);
            }

            if (material.AlphaCompare != null && material.EnableAlphaCompare)
            {
                var alphaFunc = BxlytToGL.ConvertAlphaFunc(material.AlphaCompare.CompareMode);
                GL.AlphaFunc(alphaFunc, material.AlphaCompare.Value);
            }
        }

        private static void BindTextureUniforms(BxlytShader shader,  BxlytMaterial material)
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
