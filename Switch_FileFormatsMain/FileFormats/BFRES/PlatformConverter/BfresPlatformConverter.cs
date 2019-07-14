using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResU = Syroot.NintenTools.Bfres;
using ResNX = Syroot.NintenTools.NSW.Bfres;
using ResGFXBNTX = Syroot.NintenTools.NSW.Bntx.GFX;
using Syroot.NintenTools.NSW.Bntx;

namespace FirstPlugin
{
    public class BfresPlatformConverter
    {
        public static ResNX.ResFile BFRESConvertWiiUToSwitch(ResU.ResFile resFileU)
        {
            ResNX.ResFile resFile = new ResNX.ResFile();

            foreach (var model in resFileU.Models)
            {

            }

            return resFile;
        }

        public static ResU.ResFile BFRESConvertSwitchToWiiU(ResNX.ResFile resFileNX)
        {
            ResU.ResFile resFile = new ResU.ResFile();

            return resFile;
        }

        public static Texture WiiUToSwicthBNTXTexture(ResU.Texture textureU)
        {
            Texture texture = new Texture();
            texture.Height = textureU.Height;
            texture.Width = textureU.Width;
            texture.Format = ConvertGX2ToSwitchFormat(textureU.Format);
            texture.Alignment = (int)textureU.Alignment;
            texture.ArrayLength = textureU.ArrayLength;
            texture.ChannelRed = ConvertWiiUToBNTXChannel(textureU.CompSelR);
            texture.ChannelGreen = ConvertWiiUToBNTXChannel(textureU.CompSelG);
            texture.ChannelBlue = ConvertWiiUToBNTXChannel(textureU.CompSelB);
            texture.ChannelAlpha = ConvertWiiUToBNTXChannel(textureU.CompSelA);
            texture.MipCount = textureU.MipCount == 0 ? 1 : texture.MipCount;
            texture.Swizzle = 0;

            return texture;
        }

        //Todo. Bake sizes are altered in switch somewhat, although mostly all animations should be fine

        public static ResNX.MaterialAnim FVISConvertWiiUToSwitch(ResU.VisibilityAnim VisualAnim)
        {
            ResNX.MaterialAnim matAnim = new ResNX.MaterialAnim();
            matAnim.Name = VisualAnim.Name;
            matAnim.Path = VisualAnim.Path;
            matAnim.FrameCount = VisualAnim.FrameCount;
            matAnim.BindIndices = VisualAnim.BindIndices;
            matAnim.BakedSize = VisualAnim.BakedSize;
            matAnim.Loop = VisualAnim.Flags.HasFlag(ResU.TexPatternAnimFlags.Looping);

            int CurveIndex = 0;
            for (int m = 0; m < VisualAnim.Names.Count; m++)
            {
                ResNX.MaterialAnimData matAnimData = new ResNX.MaterialAnimData();
                matAnimData.Name = VisualAnim.Names[m];
            }

            matAnim.UserData = ConvertUserDataWiiU2Switch(VisualAnim.UserData);

            return matAnim;
        }

