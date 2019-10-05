using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Toolbox.Library.IO
{
    public static class RectangleExtension
    {
        public static bool IsHit(this Rectangle hitbox, Point e)
        {
            return (e.X > hitbox.X) && (e.X < hitbox.X + hitbox.Width) &&
          (e.Y > hitbox.Y) && (e.Y < hitbox.Y + hitbox.Height);
        }

    }
}
