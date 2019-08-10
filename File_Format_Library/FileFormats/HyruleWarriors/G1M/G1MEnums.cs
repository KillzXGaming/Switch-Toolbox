using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyruleWarriors.G1M
{
    public enum VertexAttriubte
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
}
