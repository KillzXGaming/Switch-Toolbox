using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK;
using Toolbox.Library;
using Toolbox.Library.IO;
using FirstPlugin;

namespace CafeLibrary.M2
{
    public class Model
    {
        public ushort Version { get; set; }

        public string[] MaterialNames { get; set; }

        public MaterialList MaterialList { get; set; }

        public List<Mesh> Meshes { get; set; } = new List<Mesh>();

        public List<Bone> Bones { get; set; } = new List<Bone>();

        public List<MT_TEX> Textures { get; set; } = new List<MT_TEX>();

        public List<Bounding> Boundings { get; set; } = new List<Bounding>();

        public Vector3 BoundingCenter { get; set; }
        public float BoundingScale { get; set; }
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }

        public Model(Stream stream)
        {
            using (var reader = new MTFileReader(stream, this)) {
                Read(reader);
            }
        }

        public void LoadMaterials(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            MaterialList = new MaterialList(File.OpenRead(filePath), Version == 211);
        }

        public void LoadTextures(string folder)
        {
            if (MaterialList == null)
                return;

            foreach (var tex in MaterialList.Textures)
            {
                string filePath = Path.Combine(folder, Path.GetFileName(tex.Name) + ".tex");
                if (!File.Exists(filePath))
                    continue;

                MT_TEX texture = new MT_TEX();
                texture.FileName = Path.GetFileNameWithoutExtension(filePath);
                texture.Load(File.OpenRead(filePath));
                Textures.Add(texture);
            }
        }

