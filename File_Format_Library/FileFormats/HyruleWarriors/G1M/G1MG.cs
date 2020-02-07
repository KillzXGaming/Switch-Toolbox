using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library;
using Toolbox.Library.Rendering;
using OpenTK;
using System.Runtime.InteropServices;

namespace HyruleWarriors.G1M
{
    public class G1MG : G1MChunkCommon
    {
        private string Type { get; set; }

        public BoundingBox BoundingBox;

        public Material[] Materials { get; set; }
        public ShaderData[] Shaders { get; set; }
        public VertexBuffer[] VertexBuffers { get; set; }
        public VertexAttributeList[] Attributes { get; set; }
        public JointMapInfo[] JointInfos { get; set; }
        public IndexBuffer[] IndexBuffers { get; set; }
        public SubMeshInfo[] SubMeshInfos { get; set; }
        public LodGroup[] LodGroups { get; set; }

        public List<GenericRenderedObject> GenericMeshes = new List<GenericRenderedObject>();

        public G1MG(FileReader reader)
        {
            Type = reader.ReadString(3, Encoding.ASCII);
            reader.ReadByte();//padding
            reader.ReadSingle(); //unk
            BoundingBox = new BoundingBox()
            {
                Min = reader.ReadVec3(),
                Max = reader.ReadVec3(),
            };

            uint numChunks = reader.ReadUInt32();
            for (int i = 0; i < numChunks; i++)
            {
                long pos = reader.Position;
                uint chunkMagic = reader.ReadUInt32();
                uint chunkSize = reader.ReadUInt32();
                switch (chunkMagic)
                {
                    case 0x00010001: //Unknown
                        break;
                    case 0x00010002: //Materials
                        Materials = G1MCommon.ParseArray<Material>(reader);
                        break;
                    case 0x00010003: //Shaders
                        Shaders = G1MCommon.ParseArray<ShaderData>(reader);
                        break;
                    case 0x00010004: //Vertex Buffer
                        VertexBuffers = G1MCommon.ParseArray<VertexBuffer>(reader);
                        break;
                    case 0x00010005: //Specs
                        Attributes = G1MCommon.ParseArray<VertexAttributeList>(reader);
                        break;
                    case 0x00010006: //Joint map info
                        JointInfos = G1MCommon.ParseArray<JointMapInfo>(reader);
                        break;
                    case 0x00010007: //Index buffer
                        IndexBuffers = G1MCommon.ParseArray<IndexBuffer>(reader);
                        break;
                    case 0x00010008: //Sub mesh info
                        SubMeshInfos = G1MCommon.ParseArray<SubMeshInfo>(reader);
                        break;
                    case 0x00010009: //Level of detail info
                        LodGroups = G1MCommon.ParseArray<LodGroup>(reader);
                        break;
                    default:
                        Console.WriteLine("Unknown chunk! " + chunkMagic.ToString());
                        break;
                }

                reader.SeekBegin(pos + chunkSize);
            }

            ToGenericMeshes(reader.IsBigEndian);
        }

        private void ToGenericMeshes(bool isBigEndian)
        {
            foreach (SubMeshInfo meshInfo in SubMeshInfos) {
                GenericRenderedObject genericMesh = new GenericRenderedObject();
                genericMesh.Text = $"Mesh_{GenericMeshes.Count}";
                GenericMeshes.Add(genericMesh);

                STGenericPolygonGroup polyGroup = new STGenericPolygonGroup();
                genericMesh.PolygonGroups.Add(polyGroup);

                var mat = Materials[(int)meshInfo.TextureID];
                mat.Diffuse.TextureIndex = mat.TextureIndices[0][0];
                polyGroup.Material = mat;

                Console.WriteLine($"TextureID {meshInfo.TextureID}");
                Console.WriteLine($"MaterialID {meshInfo.MaterialID}");

                //Set face type
                if (meshInfo.IndexBufferFormat == 3)
                    polyGroup.PrimativeType = STPrimitiveType.Triangles;
                else if (meshInfo.IndexBufferFormat == 4)
                    polyGroup.PrimativeType = STPrimitiveType.TrangleStrips;

                //Get faces
                var buffer = IndexBuffers[(int)meshInfo.IndexBufferID];
                var indcies = buffer.GetIndices(isBigEndian,
                    meshInfo.IndexBufferOffset,
                    meshInfo.IndexBufferCount).ToArray();

                for (int f = 0; f < indcies.Length; f++)
                    polyGroup.faces.Add((int)indcies[f]);

                //Get vertices
                genericMesh.vertices.AddRange(GetVertices((int)meshInfo.VertexBufferID,
                    (int)meshInfo.IndexIntoJointMap, meshInfo.VertexBufferOffset, isBigEndian));
            }
        }

