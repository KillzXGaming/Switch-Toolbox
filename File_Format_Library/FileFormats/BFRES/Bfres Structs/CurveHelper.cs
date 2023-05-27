using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.NintenTools.NSW.Bfres;
using Toolbox.Library;
using ResU = Syroot.NintenTools.Bfres;
using Toolbox.Library.Animations;
using AampLibraryCSharp;
using static FirstPlugin.CSAB;

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

        public static Animation.KeyGroup CreateTrackWiiU(ResU.AnimCurve curve, bool valuesAsInts = false)
        {
            Animation.KeyGroup track = new Animation.KeyGroup();
            track.AnimDataOffset = curve.AnimDataOffset;
            track.Scale = curve.Scale;
            track.Offset = curve.Offset;
            track.StartFrame = curve.StartFrame;
            track.EndFrame = curve.EndFrame;
            track.Delta = curve.Delta;

            float tanscale = curve.Delta;
            if (tanscale == 0)
                tanscale = 1;

            if (curve.Scale == 0)
                curve.Scale = 1;

            float valueScale = curve.Scale > 0 ? curve.Scale : 1;

            for (int i = 0; i < curve.Frames.Length; i++)
            {
                var frame = curve.Frames[i];
                if (frame == 0 && track.Keys.Any(x => x.Frame == 0))
                    track.Keys.RemoveAt(0);

                switch (curve.CurveType)
                {
                    case ResU.AnimCurveType.Cubic:
                        {
                            track.InterpolationType = InterpolationType.HERMITE;
                            //Important to not offset the other 3 values, just the first one!
                            var value = curve.Keys[i, 0] * valueScale + curve.Offset;
                            var slopes = GetSlopes(curve, i);

                            track.Keys.Add(new Animation.KeyFrame()
                            {
                                Frame = frame,
                                Value = value,
                                In = slopes[0],
                                Out = slopes[1],
                            });
                        }
                        break;
                    case ResU.AnimCurveType.Linear:
                        {
                            track.InterpolationType = InterpolationType.LINEAR;
                            var value = curve.Keys[i, 0] * valueScale + curve.Offset;
                            var delta = curve.Keys[i, 1] * valueScale;
                            track.Keys.Add(new Animation.KeyFrame()
                            {
                                Frame = frame,
                                Value = value,
                                Delta = delta,
                            });
                        }
                        break;
                    case ResU.AnimCurveType.StepBool:
                        {
                            track.InterpolationType = InterpolationType.STEP;
                            track.Keys.Add(new Animation.KeyFrame()
                            {
                                Frame = frame,
                                Value = curve.KeyStepBoolData[i] ? 1 : 0,
                            });
                        }
                        break;
                    default:
                        {
                            track.InterpolationType = InterpolationType.STEP;
                            var value = curve.Keys[i, 0] + curve.Offset;
                            if (valuesAsInts)
                                value = (int)curve.Keys[i, 0] + curve.Offset;

                            track.Keys.Add(new Animation.KeyFrame()
                            {
                                Frame = frame,
                                Value = value,
                            });
                        }
                        break;
                }
            }

            return track;
        }

        //Method to extract the slopes from a cubic curve
        //Need to get the time, delta, out and next in slope values
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
                    var coef1 = curve.Keys[i, 1] * curve.Scale;
                    var coef2 = curve.Keys[i, 2] * curve.Scale;
                    var coef3 = curve.Keys[i, 3] * curve.Scale;
                    float time = 0;
                    float delta = 0;
                    if (i < curve.Frames.Length - 1)
                    {
                        var nextValue = curve.Keys[i + 1, 0] * curve.Scale + curve.Offset;
                        delta = nextValue - coef0;
                        time = curve.Frames[i + 1] - curve.Frames[i];
                    }

                    var slopeData = GetCubicSlopes(time, delta,
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
                    var coef1 = curve.Keys[i, 1] * curve.Scale;
                    var coef2 = curve.Keys[i, 2] * curve.Scale;
                    var coef3 = curve.Keys[i, 3] * curve.Scale;
                    float time = 0;
                    float delta = 0;
                    if (i < curve.Frames.Length - 1)
                    {
                        var nextValue = curve.Keys[i + 1, 0] * curve.Scale + curve.Offset;
                        delta = nextValue - coef0;
                        time = curve.Frames[i + 1] - curve.Frames[i];
                    }

                    var slopeData = GetCubicSlopes(time, delta,
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

        public static float[] GetCubicSlopes(float time, float delta, float[] coef)
        {
            float outSlope = coef[1] / time;
            float param = coef[3] - (-2 * delta);
            float inSlope = param / time - outSlope;
            return new float[2] { inSlope, coef[1] == 0 ? 0 : outSlope };
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
        public static Animation.KeyGroup CreateTrack(AnimCurve curve, bool valuesAsInts = false)
        {
            Animation.KeyGroup track = new Animation.KeyGroup();
            track.AnimDataOffset = curve.AnimDataOffset;
            track.Scale = curve.Scale;
            track.Offset = curve.Offset;
            track.StartFrame = curve.StartFrame;
            track.EndFrame = curve.EndFrame;
            track.Delta = curve.Delta;

            float tanscale = curve.Delta;
            if (tanscale == 0)
                tanscale = 1;

            if (curve.Scale == 0)
                curve.Scale = 1;

            float valueScale = curve.Scale > 0 ? curve.Scale : 1;

            for (int i = 0; i < curve.Frames.Length; i++)
            {
                var frame = curve.Frames[i];
                if (frame == 0 && track.Keys.Any(x => x.Frame == 0))
                    track.Keys.RemoveAt(0);

                switch (curve.CurveType)
                {
                    case AnimCurveType.Cubic:
                        {
                            track.InterpolationType = InterpolationType.HERMITE;
                            //Important to not offset the other 3 values, just the first one!
                            var value = curve.Keys[i, 0] * valueScale + curve.Offset;
                            var slopes = GetSlopes(curve, i);

                            track.Keys.Add(new Animation.KeyFrame()
                            {
                                Frame = frame,
                                Value = value,
                                In = slopes[0],
                                Out = slopes[1],
                            });
                        }
                        break;
                    case AnimCurveType.Linear:
                        {
                            track.InterpolationType = InterpolationType.LINEAR;
                            var value = curve.Keys[i, 0] * valueScale + curve.Offset;
                            var delta = curve.Keys[i, 1] * valueScale;
                            track.Keys.Add(new Animation.KeyFrame()
                            {
                                Frame = frame,
                                Value = value,
                                Delta = delta,
                            });
                        }
                        break;
                    case AnimCurveType.StepBool:
                        {
                            track.InterpolationType = InterpolationType.STEP;
                            track.Keys.Add(new Animation.KeyFrame()
                            {
                                Frame = frame,
                                Value = curve.KeyStepBoolData[i] ? 1 : 0,
                            });
                        }
                        break;
                    default:
                        {
                            track.InterpolationType = InterpolationType.STEP;
                            var value = curve.Keys[i, 0] + curve.Offset;
                            if (valuesAsInts)
                                value = (int)curve.Keys[i, 0] + curve.Offset;

                            track.Keys.Add(new Animation.KeyFrame()
                            {
                                Frame = frame,
                                Value = value,
                            });
                        }
                        break;
                }
            }

            return track;
        }
    }
}
