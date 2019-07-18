using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class SP2 : IFileFormat, IArchiveFile
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Team Sonic Racing Archive" };
        public string[] Extension { get; set; } = new string[] { "*.sp2" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                bool IsMatch = reader.ReadUInt32() == 0xD6D1820C;
                reader.Position = 0;
                return IsMatch || Utils.HasExtension(FileName, ".sp2");
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

        public List<FileInfo> files = new List<FileInfo>();
        public IEnumerable<ArchiveFileInfo> Files => files;

        public void ClearFiles() { files.Clear(); }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        private const uint ChunkTextureFile = 0xD6D1820C;
        private const uint ChunkMetaInfo = 0xB111B40E;
        private const uint ChunkAnimInfo = 0x22008309;
        private const uint ChunkAnimData = 0x29318F0A;
        private const uint ChunkSkeletonData = 0x115AB800;
        private const uint ChunkModelData = 0xCA121903;
        private const uint ChunkShaderData = 0x777A9B0E;
        private const uint ChunkMaterialData = 0x79C90901;

        private List<ChunkHeader> Chunks = new List<ChunkHeader>();
        public void Load(System.IO.Stream stream)
        {
            CanSave = false;

            using (var reader = new FileReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    ChunkHeader chunk = new ChunkHeader();
                    chunk.Position = reader.Position;
                    chunk.Identifier = reader.ReadUInt32();
                    uint unk = reader.ReadUInt32();
                    chunk.ChunkSize = reader.ReadUInt32();
                    chunk.ChunkId = reader.ReadUInt32();
                    chunk.NextFilePtr = reader.ReadUInt32();
                    chunk.FileSize = reader.ReadUInt32();
                    uint unk2 = reader.ReadUInt32();
                    uint unk3 = reader.ReadUInt32();
                    Chunks.Add(chunk);

                    var Identifer = chunk.Identifier.Reverse();

                    switch (Identifer)
                    {
                        case ChunkTextureFile:
                            if (chunk.ChunkSize > 0x88)
                            {
                                reader.Seek(chunk.Position + 0x88, System.IO.SeekOrigin.Begin);
                                chunk.FileName = reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);
                            }
                            break;
                        case ChunkMetaInfo:
                            break;
                        case ChunkAnimInfo:
                            if (chunk.ChunkSize > 0xB0)
                            {
                              //  reader.Seek(chunk.Position + 0xB0, System.IO.SeekOrigin.Begin);
                               // chunk.FileName = reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);
                            }
                            break;
                        case ChunkAnimData:
                            AnimationFile animFile = new AnimationFile();
                            animFile.Read(reader);
                            chunk.ChunkData = animFile;
                            break;
                        case ChunkSkeletonData:
                            SkeletonFile skelFile = new SkeletonFile();
                            skelFile.Read(reader);
                            chunk.ChunkData = skelFile;
                            break;
                        case ChunkModelData:
                            ModelFile modelFile = new ModelFile();
                            modelFile.Read(reader);
                            chunk.ChunkData = modelFile;
                            break;
                        case ChunkMaterialData:
                            MaterialFile matFile = new MaterialFile();
                            matFile.Read(reader);
                            chunk.ChunkData = matFile;
                            break;
                    }

                    reader.Seek(chunk.Position + chunk.ChunkSize, System.IO.SeekOrigin.Begin);
                }

                ReadGPUFile(FilePath);
            }
        }

        private void ReadGPUFile(string FileName)
        {
            string path = FileName.Replace("cpu", "gpu");
            if (!System.IO.File.Exists(path))
                return;

            int offset = 0;
            //Read the data based on CPU chunk info
            using (var reader = new FileReader(path))
            {
                for (int i = 0; i < Chunks.Count; i++)
                {
                    if (Chunks[i].FileSize != 0 || Chunks[i].FileName != string.Empty || Chunks[i].ChunkData != null)
                    {
                        long pos = reader.Position;

                        var identifer = Chunks[i].Identifier.Reverse();

                        var fileInfo = new FileInfo();

                        //Get CPU chunk data
                        if (Chunks[i].ChunkData != null)
                        {
                            if ( Chunks[i].ChunkData is AnimationFile)
                            {
                                AnimationFile animFile = (AnimationFile)Chunks[i].ChunkData;
                                fileInfo.FileName = animFile.FileName;
                                fileInfo.FileData = animFile.Data;
                            }
                            if (Chunks[i].ChunkData is SkeletonFile)
                            {
                                SkeletonFile animFile = (SkeletonFile)Chunks[i].ChunkData;
                                fileInfo.FileName = animFile.FileName;
                                fileInfo.FileData = animFile.Data;
                            }
                            if (Chunks[i].ChunkData is MaterialFile)
                            {
                                MaterialFile animFile = (MaterialFile)Chunks[i].ChunkData;
                                fileInfo.FileName = animFile.FileName;
                                fileInfo.FileData = animFile.Data;
                            }
                            if (Chunks[i].ChunkData is MaterialFile)
                            {
                                MaterialFile animFile = (MaterialFile)Chunks[i].ChunkData;
                                fileInfo.FileName = animFile.FileName;
                                fileInfo.FileData = animFile.Data;
                            }
                            if (Chunks[i].ChunkData is ModelFile)
                            {
                                ModelFile modelFile = (ModelFile)Chunks[i].ChunkData;
                                fileInfo.FileName = modelFile.FileName;

                                byte[] BufferData = new byte[0];
                                if (Chunks[i].FileSize != 0)
                                    BufferData = reader.ReadBytes((int)Chunks[i].FileSize);

                                fileInfo.FileData = Utils.CombineByteArray(modelFile.Data, modelFile.Data2, BufferData);


                                //Don't advance the stream unless the chunk has a pointer
                                if (Chunks[i].NextFilePtr != 0)
                                    reader.Seek(pos + Chunks[i].NextFilePtr, System.IO.SeekOrigin.Begin);
                            }
                        }
                        else //Else get the data from GPU
                        {
                            if (Chunks[i].FileName != string.Empty)
                                fileInfo.FileName = $"{Chunks[i].FileName}";
                            else
                                fileInfo.FileName = $"{i} {Chunks[i].ChunkId} {identifer.ToString("X")}";

                            if (Chunks[i].FileSize != 0)
                                fileInfo.FileData = reader.ReadBytes((int)Chunks[i].FileSize);
                            else
                                fileInfo.FileData = new byte[0];
                        }
                        files.Add(fileInfo);

                        //Don't advance the stream unless the chunk has a pointer
                        if (Chunks[i].NextFilePtr != 0)
                            reader.Seek(pos + Chunks[i].NextFilePtr, System.IO.SeekOrigin.Begin);
                    }
                }
            }
        }

        public void Unload()
        {

        }

        public interface IChunkData { }

        public class ChunkHeader
        {
            public IChunkData ChunkData;

            public uint Identifier;
            public long Position;
            public uint ChunkSize;
            public uint ChunkId;
            public uint NextFilePtr;
            public uint FileSize;

            public string FileName = "";
        }

        //Info in CPU file about the model
        //Note the GPU file chunk linked from this contains the buffers
        public class ModelFile : IChunkData
        {
            public string FileName = "";
            public string FileName2 = ""; //Yeah there's another file for some reason

            public byte[] Data;
            public byte[] Data2;

            public void Read(FileReader reader)
            {
                long pos = reader.Position;

                uint unk3 = reader.ReadUInt32();
                uint unk4 = reader.ReadUInt32(); //Set to 1
                uint SectionSize = reader.ReadUInt32(); //At the end, the file name
                uint Padding = reader.ReadUInt32();
                uint NextSectionOffset = reader.ReadUInt32();

                reader.Seek(pos, System.IO.SeekOrigin.Begin);
                //Model FILE
                Data = reader.ReadBytes((int)SectionSize);

                FileName = reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);

                //Todo
                Data2 = new byte[0];
        //        reader.Seek(NextSectionOffset, System.IO.SeekOrigin.Begin);
        //       Data2 = reader.ReadBytes();
            }
        }

        public class MaterialFile : IChunkData
        {
            public string FileName = "";
            public byte[] Data;

            public void Read(FileReader reader)
            {
                long pos = reader.Position;

                uint unk3 = reader.ReadUInt32();
                uint unk4 = reader.ReadUInt32(); //Set to 1
                uint SectionSize = reader.ReadUInt32(); //At the end, the file name
                uint Padding = reader.ReadUInt32();

                reader.Seek(pos, System.IO.SeekOrigin.Begin);
                //Material FILE
                Data = reader.ReadBytes((int)SectionSize);

                FileName = reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);
            }
        }

        public class SkeletonFile : IChunkData
        {
            public string FileName = "";
            public byte[] Data;

            public void Read(FileReader reader)
            {
                long pos = reader.Position;

                uint unk3 = reader.ReadUInt32();
                uint unk4 = reader.ReadUInt32(); //Set to 1
                uint SectionSize = reader.ReadUInt32(); //At the end, the file name
                uint Padding = reader.ReadUInt32();

                reader.Seek(pos, System.IO.SeekOrigin.Begin);
                //SKEL FILE
                Data = reader.ReadBytes((int)SectionSize);

                FileName = reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);
            }
        }

        public class AnimationFile : IChunkData
        {
            public string FileName = "";
            public byte[] Data;

            public void Read(FileReader reader)
            {
                long pos = reader.Position;

                uint Hash = reader.ReadUInt32(); //Maybe a hash? Idk
                uint unk4 = reader.ReadUInt32(); //Set to 1
                uint SectionSize = reader.ReadUInt32(); //At the end, the file name
                uint Padding = reader.ReadUInt32();

                reader.Seek(pos, System.IO.SeekOrigin.Begin);
                //ANIM FILE
                Data = reader.ReadBytes((int)SectionSize);

                FileName = reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);
            }
        }

        public class TextureFile : IChunkData
        {

        }

        public class TextureInfo : IChunkData
        {

        }


        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
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

        public class FileInfo : ArchiveFileInfo
        {

        }
    }
}
