using GL_EditorFramework.Interfaces;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GL_EditorFramework.StandardCameras
{
	public class WalkaroundCamera : AbstractCamera
	{
		private float maxCamMoveSpeed;

		public WalkaroundCamera(float maxCamMoveSpeed = 0.1f)
		{
			this.maxCamMoveSpeed = maxCamMoveSpeed;
		}

		public override uint MouseDown(MouseEventArgs e, I3DControl control)
		{
			if (OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.ControlLeft) &&
				e.Button == MouseButtons.Right &&
				control.PickingDepth != control.ZFar)
			{
				float delta = control.PickingDepth + control.CameraDistance;
				control.CameraTarget -= Vector3.UnitX * (float)Math.Sin(control.CamRotX) * (float)Math.Cos(control.CamRotY) * delta;
				control.CameraTarget += Vector3.UnitY * (float)Math.Sin(control.CamRotY) * delta;
				control.CameraTarget += Vector3.UnitZ * (float)Math.Cos(control.CamRotX) * (float)Math.Cos(control.CamRotY) * delta;

				Vector2 normCoords = control.NormMouseCoords(e.Location.X, e.Location.Y);

				float factoffX = (float)(-normCoords.X * control.PickingDepth) * control.FactorX;
				float factoffY = (float)(-normCoords.Y * control.PickingDepth) * control.FactorY;

				control.CameraTarget += Vector3.UnitX * (float)Math.Cos(control.CamRotX) * factoffX;
				control.CameraTarget -= Vector3.UnitX * (float)Math.Sin(control.CamRotX) * (float)Math.Sin(control.CamRotY) * factoffY;
				control.CameraTarget -= Vector3.UnitY * (float)Math.Cos(control.CamRotY) * factoffY;
				control.CameraTarget += Vector3.UnitZ * (float)Math.Sin(control.CamRotX) * factoffX;
				control.CameraTarget += Vector3.UnitZ * (float)Math.Cos(control.CamRotX) * (float)Math.Sin(control.CamRotY) * factoffY;
			}
			base.MouseDown(e, control);
			return UPDATE_CAMERA;
		}

		public override uint MouseMove(MouseEventArgs e, Point lastMouseLoc, I3DControl control)
		{
			float deltaX = e.Location.X - lastMouseLoc.X;
			float deltaY = e.Location.Y - lastMouseLoc.Y;

			if (e.Button == MouseButtons.Right)
			{
				control.CamRotX += deltaX * 0.00390625f;
				control.CamRotY += deltaY * 0.00390625f;
				return UPDATE_CAMERA;
			}
			else if (e.Button == MouseButtons.Left)
			{
				base.MouseMove(e, lastMouseLoc, control);

				if (OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.ControlLeft))
				{
					float delta = ((float)deltaY*-5 * Math.Min(0.01f, depth / 500f));
					control.CameraTarget -= Vector3.UnitX * (float)Math.Sin(control.CamRotX) * (float)Math.Cos(control.CamRotY) * delta;
					control.CameraTarget += Vector3.UnitY * (float)Math.Sin(control.CamRotY) * delta;
					control.CameraTarget += Vector3.UnitZ * (float)Math.Cos(control.CamRotX) * (float)Math.Cos(control.CamRotY) * delta;
				}
				else
				{

					//code from Whitehole

					deltaX *= Math.Min(maxCamMoveSpeed, depth * control.FactorX);
					deltaY *= Math.Min(maxCamMoveSpeed, depth * control.FactorY);

					control.CameraTarget += Vector3.UnitX * deltaX * (float)Math.Cos(control.CamRotX);
					control.CameraTarget -= Vector3.UnitX * deltaY * (float)Math.Sin(control.CamRotX) * (float)Math.Sin(control.CamRotY);
					control.CameraTarget -= Vector3.UnitY * deltaY * (float)Math.Cos(control.CamRotY);
					control.CameraTarget += Vector3.UnitZ * deltaX * (float)Math.Sin(control.CamRotX);
					control.CameraTarget += Vector3.UnitZ * deltaY * (float)Math.Cos(control.CamRotX) * (float)Math.Sin(control.CamRotY);

				}

				return UPDATE_CAMERA;
			}
			return 0;
		}

		public override uint MouseWheel(MouseEventArgs e, I3DControl control)
		{
			depth = control.PickingDepth;
			float delta = ((float)e.Delta * Math.Min(0.01f, depth / 500f));
			control.CameraTarget -= Vector3.UnitX * (float)Math.Sin(control.CamRotX) * (float)Math.Cos(control.CamRotY) * delta;
			control.CameraTarget += Vector3.UnitY * (float)Math.Sin(control.CamRotY) * delta;
			control.CameraTarget += Vector3.UnitZ * (float)Math.Cos(control.CamRotX) * (float)Math.Cos(control.CamRotY) * delta;

			Vector2 normCoords = control.NormMouseCoords(e.Location.X, e.Location.Y);

			float factoffX = (float)(-normCoords.X * delta) * control.FactorX;
			float factoffY = (float)(-normCoords.Y * delta) * control.FactorY;

			control.CameraTarget += Vector3.UnitX * (float)Math.Cos(control.CamRotX) * factoffX;
			control.CameraTarget -= Vector3.UnitX * (float)Math.Sin(control.CamRotX) * (float)Math.Sin(control.CamRotY) * factoffY;
			control.CameraTarget -= Vector3.UnitY * (float)Math.Cos(control.CamRotY) * factoffY;
			control.CameraTarget += Vector3.UnitZ * (float)Math.Sin(control.CamRotX) * factoffX;
			control.CameraTarget += Vector3.UnitZ * (float)Math.Cos(control.CamRotX) * (float)Math.Sin(control.CamRotY) * factoffY;
			return UPDATE_CAMERA;
		}
	}
}