        public static ResNX.MaterialAnim FSHUConvertWiiUToSwitch(ResU.ShaderParamAnim ShaderAnim)
        {
            ResNX.MaterialAnim matAnim = new ResNX.MaterialAnim();
            matAnim.Name = ShaderAnim.Name;
            matAnim.Path = ShaderAnim.Path;
            matAnim.FrameCount = ShaderAnim.FrameCount;
            matAnim.BindIndices = ShaderAnim.BindIndices;
            matAnim.BakedSize = ShaderAnim.BakedSize;
            matAnim.Loop = ShaderAnim.Flags.HasFlag(ResU.TexPatternAnimFlags.Looping);

            int CurveIndex = 0;
            for (int m = 0; m < ShaderAnim.ShaderParamMatAnims.Count; m++)
            {
                ResNX.MaterialAnimData matAnimData = new ResNX.MaterialAnimData();
                matAnimData.Name = ShaderAnim.ShaderParamMatAnims[m].Name;

                foreach (var paramU in ShaderAnim.ShaderParamMatAnims[m].ParamAnimInfos)
                {
                    ResNX.ParamAnimInfo animInfo = new ResNX.ParamAnimInfo();
                    animInfo.Name = paramU.Name;
                    animInfo.BeginCurve = paramU.BeginCurve;
                    animInfo.BeginConstant = paramU.BeginConstant;
                    animInfo.ConstantCount = paramU.ConstantCount;
                    animInfo.FloatCurveCount = paramU.FloatCurveCount;
                    animInfo.IntCurveCount = paramU.IntCurveCount;
                    animInfo.SubBindIndex = paramU.SubBindIndex;

                    matAnimData.ParamAnimInfos.Add(animInfo);
                }

                if (ShaderAnim.ShaderParamMatAnims[m].Curves.Count == 0)
                {
                    foreach (var constant in ShaderAnim.ShaderParamMatAnims[m].Constants)
                    {
                        //Add base values as constants
                        matAnimData.Constants.Add(new ResNX.AnimConstant()
                        {
                            Value = (float)constant.Value,
                            AnimDataOffset = constant.AnimDataOffset,
                        });
                    }
                }
                else
                {
                    matAnimData.ShaderParamCurveIndex = CurveIndex++;
                    matAnimData.BeginVisalConstantIndex = 0;

                    matAnimData.Curves = ConvertAnimCurveWiiUToSwitch(ShaderAnim.ShaderParamMatAnims[m].Curves);
                }

                matAnim.MaterialAnimDataList.Add(matAnimData);
            }

            matAnim.UserData = ConvertUserDataWiiU2Switch(ShaderAnim.UserData);

            return matAnim;
        }

        public static ResNX.MaterialAnim FTXPConvertWiiUToSwitch(ResU.TexPatternAnim texPatternAnim)
        {
            //Different versions use different lists
            if (texPatternAnim.TextureRefNames == null)
                texPatternAnim.TextureRefNames = new List<ResU.TextureRef>();
            if (texPatternAnim.TextureRefs == null)
                texPatternAnim.TextureRefs = new ResU.ResDict<ResU.TextureRef>();

            ResNX.MaterialAnim matAnim = new ResNX.MaterialAnim();
            matAnim.Name = texPatternAnim.Name;
            matAnim.Path = texPatternAnim.Path;
            matAnim.FrameCount = texPatternAnim.FrameCount;
            matAnim.BindIndices = texPatternAnim.BindIndices;
            matAnim.BakedSize = texPatternAnim.BakedSize;
            matAnim.Loop = texPatternAnim.Flags.HasFlag(ResU.TexPatternAnimFlags.Looping);

            foreach (var texRef in texPatternAnim.TextureRefNames)
                matAnim.TextureNames.Add(texRef.Name);

            foreach (var texRef in texPatternAnim.TextureRefs)
                matAnim.TextureNames.Add(texRef.Key);

            int CurveIndex = 0;
            for (int m = 0; m < texPatternAnim.TexPatternMatAnims.Count; m++)
            {
                ResNX.MaterialAnimData matAnimData = new ResNX.MaterialAnimData();
                matAnimData.Name = texPatternAnim.TexPatternMatAnims[m].Name;

                foreach (var patternInfoU in texPatternAnim.TexPatternMatAnims[m].PatternAnimInfos)
                {
                    ResNX.TexturePatternAnimInfo animInfo = new ResNX.TexturePatternAnimInfo();
                    animInfo.Name = patternInfoU.Name;
                    animInfo.CurveIndex = (ushort)patternInfoU.CurveIndex;
                    animInfo.BeginConstant = (ushort)patternInfoU.SubBindIndex;
                    matAnimData.TexturePatternAnimInfos.Add(animInfo);
                }

                if (texPatternAnim.TexPatternMatAnims[m].Curves.Count == 0)
                {
                    foreach (var baseData in texPatternAnim.TexPatternMatAnims[m].BaseDataList)
                    {
                        //Add base values as constants
                        matAnimData.Constants.Add(new ResNX.AnimConstant()
                        {
                            Value = (int)baseData,
                            AnimDataOffset = 0,
                        });
                    }
                }
                else
                {
                    matAnimData.TexturePatternCurveIndex = CurveIndex++;
                    matAnimData.BeginVisalConstantIndex = 0;

                    matAnimData.Curves = ConvertAnimCurveWiiUToSwitch(texPatternAnim.TexPatternMatAnims[m].Curves);
                }

                matAnim.MaterialAnimDataList.Add(matAnimData);
            }

            matAnim.TextureBindArray = new long[matAnim.TextureNames.Count];
            for (int i = 0; i < matAnim.TextureNames.Count; i++)
                matAnim.TextureBindArray[i] = -1;

            matAnim.UserData = ConvertUserDataWiiU2Switch(texPatternAnim.UserData);

            return matAnim;
        }

