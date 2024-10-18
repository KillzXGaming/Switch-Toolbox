using System;
using Syroot.BinaryData;
using System.IO;
using System.Text;

namespace BezelEngineArchive_Lib
{
    public class ASST : IFileData //File asset
    {
        private const string _signature = "ASST";

        public ushort unk = 2;
        public ushort unk2 = 12;
        public string FileName;
        public long UncompressedSize;
        public bool IsCompressed = true;

        public ulong FileID1;
        public ulong FileID2;

        public uint FileHash;
        public uint Unknown3;

        public byte[] FileData;

        public uint FileSize;
        public long FileOffset;

        public string FileType = "";

        public BezelEngineArchive ParentArchive;

        void IFileData.Load(FileLoader loader)
        {
            loader.CheckSignature(_signature);
            loader.LoadBlockHeader();
            unk = loader.ReadUInt16();
            unk2 = loader.ReadUInt16();
            FileSize = loader.ReadUInt32();
            UncompressedSize = loader.ReadUInt32();

            if (loader.Archive.VersionMajor2 >= 5)
            {
                FileType = Encoding.ASCII.GetString(loader.ReadBytes(8));
            }

            Unknown3 = loader.ReadUInt32();

            if (loader.Archive.VersionMajor2 >= 6)
            {
                FileID1 = loader.ReadUInt64();
                FileID2 = loader.ReadUInt64();
            }

            FileOffset = loader.ReadInt64();
            FileName = loader.LoadString();

            using (loader.TemporarySeek(FileOffset, SeekOrigin.Begin)) {
                FileData = loader.ReadBytes((int)FileSize);
            }

            if (UncompressedSize == FileSize)
                IsCompressed = false;
        }
        void IFileData.Save(FileSaver saver)
        {
            saver.WriteSignature(_signature);
            saver.SaveBlockHeader();
            saver.Write(unk);
            saver.Write(unk2);
            saver.Write((uint)FileData.Length);
            saver.Write((uint)UncompressedSize);

            if (saver.Archive.VersionMajor2 >= 5)
            {
                if (FileType.Length > 8)
                    throw new Exception($"Invalid file type length! Must be equal or less than 12 bytes! {FileType}!");

                saver.Write(Encoding.ASCII.GetBytes(FileType));
                saver.Write(new byte[8 - FileType.Length]);
            }

            saver.Write(Unknown3);

            if (saver.Archive.VersionMajor2 >= 6)
            {
                saver.Write(FileID1);
                saver.Write(FileID2);
            }

            saver.SaveBlock(FileData, (uint)saver.Archive.RawAlignment, () => saver.Write(FileData));
            saver.SaveRelocateEntryToSection(saver.Position, 1, 1, 0, 1, "Asst File Name Offset");
            saver.SaveString(FileName);
        }
    }
}
