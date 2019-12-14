using FirstPlugin.GFMDLStructs;
using FlatBuffers;
using System;
using System.Collections.Generic;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Rendering;
using System.Linq;

namespace FirstPlugin
{
    public class GFLXMeshBufferHelper
    {
        public static byte[] CreateVertexDataBuffer(GFLXMesh mesh)
        {
            var meshData = mesh.MeshData;

            //Get all the bones from model and create skin index list
            List<int> SkinningIndices = new List<int>();
            foreach (GFLXBone bone in mesh.ParentModel.Skeleton.bones)
            {
                if (bone.HasSkinning)
                    SkinningIndices.Add(mesh.ParentModel.Skeleton.bones.IndexOf(bone));
            }

            return CreateVertexDataBuffer(mesh, SkinningIndices, mesh.MeshData.Attributes);
        }

        public static byte[] CreateVertexDataBuffer(STGenericObject mesh, List<int> SkinningIndices, IList<MeshAttribute> attributes)
        {
            var stride = GetTotalBufferStride(attributes);

            var mem = new System.IO.MemoryStream();
            using (var writer = new FileWriter(mem))
            {
                //Generate a buffer based on the attributes used
                for (int v = 0; v < mesh.vertices.Count; v++)
                {
                    writer.SeekBegin(v * stride);

                    for (int a = 0; a < attributes.Count; a++)
                    {
                        uint numAttributes = attributes[a].ElementCount;

                        List<float> values = new List<float>();
                        switch ((VertexType)attributes[a].VertexType)
                        {
                            case VertexType.Position:
                                values.Add(mesh.vertices[v].pos.X);
                                values.Add(mesh.vertices[v].pos.Y);
                                values.Add(mesh.vertices[v].pos.Z);
                                break;
                            case VertexType.Normal:
                                values.Add(mesh.vertices[v].nrm.X);
                                values.Add(mesh.vertices[v].nrm.Y);
                                values.Add(mesh.vertices[v].nrm.Z);
                                values.Add(mesh.vertices[v].normalW);
                                //  values.Add(((GFLXMesh.GFLXVertex)mesh.vertices[v]).NormalW);
                                break;
                            case VertexType.Color1:
                                values.Add(mesh.vertices[v].col.X * 255);
                                values.Add(mesh.vertices[v].col.Y * 255);
                                values.Add(mesh.vertices[v].col.Z * 255);
                                values.Add(mesh.vertices[v].col.W * 255);
                                break;
                            case VertexType.Color2:
                                values.Add(mesh.vertices[v].col2.X * 255);
                                values.Add(mesh.vertices[v].col2.Y * 255);
                                values.Add(mesh.vertices[v].col2.Z * 255);
                                values.Add(mesh.vertices[v].col2.W * 255);
                                break;
                            case VertexType.UV1:
                                values.Add(mesh.vertices[v].uv0.X);
                                values.Add(1 - mesh.vertices[v].uv0.Y);
                                break;
                            case VertexType.UV2:
                                values.Add(mesh.vertices[v].uv1.X);
                                values.Add(1 - mesh.vertices[v].uv1.Y);
                                break;
                            case VertexType.UV3:
                                values.Add(mesh.vertices[v].uv2.X);
                                values.Add(1 - mesh.vertices[v].uv2.Y);
                                break;
                            case VertexType.UV4:
                                values.Add(mesh.vertices[v].uv3.X);
                                values.Add(1 - mesh.vertices[v].uv3.Y);
                                break;
                            case VertexType.BoneWeight:
                                var weights = mesh.vertices[v].boneWeights;
                                for (int b = 0; b < numAttributes; b++)
                                {
                                    if (weights.Count > b)
                                        values.Add(weights[b]);
                                    else
                                        values.Add(0);
                                }
                                break;
                            case VertexType.BoneID:
                                var boneIds = mesh.vertices[v].boneIds;
                                for (int b = 0; b < numAttributes; b++)
                                {
                                    if (boneIds.Count > b)
                                        values.Add(SkinningIndices.IndexOf(boneIds[b]));
                                    else
                                        values.Add(0);
                                }
                                break;
                            case VertexType.Binormal:
                                values.Add(mesh.vertices[v].bitan.X);
                                values.Add(mesh.vertices[v].bitan.Y);
                                values.Add(mesh.vertices[v].bitan.Z);
                                values.Add(mesh.vertices[v].bitan.W);
                                break;
                            case VertexType.Tangents:
                                values.Add(mesh.vertices[v].tan.X);
                                values.Add(mesh.vertices[v].tan.Y);
                                values.Add(mesh.vertices[v].tan.Z);
                                values.Add(mesh.vertices[v].tan.W);
                                break;
                            default:
                                throw new Exception("unsupported format!");
                        }


                        WriteBuffer(writer, numAttributes, values.ToArray(),
                            (BufferFormat)attributes[a].BufferFormat, (VertexType)attributes[a].VertexType);
                    }
                }
            }

            return mem.ToArray();
        }

