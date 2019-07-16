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
    public class LZ4F : ICompressionFormat
    {
        public string[] Description { get; set; } = new string[] { "LZ4F Compression" };
        public string[] Extension { get; set; } = new string[] { "*.cmp", "*.lz4f" };

        public bool Identify(Stream stream)
        {
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
            using (var writer = new FileWriter(mem))
            {
                var data = stream.ToArray();
                writer.Write(data.Length);
                byte[] buffer = LZ4.Frame.LZ4Frame.Compress(new MemoryStream(data),
                    LZ4.Frame.LZ4MaxBlockSize.MB1, true, true, false, true, false);

                writer.Write(buffer, 0, buffer.Length);
            }
            return stream;
        }
    }
}