        private static List<ResNX.UserData> ConvertUserDataWiiU2Switch(ResU.ResDict<ResU.UserData> UserDataU)
        {
            var UserDataNX = new List<ResNX.UserData>();
            for (int i = 0; i < UserDataU.Count; i++)
            {
                var userData = new ResNX.UserData();
                userData.Name = UserDataU[i].Name;

                if (UserDataU[i].Type == ResU.UserDataType.Byte)
                    userData.SetValue(UserDataU[i].GetValueByteArray());
                if (UserDataU[i].Type == ResU.UserDataType.Int32)
                    userData.SetValue(UserDataU[i].GetValueInt32Array());
                if (UserDataU[i].Type == ResU.UserDataType.Single)
                    userData.SetValue(UserDataU[i].GetValueSingleArray());
                if (UserDataU[i].Type == ResU.UserDataType.String)
                    userData.SetValue(UserDataU[i].GetValueStringArray());
                if (UserDataU[i].Type == ResU.UserDataType.WString)
                    userData.SetValue(UserDataU[i].GetValueStringArray());

                UserDataNX.Add(userData);
            }

            return UserDataNX;
        }

        private static ResU.ResDict<ResU.UserData> ConvertUserDataSwitch2WiiU(List<ResNX.UserData> UserDataNX)
        {
            var UserDataU = new ResU.ResDict<ResU.UserData>();
            for (int i = 0; i < UserDataNX.Count; i++)
            {
                var userData = new ResU.UserData();
                userData.Name = UserDataNX[i].Name;

                if (UserDataNX[i].Type == ResNX.UserDataType.Byte)
                    userData.SetValue(UserDataNX[i].GetValueByteArray());
                if (UserDataNX[i].Type == ResNX.UserDataType.Int32)
                    userData.SetValue(UserDataNX[i].GetValueInt32Array());
                if (UserDataNX[i].Type == ResNX.UserDataType.Single)
                    userData.SetValue(UserDataNX[i].GetValueSingleArray());
                if (UserDataNX[i].Type == ResNX.UserDataType.String)
                    userData.SetValue(UserDataNX[i].GetValueStringArray());
                if (UserDataNX[i].Type == ResNX.UserDataType.WString)
                    userData.SetValue(UserDataNX[i].GetValueStringArray());

                UserDataU.Add(userData.Name, userData);
            }

            return UserDataU;
        }

        public static ResNX.SkeletalAnim FSKAConvertWiiUToSwitch(ResU.SkeletalAnim skeletalAnimU)
        {
            ResNX.SkeletalAnim ska = new ResNX.SkeletalAnim();
            ska.Name = skeletalAnimU.Name;
            ska.Path = skeletalAnimU.Path;
            ska.FrameCount = skeletalAnimU.FrameCount;
            ska.FlagsScale = ResNX.SkeletalAnimFlagsScale.None;

            if (skeletalAnimU.FlagsScale.HasFlag(ResU.SkeletalAnimFlagsScale.Maya))
                ska.FlagsScale = ResNX.SkeletalAnimFlagsScale.Maya;
            if (skeletalAnimU.FlagsScale.HasFlag(ResU.SkeletalAnimFlagsScale.Softimage))
                ska.FlagsScale = ResNX.SkeletalAnimFlagsScale.Softimage;
            if (skeletalAnimU.FlagsScale.HasFlag(ResU.SkeletalAnimFlagsScale.Standard))
                ska.FlagsScale = ResNX.SkeletalAnimFlagsScale.Standard;

            ska.FrameCount = skeletalAnimU.FrameCount;
            ska.BindIndices = skeletalAnimU.BindIndices;
            ska.BakedSize = skeletalAnimU.BakedSize;
            ska.Loop = skeletalAnimU.Loop;
            ska.Baked = skeletalAnimU.Baked;
            foreach (var boneAnimU in skeletalAnimU.BoneAnims)
            {
                var boneAnim = new ResNX.BoneAnim();
                ska.BoneAnims.Add(boneAnim);
                boneAnim.Name = boneAnimU.Name;
                boneAnim.BeginRotate = boneAnimU.BeginRotate;
                boneAnim.BeginTranslate = boneAnimU.BeginTranslate;
                boneAnim.BeginBaseTranslate = boneAnimU.BeginBaseTranslate;
                var baseData = new ResNX.BoneAnimData();
                baseData.Translate = boneAnimU.BaseData.Translate;
                baseData.Scale = boneAnimU.BaseData.Scale;
                baseData.Rotate = boneAnimU.BaseData.Rotate;
                baseData.Flags = boneAnimU.BaseData.Flags;
                boneAnim.BaseData = baseData;
                boneAnim.FlagsBase = (ResNX.BoneAnimFlagsBase)boneAnimU.FlagsBase;
                boneAnim.FlagsCurve = (ResNX.BoneAnimFlagsCurve)boneAnimU.FlagsCurve;
                boneAnim.FlagsTransform = (ResNX.BoneAnimFlagsTransform)boneAnimU.FlagsTransform;
                boneAnim.Curves = ConvertAnimCurveWiiUToSwitch(boneAnimU.Curves);
            }

            return ska;
        }

