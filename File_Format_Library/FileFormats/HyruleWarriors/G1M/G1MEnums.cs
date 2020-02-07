using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyruleWarriors.G1M
{
    public enum VertexAttriubte : byte
    {
        Position,
        Weights,
        BoneIndices,
        Normals,
        Unknown,
        TexCoord0,
        Tangent,
        Bitangent,
        Color = 0x0A,
        Fog = 0x0B,
    }

    public enum FaceType : uint
    {
        Byte = 0x8,
        UInt16 = 0x10,
        UInt32 = 0x20,
    }
}
