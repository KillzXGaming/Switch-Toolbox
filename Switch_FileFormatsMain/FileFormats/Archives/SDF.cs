using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class SDF : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Snowdrop Engine Data Table Of Contents" };
        public string[] Extension { get; set; } = new string[] { "*.sdftoc" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; } = false;
        public bool CanRenameFiles { get; set; } = false;
        public bool CanReplaceFiles { get; set; } = false;
        public bool CanDeleteFiles { get; set; } = false;

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

        public List<FileEntry> files = new List<FileEntry>();
        public IEnumerable<ArchiveFileInfo> Files => files;

        SDFTOC_Header Header;
        SDFTOC_ID startId;
        int[] block1;
        SDFTOC_ID[] blockIds;
        public SDFTOC_Block2[] block2Array;
        byte[] DecompressedBlock;
        SDFTOC_ID endId;

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

                for (int i = 0; i < FileEntries.Count; i++)
                {
                    FileEntries[i].CanLoadFile = SupportedExtensions.Contains(Utils.GetExtension(FileEntries[i].FileName));
                    files.Add(FileEntries[i]);
                }
            }
        }

        private List<string> SupportedExtensions = new List<string>()
        { ".dds", ".tga" ,".mmb", ".png", ".jpg"  };

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

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public class FileEntry : ArchiveFileInfo
        {
            public SDF SDFParent;
            public string FilePath;
            public string FolderPath;
            public string FileBlockPath;
            public ulong PackageID;
            public ulong Offset;
            public ulong DecompressedSize;
            public List<ulong> CompressedSizes;
            public ulong DdsType;
            public bool UseDDS;
            public bool IsCompressed = false;

            public override byte[] FileData
            {
                get { return GetFileBytes(); }
                set { base.FileData = value; }
            }

            public override string FileSize { get { return STMath.GetFileSize((long)DecompressedSize, 4); } }

            public override object DisplayProperties { get { return this; } }

            public override IFileFormat OpenFile()
            {
                var FileFormat = STFileLoader.OpenFileFormat(
                    IOExtensions.RemoveIllegaleFolderNameCharacters(FileName), FileData, true);

                if (FileFormat is DDS)
                    ((DDS)FileFormat).SwitchSwizzle = true;

                return FileFormat;
            }

            public byte[] GetFileBytes()
            {
                List<byte[]> Data = new List<byte[]>();
                if (File.Exists(FileBlockPath))
                {
                    var block = File.Open(FileBlockPath, FileMode.Open);
                    using (var stream = new FileReader(block))
                    {
                        if (CompressedSizes.Count == 0)
                        {
                            //Decompressed File 
                            string FileNameBlock = Path.Combine(FolderPath, FilePath);
                            string FolerPath = Path.GetDirectoryName(FileNameBlock);
                            if (!Directory.Exists(FolerPath))
                                Directory.CreateDirectory(FolerPath);

                            Data.Add(stream.getSection((int)Offset, (int)DecompressedSize));
                        }
                        else
                        {
                            var PageSize = (double)0x10000;
                            var DecompOffset = 0;
                            var CompOffset = 0;
                            IsCompressed = true;

                            if (UseDDS)
                            {
                                bool IsDX10 = false;
                                using (var filereader = new FileReader(SDFParent.block2Array[DdsType].Data))
                                {
                                    filereader.Position = 84;
                                    IsDX10 = filereader.ReadString(4) == "DX10";

                                    if (IsDX10)
                                        Data.Add(SDFParent.block2Array[DdsType].Data.Take((int)0x94).ToArray());
                                    else
                                        Data.Add(SDFParent.block2Array[DdsType].Data.Take((int)0x80).ToArray());
                                }
                            }

                            for (var i = 0; i < CompressedSizes.Count; i++)
                            {
                                var decompSize = (int)Math.Min((int)DecompressedSize - DecompOffset, PageSize);
                                if (CompressedSizes[i] == 0 || decompSize == (int)CompressedSizes[i])
                                {
                                    stream.Seek((int)Offset + CompOffset, SeekOrigin.Begin);
                                    CompressedSizes[i] = (ulong)decompSize;
                                    Data.Add( stream.ReadBytes(decompSize));
                                }
                                else
                                {
                                    stream.Seek((int)Offset + CompOffset, SeekOrigin.Begin);
                                    Data.Add(STLibraryCompression.ZSTD.Decompress(stream.ReadBytes((int)CompressedSizes[i])));
                                }
                                DecompOffset += (int)decompSize;
                                CompOffset += (int)CompressedSizes[i];
                            }
                        }
                    }

                    block.Dispose();
                }

                return Utils.CombineByteArray(Data.ToArray());
            }
        }

        public List<FileEntry> FileEntries = new List<FileEntry>();
            
        public void ParseNames(FileReader reader, string Name = "")
        {
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

                if (count1 > 0)
                {
                    uint strangeId = reader.ReadUInt32();
                    byte chr2 = reader.ReadByte();
                    int byteCount = chr2 & 3;
                    int byteValue = chr2 >> 2;
                    ulong DdsType = readVariadicInteger(byteCount, reader);

                    for (int chunkIndex = 0; chunkIndex < count1; chunkIndex++)
                    {
                        byte ch3 = reader.ReadByte();
                       // if (ch3 == 0)
                    //    {
                    //        break;
                    //    }

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
                       //     throw new InvalidDataException($"SDF Package ID ({packageId})  outside of TOC range ({ Header.Block1Count})");
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

                        if (Header.Version <= 0x15)
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

        public void DumpFile(string Name, ulong packageId, ulong packageOffset, ulong decompresedSize,
      List<ulong> compressedSize, ulong ddsType, bool Append, bool UseDDS)
        {
            string PathFolder = Path.GetDirectoryName(FilePath);

            string layer;
            if (packageId < 1000) layer = "A";
            else if (packageId < 2000) layer = "B";
            else if (packageId < 3000) layer = "C";
            else layer = "D";

            string ID = packageId.ToString("D" + 4);

            string BlockFilePath = Path.Combine(PathFolder, $"sdf-{layer}-{ID}.sdfdata");
            if (Append)
            {
            
            }

            bool IsFile = !Name.Contains("dummy") && decompresedSize > 5;

            Console.WriteLine(Name + " " + IsFile);

            if (IsFile)
            {
                FileEntries.Add(new FileEntry()
                {
                    SDFParent = this,
                    FileName = Name,
                    FileBlockPath = BlockFilePath,
                    FilePath = Name,
                    FolderPath = PathFolder,
                    CompressedSizes = compressedSize,
                    DdsType = ddsType,
                    UseDDS = UseDDS,
                    DecompressedSize = decompresedSize,
                    PackageID = packageId,
                    Offset = packageOffset,
                });
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