        private static IList<ResNX.AnimCurve> ConvertAnimCurveWiiUToSwitch(IList<ResU.AnimCurve> curvesU)
        {
            var curvesNX = new List<ResNX.AnimCurve>();

            foreach (var curveU in curvesU)
            {
                ResNX.AnimCurve curve = new ResNX.AnimCurve();
                curve.AnimDataOffset = curveU.AnimDataOffset;
                curve.CurveType = (ResNX.AnimCurveType)curveU.CurveType;
                curve.Delta = curveU.Delta;
                curve.EndFrame = curveU.EndFrame;
                curve.Frames = curveU.Frames;
                curve.Keys = curveU.Keys;
                curve.KeyStepBoolData = curveU.KeyStepBoolData;
                curve.KeyType = (ResNX.AnimCurveKeyType)curveU.KeyType;
                curve.FrameType = (ResNX.AnimCurveFrameType)curveU.FrameType;
                curve.StartFrame = curveU.StartFrame;
                curve.Scale = curveU.Scale;
                curve.Offset = (float)curveU.Offset;

                curvesNX.Add(curve);
            }

            return curvesNX;
        }

        private static IList<ResU.AnimCurve> ConvertAnimCurveSwitchToWiiU(IList<ResNX.AnimCurve> curvesNX)
        {
            var curvesU = new List<ResU.AnimCurve>();

            foreach (var curveNX in curvesNX)
            {
                ResU.AnimCurve curve = new ResU.AnimCurve();
                curve.AnimDataOffset = curveNX.AnimDataOffset;
                curve.CurveType = (ResU.AnimCurveType)curveNX.CurveType;
                curve.Delta = curveNX.Delta;
                curve.EndFrame = curveNX.EndFrame;
                curve.Frames = curveNX.Frames;
                curve.Keys = curveNX.Keys;
                curve.KeyStepBoolData = curveNX.KeyStepBoolData;
                curve.KeyType = (ResU.AnimCurveKeyType)curveNX.KeyType;
                curve.FrameType = (ResU.AnimCurveFrameType)curveNX.FrameType;
                curve.StartFrame = curveNX.StartFrame;
                curve.Scale = curveNX.Scale;
                curve.Offset = (float)curveNX.Offset;

                curvesU.Add(curve);
            }

            return curvesU;
        }

