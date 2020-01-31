using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Linq;

namespace FirstPlugin
{
    public class APAK : IFileFormat, IArchiveFile
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "APAK" };
        public string[] Extension { get; set; } = new string[] { "*.apak" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "APAK");
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

        public List<APAKFileInfo> files = new List<APAKFileInfo>();

        public IEnumerable<ArchiveFileInfo> Files => files;

        public void ClearFiles() { files.Clear(); }

        public bool CanAddFiles { get; set; } = false;
        public bool CanRenameFiles { get; set; } = true;
        public bool CanReplaceFiles { get; set; } = true;
        public bool CanDeleteFiles { get; set; } = true;

        public Header ApakHeader;

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;
            using (var reader = new FileReader(stream)) {
                ApakHeader = new APAK.Header(reader, files);
            }
        }

        public void Save(System.IO.Stream stream) {
            using (var writer = new FileWriter(stream))
            {
                ApakHeader.Write(writer, files);
            }
        }

        public class Header
        {
            public ushort Version { get; set; } = 5;
            public bool IsBigEndian { get; set; }

            public uint Unknown1 { get; set; } = 5381; //Always 5381?
            public uint Unknown2 { get; set; }

            public Header(FileReader reader, List<APAKFileInfo> files)
            {
                reader.SetByteOrder(true);

                reader.ReadSignature(4, "APAK");
                reader.ReadUInt16();
                Version = reader.ReadUInt16();
                IsBigEndian = Version == 5;

                if (!IsBigEndian) {
                    Version = 5;
                    reader.SetByteOrder(false);
                }

                uint FileCount = reader.ReadUInt32();
                Unknown1 = reader.ReadUInt32();
                uint FileInfoSize = reader.ReadUInt32();
                uint DataTotalSize = reader.ReadUInt32();

                for (int i = 0; i < FileCount; i++)
                    files.Add(new APAKFileInfo(reader));
            }

            public void Write(FileWriter writer, List<APAKFileInfo> files)
            {
                writer.SetByteOrder(IsBigEndian);
                writer.WriteSignature("APAK");
                writer.Write((ushort)0);
                writer.Write(Version);
                writer.Write(files.Count);
                writer.Write(Unknown1);
                writer.Write(files.Count * 64);
                writer.Write(uint.MaxValue);

                long fileInfoPos = writer.Position;
                for (int i = 0; i < files.Count; i++) {
                    files[i].SaveFileFormat();
                    writer.Write(files[i].Hash);
                    writer.Write(uint.MaxValue);
                    writer.Write(files[i].FileData.Length);
                    writer.Write(files[i].FileData.Length);
                    writer.Write(files[i].Alignment);
                    writer.Write(files[i].Unknown1);
                    writer.Write(files[i].Unknown2);
                    writer.Write(files[i].Unknown3);
                    writer.WriteString(files[i].FileName, 0x20);
                }

                //The data gets ordered by largest alignment size to lowest
                //Then by the file name
                var filesSorted = files.OrderByDescending(x => x.Alignment)
                                     .ThenBy(x => x.FileName)
                                     .ToList();

                writer.Align((int)files.Max(x => x.Alignment));

                long pos = writer.Position;
                for (int i = 0; i < files.Count; i++)
                {
                    var file = filesSorted[i];
                    int index = files.IndexOf(file);

                    long dataPos = writer.Position;
                    writer.WriteUint32Offset((fileInfoPos + 4) + index * 64);
                    writer.Write(file.FileData);
                    writer.AlignBytes((int)file.Alignment);
                    long dataEndPos = writer.Position;

                    using (writer.TemporarySeek((fileInfoPos + 12) + index * 64, SeekOrigin.Begin)) {
                        writer.Write((uint)(dataEndPos - dataPos));
                    }
                }

                long endPos = writer.Position;
                uint dataSize = (uint)(endPos - pos);
                using (writer.TemporarySeek(0x14, SeekOrigin.Begin)) {
                    writer.Write(dataSize);
                }

                //Idk what this format is doing but there's tons of padding at the end and idk why :(
                //Even combining all the alignments it can still be too small
                writer.AlignBytes((int)files.Sum(x => x.Alignment));
            }
        }

        public class APAKFileInfo : ArchiveFileInfo
        {
            public uint Hash { get; set; } //Unsure about this. Can't find any matches?

            public uint Alignment { get; set; } = 64;

            //These are all 0 for files i've seen.
            public uint Unknown1 { get; set; }
            public uint Unknown2 { get; set; }
            public uint Unknown3 { get; set; }

            public APAKFileInfo(string fileName)
            {
                FileName = fileName;
                string ext = Utils.GetExtension(FileName);
                if (AlignmentTable.ContainsKey(ext))
                    Alignment = AlignmentTable[ext];
            }

            private Dictionary<string, uint> AlignmentTable = new Dictionary<string, uint>()
            {
                { ".bfres", 8192 },
                { ".pspk" , 256 },
                { ".stfl" , 64 },
                { ".strc" , 64 },
                { ".layout" , 64 },
                { ".atcol" , 64 },
                { ".stsp" , 64 },
            };

            internal uint DataOffset;

            public APAKFileInfo(FileReader reader)
            {
                long pos = reader.Position;

                Hash = reader.ReadUInt32();
                DataOffset = reader.ReadUInt32();
                uint fileSize = reader.ReadUInt32();
                uint totalFileSize = reader.ReadUInt32(); //File size + aligned data
                Alignment = reader.ReadUInt32();
                Unknown1 = reader.ReadUInt32();
                Unknown2 = reader.ReadUInt32();
                Unknown3 = reader.ReadUInt32();
                FileName = reader.ReadString(0x20, true);

                Console.WriteLine($"{Utils.GetExtension(FileName)} {Alignment}");

                long endpos = reader.Position;

                reader.Seek(DataOffset, System.IO.SeekOrigin.Begin);
                FileData = reader.ReadBytes((int)fileSize);

                reader.Seek(endpos, System.IO.SeekOrigin.Begin);
            }
        }


        public void Unload()
        {

        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo) {
            files.Add(new APAKFileInfo(archiveFileInfo.FileName)
            {
                FileData = archiveFileInfo.FileData,
            });
            return true;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo) {
            files.Remove((APAKFileInfo)archiveFileInfo);
            return true;
        }
    }
}
