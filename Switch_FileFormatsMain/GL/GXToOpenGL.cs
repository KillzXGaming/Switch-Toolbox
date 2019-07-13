using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using SuperBMDLib.Materials;
using SuperBMDLib.Materials.Enums;

namespace FirstPlugin
{
    //From https://github.com/LordNed/JStudio/blob/b9c4eabb1c7e80a8da7f63f8d5003df704de369c/JStudio/GXToOpenGL.cs
    public static class GXToOpenGL
    {
        public static TextureWrapMode GetWrapMode(BinaryTextureImage.WrapModes fromMode)
        {
            switch (fromMode)
            {
                case BinaryTextureImage.WrapModes.ClampToEdge: return TextureWrapMode.ClampToEdge;
                case BinaryTextureImage.WrapModes.Repeat: return TextureWrapMode.Repeat;
                case BinaryTextureImage.WrapModes.MirroredRepeat: return TextureWrapMode.MirroredRepeat;
            }

            return TextureWrapMode.Repeat;
        }

        public static TextureMinFilter GetMinFilter(BinaryTextureImage.FilterMode fromMode)
        {
            switch (fromMode)
            {
                case BinaryTextureImage.FilterMode.Nearest: return TextureMinFilter.Nearest;
                case BinaryTextureImage.FilterMode.Linear: return TextureMinFilter.Linear;
                case BinaryTextureImage.FilterMode.NearestMipmapNearest: return TextureMinFilter.NearestMipmapNearest;
                case BinaryTextureImage.FilterMode.NearestMipmapLinear: return TextureMinFilter.NearestMipmapLinear;
                case BinaryTextureImage.FilterMode.LinearMipmapNearest: return TextureMinFilter.LinearMipmapNearest;
                case BinaryTextureImage.FilterMode.LinearMipmapLinear: return TextureMinFilter.LinearMipmapLinear;
            }

            return TextureMinFilter.Nearest;
        }

        public static TextureMagFilter GetMagFilter(BinaryTextureImage.FilterMode fromMode)
        {
            switch (fromMode)
            {
                case BinaryTextureImage.FilterMode.Nearest: return TextureMagFilter.Nearest;
                case BinaryTextureImage.FilterMode.Linear: return TextureMagFilter.Linear;
            }

            return TextureMagFilter.Nearest;
        }

        public static void SetBlendState(SuperBMDLib.Materials.BlendMode blendMode)
        {
            if (blendMode.Type == SuperBMDLib.Materials.Enums.BlendMode.Blend)
            {
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(GetBlendFactorSrc(blendMode.SourceFact), GetBlendFactorDest(blendMode.DestinationFact));
            }
            else if (blendMode.Type == SuperBMDLib.Materials.Enums.BlendMode.None)
            {
                GL.Disable(EnableCap.Blend);
            }
            else
            {
                // Logic, Subtract?
            }
        }

        public static BlendingFactor GetBlendFactorSrc(BlendModeControl sourceFactor)
        {
            switch (sourceFactor)
            {
                case BlendModeControl.Zero: return BlendingFactor.Zero;
                case BlendModeControl.One: return BlendingFactor.One;
                case BlendModeControl.SrcColor: return BlendingFactor.SrcColor;
                case BlendModeControl.InverseSrcColor: return BlendingFactor.OneMinusSrcColor;
                case BlendModeControl.SrcAlpha: return BlendingFactor.SrcAlpha;
                case BlendModeControl.InverseSrcAlpha: return BlendingFactor.OneMinusSrcAlpha;
                case BlendModeControl.DstAlpha: return BlendingFactor.DstAlpha;
                case BlendModeControl.InverseDstAlpha: return BlendingFactor.OneMinusDstAlpha;
                default:
                    Console.WriteLine("Unsupported GXBlendModeControl: \"{0}\" in GetOpenGLBlendSrc!", sourceFactor);
                    return BlendingFactor.SrcAlpha;

            }
        }

        public static BlendingFactor GetBlendFactorDest(BlendModeControl destinationFactor)
        {
            switch (destinationFactor)
            {
                case BlendModeControl.Zero: return BlendingFactor.Zero;
                case BlendModeControl.One: return BlendingFactor.One;
                case BlendModeControl.SrcColor: return BlendingFactor.SrcColor;
                case BlendModeControl.InverseSrcColor: return BlendingFactor.OneMinusSrcColor;
                case BlendModeControl.SrcAlpha: return BlendingFactor.SrcAlpha;
                case BlendModeControl.InverseSrcAlpha: return BlendingFactor.OneMinusSrcAlpha;
                case BlendModeControl.DstAlpha: return BlendingFactor.DstAlpha;
                case BlendModeControl.InverseDstAlpha: return BlendingFactor.OneMinusDstAlpha;
                default:
                    Console.WriteLine("Unsupported GXBlendModeControl: \"{0}\" in GetOpenGLBlendDest!", destinationFactor);
                    return BlendingFactor.OneMinusSrcAlpha;
            }
        }

        public static void SetCullState(CullMode cullState)
        {
            GL.Enable(EnableCap.CullFace);
            switch (cullState)
            {
                case CullMode.None: GL.Disable(EnableCap.CullFace); break;
                case CullMode.Front: GL.CullFace(CullFaceMode.Front); break;
                case CullMode.Back: GL.CullFace(CullFaceMode.Back); break;
                case CullMode.All: GL.CullFace(CullFaceMode.FrontAndBack); break;
            }
        }

        public static void SetDepthState(ZMode depthState, bool bDepthOnlyPrePass)
        {
            if (depthState.Enable || bDepthOnlyPrePass)
            {
                GL.Enable(EnableCap.DepthTest);
                GL.DepthMask(depthState.UpdateEnable || bDepthOnlyPrePass);
                switch (depthState.Function)
                {
                    case CompareType.Never: GL.DepthFunc(DepthFunction.Never); break;
                    case CompareType.Less: GL.DepthFunc(DepthFunction.Less); break;
                    case CompareType.Equal: GL.DepthFunc(DepthFunction.Equal); break;
                    case CompareType.LEqual: GL.DepthFunc(DepthFunction.Lequal); break;
                    case CompareType.Greater: GL.DepthFunc(DepthFunction.Greater); break;
                    case CompareType.NEqual: GL.DepthFunc(DepthFunction.Notequal); break;
                    case CompareType.GEqual: GL.DepthFunc(DepthFunction.Gequal); break;
                    case CompareType.Always: GL.DepthFunc(DepthFunction.Always); break;
                    default: Console.WriteLine("Unsupported GXCompareType: \"{0}\" in GetOpenGLDepthFunc!", depthState.Function); break;
                }
            }
            else
            {
                GL.Disable(EnableCap.DepthTest);
                GL.DepthMask(false);
            }
        }

        public static void SetDitherEnabled(bool ditherEnabled)
        {
            if (ditherEnabled)
                GL.Enable(EnableCap.Dither);
            else
                GL.Disable(EnableCap.Dither);
        }
    }
}
