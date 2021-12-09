using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class Zstb : ICompressionFormat
    {
        public string[] Description { get; set; } = new string[] { "ZSTD" };
        public string[] Extension { get; set; } = new string[] { "*.zstd", "*.zst", };

        public override string ToString() { return "ZSTD"; }

        public bool Identify(Stream stream, string fileName)
        {
            using (var reader = new FileReader(stream, true))
            {
                uint magic = reader.ReadUInt32();
                reader.Position = 0;
                return magic == 0x28B52FFD || magic == 0xFD2FB528;
            }
        }

        public bool CanCompress { get; } = true;

        public Stream Decompress(Stream stream)
        {
            return new MemoryStream(SDecompress(stream.ToArray()));
        }

        public Stream Compress(Stream stream)
        {
            return new MemoryStream(SCompress(stream.ToArray()));
        }

        public static byte[] SDecompress(byte[] b)
        {
            using (var decompressor = new ZstdNet.Decompressor())
            {
                return decompressor.Unwrap(b);
            }
        }
        public static byte[] SDecompress(byte[] b, int MaxDecompressedSize)
        {
            using (var decompressor = new ZstdNet.Decompressor())
            {
                return decompressor.Unwrap(b, MaxDecompressedSize);
            }
        }
        public static byte[] SCompress(byte[] b)
        {
            using (var compressor = new ZstdNet.Compressor())
            {
                return compressor.Wrap(b);
            }
        }
    }
}
