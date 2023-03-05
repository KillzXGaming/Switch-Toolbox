using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using System.Runtime.InteropServices;

namespace DKCTF
{
    internal class IOFileExtension
    {
        public static void WriteList<T>(FileWriter writer, List<T> list)
        {
            writer.Write(list.Count);
            for (int i = 0; i < list.Count; i++)
                writer.WriteStruct(list[i]);
        }

        public static string ReadFixedString(FileReader reader, bool isSwitch = true)
        {
            if (!isSwitch)
                return reader.ReadZeroTerminatedString();

            uint len = reader.ReadUInt32();
           return reader.ReadString((int)len, true);
        }

        public static List<T> ReadList<T>(FileReader reader)
        {
            List<T> list = new List<T>();

            uint count = reader.ReadUInt32();
            for (int i = 0; i < count; i++)
                list.Add(reader.ReadStruct<T>());
            return list;
        }

        public static CObjectId ReadID(FileReader reader)
        {
            return new CObjectId()
            {
                Guid = new CGuid()
                {
                    Part1 = reader.ReadUInt32(),
                    Part2 = reader.ReadUInt16(),
                    Part3 = reader.ReadUInt16(),
                    Part4 = reader.ReadBytes(8),
                },
            };
        }

        ///
        public static byte[] DecompressedBuffer(FileReader reader, uint compSize, uint decompSize, bool isSwitch = true)
        {
            reader.SetByteOrder(false);
            CompressionType type = (CompressionType)reader.ReadUInt32();
            reader.SetByteOrder(true);

            Console.WriteLine($"type {type}");
            var data = reader.ReadBytes((int)compSize - 4);

            switch (type)
            {
                case CompressionType.None:
                    return data;
                //LZSS with byte, short, and uint types
                case CompressionType.LZSS_8:  return DecompressLZSS(data, 1, decompSize);
                case CompressionType.LZSS_16: return DecompressLZSS(data, 2, decompSize);
                case CompressionType.LZSS_32: return DecompressLZSS(data, 3, decompSize);
                case CompressionType.ZLib:
                    return STLibraryCompression.ZLIB.Decompress(data);
                default:
                    return new byte[decompSize];
            }
        }

        public enum CompressionType //8 = byte, 16 = short, 32 = uint32
        {
            None,
            LZSS_8  = 0x1,
            LZSS_16 = 0x2,
            LZSS_32 = 0x3,
            ArithmeticStream_LZSS_8 = 0x4,
            ArithmeticStream_LZSS_16 = 0x5,
            ArithmeticStream_LZSS_32 = 0x6,
            LZSS_8_3Byte = 0x7,
            LZSS_16_3Byte = 0x8,
            LZSS_32_3Byte = 0x9,
            ArithmeticStream_LZSS_8_3Byte = 0xA,
            ArithmeticStream_LZSS_16_3Byte = 0xB,
            ArithmeticStream_LZSS_32_3Byte = 0xC,
            ZLib = 0xD,
        }

        public static byte[] DecompressLZSS(byte[] input, int mode, uint decompressedLength)
        {
            byte[] decomp = new byte[decompressedLength];

            int src = 0;
            int dst = 0;

            // Otherwise, start preparing for decompression.
            byte header_byte = 0;
            byte group = 0;

            while (src < input.Length && dst < decompressedLength)
            {
                // group will start at 8 and decrease by 1 with each data chunk read.
                // When group reaches 0, we read a new header byte and reset it to 8.
                if (group == 0)
                {
                    header_byte = input[src++];
                    group = 8;
                }

                // header_byte will be shifted left one bit for every data group read, so 0x80 always corresponds to the current data group.
                // If 0x80 is set, then we read back from the decompressed buffer.
                if ((header_byte & 0x80) != 0)
                {
                    byte[] bytes = new byte[] { input[src++], input[src++] };
                    uint count = 0, length = 0;

                    switch (mode)
                    {
                        case 1: //byte
                            count = (uint)((bytes[0] >> 4) + 3);
                            length = (uint)(((bytes[0] & 0xF) << 0x8) | bytes[1]);
                            break;
                        case 2: //short
                            count = (uint)((bytes[0] >> 4) + 2);
                            length = (uint)((((bytes[0] & 0xF) << 0x8) | bytes[1]) << 1);
                            break;
                        case 3: //uint
                            count = (uint)((bytes[0] >> 4) + 1);
                            length = (uint)((((bytes[0] & 0xF) << 0x8) | bytes[1]) << 2);
                            break;
                    }

                    // With the count and length calculated, we'll set a pointer to where we want to read back data from:
                    int seek = (dst - (int)length);

                    // count refers to how many byte groups to read back; the size of one byte group varies depending on mode
                    for (uint yb = 0; yb < count; yb++)
                    {
                        switch (mode)
                        {
                            case 1: //byte
                                decomp[dst++] = decomp[(int)seek++];
                                break;
                            case 2: //short
                                for (uint b = 0; b < 2; b++)
                                    decomp[dst++] = decomp[(int)seek++];
                                break;
                            case 3: //uint
                                for (uint b = 0; b < 4; b++)
                                    decomp[dst++] = decomp[(int)seek++];
                                break;

                        }
                    }
                }
                else
                {
                    // If 0x80 is not set, then we read one byte group directly from the compressed buffer.
                    switch (mode)
                    {
                        case 1: //byte
                            decomp[dst++] = input[src++];
                            break;
                        case 2: //short
                            for (uint b = 0; b < 2; b++)
                                decomp[dst++] = input[src++];
                            break;
                        case 3: //uint
                            for (uint b = 0; b < 4; b++)
                                decomp[dst++] = input[src++];
                            break;
                    }
                }
                header_byte <<= 1;
                group--;
            }
            return decomp;
        }

        public static byte[] DecompressArithmeticStream(byte[] input, int mode, uint decompressedLength)
        {
            //TODO

            return input;
        }

        public static byte[] DecompressLZSS3Bytes(byte[] input, int mode, uint decompressedLength)
        {
            byte[] decomp = new byte[decompressedLength];

            //TODO

            return decomp;
        }
    }
}
