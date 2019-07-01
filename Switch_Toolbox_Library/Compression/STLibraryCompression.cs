using Syroot.BinaryData;
using System.IO;
using System.IO.Compression;
using K4os.Compression.LZ4.Streams;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Switch_Toolbox.Library.IO
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
                       return  DecompressZCMP(b);
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

                using (var decompressedStream = new MemoryStream(output.ToArray()))
                {
                    using (var zipStream = new DeflateStream(output, CompressionMode.Compress))
                    {
                        zipStream.Write(b, 0, b.Length);

                        return output.ToArray();
                    }
                }
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

        //Mario Tennis Aces Custom compression
        public class MTA_CUSTOM
        {
            private static uint Swap(uint X)
            {
                return ((X >> 24) & 0xff | (X >> 8) & 0xff00 |
                         (X << 8) & 0xff0000 | (X << 24) & 0xff000000);
            }

            public byte[] Decompress(byte[] data, uint decompressedLength)
            {
                uint dest = 0;
                uint source = 0;

                uint pos = 8;
                byte[] Output = new byte[decompressedLength];

                if (pos > decompressedLength)
                {
                    uint flag;
                }

                return Output;
            }
        }

        public class LZSS
        {
            //From https://github.com/IcySon55/Kuriimu/blob/f670c2719affc1eaef8b4c40e40985881247acc7/src/Kontract/Compression/LZSS.cs
            //Todo does not work with Paper Mario Color Splash
            public static byte[] Decompress(byte[] data, uint decompressedLength)
            {
                using (FileReader reader = new FileReader(new MemoryStream(data), true))
                {
                    List<byte> result = new List<byte>();

                    for (int i = 0, flags = 0; ; i++)
                    {
                        if (i % 8 == 0) flags = reader.ReadByte();
                        if ((flags & 0x80) == 0) result.Add(reader.ReadByte());
                        else
                        {
                            int lengthDist = BitConverter.ToUInt16(reader.ReadBytes(2).Reverse().ToArray(), 0);
                            int offs = lengthDist % 4096 + 1;
                            int length = lengthDist / 4096 + 3;
                            while (length > 0)
                            {
                                result.Add(result[result.Count - offs]);
                                length--;
                            }
                        }

                        if (result.Count == decompressedLength)
                        {
                            return result.ToArray();
                        }
                        else if (result.Count > decompressedLength)
                        {
                            throw new InvalidDataException("Went past the end of the stream");
                        }
                        flags <<= 1;
                    }
                }
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
