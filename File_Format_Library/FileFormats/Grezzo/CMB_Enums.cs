using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grezzo.CmbEnums
{
    //All enums from
    //https://github.com/magcius/noclip.website/blob/master/src/oot3d/cmb.ts
    public enum TextureFilter
    {
        NEAREST = 0x2600,
        LINEAR = 0x2601,
        NEAREST_MIPMAP_NEAREST = 0x2700,
        LINEAR_MIPMAP_NEAREST = 0x2701,
        NEAREST_MIPMAP_LINEAR = 0x2702,
        LINEAR_MIPMAP_LINEAR = 0x2703,
    }

    public enum SepdVertexAttribMode : ushort
    {
        ARRAY = 0,
        CONSTANT = 1,
    }

    public enum CmbDataType : ushort
    {
        Byte = 0x1400,
        UByte = 0x1401,
        Short = 0x1402,
        UShort = 0x1403,
        Int = 0x1404,
        UInt = 0x1405,
        Float = 0x1406,
    }

    public enum CullMode : byte
    {
        NONE,
        BACK,
        FRONT,
        FRONT_AND_BACK,
    }

    public enum SkinningMode : ushort
    {
        SINGLE_BONE = 0x00,
        RIGID_SKINNING = 0x01,
        SMOOTH_SKINNING = 0x02,
    }

    public enum CMBTextureWrapMode
    {
        CLAMP = 0x2900,
        REPEAT = 0x2901,
        CLAMP_TO_EDGE = 0x812F,
        MIRRORED_REPEAT = 0x8370,
    }

    public enum CombineResultOpDMP : ushort
    {
        REPLACE = 0x1E01,
        MODULATE = 0x2100,
        ADD = 0x0104,
        ADD_SIGNED = 0x8574,
        INTERPOLATE = 0x8575,
        SUBTRACT = 0x84E7,
        DOT3_RGB = 0x86AE,
        DOT3_RGBA = 0x86AF,
        MULT_ADD = 0x6401,
        ADD_MULT = 0x6402,
    };

    public enum CombineScaleDMP : ushort
    {
        _1 = 0x01,
        _2 = 0x02,
        _4 = 0x04,
    };

    public enum CombineBufferInputDMP : ushort
    {
        PREVIOUS = 0x8578,
        PREVIOUS_BUFFER = 0x8579,
    };

    public enum CombineSourceDMP : ushort
    {
        TEXTURE0 = 0x84C0,
        TEXTURE1 = 0x84C1,
        TEXTURE2 = 0x84C2,
        TEXTURE3 = 0x84C3,
        CONSTANT = 0x8576,
        PRIMARY_COLOR = 0x8577,
        PREVIOUS = 0x8578,
        PREVIOUS_BUFFER = 0x8579,
        FRAGMENT_PRIMARY_COLOR = 0x6210,
        FRAGMENT_SECONDARY_COLOR = 0x6211,
    };

    public enum CombineOpDMP : ushort
    {
        SRC_COLOR = 0x0300,
        ONE_MINUS_SRC_COLOR = 0x0301,
        SRC_ALPHA = 0x0302,
        ONE_MINUS_SRC_ALPHA = 0x0303,
        SRC_R = 0x8580,
        SRC_G = 0x8581,
        SRC_B = 0x8582,
        ONE_MINUS_SRC_R = 0x8583,
        ONE_MINUS_SRC_G = 0x8584,
        ONE_MINUS_SRC_B = 0x8585,
    };

    public enum FresnelSelector : ushort
    {
        No = 25280,
        Pri = 25281,
        Sec = 25282,
        PriSec = 25283
    };

    public enum LayerConfig
    {
        LayerConfig0 = 25264,
        LayerConfig1 = 25265,
        LayerConfig2 = 25266,
        LayerConfig3 = 25267,
        LayerConfig4 = 25268,
        LayerConfig5 = 25269,
        LayerConfig6 = 25270,
        LayerConfig7 = 25271,
    };

    public enum LUTInput : ushort
    {
        CosNormalHalf = 25248,
        CosViewHalf = 25249,
        CosNormalView = 25250,
        CosLightNormal = 25251,
        CosLightSpot = 25252,
        CosPhi = 25253
    }

    public enum StencilOp
    {
        Keep = 7680,
        Zero = 0,
        Replace = 7681,
        Increment = 7682,
        Decrement = 7683,
        Invert = 5386,
        IncrementWrap = 34055,
        DecrementWrap = 34055
    }

    public enum BumpMode
    {
        NotUsed = 25288,
        AsBump = 25289,
        AsTangent = 25290//Doesn't exist in OoT3D
    }
}
