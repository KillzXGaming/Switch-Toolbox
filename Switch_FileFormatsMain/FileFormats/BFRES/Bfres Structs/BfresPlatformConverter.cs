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

        public static ResNX.SkeletalAnim ConvertWiiUToSwitch(ResU.SkeletalAnim skeletalAnimU)
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

                foreach (var curveU in boneAnimU.Curves)
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

                    boneAnim.Curves.Add(curve);
                }
            }

            return ska;
        }


        public static ResU.SkeletalAnim ConvertSwitchToWiiU(ResNX.SkeletalAnim skeletalAnimNX)
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

                foreach (var curveNX in boneAnimNX.Curves)
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

                    boneAnimU.Curves.Add(curve);
                }
            }

            return ska;
        }
    }
}
