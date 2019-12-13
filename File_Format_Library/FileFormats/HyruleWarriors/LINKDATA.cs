using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using System.IO;

namespace FirstPlugin
{
    public class LINKDATA : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Fire Emblem Three Houses Archive" };
        public string[] Extension { get; set; } = new string[] { "*.bin" , "*.idx" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            return (FileName == "LINKDATA.IDX" || FileName == "DATA0.bin");
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public List<FileEntry> files = new List<FileEntry>();

        public IEnumerable<ArchiveFileInfo> Files => files;

        public void ClearFiles() { files.Clear(); }

        private FileReader DataReader;

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                const uint SizeOfEntry = 32;
                uint FileCount = (uint)reader.BaseStream.Length / SizeOfEntry;

                reader.SetByteOrder(false);

                for (int i = 0; i < FileCount; i++)
                {
                    FileEntry entry = new FileEntry(FilePath, this);
                    entry.FileName = $"file {i}";
                    entry.Read(reader);

                    if (entry.Size != 0 && entry.CompFlags == 0)
                        files.Add(entry);
                }

                DataReader = GetDataFile();
                SearchData();
            }
        }

        private void SearchData()
        {
            if (DataReader == null) return;

            DataReader.Position = 0;
            for (int i = 0; i < files.Count; i++)
            {
                DataReader.SeekBegin(files[i].Offset);
                var magicCheck = DataReader.ReadString(4);
                Console.WriteLine("MAGIC=" + magicCheck);
                if (magicCheck == "SARC")
                    files[i].FileName = $"Layout/{files[i].FileName}.szs";
                else if (magicCheck == "SPKG")
                    files[i].FileName = $"ShaderPackage/{files[i].FileName}.spkg";
                else
                    files[i].FileName = $"UnknownTypes/{files[i].FileName}.bin";
            }
        }

        public void Unload()
        {
            DataReader?.Dispose();
        }

        public void Save(System.IO.Stream stream)
        {
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public FileReader GetDataFile()
        {
            if (DataReader != null) return DataReader;

            if (IFileInfo.ArchiveParent != null)
            {
                foreach (var file in IFileInfo.ArchiveParent.Files)
                {
                    if (file.FileName.Contains("DATA1.bin"))
                        return new FileReader(file.FileDataStream, true);
                }
            }
            else
            {
                string path = "";
                if (FilePath.Contains("LINKDATA.IDX"))
                    path = FilePath.Replace("IDX", "BIN");

                if (FilePath.Contains("DATA0.bin"))
                    path = FilePath.Replace("DATA0", "DATA1");

                if (!System.IO.File.Exists(path))
                    throw new Exception("Failed to find data path! " + path);

                return new FileReader(path, true);
            }

            return null;
        }

        public class FileEntry : ArchiveFileInfo
        {
            private LINKDATA ParentFile;

            public string SourcePath { get; set; }

            public ulong Offset;
            public ulong Size;
            public ulong CompSize;
            public ulong CompFlags;

            public FileEntry(string path, LINKDATA data) {
                SourcePath = path;
                ParentFile = data;
            }

            public FileType Type = FileType.Unknown;

            public void Read(FileReader reader)
            {
                Offset = reader.ReadUInt64();
                Size = reader.ReadUInt64();
                CompSize = reader.ReadUInt64();
                CompFlags = reader.ReadUInt64();
            }

            public enum FileType
            {
                Unknown,
                Shader,
                Texture,
                Model,
                Sarc,
            }

            public override Stream FileDataStream
            {
                get { return GetData(ParentFile.GetDataFile()); }
            }

            private Stream GetData(FileReader reader)
            {
                reader.SeekBegin((long)Offset);

                if (CompFlags != 0)
                {
                    uint chunkSize = reader.ReadUInt32();
                    uint numChunks = reader.ReadUInt32();
                    uint size = reader.ReadUInt32();
                    if (chunkSize == 0xffffffff)
                    {

                    }

                    reader.SeekBegin((long)Offset);
                    return new SubStream(reader.BaseStream, (long)Offset, (long)Size);
                }
                else
                    return new SubStream(reader.BaseStream, (long)Offset, (long)Size);
            }
        }
    }
}
