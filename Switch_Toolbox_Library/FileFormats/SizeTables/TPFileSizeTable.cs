using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library;
using Syroot.BinaryData;

namespace Switch_Toolbox.Library
{
    public class TPFileSizeTable
    {
        public class DecompressedTableEntry
        {
            public string FilePath { get; set; }
            public uint CompressedSize { get; set; }
            public uint DecompressedSize { get; set; }
            public string Precentage { get; set; }
        }

        public bool UseCompressedSizes = false;

        public Dictionary<string, uint> FileSizes = new Dictionary<string, uint>();
        public Dictionary<string, DecompressedTableEntry> DecompressedFileSizes = new Dictionary<string, DecompressedTableEntry>();

        public bool IsInFileSizeList(string FileName) => FileSizes.ContainsKey(FileName);
        public bool IsInDecompressedFileSizeList(string FileName) => DecompressedFileSizes.ContainsKey(FileName);

        public void SetFileSizeEntry(string FileName, uint Size) {
            FileSizes[FileName] = Size;
        }

        public void SetDecompressedFileSizeEntry(string FileName, uint CompressedSize, uint DecompressedSize) {
            DecompressedFileSizes[FileName].CompressedSize = CompressedSize;
            DecompressedFileSizes[FileName].DecompressedSize = DecompressedSize;
        }

        public void ReadCompressedTable(FileReader reader)
        {
            while (reader.Position < reader.BaseStream.Length)
            {
                string FileName = reader.ReadString(BinaryStringFormat.ZeroTerminated);
                string Size = reader.ReadString(BinaryStringFormat.ZeroTerminated);

                uint sizeNum = 0;
                uint.TryParse(Size, out sizeNum);
                FileSizes.Add(FileName, sizeNum);
            }

            reader.Close();
            reader.Dispose();
        }

        public void ReadDecompressedTable(FileReader reader)
        {
            while (reader.Position < reader.BaseStream.Length)
            {
                var entry = new DecompressedTableEntry();
                string Size = reader.ReadString(BinaryStringFormat.ZeroTerminated);
                string Size2 = reader.ReadString(BinaryStringFormat.ZeroTerminated);
                entry.Precentage = reader.ReadString(BinaryStringFormat.ZeroTerminated);
                entry.FilePath = reader.ReadString(BinaryStringFormat.ZeroTerminated);

                Console.WriteLine($"Size {Size} Size2 {Size2} {entry.Precentage} {entry.FilePath}");

                uint sizeNum = 0;
                uint sizeNum2 = 0;

                uint.TryParse(Size, out sizeNum);
                uint.TryParse(Size2, out sizeNum2);

                entry.CompressedSize = sizeNum;
                entry.DecompressedSize = sizeNum2;

                DecompressedFileSizes.Add(entry.FilePath, entry);
            }

            reader.Close();
            reader.Dispose();
        }

        public void WriteCompressedTable(FileWriter writer)
        {
            foreach (var file in FileSizes)
            {
                writer.Write(file.Key, BinaryStringFormat.ZeroTerminated);
                writer.Write(file.Value.ToString(), BinaryStringFormat.ZeroTerminated);
            }

            writer.Close();
            writer.Dispose();
        }

        public void WriteDecompressedTable(FileWriter writer)
        {
            foreach (var file in DecompressedFileSizes.Values)
            {
                writer.Write(file.CompressedSize.ToString(), BinaryStringFormat.ZeroTerminated);
                writer.Write(file.DecompressedSize.ToString(), BinaryStringFormat.ZeroTerminated);
                writer.Write(file.Precentage.ToString(), BinaryStringFormat.ZeroTerminated);
                writer.Write(file.FilePath.ToString(), BinaryStringFormat.ZeroTerminated);
            }

            writer.Close();
            writer.Dispose();
        }
    }
}
