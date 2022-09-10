using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin {
    class NARC : IArchiveFile, IFileFormat {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; } = true;
        public string[] Description { get; set; } = new string[] { "Nitro Archive (NARC)" };
        public string[] Extension { get; set; } = new string[] { "*.narc" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public Type[] Types => new Type[0];

        public bool CanAddFiles { get; set; } = true;
        public bool CanRenameFiles { get; set; } = true;
        public bool CanReplaceFiles { get; set; } = true;
        public bool CanDeleteFiles { get; set; } = true;

        public IEnumerable<ArchiveFileInfo> Files => files;

        // private
        private Header header;
        private byte[] bfntUnk = new byte[] { 0x00, 0x00, 0x01, 0x00 };

        private List<FileEntry> files = new List<FileEntry>();

        public bool AddFile(ArchiveFileInfo archiveFileInfo) {
            files.Add(new FileEntry(this)
            {
                FileData = archiveFileInfo.FileData,
                FileName = archiveFileInfo.FileName,
            });
            return true;
        }

        public void ClearFiles() => files.Clear();

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo) {
            return files.Remove((FileEntry)archiveFileInfo);
        }

        public bool Identify(Stream stream) {
            using(FileReader reader = new FileReader(stream, true))
                return reader.CheckSignature(4, "NARC");
        }

        public void Load(Stream stream) {
            using(FileReader reader = new FileReader(stream)) {
                //DirectoryEntry currentDirEntry = rootDir;
                string currentDir = string.Empty;

                header = new Header(reader);

                reader.Position += 4;

                // The positions where the sections' reading was left last time.
                long bfatIndex = reader.Position + 12;
                long bfntIndex = reader.Position + reader.ReadUInt32(); // From the BFAT length.
                long fimgIndex;

                uint fileCount = reader.ReadUInt32();

                uint currentFileOffset;
                uint currentFileEnd;

                reader.Position = bfntIndex;

                fimgIndex = reader.Position + reader.ReadUInt32() + 8; // Sets FIMG section begining.
                bfntUnk = reader.ReadBytes(reader.ReadInt32() - 4);

                for(int i = 0; i < fileCount; i++) {
                    byte nameLength = reader.ReadByte();

                    if(nameLength == 0x00) { // End of the "folder".
                        List<int> slashIndices = new List<int>();
                        for(int j = 0; j < currentDir.Length; j++)
                            if(currentDir[j] == '/')
                                slashIndices.Add(j);

                        int lastSlash = slashIndices.Last();
                        slashIndices.Remove(lastSlash);
                        int slashBeforeLast = slashIndices.Count > 0 ? slashIndices.Last() : 0;

                        currentDir = currentDir.Remove(slashBeforeLast + 1);

                        if(currentDir.Length == 1)
                            currentDir = string.Empty;

                        i--;
                        continue;
                    }

                    if(nameLength >= 0x80) { // If it is a "folder".
                        // Disable file modifications:
                        CanSave = false;
                        CanAddFiles = false;
                        CanRenameFiles = false;
                        CanReplaceFiles = false;
                        CanDeleteFiles = false;

                        string dirName = reader.ReadString(nameLength & 0x7F);
                        currentDir += dirName + "/";

                        reader.Position += 2;
                        i--;
                        continue;
                    }

                    // Read BFAT section:
                    using(reader.TemporarySeek()) {
                        reader.Position = bfatIndex;

                        currentFileOffset = reader.ReadUInt32();
                        currentFileEnd = reader.ReadUInt32();

                        bfatIndex = reader.Position;
                    }

                    string name = reader.ReadString(nameLength);

                    using(reader.TemporarySeek()) {
                        reader.Position = fimgIndex + currentFileOffset;
                        files.Add(new FileEntry(this) {
                            FileName = currentDir + name,
                            BlockData = reader.ReadBytes((int) (currentFileEnd - currentFileOffset))
                    });
                    }
                }
            }
        }

        public class FileEntry : ArchiveFileInfo
        {
            public byte[] BlockData;

            public override Dictionary<string, string> ExtensionImageKeyLookup
            {
                get
                {
                    return new Dictionary<string, string>()
                    {
                        {".cbfmd", "bfres" },
                        {".cbfa",  "bfres" },
                        {".cbfsa", "bfres" },
                        {".cbntx", "bntx" },
                    };
                }
            }

            public NARC ArchiveFile;

            public FileEntry(NARC narc)
            {
                ArchiveFile = narc;
            }

            public override byte[] FileData
            {
                get { return DecompressBlock(); }
                set
                {
                    BlockData = value;
                }
            }

            private bool IsTexturesLoaded = false;
            public override IFileFormat OpenFile()
            {
                var FileFormat = base.OpenFile();
                bool IsModel = FileFormat is BFRES;

                if (IsModel && !IsTexturesLoaded)
                {
                    IsTexturesLoaded = true;
                    foreach (var file in ArchiveFile.Files)
                    {
                        if (Utils.GetExtension(file.FileName) == ".ctex")
                        {
                            file.FileFormat = file.OpenFile();
                        }
                    }
                }

                return base.OpenFile();
            }

            private byte[] DecompressBlock()
            {
                byte[] data = BlockData;

                var reader = new FileReader(data);
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                byte compType = reader.ReadByte();

                if (compType == 0x50)
                {
                    reader.Seek(4, System.IO.SeekOrigin.Begin);
                    uint decompSize = reader.ReadUInt32();
                    uint compSize = (uint)reader.BaseStream.Length - 8;

                    var comp = new STLibraryCompression.MTA_CUSTOM();
                    return comp.Decompress(data, decompSize);
                }
                else if (compType == 0x30 || compType == 0x20)
                {
                    uint decompSize = reader.ReadUInt32();
                    uint compSize = (uint)reader.BaseStream.Length - 16;

                    reader.SeekBegin(16);
                    ushort signature = reader.ReadUInt16();
                    bool IsGZIP = signature == 0x1F8B;
                    bool IsZLIB = signature == 0x789C || signature == 0x78DA;

                    byte[] filedata = reader.getSection(16, (int)compSize);
                    reader.Close();
                    reader.Dispose();

                    if (IsGZIP)
                        data = STLibraryCompression.GZIP.Decompress(filedata);
                    else
                        data = STLibraryCompression.ZLIB.Decompress(filedata, false);
                }

                return data;
            }
        }

        public void Unload() { }

        internal class Header {
            public ushort ByteOrder;
            public ushort Version;
            public uint FileSize;
            public ushort HeaderSize;
            public ushort DataBlocks;

            public Header(FileReader reader) {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                reader.ReadSignature(4, "NARC");

                ByteOrder = reader.ReadUInt16();
                reader.CheckByteOrderMark(ByteOrder);

                Version = reader.ReadUInt16();
                FileSize = reader.ReadUInt32();
                HeaderSize = reader.ReadUInt16();
                DataBlocks = reader.ReadUInt16();
            }
        }

        // Mostly everything from this point was imported from:
        // https://github.com/Jenrikku/NARCSharp
        public void Save(Stream stream) {
            using(BinaryDataWriter writer = new BinaryDataWriter(stream)) {
                #region Header
                writer.Write("NARC", BinaryStringFormat.NoPrefixOrTermination, Encoding.ASCII); // Magic.

                writer.ByteOrder = (ByteOrder) header.ByteOrder;
                writer.Write((ushort) 0xFFFE); // ByteOrder.

                writer.Write(header.Version); // Version.

                long headerLengthPorsition = writer.Position;
                writer.Position += 4; // Skips length writing.

                writer.Write(header.HeaderSize); // Header length.
                writer.Write(header.DataBlocks); // Section count.
                #endregion

                #region BTAF preparation
                long btafPosition = WriteSectionHeader("BTAF"); // Header.
                writer.Write((uint) files.Count); // File count.

                for(uint i = 0; i < files.Count; i++) // Reads unset bytes per file. (Reserved space for later)
                    writer.Position += 8;

                WriteSectionLength(btafPosition);
                #endregion

                #region BTNF
                long btnfPosition = WriteSectionHeader("BTNF"); // Header.
                writer.Write(bfntUnk.Length + 4);
                writer.Write(bfntUnk);

                foreach(ArchiveFileInfo file in files)
                    writer.Write(file.FileName, BinaryStringFormat.ByteLengthPrefix);
                writer.Write((byte) 0x00);

                writer.Align(128);

                WriteSectionLength(btnfPosition);
                #endregion

                #region GMIF
                long gmifPosition = WriteSectionHeader("GMIF"); // Header.

                long btafCurrentPosition = btafPosition + 12; // First offset-size position. (BTAF)
                foreach(ArchiveFileInfo file in files) {
                    WriteBTAFEntry(); // BTAF offset
                    writer.Write(file.FileData);
                    WriteBTAFEntry(); // BTAF size.
                    writer.Align(128);
                }

                WriteSectionLength(gmifPosition);
                #endregion

                writer.Position = headerLengthPorsition;
                writer.Write((uint) writer.BaseStream.Length); // Total file length.

                long WriteSectionHeader(string magic) {
                    long startPosition = writer.Position;
                    writer.Write(magic, BinaryStringFormat.NoPrefixOrTermination, Encoding.ASCII); // Magic.

                    writer.Position += 4; // Skips length position.

                    return startPosition;
                }

                void WriteSectionLength(long startPosition) {
                    using(writer.TemporarySeek()) {
                        long finalLength = (uint) writer.Position;

                        writer.Position = startPosition + 4;
                        writer.Write((uint) (finalLength - startPosition));
                    }
                }

                void WriteBTAFEntry() {
                    uint value = (uint) (writer.Position - (gmifPosition + 8));

                    using(writer.TemporarySeek()) {
                        writer.Position = btafCurrentPosition;
                        writer.Write(value);
                    }

                    btafCurrentPosition += 4;
                }
            }            
        }
    }
}
