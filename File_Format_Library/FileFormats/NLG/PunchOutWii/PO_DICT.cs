using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using Toolbox.Library.Rendering;

namespace FirstPlugin.PunchOutWii
{
    public enum MaterailPresets : uint
    {
        EnvDiffuseDamage = 0x1ACE1D01,
    }

    public class PO_DICT : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Punch Out Wii Dictionary" };
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
                return reader.ReadUInt32() == 0xA9F32458;
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

        private bool DrawablesLoaded;
        public override void OnAfterAdded()
        {
            if (!DrawablesLoaded)
            {
                ObjectEditor.AddContainer(DrawableContainer);
                DrawablesLoaded = true;
            }
        }

        public DrawableContainer DrawableContainer = new DrawableContainer();

        public PunchOutWii_Renderer Renderer;
        private DictionaryFile HeaderFile;

        public void Load(System.IO.Stream stream)
        {
            DrawableContainer.Name = FileName;
            Renderer = new PunchOutWii_Renderer();
            DrawableContainer.Drawables.Add(Renderer);

            Text = FileName;

            HeaderFile = new DictionaryFile();
            HeaderFile.Read(new FileReader(stream), FilePath);

            var HashList = NLG_Common.HashNames;

            string DataFile = $"{FilePath.Replace(".dict", ".data")}";
            if (System.IO.File.Exists(DataFile)) {
                using (var reader = new FileReader(DataFile, true))
                {
                    reader.SetByteOrder(true);

                    TreeNode blocks = new TreeNode("Blocks");
                    TreeNode chunks = new TreeNode("Chunks");
                    TreeNode modelFolder = new TreeNode("Models");

                    foreach (var blockInfo in HeaderFile.Blocks)
                    {
                        ChunkViewer chunkNode = new ChunkViewer("block");
                        if (blockInfo.Size > 0)
                            blocks.Nodes.Add(chunkNode);

                        chunkNode.FileData = new SubStream(reader.BaseStream, blockInfo.Offset, blockInfo.Size);
                    }

                    List<PO_Texture> currentTextures = new List<PO_Texture>();

                    List<ModelFileData> modelData = new List<ModelFileData>();

                    ModelFileData currentModel = null;

                    STTextureFolder textureFolder = new STTextureFolder("Textures");
                    Nodes.Add(blocks);
                    Nodes.Add(chunks);

                    Nodes.Add(textureFolder);
                    Nodes.Add(modelFolder);

                    foreach (var chunk in HeaderFile.DataChunks)
                    {
                        if (chunk.BlockIndex == -1)
                            continue;

                        ChunkViewer chunkNode = new ChunkViewer(chunk.Type.ToString("") + " " + chunk.Type.ToString("X"));
                        chunks.Nodes.Add(chunkNode);

                        var blockInfo = HeaderFile.Blocks[chunk.BlockIndex];
                        if (blockInfo.Offset + chunk.Offset + chunk.Size > reader.BaseStream.Length)
                            continue;

                        chunkNode.FileData = new SubStream(reader.BaseStream, blockInfo.Offset + chunk.Offset, chunk.Size);

                        uint chunkPos = blockInfo.Offset + chunk.Offset;

                        reader.SeekBegin(chunkPos);
                        switch (chunk.Type)
                        {
                            case SectionMagic.MaterialData:
                                currentModel = new ModelFileData();
                                currentModel.MaterialOffset = chunkPos;
                                modelData.Add(currentModel);
                                break;
                            case SectionMagic.TextureHeaders:
                                uint numTextures = chunk.Size / 96;
                                for (int i = 0; i < numTextures; i++)
                                {
                                    var tex = new PO_Texture();
                                    tex.ImageKey = "texture";
                                    tex.SelectedImageKey = "texture";
                                   
                                    tex.Read(reader);
                                    tex.Text = tex.HashID.ToString("X");
                                    if (HashList.ContainsKey(tex.HashID))
                                        tex.Text = HashList[tex.HashID];

                                    currentTextures.Add(tex);
                                    Renderer.TextureList.Add(tex.Text, tex);

                                    textureFolder.Nodes.Add(tex);
                                }
                                break;
                            case SectionMagic.TextureData:
                                for (int i = 0; i < currentTextures.Count; i++)
                                {
                                    reader.SeekBegin(chunkPos + currentTextures[i].DataOffset);
                                    currentTextures[i].ImageData = reader.ReadBytes((int)currentTextures[i].ImageSize);
                                }
                                break;
                            case SectionMagic.IndexData:
                                currentModel.indexBufferOffset = chunkPos;
                                break;
                            case SectionMagic.VertexData:
                                currentModel.vertexBufferOffset = chunkPos;
                                break;
                            case SectionMagic.MeshData:
                                uint numMeshes = chunk.Size / 52;
                                for (int i = 0; i < numMeshes; i++)
                                {
                                    reader.SeekBegin(chunkPos + (i * 52));
                                    PO_Mesh mesh = new PO_Mesh(reader);
                                    currentModel.meshes.Add(mesh);
                                }
                                break;
                            case SectionMagic.VertexAttributePointerData:
                                uint numAttributes = chunk.Size / 8;
                                for (int i = 0; i < numAttributes; i++)
                                {
                                    PO_VertexAttribute att = new PO_VertexAttribute();
                                    att.Offset = reader.ReadUInt32();
                                    att.Type = reader.ReadByte();
                                    att.Stride = reader.ReadByte();
                                    reader.ReadUInt16();
                                    currentModel.attributes.Add(att);
                                }
                                break;
                            case SectionMagic.ModelData:
                                uint numModels = chunk.Size / 12;
                                Console.WriteLine($"numModels {numModels}");
                                for (int i = 0; i < numModels; i++)
                                {
                                    PO_Model mdl = new PO_Model();
                                    mdl.ParentDictionary = this;
                                    mdl.HashID = reader.ReadUInt32();
                                    mdl.NumMeshes = reader.ReadUInt32();
                                    reader.ReadUInt32(); //0
                                    currentModel.models.Add(mdl);
                                }
                                break;
                            case SectionMagic.BoneData:
                                STSkeleton Skeleton = new STSkeleton();
                                DrawableContainer.Drawables.Add(Skeleton);

                                uint numBones = chunk.Size / 68;
                                for (int i = 0; i < numBones; i++)
                                {
                                    reader.SeekBegin(chunkPos + (i * 68));

                                    uint HashID = reader.ReadUInt32();
                                    reader.ReadUInt32(); //unk
                                    reader.ReadUInt32(); //unk
                                    reader.ReadUInt32(); //unk
                                    reader.ReadSingle(); //0
                                    var Scale = new OpenTK.Vector3(
                                       reader.ReadSingle(),
                                       reader.ReadSingle(),
                                       reader.ReadSingle());
                                    reader.ReadSingle(); //0
                                    var Rotate = new OpenTK.Vector3(
                                       reader.ReadSingle(),
                                       reader.ReadSingle(),
                                       reader.ReadSingle());
                                    reader.ReadSingle(); //0
                                    var Position = new OpenTK.Vector3(
                                        reader.ReadSingle(),
                                        reader.ReadSingle(),
                                        reader.ReadSingle());
                                    reader.ReadSingle(); //1

                                    STBone bone = new STBone(Skeleton);
                                    bone.Text = HashID.ToString("X");
                                    if (NLG_Common.HashNames.ContainsKey(HashID))
                                        bone.Text = NLG_Common.HashNames[HashID];
                                    else
                                        Console.WriteLine($"bone hash {HashID}");

                                    bone.position = new float[3] { Position.X, Position.Z, -Position.Y };
                                    bone.rotation = new float[4] { Rotate.X, Rotate.Z, -Rotate.Y, 1 };
                                    bone.scale = new float[3] { 0.2f, 0.2f, 0.2f };

                                    bone.RotationType = STBone.BoneRotationType.Euler;
                                    Skeleton.bones.Add(bone);
                                }

                                Skeleton.reset();
                                Skeleton.update();
                                break;
                        }
                    }

                    foreach (var modelFile in modelData)
                    {
                        int pointerIndex = 0;
                        foreach (var model in modelFile.models)
                        {
                            model.Text = model.HashID.ToString("X");
                            if (HashList.ContainsKey(model.HashID))
                                model.Text = HashList[model.HashID];

                            modelFolder.Nodes.Add(model);
                            for (int i = 0; i < model.NumMeshes; i++)
                            {
                                var mesh = modelFile.meshes[i];

                                RenderableMeshWrapper genericMesh = new RenderableMeshWrapper();
                                model.Nodes.Add(genericMesh);
                                model.RenderedMeshes.Add(genericMesh);
                                Renderer.Meshes.Add(genericMesh);

                                genericMesh.Text = mesh.HashID.ToString("X");
                                if (HashList.ContainsKey(mesh.HashID))
                                    genericMesh.Text = HashList[mesh.HashID];

                                string material = mesh.MaterialHashID.ToString("X");
                                if (HashList.ContainsKey(mesh.MaterialHashID))
                                    material = HashList[mesh.MaterialHashID];
                                genericMesh.Nodes.Add(material);


                                genericMesh.Material = new STGenericMaterial();

                                reader.SeekBegin(modelFile.MaterialOffset + mesh.MaterialOffset);
                                switch (mesh.MaterailPreset)
                                {
                                    case MaterailPresets.EnvDiffuseDamage:
                                        {
                                            uint diffuseMapHashID = reader.ReadUInt32();
                                            uint diffuseMapParam = reader.ReadUInt32();
                                            uint envSpecMapHashID = reader.ReadUInt32();
                                            uint envSpecMapParam = reader.ReadUInt32();
                                            uint specMapHashID = reader.ReadUInt32();
                                            uint specMapParam = reader.ReadUInt32();
                                            uint megaStrikeMapHashID = reader.ReadUInt32();
                                            uint megaStrikeMapParam = reader.ReadUInt32();
                                            uint dirtMapHashID = reader.ReadUInt32();
                                            uint dirtMapParam = reader.ReadUInt32();
                                            uint iceMapHashID = reader.ReadUInt32();
                                            uint iceMapParam = reader.ReadUInt32();

                                            string diffuseName = diffuseMapHashID.ToString("X");
                                            if (HashList.ContainsKey(diffuseMapHashID))
                                                diffuseName = HashList[diffuseMapHashID];

                                            var texUnit = 1;
                                            genericMesh.Material.TextureMaps.Add(new STGenericMatTexture()
                                            {
                                                textureUnit = texUnit++,
                                                Type = STGenericMatTexture.TextureType.Diffuse,
                                                Name = diffuseName,
                                            });
                                        }
                                        break;
                                    default:
                                        {
                                            uint diffuseMapHashID = reader.ReadUInt32();
                                            string diffuseName = diffuseMapHashID.ToString("X");
                                            if (HashList.ContainsKey(diffuseMapHashID))
                                                diffuseName = HashList[diffuseMapHashID];

                                            Console.WriteLine($"diffuseName {diffuseName}");

                                            var texUnit = 1;
                                            genericMesh.Material.TextureMaps.Add(new STGenericMatTexture()
                                            {
                                                textureUnit = texUnit++,
                                                Type = STGenericMatTexture.TextureType.Diffuse,
                                                Name = diffuseName,
                                            });
                                        }
                                        break;
                                }

                                Console.WriteLine($"mesh {i}");

                                STGenericPolygonGroup polyGroup = new STGenericPolygonGroup();
                                genericMesh.PolygonGroups.Add(polyGroup);
                                reader.SeekBegin(modelFile.indexBufferOffset + mesh.IndexStartOffset);

                                List<int> faces = new List<int>();
                                for (int f = 0; f < mesh.IndexCount; f++)
                                {
                                    if (mesh.IndexFormat == 0)
                                        polyGroup.faces.Add(reader.ReadUInt16());
                                    else
                                        polyGroup.faces.Add(reader.ReadByte());
                                }

                                if (mesh.FaceType == PO_Mesh.PolygonType.TriangleStrips)
                                    polyGroup.PrimativeType = STPrimativeType.TrangleStrips;
                                else
                                    polyGroup.PrimativeType = STPrimativeType.Triangles;


                                for (int a = 0; a < mesh.NumAttributePointers; a++)
                                    Console.WriteLine($"pointer {genericMesh.Text} { modelFile.vertexBufferOffset + modelFile.attributes[pointerIndex + a].Offset}");

                                for (int v = 0; v < mesh.VertexCount; v++)
                                {
                                    Vertex vert = new Vertex();
                                    genericMesh.vertices.Add(vert);

                                    int attributeIndex = 0;
                                    for (int a = 0; a < mesh.NumAttributePointers; a++)
                                    {
                                        var pointer = modelFile.attributes[pointerIndex + a];
                                        reader.SeekBegin(modelFile.vertexBufferOffset + pointer.Offset + (pointer.Stride * v));

                                        if (attributeIndex == 0)
                                        {
                                            if (pointer.Stride == 12)
                                                vert.pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                                        }
                                        if (attributeIndex == 1)
                                        {
                                            if (pointer.Stride == 12)
                                                vert.nrm = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                                        }
                                        if (attributeIndex == 2)
                                        {
                                            if (pointer.Stride == 4)
                                                vert.uv0 = new Vector2(reader.ReadUInt16() / 1024f, reader.ReadUInt16() / 1024f);
                                        }

                                        /*     if (pointer.Type == 0xD4)
                                             {
                                                 vert.boneIds = new List<int>() { reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() };
                                             }
                                             if (pointer.Type == 0xB0)
                                             {
                                                 vert.boneWeights = new List<float>() { reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() };
                                             }*/

                                        attributeIndex++;
                                    }
                                }

                                genericMesh.TransformPosition(new Vector3(0), new Vector3(-90, 0, 0), new Vector3(1));

                                pointerIndex += mesh.NumAttributePointers;
                            }
                        }
                    }
                }
            }
        }

