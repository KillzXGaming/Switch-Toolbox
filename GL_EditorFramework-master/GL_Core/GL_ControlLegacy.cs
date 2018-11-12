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
using GL_Core.Interfaces;
using GL_Core.Public_Interfaces;
using GL_Core.Cameras;

namespace GL_Core
{
	public partial class GL_ControlLegacy : GLControl
	{
		public GL_ControlLegacy():base(OpenTK.Graphics.GraphicsMode.Default,1,1, OpenTK.Graphics.GraphicsContextFlags.Default)
		{
			InitializeComponent();
		}

		private Point lastMouseLoc;
		private Point dragStartLoc;
		private float lastDepth = 1000f;
		private AbstractCamera activeCamera;
		float rot = 0;
		float lookup = 0;
		float zoom = -10f;
		Vector3 camTarget;

		float zfar = 1000f;
		float znear = 0.01f;
		float fov = MathHelper.PiOver4;

		uint[] pickingFrameBuffer = new uint[9];
		int pickingIndex;
		float pickingModelDepth = 0f;
		private float pickingDepth = 0f;

		public Matrix4 mtxMdl, mtxCam, mtxProj;
		private float factorX, factorY;

		bool stereoscopy;

		int viewPortX(int x) => stereoscopy ? x % (Width / 2) : x;
		int viewPortDX(int dx) => stereoscopy ? dx * 2 : dx;
		int viewPortXOff(int x) => stereoscopy ? (x - Width / 4)*2 : x - Width / 2;
		int viewPortWidth() => stereoscopy ? Width / 2 : Width;

		public bool Stereoscopy
		{
			get => stereoscopy;
			set
			{
				stereoscopy = value;
				OnResize(null);
				Refresh();
			}
		}

		AbstractGlDrawable mainDrawable;

		public AbstractGlDrawable MainDrawable
		{
			get => mainDrawable;
			set
			{
				if (value == null) return;
				mainDrawable = value;
				MakeCurrent();
				mainDrawable.Prepare(this);
				Refresh();
			}
		}

		public AbstractCamera ActiveCamera
		{
			get => activeCamera;
			set
			{
				if (value == null) return;
				activeCamera = value;
				MakeCurrent();
				Refresh();
			}
		}

		public Color nextPickingColor()
		{
			return Color.FromArgb(pickingIndex++);
		}
		public void UpdateModelMatrix(Matrix4 matrix)
		{
			mtxMdl = matrix;
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref mtxMdl);
		}

