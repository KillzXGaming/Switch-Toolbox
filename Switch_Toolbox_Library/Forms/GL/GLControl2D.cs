using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library;

namespace Toolbox.Library.Forms
{
    public class Abstract2DDrawable
    {
        public virtual void Prepare(GLControl2D control)
        {

        }

        public virtual void Draw(GLControl2D control)
        {

        }
    }

    public class GLControl2D : GLControl
    {
        public GLControl2D() {
            InitializeComponent();
        }

        public List<Abstract2DDrawable> Drawables = new List<Abstract2DDrawable>();

        public float ZoomFactor = 1;

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
            MakeCurrent();

            SetupRendering();
            GL.ClearColor(System.Drawing.Color.FromArgb(0, 0, 40));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Disable(EnableCap.Texture2D);

            foreach (var drawable in Drawables)
                drawable.Draw(this);

            SwapBuffers();
        }

        private void SetupRendering()
        {
            GL.Viewport(0, 0, Width, Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Ortho(0, Width * 225, Height * 225, 0, -1, 1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            // Draw over everything
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
        }

        public void AddCircle(Vector2 Position, float Scale = 1) {
            Drawables.Add(new Circle2D(Position));
        }

        public class Circle2D : Abstract2DDrawable
        {
            public float Radius = 1;
            public Vector2 Position = new Vector2(0);

            public Circle2D(Vector2 position)
            {
                Position = position;
            }

            public override void Draw(GLControl2D control)
            {
                GL.PushMatrix();
                GL.Translate(Position.X, Position.Y, 0);

                GL.Color4(System.Drawing.Color.Red);
                GL.Begin(PrimitiveType.LineLoop);
                for (int i = 0; i <= 300; i++)
                {
                    double angle = 2 * Math.PI * i / 300;
                    double x = Math.Cos(angle);
                    double y = Math.Sin(angle);
                    GL.Vertex2(x, y);
                }
                GL.End();

                GL.PopMatrix();
            }
        }

        public class Rectangle2D : Abstract2DDrawable
        {
            public float Width = 600;
            public float Height = 400;
            public Vector2 Position = new Vector2(0);

            public Rectangle2D(float width, float height, Vector2 position) {
                Width = width;
                Height = height;
                Position = position;
            }

            public override void Draw(GLControl2D control)
            {
                GL.PushMatrix();
                GL.Translate(Position.X, Position.Y, 0);

                GL.Color4(System.Drawing.Color.White);
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

                GL.PopMatrix();
            }
        }

        public void AddRectangle(float Width, float Height, Vector2 Position) {
            Drawables.Add(new Rectangle2D(Width, Height, Position));
        }
    }
}
