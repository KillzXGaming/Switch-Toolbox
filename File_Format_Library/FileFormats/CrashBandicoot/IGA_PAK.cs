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

        public uint Version;
        public uint Unknown2;
        public uint Alignment;
        public uint[] Unknowns3;
        public uint[] Hashes;

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                CanSave = true;

                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                uint Singnature = reader.ReadUInt32(); //IGAx01
                Version = reader.ReadUInt32(); //13
                uint FileSectionSize = reader.ReadUInt32(); //Total size of file entires that point to file data
                uint FileCount = reader.ReadUInt32();
                Alignment = reader.ReadUInt32();

                //Skip some unknowns for now
                Unknowns3 = reader.ReadUInt32s(5);
                uint NameTableOffset = reader.ReadUInt32();
                uint Padding = reader.ReadUInt32();
                uint NameTableSize = reader.ReadUInt32();
                Unknown2 = reader.ReadUInt32(); //Always 1?
                Hashes = reader.ReadUInt32s((int)FileCount);

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

                    file.FullName = FileNames[i]; 

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

        public void Write(FileWriter writer)
        {
            //Save all the data for opened files first and compress if needed
            for (int i = 0; i < files.Count; i++)
                files[i].SaveOpenedFile();

            writer.WriteSignature("IGA\x01");
            writer.Write(Version);
            //total size of file info and hash section
            //16 size of info, 4 for size of hash
            writer.Write(files.Count * 20); 
            writer.Write(files.Count);
            writer.Write(Alignment);
            writer.Write(Unknowns3);

            long _ofsNameTbl = writer.Position;
            writer.Write(uint.MaxValue); //NameTableOffset
            writer.Write(0);
            writer.Write(202518);
            writer.Write(1);
            writer.Write(Hashes);

            //Now after are file entries
            long fileInfoTbl = writer.Position;
            for (int i = 0; i < files.Count; i++)
            {
                writer.Write(uint.MaxValue); //FileOffset
                writer.Write(files[i].Order1);
                writer.Write(files[i].Order2);
                writer.Write(files[i].DecompressedFileSize);
                writer.Write(files[i].BlockOffset);
                writer.Write(files[i].FileCompressionType);
            }

            writer.Align(128);

            //Now save the file data
            for (int i = 0; i < files.Count; i++)
            {
                long dataPos = writer.Position;
                using (writer.TemporarySeek(fileInfoTbl + (i * 16), SeekOrigin.Begin)) {
                    writer.Write((uint)dataPos);
                }

                writer.Write(files[i].CompressedBytes);
                writer.Align(2048);

                files[i].ResetData();
            }

            //Then write placeholder name offsets
            long nameOffsetTbl = writer.Position;

            writer.WriteUint32Offset(_ofsNameTbl);
            for (int i = 0; i < files.Count; i++)
                writer.Write(uint.MaxValue);

            long nameTablePos = writer.Position;

            //Now write each string and name offset
            for (int i = 0; i < files.Count; i++)
            {
                long strPos = writer.Position;
                using (writer.TemporarySeek(nameOffsetTbl + (i * sizeof(uint)), SeekOrigin.Begin)) {
                    writer.Write((uint)(strPos - nameOffsetTbl));
                }

                writer.WriteString(files[i].FullName);
            }

            writer.Dispose();
            writer.Close();

            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void WriteString(FileWriter writer, string name)
        {

        }

        public void Unload()
        {

        }

        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
            Write(new FileWriter(mem));
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

        public class FileEntry : ArchiveFileInfo
        {
            public string FullName { get; set; }

            //The archive file used to open the file
            public string SourceFile { get; set; }

            public uint Version { get; set; }

            public short FileCompressionType;

            public FileEntry(string ArchivePath, uint version)
            {
                SourceFile = ArchivePath;
                Version = version;
            }

            private byte[] _savedBytes { get; set; }

            public uint FileOffset;
            public uint DecompressedFileSize;

            public ushort Order1;
            public ushort Order2;
            public short BlockOffset;

            public byte[] CompressedBytes
            {
                get
                {
                    if (FileOffset == 0)
                        return new byte[0];

                    if (_savedBytes != null)
                        return _savedBytes;

                    using (var reader = new FileReader(SourceFile))
                    {
                        uint size = 0;
                        reader.BaseStream.Seek(FileOffset, SeekOrigin.Begin);
                        if (FileCompressionType != -1)
                        {
                            if (Version <= 0x0B)
                                size = reader.ReadUInt16();
                            else
                                size = reader.ReadUInt32();
                        }
                        else
                            size = DecompressedFileSize;

                        return reader.ReadBytes((int)size);
                    }
                }
            }

            public void ResetData()
            {
                _savedBytes = null;
            }

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

            public void SaveOpenedFile()
            {
                if (FileFormat != null && FileFormat.CanSave)
                {
                    byte[] data = FileFormat.Save();
                    DecompressedFileSize = (uint)data.Length;

                    if (FileCompressionType != -1)
                    {
                        SevenZip.Compression.LZMA.Encoder encode = new SevenZip.Compression.LZMA.Encoder();
                        MemoryStream ms = new MemoryStream(data);
                        MemoryStream otuput = new MemoryStream(data);
                        encode.Code(ms, otuput, -1, -1, null);
                        _savedBytes = otuput.ToArray();
                    }
                    else
                        _savedBytes = data;
                }
            }

            public void Read(FileReader reader)
            {
                FileOffset = reader.ReadUInt32();
                Order1 = reader.ReadUInt16();
                Order2 = reader.ReadUInt16();
                DecompressedFileSize = reader.ReadUInt32();
                BlockOffset = reader.ReadInt16();
                FileCompressionType = reader.ReadInt16();
            }
        }
    }
}