        public void Read(MTFileReader reader)
        {
            reader.ReadUInt32(); //magic
            Version = reader.ReadUInt16();
            ushort boneCount = reader.ReadUInt16();
            ushort meshCount = reader.ReadUInt16();
            ushort matCount = reader.ReadUInt16();
            uint totalVertexCount = reader.ReadUInt32();
            uint totalIndexCount = reader.ReadUInt32();
            uint totalTrisCount = reader.ReadUInt32();
            uint vertexBufferSize = reader.ReadUInt32();
            reader.ReadUInt32(); //padding
            uint numBoundings = reader.ReadUInt32();
            if (Version == 211 || Version == 230)
                reader.ReadUInt32(); //padding

            uint bonesOffset = reader.ReadOffset();
            uint boundingOffset = reader.ReadOffset();
            uint materialNamesOffset = reader.ReadOffset();
            uint meshListOffset = reader.ReadOffset();
            uint vertexBufferOffset = reader.ReadOffset();
            uint indexBufferOffset = reader.ReadOffset();
            uint unkOffset = reader.ReadOffset();
            BoundingCenter = reader.ReadVec3();
            BoundingScale = reader.ReadSingle();
            Min = reader.ReadVec3();
            reader.ReadSingle();
            Max = reader.ReadVec3();
            reader.ReadSingle();

            reader.ReadUInt32s(5); //unknown

            reader.SeekBegin(materialNamesOffset);

            MaterialNames = new string[matCount];
            for (int i = 0; i < matCount; i++)
                MaterialNames[i] = reader.ReadString(128, true);

            byte[] boneIDs = new byte[boneCount];

            reader.SeekBegin(bonesOffset);
            for (int i = 0; i < boneCount; i++)
            {
                Bone bone = new Bone();

                bone.ID = reader.ReadByte();
                bone.ParentIndex = reader.ReadSByte();
                bone.MirrorIndex = reader.ReadSByte();
                bone.Unk = reader.ReadSByte();
                bone.ChildDistance = reader.ReadSingle();
                bone.ParentDistance = reader.ReadSingle();
                bone.Position = new Vector3(
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle());
                Bones.Add(bone);

                boneIDs[i] = bone.ID;
            }
            //Local space
            for (int i = 0; i < boneCount; i++)
                Bones[i].LocalMatrix = reader.ReadMatrix4();

            //World space
            for (int i = 0; i < boneCount; i++)
                Bones[i].WorldMatrix = reader.ReadMatrix4();

            //Bone ID property to bone index
            byte[] remapTable = Version == 137 ? new byte[0x200] : new byte[0x100];
            remapTable = reader.ReadBytes(remapTable.Length);

            Dictionary<int, int> boneIndices = new Dictionary<int, int>();
            for (int i = 0; i < boneCount; i++)
            {
                if (Bones[i].ID < 255)
                {
                    boneIDs[i] = (byte)remapTable[Bones[i].ID];

                    boneIndices.Add(boneIDs[i], i);

                    Console.WriteLine($"boneIDs {Bones[i].ID} -> {boneIDs[i]}");
                }
            }

            //Boundings after
            reader.SeekBegin(boundingOffset);
            for (int i = 0; i < numBoundings; i++)
            {
                Bounding bnd = new Bounding();
                bnd.ID = reader.ReadUInt32();
                bnd.Boundings = reader.ReadSingles(7);
                Boundings.Add(bnd);
            }

            //Scaling as vertex data is scaled down for optmization
            Vector3 scale = Max - Min;
            if (this.Version >= 190 && Bones.Count > 0)
                scale = Bones[0].WorldMatrix.ExtractScale();

            //Mesh data
            reader.SeekBegin(meshListOffset);
            for (int i = 0; i < meshCount; i++)
            {
                Mesh mesh = new Mesh();
                mesh.Read(reader, this, vertexBufferOffset, indexBufferOffset);
                Meshes.Add(mesh);
            }

            var mat = Matrix4.CreateScale(scale);
            foreach (var mesh in Meshes)
            {
                for (int i = 0; i < mesh.Vertices.Length; i++)
                {
                    //Vertex positions use 16 SNorm format which is optmized to be smaller. Scale and offset by bounding size
                    mesh.Vertices[i].Position = Min + Vector3.TransformPosition(mesh.Vertices[i].Position, mat);

                    var vertex = mesh.Vertices[i];
                    if (vertex.BoneIndices.Count == 0)
                        continue;

                    //Adjustments to single weights
                    if (vertex.BoneIndices.Count > 0 && vertex.BoneWeights.Count == 0)
                        vertex.BoneWeights.Add(1.0f);
                    //Adjustments to non matching weights as these get optmized
                    if (vertex.BoneIndices.Count != vertex.BoneWeights.Count)
                    {
                        var diff = vertex.BoneIndices.Count - vertex.BoneWeights.Count;
                        if (diff > 0)
                        {
                            var totalToFill = 1.0f - vertex.BoneWeights.Sum(x => x);
                            if (totalToFill < 0)
                                throw new Exception();

                            for (int j = 0; j < diff; j++)
                                vertex.BoneWeights.Add(totalToFill / diff);
                        }
                    }

                    for (int j = 0; j < vertex.BoneIndices.Count; j++)
                    {
                        var boneID = (int)vertex.BoneIndices[j];
                        var boneW = vertex.BoneWeights[j];

                        if (boneW == 0)
                            continue;

                        Console.WriteLine($"boneID {boneID} boneW {boneW}");
                    }

                    mesh.Vertices[i] = vertex;
                }
            }
            Console.WriteLine();
        }

        public class Bounding
        {
            public uint ID;

