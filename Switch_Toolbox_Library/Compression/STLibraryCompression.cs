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

            var FileCompression = format.IFileInfo.FileCompression;
            if (FileCompression == null) return data;

            if (FileCompression is Yaz0)
                ((Yaz0)FileCompression).Alignment = Alignment;

            return FileCompression.Compress(new MemoryStream(data)).ToArray();
        }

        public class ZSTD
        {


        }

        public class ZLIB_GZ
        {
            public static bool IsCompressed(Stream stream)
            {
                if (stream.Length < 32) return false;

                using (var reader = new FileReader(stream, true))
                {
                    reader.Position = 0;
                    ushort check = reader.ReadUInt16();
                    reader.ReadUInt16();
                    if (check != 0)
                        reader.SetByteOrder(true);
                    else
                        reader.SetByteOrder(false);

                    uint chunkCount = reader.ReadUInt32();
                    uint decompressedSize = reader.ReadUInt32();
                    if (reader.BaseStream.Length > 8 + (chunkCount * 4) + 128)
                    {
                        uint[] chunkSizes = reader.ReadUInt32s((int)chunkCount);
                        reader.Align(128);

                        //Now search for zlibbed chunks
                        uint size = reader.ReadUInt32();
                        ushort magic = reader.ReadUInt16();

                        reader.Position = 0;
                        if (magic == 0x78da || magic == 0xda78)
                            return true;
                        else
                            return false;
                    }

                    reader.Position = 0;
                }

                return false;
            }

            public static Stream Decompress(Stream stream)
            {
                using (var reader = new FileReader(stream, true))
                {
                    ushort check = reader.ReadUInt16();
                    reader.ReadUInt16();
                    if (check != 0)
                        reader.SetByteOrder(true);
                    else
                        reader.SetByteOrder(false);

                    try
                    {
                        uint chunkCount = reader.ReadUInt32();
                        uint decompressedSize = reader.ReadUInt32();
                        uint[] chunkSizes = reader.ReadUInt32s((int)chunkCount); //Not very sure about this

                        reader.Align(128);

                        List<byte[]> DecompressedChunks = new List<byte[]>();

                        Console.WriteLine($"pos {reader.Position}");

                        //Now search for zlibbed chunks
                        while (!reader.EndOfStream)
                        {
                            uint size = reader.ReadUInt32();

                            long pos = reader.Position;
                            ushort magic = reader.ReadUInt16();

                            ///Check zlib magic
                            if (magic == 0x78da || magic == 0xda78)
                            {
                                var data = STLibraryCompression.ZLIB.Decompress(reader.getSection((uint)pos, size));
                                DecompressedChunks.Add(data);

                                reader.SeekBegin(pos + size); //Seek the compressed size and align it to goto the next chunk
                                reader.Align(128);
                            }
                            else //If the magic check fails, seek back 2. This shouldn't happen, but just incase
                                reader.Seek(-2);
                        }

                        //Return the decompressed stream with all chunks combined
                        return new MemoryStream(Utils.CombineByteArray(DecompressedChunks.ToArray()));
                    }
                    catch
                    {

                    }
                }

                return null;
            }

            public static Stream Compress(Stream stream, bool isBigEndian = true)
            {
                uint decompSize = (uint)stream.Length;
                uint[] section_sizes;
                uint sectionCount = 0;

                var mem = new MemoryStream();
                using (var reader = new FileReader(stream, true))
                using (var writer = new FileWriter(mem, true))
                {
                    writer.SetByteOrder(isBigEndian);

                    if (!(decompSize % 0x10000 != 0))
                        sectionCount = decompSize / 0x10000;
                    else
                        sectionCount = (decompSize / 0x10000) + 1;

                    writer.Write(0x10000);
                    writer.Write(sectionCount);
                    writer.Write(decompSize);
                    writer.Write(new uint[sectionCount]);
                    writer.Align(128);

                    reader.SeekBegin(0);
                    section_sizes = new uint[sectionCount];
                    for (int i = 0; i < sectionCount; i++)
                    {
                        byte[] chunk = ZLIB.Compress(reader.ReadBytes(0x10000));

                        section_sizes[i] = (uint)chunk.Length;

                        writer.Write(chunk.Length);
                        writer.Write(chunk);
                        writer.Align(128);
                    }

                    writer.SeekBegin(12);
                    for (int i = 0; i < sectionCount; i++)
                        writer.Write(section_sizes[i] + 4);
                }
                return mem;
            }
        }

        public class ZLIB
        {
            public static byte[] Decompress(byte[] b, bool hasMagic = true)
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
                        if (hasMagic)
                        {
                            br.Position = 2;
                            using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes((int)br.BaseStream.Length - 6)), CompressionMode.Decompress))
                                ds.CopyTo(ms);
                        }
                        else
                        {
                            using (var ds = new DeflateStream(new MemoryStream(b), CompressionMode.Decompress))
                                ds.CopyTo(ms);
                        }

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
            [DllImport("Lib/LibTennis64.dll", CallingConvention = CallingConvention.Cdecl)]
            static extern void DecompressBuffer(IntPtr output, IntPtr input, uint len);

            public unsafe byte[] Decompress(byte[] input, uint decompressedLength)
            {
                fixed (byte* outputPtr = new byte[decompressedLength])
                {
                    fixed (byte* inputPtr = input)
                    {
                        if (Environment.Is64BitProcess)
                            DecompressBuffer((IntPtr)outputPtr, (IntPtr)inputPtr, decompressedLength);
                        else
                            MarioTennisCmp32.DecompressBuffer((IntPtr)outputPtr, (IntPtr)inputPtr, decompressedLength);
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

                        CheckFinished(ref data, ref end);
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
                            CheckFinished(ref data, ref end);

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
                                    Loop1(ref flag, ref op_len, ref chunk, ref data, ref output);
                            }
                            else
                            {
                                data += 3;
                                op_len = op_len_ext;
                                if (op_ofs >= 2)
                                {
                                    Loop1(ref flag, ref op_len, ref chunk, ref data, ref output);
                                }
                            }
                        }

                        Loop2(ref flag, ref op_len, ref data, ref output, ref chunk);
                    }
                }

                EndOperation(ref data, ref end);
            }

            unsafe void Loop1(ref uint flag, ref int op_len, ref byte* chunk, ref byte* data, ref byte* output)
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

                Loop2(ref flag, ref op_len, ref data, ref output, ref chunk);
            }

            unsafe void Loop2(ref uint flag, ref int op_len, ref byte* data, ref byte* output, ref byte* chunk)
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
            }

            unsafe void CheckFinished(ref byte* data, ref byte* end)
            {
                if (data >= end)
                    EndOperation(ref data, ref end);
            }

            unsafe void EndOperation(ref byte* data, ref byte* end)
            {
                byte* ext = end + 0x20;
                if (data < ext)
                    do
                        *end-- = *--ext;
                    while (data < ext);
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

        public class Type_Oodle
        {
            public static byte[] Decompress(byte[] data, int decompressedSize)
            {
                return Toolbox.Library.Compression.Oodle.Decompress(data, decompressedSize);
            }

            public static byte[] Compress(byte[] source, Compression.Oodle.OodleLZ_Compressor compressor, Compression.Oodle.OodleLZ_CompressionLevel level)
            {
                return Toolbox.Library.Compression.Oodle.Compress(source, compressor, level);
            }
        }
    }
}
