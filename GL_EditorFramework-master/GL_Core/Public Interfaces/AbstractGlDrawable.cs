using GL_Core.Public_Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GL_Core.Interfaces
{
	public abstract class AbstractGlDrawable
	{
		public abstract void Prepare(GL_ControlModern control);
		public abstract void Prepare(GL_ControlLegacy control);
		public abstract void Draw(GL_ControlModern control);
		public abstract void Draw(GL_ControlLegacy control);
		public virtual void DrawPicking(GL_ControlModern control) { }
		public virtual void DrawPicking(GL_ControlLegacy control) { }
		public virtual bool Picked(uint index) { return false; }//handling for Picking
		public virtual uint GetPickableSpan() => 0;//tells how many pickable subobjects are in this object

		public virtual bool MouseDown(MouseEventArgs e) { return false; }
		public virtual bool MouseMove(MouseEventArgs e, Point lastMouseLoc, float deltaX, float deltaY, ExtraArgs args, float rot, float lookup) { return false; }
		public virtual bool MouseUp(MouseEventArgs e, Point dragStartMousePos) { return false; }
		public virtual bool MouseWheel(MouseEventArgs e, float xoff, float yoff, ExtraArgs args, float rot, float lookup) { return false; }
		public virtual bool MouseClick(MouseEventArgs e) { return false; }

		public virtual bool KeyDown(KeyEventArgs e) { return false; }
		public virtual bool KeyUp(KeyEventArgs e) { return false; }

		public virtual bool TimeStep() { return false; }
	}
}
