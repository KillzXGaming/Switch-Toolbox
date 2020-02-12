using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Syroot.Maths;
using Toolbox.Library.IO;
using Toolbox.Library;

namespace LayoutBXLYT.Cafe
{
    public class WND1 : PAN1, IWindowPane
    {
        private Header layoutHeader;

        public override string Signature { get; } = "wnd1";

        public WND1() : base()
        {

        }

        public void CopyWindows()
        {
            Content = (BxlytWindowContent)Content.Clone();
            for (int f = 0; f < WindowFrames.Count; f++)
                WindowFrames[f] = (BxlytWindowFrame)WindowFrames[f].Clone();
        }

        public WND1(Header header, string name)
        {
            layoutHeader = header;
            LoadDefaults();
            Name = name;

            Content = new BxlytWindowContent(header, this.Name);
            UseOneMaterialForAll = true;
            UseVertexColorForAll = true;
            WindowKind = WindowKind.Around;
            NotDrawnContent = false;

            StretchLeft = 0;
            StretchRight = 0;
            StretchTop = 0;
            StretchBottm = 0;

            Width = 70;
            Height = 80;

            FrameElementLeft = 16;
            FrameElementRight = 16;
            FrameElementTop = 16;
            FrameElementBottm = 16;
            FrameCount = 1;

            WindowFrames = new List<BxlytWindowFrame>();
            SetFrames(header);
        }

        public void ReloadFrames()
        {
            SetFrames(layoutHeader);
        }

        public void SetFrames(Header header)
        {
            if (WindowFrames == null)
                WindowFrames = new List<BxlytWindowFrame>();

            switch (FrameCount)
            {
                case 1:
                    if (WindowFrames.Count == 0)
                        WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_LT"));
                    break;
                case 2:
                    if (WindowFrames.Count == 0)
                        WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_L"));
                    if (WindowFrames.Count == 1)
                        WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_R"));
                    break;
                case 4:
                    if (WindowFrames.Count == 0)
                        WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_LT"));
                    if (WindowFrames.Count == 1)
                        WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_RT"));
                    if (WindowFrames.Count == 2)
                        WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_LB"));
                    if (WindowFrames.Count == 3)
                        WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_RB"));
                    break;
                case 8:
                    if (WindowFrames.Count == 0)
                        WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_LT"));
                    if (WindowFrames.Count == 1)
                        WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_RT"));
                    if (WindowFrames.Count == 2)
                        WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_LB"));
                    if (WindowFrames.Count == 3)
                        WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_RB"));
                    if (WindowFrames.Count == 4)
                        WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_T"));
                    if (WindowFrames.Count == 5)
                        WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_B"));
                    if (WindowFrames.Count == 6)
                        WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_R"));
                    if (WindowFrames.Count == 7)
                        WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_L"));
                    break;
            }

            //Now search for invalid unused materials and remove them
            for (int i = 0; i < WindowFrames.Count; i++)
            {
                if (i >= FrameCount)
                    layoutHeader.TryRemoveMaterial(WindowFrames[i].Material);
                else if (!layoutHeader.Materials.Contains(WindowFrames[i].Material))
                    layoutHeader.AddMaterial(WindowFrames[i].Material);
            }
        }

        public bool UseOneMaterialForAll
        {
            get { return Convert.ToBoolean(_flag & 1); }
            set { }
        }

        public bool UseVertexColorForAll
        {
            get { return Convert.ToBoolean((_flag >> 1) & 1); }
            set { }
        }

        public WindowKind WindowKind { get; set; }

        public bool NotDrawnContent
        {
            get { return (_flag & 8) == 16; }
            set { }
        }

        public ushort StretchLeft { get; set; }
        public ushort StretchRight { get; set; }
        public ushort StretchTop { get; set; }
        public ushort StretchBottm { get; set; }
        public ushort FrameElementLeft { get; set; }
        public ushort FrameElementRight { get; set; }
        public ushort FrameElementTop { get; set; }
        public ushort FrameElementBottm { get; set; }
        private byte _flag;

        private byte frameCount;
        public byte FrameCount
        {
            get { return frameCount; }
            set
            {
                frameCount = value;
            }
        }

