using Syroot.BinaryData;
using System;
using System.Collections.Generic;
using System.IO;
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
        private BTAF btaf;
        private BTNF btnf;
        private GMIF gmif;

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
                header = new Header(reader);

                for(uint i = 0; i < header.DataBlocks; i++) {
                    long PositionBuf = reader.Position;
                    string cSectionMagic = Encoding.ASCII.GetString(reader.ReadBytes(4)).ToUpperInvariant();
                    uint cSectionSize = reader.ReadUInt32();

                    switch(cSectionMagic) {
                        case "BTAF":
                            btaf = new BTAF(reader);
                            break;
                        case "BTNF":
                            btnf = new BTNF(reader, (uint) btaf.FileDataArray.Length);
                            break;
                        case "GMIF":
                            gmif = new GMIF(reader, btaf);
                            break;
                    }

                    reader.Position = PositionBuf + cSectionSize;
                }

                for(int i = 0; i < btnf.FileNames.Length; i++)
                    files.Add(new FileEntry(this) {
                        FileName = btnf.FileNames[i],
                        BlockData = gmif.FilesData[i]
                    });
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
                writer.Write(0x00000000); // Skips length writing.

                writer.Write(header.HeaderSize); // Header length.
                writer.Write(header.DataBlocks); // Section count.
                #endregion

                #region BTAF preparation
                long btafPosition = WriteSectionHeader("BTAF"); // Header.
                writer.Write((uint) files.Count); // File count.

                for(uint i = 0; i < files.Count; i++) // Reads unset bytes per file. (Reserved space for later)
                    writer.Write((long) 0x0000000000000000);

                WriteSectionLength(btafPosition);
                #endregion

                #region BTNF
                long btnfPosition = WriteSectionHeader("BTNF"); // Header.
                writer.Write(btnf.Unknown);

                foreach(ArchiveFileInfo file in files)
                    writer.Write(file.FileName, BinaryStringFormat.ByteLengthPrefix);
                writer.Write((byte) 0x00);

                writer.Align(32);
                writer.Position += 8;

                WriteSectionLength(btnfPosition);
                #endregion

                #region GMIF
                long gmifPosition = WriteSectionHeader("GMIF"); // Header.

                long btafCurrentPosition = btafPosition + 12; // First offset-size position. (BTAF)
                foreach(ArchiveFileInfo file in files) {
                    WriteBTAFEntry(); // BTAF offset
                    writer.Write(file.FileData);
                    WriteBTAFEntry(); // BTAF size.
                    writer.Align(16);
                }

                WriteSectionLength(gmifPosition);
                #endregion

                writer.Position = headerLengthPorsition;
                writer.Write((uint) writer.BaseStream.Length); // Total file length.

                long WriteSectionHeader(string magic) {
                    long startPosition = writer.Position;
                    writer.Write(magic, BinaryStringFormat.NoPrefixOrTermination, Encoding.ASCII); // Magic.

                    writer.Write(0x00000000); // Skips length position.

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

        #region Sections
        internal class BTAF {
            public FD[] FileDataArray;

            public BTAF() { }
            public BTAF(BinaryDataReader reader) {
                uint numberOfFiles = reader.ReadUInt32();
                FileDataArray = new FD[numberOfFiles];

                for(ulong i = 0; i < numberOfFiles; i++) {
                    FileDataArray[i].offset = reader.ReadUInt32();
                    FileDataArray[i].size = reader.ReadUInt32();
                }
            }

            public class FD
            {
                public uint offset;
                public uint size;
            }
        }

        internal class BTNF {
            public string[] FileNames;
            public ulong Unknown;

            public BTNF() { }
            public BTNF(BinaryDataReader reader, uint numberOfFiles) {
                Unknown = reader.ReadUInt64();
                FileNames = new string[numberOfFiles];

                for(int i = 0; i < numberOfFiles; i++) {
                    FileNames[i] = reader.ReadString(BinaryStringFormat.ByteLengthPrefix, Encoding.ASCII);
                }
            }
        }

        internal class GMIF {
            public byte[][] FilesData;

            public GMIF() { }
            public GMIF(BinaryDataReader reader, BTAF btaf) {
                FilesData = new byte[btaf.FileDataArray.Length][];
                long PositionBuf = reader.Position;

                for(int i = 0; i < btaf.FileDataArray.Length; i++) {
                    reader.Position = PositionBuf + btaf.FileDataArray[i].offset;

                    FilesData[i] = reader.ReadBytes(
                        (int) (btaf.FileDataArray[i].size - btaf.FileDataArray[i].offset));
                }
            }
        }
        #endregion
    }
}
