using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResU = Syroot.NintenTools.Bfres;
using ResNX = Syroot.NintenTools.NSW.Bfres;

namespace FirstPlugin
{
    public class BfresPlatformConverter
    {
        public static ResNX.ResFile BFRESConvertWiiUToSwitch(ResU.ResFile resFileU)
        {
            ResNX.ResFile resFile = new ResNX.ResFile();

            return resFile;
        }

        public static ResU.ResFile BFRESConvertSwitchToWiiU(ResNX.ResFile resFileNX)
        {
            ResU.ResFile resFile = new ResU.ResFile();

            return resFile;
        }

        //Todo. Bake sizes are altered in switch somewhat, although mostly all animations should be fine

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
                matAnimData.TexturePatternCurveIndex = 0;

                foreach (var patternInfoU in texPatternAnim.TexPatternMatAnims[m].PatternAnimInfos)
                {
                    ResNX.TexturePatternAnimInfo animInfo = new ResNX.TexturePatternAnimInfo();
                    animInfo.Name = patternInfoU.Name;
                    animInfo.CurveIndex = (uint)patternInfoU.CurveIndex;
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
                    CurveIndex++;
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
    }
}
