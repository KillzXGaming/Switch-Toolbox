using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Toolbox.Library;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace LayoutBXLYT
{
    public class BloShader : BxlytShader
    {
        public BloShader() : base() {
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

        public static void SetMaterials(BxlytShader shader, GCBLO.Material material, 
            BasePane pane, Dictionary<string, STGenericTexture> textures)
        {
            var rotationMatrix = pane.GetRotationMatrix();
            shader.SetMatrix("rotationMatrix", ref rotationMatrix);

            STColor8 WhiteColor = material.WhiteColor;
            STColor8 BlackColor = material.BlackColor;

            shader.SetColor("whiteColor", WhiteColor.Color);
            shader.SetColor("blackColor", BlackColor.Color);
            shader.SetInt("debugShading", (int)Runtime.LayoutEditor.Shading);
            shader.SetInt("numTextureMaps", material.TextureMaps.Length);
            shader.SetVec2("uvScale0", new Vector2(1, 1));
            shader.SetFloat("uvRotate0", 0);
            shader.SetVec2("uvTranslate0", new Vector2(0, 0));
            shader.SetInt("flipTexture", 0);
            shader.SetInt("numTevStages", 0);
            shader.SetBool("AlphaInterpolation", material.AlphaInterpolation);
            shader.SetVec4("IndirectMat0", new Vector4(1, 1, 0, 0));
            shader.SetVec4("IndirectMat1", new Vector4(1, 1, 0, 0));
            shader.SetInt("tevTexMode", 0);
            shader.SetInt($"texCoords0GenType", 0);
            shader.SetInt($"texCoords0Source", 0);

            LoadTextureUniforms(shader, material, textures);
            LoadDefaultBlending();
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
