using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.Animations
{
    public class STAnimationTrack
    {
        public STInterpoaltionType InterpolationType { get; set; }

        public List<STKeyFrame> KeyFrames = new List<STKeyFrame>();

        public bool HasKeys =>  KeyFrames.Count > 0;

        //Key frame setup based on
        //https://github.com/gdkchan/SPICA/blob/42c4181e198b0fd34f0a567345ee7e75b54cb58b/SPICA/Formats/CtrH3D/Animation/H3DFloatKeyFrameGroup.cs
        public float GetFrameValue(float frame, float startFrame = 0)
        {
            if (KeyFrames.Count == 0) return 0;
            if (KeyFrames.Count == 1) return KeyFrames[0].Value;

            STKeyFrame LK = KeyFrames.First();
            STKeyFrame RK = KeyFrames.Last();

            float Frame = frame - startFrame;

            foreach (STKeyFrame keyFrame in KeyFrames)
            {
                if (keyFrame.Frame <= Frame) LK = keyFrame;
                if (keyFrame.Frame >= Frame && keyFrame.Frame < RK.Frame) RK = keyFrame;
            }

            if (LK.Frame != RK.Frame)
            {
                float FrameDiff = Frame - LK.Frame;
                float Weight = FrameDiff / (RK.Frame - LK.Frame);

                switch (InterpolationType)
                {
                    case STInterpoaltionType.Constant: return LK.Value;
                    case STInterpoaltionType.Step: return LK.Value;
                    case STInterpoaltionType.Linear: return InterpolationHelper.Lerp(LK.Value, RK.Value, Weight);
                    case STInterpoaltionType.Hermite:
                        return InterpolationHelper.Herp(
                            LK.Value, RK.Value,
                            LK.Slope, RK.Slope,
                            FrameDiff,
                            Weight);
                }
            }

            return LK.Value;
        }
    }
}
