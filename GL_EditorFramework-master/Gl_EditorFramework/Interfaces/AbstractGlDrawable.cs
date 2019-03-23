using GL_EditorFramework.GL_Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GL_EditorFramework.Interfaces
{
	public enum Pass
	{
		OPAQUE,
		TRANSPARENT,
		PICKING
	}

	public abstract class AbstractGlDrawable : AbstractEventHandling3DObj
	{
		public const uint REDRAW =				0x80000000;
		public const uint REDRAW_PICKING =		0xC0000000;
		public const uint NO_CAMERA_ACTION =	0x20000000;
		public const uint REPICK =				0x40000000;

		public abstract void Prepare(GL_ControlModern control);
		public abstract void Prepare(GL_ControlLegacy control);
		public abstract void Draw(GL_ControlModern control, Pass pass);
		public abstract void Draw(GL_ControlLegacy control, Pass pass);
		public virtual int GetPickableSpan() => 1;
		public virtual uint MouseEnter(int index, I3DControl control) { return 0; }
		public virtual uint MouseLeave(int index, I3DControl control) { return 0; }
		public virtual uint MouseLeaveEntirely(I3DControl control) { return 0; }
		public override uint MouseMove(MouseEventArgs e, Point lastMousePos, I3DControl control) { return REPICK; }
	}
}
