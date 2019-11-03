using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Drawing;

namespace FirstPlugin.LuigisMansion3
{
    //Parse info based on https://github.com/TheFearsomeDzeraora/LM3L
    public class LM3_DICT : TreeNodeFile, IFileFormat, ILeaveOpenOnLoad
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Luigi's Mansion 3 Dictionary" };
        public string[] Extension { get; set; } = new string[] { "*.dict" };
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
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                if (reader.ReadUInt32() == 0x5824F3A9)
                {
                    //This value seems consistant enough to tell apart from LM3
                    reader.SeekBegin(12);
                    return reader.ReadUInt32() == 0x78340300;
                }

                return false;
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

        public override void OnAfterAdded()
        {
            if (!DrawablesLoaded)
            {
                ObjectEditor.AddContainer(DrawableContainer);
                DrawablesLoaded = true;
            }
        }

        public static bool DebugMode = true;

        public List<ChunkDataEntry> chunkEntries = new List<ChunkDataEntry>();

        public bool IsCompressed = false;

        public LM3_ChunkTable ChunkTable;
        public List<FileEntry> fileEntries = new List<FileEntry>();

        public LM3_Renderer Renderer;
        public DrawableContainer DrawableContainer = new DrawableContainer();

        STTextureFolder textureFolder = new STTextureFolder("Textures");
        LM3_ModelFolder modelFolder;
        TreeNode materialNamesFolder = new TreeNode("Material Names");
        TreeNode chunkFolder = new TreeNode("Chunks");

        public static Dictionary<uint, string> HashNames = new Dictionary<uint, string>();



        public List<string> StringList = new List<string>();
        private void LoadHashes()
        {
            foreach (string hashStr in Properties.Resources.LM3_Hashes.Split('\n'))
            {
                uint hash = (uint)NLG_Common.StringToHash(hashStr);
                if (!HashNames.ContainsKey(hash))
                    HashNames.Add(hash, hashStr);

                string[] hashPaths = hashStr.Split('/');
                for (int i = 0; i < hashPaths?.Length; i++)
                {
                    hash = (uint)NLG_Common.StringToHash(hashPaths[i]);
                    if (!HashNames.ContainsKey(hash))
                        HashNames.Add(hash, hashStr);
                }
            }
        }

        
        public System.IO.Stream GetFileBufferData()
        {
            return fileEntries[54].GetData(); //Get the fourth file
        }

        public System.IO.Stream GetFileVertexData()
        {
           return fileEntries[60].GetData(); //Get the fourth file
        }

        private ushort Unknown0x4;

        public List<ChunkInfo> ChunkInfos = new List<ChunkInfo>();

