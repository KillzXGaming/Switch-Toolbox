using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Switch_Toolbox.Library.IO;

namespace Switch_Toolbox.Library
{
    public class YAZ0 : ICompressionFormat
    {
        public string[] Description { get; set; } = new string[] { "YAZ0" };
        public string[] Extension { get; set; } = new string[] { "*.yaz0", "*.szs",};

        public bool Identify(Stream stream)
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
            return new MemoryStream(EveryFileExplorer.YAZ0.Compress(stream.ToArray()));
        }
    }
}
