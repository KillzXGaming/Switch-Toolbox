using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPlugin.LuigisMansion.DarkMoon
{
    public enum DataType : uint
    {
        Texture = 0x8701B500,
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
        Index_8  = 0x8000,
    }

    public enum SubDataType : uint
    {
        ModelStart            = 0x1201B006,
        SubmeshInfo           = 0x1201B003, //Or polygon groups?
        VertexStartPointers   = 0x1201B004,
        ModelTransform        = 0x1301B001, //Matrix4x4. 0x40 in size
        MeshBuffers           = 0x1301B005, //vertex and index buffer
        MaterialName          = 0x1201B333,
        MeshIndexTable        = 0x1201B007,
        MessageData           = 0x12027020,
        ShaderData            = 0x1401B400,
        TextureHeader         = 0x0201B501,
        TextureData           = 0x1701B502,
        UILayoutMagic         = 0x92027000,
        UILayoutHeader        = 0x12027001,
        UILayoutData          = 0x12027002, //Without header
        UILayout              = 0x02027003, //All parts combined
    }
}