        public void Save(System.IO.Stream stream)
        {

        }

        public void Unload()
        {

        }


        public class ChunkViewer : TreeNodeCustom, IContextMenuNode
        {
            public System.IO.Stream FileData;

            public ChunkViewer(string text) {
                Text = text;
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

                if (sfd.ShowDialog() == DialogResult.OK) {
                    FileData.ExportToFile(sfd.FileName);
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
                editor.LoadData(FileData);
            }
        }

        public class ModelFileData
        {
            public uint MaterialOffset;

            public List<PO_Mesh> meshes = new List<PO_Mesh>();
            public List<PO_Model> models = new List<PO_Model>();
            public List<PO_VertexAttribute> attributes = new List<PO_VertexAttribute>();

            public uint vertexBufferOffset = 0;
            public uint indexBufferOffset = 0;
        }

        public class DictionaryFile
        {
            public List<BlockInfo> Blocks = new List<BlockInfo>();

            public List<ChunkDataInfo> DataChunks = new List<ChunkDataInfo>();

            public bool IsCompressed;

            public uint NumFileEntries;

            public uint FileTableOffset;
            public uint FileTableSize;

            public void Read(FileReader reader, string filePath)
            {
                reader.SetByteOrder(true);

                uint magic = reader.ReadUInt32();
                ushort Unknown = reader.ReadUInt16(); //Could also be 2 bytes, not sure. Always 0x0601
                IsCompressed = reader.ReadByte() == 1;
                reader.ReadByte(); //Padding
                reader.ReadUInt32(); //1
                reader.ReadUInt32(); //0
                NumFileEntries = reader.ReadUInt32();
                FileTableSize = reader.ReadUInt32();

                //Loop through all possible blocks which makeup the data file. 
                //They don't have a counter for these but it's always 8 total.
                //If there's 0 in place for size, data is unused
                uint offset = 0;
                for (int i = 0; i < 8; i++)
                {
                    BlockInfo block = new BlockInfo();

                    block.Offset = offset;
                    block.Size  = reader.ReadUInt32();
                    block.Parameter = reader.ReadUInt32(); //4 or 32

                    Blocks.Add(block);

                    offset += block.Size;
                }

                //Now use the last offset for the file table in .data files
                //The file table also is located in .debug files, however those are
                // not necessary and likely left over for debug purposes
                FileTableOffset = offset;

                string DataFile = $"{filePath.Replace(".dict", ".data")}";
                if (System.IO.File.Exists(DataFile))
                    ParseData(DataFile);
            }

