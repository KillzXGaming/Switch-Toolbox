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

namespace FirstPlugin.LuigisMansion3
{
    public class LM3_Renderer : GenericModelRenderer
    {
        public Dictionary<string, TexturePOWE> TextureList = new Dictionary<string, TexturePOWE>();

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
                if (TextureList.ContainsKey(tex.Name))
                {
                    BindGLTexture(tex, shader, TextureList[tex.Name]);
                    return tex.textureUnit + 1;
                }
            }

            return tex.textureUnit + 1;
        }
    }
}
