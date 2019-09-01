using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class BlendMode
    {
        public GX2BlendOp BlendOp { get; set; }
        public GX2BlendFactor SourceFactor { get; set; }
        public GX2BlendFactor DestFactor { get; set; }
        public GX2LogicOp LogicOp { get; set; }

        public BlendMode(FileReader reader, BFLYT.Header header)
        {
            BlendOp = (GX2BlendOp)reader.ReadByte();
            SourceFactor = (GX2BlendFactor)reader.ReadByte();
            DestFactor = (GX2BlendFactor)reader.ReadByte();
            LogicOp = (GX2LogicOp)reader.ReadByte();
        }

        public void Write(FileWriter writer)
        {
            writer.Write(BlendOp, false);
            writer.Write(SourceFactor, false);
            writer.Write(DestFactor, false);
            writer.Write(LogicOp, false);
        }

        public enum GX2BlendFactor : byte
        {
            Factor0 = 0,
            Factor1 = 1,
            DestColor = 2,
            DestInvColor = 3,
            SourceAlpha = 4,
            SourceInvAlpha = 5,
            DestAlpha = 6,
            DestInvAlpha = 7,
            SourceColor = 8,
            SourceInvColor = 9
        }

        public enum GX2BlendOp : byte
        {
            Disable = 0,
            Add = 1,
            Subtract = 2,
            ReverseSubtract = 3,
            SelectMin = 4,
            SelectMax = 5
        }

        public enum GX2LogicOp : byte
        {
            Disable = 0,
            NoOp = 1,
            Clear = 2,
            Set = 3,
            Copy = 4,
            InvCopy = 5,
            Inv = 6,
            And = 7,
            Nand = 8,
            Or = 9,
            Nor = 10,
            Xor = 11,
            Equiv = 12,
            RevAnd = 13,
            InvAd = 14,
            RevOr = 15,
            InvOr = 16
        }
    }
}
