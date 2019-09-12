using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using SharpYaml.Serialization;

namespace LayoutBXLYT
{
    public class BFLAN : IEditorForm<LayoutEditor>, IFileFormat, IConvertableTextFormat
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Cafe Layout Animation (GUI)" };
        public string[] Extension { get; set; } = new string[] { "*.bflan" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "FLAN");
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

        #region Text Converter Interface
        public TextFileType TextFileType => TextFileType.Xml;
        public bool CanConvertBack => true;

        public string ConvertToString()
        {
            var serializerSettings = new SerializerSettings()
            {
                //  EmitTags = false
            };

            serializerSettings.DefaultStyle = SharpYaml.YamlStyle.Any;
            serializerSettings.ComparerForKeySorting = null;
            serializerSettings.RegisterTagMapping("Header", typeof(Header));

            return FLAN.ToXml(header);

            var serializer = new Serializer(serializerSettings);
            string yaml = serializer.Serialize(header, typeof(Header));
            return yaml;
        }

        public void ConvertFromString(string text)
        {
            header = FLAN.FromXml(text);
            header.FileInfo = this;
        }

        #endregion

        Header header;

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            header = new Header();
            header.Read(new FileReader(stream),this);
        }
        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream) {
            header.Write(new FileWriter(stream));
        }

        public LayoutEditor OpenForm()
        {
            LayoutEditor editor = new LayoutEditor();
            editor.Dock = DockStyle.Fill;
            editor.LoadBxlan(header);
            return editor;
        }

        public void FillEditor(Form control)
        {
            ((LayoutEditor)control).LoadBxlan(header);
        }

        public class Header : BxlanHeader
        {
            private const string Magic = "FLAN";
            private ushort ByteOrderMark;
            private ushort HeaderSize;

            //As of now this should be empty but just for future proofing
            private List<SectionCommon> UnknownSections = new List<SectionCommon>();

            public PAT1 AnimationTag;
            public PAI1 AnimationInfo;

            public void Read(FileReader reader, BFLAN bflan)
            {
                AnimationTag = new PAT1();
                AnimationInfo = new PAI1();

                reader.SetByteOrder(true);
                reader.ReadSignature(4, Magic);
                ByteOrderMark = reader.ReadUInt16();
                reader.CheckByteOrderMark(ByteOrderMark);
                HeaderSize = reader.ReadUInt16();
                Version = reader.ReadUInt32();
                SetVersionInfo();
                uint FileSize = reader.ReadUInt32();
                ushort sectionCount = reader.ReadUInt16();
                reader.ReadUInt16(); //Padding

                FileInfo = bflan;
                IsBigEndian = reader.ByteOrder == Syroot.BinaryData.ByteOrder.BigEndian;

                reader.SeekBegin(HeaderSize);
                for (int i = 0; i < sectionCount; i++)
                {
                    long pos = reader.Position;
                    string Signature = reader.ReadString(4, Encoding.ASCII);
                    uint SectionSize = reader.ReadUInt32();

                    SectionCommon section = new SectionCommon(Signature);
                    switch (Signature)
                    {
                        case "pat1":
                            AnimationTag = new PAT1(reader, this);
                            break;
                        case "pai1":
                            AnimationInfo = new PAI1(reader, this);
                            break;
                        default:
                            section.Data = reader.ReadBytes((int)SectionSize - 8);
                            UnknownSections.Add(section);
                            break;
                    }

                    section.SectionSize = SectionSize;
                    reader.SeekBegin(pos + SectionSize);
                }
            }

            public void Write(FileWriter writer)
            {
                writer.SetByteOrder(true);
                writer.WriteSignature(Magic);
                if (!IsBigEndian)
                    writer.Write((ushort)0xFFFE);
                else
                    writer.Write((ushort)0xFEFF);
                writer.SetByteOrder(IsBigEndian);
                writer.Write(HeaderSize);
                writer.Write(Version);
                writer.Write(uint.MaxValue); //Reserve space for file size later
                writer.Write(ushort.MaxValue); //Reserve space for section count later
                writer.Seek(2); //padding

                int sectionCount = 0;

                WriteSection(writer, "pat1", AnimationTag, () => AnimationTag.Write(writer, this));
                sectionCount++;

                WriteSection(writer, "pai1", AnimationInfo, () => AnimationInfo.Write(writer, this));
                sectionCount++;

                foreach (var section in UnknownSections)
                {
                    WriteSection(writer, section.Signature, section, () => section.Write(writer, this));
                    sectionCount++;
                }

                //Write the total section count
                using (writer.TemporarySeek(0x10, System.IO.SeekOrigin.Begin))
                {
                    writer.Write((ushort)sectionCount);
                }

                //Write the total file size
                using (writer.TemporarySeek(0x0C, System.IO.SeekOrigin.Begin))
                {
                    writer.Write((uint)writer.BaseStream.Length);
                }
            }
        }

        public class PAT1 : SectionCommon
        {
            [DisplayName("Name"), CategoryAttribute("Animation")]
            public string Name { get; set; }

            [DisplayName("Groups"), CategoryAttribute("Animation")]
            public List<string> Groups { get; set; }

            [DisplayName("Start"), CategoryAttribute("Frames")]
            public ushort StartFrame { get; set; }

            [DisplayName("End"), CategoryAttribute("Frames")]
            public ushort EndFrame { get; set; }

            [DisplayName("Animation Order"), CategoryAttribute("Parameters")]
            public ushort AnimationOrder { get; set; }

            [DisplayName("Child Binding"), CategoryAttribute("Parameters")]
            public bool ChildBinding { get; set; }

            private byte[] UnknownData;

            private byte flags;

            public PAT1()
            {
                AnimationOrder = 2;
                Name = "";
                EndFrame = 0;
                StartFrame = 0;
                ChildBinding = false;
                Groups = new List<string>();
            }

            public PAT1(FileReader reader, Header header)
            {
                long startPos = reader.Position - 8;

                Groups = new List<string>();

                AnimationOrder = reader.ReadUInt16();
                ushort groupCount = reader.ReadUInt16();
                uint animNameOffset = reader.ReadUInt32();
                uint groupNamesOffset = reader.ReadUInt32();
                StartFrame = reader.ReadUInt16();
                EndFrame = reader.ReadUInt16();
                ChildBinding = reader.ReadBoolean();
                UnknownData = reader.ReadBytes((int)(startPos + animNameOffset - reader.Position));

                reader.SeekBegin(startPos + animNameOffset);
                Name = reader.ReadZeroTerminatedString();

                reader.SeekBegin(startPos + groupNamesOffset);
                for (int i = 0; i < groupCount; i++)
                    Groups.Add(reader.ReadString(28, true));
            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                long startPos = writer.Position - 8;

                writer.Write(AnimationOrder);
                writer.Write((ushort)Groups.Count);
                writer.Write(uint.MaxValue); //animNameOffset
                writer.Write(uint.MaxValue); //groupNamesOffset
                writer.Write(StartFrame);
                writer.Write(EndFrame);
                writer.Write(ChildBinding);
                writer.Write(UnknownData);

                writer.WriteUint32Offset(startPos + 12, startPos);
                writer.WriteString(Name);
                writer.Align(4);

                writer.WriteUint32Offset(startPos + 16, startPos);
                for (int i = 0; i < Groups.Count; i++)
                    writer.WriteString(Groups[i], 28);
            }
        }

        public class PAI1 : SectionCommon
        {
            public ushort FrameSize;

            public bool Loop;

            public List<string> Textures { get; set; }
            public List<PaiEntry> Entries = new List<PaiEntry>();

            public PAI1()
            {
                Textures = new List<string>();
            }

            public PAI1(FileReader reader, Header header)
            {
                long startPos = reader.Position - 8;

                Textures = new List<string>();

                FrameSize = reader.ReadUInt16();
                Loop = reader.ReadBoolean();
                reader.ReadByte(); //padding
                var numTextures = reader.ReadUInt16();
                var numEntries = reader.ReadUInt16();
                var entryOffsetTbl = reader.ReadUInt32();

                var texOffsets = reader.ReadUInt32s(numTextures);
                for (int i = 0; i < numTextures; i++)
                {
                    reader.SeekBegin(startPos + texOffsets[i]);
                    Textures.Add(reader.ReadZeroTerminatedString());
                }

                reader.SeekBegin(startPos + entryOffsetTbl);
                var entryOffsets = reader.ReadUInt32s(numEntries);
                for (int i = 0; i < numEntries; i++)
                {
                    reader.SeekBegin(startPos + entryOffsets[i]);
                    Entries.Add(new PaiEntry(reader, header));
                }
            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                long startPos = writer.Position - 8;

                writer.Write(FrameSize);
                writer.Write(Loop);
                writer.Write((byte)0);
                writer.Write((ushort)Textures.Count);
                writer.Write((ushort)Entries.Count);
                long entryOfsTblPos = writer.Position;
                writer.Write(0);

                if (Textures.Count > 0)
                {
                    long startOfsPos = writer.Position;
                    writer.Write(new uint[Textures.Count]);
                    for (int i = 0; i < Textures.Count; i++)
                    {
                        writer.WriteUint32Offset(startOfsPos + (i * 4), startPos);
                        writer.WriteString(Textures[i]);
                    }
                }
                if (Entries.Count > 0)
                {
                    writer.WriteUint32Offset(entryOfsTblPos, startPos);

                    long startOfsPos = writer.Position;
                    writer.Write(new uint[Entries.Count]);
                    for (int i = 0; i < Entries.Count; i++)
                    {
                        writer.WriteUint32Offset(startOfsPos + (i * 4), startPos);
                        Entries[i].Write(writer, header);
                    }
                }
            }
        }

        public class PaiEntry
        {
            [DisplayName("Name"), CategoryAttribute("Animation")]
            public string Name { get; set; }

            [DisplayName("Target"), CategoryAttribute("Animation")]
            public AnimationTarget Target { get; set; }

            public List<PaiTag> Tags = new List<PaiTag>();

            public PaiEntry(FileReader reader, Header header)
            {
                long startPos = reader.Position;

                Name = reader.ReadString(28, true);
                var numTags = reader.ReadByte();
                Target = reader.ReadEnum<AnimationTarget>(false);
                reader.ReadUInt16(); //padding

                var offsets = reader.ReadUInt32s(numTags);
                for (int i = 0; i < numTags; i++)
                {
                    reader.SeekBegin(startPos + offsets[i]);
                    Tags.Add(new PaiTag(reader, header, Target));
                }
            }

            public void Write(FileWriter writer, LayoutHeader header)
            {
                long startPos = writer.Position;

                writer.WriteString(Name, 28);
                writer.Write((byte)Tags.Count);
                writer.Write(Target, false);
                writer.Write((ushort)0);
                if (Tags.Count > 0)
                {
                    writer.Write(new uint[Tags.Count]);
                    for (int i = 0; i < Tags.Count; i++)
                    {
                        writer.WriteUint32Offset(startPos + 32 + (i * 4), startPos);
                        Tags[i].Write(writer, header, Target);
                    }
                }
            }
        }

        public class PaiTag
        {
            public List<PaiTagEntry> Entries = new List<PaiTagEntry>();

            private string Tag;

            public string Type
            {
                get { return TypeDefine.ContainsKey(Tag) ? TypeDefine[Tag] : Tag; }
            }

            public Dictionary<string, string> TypeDefine = new Dictionary<string, string>()
            {
                {"FLPA","PaneSRT" },
                {"FLVI","Visibility" },
                {"FLTS","TextureSRT" },
                {"FLVC","VertexColor" },
                {"FLMC","MaterialColor" },
                {"FLTP","TexturePattern" },
                {"FLIM","IndTextureSRT" },
                {"FLAC","AlphaTest" },
                {"FLCT","FontShadow" },
                {"FLCC","PerCharacterTransformCurve" },
            };

            private uint Unknown {get;set;}

            public PaiTag(FileReader reader, Header header, AnimationTarget target)
            {
                if ((byte)target == 2)
                    Unknown = reader.ReadUInt32(); //This doesn't seem to be included in the offsets to the entries (?)

                long startPos = reader.Position;
                Tag = reader.ReadString(4, Encoding.ASCII);
                var numEntries = reader.ReadByte();
                reader.Seek(3);
                var offsets = reader.ReadUInt32s((int)numEntries);
                for (int i = 0; i < numEntries; i++)
                {
                    reader.SeekBegin(startPos + offsets[i]);
                    switch (Tag)
                    {
                        case "FLPA":
                            Entries.Add(new FLPATagEntry(reader, header));
                            break;
                        case "FLTS":
                            Entries.Add(new FLTSTagEntry(reader, header));
                            break;
                        case "FLVI":
                            Entries.Add(new FLVITagEntry(reader, header));
                            break;
                        case "FLVC":
                            Entries.Add(new FLVCTagEntry(reader, header));
                            break;
                        case "FLMC":
                            Entries.Add(new FLMCTagEntry(reader, header));
                            break;
                        case "FLTP":
                            Entries.Add(new FLTPTagEntry(reader, header));
                            break;
                        default:
                            Entries.Add(new PaiTagEntry(reader, header));
                            break;
                    }
                }
            }

            public void Write(FileWriter writer, LayoutHeader header, AnimationTarget target)
            {
                if ((byte)target == 2)
                    writer.Write(Unknown);

                long startPos = writer.Position;

                writer.WriteSignature(Tag);
                writer.Write((byte)Entries.Count);
                writer.Seek(3);
                writer.Write(new uint[Entries.Count]);
                for (int i = 0; i < Entries.Count; i++)
                {
                    writer.WriteUint32Offset(startPos + 8 + (i * 4), startPos);
                    Entries[i].Write(writer, header);
                }
            }
        }

        public class PaiTagEntry
        {
            [Browsable(false)]
            public virtual string TargetName
            {
                get { return AnimationTarget.ToString(); }
            }

            public byte AnimationTarget;

            [DisplayName("Index"), CategoryAttribute("Tag")]
            public byte Index { get; private set; }

            [DisplayName("Data Type"), CategoryAttribute("Tag")]
            public KeyType DataType { get; private set; }

            public byte Unknown;

            public List<KeyFrame> KeyFrames = new List<KeyFrame>();

            public PaiTagEntry(FileReader reader, Header header)
            {
                long startPos = reader.Position;
                Index = reader.ReadByte();
                AnimationTarget = reader.ReadByte();
                DataType = reader.ReadEnum<KeyType>(true);
                Unknown = reader.ReadByte();
                var KeyFrameCount = reader.ReadUInt16();
                reader.ReadUInt16(); //Padding
                uint keyFrameOffset = reader.ReadUInt32();


                reader.SeekBegin(startPos + keyFrameOffset);
                for (int i = 0; i < KeyFrameCount; i++)
                    KeyFrames.Add(new KeyFrame(reader, DataType));
            }

            public void Write(FileWriter writer, LayoutHeader header)
            {
                long startPos = writer.Position;

                writer.Write(Index);
                writer.Write(AnimationTarget);
                writer.Write(DataType, true);
                writer.Write(Unknown);
                writer.Write((ushort)KeyFrames.Count);
                writer.Write((ushort)0); //padding
                writer.Write(0); //key offset

                if (KeyFrames.Count > 0)
                {
                    writer.WriteUint32Offset(startPos + 8, startPos);
                    for (int i = 0; i < KeyFrames.Count; i++)
                        KeyFrames[i].Write(writer, DataType);
                }
            }
        }

        public class KeyFrame
        {
            [DisplayName("Blend"), CategoryAttribute("Key Frame")]
            public float Blend { get; set; }
            [DisplayName("Frame"), CategoryAttribute("Key Frame")]
            public float Frame { get; set; }
            [DisplayName("Value"), CategoryAttribute("Key Frame")]
            public float Value { get; set; }

            public KeyFrame(FileReader reader, KeyType DataType)
            {
                Frame = reader.ReadSingle();
                switch (DataType)
                {
                    case KeyType.Float:
                        Value = reader.ReadSingle();
                        Blend = reader.ReadSingle();
                        break;
                    case KeyType.Uin16:
                        Value = (float)reader.ReadInt16();
                        Blend = (float)reader.ReadInt16();
                        break;
                }
            }

            public void Write(FileWriter writer, KeyType DataType)
            {
                writer.Write(Frame);
                switch (DataType)
                {
                    case KeyType.Float:
                        writer.Write(Value);
                        writer.Write(Blend);
                        break;
                    case KeyType.Uin16:
                        writer.Write((ushort)Value);
                        writer.Write((ushort)Blend);
                        break;
                }
            }
        }

        public class FLPATagEntry : PaiTagEntry
        {
            public override string TargetName => Target.ToString();
            [DisplayName("Target"), CategoryAttribute("Tag")]
            public LPATarget Target
            {
                get { return (LPATarget)AnimationTarget; }
                set { AnimationTarget = (byte)value; }
            }
            public FLPATagEntry(FileReader reader, Header header) : base(reader, header) { }
        }

        public class FLTSTagEntry : PaiTagEntry
        {
            public override string TargetName => Target.ToString();
            [DisplayName("Target"), CategoryAttribute("Tag")]
            public LTSTarget Target
            {
                get { return (LTSTarget)AnimationTarget; }
                set { AnimationTarget = (byte)value; }
            }
            public FLTSTagEntry(FileReader reader, Header header) : base(reader, header) { }
        }

        public class FLVITagEntry : PaiTagEntry
        {
            public override string TargetName => Target.ToString();
            [DisplayName("Target"), CategoryAttribute("Tag")]
            public LVITarget Target
            {
                get { return (LVITarget)AnimationTarget; }
                set { AnimationTarget = (byte)value; }
            }
            public FLVITagEntry(FileReader reader, Header header) : base(reader, header) { }
        }

        public class FLVCTagEntry : PaiTagEntry
        {
            public override string TargetName => Target.ToString();
            [DisplayName("Target"), CategoryAttribute("Tag")]
            public LVCTarget Target
            {
                get { return (LVCTarget)AnimationTarget; }
                set { AnimationTarget = (byte)value; }
            }
            public FLVCTagEntry(FileReader reader, Header header) : base(reader, header) { }
        }

        public class FLMCTagEntry : PaiTagEntry
        {
            public override string TargetName => Target.ToString();
            [DisplayName("Target"), CategoryAttribute("Tag")]
            public LMCTarget Target
            {
                get { return (LMCTarget)AnimationTarget; }
                set { AnimationTarget = (byte)value; }
            }
            public FLMCTagEntry(FileReader reader, Header header) : base(reader, header) { }
        }

        public class FLTPTagEntry : PaiTagEntry
        {
            public override string TargetName => Target.ToString();
            [DisplayName("Target"), CategoryAttribute("Tag")]
            public LTPTarget Target
            {
                get { return (LTPTarget)AnimationTarget; }
                set { AnimationTarget = (byte)value; }
            }
            public FLTPTagEntry(FileReader reader, Header header) : base(reader, header) { }
        }
    }
}
