using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirstPlugin.GFMDLStructs;

namespace FirstPlugin
{
    public class GfbmdlImportSettings
    {
        public bool OptmizeZeroWeights { get; set; } = true;

        public bool FlipUVsVertical { get; set; }

        public bool ResetUVTransform { get; set; }

        public List<MeshSetting> MeshSettings = new List<MeshSetting>();

        public class MeshSetting
        {
            public bool FlipUVsVertical { get; set; }

            public bool HasNormals { get; set; }
            public bool HasTangents { get; set; }
            public bool HasBitangents { get; set; }
            public bool HasTexCoord1 { get; set; }
            public bool HasTexCoord2 { get; set; }
            public bool HasTexCoord3 { get; set; }
            public bool HasTexCoord4 { get; set; }
            public bool HasColor1 { get; set; }
            public bool HasColor2 { get; set; }
            public bool HasColor3 { get; set; }
            public bool HasColor4 { get; set; }
            public bool HasBoneIndices { get; set; }
            public bool HasWeights { get; set; }

            public bool SetNormalsToColorChannel2 { get; set; }

            public BufferFormat PositionFormat { get; set; } = BufferFormat.Float;
            public BufferFormat NormalFormat { get; set; } = BufferFormat.HalfFloat;
            public BufferFormat TangentsFormat { get; set; } = BufferFormat.HalfFloat;
            public BufferFormat TexCoord1Format { get; set; } = BufferFormat.Float;
            public BufferFormat TexCoord2Format { get; set; } = BufferFormat.Float;
            public BufferFormat TexCoord3Format { get; set; } = BufferFormat.Float;
            public BufferFormat TexCoord4Format { get; set; } = BufferFormat.Float;
            public BufferFormat Color1Format { get; set; } = BufferFormat.Byte;
            public BufferFormat Color2Format { get; set; } = BufferFormat.Byte;
            public BufferFormat Color3Format { get; set; } = BufferFormat.Byte;
            public BufferFormat Color4Format { get; set; } = BufferFormat.Byte;
            public BufferFormat BoneIndexFormat { get; set; } = BufferFormat.Byte;
            public BufferFormat BoneWeightFormat { get; set; } = BufferFormat.BytesAsFloat;
            public BufferFormat BitangentnFormat { get; set; } = BufferFormat.HalfFloat;

            public string Material { get; set; }

            public string MaterialFile { get; set; }
        }
    }
}
