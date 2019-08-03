using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Rendering;
using Grezzo.CmbEnums;
using OpenTK.Graphics.OpenGL;

namespace FirstPlugin
{
    public class CMB : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "CMB" };
        public string[] Extension { get; set; } = new string[] { "*CTR Model Binary" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "cmb ");
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

        bool DrawablesLoaded = false;
        public override void OnClick(TreeView treeView)
        {
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

                viewport.ReloadDrawables(DrawableContainer);
                LibraryGUI.LoadEditor(viewport);

                viewport.Text = Text;
            }
        }

        public CMB_Renderer Renderer;
        public DrawableContainer DrawableContainer = new DrawableContainer();

        public Header header;
        STTextureFolder texFolder;
        STSkeleton Skeleton;

        public void Load(System.IO.Stream stream)
        {
            Renderer = new CMB_Renderer();
            DrawableContainer.Drawables.Add(Renderer);
            Skeleton = new STSkeleton();
            //These models/skeletons come out massive so scale them with an overridden scale
            Skeleton.PreviewScale = Renderer.PreviewScale;
            Skeleton.BonePointScale = 40;
           
            DrawableContainer.Drawables.Add(Skeleton);

            header = new Header();
            header.Read(new FileReader(stream));

            Text = header.Name;

            DrawableContainer.Name = Text;

            //Load textures
            if (header.SectionData.TextureChunk != null)
            {
                texFolder = new TextureFolder("Texture");
                TreeNode meshFolder = new TreeNode("Meshes");
                TreeNode materialFolder = new TreeNode("Materials");
                TreeNode skeletonFolder = new TreeNode("Skeleton");

                List<STGenericMaterial> materials = new List<STGenericMaterial>();

                bool HasTextures = header.SectionData.TextureChunk != null &&
                     header.SectionData.TextureChunk.Textures.Count != 0;

                bool HasMeshes = header.SectionData.SkeletalMeshChunk != null &&
                     header.SectionData.SkeletalMeshChunk.ShapeChunk.SeperateShapes.Count != 0;

                bool HasSkeleton = header.SectionData.SkeletonChunk != null &&
                    header.SectionData.SkeletonChunk.Bones.Count != 0;

                bool HasMaterials = header.SectionData.MaterialChunk != null &&
                   header.SectionData.MaterialChunk.Materials.Count != 0;

                if (HasSkeleton)
                {
                    foreach (var bone in header.SectionData.SkeletonChunk.Bones)
                    {
                        STBone genericBone = new STBone(Skeleton);
                        genericBone.parentIndex = bone.ParentIndex;
                        genericBone.position = new float[3];
                        genericBone.scale = new float[3];
                        genericBone.rotation = new float[4];
                        genericBone.Checked = true;

                        genericBone.Text = $"Bone {bone.ID}";
                        genericBone.RotationType = STBone.BoneRotationType.Euler;

                        genericBone.position[0] = bone.Translation.X;
                        genericBone.position[1] = bone.Translation.Y;
                        genericBone.position[2] = bone.Translation.Z;

                        genericBone.scale[0] = bone.Scale.X;
                        genericBone.scale[1] = bone.Scale.Y;
                        genericBone.scale[2] = bone.Scale.Z;

                        genericBone.rotation[0] = bone.Rotation.X;
                        genericBone.rotation[1] = bone.Rotation.Y;
                        genericBone.rotation[2] = bone.Rotation.Z;

                        Skeleton.bones.Add(genericBone);
                    }

                    foreach (var bone in Skeleton.bones)
                    {
                        if (bone.Parent == null)
                            skeletonFolder.Nodes.Add(bone);
                    }

                    Skeleton.reset();
                    Skeleton.update();
                }

                if (HasTextures)
                {
                    int texIndex = 0;
                    foreach (var tex in header.SectionData.TextureChunk.Textures)
                    {
                        var texWrapper = new CTXB.TextureWrapper();
                        texWrapper.Text = $"Texture {texIndex++}";
                        texWrapper.ImageKey = "texture";
                        texWrapper.SelectedImageKey = texWrapper.ImageKey;

                        if (tex.Name != string.Empty)
                            texWrapper.Text = tex.Name;

                        texWrapper.Width = tex.Width;
                        texWrapper.Height = tex.Height;
                        texWrapper.Format = CTR_3DS.ConvertPICAToGenericFormat(tex.PicaFormat);
                        texWrapper.ImageData = tex.ImageData;
                        texFolder.Nodes.Add(texWrapper);

                        Renderer.TextureList.Add(texWrapper);
                    }
                }

                if (HasMaterials)
                {
                    int materialIndex = 0;
                    foreach (var mat in header.SectionData.MaterialChunk.Materials)
                    {
                        CMBMaterialWrapper material = new CMBMaterialWrapper(mat);
                        material.Text = $"Material {materialIndex++}";
                        materialFolder.Nodes.Add(material);
                        materials.Add(material);

                        bool HasDiffuse = false;
                        foreach (var tex in mat.TextureMaps)
                        {
                            if (tex.TextureIndex != -1)
                            {
                                CMBTextureMapWrapper matTexture = new CMBTextureMapWrapper(tex);
                                matTexture.TextureIndex = tex.TextureIndex;
                                material.TextureMaps.Add(matTexture);

                                if (tex.TextureIndex < Renderer.TextureList.Count && tex.TextureIndex >= 0)
                                {
                                    matTexture.Name = Renderer.TextureList[tex.TextureIndex].Text;
                                    material.Nodes.Add(matTexture.Name);
                                }

                                if (!HasDiffuse && matTexture.Name != "bg_syadowmap") //Quick hack till i do texture env stuff
                                {
                                    matTexture.Type = STGenericMatTexture.TextureType.Diffuse;
                                    HasDiffuse = true;
                                }
                            }
                        }
                    }
                }

                if (HasMeshes)
                {
                    int MeshIndex = 0;
                    foreach (var mesh in header.SectionData.SkeletalMeshChunk.MeshChunk.Meshes)
                    {
                        STGenericMaterial mat = new STGenericMaterial();
                        if (materials.Count > mesh.MaterialIndex) //Incase materials for some reason are in a seperate file, check this
                            mat = materials[mesh.MaterialIndex];

                        CmbMeshWrapper genericMesh = new CmbMeshWrapper(mat);
                        genericMesh.Text = $"Mesh {MeshIndex++}";
                        genericMesh.MaterialIndex = mesh.MaterialIndex;

                        //Wow this is long
                        var shape = header.SectionData.SkeletalMeshChunk.ShapeChunk.SeperateShapes[(int)mesh.SepdIndex];
                        genericMesh.Shape = shape;

                        //Now load the vertex and face data
                        if (shape.Position.VertexData != null)
                        {
                            int VertexCount = shape.Position.VertexData.Length;
                            for (int v = 0; v < VertexCount; v++)
                            {
                                Vertex vert = new Vertex();
                                vert.pos = new OpenTK.Vector3(
                                    shape.Position.VertexData[v].X,
                                    shape.Position.VertexData[v].Y,
                                    shape.Position.VertexData[v].Z);

                                if (shape.Normal.VertexData != null && shape.Normal.VertexData.Length > v)
                                {
                                    vert.nrm = new OpenTK.Vector3(
                                    shape.Normal.VertexData[v].X,
                                    shape.Normal.VertexData[v].Y,
                                    shape.Normal.VertexData[v].Z).Normalized();
                                }

                                if (shape.Color.VertexData != null && shape.Color.VertexData.Length > v)
                                {
                                    vert.col = new OpenTK.Vector4(
                                    shape.Color.VertexData[v].X,
                                    shape.Color.VertexData[v].Y,
                                    shape.Color.VertexData[v].Z,
                                    shape.Color.VertexData[v].W).Normalized();
                                }

                                if (shape.TexCoord0.VertexData != null && shape.TexCoord0.VertexData.Length > v)
                                {
                                    vert.uv0 = new OpenTK.Vector2(
                                    shape.TexCoord0.VertexData[v].X,
                                    shape.TexCoord0.VertexData[v].Y);
                                }

                                if (shape.TexCoord1.VertexData != null)
                                {
                               
                                }

                                if (shape.TexCoord2.VertexData != null)
                                {
                               
                                }

                                for (int i = 0; i < 16; i++)
                                {
                                    if (i < shape.Primatives[0].BoneIndexTable.Length)
                                    {
                                        int boneId = shape.Primatives[0].BoneIndexTable[i];

                                        if (shape.Primatives[0].SkinningMode == SkinningMode.RIGID_SKINNING)
                                        {
                                            vert.pos = OpenTK.Vector3.TransformPosition(vert.pos, Skeleton.bones[boneId].Transform);
                                            vert.nrm = OpenTK.Vector3.TransformNormal(vert.nrm, Skeleton.bones[boneId].Transform);
                                        }
                                    }
                                }

                                    bool HasSkinning = shape.Primatives[0].SkinningMode != SkinningMode.SINGLE_BONE
                                    && shape.BoneIndices.Type == CmbDataType.UByte; //Noclip checks the type for ubyte so do the same

                                bool HasWeights = shape.Primatives[0].SkinningMode == SkinningMode.SMOOTH_SKINNING;

                           /*    if (shape.BoneIndices.VertexData != null && HasSkinning && shape.BoneIndices.VertexData.Length > v)
                                {
                                    var BoneIndices = shape.BoneIndices.VertexData[v];
                                    for (int j = 0; j < shape.boneDimension; j++)
                                    {
                                        ushort index = shape.Primatives[0].BoneIndexTable[(uint)BoneIndices[j]];
                                        vert.boneIds.Add((int)index);
                                    }
                                }
                                if (shape.BoneWeights.VertexData != null && HasWeights && shape.BoneWeights.VertexData.Length > v)
                                {
                                    var BoneWeights = shape.BoneWeights.VertexData[v];
                                    for (int j = 0; j < shape.boneDimension; j++)
                                    {
                                        vert.boneWeights.Add(BoneWeights[j]);
                                    }
                                }*/

                                genericMesh.vertices.Add(vert);
                            }
                        }

                        foreach (var prim in shape.Primatives)
                        {
                            STGenericPolygonGroup group = new STGenericPolygonGroup();
                            genericMesh.PolygonGroups.Add(group);

                            for (int i = 0; i < prim.Primatives[0].Indices.Length; i++)
                            {
                                group.faces.Add((int)prim.Primatives[0].Indices[i]);
                            }
                        }

                        Renderer.Meshes.Add(genericMesh);
                        meshFolder.Nodes.Add(genericMesh);
                    }
                }
            
                if (meshFolder.Nodes.Count > 0)
                    Nodes.Add(meshFolder);

                if (skeletonFolder.Nodes.Count > 0)
                    Nodes.Add(skeletonFolder);

                if (materialFolder.Nodes.Count > 0)
                    Nodes.Add(materialFolder);

                if (texFolder.Nodes.Count > 0)
                    Nodes.Add(texFolder);

                materials.Clear();
            }
        }

        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }

        public enum CMBVersion
        {
            OOT3DS,
            MM3DS,
            LM3DS,
        }

        public class CMBMaterialWrapper : STGenericMaterial
        {
            public Material Material { get; set; }

            public CMBMaterialWrapper(Material material)
            {
                Material = material;
            }

            public override void OnClick(TreeView treeView)
            {
                STPropertyGrid editor = (STPropertyGrid)LibraryGUI.GetActiveContent(typeof(STPropertyGrid));
                if (editor == null)
                {
                    editor = new STPropertyGrid();
                    LibraryGUI.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.LoadProperty(Material, null);
            }
        }

        public class CMBTextureMapWrapper : STGenericMatTexture
        {
            public int TextureIndex { get; set; }

            public TextureMap TextureMapData;

            public CMBTextureMapWrapper(TextureMap texMap)
            {
                TextureMapData = texMap;

                //Linear filtering looks better according to noclip
                if (TextureMapData.MinFiler == TextureFilter.LINEAR_MIPMAP_NEAREST)
                    TextureMapData.MinFiler = TextureFilter.LINEAR_MIPMAP_LINEAR;

                 this.WrapModeS = ConvertWrapMode(TextureMapData.WrapS);
                 this.WrapModeT = ConvertWrapMode(TextureMapData.WrapT);
                 this.MinFilter = ConvertMinFilterMode(TextureMapData.MinFiler);
                 this.MagFilter = ConvertMagFilterMode(TextureMapData.MagFiler);
            }

            private STTextureMinFilter ConvertMinFilterMode(TextureFilter PicaFilterMode)
            {
                switch (PicaFilterMode)
                {
                    case TextureFilter.LINEAR: return STTextureMinFilter.Linear;
                    case TextureFilter.LINEAR_MIPMAP_LINEAR: return  STTextureMinFilter.LinearMipMapNearest;
                    case TextureFilter.LINEAR_MIPMAP_NEAREST: return STTextureMinFilter.NearestMipmapLinear;
                    case TextureFilter.NEAREST: return STTextureMinFilter.Nearest;
                    case TextureFilter.NEAREST_MIPMAP_LINEAR: return STTextureMinFilter.NearestMipmapLinear;
                    case TextureFilter.NEAREST_MIPMAP_NEAREST: return STTextureMinFilter.NearestMipmapNearest;
                    default: return 0;
                }
            }

            private STTextureMagFilter ConvertMagFilterMode(TextureFilter PicaFilterMode)
            {
                switch (PicaFilterMode)
                {
                    case TextureFilter.LINEAR: return STTextureMagFilter.Linear;
                    case TextureFilter.NEAREST: return STTextureMagFilter.Nearest;
                    default: return 0;
                }
            }

            private STTextureWrapMode ConvertWrapMode(CMBTextureWrapMode PicaWrapMode)
            {
                switch (PicaWrapMode)
                {
                    case CMBTextureWrapMode.REPEAT: return STTextureWrapMode.Repeat;
                    case CMBTextureWrapMode.MIRRORED_REPEAT: return STTextureWrapMode.Mirror;
                    case CMBTextureWrapMode.CLAMP: return STTextureWrapMode.Clamp;
                    case CMBTextureWrapMode.CLAMP_TO_EDGE: return STTextureWrapMode.Clamp;
                    default: return STTextureWrapMode.Repeat;
                }
            }
        }

        public class CmbMeshWrapper : GenericRenderedObject
        {
            public SeperateShape Shape { get; set; }

            STGenericMaterial material;

            public CmbMeshWrapper(STGenericMaterial mat) {
                material = mat;
            }

            public override STGenericMaterial GetMaterial()
            {
                return material;
            }
        }

        private class TextureFolder : STTextureFolder, ITextureIconLoader
        {
            public List<STGenericTexture> IconTextureList
            {
                get
                {
                    List<STGenericTexture> textures = new List<STGenericTexture>();
                    foreach (STGenericTexture node in Nodes)
                        textures.Add(node);

                    return textures;
                }
                set {}
            }

            public TextureFolder(string text) : base(text)
            {

            }
        }

        public class Header
        {
            public string Name { get; set; }

            public CMBVersion Version;

            public uint ChunkCount; //Fixed count per game

            public uint Unknown;

            public SectionData SectionData;

            public void Read(FileReader reader)
            {
                string magic = reader.ReadSignature(4, "cmb ");
                uint FileSize = reader.ReadUInt32();
                ChunkCount = reader.ReadUInt32();
                Unknown = reader.ReadUInt32();

                Name = reader.ReadString(0x10).TrimEnd('\0');

                //Check the chunk count used by the game
                if (ChunkCount == 0x0F)
                    Version = CMBVersion.LM3DS;
                else if (ChunkCount == 0x0A)
                    Version = CMBVersion.MM3DS;
                else if (ChunkCount == 0x06)
                    Version = CMBVersion.OOT3DS;
                else
                    throw new Exception("Unexpected chunk count! " + ChunkCount);

                SectionData = new SectionData();
                SectionData.Read(reader, this);
            }

            public void Write(FileWriter writer)
            {
                writer.WriteSignature("cmb ");
                writer.Write(uint.MaxValue); //Reserve space for file size offset
                writer.Write(ChunkCount);
                writer.Write(Unknown);
                writer.WriteString(Name, 0x10);

                SectionData.Write(writer, this);

                //Write the total file size
                using (writer.TemporarySeek(4, System.IO.SeekOrigin.Begin))
                {
                    writer.Write((uint)writer.BaseStream.Length);
                }
            }
        }

        public class SectionData
        {
            public SkeletonChunk SkeletonChunk;
            public QuadTreeChunk QuadTreeChunk;
            public MaterialChunk MaterialChunk;

            public TextureChunk TextureChunk;
            public SkeletalMeshChunk SkeletalMeshChunk;
            public LUTSChunk LUTSChunk;
            public VertexAttributesChunk VertexAttributesChunk;

            public ushort[] Indices;

            public void Read(FileReader reader, Header header)
            {
                uint numIndices = reader.ReadUInt32();
                SkeletonChunk = ReadChunkSection<SkeletonChunk>(reader, header);
                if (header.Version >= CMBVersion.MM3DS)
                    QuadTreeChunk = ReadChunkSection<QuadTreeChunk>(reader, header);

                MaterialChunk = ReadChunkSection<MaterialChunk>(reader, header);
                TextureChunk = ReadChunkSection<TextureChunk>(reader, header);
                SkeletalMeshChunk = ReadChunkSection<SkeletalMeshChunk>(reader, header);
                LUTSChunk = ReadChunkSection<LUTSChunk>(reader, header);
                VertexAttributesChunk = ReadChunkSection<VertexAttributesChunk>(reader, header);

                uint indexBufferOffset = reader.ReadUInt32();
                uint textureDataOffset = reader.ReadUInt32();

                if (header.Version >= CMBVersion.MM3DS)
                    reader.ReadUInt32(); //Padding?

                if (VertexAttributesChunk != null)
                {
                    long bufferStart = VertexAttributesChunk.StartPosition;
                    foreach (var shape in SkeletalMeshChunk.ShapeChunk.SeperateShapes)
                    {
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.PositionSlice, shape.Position, 3, bufferStart);
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.NormalSlice, shape.Normal, 3, bufferStart);
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.TangentSlice, shape.Tangent, 3, bufferStart);
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.ColorSlice, shape.Color, 4, bufferStart);
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.Texcoord0Slice, shape.TexCoord0, 2, bufferStart);
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.Texcoord1Slice, shape.TexCoord1, 2, bufferStart);
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.Texcoord2Slice, shape.TexCoord2, 2, bufferStart);

                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.BoneIndicesSlice, shape.BoneIndices, shape.boneDimension, bufferStart);
                        ReadVertexDataFromSlice(reader, VertexAttributesChunk.BoneWeightsSlice, shape.BoneWeights, shape.boneDimension, bufferStart);
                    }
                }

                if (indexBufferOffset != 0)
                {
                    foreach (var shape in SkeletalMeshChunk.ShapeChunk.SeperateShapes)
                    {
                        foreach (var prim in shape.Primatives)
                        {
                            foreach (var subprim in prim.Primatives) //Note 3DS usually only has one sub primative
                            {
                                subprim.Indices = new uint[subprim.IndexCount];

                                reader.SeekBegin(indexBufferOffset + subprim.Offset);

                                switch (subprim.IndexType)
                                {
                                    case CmbDataType.UByte:
                                        for (int i = 0; i < subprim.IndexCount; i++)
                                            subprim.Indices[i] = reader.ReadByte();
                                        break;
                                    case CmbDataType.UShort:
                                        for (int i = 0; i < subprim.IndexCount; i++)
                                            subprim.Indices[i] = reader.ReadUInt16();
                                        break;
                                    case CmbDataType.UInt:
                                        for (int i = 0; i < subprim.IndexCount; i++)
                                            subprim.Indices[i] = reader.ReadUInt32();
                                        break;
                                    default:
                                        throw new Exception("Unsupported index type! " + subprim.IndexType);
                                }
                            }
                        }
                    }
                }

                foreach (var tex in TextureChunk.Textures)
                {
                    reader.SeekBegin(textureDataOffset + tex.DataOffset);
                    tex.ImageData = reader.ReadBytes((int)tex.ImageSize);
                }
            }

            private static void ReadVertexDataFromSlice(FileReader reader, BufferSlice Slice, SepdVertexAttribute VertexAttribute, int elementCount, long bufferStart)
            {
                if (Slice == null || Slice.Size == 0)
                    return;

                reader.SeekBegin(bufferStart + VertexAttribute.StartPosition + Slice.Offset);

                int StrideSize = CalculateStrideSize(VertexAttribute.Type, elementCount);
                int VertexCount = (int)Slice.Size / StrideSize;

                Console.WriteLine($"{VertexAttribute.GetType()} {VertexAttribute.Type} {elementCount} {StrideSize} {VertexCount}");

                VertexAttribute.VertexData = new Syroot.Maths.Vector4F[VertexCount];
                for (int v = 0; v < VertexCount; v++)
                {
                    VertexAttribute.VertexData[v] = ReadVertexBufferData(reader, VertexAttribute, elementCount);
                }
            }

            private static Syroot.Maths.Vector4F ReadVertexBufferData(FileReader reader, SepdVertexAttribute VertexAttribute, int elementCount)
            {
                List<float> values = new List<float>();

                for (int i = 0; i < elementCount; i++)
                {
                    switch (VertexAttribute.Type)
                    {
                        case CmbDataType.Byte:
                            values.Add(reader.ReadSByte());
                            break;
                        case CmbDataType.Float:
                            values.Add(reader.ReadSingle());
                            break;
                        case CmbDataType.Int:
                            values.Add(reader.ReadInt32());
                            break;
                        case CmbDataType.Short:
                            values.Add(reader.ReadInt16());
                            break;
                        case CmbDataType.UByte:
                            values.Add(reader.ReadByte());
                            break;
                        case CmbDataType.UInt:
                            values.Add(reader.ReadUInt32());
                            break;
                        case CmbDataType.UShort:
                            values.Add(reader.ReadUInt16());
                            break;
                        default: throw new Exception("Unknwon format! " + VertexAttribute.Type);
                    }
                }

                while (values.Count < 4) values.Add(0);

                return new Syroot.Maths.Vector4F(
                    values[0] * VertexAttribute.Scale,
                    values[1] * VertexAttribute.Scale, 
                    values[2] * VertexAttribute.Scale, 
                    values[3] * VertexAttribute.Scale);
            }

            private static int CalculateStrideSize(CmbDataType type, int elementCount)
            {
                switch (type)
                {
                    case CmbDataType.Byte: return elementCount * sizeof(sbyte); 
                    case CmbDataType.Float: return elementCount * sizeof(float); 
                    case CmbDataType.Int: return elementCount * sizeof(int); 
                    case CmbDataType.Short: return elementCount * sizeof(short);
                    case CmbDataType.UByte: return elementCount * sizeof(byte);
                    case CmbDataType.UInt: return elementCount * sizeof(uint); 
                    case CmbDataType.UShort: return elementCount * sizeof(ushort);
                    default: throw new Exception("Unknwon format! " + type);
                }
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.Write(Indices != null ? Indices.Length : 0);
                //Reserve space for all the offses
                writer.Write(0); //SkeletonChunk
                if (header.Version >= CMBVersion.MM3DS)
                    writer.Write(0); //QuadTreeChunk
                writer.Write(0); //MaterialChunk
                writer.Write(0); //TextureChunk
                writer.Write(0); //SkeletalMeshChunk
                writer.Write(0); //LUTSChunk
                writer.Write(0); //VertexAttributesChunk
                writer.Write(0); //indexBufferOffset
                writer.Write(0); //textureDataOffset

                if (header.Version >= CMBVersion.MM3DS)
                    writer.Write(0); //padding or unknown unused section

                //Write sections and offsets
                int _offsetPos = 4;
                if (SkeletonChunk != null)
                {
                    writer.WriteUint32Offset(pos + _offsetPos);
                    SkeletonChunk.Write(writer, header);
                }

                if (QuadTreeChunk != null && header.Version >= CMBVersion.MM3DS)
                {
                    writer.WriteUint32Offset(pos + (_offsetPos += 4));
                    QuadTreeChunk.Write(writer, header);
                }

                if (MaterialChunk != null)
                {
                    writer.WriteUint32Offset(pos + (_offsetPos += 4));
                    MaterialChunk.Write(writer, header);
                }

                if (TextureChunk != null)
                {
                    writer.WriteUint32Offset(pos + (_offsetPos += 4));
                    TextureChunk.Write(writer, header);
                }

                if (SkeletalMeshChunk != null)
                {
                    writer.WriteUint32Offset(pos + (_offsetPos += 4));
                    SkeletalMeshChunk.Write(writer, header);
                }

                if (LUTSChunk != null)
                {
                    writer.WriteUint32Offset(pos + (_offsetPos += 4));
                    LUTSChunk.Write(writer, header);
                }

                if (VertexAttributesChunk != null)
                {
                    writer.WriteUint32Offset(pos + (_offsetPos += 4));
                    VertexAttributesChunk.Write(writer, header);
                }

                if (Indices != null && Indices.Length > 0)
                {
                    writer.WriteUint32Offset(pos + (_offsetPos += 4));
                    writer.Write(Indices);
                }

                if (TextureChunk != null && TextureChunk.Textures.Count > 0)
                {
                    long dataStart = writer.Position;
                    writer.WriteUint32Offset(pos + (_offsetPos += 4));
                    //Save image data
                    foreach (var tex in TextureChunk.Textures)
                    {
                        writer.SeekBegin(tex.DataOffset + dataStart);
                        writer.Write(tex.ImageData);
                    }
                }
            }
        }

        //Connects all the meshes, vertex attributes, and shape data together
        public class SkeletalMeshChunk : IChunkCommon
        {
            private const string Magic = "sklm";

            public MeshesChunk MeshChunk { get; set; }
            public ShapesChunk ShapeChunk { get; set; }

            public void Read(FileReader reader, Header header)
            {
                long pos = reader.Position;

                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                MeshChunk = ReadChunkSection<MeshesChunk>(reader, header, pos);
                ShapeChunk = ReadChunkSection<ShapesChunk>(reader, header,pos);
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
            }
        }

        public class MeshesChunk : IChunkCommon
        {
            private const string Magic = "mshs";

            public List<Mesh> Meshes = new List<Mesh>();

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint meshCount = reader.ReadUInt32();
                uint unknown = reader.ReadUInt32();

                long meshPos = reader.Position;
                for (int i = 0; i < meshCount; i++)
                {
                    if (header.Version == CMBVersion.OOT3DS)
                        reader.SeekBegin(meshPos + (i * 4));
                    else if (header.Version == CMBVersion.MM3DS)
                        reader.SeekBegin(meshPos + (i * 0x0C));
                    else if (header.Version >= CMBVersion.LM3DS)
                        reader.SeekBegin(meshPos + (i * 0x58));

                    Mesh mesh = new Mesh();
                    mesh.SepdIndex = reader.ReadUInt16();
                    mesh.MaterialIndex = reader.ReadByte();
                    Meshes.Add(mesh);

                    Console.WriteLine($"SepdIndex {mesh.SepdIndex}");
                    Console.WriteLine($"MaterialIndex { mesh.MaterialIndex}");
                }
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                writer.Write(Meshes.Count);
                for (int i = 0; i < Meshes.Count; i++)
                {

                }
            }

            public class Mesh
            {
                public ushort SepdIndex { get; set; }
                public byte MaterialIndex { get; set; }
            }
        }

        public class ShapesChunk : IChunkCommon
        {
            private const string Magic = "shp ";

            public uint Unknown;

            public List<SeperateShape> SeperateShapes = new List<SeperateShape>();

            public void Read(FileReader reader, Header header)
            {
                long pos = reader.Position;

                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint sepdCount = reader.ReadUInt32();
                Unknown = reader.ReadUInt32();
                ushort[] offsets = reader.ReadUInt16s((int)sepdCount);
                for (int i = 0; i < sepdCount; i++)
                {
                    reader.SeekBegin(pos + offsets[i]);
                    var sepd= new SeperateShape();
                    sepd.Read(reader, header);
                    SeperateShapes.Add(sepd);
                }
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize

            }
        }

        public class SeperateShape : IChunkCommon
        {
            private const string Magic = "sepd";

            public SepdVertexAttribute Position { get; set; }
            public SepdVertexAttribute Normal { get; set; }
            public SepdVertexAttribute Tangent { get; set; }
            public SepdVertexAttribute Color { get; set; }
            public SepdVertexAttribute TexCoord0 { get; set; }
            public SepdVertexAttribute TexCoord1 { get; set; }
            public SepdVertexAttribute TexCoord2 { get; set; }
            public SepdVertexAttribute BoneIndices { get; set; }
            public SepdVertexAttribute BoneWeights { get; set; }

            public List<PrimativesChunk> Primatives = new List<PrimativesChunk>();

            public ushort boneDimension;

            public void Read(FileReader reader, Header header)
            {
                long pos = reader.Position;

                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint count = reader.ReadUInt16();

                if (header.Version >= CMBVersion.LM3DS)
                    reader.SeekBegin(pos + 0x3C);
                else
                    reader.SeekBegin(pos + 0x24);

                Console.WriteLine("sepd count " + count);

                Position = ReadVertexAttrib(reader);
                Normal = ReadVertexAttrib(reader);
                if (header.Version >= CMBVersion.MM3DS)
                    Tangent = ReadVertexAttrib(reader);

                Color = ReadVertexAttrib(reader);
                TexCoord0 = ReadVertexAttrib(reader);
                TexCoord1 = ReadVertexAttrib(reader);
                TexCoord2 = ReadVertexAttrib(reader);
                BoneIndices = ReadVertexAttrib(reader);
                BoneWeights = ReadVertexAttrib(reader);

                boneDimension = reader.ReadUInt16();
                reader.ReadUInt16(); //padding

                ushort[] Offsets = reader.ReadUInt16s((int)count);

                for (int i = 0; i < count; i++)
                {
                    reader.SeekBegin(pos + Offsets[i]);
                    PrimativesChunk prim = new PrimativesChunk();
                    prim.Read(reader, header);
                    Primatives.Add(prim);
                }
            }

            public void Write(FileWriter writer, Header header)
            {

            }

            private SepdVertexAttribute ReadVertexAttrib(FileReader reader)
            {
                long pos = reader.Position;

                SepdVertexAttribute att = new SepdVertexAttribute();
                att.StartPosition = reader.ReadUInt32();
                att.Scale = reader.ReadSingle();
                att.Type = reader.ReadEnum<CmbDataType>(true);
                att.Mode = reader.ReadEnum<SepdVertexAttribMode>(true);
                att.Constants = new float[4];
                att.Constants[0] = reader.ReadSingle();
                att.Constants[1] = reader.ReadSingle();
                att.Constants[2] = reader.ReadSingle();
                att.Constants[3] = reader.ReadSingle();

                reader.SeekBegin(pos + 0x1C);

                return att;
            }
        }

        public class SepdVertexAttribute
        {
            public uint StartPosition { get; set; }
            public float Scale { get; set; }
            public CmbDataType Type { get; set; }
            public SepdVertexAttribMode Mode { get; set; }

            public Syroot.Maths.Vector4F[] VertexData { get; set; }

            public float[] Constants { get; set; }
        }

        public class PrimativesChunk : IChunkCommon
        {
            private const string Magic = "prms";

            public SkinningMode SkinningMode;

            public List<SubPrimativeChunk> Primatives = new List<SubPrimativeChunk>();

            public ushort[] BoneIndexTable { get; set; }

            public void Read(FileReader reader, Header header)
            {
                long pos = reader.Position;

                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint count = reader.ReadUInt32();
                SkinningMode = reader.ReadEnum<SkinningMode>(true);
                ushort boneTableCount = reader.ReadUInt16();
                uint boneIndexOffset = reader.ReadUInt32();
                uint primativeOffset = reader.ReadUInt32();

                reader.SeekBegin(pos + boneIndexOffset);
                BoneIndexTable = reader.ReadUInt16s(boneTableCount);

                reader.SeekBegin(pos + primativeOffset);
                for (int i = 0; i < count; i++)
                {
                    SubPrimativeChunk prim = new SubPrimativeChunk();
                    prim.Read(reader, header);
                    Primatives.Add(prim);
                }
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
            }
        }

        public class SubPrimativeChunk : IChunkCommon
        {
            private const string Magic = "prm ";

            public SkinningMode SkinningMode { get; private set; }

            public CmbDataType IndexType { get; private set; }

            public ushort IndexCount { get; private set; }

            public uint Offset { get; private set; }

            private uint[] _indices;
            public uint[] Indices
            {
                get
                {
                    return _indices;
                }
                set
                {
                    _indices = value;
                }
            }

            public void Read(FileReader reader, Header header)
            {
                long pos = reader.Position;

                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint unknown = reader.ReadUInt32();
                uint unknown2 = reader.ReadUInt32();
                IndexType = reader.ReadEnum<CmbDataType>(true);
                reader.Seek(2); //padding

                IndexCount = reader.ReadUInt16();

                //This value is the index, so we'll use it as an offset
                //Despite the data type, this is always * 2
                Offset = (uint)reader.ReadUInt16() * sizeof(ushort); 
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
            }
        }

        public class LUTSChunk : IChunkCommon
        {
            private const string Magic = "luts";

            private byte[] data;

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();

                data = reader.getSection((uint)reader.Position, sectionSize);
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                writer.Write(data);
            }
        }

        public class VertexAttributesChunk : IChunkCommon
        {
            private const string Magic = "vatr";

            public BufferSlice PositionSlice;
            public BufferSlice NormalSlice;
            public BufferSlice TangentSlice; //Used in MM3DS and newer
            public BufferSlice ColorSlice;
            public BufferSlice Texcoord0Slice;
            public BufferSlice Texcoord1Slice;
            public BufferSlice Texcoord2Slice;
            public BufferSlice BoneIndicesSlice;
            public BufferSlice BoneWeightsSlice;

            public long StartPosition;

            public void Read(FileReader reader, Header header)
            {
                StartPosition = reader.Position;

                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint maxIndex = reader.ReadUInt32();

                PositionSlice = ReadSlice(reader);
                NormalSlice = ReadSlice(reader);
                if (header.Version >= CMBVersion.MM3DS)
                    TangentSlice = ReadSlice(reader);

                ColorSlice = ReadSlice(reader);
                Texcoord0Slice = ReadSlice(reader);
                Texcoord1Slice = ReadSlice(reader);
                Texcoord2Slice = ReadSlice(reader);
                BoneIndicesSlice = ReadSlice(reader);
                BoneWeightsSlice = ReadSlice(reader);
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
            }

            private BufferSlice ReadSlice(FileReader reader)
            {
                BufferSlice slice = new BufferSlice();
                slice.Size = reader.ReadUInt32();
                slice.Offset = reader.ReadUInt32();
                return slice;
            }
        }

        public class BufferSlice
        {
            public uint Offset;
            public uint Size;
        }

        public class SkeletonChunk : IChunkCommon
        {
            private const string Magic = "skl ";

            public List<BoneChunk> Bones = new List<BoneChunk>();

            public uint Unknown;

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint boneCount = reader.ReadUInt32();
                Unknown = reader.ReadUInt32();

                for (int i = 0; i < boneCount; i++)
                {
                    BoneChunk bone = new BoneChunk();
                    bone.ID = reader.ReadInt16() & 0xFFFF;
                    bone.ParentIndex = reader.ReadInt16();
                    bone.Scale = reader.ReadVec3SY();
                    bone.Rotation = reader.ReadVec3SY();
                    bone.Translation = reader.ReadVec3SY();
                    if (header.Version >= CMBVersion.MM3DS)
                        bone.Unknown = reader.ReadInt32();

                    Bones.Add(bone);
                }
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                writer.Write(Bones.Count);
                writer.Write(Unknown);
                for (int i = 0; i < Bones.Count; i++)
                {
                    writer.Write(Bones[i].ID);
                    writer.Write(Bones[i].ParentIndex);
                    writer.Write(Bones[i].Scale);
                    writer.Write(Bones[i].Rotation);
                    writer.Write(Bones[i].Translation);
                    if (header.Version >= CMBVersion.MM3DS)
                        writer.Write(Bones[i].Unknown);
                }
            }
        }

        public class BoneChunk
        {
            public int ID { get; set; }
            public int ParentIndex { get; set; }

            public Syroot.Maths.Vector3F Scale { get; set; }
            public Syroot.Maths.Vector3F Rotation { get; set; }
            public Syroot.Maths.Vector3F Translation { get; set; }

            //An unknown value used in versions MM3DS and newer
            public int Unknown { get; set; }
        }

        public class QuadTreeChunk : IChunkCommon
        {
            private const string Magic = "qtrs";

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
            }
        }

        public class MaterialChunk : IChunkCommon
        {
            private const string Magic = "mats";

            public List<Material> Materials = new List<Material>();

            internal int textureCombinerSettingsTableOffs;
            public void Read(FileReader reader, Header header)
            {
                long pos = reader.Position;

                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint count = reader.ReadUInt32();

                int materialSize = 0x15C;
                if (header.Version >= CMBVersion.MM3DS)
                    materialSize = 0x16C;

                Console.WriteLine($"materialSize {materialSize.ToString("x")}");

                textureCombinerSettingsTableOffs = (int)(pos + (count * materialSize));

                for (int i = 0; i < count; i++)
                {
                    reader.SeekBegin(pos + 0xC + (i * materialSize));

                    Material mat = new Material();
                    mat.Read(reader, header, this);
                    Materials.Add(mat);
                }
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
            }
        }

        //Thanks for noclip for material RE stuff
        //https://github.com/magcius/noclip.website/blob/9270b9e5022c691703689990f9c536cd9058e5cd/src/oot3d/cmb.ts#L232
        public class Material
        {
            public bool IsTransparent = false;

            public CullMode CullMode { get; set; }

            public bool IsPolygonOffsetEnabled { get; set; }
            public uint PolygonOffset { get; set; }

            public TextureMap[] TextureMaps { get; set; }
            public TextureMatrix[] TextureMaticies { get; set; }

            public List<TextureCombiner> TextureCombiners { get; set; }

            public STColor[] ConstantColors { get; set; }

            public bool AlphaTestEnable { get; set; }
            public float AlphaTestReference { get; set; }

            public bool DepthTestEnable { get; set; }

            public bool DepthWriteEnable { get; set; }

            public AlphaFunction AlphaTestFunction { get; set; }

            public DepthFunction DepthTestFunction { get; set; }

            public bool BlendEnaled { get; set; }

            public BlendingFactor BlendingFactorSrcRGB { get; set; }
            public BlendingFactor BlendingFactorDestRGB { get; set; }

            public BlendingFactor BlendingFactorSrcAlpha { get; set; }
            public BlendingFactor BlendingFactorDestAlpha { get; set; }

            public float BlendColorR { get; set; }
            public float BlendColorG { get; set; }
            public float BlendColorB { get; set; }
            public float BlendColorA { get; set; }

            public void Read(FileReader reader, Header header, MaterialChunk materialChunkParent)
            {
                TextureMaps = new TextureMap[3];
                TextureMaticies = new TextureMatrix[3];
                TextureCombiners = new List<TextureCombiner>();

                long pos = reader.Position;

                CullMode = reader.ReadEnum<CullMode>(true); //byte
                IsPolygonOffsetEnabled = reader.ReadBoolean(); //byte
                PolygonOffset = reader.ReadUInt32();
                PolygonOffset = IsPolygonOffsetEnabled ? PolygonOffset / 0x10000 : 0;

                //Texture bind data
                reader.SeekBegin(pos + 0x10);
                for (int j = 0; j < 3; j++)
                {
                    TextureMaps[j] = new TextureMap();
                    TextureMaps[j].TextureIndex = reader.ReadInt16();
                    reader.ReadInt16(); //padding
                    TextureMaps[j].MinFiler = (TextureFilter)reader.ReadUInt16();
                    TextureMaps[j].MagFiler = (TextureFilter)reader.ReadUInt16();
                    TextureMaps[j].WrapS = (CMBTextureWrapMode)reader.ReadUInt16();
                    TextureMaps[j].WrapT = (CMBTextureWrapMode)reader.ReadUInt16();
                    TextureMaps[j].MinLOD = reader.ReadSingle();
                    TextureMaps[j].LodBias = reader.ReadSingle();
                    TextureMaps[j].borderColorR = reader.ReadByte();
                    TextureMaps[j].borderColorG = reader.ReadByte();
                    TextureMaps[j].borderColorB = reader.ReadByte();
                    TextureMaps[j].borderColorA = reader.ReadByte();
                }

                for (int j = 0; j < 3; j++)
                {
                    TextureMaticies[j] = new TextureMatrix();
                    TextureMaticies[j].Flags = reader.ReadUInt32();
                    TextureMaticies[j].Scale = reader.ReadVec2SY();
                    TextureMaticies[j].Translate = reader.ReadVec2SY();
                    TextureMaticies[j].Rotate = reader.ReadSingle();
                }

                uint unkColor0 = reader.ReadUInt32();

                ConstantColors = new STColor[6];
                ConstantColors[0] = STColor.FromBytes(reader.ReadBytes(4));
                ConstantColors[1] = STColor.FromBytes(reader.ReadBytes(4));
                ConstantColors[2] = STColor.FromBytes(reader.ReadBytes(4));
                ConstantColors[3] = STColor.FromBytes(reader.ReadBytes(4));
                ConstantColors[4] = STColor.FromBytes(reader.ReadBytes(4));
                ConstantColors[5] = STColor.FromBytes(reader.ReadBytes(4));

                float bufferColorR = reader.ReadSingle();
                float bufferColorG = reader.ReadSingle();
                float bufferColorB = reader.ReadSingle();
                float bufferColorA = reader.ReadSingle();

                int bumpTextureIndex = reader.ReadInt16();
                int lightEnvBumpUsage = reader.ReadInt16();

                // Fragment lighting table.
                byte reflectanceRSamplerIsAbs = reader.ReadByte();
                ushort reflectanceRSamplerInput = reader.ReadUInt16();
                uint reflectanceRSamplerScale = reader.ReadUInt32();

                byte reflectanceGSamplerIsAbs = reader.ReadByte();
                ushort reflectanceGSamplerInput = reader.ReadUInt16();
                uint reflectanceGSamplerScale = reader.ReadUInt32();

                byte reflectanceBSamplerIsAbs = reader.ReadByte();
                ushort reflectanceBSamplerInput = reader.ReadUInt16();
                uint reflectanceBSamplerScale = reader.ReadUInt32();

                byte reflectance0SamplerIsAbs = reader.ReadByte();
                ushort reflectance0SamplerInput = reader.ReadUInt16();
                uint reflectance0SamplerScale = reader.ReadUInt32();

                byte reflectance1SamplerIsAbs = reader.ReadByte();
                ushort reflectance1SamplerInput = reader.ReadUInt16();
                uint reflectance1SamplerScale = reader.ReadUInt32();

                byte fresnelSamplerIsAbs = reader.ReadByte();
                ushort fresnelSamplerInput = reader.ReadUInt16();
                uint fresnelSamplerScale = reader.ReadUInt32();

                reader.SeekBegin(pos + 0x120);
                uint textureCombinerTableCount = reader.ReadUInt32();
                int textureCombinerTableIdx = (int)pos + 0x124;
                for (int i = 0; i < textureCombinerTableCount; i++)
                {
                    reader.SeekBegin(textureCombinerTableIdx + 0x00);
                    ushort textureCombinerIndex = reader.ReadUInt16();

                    reader.SeekBegin(materialChunkParent.textureCombinerSettingsTableOffs + textureCombinerIndex * 0x28);
                    TextureCombiner combner = new TextureCombiner();
                    combner.combineRGB = reader.ReadEnum<CombineResultOpDMP>(false);
                    combner.combineAlpha = reader.ReadEnum<CombineResultOpDMP>(false);
                    combner.scaleRGB = reader.ReadEnum<CombineScaleDMP>(false);
                    combner.scaleAlpha = reader.ReadEnum<CombineScaleDMP>(false);
                    combner.bufferInputRGB = reader.ReadEnum<CombineBufferInputDMP>(false);
                    combner.bufferInputAlpha = reader.ReadEnum<CombineBufferInputDMP>(false);
                    combner.source0RGB = reader.ReadEnum<CombineSourceDMP>(false);
                    combner.source1RGB = reader.ReadEnum<CombineSourceDMP>(false);
                    combner.source2RGB = reader.ReadEnum<CombineSourceDMP>(false);
                    combner.op0RGB = reader.ReadEnum<CombineOpDMP>(false);
                    combner.op1RGB = reader.ReadEnum<CombineOpDMP>(false);
                    combner.op2RGB = reader.ReadEnum<CombineOpDMP>(false);
                    combner.source0Alpha = reader.ReadEnum<CombineSourceDMP>(false);
                    combner.source1Alpha = reader.ReadEnum<CombineSourceDMP>(false);
                    combner.source2Alpha = reader.ReadEnum<CombineSourceDMP>(false);
                    combner.op0Alpha = reader.ReadEnum<CombineOpDMP>(false);
                    combner.op1Alpha = reader.ReadEnum<CombineOpDMP>(false);
                    combner.op2Alpha = reader.ReadEnum<CombineOpDMP>(false);
                    combner.constantIndex = reader.ReadUInt32();
                    TextureCombiners.Add(combner);

                    textureCombinerTableIdx += 0x2;
                }

                reader.SeekBegin(pos + 0x130);
                AlphaTestEnable = reader.ReadBoolean();
                AlphaTestReference = reader.ReadByte() / 0xFF;
                AlphaTestFunction = (AlphaFunction)reader.ReadUInt16();

                DepthTestEnable = reader.ReadBoolean();
                DepthWriteEnable = reader.ReadBoolean();
                DepthTestFunction = (DepthFunction)reader.ReadUInt16();

                if (!AlphaTestEnable)
                    AlphaTestFunction = AlphaFunction.Always;

                if (!DepthTestEnable)
                    DepthTestFunction = DepthFunction.Always;

                reader.SeekBegin(pos + 0x138);
                BlendEnaled = reader.ReadBoolean();
                Console.WriteLine("BlendEnaled " + BlendEnaled);

                //Unknown. 
                reader.ReadByte();
                reader.ReadUInt16();

                reader.SeekBegin(pos + 0x13C);

                BlendingFactorSrcAlpha = (BlendingFactor)reader.ReadUInt16();
                BlendingFactorDestAlpha = (BlendingFactor)reader.ReadUInt16();

                reader.SeekBegin(pos + 0x144);

                BlendingFactorSrcRGB = (BlendingFactor)reader.ReadUInt16();
                BlendingFactorDestRGB = (BlendingFactor)reader.ReadUInt16();

                //  BlendingFunctionAlpha = (AlphaFunction)reader.ReadUInt16();

                reader.SeekBegin(pos + 0x14C);

                BlendColorR = reader.ReadSingle();
                BlendColorG = reader.ReadSingle();
                BlendColorB = reader.ReadSingle();
                BlendColorA = reader.ReadSingle();

                IsTransparent = BlendEnaled;

                Console.WriteLine(ToString());
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"----------------------------------------------------\n");
                sb.Append($"AlphaTest {AlphaTestEnable} {AlphaTestFunction} {AlphaTestReference}\n");
                sb.Append($"DepthTest {DepthTestEnable} {DepthTestFunction} DepthWrite {DepthTestFunction}\n");

                sb.Append($"BlendingFactorSrcRGB {BlendingFactorSrcRGB}\n");
                sb.Append($"BlendingFactorDestRGB {BlendingFactorDestRGB}\n");
                sb.Append($"BlendingFactorSrcAlpha {BlendingFactorSrcAlpha}\n");
                sb.Append($"BlendingFactorDestAlpha {BlendingFactorDestAlpha}\n");
                sb.Append($"BlendEnaled {BlendEnaled}\n");
                sb.Append($"----------------------------------------------------\n");

                sb.AppendLine();

                return sb.ToString();
            }
        }

        public class TextureCombiner
        {
            public CombineResultOpDMP combineRGB;
            public CombineResultOpDMP combineAlpha;
            public CombineScaleDMP scaleRGB;
            public CombineScaleDMP scaleAlpha;
            public CombineBufferInputDMP bufferInputRGB;
            public CombineBufferInputDMP bufferInputAlpha;
            public CombineSourceDMP source0RGB;
            public CombineSourceDMP source1RGB;
            public CombineSourceDMP source2RGB;
            public CombineOpDMP op0RGB;
            public CombineOpDMP op1RGB;
            public CombineOpDMP op2RGB;
            public CombineSourceDMP source0Alpha;
            public CombineSourceDMP source1Alpha;
            public CombineSourceDMP source2Alpha;

            public CombineOpDMP op0Alpha;
            public CombineOpDMP op1Alpha;
            public CombineOpDMP op2Alpha;
            public uint constantIndex;

        }

        public class TextureMatrix
        {
            public uint Flags { get; set; }
            public Syroot.Maths.Vector2F Scale { get; set; }
            public Syroot.Maths.Vector2F Translate { get; set; }
            public float Rotate { get; set; }
        }

        public class TextureMap
        {
            public short TextureIndex { get; set; }
            public TextureFilter MinFiler { get; set; }
            public TextureFilter MagFiler { get; set; }
            public CMBTextureWrapMode WrapS { get; set; }
            public CMBTextureWrapMode WrapT { get; set; }
            public float MinLOD { get; set; }
            public float LodBias { get; set; }
            public byte borderColorR { get; set; }
            public byte borderColorG { get; set; }
            public byte borderColorB { get; set; }
            public byte borderColorA { get; set; }
        }

        public class TextureChunk : IChunkCommon
        {
            private const string Magic = "tex ";

            public List<CTXB.Texture> Textures = new List<CTXB.Texture>();

            public void Read(FileReader reader, Header header)
            {
                reader.ReadSignature(4, Magic);
                uint sectionSize = reader.ReadUInt32();
                uint TextureCount = reader.ReadUInt32();
                for (int i = 0; i < TextureCount; i++)
                    Textures.Add(new CTXB.Texture(reader));
            }

            public void Write(FileWriter writer, Header header)
            {
                long pos = writer.Position;

                writer.WriteSignature(Magic);
                writer.Write(uint.MaxValue);//SectionSize
                for (int i = 0; i < Textures.Count; i++)
                    Textures[i].Write(writer);

                //Write the total file size
                writer.WriteSectionSizeU32(pos + 4, pos, writer.Position);
            }
        }

        public static T ReadChunkSection<T>(FileReader reader, Header header, long startPos = 0)
             where T : IChunkCommon, new()
        {
            long pos = reader.Position;

            //Read offset and seek it
            uint offset = reader.ReadUInt32();
            reader.SeekBegin(startPos + offset);

            //Create chunk instance
            T chunk = new T();
            chunk.Read(reader, header);

            //Seek back and shift 4 from reading offset
            reader.SeekBegin(pos + 0x4);
            return chunk;
        }

        public interface IChunkCommon
        {
            void Read(FileReader reader, Header header);
            void Write(FileWriter writer, Header header);
        }
    }
}
