using SELib;
using OpenTK;

namespace Switch_Toolbox.Library.Animations
{
    public class SEANIM
    {
        public static void SaveAnimation(string FileName, Animation anim, STSkeleton skeleton)
        {
            SEAnim seAnim = new SEAnim();

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

                    seAnim.AddTranslationKey(boneAnim.Text, frame, bone.pos.X, bone.pos.Y, bone.pos.Z);
                    seAnim.AddRotationKey(boneAnim.Text, frame, bone.rot.X, bone.rot.Y, bone.rot.Z, bone.rot.W);
                  //seAnim.AddScaleKey(boneAnim.Text, frame, scale.X, scale.Y, scale.Z);
                }
            }

            seAnim.Write(FileName);
        }
    }
}
