using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class ZAR : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Zelda/Grezzo Archive" };
        public string[] Extension { get; set; } = new string[] { "*.zar", "*.gar" };
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
                return reader.CheckSignature(4, "ZAR/x01") || reader.CheckSignature(4, "GAR\x02") || reader.CheckSignature(4, "GAR\x05");
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

        public enum VersionMagic
        {
            ZAR1, //OOT3D
            GAR2, //MM3D
            GAR5, //LM3DS
        }

        public Header header;

        public void Load(System.IO.Stream stream)
        {
            header = new Header();
            header.Read(new FileReader(stream), files);
        }

        public class Header
        {
            public VersionMagic Version;

            public uint FileSize;
            public ushort FileGroupCount;
            public ushort FileCount;
            public uint FileGroupOffset;
            public uint FileInfoOffset;
            public uint DataOffset;
            public string Codename;

            public List<FileGroup> FileGroups = new List<FileGroup>();
            public List<GarFileInfo> GarFileInfos = new List<GarFileInfo>();

            public void Read(FileReader reader, List<FileEntry> files)
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                string Signature = reader.ReadString(4, Encoding.ASCII);
                switch (Signature)
                {
                    case "ZAR\x01":
                        Version = VersionMagic.ZAR1;
                        break;
                    case "GAR\x02":
                        Version = VersionMagic.GAR2;
                        break;
                    case "GAR\x05":
                        Version = VersionMagic.GAR5;
                        break;
                }

                FileSize = reader.ReadUInt32();
                FileGroupCount = reader.ReadUInt16();
                FileCount = reader.ReadUInt16();
                FileGroupOffset = reader.ReadUInt32();
                FileInfoOffset = reader.ReadUInt32();
                DataOffset = reader.ReadUInt32();
                Codename = reader.ReadString(0x08);

                switch (Codename)
                {
                    case "queen\0\0\0":
                    case "jenkins\0":
                        ReadZeldaArchive(reader, files);
                        break;
                    case "agora\0\0\0":
                    case "SYSTEM\0\0":
                        ReadGrezzoArchive(reader, files);
                        break;
                    default:
                        throw new Exception($"Unexpected codename! {Codename}");
                }
            }

            private void ReadGrezzoArchive(FileReader reader, List<FileEntry> files)
            {
              
            }

            private void ReadZeldaArchive(FileReader reader, List<FileEntry> files)
            {
                reader.SeekBegin(FileGroupOffset);
                for (int i = 0; i < FileGroupCount; i++)
                    FileGroups.Add(new FileGroup(reader));

                for (int i = 0; i < FileGroupCount; i++)
                    FileGroups[i].Ids = reader.ReadInt32s((int)FileGroups[i].FileCount);


                reader.SeekBegin(FileInfoOffset);
                for (int i = 0; i < FileGroupCount; i++)
                {
                    for (int f = 0; f < FileGroups[i].FileCount; f++)
                        GarFileInfos.Add(new GarFileInfo(reader));
                }

                reader.SeekBegin(DataOffset);
                uint[] Offsets = reader.ReadUInt32s(FileCount);
                for (int i = 0; i < GarFileInfos.Count; i++)
                {
                    files.Add(new FileEntry()
                    {
                        FileName = GarFileInfos[i].FileName,
                        FileData = reader.getSection(Offsets[i], GarFileInfos[i].FileSize)
                    });
                }
            }

            public class FileGroup
            {
                public uint FileCount;
                public uint DataOffset;
                public uint InfoOffset;

                public int[] Ids;

                public FileGroup(FileReader reader)
                {
                    FileCount = reader.ReadUInt32();
                    DataOffset = reader.ReadUInt32();
                    InfoOffset = reader.ReadUInt32();
                    reader.ReadUInt32();
                }
            }

            public class ZarFileInfo
            {
                public uint FileSize;
                public string FileName;

                public ZarFileInfo(FileReader reader)
                {
                    FileSize = reader.ReadUInt32();
                    FileName = reader.LoadString(false, typeof(uint));
                }
            }

            public class GarFileInfo
            {
                public uint FileSize;
                public string FileName;
                public string Name;

                public GarFileInfo(FileReader reader)
                {
                    FileSize = reader.ReadUInt32();
                    Name = reader.LoadString(false, typeof(uint));
                    FileName = reader.LoadString(false, typeof(uint));
                }
            }
        }

        public void Unload()
        {

        }

        public byte[] Save()
        {
            return null;
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
            public uint Unknown { get; set; }

            internal uint Size;
            internal uint Offset;
            internal uint NameOffset;

            public void Read(FileReader reader)
            {
                uint Unknown = reader.ReadUInt32();
                NameOffset = reader.ReadUInt32();
                Offset = reader.ReadUInt32();
                Size = reader.ReadUInt32();
            }
        }
    }
}
