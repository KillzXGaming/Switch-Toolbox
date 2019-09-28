using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.Animations;

namespace LayoutBXLYT
{
    public class LytKeyFrame : STKeyFrame
    {
        public KeyFrame bxlytKeyframe;

        public LytKeyFrame(KeyFrame key)
        {
            bxlytKeyframe = key;
        }

        public override float Value => bxlytKeyframe.Value;
        public override float Slope => bxlytKeyframe.Slope;
        public override float Frame => bxlytKeyframe.Frame;
    }
}
