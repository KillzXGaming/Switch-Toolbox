using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class Gzip : ICompressionFormat
    {
        public string[] Description { get; set; } = new string[] { "GZIP Compressed" };
        public string[] Extension { get; set; } = new string[] { "*.gzip", };

        private long startPosition = 0;

        private bool IsSonicWinterOlypmics = false;

        public override string ToString() { return "Gzip"; }

        public bool Identify(Stream stream, string fileName)
        {
            using (var reader = new FileReader(stream, true))
            {
                reader.SetByteOrder(true);

                ushort magicNumber = reader.ReadUInt16();

                reader.Position = 0;
                string magicSig = reader.ReadString(4);
                IsSonicWinterOlypmics = magicSig == "ZLIB";
                if (IsSonicWinterOlypmics)
                    startPosition = 64;

                return magicNumber == 0x1f8b || IsSonicWinterOlypmics;
            }
        }

        public bool CanCompress { get; } = true;

        public Stream Decompress(Stream stream)
        {
            stream.Position = startPosition;

            var mem = new System.IO.MemoryStream();
            using (GZipStream source = new GZipStream(stream, CompressionMode.Decompress, false))
            {
                source.CopyTo(mem);
            }   
            return mem;
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
