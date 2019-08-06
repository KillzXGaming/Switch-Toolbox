using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Drawing;
using System.Windows.Forms;

namespace FirstPlugin
{
    public class SWU : TreeNodeFile, IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Sonic and All Stars Racing Transformed Archive  (Wii U)" };
        public string[] Extension { get; set; } = new string[] { "*.swu" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return Utils.HasExtension(FileName, ".swu");
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
                reader.SetByteOrder(true);

                Text = FileName;
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
                            SWUTexture texture = new SWUTexture();
                            reader.SeekBegin(chunk.Position + 72);
                            texture.ImageKey = "texture";
                            texture.SelectedImageKey = "texture";
                            texture.ReadChunk(reader);
                            chunk.ChunkData = texture;
                            if (chunk.ChunkSize > 244)
                            {
                                reader.Seek(chunk.Position + 244, System.IO.SeekOrigin.Begin);
                                chunk.FileName = reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);
                                texture.Text = chunk.FileName;
                            }
                            Nodes.Add(texture);
                            break;
                        case ChunkMetaInfo:
                            break;
                        case ChunkAnimInfo:
                            if (chunk.ChunkSize > 0xB0)
                            {
                                reader.Seek(chunk.Position + 0xB0, System.IO.SeekOrigin.Begin);
                                chunk.FileName = reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);
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

            TreeHelper.CreateFileDirectory(this);
        }

        public class SWUTexture : STGenericTexture, IChunkData
        {
            public byte[] ImageData;

            public FileType FileType { get; set; } = FileType.Image;

            public override bool CanEdit { get; set; } = false;

            public override TEX_FORMAT[] SupportedFormats
            {
                get
                {
                    return new TEX_FORMAT[] {
                    TEX_FORMAT.R8G8B8A8_UNORM,
                };
                }
            }

            public override void OnClick(TreeView treeview)
            {
                ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
                if (editor == null)
                {
                    editor = new ImageEditorBase();
                    editor.Dock = DockStyle.Fill;
                    LibraryGUI.LoadEditor(editor);
                }

                if (GX2Surface != null)
                {
                    var tex = Bfres.Structs.FTEX.FromGx2Surface(GX2Surface, Text);
                    editor.LoadProperties(tex);
                }

                editor.Text = Text;
                editor.LoadImage(this);
            }

            GX2.GX2Surface GX2Surface;

            public void ReadChunk(FileReader reader)
            {
               reader.SetByteOrder(true);

                    Console.WriteLine("TEX pos " + reader.Position);
                    GX2Surface = new GX2.GX2Surface();
                    GX2Surface.dim = reader.ReadUInt32();
                    GX2Surface.width = reader.ReadUInt32();
                    GX2Surface.height = reader.ReadUInt32();
                    GX2Surface.depth = reader.ReadUInt32();
                    GX2Surface.numMips = reader.ReadUInt32();
                    GX2Surface.format = reader.ReadUInt32();
                    GX2Surface.aa = reader.ReadUInt32();
                    GX2Surface.use = reader.ReadUInt32();
                    GX2Surface.imageSize = reader.ReadUInt32();
                    GX2Surface.imagePtr = reader.ReadUInt32();
                    GX2Surface.mipSize = reader.ReadUInt32();
                    GX2Surface.mipPtr = reader.ReadUInt32();
                    GX2Surface.tileMode = reader.ReadUInt32();
                    GX2Surface.swizzle = reader.ReadUInt32();
                    GX2Surface.alignment = reader.ReadUInt32();
                    GX2Surface.pitch = reader.ReadUInt32();
                    GX2Surface.mipOffset = reader.ReadUInt32s(13);
                    GX2Surface.firstMip = reader.ReadUInt32();
                    GX2Surface.imageCount = reader.ReadUInt32();
                    GX2Surface.firstSlice = reader.ReadUInt32();
                    GX2Surface.numSlices = reader.ReadUInt32();
                    GX2Surface.compSel = reader.ReadBytes(4);
                    GX2Surface.texRegs = reader.ReadUInt32s(4);

                    RedChannel = GX2ChanneToGeneric((Syroot.NintenTools.Bfres.GX2.GX2CompSel)GX2Surface.compSel[0]);
                    GreenChannel = GX2ChanneToGeneric((Syroot.NintenTools.Bfres.GX2.GX2CompSel)GX2Surface.compSel[1]);
                    BlueChannel = GX2ChanneToGeneric((Syroot.NintenTools.Bfres.GX2.GX2CompSel)GX2Surface.compSel[2]);
                    AlphaChannel = GX2ChanneToGeneric((Syroot.NintenTools.Bfres.GX2.GX2CompSel)GX2Surface.compSel[3]);

                    if (GX2Surface.numMips > 13)
                        return;

                    Width = GX2Surface.width;
                    Height = GX2Surface.height;
                    MipCount = GX2Surface.numMips;
                    ArrayCount = GX2Surface.numArray;
                    Format = Bfres.Structs.FTEX.ConvertFromGx2Format((Syroot.NintenTools.Bfres.GX2.GX2SurfaceFormat)GX2Surface.format);
            }

            private STChannelType GX2ChanneToGeneric(Syroot.NintenTools.Bfres.GX2.GX2CompSel comp)
            {
                if (comp == Syroot.NintenTools.Bfres.GX2.GX2CompSel.ChannelR) return STChannelType.Red;
                else if (comp == Syroot.NintenTools.Bfres.GX2. GX2CompSel.ChannelG) return STChannelType.Green;
                else if (comp == Syroot.NintenTools.Bfres.GX2.GX2CompSel.ChannelB) return STChannelType.Blue;
                else if (comp == Syroot.NintenTools.Bfres.GX2.GX2CompSel.ChannelA) return STChannelType.Alpha;
                else if (comp == Syroot.NintenTools.Bfres.GX2.GX2CompSel.Always0) return STChannelType.Zero;
                else return STChannelType.One;
            }

            public override void SetImageData(Bitmap bitmap, int ArrayLevel)
            {
                throw new NotImplementedException("Cannot set image data! Operation not implemented!");
            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
            {
                if (GX2Surface != null)
                {
                    GX2Surface.data = ImageData;
                    GX2Surface.mipData = ImageData;

                    return GX2.Decode(GX2Surface, ArrayLevel, MipLevel);
                }
                else
                {
                    return ImageData;
                }
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
                            if (Chunks[i].ChunkData is SWUTexture)
                            {
                                SWUTexture texFile = (SWUTexture)Chunks[i].ChunkData;
                                if (Chunks[i].FileSize != 0)
                                    texFile.ImageData = reader.ReadBytes((int)Chunks[i].FileSize);

                                continue;
                            }
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

        public class FileInfo : ArchiveFileInfo
        {

        }
    }
}
