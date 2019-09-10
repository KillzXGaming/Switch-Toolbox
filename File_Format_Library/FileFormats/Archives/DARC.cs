using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class DARC : IArchiveFile, IFileFormat, ILeaveOpenOnLoad
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "DARC" };
        public string[] Extension { get; set; } = new string[] { "*.arc", "*.darc" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "darc");
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

        public void ClearFiles() { files.Clear(); }

        public uint Version;
        public ushort Bom;

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                reader.ReadSignature(4, "darc");
                Bom = reader.ReadUInt16();
                reader.CheckByteOrderMark(Bom);
                ushort headerLength = reader.ReadUInt16();

                Version = reader.ReadUInt32();
                uint FileSize = reader.ReadUInt32();
                uint FileTableOffset = reader.ReadUInt32();
                uint FileTableLength = reader.ReadUInt32();
                uint FileDataOffset = reader.ReadUInt32();

                Console.WriteLine("Version " + Version);
                Console.WriteLine("FileTableOffset " + FileTableOffset);
                Console.WriteLine("FileTableLength " + FileTableLength);
                Console.WriteLine("FileDataOffset " + FileDataOffset);

                uint endOfTable = FileDataOffset + FileTableLength;

                List<NodeEntry> entries = new List<NodeEntry>();
                reader.SeekBegin(FileTableOffset);
                entries.Add(new NodeEntry(reader));
                for (int i = 0; i < entries[0].Size - 1; i++)
                    entries.Add(new NodeEntry(reader));

                for (int i = 0; i < entries.Count; i++)
                    entries[i].Name = ReadCStringW(reader);

                for (int i = 0; i < entries.Count; i++)
                {
                    string Name = entries[i].Name;
                    if (entries[i].IsFolder)
                    {
                        for (int s = 0; s < entries[i].Size; s++)
                            entries[i].FullName += $"{Name}/";
                    }
                    else
                        entries[i].FullName = Name;
                }

                for (int i = 0; i < entries.Count; i++)
                {
                    if (!entries[i].IsFolder)
                    {
                        var file = new FileEntry();
                        file.FileName = entries[i].FullName;
                        file.FileDataStream = new SubStream(reader.BaseStream, entries[i].Offset, entries[i].Size);
                        files.Add(file);
                    }
                }
            }
        }

        public string ReadCStringW(FileReader reader) => string.Concat(Enumerable.Range(0, 999).Select(_ => (char)reader.ReadInt16()).TakeWhile(c => c != 0));

        public void Unload()
        {

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

        public class NodeEntry
        {
            public uint NameOffset;
            public uint Size;
            public uint Offset;

            public bool IsFolder => (NameOffset >> 24) == 1;

            public string Name;

            public string FullName;

            public NodeEntry(FileReader reader)
            {
                NameOffset = reader.ReadUInt32();
                Offset = reader.ReadUInt32();
                Size = reader.ReadUInt32();
            }
        }

        public class FileEntry : ArchiveFileInfo
        {
        }
    }
}
