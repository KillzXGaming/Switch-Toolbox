using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using OpenTK;

namespace DKCTF
{
    /// <summary>
    /// Represents a model file format for loading mesh and material data.
    /// </summary>
    public class CMDL : FileForm
    {
        /// <summary>
        /// The meshes of the model used to display the model.
        /// </summary>
        public List<CMesh> Meshes = new List<CMesh>();

        /// <summary>
        /// The materials of the model for rendering the mesh.
        /// </summary>
        public List<CMaterial> Materials = new List<CMaterial>();

        /// <summary>
        /// The vertex buffer list to read the buffer attributes.
        /// </summary>
        List<VertexBuffer> VertexBuffers = new List<VertexBuffer>();

        /// <summary>
        /// The index buffer list to read the index buffer data.
        /// </summary>
        List<CGraphicsIndexBufferToken> IndexBuffer = new List<CGraphicsIndexBufferToken>();

        /// <summary>
        /// Determines which variant of the file to parse. Switch reads strings and materials differently.
        /// </summary>
        bool IsSwitch => this.FileHeader.FormType == "SMDL" && this.FileHeader.VersionA >= 0x3A ||
                                this.FileHeader.FormType == "CMDL" && this.FileHeader.VersionA >= 0x35;

        /// <summary>
        /// The meta data header for parsing gpu buffers and decompressing.
        /// </summary>
        SMetaData Meta;

        public CMDL() { }

        public CMDL(System.IO.Stream stream) : base(stream)
        {
        }

        public override void ReadMetaData(FileReader reader)
        {
            Meta = new SMetaData();
            Meta.Unknown = reader.ReadUInt32();
            Meta.GPUOffset = reader.ReadUInt32();
            Meta.ReadBufferInfo = IOFileExtension.ReadList<SReadBufferInfo>(reader);
            Meta.VertexBuffers = IOFileExtension.ReadList<SBufferInfo>(reader);
            Meta.IndexBuffers = IOFileExtension.ReadList<SBufferInfo>(reader);
            Console.WriteLine();
        }

        public override void WriteMetaData(FileWriter writer)
        {
            writer.Write(Meta.GPUOffset);
            writer.Write(Meta.Unknown);
            IOFileExtension.WriteList(writer, Meta.ReadBufferInfo);
            IOFileExtension.WriteList(writer, Meta.VertexBuffers);
            IOFileExtension.WriteList(writer, Meta.IndexBuffers);
        }

