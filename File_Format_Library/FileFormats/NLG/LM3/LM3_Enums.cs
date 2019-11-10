using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPlugin.LuigisMansion3
{
    public enum DataType : ushort
    {
        Texture = 0xB500,
        Model   = 0xB000,
        Unknown = 0x7000,
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

    public enum SubDataType2 : uint
    {
        TextureHeader = 0x38C1B501,
        TextureData = 0x5241B502,

        ModelStart = 0x1201B006,
        SubmeshInfo           = 0x1201B003, //Or polygon groups?
        VertexStartPointers   = 0x1201B004,
        ModelTransform        = 0x1301B001, //Matrix4x4. 0x40 in size
        MeshBuffers           = 0x1301B005, //vertex and index buffer
        MaterialName          = 0x1201B333,
        MeshIndexTable        = 0x1201B007,
        MessageData           = 0x12027020,
        ShaderData            = 0x1401B400,
        UILayoutMagic         = 0x92027000,
        UILayoutHeader        = 0x12027001,
        UILayoutData          = 0x12027002, //Without header
        UILayout              = 0x02027003, //All parts combined
    }

    public enum SubDataType : ushort
    {
        TextureHeader = 0xB501,
        TextureData   = 0xB502,
        MaterailData    = 0xB006, //Also marks start of model data
        SubmeshInfo   = 0xB003, //Or polygon groups?
        VertexStartPointers = 0xB004,
        ModelTransform = 0xB001, //Matrix4x4. 0x40 in size
        ModelInfo = 0xB002, //Contains mesh count and model hash
        MeshBuffers = 0xB005, //vertex and index buffer
        MaterialName = 0xB333,
        MeshIndexTable = 0xB007,
        MessageData = 0x7020,
        ShaderData = 0xB400,
        UILayoutMagic = 0x7000,
        UILayoutHeader = 0x7001,
        UILayoutData = 0x7002, //Without header
        UILayout = 0x7003, //All parts combined

        HavokPhysics = 0xC900,

        BoneData = 0xB102,
        BoneHashList = 0xB103,
    }
}
