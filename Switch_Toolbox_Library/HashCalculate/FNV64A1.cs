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

        public static ulong CalculateSuffix(string text) {
            return CalculateSuffix(Encoding.Default.GetBytes(text));
        }

        public static ulong CalculateSuffix(byte[] bytes)
        {
            const ulong fnv64Basis = 0x222645;
            const ulong fnv64Prime = 0x0001b3;
            const ulong mask = 0xffffff;

            ulong hash = fnv64Basis & mask;
            for (var i = 0; i < bytes.Length; i++)
            {
                hash = hash ^ bytes[i];
                hash *= fnv64Prime;
                hash = hash & mask;
            }

            return hash;
        }

        //https://gist.github.com/rasmuskl/3786618
        public static ulong Calculate(byte[] bytes)
        {
            const ulong fnv64Offset = 14695981039346656037;
            const ulong fnv64Prime = 0x100000001b3;
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
