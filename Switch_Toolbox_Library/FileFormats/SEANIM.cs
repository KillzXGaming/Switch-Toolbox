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
                anim.NextFrame(skeleton);

                //Add frames to the playing animation
                anim.Frame += 1f;

                //Reset it when it reaches the total frame count
                if (anim.Frame >= anim.FrameCount)
                    anim.Frame = 0;

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
