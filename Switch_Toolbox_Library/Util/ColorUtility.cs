using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;

namespace Switch_Toolbox.Library
{
    public class ColorUtility
    {
        public static Vector3 ToVector3(Color color)
        {
            return new Vector3(color.R / 255.0f,
                               color.G / 255.0f,
                               color.B / 255.0f);
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
