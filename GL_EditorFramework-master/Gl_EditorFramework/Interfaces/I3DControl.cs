using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GL_EditorFramework.Interfaces
{
	public interface I3DControl
	{
		void AttachRedrawer();
		void AttachPickingRedrawer();
		void DetachRedrawer();
		void DetachPickingRedrawer();

		ulong RedrawerFrame { get; }

		int ViewWidth { get; }
		int ViewHeighth { get; }
		Vector2 NormMouseCoords(int x, int y);

		Point DragStartPos { get; set; }

		float ZFar { get; set; }
		float ZNear { get; set; }
		float Fov { get; set; }
		float FactorX { get; }
		float FactorY { get; }

		Vector3 CameraTarget { get; set; }
		float CameraDistance { get; set; }
		float CamRotX { get; set; }
		float CamRotY { get; set; }

		Vector3 coordFor(int x, int y, float depth);

		Vector3 screenCoordFor(Vector3 coord);

		float PickingDepth { get; }

		float NormPickingDepth { get; }
	}
}