        public List<Vertex> GetVertices(int index, int jointMapIndex, uint offset,  bool isBigEndian)
        {
            var buffer = VertexBuffers[index];
            var attributeList = Attributes[index];

            List<Vertex> vertices = new List<Vertex>();

            var skinningList = JointInfos[jointMapIndex];
            using (var dataReader = new FileReader(buffer.BufferData))
            {
                dataReader.SetByteOrder(isBigEndian);
                dataReader.SeekBegin(offset);
                for (int i = 0; i < buffer.ElementCount; i++)
                {
                    Vertex vertex = new Vertex();
                    vertices.Add(vertex);
                    foreach (var att in attributeList.Attributes)
                    {
                        dataReader.SeekBegin((uint)(i * buffer.Stride + att.Offset));
                        switch (att.AttributeType)
                        {
                            case VertexAttriubte.Position:
                                if (att.DataType == 0x02 || att.DataType == 0x03 ||
                                    att.DataType == 0x0200 || att.DataType == 0x0300)
                                {
                                    vertex.pos = new Vector3(
                                        dataReader.ReadSingle(),
                                        dataReader.ReadSingle(),
                                        dataReader.ReadSingle());
                                    dataReader.ReadSingle(); //extra
                                }
                                else if (att.DataType == 0x000B || att.DataType == 0x000B)
                                {
                                    vertex.pos = new Vector3(
                                         dataReader.ReadHalfSingle(),
                                         dataReader.ReadHalfSingle(),
                                         dataReader.ReadHalfSingle());
                                    dataReader.ReadHalfSingle(); //extra
                                }
                                break;
                            case VertexAttriubte.Normals:
                                if (att.DataType == 0x000B || att.DataType  == 0x000B)
                                {
                                    vertex.nrm = new Vector3(
                                         dataReader.ReadHalfSingle(),
                                         dataReader.ReadHalfSingle(),
                                         dataReader.ReadHalfSingle());
                                    dataReader.ReadHalfSingle(); //extra
                                }
                                else if (att.DataType == 0x02 || att.DataType == 0x03 ||
                                         att.DataType == 0x0200 || att.DataType == 0x0300)
                                {
                                    vertex.nrm = new Vector3(
                                        dataReader.ReadSingle(),
                                        dataReader.ReadSingle(),
                                        dataReader.ReadSingle());
                                    dataReader.ReadSingle(); //extra
                                }
                                break;
                            case VertexAttriubte.TexCoord0:
                                if (att.DataType == 0x0001 || att.DataType == 0x0100)
                                {
                                    vertex.uv0 = new Vector2(
                                         dataReader.ReadSingle(),
                                         dataReader.ReadSingle());
                                }
                                else if (att.DataType == 0x000A || att.DataType == 0x0A00)
                                {
                                    vertex.uv0 = new Vector2(
                                         dataReader.ReadHalfSingle(),
                                         dataReader.ReadHalfSingle());
                                }
                                break;
                            case VertexAttriubte.Color:
                                if (att.Layer == 0)
                                {
                                     if (att.DataType == 0x0200 || att.DataType == 0x0300)
                                    {
                                        vertex.col = new Vector4(
                                            dataReader.ReadHalfSingle(),
                                            dataReader.ReadHalfSingle(),
                                            dataReader.ReadHalfSingle(),
                                            1.0f);
                                    }
                                    else if (att.DataType == 0x0003 || att.DataType == 0x0300)
                                    {
                                        vertex.col = new Vector4(
                                            dataReader.ReadSingle(),
                                            dataReader.ReadSingle(),
                                            dataReader.ReadSingle(),
                                            dataReader.ReadSingle());
                                    }
                                    else if (att.DataType == 0x000B || att.DataType == 0x0B00)
                                    {
                                        vertex.col = new Vector4(
                                            dataReader.ReadHalfSingle(),
                                            dataReader.ReadHalfSingle(),
                                            dataReader.ReadHalfSingle(),
                                            dataReader.ReadHalfSingle());
                                    }
                                    else if (att.DataType == 0x000D || att.DataType == 0x0D00)
                                    {
                                        vertex.col = new Vector4(
                                            dataReader.ReadByte() / 255f,
                                            dataReader.ReadByte() / 255f,
                                            dataReader.ReadByte() / 255f,
                                            dataReader.ReadByte() / 255f);
                                    }
                                }
                                else //Cloth layer
                                {

                                }
                                break;
                            case VertexAttriubte.Weights:
                                if (att.DataType == 0x0000)
                                {
                                    vertex.boneWeights.Add(dataReader.ReadSingle());
                                    float value2 = 1.0f - vertex.boneWeights[0];
                                    vertex.boneWeights.Add(value2);
                                }
                                else if (att.DataType == 0x0001 || att.DataType == 0x0100)
                                {
                                    vertex.boneWeights.Add(dataReader.ReadSingle());
                                    vertex.boneWeights.Add(dataReader.ReadSingle());
                                    float z = 1.0f - vertex.boneWeights[0] - vertex.boneWeights[1];
                                    vertex.boneWeights.Add(z);
                                }
                                else if (att.DataType == 0x0002 || att.DataType == 0x0200)
                                {
                                    for (int j = 0; j < 3; j++)
                                        vertex.boneWeights.Add(dataReader.ReadSingle());
                                    var value4 = 1 - vertex.boneWeights[0] - vertex.boneWeights[1] - vertex.boneWeights[2];

                                    vertex.boneWeights.Add(value4);
                                }
                                else if (att.DataType == 0x0003 || att.DataType == 0x0300)
                                {
                                    for (int j = 0; j < 4; j++)
                                        vertex.boneWeights.Add(dataReader.ReadSingle());
                                }
                                else if (att.DataType == 0x000A || att.DataType == 0x0A00)
                                {
                                    vertex.boneWeights.Add(dataReader.ReadHalfSingle());
                                    vertex.boneWeights.Add(dataReader.ReadHalfSingle());
                                    float z = 1.0f - vertex.boneWeights[0] - vertex.boneWeights[1];
                                    vertex.boneWeights.Add(z);
                                }
                                else if (att.DataType == 0x000B || att.DataType == 0x0B00)
                                {
                                    for (int j = 0; j < 4; j++)
                                        vertex.boneWeights.Add(dataReader.ReadHalfSingle());
                                }
                                else if (att.DataType == 0x000D || att.DataType == 0x0D00)
                                {
                                    for (int j = 0; j < 4; j++)
                                        vertex.boneWeights.Add(dataReader.ReadByte() / 255f);
                                }
                                break;
                            case VertexAttriubte.BoneIndices:
                                if (att.DataType == 0x0005 || att.DataType == 0x0500)
                                {
                                    for (int j = 0; j < 4; j++)
                                        vertex.boneIds.Add(dataReader.ReadByte()); 
                                }
                                else if (att.DataType == 0x0007 || att.DataType == 0x0700)
                                {
                                    for (int j = 0; j < 4; j++)
                                        vertex.boneIds.Add(dataReader.ReadUInt16());
                                }
                                else if (att.DataType == 0x000D || att.DataType == 0x0D00)
                                {
                                    for (int j = 0; j < 4; j++)
                                        vertex.boneIds.Add(dataReader.ReadByte());
                                }
                                break;
                        }
                    }

                    if (vertex.boneWeights.All(x => x == 0))
                    {
                        vertex.boneIds.Clear();
                        vertex.boneWeights.Clear();

                        vertex.boneIds.Add((int)skinningList.JointIndices[0]);
                        vertex.boneWeights.Add(1);
                    }
                    else if (vertex.boneWeights.Count > 0)
                    {
                        for (int j = 0; j < vertex.boneIds.Count; j++)
                        {
                            int id = vertex.boneIds[j] / 3;
                            if (skinningList.JointIndices.Length > id)
                                id = (int)skinningList.JointIndices[id];
                            vertex.boneIds[j] = id;
                        }
                    }
                }
            }
            return vertices;
        }
    }

