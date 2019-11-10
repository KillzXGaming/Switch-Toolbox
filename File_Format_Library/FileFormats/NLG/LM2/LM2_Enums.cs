using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPlugin.LuigisMansion.DarkMoon
{
    public enum DataType : ushort
    {
        Texture = 0xB500,
        Model = 0xB000,
    }

    public enum VertexDataFormat
    {
        Float16,
        Float32,
        Float32_32,
        Float32_32_32,
    }

    public enum IndexFormat : ushort
    {
        Index_16 = 0x0,
        Index_8 = 0x8000,
    }

    public enum SubDataType : ushort
    {
        MaterialData          = 0xB006,
        ModelData             = 0xB002,
        SubmeshInfo           = 0xB003, //Or polygon groups?
        VertexStartPointers   = 0xB004,
        ModelTransform        = 0xB001, //Matrix4x4. 0x40 in size
        MeshBuffers           = 0xB005, //vertex and index buffer
        BoneData              = 0xB102,
        BoneHashes            = 0xB103,
        MaterialName          = 0xB333,
        MeshIndexTable        = 0xB007,
        MessageData           = 0x7020,
        ShaderData            = 0xB400,
        TextureHeader         = 0xB501,
        TextureData           = 0xB502,
        UILayoutMagic         = 0x7000,
        UILayoutHeader        = 0x7001,
        UILayoutData          = 0x7002, //Without header
        UILayout              = 0x7003, //All parts combined
    }
}