            public float[] Boundings = new float[7];
        }
    }


    public class MTFileReader : FileReader
    {
        private Model Model;

        public MTFileReader(Stream stream, Model model) : base(stream)
        {
            Model = model;
        }

        public uint ReadOffset()
        {
            if (Model.Version == 211)
                return (uint)ReadUInt64();
            else
                return ReadUInt32();
        }
    }
    public class Bone
    {
        public Vector3 Position;

        public Matrix4 WorldMatrix;
        public Matrix4 LocalMatrix;

        public float ParentDistance;
        public float ChildDistance;

        public byte ID;
        public sbyte ParentIndex;
        public sbyte Unk;
        public sbyte MirrorIndex;
    }

    public class Mesh
    {
        public byte Order;

        public uint BoundingBoxID;
        public uint MaterialID;
        public sbyte LOD;

        public byte BoundingStartID;
        public ushort BatchID;

        public ushort[] Indices;

        public uint VertexFormatHash;

        public Vertex[] Vertices = new Vertex[0];

        public class Vertex
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2[] TexCoords = new Vector2[1];
            public Vector4 Color = Vector4.One;
            public List<float> BoneWeights = new List<float>();
            public List<int> BoneIndices = new List<int>();
        }

        public void Read(FileReader reader, Model model, uint vertexBufferOffset, uint indexBufferOffset)
        {
            reader.ReadUInt16(); //flags
            ushort vertexCount = reader.ReadUInt16();
            uint meshFlags = reader.ReadUInt32();
            reader.ReadByte();
            Order = reader.ReadByte();
            byte stride = reader.ReadByte();
            reader.ReadByte();
            uint vertexID = reader.ReadUInt32();
            uint vertexDataOffset = reader.ReadUInt32();
            VertexFormatHash = reader.ReadUInt32();

            uint indexID = reader.ReadUInt32();
            uint indexCount = reader.ReadUInt32();
            uint indexDataOffset = reader.ReadUInt32();
            byte unk = reader.ReadByte();
            BoundingStartID = reader.ReadByte();
            BatchID = reader.ReadUInt16();
            reader.ReadUInt32();
            reader.ReadUInt32();

            BoundingBoxID = (meshFlags >> 0) & 0xfff;
            MaterialID = (meshFlags >> 12) & 0xfff;
            LOD = (sbyte)(meshFlags >> 24);

            if (model.Version == 211)
            {
                reader.ReadUInt32();
                reader.ReadUInt32();
            }

            using (reader.TemporarySeek(vertexBufferOffset + vertexDataOffset + (vertexID * stride), SeekOrigin.Begin)) {
                ParseVertexBuffer(reader, VertexFormatHash, vertexCount, stride);
            }
            using (reader.TemporarySeek(indexBufferOffset + indexDataOffset + (indexID * 2), SeekOrigin.Begin)) {
                if (model.Version == 212 || model.Version == 211)
                    ReadTriStrips(reader, indexCount, vertexID);
                else
                    ReadTris(reader, indexCount, vertexID);
            }
        }

        private void ParseVertexBuffer(FileReader reader, uint formatHash, ushort count, byte stride)
        {
            if (!MT_Globals.AttributeGroups.ContainsKey(formatHash))
            {
               // throw new Exception($"Unsupported attribute layout! {formatHash}");

                return;
            }

            Vertices = new Vertex[count];
            var attributeGroup = MT_Globals.AttributeGroups[formatHash];
            long basePos = reader.Position;

            Console.WriteLine($"ATTRIBUTE {attributeGroup.Name}");

            for (int i = 0; i < count; i++)
            {
                Vertices[i] = new Vertex();

                var maxUVSet = attributeGroup.Attributes.Where(x => x.Name.ToLower() == "texcoord").Sum(x => x.Set);
                Vertices[i].TexCoords = new Vector2[maxUVSet + 1];

                foreach (var att in attributeGroup.Attributes)
                {
                    reader.SeekBegin((uint)basePos + (i * stride) + att.Offset);
                    switch (att.Name.ToLower())
                    {
                        case "position":
                            Vertices[i].Position.X = Parse(reader, att.Format);
                            Vertices[i].Position.Y = Parse(reader, att.Format);
                            if (att.ElementCount > 2)
                                Vertices[i].Position.Z = Parse(reader, att.Format);
                            break;
                        case "normal":
                            Vertices[i].Normal.X = Parse(reader, att.Format);
                            Vertices[i].Normal.Y = Parse(reader, att.Format);
                            Vertices[i].Normal.Z = Parse(reader, att.Format);
                            Vertices[i].Normal = Vertices[i].Normal.Normalized();
                            break;
                        case "texcoord":
                            Vector2 uv = new Vector2();
                            uv.X = Parse(reader, att.Format);
                            uv.Y = Parse(reader, att.Format);
                            Vertices[i].TexCoords[att.Set] = uv;
                            break;
                        case "vertexcolor":
                            Vertices[i].Color.X = Parse(reader, att.Format);
                            Vertices[i].Color.Y = Parse(reader, att.Format);
                            Vertices[i].Color.Z = Parse(reader, att.Format);
                            if (att.ElementCount == 4) //Alpha
                                Vertices[i].Color.W = Parse(reader, att.Format);
                            break;
                        case "vertexalpha":
                            Vertices[i].Color.W = Parse(reader, att.Format);
                            break;
                        case "weight":
                            for (int j = 0; j < att.ElementCount; j++)
                                Vertices[i].BoneWeights.Add(Parse(reader, att.Format));
                            break;
                        case "joint":
                            for (int j = 0; j < att.ElementCount; j++)
                                Vertices[i].BoneIndices.Add((int)Parse(reader, att.Format));
                            break;
                    }
                }
            }
        }

        private float Parse(FileReader reader, Shader.AttributeGroup.AttributeFormat format)
        {
            switch (format)
            {
                case Shader.AttributeGroup.AttributeFormat.Float: return reader.ReadSingle();
                case Shader.AttributeGroup.AttributeFormat.HalfFloat: return reader.ReadHalfSingle();
                case Shader.AttributeGroup.AttributeFormat.UShort: return reader.ReadUInt16(); 
                case Shader.AttributeGroup.AttributeFormat.Short: return reader.ReadInt16();
                case Shader.AttributeGroup.AttributeFormat.S16N: return reader.ReadInt16() * (1f / short.MaxValue);
                case Shader.AttributeGroup.AttributeFormat.U16N: return reader.ReadUInt16() * (1f / ushort.MaxValue);
                case Shader.AttributeGroup.AttributeFormat.Sbyte: return reader.ReadSByte();
                case Shader.AttributeGroup.AttributeFormat.Byte: return reader.ReadByte();
                case Shader.AttributeGroup.AttributeFormat.S8N: return reader.ReadSByte() * (1f / sbyte.MaxValue);
                case Shader.AttributeGroup.AttributeFormat.U8N: return reader.ReadByte() * (1f / byte.MaxValue);
                case (Shader.AttributeGroup.AttributeFormat)11: return (reader.ReadByte() - 127) * 0.0078125f;
                case Shader.AttributeGroup.AttributeFormat.RGB: return reader.ReadByte();
                case Shader.AttributeGroup.AttributeFormat.RGBA: return reader.ReadByte();
            }
            return 0;
        }

        private void ReadTris(FileReader reader, uint indexCount, uint vertexID)
        {
            Indices = new ushort[indexCount / 3];
            for (int Index = 0; Index < Indices.Length; Index++)
                Indices[Index] = (ushort)(reader.ReadInt16() - vertexID); //Make sure index is relative rather than one big buffer
        }

        private void ReadTriStrips(FileReader reader, uint indexCount, uint vertexID)
        {
            List<ushort> indices = new List<ushort>();

            int startDirection = 1;
            //Start with triangle (f1, f2, f3)
            ushort f1 = reader.ReadUInt16();
            ushort f2 = reader.ReadUInt16();
            int faceDirection = startDirection;
            int p = 2;
            ushort f3;
            do
            {
                f3 = reader.ReadUInt16();
                p++;
                if (f3 == ushort.MaxValue) //Value after 0xFFFF is triangle (f1, f2, f3)
                {
                    f1 = reader.ReadUInt16();
                    f2 = reader.ReadUInt16();
                    faceDirection = startDirection;
                    p += 2;
                }
                else
                {
                    //Direction switches each strip
                    faceDirection *= -1;
                    //Skip single points
                    if ((f1 != f2) && (f2 != f3) && (f3 != f1))
                    {
                        if (faceDirection > 0) //Determine strip face orientation
                        {
                            indices.Add(f3);
                            indices.Add(f2);
                            indices.Add(f1);
                        }
                        else
                        {
                            indices.Add(f2);
                            indices.Add(f3);
                            indices.Add(f1);
                        }
                    }
                    //Remap indices for next strip
                    f1 = f2;
                    f2 = f3;
                }
            } while (p < indexCount);
            //Shift indices as indices in .mod index one global vertex table
            for (int i = 0; i < indices.Count; i++)
                indices[i] = (ushort)(indices[i] - vertexID);

            Indices = indices.ToArray();
        }
    }
}
