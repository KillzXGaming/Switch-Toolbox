using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace System
{
    public static class StringExtension
    {
        //https://stackoverflow.com/questions/5617320/given-full-path-check-if-path-is-subdirectory-of-some-other-path-or-otherwise/31941159
        /// <summary>
        /// Returns true if <paramref name="path"/> starts with the path <paramref name="baseDirPath"/>.
        /// The comparison is case-insensitive, handles / and \ slashes as folder separators and
        /// only matches if the base dir folder name is matched exactly ("c:\foobar\file.txt" is not a sub path of "c:\foo").
        /// </summary>
        public static bool IsSubPathOf(this string path, string baseDirPath)
        {
            string normalizedPath = Path.GetFullPath(path.Replace('/', '\\')
                .WithEnding("\\"));

            string normalizedBaseDirPath = Path.GetFullPath(baseDirPath.Replace('/', '\\')
                .WithEnding("\\"));

            return normalizedPath.StartsWith(normalizedBaseDirPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns <paramref name="str"/> with the minimal concatenation of <paramref name="ending"/> (starting from end) that
        /// results in satisfying .EndsWith(ending).
        /// </summary>
        /// <example>"hel".WithEnding("llo") returns "hello", which is the result of "hel" + "lo".</example>
        public static string WithEnding( this string str, string ending)
        {
            if (str == null)
                return ending;

            string result = str;

            // Right() is 1-indexed, so include these cases
            // * Append no characters
            // * Append up to N characters, where N is ending length
            for (int i = 0; i <= ending.Length; i++)
            {
                string tmp = result + ending.Right(i);
                if (tmp.EndsWith(ending))
                    return tmp;
            }

            return result;
        }

        /// <summary>Gets the rightmost <paramref name="length" /> characters from a string.</summary>
        /// <param name="value">The string to retrieve the substring from.</param>
        /// <param name="length">The number of characters to retrieve.</param>
        /// <returns>The substring.</returns>
        public static string Right( this string value, int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", length, "Length is less than zero");
            }

            return (length < value.Length) ? value.Substring(value.Length - length) : value;
        }

        public static string Strip(this string value)
        {
           return value.Replace(" ", string.Empty);
        }

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
