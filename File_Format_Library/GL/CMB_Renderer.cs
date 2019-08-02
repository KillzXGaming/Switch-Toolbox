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

namespace FirstPlugin
{
    public class CMB_Renderer : GenericModelRenderer
    {
        public override float PreviewScale { get; set; } = 0.01f;

        public List<CTXB.TextureWrapper> TextureList = new List<CTXB.TextureWrapper>();

        public override void OnRender(GLControl control)
        {
           
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
