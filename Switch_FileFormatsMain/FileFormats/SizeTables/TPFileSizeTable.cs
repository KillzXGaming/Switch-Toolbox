using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library;
using Syroot.BinaryData;

namespace FirstPlugin
{
    public class TPFileSizeTable
    {
        public class DecompressedTableEntry
        {
            public string FilePath { get; set; }
            public uint Size { get; set; }
            public uint Size2 { get; set; }
            public string Precentage { get; set; }
        }

        public bool UseCompressedSizes = false;

        public Dictionary<string, uint> FileSizes = new Dictionary<string, uint>();
        public Dictionary<string, DecompressedTableEntry> DecompressedFileSizes = new Dictionary<string, DecompressedTableEntry>();
        
        public static void SetTables(IFileFormat FileFormat)
        {
            //Read the tables
            TPFileSizeTable CompressedFileTbl = new TPFileSizeTable();
            CompressedFileTbl.ReadCompressedTable(new FileReader($"{Runtime.TpGamePath}/FileSizeList.txt"));

            TPFileSizeTable DecompressedFileTbl = new TPFileSizeTable();
            DecompressedFileTbl.ReadDecompressedTable(new FileReader($"{Runtime.TpGamePath}/DecompressedSizeList.txt"));

            //   var tableSave = new MemoryStream();
            //   CompressedFileTbl.Write(new FileWriter(tableSave));
            //  File.WriteAllBytes($"{Runtime.TpGamePath}/FileSizeListTEST.txt", tableSave.ToArray());
            //       bool IsSuccess = CompressedFileTbl.TrySetSize(FileName, IFileInfo.CompressedSize, IFileInfo.DecompressedSize, ArchiveSizes);

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

                uint sizeNum = 0;
                uint sizeNum2 = 0;

                uint.TryParse(Size, out sizeNum);
                uint.TryParse(Size2, out sizeNum2);

                entry.Size = sizeNum;
                entry.Size2 = sizeNum2;

                DecompressedFileSizes.Add(entry.FilePath, entry);
            }
        }

        public bool TrySetSize(string Path, uint DecompSize, uint CompSize, Dictionary<string, uint> ArchiveFiles = null)
        {
            string RelativePath = Path.Replace(Runtime.TpGamePath, "");

            if (FileSizes.ContainsKey(RelativePath))
            {
                if (UseCompressedSizes)
                    FileSizes[RelativePath] = CompSize;
                else
                    FileSizes[RelativePath] = DecompSize;
            }

            return false;
        }

        public void WriteCompressedTable(FileWriter writer)
        {
            foreach (var file in FileSizes)
            {
                writer.Write(file.Key, BinaryStringFormat.ZeroTerminated);
                writer.Write(file.Value.ToString(), BinaryStringFormat.ZeroTerminated);
            }
        }

        public void WriteDecompressedTable(FileWriter writer)
        {
            foreach (var file in DecompressedFileSizes.Values)
            {
                writer.Write(file.Size.ToString(), BinaryStringFormat.ZeroTerminated);
                writer.Write(file.Size2.ToString(), BinaryStringFormat.ZeroTerminated);
                writer.Write(file.Precentage.ToString(), BinaryStringFormat.ZeroTerminated);
                writer.Write(file.FilePath.ToString(), BinaryStringFormat.ZeroTerminated);
            }
        }
    }
}
