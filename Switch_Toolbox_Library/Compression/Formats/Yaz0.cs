using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using System.Runtime.InteropServices;

namespace Toolbox.Library
{
    public class Yaz0 : ICompressionFormat
    {
        public int Alignment = 0;

        public string[] Description { get; set; } = new string[] { "Yaz0" };
        public string[] Extension { get; set; } = new string[] { "*.yaz0", "*.szs", };

        public override string ToString() { return "Yaz0"; }

        public bool Identify(Stream stream, string fileName)
        {
            using (var reader = new FileReader(stream, true))
            {
                return reader.CheckSignature(4, "Yaz0");
            }
        }

        public bool CanCompress { get; } = true;

        public Stream Decompress(Stream stream)
        {
            var comp = stream.ToBytes();
            UInt32 decompressedSize = (uint)(comp[4] << 24 | comp[5] << 16 | comp[6] << 8 | comp[7]);
            var data = Decompress(comp);
           // var data = EveryFileExplorer.YAZ0.Decompress(comp);
          //  System.IO.File.WriteAllBytes("testYaz0.dec", data);
            return new MemoryStream(data);
        }

        [DllImport("Lib/Yaz0.dll")]
        static unsafe extern byte* decompress(byte* src, uint src_len, uint* dest_len);

        [DllImport("Lib/Yaz0.dll")]
        static unsafe extern byte* compress(byte* src, uint src_len, uint* dest_len, byte opt_compr);

        [DllImport("Lib/Yaz0.dll")]
        static unsafe extern  void freePtr(void* ptr);

        private unsafe byte[] Decompress(byte[] data)
        {
            return EveryFileExplorer.YAZ0.Decompress(data);

            uint src_len = (uint)data.Length;

            uint dest_len;
            fixed (byte* inputPtr = data)
            {
                byte* outputPtr = decompress(inputPtr, src_len, &dest_len);
                byte[] decomp = new byte[dest_len];
                Marshal.Copy((IntPtr)outputPtr, decomp, 0, (int)dest_len);
                freePtr(outputPtr);
                return decomp;
            }
        }

        public Stream Compress(Stream stream)
        {
            return new MemoryStream(EveryFileExplorer.YAZ0.Compress(
             stream.ToArray(), Runtime.Yaz0CompressionLevel, (uint)Alignment));

            var mem = new MemoryStream();
            using (var writer = new FileWriter(mem, true))
            {
                writer.SetByteOrder(true);
                writer.WriteSignature("Yaz0");
                writer.Write((uint)stream.Length);
                writer.Write((uint)Alignment);
                writer.Write(0);
                writer.Write(Compress(stream.ToArray(), (byte)Runtime.Yaz0CompressionLevel));
            }
            return mem;
        }

        public static unsafe byte[] Compress(byte[] src, byte opt_compr)
        {
            uint src_len = (uint)src.Length;

            uint dest_len;
            fixed (byte* inputPtr = src)
            {
                byte* outputPtr = compress(inputPtr, src_len, &dest_len, opt_compr);
                byte[] comp = new byte[dest_len];
                Marshal.Copy((IntPtr)outputPtr, comp, 0, (int)dest_len);
                freePtr(outputPtr);
                return comp;
            }

            Console.WriteLine($"opt_compr {opt_compr}");

            uint range;
            if (opt_compr == 0)
                range = 0;
            else if (opt_compr < 9)
                range = (uint)(0x10e0 * opt_compr / 9 - 0x0e0);
            else
                range = 0x1000;

            uint pos = 0;
            uint src_end = (uint)src.Length;

            byte[] dest = new byte[src_end + (src_end + 8) / 8];
            uint dest_pos = 0;
            uint code_byte_pos = 0;

            ulong found = 0;
            ulong found_len = 0;
            uint delta;
            int max_len = 0x111;

            while (pos < src_end)
            {
                code_byte_pos = dest_pos;
                dest[dest_pos] = 0; dest_pos++;

                for (int i = 0; i < 8; i++)
                {
                    if (pos >= src_end)
                        break;

                    found_len = 1;

                    if (range != 0)
                    {
                        // Going after speed here.
                        // Dunno if using a tuple is slower, so I don't want to risk it.
                        ulong search = compressionSearch(src, pos, max_len, range, src_end);
                        found = search >> 32;
                        found_len = search & 0xFFFFFFFF;
                    }

                    if (found_len > 2)
                    {
                        delta = (uint)(pos - found - 1);

                        if (found_len < 0x12)
                        {
                            dest[dest_pos] = (byte)(delta >> 8 | (found_len - 2) << 4); dest_pos++;
                            dest[dest_pos] = (byte)(delta & 0xFF); dest_pos++;
                        }
                        else
                        {
                            dest[dest_pos] = (byte)(delta >> 8); dest_pos++;
                            dest[dest_pos] = (byte)(delta & 0xFF); dest_pos++;
                            dest[dest_pos] = (byte)((found_len - 0x12) & 0xFF); dest_pos++;
                        }

                        pos += (uint)found_len;
                    }

                    else
                    {
                        dest[code_byte_pos] |= (byte)(1 << (7 - i));
                        dest[dest_pos] = src[pos]; dest_pos++; pos++;
                    }
                }
            }

            byte[] result = new byte[dest_pos];
            Array.Copy(dest, result, dest_pos);
            return result;
        }