        public System.Drawing.Color[] GetVertexColors()
        {
            return new System.Drawing.Color[4]
            {
                    Content.ColorTopLeft.Color,
                    Content.ColorTopRight.Color,
                    Content.ColorBottomLeft.Color,
                    Content.ColorBottomRight.Color,
            };
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytWindowContent Content { get; set; }

        [Browsable(false)]
        public List<BxlytWindowFrame> WindowFrames { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytWindowFrame TopLeftFrame
        {
            get { return WindowFrames.Count >= 1 ? WindowFrames[0] : null; }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytWindowFrame TopRightFrame
        {
            get { return WindowFrames.Count >= 2 ? WindowFrames[1] : null; }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytWindowFrame BottomLeftFrame
        {
            get { return WindowFrames.Count >= 3 ? WindowFrames[2] : null; }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytWindowFrame BottomRightFrame
        {
            get { return WindowFrames.Count >= 4 ? WindowFrames[3] : null; }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytWindowFrame TopFrame
        {
            get { return WindowFrames.Count >= 5 ? WindowFrames[4] : null; }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytWindowFrame BottomFrame
        {
            get { return WindowFrames.Count >= 6 ? WindowFrames[5] : null; }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytWindowFrame LeftFrame
        {
            get { return WindowFrames.Count >= 7 ? WindowFrames[6] : null; }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytWindowFrame RightFrame
        {
            get { return WindowFrames.Count >= 8 ? WindowFrames[7] : null; }
        }

        public WND1(FileReader reader, Header header) : base(reader, header)
        {
            layoutHeader = header;
            WindowFrames = new List<BxlytWindowFrame>();

            long pos = reader.Position - 0x54;

            StretchLeft = reader.ReadUInt16();
            StretchRight = reader.ReadUInt16();
            StretchTop = reader.ReadUInt16();
            StretchBottm = reader.ReadUInt16();
            FrameElementLeft = reader.ReadUInt16();
            FrameElementRight = reader.ReadUInt16();
            FrameElementTop = reader.ReadUInt16();
            FrameElementBottm = reader.ReadUInt16();
            FrameCount = reader.ReadByte();
            _flag = reader.ReadByte();
            reader.ReadUInt16();//padding
            uint contentOffset = reader.ReadUInt32();
            uint frameOffsetTbl = reader.ReadUInt32();

            WindowKind = (WindowKind)((_flag >> 2) & 3);

            reader.SeekBegin(pos + contentOffset);
            Content = new BxlytWindowContent(reader, header);

            reader.SeekBegin(pos + frameOffsetTbl);

            var offsets = reader.ReadUInt32s(FrameCount);
            foreach (int offset in offsets)
            {
                reader.SeekBegin(pos + offset);
                WindowFrames.Add(new BxlytWindowFrame(reader, header));
            }
        }

        public override void Write(FileWriter writer, LayoutHeader header)
        {
            long pos = writer.Position - 8;

            base.Write(writer, header);
            writer.Write(StretchLeft);
            writer.Write(StretchRight);
            writer.Write(StretchTop);
            writer.Write(StretchBottm);
            writer.Write(FrameElementLeft);
            writer.Write(FrameElementRight);
            writer.Write(FrameElementTop);
            writer.Write(FrameElementBottm);
            writer.Write(FrameCount);
            writer.Write(_flag);
            writer.Write((ushort)0);

            long _ofsContentPos = writer.Position;
            writer.Write(0);
            writer.Write(0);

            writer.WriteUint32Offset(_ofsContentPos, pos);
            Content.Write(writer);

            if (WindowFrames.Count > 0)
            {
                writer.WriteUint32Offset(_ofsContentPos + 4, pos);
                //Reserve space for frame offsets
                long _ofsFramePos = writer.Position;
                writer.Write(new uint[FrameCount]);
                for (int i = 0; i < FrameCount; i++)
                {
                    writer.WriteUint32Offset(_ofsFramePos + (i * 4), pos);
                    WindowFrames[i].Write(writer);
                }
            }
        }
    }
}
