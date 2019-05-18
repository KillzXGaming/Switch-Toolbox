using SELib;
using OpenTK;

namespace Switch_Toolbox.Library.Animations
{
    public class SEANIM
    {
        public static void SaveAnimation(string FileName, Animation anim, STSkeleton skeleton)
        {
            SEAnim seAnim = new SEAnim();
            seAnim.Looping = anim.CanLoop;

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
