using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Toolbox.Library
{
    public static class STMath
    {
        public const float Deg2Rad = (float)System.Math.PI / 180.0f;
        public const float Rad2Deg = 180.0f / (float)System.Math.PI;

        private const long SizeOfKb = 1024;
        private const long SizeOfMb = SizeOfKb * 1024;
        private const long SizeOfGb = SizeOfMb * 1024;
        private const long SizeOfTb = SizeOfGb * 1024;

        public static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / SizeOfKb) / SizeOfKb;
        }

        static double ConvertKilobytesToMegabytes(long kilobytes)
        {
            return kilobytes / SizeOfKb;
        }

        public static string GetFileSize(this long value, int decimalPlaces = 0)
        {
            var asTb = Math.Round((double)value / SizeOfTb, decimalPlaces);
            var asGb = Math.Round((double)value / SizeOfGb, decimalPlaces);
            var asMb = Math.Round((double)value / SizeOfMb, decimalPlaces);
            var asKb = Math.Round((double)value / SizeOfKb, decimalPlaces);
            string chosenValue = asTb > 1 ? string.Format("{0} TB", asTb)
                : asGb > 1 ? string.Format("{0} GB", asGb)
                : asMb > 1 ? string.Format("{0} MB", asMb)
                : asKb > 1 ? string.Format("{0} KB", asKb)
                : string.Format("{0} bytes", Math.Round((double)value, decimalPlaces));
            return chosenValue;
        }

        //From https://github.com/Ploaj/SSBHLib/blob/e37b0d83cd088090f7802be19b1d05ec998f2b6a/CrossMod/Tools/CrossMath.cs#L42
        //Seems to give good results
        public static Vector3 ToEulerAngles(double X, double Y, double Z, double W)
        {
            return ToEulerAngles(new Quaternion((float)X, (float)Y, (float)Z, (float)W));
        }

        public static Vector3 ToEulerAngles(float X, float Y, float Z, float W)
        {
            return ToEulerAngles(new Quaternion(X, Y, Z, W));
        }

        public static Vector3 ToEulerAngles(Quaternion q)
        {
            Matrix4 mat = Matrix4.CreateFromQuaternion(q);
            float x, y, z;
            y = (float)Math.Asin(Clamp(mat.M13, -1, 1));

            if (Math.Abs(mat.M13) < 0.99999)
            {
                x = (float)Math.Atan2(-mat.M23, mat.M33);
                z = (float)Math.Atan2(-mat.M12, mat.M11);
            }
            else
            {
                x = (float)Math.Atan2(mat.M32, mat.M22);
                z = 0;
            }
            return new Vector3(x, y, z) * -1;
        }

        public static Quaternion FromEulerAngles(Vector3 rotation)
        {
            Quaternion xRotation = Quaternion.FromAxisAngle(Vector3.UnitX, rotation.X);
            Quaternion yRotation = Quaternion.FromAxisAngle(Vector3.UnitY, rotation.Y);
            Quaternion zRotation = Quaternion.FromAxisAngle(Vector3.UnitZ, rotation.Z);
            Quaternion q = (zRotation * yRotation * xRotation);

            if (q.W < 0)
                q *= -1;

            return q;
        }

        public static Matrix4 RotationFromTo(Vector3 start, Vector3 end)
        {
            var axis = Vector3.Cross(start, end).Normalized();
            var angle = (float)Math.Acos(Vector3.Dot(start, end));
            return Matrix4.CreateFromAxisAngle(axis, angle);
        }

        public static Quaternion QuatRotationFromTo(Vector3 start, Vector3 end)
        {
            var axis = Vector3.Cross(start, end).Normalized();
            var angle = (float)Math.Acos(Vector3.Dot(start, end));
            return Quaternion.FromAxisAngle(axis, angle);
        }

        public static Vector3 GetEulerAngle(Matrix4 m)
        {
            float pitch, yaw, roll;         // 3 angles
            yaw = Rad2Deg * (float)Math.Asin(GetValue(m, 8));
            if (GetValue(m, 10) < 0)
            {
                if (yaw >= 0) yaw = 180.0f - yaw;
                else yaw = -180.0f - yaw;
            }

            // find roll (around z-axis) and pitch (around x-axis)
            // if forward vector is (1,0,0) or (-1,0,0), then m[0]=m[4]=m[9]=m[10]=0
            if (m.M11 > -double.Epsilon && m.M11 < double.Epsilon)
            {
                roll = 0;  //@@ assume roll=0
                pitch = Rad2Deg * (float)Math.Atan2(GetValue(m, 1), GetValue(m, 5));
            }
            else
            {
                roll = Rad2Deg * (float)Math.Atan2(-GetValue(m, 4), GetValue(m, 0));
                pitch = Rad2Deg * (float)Math.Atan2(-GetValue(m, 9), GetValue(m, 10));
            }
            return new Vector3(pitch, yaw, roll) * Deg2Rad;
        }

        static float GetValue(Matrix4 mat, int index)
        {
            switch (index)
            {
                case 0: return mat.M11;
                case 1: return mat.M12;
                case 2: return mat.M13;
                case 3: return mat.M14;
                case 4: return mat.M21;
                case 5: return mat.M22;
                case 6: return mat.M23;
                case 7: return mat.M24;
                case 8: return mat.M31;
                case 9: return mat.M32;
                case 10: return mat.M33;
                case 11: return mat.M34;
                case 12: return mat.M41;
                case 13: return mat.M42;
                case 14: return mat.M43;
                case 15: return mat.M44;
                default:
                    throw new Exception("Invalid index for 4x4 matrix!");
            }
        }

        public static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
    }
}
