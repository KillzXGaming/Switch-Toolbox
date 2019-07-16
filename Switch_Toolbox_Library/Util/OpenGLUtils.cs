using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Toolbox.Library
{
    public class OpenGLUtils
    {
        public static Vector3 GetMax(Vector3 value1, Vector3 value2)
        {
            Vector3 val = new Vector3(0);
            val.X = Math.Max(value1.X, value2.X);
            val.Y = Math.Max(value1.Y, value2.Y);
            val.Z = Math.Max(value1.Z, value2.Z);
            return val;
        }

        public static Vector3 GetMin(Vector3 value1, Vector3 value2)
        {
            Vector3 val = new Vector3(0);
            val.X = Math.Min(value1.X, value2.X);
            val.Y = Math.Min(value1.Y, value2.Y);
            val.Z = Math.Min(value1.Z, value2.Z);
            return val;
        }
    }
}
