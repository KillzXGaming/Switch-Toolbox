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
    public class GL_ControlLegacy : GL_ControlBase
    {
        public GL_ControlLegacy(int redrawerInterval) : base(1, redrawerInterval)
        {

        }

        public GL_ControlLegacy() : base(1, 16)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode) return;
            MakeCurrent();
            Framework.Initialize();
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

        public override void UpdateModelMatrix(Matrix4 matrix)
        {
            if (DesignMode) return;
            mtxMdl = matrix;
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref mtxMdl);
        }

        public override void ApplyModelTransform(Matrix4 matrix)
        {
            if (DesignMode) return;
            mtxMdl *= matrix;
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref mtxMdl);
        }

        public override void ResetModelMatrix()
        {
            if (DesignMode) return;
            mtxMdl = Matrix4.Identity;
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref mtxMdl);
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            if (mainDrawable == null || DesignMode)
            {
                e.Graphics.Clear(this.BackColor);
                e.Graphics.DrawString("Legacy Gl" + (stereoscopy ? " stereoscopy" : ""), SystemFonts.DefaultFont, SystemBrushes.ControlLight, 10f, 10f);
                return;
            }

            MakeCurrent();

            GL.ClearColor(BackgroundColor1);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (stereoscopy)
            {
                #region left eye
                GL.Viewport(0, 0, Width / 2, Height);

                ResetModelMatrix();
                mtxCam =
                    Matrix4.CreateTranslation(camTarget) *
                    Matrix4.CreateRotationY(camRotX) *
                    Matrix4.CreateRotationX(camRotY) *
                    Matrix4.CreateTranslation(0.25f, 0, camDistance) *
                    Matrix4.CreateRotationY(0.02f);

                GL.MatrixMode(MatrixMode.Projection);
                Matrix4 computedMatrix = mtxCam * mtxProj;
                GL.LoadMatrix(ref computedMatrix);

				mainDrawable.Draw(this, Pass.OPAQUE);

				mainDrawable.Draw(this, Pass.TRANSPARENT);

				GL.MatrixMode(MatrixMode.Projection);
				GL.LoadIdentity();

				if (GradientBackground)
					DrawGradientBG();

				if (showOrientationCube)
				{
					GL.MatrixMode(MatrixMode.Modelview);
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

                ResetModelMatrix();
                mtxCam =
                    Matrix4.CreateTranslation(camTarget) *
                    Matrix4.CreateRotationY(camRotX) *
                    Matrix4.CreateRotationX(camRotY) *
                    Matrix4.CreateTranslation(-0.25f, 0, camDistance) *
                    Matrix4.CreateRotationY(-0.02f);

                GL.MatrixMode(MatrixMode.Projection);
                computedMatrix = mtxCam * mtxProj;
                GL.LoadMatrix(ref computedMatrix);

				mainDrawable.Draw(this, Pass.OPAQUE);

				mainDrawable.Draw(this, Pass.TRANSPARENT);

				GL.MatrixMode(MatrixMode.Projection);
				GL.LoadIdentity();

				if (GradientBackground)
					DrawGradientBG();

				if (showOrientationCube)
				{
					GL.MatrixMode(MatrixMode.Modelview);
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

                ResetModelMatrix();
                mtxCam =
                    Matrix4.CreateTranslation(camTarget) *
                    Matrix4.CreateRotationY(camRotX) *
                    Matrix4.CreateRotationX(camRotY) *
                    Matrix4.CreateTranslation(0, 0, camDistance);

                GL.MatrixMode(MatrixMode.Projection);
                Matrix4 computedMatrix = mtxCam * mtxProj;
                GL.LoadMatrix(ref computedMatrix);

				mainDrawable.Draw(this, Pass.OPAQUE);

				mainDrawable.Draw(this, Pass.TRANSPARENT);

				GL.MatrixMode(MatrixMode.Projection);
				GL.LoadIdentity();

				if (GradientBackground)
					DrawGradientBG();

				if (showOrientationCube)
				{
					GL.MatrixMode(MatrixMode.Modelview);
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

            ResetModelMatrix();
            mtxCam =
                Matrix4.CreateTranslation(camTarget) *
                Matrix4.CreateRotationY(camRotX) *
                Matrix4.CreateRotationX(camRotY) *
                Matrix4.CreateTranslation(0, 0, camDistance);

            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 computedMatrix = mtxCam * mtxProj;
            GL.LoadMatrix(ref computedMatrix);

			if (showOrientationCube)
				skipPickingColors(6); //the orientation cube faces

			mainDrawable.Draw(this, Pass.PICKING);

			if (showOrientationCube)
			{
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
