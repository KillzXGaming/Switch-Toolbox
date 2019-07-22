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

namespace FirstPlugin.LuigisMansion.DarkMoon
{
    //Parse info based on https://github.com/TheFearsomeDzeraora/LM2L
    public class LM2_DICT : TreeNodeFile, IFileFormat, IArchiveFile
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Luigi's Mansion 2 Dark Moon Archive Dictionary" };
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
                return reader.ReadUInt32() == 0x5824F3A9;
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

        public List<ChunkDataEntry> files = new List<ChunkDataEntry>();
        public IEnumerable<ArchiveFileInfo> Files => files;

        public void ClearFiles() { files.Clear(); }

        public bool IsCompressed = false;

        public LM2_ChunkTable ChunkTable;
        public List<FileEntry> fileEntries = new List<FileEntry>();

        public LM2_Renderer Renderer;
        public DrawableContainer DrawableContainer = new DrawableContainer();

        TreeNode textureFolder = new TreeNode("Textures");
        TreeNode modelFolder = new TreeNode("Models");

        public byte[] GetFile003Data()
        {
           return fileEntries[3].GetData(); //Get the fourth file
        }

        public void Load(System.IO.Stream stream)
        {
            DrawableContainer.Name = FileName;
            Renderer = new LM2_Renderer();
            DrawableContainer.Drawables.Add(Renderer);

            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                uint Identifier = reader.ReadUInt32();
                ushort Unknown = reader.ReadUInt16(); //Could also be 2 bytes, not sure. Always 0x0401
                IsCompressed = reader.ReadByte() == 1;
                reader.ReadByte(); //Padding
                uint FileCount = reader.ReadUInt32();
                uint LargestCompressedFile = reader.ReadUInt32();

                reader.SeekBegin(0x2C);
                byte[] Unknowns = reader.ReadBytes((int)FileCount);

                TreeNode tableNodes = new TreeNode("File Section Entries");

                long FileTablePos = reader.Position;
                for (int i = 0; i < FileCount; i++)
                {
                    var file = new FileEntry(this);
                    file.Text = $"entry {i}";
                    file.Read(reader);
                    fileEntries.Add(file);
                    tableNodes.Nodes.Add(file);

                    //The first file stores a chunk layout
                    //The second one seems to be a duplicate? 
                    if (i == 0) 
                    {
                        using (var tableReader = new FileReader(file.GetData()))
                        {
                            ChunkTable = new LM2_ChunkTable();
                            ChunkTable.Read(tableReader);

                            TreeNode debugFolder = new TreeNode("DEBUG TABLE INFO");
                            Nodes.Add(debugFolder);

                            TreeNode list1 = new TreeNode("Entry List 1");
                            TreeNode list2 = new TreeNode("Entry List 2 ");
                            debugFolder.Nodes.Add(tableNodes);
                            debugFolder.Nodes.Add(list1);
                            debugFolder.Nodes.Add(list2);

                            foreach (var chunk in ChunkTable.ChunkEntries)
                            {
                                list1.Nodes.Add($"ChunkType {chunk.ChunkType} ChunkOffset {chunk.ChunkOffset}  Unknown1 {chunk.Unknown1}  ChunkSubCount {chunk.ChunkSubCount}  Unknown3 {chunk.Unknown3}");
                            }
                            foreach (var chunk in ChunkTable.ChunkSubEntries)
                            {
                                list2.Nodes.Add($"ChunkType {chunk.ChunkType} ChunkSize {chunk.ChunkSize}   Unknown {chunk.ChunkOffset}");
                            }
                        }
                    }
                }

                //Set an instance of our current data
                //Chunks are in order, so you build off of when an instance gets loaded
                TexturePOWE currentTexture = new TexturePOWE();
                LM2_Model currentModel = new LM2_Model(this);

                //Each part of the file is divided into multiple file/section entries
                //The first entry being the chunk table parsed before this
                //The second file being a duplicate (sometimes slightly larger than the first)
                //The third file stores texture headers, while the fourth one usually has the rest of the main data
                //Any additional ones currently are unknown how they work. Some of which have unknown compression aswell

                byte[] File002Data = fileEntries[2].GetData(); //Get the third file 
                byte[] File003Data = fileEntries[3].GetData(); //Get the fourth file

                int chunkId = 0;
                uint ImageHeaderIndex = 0;
                uint modelIndex = 0;
                foreach (var chunk in ChunkTable.ChunkSubEntries)
                {
                    var chunkEntry = new ChunkDataEntry(this, chunk);
                    chunkEntry.DataFile = File003Data;
                    chunkEntry.FileName = $"Chunk {chunk.ChunkType} {chunkId++}";
                    files.Add(chunkEntry);

                    switch (chunk.ChunkType)
                    {
                        case SubDataType.TextureHeader:
                            chunkEntry.DataFile = File002Data;

                            //Read the info
                            using (var textureReader = new FileReader(chunkEntry.FileData))
                            {
                                currentTexture = new TexturePOWE();
                                currentTexture.ImageKey = "texture";
                                currentTexture.SelectedImageKey = currentTexture.ImageKey;
                                currentTexture.Index = ImageHeaderIndex;
                                currentTexture.Read(textureReader);
                                currentTexture.Text = $"Texture {ImageHeaderIndex}";
                                textureFolder.Nodes.Add(currentTexture);

                                ImageHeaderIndex++;
                            }
                            break;
                        case SubDataType.TextureData:
                            currentTexture.ImageData = chunkEntry.FileData;
                            break;
                        case SubDataType.ModelStart:
                            currentModel = new LM2_Model(this);
                            currentModel.ModelInfo = new LM2_ModelInfo();
                            currentModel.Text = $"Model {modelIndex}";
                            currentModel.ModelInfo.Data = chunkEntry.FileData;
                            modelFolder.Nodes.Add(currentModel);
                            modelIndex++;
                            break;
                        case SubDataType.MeshBuffers:
                            currentModel.BufferStart = chunkEntry.Entry.ChunkOffset;
                            currentModel.BufferSize = chunkEntry.Entry.ChunkSize;
                            break;
                        case SubDataType.VertexStartPointers:
                            using (var vtxPtrReader = new FileReader(chunkEntry.FileData))
                            {
                                while (!vtxPtrReader.EndOfStream)
                                    currentModel.VertexBufferPointers.Add(vtxPtrReader.ReadUInt32());
                            }
                            break;
                        case SubDataType.SubmeshInfo:
                            int MeshCount = chunkEntry.FileData.Length / 0x28;
                            using (var meshReader = new FileReader(chunkEntry.FileData))
                            {
                                for (uint i = 0; i < MeshCount; i++)
                                {
                                    LM2_Mesh mesh = new LM2_Mesh();
                                    mesh.Read(meshReader);
                                    currentModel.Meshes.Add(mesh);
                                }
                            }
                            break;
                        case SubDataType.ModelTransform:
                            using (var transformReader = new FileReader(chunkEntry.FileData))
                            {
                                if (transformReader.BaseStream.Length == 0x40)
                                    currentModel.Transform = transformReader.ReadMatrix4();
                                else
                                    currentModel.Transform = OpenTK.Matrix4.Identity;
                            }
                            break;
                        default:
                            break;
                    }
                }

                foreach (LM2_Model model in modelFolder.Nodes)
                {
                    model.ReadVertexBuffers();
                }

                if (modelFolder.Nodes.Count > 0)
                    Nodes.Add(modelFolder);

                if (textureFolder.Nodes.Count > 0)
                    Nodes.Add(textureFolder);
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

        public class ChunkDataEntry : ArchiveFileInfo
        {
            public byte[] DataFile;
            public LM2_DICT ParentDictionary { get; set; }
            public ChunkSubEntry Entry;

            public ChunkDataEntry(LM2_DICT dict, ChunkSubEntry entry)
            {
                ParentDictionary = dict;
                Entry = entry;
            }

            public override byte[] FileData
            {
                get { return GetData(); }
                set
                {

                }
            }

            private byte[] GetData( )
            {
                using (var reader = new FileReader(DataFile))
                {
                    reader.SeekBegin(Entry.ChunkOffset);
                    return reader.ReadBytes((int)Entry.ChunkSize);
                }
            }
        }

        public class FileEntry : TreeNodeFile, IContextMenuNode
        {
            public LM2_DICT ParentDictionary { get; set; }

            public uint Offset;
            public uint DecompressedSize;
            public uint CompressedSize;
            public ushort Unknown1; 
            public byte Unknown2;
            public byte Unknown3; //Possibly the effect? 0 for image block, 1 for info

            public FileEntry(LM2_DICT dict)
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

            private bool IsTextureBinary()
            {
                byte[] Data = GetData();

                if (Data.Length < 4)
                    return false;

                using (var reader = new FileReader(Data))
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
                {
                    System.IO.File.WriteAllBytes(sfd.FileName, GetData());
                }
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

            public byte[] GetData()
            {
                byte[] Data = new byte[DecompressedSize];

                string FolderPath = System.IO.Path.GetDirectoryName(ParentDictionary.FilePath);
                string DataFile = System.IO.Path.Combine(FolderPath, $"{ParentDictionary.FileName.Replace(".dict", ".data")}");

                if (System.IO.File.Exists(DataFile))
                {
                    using (var reader = new FileReader(DataFile))
                    {
                        reader.SeekBegin(Offset);
                        if (ParentDictionary.IsCompressed)
                        {
                            ushort Magic = reader.ReadUInt16();
                            reader.SeekBegin(Offset);

                            Data = reader.ReadBytes((int)CompressedSize);
                            if (Magic == 0x9C78 || Magic == 0xDA78)
                                return STLibraryCompression.ZLIB.Decompress(Data);
                            else //Unknown compression 
                                return Data;
                        }
                        else
                        {
                            return reader.ReadBytes((int)DecompressedSize);
                        }
                    }
                }

                return Data;
            }
        }
    }
}
