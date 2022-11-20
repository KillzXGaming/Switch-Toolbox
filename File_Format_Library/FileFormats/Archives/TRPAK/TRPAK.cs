using FirstPlugin.FileFormats.Hashes;
using FlatBuffers.TRPAK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    internal class TRPAK : TreeNodeFile, IArchiveFile, IFileFormat
    {
        public static bool shownOodleError;

        public bool CanAddFiles => false;
        public bool CanDeleteFiles => false;
        public bool CanRenameFiles => false;
        public bool CanReplaceFiles => false;
        public bool CanSave { get; set; } = false;
        public string[] Description { get; set; } = new string[] { "tr Package" };
        public string[] Extension { get; set; } = new string[] { "*.trpak" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IEnumerable<ArchiveFileInfo> Files { get; set; }
        public FileType FileType { get; set; } = FileType.Archive;
        public IFileInfo IFileInfo { get; set; }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            throw new NotImplementedException();
        }

        public void ClearFiles()
        {
            throw new NotImplementedException();
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            throw new NotImplementedException();
        }

        public bool Identify(Stream stream)
        {
            return Utils.GetExtension(FileName) == ".trpak";
        }

        public void Load(Stream stream)
        {
            GFPAKHashCache.EnsureHashCache();
            Files = new List<ArchiveFileInfo>();
            FlatBuffers.TRPAK.TRPAK trpak = FlatBuffers.TRPAK.TRPAK.GetRootAsTRPAK(new FlatBuffers.ByteBuffer(stream.ToBytes()));
            if (trpak.FilesLength != trpak.HashesLength)
            {
                throw new Exception("not the same amount of Hashes and File Entries in Trpak Container");
            }
            for (int i = 0; i < trpak.FilesLength; i++)
            {
                FlatBuffers.TRPAK.File? file = trpak.Files(i);
                ulong hash = trpak.Hashes(i);
                if (file.HasValue)
                {
                    ArchiveFileInfo AFI = new ArchiveFileInfo();
                    byte[] FileData = file.Value.GetDataArray();
                    if (file.Value.CompressionType == Compression.OODLE)
                    {
                        if (!shownOodleError && !System.IO.File.Exists($"{Runtime.ExecutableDir}\\oo2core_6_win64.dll"))
                        {
                            MessageBox.Show("'oo2core_6_win64.dll' not found in the executable folder! User must provide their own copy!");
                            shownOodleError = true;
                        }
                        byte[] FileDatadecompressed = Toolbox.Library.Compression.Oodle.Decompress(FileData, (long)file.Value.DecompressedSize);
                        FileData = FileDatadecompressed;
                    }
                    AFI.FileData = FileData;
                    AFI.Name = GetName(hash, FileData);
                    AFI.FileName = AFI.Name;
                    ((List<ArchiveFileInfo>)Files).Add(AFI);
                }
            }
            GFPAKHashCache.WriteCache();
        }

        public void Save(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Unload()
        {
            foreach (var file in Files)
            {
                if (file.FileFormat != null)
                    file.FileFormat.Unload();

                file.FileData = null;
            }

           ((List<ArchiveFileInfo>)Files).Clear();

            GC.SuppressFinalize(this);
        }

        private string FindMatch(byte[] f, string FileName)
        {
            foreach (IFileFormat fileFormat in FileManager.GetFileFormats())
            {
                fileFormat.FileName = FileName;

                if (fileFormat.Identify(new MemoryStream(f)))
                {
                    return fileFormat.Extension[0].Replace("*", "");
                }
            }
            return "";
        }

        //For BNTX, BNSH, etc
        private string GetBinaryHeaderName(byte[] Data)
        {
            using (var reader = new FileReader(Data))
            {
                reader.Seek(0x10, SeekOrigin.Begin);
                uint NameOffset = reader.ReadUInt32();

                reader.Seek(NameOffset, SeekOrigin.Begin);
                return reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);
            }
        }

        private string GetName(ulong fileHash, byte[] Data)
        {
            string fileHashName = GFPAKHashCache.GetHashName(fileHash) ?? "";
            string ext = FindMatch(Data, fileHashName);
            if (ext == ".bntx" || ext == ".bfres" || ext == ".bnsh" || ext == ".bfsha")
            {
                string fileName = GetBinaryHeaderName(Data);
                //Check for matches for shaders
                if (ext == ".bnsh")
                {
                    if (FNV64A1.Calculate($"{fileName}.bnsh_fsh") == fileHash)
                        fileName = $"{fileName}.bnsh_fsh";
                    else if (FNV64A1.Calculate($"{fileName}.bnsh_vsh") == fileHash)
                        fileName = $"{fileName}.bnsh_vsh";
                }
                else
                    fileName = $"{fileName}{ext}";

                return $"{fileName}";
            }
            else
            {
                if (fileHashName != "")
                {
                    return $"{fileHashName}{ext}";
                }
                else
                    return $"{fileHash.ToString("X")}{ext}";
            }
        }
    }
}