        public override void ReadChunk(FileReader reader, CChunkDescriptor chunk)
        {
            switch (chunk.ChunkType)
            {
                case "CMDL":
                    break;
                case "WMDL":
                    break;
                case "SMDL":
                    reader.ReadUInt32(); //unk
                    break;
                case "HEAD":
                    reader.ReadStruct<SModelHeader>();
                    break;
                case "MTRL":
                    if (IsSwitch)
                        ReadMaterials(reader);
                    else
                        ReadMaterialsU(reader);
                    break;
                case "MESH":
                    ReadMesh(reader);
                    break;
                case "VBUF":
                    ReadVertexBuffer(reader);
                    break;
                case "IBUF":
                    ReadIndexBuffer(reader);
                    break;
                case "GPU ":

                    long startPos = reader.Position;
                    for (int i = 0; i < Meta.IndexBuffers.Count; i++)
                    {
                        var buffer = Meta.IndexBuffers[i];
                        //First buffer or specific buffer
                        var info = Meta.ReadBufferInfo[(int)buffer.ReadBufferIndex];
                        //Seek into the buffer region
                        reader.SeekBegin(info.Offset + buffer.Offset);

                        //Decompress
                        var data = IOFileExtension.DecompressedBuffer(reader, buffer.CompressedSize, buffer.DecompressedSize, IsSwitch);
                      //  if (buffer.DecompressedSize != data.Length)
                      //      throw new Exception();

                        //All indices
                        var indices = BufferHelper.LoadIndexBuffer(data, this.IndexBuffer[i].IndexType, IsSwitch);

                        //Read
                        foreach (var mesh in Meshes)
                        {
                            if (mesh.Header.IndexBufferIndex == i)
                            {
                                //Select indices to use
                                mesh.Indices = indices.Skip((int)mesh.Header.IndexStart).Take((int)mesh.Header.IndexCount).ToArray();
                            }
                        }
                    }
                    for (int i = 0; i < Meta.VertexBuffers.Count; i++)
                    {
                        var vertexInfo = VertexBuffers[i];

                        var buffer = Meta.VertexBuffers[i];
                        //First buffer or specific buffer
                        var info = Meta.ReadBufferInfo[(int)buffer.ReadBufferIndex];
                        //Seek into the buffer region
                        reader.SeekBegin(info.Offset + buffer.Offset);

                        using (reader.TemporarySeek(reader.Position, System.IO.SeekOrigin.Begin))
                        {
                            reader.SetByteOrder(false);
                            var type = reader.ReadUInt32();
                            reader.SetByteOrder(true);

                            if (type != 13)
                            {
                                byte[] b = reader.ReadBytes((int)buffer.CompressedSize - 4);
                                System.IO.File.WriteAllBytes($"{Toolbox.Library.Runtime.ExecutableDir}\\VBuffer_{type}_{i}_{buffer.DecompressedSize}.bin", b);
                            }
                        }

                        //Decompress
                        var data = IOFileExtension.DecompressedBuffer(reader, buffer.CompressedSize, buffer.DecompressedSize, IsSwitch);
                        if (buffer.DecompressedSize != data.Length)
                            throw new Exception();

                        var vertices = BufferHelper.LoadVertexBuffer(data, vertexInfo,IsSwitch);

                        //Read
                        foreach (var mesh in Meshes)
                        {
                            if (mesh.Header.VertexBufferIndex == i)
                            {
                                //Only use the vertices referenced in the indices
                                //Some meshes use the same big buffer and can add too many unecessary vertices
                                mesh.SetupVertices(vertices);
                            }
                        }
                        startPos += buffer.CompressedSize;
                    }
                    break;
            }
        }

        private void ReadMaterialsU(FileReader reader)
        {
            uint numMaterials = reader.ReadUInt32();
            for (int i = 0; i < numMaterials; i++)
            {
                CMaterial material = new CMaterial();
                Materials.Add(material);
                material.Name = reader.ReadZeroTerminatedString();
                material.ID = reader.ReadStruct<CObjectId>();
                material.Type = reader.ReadStruct<Magic>();
                material.Flags = reader.ReadUInt32();
                uint numData = reader.ReadUInt32();

                //Actual data type data
                for (int j = 0; j < numData; j++)
                {
                    var dtype = reader.ReadStruct<Magic>();
                    uint dformat = reader.ReadUInt32();

                    Console.WriteLine($"dtype {dtype} {dformat}");

                    switch (dformat)
                    {
                        case 0: //Texture
                            material.Textures.Add(dtype, reader.ReadStruct<CMaterialTextureTokenData>());
                            break;
                        case 1: //Color
                            material.Colors.Add(dtype, reader.ReadStruct<Color4f>());
                            break;
                        case 2: //Scaler
                            material.Scalars.Add(dtype, reader.ReadSingle());
                            break;
                        case 3: //int
                            material.Int.Add(dtype, reader.ReadInt32());
                            break;
                        case 4: //CLayeredTextureData
                            {
                                reader.ReadUInt32();
                                reader.ReadSingles(4); //color
                                reader.ReadSingles(4); //color
                                reader.ReadSingles(4); //color
                                reader.ReadByte(); //Flags
                                var texture1 = reader.ReadStruct<CObjectId>();
                                if (!texture1.IsZero())
                                    reader.ReadStruct<STextureUsageInfo>();
                                var texture2 = reader.ReadStruct<CObjectId>();
                                if (!texture2.IsZero())
                                    reader.ReadStruct<STextureUsageInfo>();
                                var texture3 = reader.ReadStruct<CObjectId>();
                                if (!texture3.IsZero())
                                    reader.ReadStruct<STextureUsageInfo>();
                            }
                            break;
                        case 5: //int4
                            material.Int4.Add(dtype, reader.ReadInt32s(4));
                            break;
                        default:
                            throw new Exception($"Unsupported material type {dformat}!");
                    }
                }
            }
        }

