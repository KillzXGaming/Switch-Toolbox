using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FirstPlugin
{
    public class DepthGLControl
    {
        public DepthFunction DepthFunction = DepthFunction.Lequal;

        public bool EnableTest{ get; set; }
        public bool EnableWrite { get; set; }

        public void LoadRenderPass()
        {
            if (EnableTest)
                GL.Enable(EnableCap.DepthTest);
            else
                GL.Disable(EnableCap.DepthTest);

            GL.DepthFunc(DepthFunction);
            GL.DepthMask(EnableWrite);
        }
    }
}
