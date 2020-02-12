using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class WTA : IFileFormat, IArchiveFile, ILeaveOpenOnLoad
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "WT Archive" };
        public string[] Extension { get; set; } = new string[] { "*.wta" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(3, "WTA");
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

        public List<FileInfo> files = new List<FileInfo>();

        public IEnumerable<ArchiveFileInfo> Files => files;

        public void ClearFiles() { files.Clear(); }

        public bool CanAddFiles { get; set; } = true;
        public bool CanRenameFiles { get; set; } = true;
        public bool CanReplaceFiles { get; set; } = true;
        public bool CanDeleteFiles { get; set; } = true;

        public Header ApakHeader;

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;
            using (var reader = new FileReader(stream, true))
            {
                ApakHeader = new WTA.Header(reader, files);
            }
        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream))
            {
                ApakHeader.Write(writer, files);
            }
        }

        public class Header
        {
            public ushort Version { get; set; }
            public bool IsBigEndian { get; set; }

            public uint Unknown1 { get; set; }
            public uint Unknown2 { get; set; }

            public Header(FileReader reader, List<FileInfo> files)
            {
                reader.SetByteOrder(false);

                uint magic = reader.ReadUInt32();
                uint Version = reader.ReadUInt32();
                if (Version != 1)
                    reader.SetByteOrder(true);
                reader.ReadUInt32(); //0
                uint alignment = reader.ReadUInt32();
                reader.ReadUInt32(); //slightly larger than file size
                reader.ReadUInt32(); //0
                uint dataOffset =  reader.ReadUInt32();
                uint stringTableOffset = reader.ReadUInt32();
                uint stringTableSize = reader.ReadUInt32();
                reader.ReadUInt32(); //64
                uint unkSectionCount = reader.ReadUInt32(); 
                uint FileCount = reader.ReadUInt32();
                reader.ReadUInt32(); //1
                reader.ReadUInt32(); //0
                reader.ReadUInt32(); //0
                reader.ReadUInt32(); //0

                //Skip an unknown section that is 64 bytes in size
                reader.Seek(unkSectionCount * 32);

                for (int i = 0; i < FileCount; i++)
                    files.Add(new FileInfo(reader, dataOffset, stringTableOffset));

                //Now read data and align offsets
                reader.SeekBegin(dataOffset);
                for (int i = 0; i < FileCount; i++)
                {
                    if (files[i].CompressedSize == 0)
                        continue;

                    files[i].FileName = $"File {i}";
                    files[i].DataOffset = reader.Position;
                    reader.Seek((int)files[i].CompressedSize);

                    if (files[i].Alignment != 0)
                        reader.Align((int)files[i].Alignment);
                }

                //Try to get file names from file formats inside
                //The string table for this file uses a bunch of ids and not very ideal for viewing
                for (int i = 0; i < FileCount; i++)
                {
                    if (files[i].CompressedSize == 0)
                        continue;

                    reader.SeekBegin(files[i].DataOffset);
                    var data = reader.ReadBytes((int)files[i].CompressedSize);
                    if (files[i].CompressedSize != files[i].UncompressedSize && data[0] == 0x78 && data[1] == 0x5E)
                        data = STLibraryCompression.ZLIB.Decompress(data);

                    using (var dataReader = new FileReader(data))
                    {
                        if (dataReader.CheckSignature(4, "FRES") || dataReader.CheckSignature(4, "BNTX"))
                        {
                            dataReader.SetByteOrder(false);
                            dataReader.SeekBegin(16);
                            uint fileNameOffset = dataReader.ReadUInt32();
                            dataReader.SeekBegin(fileNameOffset );
                            files[i].FileName = dataReader.ReadZeroTerminatedString();
                        }
                    }
                }
            }

            public void Write(FileWriter writer, List<FileInfo> files)
            {
                writer.SetByteOrder(IsBigEndian);
                writer.WriteSignature("WTA ");
            }
        }

        public class FileInfo : ArchiveFileInfo
        {
            public uint Alignment;
            public uint CompressedSize;
            public uint UncompressedSize;

            public long DataOffset;

            public override Stream FileDataStream
            {
                get { return DecompressData(); }
            }

            private Stream DecompressData()
            {
                using (var reader = new FileReader(baseStream, true))
                {
                    reader.SeekBegin(DataOffset);
                    var data = reader.ReadBytes((int)CompressedSize);
                    if (CompressedSize != UncompressedSize && data[0] == 0x78 && data[1] == 0x5E)
                        data = STLibraryCompression.ZLIB.Decompress(data);

                    return new MemoryStream(data);
                }
            }

            private Stream baseStream;

            public FileInfo(FileReader reader, uint dataOffset, uint stringTableOffset)
            {
                long pos = reader.Position;

                baseStream = reader.BaseStream;
                uint nameLength = reader.ReadUInt32();
                uint hash = reader.ReadUInt32();
                Alignment = reader.ReadUInt32();
                UncompressedSize = reader.ReadUInt32();
                CompressedSize = reader.ReadUInt32();
                uint nameOffset = reader.ReadUInt32();
                uint unk = reader.ReadUInt32();
                uint unk2 = reader.ReadUInt32();

                // FileName = reader.ReadString(0x20, true);
                // reader.ReadUInt32();

                long endpos = reader.Position;
                reader.Seek(endpos, System.IO.SeekOrigin.Begin);
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
    }
}
