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

namespace FirstPlugin
{
    public class PKG : IArchiveFile, IFileFormat, ILeaveOpenOnLoad
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; } = false;
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
            using (var reader = new StreamReader(mem))
            {
                while (!reader.EndOfStream)
                {
                    string str = reader.ReadLine();
                    //Thanks to mr cheeze https://pastebin.com/qi7Sduua
                    ulong hash = Crc64.Compute(System.Text.Encoding.UTF8.GetBytes(str));
                    if (!HashList.ContainsKey(hash))
                        HashList.Add(hash, str);
                }
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

                    file.FileName = nameHash.ToString();
                    file.FileDataStream = new SubStream(reader.BaseStream,
                        fileStartOffset, size);

                    string ext = ".bin";
                    if (size > 4)
                    {
                        using (reader.TemporarySeek(fileStartOffset, SeekOrigin.Begin))
                        {
                            string magic = reader.ReadString(4);
                            if (magic == "FWAV") ext = ".bfwav";
                            if (magic == "MTXT") ext = ".bctex";
                            if (magic == "MCAN") ext = ".bccam";
                            if (magic == "MANM") ext = ".bcskla";
                            if (magic == "MSAS") ext = ".bmsas";
                            if (magic == "MMDL") ext = ".mmdl"; //Original extension is bcmdl but use mmdl for noesis script
                            if (magic == "MSUR") ext = ".bsmat";
                            if (magic == "MNAV") ext = ".bmscd";
                            if (magic.Contains(" Lua")) ext = ".lc";

                            if (magic == "MSUR")
                            {
                                reader.ReadUInt32();
                                file.FileName = $"imats/" + reader.ReadZeroTerminatedString();
                            }
                            if (magic.Contains("Lua"))
                                file.FileName = $"scripts/" + file.FileName + ".lua";
                            else if (magic == "MSAS")
                            {
                                reader.ReadUInt32();
                                ushort length = reader.ReadUInt16();
                                file.FileName = $"script_data/" + reader.ReadString(length);
                            }
                            else if (magic == "MSCD")
                            {
                                reader.ReadUInt32();
                                reader.ReadUInt32();
                                file.FileName = $"collision/" + reader.ReadZeroTerminatedString();
                            }
                            else if (magic == "MSAD")
                            {
                                reader.ReadUInt32();
                                file.FileName = $"script_components/" + reader.ReadZeroTerminatedString();
                            }
                            else if (magic == "MMDL")
                                file.FileName = $"models/" + file.FileName;
                            else if (magic == "MCAN")
                                file.FileName = $"cameras/" + file.FileName;
                            else if (magic == "MANM")
                                file.FileName = $"anims/" + file.FileName;
                            else if (magic == "MNAV")
                                file.FileName = $"ai_naviagtion/" + file.FileName;
                            else if (magic == "FWAV")
                                file.FileName = $"audio/" + file.FileName;
                            else
                                file.FileName = $"unknown/" + file.FileName;
                        }
                    }

                    file.FileName += ext;

                    file.FileName = $"files/{file.FileName}";

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
                    ulong hash = ulong.Parse(Path.GetFileNameWithoutExtension(files[i].FileName));
                    writer.Write(hash);
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
                }
                using (writer.TemporarySeek(4, SeekOrigin.Begin))
                {
                    writer.Write((int)writer.BaseStream.Length);
                }
            }
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            files.Add(new FileEntry()
            {
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
        }
    }
}
