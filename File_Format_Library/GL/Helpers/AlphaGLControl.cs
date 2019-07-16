using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FirstPlugin
{
    public class AlphaGLControl
    {
        public bool EnableAlphaTest = false;

        public AlphaFunction AlphaFunction;
        public float AlphaTestRef = 0.5f;

        public void LoadRenderPass()
        {
            if (EnableAlphaTest)
                GL.Enable(EnableCap.AlphaTest);
            else
                GL.Disable(EnableCap.AlphaTest);

            GL.AlphaFunc(AlphaFunction, AlphaTestRef);
        }
    }
}