        public static ResU.SkeletalAnim FSKAConvertSwitchToWiiU(ResNX.SkeletalAnim skeletalAnimNX)
        {
            ResU.SkeletalAnim ska = new ResU.SkeletalAnim();
            ska.Name = skeletalAnimNX.Name;
            ska.Path = skeletalAnimNX.Path;
            ska.FrameCount = skeletalAnimNX.FrameCount;
            ska.FlagsScale = ResU.SkeletalAnimFlagsScale.None;

            if (skeletalAnimNX.FlagsScale.HasFlag(ResNX.SkeletalAnimFlagsScale.Maya))
                ska.FlagsScale = ResU.SkeletalAnimFlagsScale.Maya;
            if (skeletalAnimNX.FlagsScale.HasFlag(ResNX.SkeletalAnimFlagsScale.Softimage))
                ska.FlagsScale = ResU.SkeletalAnimFlagsScale.Softimage;
            if (skeletalAnimNX.FlagsScale.HasFlag(ResNX.SkeletalAnimFlagsScale.Standard))
                ska.FlagsScale = ResU.SkeletalAnimFlagsScale.Standard;

            ska.FrameCount = skeletalAnimNX.FrameCount;
            ska.BindIndices = skeletalAnimNX.BindIndices;
            ska.BakedSize = skeletalAnimNX.BakedSize;
            ska.Loop = skeletalAnimNX.Loop;
            ska.Baked = skeletalAnimNX.Baked;
            foreach (var boneAnimNX in skeletalAnimNX.BoneAnims)
            {
                var boneAnimU = new ResU.BoneAnim();
                ska.BoneAnims.Add(boneAnimU);
                boneAnimU.Name = boneAnimNX.Name;
                boneAnimU.BeginRotate = boneAnimNX.BeginRotate;
                boneAnimU.BeginTranslate = boneAnimNX.BeginTranslate;
                boneAnimU.BeginBaseTranslate = boneAnimNX.BeginBaseTranslate;
                var baseData = new ResU.BoneAnimData();
                baseData.Translate = boneAnimNX.BaseData.Translate;
                baseData.Scale = boneAnimNX.BaseData.Scale;
                baseData.Rotate = boneAnimNX.BaseData.Rotate;
                baseData.Flags = boneAnimNX.BaseData.Flags;
                boneAnimU.BaseData = baseData;
                boneAnimU.FlagsBase = (ResU.BoneAnimFlagsBase)boneAnimNX.FlagsBase;
                boneAnimU.FlagsCurve = (ResU.BoneAnimFlagsCurve)boneAnimNX.FlagsCurve;
                boneAnimU.FlagsTransform = (ResU.BoneAnimFlagsTransform)boneAnimNX.FlagsTransform;
                boneAnimU.Curves = ConvertAnimCurveSwitchToWiiU(boneAnimNX.Curves);
            }

            return ska;
        }


        private static ResU.GX2.GX2CompSel ConvertBNTXToWiiUChannel(ResGFXBNTX.ChannelType compNX)
        {
            ResU.GX2.GX2CompSel type = new ResU.GX2.GX2CompSel();

            switch (compNX)
            {
                case ResGFXBNTX.ChannelType.Zero:
                    type = ResU.GX2.GX2CompSel.Always0;
                    break;
                    case ResGFXBNTX.ChannelType.One:
                    type = ResU.GX2.GX2CompSel.Always1;
                    break;
                case ResGFXBNTX.ChannelType.Alpha:
                    type = ResU.GX2.GX2CompSel.ChannelA;
                    break;
                    case ResGFXBNTX.ChannelType.Blue:
                    type = ResU.GX2.GX2CompSel.ChannelB;
                    break;
                case ResGFXBNTX.ChannelType.Green:
                    type = ResU.GX2.GX2CompSel.ChannelG;
                    break;
                case ResGFXBNTX.ChannelType.Red:
                    type = ResU.GX2.GX2CompSel.ChannelR;
                    break;
            }

            return type;
        }

        private static ResGFXBNTX.ChannelType ConvertWiiUToBNTXChannel(ResU.GX2.GX2CompSel compU)
        {
            ResGFXBNTX.ChannelType type = new ResGFXBNTX.ChannelType();

            switch (compU)
            {
                case ResU.GX2.GX2CompSel.Always0:
                    type = ResGFXBNTX.ChannelType.Zero;
                    break;
                case ResU.GX2.GX2CompSel.Always1:
                    type = ResGFXBNTX.ChannelType.One;
                    break;
                case ResU.GX2.GX2CompSel.ChannelA:
                    type = ResGFXBNTX.ChannelType.Alpha;
                    break;
                case ResU.GX2.GX2CompSel.ChannelB:
                    type = ResGFXBNTX.ChannelType.Blue;
                    break;
                case ResU.GX2.GX2CompSel.ChannelG:
                    type = ResGFXBNTX.ChannelType.Green;
                    break;
                case ResU.GX2.GX2CompSel.ChannelR:
                    type = ResGFXBNTX.ChannelType.Red;
                    break;
            }

            return type;
        }

