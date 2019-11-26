using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Animations;
using OpenTK;

namespace FirstPlugin
{
    public class CSAB : TreeNodeFile, IFileFormat, IAnimationContainer
    {
        public FileType FileType { get; set; } = FileType.Animation;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "CTR Skeletal Animation Binary" };
        public string[] Extension { get; set; } = new string[] { "*.csab" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "csab");
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public STAnimation AnimationController => header;

        public override void OnClick(TreeView treeview)
        {
          
        }

        public Header header;

        public void Load(System.IO.Stream stream)
        {
            try
            {
                header = new Header();
                header.Read(new FileReader(stream));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            this.Text = FileName;

            foreach (var bone in header.Nodes)
                header.AnimGroups.Add(bone);
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        public enum GameVersion
        {
            OOT3D,
            MM3D,
            LM3DS,
        }

        public enum AnimationTrackType
        {
            LINEAR = 0x01,
            HERMITE = 0x02,
        };

        public class Header : STSkeletonAnimation
        {
            public GameVersion Version;

            public List<AnimationNode> Nodes = new List<AnimationNode>();

            public override STSkeleton GetActiveSkeleton()
            {
                var containers = Toolbox.Library.Forms.ObjectEditor.GetDrawableContainers(); 
                foreach (var container in containers) {
                    foreach (var draw in container.Drawables)
                        if (draw is STSkeleton)
                            return (STSkeleton)draw;
                }

                return base.GetActiveSkeleton();
            }

            public override void NextFrame()
            {
                if (Frame > FrameCount) return;

                var skeleton = GetActiveSkeleton();

                if (Frame == 0)
                    skeleton.reset();

                bool Updated = false; // no need to update skeleton of animations that didn't change
                foreach (var node in Nodes)
                {
                    Console.WriteLine($"node.BoneIndex {node.BoneIndex}");

                    if (node.BoneIndex < skeleton.bones.Count) {
                        var b = skeleton.bones[node.BoneIndex];
                        if (b == null) continue;


                        Updated = true;

                        if (node.TranslateX.HasKeys)
                            b.pos.X = node.TranslateX.GetFrameValue(Frame);
                        if (node.TranslateY.HasKeys)
                            b.pos.Y = node.TranslateY.GetFrameValue(Frame);
                        if (node.TranslateZ.HasKeys)
                            b.pos.Z = node.TranslateZ.GetFrameValue(Frame);

                        if (node.ScaleX.HasKeys)
                            b.sca.X = node.ScaleX.GetFrameValue(Frame);
                        else b.sca.X = 1;
                        if (node.ScaleY.HasKeys)
                            b.sca.Y = node.ScaleY.GetFrameValue(Frame);
                        else b.sca.Y = 1;
                        if (node.ScaleZ.HasKeys)
                            b.sca.Z = node.ScaleZ.GetFrameValue(Frame);
                        else b.sca.Z = 1;


                        if (node.RotationX.HasKeys || node.RotationY.HasKeys || node.RotationZ.HasKeys)
                        {
                            float x = node.RotationX.HasKeys ? node.RotationX.GetFrameValue(Frame) : b.rotation[0];
                            float y = node.RotationY.HasKeys ? node.RotationY.GetFrameValue(Frame) : b.rotation[1];
                            float z = node.RotationZ.HasKeys ? node.RotationZ.GetFrameValue(Frame) : b.rotation[2];
                            b.rot = EulerToQuat(z, y, x);
                        }
                    }
                }

                if (Updated) {
                    skeleton.update();
                }
            }

            public static Quaternion EulerToQuat(float z, float y, float x)
            {
                {
                    Quaternion xRotation = Quaternion.FromAxisAngle(Vector3.UnitX, x);
                    Quaternion yRotation = Quaternion.FromAxisAngle(Vector3.UnitY, y);
                    Quaternion zRotation = Quaternion.FromAxisAngle(Vector3.UnitZ, z);

                    Quaternion q = (zRotation * yRotation * xRotation);

                    if (q.W < 0)
                        q *= -1;

                    //return xRotation * yRotation * zRotation;
                    return q;
                }
            }

            public void Read(FileReader reader)
            {
                reader.SetByteOrder(false);

                reader.ReadSignature(4, "csab");
                uint FileSize = reader.ReadUInt32();
                uint versionNum = reader.ReadUInt32();
                if (versionNum == 5)
                    Version = GameVersion.MM3D;
                else if (versionNum == 3)
                    Version = GameVersion.OOT3D;
                else
                    Version = GameVersion.LM3DS;

                uint padding = reader.ReadUInt32(); //Unsure
                if (Version >= GameVersion.MM3D)
                {
                    uint unknown = reader.ReadUInt32();//0x42200000
                    uint unknown2 = reader.ReadUInt32();//0x42200000
                    uint unknown3 = reader.ReadUInt32();//0x42200000
                }
                uint numAnimations = reader.ReadUInt32(); //Unsure
                uint location = reader.ReadUInt32(); //Unsure
                uint unknown4 = reader.ReadUInt32();//0x00
                uint unknown5 = reader.ReadUInt32();//0x00
                uint unknown6 = reader.ReadUInt32();//0x00
                uint unknown7 = reader.ReadUInt32();//0x00
                uint unknown8 = reader.ReadUInt32();//0x00

                reader.SeekBegin(0x28);
                if (Version >= GameVersion.MM3D)
                    reader.SeekBegin(0x34);

                uint duration = reader.ReadUInt32();

                reader.SeekBegin(0x30);
                if (Version >= GameVersion.MM3D)
                    reader.SeekBegin(0x3C);

                uint nodeCount = reader.ReadUInt32();
                uint boneCount = reader.ReadUInt32();
                if (nodeCount != boneCount) throw new Exception("Unexpected bone and node count!");

                FrameCount = duration;

                Console.WriteLine($"duration {duration}");
                Console.WriteLine($"boneCount {boneCount}");

                uint nodeSize = 0x18;

                reader.SeekBegin(0x38);
                if (Version >= GameVersion.MM3D)
                {
                    nodeSize = 0x24;
                    reader.SeekBegin(0x44);
                }


                ushort[] BoneIndexTable = reader.ReadUInt16s((int)boneCount);
                reader.Align(4);

                uint[] nodeOffsets = reader.ReadUInt32s((int)nodeCount);
                for (int i = 0; i < nodeCount; i++)
                {
                    reader.SeekBegin(nodeOffsets[i] + nodeSize);
                    AnimationNode node = new AnimationNode();
                    node.Read(reader, Version);
                    Nodes.Add(node);
                }
            }
        }

        public class AnimationNode : STAnimGroup
        {
            public ushort BoneIndex { get; set; }

            public AnimTrack TranslateX { get; set; }
            public AnimTrack TranslateY { get; set; }
            public AnimTrack TranslateZ { get; set; }
            public AnimTrack RotationX { get; set; }
            public AnimTrack RotationY { get; set; }
            public AnimTrack RotationZ { get; set; }
            public AnimTrack ScaleX { get; set; }
            public AnimTrack ScaleY { get; set; }
            public AnimTrack ScaleZ { get; set; }

            public override List<STAnimationTrack> GetTracks()
            {
                List<STAnimationTrack> tracks = new List<STAnimationTrack>();
                tracks.Add(TranslateX);
                tracks.Add(TranslateY);
                tracks.Add(TranslateZ);
                tracks.Add(RotationX);
                tracks.Add(RotationY);
                tracks.Add(RotationZ);
                tracks.Add(ScaleX);
                tracks.Add(ScaleY);
                tracks.Add(ScaleZ);
                return tracks;
            }

            public void Read(FileReader reader, GameVersion version)
            {
                long pos = reader.Position;
                reader.ReadSignature(4, "anod");
                BoneIndex = reader.ReadUInt16();
                reader.ReadUInt16();//0x00
                TranslateX = ParseTrack(reader, version, pos);
                TranslateY = ParseTrack(reader, version, pos);
                TranslateZ = ParseTrack(reader, version, pos);
                RotationX = ParseTrack(reader, version, pos);
                RotationY = ParseTrack(reader, version, pos);
                RotationZ = ParseTrack(reader, version, pos);
                ScaleX = ParseTrack(reader, version, pos);
                ScaleY = ParseTrack(reader, version, pos);
                ScaleZ = ParseTrack(reader, version, pos);
                reader.ReadUInt16();//0x00
            }
        }

        private static AnimTrack ParseTrack(FileReader reader, GameVersion version, long startPos)
        {
            long pos = reader.Position;

            uint Offset = reader.ReadUInt16();
            if (Offset == 0) return new AnimTrack();

            reader.SeekBegin(startPos + Offset);
            var track = new AnimTrack(reader, version);

            reader.SeekBegin(pos + sizeof(ushort)); //Seek back to next offset
            return track;
        }

        public class AnimTrack : STAnimationTrack
        {
            public List<LinearKeyFrame> KeyFramesLinear = new List<LinearKeyFrame>();
            public List<HermiteKeyFrame> KeyFramesHermite = new List<HermiteKeyFrame>();

            public uint TrackInterpolationType;

            public AnimTrack() { }

            public AnimTrack(FileReader reader, GameVersion version)
            {
                uint numKeyFrames =0;

                if (version >= GameVersion.MM3D)
                {
                    reader.ReadByte(); //unk
                    TrackInterpolationType = reader.ReadByte();
                    numKeyFrames = reader.ReadUInt16();
                }
                else
                {
                    TrackInterpolationType = reader.ReadUInt32();
                    numKeyFrames = reader.ReadUInt32();
                    uint unknown = reader.ReadUInt32();
                    uint endFrame = reader.ReadUInt32();
                }

                if (TrackInterpolationType == (uint)AnimationTrackType.LINEAR)
                {
                    InterpolationType = STInterpoaltionType.Linear;

                    if (version >= GameVersion.MM3D)
                    {
                        float scale = reader.ReadSingle();
                        float bias = reader.ReadSingle();

                        for (uint i = 0; i < numKeyFrames; i++)
                        {
                            float Value = reader.ReadUInt16() * scale - bias;

                            KeyFrames.Add(new STKeyFrame()
                            {
                                Frame = i,
                                Value = Value
                            });
                        }
                    }
                    else
                    {
                        for (int i = 0; i < numKeyFrames; i++)
                        {
                            uint Time = reader.ReadUInt32();
                            float Value = reader.ReadSingle();

                            KeyFrames.Add(new STKeyFrame()
                            {
                                Frame = Time,
                                Value = Value
                            });
                        }
                    }
                }
                else if (TrackInterpolationType == (uint)AnimationTrackType.HERMITE)
                {
                    InterpolationType = STInterpoaltionType.Hermite;

                    for (int i = 0; i < numKeyFrames; i++)
                    {
                        uint Time = reader.ReadUInt32();
                        float Value = reader.ReadSingle();
                        float TangentIn = reader.ReadSingle();
                        float TangentOut = reader.ReadSingle();

                        KeyFrames.Add(new STHermiteKeyFrame()
                        {
                            Frame = Time,
                            TangentIn = TangentIn,
                            TangentOut = TangentOut,
                            Value = Value,
                        });
                    }
                }
                else
                    throw new Exception("Unknown interpolation type! " + InterpolationType);
            }
        }

        public class HermiteKeyFrame
        {
            public uint Time { get; set; }
            public float Value { get; set; }
            public float TangentIn { get; set; }
            public float TangentOut { get; set; }
        }

        public class LinearKeyFrame
        {
            public uint Time { get; set; }
            public float Value { get; set; }
        }
    }
}