        private void ReadMaterials(FileReader reader)
        {
            uint numMaterials = reader.ReadUInt32();
            for (int i = 0; i < numMaterials; i++)
            {
                CMaterial material = new CMaterial();
                Materials.Add(material);

                uint size = reader.ReadUInt32();
                material.Name = reader.ReadString((int)size, true);
                material.ID = reader.ReadStruct<CObjectId>();
                material.Type = reader.ReadStruct<Magic>();
                material.Flags = reader.ReadUInt32();
                uint numData = reader.ReadUInt32();

                //A list of data types
                for (int j = 0; j < numData; j++)
                {
                    var dtype = reader.ReadStruct<Magic>();
                    var dformat = reader.ReadStruct<Magic>();
                }

                //Actual data type data
                for (int j = 0; j < numData; j++)
                {
                    var dtype = reader.ReadStruct<Magic>();
                    var dformat = reader.ReadStruct<Magic>();

                    Console.WriteLine($"dtype {dtype} {dformat}");

                    switch (dformat)
                    {
                        case "TXTR": //Texture
                            material.Textures.Add(dtype, reader.ReadStruct<CMaterialTextureTokenData>());
                            break;
                        case "COLR": //Color
                            material.Colors.Add(dtype, reader.ReadStruct<Color4f>());
                            break;
                        case "SCLR": //Scaler
                            material.Scalars.Add(dtype, reader.ReadSingle());
                            break;
                        case "INT ": //int
                            material.Int.Add(dtype, reader.ReadInt32());
                            break;
                        case "INT4": //int4
                            material.Int4.Add(dtype, reader.ReadInt32s(4));
                            break;
                        case "CPLX": //CLayeredTextureData
                            {
                                reader.ReadUInt32();
                                reader.ReadSingles(4); //color
                                reader.ReadSingles(4); //color
                                reader.ReadSingles(4); //color
                                reader.ReadByte(); //Flags
                                var texture1 = reader.ReadStruct<CObjectId>();
                                if (!texture1.IsZero())
                                    reader.ReadStruct<STextureUsageInfo>();
                                var texture2 = reader.ReadStruct<CObjectId>();
                                if (!texture2.IsZero())
                                    reader.ReadStruct<STextureUsageInfo>();
                                var texture3 = reader.ReadStruct<CObjectId>();
                                if (!texture3.IsZero())
                                    reader.ReadStruct<STextureUsageInfo>();
                            }
                            break;
                        default:
                            throw new Exception($"Unsupported material type {dformat}!");
                    }
                }
            }
        }

        private void ReadMesh(FileReader reader)
        {
            uint numMeshes = reader.ReadUInt32();
            for (int i = 0; i < numMeshes; i++)
                Meshes.Add(new CMesh()
                {
                    Header = reader.ReadStruct<CRenderMesh>(),
                });
        }

        private void ReadVertexBuffer(FileReader reader)
        {
            uint numBuffers = reader.ReadUInt32();
            for (int i = 0; i < numBuffers; i++)
            {
                VertexBuffer vertexBuffer = new VertexBuffer();
                vertexBuffer.VertexCount = reader.ReadUInt32();

                uint numAttributes = reader.ReadUInt32();

                for (int j = 0; j < numAttributes; j++)
                    vertexBuffer.Components.Add(reader.ReadStruct<SVertexDataComponent>());
                VertexBuffers.Add(vertexBuffer);
            }
        }

        private void ReadIndexBuffer(FileReader reader)
        {
            uint numBuffers = reader.ReadUInt32();
            for (int i = 0; i < numBuffers; i++)
                IndexBuffer.Add(reader.ReadStruct<CGraphicsIndexBufferToken>());
        }

        public class VertexBuffer
        {
            public List<SVertexDataComponent> Components = new List<SVertexDataComponent>();

            public uint VertexCount;
        }

        public class CVertex
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TexCoord0;
            public Vector2 TexCoord1;
            public Vector2 TexCoord2;

            public Vector4 BoneWeights = new Vector4(1, 0, 0, 0);
            public Vector4 BoneIndices = new Vector4(0);

            public Vector4 Color = Vector4.One;

            public Vector4 Tangent;
        }

        public class CMaterial
        {
            public string Name { get; set; }
            public string Type { get; set; }

            public CObjectId ID { get; set; }

