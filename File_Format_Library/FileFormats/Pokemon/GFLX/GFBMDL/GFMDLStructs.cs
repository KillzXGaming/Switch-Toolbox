using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FirstPlugin.GFMDLStructs
{
    public enum VertexType
    {
        Position = 0,
        Normal = 1,
        Tangents = 2,
        UV1 = 3,
        UV2 = 4,
        UV3 = 5,
        UV4 = 6,
        Color1 = 7,
        Color2 = 8,
        Color3 = 9,
        Color4 = 10,
        BoneID = 11,
        BoneWeight = 12,
        Bitangent = 13,
        Unknown2 = 14,
    }

    public enum BufferFormat : uint
    {
        Float = 0,
        HalfFloat = 1,
        Byte = 3,
        Short = 5,
        BytesAsFloat = 8,
    }

    public class Model
    {
        public uint Version { get; set; }

        public BoundingBox Bounding { get; set; }

        public IList<string> TextureNames { get; set; }
        public IList<string> ShaderNames { get; set; }
        public IList<UnknownEmpty> Unknown { get; set; }
        public IList<string> MaterialNames { get; set; }
        public IList<Material> Materials { get; set; }
        public IList<Group> Groups { get; set; }
        public IList<Mesh> Meshes { get; set; }
        public IList<Bone> Bones { get; set; }
        public IList<CollisionGroup> CollisionGroups { get; set; }
    }

    public class Material
    {
        public string Name { get; set; }
        public string ShaderGroup { get; set; }

        public int RenderLayer { get; set; }

        public byte Unknown1 { get; set; }
        public byte Unknown2 { get; set; }

        public int Parameter1 { get; set; }
        public int Parameter2 { get; set; }
        public int Parameter3 { get; set; }
        public int ShaderIndex { get; set; }
        public int Parameter4 { get; set; }
        public int Parameter5 { get; set; }

        public IList<TextureMap> TextureMaps { get; set; }

        public IList<MatSwitch> Switches { get; set; }
        public IList<MatFloat> Values { get; set; }
        public IList<MatColor> Colors { get; set; }

        public byte Unknown3 { get; set; }
        public byte Unknown4 { get; set; }
        public byte Unknown5 { get; set; }
        public byte Unknown6 { get; set; }
        public byte Unknown7 { get; set; }

        public MaterialCommon Common { get; set; }

        public void Export(string name)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            System.IO.File.WriteAllText($"mat_{name}.json", json);
        }

        public static Material Replace(string filePath)
        {
            string json = System.IO.File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<Material>(json);
        }
    }

    public class TextureMap
    {
        public string Sampler { get; set; }
        public uint Index { get; set; }
        public TextureParams Params { get; set; }
    }

    public class TextureParams
    {
        public uint Unknown1 { get; set; }
        public uint WrapModeX { get; set; }
        public uint WrapModeY { get; set; }
        public uint WrapModeZ { get; set; }
        public float Unknown5 { get; set; }
        public float Unknown6 { get; set; }
        public float Unknown7 { get; set; }
        public float Unknown8 { get; set; }

        public float lodBias { get; set; }
    }

    public class MatInt
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public class MatFloat
    {
        public string Name { get; set; }
        public float Value { get; set; }
    }

    public class MatColor
    {
        public string Name { get; set; }
        public ColorRGB32 Color { get; set; }
    }

    public class MatSwitch
    {
        public string Name { get; set; }
        public bool Value { get; set; }
    }

    public class MaterialCommon
    {
        public IList<MatSwitch> Switches { get; set; }
        public IList<MatInt> Values { get; set; }
        public IList<MatColor> Colors { get; set; }
    }

    public class UnknownEmpty
    {
        public uint Zero { get; set; }
    }

    public class Group
    {
        public uint BoneIndex { get; set; }
        public uint MeshIndex { get; set; }

        public BoundingBox Bounding { get; set; }

        public uint Layer { get; set; }
    }

    public class Mesh
    {
        public IList<MeshPolygon> Polygons { get; set; }
        public IList<MeshAttribute> Attributes { get; set; }
        public IList<byte> Data { get; set; }

        public void SetData(byte[] data)
        {
            if (Data == null)
                Data = new List<byte>();

            Data.Clear();
            for (int i = 0; i < data.Length; i++)
                Data.Add(data[i]);
        }
    }

    public class MeshPolygon
    {
        public uint MaterialIndex { get; set; }

        public IList<ushort> Faces { get; set; }
    }

    public class MeshAttribute
    {
        public uint VertexType { get; set; }
        public uint BufferFormat { get; set; }
        public uint ElementCount { get; set; }
    }

    public class Bone
    {
        public string Name { get; set; }

        public uint BoneType { get; set; }
        public int Parent { get; set; }
        public uint Zero { get; set; }
        public bool SegmentScale { get; set; }

        public Vector3 Scale { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Translation { get; set; }
        public Vector3 RadiusStart { get; set; }
        public Vector3 RadiusEnd { get; set; }

        public BoneRigidData RigidCheck { get; set; }
    }

    public class BoneRigidData
    {
        public uint Unknown1 { get; set; }
    }

    public class CollisionGroup
    {
        public uint BoneIndex { get; set; }
        public uint Unknown1 { get; set; }

        public IList<uint> BoneChildren { get; set; }
        public BoundingBox Bounding { get; set; }
    }

    public class BoundingBox
    {
        public float MinX { get; set; }
        public float MinY { get; set; }
        public float MinZ { get; set; }

        public float MaxX { get; set; }
        public float MaxY { get; set; }
        public float MaxZ { get; set; }
    }

    public class Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3() { }
        public Vector3(float x, float y, float z) {
            X = x; Y = y; Z = z;
        }
    }

    public class ColorRGB32
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }

        public ColorRGB32() { }
        public ColorRGB32(float r, float g, float b) {
            R = r; G = g; B = b;
        }
    }

    public class Vector4
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public Vector4() { }
        public Vector4(float x, float y, float z, float w) {
            X = x; Y = y; Z = z; W = w;
        }
    }
}
