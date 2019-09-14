using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Syroot.BinaryData;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;
using Toolbox.Library;
using System.Collections;
using System.Drawing;

namespace FirstPlugin
{
    public class EFTB : TreeNodeFile, IFileFormat
    {
        #region EFTB
        public static readonly string SIGNATURE = "EFTB";

        public FileType FileType { get; set; } = FileType.Effect;
        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Effect Table" };
        public string[] Extension { get; set; } = new string[] { "*.esetlist", "*.sesetlist" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }
        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public bool Identify(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                return SIGNATURE.Equals(reader.ReadString(4, Encoding.ASCII));
            }
        }

        private byte[] unknownHeaderData;

        public void Load(Stream stream)
        {
            Text = FileName;

            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = ByteOrder.BigEndian;

                // validate signature
                string fileSignature = reader.ReadString(4, Encoding.ASCII);
                if (!SIGNATURE.Equals(fileSignature))
                {
                    throw new InvalidOperationException($"Cannot read {fileSignature} file as {SIGNATURE}");
                }

                unknownHeaderData = reader.ReadBytes(0x2c);

                // read sections
                Nodes.AddRange(NodeBase.ReadNodes(reader, true));
            }

            CanSave = true;
            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new ToolStripMenuItem("Save", null, Save, Keys.Control | Keys.S));
        }

        public void Unload() { }

        public void Save(Stream stream)
        {
            using (var writer = new FileWriter(stream))
            {
                writer.ByteOrder = ByteOrder.BigEndian;
                writer.WriteSignature(SIGNATURE);
                writer.Write(unknownHeaderData);
                NodeBase.WriteRootNodes(writer, Nodes.GetEnumerator());
            }
        }

        private void Save(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(typeof(PTCL));
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
                STFileSaver.SaveFileFormat(this, sfd.FileName);
        }

        #endregion

        #region NodeBase

        // holds data for EFTB node headers. this is tightly coupled to reading/writing the file and should not be persisted in application models.
        public class Header
        {
            public static readonly uint NULL_OFFSET = 0xffffffff;
            public long Position { get; set; } // not a part of the header, but describes where it's found in the stream
            public string Signature { get; set; } // node signature/magic (4 bytes)
            public uint NodeSize { get; set; } // size of the entire node including header, children, and attributes
            public uint ChildrenOffset { get; set; } = NULL_OFFSET; // offset to the first child
            public uint SiblingOffset { get; set; } = NULL_OFFSET; // offset to the next sibling node
            public uint AttributesOffset { get; set; } = NULL_OFFSET; // offset to the first attribute
            public uint DataOffset { get; set; } // offset to the start of binary data (i.e. size of the header)
            public ushort ChildrenCount { get; set; } // usually number of children, but there are many exceptions
            public byte[] Extra { get; set; } // extra header data past the standard 32 bytes

            public static Header Read(FileReader reader)
            {
                var header = new Header() { Position = reader.Position };
                header.Signature = reader.ReadString(4, Encoding.ASCII);
                header.NodeSize = reader.ReadUInt32();
                header.ChildrenOffset = reader.ReadUInt32();
                header.SiblingOffset = reader.ReadUInt32();
                header.AttributesOffset = reader.ReadUInt32();
                header.DataOffset = reader.ReadUInt32();
                uint unknownPadding = reader.ReadUInt32();
                header.ChildrenCount = reader.ReadUInt16();
                ushort unknown1 = reader.ReadUInt16();

                return header;
            }

            public class OffsetType
            {
                public static readonly int Size = 0;
                public static readonly int Children = 1;
                public static readonly int Sibling = 2;
                public static readonly int Attributes = 3;
                public static readonly int Data = 4;
            }
        }

        public interface IEFTBNode
        {
            string Signature { get; }
            void ReadNode(FileReader reader, Header header);
            Offset[] WriteNode(FileWriter writer);
        }

        // TODO is this common accross several formats? can split it out of EFTB.
        public class NodeBase : TreeNodeCustom, IEFTBNode
        {
            public string Signature => GetType().Name == typeof(NodeBase).Name ? Text : GetType().Name;
            public byte[] UnknownHeader { get; protected set; }
            public byte[] UnknownData { get; set; }
            public List<NodeBase> Attributes { get; } = new List<NodeBase>();
            public List<IEFTBNode> Children { get; } = new List<IEFTBNode>();

            public static NodeBase[] ReadNodes(FileReader reader, bool align = false)
            {
                var nodes = new List<NodeBase>();
                Header header;
                while (true)
                {
                    if (align) reader.Align(16);
                    using (reader.TemporarySeek())
                    {
                        header = Header.Read(reader);
                    }
                    var node = Create(header.Signature);
                    node.ReadNode(reader, header);
                    nodes.Add(node);
                    if (header.SiblingOffset != Header.NULL_OFFSET)
                    {
                        reader.Seek(header.SiblingOffset);
                    }
                    else
                    {
                        break;
                    }
                }

                return nodes.ToArray();
            }

            public static NodeBase Create(string signature)
            {
                NodeBase section;
                try
                {
                    section = (NodeBase)Activator.CreateInstance(Type.GetType($"{typeof(EFTB).FullName}+{signature}", true));
                }
                catch (TypeLoadException)
                {
                    section = new NodeBase(signature);
                }

                return section;
            }

            public static void WriteRootNodes(FileWriter writer, IEnumerator enumerator)
            {
                var last = !enumerator.MoveNext();
                while (!last)
                {
                    int pos = (int)writer.Position;
                    NodeBase node = (NodeBase)enumerator.Current;
                    Offset[] offsets = node.WriteNode(writer);
                    last = !enumerator.MoveNext();
                    if (!last) writer.Align(16);
                    offsets[Header.OffsetType.Sibling].Satisfy(last ? (int)Header.NULL_OFFSET : (int)writer.Position - pos);
                }
            }

            public NodeBase()
            {
                Text = GetType().Name;
            }

            public NodeBase(string text)
            {
                Text = text;
            }

            public override void OnClick(TreeView treeview)
            {
                if (UnknownData != null && UnknownData.Length > 0)
                {
                    HexEditor editor = (HexEditor)LibraryGUI.GetActiveContent(typeof(HexEditor));
                    if (editor == null)
                    {
                        editor = new HexEditor();
                        LibraryGUI.LoadEditor(editor);
                    }
                    editor.Text = Text;
                    editor.Dock = DockStyle.Fill;
                    editor.LoadData(UnknownData);
                    editor.SaveData += (object sender, byte[] data) => this.UnknownData = data;
                }
            }

            public virtual void ReadNode(FileReader reader, Header header)
            {
                using (reader.TemporarySeek(0x20))
                {
                    // sometimes the header has more data
                    int extraHeaderSize = (int)(header.DataOffset - 0x20);
                    header.Extra = extraHeaderSize > 0 ? reader.ReadBytes(extraHeaderSize) : new byte[0];
                    UnknownHeader = header.Extra; // save it so it can be written verbatim

                    ReadBinaryData(reader, header);
                }

                if (header.AttributesOffset != Header.NULL_OFFSET)
                {
                    using (reader.TemporarySeek(header.AttributesOffset))
                    {
                        var nodes = ReadNodes(reader);
                        Attributes.AddRange(nodes);
                    }
                }

                if (header.ChildrenOffset != Header.NULL_OFFSET)
                {
                    using (reader.TemporarySeek(header.ChildrenOffset))
                    {
                        var nodes = ReadNodes(reader);
                        Children.AddRange(nodes);
                        Nodes.AddRange(nodes);
                    }
                }
            }

            protected virtual void ReadBinaryData(FileReader reader, Header header)
            {
                var stop = header.Position + (
                   header.AttributesOffset != Header.NULL_OFFSET ? header.AttributesOffset :
                   header.ChildrenOffset != Header.NULL_OFFSET ? header.ChildrenOffset :
                   header.SiblingOffset != Header.NULL_OFFSET ? header.SiblingOffset :
                   header.NodeSize);
                UnknownData = reader.ReadBytes((int)stop - (int)reader.Position);
            }

            public virtual Offset[] WriteNode(FileWriter writer)
            {
                int start = (int)writer.Position;
                Offset[] offsets = WriteHeader(writer);

                int dataOffset = (int)writer.Position - start;
                offsets[Header.OffsetType.Data].Satisfy(dataOffset);
                WriteBinaryData(writer);
                int dataEndOffset = (int)writer.Position - start;

                if (Attributes.Count > 0)
                {
                    offsets[Header.OffsetType.Attributes].Satisfy((int)writer.Position - start);
                    WriteNodes(writer, Attributes.GetEnumerator());
                }
                else
                {
                    offsets[Header.OffsetType.Attributes].Satisfy((int)Header.NULL_OFFSET);
                }

                int childrenOffset = (int)writer.Position - start;
                if (Children.Count > 0)
                {
                    offsets[Header.OffsetType.Children].Satisfy(childrenOffset);
                    WriteNodes(writer, Children.GetEnumerator());
                }
                else
                {
                    offsets[Header.OffsetType.Children].Satisfy((int)Header.NULL_OFFSET);
                }

                offsets[Header.OffsetType.Size].Satisfy(CalculateSize(dataOffset, dataEndOffset, childrenOffset, (int)writer.Position - start));

                return offsets; // leave it to caller to satisfy sibling offset
            }

            protected virtual Offset[] WriteHeader(FileWriter writer)
            {
                writer.WriteSignature(Signature);
                var offsets = writer.ReserveOffset(5);
                writer.Write(0);
                writer.Write((ushort)Children.Count);
                writer.Write((ushort)1);
                writer.Write(UnknownHeader);

                return offsets;
            }

            protected virtual void WriteBinaryData(FileWriter writer)
            {
                writer.Write(UnknownData);
            }

            protected virtual void WriteNodes(FileWriter writer, IEnumerator enumerator)
            {
                var last = !enumerator.MoveNext();
                while (!last)
                {
                    int pos = (int)writer.Position;
                    IEFTBNode node = (IEFTBNode)enumerator.Current;
                    Offset[] offsets = node.WriteNode(writer);
                    last = !enumerator.MoveNext();
                    offsets[Header.OffsetType.Sibling].Satisfy(last ? (int)Header.NULL_OFFSET : (int)writer.Position - pos);
                }
            }

            protected virtual int CalculateSize(int dataOffset, int dataEndOffset, int childrenOffset, int endOffset)
            {
                return endOffset;
            }

            protected void ReadName(FileReader reader)
            {
                reader.Seek(0x10); // 0-padding
                long start = reader.Position;
                Text = reader.ReadZeroTerminatedString();
                long length = reader.Position - start;
                reader.Seek(0x40 - length);
            }

            protected void WriteName(FileWriter writer)
            {
                writer.Seek(0x10); // 0-padding
                long start = writer.Position;
                writer.WriteString(Text);
                long length = writer.Position - start;
                writer.Seek(0x40 - length);
            }
        }

        public class NodeSpecial : NodeBase // TODO idk what's special about it except that it calculates size differently
        {
            protected override void ReadBinaryData(FileReader reader, Header header)
            {
                UnknownData = reader.ReadBytes((int)header.NodeSize);
            }

            protected override int CalculateSize(int dataOffset, int dataEndOffset, int childrenOffset, int endOffset)
            {
                return dataEndOffset - dataOffset;
            }
        }

        #endregion

        #region ESTA

        // Emitter SeT Array
        public class ESTA : NodeBase
        {
            public ESTA() : base("Emitter Sets") { }
        }

        // Emitter SET
        public class ESET : NodeBase
        {
            protected override void ReadBinaryData(FileReader reader, Header header)
            {
                ReadName(reader);
                UnknownData = reader.ReadBytes(0x10);
            }

            protected override void WriteBinaryData(FileWriter writer)
            {
                WriteName(writer);
                // TODO first uint is total number of EMTR (including all descendents)
                // 2nd is always 0, still don't know what 3 and 4 are (4 is almost always 0, sometimes 0x000000ff)
                writer.Write(UnknownData);
            }
        }

        // EMiTteR
        public class EMTR : NodeBase
        {
            public float Radius { get; set; }
            public STColorArray Color0Array { get; private set; }
            public STColorArray Color0AlphaArray { get; private set; } // TODO merge alpha into STColorArray type?
            public STColorArray Color1Array { get; private set; }
            public STColorArray Color1AlphaArray { get; private set; }
            public STColorArray ScaleArray { get; private set; } // TODO what is this really?
            public STColor ConstantColor0 { get; private set; } // TODO merge this into STColorArray type if there's always either one or the other
            public STColor ConstantColor1 { get; private set; } // -- can choose whether to read/write const or anim based on key #
            public float BlinkIntensity0 { get; set; }
            public float BlinkDuration0 { get; set; }
            public float BlinkIntensity1 { get; set; }
            public float BlinkDuration1 { get; set; }
            public List<SamplerInfo> Samplers = new List<SamplerInfo>();

            public uint UnknownCount { get; set; }
            public byte[] Unknown0 { get; set; }
            public byte[] Unknown1 { get; set; }
            public byte[] Unknown2 { get; set; }
            public byte[] Unknown3 { get; set; }
            public byte[] Unknown4 { get; set; }
            public byte[] Unknown5 { get; set; }

            public override void OnClick(TreeView treeview)
            {
                Forms.EmitterEditor editor = (Forms.EmitterEditor)LibraryGUI.GetActiveContent(typeof(Forms.EmitterEditor));
                if (editor == null)
                {
                    editor = new Forms.EmitterEditor();
                    LibraryGUI.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.LoadEmitter(this);
            }

            protected override void ReadBinaryData(FileReader reader, Header header)
            {
                ReadName(reader);
                reader.Seek(0x10); // 0-padding

                // color arrays with key counts
                Color0Array = new STColorArray(reader.ReadUInt32());
                Color0AlphaArray = new STColorArray(reader.ReadUInt32(), true);
                Color1Array = new STColorArray(reader.ReadUInt32());
                Color1AlphaArray = new STColorArray(reader.ReadUInt32(), true);
                ScaleArray = new STColorArray(reader.ReadUInt32());
                UnknownCount = reader.ReadUInt32();
                reader.Seek(0x38); // 0-padding

                Unknown0 = reader.ReadBytes(0x30);


                BlinkIntensity0 = reader.ReadSingle();
                BlinkIntensity1 = reader.ReadSingle();
                BlinkDuration0 = reader.ReadSingle();
                BlinkDuration1 = reader.ReadSingle();

                Unknown1 = reader.ReadBytes(0x2c0);

                Radius = reader.ReadSingle();
                reader.Seek(0x0c); // 0-padding

                // random and animated color table
                Color0Array.ReadColorData(reader);
                Color0AlphaArray.ReadColorData(reader);
                Color1Array.ReadColorData(reader);
                Color1AlphaArray.ReadColorData(reader);
                Unknown2 = reader.ReadBytes(0x40);
                ScaleArray.ReadColorData(reader);

                Unknown3 = reader.ReadBytes(0x328);

                // constant colors
                ConstantColor0 = new STColor(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                ConstantColor1 = new STColor(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

                Unknown4 = reader.ReadBytes(0x30);

                // samplers
                for (int i = 0; i < 3; ++i)
                {
                    SamplerInfo samplerInfo = new SamplerInfo();
                    samplerInfo.ReadSamplerData(reader);
                    Samplers.Add(samplerInfo);
                }

                Unknown5 = reader.ReadBytes(0x30);
            }

            protected override void WriteBinaryData(FileWriter writer)
            {
                WriteName(writer);
                writer.Seek(0x10); // 0-padding
                writer.Write(Color0Array.KeyCount);
                writer.Write(Color0AlphaArray.KeyCount);
                writer.Write(Color1Array.KeyCount);
                writer.Write(Color1AlphaArray.KeyCount);
                writer.Write(ScaleArray.KeyCount);
                writer.Write(UnknownCount);
                writer.Seek(0x38); // 0-padding
                writer.Write(Unknown0);
                writer.Write(BlinkIntensity0);
                writer.Write(BlinkIntensity1);
                writer.Write(BlinkDuration0);
                writer.Write(BlinkDuration1);
                writer.Write(Unknown1);
                writer.Write(Radius);
                writer.Seek(0x0c); // 0-padding
                Color0Array.WriteColorData(writer);
                Color0AlphaArray.WriteColorData(writer);
                Color1Array.WriteColorData(writer);
                Color1AlphaArray.WriteColorData(writer);
                writer.Write(Unknown2);
                ScaleArray.WriteColorData(writer);
                writer.Write(Unknown3);
                writer.Write(ConstantColor0.R);
                writer.Write(ConstantColor0.G);
                writer.Write(ConstantColor0.B);
                writer.Write(ConstantColor0.A);
                writer.Write(ConstantColor1.R);
                writer.Write(ConstantColor1.G);
                writer.Write(ConstantColor1.B);
                writer.Write(ConstantColor1.A);
                writer.Write(Unknown4);
                foreach (var sampler in Samplers)
                {
                    sampler.WriteSamplerData(writer);
                }
                writer.Write(Unknown5);
            }

            protected override int CalculateSize(int dataOffset, int dataEndOffset, int childrenOffset, int endOffset)
            {
                return childrenOffset;
            }

            public class STColorArray
            {
                public const int MAX_KEYS = 8;
                public uint KeyCount { get; private set; }
                public STColor[] ColorKeys { get; private set; } = new STColor[MAX_KEYS]; // TODO refactor to list to make this more generic, them move to Library
                public bool Timed { get; set; } = false;
                public bool IsAlpha { get; private set; }

                public STColorArray(uint keyCount, bool isAlpha = false)
                {
                    KeyCount = keyCount;
                    IsAlpha = isAlpha;
                }

                public void ReadColorData(FileReader reader)
                {
                    for (var i = 0; i < MAX_KEYS; ++i)
                    {
                        ColorKeys[i] = new STColor(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        ColorKeys[i].Time = reader.ReadSingle();
                        if (ColorKeys[i].Time != 0)
                        {
                            Timed = true;
                        }
                    }
                    // add timing information internally even for non-timed arrays, so that we can easily add timing
                    if (!Timed)
                    {
                        for (var i = 1; i < KeyCount; ++i)
                        {
                            ColorKeys[i].Time = (float)i / (KeyCount - 1);
                        }
                    }
                }

                public void WriteColorData(FileWriter writer)
                {
                    for (var i = 0; i < KeyCount; ++i)
                    {
                        writer.Write(ColorKeys[i].R);
                        writer.Write(ColorKeys[i].G);
                        writer.Write(ColorKeys[i].B);
                        writer.Write(Timed ? ColorKeys[i].Time : 0);
                    }

                    for (var i = KeyCount; i < MAX_KEYS; ++i)
                    {
                        writer.Write(0);
                        writer.Write(0);
                        writer.Write(0);
                        writer.Write(0);
                    }
                }

                public void AddKey()
                {
                    if (KeyCount == MAX_KEYS)
                    {
                        return;
                    }

                    for (var i = 1; i < KeyCount; ++i)
                    {
                        ColorKeys[i].Time *= (float)(KeyCount - 1) / KeyCount;
                    }

                    if (++KeyCount > 1)
                    {
                        STColor prevColor = ColorKeys[KeyCount - 2];
                        STColor newColor = ColorKeys[KeyCount - 1];
                        newColor.R = prevColor.R;
                        newColor.G = prevColor.G;
                        newColor.B = prevColor.B;
                        newColor.Time = 1;
                    }
                }

                public void RemoveKey()
                {
                    if (KeyCount == 0) return; // can't have negative keys

                    --KeyCount;

                    if (KeyCount == 0) return; // removed key's time was already 0

                    ColorKeys[KeyCount].Time = 0;

                    if (KeyCount == 1) return; // no intermediate keys to update

                    var ratio = 1 / ColorKeys[KeyCount - 1].Time;
                    for (var i = 1; i < KeyCount; ++i)
                    {
                        ColorKeys[i].Time *= ratio;
                    }
                }
            }

            public class SamplerInfo
            {
                public bool Enabled { get; set; }
                public uint TextureId { get; set; }
                public byte[] Flags = new byte[8];
                public float unknownFloat;

                public void ReadSamplerData(FileReader reader)
                {
                    Enabled = reader.ReadUInt32() == 0; // disabled if NULL_OFFSET
                    TextureId = reader.ReadUInt32();
                    Flags[0] = reader.ReadByte(); // 0, 1, or 2
                    Flags[1] = reader.ReadByte(); // 0, 1, or 2
                    reader.Seek(0x01); // 0-padding
                    Flags[2] = reader.ReadByte(); // 0 or 1
                    unknownFloat = reader.ReadSingle(); // 2, 6 (rare), or 15.99
                    reader.Seek(0x04); // 0-padding
                    Flags[3] = reader.ReadByte(); // 0 or 1
                    Flags[4] = reader.ReadByte(); // 0 or 1
                    Flags[5] = reader.ReadByte(); // 0 or 1
                    Flags[6] = reader.ReadByte(); // 0 or 1
                    Flags[7] = reader.ReadByte(); // 0 or 1
                    reader.Seek(0x07); // 0-padding
                }

                public void WriteSamplerData(FileWriter writer)
                {
                    writer.Write(Enabled ? 0 : Header.NULL_OFFSET);
                    writer.Write(TextureId);
                    writer.Write(Flags[0]);
                    writer.Write(Flags[1]);
                    writer.Seek(0x01);
                    writer.Write(Flags[2]);
                    writer.Write(unknownFloat);
                    writer.Seek(0x04); // 0-padding
                    writer.Write(Flags[3]);
                    writer.Write(Flags[4]);
                    writer.Write(Flags[5]);
                    writer.Write(Flags[6]);
                    writer.Write(Flags[7]);
                    writer.Seek(0x07); // 0-padding
                }
            }
        }

        public class CADP : NodeBase
        {
            public Shape ShapeFlag { get; set; }

            protected override void ReadBinaryData(FileReader reader, Header header)
            {
                ShapeFlag = (Shape)reader.ReadUInt32();
                base.ReadBinaryData(reader, header);
            }

            protected override void WriteBinaryData(FileWriter writer)
            {
                writer.Write((int)ShapeFlag);
                base.WriteBinaryData(writer);
            }

            [Flags]
            public enum Shape
            {
                // Sphere = 0,
                Rod = 1,
                SidewaysRod = 2,
                HollowRod = 4,
                Cone = 8,
                Square = 16,
                Point = 32,
                WidePoint = 64
            }
        }

        #endregion

        #region TEXA

        // TEXture Array?
        public class TEXA : NodeBase
        {
            public TEXA()
            {
                Text = "Textures";
            }

            public override void ReadNode(FileReader reader, Header header)
            {
                // TODO make sure header.DataOffset is always 0x20 and header.AttributesOffset is always NULL_OFFSET
                UnknownHeader = new byte[0];
                UnknownData = new byte[0];

                if (header.ChildrenOffset != Header.NULL_OFFSET)
                {
                    using (reader.TemporarySeek(header.ChildrenOffset))
                    {
                        var nodes = new List<TEXR>();
                        Header texrHeader;
                        while (true)
                        {
                            using (reader.TemporarySeek())
                            {
                                texrHeader = Header.Read(reader);
                            }
                            var node = new TEXR();
                            node.ReadNode(reader, texrHeader);
                            nodes.Add(node);
                            if (texrHeader.SiblingOffset != Header.NULL_OFFSET)
                            {
                                reader.Seek(texrHeader.SiblingOffset);
                            }
                            else
                            {
                                break;
                            }
                        }

                        Children.AddRange(nodes);
                        Nodes.AddRange(nodes.ToArray());
                    }
                }
            }

            protected override void WriteNodes(FileWriter writer, IEnumerator enumerator)
            {
                // first write all TEXR header + data
                var texrOffsets = new List<Offset[]>();
                var texrPositions = new List<int>();
                var last = !enumerator.MoveNext();
                while (!last)
                {
                    int pos = (int)writer.Position;
                    texrPositions.Add(pos);
                    IEFTBNode node = (IEFTBNode)enumerator.Current;
                    Offset[] offsets = node.WriteNode(writer);
                    texrOffsets.Add(offsets);
                    last = !enumerator.MoveNext();
                    offsets[Header.OffsetType.Sibling].Satisfy(last ? (int)Header.NULL_OFFSET : (int)writer.Position - pos);
                }

                // then write all their GX2B children
                enumerator.Reset();
                last = !enumerator.MoveNext();
                int i = 0;
                while (!last)
                {
                    int pos = (int)writer.Position;
                    texrOffsets[i][Header.OffsetType.Children].Satisfy(pos - texrPositions[i]);
                    IEFTBNode node = (IEFTBNode)enumerator.Current;
                    Offset[] offsets = ((TEXR)node).WriteGX2B(writer);
                    last = !enumerator.MoveNext();
                    offsets[Header.OffsetType.Sibling].Satisfy(last ? (int)Header.NULL_OFFSET : (int)writer.Position - pos);
                    ++i;
                }
            }

            protected override int CalculateSize(int dataOffset, int dataEndOffset, int childrenOffset, int endOffset)
            {
                return endOffset - dataOffset;
            }
        }

        public class TEXR : STGenericTexture, IEFTBNode
        {
            public string Signature => "TEXR";

            public override bool CanEdit { get; set; } = false;
            public uint TextureId { get; private set; }

            public override TEX_FORMAT[] SupportedFormats
            {
                get
                {
                    return new TEX_FORMAT[]
                    {
                        TEX_FORMAT.BC1_UNORM,
                        TEX_FORMAT.BC1_UNORM_SRGB,
                        TEX_FORMAT.BC2_UNORM,
                        TEX_FORMAT.BC2_UNORM_SRGB,
                        TEX_FORMAT.BC3_UNORM,
                        TEX_FORMAT.BC3_UNORM_SRGB,
                        TEX_FORMAT.BC4_UNORM,
                        TEX_FORMAT.BC4_SNORM,
                        TEX_FORMAT.BC5_UNORM,
                        TEX_FORMAT.BC5_SNORM,
                        TEX_FORMAT.B5G6R5_UNORM,
                        TEX_FORMAT.B8G8R8A8_UNORM_SRGB,
                        TEX_FORMAT.B8G8R8A8_UNORM,
                        TEX_FORMAT.B5G5R5A1_UNORM,
                        TEX_FORMAT.R8G8B8A8_UNORM_SRGB,
                        TEX_FORMAT.R8G8B8A8_UNORM,
                        TEX_FORMAT.R8_UNORM,
                        TEX_FORMAT.R8G8_UNORM,
                    };
                }
            }

            private int childrenCount;
            private uint TileMode;
            private uint Swizzle = 0;
            private byte WrapMode = 11;
            private byte Depth = 1;
            private uint MipCount;
            private uint CompSel;
            private uint ImageSize;
            private SurfaceFormat SurfFormat;
            private byte[] data;
            private uint unknown1;
            private uint unknown2;
            private uint unknown3;
            private uint unknown4;
            private byte unknown5;
            private short unknown6;
            private uint unknown7;
            private GX2B gx2b;

            public TEXR()
            {
                ImageKey = "Texture";
                SelectedImageKey = "Texture";
            }

            public override void OnClick(TreeView treeView)
            {
                ImageEditorBase editor = (ImageEditorBase)LibraryGUI.GetActiveContent(typeof(ImageEditorBase));
                if (editor == null)
                {
                    editor = new ImageEditorBase();
                    editor.Dock = DockStyle.Fill;
                    LibraryGUI.LoadEditor(editor);
                }

                editor.Text = Text;
                editor.LoadProperties(GenericProperties);
                editor.LoadImage(this);
            }

            public override byte[] GetImageData(int ArrayLevel = 0, int MipLevel = 0)
            {
                uint GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC5_UNORM;

                switch (SurfFormat)
                {
                    case SurfaceFormat.T_BC1_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC1_UNORM;
                        Format = TEX_FORMAT.BC1_UNORM;
                        break;
                    case SurfaceFormat.T_BC1_SRGB:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC1_SRGB;
                        Format = TEX_FORMAT.BC1_UNORM_SRGB;
                        break;
                    case SurfaceFormat.T_BC2_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC2_UNORM;
                        Format = TEX_FORMAT.BC2_UNORM;
                        break;
                    case SurfaceFormat.T_BC2_SRGB:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC2_SRGB;
                        Format = TEX_FORMAT.BC2_UNORM_SRGB;
                        break;
                    case SurfaceFormat.T_BC3_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC3_UNORM;
                        Format = TEX_FORMAT.BC3_UNORM;
                        break;
                    case SurfaceFormat.T_BC3_SRGB:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC3_SRGB;
                        Format = TEX_FORMAT.BC3_UNORM_SRGB;
                        break;
                    case SurfaceFormat.T_BC4_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC4_UNORM;
                        Format = TEX_FORMAT.BC4_UNORM;
                        break;
                    case SurfaceFormat.T_BC4_SNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC4_SNORM;
                        Format = TEX_FORMAT.BC4_SNORM;
                        break;
                    case SurfaceFormat.T_BC5_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC5_UNORM;
                        Format = TEX_FORMAT.BC5_UNORM;
                        break;
                    case SurfaceFormat.T_BC5_SNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.T_BC5_SNORM;
                        Format = TEX_FORMAT.BC5_SNORM;
                        break;
                    case SurfaceFormat.TC_R8_G8_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.TC_R8_G8_UNORM;
                        Format = TEX_FORMAT.R8G8_UNORM;
                        break;
                    case SurfaceFormat.TCS_R8_G8_B8_A8_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNORM;
                        Format = TEX_FORMAT.R8G8B8A8_UNORM;
                        break;
                    case SurfaceFormat.TCS_R8_G8_B8_A8:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.TCS_R8_G8_B8_A8_UNORM;
                        Format = TEX_FORMAT.R8G8B8A8_UNORM;
                        break;
                    case SurfaceFormat.TC_R8_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.TC_R8_UNORM;
                        Format = TEX_FORMAT.R8_UNORM;
                        break;
                    case SurfaceFormat.TCS_R5_G6_B5_UNORM:
                        GX2Format = (uint)GX2.GX2SurfaceFormat.TCS_R5_G6_B5_UNORM;
                        Format = TEX_FORMAT.B5G6R5_UNORM;
                        break;
                    default:
                        throw new Exception("Format unsupported! " + SurfFormat);
                }


                int swizzle = (int)Swizzle;
                int pitch = (int)0;
                uint bpp = GX2.surfaceGetBitsPerPixel(GX2Format) >> 3;

                GX2.GX2Surface surf = new GX2.GX2Surface();
                surf.bpp = bpp;
                surf.height = Height;
                surf.width = Width;
                surf.aa = (uint)0;
                surf.alignment = 0;
                surf.depth = Depth;
                surf.dim = 0x1;
                surf.format = GX2Format;
                surf.use = 0x1;
                surf.pitch = 0;
                surf.data = data;
                surf.numMips = 1;
                surf.mipOffset = new uint[0];
                surf.mipData = null;
                surf.tileMode = TileMode;
                surf.swizzle = Swizzle;
                surf.imageSize = ImageSize;

                return GX2.Decode(surf, ArrayLevel, MipLevel);
            }

            public override void SetImageData(Bitmap bitmap, int ArrayLevel)
            {
                throw new NotImplementedException();
            }

            public void ReadNode(FileReader reader, Header header)
            {
                // TEXR tracks child count and sibling offsets like a standard tree, but each TEXR really only has one GX2B
                childrenCount = header.ChildrenCount; // read the child count here so we can just write it back

                using (reader.TemporarySeek(0x20))
                {
                    ReadBinaryData(reader);
                }

                if (header.ChildrenOffset != Header.NULL_OFFSET)
                {
                    using (reader.TemporarySeek(header.ChildrenOffset))
                    {
                        Header gx2bHeader;
                        using (reader.TemporarySeek())
                        {
                            gx2bHeader = Header.Read(reader);
                        }
                        gx2b = new GX2B();
                        gx2b.ReadNode(reader, gx2bHeader);
                        data = gx2b.UnknownData;
                    }
                }
            }

            private void ReadBinaryData(FileReader reader)
            {
                Width = reader.ReadUInt16();
                Height = reader.ReadUInt16();
                unknown1 = reader.ReadUInt32();
                CompSel = reader.ReadUInt32();
                MipCount = reader.ReadUInt32();
                unknown2 = reader.ReadUInt32();
                TileMode = reader.ReadUInt32();
                unknown3 = reader.ReadUInt32();
                ImageSize = reader.ReadUInt32();
                unknown4 = reader.ReadUInt32();
                TextureId = reader.ReadUInt32();
                Text = TextureId.ToString("X8");
                SurfFormat = reader.ReadEnum<SurfaceFormat>(false);
                unknown5 = reader.ReadByte();
                unknown6 = reader.ReadInt16();
                unknown7 = reader.ReadUInt32();
            }

            public Offset[] WriteNode(FileWriter writer)
            {
                int start = (int)writer.Position;
                Offset[] offsets = WriteHeader(writer);

                int dataOffset = (int)writer.Position - start;
                offsets[Header.OffsetType.Data].Satisfy(dataOffset);
                WriteBinaryData(writer);
                int dataEndOffset = (int)writer.Position - start;

                offsets[Header.OffsetType.Attributes].Satisfy((int)Header.NULL_OFFSET);

                offsets[Header.OffsetType.Size].Satisfy(dataEndOffset - dataOffset);

                return offsets; // leave it to caller to satisfy sibling offset and children offset
            }

            protected Offset[] WriteHeader(FileWriter writer)
            {
                writer.WriteSignature(Signature);
                var offsets = writer.ReserveOffset(5);
                writer.Write(0);
                writer.Write((ushort)childrenCount);
                writer.Write((ushort)1);

                return offsets;
            }

            private void WriteBinaryData(FileWriter writer)
            {
                writer.Write((ushort)Width);
                writer.Write((ushort)Height);
                writer.Write(unknown1);
                writer.Write(CompSel);
                writer.Write(MipCount);
                writer.Write(unknown2);
                writer.Write(TileMode);
                writer.Write(unknown3);
                writer.Write(ImageSize);
                writer.Write(unknown4);
                writer.Write(TextureId);
                writer.Write((byte)SurfFormat);
                writer.Write(unknown5);
                writer.Write(unknown6);
                writer.Write(unknown7);
            }

            public Offset[] WriteGX2B(FileWriter writer)
            {
                return gx2b.WriteNode(writer);
            }

            public enum SurfaceFormat : byte
            {
                INVALID = 0x0,
                TCS_R8_G8_B8_A8 = 2,
                T_BC1_UNORM = 3,
                T_BC1_SRGB = 4,
                T_BC2_UNORM = 5,
                T_BC2_SRGB = 6,
                T_BC3_UNORM = 7,
                T_BC3_SRGB = 8,
                T_BC4_UNORM = 9,
                T_BC4_SNORM = 10,
                T_BC5_UNORM = 11,
                T_BC5_SNORM = 12,
                TC_R8_UNORM = 13,
                TC_R8_G8_UNORM = 14,
                TCS_R8_G8_B8_A8_UNORM = 15,
                TCS_R5_G6_B5_UNORM = 25,
            };
        }

        public class GX2B : NodeSpecial
        {
        }

        #endregion

        #region PRMA

        // P?R?M? Array?
        public class PRMA : NodeBase
        {
            public override Offset[] WriteNode(FileWriter writer)
            {
                Offset[] offsets = base.WriteNode(writer);
                if (Nodes.Count == 0)
                {
                    offsets[Header.OffsetType.Data].Satisfy((int)Header.NULL_OFFSET); // empty PRMA has no binary data offset for some reason
                }

                return offsets;
            }

            protected override int CalculateSize(int dataOffset, int dataEndOffset, int childrenOffset, int endOffset)
            {
                return endOffset - dataOffset;
            }
        }

        // P?R?I?M?
        public class PRIM : NodeSpecial
        {
        }

        #endregion

        #region SHDA

        // SHaDer Array
        public class SHDA : NodeBase
        {
            private ushort childrenCount;

            public override void ReadNode(FileReader reader, Header header)
            {
                header.DataOffset = SwapBytes(header.DataOffset);
                childrenCount = header.ChildrenCount; // SHDA needs to keep track of this for writing because idk what it is (it's not actually child count)
                base.ReadNode(reader, header);
            }

            public override Offset[] WriteNode(FileWriter writer)
            {
                Offset[] offsets = base.WriteNode(writer);
                offsets[Header.OffsetType.Data].Satisfy(0x20000000); // TODO don't hardcode this; determine the offset then flip the bytes.

                return offsets;
            }

            protected override Offset[] WriteHeader(FileWriter writer)
            {
                writer.WriteSignature(Signature);
                var offsets = writer.ReserveOffset(5);
                writer.Write(0);
                writer.Write(childrenCount); // overriding just so we can set this
                writer.Write((ushort)1);

                return offsets;
            }

            protected override int CalculateSize(int dataOffset, int dataEndOffset, int childrenOffset, int endOffset)
            {
                return dataEndOffset - dataOffset;
            }

            private uint SwapBytes(uint x)
            {
                // swap adjacent 16-bit blocks
                x = (x >> 16) | (x << 16);
                // swap adjacent 8-bit blocks
                return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
            }
        }

        // SHaDer Blocks
        public class SHDB : NodeSpecial
        {
        }

        #endregion
    }
}