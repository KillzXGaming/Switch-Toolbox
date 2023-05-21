using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Runtime.InteropServices;
using Toolbox.Library.Compression.LZ77_wii_11_compresss.Formats.Nitro;

namespace Toolbox.Library
{
    public class LZ77 : ICompressionFormat
    {
        public string[] Description { get; set; } = new string[] { "LZ77 Compressed" };
        public string[] Extension { get; set; } = new string[] { "*.lz", };

        public bool Identify(Stream stream, string fileName)
        {
            if (stream.Length < 16)
                return false;

            using (var reader = new FileReader(stream, true))
            {
                if(Utils.GetExtension(fileName) == ".lz")
                {
                    reader.SeekBegin(12);
                    return reader.ReadByte() == 0x11;
                }
            }
            return false;
        }

        public bool CanCompress { get; } = false;

        private bool UseLZMAMagicHeader = true;

        public Stream Decompress(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                reader.SeekBegin(12);
                byte type = reader.ReadByte();
                if (type == 0x11)
                {
                    uint decomp_size = reader.ReadUInt32();

                    var sub = new SubStream(stream, 16);
                    return new MemoryStream(LZ77_WII.Decompress11(sub.ToArray(), (int)decomp_size));
                }
                else
                {
                    return new MemoryStream();
                }
            }
        }
        // A modified version of dsdecmp for compressing files into the Wii LZ77 type 11 .lz  -Adapted by:DanielSvoboda
        public Stream Compress(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                using (MemoryStream outstream = new MemoryStream())
                {
                    LZ11 lz11 = new LZ11();
                    int compressedSize = lz11.Compress(stream, stream.Length, outstream);
                    return new MemoryStream(outstream.ToArray());
                }
            }
        }
    }
}
