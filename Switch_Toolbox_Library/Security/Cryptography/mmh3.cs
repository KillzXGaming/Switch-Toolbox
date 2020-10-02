using System.IO;
using System.Text;

namespace Toolbox.Library.Security.Cryptography
{
    public static class MurMurHash3
    {
        private const uint Seed = 0;

        public static uint Hash(string s)
        {
            var ss = new MemoryStream(Encoding.UTF8.GetBytes(s));
            return Hash(ss);
        }

        public static uint Hash(Stream stream)
        {
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            var h1 = Seed;
            uint streamLength = 0;

            using (var reader = new BinaryReader(stream))
            {
                var chunk = reader.ReadBytes(4);
                while (chunk.Length > 0)
                {
                    streamLength += (uint)chunk.Length;
                    uint k1;
                    switch (chunk.Length)
                    {
                        case 4:
                            k1 = (uint)(chunk[0] | chunk[1] << 8 | chunk[2] << 16 | chunk[3] << 24);
                            k1 *= c1;
                            k1 = Rot(k1, 15);
                            k1 *= c2;
                            h1 ^= k1;
                            h1 = Rot(h1, 13);
                            h1 = h1 * 5 + 0xe6546b64;
                            break;
                        case 3:
                            k1 = (uint) (chunk[0] | chunk[1] << 8 | chunk[2] << 16);
                            k1 *= c1;
                            k1 = Rot(k1, 15);
                            k1 *= c2;
                            h1 ^= k1;
                            break;
                        case 2:
                            k1 = (uint) (chunk[0] | chunk[1] << 8);
                            k1 *= c1;
                            k1 = Rot(k1, 15);
                            k1 *= c2;
                            h1 ^= k1;
                            break;
                        case 1:
                            k1 = (chunk[0]);
                            k1 *= c1;
                            k1 = Rot(k1, 15);
                            k1 *= c2;
                            h1 ^= k1;
                            break;
                    }
                    chunk = reader.ReadBytes(4);
                }
            }

            h1 ^= streamLength;
            h1 = Mix(h1);

            return h1;
        }

        private static uint Rot(uint x, byte r)
        {
            return (x << r) | (x >> (32 - r));
        }

        private static uint Mix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }
    }
}
