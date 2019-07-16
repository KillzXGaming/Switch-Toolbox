using Syroot.BinaryData;
using System.IO;
using OpenTK;

namespace Toolbox.Library.IO
{
    public enum DataType
    {
        uint8,
        int8,
        uint16,
        int16,
        int32,
        uint32,
        int64,
        uint64,
    }

    public class FileExt
    {
        public static System.Drawing.Color[] ReadColors(int Count)
        {
            var colors = new System.Drawing.Color[Count];
            for (int i = 0; i < Count; i ++)
            {
                colors[i] = new System.Drawing.Color();
            }
            return colors;
        }

        public static Vector2 ToVec2(Syroot.Maths.Vector2F v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static Vector3 ToVec3(Syroot.Maths.Vector3F v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
        public static Vector4 ToVec4(Syroot.Maths.Vector4F v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }
        public static Vector2 ToVec2(float[] v)
        {
            return new Vector2(v[0], v[1]);
        }
        public static Vector3 ToVec3(float[] v)
        {
            return new Vector3(v[0], v[1], v[2]);
        }
        public static Vector4 ToVec4(float[] v)
        {
            return new Vector4(v[0], v[1], v[2], v[3]);
        }


        public static string DataToString(Syroot.Maths.Vector2F v)
        {
            return $"{v.X},{v.Y}";
        }
        public static string DataToString(Syroot.Maths.Vector3F v)
        {
            return $"{v.X},{v.Y},{v.Z}";
        }
        public static string DataToString(Syroot.Maths.Vector4F v)
        {
            return $"{v.X},{v.Y},{v.Z} {v.W}";
        }
    }
}
