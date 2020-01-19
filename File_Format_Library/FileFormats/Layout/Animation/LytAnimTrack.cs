using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Animations;

namespace LayoutBXLYT
{
    public class LytAnimTrack : STAnimationTrack
    {
        public void LoadKeyFrames(List<KeyFrame> keyFrames)
        {
            for (int i = 0; i < keyFrames?.Count; i++)
                KeyFrames.Add(new LytKeyFrame(keyFrames[i]));
        }
    }
}