using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GL_EditorFramework.Interfaces
{
	public abstract class AbstractCamera : AbstractEventHandling3DObj
	{
		public const uint UPDATE_CAMERA = 0x80000000;
		private const float depthAdjust = 1;

		protected float depth, desiredDepth;

		public override uint MouseDown(MouseEventArgs e, I3DControl control)
		{
			desiredDepth = control.PickingDepth;
			return 0;
		}

		public override uint MouseMove(MouseEventArgs e, Point lastMouseLoc, I3DControl control)
		{
			if (e.Button == MouseButtons.Left)
			{
				if (depth < desiredDepth - depthAdjust)
					depth += depthAdjust;
				else if (depth != desiredDepth)
					depth = desiredDepth;
			}

			return 0;
		}

		public virtual void Update() { }
	}
}
