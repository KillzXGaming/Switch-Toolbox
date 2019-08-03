using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Rendering;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library;
using Grezzo.CmbEnums;

namespace FirstPlugin
{
    //Rendering methods based on noclip
    //https://github.com/magcius/noclip.website/blob/master/src/oot3d/render.ts
    public class CMB_Renderer : GenericModelRenderer
    {
        public override float PreviewScale { get; set; } = 0.01f;

        public List<CTXB.TextureWrapper> TextureList = new List<CTXB.TextureWrapper>();

        private string FragmentShader()
        {
            string frag = "";
            return frag;
        }

        private string GenerateAlphaTestCompare(AlphaFunction compare, int reference)
        {
            float refSingle = (float)reference;
            switch (compare)
            {
                case AlphaFunction.Never: return "false";
                case AlphaFunction.Less: return $"t_CmbOut.a <  {refSingle}";
                case AlphaFunction.Lequal: return $"t_CmbOut.a <= {refSingle}";
                case AlphaFunction.Equal: return $"t_CmbOut.a == {refSingle}";
                case AlphaFunction.Notequal: return $"t_CmbOut.a != {refSingle}";
                case AlphaFunction.Greater: return $"t_CmbOut.a >  ${refSingle}";
                case AlphaFunction.Gequal: return $"t_CmbOut.a >= ${refSingle}";
                case AlphaFunction.Always: return "true";
                default: throw new Exception("Unsupported alpha function");
            }
        }

        private string GenerateTextureEnvironment()
        {
            string vale = $"";
            return vale;
        }

        public override void OnRender(GLControl control)
        {
            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Gequal, 0.1f);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public override void DrawModels(ShaderProgram shader, GL_ControlModern control)
        {
            shader.EnableVertexAttributes();

            List<STGenericObject> opaque = new List<STGenericObject>();
            List<STGenericObject> transparent = new List<STGenericObject>();

            for (int m = 0; m < Meshes.Count; m++)
            {
                if (((CMB.CMBMaterialWrapper)Meshes[m].GetMaterial()).Material.IsTransparent)
                    transparent.Add(Meshes[m]);
                else
                    opaque.Add(Meshes[m]);
            }

            for (int m = 0; m < transparent.Count; m++)
            {
                DrawModel(control, Skeleton, transparent[m].GetMaterial(), transparent[m], shader);
            }

            for (int m = 0; m < opaque.Count; m++)
            {
                DrawModel(control, Skeleton, opaque[m].GetMaterial(), opaque[m], shader);
            }

            shader.DisableVertexAttributes();
        }

        public override void SetRenderData(STGenericMaterial mat, ShaderProgram shader, STGenericObject m)
        {
            var cmbMaterial = ((CMB.CMBMaterialWrapper)mat).Material;

            shader.SetBoolToInt("isTransparent", cmbMaterial.BlendEnaled);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(cmbMaterial.BlendingFactorSrcAlpha, cmbMaterial.BlendingFactorDestAlpha);
            GL.BlendColor(1.0f, 1.0f, 1.0f, cmbMaterial.BlendColorA);
            if (cmbMaterial.AlphaTestEnable)
                GL.Enable(EnableCap.AlphaTest);
            else
                GL.Disable(EnableCap.AlphaTest);

            GL.AlphaFunc(cmbMaterial.AlphaTestFunction, cmbMaterial.AlphaTestReference);
        }

        public override int BindTexture(STGenericMatTexture tex, ShaderProgram shader)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + tex.textureUnit + 1);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.RenderableTex.TexID);

            string activeTex = tex.Name;

            foreach (var texture in TextureList)
            {
                if (TextureList.IndexOf(texture) == ((CMB.CMBTextureMapWrapper)tex).TextureIndex)
                {
                    BindGLTexture(tex, shader, TextureList[((CMB.CMBTextureMapWrapper)tex).TextureIndex]);
                    return tex.textureUnit + 1;
                }
            }

            return tex.textureUnit + 1;
        }
    }
}