        public static ulong ReadULong(byte[] v, uint i)
        {
            int i1 = v[i] | (v[i + 1] << 8) | (v[i + 2] << 16) | (v[i + 3] << 24);
            int i2 = v[i + 4] | (v[i + 5] << 8) | (v[i + 6] << 16) | (v[i + 7] << 24);
            return (ulong)((uint)i1 | ((long)i2 << 32));
        }

        public static long IndexOfByte(byte[] src, byte v, uint i, uint c)
        {
            // https://stackoverflow.com/a/46678141

            ulong t;
            uint p, pEnd;

            for (p = i; ((long)p & 7) != 0; c--, p++)
                if (c == 0)
                    return -1;
                else if (src[p] == v)
                    return p;

            ulong r = v; r |= r << 8; r |= r << 16; r |= r << 32;

            for (pEnd = (uint)(p + (c & ~7)); p < pEnd; p += 8)
            {
                t = ReadULong(src, p) ^ r;
                t = (t - 0x0101010101010101) & ~t & 0x8080808080808080;
                if (t != 0)
                {
                    t &= (ulong)-(long)t;
                    return p + dbj8[t * 0x07EDD5E59A4E28C2 >> 58];
                }
            }

            for (pEnd += c & 7; p < pEnd; p++)
                if (src[p] == v)
                    return p;

            return -1;
        }

        readonly static sbyte[] dbj8 =
        {
             7, -1, -1, -1, -1,  5, -1, -1, -1,  4, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1,  6, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
            -1, -1, -1,  3, -1, -1, -1, -1, -1, -1,  1, -1,  2,  0, -1, -1,
        };

        public static unsafe long find(byte[] byteArray, byte byteToFind, uint start, uint length)
        {
            return Array.IndexOf(byteArray, byteToFind, (int)start, (int)length);
            return IndexOfByte(byteArray, byteToFind, start, length);
        }

        public static ulong compressionSearch(byte[] src, uint pos, int max_len, uint range, uint src_end)
        {
            ulong found_len = 1;
            ulong found = 0;

            long search;
            uint cmp_end, cmp1, cmp2;
            byte c1;
            uint len;

            if (pos + 2 < src_end)
            {
                search = ((long)pos - (long)range);
                if (search < 0)
                    search = 0;

                cmp_end = (uint)(pos + max_len);
                if (cmp_end > src_end)
                    cmp_end = src_end;

                c1 = src[pos];
                while (search < pos)
                {
                    search = find(src, c1, (uint)search, (uint)(pos - search));
                    if (search < 0)
                        break;

                    cmp1 = (uint)(search + 1);
                    cmp2 = pos + 1;

                    while (cmp2 < cmp_end && src[cmp1] == src[cmp2])
                    {
                        cmp1++; cmp2++;
                    }

                    len = cmp2 - pos;
                    if (found_len < len)
                    {
                        found_len = len;
                        found = (uint)search;
                        if ((long)found_len == max_len)
                            break;
                    }

                    search++;
                }
            }

            return (ulong)((found << 32) | found_len);
        }
    }
}
