using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Security.Cryptography;
using Newtonsoft.Json;

namespace FirstPlugin
{
    public class PKG : IArchiveFile, IFileFormat, ILeaveOpenOnLoad, ISaveOpenedFileStream
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; } = true;
        public string[] Description { get; set; } = new string[] { "PKG" };
        public string[] Extension { get; set; } = new string[] { "*.pkg" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; } = true;
        public bool CanRenameFiles { get; set; } = true;
        public bool CanReplaceFiles { get; set; } = true;
        public bool CanDeleteFiles { get; set; } = true;

        public bool Identify(System.IO.Stream stream)
        {
            return FileName.EndsWith(".pkg");
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

        static Dictionary<ulong, string> HashList = new Dictionary<ulong, string>();

        static void CalculateHashes()
        {
            var mem = new MemoryStream(Properties.Resources.MetroidDread);
            using (var reader = new StreamReader(mem)) {
                //Thanks to UltiNaruto for the hash list
                HashList = JsonConvert.DeserializeObject<Dictionary<ulong, string>>(reader.ReadToEnd());
            }
        }

        private System.IO.Stream _stream;
        public void Load(System.IO.Stream stream)
        {
            if (HashList.Count == 0)
                CalculateHashes();

            _stream = stream;
            using (var reader = new FileReader(stream, true))
            {
                reader.SetByteOrder(false);
                uint headerSize = reader.ReadUInt32();
                uint fileSize = reader.ReadUInt32();
                uint numFiles = reader.ReadUInt32();

                for (int i = 0; i < numFiles; i++)
                {
                    var file = new FileEntry();
                    ulong nameHash = reader.ReadUInt64();
                    uint fileStartOffset = reader.ReadUInt32();
                    uint fileEndOffset = reader.ReadUInt32();

                    uint size = fileEndOffset - fileStartOffset;

                    file.Hash = nameHash;
                    file.FileName = nameHash.ToString("X");
                    file.FileDataStream = new SubStream(reader.BaseStream,
                        fileStartOffset, size);

                    if (HashList.ContainsKey(nameHash))
                        file.FileName = HashList[nameHash];

                    files.Add(file);
                }
                files = files.OrderBy(x => x.FileName).ToList();
            }
        }

        public void Unload()
        {
            _stream?.Dispose();
        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream))
            {
                writer.Write(uint.MaxValue); //header size
                writer.Write(uint.MaxValue); //file size
                writer.Write(files.Count);
                for (int i = 0; i < files.Count; i++)
                {
                    writer.Write(files[i].Hash);
                    writer.Write(uint.MaxValue); //start offset
                    writer.Write(uint.MaxValue); //end offset
                }
                writer.Align(128);

                writer.WriteUint32Offset(0, 4); //Size of header - 4
                for (int i = 0; i < files.Count; i++)
                {
                    writer.WriteUint32Offset(20 + (i * 16)); //start offset
                    files[i].FileDataStream.CopyTo(writer.BaseStream);
                    writer.WriteUint32Offset(24 + (i * 16)); //end offset

                    writer.Align(8);
                }
                using (writer.TemporarySeek(4, SeekOrigin.Begin)) {
                    writer.Write((int)writer.BaseStream.Length);
                }
            }
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            var hash = Crc64.Compute(archiveFileInfo.FileName);

            files.Add(new FileEntry()
            {
                Hash = hash,
                FileDataStream = archiveFileInfo.FileDataStream,
                FileName = archiveFileInfo.FileName,
            });
            return true;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            files.Remove((FileEntry)archiveFileInfo);
            return true;
        }

        public class FileEntry : ArchiveFileInfo
        {
            public ulong Hash { get; set; }

            public override string FileName
            {
                get => base.FileName;
                set
                {
                    if (base.FileName != value) {
                        //Filename changed (not empty previously)
                        bool isRenamed = !string.IsNullOrEmpty(base.FileName);
                        base.FileName = value;
                        if (isRenamed)
                            Hash = Crc64.Compute(value);
                    }
                }
            }
        }
    }
}
