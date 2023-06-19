using SELib;
using OpenTK;
using System.Linq;
using System;

namespace Toolbox.Library.Animations
{
    public class SEANIM
    {
        public static Animation Read(string FileName, STSkeleton skeleton)
        {
            Animation anim = new Animation();
            var seanim = SEAnim.Read(FileName);
            anim.FrameCount = seanim.FrameCount;
            anim.CanLoop = seanim.Looping;

            foreach (var bone in seanim.Bones)
            {
                STBone genericBone = skeleton.GetBone(bone);
                if (genericBone != null)
                {
                    var boneAnim = new Animation.KeyNode(bone);
                    boneAnim.RotType = Animation.RotationType.EULER;
                    boneAnim.UseSegmentScaleCompensate = false;
                    anim.Bones.Add(boneAnim);

                    float PositionX = 0;
                    float PositionY = 0;
                    float PositionZ = 0;

                    float RotationX = 0;
                    float RotationY = 0;
                    float RotationZ = 0;

                    float ScaleX = 0;
                    float ScaleY = 0;
                    float ScaleZ = 0;

                    if (seanim.AnimType == AnimationType.Relative)
                    {
                        PositionX = genericBone.Position.X;
                        PositionY = genericBone.Position.Y;
                        PositionZ = genericBone.Position.Z;

                        RotationX = genericBone.EulerRotation.X;
                        RotationY = genericBone.EulerRotation.Y;
                        RotationZ = genericBone.EulerRotation.Z;

                        ScaleX = genericBone.Scale.X;
                        ScaleY = genericBone.Scale.Y;
                        ScaleZ = genericBone.Scale.Z;
                    }

                    System.Console.WriteLine(bone);

                    if (seanim.AnimationPositionKeys.ContainsKey(bone))
                    {
                        var translationKeys = seanim.AnimationPositionKeys[bone];
                        foreach (SEAnimFrame animFrame in translationKeys)
                        {
                            System.Console.WriteLine(animFrame.Frame + " T " + ((SELib.Utilities.Vector3)animFrame.Data).X);
                            System.Console.WriteLine(animFrame.Frame + " T " + ((SELib.Utilities.Vector3)animFrame.Data).Y);
                            System.Console.WriteLine(animFrame.Frame + " T " + ((SELib.Utilities.Vector3)animFrame.Data).Z);

                            boneAnim.XPOS.Keys.Add(new Animation.KeyFrame()
                            {
                                Value = (float)((SELib.Utilities.Vector3)animFrame.Data).X + PositionX,
                                Frame = animFrame.Frame,
                            });
                            boneAnim.YPOS.Keys.Add(new Animation.KeyFrame()
                            {
                                Value = (float)((SELib.Utilities.Vector3)animFrame.Data).Y + PositionY,
                                Frame = animFrame.Frame,
                            });
                            boneAnim.ZPOS.Keys.Add(new Animation.KeyFrame()
                            {
                                Value = (float)((SELib.Utilities.Vector3)animFrame.Data).Z + PositionZ,
                                Frame = animFrame.Frame,
                            });
                        }
                    }
                    if (seanim.AnimationRotationKeys.ContainsKey(bone))
                    {
                        var rotationnKeys = seanim.AnimationRotationKeys[bone];
                        foreach (SEAnimFrame animFrame in rotationnKeys)
                        {
                            var quat = ((SELib.Utilities.Quaternion)animFrame.Data);
                            var euler = STMath.ToEulerAngles(quat.X, quat.Y, quat.Z, quat.W);

                            System.Console.WriteLine(animFrame.Frame + " R " + euler.X);
                            System.Console.WriteLine(animFrame.Frame + " R " + euler.Y);
                            System.Console.WriteLine(animFrame.Frame + " R " + euler.Z);

                            boneAnim.XROT.Keys.Add(new Animation.KeyFrame()
                            {
                                Value = euler.X + RotationX,
                                Frame = animFrame.Frame,
                            });
                            boneAnim.YROT.Keys.Add(new Animation.KeyFrame()
                            {
                                Value = euler.Y + RotationY,
                                Frame = animFrame.Frame,
                            });
                            boneAnim.ZROT.Keys.Add(new Animation.KeyFrame()
                            {
                                Value = euler.Z + RotationZ,
                                Frame = animFrame.Frame,
                            });
                        }
                    }
                    if (seanim.AnimationScaleKeys.ContainsKey(bone))
                    {
                        var scaleKeys = seanim.AnimationScaleKeys[bone];
                        foreach (SEAnimFrame animFrame in scaleKeys)
                        {
                            System.Console.WriteLine(animFrame.Frame + " S " + ((SELib.Utilities.Vector3)animFrame.Data).X);
                            System.Console.WriteLine(animFrame.Frame + " S " + ((SELib.Utilities.Vector3)animFrame.Data).Y);
                            System.Console.WriteLine(animFrame.Frame + " S " + ((SELib.Utilities.Vector3)animFrame.Data).Z);

                            boneAnim.XSCA.Keys.Add(new Animation.KeyFrame()
                            {
                                Value = (float)((SELib.Utilities.Vector3)animFrame.Data).X + ScaleX,
                                Frame = animFrame.Frame,
                            });
                            boneAnim.YSCA.Keys.Add(new Animation.KeyFrame()
                            {
                                Value = (float)((SELib.Utilities.Vector3)animFrame.Data).Y + ScaleY,
                                Frame = animFrame.Frame,
                            });
                            boneAnim.ZSCA.Keys.Add(new Animation.KeyFrame()
                            {
                                Value = (float)((SELib.Utilities.Vector3)animFrame.Data).Z + ScaleZ,
                                Frame = animFrame.Frame,
                            });
                        }
                    }
                    else
                    {
                        boneAnim.XSCA.Keys.Add(new Animation.KeyFrame()
                        {
                            Value = 1,
                            Frame = 0,
                        });
                        boneAnim.YSCA.Keys.Add(new Animation.KeyFrame()
                        {
                            Value = 1,
                            Frame = 0,
                        });
                        boneAnim.ZSCA.Keys.Add(new Animation.KeyFrame()
                        {
                            Value = 1,
                            Frame = 0,
                        });
                    }
                }
            }



            return anim;
        }

