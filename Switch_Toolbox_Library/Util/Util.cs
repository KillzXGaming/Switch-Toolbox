using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Switch_Toolbox.Library
{
   public class Utils
    {
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
        static int i = 0;
        public static string RenameDuplicateString(List<string> strings, string oldString)
        {
            foreach (string s in strings)
            {
                if (strings.Contains(oldString))
                {
                    oldString += i.ToString();

                    if (strings.Contains(oldString))
                        RenameDuplicateString(strings, oldString);
                    else
                        return oldString;
                }
            }
            return oldString;
        }
        public static Matrix4 TransformValues(Vector3 translation, Vector3 rotation, float scale)
        {
            return TransformValues(translation, rotation, new Vector3(scale));
        }
        public static Matrix4 TransformValues(Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            Matrix4 positionMat = Matrix4.CreateTranslation(translation);
            Matrix4 rotXMat = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(rotation.X));
            Matrix4 rotYMat = Matrix4.CreateRotationY(MathHelper.DegreesToRadians(rotation.Y));
            Matrix4 rotZMat = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotation.Z));
            Matrix4 scaleMat = Matrix4.CreateScale(scale);
            return scaleMat * (rotXMat * rotYMat * rotZMat) * positionMat;
        }

        public static string GenerateUniqueHashID()
        {
            return Guid.NewGuid().ToString();
        }

        public static string GetAllFilters(IEnumerable<IFileFormat> format)
        {
            var alltypes = format;

            string Filter = "All Supported Files|";
            List<string> FilterEach = new List<string>();
            foreach (IFileFormat f in format)
            {
                for (int i = 0; i < f.Extension.Length; i++)
                {
                    Filter += $"{f.Extension[i]};";
                    FilterEach.Add($"{f.Description[0]} ({f.Extension[i]}) |{f.Extension[i]}|");
                }
            }
            Filter += "|";
            Filter += string.Join("", FilterEach.ToArray());
            Filter += "All files(*.*)|*.*";
            return Filter;
        }
    }
}
