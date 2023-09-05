using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using OpenTK;
using System.Text;
using System.Threading;
using System.Globalization;

namespace Toolbox.Library.Animations
{
    //Todo rewrite this
    //Currently from forge
    //https://raw.githubusercontent.com/jam1garner/Smash-Forge/master/Smash%20Forge/Filetypes/SMD.cs
    public class SMD
    {
        public STSkeleton Bones;
        public Animation Animation; // todo

        public SMD()
        {
            Bones = new STSkeleton();
        }

        public SMD(string fname)
        {
            Read(fname);
        }

        public void Read(string fname)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            StreamReader reader = File.OpenText(fname);
            string line;

            string current = "";

            Bones = new STSkeleton();
            Dictionary<int, STBone> BoneList = new Dictionary<int, STBone>();

            int time = 0;
            while ((line = reader.ReadLine()) != null)
            {
                line = Regex.Replace(line, @"\s+", " ");
                string[] args = line.Replace(";", "").TrimStart().Split(' ');

                if (args[0].Equals("triangles") || args[0].Equals("end") || args[0].Equals("skeleton") || args[0].Equals("nodes"))
                {
                    current = args[0];
                    continue;
                }

                if (current.Equals("nodes"))
                {
                    int id = int.Parse(args[0]);
                    STBone b = new STBone(Bones);
                    b.Text = args[1].Replace('"', ' ').Trim();
                    int s = 2;
                    while (args[s].Contains("\""))
                        b.Text += args[s++];
                    b.parentIndex = int.Parse(args[s]);
                    BoneList.Add(id, b);
                }

                if (current.Equals("skeleton"))
                {
                    if (args[0].Contains("time"))
                        time = int.Parse(args[1]);
                    else
                    {
                        if (time == 0)
                        {
                            STBone b = BoneList[int.Parse(args[0])];
                            b.Position = new Vector3(
                                float.Parse(args[1]),
                                float.Parse(args[2]),
                                float.Parse(args[3]));
                            b.EulerRotation = new Vector3(
                                float.Parse(args[4]),
                                float.Parse(args[5]),
                                float.Parse(args[6]));
                            b.Scale = Vector3.One;

                            b.pos = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                            b.rot = STSkeleton.FromEulerAngles(float.Parse(args[6]), float.Parse(args[5]), float.Parse(args[4]));

                            Bones.bones.Add(b);

                            if (b.parentIndex != -1)
                                b.parentIndex = Bones.bones.IndexOf(BoneList[b.parentIndex]);
                        }
                    }
                }
            }
            Bones.reset();

            Thread.CurrentThread.CurrentCulture = CultureInfo.DefaultThreadCurrentCulture;
        }

        public void Save(string FileName)
        {
           var culture = new CultureInfo("en-US");

            StringBuilder o = new StringBuilder();

            o.AppendLine("version 1");

            if (Bones != null)
            {
                o.AppendLine("nodes");
                for (int i = 0; i < Bones.bones.Count; i++)
                    o.AppendLine("  " + i + " \"" + Bones.bones[i].Text + "\" " + Bones.bones[i].parentIndex);
                o.AppendLine("end");

                o.AppendLine("skeleton");
                o.AppendLine("time 0");
                for (int i = 0; i < Bones.bones.Count; i++)
                {
                    STBone b = Bones.bones[i];
                    o.AppendFormat(culture, "{0} {1} {2} {3} {4} {5} {6}\n", i, 
                        b.Position.X,
                        b.Position.Y,
                        b.Position.Z,
                        b.EulerRotation.X,
                        b.EulerRotation.Y,
                        b.EulerRotation.Z);
                }
                o.AppendLine("end");
            }

            File.WriteAllText(FileName, o.ToString());
        }

