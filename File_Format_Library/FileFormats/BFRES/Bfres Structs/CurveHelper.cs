using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.NintenTools.NSW.Bfres;
using Toolbox.Library;
using ResU = Syroot.NintenTools.Bfres;
using Toolbox.Library.Animations;

namespace Bfres.Structs
{
    public class CurveHelper
    {
        public static AnimCurveFrameType GetFrameType(uint FrameCount)
        {
            if (FrameCount < byte.MaxValue) return AnimCurveFrameType.Byte;
            if (FrameCount < Int16.MaxValue) return AnimCurveFrameType.Decimal10x5;
            else return AnimCurveFrameType.Single;
        }

        public static AnimCurveKeyType GetKeyType(float Value)
        {
            if (Value < byte.MaxValue) return AnimCurveKeyType.SByte;
            if (Value < Int16.MaxValue) return AnimCurveKeyType.Int16;
            else return AnimCurveKeyType.Single;
        }

        public static Animation.KeyGroup CreateTrackWiiU(ResU.AnimCurve animCurve)
        {
            Animation.KeyGroup track = new Animation.KeyGroup();
            track.AnimDataOffset = animCurve.AnimDataOffset;
            track.Scale = animCurve.Scale;
            track.Offset = animCurve.Offset;
            track.StartFrame = animCurve.StartFrame;
            track.EndFrame = animCurve.EndFrame;
            track.Delta = animCurve.Delta;

            float tanscale = animCurve.Delta;
            if (tanscale == 0)
                tanscale = 1;

            if (animCurve.Scale == 0)
                animCurve.Scale = 1;

            for (int i = 0; i < (ushort)animCurve.Frames.Length; i++)
            {
                switch (animCurve.CurveType)
                {
                    case ResU.AnimCurveType.Cubic: //4 elements are stored for cubic
                        track.InterpolationType = InterpolationType.HERMITE;
                        var coef0 = animCurve.Offset + (animCurve.Keys[i, 0] * animCurve.Scale);
                        var coef1 = animCurve.Offset + (animCurve.Keys[i, 1] * animCurve.Scale);
                        var coef2 = animCurve.Offset + (animCurve.Keys[i, 2] * animCurve.Scale);
                        var coef3 = animCurve.Offset + (animCurve.Keys[i, 3] * animCurve.Scale);
                        var slopes = GetSlopes(animCurve, i);

                        track.Keys.Add(new Animation.KeyFrame()
                        {
                            IsKeyed = true,
                            InterType = InterpolationType.HERMITE,
                            Frame = (int)animCurve.Frames[i],
                            Value = coef0,
                           // Slope1 = slopes[0],
                          //  Slope2 = slopes[1],
                        });
                        break;
                    case ResU.AnimCurveType.Linear: //2 elements are stored for linear
                        track.InterpolationType = InterpolationType.LINEAR;
                        track.Keys.Add(new Animation.KeyFrame()
                        {
                            IsKeyed = true,
                            InterType = InterpolationType.LINEAR,
                            Frame = (int)animCurve.Frames[i],
                            Value = animCurve.Offset + (animCurve.Keys[i, 0] * animCurve.Scale),
                            Delta = animCurve.Offset + (animCurve.Keys[i, 1] * animCurve.Scale),
                        });
                        break;
                    case ResU.AnimCurveType.StepInt: //1 element are stored for step
                        track.InterpolationType = InterpolationType.STEP;
                        track.Keys.Add(new Animation.KeyFrame()
                        {
                            IsKeyed = true,
                            InterType = InterpolationType.STEP,
                            Frame = (int)animCurve.Frames[i],
                            Value = (int)animCurve.Offset + (int)animCurve.Keys[i, 0] * animCurve.Scale,
                        });

                        Console.WriteLine($"Frame {animCurve.Frames[i]} FrameINT {(int)animCurve.Frames[i]} Offset " + (int)animCurve.Offset + " " + ((int)animCurve.Offset + (int)animCurve.Keys[i, 0] * animCurve.Scale));
                        break;
                    default:
                        throw new Exception("Unsupported anim type!");
                }
            }

            return track;
        }

