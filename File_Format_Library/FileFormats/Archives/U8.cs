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
    public class U8 : IArchiveFile, IFileFormat, IDirectoryContainer
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "U8" };
        public string[] Extension { get; set; } = new string[] { "*.u8"};
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        private readonly uint BEMagic = 0x55AA382D;
        private readonly uint LEMagic = 0x2D38AA55;

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

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

        public List<INode> nodes = new List<INode>();
        public List<FileEntry> files = new List<FileEntry>();

        public IEnumerable<ArchiveFileInfo> Files => files;
        public IEnumerable<INode> Nodes => nodes;

        public void ClearFiles() { nodes.Clear(); }

        public string Name
        {
            get { return FileName; }
            set { FileName = value; }
        }


        private bool IsBigEndian = false;

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
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
                for (int i = 0; i < TotalNodeCount; i++)
                {
                    entries[i].Name = StringTable[entries[i].StringPoolOffset];
                }

                entries[0].Name = "Root";

               //Setup our directory entries for loading to the tree
               DirectoryEntry[] dirs = new DirectoryEntry[TotalNodeCount];
                for (int i = 0; i < dirs.Length; i++)
                    dirs[i] = new DirectoryEntry();

                DirectoryEntry currentDir = dirs[1];
                nodes.Add(dirs[0]);

                //Skip root so start index at 1
                int dirIndex = 1;
                for (int i = 0; i < TotalNodeCount; i++)
                {
                    var node = entries[i];
                    if (node.Name == string.Empty)
                        continue;

                    Console.WriteLine($"{ node.Name} {i} {node.nodeType} {node.Setting1}");

                    if (node.nodeType == NodeEntry.NodeType.Directory)
                    {
                        dirs[i].Name = node.Name;
                        dirs[i].nodeEntry = node;
                        currentDir = dirs[i];

                        if (i != 0)
                            dirs[node.Setting1].AddNode(currentDir);
                    }
                    else
                    {
                        FileEntry entry = new FileEntry();
                        entry.FileName = node.Name;
                        entry.Name = node.Name;
                        entry.nodeEntry = node;
                        currentDir.nodes.Add(entry);

                        reader.SeekBegin(entry.nodeEntry.Setting1);
                        entry.FileData = reader.ReadBytes((int)entry.nodeEntry.Setting2);
                        files.Add(entry);
                    }
                }
            }
        }

        public void SaveFile(FileWriter writer)
        {
            writer.SetByteOrder(IsBigEndian);

            long pos = writer.Position;
            writer.Write(BEMagic);
        
        }

        public class FileEntry : ArchiveFileInfo
        {
            public override bool OpenFileFormatOnLoad
            {
                get { return true; }
            }

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