        private static ResU.GX2.GX2SurfaceFormat ConvertSwitchToGX2Format(ResGFXBNTX.SurfaceFormat Format)
        {
            switch (Format)
            {
                case ResGFXBNTX.SurfaceFormat.BC1_SRGB: return ResU.GX2.GX2SurfaceFormat.T_BC1_SRGB;
                case ResGFXBNTX.SurfaceFormat.BC1_UNORM: return ResU.GX2.GX2SurfaceFormat.T_BC1_UNorm;
                case ResGFXBNTX.SurfaceFormat.BC2_SRGB: return ResU.GX2.GX2SurfaceFormat.T_BC2_SRGB;
                case ResGFXBNTX.SurfaceFormat.BC2_UNORM: return ResU.GX2.GX2SurfaceFormat.T_BC2_UNorm;
                case ResGFXBNTX.SurfaceFormat.BC3_SRGB: return ResU.GX2.GX2SurfaceFormat.T_BC3_SRGB;
                case ResGFXBNTX.SurfaceFormat.BC3_UNORM: return ResU.GX2.GX2SurfaceFormat.T_BC3_UNorm;
                case ResGFXBNTX.SurfaceFormat.BC4_SNORM: return ResU.GX2.GX2SurfaceFormat.T_BC4_SNorm;
                case ResGFXBNTX.SurfaceFormat.BC4_UNORM: return ResU.GX2.GX2SurfaceFormat.T_BC4_UNorm;
                case ResGFXBNTX.SurfaceFormat.BC5_SNORM: return ResU.GX2.GX2SurfaceFormat.T_BC5_SNorm;
                case ResGFXBNTX.SurfaceFormat.BC5_UNORM: return ResU.GX2.GX2SurfaceFormat.T_BC5_UNorm;
                case ResGFXBNTX.SurfaceFormat.R8_G8_B8_A8_SRGB: return ResU.GX2.GX2SurfaceFormat.TCS_R8_G8_B8_A8_SRGB;
                case ResGFXBNTX.SurfaceFormat.R8_G8_B8_A8_UNORM: return ResU.GX2.GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNorm;
                case ResGFXBNTX.SurfaceFormat.A1_B5_G5_R5_UNORM: return ResU.GX2.GX2SurfaceFormat.TC_A1_B5_G5_R5_UNorm;
                case ResGFXBNTX.SurfaceFormat.R5_G5_B5_A1_UNORM: return ResU.GX2.GX2SurfaceFormat.TC_R5_G5_B5_A1_UNorm;
                case ResGFXBNTX.SurfaceFormat.R16_G16_B16_A16_UNORM: return ResU.GX2.GX2SurfaceFormat.TC_R16_G16_B16_A16_UNorm;
                case ResGFXBNTX.SurfaceFormat.R16_G16_UNORM: return ResU.GX2.GX2SurfaceFormat.TC_R16_G16_UNorm;
                case ResGFXBNTX.SurfaceFormat.R16_UNORM: return ResU.GX2.GX2SurfaceFormat.TCD_R16_UNorm;
                case ResGFXBNTX.SurfaceFormat.R10_G10_B10_A2_UNORM: return ResU.GX2.GX2SurfaceFormat.TCS_R10_G10_B10_A2_UNorm;
                case ResGFXBNTX.SurfaceFormat.R4_G4_B4_A4_UNORM: return ResU.GX2.GX2SurfaceFormat.TC_R4_G4_B4_A4_UNorm;
                case ResGFXBNTX.SurfaceFormat.R4_G4_UNORM: return ResU.GX2.GX2SurfaceFormat.T_R4_G4_UNorm;
                case ResGFXBNTX.SurfaceFormat.R5_G6_B5_UNORM: return ResU.GX2.GX2SurfaceFormat.TCS_R5_G6_B5_UNorm;
                case ResGFXBNTX.SurfaceFormat.R8_G8_UNORM: return ResU.GX2.GX2SurfaceFormat.TC_R8_G8_UNorm;
                case ResGFXBNTX.SurfaceFormat.R8_UNORM: return ResU.GX2.GX2SurfaceFormat.TC_R8_UNorm;
                default:
                    throw new Exception("Unsuppored format " + Format);
            }
        }

