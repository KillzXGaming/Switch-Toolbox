using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace Toolbox.Library
{
    public class ZCMP : ICompressionFormat
    {
        public string[] Description { get; set; } = new string[] { "ZLIB Compression (ZCMP)" };
        public string[] Extension { get; set; } = new string[] { "*.cmp" };

        public bool Identify(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                return reader.CheckSignature(4, "ZCMP");
            }
        }

        public bool CanCompress { get; } = true;

        public Stream Decompress(Stream stream)
        {
            using (var br = new FileReader(stream, true))
            {
                var ms = new System.IO.MemoryStream();
                br.BaseStream.Position = 130;
                using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes((int)br.BaseStream.Length - 80)), CompressionMode.Decompress))
                    ds.CopyTo(ms);
                return ms;
            }
        }

        public Stream Compress(Stream stream)
        {
            var mem = new MemoryStream();
            mem.Write(new byte[] { 0x78, 0xDA }, 0, 2);
            using (var zipStream = new DeflateStream(mem, CompressionMode.Compress))
            {
                zipStream.CopyTo(stream);
                return mem;
            }
        }
    }
}
