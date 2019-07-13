using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.Rendering;
using Switch_Toolbox.Library;
using GL_EditorFramework.GL_Core;
using GL_EditorFramework.Interfaces;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SuperBMDLib.Materials.Enums;

namespace FirstPlugin
{
    public class BMD_Renderer : GenericModelRenderer
    {
        public List<BMDTextureWrapper> TextureList = new List<BMDTextureWrapper>();

        public override void OnRender(GLControl control)
        {
         
        }

        public override void SetRenderData(STGenericMaterial mat, ShaderProgram shader, STGenericObject m)
        {
            var bmdMaterial = (BMDMaterialWrapper)mat;

            switch (bmdMaterial.Material.CullMode)
            {
                case CullMode.None:
                    GL.Disable(EnableCap.CullFace);
                    break;
                case CullMode.Back:
                    GL.CullFace(CullFaceMode.Back);
                    break;
                case CullMode.Front:
                    GL.CullFace(CullFaceMode.Front);
                    break;
                case CullMode.All:
                    GL.CullFace(CullFaceMode.FrontAndBack);
                    break;
            }
        }

        public override int BindTexture(STGenericMatTexture tex, ShaderProgram shader)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + tex.textureUnit + 1);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.RenderableTex.TexID);

            string activeTex = tex.Name;

            foreach (var texture in TextureList)
            {
                if (TextureList.IndexOf(texture) == ((BMDTextureMap)tex).TextureIndex)
                {
                    BindGLTexture(tex, shader, TextureList[((BMDTextureMap)tex).TextureIndex]);
                    return tex.textureUnit + 1;
                }
            }

            return tex.textureUnit + 1;
        }
    }
}
