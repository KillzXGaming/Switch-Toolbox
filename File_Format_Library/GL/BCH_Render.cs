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
using FirstPlugin.CtrLibrary;
using SPICA.PICA.Commands;

namespace FirstPlugin
{
    public class BCH_Renderer : GenericModelRenderer
    {
        public override void OnRender(GLControl control)
        {
        
        }

        public override void SetRenderData(STGenericMaterial mat, ShaderProgram shader, STGenericObject m)
        {
            var h3dMaterialWrapper = (H3DMaterialWrapper)mat;
            var h3dMaterial = h3dMaterialWrapper.Material;

            if (h3dMaterial.MaterialParams.AlphaTest.Enabled)
                GL.Enable(EnableCap.AlphaTest);
            else
                GL.Disable(EnableCap.AlphaTest);

            float alphaRef = h3dMaterial.MaterialParams.AlphaTest.Reference / 255f;
            GL.AlphaFunc(ConvertAlphaFunction(h3dMaterial.MaterialParams.AlphaTest.Function), alphaRef);
        }

        private static AlphaFunction ConvertAlphaFunction(PICATestFunc func)
        {
            switch (func)
            {
                case PICATestFunc.Always: return AlphaFunction.Always;
                case PICATestFunc.Equal: return AlphaFunction.Equal;
                case PICATestFunc.Gequal: return AlphaFunction.Gequal;
                case PICATestFunc.Greater: return AlphaFunction.Greater;
                case PICATestFunc.Lequal: return AlphaFunction.Lequal;
                case PICATestFunc.Less: return AlphaFunction.Less;
                case PICATestFunc.Never: return AlphaFunction.Never;
                case PICATestFunc.Notequal: return AlphaFunction.Notequal;
                default: return AlphaFunction.Always;
            }
        }

        public override int BindTexture(STGenericMatTexture tex, ShaderProgram shader)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + tex.textureUnit + 1);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.RenderableTex.TexID);

            string activeTex = tex.Name;
            foreach (var texture in PluginRuntime.bchTexContainers)
            {
                if (texture.ResourceNodes.ContainsKey(tex.Name))
                {
                    BindGLTexture(tex, shader, (STGenericTexture)texture.ResourceNodes[tex.Name]);
                    return tex.textureUnit + 1;
                }
            }

            return tex.textureUnit + 1;
        }
    }
}
