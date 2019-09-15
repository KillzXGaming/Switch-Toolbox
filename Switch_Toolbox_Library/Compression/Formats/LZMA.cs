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

        public Stream Decompress(Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.SetByteOrder(true);

                var output = new System.IO.MemoryStream();
                if (reader.CheckSignature(4, "LZMA", 1))
                {
                    byte unk = reader.ReadByte();
                    uint magic = reader.ReadUInt32();
                    ushort unk2 = reader.ReadUInt16();
                }

                uint decompressedSize = reader.ReadUInt32();
                var properties = reader.ReadBytes(5);

                var compressedSize = stream.Length - 16;
                var copressedBytes = reader.ReadBytes((int)compressedSize);

                SevenZip.Compression.LZMA.Decoder decode = new SevenZip.Compression.LZMA.Decoder();
                decode.SetDecoderProperties(properties);


                MemoryStream ms = new MemoryStream(copressedBytes);
                decode.Code(ms, output, compressedSize, decompressedSize, null);

                return output;
            }
        }

        public Stream Compress(Stream stream)
        {
            MemoryStream mem = new MemoryStream();
            using (GZipStream gzip = new GZipStream(mem, CompressionMode.Compress, true))
            {
                stream.CopyTo(gzip);
            }
            return mem;
        }
    }
}
