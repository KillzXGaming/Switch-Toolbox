using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;

namespace FirstPlugin
{
    public class SDF : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Snow Engine Data Table Of Contents" };
        public string[] Extension { get; set; } = new string[] { "*.sdftoc" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "WEST");
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

        SDFTOC_Header Header;
        SDFTOC_ID startId;
        int[] block1;
        SDFTOC_ID[] blockIds;
        SDFTOC_Block2[] block2Array;
        byte[] DecompressedBlock;
        SDFTOC_ID endId;

        List<string> FilePaths = new List<string>();

        //Thanks to https://github.com/GoldFarmer/rouge_sdf/blob/master/main.cpp for docs/structs
        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

                //Read header
                Header = new SDFTOC_Header();
                Header.Read(reader);

                //Read first id
                startId = new SDFTOC_ID(reader);

                //Check this flag
                byte Flag1 = reader.ReadByte();
                if (Flag1 != 0)
                {
                    byte[] unk = reader.ReadBytes(0x140);
                }

                //Read first block
                block1 = reader.ReadInt32s((int)Header.Block1Count);

                //Read ID blocks
                blockIds = new SDFTOC_ID[Header.Block1Count];
                for (int i = 0; i < Header.Block1Count; i++)
                {
                    blockIds[i] = new SDFTOC_ID(reader);
                }

                //Read block 2 (DDS headers)
                block2Array = new SDFTOC_Block2[Header.Block2Count];
                for (int i = 0; i < Header.Block2Count; i++)
                {
                    block2Array[i] = new SDFTOC_Block2(reader, Header);
                }

                //Here is the compressed block. Check the magic first
                uint magic = reader.ReadUInt32();
                reader.Seek(-4, SeekOrigin.Current);

                //Read and decompress the compressed block
                //Contains file names and block info
                DecompressNameBlock(magic, reader.ReadBytes((int)Header.CompressedSize), Header);

                //Read last id 
                endId = new SDFTOC_ID(reader);

                Text = FileName;

