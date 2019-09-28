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
    public class BrlytShader : BxlytShader
    {
        public BRLYT.Material material;

        public BrlytShader(BRLYT.Material mat) : base()
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
            SetBool("ThresholdingAlphaInterpolation", false);
            var rotationMatrix = Matrix4.Identity;
            SetMatrix("rotationMatrix", ref rotationMatrix);
            SetInt("numTevStages", 0);

            SetVec2("uvScale0", new Vector2(1, 1));
            SetFloat("uvRotate0", 0);
            SetVec2("uvTranslate0", new Vector2(0, 0));
            SetVec4("IndirectMat0", new Vector4(1, 1, 0, 0));
            SetVec4("IndirectMat1", new Vector4(1, 1, 0, 0));
            SetInt($"texCoords0GenType", 0);
            SetInt($"texCoords0Source", 0);
        }

        public void SetMaterials(BasePane pane, Dictionary<string, STGenericTexture> textures)
        {
            var paneRotate = pane.Rotate;
            if (pane.animController.PaneSRT?.Count > 0)
            {
                foreach (var animItem in pane.animController.PaneSRT)
                {
                    switch (animItem.Key)
                    {
                        case LPATarget.RotateX:
                            paneRotate.X = animItem.Value; break;
                        case LPATarget.RotateY:
                            paneRotate.Y = animItem.Value; break;
                        case LPATarget.RotateZ:
                            paneRotate.Z = animItem.Value; break;
                    }
                }
            }

            Matrix4 rotationX = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(paneRotate.X));
            Matrix4 rotationY = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(paneRotate.Y));
            Matrix4 rotationZ = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(paneRotate.Z));
            var rotationMatrix = rotationX * rotationY * rotationZ;

            SetMatrix("rotationMatrix", ref rotationMatrix);

            var WhiteColor = material.WhiteColor;
            var BlackColor = material.BlackColor;

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

            SetColor("whiteColor", Color.FromArgb(255, WhiteColor.R, WhiteColor.G, WhiteColor.B));
            SetColor("blackColor", BlackColor.Color);
            SetInt("debugShading", (int)Runtime.LayoutEditor.Shading);
            SetInt("numTextureMaps", material.TextureMaps.Length);
            SetVec2("uvScale0", new Vector2(1, 1));
            SetFloat("uvRotate0", 0);
            SetVec2("uvTranslate0", new Vector2(0, 0));
            SetInt("flipTexture", 0);
            SetInt("numTevStages", 0);
            SetBool("ThresholdingAlphaInterpolation", material.ThresholdingAlphaInterpolation);
            SetVec4("IndirectMat0", new Vector4(1, 1, 0, 0));
            SetVec4("IndirectMat1", new Vector4(1, 1, 0, 0));
            SetInt("tevTexMode", 0);
            SetInt($"texCoords0GenType", 0);
            SetInt($"texCoords0Source", 0);

            BindTextureUniforms();

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

            if (material.TextureTransforms.Count > 0)
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