        public static float[] GetSlopes(AnimCurve curve, float index)
        {
            float[] slopes = new float[2];
            if (curve.CurveType == AnimCurveType.Cubic)
            {
                float InSlope = 0;
                float OutSlope = 0;
                for (int i = 0; i < curve.Frames.Length; i++)
                {
                    var coef0 = curve.Keys[i, 0] * curve.Scale + curve.Offset;
                    var coef1 = curve.Keys[i, 1] * curve.Scale + curve.Offset;
                    var coef2 = curve.Keys[i, 2] * curve.Scale + curve.Offset;
                    var coef3 = curve.Keys[i, 3] * curve.Scale + curve.Offset;
                    float time = 0;
                    float delta = 0;
                    if (i < curve.Frames.Length - 1)
                    {
                        var nextValue = curve.Keys[i + 1, 0] * curve.Scale + curve.Offset;
                        delta = nextValue - coef0;
                        time = curve.Frames[i + 1] - curve.Frames[i];
                    }

                    var slopeData = CurveInterpolationHelper.GetCubicSlopes(time, delta,
                        new float[4] { coef0, coef1, coef2, coef3, });

                    if (index == i)
                    {
                        OutSlope = slopeData[1];
                        return new float[2] { InSlope, OutSlope };
                    }

                    //The previous inslope is used
                    InSlope = slopeData[0];
                }
            }

            return slopes;
        }

        public static float[] GetSlopes(ResU.AnimCurve curve, float index)
        {
            float[] slopes = new float[2];
            if (curve.CurveType == ResU.AnimCurveType.Cubic)
            {
                float InSlope = 0;
                float OutSlope = 0;
                for (int i = 0; i < curve.Frames.Length; i++)
                {
                    var coef0 = curve.Keys[i, 0] * curve.Scale + curve.Offset;
                    var coef1 = curve.Keys[i, 1] * curve.Scale + curve.Offset;
                    var coef2 = curve.Keys[i, 2] * curve.Scale + curve.Offset;
                    var coef3 = curve.Keys[i, 3] * curve.Scale + curve.Offset;
                    float time = 0;
                    float delta = 0;
                    if (i < curve.Frames.Length - 1)
                    {
                        var nextValue = curve.Keys[i + 1, 0] * curve.Scale + curve.Offset;
                        delta = nextValue - coef0;
                        time = curve.Frames[i + 1] - curve.Frames[i];
                    }

                    var slopeData = CurveInterpolationHelper.GetCubicSlopes(time, delta,
                        new float[4] { coef0, coef1, coef2, coef3, });

                    if (index == i)
                    {
                        OutSlope = slopeData[1];
                        return new float[2] { InSlope, OutSlope };
                    }

                    //The previous inslope is used
                    InSlope = slopeData[0];
                }
            }

            return slopes;
        }

        public static BooleanKeyGroup CreateBooleanTrackWiiU(ResU.AnimCurve animCurve)
        {
            BooleanKeyGroup track = new BooleanKeyGroup();
            track.AnimDataOffset = animCurve.AnimDataOffset;
            track.Scale = animCurve.Scale;
            track.Offset = animCurve.Offset;
            track.StartFrame = animCurve.StartFrame;
            track.EndFrame = animCurve.EndFrame;
            track.Delta = animCurve.Delta;

            for (int i = 0; i < (ushort)animCurve.Frames.Length; i++)
            {
                switch (animCurve.CurveType)
                {
                    case ResU.AnimCurveType.StepBool: //1 element are stored for step
                        track.Keys.Add(new BooleanKeyFrame()
                        {
                            IsKeyed = true,
                            InterType = InterpolationType.STEPBOOL,
                            Frame = (int)animCurve.Frames[i],
                            Visible = animCurve.KeyStepBoolData[i],
                        });
                        break;
                    default:
                        throw new Exception("Unsupported anim type!");
                }
            }

            return track;
        }

