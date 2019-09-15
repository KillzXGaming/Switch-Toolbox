using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using K4os.Compression.LZ4.Streams;

namespace Toolbox.Library
{
    public class lz4 : ICompressionFormat
    {
        public string[] Description { get; set; } = new string[] { "LZ4 Compression" };
        public string[] Extension { get; set; } = new string[] { "*.lz4" };

        public override string ToString() { return "lz4"; }

        public bool Identify(Stream stream, string fileName)
        {
            return false;

            using (var reader = new FileReader(stream, true))
            {
                uint DecompressedSize = reader.ReadUInt32();
                uint magicCheck = reader.ReadUInt32();

                bool LZ4FDefault = magicCheck == 0x184D2204;

                return LZ4FDefault;
            }
        }

        public bool CanCompress { get; } = true;

        public Stream Decompress(Stream stream)
        {
            using (MemoryStream mem = new MemoryStream())
            {
                using (var source = LZ4Stream.Decode(stream))
                {
                    source.CopyTo(mem);
                }
                return mem;
            }
        }

        public Stream Compress(Stream stream)
        {
            var mem = new MemoryStream();
            using (var source = LZ4Stream.Encode(stream, null, true))
            {
                source.CopyTo(mem);
            }
            return mem;
        }
    }
}
