using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Animations;

namespace FirstPlugin
{
    public class GfbanimKeyFrameLoader
    {
        public static STAnimationTrack[] LoadRotationTrack(Gfbanim.DynamicQuatTrack dynamicTrack)
        {
            STAnimationTrack[] tracks = new STAnimationTrack[3];
            tracks[0] = new STAnimationTrack(STInterpoaltionType.Linear);
            tracks[1] = new STAnimationTrack(STInterpoaltionType.Linear);
            tracks[2] = new STAnimationTrack(STInterpoaltionType.Linear);

            for (int i = 0; i < dynamicTrack.ValuesLength; i++)
            {
                var quat = dynamicTrack.Values(i).Value;
                tracks[0].KeyFrames.Add(new STKeyFrame(i, ConvertRotation(quat.X)));
                tracks[1].KeyFrames.Add(new STKeyFrame(i, ConvertRotation(quat.Y)));
                tracks[2].KeyFrames.Add(new STKeyFrame(i, ConvertRotation(quat.Z)));
            }
            return tracks;
        }

        public static float ConvertRotation(ushort val)
        {
            return val;
        }

        public static STAnimationTrack[] LoadVectorTrack(Gfbanim.DynamicVectorTrack dynamicTrack)
        {
            STAnimationTrack[] tracks = new STAnimationTrack[3];
            tracks[0] = new STAnimationTrack(STInterpoaltionType.Linear);
            tracks[1] = new STAnimationTrack(STInterpoaltionType.Linear);
            tracks[2] = new STAnimationTrack(STInterpoaltionType.Linear);

            for (int i = 0; i < dynamicTrack.ValuesLength; i++)
            {
                var vec = dynamicTrack.Values(i).Value;
                tracks[0].KeyFrames.Add(new STKeyFrame(i, vec.X));
                tracks[1].KeyFrames.Add(new STKeyFrame(i, vec.Y));
                tracks[2].KeyFrames.Add(new STKeyFrame(i, vec.Z));
            }
            return tracks;
        }

        public static STAnimationTrack[] LoadVectorTrack(Gfbanim.FramedVectorTrack16 framedTrack)
        {
            ushort[] frames = framedTrack.GetFramesArray();

            STAnimationTrack[] tracks = new STAnimationTrack[3];
            tracks[0] = new STAnimationTrack(STInterpoaltionType.Linear);
            tracks[1] = new STAnimationTrack(STInterpoaltionType.Linear);
            tracks[2] = new STAnimationTrack(STInterpoaltionType.Linear);

            for (int i = 0; i < framedTrack.ValuesLength; i++)
            {
                var vec = framedTrack.Values(i).Value;
                int frame = i;

                if (i < frames?.Length) frame = frames[i];

                tracks[0].KeyFrames.Add(new STKeyFrame(frame, vec.X));
                tracks[1].KeyFrames.Add(new STKeyFrame(frame, vec.Y));
                tracks[2].KeyFrames.Add(new STKeyFrame(frame, vec.Z));
            }
            return tracks;
        }

        public static STAnimationTrack[] LoadVectorTrack(Gfbanim.FramedVectorTrack8 framedTrack)
        {
            byte[] frames = framedTrack.GetFramesArray();

            STAnimationTrack[] tracks = new STAnimationTrack[3];
            tracks[0] = new STAnimationTrack(STInterpoaltionType.Linear);
            tracks[1] = new STAnimationTrack(STInterpoaltionType.Linear);
            tracks[2] = new STAnimationTrack(STInterpoaltionType.Linear);

            for (int i = 0; i < framedTrack.ValuesLength; i++)
            {
                var vec = framedTrack.Values(i).Value;
                int frame = i;

                if (i < frames?.Length) frame = frames[i];

                tracks[0].KeyFrames.Add(new STKeyFrame(frame, vec.X));
                tracks[1].KeyFrames.Add(new STKeyFrame(frame, vec.Y));
                tracks[2].KeyFrames.Add(new STKeyFrame(frame, vec.Z));
            }
            return tracks;
        }

        public static STAnimationTrack LoadBooleanTrack(Gfbanim.DynamicBooleanTrack framedTrack)
        {
            STAnimationTrack track = new STAnimationTrack(STInterpoaltionType.Step);
            for (int i = 0; i < framedTrack.ValuesLength; i++)
            {
                ushort val = framedTrack.Values(i);
                int frame = i;

                //Visibily is handled by bits from a ushort
                track.KeyFrames.Add(new STKeyFrame(frame, val));
            }
            return track;
        }

        public static STAnimationTrack LoadBooleanTrack(Gfbanim.FramedBooleanTrack framedTrack)
        {
            ushort[] frames = framedTrack.GetFramesArray();

            STAnimationTrack track = new STAnimationTrack(STInterpoaltionType.Step);
            for (int i = 0; i < framedTrack.ValuesLength; i++)
            {
                ushort val = framedTrack.Values(i);
                int frame = i;

                if (i < frames?.Length) frame = frames[i];

                //Visibily is handled by bits from a ushort
                track.KeyFrames.Add(new STKeyFrame(frame, val));
            }
            return track;
        }

        public static STAnimationTrack[] LoadRotationTrack(Gfbanim.FramedQuatTrack8 framedTrack)
        {
            byte[] frames = framedTrack.GetFramesArray();

            STAnimationTrack[] tracks = new STAnimationTrack[3];
            tracks[0] = new STAnimationTrack(STInterpoaltionType.Linear);
            tracks[1] = new STAnimationTrack(STInterpoaltionType.Linear);
            tracks[2] = new STAnimationTrack(STInterpoaltionType.Linear);

            for (int i = 0; i < framedTrack.ValuesLength; i++)
            {
                var quat = framedTrack.Values(i).Value;
                int frame = i;

                if (i < frames?.Length) frame = frames[i];

                tracks[0].KeyFrames.Add(new STKeyFrame(frame, ConvertRotation(quat.X)));
                tracks[1].KeyFrames.Add(new STKeyFrame(frame, ConvertRotation(quat.Y)));
                tracks[2].KeyFrames.Add(new STKeyFrame(frame, ConvertRotation(quat.Z)));
            }
            return tracks;
        }

        public static STAnimationTrack[] LoadRotationTrack(Gfbanim.FramedQuatTrack16 framedTrack)
        {
            ushort[] frames = framedTrack.GetFramesArray();

            STAnimationTrack[] tracks = new STAnimationTrack[3];
            tracks[0] = new STAnimationTrack(STInterpoaltionType.Linear);
            tracks[1] = new STAnimationTrack(STInterpoaltionType.Linear);
            tracks[2] = new STAnimationTrack(STInterpoaltionType.Linear);

            for (int i = 0; i < framedTrack.ValuesLength; i++)
            {
                var quat = framedTrack.Values(i).Value;
                int frame = i;

                if (i < frames?.Length) frame = frames[i];

                tracks[0].KeyFrames.Add(new STKeyFrame(frame, ConvertRotation(quat.X)));
                tracks[1].KeyFrames.Add(new STKeyFrame(frame, ConvertRotation(quat.Y)));
                tracks[2].KeyFrames.Add(new STKeyFrame(frame, ConvertRotation(quat.Z)));
            }
            return tracks;
        }
    }
}
