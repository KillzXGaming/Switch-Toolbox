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
    public class Zstb_Kirby : ICompressionFormat
    {
        public string[] Description { get; set; } = new string[] { "ZSTD Compression (Kirby)" };
        public string[] Extension { get; set; } = new string[] { "*.cmp" };

        public override string ToString() { return "ZSTD (Kirby)"; }

        public bool Identify(Stream stream, string fileName)
        {
            if (stream.Length < 12) return false;

            using (var reader = new FileReader(stream, true))
            {
                uint DecompressedSize = reader.ReadUInt32();
                uint magicCheck = reader.ReadUInt32();

                bool ZSTDDefault = magicCheck == 0xFD2FB528;

                return ZSTDDefault;
            }
        }

        public bool CanCompress { get; } = true;

        public Stream Decompress(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                reader.Position = 0;
                int OuSize = reader.ReadInt32();
                int InSize = (int)stream.Length - 4;
                var dec = Zstb.SDecompress(reader.getSection(4, InSize));
                return new MemoryStream(dec);
            }
        }

        public Stream Compress(Stream stream)
        {
            var mem = new MemoryStream();
            using (var writer = new FileWriter(mem, true))
            {
                writer.Write((uint)stream.Length);
                byte[] buffer = Zstb.SCompress(stream.ToArray());

                writer.Write(buffer, 0, buffer.Length);
            }
            return mem;
        }
    }
}