            private void ParseData(string fileName)
            {
                using (var reader = new FileReader(fileName))
                {
                    reader.SetByteOrder(true);

                    //Now for the chunk table. It basically has 2 sections.
                    //One is for files
                    //The second contains the data

                    List<ChunkFileInfo> fileChunks = new List<ChunkFileInfo>();

                    reader.SeekBegin(FileTableOffset);
                    for (int i = 0; i < NumFileEntries; i++)
                    {
                        ChunkDataInfo chunk = new ChunkDataInfo();
                        DataChunks.Add(chunk);

                        byte chunkFlags = reader.ReadByte();
                        byte unk = reader.ReadByte(); //1
                        chunk.Type = (SectionMagic)reader.ReadUInt16();
                        chunk.Size = reader.ReadUInt32();
                        chunk.Offset = reader.ReadUInt32();

                        if (chunkFlags == 0x12)
                            chunk.BlockIndex = 0;
                        if (chunkFlags == 0x25)
                            chunk.BlockIndex = 1;
                        if (chunkFlags == 2)
                            chunk.BlockIndex = 2;
                        if (chunkFlags == 0x42)
                            chunk.BlockIndex = 3;
                        if (chunkFlags == 3)
                            chunk.BlockIndex = 0;
                    }

                    while (reader.Position <= reader.BaseStream.Length - 12)
                    {
                        ChunkDataInfo chunk = new ChunkDataInfo();
                        DataChunks.Add(chunk);

                        byte chunkFlags = reader.ReadByte();
                        byte unk = reader.ReadByte(); //1
                        chunk.Type = (SectionMagic)reader.ReadUInt16();
                        chunk.Size = reader.ReadUInt32();
                        chunk.Offset = reader.ReadUInt32();

                        byte blockFlag = (byte)(chunkFlags >> 4);
                        if (blockFlag < 7)
                            chunk.BlockIndex = (sbyte)blockFlag;

                    /*    Console.WriteLine($"chunkFlags { chunkFlags }");
                        Console.WriteLine($"unk { unk }");
                        Console.WriteLine($"Type { chunk.Type }");
                        Console.WriteLine($"Offset { chunk.Offset }");
                        Console.WriteLine($"Size { chunk.Size }");*/
                    }

                    return;
                }
            }
        }


        public enum SectionMagic : ushort
        {
            TextureHeaders = 0xB601,
            TextureData = 0xB603,
            MaterialData = 0xB016,
            IndexData = 0xB007,
            VertexData = 0xB006,
            VertexAttributePointerData = 0xB005,
            MeshData = 0xB004,
            ModelData = 0xB003,
            MatrixData = 0xB002,
            SkeletonData = 0xB008,
            BoneHashes = 0xB00B,
            BoneData = 0xB00A,
            UnknownHashList = 0xB00C,
        }

        public class BlockInfo
        {
            public uint Offset;
            public uint Size;
            public uint Parameter;
        }

        public class ChunkFileInfo
        {
            public uint Type;
            public uint NumberEntries;
            public uint NumberSubEntries;
        }

        public class ChunkDataInfo
        {
            public sbyte BlockIndex = -1;
            public SectionMagic Type;
            public uint Offset;
            public uint Size;
        }
    }
}
