using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class LZ77_WII
    {
        public static byte[] Decompress(byte[] input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return input;
        }
    }
}
