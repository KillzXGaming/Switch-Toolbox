using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library;
using Toolbox.Library.Rendering;

namespace FirstPlugin.Forms
{
    public partial class LayoutViewer : UserControl
    {
        private RenderableTex backgroundTex;

        public LayoutViewer()
        {
            InitializeComponent();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!Runtime.OpenTKInitialized)
                return;

            glControl1.Context.MakeCurrent(glControl1.WindowInfo);
            OnRender();
        }

        private void OnRender()
        {
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, glControl1.Width, glControl1.Height, 0, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.ClearColor(System.Drawing.Color.FromArgb(40, 40, 40));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            DrawBackground();

            glControl1.SwapBuffers();
        }

        private void DrawBackground()
        {
            if (backgroundTex == null)
                backgroundTex = RenderableTex.FromBitmap(Properties.Resources.GridBackground);

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, backgroundTex.TexID);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)backgroundTex.TextureWrapR);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)backgroundTex.TextureWrapT);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)backgroundTex.TextureMagFilter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)backgroundTex.TextureMinFilter);

            float scale = 4;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.PushMatrix();
            GL.Scale(scale, scale, scale);
            GL.Translate(0, 0, 0);

            GL.Color4(Color.White);

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(-1, -1);
            GL.Vertex3(-Width, -Height, 0);
            GL.TexCoord2(0, -1);
            GL.Vertex3(Width, -Height, 0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(Width, Height, 0);
            GL.TexCoord2(-1, 0);
            GL.Vertex3(-Width, Height, 0);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.PopMatrix();
        }

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            glControl1.Invalidate();
        }
    }
}
