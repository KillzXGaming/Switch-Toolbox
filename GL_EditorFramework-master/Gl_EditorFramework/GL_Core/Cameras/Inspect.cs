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
	public class InspectCamera : AbstractCamera
	{
		private float maxCamMoveSpeed;

		public InspectCamera(float maxCamMoveSpeed = 0.1f)
		{
			this.maxCamMoveSpeed = maxCamMoveSpeed;
		}

		public override uint MouseDown(MouseEventArgs e, I3DControl control)
		{
			if (OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.ControlLeft) &&
				e.Button == MouseButtons.Right &&
				control.PickingDepth != control.ZFar)
			{
				control.CameraTarget = control.coordFor(e.X, e.Y, control.PickingDepth);

				return UPDATE_CAMERA;
			}
			return base.MouseDown(e, control);
		}

		public override uint MouseMove(MouseEventArgs e, Point lastMouseLoc, I3DControl control)
		{
			float deltaX = e.Location.X - lastMouseLoc.X;
			float deltaY = e.Location.Y - lastMouseLoc.Y;

			if (e.Button == MouseButtons.Right)
			{
				control.CamRotX += deltaX * 0.002f;
				control.CamRotY += deltaY * 0.002f;
				return UPDATE_CAMERA;
			}
			else if (e.Button == MouseButtons.Left)
			{
				base.MouseMove(e, lastMouseLoc, control);

				if (OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.ControlLeft))
					control.CameraDistance *= 1f - deltaY*-5 * 0.001f;
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
			control.CameraDistance *= 1f - e.Delta * 0.001f;
			return UPDATE_CAMERA;
		}
	}
}
