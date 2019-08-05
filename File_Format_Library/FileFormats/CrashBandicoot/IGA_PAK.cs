using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class IGA_PAK : IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Crash Team Racing Archive" };
        public string[] Extension { get; set; } = new string[] { "*.pak" };
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
                return reader.CheckSignature(3, "IGA");
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

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                uint Singnature = reader.ReadUInt32(); //IGAx01
                uint Version = reader.ReadUInt32(); //13
                uint FileSectionSize = reader.ReadUInt32(); //Total size of file entires that point to file data
                uint FileCount = reader.ReadUInt32();
                uint ChunkSize = reader.ReadUInt32();

                //Skip some unknowns for now
                reader.Seek(20, System.IO.SeekOrigin.Current);
                uint NameTableOffset = reader.ReadUInt32();
                uint Padding = reader.ReadUInt32();
                uint NameTableSize = reader.ReadUInt32();
                uint Unknown2 = reader.ReadUInt32(); //Always 1?
                uint[] Hashes = reader.ReadUInt32s((int)FileCount);

                //Read the filenames first
                long pos = reader.Position;

                string[] FileNames = new string[FileCount];
                for (int i = 0; i < FileCount; i++)
                {
                    reader.SeekBegin(NameTableOffset + (i * sizeof(uint)));
                    uint NameOffset = reader.ReadUInt32();

                    reader.SeekBegin(NameTableOffset + NameOffset);
                    FileNames[i] = reader.ReadZeroTerminatedString();
                }

                //Go back for file entries
                reader.Position = pos;

                for (int i = 0; i < FileCount; i++)
                {
                    var file = new FileEntry(FilePath, Version);
                    file.FileName = FileNames[i];
                    file.Read(reader);
                    files.Add(file);

                   // if (Utils.GetExtension(file.FileName) == ".igz")
                    //    file.OpenFileFormatOnLoad = true;

                    //Remove this stupid long pointless path
                    file.FileName = file.FileName.Replace("temporary/octane/data/nx/output/", string.Empty);
                    file.FileName = file.FileName.Replace("temporary/mack/data/win64/output/", string.Empty);
                    file.FileName = file.FileName.Replace("temporary/mack/data/nx/output/", string.Empty);



                    // if (file.FileOffset >= 0xff000000)
                    //     file.FileOffset = 


                }
            }
        }

        public void Unload()
        {

        }

        public byte[] Save()
        {
            return null;
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
            //The archive file used to open the file
            public string SourceFile { get; set; }

            public uint Version { get; set; }

            public int FileCompressionType;

            public FileEntry(string ArchivePath, uint version)
            {
                SourceFile = ArchivePath;
                Version = version;
            }

            public uint FileOffset;
            public uint DecompressedFileSize;

            public override byte[] FileData
            {
                get
                {
                    if (FileOffset == 0)
                        return new byte[0];

                    using (var reader = new FileReader(SourceFile))
                    {
                        int size = 0;
                        int offset = (int)FileOffset;
                        uint compressedSize = 0;
                        int def_block = 0x8000;

                        if (FileCompressionType != -1)
                        {
                            MemoryStream output = new MemoryStream();

                            reader.BaseStream.Seek(offset, SeekOrigin.Begin);

                            if (Version <= 0x0B)
                                compressedSize = reader.ReadUInt16();
                            else
                                compressedSize = reader.ReadUInt32();

                            if (def_block > DecompressedFileSize - size) def_block = (int)DecompressedFileSize - size;


                            var properties = reader.ReadBytes(5);
                            var copressedBytes = reader.ReadBytes((int)compressedSize);

                            SevenZip.Compression.LZMA.Decoder decode = new SevenZip.Compression.LZMA.Decoder();
                            decode.SetDecoderProperties(properties);

                            MemoryStream ms = new MemoryStream(copressedBytes);
                            decode.Code(ms, output, compressedSize, def_block, null);

                            return output.ToArray();
                        }
                        else
                        {
                            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                            return reader.ReadBytes((int)DecompressedFileSize);
                        }
                    }
                }
            }

            public void Read(FileReader reader)
            {
                FileOffset = reader.ReadUInt32();
                uint NameOffset = reader.ReadUInt32();
                DecompressedFileSize = reader.ReadUInt32();
                FileCompressionType = reader.ReadInt32();
            }
        }
    }
}
