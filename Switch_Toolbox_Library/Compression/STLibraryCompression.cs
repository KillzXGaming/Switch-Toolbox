using Syroot.BinaryData;
using System.IO;
using System.IO.Compression;
using K4os.Compression.LZ4.Streams;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.InteropServices;

namespace Toolbox.Library.IO
{
    public class STLibraryCompression
    {
        public static byte[] CompressFile(byte[] data, IFileFormat format)
        {
            int Alignment = 0;

            if (format.IFileInfo != null)
                Alignment = format.IFileInfo.Alignment;

            switch (format.IFileInfo.CompressionType)
            {
                case CompressionType.Yaz0:
                    return EveryFileExplorer.YAZ0.Compress(data, 3, (uint)Alignment);
                case CompressionType.Zstb:
                    return ZSTD.Compress(data);
                case CompressionType.Zlib:
                    return ZLIB.Compress(data);
                case CompressionType.None:
                    return data;
                default:
                    return data;
            }
        }

        public class ZSTD
        {
            public static byte[] Decompress(byte[] b)
            {
                using (var decompressor = new ZstdNet.Decompressor())
                {
                    return decompressor.Unwrap(b);
                }
            }
            public static byte[] Decompress(byte[] b, int MaxDecompressedSize)
            {
                using (var decompressor = new ZstdNet.Decompressor())
                {
                    return decompressor.Unwrap(b, MaxDecompressedSize);
                }
            }
            public static byte[] Compress(byte[] b)
            {
                using (var compressor = new ZstdNet.Compressor())
                {
                    return compressor.Wrap(b);
                }
            }

        }

