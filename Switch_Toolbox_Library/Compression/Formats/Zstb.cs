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

        public void Init(string fileName) { fileNameTemp = fileName; }

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
        public static byte[] SCompress(byte[] b, int level = 19)
        {
            using (var compressor = new ZstdNet.Compressor(new ZstdNet.CompressionOptions(level)))
            {
                return compressor.Wrap(b);
            }
        }

        static byte[] GetExternalDictionaries()
        {
            byte[] dictionary = new byte[0];

            var userDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SwitchToolbox");
            if (!Directory.Exists(userDir))
                Directory.CreateDirectory(userDir);

            //Create folder for TOTK contents if it does not exist
            if (!Directory.Exists(Path.Combine(userDir, "TOTK")))
                Directory.CreateDirectory(Path.Combine(userDir, "TOTK"));

            string folder = Path.Combine(userDir, "TOTK", "ZstdDictionaries");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            void TransferZDic(string path)
            {
                //Check if old directory contains the file and move it
                string fileOld = Path.Combine(Runtime.ExecutableDir, "Lib", "ZstdDictionaries", path);
                string fileNew = Path.Combine(folder, path);
                if (!File.Exists(fileNew) && File.Exists(fileOld))
                {
                    File.Move(fileOld, fileNew); 
                }
            }
            TransferZDic("bcett.byml.zsdic");
            TransferZDic("pack.zsdic");
            TransferZDic("zs.zsdic");

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
