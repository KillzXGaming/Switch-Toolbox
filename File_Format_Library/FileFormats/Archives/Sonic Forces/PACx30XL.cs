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
    public class PACx30XL : IArchiveFile, IFileFormat, IDirectoryContainer
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Sonic Forces PAC" };
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
                return reader.CheckSignature(8, "PACx301L") || reader.CheckSignature(8, "PACx302L");
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

        public List<INode> nodes = new List<INode>();
        public IEnumerable<INode> Nodes => nodes;

        public string Name { get; set; }

        public void ClearFiles() { files.Clear(); }

        public uint Checksum;

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                bool IsVersion2 = reader.CheckSignature(8, "PACx402L");
                reader.SeekBegin(8);

                Checksum = reader.ReadUInt32();
                uint FileSize = reader.ReadUInt32();

                if (IsVersion2)
                {

                }
                else
                {
                    uint NodeSectionSize = reader.ReadUInt32();
                    uint PacDependsSectionSize = reader.ReadUInt32();
                    uint EntrySectionSize = reader.ReadUInt32();
                    uint StringTableSize = reader.ReadUInt32();
                    uint DataSectionSize = reader.ReadUInt32();
                    uint OffsetTableSize = reader.ReadUInt32();
                    ushort PacType = reader.ReadUInt16();
                    ushort constant = reader.ReadUInt16();
                    uint dependPacCount = reader.ReadUInt32();
                    PacNodeTree tree = new PacNodeTree();
                    tree.Read(reader, this);

                    var rootNode = tree.RootNode;
                    var dirRoot = new DirectoryEntry(rootNode);
                    LoadTree(rootNode, dirRoot);
                    nodes.Add(dirRoot);
                }
            }
        }

        public void LoadTree(PacNode node, DirectoryEntry parentNode)
        {
            INode newNode = null;
            if (node.HasData  && node.Data != null)
                newNode = new FileEntry(node);
            else
                newNode = new DirectoryEntry(node);

            parentNode.nodes.Add(newNode);
            for (int i = 0; i < node.Children.Count; i++)
            {
                LoadTree(node.Children[i], (DirectoryEntry)newNode);
            }
        }

        public class DirectoryEntry : IDirectoryContainer
        {
            public string Name { get; set; }

            public List<INode> nodes = new List<INode>();
            public IEnumerable<INode> Nodes => nodes;

            public DirectoryEntry(PacNode node)
            {
                Name = node.Name;
                if (node.Name == null) Name = "Node";
            }
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

            public uint rootNodeOffset;

            public void Read(FileReader reader, PACx30XL pac)
            {
                uint nodeCount = reader.ReadUInt32();
                uint dataNodeCount = reader.ReadUInt32();
                rootNodeOffset = reader.ReadUInt32();
                uint dataNodeIndicesOffset = reader.ReadUInt32();

                reader.SeekBegin(rootNodeOffset);
                RootNode = new PacNode(pac, this);
                RootNode.Read(reader);
            }
        }

        public class PacNode
        {
            public PACx30XL PacFile;

            public byte[] Data;

            public PacNodeTree ParentTree;

            public string Name { get; set; }

            public bool HasData { get; set; }

            public List<PacNode> Children = new List<PacNode>();

            public PacNode(PACx30XL pac, PacNodeTree tree)
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

                if (nameOffset != 0)
                {
                    reader.SeekBegin((long)nameOffset);
                    Name = reader.ReadZeroTerminatedString();
                }
                if (dataOffset != 0)
                {
                    reader.SeekBegin((long)dataOffset);
                    if (reader.ReadUInt32() == PacFile.Checksum)
                    {
                        uint dataSize = reader.ReadUInt32();
                        ulong padding = reader.ReadUInt64();
                        ulong dataBlockOffset = reader.ReadUInt64();
                        ulong padding2 = reader.ReadUInt64();
                        ulong extensionOffset = reader.ReadUInt64();
                        ulong dataType = reader.ReadUInt32();

                        reader.SeekBegin((long)extensionOffset);
                        string extension = reader.ReadZeroTerminatedString();
                        Name += extension;

                        reader.SeekBegin((long)dataBlockOffset);
                        Data = reader.ReadBytes((int)dataSize);
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
                    for (int i =0; i < childCount; i++)
                    {
                        int childIndex = Indices[i];
                        reader.SeekBegin(ParentTree.rootNodeOffset + (childIndex * 40));
                        PacNode node = new PacNode(PacFile, ParentTree);
                        node.Read(reader);
                        Children.Add(node);
                    }
                }
            }
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