        private static void WriteKey(Animation.KeyGroup keys)
        {
            foreach (Animation.KeyFrame key in keys.Keys)
            {

            }
        }

        public static void Save(STSkeletonAnimation anim, string FileName)
        {
            STSkeleton skeleton = anim.GetActiveSkeleton();

            SEAnim seAnim = new SEAnim();
            seAnim.Looping = anim.Loop;
            seAnim.AnimType = AnimationType.Absolute;

            anim.SetFrame(0);
            for (int frame = 0; frame < Math.Max(1, anim.FrameCount); frame++)
            {
                anim.SetFrame(frame);
                anim.NextFrame();

                foreach (STAnimGroup boneAnim in anim.AnimGroups)
                {
                    if (boneAnim.GetTracks().Any(x => x.HasKeys))
                    {
                        STBone bone = skeleton.GetBone(boneAnim.Name);
                        if (bone == null) continue;

                        Vector3 position = bone.GetPosition();
                        Quaternion rotation = bone.GetRotation();
                        Vector3 scale = bone.GetScale();

                        seAnim.AddTranslationKey(boneAnim.Name, frame, position.X, position.Y, position.Z);
                        seAnim.AddRotationKey(boneAnim.Name, frame, rotation.X, rotation.Y, rotation.Z, rotation.W);
                        seAnim.AddScaleKey(boneAnim.Name, frame, scale.X, scale.Y, scale.Z);
                    }
                }
            }

            seAnim.Write(FileName);
        }

        public static void SaveAnimation(string FileName, Animation anim, STSkeleton skeleton)
        {
            anim.SetFrame(anim.FrameCount - 1); //from last frame
            for (int f = 0; f < anim.FrameCount; ++f) //go through each frame with nextFrame
                anim.NextFrame(skeleton);
            anim.NextFrame(skeleton);  //go on first frame

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
                    if (boneAnim.HasKeyedFrames(frame))
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
            }

            seAnim.Write(FileName);
        }
    }
}
