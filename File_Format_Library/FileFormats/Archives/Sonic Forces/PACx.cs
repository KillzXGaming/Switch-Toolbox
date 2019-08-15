using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using System.Runtime.InteropServices;

namespace FirstPlugin
{
    public class PACx : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Sonic Forces / Tokyo Olympics 2020 Archive" };
        public string[] Extension { get; set; } = new string[] { "*.pac" };
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
                return reader.CheckSignature(8, "PACx301L") || 
                       reader.CheckSignature(8, "PACx302L") ||
                       reader.CheckSignature(8, "PACx402L");
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

        public string Name { get; set; }

        public void ClearFiles() { files.Clear(); }

        public uint Checksum;

        public static bool IsVersion4;

        public List<SplitEntry> SplitEntries = new List<SplitEntry>();
        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                IsVersion4 = reader.CheckSignature(8, "PACx402L");
                if (IsVersion4)
                {
                    var header = reader.ReadStruct<HeaderV4>();
                    var chunks = reader.ReadMultipleStructs<Chunk>(header.ChunkCount);
                    Checksum = header.PacID;

                    //Decompress each chunk now
                    reader.SeekBegin(header.RootOffset);
                    ReadRootPac(DecompressChunks(reader, chunks));

                    //Read splits from root pac
                    if (SplitEntries.Count > 0)
                    {
                        foreach (var pacSplit in SplitEntries)
                            ReadSplitPac(reader, pacSplit);
                    }
                }
                else
                {
                    var header3 = reader.ReadStruct<HeaderV3>();

                    PacNodeTree tree = new PacNodeTree();
                    tree.Read(reader, header3);

                    var rootNode = tree.RootNode;
                    LoadTree(rootNode);
                }
            }
        }

        public void ReadSplitPac(FileReader reader, SplitEntry entry)
        {
            reader.SeekBegin(entry.SplitOffset);
            ReadRootPac(DecompressChunks(reader, entry.Chunks), entry.Name);
        }

        public byte[] DecompressChunks(FileReader reader, List<Chunk> chunks)
        {
            List<byte[]> PacChunks = new List<byte[]>();
            for (int i = 0; i < chunks.Count; i++)
            {
                if (chunks[i].CompressedSize == chunks[i].UncompressedSize)
                {
                    PacChunks.Add(reader.ReadBytes((int)chunks[i].UncompressedSize));
                }
                else
                {
                    PacChunks.Add(STLibraryCompression.Type_LZ4.Decompress(
                     reader.ReadBytes((int)chunks[i].CompressedSize), 0,
                     (int)chunks[i].CompressedSize, (int)chunks[i].UncompressedSize));
                }
            }
            return Utils.CombineByteArray(PacChunks.ToArray());
        }

        public void ReadRootPac(byte[] buffer, string splitName = "")
        {
            using (var reader = new FileReader(new System.IO.MemoryStream(buffer)))
            {
                var header3 = reader.ReadStruct<HeaderV3>();

                PacNodeTree tree = new PacNodeTree();
                tree.Read(reader, header3);

                var rootNode = tree.RootNode;
                LoadTree(rootNode, splitName);

                if (header3.SplitCount != 0)
                {
                    //Read the split data if present 
                    reader.SeekBegin(48 + header3.NodesSize);
                    ulong splitCount = reader.ReadUInt64();
                    ulong splitEntriesOffset = reader.ReadUInt64();

                    reader.SeekBegin(splitEntriesOffset);
                    for (int i = 0; i < (int)splitCount; i++)
                    {
                        SplitEntry entry = new SplitEntry();
                        entry.SplitNameOffset = reader.ReadUInt64();
                        entry.SplitCompressedSize = reader.ReadUInt32();
                        entry.SplitUncompressedSize = reader.ReadUInt32();
                        entry.SplitOffset = reader.ReadUInt32();
                        entry.SplitChunkCount = reader.ReadUInt32();
                        entry.SplitChunksOffset = reader.ReadUInt32();

                        using (reader.TemporarySeek((long)entry.SplitNameOffset, System.IO.SeekOrigin.Begin))
                        {
                            entry.Name = reader.ReadZeroTerminatedString();
                        }
                        using (reader.TemporarySeek((long)entry.SplitChunksOffset, System.IO.SeekOrigin.Begin))
                        {
                            entry.Chunks = reader.ReadMultipleStructs<Chunk>(entry.SplitChunkCount);
                        }


                        Console.WriteLine("SplitName " + entry.Name);
                        Console.WriteLine("SplitCompressedSize " + entry.SplitCompressedSize);
                        Console.WriteLine("SplitUncompressedSize " + entry.SplitUncompressedSize);
                        Console.WriteLine("SplitOffset " + entry.SplitOffset);
                        Console.WriteLine("SplitChunkCount " + entry.SplitChunkCount);
                        Console.WriteLine("SplitChunksOffset " + entry.SplitChunksOffset);

                        SplitEntries.Add(entry);
                    }
                }
            }
        }

        //Documentation from https://pastebin.com/RSAbK46c
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class HeaderV4
        {
            public Magic8 Magic = "PACx402L";
            public uint PacID;
            public uint FileSize;
            public uint RootOffset;
            public uint RootCompressedSize;
            public uint RootUncompressedSize;
            public PacType Type = PacType.HasNoSplit;
            public ushort Constant = 0x208;
            public uint ChunkCount;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class Chunk
        {
            // When decompressing the root pac allocate a buffer of
            // size Header.RootUncompressedSize, then loop through these
            // chunks and decompress each one, one-by-one, into that buffer.

            // If you try to decompress all at once instead the data can be corrupted.
            public uint CompressedSize; //Compressed as LZ4 
            public uint UncompressedSize;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class HeaderV3
        {
            public Magic8 Magic = "PACx402L";
            public uint PacID;
            public uint FileSize;
            public uint NodesSize;
            public uint SplitsInfoSize;
            public uint DataEntriesSize;
            public uint StringTableSize;
            public uint DataSize;
            public uint OffsetTableSize;
            public PacType Type = PacType.HasNoSplit;
            public ushort Constant = 0x108;
            public uint SplitCount;
        }

        public class SplitEntry
        {
            public ulong SplitNameOffset;
            public uint SplitCompressedSize;
            public uint SplitUncompressedSize;
            public uint SplitOffset;
            public uint SplitChunkCount;
            public ulong SplitChunksOffset;

            public string Name;
            public List<Chunk> Chunks;
        }

        public class EmbeddedPAC
        {

        }

        public enum PacType : ushort
        {
            // PAC has no splits
            HasNoSplit = 1,

            // PAC is a split
            IsSplit = 2,

            // PAC has splits
            HasSplit = 5
        }

        public void LoadTree(PacNode node, string fullPath = "")
        {
            bool IsFile = node.HasData && node.Data != null;

            if (IsFile)
            {
                FileEntry newNode = new FileEntry(node);
                newNode.FileName = $"{fullPath}.{newNode.Name}";
                files.Add(newNode);
            }

            if (node.Name != "Node" && node.Name != null)
                fullPath += $"/{node.Name}";

            for (int i = 0; i < node.Children.Count; i++)
                LoadTree(node.Children[i], fullPath);
        }

        public class FileEntry : ArchiveFileInfo
        {
            public FileEntry(PacNode node)
            {
                Name = node.Name;
                FileData = node.Data;
                if (node.Name == null) Name = "File Node";
                if (FileData == null) FileData = new byte[0];
            }
        }

        public class PacNodeTree
        {
            public PacNode RootNode { get; set; }

            public ulong rootNodeOffset;

            public void Read(FileReader reader, HeaderV3 pac)
            {
                uint nodeCount = reader.ReadUInt32();
                uint dataNodeCount = reader.ReadUInt32();
                rootNodeOffset = reader.ReadUInt64();
                ulong dataNodeIndicesOffset = reader.ReadUInt64();

                reader.SeekBegin(rootNodeOffset);
                RootNode = new PacNode(pac, this);
                RootNode.Read(reader);
            }
        }

        public class PacNode
        {
            public HeaderV3 PacFile;

            public byte[] Data;

            public PacNodeTree ParentTree;
            public string Name { get; set; }
            public bool HasData { get; set; }
            public List<PacNode> Children = new List<PacNode>();
            public DataType DataType;

            public PacNode(HeaderV3 pac, PacNodeTree tree)
            {
                PacFile = pac;
                ParentTree = tree;
            }

            public void Read(FileReader reader)
            {
                ulong nameOffset = reader.ReadUInt64();
                ulong dataOffset = reader.ReadUInt64();
                ulong childIndicesOffset = reader.ReadUInt64();
                int parentIndex = reader.ReadInt32();
                int index = reader.ReadInt32();
                int dataIndex = reader.ReadInt32();
                ushort childCount = reader.ReadUInt16();
                HasData = reader.ReadBoolean();
                byte fullPathSize = reader.ReadByte();

                Console.WriteLine($"nameOffset {nameOffset}");
                Console.WriteLine($"dataOffset {dataOffset}");
                Console.WriteLine($"childIndicesOffset {childIndicesOffset}");
                Console.WriteLine($"parentIndex {parentIndex}");
                Console.WriteLine($"index {index}");
                Console.WriteLine($"dataIndex {dataIndex}");
                Console.WriteLine($"childCount {childCount}");
                Console.WriteLine($"HasData {HasData}");

                if (nameOffset == 5490503897632162128)
                    return;

                if (nameOffset != 0)
                {
                    reader.SeekBegin((long)nameOffset);
                    Name = reader.ReadZeroTerminatedString();
                }
                if (dataOffset != 0)
                {
                    reader.SeekBegin((long)dataOffset);
                    uint PacID = reader.ReadUInt32();

                    //Detecting data in v4 is a pain
                    //Check if the offset is set within the data section
                    var dataPos = 48 + PacFile.NodesSize + PacFile.SplitsInfoSize;

                    if (PacID == PacFile.PacID || IsVersion4 &&  dataOffset >= dataPos)
                    {
                        ulong dataSize = reader.ReadUInt64();
                        uint padding = reader.ReadUInt32();
                        ulong dataBlockOffset = reader.ReadUInt64();
                        ulong padding2 = reader.ReadUInt64();
                        ulong extensionOffset = reader.ReadUInt64();
                        DataType = reader.ReadEnum<DataType>(false);

                        if (extensionOffset != 0)
                        {
                            reader.SeekBegin((long)extensionOffset);
                            string extension = reader.ReadZeroTerminatedString();
                            Name += extension;
                        }

                        if (dataBlockOffset != 0)
                        {
                            reader.SeekBegin((long)dataBlockOffset);
                            Data = reader.ReadBytes((int)dataSize);
                        }
                    }
                    else
                    {
                        reader.SeekBegin((long)dataOffset);
                        PacNodeTree tree = new PacNodeTree();
                        tree.Read(reader, PacFile);
                        Children.Add(tree.RootNode);
                    }
                }
                if (childIndicesOffset != 0)
                {
                    reader.SeekBegin((long)childIndicesOffset);
                    int[] Indices = reader.ReadInt32s(childCount);
                    for (int i = 0; i < childCount; i++)
                    {
                        int childIndex = Indices[i];
                        reader.SeekBegin((uint)ParentTree.rootNodeOffset + (childIndex * 40));
                        PacNode node = new PacNode(PacFile, ParentTree);
                        node.Read(reader);
                        Children.Add(node);
                    }
                }
            }
        }

        public enum DataType : ulong
        {
            RegularFile = 0,
            NotHere = 1,
            BINAFile = 2
        }

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
    }
}
