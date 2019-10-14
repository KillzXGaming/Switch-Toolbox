using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.IO
{
    public class Bit
    {
        //From https://github.com/AustinHasten/BenzinNX/blob/06a5a600748696beac3555cd48a5837b22b0b026/include/ReadTypes.py#L33
        public static uint ExtractBits(uint val, int numBits, int startBit)
        {
            uint mask = 0;
            for (int i = startBit; i < startBit + numBits; i++)
                mask |= (0x80000000 >> i);

            return (val & mask) >> (32 - (startBit + numBits));
        }

        public static uint BitInsert(uint value, int newValue, int numBits, int startBit)
        {
            uint mask = 0;
            for (int i = startBit; i < startBit + numBits; i++)
                mask |= (0x80000000 >> i);

            value &= mask == 0 ? (uint)1 : (uint)0;
            value |= (uint)(newValue << (32 - (startBit + numBits))) & mask;
            return value;
        }
    }
}
