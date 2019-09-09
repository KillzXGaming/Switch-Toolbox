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

namespace FirstPlugin
{
    public class EFTB : TreeNodeFile, IFileFormat
    {
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

        #region Sections

        // TODO is this common accross several formats? can split it out of EFTB.
        public class NodeBase : TreeNodeCustom
        {
            public static readonly uint NULL_OFFSET = 0xffffffff;
            public string Signature => GetType().Name == typeof(NodeBase).Name ? Text : GetType().Name;
            public byte[] UnknownHeader { get; protected set; }
            public byte[] UnknownData { get; set; }
            public List<NodeBase> Attributes { get; } = new List<NodeBase>();
            public List<NodeBase> Children { get; } = new List<NodeBase>();

            public static NodeBase[] ReadNodes(FileReader reader, bool align = false)
            {
                var nodes = new List<NodeBase>();
                Header header;
                while (true)
                {
                    if (align) reader.Align(16);
                    using (reader.TemporarySeek())
                    {
                        header = ReadHeader(reader);
                    }
                    var node = Create(header.Signature);
                    node.ReadNode(reader, header);
                    nodes.Add(node);
                    if (header.SiblingOffset != NULL_OFFSET)
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

            private static Header ReadHeader(FileReader reader)
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
                    offsets[OffsetType.Sibling].Satisfy(last ? (int)NULL_OFFSET : (int)writer.Position - pos);
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

            protected virtual void ReadNode(FileReader reader, Header header)
            {
                using (reader.TemporarySeek(0x20))
                {
                    // sometimes the header has more data
                    int extraHeaderSize = (int)(header.DataOffset - 0x20);
                    header.Extra = extraHeaderSize > 0 ? reader.ReadBytes(extraHeaderSize) : new byte[0];
                    UnknownHeader = header.Extra; // save it so it can be written verbatim

                    ReadBinaryData(reader, header);
                }

                if (header.AttributesOffset != NULL_OFFSET)
                {
                    using (reader.TemporarySeek(header.AttributesOffset))
                    {
                        var nodes = ReadNodes(reader);
                        Attributes.AddRange(nodes);
                    }
                }

                if (header.ChildrenOffset != NULL_OFFSET)
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
                   header.AttributesOffset != NULL_OFFSET ? header.AttributesOffset :
                   header.ChildrenOffset != NULL_OFFSET ? header.ChildrenOffset :
                   header.SiblingOffset != NULL_OFFSET ? header.SiblingOffset :
                   header.NodeSize);
                UnknownData = reader.ReadBytes((int)stop - (int)reader.Position);
            }

            public virtual Offset[] WriteNode(FileWriter writer)
            {
                int start = (int)writer.Position;
                Offset[] offsets = WriteHeader(writer);

                int dataOffset = (int)writer.Position - start;
                offsets[OffsetType.Data].Satisfy(dataOffset);
                WriteBinaryData(writer);
                int dataEndOffset = (int)writer.Position - start;

                if (Attributes.Count > 0)
                {
                    offsets[OffsetType.Attributes].Satisfy((int)writer.Position - start);
                    WriteNodes(writer, Attributes.GetEnumerator());
                }
                else
                {
                    offsets[OffsetType.Attributes].Satisfy((int)NULL_OFFSET);
                }

                int childrenOffset = (int)writer.Position - start;
                if (Children.Count > 0)
                {
                    offsets[OffsetType.Children].Satisfy(childrenOffset);
                    WriteNodes(writer, Children.GetEnumerator());
                }
                else
                {
                    offsets[OffsetType.Children].Satisfy((int)NULL_OFFSET);
                }

                offsets[OffsetType.Size].Satisfy(CalculateSize(dataOffset, dataEndOffset, childrenOffset, (int)writer.Position - start));

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
                    NodeBase node = (NodeBase)enumerator.Current;
                    Offset[] offsets = node.WriteNode(writer);
                    last = !enumerator.MoveNext();
                    offsets[OffsetType.Sibling].Satisfy(last ? (int)NULL_OFFSET : (int)writer.Position - pos);
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

            // holds data for EFTB node headers. this is tightly coupled to reading/writing the file and should not be persisted in application models.
            protected class Header
            {
                public long Position { get; set; } // not a part of the header, but describes where it's found in the stream
                public string Signature { get; set; } // node signature/magic (4 bytes)
                public uint NodeSize { get; set; } // size of the entire node including header, children, and attributes
                public uint ChildrenOffset { get; set; } = NULL_OFFSET; // offset to the first child
                public uint SiblingOffset { get; set; } = NULL_OFFSET; // offset to the next sibling node
                public uint AttributesOffset { get; set; } = NULL_OFFSET; // offset to the first attribute
                public uint DataOffset { get; set; } // offset to the start of binary data (i.e. size of the header)
                public ushort ChildrenCount { get; set; } // usually number of children, but there are many exceptions
                public byte[] Extra { get; set; } // extra header data past the standard 32 bytes
            }

            protected class OffsetType
            {
                public static readonly int Size = 0;
                public static readonly int Children = 1;
                public static readonly int Sibling = 2;
                public static readonly int Attributes = 3;
                public static readonly int Data = 4;
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
                reader.Seek(0x08); // 0-padding

                Unknown0 = reader.ReadBytes(0x60);

                BlinkIntensity0 = reader.ReadSingle();
                BlinkDuration0 = reader.ReadSingle();
                BlinkIntensity1 = reader.ReadSingle();
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
                writer.Seek(0x08); // 0-padding
                writer.Write(Unknown0);
                writer.Write(BlinkIntensity0);
                writer.Write(BlinkDuration0);
                writer.Write(BlinkIntensity1);
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
                public ulong TextureId { get; set; }
                private byte[] unknown;

                public void ReadSamplerData(FileReader reader)
                {
                    TextureId = reader.ReadUInt64();
                    unknown = reader.ReadBytes(0x18);
                }

                public void WriteSamplerData(FileWriter writer)
                {
                    writer.Write(TextureId);
                    writer.Write(unknown);
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

        // TEXture Array?
        public class TEXA : NodeBase
        {
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
                    NodeBase node = (NodeBase)enumerator.Current;
                    Offset[] offsets = node.WriteNode(writer);
                    texrOffsets.Add(offsets);
                    last = !enumerator.MoveNext();
                    offsets[OffsetType.Sibling].Satisfy(last ? (int)NULL_OFFSET : (int)writer.Position - pos);
                }

                // then write all their GX2B children
                enumerator.Reset();
                last = !enumerator.MoveNext();
                int i = 0;
                while (!last)
                {
                    int pos = (int)writer.Position;
                    texrOffsets[i][OffsetType.Children].Satisfy(pos - texrPositions[i]);
                    NodeBase node = (NodeBase)enumerator.Current;
                    Offset[] offsets = ((TEXR)node).WriteGX2B(writer);
                    last = !enumerator.MoveNext();
                    offsets[OffsetType.Sibling].Satisfy(last ? (int)NULL_OFFSET : (int)writer.Position - pos);
                    ++i;
                }
            }

            protected override int CalculateSize(int dataOffset, int dataEndOffset, int childrenOffset, int endOffset)
            {
                return endOffset - dataOffset;
            }
        }

        public class TEXR : NodeSpecial
        {
            private int childrenCount;

            public Offset[] WriteGX2B(FileWriter writer)
            {
                return ((GX2B)Nodes[0]).WriteNode(writer);
            }

            protected override void ReadNode(FileReader reader, Header header)
            {
                // TEXR tracks child count and sibling offsets like a standard tree, but each TEXR really only has one GX2B
                childrenCount = header.ChildrenCount; // read the child count here so we can just write it back
                base.ReadNode(reader, header); // this will read all linked children
                TreeNode node = Nodes[0];
                Nodes.Clear(); // remove all except the first
                Nodes.Add(node);
            }

            protected override Offset[] WriteHeader(FileWriter writer)
            {
                writer.WriteSignature(Signature);
                var offsets = writer.ReserveOffset(5);
                writer.Write(0);
                writer.Write((ushort)childrenCount); // overriding just so we can set this
                writer.Write((ushort)1);

                return offsets;
            }

            protected override void WriteNodes(FileWriter writer, IEnumerator enumerator)
            {
                // do nothing. will write them later.
            }
        }

        public class GX2B : NodeSpecial
        {
        }

        // P?R?M? Array?
        public class PRMA : NodeBase
        {
            public override Offset[] WriteNode(FileWriter writer)
            {
                Offset[] offsets = base.WriteNode(writer);
                if (Nodes.Count == 0)
                {
                    offsets[OffsetType.Data].Satisfy((int)NULL_OFFSET); // empty PRMA has no binary data offset for some reason
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

        // SHaDer Array
        public class SHDA : NodeBase
        {
            private ushort childrenCount;

            protected override void ReadNode(FileReader reader, Header header)
            {
                header.DataOffset = SwapBytes(header.DataOffset);
                childrenCount = header.ChildrenCount; // SHDA needs to keep track of this for writing because idk what it is (it's not actually child count)
                base.ReadNode(reader, header);
            }

            public override Offset[] WriteNode(FileWriter writer)
            {
                Offset[] offsets = base.WriteNode(writer);
                offsets[OffsetType.Data].Satisfy(0x20000000); // TODO don't hardcode this; determine the offset then flip the bytes.

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