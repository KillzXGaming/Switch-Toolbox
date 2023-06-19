using System;
using System.IO;
using System.Collections.Generic;
using OpenTK;
using System.Windows.Forms;

namespace Toolbox.Library.Animations
{
    //Todo rewrite this
    //Currently from forge
    //https://github.com/jam1garner/Smash-Forge/blob/3a5b770a96b2ba7e67ff3912ca23941851f6d9eb/Smash%20Forge/Filetypes/Animation/ANIM.cs
    public class ANIM
	{
        static float Deg2Rad = (float)(Math.PI / 180f);
        static float Rad2Deg = (float)(180f / Math.PI);

        private class AnimHeader
        {
            public string animVersion;
            public string mayaVersion;
            public float startTime;
            public float endTime;
            public float startUnitless;
            public float endUnitless;
            public string timeUnit;
            public string linearUnit;
            public string angularUnit;

            public AnimHeader()
            {
                animVersion = "1.1";
                mayaVersion = "2015";
                startTime = 0;
                endTime = 0;
                startUnitless = 0;
                endUnitless = 0;
                timeUnit = "ntscf";
                linearUnit = "cm";
                angularUnit = "deg";
            }
        }

        private class AnimKey{
			public float input, output;
			public string intan, outtan;
			public float t1 = 0, w1 = 1;
		}

		private class AnimData{
			public string type, input, output, preInfinity, postInfinity;
			public bool weighted = false;
			public List<AnimKey> keys = new List<AnimKey>();

			public float getValue(int frame){
				AnimKey f1 = null, f2 = null;
				for (int i = 0; i < keys.Count-1; i++) {
					if ((keys [i].input-1 <= frame && keys [i + 1].input-1 >= frame)) {
						f1 = keys [i];
						f2 = keys [i + 1];
						break;
					}
				}
				if (f1 == null) {
					if (keys.Count <= 1) {
						return keys [0].output;
					} else {
						f1 = keys [keys.Count - 2];
						f2 = keys [keys.Count - 1];
					}
				}

				return Animation.HermiteInterpolate (frame+1, f1.input, f2.input, weighted ? f1.t1 : 0, weighted ? f2.t1 : 0, f1.output, f2.output);
			}
		}

		private class AnimBone{
			public string name;
			public List<AnimData> atts = new List<AnimData>();
		}

		public static Animation read(string filename, STSkeleton vbn){
			StreamReader reader = File.OpenText(filename);
			string line;

			bool isHeader = true;

			string angularUnit, linearUnit, timeUnit;
			int startTime = 0;
			int endTime = 0;
			List<AnimBone> bones = new List<AnimBone>();
			Animation.KeyNode current = null;
			Animation.KeyFrame att = new Animation.KeyFrame();
			bool inKeys = false;
            string type = "";
            
            Animation a = new Animation(filename);

            while ((line = reader.ReadLine()) != null) {
				string[] args = line.Replace (";", "").TrimStart().Split (' ');

				if (isHeader) {
					if (args [0].Equals ("anim"))
						isHeader = false;
					else if (args [0].Equals ("angularUnit"))
						angularUnit = args [1];
					else if (args [0].Equals ("endTime"))
						endTime = (int)Math.Ceiling(float.Parse (args [1]));
					else if (args [0].Equals ("startTime"))
						startTime = (int)Math.Ceiling(float.Parse (args [1]));
				}

				if (!isHeader) {

					if (inKeys) {
						if(args[0].Equals("}")){
							inKeys = false;
							continue;
						}
						Animation.KeyFrame k = new Animation.KeyFrame ();
                        //att.keys.Add (k);
                        if (type.Contains("translate"))
                        {
                            if (type.Contains("X")) current.XPOS.Keys.Add(k);
                            if (type.Contains("Y")) current.YPOS.Keys.Add(k);
                            if (type.Contains("Z")) current.ZPOS.Keys.Add(k);
                        }
                        if (type.Contains("rotate"))
                        {
                            if (type.Contains("X")) current.XROT.Keys.Add(k);
                            if (type.Contains("Y")) current.YROT.Keys.Add(k);
                            if (type.Contains("Z")) current.ZROT.Keys.Add(k);
                        }
                        if (type.Contains("scale"))
                        {
                            if (type.Contains("X")) current.XSCA.Keys.Add(k);
                            if (type.Contains("Y")) current.YSCA.Keys.Add(k);
                            if (type.Contains("Z")) current.ZSCA.Keys.Add(k);
                        }
                        k.Frame = float.Parse (args [0])-1;
						k.Value = float.Parse (args [1]);
                        if (type.Contains("rotate"))
                        {
                            k.Value *= Deg2Rad;
                        }
                            //k.intan = (args [2]);
                            //k.outtan = (args [3]);
                        if (args.Length > 7 && att.Weighted)
                        {
                            k.In = float.Parse(args[7]) * Deg2Rad;
                            k.Out = float.Parse(args[8]) * Deg2Rad;
                        }
                    }

					if (args [0].Equals ("anim")) {
						inKeys = false;
						if (args.Length == 5) {
							//TODO: finish this type
							// can be name of attribute
						}
						if (args.Length == 7) {
							// see of the bone of this attribute exists
							current = null;
							foreach (Animation.KeyNode b in a.Bones)
								if (b.Text.Equals (args [3])) {
									current = b;
									break;
								}
							if (current == null) {
								current = new Animation.KeyNode (args[3]);
                                current.RotType = Animation.RotationType.EULER;
								a.Bones.Add (current);
							}
							current.Text = args [3];

							att = new Animation.KeyFrame();
                            att.InterType = InterpolationType.HERMITE;
							type = args [2];
							//current.Nodes.Add (att);

							// row child attribute aren't needed here
						}
					}

                    /*if (args [0].Equals ("input"))
						att.input = args [1];
					if (args [0].Equals ("output"))
						att.output = args [1];
					if (args [0].Equals ("preInfinity"))
						att.preInfinity = args [1];
					if (args [0].Equals ("postInfinity"))
						att.postInfinity = args [1];*/
                    if (args[0].Equals("weighted"))
                        att.Weighted = args[1].Equals("1");


                    // begining keys section
                    if (args [0].Contains ("keys")) {
						inKeys = true;
					}
				}
			}

            a.FrameCount = endTime-1;

            reader.Close();
			return a;
		}

