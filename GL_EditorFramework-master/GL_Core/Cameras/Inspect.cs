using GL_Core.Public_Interfaces;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GL_Core.Cameras
{
    public class InspectCamera : AbstractCamera
	{
		public InspectCamera()
		{

		}
		public override bool MouseMove(MouseEventArgs e, Point lastMouseLoc, float deltaX, float deltaY, ExtraArgs args, ref Vector3 camTarget, ref float rot, ref float lookup, ref float zoom)
		{
			if (e.Button == MouseButtons.Right)
			{
				rot += deltaX * 0.01f;
				lookup += deltaY * 0.01f;
				return true;
			}
			else if (e.Button == MouseButtons.Left)
			{
				deltaX *= Math.Min(0.1f, args.pickingDepth * args.factorX);
				deltaY *= Math.Min(0.1f, args.pickingDepth * args.factorY);

				camTarget += Vector3.UnitX * (float)Math.Cos(rot) * deltaX;
				camTarget -= Vector3.UnitX * (float)Math.Sin(rot) * (float)Math.Sin(lookup) * deltaY;
				camTarget -= Vector3.UnitY * (float)Math.Cos(lookup) * deltaY;
				camTarget += Vector3.UnitZ * (float)Math.Sin(rot) * deltaX;
				camTarget += Vector3.UnitZ * (float)Math.Cos(rot) * (float)Math.Sin(lookup) * deltaY;
				return true;
			}
			return false;
		}

		public override bool MouseWheel(MouseEventArgs e, float xoff, float yoff, ExtraArgs args, ref Vector3 camTarget, ref float rot, ref float lookup, ref float zoom)
		{
			zoom *= 1f - e.Delta * 0.001f;
			return true;
		}
	}
}