        public static uint GetTotalBufferStride(IList<MeshAttribute> attributes)
        {
            uint VertBufferStride = 0;
            for (int i = 0; i < attributes?.Count; i++)
            {
                var attribute = attributes[i];
                switch ((VertexType)attribute.VertexType)
                {
                    case VertexType.Position:
                        if (attribute.BufferFormat == (uint)BufferFormat.HalfFloat)
                            VertBufferStride += 0x08;
                        else if (attribute.BufferFormat == (uint)BufferFormat.Float)
                            VertBufferStride += 0x0C;
                        else
                            throw new Exception($"Unknown Combination! {attribute.VertexType} {attribute.BufferFormat}");
                        break;
                    case VertexType.Normal:
                        if (attribute.BufferFormat == (uint)BufferFormat.HalfFloat)
                            VertBufferStride += 0x08;
                        else if (attribute.BufferFormat == (uint)BufferFormat.Float)
                            VertBufferStride += 0x0C;
                        else
                            throw new Exception($"Unknown Combination! {attribute.VertexType} {attribute.BufferFormat}");
                        break;
                    case VertexType.Binormal:
                        if (attribute.BufferFormat == (uint)BufferFormat.HalfFloat)
                            VertBufferStride += 0x08;
                        else if (attribute.BufferFormat == (uint)BufferFormat.Float)
                            VertBufferStride += 0x0C;
                        else
                            throw new Exception($"Unknown Combination! {attribute.VertexType} {attribute.BufferFormat}");
                        break;
                    case VertexType.UV1:
                    case VertexType.UV2:
                    case VertexType.UV3:
                    case VertexType.UV4:
                        if (attribute.BufferFormat == (uint)BufferFormat.HalfFloat)
                            VertBufferStride += 0x04;
                        else if (attribute.BufferFormat == (uint)BufferFormat.Float)
                            VertBufferStride += 0x08;
                        else
                            throw new Exception($"Unknown Combination! {attribute.VertexType} {attribute.BufferFormat}");
                        break;
                    case VertexType.Color1:
                    case VertexType.Color2:
                        if (attribute.BufferFormat == (uint)BufferFormat.Byte)
                            VertBufferStride += 0x04;
                        else
                            throw new Exception($"Unknown Combination! {attribute.VertexType} {attribute.BufferFormat}");
                        break;
                    case VertexType.BoneID:
                        if (attribute.BufferFormat == (uint)BufferFormat.Short)
                            VertBufferStride += 0x08;
                        else if (attribute.BufferFormat == (uint)BufferFormat.Byte)
                            VertBufferStride += 0x04;
                        else
                            throw new Exception($"Unknown Combination! {attribute.VertexType} {attribute.BufferFormat}");
                        break;
                    case VertexType.BoneWeight:
                        if (attribute.BufferFormat == (uint)BufferFormat.BytesAsFloat)
                            VertBufferStride += 0x04;
                        else
                            throw new Exception($"Unknown Combination! {attribute.VertexType} {attribute.BufferFormat}");
                        break;
                    case VertexType.Tangents:
                        if (attribute.BufferFormat == (uint)BufferFormat.HalfFloat)
                            VertBufferStride += 0x08;
                        else
                            throw new Exception($"Unknown Combination! {attribute.VertexType} {attribute.BufferFormat}");
                        break;
                    default:
                        throw new Exception($"Unknown Combination! {attribute.VertexType} {attribute.BufferFormat}");
                }
            }

            return VertBufferStride;
        }

