using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StringExtension
    {
        public static string Repeat(this string value, int count)
        {
            return new StringBuilder(value.Length * count).Insert(0, value, count).ToString();
        }

        public static unsafe string TruncateAndFill(this string s, int length, char fillChar)
        {
            char* buffer = stackalloc char[length];

            int i;
            int min = Math.Min(s.Length, length);
            for (i = 0; i < min; i++)
                buffer[i] = s[i];

            while (i < length)
                buffer[i++] = fillChar;

            return new string(buffer, 0, length);
        }

        public static String ToFixedString(this String value, int length, char appendChar = ' ')
        {
            int currlen = value.Length;
            int needed = length == currlen ? 0 : (length - currlen);

            return needed == 0 ? value :
                (needed > 0 ? value + new string(' ', needed) :
                    new string(new string(value.ToCharArray().Reverse().ToArray()).
                        Substring(needed * -1, value.Length - (needed * -1)).ToCharArray().Reverse().ToArray()));
        }
    }
}
