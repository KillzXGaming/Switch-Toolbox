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

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                return reader.ReadUInt32() == 0x55AA382D;
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

        public IEnumerable<ArchiveFileInfo> Files
        {
            get { return null; }
            set { }
        }

        public IEnumerable<INode> Nodes => nodes;

        public string Name
        {
            get { return FileName; }
            set { FileName = value; }
        }

        private readonly uint Magic = 0x55AA382D;

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                uint Signature = reader.ReadUInt32();
                uint FirstNodeOffset = reader.ReadUInt32();
                uint NodeSectionSize = reader.ReadUInt32();
                uint FileDataOffset = reader.ReadUInt32();
                byte[] Reserved = new byte[4];

                var RootNode = new NodeEntry();
                RootNode.Read(reader);
                nodes.Add(RootNode);
            }
        }

        public void SaveFile(FileWriter writer)
        {
            long pos = writer.Position;

            writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
            writer.Write(Magic);
        
        }

        public class NodeEntry : INode
        {
            public NodeType nodeType
            {
                get { return (NodeType)(flags >> 24); }
            }

            public enum NodeType
            {
                Directory,
                File,
            }

            private uint _stringPoolOffset
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

        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
            SaveFile(new FileWriter(mem));
            return mem.ToArray();
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