        public static Animation Read(string fname,STSkeleton v)
        {
            Animation a = new Animation();

            StreamReader reader = File.OpenText(fname);
            string line;

            string current = "";
            bool readBones = false;
            int frame = 0, prevframe = 0;
            Animation.KeyFrame k = new Animation.KeyFrame();

            STSkeleton vbn = v;
            if (v != null && v.bones.Count == 0)
            {
                readBones = true;
            }
            else
                vbn = new STSkeleton();

            while ((line = reader.ReadLine()) != null)
            {
                line = Regex.Replace(line, @"\s+", " ");
                string[] args = line.Replace(";", "").TrimStart().Split(' ');

                if (args[0].Equals("nodes") || args[0].Equals("skeleton") || args[0].Equals("end") || args[0].Equals("time"))
                {
                    current = args[0];
                    if (args.Length > 1)
                    {
                        prevframe = frame;
                        frame = int.Parse(args[1]);

                        /*if (frame != prevframe + 1) {
							Console.WriteLine ("Needs interpolation " + frame);
						}*/

                        k = new Animation.KeyFrame();
                        k.Frame = frame;
                        //a.addKeyframe(k);
                    }
                    continue;
                }

                if (current.Equals("nodes"))
                {
                    STBone b = new STBone(vbn);
                    b.Text = args[1].Replace("\"", "");
                    b.parentIndex = int.Parse(args[2]);
                    //b.children = new System.Collections.Generic.List<int> ();
                    vbn.bones.Add(b);
                    Animation.KeyNode node = new Animation.KeyNode(b.Text);
                    a.Bones.Add(node);
                }

                if (current.Equals("time"))
                {
                    // reading the skeleton if this isn't an animation
                    if (readBones && frame == 0)
                    {
                        STBone b = vbn.bones[int.Parse(args[0])];
                        b.Position = new Vector3(
                            float.Parse(args[1]),
                            float.Parse(args[2]),
                            float.Parse(args[3]));
                        b.EulerRotation = new Vector3(
                            float.Parse(args[4]),
                            float.Parse(args[5]),
                            float.Parse(args[6]));
                        b.Scale = Vector3.One;

                        b.pos = new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        b.rot = STSkeleton.FromEulerAngles(float.Parse(args[6]), float.Parse(args[5]), float.Parse(args[4]));

                        if (b.parentIndex != -1)
                            vbn.bones[b.parentIndex].Nodes.Add(b);
                    }
                    Animation.KeyNode bone = a.GetBone(vbn.bones[int.Parse(args[0])].Text);
                    bone.RotType = Animation.RotationType.EULER;

                    Animation.KeyFrame n = new Animation.KeyFrame();
                    n.Value = float.Parse(args[1]);
                    n.Frame = frame;
                    bone.XPOS.Keys.Add(n);

                    n = new Animation.KeyFrame();
                    n.Value = float.Parse(args[2]);
                    n.Frame = frame;
                    bone.YPOS.Keys.Add(n);

                    n = new Animation.KeyFrame();
                    n.Value = float.Parse(args[3]);
                    n.Frame = frame;
                    bone.ZPOS.Keys.Add(n);

                    n = new Animation.KeyFrame();
                    n.Value = float.Parse(args[4]);
                    n.Frame = frame;
                    bone.XROT.Keys.Add(n);

                    n = new Animation.KeyFrame();
                    n.Value = float.Parse(args[5]);
                    n.Frame = frame;
                    bone.YROT.Keys.Add(n);

                    n = new Animation.KeyFrame();
                    n.Value = float.Parse(args[6]);
                    n.Frame = frame;
                    bone.ZROT.Keys.Add(n);

                    if (args.Length > 7)
                    {
                        n = new Animation.KeyFrame();
                        n.Value = float.Parse(args[7]);
                        n.Frame = frame;
                        bone.XSCA.Keys.Add(n);

                        n = new Animation.KeyFrame();
                        n.Value = float.Parse(args[8]);
                        n.Frame = frame;
                        bone.YSCA.Keys.Add(n);

                        n = new Animation.KeyFrame();
                        n.Value = float.Parse(args[9]);
                        n.Frame = frame;
                        bone.ZSCA.Keys.Add(n);
                    }
                    else
                    {
                        bone.XSCA.Keys.Add(new Animation.KeyFrame()
                        {
                            Value = 1.0f,
                            Frame = frame,
                        });
                        bone.YSCA.Keys.Add(new Animation.KeyFrame()
                        {
                            Value = 1.0f,
                            Frame = frame,
                        });
                        bone.ZSCA.Keys.Add(new Animation.KeyFrame()
                        {
                            Value = 1.0f,
                            Frame = frame,
                        });
                    }
                }
            }

            a.FrameCount = frame;
            vbn.update();

            return a;
        }

        public static void Save(STSkeletonAnimation anim, String Fname)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            STSkeleton Skeleton = anim.GetActiveSkeleton();

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@Fname))
            {
                file.WriteLine("version 1");

                file.WriteLine("nodes");
                foreach (STBone b in Skeleton.bones)
                {
                    file.WriteLine(Skeleton.bones.IndexOf(b) + " \"" + b.Text + "\" " + b.parentIndex);
                }
                file.WriteLine("end");

                file.WriteLine("skeleton");
                anim.SetFrame(0);
                for (int i = 0; i <= Math.Max(1, anim.FrameCount); i++)
                {
                    anim.SetFrame(i);
                    anim.NextFrame();

                    file.WriteLine($"time {i}");

                    foreach (var sb in anim.AnimGroups)
                    {
                        STBone b = Skeleton.GetBone(sb.Name);
                        if (b == null) continue;
                        Vector3 eul = STMath.ToEulerAngles(b.rot);
                        Vector3 scale = b.GetScale();
                        Vector3 translate = b.GetPosition();

                        file.WriteLine($"{ Skeleton.bones.IndexOf(b)} {translate.X} {translate.Y} {translate.Z} {eul.X} {eul.Y} {eul.Z}");
                    }

                }
                file.WriteLine("end");

                file.Close();
            }
        }

        public static void Save(Animation anim, STSkeleton Skeleton, String Fname)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@Fname))
            {
                file.WriteLine("version 1");

                file.WriteLine("nodes");
                foreach (STBone b in Skeleton.bones)
                {
                    file.WriteLine(Skeleton.bones.IndexOf(b) + " \"" + b.Text + "\" " + b.parentIndex);
                }
                file.WriteLine("end");

                file.WriteLine("skeleton");
                anim.SetFrame(0);
                for (int i = 0; i < anim.FrameCount; i++)
                {
                    anim.NextFrame(Skeleton, false, true);

                    file.WriteLine($"time {i}");

                    foreach (Animation.KeyNode sb in anim.Bones)
                    {
                        STBone b = Skeleton.GetBone(sb.Text);
                        if (b == null) continue;
                        Vector3 eul = STMath.ToEulerAngles(b.rot);
                        Vector3 scale = b.GetScale();
                        Vector3 translate = b.GetPosition();

                        file.WriteLine($"{ Skeleton.bones.IndexOf(b)} {translate.X} {translate.Y} {translate.Z} {eul.X} {eul.Y} {eul.Z}");
                    }

                }
                file.WriteLine("end");

                file.Close();
            }
        }
    }
}