        public static void CreateANIM(string fname, Animation a, STSkeleton vbn)
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@fname))
            {
                float frameCount = Math.Max(1, a.FrameCount);

                AnimHeader header = new AnimHeader();
                file.WriteLine("animVersion " + header.animVersion + ";");
                file.WriteLine("mayaVersion " + header.mayaVersion + ";");
                file.WriteLine("timeUnit " + header.timeUnit + ";");
                file.WriteLine("linearUnit " + header.linearUnit + ";");
                file.WriteLine("angularUnit " + header.angularUnit + ";");
                file.WriteLine("startTime " + 1 + ";");
                file.WriteLine("endTime " + frameCount + ";");

                a.SetFrame(frameCount - 1); //from last frame
                for (int li = 0; li < frameCount; ++li) //go through each frame with nextFrame
                    a.NextFrame(vbn, false, true);
                a.NextFrame(vbn, false, true);  //go on first frame

                int i = 0;

                // writing node attributes
                foreach (STBone b in vbn.getBoneTreeOrder())
                {
                    i = vbn.boneIndex(b.Text);

                    if (a.HasBone(b.Text))
                    {
                        // write the bone attributes
                        // count the attributes
                        Animation.KeyNode n = a.GetBone(b.Text);
                        int ac = 0;

                        if (n.XPOS.HasAnimation())
                        {
                            file.WriteLine("anim translate.translateX translateX " + b.Text + " 0 0 " + (ac++) + ";");
                            writeKey(file, n.XPOS, n, a.Size(), "translateX");
                            file.WriteLine("}");
                        }
                        if (n.YPOS.HasAnimation())
                        {
                            file.WriteLine("anim translate.translateY translateY " + b.Text + " 0 0 " + (ac++) + ";");
                            writeKey(file, n.YPOS, n, a.Size(), "translateY");
                            file.WriteLine("}");
                        }
                        if (n.ZPOS.HasAnimation())
                        {
                            file.WriteLine("anim translate.translateZ translateZ " + b.Text + " 0 0 " + (ac++) + ";");
                            writeKey(file, n.ZPOS, n, a.Size(), "translateZ");
                            file.WriteLine("}");
                        }
                        if (n.XROT.HasAnimation())
                        {
                            file.WriteLine("anim rotate.rotateX rotateX " + b.Text + " 0 0 " + (ac++) + ";");
                            writeKey(file, n.XROT, n, a.Size(), "rotateX");
                            file.WriteLine("}");
                        }
                        if (n.YROT.HasAnimation())
                        {
                            file.WriteLine("anim rotate.rotateY rotateY " + b.Text + " 0 0 " + (ac++) + ";");
                            writeKey(file, n.YROT, n, a.Size(), "rotateY");
                            file.WriteLine("}");
                        }
                        if (n.ZROT.HasAnimation())
                        {
                            file.WriteLine("anim rotate.rotateZ rotateZ " + b.Text + " 0 0 " + (ac++) + ";");
                            writeKey(file, n.ZROT, n, a.Size(), "rotateZ");
                            file.WriteLine("}");
                        }

                        if (n.XSCA.HasAnimation())
                        {
                            file.WriteLine("anim scale.scaleX scaleX " + b.Text + " 0 0 " + (ac++) + ";");
                            writeKey(file, n.XSCA, n, a.Size(), "scaleX", n.UseSegmentScaleCompensate);
                            file.WriteLine("}");
                        }
                        if (n.YSCA.HasAnimation())
                        {
                            file.WriteLine("anim scale.scaleY scaleY " + b.Text + " 0 0 " + (ac++) + ";");
                            writeKey(file, n.YSCA, n, a.Size(), "scaleY", n.UseSegmentScaleCompensate);
                            file.WriteLine("}");
                        }
                        if (n.ZSCA.HasAnimation())
                        {
                            file.WriteLine("anim scale.scaleZ scaleZ " + b.Text + " 0 0 " + (ac++) + ";");
                            writeKey(file, n.ZSCA, n, a.Size(), "scaleZ", n.UseSegmentScaleCompensate);
                            file.WriteLine("}");
                        }

                        if (ac == 0)
                            file.WriteLine("anim " + b.Text + " 0 0 0;");
                    }
                    else
                    {
                        file.WriteLine("anim " + b.Text + " 0 0 0;");
                    }
                }
            }
        }

        private static void writeKey(StreamWriter file, Animation.KeyGroup keys, Animation.KeyNode rt, int size, string type, bool useSegmentCompenseateScale = false)
        {
            bool isAngular = type == "rotateX" || type == "rotateY" || type == "rotateZ";

            string interp = isAngular ? "angular" : "linear";

            file.WriteLine("animData {");
            file.WriteLine("  input time;");
            file.WriteLine($"  output {interp};");
            file.WriteLine("  weighted 1;");
            file.WriteLine("  preInfinity constant;");
            file.WriteLine("  postInfinity constant;");
            file.WriteLine("  keys {");

            if (((Animation.KeyFrame)keys.Keys[0]).InterType == InterpolationType.CONSTANT)
                size = 1;

            int f = 1;
            foreach (Animation.KeyFrame key in keys.Keys)
            {
                float v = 0;

                switch (type)
                {
                    case "translateX":
                    case "translateY":
                    case "translateZ":
                        v = key.Value;
                        break;
                    case "rotateX":
                        if (rt.RotType == Animation.RotationType.EULER)
                            v = key.Value * Rad2Deg;
                        if (rt.RotType == Animation.RotationType.QUATERNION)
                        {
                            Quaternion q = new Quaternion(rt.XROT.GetValue(key.Frame), rt.YROT.GetValue(key.Frame), rt.ZROT.GetValue(key.Frame), rt.WROT.GetValue(key.Frame));
                            v = quattoeul(q).X * Rad2Deg;
                        }
                        break;
                    case "rotateY":
                        if (rt.RotType == Animation.RotationType.EULER)
                            v = key.Value * Rad2Deg;
                        if (rt.RotType == Animation.RotationType.QUATERNION)
                        {
                            Quaternion q = new Quaternion(rt.XROT.GetValue(key.Frame), rt.YROT.GetValue(key.Frame), rt.ZROT.GetValue(key.Frame), rt.WROT.GetValue(key.Frame));
                            v = quattoeul(q).Y * Rad2Deg;
                        }
                        break;
                    case "rotateZ":
                        if (rt.RotType == Animation.RotationType.EULER)
                            v = key.Value * Rad2Deg;
                        if (rt.RotType == Animation.RotationType.QUATERNION)
                        {
                            Quaternion q = new Quaternion(rt.XROT.GetValue(key.Frame), rt.YROT.GetValue(key.Frame), rt.ZROT.GetValue(key.Frame), rt.WROT.GetValue(key.Frame));
                            v = quattoeul(q).Z * Rad2Deg;
                        }
                        break;
                    case "scaleX":
                    case "scaleY":
                    case "scaleZ":
                        if (useSegmentCompenseateScale)
                        v = 1f / key.Value;
                        else
                            v = key.Value;
                        break;
                }

                file.WriteLine(" " + (key.Frame + 1) + " {0:N6} fixed fixed 1 1 0 0 1 0 1;".Replace(",","."), v);
            }

            file.WriteLine(" }");
        }
        public static Vector3 quattoeul(Quaternion q){
			float sqw = q.W * q.W;
			float sqx = q.X * q.X;
			float sqy = q.Y * q.Y;
			float sqz = q.Z * q.Z;

			float normal = (float)Math.Sqrt (sqw + sqx + sqy + sqz);
			float pole_result = (q.X * q.Z) + (q.Y * q.W);

			if (pole_result > (0.5 * normal)){
				float ry = (float)Math.PI / 2;
				float rz = 0;
				float rx = 2 * (float)Math.Atan2(q.X, q.W);
				return new Vector3(rx, ry, rz);
			}
			if (pole_result < (-0.5 * normal)){
				float ry = (float)Math.PI/2;
				float rz = 0;
				float rx = -2 * (float)Math.Atan2(q.X, q.W);
				return new Vector3(rx, ry, rz);
			}

			float r11 = 2*(q.X*q.Y + q.W*q.Z);
			float r12 = sqw + sqx - sqy - sqz;
			float r21 = -2*(q.X*q.Z - q.W*q.Y);
			float r31 = 2*(q.Y*q.Z + q.W*q.X);
			float r32 = sqw - sqx - sqy + sqz;

			float frx = (float)Math.Atan2( r31, r32 );
			float fry = (float)Math.Asin ( r21 );
			float frz = (float)Math.Atan2( r11, r12 );
			return new Vector3(frx, fry, frz);
		}
	}
}

