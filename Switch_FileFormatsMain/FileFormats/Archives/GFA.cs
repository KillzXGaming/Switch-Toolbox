using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;

namespace FirstPlugin
{
    public class GFA : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Good Feel Archive" };
        public string[] Extension { get; set; } = new string[] { "*.gfa" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; } = true;
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "GFAC");
            }
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

        private uint Unknown1;
        private uint Version;
        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                reader.ReadSignature(4, "GFAC");
                Unknown1 = reader.ReadUInt32();
                Version = reader.ReadUInt32();
                uint FileInfoOffset = reader.ReadUInt32();
                uint FileInfoSize = reader.ReadUInt32();
                uint DataOffset = reader.ReadUInt32();
                uint DataSize = reader.ReadUInt32();
                byte[] Padding = reader.ReadBytes(0x10); //Not sure

                reader.SeekBegin(FileInfoOffset);
                uint FileCount = reader.ReadUInt32();
                for (int i = 0; i < FileCount; i++)
                {
                    var file = new FileEntry();
                    file.Read(reader);
                    files.Add(file);
                }

                reader.SeekBegin(DataOffset);
                reader.ReadSignature(4, "GFCP");
                uint VersionGFCP = reader.ReadUInt32();
                uint CompressionType = reader.ReadUInt32();
                uint DecompressedSize = reader.ReadUInt32();
                uint CompressedSize = reader.ReadUInt32();
            }
        }

        public void Unload()
        {

        }

        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
            using (var writer = new FileWriter(mem))
            {
                writer.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

                writer.WriteSignature("GFAC");
                writer.Write(Unknown1);
                writer.Write(Version);
                writer.Write(uint.MaxValue); //Info offset for later
                writer.Write(uint.MaxValue); //Info size for later
                writer.Write(uint.MaxValue); //Data offset for later
                writer.Write(uint.MaxValue); //Data size for later
                writer.Write(new byte[0x10]); //Padding

                writer.WriteUint32Offset(12); //Save info offset
                writer.Write(files.Count);

             /*   //Save info
                for (int i = 0; i < files.Count; i++)
                    files[i].Write(writer);

                //Save strings and offsets
                for (int i = 0; i < files.Count; i++)
                    files[i].Write(writer);

                writer.Write(new uint[files.Count]); //Save space for offsets
                for (int i = 0; i < files.Count; i++)
                    writer.Write(files[i].FileData.Length);
                */
            }

            return mem.ToArray();
        }

        private void Align(FileWriter writer, int alignment)
        {
            var startPos = writer.Position;
            long position = writer.Seek((-writer.Position % alignment + alignment) % alignment, System.IO.SeekOrigin.Current);

            writer.Seek(startPos, System.IO.SeekOrigin.Begin);
            while (writer.Position != position)
            {
                writer.Write((byte)0x30);
            }
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public class FileEntry : ArchiveFileInfo
        {
            public uint Hash { get; set; }

            public void Read(FileReader reader)
            {
                Hash = reader.ReadUInt32();
                FileName = GetName(reader);
                uint Size = reader.ReadUInt32();
                uint Offset = reader.ReadUInt32();
            }
        }

        private static string GetName(FileReader reader)
        {
            uint Offset = reader.ReadUInt32();
            using (reader.TemporarySeek(Offset & 0x00ffffff, System.IO.SeekOrigin.Begin))
            {
                return reader.ReadZeroTerminatedString();
            }
        }
    }
}
