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
        public static Matrix3x4 ToMat3x4(Syroot.Maths.Matrix3x4 mat4)
        {
            Matrix3x4 mat = new Matrix3x4();
            mat.M11 = mat4.M11;
            mat.M12 = mat4.M12;
            mat.M13 = mat4.M13;
            mat.M14 = mat4.M14;
            mat.M21 = mat4.M21;
            mat.M22 = mat4.M22;
            mat.M23 = mat4.M23;
            mat.M24 = mat4.M24;
            mat.M31 = mat4.M31;
            mat.M32 = mat4.M32;
            mat.M33 = mat4.M33;
            mat.M34 = mat4.M34;
            return mat;
        }
        public static byte[] CombineByteArray(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }
        public static byte[] SubArray(byte[] data, uint offset, uint length)
        {
            return data.Skip((int)offset).Take((int)length).ToArray();
        }

        static int i = 0;
        public static string RenameDuplicateString(List<string> strings, string oldString)
        {
            foreach (string s in strings)
            {
                if (strings.Contains(oldString))
                {
                    oldString = $"{oldString}_0{i++}";

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
