using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.Animations
{
    public enum VisibiltyAnimType
    {
        Bone,
        Material
    }

    public class VisibilityAnimation : Animation
    {
        VisibiltyAnimType visibiltyType = VisibiltyAnimType.Bone;

        public bool[] BaseValues { get; set; }

        public List<string> BoneNames = new List<string>();
        public List<string> MaterialNames = new List<string>();

        public List<BooleanKeyGroup> Values = new List<BooleanKeyGroup>();

        public virtual void NextFrame(Viewport viewport)
        {
            if (Frame >= FrameCount) return;
        }
    }
}
