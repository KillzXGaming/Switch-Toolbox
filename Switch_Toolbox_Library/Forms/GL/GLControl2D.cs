using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Switch_Toolbox.Library;

namespace Switch_Toolbox.Library.Forms
{
    public class GLControl2D : GLControl
    {
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // GLControl2D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "GLControl2D";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.GLControl2D_Paint);
            this.ResumeLayout(false);

        }

        private void GLControl2D_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            if (Runtime.OpenTKInitialized == false)
                return;

            MakeCurrent();

            GL.ClearColor(System.Drawing.Color.FromArgb(40, 40, 40));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Disable(EnableCap.Texture2D);

            SwapBuffers();
        }
    }
}
