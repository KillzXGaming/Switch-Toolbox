using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Toolbox.Library.IO
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Magic
    {
        int value;
        public static implicit operator string(Magic magic) => Encoding.ASCII.GetString(BitConverter.GetBytes(magic.value));
        public static implicit operator Magic(string s) => new Magic { value = BitConverter.ToInt32(Encoding.ASCII.GetBytes(s), 0) };
    }

}
