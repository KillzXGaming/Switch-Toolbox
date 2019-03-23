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
using GL_EditorFramework.Interfaces;
using GL_EditorFramework.StandardCameras;

namespace GL_EditorFramework.GL_Core
{
    public class GL_ControlModern : GL_ControlBase
    {
        public GL_ControlModern(int redrawerInterval) : base(3, redrawerInterval)
        {

        }

        public GL_ControlModern() : base(3, 16)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode) return;
            MakeCurrent();
            Framework.Initialize();
        }

        private ShaderProgram shader;
        public ShaderProgram CurrentShader
        {
            get => shader;
            set
            {
                if (value == null || value == shader || DesignMode) return;
                shader = value;

                shader.Setup(mtxMdl, mtxCam, mtxProj);
            }
        }

        public override AbstractGlDrawable MainDrawable
        {
            get => mainDrawable;
            set
            {
                if (value == null || DesignMode) return;
                mainDrawable = value;
                MakeCurrent();
                mainDrawable.Prepare(this);
                Refresh();
            }
        }

        public void ReloadShaders()
        {
            if (mainDrawable != null)
            {
                mainDrawable.Prepare(this);
                Refresh();
            }
        }

        public override void UpdateModelMatrix(Matrix4 matrix)
        {
            if (DesignMode) return;
			mtxMdl = matrix;
			if (shader!=null)
				shader.UpdateModelMatrix(matrix);
        }

        public override void ApplyModelTransform(Matrix4 matrix)
        {
            if (DesignMode) return;
            shader.UpdateModelMatrix(mtxMdl *= matrix);
        }

        public override void ResetModelMatrix()
        {
            if (DesignMode) return;
            shader.UpdateModelMatrix(mtxMdl = Matrix4.Identity);
        }

        protected override void OnResize(EventArgs e)
        {
            if (DesignMode)
            {
                base.OnResize(e);
                return;
            }
            MakeCurrent();

            float aspect_ratio;
            if (stereoscopy)
                aspect_ratio = Width / 2 / (float)Height;
            else
                aspect_ratio = Width / (float)Height;

            mtxProj = Matrix4.CreatePerspectiveFieldOfView(fov, aspect_ratio, znear, zfar);

            orientationCubeMtx = Matrix4.CreateOrthographic(Width, Height, 0.125f, 2f) * Matrix4.CreateTranslation(0, 0, 1);

            //using the calculation from whitehole
            factorX = (2f * (float)Math.Tan(fov * 0.5f) * aspect_ratio) / Width;

            factorY = (2f * (float)Math.Tan(fov * 0.5f)) / Height;

            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            if (mainDrawable == null || DesignMode)
            {
                base.OnPaint(e);
                e.Graphics.Clear(Color.Black);
                e.Graphics.DrawString("Modern Gl" + (stereoscopy ? " stereoscopy" : ""), SystemFonts.DefaultFont, SystemBrushes.ControlLight, 10f, 10f);
                return;
            }
            MakeCurrent();

            base.OnPaint(e);

			GL.ClearColor(BackgroundColor1);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();

			GL.MatrixMode(MatrixMode.Modelview);

			if (stereoscopy)
            {
                #region left eye
                GL.Viewport(0, 0, Width / 2, Height);

                mtxMdl = Matrix4.Identity;
                mtxCam =
                    Matrix4.CreateTranslation(camTarget) *
                    Matrix4.CreateRotationY(camRotX) *
                    Matrix4.CreateRotationX(camRotY) *
                    Matrix4.CreateTranslation(0.25f, 0, camDistance) *
                    Matrix4.CreateRotationY(0.02f);

                mainDrawable.Draw(this, Pass.OPAQUE);

				mainDrawable.Draw(this, Pass.TRANSPARENT);

				shader = null;
				GL.UseProgram(0);

				if (GradientBackground)
					DrawGradientBG();

				if (showOrientationCube)
				{
					orientationCubeMtx =
					Matrix4.CreateRotationY(camRotX) *
					Matrix4.CreateRotationX(camRotY) *
					Matrix4.CreateScale(80f / Width, 40f / Height, 0.25f) *
					Matrix4.CreateTranslation(1 - 160f / Width, 1 - 80f / Height, 0) *
					Matrix4.CreateRotationY(0.03125f);
					GL.LoadMatrix(ref orientationCubeMtx);
					
					DrawOrientationCube();
				}

				if (showFakeCursor)
					DrawFakeCursor();
				#endregion

				#region right eye
				GL.Viewport(Width / 2, 0, Width / 2, Height);

                mtxMdl = Matrix4.Identity;
                mtxCam =
                    Matrix4.CreateTranslation(camTarget) *
                    Matrix4.CreateRotationY(camRotX) *
                    Matrix4.CreateRotationX(camRotY) *
                    Matrix4.CreateTranslation(-0.25f, 0, camDistance) *
                    Matrix4.CreateRotationY(-0.02f);

				mainDrawable.Draw(this, Pass.OPAQUE);

				mainDrawable.Draw(this, Pass.TRANSPARENT);

				shader = null;
				GL.UseProgram(0);

				if (GradientBackground)
					DrawGradientBG();

				if (showOrientationCube)
				{
					orientationCubeMtx =
					Matrix4.CreateRotationY(camRotX) *
					Matrix4.CreateRotationX(camRotY) *
					Matrix4.CreateScale(80f / Width, 40f / Height, 0.25f) *
					Matrix4.CreateTranslation(1 - 160f / Width, 1 - 80f / Height, 0) *
					Matrix4.CreateRotationY(-0.03125f);
					GL.LoadMatrix(ref orientationCubeMtx);
					
					DrawOrientationCube();
				}

				if (showFakeCursor)
					DrawFakeCursor();

				#endregion
			}
            else
            {
                GL.Viewport(0, 0, Width, Height);

                mtxMdl = Matrix4.Identity;
                mtxCam =
                    Matrix4.CreateTranslation(camTarget) *
                    Matrix4.CreateRotationY(camRotX) *
                    Matrix4.CreateRotationX(camRotY) *
                    Matrix4.CreateTranslation(0, 0, camDistance);

				mainDrawable.Draw(this, Pass.OPAQUE);

				mainDrawable.Draw(this, Pass.TRANSPARENT);

				shader = null;
				GL.UseProgram(0);

				if (GradientBackground)
					DrawGradientBG();

				if (showOrientationCube)
				{
					orientationCubeMtx =
					Matrix4.CreateRotationY(camRotX) *
					Matrix4.CreateRotationX(camRotY) *
					Matrix4.CreateScale(40f / Width, 40f / Height, 0.25f) *
					Matrix4.CreateTranslation(1 - 80f / Width, 1 - 80f / Height, 0);
					GL.LoadMatrix(ref orientationCubeMtx);
					
					DrawOrientationCube();
				}
			}
            SwapBuffers();
        }

        public override void DrawPicking()
        {
            if (DesignMode) return;
            MakeCurrent();
            GL.ClearColor(0f, 0f, 0f, 0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (stereoscopy)
                GL.Viewport(0, 0, Width / 2, Height);
            else
                GL.Viewport(0, 0, Width, Height);

			if (showOrientationCube)
				skipPickingColors(6); //the orientation cube faces

			mainDrawable.Draw(this, Pass.PICKING);

			if (showOrientationCube)
			{
				GL.UseProgram(0);
				GL.Disable(EnableCap.Texture2D);
				GL.MatrixMode(MatrixMode.Projection);
				GL.LoadIdentity();
				GL.MatrixMode(MatrixMode.Modelview);
				orientationCubeMtx =
					Matrix4.CreateRotationY(camRotX) *
					Matrix4.CreateRotationX(camRotY) *
					Matrix4.CreateScale((stereoscopy ? 80f : 40f) / Width, 40f / Height, 0.25f) *
					Matrix4.CreateTranslation(1 - (stereoscopy ? 160f : 80f) / Width, 1 - 80f / Height, 0);
				GL.LoadMatrix(ref orientationCubeMtx);
				GL.Disable(EnableCap.DepthTest);

				GL.Begin(PrimitiveType.Quads);
				GL.Color4(Color.FromArgb(1)); //UP
				GL.Vertex3(-1f, 1f, 1f);
				GL.Vertex3(1f, 1f, 1f);
				GL.Vertex3(1f, 1f, -1f);
				GL.Vertex3(-1f, 1f, -1f);
				GL.Color4(Color.FromArgb(2)); //DOWN
				GL.Vertex3(-1f, -1f, -1f);
				GL.Vertex3(1f, -1f, -1f);
				GL.Vertex3(1f, -1f, 1f);
				GL.Vertex3(-1f, -1f, 1f);
				GL.Color4(Color.FromArgb(3)); //FRONT
				GL.Vertex3(1f, 1f, 1f);
				GL.Vertex3(-1f, 1f, 1f);
				GL.Vertex3(-1f, -1f, 1f);
				GL.Vertex3(1f, -1f, 1f);
				GL.Color4(Color.FromArgb(4)); //BACK
				GL.Vertex3(-1f, 1f, -1f);
				GL.Vertex3(1f, 1f, -1f);
				GL.Vertex3(1f, -1f, -1f);
				GL.Vertex3(-1f, -1f, -1f);
				GL.Color4(Color.FromArgb(5)); //LEFT
				GL.Vertex3(1f, 1f, -1f);
				GL.Vertex3(1f, 1f, 1f);
				GL.Vertex3(1f, -1f, 1f);
				GL.Vertex3(1f, -1f, -1f);
				GL.Color4(Color.FromArgb(6)); //RIGHT
				GL.Vertex3(-1f, 1f, 1f);
				GL.Vertex3(-1f, 1f, -1f);
				GL.Vertex3(-1f, -1f, -1f);
				GL.Vertex3(-1f, -1f, 1f);
				GL.End();
				GL.Enable(EnableCap.DepthTest);
				GL.Enable(EnableCap.Texture2D);
			}
        }

    }
}
