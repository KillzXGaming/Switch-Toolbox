using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Switch_Toolbox.Library.IO
{
    public static class IOExtensions
    {
        public static uint Reverse(this uint x)
        {
            // swap adjacent 16-bit blocks
            x = (x >> 16) | (x << 16);
            // swap adjacent 8-bit blocks
            return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
        }

        //https://stackoverflow.com/questions/2230826/remove-invalid-disallowed-bad-characters-from-filename-or-directory-folder/12800424#12800424
        public static string RemoveIllegaleCharacters(this string str)
        {
            return string.Join("_", str.Split(Path.GetInvalidFileNameChars()));
        }

    }
}
