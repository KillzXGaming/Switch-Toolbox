using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Switch_Toolbox.Library
{
    public static class STMath
    {
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
            string chosenValue = asTb > 1 ? string.Format("{0}Tb", asTb)
                : asGb > 1 ? string.Format("{0}Gb", asGb)
                : asMb > 1 ? string.Format("{0}Mb", asMb)
                : asKb > 1 ? string.Format("{0}Kb", asKb)
                : string.Format("{0}B", Math.Round((double)value, decimalPlaces));
            return chosenValue;
        }

        //From https://github.com/Ploaj/SSBHLib/blob/e37b0d83cd088090f7802be19b1d05ec998f2b6a/CrossMod/Tools/CrossMath.cs#L42
        //Seems to give good results
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

        public static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
    }
}
