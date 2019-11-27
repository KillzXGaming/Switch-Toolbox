using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using OpenTK;
using FlatBuffers.Gfbmdl;
using Toolbox.Library.Rendering;
using Toolbox.Library;

namespace FirstPlugin
{
    public class GFLXMeshBufferHelper
    {
        public static void ReloadVertexData(GFLXMesh mesh)
        {
            var flatMesh = mesh.MeshData;
            var buf = flatMesh.ByteBuffer;
            for (int v = 0; v < mesh.vertices.Count; v++)
            {
               // flatMesh.ByteBuffer.PutByte();
               // flatMesh.Data(v);
            }
        }

        public static uint GetTotalBufferStride(Mesh mesh)
        {
            uint VertBufferStride = 0;
            for (int i = 0; i < mesh.AlignmentsLength; i++)
            {
                var attribute = mesh.Alignments(i).Value;
                switch (attribute.TypeID)
                {
                    case VertexType.Position:
                        if (attribute.FormatID == BufferFormat.HalfFloat)
                            VertBufferStride += 0x08;
                        else if (attribute.FormatID == BufferFormat.Float)
                            VertBufferStride += 0x0C;
                        else
                            throw new Exception($"Unknown Combination! {attribute.TypeID} {attribute.FormatID}");
                        break;
                    case VertexType.Normal:
                        if (attribute.FormatID == BufferFormat.HalfFloat)
                            VertBufferStride += 0x08;
                        else if (attribute.FormatID == BufferFormat.Float)
                            VertBufferStride += 0x0C;
                        else
                            throw new Exception($"Unknown Combination! {attribute.TypeID} {attribute.FormatID}");
                        break;
                    case VertexType.Binormal:
                        if (attribute.FormatID == BufferFormat.HalfFloat)
                            VertBufferStride += 0x08;
                        else if (attribute.FormatID == BufferFormat.Float)
                            VertBufferStride += 0x0C;
                        else
                            throw new Exception($"Unknown Combination! {attribute.TypeID} {attribute.FormatID}");
                        break;
                    case VertexType.UV1:
                    case VertexType.UV2:
                    case VertexType.UV3:
                    case VertexType.UV4:
                        if (attribute.FormatID == BufferFormat.HalfFloat)
                            VertBufferStride += 0x04;
                        else if (attribute.FormatID == BufferFormat.Float)
                            VertBufferStride += 0x08;
                        else
                            throw new Exception($"Unknown Combination! {attribute.TypeID} {attribute.FormatID}");
                        break;
                    case VertexType.Color1:
                    case VertexType.Color2:
                        if (attribute.FormatID == BufferFormat.Byte)
                            VertBufferStride += 0x04;
                        else
                            throw new Exception($"Unknown Combination! {attribute.TypeID} {attribute.FormatID}");
                        break;
                    case VertexType.BoneID:
                        if (attribute.FormatID == BufferFormat.Short)
                            VertBufferStride += 0x08;
                        else if (attribute.FormatID == BufferFormat.Byte)
                            VertBufferStride += 0x04;
                        else
                            throw new Exception($"Unknown Combination! {attribute.TypeID} {attribute.FormatID}");
                        break;
                    case VertexType.BoneWeight:
                        if (attribute.FormatID == BufferFormat.BytesAsFloat)
                            VertBufferStride += 0x04;
                        else
                            throw new Exception($"Unknown Combination! {attribute.TypeID} {attribute.FormatID}");
                        break;
                    default:
                        Console.WriteLine($"attribute.FormatID {attribute.FormatID}");
                        if (attribute.FormatID == BufferFormat.HalfFloat)
                            VertBufferStride += 0x08;
                        else if (attribute.FormatID == BufferFormat.Float)
                            VertBufferStride += 0x0C;
                        else
                            throw new Exception($"Unknown Combination! {attribute.TypeID} {attribute.FormatID}");
                        break;
                }
            }

            return VertBufferStride;
        }

