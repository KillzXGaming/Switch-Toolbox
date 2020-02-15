using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace LayoutBXLYT.Revolution
{
    public class AlphaCompare : LayoutBXLYT.BxlytAlphaCompare
    {
        public GfxAlphaFunction Comp0 { get; set; }
        public GfxAlphaFunction Comp1 { get; set; }
        public GfxAlphaOp AlphaOp { get; set; }
        public byte Ref0 { get; set; }
        public byte Ref1 { get; set; }

        public AlphaCompare()
        {
            Comp0 = GfxAlphaFunction.Always;
            Comp1 = GfxAlphaFunction.Always;
            AlphaOp = GfxAlphaOp.And;
            Ref0 = 0;
            Ref1 = 0;
        }

        public bool HasDefaults() {
            return Comp0 == GfxAlphaFunction.Always &&
                   Comp1 == GfxAlphaFunction.Always &&
                   AlphaOp == GfxAlphaOp.And &&
                   Ref0 == 0 &&
                   Ref1 == 0;
        }

        public AlphaCompare(FileReader reader) : base()
        {
            byte c = reader.ReadByte();
            Comp0 = (GfxAlphaFunction)(c & 0x7);
            Comp1 = (GfxAlphaFunction)((c >> 4) & 0x7);
            AlphaOp = (GfxAlphaOp)reader.ReadByte();
            Ref0 = reader.ReadByte();
            Ref1 = reader.ReadByte();
        }

        public override void Write(FileWriter writer)
        {
            byte c = 0;
            c |= (byte)(((byte)Comp1 & 0x7) << 4);
            c |= (byte)(((byte)Comp0 & 0x7) << 0);
            writer.Write(c);
            writer.Write((byte)AlphaOp);
            writer.Write(Ref0);
            writer.Write(Ref1);
        }
    }
}
