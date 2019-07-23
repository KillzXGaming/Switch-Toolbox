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
    public class BinGzArchive : IArchiveFile, IFileFormat
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
                    string Magic = reader.ReadString(8);
                    switch (Magic)
                    {
                        case "G1TG0060": //PC or Wii U Texture
                            GITextureContainer GITextureU = new GITextureContainer();
                            GITextureU.Read(reader, true);
                            break;
                        case "GT1G0600": //Switch Texture
                            GITextureContainer GITexture = new GITextureContainer();
                            GITexture.Read(reader, false);
                            break;
                    }

                    fileEntry.FileData = reader.ReadBytes((int)Sizes[i]);
                    fileEntry.FileName = $"File {i}";
                    files.Add(fileEntry);


                }
             }
        }

        public class GITextureContainer
        {
            public void Read(FileReader reader, bool IsWiiU)
            {
                long StartPos = reader.Position;

                uint FileSize = reader.ReadUInt32();
                uint DataOffset = reader.ReadUInt32();
                uint TextureCount = reader.ReadUInt32();
                uint unk = reader.ReadUInt32();
                uint unk2 = reader.ReadUInt32();
                uint[] unk3s = reader.ReadUInt32s((int)TextureCount);

                for (int i = 0; i < TextureCount; i++)
                {
                    reader.SeekBegin(StartPos + DataOffset + (i * 4));

                    uint InfoOffset = reader.ReadUInt32();

                    reader.SeekBegin(DataOffset + InfoOffset);
                    byte unk4 = reader.ReadByte();
                    byte format = reader.ReadByte();

                }
            }
        }

        public class GITexture : STGenericTexture
        {
            public override bool CanEdit { get; set; } = false;

            public byte[] ImageData;

            public override TEX_FORMAT[] SupportedFormats
            {
                get
                {
                    return new TEX_FORMAT[] {
                    TEX_FORMAT.R8G8B8A8_UNORM,
                };
                }
            }

            public override void SetImageData(Bitmap bitmap, int ArrayLevel)
            {
            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
            {
                return ImageData;
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
