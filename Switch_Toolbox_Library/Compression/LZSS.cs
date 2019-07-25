using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using K4os.Compression.LZ4.Streams;

namespace Toolbox.Library
{
    //From https://github.com/xdanieldzd/N3DSCmbViewer/blob/3c3f66cf40d9122f8d0ebeab07c4db659b426b8b/N3DSCmbViewer/LZSS.cs
    //and https://github.com/lue/MM3D/blob/master/src/lzs.cpp
    public class LZSS : ICompressionFormat
    {
        public string[] Description { get; set; } = new string[] { "LZSS Compression" };
        public string[] Extension { get; set; } = new string[] { "*.lzs", "*.lzss" };

        public bool Identify(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                return reader.CheckSignature(4, "LzS\x01");
            }
        }

        public bool CanCompress { get; } = true;

        public Stream Decompress(Stream stream)
        {
            byte[] arcdata = stream.ToArray();

            string tag = Encoding.ASCII.GetString(arcdata, 0, 4);
            uint unknown = BitConverter.ToUInt32(arcdata, 4);
            uint decompressedSize = BitConverter.ToUInt32(arcdata, 8);
            uint compressedSize = BitConverter.ToUInt32(arcdata, 12);

            if (arcdata.Length != compressedSize + 0x10) throw new Exception("compressed size mismatch");

            List<byte> outdata = new List<byte>();
            byte[] BUFFER = new byte[4096];
            for (int i = 0; i < BUFFER.Length; i++) BUFFER[i] = 0;
            byte flags8 = 0;
            ushort writeidx = 0xFEE;
            ushort readidx = 0;
            uint fidx = 0x10;

            while (fidx < arcdata.Length)
            {
                flags8 = arcdata[fidx];
                fidx++;

                for (int i = 0; i < 8; i++)
                {
                    if ((flags8 & 1) != 0)
                    {
                        outdata.Add(arcdata[fidx]);
                        BUFFER[writeidx] = arcdata[fidx];
                        writeidx++; writeidx %= 4096;
                        fidx++;
                    }
                    else
                    {
                        readidx = arcdata[fidx];
                        fidx++;
                        readidx |= (ushort)((arcdata[fidx] & 0xF0) << 4);
                        for (int j = 0; j < (arcdata[fidx] & 0x0F) + 3; j++)
                        {
                            outdata.Add(BUFFER[readidx]);
                            BUFFER[writeidx] = BUFFER[readidx];
                            readidx++; readidx %= 4096;
                            writeidx++; writeidx %= 4096;
                        }
                        fidx++;
                    }
                    flags8 >>= 1;
                    if (fidx >= arcdata.Length) break;
                }
            }

            if (decompressedSize != outdata.Count)
                throw new Exception(string.Format("Size mismatch: got {0} bytes after decompression, expected {1}.\n", outdata.Count, decompressedSize));

            return new MemoryStream(outdata.ToArray());
        }

        public Stream Compress(Stream stream)
        {
            var mem = new MemoryStream();
            return stream;
        }
    }
}
