using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.IO
{
    class Bit
    {
        //From https://github.com/shibbo/flyte/blob/337383c01c50dff155e4b4e170d248118db0c0aa/flyte/utils/Bit.cs
        public static uint ExtractBits(uint val, int numBits, int startBit)
        {
            uint mask = 0;

            for (int i = startBit; i < startBit + numBits; i++)
                mask |= (0x80000000 >> i);

            return (val & mask) >> (32 - (startBit + numBits));
        }
    }
}