        public bool DrawablesLoaded = false;
        public void Load(System.IO.Stream stream)
        {
            CanSave = false;

            LoadHashes();
            modelFolder = new LM3_ModelFolder(this); 
            DrawableContainer.Name = FileName;
            Renderer = new LM3_Renderer();
            DrawableContainer.Drawables.Add(Renderer);
            
            Text = FileName;

            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                uint Identifier = reader.ReadUInt32();
                Unknown0x4 = reader.ReadUInt16(); //Could also be 2 bytes, not sure. Always 0x0401
                IsCompressed = reader.ReadByte() == 1;
                reader.ReadByte(); //Padding
                uint SizeLargestFile = reader.ReadUInt32();
                byte numFiles = reader.ReadByte();
                byte numChunkInfos = reader.ReadByte();
                byte numStrings = reader.ReadByte();
                reader.ReadByte(); //padding

                //Start of the chunk info. A fixed list of chunk information

                for (int i = 0; i < numChunkInfos; i++)
                {
                    ChunkInfo chunk = new ChunkInfo();
                    chunk.Read(reader);
                    ChunkInfos.Add(chunk);
                }

                TreeNode tableNodes = new TreeNode("File Section Entries");
                TreeNode chunkLookupNodes = new TreeNode("Chunk Lookup Files");
                tableNodes.Nodes.Add(chunkLookupNodes);

                Nodes.Add(tableNodes);

                TreeNode stringFolder = new TreeNode("Strings");
                TreeNode chunkTexFolder = new TreeNode("Texture");
                TreeNode chunkModelFolder = new TreeNode("Model");

                long FileTablePos = reader.Position;
                for (int i = 0; i < numFiles; i++)
                {
                    var file = new FileEntry(this);
                    file.Read(reader);
                    fileEntries.Add(file);

                    if (file.DecompressedSize > 0)
                    {
                        file.Text = $"entry {i}";

                        if (i < 52)
                            chunkLookupNodes.Nodes.Add(file);
                        else
                            tableNodes.Nodes.Add(file);
                    }

                    //The first file stores a chunk layout
                    //The second one seems to be a duplicate? 
                    if (i == 0)
                    {
                        using (var tableReader = new FileReader(file.GetData()))
                        {
                            ChunkTable = new LM3_ChunkTable();
                            ChunkTable.Read(tableReader);

                            if (DebugMode)
                            {
                                TreeNode debugFolder = new TreeNode("DEBUG TABLE INFO");
                                Nodes.Add(debugFolder);

                                TreeNode list1 = new TreeNode("Entry List 1");
                                TreeNode list2 = new TreeNode("Entry List 2 ");
                                debugFolder.Nodes.Add(list1);
                                debugFolder.Nodes.Add(list2);
                                debugFolder.Nodes.Add(chunkFolder);

                                foreach (var chunk in ChunkTable.ChunkEntries)
                                {
                                    list1.Nodes.Add($"ChunkType {chunk.ChunkType.ToString("X")} ChunkOffset {chunk.ChunkOffset} ChunkSize {chunk.ChunkSize}  ChunkSubCount {chunk.ChunkSubCount}  Unknown3 {chunk.Unknown3}");
                                }
                                foreach (var chunk in ChunkTable.ChunkSubEntries)
                                {
                                    list2.Nodes.Add($"ChunkType 0x{chunk.ChunkType.ToString("X")} Size {chunk.ChunkSize}   Offset {chunk.ChunkOffset}");
                                }
                            }   
                        }
                    }
                }

                for (int i = 0; i < numStrings; i++)
                {
                    StringList.Add(reader.ReadZeroTerminatedString());
                    stringFolder.Nodes.Add(StringList[i]);
                }

                TreeNode havokFolder = new TreeNode("Havok Physics");

                //Model data block
                //Contains texture hash refs and model headers
                var File052Data = fileEntries[52].GetData();

                //Unsure, layout data??
                var File053Data = fileEntries[53].GetData();

                //Contains model data
                var File054Data = fileEntries[54].GetData();

                //Image header block. Also contains shader data
                var File063Data = fileEntries[63].GetData();

                //Image data block
                var File065Data = fileEntries[65].GetData();

                //Get a list of chunk hashes

                List<uint> ModelHashes = new List<uint>();
                for (int i = 0; i < ChunkTable.ChunkEntries.Count; i++)
                {
                    if (ChunkTable.ChunkEntries[i].ChunkType == DataType.Model)
                    {
                        using (var chunkReader = new FileReader(File052Data, true))
                        {
                            chunkReader.SeekBegin(ChunkTable.ChunkEntries[i].ChunkOffset);
                            uint magic = chunkReader.ReadUInt32();
                            uint hash = chunkReader.ReadUInt32();
                            ModelHashes.Add(hash);
                        }
                    }
                }

                //Set an instance of our current data
                //Chunks are in order, so you build off of when an instance gets loaded
                LM3_Model currentModel = new LM3_Model(this);

                TexturePOWE currentTexture = new TexturePOWE();
                ChunkDataEntry currentVertexPointerList = null;

                List<uint> TextureHashes = new List<uint>();

                int chunkId = 0;
                uint modelIndex = 0;
                uint ImageHeaderIndex = 0;
                uint havokFileIndex = 0;
                foreach (var chunk in ChunkTable.ChunkSubEntries)
                {
                    Console.WriteLine($"chunk.ChunkType {chunk.ChunkType}");
                    var chunkEntry = new ChunkDataEntry(this, chunk);
                    chunkEntry.Text = $"{chunkId} {chunk.ChunkType.ToString("X")} {chunk.ChunkType} {chunk.ChunkOffset} {chunk.ChunkSize}";

                    switch (chunk.ChunkType)
                    {
                        case SubDataType.HavokPhysics:
                            chunkEntry.DataFile = File052Data;
                            chunkEntry.Text = $"File_{havokFileIndex++}.hkx";
                            havokFolder.Nodes.Add(chunkEntry);
                            break;
                        case SubDataType.TextureHeader:
                            chunkEntry.DataFile = File063Data;

                            //Read the info
                            using (var textureReader = new FileReader(chunkEntry.FileData, true))
                            {
                                currentTexture = new TexturePOWE();
                                currentTexture.ImageKey = "texture";
                                currentTexture.SelectedImageKey = currentTexture.ImageKey;
                                currentTexture.Index = ImageHeaderIndex;
                                currentTexture.Read(textureReader);
                                if (DebugMode)
                                    currentTexture.Text = $"Texture {ImageHeaderIndex} {currentTexture.Unknown} {currentTexture.Unknown2} {currentTexture.Unknown3.ToString("X")}";
                                else
                                    currentTexture.Text = $"Texture {currentTexture.ID2.ToString("X")}";

                                if (HashNames.ContainsKey(currentTexture.ID2))
                                    currentTexture.Text = HashNames[currentTexture.ID2];

                                textureFolder.Nodes.Add(currentTexture);
                                if (!Renderer.TextureList.ContainsKey(currentTexture.ID2.ToString("x")))
                                    Renderer.TextureList.Add(currentTexture.ID2.ToString("x"), currentTexture);

                                TextureHashes.Add(currentTexture.ID2);

                                ImageHeaderIndex++;
                            }
                            break;
                        case SubDataType.TextureData:
                            chunkEntry.DataFile = File065Data;
                            currentTexture.ImageData = chunkEntry.FileData.ToBytes();
                            break;
                        case SubDataType.ModelStart:
                            chunkEntry.DataFile = File052Data;
                            currentModel = new LM3_Model(this);
                            currentModel.ModelInfo = new LM3_ModelInfo();
                            currentModel.Text = $"Model {modelIndex}";
                            currentModel.ModelInfo.Data = chunkEntry.FileData.ToBytes();
                            if (ModelHashes.Count > modelIndex)
                            {
                                currentModel.Text = $"Model {modelIndex} {ModelHashes[(int)modelIndex].ToString("x")}";
                                if (HashNames.ContainsKey(ModelHashes[(int)modelIndex]))
                                    currentModel.Text = HashNames[ModelHashes[(int)modelIndex]];
                            }

                            modelIndex++;
                            break;
                        case SubDataType.MeshBuffers:
                            chunkEntry.DataFile = File054Data;
                            currentModel.BufferStart = chunkEntry.Entry.ChunkOffset;
                            currentModel.BufferSize = chunkEntry.Entry.ChunkSize;
                            break;
                        case SubDataType.VertexStartPointers:
                            chunkEntry.DataFile = File052Data;
                            currentVertexPointerList = chunkEntry;
                            break;
                        case SubDataType.SubmeshInfo:
                            chunkEntry.DataFile = File052Data;
                            int MeshCount = (int)chunkEntry.FileData.Length / 0x40;

                            using (var vtxPtrReader = new FileReader(currentVertexPointerList.FileData))
                            using (var meshReader = new FileReader(chunkEntry.FileData))
                            {
                                for (uint i = 0; i < MeshCount; i++)
                                {
                                    meshReader.SeekBegin(i * 0x40);
                                    LM3_Mesh mesh = new LM3_Mesh();
                                    mesh.Read(meshReader);
                                    currentModel.Meshes.Add(mesh);

                                    Console.WriteLine($"mesh.Unknown3 {mesh.Unknown3 }");

                                    var buffer = new LM3_Model.PointerInfo();
                                    buffer.Read(vtxPtrReader, mesh.Unknown3 != 4294967295);
                                    currentModel.VertexBufferPointers.Add(buffer);
                                }
                            }

                            modelFolder.Nodes.Add(currentModel);
                            break;
                        case SubDataType.ModelTransform:
                            chunkEntry.DataFile = File052Data;
                            using (var transformReader = new FileReader(chunkEntry.FileData))
                            {
                                //This is possibly very wrong
                                //The data isn't always per mesh, but sometimes is
                                if (transformReader.BaseStream.Length / 0x40 == currentModel.Meshes.Count)
                                {
                                    for (int i = 0; i < currentModel.Meshes.Count; i++)
                                        currentModel.Meshes[i].Transform = transformReader.ReadMatrix4();
                                }
                            }
                            break;
                        case SubDataType.MaterialName:
                            chunkEntry.DataFile = File053Data;
                      /*      using (var matReader = new FileReader(chunkEntry.FileData))
                               {
                                   materialNamesFolder.Nodes.Add(matReader.ReadZeroTerminatedString());
                               }*/
                            break;
                        case SubDataType.UILayoutMagic:
                            chunkEntry.DataFile = File053Data;
                            break;
                        case SubDataType.UILayout:
                            chunkEntry.DataFile = File053Data;
                            break;
                        case SubDataType.BoneData:
                            if (chunk.ChunkSize > 0x40 && currentModel.Skeleton == null)
                            {
                                chunkEntry.DataFile = File052Data;
                                using (var boneReader = new FileReader(chunkEntry.FileData))
                                {
                                    currentModel.Skeleton = new STSkeleton();
                                    DrawableContainer.Drawables.Add(currentModel.Skeleton);

                                    uint numBones = chunk.ChunkSize / 0x40;
                                    for (int i = 0; i < numBones; i++)
                                    {
                                        boneReader.SeekBegin(i * 0x40);
                                        uint hash = boneReader.ReadUInt32();

                                        STBone bone = new STBone(currentModel.Skeleton);
                                        bone.Text = hash.ToString("x");
                                        if (HashNames.ContainsKey(hash))
                                            bone.Text = HashNames[hash];
                                        bone.position = new float[3] { 0, 0, 0 };
                                        bone.rotation = new float[4] { 0, 0, 0, 1 };
                                        bone.scale = new float[3] { 0.2f, 0.2f, 0.2f };


                                        boneReader.SeekBegin(48 + (i * 0x40));
                                        var Position = new OpenTK.Vector3(boneReader.ReadSingle(), boneReader.ReadSingle(), boneReader.ReadSingle());
                                        Position = OpenTK.Vector3.TransformPosition(Position, OpenTK.Matrix4.CreateRotationX(OpenTK.MathHelper.DegreesToRadians(90)));
                                        bone.position[0] = Position.X;
                                        bone.position[2] = Position.Y;
                                        bone.position[1] = Position.Z;


                                        bone.RotationType = STBone.BoneRotationType.Euler;
                                        currentModel.Skeleton.bones.Add(bone);
                                    }

                                    currentModel.Skeleton.reset();
                                    currentModel.Skeleton.update();
                                }
                            }
                            break;
                        case (SubDataType)0x5012:
                        case (SubDataType)0x5013:
                        case (SubDataType)0x5014:
                            chunkEntry.DataFile = File063Data;
                            break;
                        case (SubDataType)0x7101:
                        case (SubDataType)0x7102:
                        case (SubDataType)0x7103:
                        case (SubDataType)0x7104:
                        case (SubDataType)0x7105:
                        case (SubDataType)0x7106:
                        case (SubDataType)0x6503:
                        case (SubDataType)0x6501:
                            chunkEntry.DataFile = File053Data;
                            break;
                        default:
                            chunkEntry.DataFile = File052Data;
                            break;
                    }

                    if (chunk.ChunkType != SubDataType.HavokPhysics)
                        chunkFolder.Nodes.Add(chunkEntry);

                    chunkId++;
                }

                foreach (var model in modelFolder.Nodes)
                {
                    ((LM3_Model)model).ModelInfo.Read(new FileReader(
                        ((LM3_Model)model).ModelInfo.Data), ((LM3_Model)model), TextureHashes);
                }

                if (havokFolder.Nodes.Count > 0)
                    Nodes.Add(havokFolder);

                if (textureFolder.Nodes.Count > 0)
                    Nodes.Add(textureFolder);

                if (modelFolder.Nodes.Count > 0)
                    Nodes.Add(modelFolder);

                if (stringFolder.Nodes.Count > 0)
                    Nodes.Add(stringFolder);
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream))
            {
                writer.SetByteOrder(true);
                writer.Write(0x78340300);
                writer.SetByteOrder(false);
                writer.Write(Unknown0x4);
                writer.Write(IsCompressed);
                writer.Write((byte)0); //padding
                writer.Write(GetLargestFileSize());
                writer.Write((byte)fileEntries.Count);
                writer.Write((byte)ChunkInfos.Count);
                writer.Write((byte)StringList.Count);
                writer.Write((byte)0); //padding

                for (int i = 0; i < ChunkInfos.Count; i++)
                {
                    writer.Write(ChunkInfos[i].Unknown1);
                    writer.Write(ChunkInfos[i].Unknown2);
                    writer.Write(ChunkInfos[i].Unknown3);
                    writer.Write(ChunkInfos[i].Unknown4);
                    writer.Write(ChunkInfos[i].Unknown5);
                    writer.Write(ChunkInfos[i].Unknown6);
                }

                for (int i = 0; i < fileEntries.Count; i++)
                {
           
                }

                for (int i = 0; i < StringList.Count; i++)
                    writer.WriteString(StringList[i]);
            }
        }

        private uint GetLargestFileSize()
        {
            return 0;
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public class ChunkInfo
        {
            public uint Unknown1;
            public uint Unknown2;
            public uint Unknown3;
            public uint Unknown4;
            public uint Unknown5;
            public uint Unknown6;

            public void Read(FileReader reader)
            {
                uint Unknown1 = reader.ReadUInt32();
                uint Unknown2 = reader.ReadUInt32();
                uint Unknown3 = reader.ReadUInt32();
                uint Unknown4 = reader.ReadUInt32();
                uint Unknown5 = reader.ReadUInt32();
                uint Unknown6 = reader.ReadUInt32();
            }
        }

        public class ChunkDataEntry : TreeNodeFile, IContextMenuNode
        {
            public System.IO.Stream DataFile;
            public LM3_DICT ParentDictionary { get; set; }
            public ChunkSubEntry Entry;

            public ChunkDataEntry(LM3_DICT dict, ChunkSubEntry entry)
            {
                ParentDictionary = dict;
                Entry = entry;
            }

            public System.IO.Stream FileData
            {
                get
                {
                    if (Entry.ChunkSize == 0)
                        return new System.IO.MemoryStream();
                    else if (Entry.ChunkOffset + Entry.ChunkSize <= DataFile?.Length)
                        return new SubStream(DataFile, Entry.ChunkOffset, Entry.ChunkSize);
                    else
                        return new System.IO.MemoryStream();
                }
            }

            public ToolStripItem[] GetContextMenuItems()
            {
                List<ToolStripItem> Items = new List<ToolStripItem>();
                Items.Add(new STToolStipMenuItem("Export Raw Data", null, Export, Keys.Control | Keys.E));
                return Items.ToArray();
            }

            private void Export(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = Text;
                sfd.Filter = "Raw Data (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                    FileData.ExportToFile(sfd.FileName);
            }

            public override void OnClick(TreeView treeView)
            {
                HexEditor editor = (HexEditor)LibraryGUI.GetActiveContent(typeof(HexEditor));
                if (editor == null)
                {
                    editor = new HexEditor();
                    LibraryGUI.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.LoadData(FileData);
            }
        }

        public class FileEntry : TreeNodeFile, IContextMenuNode
        {
            public LM3_DICT ParentDictionary { get; set; }

            public uint Offset;
            public uint DecompressedSize;
            public uint CompressedSize;
            public ushort Unknown1; 
            public byte Unknown2;
            public byte Unknown3; //Possibly the effect? 0 for image block, 1 for info

            public FileEntry(LM3_DICT dict)
            {
                ParentDictionary = dict;
            }

            public void Read(FileReader reader)
            {
                Offset = reader.ReadUInt32();
                DecompressedSize = reader.ReadUInt32();
                CompressedSize = reader.ReadUInt32();
                Unknown1 = reader.ReadUInt16();
                Unknown2 = reader.ReadByte();
                Unknown3 = reader.ReadByte();
            }

            public void Write()
            {

            }

            private bool IsTextureBinary()
            {
                var stream = GetData();
                if (stream.Length < 4)
                    return false;

                using (var reader = new FileReader(stream, true))
                {
                    return reader.ReadUInt32() == 0xE977D350;
                }
            }

            public ToolStripItem[] GetContextMenuItems()
            {
                List<ToolStripItem> Items = new List<ToolStripItem>();
                Items.Add(new STToolStipMenuItem("Export Raw Data", null, Export, Keys.Control | Keys.E));
                return Items.ToArray();
            }

            private void Export(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = Text;
                sfd.Filter = "Raw Data (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                    GetData().ExportToFile(sfd.FileName);
            }

            public override void OnClick(TreeView treeView)
            {
                HexEditor editor = (HexEditor)LibraryGUI.GetActiveContent(typeof(HexEditor));
                if (editor == null)
                {
                    editor = new HexEditor();
                    LibraryGUI.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.LoadData(GetData());
            }

            public System.IO.Stream GetData()
            {
                System.IO.Stream Data = new System.IO.MemoryStream();

                string FolderPath = System.IO.Path.GetDirectoryName(ParentDictionary.FilePath);
                string DataFile = System.IO.Path.Combine(FolderPath, $"{ParentDictionary.FileName.Replace(".dict", ".data")}");

                if (System.IO.File.Exists(DataFile))
                {
                    using (var reader = new FileReader(DataFile, true))
                    {
                        if (Offset > reader.BaseStream.Length)
                            return new System.IO.MemoryStream();

                        reader.SeekBegin(Offset);
                        if (ParentDictionary.IsCompressed)
                        {
                            ushort Magic = reader.ReadUInt16();
                            reader.SeekBegin(Offset);

                            if (Magic == 0x9C78 || Magic == 0xDA78)
                                return new System.IO.MemoryStream(STLibraryCompression.ZLIB.Decompress(reader.ReadBytes((int)CompressedSize)));
                            else //Unknown compression 
                                return Data;
                        }
                        else if (DecompressedSize == 0)
                            return new System.IO.MemoryStream();
                        else if (Offset + DecompressedSize <= reader.BaseStream.Length)
                            return new SubStream(reader.BaseStream, Offset, DecompressedSize);
                    }
                }

                return Data;
            }
        }
    }
}
