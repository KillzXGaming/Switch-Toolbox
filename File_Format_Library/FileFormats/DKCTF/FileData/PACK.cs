using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace DKCTF
{
    /// <summary>
    /// Represents an package file format for storing file data.
    /// </summary>
    public class PACK : FileForm
    {
        public CFormDescriptor TocHeader;

        public List<DirectoryAssetEntry> Assets = new List<DirectoryAssetEntry>();
        public List<CNameTagEntry> NameTagEntries = new List<CNameTagEntry>();
        public Dictionary<string, uint> MetaOffsets = new Dictionary<string, uint>();

        public long MetaDataOffset;

        public PACK() { }

        public PACK(System.IO.Stream stream) : base(stream, true)
        {

        }

        public override void Read(FileReader reader)
        {
            TocHeader = reader.ReadStruct<CFormDescriptor>();
            long p = reader.Position;

            while (reader.BaseStream.Position < p + (long)TocHeader.DataSize)
            {
                var chunk = reader.ReadStruct<CChunkDescriptor>();
                var pos = reader.Position;

                reader.SeekBegin(pos + chunk.DataOffset);
                ReadChunk(reader, chunk);

                reader.SeekBegin(pos + chunk.DataSize);
            }
        }

        public override void ReadChunk(FileReader reader, CChunkDescriptor chunk)
        {
            switch (chunk.ChunkType)
            {
                case "TOCC":
                    TocHeader = reader.ReadStruct<CFormDescriptor>();
                    break;
                case "ADIR":
                    ReadAssetDirectoryChunk(reader);
                    break;
                case "META":
                    ReadMetaChunk(reader);
                    break;
                case "STRG":
                    ReadFileNameChunk(reader);
                    break;
            }
        }

        private void ReadAssetDirectoryChunk(FileReader reader)
        {
            uint numEntries = reader.ReadUInt32();
            for (int i = 0; i < numEntries; i++)
            {
                DirectoryAssetEntry entry = new DirectoryAssetEntry();
                entry.Read(reader, this.FileHeader);
                Assets.Add(entry);
            }
        }

        private void ReadMetaChunk(FileReader reader)
        {
            MetaDataOffset = reader.Position + 4;

            uint numEntries = reader.ReadUInt32();
            for (int i = 0; i < numEntries; i++)
            {
                var id = IOFileExtension.ReadID(reader);

                uint offset = reader.ReadUInt32();
                if (!MetaOffsets.ContainsKey(id.ToString()))
                    MetaOffsets.Add(id.ToString(), offset);
            }
        }

        private void ReadFileNameChunk(FileReader reader)
        {
            uint numEntries = reader.ReadUInt32();
            for (int i = 0; i < numEntries; i++)
            {
                string type = reader.ReadString(4, Encoding.ASCII);
                var id = IOFileExtension.ReadID(reader);

                string name = reader.ReadZeroTerminatedString();

                // reader.Align(4);
                NameTagEntries.Add(new CNameTagEntry()
                {
                    Name = name,
                    FileID = new CObjectTag()
                    {
                        Type = type,
                        Objectid = id,
                    },
                });
            }
        }

        public class DirectoryAssetEntry
        {
            public string Type;
            public CObjectId FileID;

            public long Offset;
            public long DecompressedSize;
            public long Size;

            public void Read(FileReader reader, CFormDescriptor header)
            {
                Type = reader.ReadString(4, Encoding.ASCII);
                FileID = IOFileExtension.ReadID(reader);
                if (header.VersionA >= 1 && header.VersionB >= 1)
                {
                    long flag1 = reader.ReadUInt32();
                    long flag2 = reader.ReadUInt32();
                    Offset = reader.ReadInt64();
                    DecompressedSize = reader.ReadInt64();
                    Size = reader.ReadInt64();
                }
                else
                {
                    Offset = reader.ReadInt64();
                    Size = reader.ReadInt64();
                    DecompressedSize = Size;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class MetaOffsetEntry
        {
            public CObjectId FileID;
            public uint FileOffset;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PakHeader
        {
            CFormDescriptor PackForm;
            CFormDescriptor TocForm;
        }

        public class CNameTagEntry
        {
            public CObjectTag FileID;
            public string Name;
        }
    }
}
