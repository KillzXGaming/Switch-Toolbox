using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
