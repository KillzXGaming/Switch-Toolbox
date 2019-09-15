using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class ZlibGZ : ICompressionFormat
    {
        public bool IsBigEndian = true;

        public string[] Description { get; set; } = new string[] { "ZLIB GZ" };
        public string[] Extension { get; set; } = new string[] { "*.gz", };

        public bool Identify(Stream stream, string fileName)
        {
            if (Utils.GetExtension(fileName) != ".gz")
                return false;

            return STLibraryCompression.ZLIB_GZ.IsCompressed(stream);
        }

        public bool CanCompress { get; } = true;

        public Stream Decompress(Stream stream)
        {
            return STLibraryCompression.ZLIB_GZ.Decompress(stream);
        }

        public Stream Compress(Stream stream)
        {
            return STLibraryCompression.ZLIB_GZ.Compress(stream, IsBigEndian);
        }
    }
}