                LoadTree();
            }
        }

        private void LoadTree()
        {
            // Get a list of everything under the users' temp folder as an example
            string[] fileList;
            fileList = FilePaths.ToArray();

            // Parse the file list into a TreeNode collection
                TreeNode node = GetNodes(new TreeNode(), fileList);
                Nodes.Add(node); // Add the new nodes

            // Copy the new nodes to an array
               int nodeCount = node.Nodes.Count;
              TreeNode[] nodes = new TreeNode[nodeCount];
              node.Nodes.CopyTo(nodes, 0);

             Nodes.AddRange(nodes); // Add the new nodes
        }

        private TreeNode GetNodes(TreeNode parent, string[] fileList)
        {
            // build a TreeNode collection from the file list
            foreach (string strPath in fileList)
            {
                // Every time we parse a new file path, we start at the top level again
                TreeNode thisParent = parent;

                // split the file path into pieces at every backslash
                foreach (string pathPart in strPath.Split('\\'))
                {
                    // check if we already have a node for this
                    TreeNode[] tn = thisParent.Nodes.Find(pathPart, false);

                    if (tn.Length == 0)
                    {
                        // no node found, so add one
                        thisParent = thisParent.Nodes.Add(pathPart, pathPart);
                    }
                    else
                    {
                        // we already have this node, so use it as the parent of the next part of the path
                        thisParent = tn[0];
                    }
                }

            }
            return parent;
        }

        void FillTreeNodes(TreeNode root, List<string> files)
        {
            var rootText = root.Text;
            var rootTextLength = rootText.Length;
            var nodeStrings = files;
            foreach (var node in nodeStrings)
            {
                string nodeString = node;
                nodeString = nodeString.Replace(@"\", "/");

                var roots = nodeString.Split(new char[] { '/' },
                    StringSplitOptions.RemoveEmptyEntries);

                // The initial parent is the root node
                var parentNode = root;
                var sb = new StringBuilder(rootText, nodeString.Length + rootTextLength);
                for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
                {
                    // Build the node name
                    var parentName = roots[rootIndex];
                    sb.Append("/");
                    sb.Append(parentName);
                    var nodeName = sb.ToString();

                    // Search for the node
                    var index = parentNode.Nodes.IndexOfKey(nodeName);
                    if (index == -1)
                    {
                        // Node was not found, add it

                        var temp = new TreeNode(parentName, 0, 0);
                        if (rootIndex == roots.Length - 1)
                            temp = new TreeNode(parentName); //File entry

                        temp.Name = nodeName;
                        parentNode.Nodes.Add(temp);
                        parentNode = temp;
                    }
                    else
                    {
                        // Node was found, set that as parent and continue
                        parentNode = parentNode.Nodes[index];
                    }
                }
            }
        }

        public void DecompressNameBlock(uint magic, byte[] CompressedBlock, SDFTOC_Header header)
        {
            byte[] decomp = null;
            if (magic == 0xDFF25B82 || magic == 0xFD2FB528)
                decomp = STLibraryCompression.ZSTD.Decompress(CompressedBlock);
            else if (header.Version > 22)
                decomp = STLibraryCompression.Type_LZ4.Decompress(CompressedBlock);
            else
                decomp = STLibraryCompression.ZLIB.Decompress(CompressedBlock);

            //Now it's decompressed lets parse!
            using (var reader = new FileReader(decomp))
            {
                ParseNames(reader);
            }
        }

        private ulong readVariadicInteger(int Count, FileReader reader)
        {
            ulong result = 0;

            for (int i = 0; i < Count; i++)
            {
                result |= (ulong)(reader.ReadByte()) << (i * 8);
            }
            return result;
        }

        public void ParseNames(FileReader reader, string Name = "")
        {
            if (!Name.Contains("dummy") && FilePaths.Count < 200)
                FilePaths.Add(Name);

            char ch = reader.ReadChar();

            if (ch == 0)
                throw new Exception("Unexcepted byte in file tree");

            if (ch >= 1 && ch <= 0x1f) //string part
            {
                while (ch-- > 0)
                {
                    Name += reader.ReadChar();
                }
                ParseNames(reader, Name);
            }
            else if (ch >= 'A' && ch <= 'Z') //file entry
            {
               int var = Convert.ToInt32(ch - 'A');

                ch = Convert.ToChar(var);
                int count1 = ch & 7;
                int flag1 = (ch >> 3) & 1;
             //   int flag1 = ch & 8;

                if (count1 != 0)
                {
                    uint strangeId = reader.ReadUInt32();
                    byte chr2 = reader.ReadByte();
                    int byteCount = chr2 & 3;
                    int byteValue = chr2 >> 2;
                    ulong DdsType = readVariadicInteger(byteCount, reader);

                    for (int chunkIndex = 0; chunkIndex < count1; chunkIndex++)
                    {
                        byte ch3 = reader.ReadByte();
                        if (ch3 == 0)
                        {
                            break;
                        }

                        int compressedSizeByteCount = (ch3 & 3) + 1;
                        int packageOffsetByteCount = (ch3 >> 2) & 7;
                        bool hasCompression = ((ch3 >> 5) & 1) != 0;

                        ulong decompressedSize =0;
                        ulong compressedSize = 0;
                        ulong packageOffset = 0;
                        long fileId = -1;

                        if (compressedSizeByteCount > 0)
                        {
                            decompressedSize = readVariadicInteger(compressedSizeByteCount, reader);
                        }
                        if (hasCompression)
                        {
                            compressedSize = readVariadicInteger(compressedSizeByteCount, reader);
                        }
                        if (packageOffsetByteCount != 0)
                        {
                            packageOffset = readVariadicInteger(packageOffsetByteCount, reader);
                        }

                        ulong packageId = readVariadicInteger(2, reader);

                        if (packageId >= Header.Block1Count)
                        {
                            throw new InvalidDataException("SDF Package ID outside of TOC range");
                        }


                        List<ulong> compSizeArray = new List<ulong>();

                        if (hasCompression)
                        {
                              ulong pageCount = (decompressedSize + 0xffff) >> 16;
                         //   var pageCount = NextMultiple(decompressedSize, 0x10000) / 0x10000;
                            if (pageCount > 1)
                            {
                                for (ulong page = 0; page < pageCount; page++)
                                {
                                    ulong compSize = readVariadicInteger(2, reader);
                                    compSizeArray.Add(compSize);
                                }
                            }
                        }

                        if (Header.Version <= 0x16)
                        {
                            fileId = (long)readVariadicInteger(4, reader);
                        }   

                        if (compSizeArray.Count == 0 && hasCompression)
                            compSizeArray.Add(compressedSize);

                        DumpFile(Name, packageId, packageOffset, decompressedSize, compSizeArray, DdsType, chunkIndex != 0, byteCount != 0 && chunkIndex == 0);
                    }
                }
                if ((ch & 8) != 0) //flag1
                {
                    byte ch3 = reader.ReadByte();
                    while (ch3-- > 0)
                    {
                        reader.ReadByte();
                        reader.ReadByte();
                    }
                }
            }
            else
            {
                uint offset = reader.ReadUInt32();
                ParseNames(reader, Name);
                reader.Seek(offset, SeekOrigin.Begin);
                ParseNames(reader, Name);
            }
        }

        public static ulong NextMultiple(ulong value, ulong multiple) => NextMultiple((long)value, multiple);
        public static ulong NextMultiple(long value, ulong multiple)
        {
            return (ulong)Math.Ceiling(value / (double)multiple) * multiple;
        }

        public void DumpFile(string Name, ulong packageId, ulong packageOffset, ulong decompresedSize,
      List<ulong> compressedSize, ulong ddsType, bool Append, bool UseDDS)
        {
            string PathFolder = Path.GetDirectoryName(FileName);

            string layer;
            Console.WriteLine(Name + " " + packageId + " " + packageOffset + " " + decompresedSize + " " + ddsType + " " + UseDDS);
            if (packageId < 1000)
            {
                layer = "A";
            }
            else if (packageId < 2000)
            {
                layer = "B";
            }
            else
            {
                layer = "C";
            }

        }

        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }

        public class SDFTOC_Header
        {
            public uint Version { get; set; }
            public uint DecompressedSize { get; set; }
            public uint CompressedSize { get; set; }
            public uint Zero { get; set; }
            public uint Block1Count { get; set; }
            public uint Block2Count { get; set; }

            public void Read(FileReader reader)
            {
                reader.CheckSignature(4, "WEST");
                reader.Seek(4, System.IO.SeekOrigin.Begin);
                Version = reader.ReadUInt32();
                DecompressedSize = reader.ReadUInt32();
                CompressedSize = reader.ReadUInt32();
                Zero = reader.ReadUInt32();
                Block1Count = reader.ReadUInt32();
                Block2Count = reader.ReadUInt32();
            }
        }
        public class SDFTOC_ID
        {
            public ulong ubisoft { get; set; }
            public byte[] Data { get; set; }
            public ulong massive { get; set; }

            public SDFTOC_ID(FileReader reader)
            {
                ubisoft = reader.ReadUInt64();
                Data = reader.ReadBytes(0x20);
                massive = reader.ReadUInt64();
            }
        }
        public class SDFTOC_Block2 //Seems to be for DDS headers
        {
            public uint UsedBytes { get; set; }
            public byte[] Data { get; set; }

            public SDFTOC_Block2(FileReader reader, SDFTOC_Header header)
            {
                if (header.Version == 22)
                {
                    UsedBytes = reader.ReadUInt32();
                    Data = reader.ReadBytes(0xC8);
                }
                else
                {
                    UsedBytes = reader.ReadUInt32();
                    Data = reader.ReadBytes(0x94);
                }
            }
        }
    }
}
