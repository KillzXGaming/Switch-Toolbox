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
            if (stream.Length < 16) return false;

            using (var reader = new FileReader(stream, true))
            {
                startPosition = stream.Position;

                reader.SetByteOrder(true);

                bool IsValid = false;
                for (int i = 0; i < 8; i++)
                {
                    ushort magicNumber = reader.ReadUInt16();

                    IsValid = magicNumber == 0x789C || magicNumber == 0x78DA;
                    if (IsValid) {
                        startPosition = reader.Position - 2;
                        break;
                    }
                }

                return IsValid;
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
