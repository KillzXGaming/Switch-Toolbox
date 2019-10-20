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
            SetInt("flipTexture", 0);
            SetInt("textures0", 0);
            SetInt("textures1", 0);
            SetInt("textures2", 0);
            SetBool("ThresholdingAlphaInterpolation", false);
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

        public void SetMaterials(BasePane pane, Dictionary<string, STGenericTexture> textures)
        {
            var rotationMatrix = pane.GetRotationMatrix();
            SetMatrix("rotationMatrix", ref rotationMatrix);

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

            SetColor("whiteColor", WhiteColor.Color);
            SetColor("blackColor", BlackColor.Color);
            SetInt("debugShading", (int)Runtime.LayoutEditor.Shading);
            SetInt("numTextureMaps", material.TextureMaps.Length);
            SetVec2("uvScale0", new Vector2(1, 1));
            SetFloat("uvRotate0", 0);
            SetVec2("uvTranslate0", new Vector2(0, 0));
            SetInt("flipTexture", 0);
            SetInt("numTevStages", material.TevStages.Length);
            SetBool("ThresholdingAlphaInterpolation", material.ThresholdingAlphaInterpolation);
            SetVec4("IndirectMat0", new Vector4(1, 1, 0, 0));
            SetVec4("IndirectMat1", new Vector4(1, 1, 0, 0));
            SetInt("tevTexMode", 0);
            SetInt($"texCoords0GenType", 0);
            SetInt($"texCoords0Source", 0);
            SetInt("hasTexture0", 0);
            SetInt("hasTexture1", 0);
            SetInt("hasTexture2", 0);
            SetInt("textures0", 0);
            SetInt("textures1", 0);
            SetInt("textures2", 0);

            BindTextureUniforms();

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
                    SetInt($"textures{i}", id);
                    bool isBinded = BxlytToGL.BindGLTexture(material.TextureMaps[i], textures[TexName]);
                    if (isBinded)
                        SetInt($"hasTexture{i}", 1);

                    id++;
                }
            }

            for (int i = 0; i < material.TexCoords?.Length; i++)
            {
                SetInt($"texCoords{i}GenType", (int)material.TexCoords[i].GenType);
                SetInt($"texCoords{i}Source", (int)material.TexCoords[i].Source);
            }

            for (int i = 0; i < material.TevStages?.Length; i++)
            {
                SetInt($"tevStage{i}RGB", (int)material.TevStages[i].ColorMode);
                SetInt($"tevStage{i}A",   (int)material.TevStages[i].AlphaMode);
            }

            if (material.TextureTransforms.Length > 0)
            {
                var transform = material.TextureTransforms[0];
                var scale = transform.Scale;
                var rotate = transform.Rotate;
                var translate = transform.Translate;

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

                SetVec2("uvScale0", new Vector2(scale.X, scale.Y));
                SetFloat("uvRotate0", rotate);
                SetVec2("uvTranslate0", new Vector2(translate.X, translate.Y));
            }


            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Always, 0f);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.Disable(EnableCap.ColorLogicOp);
            GL.LogicOp(LogicOp.Noop);

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
