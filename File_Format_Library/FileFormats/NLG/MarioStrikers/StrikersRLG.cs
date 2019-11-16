using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Rendering;
using OpenTK;
using Toolbox.Library.Forms;
using System.Windows.Forms;

namespace FirstPlugin.NLG
{
    public class StrikersRLG : TreeNodeFile, IFileFormat, IContextMenuNode
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }

        public string[] Description { get; set; } = new string[] { "Strikers Model" };
        public string[] Extension { get; set; } = new string[] { "*.glg", "*.rlg" };
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            return Utils.GetExtension(FileName) == ".rlg" || Utils.GetExtension(FileName) == ".glg";

            using (var reader = new FileReader(stream, true))
            {
                return reader.CheckSignature(4, 0x8001B000);
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

        Viewport viewport
        {
            get
            {
                var editor = LibraryGUI.GetObjectEditor();
                return editor.GetViewport();
            }
            set
            {
                var editor = LibraryGUI.GetObjectEditor();
                editor.LoadViewport(value);
            }
        }

        private bool DrawablesLoaded;
        public override void OnClick(TreeView treeView)
        {
            Renderer.UpdateVertexData();

            if (Runtime.UseOpenGL)
            {
                if (viewport == null)
                {
                    viewport = new Viewport(ObjectEditor.GetDrawableContainers());
                    viewport.Dock = DockStyle.Fill;
                }
                 
                if (!DrawablesLoaded)
                {
                    ObjectEditor.AddContainer(DrawableContainer);
                    DrawablesLoaded = true;
                }

                Viewport editor = (Viewport)LibraryGUI.GetActiveContent(typeof(Viewport));
                if (editor == null)
                {
                    editor = viewport;
                    LibraryGUI.LoadEditor(viewport);
                }

                viewport.ReloadDrawables(DrawableContainer);
                viewport.Text = Text;
            }
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Export", null, ExportModelAction, Keys.Control | Keys.E));
            return Items.ToArray();
        }

        private void ExportModelAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Supported Formats|*.dae;";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ExportModelSettings exportDlg = new ExportModelSettings();
                if (exportDlg.ShowDialog() == DialogResult.OK)
                    ExportModel(sfd.FileName, exportDlg.Settings);
            }
        }

        public void ExportModel(string fileName, DAE.ExportSettings settings)
        {
            List<STGenericMaterial> Materials = new List<STGenericMaterial>();
            foreach (var mesh in Renderer.Meshes)
                if (mesh.GetMaterial() != null)
                    Materials.Add(mesh.GetMaterial());

            var textures = new List<STGenericTexture>();
            foreach (var tex in PluginRuntime.stikersTextures)
                textures.Add(tex);

            var model = new STGenericModel();
            model.Materials = Materials;
            model.Objects = Renderer.Meshes;

            DAE.Export(fileName, settings, model, textures);
        }

        public enum SectionMagic : uint
        {
            MaterialData = 0x0001B016,
            IndexData = 0x0001B007,
            VertexData = 0x0001B006,
            VertexAttributePointerData = 0x0001B005,
            MeshData = 0x0001B004,
            ModelData = 0x0001B003,
            MatrixData = 0x0001B002,
            SkeletonData = 0x8001B008,
            BoneHashes = 0x0001B00B,
            BoneData = 0x0001B00A,
            UnknownHashList = 0x0001B00C,
        }

        public enum MaterailPresets : uint
        {
            EnvDiffuseDamage = 0x1ACE1D01,
        }

        private Strikers_Renderer Renderer;
        public DrawableContainer DrawableContainer = new DrawableContainer();

        public List<Model> Models = new List<Model>();

        public bool IsGamecube = false;

        public void Load(System.IO.Stream stream)
        {
            Renderer = new Strikers_Renderer();
            DrawableContainer.Name = FileName;
            DrawableContainer.Drawables.Add(Renderer);

            IsGamecube = Utils.GetExtension(FileName) == ".glg";

            Text = FileName;

            using (var reader = new FileReader(stream))
            {
                reader.SetByteOrder(true);
                reader.ReadUInt32(); //magic
                reader.ReadUInt32(); //fileSize

                //Create a list of all the major sections 
                //The sections are not in order, so we must order them while parsing
                Dictionary<SectionMagic, SectionHeader> SectionLookup = new Dictionary<SectionMagic, SectionHeader>();

                while (!reader.EndOfStream) {
                    uint magic = reader.ReadUInt32();
                    uint sectionSize = reader.ReadUInt32();
                        
                    long pos = reader.Position;
                    Console.WriteLine($"Magic {(SectionMagic)magic} sectionSize {sectionSize}");

                    if (!SectionLookup.ContainsKey((SectionMagic)magic))
                        SectionLookup.Add((SectionMagic)magic, new SectionHeader(sectionSize, pos));

                    //This section will skip sub sections so don't do that
                    if (magic != 0x8001B000)
                        reader.SeekBegin(pos + sectionSize);

                    if (!IsGamecube)
                        reader.Align(4);
                }

                var HashList = NLG_Common.HashNames;

                //Now read our model properly
                //First get our model info

                Matrix4 TransformMatrix = Matrix4.Identity;

                uint totalNumMeshes = 0;

                uint modelSize = 12;
                if (IsGamecube)
                    modelSize = 16;
                if (SectionLookup.ContainsKey(SectionMagic.ModelData))
                {
                    var modelHeader = SectionLookup[SectionMagic.ModelData];
                    for (int m = 0; m < modelHeader.Size / modelSize; m++)
                    {
                        reader.SeekBegin(modelHeader.Position + (m * modelSize));
                        if (IsGamecube)
                        {
                            totalNumMeshes += reader.ReadUInt32();
                            reader.ReadUInt32();
                        }
                        else
                        {
                            reader.ReadUInt32();
                            totalNumMeshes += reader.ReadUInt32();
                        }
                    }
                }

                uint meshIndex = 0;
                if (SectionLookup.ContainsKey(SectionMagic.ModelData))
                {
                    var modelHeader = SectionLookup[SectionMagic.ModelData];
                    reader.SeekBegin(modelHeader.Position);
                    uint numModels = modelHeader.Size / modelSize;
                    uint hashId = 0;

                    for (int m = 0; m < numModels; m++)
                    {
                        Model model = new Model();
                        Nodes.Add(model);
                        Models.Add(model);

                        uint numMeshes = 0;
                        reader.SeekBegin(modelHeader.Position + (m * modelSize));
                        if (IsGamecube)
                        {
                            numMeshes = reader.ReadUInt32();
                            hashId = reader.ReadUInt32();
                        }
                        else
                        {
                            hashId = reader.ReadUInt32();
                            numMeshes = reader.ReadUInt32();
                        }

                        model.Text = hashId.ToString("X");
                        if (HashList.ContainsKey(hashId))
                            model.Text = HashList[hashId];



                        if (SectionLookup.ContainsKey(SectionMagic.MeshData))
                        {
                            var meshHeader = SectionLookup[SectionMagic.MeshData];
                            reader.SeekBegin(meshHeader.Position);
                            for (int i = 0; i < numMeshes; i++)
                            {
                                if (IsGamecube)
                                {
                                    uint meshSize = meshHeader.Size / totalNumMeshes;
                                    reader.SeekBegin(meshHeader.Position + ((meshIndex + i) * meshSize));
                                }
                                else
                                    reader.SeekBegin(meshHeader.Position + ((meshIndex + i) * 48));

                                var mesh = new MeshData(reader, IsGamecube);
                                model.Meshes.Add(mesh);
                            }

                            meshIndex += numMeshes;
                        }
                    }

                    if (SectionLookup.ContainsKey(SectionMagic.MatrixData))
                    {
                        var matSection = SectionLookup[SectionMagic.MatrixData];
                        reader.SeekBegin(matSection.Position);
                        TransformMatrix = reader.ReadMatrix4(true);
                    }

                    List<VertexAttribute> Pointers = new List<VertexAttribute>();
                    if (SectionLookup.ContainsKey(SectionMagic.VertexAttributePointerData))
                    {
                        var pointerSection = SectionLookup[SectionMagic.VertexAttributePointerData];
                        reader.SeekBegin(pointerSection.Position);
                        int attributeSize = 8;
                        if (IsGamecube)
                            attributeSize = 6;

                        for (int i = 0; i < pointerSection.Size / attributeSize; i++)
                        {
                            VertexAttribute pointer = new VertexAttribute();
                            if (IsGamecube)
                            {
                                pointer.Offset = reader.ReadUInt32();
                                pointer.Type = reader.ReadByte();
                                pointer.Stride = reader.ReadByte();
                            }
                            else
                            {
                                pointer.Offset = reader.ReadUInt32();
                                pointer.Type = reader.ReadByte();
                                pointer.Stride = reader.ReadByte();
                                reader.ReadUInt16();
                            }

                            Pointers.Add(pointer);
                        }
                    }

                    int pointerIndex = 0;
                    foreach (var model in Models)
                    {
                        for (int i = 0; i < model.Meshes.Count; i++)
                        {
                            RenderableMeshWrapper mesh = new RenderableMeshWrapper();
                            model.Nodes.Add(mesh);
                            Renderer.Meshes.Add(mesh);

                            mesh.Text = model.Meshes[i].HashID.ToString("X");
                            if (HashList.ContainsKey(model.Meshes[i].HashID))
                                mesh.Text = HashList[model.Meshes[i].HashID];

                            string material = model.Meshes[i].MaterialHashID.ToString("X");
                            if (HashList.ContainsKey(model.Meshes[i].MaterialHashID))
                                material = HashList[model.Meshes[i].MaterialHashID];
                            mesh.Nodes.Add(material);

                            var faceSecton = SectionLookup[SectionMagic.IndexData];
                            var vertexSecton = SectionLookup[SectionMagic.VertexData];

                            STGenericPolygonGroup polyGroup = new STGenericPolygonGroup();
                            mesh.PolygonGroups.Add(polyGroup);
                            reader.SeekBegin(faceSecton.Position + model.Meshes[i].IndexStartOffset);


                            List<int> faces = new List<int>();
                            for (int f = 0; f < model.Meshes[i].IndexCount; f++)
                            {
                                if (model.Meshes[i].IndexFormat == 0)
                                    polyGroup.faces.Add(reader.ReadUInt16());
                                else
                                    polyGroup.faces.Add(reader.ReadByte());
                            }

                            if (model.Meshes[i].FaceType == MeshData.PolygonType.TriangleStrips)
                                polyGroup.PrimativeType = STPrimativeType.TrangleStrips;
                            else
                                polyGroup.PrimativeType = STPrimativeType.Triangles;

                            if (IsGamecube)
                            {
                                uint size = Pointers[pointerIndex + 1].Offset - Pointers[pointerIndex].Offset;
                                model.Meshes[i].VertexCount = (ushort)(size / Pointers[pointerIndex].Stride);
                            }

                            Console.WriteLine($"mesh {mesh.Text} {model.Meshes[i].VertexCount}");

                            for (int v = 0; v < model.Meshes[i].VertexCount; v++)
                            {
                                Vertex vert = new Vertex();
                                mesh.vertices.Add(vert);

                                for (int a = 0; a < model.Meshes[i].NumAttributePointers; a++)
                                {
                                    var pointer = Pointers[pointerIndex + a];
                                    reader.SeekBegin(vertexSecton.Position + pointer.Offset + (pointer.Stride * v));

                                    if (pointer.Type == 0)
                                    {
                                        if (pointer.Stride == 6)
                                            vert.pos = new Vector3(reader.ReadInt16() / 1024f, reader.ReadInt16() / 1024f, reader.ReadInt16() / 1024f);
                                        else if (pointer.Stride == 12)
                                            vert.pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                                 //       vert.pos = Vector3.TransformPosition(vert.pos, TransformMatrix);
                                    }

                                    if (pointer.Type == 1)
                                    {
                                        if (pointer.Stride == 3)
                                            vert.nrm = Read_8_8_8_Snorm(reader).Normalized();
                                        else if (pointer.Stride == 12)
                                            vert.nrm = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                                    }
                                    if (pointer.Type == 3)
                                    {
                                        vert.uv0 = new Vector2(reader.ReadUInt16() / 1024f, reader.ReadUInt16() / 1024f);
                                    }


                                    if (pointer.Type == 0x67)
                                    {
                                        vert.pos = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                                    }
                                    if (pointer.Type == 0xFE)
                                    {
                                        vert.nrm = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                                    }
                                    if (pointer.Type == 0x26)
                                    {
                                        vert.uv0 = new Vector2(reader.ReadUInt16() / 1024f, reader.ReadUInt16() / 1024f);
                                    }
                                    if (pointer.Type == 0xCC)
                                    {
                                        vert.uv0 = new Vector2(reader.ReadUInt16() / 1024f, reader.ReadUInt16() / 1024f);
                                    }
                                    if (pointer.Type == 0x17)
                                    {
                                        vert.uv1 = new Vector2(reader.ReadUInt16() / 1024f, reader.ReadUInt16() / 1024f);
                                    }
                                    if (pointer.Type == 0xD4)
                                    {
                                        vert.boneIds = new List<int>() { reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte() };
                                    }
                                    if (pointer.Type == 0xB0)
                                    {
                                        vert.boneWeights = new List<float>() { reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle() };
                                    }
                                }
                            }

                            mesh.TransformPosition(new Vector3(0), new Vector3(-90, 0, 0), new Vector3(1));

                            pointerIndex += model.Meshes[i].NumAttributePointers;
                        }


                        for (int i = 0; i < model.Meshes.Count; i++)
                        {
                            if (IsGamecube)
                            {
                                var renderedMesh = (RenderableMeshWrapper)Renderer.Meshes[i];
                                renderedMesh.Material = new STGenericMaterial();

                                string diffuseName = model.Meshes[i].TexturHashID.ToString("X");
                                if (HashList.ContainsKey(model.Meshes[i].TexturHashID))
                                    diffuseName = HashList[model.Meshes[i].TexturHashID];

                                var texUnit = 1;
                                renderedMesh.Material.TextureMaps.Add(new STGenericMatTexture()
                                {
                                    textureUnit = texUnit++,
                                    Type = STGenericMatTexture.TextureType.Diffuse,
                                    Name = diffuseName,
                                });
                            }

                            if (SectionLookup.ContainsKey(SectionMagic.MaterialData))
                            {
                                var materialSecton = SectionLookup[SectionMagic.MaterialData];
                                reader.SeekBegin(materialSecton.Position + model.Meshes[i].MaterialOffset);

                                var renderedMesh = (RenderableMeshWrapper)Renderer.Meshes[i];
                                renderedMesh.Material = new STGenericMaterial();

                                switch (model.Meshes[i].MaterailPreset)
                                {
                                    case MaterailPresets.EnvDiffuseDamage:
                                        {
                                            uint diffuseMapHashID = reader.ReadUInt32();
                                            uint diffuseMapParam = reader.ReadUInt32();

                                            string diffuseName = diffuseMapHashID.ToString("X");
                                            if (HashList.ContainsKey(diffuseMapHashID))
                                                diffuseName = HashList[diffuseMapHashID];

                                            var texUnit = 1;
                                            renderedMesh.Material.TextureMaps.Add(new STGenericMatTexture()
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

                                            var texUnit = 1;
                                            renderedMesh.Material.TextureMaps.Add(new STGenericMatTexture()
                                            {
                                                textureUnit = texUnit++,
                                                Type = STGenericMatTexture.TextureType.Diffuse,
                                                Name = diffuseName,
                                            });
                                        }
                                        break;
                                }
                            }
                        }
                    }


                    List<BoneListEntry> BoneLists = new List<BoneListEntry>();

                    List<uint> boneHashOrder = new List<uint>();

                    TreeNode parentBoneList = new TreeNode("Bone List");
                    Nodes.Add(parentBoneList);

                    if (SectionLookup.ContainsKey(SectionMagic.SkeletonData))
                    {
                        var skeletonSection = SectionLookup[SectionMagic.SkeletonData];
                        reader.SeekBegin(skeletonSection.Position);

                        //Read all sub sections
                        while (reader.Position < skeletonSection.Position + skeletonSection.Size)
                        {
                            uint magic = reader.ReadUInt32();
                            uint sectionSize = reader.ReadUInt32();

                            long pos = reader.Position;
                            BoneListEntry entry = new BoneListEntry();
                            BoneLists.Add(entry);

                            //Bone hashes appear for each mesh if it uses rigging
                            //Meshes index these lists for rigging
                            if ((SectionMagic)magic == SectionMagic.BoneHashes)
                            {
                                TreeNode boneListNode = new TreeNode("Mesh Bone List");
                                parentBoneList.Nodes.Add(boneListNode);

                                uint numHashes = sectionSize / 4;
                                for (int i = 0; i < numHashes; i++)
                                {
                                    entry.Hashes.Add(reader.ReadUInt32());
                                    if (IsGamecube)
                                        reader.ReadUInt32();

                                    string hashName = entry.Hashes[i].ToString("X");
                                    if (HashList.ContainsKey(entry.Hashes[i]))
                                        hashName = HashList[entry.Hashes[i]];

                                    boneListNode.Nodes.Add(hashName);
                                }
                            }
                            if ((SectionMagic)magic == SectionMagic.BoneData)
                            {
                                uint numBones = sectionSize / 68;
                                for (int i = 0; i < numBones; i++)
                                {
                                    reader.SeekBegin(pos + i * 68);
                                    BoneEntry bone = new BoneEntry();
                                    bone.HashID = reader.ReadUInt32();
                                    reader.ReadUInt32(); //unk
                                    reader.ReadUInt32(); //unk
                                    reader.ReadUInt32(); //unk
                                    reader.ReadSingle(); //0
                                    bone.Scale = new Vector3(
                                       reader.ReadSingle(),
                                       reader.ReadSingle(),
                                       reader.ReadSingle());
                                    reader.ReadSingle(); //0
                                    bone.Rotate = new Vector3(
                                       reader.ReadSingle(),
                                       reader.ReadSingle(),
                                       reader.ReadSingle());
                                    reader.ReadSingle(); //0
                                    bone.Translate = new Vector3(
                                        reader.ReadSingle(),
                                        reader.ReadSingle(),
                                        reader.ReadSingle());
                                    reader.ReadSingle(); //1

                              //      bone.Translate = Vector3.TransformPosition(bone.Translate, TransformMatrix);

                                    entry.Bones.Add(bone);
                                }
                            }

                            reader.SeekBegin(pos + sectionSize);
                        }
                    }

                    List<BoneEntry> BoneListSorted = new List<BoneEntry>();
                    foreach (var hash in boneHashOrder)
                    {
                        foreach (var boneList in BoneLists)
                        {
                            foreach (var bone in boneList.Bones)
                                if (bone.HashID == hash)
                                    BoneListSorted.Add(bone);
                        }
                    }

                    STSkeleton skeleton = new STSkeleton();
                    DrawableContainer.Drawables.Add(skeleton);

                    foreach (var boneList in BoneLists)
                    {
                        foreach (var bone in boneList.Bones)
                        {
                            STBone stBone = new STBone(skeleton);
                            skeleton.bones.Add(stBone);
                            stBone.Text = bone.HashID.ToString("X");
                            if (HashList.ContainsKey(bone.HashID))
                                stBone.Text = HashList[bone.HashID];

                            stBone.position = new float[3] { bone.Translate.X, bone.Translate.Z, -bone.Translate.Y };
                            stBone.rotation = new float[4] { bone.Rotate.X, bone.Rotate.Z, -bone.Rotate.Y, 1 };

                            //   stBone.scale = new float[3] { bone.Scale.X, bone.Scale.Y, bone.Scale.Z };
                            stBone.scale = new float[3] { 0.2f, 0.2f, 0.2f };

                            stBone.RotationType = STBone.BoneRotationType.Euler;
                        }
                    }


                    skeleton.reset();
                    skeleton.update();

                    TreeNode skeletonNode = new TreeNode("Skeleton");
                    Nodes.Add(skeletonNode);
                    foreach (var bone in skeleton.bones)
                    {
                        if (bone.Parent == null)
                            skeletonNode.Nodes.Add(bone);
                    }
                }
            }
        }

        public class Model : STGenericModel
        {
            public List<MeshData> Meshes = new List<MeshData>();
        }

        public static Vector3 Read_8_8_8_Snorm(FileReader reader)
        {
            return new Vector3(reader.ReadSByte() / 255f, reader.ReadSByte() / 255f, reader.ReadSByte() / 255f);
        }

        public class BoneListEntry
        {
            public List<uint> Hashes = new List<uint>();
            public List<BoneEntry> Bones = new List<BoneEntry>();
        }

        public class BoneEntry
        {
            public uint HashID;

            public int ParentIndex = -1;

            public Vector3 Scale;
            public Vector3 Rotate;
            public Vector3 Translate;
        }

        public class RenderableMeshWrapper : GenericRenderedObject
        {
            public STGenericMaterial Material = new STGenericMaterial();

            public override STGenericMaterial GetMaterial()
            {
                return Material;
            }
        }

        public class VertexAttribute
        {
            public uint Offset;
            public byte Stride;
            public byte Type;
            public byte Format;
        }

        public class MeshData
        {
            public uint[] BoneHashes;

            public enum PolygonType : byte
            {
                Triangles = 0,
                TriangleStrips = 1,
            }

            public uint IndexStartOffset;
            public ushort IndexFormat;
            public ushort IndexCount;
            public ushort VertexCount;
            public byte Unknown;
            public byte NumAttributePointers;

            public uint HashID;
            public uint MaterialHashID;

            //Only on gamecube (wii uses seperate section)
            public uint TexturHashID;

            public uint MaterialOffset;

            public MaterailPresets MaterailPreset;

            public PolygonType FaceType = PolygonType.TriangleStrips;

            public MeshData(FileReader reader, bool isGamecube)
            {
                if (isGamecube)
                {
                    reader.ReadUInt16(); //0
                    IndexFormat = reader.ReadUInt16();
                    IndexStartOffset = reader.ReadUInt32();
                    IndexCount = reader.ReadUInt16();
                    FaceType = reader.ReadEnum<PolygonType>(false);
                    NumAttributePointers = reader.ReadByte();
                    reader.ReadUInt32();
                    MaterialHashID = reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    MaterialOffset = reader.ReadUInt32();
                    reader.ReadUInt32();
                    TexturHashID = reader.ReadUInt32();
                    reader.ReadUInt32();
                }
                else
                {
                    IndexStartOffset = reader.ReadUInt32();
                    IndexFormat = reader.ReadUInt16();
                    IndexCount = reader.ReadUInt16();
                    VertexCount = reader.ReadUInt16();
                    Unknown = reader.ReadByte();
                    NumAttributePointers = reader.ReadByte();
                    reader.ReadUInt32();
                    MaterialHashID = reader.ReadUInt32();
                    HashID = reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    MaterialOffset = reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                    reader.ReadUInt32();
                }

                MaterailPreset = (MaterailPresets)MaterialHashID;
            }
        }

        public class SectionHeader
        {
            public long Position;
            public uint Size;

            public SectionHeader(uint size, long pos)
            {
                Size = size;
                Position = pos;
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }
    }
}