        public class ZLIB
        {
            public static byte[] Decompress(byte[] b)
            {
                using (var br = new FileReader(new MemoryStream(b), true))
                {
                    if (br.ReadString(4) == "ZCMP")
                    {
                        return DecompressZCMP(b);
                    }
                    else
                    {
                        var ms = new System.IO.MemoryStream();
                        br.BaseStream.Position = 2;
                        using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes((int)br.BaseStream.Length - 6)), CompressionMode.Decompress))
                            ds.CopyTo(ms);
                        return ms.ToArray();
                    }
                }
            }

            public static Byte[] DecompressZCMP(byte[] b)
            {
                Console.WriteLine("DecompressZCMP");

                using (var br = new FileReader(new MemoryStream(b), true))
                {
                    var ms = new System.IO.MemoryStream();
                    br.BaseStream.Position = 130;
                    using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes((int)br.BaseStream.Length - 80)), CompressionMode.Decompress))
                        ds.CopyTo(ms);
                    return ms.ToArray();
                }
            }

            public static byte[] Compress(byte[] b, uint Position = 0)
            {
                var output = new MemoryStream();
                output.Write(new byte[] { 0x78, 0xDA }, 0, 2);

                using (var zipStream = new DeflateStream(output, CompressionMode.Compress, true))
                    zipStream.Write(b, 0, b.Length);

                //Add this as it weirdly prevents the data getting corrupted
                //From https://github.com/IcySon55/Kuriimu/blob/f670c2719affc1eaef8b4c40e40985881247acc7/src/Kontract/Compression/ZLib.cs
                var adler = b.Aggregate(Tuple.Create(1, 0), (x, n) => Tuple.Create((x.Item1 + n) % 65521, (x.Item1 + x.Item2 + n) % 65521));
                output.Write(new[] { (byte)(adler.Item2 >> 8), (byte)adler.Item2, (byte)(adler.Item1 >> 8), (byte)adler.Item1 }, 0, 4);
                return output.ToArray();
            }

            public static void CopyStream(System.IO.Stream input, System.IO.Stream output)
            {
                byte[] buffer = new byte[2000];
                int len;
                while ((len = input.Read(buffer, 0, 2000)) > 0)
                {
                    output.Write(buffer, 0, len);
                }
                output.Flush();
            }
        }

        public class BPE
        {
            public static unsafe byte[] Decompress(byte[] input, uint decompressedLength)
            {
                fixed (byte* outputPtr = new byte[decompressedLength])
                {
                    fixed (byte* inputPtr = input)
                    {
                        Decompress(outputPtr, inputPtr, decompressedLength);
                    }

                    byte[] decomp = new byte[decompressedLength];
                    Marshal.Copy((IntPtr)outputPtr, decomp, 0, decomp.Length);
                    return decomp;
                }
            }

            public static unsafe void Decompress(byte* output, byte* input, uint decompressedLength)
            {

            }
        }

        //Mario Tennis Aces Custom compression
        public class MTA_CUSTOM
        {
            [DllImport("Lib/LibTennis.dll", CallingConvention = CallingConvention.Cdecl)]
            static extern void DecompressBuffer(IntPtr output, IntPtr input, uint len);

            public unsafe byte[] Decompress(byte[] input, uint decompressedLength)
            {
                fixed (byte* outputPtr = new byte[decompressedLength])
                {
                    fixed (byte* inputPtr = input)
                    {
                        DecompressBuffer((IntPtr)outputPtr, (IntPtr)inputPtr, decompressedLength);

                     //   Decompress(outputPtr, inputPtr, decompressedLength);
                    }

                    byte[] decomp = new byte[decompressedLength];
                    Marshal.Copy((IntPtr)outputPtr, decomp, 0, decomp.Length);
                    return decomp;
                }
            }

            //Thanks Simon. Code ported from
            //https://github.com/simontime/MarioTennisAces0x50Decompressor/blob/master/decompress.c
            public unsafe void Decompress(byte* output, byte* input, uint decompressedLength)
            {
                uint pos = 8;
                byte* end = input + decompressedLength;
                byte* data = input + pos;

                Console.WriteLine($"decompressedLength " + decompressedLength);
                Console.WriteLine($"pos " + pos);

                if (pos < decompressedLength)
                {
                    uint flag;

                    while (true)
                    {
                        flag = 0xFF000000 * data[0];
                        if (flag != 0)
                            break;

                        data++;

                        for (int i = 0; i < 8; i++)
                            *output++ = *data++;

                        var checkFinished = CheckFinished(data, end);
                        data = checkFinished.data;
                        end = checkFinished.end;
                    }

                    flag |= 0x800000;

                    data++;

                    Console.WriteLine($"flag  " + flag);

                    //IterateFlag
                    while ((flag & 0x80000000) == 0)
                    {
                        flag <<= 1;
                        *output++ = *data++;
                    }

                    Console.WriteLine($"Pass 3 ");

                    while (true)
                    {
                        flag <<= 1;

                        if (flag == 0)
                        {
                            var checkFinished2 = CheckFinished(data, end);
                            data = checkFinished2.data;
                            end = checkFinished2.end;
                        }

                        int op_ofs = (data[0] >> 4) | (data[1] << 4);
                        int op_len = data[0] & 0xF;

                        if (op_ofs == 0)
                            return;

                        byte* chunk = output - op_ofs;
                        if (op_len > 1)
                            data += 2;
                        else
                        {
                            int op_len_ext = data[2] + (op_len | 0x10);

                            if (op_len == 1)
                            {
                                int add_len = (data[3] << 8) | 0xFF;
                                data += 4;

                                op_len = op_len_ext + add_len;

                                if (op_ofs >= 2)
                                {
                                    var loop1Data = Loop1(flag, op_len, chunk, data, output);
                                    flag = loop1Data.flag;
                                    op_len = loop1Data.op_len;
                                    chunk = loop1Data.chunk;
                                    data = loop1Data.data;
                                    output = loop1Data.output;
                                }
                            }
                            else
                            {
                                data += 3;
                                op_len = op_len_ext;
                                if (op_ofs >= 2)
                                {
                                    var loop1Data = Loop1(flag, op_len, chunk, data, output);
                                    flag = loop1Data.flag;
                                    op_len = loop1Data.op_len;
                                    chunk = loop1Data.chunk;
                                    data = loop1Data.data;
                                    output = loop1Data.output;
                                }
                            }
                        }

                        var loop2Data2 = Loop2(flag, op_len, data, output, chunk);
                        flag = loop2Data2.flag;
                        op_len = loop2Data2.op_len;
                        chunk = loop2Data2.chunk;
                        data = loop2Data2.data;
                        output = loop2Data2.output;
                    }
                }

                var endOp = EndOperation(data, end);
                data = endOp.data;
                end = endOp.end;
            }

            private unsafe class Data
            {
                public uint flag;
                public int op_len;
                public byte* chunk;
                public byte* data;
                public byte* output;
                public byte* end;
            }

            unsafe Data Loop1(uint flag, int op_len, byte* chunk, byte* data, byte* output)
            {
                if ((((byte)*chunk ^ (byte)*output) & 1) == 0)
                {
                    if (((byte)*chunk & 1) != 0)
                    {
                        *output++ = *chunk++;
                        op_len--;
                    }

                    uint op_len_sub = (uint)op_len - 2;

                    if (op_len >= 2)
                    {
                        int masked_len = (((int)op_len_sub >> 1) + 1) & 7;

                        byte* out_ptr = output;
                        byte* chunk_ptr = chunk;

                        if (masked_len != 0)
                        {
                            while (masked_len-- != 0)
                            {
                                *out_ptr++ = *chunk_ptr++;
                                *out_ptr++ = *chunk_ptr++;
                                op_len -= 2;
                            }
                        }

                        uint masked_ext_len = op_len_sub & 0xFFFFFFFE;

                        if (op_len_sub >= 0xE)
                        {
                            do
                            {
                                for (int i = 0; i < 0x10; i++)
                                    *out_ptr++ = *chunk_ptr++;

                                op_len -= 0x10;
                            }
                            while (op_len > 1);
                        }

                        output += masked_ext_len + 2;
                        op_len = (int)op_len_sub - (int)masked_ext_len;
                        chunk += masked_ext_len + 2;
                    }

                    if (op_len == 0)
                    {
                        if ((flag & 0x80000000) == 0)
                        {
                            flag <<= 1;
                            *output++ = *data++;
                        }
                    }
                }

                return Loop2(flag, op_len, data, output, chunk);
            }

            unsafe Data Loop2(uint flag, int op_len, byte* data, byte* output, byte* chunk)
            {
                int masked_len = op_len & 7;
                byte* out_ptr = output;
                byte* chunk_ptr = chunk;

                if (masked_len != 0)
                {
                    while (masked_len-- != 0)
                        *out_ptr++ = *chunk_ptr++;
                }

                if (op_len - 1 >= 7)
                {
                    do
                    {
                        for (int i = 0; i < 8; i++)
                            *out_ptr++ = *chunk_ptr++;
                    }
                    while (chunk_ptr != chunk + op_len);
                }

                output += op_len;

                if ((flag & 0x80000000) == 0)
                {
                    flag <<= 1;
                    *output++ = *data++;
                }

                return new Data()
                {
                    flag = flag,
                    op_len = op_len,
                    data = data,
                    output = output,
                    chunk = chunk,
                };
            }

            unsafe Data CheckFinished(byte* data, byte* end)
            {
                if (data >= end)
                    return EndOperation(data, end);

                return new Data()
                {
                    data = data,
                    end = end,
                };
            }

            unsafe Data EndOperation(byte* data, byte* end)
            {
                byte* ext = end + 0x20;
                if (data < ext)
                    do
                        *end-- = *--ext;
                    while (data < ext);

                return new Data()
                {
                    data = data,
                    end = end,
                };
            }
        }

        public class LZSS
        {
            static class LzssParameters
            {
                /// <summary>Size of the ring buffer.</summary>
                public const int N = 4096;
                /// <summary>Maximum match length for position coding. (0x0F + THRESHOLD).</summary>
                public const int F = 18;
                /// <summary>Minimum match length for position coding.</summary>
                public const int THRESHOLD = 3;
                /// <summary>Index for root of binary search trees.</summary>
                public const int NIL = N;
                /// <summary>Character used to fill the ring buffer initially.</summary>
                //private const ubyte BUFF_INIT = ' ';
                public const byte BUFF_INIT = 0; // Changed for F-Zero GX
            }

            public static byte[] Decompress(byte[] input, uint decompressedLength)
            {
                List<byte> output = new List<byte>();
                byte[] ringBuf = new byte[LzssParameters.N];
                int inputPos = 0, ringBufPos = LzssParameters.N - LzssParameters.F;

                ushort flags = 0;

                // Clear ringBuf with a character that will appear often
                for (int i = 0; i < LzssParameters.N - LzssParameters.F; i++)
                    ringBuf[i] = LzssParameters.BUFF_INIT;

                while (inputPos < input.Length)
                {
                    // Use 16 bits cleverly to count to 8.
                    // (After 8 shifts, the high bits will be cleared).
                    if ((flags & 0xFF00) == 0)
                        flags = (ushort)(input[inputPos++] | 0x8000);

                    if ((flags & 1) == 1)
                    {
                        // Copy data literally from input
                        byte c = input[inputPos++];
                        output.Add(c);
                        ringBuf[ringBufPos++ % LzssParameters.N] = c;
                    }
                    else
                    {
                        // Copy data from the ring buffer (previous data).
                        int index = ((input[inputPos + 1] & 0xF0) << 4) | input[inputPos];
                        int count = (input[inputPos + 1] & 0x0F) + LzssParameters.THRESHOLD;
                        inputPos += 2;

                        for (int i = 0; i < count; i++)
                        {
                            byte c = ringBuf[(index + i) % LzssParameters.N];
                            output.Add(c);
                            ringBuf[ringBufPos++ % LzssParameters.N] = c;
                        }
                    }

                    // Advance flags & count bits
                    flags >>= 1;
                }

                return output.ToArray();
            }
        }

        public class LZ77
        {
            /// <summary>
            /// Decompresses LZ77-compressed data from the given input stream.
            /// </summary>
            /// <param name="input">The input stream to read from.</param>
            /// <returns>The decompressed data.</returns>
            public static byte[] Decompress(byte[] input)
            {
                BinaryReader reader = new BinaryReader(new MemoryStream(input));

                // Check LZ77 type.
                //   if (reader.ReadByte() != 0x10)
                //     throw new System.Exception("Input stream does not contain LZ77-compressed data.");

                // Read the size.
                int size = reader.ReadUInt16() | (reader.ReadByte() << 16);

                // Create output stream.
                MemoryStream output = new MemoryStream(size);

                // Begin decompression.
                while (output.Length < size)
                {
                    // Load flags for the next 8 blocks.
                    int flagByte = reader.ReadByte();

                    // Process the next 8 blocks.
                    for (int i = 0; i < 8; i++)
                    {
                        // Check if the block is compressed.
                        if ((flagByte & (0x80 >> i)) == 0)
                        {
                            // Uncompressed block; copy single byte.
                            output.WriteByte(reader.ReadByte());
                        }
                        else
                        {
                            // Compressed block; read block.
                            ushort block = reader.ReadUInt16();
                            // Get byte count.
                            int count = ((block >> 4) & 0xF) + 3;
                            // Get displacement.
                            int disp = ((block & 0xF) << 8) | ((block >> 8) & 0xFF);

                            // Save current position and copying position.
                            long outPos = output.Position;
                            long copyPos = output.Position - disp - 1;

                            // Copy all bytes.
                            for (int j = 0; j < count; j++)
                            {
                                // Read byte to be copied.
                                output.Position = copyPos++;
                                byte b = (byte)output.ReadByte();

                                // Write byte to be copied.
                                output.Position = outPos++;
                                output.WriteByte(b);
                            }
                        }

                        // If all data has been decompressed, stop.
                        if (output.Length >= size)
                        {
                            break;
                        }
                    }
                }

                output.Position = 0;
                return output.ToArray();
            }
        }

        public class GZIP
        {
            public static byte[] Decompress(byte[] b)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    using (GZipStream source = new GZipStream(new MemoryStream(b), CompressionMode.Decompress, false))
                    {
                        source.CopyTo(mem);
                    }
                    return mem.ToArray();
                }
            }

            public static byte[] Compress(byte[] b)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    using (GZipStream gzip = new GZipStream(mem,
                        CompressionMode.Compress))
                    {
                        gzip.Write(b, 0, b.Length);
                    }
                    return mem.ToArray();
                }
            }
        }

        public class Type_LZ4F
        {
            public static byte[] Decompress(byte[] data)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    using (var source = LZ4Stream.Decode(new MemoryStream(data)))
                    {
                        source.CopyTo(mem);
                    }
                    return mem.ToArray();
                }
            }
            public static byte[] Compress(byte[] data)
            {
                var stream = new MemoryStream();
                using (var writer = new FileWriter(stream))
                {
                    writer.Write(data.Length);
                    byte[] buffer = LZ4.Frame.LZ4Frame.Compress(new MemoryStream(data),
                        LZ4.Frame.LZ4MaxBlockSize.MB1, true, true, false, true, false);

                    writer.Write(buffer, 0, buffer.Length);
                }
                return stream.ToArray();
            }
        }
        public class Type_LZ4
        {
            public static byte[] Decompress(byte[] data, int inputOffset, int InputLength, int decompressedSize)
            {
                return LZ4.LZ4Codec.Decode(data, inputOffset, InputLength, decompressedSize);
            }
            public static byte[] Decompress(byte[] data)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    using (var source = LZ4Stream.Decode(new MemoryStream(data)))
                    {
                        source.CopyTo(mem);
                        mem.Write(data, 0, data.Length);
                    }
                    return mem.ToArray();
                }
            }
            public static byte[] Compress(byte[] data, int inputOffset = 0)
            {
                return LZ4.LZ4Codec.Encode(data, inputOffset, data.Length);
            }
        }
    }

}