        public static List<Vertex> LoadVertexData(Mesh mesh, OpenTK.Matrix4 transform, List<int> boneSkinningIds)
        {
            List<Vertex> Vertices = new List<Vertex>();

            uint VertBufferStride = GetTotalBufferStride(mesh.Attributes);

            using (var reader = new FileReader(mesh.Data.ToArray()))
            {
                uint numVertex = (uint)reader.BaseStream.Length / VertBufferStride;
                for (int v = 0; v < numVertex; v++)
                {
                    Vertex vertex = new Vertex();
                    Vertices.Add(vertex);

                    for (int att = 0; att < mesh.Attributes?.Count; att++)
                    {
                        var attribute = mesh.Attributes[att];

                        switch ((VertexType)attribute.VertexType)
                        {
                            case VertexType.Position:
                                var pos = ParseBuffer(reader, attribute.BufferFormat, attribute.VertexType, attribute.ElementCount);
                                vertex.pos = new OpenTK.Vector3(pos.X, pos.Y, pos.Z);
                                vertex.pos = OpenTK.Vector3.TransformPosition(vertex.pos, transform);
                                break;
                            case VertexType.Normal:
                                var normal = ParseBuffer(reader, attribute.BufferFormat, attribute.VertexType, attribute.ElementCount);
                                vertex.nrm = new OpenTK.Vector3(normal.X, normal.Y, normal.Z);
                                vertex.normalW = normal.W;
                                vertex.nrm = OpenTK.Vector3.TransformNormal(vertex.nrm, transform);
                                break;
                            case VertexType.UV1:
                                var texcoord1 = ParseBuffer(reader, attribute.BufferFormat, attribute.VertexType, attribute.ElementCount);
                                vertex.uv0 = new OpenTK.Vector2(texcoord1.X, texcoord1.Y);
                                break;
                            case VertexType.UV2:
                                var texcoord2 = ParseBuffer(reader, attribute.BufferFormat, attribute.VertexType, attribute.ElementCount);
                                vertex.uv1 = new OpenTK.Vector2(texcoord2.X, texcoord2.Y);
                                break;
                            case VertexType.UV3:
                                var texcoord3 = ParseBuffer(reader, attribute.BufferFormat, attribute.VertexType, attribute.ElementCount);
                                vertex.uv2 = new OpenTK.Vector2(texcoord3.X, texcoord3.Y);
                                break;
                            case VertexType.UV4:
                                var texcoord4 = ParseBuffer(reader, attribute.BufferFormat, attribute.VertexType, attribute.ElementCount);
                                vertex.uv3 = new OpenTK.Vector2(texcoord4.X, texcoord4.Y);
                                break;
                            case VertexType.BoneWeight:
                                var weights = ParseBuffer(reader, attribute.BufferFormat, attribute.VertexType, attribute.ElementCount);
                                vertex.boneWeights.AddRange(new float[]
                                {
                                    weights.X,
                                    weights.Y,
                                    weights.Z,
                                    weights.W
                                });
                                break;
                            case VertexType.BoneID:
                                var boneIndices = ParseBuffer(reader, attribute.BufferFormat, attribute.VertexType, attribute.ElementCount);
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
                                if (attribute.BufferFormat == (uint)BufferFormat.Byte)
                                    colors1 = ParseBuffer(reader, attribute.BufferFormat, attribute.VertexType, attribute.ElementCount);

                                vertex.col = colors1 / 255f;
                                break;
                            case VertexType.Color2:
                                OpenTK.Vector4 colors2 = new OpenTK.Vector4(1, 1, 1, 1);
                                if (attribute.BufferFormat == (uint)BufferFormat.Byte)
                                    colors2 = ParseBuffer(reader, attribute.BufferFormat, attribute.VertexType, attribute.ElementCount);

                                vertex.col2 = colors2 / 255f;
                                break;
                            case VertexType.Binormal:
                                var binormals = ParseBuffer(reader, attribute.BufferFormat, attribute.VertexType, attribute.ElementCount);
                                vertex.bitan = new OpenTK.Vector4(binormals.X, binormals.Y, binormals.Z, binormals.W);
                                break;
                            case VertexType.Tangents:
                                var tans = ParseBuffer(reader, attribute.BufferFormat, attribute.VertexType, attribute.ElementCount);
                                vertex.tan = new OpenTK.Vector4(tans.X, tans.Y, tans.Z, tans.W);
                                break;
                            default:
                                var values = ParseBuffer(reader, attribute.BufferFormat, attribute.VertexType, attribute.ElementCount);
                                vertex.Unknowns.Add(values);
                                break;
                        }
                    }
                }
            }

            return Vertices;
        }

        private static OpenTK.Vector4 ParseBuffer(FileReader reader, uint format, uint attType, uint numElements)
        {
            VertexType AttributeType = (VertexType)attType;
            BufferFormat Format = (BufferFormat)format;

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
                        float[] values = new float[4];
                        for (int i = 0; i < numElements; i++)
                            values[i] = reader.ReadHalfSingle();

                        return new OpenTK.Vector4(values[0], values[1], values[2], values[3]);
                    default:
                        throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                }
            }
        }


        private static void WriteBuffer(FileWriter writer, uint elementCount,  float[] value, BufferFormat Format, VertexType AttributeType)
        {
            for (int i = 0; i < elementCount; i++)
            {
                switch (Format)
                {
                    case BufferFormat.Float:
                        writer.Write(value[i]); break;
                    case BufferFormat.HalfFloat:
                        writer.WriteHalfFloat(value[i]); break;
                    case BufferFormat.Byte:
                        writer.Write((byte)value[i]); break;
                    case BufferFormat.Short:
                        writer.Write((short)value[i]); break;
                    case BufferFormat.BytesAsFloat:
                        writer.Write((byte)(value[i] * 255)); break;
                    default:
                        throw new Exception($"Unknown Combination! {AttributeType} {Format}");
                }
            }
        }
    }
}
