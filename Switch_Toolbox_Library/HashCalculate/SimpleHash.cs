using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.Hash
{
    //https://github.com/IcySon55/Kuriimu/blob/master/src/Cetera/Hash/SimpleHash.cs
    public class SimpleHash
    {
        public static uint Create(string input, uint magic, uint hashCount)
        {
            return Create(input, magic) % hashCount;
        }

        public static uint Create(string input, uint magic)
        {
            return input.Aggregate(0u, (hash, c) => hash * magic + c);
        }
    }
}
