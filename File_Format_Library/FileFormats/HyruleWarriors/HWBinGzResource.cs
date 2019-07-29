using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class BinGzArchive : TreeNodeFile, IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Hyrule Warriors Resource (bin.gz)" };
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
            return (FileName.Contains(".bin.gz"));
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

        private void CheckEndianness(FileReader reader)
        {
            uint Count = reader.ReadUInt32();
            uint FirstOffset = reader.ReadUInt32();

            if (FirstOffset == (Count * 8) + 4)
                reader.SetByteOrder(false);
            else
                reader.SetByteOrder(true);

            reader.Position = 0;
        }

        private Stream CheckCompression(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                reader.SeekBegin(132);
                ushort magic = reader.ReadUInt16();
                if (reader.ReadUInt16() == 0X78DA)
                {
                    var data = STLibraryCompression.ZLIB.Decompress(reader.getSection(132, (int)reader.BaseStream.Length - 132));
                    return new MemoryStream(data);
                }
            }
            return stream;
        }

        public void Load(Stream stream)
        {
            stream = CheckCompression(stream);
            using (var reader = new FileReader(stream))
            {
                CheckEndianness(reader);

                uint Count = reader.ReadUInt32();

                uint[] Offsets = new uint[Count];
                uint[] Sizes = new uint[Count];

                for (int i = 0; i < Count; i++)
                {
                    Offsets[i] = reader.ReadUInt32();
                    Sizes[i] = reader.ReadUInt32();
                }

                for (int i = 0; i < Count; i++)
                {
                    var fileEntry = new FileEntry();
                    reader.SeekBegin(Offsets[i]);
                    string Magic = reader.ReadString(4);
                    reader.Seek(-4);
                    reader.SeekBegin(Offsets[i]);
                    fileEntry.FileData = reader.ReadBytes((int)Sizes[i]);
                    fileEntry.FileName = $"File {i}";

                    switch (Magic)
                    {
                        case "GT1G": //Textures
                        case "G1TG": //Textures
                            GT1 GITextureU = new GT1();
                            GITextureU.FileName = $"TextureContainer{i}.gt1";
                            GITextureU.Read(new FileReader(fileEntry.FileData));
                            Nodes.Add(GITextureU);
                            break;
                        default:
                            files.Add(fileEntry);
                            break;

                    }
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

        }
    }
}
