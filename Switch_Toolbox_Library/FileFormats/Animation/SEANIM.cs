using SELib;
using OpenTK;

namespace Switch_Toolbox.Library.Animations
{
    public class SEANIM
    {
        public static Animation Read(string FileName)
        {
            Animation anim = new Animation();
            var seanim = SEAnim.Read(FileName);

            return anim;
        }

        private static void WriteKey(Animation.KeyGroup keys)
        {
            foreach (Animation.KeyFrame key in keys.Keys)
            {

            }
        }

        public static void SaveAnimation(string FileName, Animation anim, STSkeleton skeleton)
        {
            anim.SetFrame(anim.FrameCount - 1); //from last frame
            for (int f = 0; f < anim.FrameCount; ++f) //go through each frame with nextFrame
                anim.NextFrame(skeleton);
            anim.NextFrame(skeleton);  //go on first frame

            foreach (STBone b in skeleton.getBoneTreeOrder())
            {
                if (anim.HasBone(b.Text))
                {
                    Animation.KeyNode n = anim.GetBone(b.Text);

                    if (n.XPOS.HasAnimation())
                        WriteKey(n.XPOS);
                    if (n.YPOS.HasAnimation())
                        WriteKey(n.YPOS);
                    if (n.ZPOS.HasAnimation())
                        WriteKey(n.ZPOS);
                    if (n.XROT.HasAnimation())
                        WriteKey(n.XROT);
                    if (n.YROT.HasAnimation())
                        WriteKey(n.YROT);
                    if (n.ZROT.HasAnimation())
                        WriteKey(n.ZROT);
                    if (n.XSCA.HasAnimation())
                        WriteKey(n.XSCA);
                    if (n.YSCA.HasAnimation())
                        WriteKey(n.YSCA);
                    if (n.ZSCA.HasAnimation())
                        WriteKey(n.ZSCA);
                }
            }

            SEAnim seAnim = new SEAnim();
            seAnim.Looping = anim.CanLoop;
            seAnim.AnimType = AnimationType.Absolute;
            //Reset active animation to 0
            anim.SetFrame(0);
            for (int frame = 0; frame < anim.FrameCount; frame++)
            {
                anim.NextFrame(skeleton, false, true);

                foreach (Animation.KeyNode boneAnim in anim.Bones)
                {
                    STBone bone = skeleton.GetBone(boneAnim.Text);
                    if (bone == null) continue;

                    Vector3 position = bone.GetPosition();
                    Quaternion rotation = bone.GetRotation();
                    Vector3 scale = bone.GetScale();

                    seAnim.AddTranslationKey(boneAnim.Text, frame, position.X, position.Y, position.Z);
                    seAnim.AddRotationKey(boneAnim.Text, frame, rotation.X, rotation.Y, rotation.Z, rotation.W);
                    seAnim.AddScaleKey(boneAnim.Text, frame, scale.X, scale.Y, scale.Z);
                }
            }

            seAnim.Write(FileName);
        }
    }
}
