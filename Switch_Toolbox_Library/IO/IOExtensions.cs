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

        public static string RemoveIllegaleCharacters(this string str)
        {
            string illegal = "\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(str)));
            illegal = r.Replace(illegal, "");
            return illegal;
        }
    }
}
