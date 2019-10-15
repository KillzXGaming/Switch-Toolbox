using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Toolbox.Library;

namespace Toolbox.Library.Forms
{
    public class Viewport2D : UserControl
    {
        public virtual bool UseOrtho { get; set; } = false;
        public virtual bool UseGrid { get; set; } = true;

        public Camera2D Camera = new Camera2D();

        public class Camera2D
        {
            public Matrix4 ViewMatrix = Matrix4.Identity;
            public Matrix4 ModelMatrix = Matrix4.Identity;

            public Matrix4 ModelViewMatrix
            {
                get
                {
                    return ModelMatrix * ViewMatrix;
                }
            }

            public float Zoom = 1;
            public Vector2 Position;
        }

        private GLControl glControl1;
        private Color BackgroundColor = Color.FromArgb(40, 40, 40);

        public Viewport2D()
        {
            glControl1 = new GLControl();
            glControl1.Dock = DockStyle.Fill;
            glControl1.MouseDown += glControl1_MouseDown;
            glControl1.MouseUp += glControl1_MouseUp;
            glControl1.MouseMove += glControl1_MouseMove;
            glControl1.Paint += glControl1_Paint;
            glControl1.Resize += glControl1_Resize;
            Controls.Add(glControl1);
        }


        public void UpdateViewport() {
            glControl1.Invalidate();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!Runtime.OpenTKInitialized)
                return;

            glControl1.Context.MakeCurrent(glControl1.WindowInfo);

            RenderEditor();
            SetupScene();
        }

        private void RenderEditor()
        {
            glControl1.MakeCurrent();

            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            if (UseOrtho)
            {
                float halfW = glControl1.Width / 2.0f, halfH = glControl1.Height / 2.0f;
                var orthoMatrix = Matrix4.CreateOrthographic(halfW, halfH, -10000, 10000);
                GL.LoadMatrix(ref orthoMatrix);
                GL.MatrixMode(MatrixMode.Modelview);
                Camera.ViewMatrix = orthoMatrix;
            }
            else
            {
                var cameraPosition = new Vector3(Camera.Position.X, Camera.Position.Y, -(Camera.Zoom * 500));

                var perspectiveMatrix = Matrix4.CreateRotationY(90) * Matrix4.CreateTranslation(cameraPosition) * Matrix4.CreatePerspectiveFieldOfView(1.3f, glControl1.Width / glControl1.Height, 0.01f, 100000);
                GL.LoadMatrix(ref perspectiveMatrix);
                GL.MatrixMode(MatrixMode.Modelview);
                Camera.ViewMatrix = perspectiveMatrix;
            }

            GL.ClearColor(BackgroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (UseOrtho)
            {
                GL.PushMatrix();
                GL.Scale(Camera.Zoom, Camera.Zoom, 1);
                GL.Translate(Camera.Position.X, Camera.Position.Y, 0);

                Matrix4 scaleMat = Matrix4.CreateScale(Camera.Zoom, Camera.Zoom, 1);
                Matrix4 transMat = Matrix4.CreateTranslation(Camera.Position.X,  -Camera.Position.Y, 0);

                Camera.ModelMatrix = scaleMat * transMat;
            }
        }

        private void SetupScene()
        {
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Always, 0f);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BlendEquation(BlendEquationMode.FuncAdd);

            if (UseGrid)
                Render2D.DrawGrid(BackgroundColor);

            RenderSceme();

            if (UseOrtho)
                GL.PopMatrix();

            GL.UseProgram(0);
            glControl1.SwapBuffers();
        }

        public virtual void RenderSceme()
        {

        }

        private Point originMouse;
        private bool mouseCameraDown;

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Shift && e.Button == MouseButtons.Left ||
              e.Button == MouseButtons.Middle)
            {
                originMouse = e.Location;
                mouseCameraDown = true;
                glControl1.Invalidate();
            }
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)
            {
                mouseCameraDown = false;
            }
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseCameraDown)
            {
                var pos = new Vector2(e.Location.X - originMouse.X, e.Location.Y - originMouse.Y);
                Camera.Position.X += pos.X;
                Camera.Position.Y += pos.Y;

                originMouse = e.Location;

                glControl1.Invalidate();
            }
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            glControl1.Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            if (UseOrtho)
            {
                if (e.Delta > 0 && Camera.Zoom > 0)
                    Camera.Zoom += 0.1f;
                if (e.Delta < 0 && Camera.Zoom < 100 && Camera.Zoom > 0.1)
                    Camera.Zoom -= 0.1f;
            }
            else
            {
                if (e.Delta > 0 && Camera.Zoom > 0.1)
                    Camera.Zoom -= 0.1f;
                if (e.Delta < 0 && Camera.Zoom < 100 && Camera.Zoom > 0)
                    Camera.Zoom += 0.1f;
            }

            glControl1.Invalidate();
        }
    }
}
