using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library.Rendering;
using OpenTK;

namespace FirstPlugin
{
    public class GFBMDL : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Graphic Model" };
        public string[] Extension { get; set; } = new string[] { "*.gfbmdl" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                bool IsMatch = reader.ReadUInt32() == 0x20000000;
                reader.Position = 0;

                return IsMatch;
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
                var editor = LibraryGUI.Instance.GetObjectEditor();
                return editor.GetViewport();
            }
            set
            {
                var editor = LibraryGUI.Instance.GetObjectEditor();
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
                LibraryGUI.Instance.LoadEditor(viewport);

                viewport.Text = Text;
            }
        }

        public Header header;
        public GFBMDL_Render Renderer;

        public DrawableContainer DrawableContainer = new DrawableContainer();

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;
            DrawableContainer.Name = FileName;
            Renderer = new GFBMDL_Render();

            header = new Header();
            header.Read(new FileReader(stream), this);

        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
            header.Write(new FileWriter(mem));
            return mem.ToArray();
        }

        //Todo replace tedius offset handling with a class to store necessary data and methods to execute
        public class Header
        {
            public STSkeleton Skeleton { get; set; }

            public uint Version { get; set; }
            public float[] Boundings { get; set; }
            public List<string> TextureMaps = new List<string>();
            public List<string> MaterialNames = new List<string>();
            public List<VertexBuffer> VertexBuffers = new List<VertexBuffer>();
            public List<VisGroup> VisualGroups = new List<VisGroup>();
            public List<MaterialShaderData> Materials = new List<MaterialShaderData>();

            public List<GFBMaterial> GenericMaterials = new List<GFBMaterial>();

            public void Read(FileReader reader, GFBMDL Root)
            {
                Skeleton = new STSkeleton();
                Root.DrawableContainer.Drawables.Add(Skeleton);
                Root.DrawableContainer.Drawables.Add(Root.Renderer);

                reader.SetByteOrder(false);

                Version = reader.ReadUInt32();
                Boundings = reader.ReadSingles(9);
                long TextureOffset = reader.ReadOffset(true, typeof(uint));
                long MaterialOffset = reader.ReadOffset(true, typeof(uint));
                long Unknown1ffset = reader.ReadOffset(true, typeof(uint));
                long Unknown2Offset = reader.ReadOffset(true, typeof(uint));
                long ShaderOffset = reader.ReadOffset(true, typeof(uint));
                long VisGroupOffset = reader.ReadOffset(true, typeof(uint));
                long VerteBufferOffset = reader.ReadOffset(true, typeof(uint));
                long BoneDataOffset = reader.ReadOffset(true, typeof(uint));

                if (TextureOffset != 0)
                {
                    reader.SeekBegin(TextureOffset);
                    uint Count = reader.ReadUInt32();
                    TextureMaps = reader.ReadNameOffsets(Count, true, typeof(uint), true);
                }

                if (MaterialOffset != 0)
                {
                    reader.SeekBegin(MaterialOffset);
                    uint Count = reader.ReadUInt32();
                    MaterialNames = reader.ReadNameOffsets(Count, true, typeof(uint));
                }

                
                if (ShaderOffset != 0)
                {
                    reader.SeekBegin(ShaderOffset);
                    uint Count = reader.ReadUInt32();
                    for (int i = 0; i < Count; i++)
                    {
                        MaterialShaderData shaderData = new MaterialShaderData();
                        shaderData.Read(reader);
                        Materials.Add(shaderData);
                    }
                }

                if (VisGroupOffset != 0)
                {
                    reader.SeekBegin(VisGroupOffset);
                    uint Count = reader.ReadUInt32();
                    for (int i = 0; i < Count; i++)
                    {
                        VisGroup visualGroup = new VisGroup();
                        visualGroup.Read(reader);
                        VisualGroups.Add(visualGroup);
                    }
                }

                if (VerteBufferOffset != 0)
                {
                    reader.SeekBegin(VerteBufferOffset);
                    uint Count = reader.ReadUInt32();
                    for (int i = 0; i < Count; i++)
                    {
                        VertexBuffer vertexBuffer = new VertexBuffer();
                        vertexBuffer.Read(reader);
                        VertexBuffers.Add(vertexBuffer);
                    }
                }

                if (BoneDataOffset != 0)
                {
                    TreeNode SkeletonWrapper = new TreeNode("Skeleton");
                    Root.Nodes.Add(SkeletonWrapper);

                    reader.SeekBegin(BoneDataOffset);
                    uint Count = reader.ReadUInt32();
                    for (int i = 0; i < Count; i++)
                    {
                        var bone = new Bone(Skeleton);
                        bone.Read(reader);
                        Skeleton.bones.Add(bone);
                    }

                    foreach (var bone in Skeleton.bones)
                    {
                        if (bone.Parent == null)
                            SkeletonWrapper.Nodes.Add(bone);
                    }
                }

                TreeNode MaterialFolderWrapper = new TreeNode("Materials");
                Root.Nodes.Add(MaterialFolderWrapper);

                for (int i = 0; i < Materials.Count; i++)
                {
                    GFBMaterial mat = new GFBMaterial(Root);
                    mat.Text = Materials[i].Name;

                    int textureUnit = 1;
                    foreach (var textureMap in Materials[i].TextureMaps)
                    {
                        textureMap.Name = TextureMaps[(int)textureMap.Index];

                        STGenericMatTexture matTexture = new STGenericMatTexture();
                        matTexture.Name = textureMap.Name;
                        matTexture.textureUnit = textureUnit++;
                        matTexture.wrapModeS = 0;
                        matTexture.wrapModeT = 0;

                        if (textureMap.Effect == "Col0Tex")
                        {
                            matTexture.Type = STGenericMatTexture.TextureType.Diffuse;
                        }

                        mat.TextureMaps.Add(matTexture);

                        if (textureMap.Effect != string.Empty)
                            mat.Nodes.Add($"{textureMap.Name} ({textureMap.Effect})");
                        else
                            mat.Nodes.Add($"{textureMap.Name}");
                    }

                    GenericMaterials.Add(mat);
                    MaterialFolderWrapper.Nodes.Add(mat);
                }

                TreeNode VisualGroupWrapper = new TreeNode("Visual Groups");
                Root.Nodes.Add(VisualGroupWrapper);

                for (int i = 0; i < VisualGroups.Count; i++)
                {
                    GFBMesh mesh = new GFBMesh(Root);
                    mesh.Checked = true;
                    mesh.ImageKey = "model";
                    mesh.SelectedImageKey = "model";

                    mesh.BoneIndex = (int)VisualGroups[i].BoneIndex;
                    mesh.Text = Skeleton.bones[(int)VisualGroups[i].BoneIndex].Text;
                    Root.Renderer.Meshes.Add(mesh);

                    var Buffer = VertexBuffers[i];
                    for (int v= 0; v < Buffer.Positions.Count; v++)
                    {
                        Vertex vertex = new Vertex();
                        vertex.pos = Buffer.Positions[v];

                        if (Buffer.Normals.Count > 0)
                            vertex.nrm = Buffer.Normals[v];
                        if (Buffer.TexCoord1.Count > 0)
                            vertex.uv0 = Buffer.TexCoord1[v];
                        if (Buffer.TexCoord2.Count > 0)
                            vertex.uv1 = Buffer.TexCoord2[v];
                        if (Buffer.TexCoord3.Count > 0)
                            vertex.uv2 = Buffer.TexCoord3[v];
                        if (Buffer.Weights.Count > 0)
                            vertex.boneWeights = new List<float>(Buffer.Weights[v]);
                        if (Buffer.BoneIndex.Count > 0)
                            vertex.boneIds = new List<int>(Buffer.BoneIndex[v]);
                        if (Buffer.Colors1.Count > 0)
                            vertex.col = Buffer.Colors1[v];
                        if (Buffer.Binormals.Count > 0)
                            vertex.bitan = Buffer.Binormals[v];

                        mesh.vertices.Add(vertex);
                    }

                    int polyIndex = 0;
                    foreach (var group in Buffer.PolygonGroups)
                    {
                        TreeNode polyWrapper = new TreeNode($"Polygon Group {polyIndex++}");
                        polyWrapper.ImageKey = "mesh";
                        polyWrapper.SelectedImageKey = "mesh";

                        var polygonGroup = new STGenericPolygonGroup();
                        polygonGroup.faces = group.Faces.ToList();
                        polygonGroup.MaterialIndex = group.MaterialID;
                        mesh.PolygonGroups.Add(polygonGroup);

                        mesh.Nodes.Add(polyWrapper);
                    }

                    VisualGroupWrapper.Nodes.Add(mesh);
                }


                Skeleton.update();
                Skeleton.reset();
            }

            public void Write(FileWriter writer)
            {
                writer.Write(Version);
            }
        }

        public class VisGroup
        {
            public Vector3 BoundingBoxMax = new Vector3(0);
            public Vector3 BoundingBoxMin = new Vector3(0);

            public uint BoneIndex { get; set; }

            public void Read(FileReader reader)
            {
                long DataPosition = reader.Position;
                var DataOffset = reader.ReadOffset(true, typeof(uint));

                reader.SeekBegin(DataOffset);
                long InfoPosition = reader.Position;

                int InfoOffset = reader.ReadInt32();

                //Read the info section for position data
                reader.SeekBegin(InfoPosition - InfoOffset);
                ushort HeaderLength = reader.ReadUInt16();
                ushort UnknownPosition = 0;
                ushort VisBonePosition = 0;
                ushort Unknown2Position = 0;
                ushort VisBoundsPosition = 0;
                ushort Unknown3Position = 0;

                if (HeaderLength == 0x0A)
                {
                    UnknownPosition = reader.ReadUInt16();
                    VisBonePosition = reader.ReadUInt16();
                    Unknown2Position = reader.ReadUInt16();
                    VisBoundsPosition = reader.ReadUInt16();
                }
                else if (HeaderLength == 0x0C)
                {
                    UnknownPosition = reader.ReadUInt16();
                    VisBonePosition = reader.ReadUInt16();
                    Unknown2Position = reader.ReadUInt16();
                    VisBoundsPosition = reader.ReadUInt16();
                    Unknown3Position = reader.ReadUInt16();
                }
                else
                    throw new Exception("Unexpected Header Length! " + HeaderLength);

                if (VisBoundsPosition != 0)
                {
                    reader.SeekBegin(InfoPosition + VisBoundsPosition);
                    BoundingBoxMin = reader.ReadVec3();
                    BoundingBoxMax = reader.ReadVec3();
                }

                if (VisBonePosition != 0)
                {
                    reader.SeekBegin(InfoPosition + VisBonePosition);
                    BoneIndex = reader.ReadUInt32();
                }

                //Seek back to next in array
                reader.SeekBegin(DataPosition + sizeof(uint));
            }
        }

        public class MaterialShaderData
        {
            public string Name { get; set; }
            public string ShaderName { get; set; }

            public List<TextureMap> TextureMaps = new List<TextureMap>();

            public void Read(FileReader reader)
            {
                long DataPosition = reader.Position;
                var DataOffset = reader.ReadOffset(true, typeof(uint));

                reader.SeekBegin(DataOffset);
                long InfoPosition = reader.Position;

                int InfoOffset = reader.ReadInt32();

                //Read the info section for position data
                reader.SeekBegin(InfoPosition - InfoOffset);
                ushort HeaderLength = reader.ReadUInt16();
                ushort ShaderPropertyPosition = reader.ReadUInt16();
                ushort MaterialStringNamePosition = reader.ReadUInt16();
                ushort ShaderStringNamePosition = reader.ReadUInt16();
                ushort Unknown1Position = reader.ReadUInt16();
                ushort Unknown2Position = reader.ReadUInt16();
                ushort Unknown3Position = reader.ReadUInt16();
                ushort ShaderParam1Position = reader.ReadUInt16();
                ushort ShaderParam2Position = reader.ReadUInt16();
                ushort ShaderParam3Position = reader.ReadUInt16();
                ushort ShaderParam4Position = reader.ReadUInt16();
                ushort ShaderParam5Position = reader.ReadUInt16();
                ushort ShaderParam6Position = reader.ReadUInt16();
                ushort TextureMapsPosition = reader.ReadUInt16();
                ushort ShaderParamAPosition = reader.ReadUInt16();
                ushort ShaderParamBPosition = reader.ReadUInt16();
                ushort ShaderParamCPosition = reader.ReadUInt16();
                ushort Unknown4Position = reader.ReadUInt16();
                ushort Unknown5Position = reader.ReadUInt16();
                ushort Unknown6Position = reader.ReadUInt16();
                ushort Unknown7Position = reader.ReadUInt16();
                ushort Unknown8Position = reader.ReadUInt16();
                ushort ShaderProperty2Position = reader.ReadUInt16();

                if (MaterialStringNamePosition != 0)
                {
                    reader.SeekBegin(InfoPosition + MaterialStringNamePosition);
                    Name = reader.ReadNameOffset(true, typeof(uint), true);
                }

                if (ShaderStringNamePosition != 0)
                {
                    reader.SeekBegin(InfoPosition + ShaderStringNamePosition);
                    ShaderName = reader.ReadNameOffset(true, typeof(uint), true);
                }

                if (TextureMapsPosition != 0)
                {
                    reader.SeekBegin(InfoPosition + TextureMapsPosition);
                    var TextureMapOffset = reader.ReadOffset(true, typeof(uint));
                    reader.SeekBegin(TextureMapOffset);

                    uint Count = reader.ReadUInt32();
                    for (int i = 0; i < Count; i++)
                    {
                        TextureMap textureMap = new TextureMap();
                        textureMap.Read(reader);
                        TextureMaps.Add(textureMap);
                    }
                }

                //Seek back to next in array
                reader.SeekBegin(DataPosition + sizeof(uint));
            }
        }

        public class TextureMap
        {
            public string Effect { get; set; }
            public string Name { get; set; }

            public uint Index { get; set; } = 0;

            public void Read(FileReader reader)
            {
                long DataPosition = reader.Position;
                var DataOffset = reader.ReadOffset(true, typeof(uint));

                reader.SeekBegin(DataOffset);
                long InfoPosition = reader.Position;
                int InfoOffset = reader.ReadInt32();

                //Read the info section for position data
                reader.SeekBegin(InfoPosition - InfoOffset);
                ushort HeaderSize = reader.ReadUInt16();
                ushort EffectPosition = 0;
                ushort UnknownPosition = 0;
                ushort IdPosition = 0;
                ushort UnknownPosition2 = 0;

                if (HeaderSize == 0x0A)
                {
                    EffectPosition = reader.ReadUInt16();
                    UnknownPosition = reader.ReadUInt16();
                    IdPosition = reader.ReadUInt16();
                    UnknownPosition2 = reader.ReadUInt16();
                }
                else
                    throw new Exception("Unexpected header size! " + HeaderSize);

                if (EffectPosition != 0)
                {
                    reader.SeekBegin(InfoPosition + EffectPosition);
                    uint NameLength = reader.ReadUInt32();
                    Effect = reader.ReadString((int)NameLength);
                }

                if (IdPosition != 0)
                {
                    reader.SeekBegin(InfoPosition + IdPosition);
                    Index = reader.ReadUInt32();
                }

                //Seek back to next in array
                reader.SeekBegin(DataPosition + sizeof(uint));
            }
        }

        public class VertexBuffer
        {
            public List<VertexAttribute> Attributes = new List<VertexAttribute>();
            public List<PolygonGroup> PolygonGroups = new List<PolygonGroup>();

            public List<Vector3> Positions = new List<Vector3>();
            public List<Vector3> Normals = new List<Vector3>();
            public List<Vector2> TexCoord1 = new List<Vector2>();
            public List<Vector2> TexCoord2 = new List<Vector2>();
            public List<Vector2> TexCoord3 = new List<Vector2>();
            public List<Vector2> TexCoord4 = new List<Vector2>();
            public List<int[]> BoneIndex = new List<int[]>();
            public List<float[]> Weights = new List<float[]>();
            public List<Vector4> Colors1 = new List<Vector4>();
            public List<Vector4> Colors2 = new List<Vector4>();
            public List<Vector4> Binormals = new List<Vector4>();

            public void Read(FileReader reader)
            {
                long DataPosition = reader.Position;
                var VertexBufferDataOffset = reader.ReadOffset(true, typeof(uint));

                reader.SeekBegin(VertexBufferDataOffset);
                long InfoPosition = reader.Position;
                int InfoOffset = reader.ReadInt32();

                //Read the info section for position data
                reader.SeekBegin(InfoPosition - InfoOffset);
                ushort InfoSize = reader.ReadUInt16();
                ushort VerticesPosition = 0;
                ushort FacesPosition = 0;
                ushort AttributeInfoPosition = 0;
                ushort BufferUnknownPosition = 0;

                if (InfoSize == 0x0A)
                {
                    BufferUnknownPosition = reader.ReadUInt16();
                    FacesPosition = reader.ReadUInt16();
                    AttributeInfoPosition = reader.ReadUInt16();
                    VerticesPosition = reader.ReadUInt16();
                }
                else
                    throw new Exception("Unexpected Vertex Buffer Info Header Size! " + InfoSize);

                uint VertBufferStride = 0;

                if (AttributeInfoPosition != 0)
                {
                    reader.SeekBegin(InfoPosition + AttributeInfoPosition);
                    var AttributeOffset = reader.ReadOffset(true, typeof(uint));

                    reader.SeekBegin(AttributeOffset);
                    uint AttributeCount = reader.ReadUInt32();

                    for (int i = 0; i < AttributeCount; i++)
                    {
                        var attribute = new VertexAttribute();
                        attribute.Read(reader);
                        Attributes.Add(attribute);

                        switch (attribute.Type)
                        {
                            case VertexAttribute.BufferType.Position:
                                if (attribute.Format == VertexAttribute.BufferFormat.HalfFloat)
                                    VertBufferStride += 0x08;
                                else if (attribute.Format == VertexAttribute.BufferFormat.Float)
                                    VertBufferStride += 0x0C;
                                else
                                    throw new Exception($"Unknown Combination! {attribute.Type} {attribute.Format}");
                                break;
                            case VertexAttribute.BufferType.Normal:
                                if (attribute.Format == VertexAttribute.BufferFormat.HalfFloat)
                                    VertBufferStride += 0x08;
                                else if (attribute.Format == VertexAttribute.BufferFormat.Float)
                                    VertBufferStride += 0x0C;
                                else
                                    throw new Exception($"Unknown Combination! {attribute.Type} {attribute.Format}");
                                break;
                            case VertexAttribute.BufferType.Binormal:
                                if (attribute.Format == VertexAttribute.BufferFormat.HalfFloat)
                                    VertBufferStride += 0x08;
                                else if (attribute.Format == VertexAttribute.BufferFormat.Float)
                                    VertBufferStride += 0x0C;
                                else
                                    throw new Exception($"Unknown Combination! {attribute.Type} {attribute.Format}");
                                break;
                            case VertexAttribute.BufferType.TexCoord1:
                            case VertexAttribute.BufferType.TexCoord2:
                            case VertexAttribute.BufferType.TexCoord3:
                            case VertexAttribute.BufferType.TexCoord4:
                                if (attribute.Format == VertexAttribute.BufferFormat.HalfFloat)
                                    VertBufferStride += 0x04;
                                else if (attribute.Format == VertexAttribute.BufferFormat.Float)
                                    VertBufferStride += 0x08;
                                else
                                    throw new Exception($"Unknown Combination! {attribute.Type} {attribute.Format}");
                                break;
                            case VertexAttribute.BufferType.Color1:
                            case VertexAttribute.BufferType.Color2:
                                if (attribute.Format == VertexAttribute.BufferFormat.Byte)
                                    VertBufferStride += 0x04;
                                else
                                    throw new Exception($"Unknown Combination! {attribute.Type} {attribute.Format}");
                                break;
                            case VertexAttribute.BufferType.BoneIndex:
                                if (attribute.Format == VertexAttribute.BufferFormat.Short)
                                    VertBufferStride += 0x08;
                                else if (attribute.Format == VertexAttribute.BufferFormat.Byte)
                                    VertBufferStride += 0x04;
                                else
                                    throw new Exception($"Unknown Combination! {attribute.Type} {attribute.Format}");
                                break;
                            case VertexAttribute.BufferType.Weights:
                                if (attribute.Format == VertexAttribute.BufferFormat.BytesAsFloat)
                                    VertBufferStride += 0x04;
                                else
                                    throw new Exception($"Unknown Combination! {attribute.Type} {attribute.Format}");
                                break;
                        }

                        Console.WriteLine($"{attribute.Format} {attribute.Type} VertBufferStride {VertBufferStride}");
                    }
                }

                if (FacesPosition != 0)
                {
                    reader.SeekBegin(InfoPosition + FacesPosition);
                    var BufferOffset = reader.ReadOffset(true, typeof(uint));

                    reader.SeekBegin(BufferOffset);
                    uint GroupCount = reader.ReadUInt32();

                    for (int i = 0; i < GroupCount; i++)
                    {
                        var polygonGroup = new PolygonGroup();
                        polygonGroup.Read(reader);
                        PolygonGroups.Add(polygonGroup);
                    }
                }

                if (VerticesPosition != 0)
                {
                    reader.SeekBegin(InfoPosition + VerticesPosition);
                    var VertexOffset = reader.ReadOffset(true, typeof(uint));

                    reader.SeekBegin(VertexOffset);
                    uint VertexBufferSize = reader.ReadUInt32();

                    for (int v = 0; v < VertexBufferSize / VertBufferStride; v++)
                    {
                        for (int att = 0; att < Attributes.Count; att++)
                        {
                            switch (Attributes[att].Type)
                            {
                                case VertexAttribute.BufferType.Position:
                                    var pos = ParseBuffer(reader, Attributes[att].Format, Attributes[att].Type);
                                    Positions.Add(new Vector3(pos.X, pos.Y, pos.Z));
                                    break;
                                case VertexAttribute.BufferType.Normal:
                                    var normal = ParseBuffer(reader, Attributes[att].Format, Attributes[att].Type);
                                    Normals.Add(new Vector3(normal.X, normal.Y, normal.Z));
                                    break;
                                case VertexAttribute.BufferType.TexCoord1:
                                    var texcoord1 = ParseBuffer(reader, Attributes[att].Format, Attributes[att].Type);
                                    TexCoord1.Add(new Vector2(texcoord1.X, texcoord1.Y));
                                    break;
                                case VertexAttribute.BufferType.TexCoord2:
                                    var texcoord2 = ParseBuffer(reader, Attributes[att].Format, Attributes[att].Type);
                                    TexCoord2.Add(new Vector2(texcoord2.X, texcoord2.Y));
                                    break;
                                case VertexAttribute.BufferType.TexCoord3:
                                    var texcoord3 = ParseBuffer(reader, Attributes[att].Format, Attributes[att].Type);
                                    TexCoord3.Add(new Vector2(texcoord3.X, texcoord3.Y));
                                    break;
                                case VertexAttribute.BufferType.TexCoord4:
                                    var texcoord4 = ParseBuffer(reader, Attributes[att].Format, Attributes[att].Type);
                                    TexCoord4.Add(new Vector2(texcoord4.X, texcoord4.Y));
                                    break;
                                case VertexAttribute.BufferType.Weights:
                                    var weights = ParseBuffer(reader, Attributes[att].Format, Attributes[att].Type);
                                    Weights.Add(new float[] { weights.X, weights.Y, weights.Z, weights.W });
                                    break;
                                case VertexAttribute.BufferType.BoneIndex:
                                    var boneIndices = ParseBuffer(reader, Attributes[att].Format, Attributes[att].Type);
                                    BoneIndex.Add(new int[] { (int)boneIndices.X, (int)boneIndices.Y, (int)boneIndices.Z, (int)boneIndices.W });
                                    break;
                                case VertexAttribute.BufferType.Color1:
                                    var colors1 = ParseBuffer(reader, Attributes[att].Format, Attributes[att].Type);
                                    Colors1.Add(new Vector4(colors1.X, colors1.Y, colors1.Z, colors1.W));
                                    break;
                                case VertexAttribute.BufferType.Color2:
                                    var colors2 = ParseBuffer(reader, Attributes[att].Format, Attributes[att].Type);
                                    Colors2.Add(new Vector4(colors2.X, colors2.Y, colors2.Z, colors2.W));
                                    break;
                                case VertexAttribute.BufferType.Binormal:
                                    var binormals = ParseBuffer(reader, Attributes[att].Format, Attributes[att].Type);
                                    Binormals.Add(new Vector4(binormals.X, binormals.Y, binormals.Z, binormals.W));
                                    break;
                            }
                        }

                    }
                }



                //Seek back to next in array
                reader.SeekBegin(DataPosition + sizeof(uint));
            }

            private Vector4 ParseBuffer(FileReader reader, VertexAttribute.BufferFormat Format, VertexAttribute.BufferType AttributeType)
            {
                if (AttributeType == VertexAttribute.BufferType.Position)
                {
                    switch (Format)
                    {
                        case VertexAttribute.BufferFormat.Float:
                            return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0);
                        case VertexAttribute.BufferFormat.HalfFloat:
                            return new Vector4(reader.ReadHalfSingle(), reader.ReadHalfSingle(),
                                               reader.ReadHalfSingle(), reader.ReadHalfSingle());
                        default:
                            throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                    }
                }
                else if (AttributeType == VertexAttribute.BufferType.Normal)
                {
                    switch (Format)
                    {
                        case VertexAttribute.BufferFormat.Float:
                            return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0);
                        case VertexAttribute.BufferFormat.HalfFloat:
                            return new Vector4(reader.ReadHalfSingle(), reader.ReadHalfSingle(),
                                               reader.ReadHalfSingle(), reader.ReadHalfSingle());
                        default:
                            throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                    }
                }
                else if (AttributeType == VertexAttribute.BufferType.Binormal)
                {
                    switch (Format)
                    {
                        case VertexAttribute.BufferFormat.Float:
                            return new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0);
                        case VertexAttribute.BufferFormat.HalfFloat:
                            return new Vector4(reader.ReadHalfSingle(), reader.ReadHalfSingle(),
                                               reader.ReadHalfSingle(), reader.ReadHalfSingle());
                        default:
                            throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                    }
                }
                else if (AttributeType == VertexAttribute.BufferType.TexCoord1 ||
                         AttributeType == VertexAttribute.BufferType.TexCoord2 ||
                         AttributeType == VertexAttribute.BufferType.TexCoord3 ||
                         AttributeType == VertexAttribute.BufferType.TexCoord4)
                {
                    switch (Format)
                    {
                        case VertexAttribute.BufferFormat.Float:
                            return new Vector4(reader.ReadSingle(), reader.ReadSingle(), 0, 0);
                        case VertexAttribute.BufferFormat.HalfFloat:
                            return new Vector4(reader.ReadHalfSingle(), reader.ReadHalfSingle(), 0, 0);
                        default:
                            throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                    }
                }
                else if (AttributeType == VertexAttribute.BufferType.Color1 ||
                         AttributeType == VertexAttribute.BufferType.Color2)
                {
                    switch (Format)
                    {
                        case VertexAttribute.BufferFormat.Byte:
                            return new Vector4(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                        default:
                            throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                    }
                }
                else if (AttributeType == VertexAttribute.BufferType.BoneIndex)
                {
                    switch (Format)
                    {
                        case VertexAttribute.BufferFormat.Short:
                            return new Vector4(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
                        case VertexAttribute.BufferFormat.Byte:
                            return new Vector4(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                        default:
                            throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                    }
                }
                else if (AttributeType == VertexAttribute.BufferType.Weights)
                {
                    switch (Format)
                    {
                        case VertexAttribute.BufferFormat.BytesAsFloat:
                            return new Vector4(reader.ReadByteAsFloat(), reader.ReadByteAsFloat(),
                                               reader.ReadByteAsFloat(), reader.ReadByteAsFloat());
                        default:
                            throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                    }
                }

                return new Vector4(0);
            }
        }

        //Represents an offset which points to another
        public class OffsetInfo
        {

        }

        public class PolygonGroup
        {
            public int MaterialID { get; set; }
            public int[] Faces { get; set; }

            public void Read(FileReader reader)
            {
                long DataPosition = reader.Position;
                var DataOffset = reader.ReadOffset(true, typeof(uint));

                reader.SeekBegin(DataOffset);
                long InfoPosition = reader.Position;

                int InfoOffset = reader.ReadInt32();

                //Read the info section for position data
                reader.SeekBegin(InfoPosition - InfoOffset);
                ushort PolygonHeaderSize = reader.ReadUInt16();
                ushort PolygonStartPosition = 0;
                ushort PolgonMaterialId = 0;
                ushort PolygonUnknown = 0;

                if (PolygonHeaderSize == 0x08)
                {
                    PolygonStartPosition = reader.ReadUInt16();
                    PolgonMaterialId = reader.ReadUInt16();
                    PolygonUnknown = reader.ReadUInt16();
                }
                else
                    throw new Exception("Unexpected Polygon Buffer Info Header Size! " + PolygonHeaderSize);

                if (PolgonMaterialId != 0)
                {
                    reader.SeekBegin(InfoPosition + PolgonMaterialId);
                    MaterialID = reader.ReadInt32();
                }

                if (PolygonStartPosition != 0)
                {
                    reader.SeekBegin(InfoPosition + PolygonStartPosition);
                    uint FaceCount = reader.ReadUInt32();
                    Faces = new int[FaceCount];
                    for (int i = 0; i < FaceCount; i++)
                        Faces[i] = reader.ReadUInt16();
                }

                //Seek back to next in array
                reader.SeekBegin(DataPosition + sizeof(uint));
            }
        }

        public class VertexAttribute
        {
            public BufferFormat Format { get; set; } = BufferFormat.Float;
            public BufferType Type { get; set; } = BufferType.Position;

            public enum BufferType
            {
                Position = 0,
                Normal = 1,
                Binormal = 2,
                TexCoord1 = 3,
                TexCoord2 = 4,
                TexCoord3 = 5,
                TexCoord4 = 6,
                Color1 = 7,
                Color2 = 8,
                BoneIndex = 11,
                Weights = 12,
            }

            public enum BufferFormat
            {
                Float = 0,
                HalfFloat = 1,
                Byte = 3,
                Short = 5,
                BytesAsFloat = 8,
            }

            public void Read(FileReader reader)
            {
                long DataPosition = reader.Position;
                var DataOffset = reader.ReadOffset(true, typeof(uint));

                reader.SeekBegin(DataOffset);
                long InfoPosition = reader.Position;
                int InfoOffset = reader.ReadInt32();

                //Read the info section for position data
                reader.SeekBegin(InfoPosition - InfoOffset);
                ushort LayoutHeaderLength = reader.ReadUInt16();
                ushort LayoutSizePosition = 0;
                ushort LayoutTypePosition = 0;
                ushort LayoutFormatPosition = 0;
                ushort LayoutUnknownPosition = 0;

                if (LayoutHeaderLength == 0x0A)
                {
                    LayoutSizePosition = reader.ReadUInt16();
                    LayoutTypePosition = reader.ReadUInt16();
                    LayoutFormatPosition = reader.ReadUInt16();
                    LayoutUnknownPosition = reader.ReadUInt16();
                }
                else
                    throw new Exception("Unexpected Attribute Layout Header Size! " + LayoutHeaderLength);

                if (LayoutFormatPosition != 0)
                {
                    reader.SeekBegin(InfoPosition + LayoutFormatPosition);
                    Format = reader.ReadEnum<BufferFormat>(true);
                }

                if (LayoutTypePosition != 0)
                {
                    reader.SeekBegin(InfoPosition + LayoutTypePosition);
                    Type = reader.ReadEnum<BufferType>(true);
                }

                //Seek back to next in array
                reader.SeekBegin(DataPosition + sizeof(uint));
            }
        }

        public class Bone : STBone
        {
            internal BoneInfo BoneInfo { get; set; }

            public Bone(STSkeleton skeleton) : base(skeleton) { }

            public void Read(FileReader reader)
            {
                long DataPosition = reader.Position;
                var BoneDataOffset = reader.ReadOffset(true, typeof(uint));

                reader.SeekBegin(BoneDataOffset);
                long InfoPosition = reader.Position;

                int BoneInfoOffset = reader.ReadInt32();

                //Read the info section for position data
                reader.SeekBegin(InfoPosition - BoneInfoOffset);

                BoneInfo = new BoneInfo();
                BoneInfo.Read(reader);

                RotationType = BoneRotationType.Euler;
                Checked = true;

                if (BoneInfo.NamePosition != 0)
                {
                    reader.SeekBegin(InfoPosition + BoneInfo.NamePosition);
                    uint NameLength = reader.ReadUInt32();
                    Text = reader.ReadString((int)NameLength);
                }

                if (BoneInfo.RotationPosition != 0)
                {
                    reader.SeekBegin(InfoPosition + BoneInfo.RotationPosition);
                    float RotationX = reader.ReadSingle();
                    float RotationY = reader.ReadSingle();
                    float RotationZ = reader.ReadSingle();
                    rotation = new float[] { RotationX,RotationY, RotationZ };
                }

                if (BoneInfo.TranslationPosition != 0)
                {
                    reader.SeekBegin(InfoPosition + BoneInfo.TranslationPosition);
                    float TranslateX = reader.ReadSingle();
                    float TranslateY = reader.ReadSingle();
                    float TranslateZ = reader.ReadSingle();
                    position = new float[] { TranslateX, TranslateY, TranslateZ };
                }

                if (BoneInfo.ScalePosition != 0)
                {
                    reader.SeekBegin(InfoPosition + BoneInfo.ScalePosition);
                    float ScaleX = reader.ReadSingle();
                    float ScaleY = reader.ReadSingle();
                    float ScaleZ = reader.ReadSingle();
                    scale = new float[] { ScaleX, ScaleY, ScaleZ };
                }

                if (BoneInfo.ParentPosition != 0)
                {
                    reader.SeekBegin(InfoPosition + BoneInfo.ParentPosition);
                    parentIndex = reader.ReadInt32();
                }

                //Seek back to next bone in array
                reader.SeekBegin(DataPosition + sizeof(uint));
            }
        }

        //A section that stores position info for bone data
        public class BoneInfo
        {
            internal ushort SectionSize { get; set; }
            internal ushort NamePosition { get; set; }
            internal ushort UnknownPosition { get; set; }
            internal ushort Unknown2Position { get; set; }
            internal ushort ParentPosition { get; set; }
            internal ushort Unknown3Position { get; set; }
            internal ushort IsVisiblePosition { get; set; }
            internal ushort ScalePosition { get; set; }
            internal ushort RotationPosition { get; set; }
            internal ushort TranslationPosition { get; set; }
            internal ushort Unknown4Position { get; set; }
            internal ushort Unknown5Position { get; set; }

            public void Read(FileReader reader)
            {
                SectionSize = reader.ReadUInt16();
                NamePosition = reader.ReadUInt16();
                UnknownPosition = reader.ReadUInt16();
                Unknown2Position = reader.ReadUInt16(); 
                ParentPosition = reader.ReadUInt16();
                Unknown3Position = reader.ReadUInt16(); //Padding
                IsVisiblePosition = reader.ReadUInt16(); //Points to byte. 0 or 1 for visibilty
                ScalePosition = reader.ReadUInt16();
                RotationPosition = reader.ReadUInt16();
                TranslationPosition = reader.ReadUInt16();
                Unknown4Position = reader.ReadUInt16(); //Padding
                Unknown5Position = reader.ReadUInt16();  //Padding
            }
        }

        public class Material
        {

        }
    }
}
