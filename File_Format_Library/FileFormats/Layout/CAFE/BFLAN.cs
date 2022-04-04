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
    public class BFLAN : BXLAN, IEditorForm<LayoutEditor>, IFileFormat, IConvertableTextFormat
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

            return FLAN.ToXml((Header)BxlanHeader);

            var serializer = new Serializer(serializerSettings);
            string yaml = serializer.Serialize(BxlanHeader, typeof(Header));
            return yaml;
        }

        public void ConvertFromString(string text)
        {
            BxlanHeader = FLAN.FromXml(text);
            BxlanHeader.FileInfo = this;
        }

        #endregion

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            BxlanHeader = new Header();
            BxlanHeader.Read(new FileReader(stream),this);
        }
        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream) {
            BxlanHeader.Write(new FileWriter(stream));
        }

        public LayoutEditor OpenForm()
        {
            LayoutEditor editor = new LayoutEditor();
            editor.Dock = DockStyle.Fill;
            editor.LoadBxlan(BxlanHeader);
            return editor;
        }

        public void FillEditor(Form control)
        {
            ((LayoutEditor)control).LoadBxlan(BxlanHeader);
        }

        public class Header : BxlanHeader
        {
            private const string Magic = "FLAN";
            private ushort ByteOrderMark;
            private ushort HeaderSize;

            //As of now this should be empty but just for future proofing
            private List<SectionCommon> UnknownSections = new List<SectionCommon>();

            public override void Read(FileReader reader, BXLAN bflan)
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

                FileInfo = (IFileFormat)bflan;
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

            public override void Write(FileWriter writer)
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

        public class PAT1 : BxlanPAT1
        {
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
                if (header.VersionMajor >= 8)
                    reader.ReadUInt32(); //unk

                StartFrame = reader.ReadInt16();
                EndFrame = reader.ReadInt16();

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
                if (header.VersionMajor >= 8)
                    writer.Write(0); //unk

                writer.Write((ushort)StartFrame);
                writer.Write((ushort)EndFrame);

                writer.Write(ChildBinding);
                writer.Write(UnknownData);

                writer.WriteUint32Offset(startPos + 12, startPos);
                writer.WriteString(Name);
                writer.Align(4);

                writer.WriteUint32Offset(startPos + 16, startPos);
                for (int i = 0; i < Groups.Count; i++)
                    writer.WriteString(Groups[i], 28);
            
                writer.Align(4);
            }
        }

        public class PAI1 : BxlanPAI1
        {
            public PAI1()
            {
                Textures = new List<string>();
            }

            public override BxlanPaiEntry AddEntry(string name, byte target) {
                var entry = new PaiEntry(name, target);
                Entries.Add(entry);
                return entry;
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

                long texStart = reader.Position;
                var texOffsets = reader.ReadUInt32s(numTextures);
                for (int i = 0; i < numTextures; i++)
                {
                    reader.SeekBegin(texStart + texOffsets[i]);
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
                        writer.WriteUint32Offset(startOfsPos + (i * 4), startOfsPos);
                        writer.WriteString(Textures[i]);
                    }
                    writer.Align(4);
                }
                if (Entries.Count > 0)
                {
                    writer.WriteUint32Offset(entryOfsTblPos, startPos);

                    long startOfsPos = writer.Position;
                    writer.Write(new uint[Entries.Count]);
                    for (int i = 0; i < Entries.Count; i++)
                    {
                        writer.WriteUint32Offset(startOfsPos + (i * 4), startPos);
                        ((PaiEntry)Entries[i]).Write(writer, header);
                    }
                }
            }
        }

        public class PaiEntry : BxlanPaiEntry
        {
            public override BxlanPaiTag AddEntry(string tag) {
                var paiTag = new PaiTag(tag);
                Tags.Add(paiTag);
                return paiTag;
            }

            public PaiEntry(string name, byte target)
            {
                Name = name;
                Target = (AnimationTarget)target;
            }

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
                        ((PaiTag)Tags[i]).Write(writer, header, Target);
                    }
                }
            }
        }

        public class PaiTag : BxlanPaiTag
        {
            private uint Unknown {get;set;}

            public PaiTag(string tag)
            {
                Tag = tag;
            }

            public PaiTag(FileReader reader, BxlanHeader header, AnimationTarget target)
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
                            Entries.Add(new LPATagEntry(reader, header));
                            break;
                        case "FLTS":
                            Entries.Add(new LTSTagEntry(reader, header));
                            break;
                        case "FLVI":
                            Entries.Add(new LVITagEntry(reader, header));
                            break;
                        case "FLVC":
                            Entries.Add(new LVCTagEntry(reader, header));
                            break;
                        case "FLMC":
                            Entries.Add(new LMCTagEntry(reader, header));
                            break;
                        case "FLTP":
                            Entries.Add(new LTPTagEntry(reader, header));
                            break;
                        default:
                            Entries.Add(new BxlanPaiTagEntry(reader, header));
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
                    ((BxlanPaiTagEntry)Entries[i]).Write(writer, header);
                }
            }
        }
    }
}