            public uint Flags { get; set; }

            public Dictionary<string, CMaterialTextureTokenData> Textures = new Dictionary<string, CMaterialTextureTokenData>();

            public Dictionary<string, float> Scalars = new Dictionary<string, float>();
            public Dictionary<string, int> Int = new Dictionary<string, int>();
            public Dictionary<string, int[]> Int4 = new Dictionary<string, int[]>();

            public Dictionary<string, Color4f> Colors = new Dictionary<string, Color4f>();
        }

        public class CMesh
        {
            public CRenderMesh Header;

            public List<CVertex> Vertices = new List<CVertex>();

            public uint[] Indices;

            public void SetupVertices(List<CVertex> vertices)
            {
                //Here we optmize the vertices to only use the vertices used by the mesh rather than use one giant list
                List<CVertex> vertexList = new List<CVertex>();
                List<uint> remappedIndices = new List<uint>();
                for (int i = 0; i < Indices.Length; i++)
                {
                    remappedIndices.Add((uint)vertexList.Count);
                    vertexList.Add(vertices[(int)Indices[i]]);
                }
                this.Vertices = vertexList;
                this.Indices = remappedIndices.ToArray();
            }
        }

        public class SSkinnedModelHeader : CChunkDescriptor
        {
            public uint Unknown;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class CRenderMesh
        {
            public PrimtiiveType PrimtiiveMode;
            public ushort MaterialIndex;
            public byte VertexBufferIndex;
            public byte IndexBufferIndex;
            public uint IndexStart;
            public uint IndexCount;
            public ushort field_10;
            public byte field_12;
            public byte field_13;
            public byte field_14;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class CMaterialTextureTokenData
        {
            public CObjectId FileID;
            public STextureUsageInfo UsageInfo;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class STextureUsageInfo
        {
            public uint Flags;
            public uint TextureFilter;
            public uint TextureWrapX;
            public uint TextureWrapY;
            public uint TextureWrapZ;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class SModelHeader 
        {
            public uint NumOpaqueMeshes;
            public uint Num1PassTranslucentMeshes;
            public uint Num2PassTranslucentMeshes;
            public uint Num1BitMeshes;
            public uint NumAdditiveMeshes;
            public CAABox BoundingBox;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class CGraphicsIndexBufferToken
        {
            public IndexFormat IndexType;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class SVertexDataComponent
        {
            public uint field_0;
            public uint Offset;
            public uint Stride;
            public VertexFormat Format;
            public EVertexComponent Type;
        }

        public enum IndexFormat
        {
            Uint16 = 1,
            Uint32 = 2,
        }

        public enum PrimtiiveType
        {
            Triangles = 3,
        }

        public enum VertexFormat
        {
            Byte = 0,
            Format_16_16_HalfSingle = 20,
            Format_8_8_8_8_Uint = 22,
            Format_16_16_16_HalfSingle = 34,
            Format_32_32_32_Single = 37,
        }

        public enum EVertexComponent
        {
            in_position,
            in_normal,
            in_tangent0,
            in_tangent1,
            in_texCoord0,
            in_texCoord1,
            in_texCoord2,
            in_texCoord3,
            in_color,
            in_boneIndices,
            in_boneWeights,
            in_bakedLightingCoord,
            in_bakedLightingTangent,
            in_vertInstanceColor,
            //3x4 matrices
            in_vertTransform0,
            in_vertTransform1,
            in_vertTransform2,
            //3x4 matrices for instancing
            in_vertTransformIT0,
            in_vertTransformIT1,
            in_vertTransformIT2,
            in_lastPosition,
            in_currentPosition,
        }

        //Meta data from PAK archive

        public class SMetaData
        {
            public uint Unknown;
            public uint GPUOffset;
            public List<SReadBufferInfo> ReadBufferInfo = new List<SReadBufferInfo>();
            public List<SBufferInfo> VertexBuffers = new List<SBufferInfo>();
            public List<SBufferInfo> IndexBuffers = new List<SBufferInfo>();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class SReadBufferInfo
        {
            public uint Size;
            public uint Offset;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class SBufferInfo
        {
            public uint ReadBufferIndex;
            public uint Offset;
            public uint CompressedSize;
            public uint DecompressedSize;
        }
    }
}
