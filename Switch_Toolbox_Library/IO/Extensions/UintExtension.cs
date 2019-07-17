using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace System
{
    public static class UintExtension
    {
        public static uint SwapBytes(this uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

    }
}