		public void ApplyModelTransform(Matrix4 matrix)
		{
			mtxMdl *= matrix;
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref mtxMdl);
		}

		public void ResetModelMatrix()
		{
			mtxMdl = Matrix4.Identity;
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref mtxMdl);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (DesignMode) return;

			activeCamera = new WalkaroundCamera();

			GL.Enable(EnableCap.DepthTest);
		}

		protected override void OnResize(EventArgs e)
		{
			if (DesignMode || this.Context == null) return;
			MakeCurrent();

			float aspect_ratio;
			if (stereoscopy)
				aspect_ratio = Width / 2 / (float)Height;
			else
				aspect_ratio = Width / (float)Height;

			mtxProj = Matrix4.CreatePerspectiveFieldOfView(fov, aspect_ratio, znear, zfar);

			//using the calculation from whitehole
			factorX = (2f * (float)Math.Tan(fov * 0.5f) * aspect_ratio) / Width;

			factorY = (2f * (float)Math.Tan(fov * 0.5f)) / Height;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (mainDrawable == null || DesignMode)
			{
				e.Graphics.Clear(this.BackColor);
				e.Graphics.DrawString("Legacy Gl" + (stereoscopy ? " stereoscopy" : ""), SystemFonts.DefaultFont, SystemBrushes.ControlLight, 10f, 10f);
				return;
			}
			
			GL.ClearColor(0.125f, 0.125f, 0.125f, 1.0f);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			if (stereoscopy)
			{
				#region left eye
				GL.Viewport(0, 0, Width / 2, Height);

				mtxMdl = Matrix4.Identity;
				mtxCam =
					Matrix4.CreateTranslation(camTarget) *
					Matrix4.CreateRotationY(rot + 0.02f) *
					Matrix4.CreateRotationX(lookup) *
					Matrix4.CreateTranslation(0, 0, zoom);

				GL.MatrixMode(MatrixMode.Projection);
				Matrix4 computedMatrix = mtxCam * mtxProj;
				GL.LoadMatrix(ref computedMatrix);

				mainDrawable.Draw(this);
				#endregion

				#region right eye
				GL.Viewport(Width / 2, 0, Width / 2, Height);

				mtxMdl = Matrix4.Identity;
				mtxCam =
					Matrix4.CreateTranslation(camTarget) *
					Matrix4.CreateRotationY(rot - 0.02f) *
					Matrix4.CreateRotationX(lookup) *
					Matrix4.CreateTranslation(0, 0, zoom);

				computedMatrix = mtxCam * mtxProj;
				GL.LoadMatrix(ref computedMatrix);
				mainDrawable.Draw(this);
				#endregion
			}
			else
			{
				GL.Viewport(0, 0, Width, Height);

				mtxMdl = Matrix4.Identity;
				mtxCam =
					Matrix4.CreateTranslation(camTarget) *
					Matrix4.CreateRotationY(rot - 0.02f) *
					Matrix4.CreateRotationX(lookup) *
					Matrix4.CreateTranslation(0, 0, zoom);

				GL.MatrixMode(MatrixMode.Projection);
				Matrix4 computedMatrix = mtxCam * mtxProj;
				GL.LoadMatrix(ref computedMatrix);
				mainDrawable.Draw(this);
			}
			
			SwapBuffers();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (mainDrawable == null) return;

			lastMouseLoc = e.Location;
			lastDepth = pickingDepth;
			dragStartLoc = e.Location;
			if (mainDrawable.MouseDown(e) ||
			activeCamera.MouseDown(e, ref camTarget, ref rot, ref lookup, ref zoom))
				Refresh();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (mainDrawable == null) return;

			float deltaX = viewPortDX(e.Location.X - lastMouseLoc.X);
			float deltaY = e.Location.Y - lastMouseLoc.Y;

			bool shouldredraw = false;
			shouldredraw = shouldredraw || activeCamera.MouseMove(e, lastMouseLoc, deltaX, deltaY, new ExtraArgs(factorX, factorY, lastDepth), ref camTarget, ref rot, ref lookup, ref zoom);

			lastMouseLoc = e.Location;

			#region picking
			MakeCurrent();

			GL.ClearColor(0f, 0f, 0f, 0f);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			if (stereoscopy)
				GL.Viewport(0, 0, Width / 2, Height);
			else
				GL.Viewport(0, 0, Width, Height);

			int pickingMouseX = stereoscopy ? (lastMouseLoc.X % (Width / 2)) : lastMouseLoc.X;

			mtxMdl = Matrix4.Identity;
			mtxCam =
				Matrix4.CreateTranslation(camTarget) *
				Matrix4.CreateRotationY(rot - (stereoscopy ? 0.02f : 0f)) *
				Matrix4.CreateRotationX(lookup) *
				Matrix4.CreateTranslation(0, 0, zoom);

			GL.MatrixMode(MatrixMode.Projection);
			Matrix4 computedMatrix = mtxCam * mtxProj;
			GL.LoadMatrix(ref computedMatrix);

			pickingIndex = 1;

			mainDrawable.DrawPicking(this);
			GL.Flush();

			GL.ReadPixels(pickingMouseX, Height - lastMouseLoc.Y, 1, 1, PixelFormat.DepthComponent, PixelType.Float, ref pickingModelDepth);

			pickingModelDepth = -(zfar * znear / (pickingModelDepth * (zfar - znear) - zfar));



			GL.Flush();

			GL.ReadPixels(pickingMouseX - 1, Height - lastMouseLoc.Y + 1, 3, 3, PixelFormat.Bgra, PixelType.UnsignedByte, pickingFrameBuffer);



			// depth math from http://www.opengl.org/resources/faq/technical/depthbuffer.htm

			GL.ReadPixels(pickingMouseX, Height - lastMouseLoc.Y, 1, 1, PixelFormat.DepthComponent, PixelType.Float, ref pickingDepth);

			pickingDepth = -(zfar * znear / (pickingDepth * (zfar - znear) - zfar));

			shouldredraw = shouldredraw || mainDrawable.Picked(pickingFrameBuffer[4]);

			#endregion

			shouldredraw = shouldredraw || mainDrawable.MouseMove(e, lastMouseLoc, deltaX, deltaY, new ExtraArgs(factorX, factorY, lastDepth), rot, lookup);

			if (shouldredraw)
				Refresh();
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (mainDrawable == null) return;

			int x = viewPortX(e.X);

			if (mainDrawable.MouseWheel(e, viewPortXOff(x), e.Y - Height / 2, new ExtraArgs(factorX,factorY, lastDepth), rot, lookup) ||
			activeCamera.MouseWheel(e, viewPortXOff(x), e.Y - Height / 2, new ExtraArgs(factorX, factorY, lastDepth), ref camTarget, ref rot, ref lookup, ref zoom))
				Refresh();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (mainDrawable == null) return;

			bool shouldredraw = false;

			if ((Math.Abs(e.X - dragStartLoc.X) < 3) && (Math.Abs(e.Y - dragStartLoc.Y) < 3))
			{
				shouldredraw = shouldredraw || mainDrawable.MouseClick(e) ||
				activeCamera.MouseClick(e, ref camTarget, ref rot, ref lookup, ref zoom);
			}
			shouldredraw = shouldredraw || mainDrawable.MouseUp(e, dragStartLoc) ||
			activeCamera.MouseUp(e, dragStartLoc, ref camTarget, ref rot, ref lookup, ref zoom);

			if (shouldredraw)
				Refresh();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (mainDrawable == null) return;

			if (mainDrawable.KeyDown(e) ||
			activeCamera.KeyDown(e, ref camTarget, ref rot, ref lookup, ref zoom))
				Refresh();
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (mainDrawable == null) return;

			if (mainDrawable.KeyUp(e) ||
			activeCamera.KeyUp(e, ref camTarget, ref rot, ref lookup, ref zoom))
				Refresh();
		}
	}
}
