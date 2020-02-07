using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using HyruleWarriors.G1M;

namespace FirstPlugin
{
    public class BinGzArchive : TreeNodeFile, IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Hyrule Warriors Resource (bin.gz)" };
        public string[] Extension { get; set; } = new string[] { "*.bin.gz" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                bool isTexture = reader.CheckSignature(4, "G1TG") || reader.CheckSignature(4, "GT1G");
                bool isModel = reader.CheckSignature(4, "_M1G") || reader.CheckSignature(4, "G1M_");

                return (FileName.Contains(".bin.gz")) && !isTexture && !isModel;
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

        private void CheckEndianness(FileReader reader)
        {
            reader.SetByteOrder(false);

            uint Count = reader.ReadUInt32();
            uint FirstOffset = reader.ReadUInt32();

            if (FirstOffset == (Count * 8) + 4)
                reader.SetByteOrder(false);
            else
                reader.SetByteOrder(true);

            reader.Position = 0;
        }

        public bool isBigEndian = true;


        public override void OnAfterAdded()
        {
            var editor = LibraryGUI.GetObjectEditor();
            foreach (var file in files)
            {
                if (file.FileFormat == null)
                    continue;

                if (file.FileFormat is G1M)
                    editor.SelectNode((G1M)file.FileFormat);
            }
        }

        public void Load(Stream stream)
        {
            CanSave = true;

            using (var reader = new FileReader(stream))
            {
                CheckEndianness(reader);
                isBigEndian = reader.IsBigEndian;

                uint Count = reader.ReadUInt32();

                Console.WriteLine($"Count {Count}");

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
                    fileEntry.OpenFileFormatOnLoad = true;

                    switch (Magic)
                    {
                        case "GT1G": //Textures
                        case "G1TG": //Textures
                            fileEntry.FileName = $"TextureContainer_{i}.gt1";
                            break;
                        case "_M1G":
                        case "G1M_":
                            fileEntry.FileName = $"Model_{i}.g1m";
                            break;
                        case "_A1G":
                        case "G1A_":
                            fileEntry.FileName = $"Animation_{i}.g1a";
                            break;
                        default:
                            break;
                    }

                    files.Add(fileEntry);
                }
            }
        }
     
        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream))
            {
                writer.SetByteOrder(isBigEndian);
                writer.Write(files.Count);
                long pos = writer.Position;
                writer.Write(new uint[files.Count * 2]);
                for (int i = 0; i < files.Count; i++)
                {
                    files[i].SaveFileFormat();

                    writer.WriteUint32Offset(pos + (i * 8));
                    writer.Write(files[i].FileData.Length, pos + 4 + (i * 8));
                    writer.Write(files[i].FileData);
                }
            }
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
