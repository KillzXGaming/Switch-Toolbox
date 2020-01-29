using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.NodeWrappers;
using Toolbox.Library.Animations;
using SPICA.Formats.CtrH3D.Animation;
using FirstPlugin.CtrLibrary.Forms;
using System.Windows.Forms;

namespace FirstPlugin.CtrLibrary
{
    public class H3DSkeletalAnimWrapper : STGenericWrapper
    {
        public BCH BCHParent;

        public H3DAnimationController Animation = new H3DAnimationController();

        private H3DAnimation H3DAnimation;

        public override void OnClick(TreeView treeview)
        {
            var editor = BCHParent.LoadEditor<BCHAnimationEditor>();
            editor.LoadAnimation(H3DAnimation);
        }

        public H3DSkeletalAnimWrapper(H3DAnimation h3dAnim, BCH bch)
        {
            ImageKey = "anim";
            SelectedImageKey = "anim";

            BCHParent = bch;
            Text = h3dAnim.Name;

            H3DAnimation = h3dAnim;
            Animation.FrameCount = h3dAnim.FramesCount;
            Animation.Name = h3dAnim.Name;
            Animation.Loop = h3dAnim.AnimationFlags.HasFlag(H3DAnimationFlags.IsLooping);
            foreach (var bone in h3dAnim.Elements)
                Animation.AnimGroups.Add(new H3DBoneAnimGroup(bone));
        }
    }

    public class H3DAnimationController : STAnimation
    {
        public override void NextFrame()
        {
          
        }
    }

    public class H3DBoneAnimGroup : STAnimGroup
    {
        public STAnimationTrack TranslateX { get; set; } = new STAnimationTrack();
        public STAnimationTrack TranslateY { get; set; } = new STAnimationTrack();
        public STAnimationTrack TranslateZ { get; set; } = new STAnimationTrack();

        public STAnimationTrack RotationX { get; set; } = new STAnimationTrack();
        public STAnimationTrack RotationY { get; set; } = new STAnimationTrack();
        public STAnimationTrack RotationZ { get; set; } = new STAnimationTrack();

        public STAnimationTrack ScaleX { get; set; }= new STAnimationTrack();
        public STAnimationTrack ScaleY { get; set; } = new STAnimationTrack();
        public STAnimationTrack ScaleZ { get; set; } = new STAnimationTrack();

        public H3DBoneAnimGroup(H3DAnimationElement element)
        {
            Name = element.Name;
        }

        public override List<STAnimationTrack> GetTracks()
        {
            List<STAnimationTrack> tracks = new List<STAnimationTrack>();
            tracks.Add(TranslateX);
            tracks.Add(TranslateY);
            tracks.Add(TranslateZ);
            return base.GetTracks();
        }
    }
}
