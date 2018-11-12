using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GL_Core.Public_Interfaces
{
	public abstract class AbstractCamera
	{
		public virtual bool MouseDown(MouseEventArgs e, ref Vector3 camTarget, ref float rot, ref float lookup, ref float zoom) { return false; }
		public virtual bool MouseMove(MouseEventArgs e, Point lastMouseLoc, float deltaX, float deltaY, ExtraArgs args, ref Vector3 camTarget, ref float rot, ref float lookup, ref float zoom) { return false; }
		public virtual bool MouseUp(MouseEventArgs e, Point dragStartMousePos, ref Vector3 camTarget, ref float rot, ref float lookup, ref float zoom) { return false; }
		public virtual bool MouseWheel(MouseEventArgs e, float xoff, float yoff, ExtraArgs args, ref Vector3 camTarget, ref float rot, ref float lookup, ref float zoom) { return false; }
		public virtual bool MouseClick(MouseEventArgs e, ref Vector3 camTarget, ref float rot, ref float lookup, ref float zoom) { return false; }

		public virtual bool KeyDown(KeyEventArgs e, ref Vector3 camTarget, ref float rot, ref float lookup, ref float zoom) { return false; }
		public virtual bool KeyUp(KeyEventArgs e, ref Vector3 camTarget, ref float rot, ref float lookup, ref float zoom) { return false; }

		public virtual bool TimeStep(ref Vector3 camTarget, ref float rot, ref float lookup, ref float zoom) { return false; }
	}

	public struct ExtraArgs{
		public float factorX, factorY, pickingDepth;
		public ExtraArgs(float factorX, float factorY, float pickingDepth)
		{
			this.factorX = factorX;
			this.factorY = factorY;
			this.pickingDepth = pickingDepth;
		}

	}
}
