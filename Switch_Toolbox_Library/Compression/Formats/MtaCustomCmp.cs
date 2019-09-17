using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class MtaCustomCmp : ICompressionFormat
    {
        public int Alignment = 0;

        public string[] Description { get; set; } = new string[] { "Mario Tennis Custom" };
        public string[] Extension { get; set; } = new string[] { "*.cbtx", };

        public override string ToString() { return "Mario Tennis Custom"; }

        public bool Identify(Stream stream, string fileName)
        {
            return Utils.GetExtension(fileName) == ".cbtx";
        }

        public bool CanCompress { get; } = false;

        public Stream Decompress(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                reader.SeekBegin(4);
                uint decompSize = reader.ReadUInt32();
                var acesComp = new STLibraryCompression.MTA_CUSTOM();
                return new MemoryStream(acesComp.Decompress(stream.ToArray(), decompSize));
            }
        }

        public Stream Compress(Stream stream)
        {
            return new MemoryStream();
        }
    }
}