        public static BooleanKeyGroup CreateBooleanTrack(AnimCurve animCurve)
        {
            BooleanKeyGroup track = new BooleanKeyGroup();
            track.AnimDataOffset = animCurve.AnimDataOffset;
            track.Scale = animCurve.Scale;
            track.Offset = animCurve.Offset;
            track.StartFrame = animCurve.StartFrame;
            track.EndFrame = animCurve.EndFrame;
            track.Delta = animCurve.Delta;

            for (int i = 0; i < (ushort)animCurve.Frames.Length; i++)
            {
                switch (animCurve.CurveType)
                {
                    case AnimCurveType.StepBool: //1 element are stored for step
                        track.Keys.Add(new BooleanKeyFrame()
                        {
                            IsKeyed = true,
                            InterType = InterpolationType.STEPBOOL,
                            Frame = (int)animCurve.Frames[i],
                            Visible = animCurve.KeyStepBoolData[i],
                        });
                        break;
                    default:
                        throw new Exception("Unsupported anim type!");
                }
            }

            return track;
        }
        public static Animation.KeyGroup CreateTrack(AnimCurve animCurve)
        {
            Animation.KeyGroup track = new Animation.KeyGroup();
            track.AnimDataOffset = animCurve.AnimDataOffset;
            track.Scale = animCurve.Scale;
            track.Offset = animCurve.Offset;
            track.StartFrame = animCurve.StartFrame;
            track.EndFrame = animCurve.EndFrame;
            track.Delta = animCurve.Delta;

            float tanscale = animCurve.Delta;
            if (tanscale == 0)
                tanscale = 1;

            if (animCurve.Scale == 0)
                animCurve.Scale = 1;

            for (int i = 0; i < (ushort)animCurve.Frames.Length; i++)
            {
                switch (animCurve.CurveType)
                {
                    case AnimCurveType.Cubic: //4 elements are stored for cubic
                        track.InterpolationType = InterpolationType.HERMITE;
                        var coef0 = animCurve.Offset + (animCurve.Keys[i, 0] * animCurve.Scale);
                        var coef1 = animCurve.Offset + (animCurve.Keys[i, 1] * animCurve.Scale);
                        var coef2 = animCurve.Offset + (animCurve.Keys[i, 2] * animCurve.Scale);
                        var coef3 = animCurve.Offset + (animCurve.Keys[i, 3] * animCurve.Scale);
                        var slopes = GetSlopes(animCurve, i);

                        var inSlope = slopes[0] * animCurve.Scale + animCurve.Offset;
                        var outSlope = slopes[1] * animCurve.Scale + animCurve.Offset;

                        track.Keys.Add(new Animation.KeyFrame()
                        {
                            IsKeyed = true,
                            InterType = InterpolationType.HERMITE,
                            Frame = (int)animCurve.Frames[i],
                            Value = coef0,
                            Slope1 = inSlope,
                            Slope2 = outSlope,
                        });
                        break;
                    case AnimCurveType.Linear: //2 elements are stored for linear
                        track.InterpolationType = InterpolationType.LINEAR;
                        track.Keys.Add(new Animation.KeyFrame()
                        {
                            IsKeyed = true,
                            InterType = InterpolationType.LINEAR,
                            Frame = (int)animCurve.Frames[i],
                            Value = animCurve.Offset + (animCurve.Keys[i, 0] * animCurve.Scale),

                            Value1 = animCurve.Offset + (animCurve.Keys[i, 0] * animCurve.Scale),
                            Delta = animCurve.Offset + (animCurve.Keys[i, 1] * animCurve.Scale),
                        });
                        break;
                    case AnimCurveType.StepInt: //1 element are stored for step
                        track.InterpolationType = InterpolationType.STEP;
                        track.Keys.Add(new Animation.KeyFrame()
                        {
                            IsKeyed = true,
                            InterType = InterpolationType.STEP,
                            Frame = (int)animCurve.Frames[i],
                            Value = (int)animCurve.Offset + (int)animCurve.Keys[i, 0] * animCurve.Scale,
                            Value1 = (int)animCurve.Offset + (int)animCurve.Keys[i, 0] * animCurve.Scale,

                        });
                        break;
                    default:
                        throw new Exception("Unsupported anim type!");
                }
            }

            return track;
        }
    }
}
