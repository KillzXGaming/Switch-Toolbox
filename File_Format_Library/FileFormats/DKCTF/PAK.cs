using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Toolbox.Library;
using Toolbox.Library.IO;
using System.IO;

namespace DKCTF
{
    public class PAK : TreeNodeFile, IArchiveFile, IFileFormat, ILeaveOpenOnLoad
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "DKCTF Archive" };
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
                bool IsForm = reader.CheckSignature(4, "RFRM");
                bool FormType = reader.CheckSignature(4, "PACK", 20);

                return IsForm && FormType;
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

        //For file searching
        public Dictionary<string, FileEntry> ModelFiles = new Dictionary<string, FileEntry>();
        public Dictionary<string, FileEntry> SkeletonFiles = new Dictionary<string, FileEntry>();
        public Dictionary<string, FileEntry> TextureFiles = new Dictionary<string, FileEntry>();
        public Dictionary<string, CHAR> CharFiles = new Dictionary<string, CHAR>();
        public Dictionary<string, FileEntry> AnimFiles = new Dictionary<string, FileEntry>();

        public PACK PakData;

        internal bool IsMPR;

        public void Load(System.IO.Stream stream)
        {
            PACK pack = new PACK(stream);
            PakData = pack;

            for (int i = 0; i < pack.Assets.Count; i++)
            {
                string ext = pack.Assets[i].Type.ToLower();

                FileEntry file = new FileEntry();
                file.ParentArchive = this;
                file.ArchiveStream = stream;
                file.AssetEntry = pack.Assets[i];

                string dir = pack.Assets[i].Type;
                if (DirectoryLabels.ContainsKey(dir))
                    dir = DirectoryLabels[dir];

                file.FileName = $"{dir}/{pack.Assets[i].FileID}.{ext}";
                file.SubData = new SubStream(stream, pack.Assets[i].Offset, pack.Assets[i].Size);

                if (pack.MetaOffsets.ContainsKey(pack.Assets[i].FileID.ToString()))
                    file.MetaPointer = pack.MetaDataOffset + pack.MetaOffsets[pack.Assets[i].FileID.ToString()];
                files.Add(file);

                switch (file.AssetEntry.Type)
                {
                    case "SMDL": ModelFiles.Add(file.AssetEntry.FileID.ToString(), file); break;
                    case "TXTR": TextureFiles.Add(file.AssetEntry.FileID.ToString(), file); break;
                    case "SKEL": SkeletonFiles.Add(file.AssetEntry.FileID.ToString(), file); break;
                    case "ANIM": AnimFiles.Add(file.AssetEntry.FileID.ToString(), file); break;
                    case "CHAR":
                        var c = new CHAR(new MemoryStream(file.FileData));
                        file.FileName = $"Characters/{c.Name}/{c.Name}.char";
                        CharFiles.Add(file.AssetEntry.FileID.ToString(), c);
                        break;
                }
            }

            foreach (var c in CharFiles)
            {
                if (SkeletonFiles.ContainsKey(c.Value.SkeletonFileID.ToString()))
                    SkeletonFiles[c.Value.SkeletonFileID.ToString()].FileName = $"Characters/{c.Value.Name}/Models/{c.Value.SkeletonFileID}.skel";

                foreach (var m in c.Value.Models)
                {
                    if (ModelFiles.ContainsKey(m.FileID.ToString()))
                        ModelFiles[m.FileID.ToString()].FileName = $"Characters/{c.Value.Name}/Models/{m.Name}.smdl";
                }
                foreach (var m in c.Value.Animations)
                {
                    if (AnimFiles.ContainsKey(m.FileID.ToString()))
                        AnimFiles[m.FileID.ToString()].FileName = $"Characters/{c.Value.Name}/Animations/{m.Name}.anim";
                }
            }
            foreach (var file in files)
            {
                if (PakFileList.GuiToFilePath.ContainsKey(file.AssetEntry.FileID.ToString()))
                {
                    file.FileName = "_LabeledFiles/" + PakFileList.GuiToFilePath[file.AssetEntry.FileID.ToString()];
                    //Organize the data type folders for easier access. 
                    if (file.AssetEntry.Type == "SMDL") file.FileName = file.FileName.Replace("exportData", "models");
                    if (file.AssetEntry.Type == "CMDL") file.FileName = file.FileName.Replace("exportData", "models");
                    if (file.AssetEntry.Type == "TXTR") file.FileName = file.FileName.Replace("exportData", "textures");
                    if (file.AssetEntry.Type == "ANIM") file.FileName = file.FileName.Replace("exportData", "animations");
                }
            }
          //  files = files.OrderBy(x => x.FileName).ToList();
        }

        Dictionary<string, string> DirectoryLabels = new Dictionary<string, string>()
        {
            { "CHAR", "Characters" },
            { "CHPR", "Character Project" },
            { "CMDL", "Static Models" },
            { "SMDL", "Skinned Models" },
            { "TXTR", "Textures" },
            { "MTRL", "Shaders" },
            { "CSMP", "AudioSample" },
            { "CAUD", "AudioData" },
            { "GENP", "Gpsys" },
            { "ANIM", "Animations" },
            { "XFRM", "Xfpsys" },
            { "WMDL", "World Models" },
            { "DCLN", "Collision Models" },
            { "CLSN", "Collision Static Models" },
        };


        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }
    }

    public class FileEntry : ArchiveFileInfo
    {
        public PACK.DirectoryAssetEntry AssetEntry;

        public PAK ParentArchive;

        public long MetaPointer;

        public Stream SubData;

        public Stream ArchiveStream;

        public override byte[] FileData
        {
            get
            {
                List<byte[]> Data = new List<byte[]>();

                using (var reader = new FileReader(SubData, true))
                {
                    var data = reader.ReadBytes((int)reader.BaseStream.Length);

                    reader.Position = 0;
                    if (AssetEntry.DecompressedSize != AssetEntry.Size)
                        data = IOFileExtension.DecompressedBuffer(reader, (uint)AssetEntry.Size, (uint)AssetEntry.DecompressedSize, true);

                    Data.Add(data);

                    if (WriteMetaData)
                    {
                        using (var r = new FileReader(ArchiveStream, true)) {
                            r.SetByteOrder(!ParentArchive.PakData.IsLittleEndian);

                            Data.Add(FileForm.WriteMetaFooter(r, (uint)MetaPointer, AssetEntry.Type, ParentArchive.PakData));
                        }
                    }
                }

                if (AssetEntry.Type == "TXTR")
                {
                    var txt = new TXTR();
                     return txt.CreateUncompressedFile(Utils.CombineByteArray(Data.ToArray()), ParentArchive.PakData.FileHeader, ParentArchive.PakData.IsMPR);
                }


                return Utils.CombineByteArray(Data.ToArray());
            }
        }

        public bool WriteMetaData
        {
            get
            {
                switch (AssetEntry.Type)
                {
                    case "CMDL":
                    case "SMDL":
                    case "WMDL":
                    case "TXTR":
                        return true;
                    default:
                        return false;
                }
            }
        }

        public override IFileFormat OpenFile()
        {
            var pak = this.ParentArchive;

            var file = base.OpenFile();
            if (file is CModel)
            {
                ((CModel)file).LoadTextures(pak.TextureFiles);

                FileEntry GetSkeleton()
                {
                    foreach (var c in pak.CharFiles)
                    {
                        foreach (var m in c.Value.Models)
                        {
                            if (AssetEntry.FileID.ToString() == m.FileID.ToString())
                                return pak.SkeletonFiles[c.Value.SkeletonFileID.ToString()];
                        }
                    }
                    return null;
                }
                var skelFile = GetSkeleton();
                if (skelFile != null)
                {
                    var skel = new SKEL(new MemoryStream(skelFile.FileData));
                    ((CModel)file).LoadSkeleton(skel);
                }
            }
            if (file is CCharacter)
                ((CCharacter)file).LoadModels(pak);
            
            this.FileFormat = file;

            return file;
        }
    }
}
