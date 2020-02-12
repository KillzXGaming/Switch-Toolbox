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
        private readonly static int N = 4096;
        private readonly static ushort[] textBuffer = new ushort[N + 17];
        private readonly static int F = 18;
        private static readonly int threshold = 2;

        public static byte[] Decompress(byte[] input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            using (var reader = new FileReader(new MemoryStream(input), true))
            {
                reader.SetByteOrder(true);
                reader.ReadSignature(4, "LZ77");

                reader.Position = 0;

                int i, j, k, r, c, z;
                uint flags, decompressedSize, currentSize = 0;

                byte[] temp = new byte[8];
                reader.Read(temp, 0, 8);

                if (temp[4] != 0x10)
                { reader.Dispose(); throw new Exception("Unsupported Compression Type!"); }

                decompressedSize = (BitConverter.ToUInt32(temp, 4)) >> 8;

                for (i = 0; i < N - F; i++) textBuffer[i] = 0xdf;
                r = N - F; flags = 7; z = 7;

                MemoryStream outFile = new MemoryStream();
                while (outFile.Length < decompressedSize)
                {
                    flags <<= 1;
                    z++;

                    if (z == 8)
                    {
                        if ((c = reader.ReadByte()) == -1) break;

                        flags = (uint)c;
                        z = 0;
                    }

                    if ((flags & 0x80) == 0)
                    {
                        if ((c = reader.ReadByte()) == reader.Length - 1) break;
                        if (currentSize < decompressedSize) outFile.WriteByte((byte)c);

                        textBuffer[r++] = (byte)c;
                        r &= (N - 1);
                        currentSize++;
                    }
                    else
                    {
                        if ((i = reader.ReadByte()) == -1) break;
                        if ((j = reader.ReadByte()) == -1) break;

                        j = j | ((i << 8) & 0xf00);
                        i = ((i >> 4) & 0x0f) + threshold;
                        for (k = 0; k <= i; k++)
                        {
                            c = textBuffer[(r - j - 1) & (N - 1)];
                            if (currentSize < decompressedSize) outFile.WriteByte((byte)c); textBuffer[r++] = (byte)c; r &= (N - 1); currentSize++;
                        }
                    }
                }
                return outFile.ToArray();
            }
            return input;
        }
    }
}
