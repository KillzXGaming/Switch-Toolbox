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
    public class WalkaroundCamera : AbstractCamera
	{
		public WalkaroundCamera()
		{

		}
		public override bool MouseMove(MouseEventArgs e, Point lastMouseLoc, float deltaX, float deltaY, ExtraArgs args, ref Vector3 camTarget, ref float rot, ref float lookup, ref float zoom)
		{

			if (e.Button == MouseButtons.Right)
			{
				rot += deltaX * 0.002f;
				lookup += deltaY * 0.002f;
				return true;
			}
			else if (e.Button == MouseButtons.Left)
			{
				//code from Whitehole
				
				deltaX *= Math.Min(0.1f, args.pickingDepth* args.factorX);
				deltaY *= Math.Min(0.1f, args.pickingDepth* args.factorY);

				camTarget += Vector3.UnitX * deltaX * (float)Math.Cos(rot);
				camTarget -= Vector3.UnitX * deltaY * (float)Math.Sin(rot) * (float)Math.Sin(lookup);
				camTarget -= Vector3.UnitY * deltaY * (float)Math.Cos(lookup);
				camTarget += Vector3.UnitZ * deltaX * (float)Math.Sin(rot);
				camTarget += Vector3.UnitZ * deltaY * (float)Math.Cos(rot) * (float)Math.Sin(lookup);

				return true;
			}
			return false;
		}

		public override bool MouseWheel(MouseEventArgs e, float xoff, float yoff, ExtraArgs args, ref Vector3 camTarget, ref float rot, ref float lookup, ref float zoom)
		{
			float delta = (float)(e.Delta * Math.Min(0.01f, args.pickingDepth / 500f));
			camTarget -= Vector3.UnitX * (float)Math.Sin(rot) * (float)Math.Cos(lookup) * delta;
			camTarget += Vector3.UnitY * (float)Math.Sin(lookup)						* delta;
			camTarget += Vector3.UnitZ * (float)Math.Cos(rot) * (float)Math.Cos(lookup) * delta;

			float factoffX = (float)(-xoff * delta) * args.factorX;
			float factoffY = (float)(-yoff * delta) * args.factorY;

			camTarget += Vector3.UnitX * (float)Math.Cos(rot) * factoffX;
			camTarget -= Vector3.UnitX * (float)Math.Sin(rot) * (float)Math.Sin(lookup) * factoffY;
			camTarget -= Vector3.UnitY * (float)Math.Cos(lookup) * factoffY;
			camTarget += Vector3.UnitZ * (float)Math.Sin(rot) * factoffX;
			camTarget += Vector3.UnitZ * (float)Math.Cos(rot) * (float)Math.Sin(lookup) * factoffY;
			return true;
		}
	}
}
