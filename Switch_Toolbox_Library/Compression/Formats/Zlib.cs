using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class Zlib : ICompressionFormat
    {
        public string[] Description { get; set; } = new string[] { "ZLIB Compressed" };
        public string[] Extension { get; set; } = new string[] { "*.z", "*.zlib", };

        private long startPosition = 0;

        public override string ToString() { return "Zlib"; }

        public bool Identify(Stream stream, string fileName)
        {
            using (var reader = new FileReader(stream, true))
            {
                startPosition = stream.Position;

                reader.SetByteOrder(true);

                ushort magicNumber = reader.ReadUInt16();

                reader.Position = startPosition + 4;
                ushort magicNumber2 = reader.ReadUInt16();

                //Check 2 cases which the file is zlibbed.
                //One is that it is compressed with magic at start
                //Another is a size (uint) then magic
                bool IsValid = magicNumber == 0x789C || magicNumber == 0x78DA;
                bool IsValid2 = magicNumber2 == 0x789C || magicNumber2 == 0x78DA;
                if (IsValid2)
                    startPosition = stream.Position + 4;

                return IsValid || IsValid2;
            }
        }

        public bool CanCompress { get; } = true;

        public Stream Decompress(Stream stream)
        {
             stream.Position = startPosition;

            var data = STLibraryCompression.ZLIB.Decompress(stream.ToArray());
            return new MemoryStream(data);
        }

        public Stream Compress(Stream stream)
        {
            stream.Position = startPosition;

            var data = STLibraryCompression.ZLIB.Compress(stream.ToArray());
            return new MemoryStream(data);
        }
    }
}