    public struct BoundingBox
    {
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }
    }

    public class Material : STGenericMaterial, IChunk
    {
        public uint Unknown1;
        public uint Unknown2;
        public uint Unknown3;

        public List<ushort[]> TextureIndices;

        public G1MTextureMap Diffuse;

        public void Read(FileReader reader) {
            Unknown1 = reader.ReadUInt32(); //0
            uint numTextures = reader.ReadUInt32();
            Unknown2 = reader.ReadUInt32(); //0, 1, -1
            Unknown3 = reader.ReadUInt32(); //0, 1, -1

            TextureIndices = new List<ushort[]>();
            for (int i = 0; i < numTextures; i++)
                TextureIndices.Add(reader.ReadUInt16s(6));

            Diffuse = new G1MTextureMap();
            Diffuse.Type = STGenericMatTexture.TextureType.Diffuse;
            TextureMaps.Add(Diffuse);
        }

        public void Write(FileWriter writer) {
            writer.Write(Unknown1);
            writer.Write(TextureIndices.Count);
            writer.Write(Unknown2);
            writer.Write(Unknown3);
            foreach (var indices in TextureIndices)
                writer.Write(indices);
        }
    }

    public class G1MTextureMap : STGenericMatTexture
    {
        public int TextureIndex { get; set; } = -1;
    }

    public class ShaderData : IChunk
    {
        public void Read(FileReader reader)
        {

        }

        public void Write(FileWriter writer)
        {
        }
    }

    public class VertexBuffer : IChunk
    {
        public uint Unknown1 { get; set; }
        public uint Stride { get; set; }
        public uint ElementCount { get; set; }
        public uint Unknown2 { get; set; }

        public byte[] BufferData { get; set; }

        public void Read(FileReader reader) {
            Unknown1 = reader.ReadUInt32();
            Stride = reader.ReadUInt32();
            ElementCount = reader.ReadUInt32();
            Unknown2 = reader.ReadUInt32();
            BufferData = reader.ReadBytes((int)(ElementCount * Stride));
        }

        public void Write(FileWriter writer) {
            writer.Write(Unknown1);
            writer.Write(Stride);
            writer.Write(ElementCount);
            writer.Write(Unknown2);
            writer.Write(BufferData);
        }
    }

    public class VertexAttributeList : IChunk
    {
        public List<VertexAttribute> Attributes = new List<VertexAttribute>();

        public void Read(FileReader reader)
        {
            uint numBuffers = reader.ReadUInt32();
            int[] bufferIndices = reader.ReadInt32s((int)numBuffers);
            uint numAttributes = reader.ReadUInt32();
            for (int i = 0; i < numAttributes; i++)
            {
                var buffer = new VertexAttribute();
                buffer.BufferIndex = bufferIndices[(int)reader.ReadUInt16()];
                buffer.Offset = reader.ReadUInt16();
                byte flag1 = reader.ReadByte();
                byte flag2 = reader.ReadByte();
                buffer.DataType = (ushort)((flag1 << 8) | flag2);
                buffer.AttributeType = (VertexAttriubte)reader.ReadByte();
                buffer.Layer = reader.ReadByte();
                Attributes.Add(buffer);
            }
        }

        public void Write(FileWriter writer)
        {
            writer.Write(Attributes.Count);
        }
    }

    public class VertexAttribute
    {
        public int BufferIndex { get; set; }
        public ushort Offset { get; set; }
        public ushort DataType { get; set; }
        public VertexAttriubte AttributeType { get; set; }
        public byte Layer { get; set; }
    }

    public class JointMapInfo : IChunk
    {
        public uint[] UnkIndices { get; set; }
        public uint[] JointIndices { get; set; }
        public uint[] ClothIndices { get; set; }

        public void Read(FileReader reader)
        {
            uint numJoints = reader.ReadUInt32();

            UnkIndices = new uint[(int)numJoints];
            JointIndices = new uint[(int)numJoints];
            ClothIndices = new uint[(int)numJoints];
            for (int i = 0; i < numJoints; i++) {
                UnkIndices[i] = reader.ReadUInt32();
                ClothIndices[i] = reader.ReadUInt32() & 0xFFFF;
                JointIndices[i] = reader.ReadUInt32() & 0xFFFF;
            }
        }

        public void Write(FileWriter writer)
        {
            writer.Write(JointIndices.Length);
            for (int i = 0; i < JointIndices.Length; i++) {
                writer.Write(UnkIndices[i]);
                writer.Write(ClothIndices[i]);
                writer.Write(JointIndices[i]);
            }
        }
    }

    public class IndexBuffer : IChunk
    {
        public uint ElementCount { get; set; }
        public FaceType DataType { get; set; }
        public uint Unknown1 { get; set; }

        public uint Stride
        {
            get
            {
                if      (DataType == FaceType.Byte) return 1;
                else if (DataType == FaceType.UInt16) return 2;
                else if (DataType == FaceType.UInt32) return 4;
                else
                    throw new Exception("Unknown index stride! " + DataType);
            }
        }

        public byte[] BufferData { get; set; }

        public IEnumerable<uint> GetIndices(bool isBigEndian, uint offset, uint elementCount)
        {
            using (var reader = new FileReader(BufferData))
            {
                reader.SetByteOrder(isBigEndian);
                reader.SeekBegin(offset * Stride);

                switch (DataType)
                {
                    case FaceType.Byte:
                        for (; elementCount > 0; elementCount--)
                        {
                            yield return reader.ReadByte();
                        }
                        break;
                    case FaceType.UInt16:
                        for (; elementCount > 0; elementCount--)
                        {
                            yield return reader.ReadUInt16();
                        }
                        break;
                    case FaceType.UInt32:
                        for (; elementCount > 0; elementCount--)
                        {
                            yield return reader.ReadUInt32();
                        }
                        break;
                    default:
                        throw new Exception("Unknown type! " + DataType);
                }
            }
        }

        public void Read(FileReader reader)
        {
            ElementCount = reader.ReadUInt32();
            DataType = reader.ReadEnum<FaceType>(true);
            Unknown1 = reader.ReadUInt32();
            BufferData = reader.ReadBytes((int)(ElementCount * Stride));
            reader.Align(4);
        }

        public void Write(FileWriter writer)
        {
            writer.Write(ElementCount);
            writer.Write(DataType, true);
            writer.Write(Stride);
            writer.Write(Unknown1);
            writer.Write(BufferData);
            writer.Align(4);
        }
    }

    public class SubMeshInfo : IChunk
    {
        public uint UnknownIndex;
        public uint VertexBufferID;
        public uint IndexIntoJointMap;
        public uint Unknown2;
        public uint TextureCount;
        public uint MaterialID;
        public uint TextureID;
        public uint IndexBufferID;
        public uint Unknown;
        public uint IndexBufferFormat;
        public uint VertexBufferOffset;
        public uint VertexBufferCount;
        public uint IndexBufferOffset;
        public uint IndexBufferCount;

        public void Read(FileReader reader)
        {
            UnknownIndex = reader.ReadUInt32();
            VertexBufferID = reader.ReadUInt32();
            IndexIntoJointMap = reader.ReadUInt32();
            Unknown2 = reader.ReadUInt32();
            TextureCount = reader.ReadUInt32();
            MaterialID = reader.ReadUInt32();
            TextureID = reader.ReadUInt32();
            IndexBufferID = reader.ReadUInt32();
            Unknown = reader.ReadUInt32();
            IndexBufferFormat = reader.ReadUInt32();
            VertexBufferOffset = reader.ReadUInt32();
            VertexBufferCount = reader.ReadUInt32();
            IndexBufferOffset = reader.ReadUInt32();
            IndexBufferCount = reader.ReadUInt32();

        }

        public void Write(FileWriter writer)
        {
            writer.Write(UnknownIndex);
            writer.Write(VertexBufferID);
            writer.Write(IndexIntoJointMap);
            writer.Write(Unknown2);
            writer.Write(TextureCount);
            writer.Write(MaterialID);
            writer.Write(TextureID);
            writer.Write(IndexBufferID);
            writer.Write(Unknown);
            writer.Write(IndexBufferFormat);
            writer.Write(VertexBufferOffset);
            writer.Write(VertexBufferCount);
            writer.Write(IndexBufferOffset);
            writer.Write(IndexBufferCount);
        }
    }

    public class LodGroup : IChunk
    {
        public List<LodMesh> Meshes = new List<LodMesh>();

        public void Read(FileReader reader)
        {
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            uint numMeshes1 = reader.ReadUInt32();
            uint numMeshes2 = reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();

            for (int i = 0; i < numMeshes1 + numMeshes2; i++) {
                LodMesh msh = new LodMesh();
                Meshes.Add(msh);
                msh.Name = reader.ReadString(16, true);
                msh.ID = reader.ReadUInt32();
                msh.ID2 = reader.ReadUInt32();
                uint numIndces = reader.ReadUInt32();
                msh.Indices = reader.ReadUInt32s((int)numIndces);
                if (numIndces == 0)
                    reader.ReadUInt32();
            }
        }

        public void Write(FileWriter writer)
        {

        }
    }

    public class LodMesh
    {
        public string Name { get; set; }
        public uint ID { get; set; }
        public uint ID2 { get; set; }
        public uint[] Indices { get; set; }
    }
}
