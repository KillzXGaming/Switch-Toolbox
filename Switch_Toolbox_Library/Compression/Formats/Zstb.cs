using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace Toolbox.Library
{
    public class Zstb : ICompressionFormat
    {
        public string[] Description { get; set; } = new string[] { "ZSTD" };
        public string[] Extension { get; set; } = new string[] { "*.zstd", "*.zst", };

        public override string ToString() { return "ZSTD"; }

        static string fileNameTemp = "";

        public bool Identify(Stream stream, string fileName)
        {
            //Small hack to check current file name
            fileNameTemp = fileName;
            using (var reader = new FileReader(stream, true))
            {
                uint magic = reader.ReadUInt32();
                reader.Position = 0;
                return magic == 0x28B52FFD || magic == 0xFD2FB528;
            }
        }

        public bool CanCompress { get; } = true;

        public Stream Decompress(Stream stream)
        {
            return new MemoryStream(SDecompress(stream.ToArray()));
        }

        public Stream Compress(Stream stream)
        {
            return new MemoryStream(SCompress(stream.ToArray()));
        }

        public static byte[] SDecompress(byte[] b)
        {
            var options = new ZstdNet.DecompressionOptions(GetExternalDictionaries());
            using (var decompressor = new ZstdNet.Decompressor(options))
            {
                return decompressor.Unwrap(b);
            }
        }

        public static byte[] SDecompress(byte[] b, byte[] dict)
        {
            var options = new ZstdNet.DecompressionOptions(dict);
            using (var decompressor = new ZstdNet.Decompressor(options))
            {
                return decompressor.Unwrap(b);
            }
        }

        public static byte[] SDecompress(byte[] b, int MaxDecompressedSize)
        {
            var options = new ZstdNet.DecompressionOptions(GetExternalDictionaries());
            using (var decompressor = new ZstdNet.Decompressor(options))
            {
                return decompressor.Unwrap(b, MaxDecompressedSize);
            }
        }
        public static byte[] SCompress(byte[] b)
        {
            using (var compressor = new ZstdNet.Compressor())
            {
                return compressor.Wrap(b);
            }
        }

        static byte[] GetExternalDictionaries()
        {
            byte[] dictionary = new byte[0];

            string folder = Path.Combine(Runtime.ExecutableDir, "Lib", "ZstdDictionaries");
            if (Directory.Exists(folder))
            {
                void CheckZDic(string fileName, string expectedExtension)
                {
                    //Dictionary already set
                    if (dictionary.Length != 0) return;

                    string zDictPath = Path.Combine(folder, fileName);
                    //Then check if the input file uses the expected extension
                    if (File.Exists(zDictPath) && fileNameTemp.EndsWith(expectedExtension))
                        dictionary = File.ReadAllBytes(zDictPath);
                }

                //Order matters, zs must go last
                CheckZDic("bcett.byml.zsdic",  "bcett.byml.zs" );
                CheckZDic("pack.zsdic", "pack.zs" );
                CheckZDic("zs.zsdic", ".zs" );
            }
            return dictionary;
        }
    }
}
