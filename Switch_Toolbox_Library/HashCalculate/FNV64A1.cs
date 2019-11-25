using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public class FNV64A1
    {
        public static ulong Calculate(string text) {
            return Calculate(Encoding.Default.GetBytes(text));
        }

        //https://github.com/kwsch/pkNX/blob/e8cd9cc30feb0a6f6e8cc1f6f6e04288aef0a8cb/pkNX.Containers/Misc/FnvHash.cs
        public static ulong Calculate(byte[] bytes)
        {
            const ulong fnv64Offset = 0xCBF29CE484222645;
            const ulong fnv64Prime = 0x00000100000001b3;
            ulong hash = fnv64Offset;

            for (var i = 0; i < bytes.Length; i++)
            {
                hash = hash ^ bytes[i];
                hash *= fnv64Prime;
            }

            return hash;
        }
    }
}
