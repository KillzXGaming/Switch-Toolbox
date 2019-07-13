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

        public override void DrawModels(ShaderProgram shader, GL_ControlModern control)
        {
            shader.EnableVertexAttributes();

            List<STGenericObject> opaque = new List<STGenericObject>();
            List<STGenericObject> transparent = new List<STGenericObject>();

            for (int m = 0; m < Meshes.Count; m++)
            {
                if (((BMDMaterialWrapper)Meshes[m].GetMaterial()).isTransparent)
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
            var bmdMaterial = (BMDMaterialWrapper)mat;

            shader.SetBoolToInt("isTransparent", bmdMaterial.isTransparent);

            GXToOpenGL.SetBlendState(bmdMaterial.Material.BMode);
            GXToOpenGL.SetCullState(bmdMaterial.Material.CullMode);
            GXToOpenGL.SetDepthState(bmdMaterial.Material.ZMode, false);
            GXToOpenGL.SetDitherEnabled(bmdMaterial.Material.Dither);
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
