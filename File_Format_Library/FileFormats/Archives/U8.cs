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
    public class U8 : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "U8" };
        public string[] Extension { get; set; } = new string[] { "*.u8", "*.arc", "*.cmparc"};
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        private readonly uint BEMagic = 0x55AA382D;
        private readonly uint LEMagic = 0x2D38AA55;
        private int LZType = 0x0, LZSize = 0;

        public bool Identify(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                byte LZCheck = reader.ReadByte();
                if (LZCheck == 0x11 || LZCheck == 0x10)
                {
                    LZType = LZCheck;

                    // For the Wii's U8 ARC files compressed with LZ77 Type 10 or Type 11, the decompiled file size is written in little endian.
                    reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                    LZSize = reader.ReadInt32();
                    reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                }
                else
                {
                    reader.Position = 0;
                }

                uint signature = reader.ReadUInt32();

                reader.Position = 0;
                return signature == BEMagic || signature == LEMagic;
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

        public string Name
        {
            get { return FileName; }
            set { FileName = value; }
        }


        private bool IsBigEndian = false;

        public void Load(Stream stream)
        {
            SubStream sub;
            if (LZType == 0x11 || LZType == 0x10)
            {
                sub = new SubStream(stream, 4);
            }
            else
            {
                sub = null;
            }

            Stream dataStream = 
                LZType == 0x11 ? new MemoryStream(LZ77_WII.Decompress11(sub.ToArray(), LZSize)) : 
                LZType == 0x10 ? new MemoryStream(LZ77_WII.Decompress10LZ(sub.ToArray(), LZSize)) : stream;
            
            dataStream.Position = 0;

            using (var reader = new FileReader(dataStream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                uint Signature = reader.ReadUInt32();
                IsBigEndian = Signature == BEMagic;

                reader.SetByteOrder(IsBigEndian);
                uint FirstNodeOffset = reader.ReadUInt32();
                uint NodeSectionSize = reader.ReadUInt32();
                uint FileDataOffset = reader.ReadUInt32();
                byte[] Reserved = new byte[4];

                Console.WriteLine("FirstNodeOffset " + FirstNodeOffset);

                reader.SeekBegin(FirstNodeOffset);
                var RootNode = new NodeEntry();
                RootNode.Read(reader);

                //Root has total number of nodes 
                uint TotalNodeCount = RootNode.Setting2;

                //Read all our entries
                List<NodeEntry> entries = new List<NodeEntry>();
                entries.Add(RootNode);
                for (int i = 0; i < TotalNodeCount - 1; i++)
                {
                    var node = new NodeEntry();
                    node.Read(reader);
                    entries.Add(node);
                }

                //Read string pool
                uint stringPoolPos = 0;
                Dictionary<uint, string> StringTable = new Dictionary<uint, string>();
                for (int i = 0; i < TotalNodeCount; i++)
                {
                    string str = reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated, Encoding.ASCII);
                    StringTable.Add(stringPoolPos, str);
                    stringPoolPos += (uint)str.Length + 1;
                }

                //Set the strings
                for (int i = 0; i < TotalNodeCount; i++) {
                    entries[i].Name = StringTable[entries[i].StringPoolOffset];
                }

                entries[0].Name = "Root";

                SetFileNames(entries, 1, entries.Count, "");

                for (int i = 0; i < entries.Count; i++)
                {
                    if (entries[i].nodeType != NodeEntry.NodeType.Directory)
                    {
                        FileEntry entry = new FileEntry();
                        reader.SeekBegin(entries[i].Setting1);
                        entry.FileData = reader.ReadBytes((int)entries[i].Setting2);
                        entry.FileName = entries[i].FullPath;
                        files.Add(entry);
                    }
                }
            }
        }

        private int SetFileNames(List<NodeEntry> fileEntries, int firstIndex, int lastIndex, string directory)
        {
            int currentIndex = firstIndex;
            while (currentIndex < lastIndex)
            {
                NodeEntry entry = fileEntries[currentIndex];
                string filename = entry.Name;
                entry.FullPath = directory + filename;

                if (entry.nodeType == NodeEntry.NodeType.Directory)
                {
                    entry.FullPath += "/";
                    currentIndex = SetFileNames(fileEntries, currentIndex + 1, (int)entry.Setting2, entry.FullPath);
                }
                else
                {
                    ++currentIndex;
                }
            }

            return currentIndex;
        }

        public void SaveFile(FileWriter writer)
        {
            writer.SetByteOrder(IsBigEndian);

            long pos = writer.Position;
            writer.Write(BEMagic);
        
        }

        public class FileEntry : ArchiveFileInfo
        {
            public NodeEntry nodeEntry;
        }

        public class DirectoryEntry : IDirectoryContainer
        {
            public NodeEntry nodeEntry;

            public string Name { get; set; }

            public IEnumerable<INode> Nodes { get { return nodes; } }
            public List<INode> nodes = new List<INode>();

            public void AddNode(INode node)
            {
                nodes.Add(node);
            }
        }

        public class NodeEntry : INode
        {
            public string FullPath { get; set; }

            public NodeType nodeType
            {
                get { return (NodeType)(flags >> 24); }
            }

            public enum NodeType
            {
                File,
                Directory,
            }

            public uint StringPoolOffset
            {
                get { return flags & 0x00ffffff; }
            }

            private uint flags;

            public uint Setting1; //Offset (file) or parent index (directory)
            public uint Setting2; //Size (file) or node count (directory)

            public string Name { get; set; }

            public void Read(FileReader reader)
            {
                flags = reader.ReadUInt32();
                Setting1 = reader.ReadUInt32();
                Setting2 = reader.ReadUInt32();
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
            SaveFile(new FileWriter(stream));
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