        private static ResGFXBNTX.SurfaceFormat ConvertGX2ToSwitchFormat(ResU.GX2.GX2SurfaceFormat Format)
        {
            switch (Format)
            {
                case ResU.GX2.GX2SurfaceFormat.T_BC1_SRGB: return ResGFXBNTX.SurfaceFormat.BC1_SRGB;
                case ResU.GX2.GX2SurfaceFormat.T_BC1_UNorm: return ResGFXBNTX.SurfaceFormat.BC1_UNORM;
                case ResU.GX2.GX2SurfaceFormat.T_BC2_SRGB: return ResGFXBNTX.SurfaceFormat.BC2_SRGB;
                case ResU.GX2.GX2SurfaceFormat.T_BC2_UNorm: return ResGFXBNTX.SurfaceFormat.BC2_UNORM;
                case ResU.GX2.GX2SurfaceFormat.T_BC3_SRGB: return ResGFXBNTX.SurfaceFormat.BC3_SRGB;
                case ResU.GX2.GX2SurfaceFormat.T_BC3_UNorm: return ResGFXBNTX.SurfaceFormat.BC3_UNORM;
                case ResU.GX2.GX2SurfaceFormat.T_BC4_SNorm: return ResGFXBNTX.SurfaceFormat.BC4_SNORM;
                case ResU.GX2.GX2SurfaceFormat.T_BC4_UNorm: return ResGFXBNTX.SurfaceFormat.BC4_UNORM;
                case ResU.GX2.GX2SurfaceFormat.T_BC5_SNorm: return ResGFXBNTX.SurfaceFormat.BC5_SNORM;
                case ResU.GX2.GX2SurfaceFormat.T_BC5_UNorm: return ResGFXBNTX.SurfaceFormat.BC5_SNORM;
                case ResU.GX2.GX2SurfaceFormat.TCS_R8_G8_B8_A8_SRGB: return ResGFXBNTX.SurfaceFormat.R8_G8_B8_A8_SRGB;
                case ResU.GX2.GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNorm: return ResGFXBNTX.SurfaceFormat.R8_G8_B8_A8_UNORM;
                case ResU.GX2.GX2SurfaceFormat.TC_A1_B5_G5_R5_UNorm: return ResGFXBNTX.SurfaceFormat.A1_B5_G5_R5_UNORM;
                case ResU.GX2.GX2SurfaceFormat.TC_R5_G5_B5_A1_UNorm: return ResGFXBNTX.SurfaceFormat.R5_G5_B5_A1_UNORM;
                case ResU.GX2.GX2SurfaceFormat.TC_R16_G16_B16_A16_UNorm: return ResGFXBNTX.SurfaceFormat.R16_G16_B16_A16_UNORM;
                case ResU.GX2.GX2SurfaceFormat.TC_R16_G16_UNorm: return ResGFXBNTX.SurfaceFormat.R16_G16_UNORM;
                case ResU.GX2.GX2SurfaceFormat.TCD_R16_UNorm: return ResGFXBNTX.SurfaceFormat.R16_UNORM;
                case ResU.GX2.GX2SurfaceFormat.TCS_R10_G10_B10_A2_UNorm: return ResGFXBNTX.SurfaceFormat.R10_G10_B10_A2_UNORM;
                case ResU.GX2.GX2SurfaceFormat.TC_R4_G4_B4_A4_UNorm: return ResGFXBNTX.SurfaceFormat.R4_G4_B4_A4_UNORM;
                case ResU.GX2.GX2SurfaceFormat.T_R4_G4_UNorm: return ResGFXBNTX.SurfaceFormat.R4_G4_UNORM;
                case ResU.GX2.GX2SurfaceFormat.TCS_R5_G6_B5_UNorm: return ResGFXBNTX.SurfaceFormat.R5_G6_B5_UNORM;
                case ResU.GX2.GX2SurfaceFormat.TC_R8_G8_UNorm: return ResGFXBNTX.SurfaceFormat.R8_G8_UNORM;
                case ResU.GX2.GX2SurfaceFormat.TC_R8_UNorm: return ResGFXBNTX.SurfaceFormat.R8_UNORM;
                default:
                    throw new Exception("Unsuppored format " + Format);
            }
        }
    }
}
