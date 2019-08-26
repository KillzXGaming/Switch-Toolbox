using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using System.Runtime.InteropServices;
using Toolbox.Library.IO;
using AudioLibrary;

namespace FirstPlugin
{
    public class BFGRP : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Audio;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Audio Group" };
        public string[] Extension { get; set; } = new string[] { "*.bfgrp" };
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
                return reader.CheckSignature(4, "FGRP");
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

        public List<AudioEntry> files = new List<AudioEntry>();
        public IEnumerable<ArchiveFileInfo> Files => files;

        public void ClearFiles() { files.Clear(); }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo) {
            return false;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo) {
            return false;
        }

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.SetByteOrder(true);
                var header = reader.ReadStruct<AudioHeader>();

                Console.WriteLine($"Magic " + header.Magic);
                Console.WriteLine($"HeaderSize " + header.HeaderSize);
                Console.WriteLine($"Version " + header.Version);
                Console.WriteLine($"BlockCount " + header.BlockCount);
                Console.WriteLine($"ByteOrder " + header.ByteOrder);
     
                var blocks = reader.ReadMultipleStructs<BlockReference>(header.BlockCount);
                for (int i = 0; i < blocks.Count; i++)
                {
                    Console.WriteLine($"Offset " + blocks[i].Offset);

                    reader.SeekBegin(blocks[i].Offset);
                    switch (blocks[i].Type)
                    {
                        case AudioBlockType.InfoBlockType:
                            ParseInfoBlock(reader);
                            break;
                        case AudioBlockType.InfoBlockType2:
                            ParseInfoBlock(reader);
                            break;
                    }
                }
            }
        }

        private void ParseInfoBlock(FileReader reader)
        {
            long basePos = reader.Position;

            var infoBlock = reader.ReadStruct<InfoBlock>();
            var references = reader.ReadMultipleStructs<SectionReference>(infoBlock.NumEntries);

            for (int i = 0; i < infoBlock.NumEntries; i++)
            {
                reader.SeekBegin(basePos + (long)references[i].Offset + (i * 16));
                LocationInfo info = new LocationInfo();
                info.Read(reader);

                files.Add(new AudioEntry()
                {
                    FileName = $"File {i}",
                    FileData = info.FileData.Data,
                });
            }
        }

        public class AudioEntry : ArchiveFileInfo
        {

        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class InfoBlock
        {
            public Magic Magic = "INFO";
            public uint BlockSize;
            public uint NumEntries;
        }

        public class LocationInfo : IAudioData
        { 
            public uint FileIndex;
            public uint FileSize;

            public FileInfo FileData;

            public void Read(FileReader reader)
            {
                long basePos = reader.Position; 

                FileIndex = reader.ReadUInt32();
                FileData = reader.ReadReference<FileInfo>(basePos);
                FileSize = reader.ReadUInt32();
            }

            public void Write(FileWriter writer)
            {

            }
        }

        public class FileInfo : IAudioData
        {
            public byte[] Data;

            public void Read(FileReader reader)
            {
                reader.ReadSignature(4,"FILE");
                uint blockSize = reader.ReadUInt32();
                Data = reader.ReadBytes((int)blockSize);
            }

            public void Write(FileWriter writer)
            {

            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class FileBlock
        {
            public Magic Magic = "FILE";
            public uint BlockSize;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class InfoExBlock
        {
            public Magic Magic = "INFX";
            public uint BlockSize;
            public ulong DependencyInfoOffset;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class DependencyInfo
        {
            public uint ItemID;
            public uint Flags;
        }
    }
}
