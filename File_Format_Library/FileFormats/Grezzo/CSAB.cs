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

namespace FirstPlugin
{
    public class CSAB : Animation, IFileFormat
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

        public Header header;

        public void Load(System.IO.Stream stream)
        {
            header = new Header();
            header.Read(new FileReader(stream));

            ImageKey = "";
        }
        public void Unload()
        {

        }

        public byte[] Save()
        {
            return null;
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

        public class Header
        {
            public GameVersion Version;

            public List<AnimationNode> Nodes = new List<AnimationNode>();

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
                uint duration = reader.ReadUInt32();
                uint nodeCount = reader.ReadUInt32();
                uint boneCount = reader.ReadUInt32();
                if (nodeCount != boneCount) throw new Exception("Unexpected bone and node count!");

                ushort[] BoneIndexTable = reader.ReadUInt16s((int)boneCount);
                reader.Align(4);
                uint[] nodeOffsets = reader.ReadUInt32s((int)nodeCount);
                for (int i = 0; i < nodeCount; i++)
                {
                    reader.SeekBegin(nodeOffsets[i] + 0x18);
                    AnimationNode node = new AnimationNode();
                    node.Read(reader, Version);
                    Nodes.Add(node);
                }
            }
        }

        public class AnimationNode
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
            if (Offset == 0) return null;

            reader.SeekBegin(startPos + Offset);
            var track = new AnimTrack(reader, version);

            reader.SeekBegin(pos + sizeof(ushort)); //Seek back to next offset
            return track;
        }

        public class AnimTrack
        {
            public List<LinearKeyFrame> KeyFramesLinear = new List<LinearKeyFrame>();
            public List<HermiteKeyFrame> KeyFramesHermite = new List<HermiteKeyFrame>();

            public uint InterpolationType;

            public AnimTrack(FileReader reader, GameVersion version)
            {
                uint numKeyFrames =0;

                if (version >= GameVersion.MM3D)
                {
                    InterpolationType = reader.ReadByte();
                    numKeyFrames = reader.ReadUInt16();
                }
                else
                {
                    InterpolationType = reader.ReadUInt32();
                    numKeyFrames = reader.ReadUInt32();
                    uint unknown = reader.ReadUInt32();
                    uint endFrame = reader.ReadUInt32();
                }

                if (InterpolationType == (uint)AnimationTrackType.LINEAR)
                {
                    if (version >= GameVersion.MM3D)
                    {
                        float scale = reader.ReadSingle();
                        float bias = reader.ReadSingle();

                        for (uint i = 0; i < numKeyFrames; i++)
                        {
                            LinearKeyFrame keyFrame = new LinearKeyFrame();
                            keyFrame.Time = i;
                            keyFrame.Value = reader.ReadUInt16() * scale - bias;
                            KeyFramesLinear.Add(keyFrame);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < numKeyFrames; i++)
                        {
                            LinearKeyFrame keyFrame = new LinearKeyFrame();
                            keyFrame.Time = reader.ReadUInt32();
                            keyFrame.Value = reader.ReadSingle();
                            KeyFramesLinear.Add(keyFrame);
                        }
                    }
                }
                else if (InterpolationType == (uint)AnimationTrackType.HERMITE)
                {
                    for (int i = 0; i < numKeyFrames; i++)
                    {
                        HermiteKeyFrame keyFrame = new HermiteKeyFrame();
                        keyFrame.Time = reader.ReadUInt32();
                        keyFrame.Value = reader.ReadSingle();
                        keyFrame.TangentIn = reader.ReadSingle();
                        keyFrame.TangentOut = reader.ReadSingle();
                        KeyFramesHermite.Add(keyFrame);
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