        public static List<Vertex> LoadVertexData(Mesh mesh, OpenTK.Matrix4 transform, List<int> boneSkinningIds)
        {
            List<Vertex> Vertices = new List<Vertex>();

            uint VertBufferStride = GetTotalBufferStride(mesh);

            using (var reader = new FileReader(mesh.GetDataArray()))
            {
                uint numVertex = (uint)reader.BaseStream.Length / VertBufferStride;
                for (int v = 0; v < numVertex; v++)
                {
                    Vertex vertex = new Vertex();
                    Vertices.Add(vertex);

                    for (int att = 0; att < mesh.AlignmentsLength; att++)
                    {
                        var attribute = mesh.Alignments(att).Value;

                        switch (attribute.TypeID)
                        {
                            case VertexType.Position:
                                var pos = ParseBuffer(reader, attribute.FormatID, attribute.TypeID);
                                vertex.pos = new OpenTK.Vector3(pos.X, pos.Y, pos.Z);
                                vertex.pos = OpenTK.Vector3.TransformPosition(vertex.pos, transform);
                                break;
                            case VertexType.Normal:
                                var normal = ParseBuffer(reader, attribute.FormatID, attribute.TypeID);
                                vertex.nrm = new OpenTK.Vector3(normal.X, normal.Y, normal.Z);
                                vertex.nrm = OpenTK.Vector3.TransformNormal(vertex.nrm, transform);
                                break;
                            case VertexType.UV1:
                                var texcoord1 = ParseBuffer(reader, attribute.FormatID, attribute.TypeID);
                                vertex.uv0 = new OpenTK.Vector2(texcoord1.X, texcoord1.Y);
                                break;
                            case VertexType.UV2:
                                var texcoord2 = ParseBuffer(reader, attribute.FormatID, attribute.TypeID);
                                vertex.uv1 = new OpenTK.Vector2(texcoord2.X, texcoord2.Y);
                                break;
                            case VertexType.UV3:
                                var texcoord3 = ParseBuffer(reader, attribute.FormatID, attribute.TypeID);
                                vertex.uv2 = new OpenTK.Vector2(texcoord3.X, texcoord3.Y);
                                break;
                            case VertexType.UV4:
                                var texcoord4 = ParseBuffer(reader, attribute.FormatID, attribute.TypeID);
                                break;
                            case VertexType.BoneWeight:
                                var weights = ParseBuffer(reader, attribute.FormatID, attribute.TypeID);
                                vertex.boneWeights.AddRange(new float[]
                                {
                                    weights.X,
                                    weights.Y,
                                    weights.Z,
                                    weights.W
                                });
                                break;
                            case VertexType.BoneID:
                                var boneIndices = ParseBuffer(reader, attribute.FormatID, attribute.TypeID);
                                vertex.boneIds.AddRange(new int[]
                                {
                                    boneSkinningIds[(int)boneIndices.X],
                                    boneSkinningIds[(int)boneIndices.Y],
                                    boneSkinningIds[(int)boneIndices.Z],
                                    boneSkinningIds[(int)boneIndices.W],
                                });
                                break;
                            case VertexType.Color1:
                                OpenTK.Vector4 colors1 = new OpenTK.Vector4(1, 1, 1, 1);
                                if (attribute.FormatID == BufferFormat.Byte)
                                    colors1 = ParseBuffer(reader, attribute.FormatID, attribute.TypeID);

                                vertex.col = colors1 / 255f;
                                break;
                            case VertexType.Color2:
                                OpenTK.Vector4 colors2 = new OpenTK.Vector4(1, 1, 1, 1);
                                if (attribute.FormatID == BufferFormat.Byte)
                                    colors2 = ParseBuffer(reader, attribute.FormatID, attribute.TypeID);

                                break;
                            case VertexType.Binormal:
                                var binormals = ParseBuffer(reader, attribute.FormatID, attribute.TypeID);

                                vertex.bitan = new OpenTK.Vector4(binormals.X, binormals.Y, binormals.Z, binormals.W);
                                break;
                            default:
                                ParseBuffer(reader, attribute.FormatID, attribute.TypeID);
                                break;
                        }
                    }
                }
            }

            return Vertices;
        }

        private static OpenTK.Vector4 ParseBuffer(FileReader reader, BufferFormat Format, VertexType AttributeType)
        {
            if (AttributeType == VertexType.Position)
            {
                switch (Format)
                {
                    case BufferFormat.Float:
                        return new OpenTK.Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0);
                    case BufferFormat.HalfFloat:
                        return new OpenTK.Vector4(reader.ReadHalfSingle(), reader.ReadHalfSingle(),
                                           reader.ReadHalfSingle(), reader.ReadHalfSingle());
                    default:
                        throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                }
            }
            else if (AttributeType == VertexType.Normal)
            {
                switch (Format)
                {
                    case BufferFormat.Float:
                        return new OpenTK.Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0);
                    case BufferFormat.HalfFloat:
                        return new OpenTK.Vector4(reader.ReadHalfSingle(), reader.ReadHalfSingle(),
                                           reader.ReadHalfSingle(), reader.ReadHalfSingle());
                    default:
                        throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                }
            }
            else if (AttributeType == VertexType.Binormal)
            {
                switch (Format)
                {
                    case BufferFormat.Float:
                        return new OpenTK.Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), 0);
                    case BufferFormat.HalfFloat:
                        return new OpenTK.Vector4(reader.ReadHalfSingle(), reader.ReadHalfSingle(),
                                           reader.ReadHalfSingle(), reader.ReadHalfSingle());
                    default:
                        throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                }
            }
            else if (AttributeType == VertexType.UV1 ||
                     AttributeType == VertexType.UV2 ||
                     AttributeType == VertexType.UV3 ||
                     AttributeType == VertexType.UV4)
            {
                switch (Format)
                {
                    case BufferFormat.Float:
                        return new OpenTK.Vector4(reader.ReadSingle(), reader.ReadSingle(), 0, 0);
                    case BufferFormat.HalfFloat:
                        return new OpenTK.Vector4(reader.ReadHalfSingle(), reader.ReadHalfSingle(), 0, 0);
                    default:
                        throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                }
            }
            else if (AttributeType == VertexType.Color1 ||
                     AttributeType == VertexType.Color2)
            {
                switch (Format)
                {
                    case BufferFormat.Byte:
                        return new OpenTK.Vector4(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                    default:
                        throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                }
            }
            else if (AttributeType == VertexType.BoneID)
            {
                switch (Format)
                {
                    case BufferFormat.Short:
                        return new OpenTK.Vector4(reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16(), reader.ReadInt16());
                    case BufferFormat.Byte:
                        return new OpenTK.Vector4(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                    default:
                        throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                }
            }
            else if (AttributeType == VertexType.BoneWeight)
            {
                switch (Format)
                {
                    case BufferFormat.BytesAsFloat:
                        return new OpenTK.Vector4(reader.ReadByteAsFloat(), reader.ReadByteAsFloat(),
                                           reader.ReadByteAsFloat(), reader.ReadByteAsFloat());
                    default:
                        throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                }
            }
            else
            {
                switch (Format)
                {
                    case BufferFormat.HalfFloat:
                        return new OpenTK.Vector4(reader.ReadHalfSingle(), reader.ReadHalfSingle(),
                                           reader.ReadHalfSingle(), reader.ReadHalfSingle());
                    default:
                        throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                }
            }
        }
    }
}
