using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class Yaz0 : ICompressionFormat
    {
        public int Alignment = 0;

        public string[] Description { get; set; } = new string[] { "Yaz0" };
        public string[] Extension { get; set; } = new string[] { "*.yaz0", "*.szs",};

        public override string ToString() { return "Yaz0"; }

        public bool Identify(Stream stream, string fileName)
        {
            using (var reader = new FileReader(stream, true))
            {
                return reader.CheckSignature(4, "Yaz0");
            }
        }

        public bool CanCompress { get; } = true;

        public Stream Decompress(Stream stream)
        {
           return new MemoryStream(EveryFileExplorer.YAZ0.Decompress(stream.ToArray()));
        }

        public Stream Compress(Stream stream)
        {
            return new MemoryStream(EveryFileExplorer.YAZ0.Compress(stream.ToArray(), Alignment, (uint)Runtime.Yaz0CompressionLevel));
        }
    }
}
