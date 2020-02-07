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
using FirstPlugin;

namespace HyruleWarriors.G1M
{
    public class G1M_Renderer : GenericModelRenderer
    {
        public G1M G1MFile { get; set; }

        public List<ushort> SkinningIndices { get; set; }

        public override void OnRender(GLControl control)
        {
            
        }

        public override void SetRenderData(STGenericMaterial mat, ShaderProgram shader, STGenericObject m)
        {
            shader.SetBoolToInt("NoSkinning", Skeleton.bones.Count == 0);

            if (mat.Text == "driver_cloth")
            {
                GL.Disable(EnableCap.CullFace);
            }
            else
            {
                GL.Enable(EnableCap.CullFace);
            }
        }

        public override int BindTexture(STGenericMatTexture tex, ShaderProgram shader)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + tex.textureUnit + 1);
            GL.BindTexture(TextureTarget.Texture2D, RenderTools.defaultTex.RenderableTex.TexID);

            string activeTex = tex.Name;

            foreach (var texContainer in PluginRuntime.G1TextureContainers)
            {
                if (G1MFile.IFileInfo.ArchiveParent != texContainer.IFileInfo.ArchiveParent)
                    continue;

                var textureList = texContainer.TextureList;
                foreach (var texture in texContainer.TextureList)
                {
                    if (textureList.IndexOf(texture) == ((G1MTextureMap)tex).TextureIndex)
                    {
                        BindGLTexture(tex, shader, textureList[((G1MTextureMap)tex).TextureIndex]);
                        return tex.textureUnit + 1;
                    }
                }
            }

            return tex.textureUnit + 1;
        }
    }
}
