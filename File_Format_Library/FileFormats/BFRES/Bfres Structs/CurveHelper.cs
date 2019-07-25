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
                        track.Keys.Add(new Animation.KeyFrame()
                        {
                            IsKeyed = true,
                            InterType = InterpolationType.HERMITE,
                            Frame = (int)animCurve.Frames[i],
                            Value = animCurve.Offset + (animCurve.Keys[i, 0] * animCurve.Scale),
                            Slope1 = animCurve.Offset + (animCurve.Keys[i, 1] * animCurve.Scale),
                            Slope2 = animCurve.Offset + (animCurve.Keys[i, 2] * animCurve.Scale),
                            Delta = animCurve.Offset + (animCurve.Keys[i, 3] * animCurve.Scale),
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
                            Value1 = (int)animCurve.Offset + (int)animCurve.Keys[i, 0] * animCurve.Scale,
                        });

                        Console.WriteLine($"Frame {animCurve.Frames[i]} FrameINT {(int)animCurve.Frames[i]} Offset " + (int)animCurve.Offset + " " + ((int)animCurve.Offset + (int)animCurve.Keys[i, 0] * animCurve.Scale));
                        break;
                    default:
                        throw new Exception("Unsupported anim type!");
                }
            }

            return track;
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
                        track.Keys.Add(new Animation.KeyFrame()
                        {
                            IsKeyed = true,
                            InterType = InterpolationType.HERMITE,
                            Frame = (int)animCurve.Frames[i],
                            Value = animCurve.Offset + (animCurve.Keys[i, 0] * animCurve.Scale),

                            Value1 = animCurve.Offset + (animCurve.Keys[i, 0] * animCurve.Scale),
                            Slope1 = animCurve.Offset + (animCurve.Keys[i, 1] * animCurve.Scale),
                            Slope2 = animCurve.Offset + (animCurve.Keys[i, 2] * animCurve.Scale),
                            Delta = animCurve.Offset + (animCurve.Keys[i, 3] * animCurve.Scale),
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
