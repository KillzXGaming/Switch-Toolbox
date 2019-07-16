using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;

namespace Toolbox.Library
{
    public class ColorUtility
    {
        public static Vector3 ToVector3(Color color)
        {
            return new Vector3(color.R / 255.0f,
                               color.G / 255.0f,
                               color.B / 255.0f);
        }

        public static Vector4 ToVector4(byte[] color)
        {
            if (color == null || color.Length != 4)
                throw new Exception("Invalid color length found! (ToVector4)");

            return new Vector4(color[0] / 255.0f,
                               color[1] / 255.0f,
                               color[2] / 255.0f,
                               color[3] / 255.0f);
        }

        public static Vector4 ToVector4(Color color)
        {
            return new Vector4(color.R / 255.0f,
                               color.G / 255.0f,
                               color.B / 255.0f,
                               color.A / 255.0f);
        }
    }
}
