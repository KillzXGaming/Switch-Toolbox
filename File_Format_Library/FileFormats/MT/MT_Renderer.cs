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

namespace FirstPlugin
{
    public class MT_Renderer : GenericModelRenderer
    {
        public override void OnRender(GLControl control)
        {
            
        }

        public override void SetRenderData(STGenericMaterial mat, ShaderProgram shader, STGenericObject m)
        {
            shader.SetBoolToInt("NoSkinning", Skeleton.bones.Count == 0);
        }
    }
}
