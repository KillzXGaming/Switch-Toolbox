using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class LZMA : ICompressionFormat
    {
        public string[] Description { get; set; } = new string[] { "LZMA Compressed" };
        public string[] Extension { get; set; } = new string[] { "*.lzma", };

        public override string ToString() { return "LZMA"; }

        public bool Identify(Stream stream, string fileName)
        {
            using (var reader = new FileReader(stream, true))
            {
                return reader.CheckSignature(4,"LZMA", 1);
            }
        }

        public bool CanCompress { get; } = true;

        private bool UseLZMAMagicHeader = true;

        public Stream Decompress(Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.SetByteOrder(false);

                var output = new System.IO.MemoryStream();
                if (reader.CheckSignature(4, "LZMA", 1))
                {
                    byte unk = reader.ReadByte(); //0xFF
                    uint magic = reader.ReadUInt32();
                    reader.ReadByte(); //padding
                    UseLZMAMagicHeader = true;
                }

                byte[] properties = reader.ReadBytes(5); //Property and dictionary size
                ulong decompressedSize = reader.ReadUInt64();

                var compressedSize = stream.Length - reader.Position;
                var copressedBytes = reader.ReadBytes((int)compressedSize);

                SevenZip.Compression.LZMA.Decoder decode = new SevenZip.Compression.LZMA.Decoder();
                decode.SetDecoderProperties(properties);

                MemoryStream ms = new MemoryStream(copressedBytes);
                decode.Code(ms, output, compressedSize, (int)decompressedSize, null);

                return output;
            }
        }

        public Stream Compress(Stream stream)
        {
            MemoryStream mem = new MemoryStream();
            using (var writer = new FileWriter(mem, true))
            {
                writer.SetByteOrder(false);
                if (UseLZMAMagicHeader)
                {
                    writer.Write((byte)0xFF);
                    writer.WriteSignature("LZMA");
                    writer.Write((byte)0);
                }
            }

            SevenZip.Compression.LZMA.Encoder encode = new SevenZip.Compression.LZMA.Encoder();
            encode.WriteCoderProperties(mem);
            mem.Write(BitConverter.GetBytes(stream.Length), (int)stream.Position, 8);
            encode.Code(stream, mem, -1, -1, null);

            return mem;
        }
    }
}
