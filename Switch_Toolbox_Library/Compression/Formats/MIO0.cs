using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class MIO0 : ICompressionFormat
    {
        public string[] Description { get; set; } = new string[] { "MIO0" };
        public string[] Extension { get; set; } = new string[] { "*.mio0" };

        public bool Identify(Stream stream, string fileName)
        {
            using (var reader = new FileReader(stream, true))
            {
                return reader.CheckSignature(4, "MIO0");
            }
        }

        public bool CanCompress { get; } = true;

        public Stream Decompress(Stream stream)
        {
            return new MemoryStream(Decompress(stream.ToArray()));
        }

        public static byte[] Decompress(byte[] data)
        {
            List<byte> output = new List<byte>();

            using (var inputFile = new FileReader(data))
            using (var reader = new FileReader(data))
            {
                uint magicNumber = reader.ReadUInt32();
                uint decompressedSize = reader.ReadUInt32();
                uint compressedOffset = reader.ReadUInt32();
                uint uncompressedOffset = reader.ReadUInt32();

            }

            return output.ToArray();
        }

        public Stream Compress(Stream stream)
        {
            return new MemoryStream();
        }
    }
}
