using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace LayoutBXLYT.CTR
{
    public class TevStage : BxlytTevStage
    {
        public TevSource[] ColorSources;
        public TevColorOp[] ColorOperators;
        public TevScale ColorScale;
        public Boolean ColorSavePrevReg;

        public byte ColorUnknown;
        public TevSource[] AlphaSources;
        public TevAlphaOp[] AlphaOperators;
        public TevScale AlphaScale;
        public Boolean AlphaSavePrevReg;

        public uint ConstColors;

        private uint flags1;
        private uint flags2;

        public TevStage() { }

        public TevStage(FileReader reader, BxlytHeader header)
        {
            flags1 = reader.ReadUInt32();
            ColorSources = new TevSource[] { (TevSource)(flags1 & 0xF), (TevSource)((flags1 >> 4) & 0xF), (TevSource)((flags1 >> 8) & 0xF) };
            ColorOperators = new TevColorOp[] { (TevColorOp)((flags1 >> 12) & 0xF), (TevColorOp)((flags1 >> 16) & 0xF), (TevColorOp)((flags1 >> 20) & 0xF) };
            ColorMode = (TevMode)((flags1 >> 24) & 0xF);
            ColorScale = (TevScale)((flags1 >> 28) & 0x3);
            ColorSavePrevReg = ((flags1 >> 30) & 0x1) == 1;
            flags2 = reader.ReadUInt32();
            AlphaSources = new TevSource[] { (TevSource)(flags2 & 0xF), (TevSource)((flags2 >> 4) & 0xF), (TevSource)((flags2 >> 8) & 0xF) };
            AlphaOperators = new TevAlphaOp[] { (TevAlphaOp)((flags2 >> 12) & 0xF), (TevAlphaOp)((flags2 >> 16) & 0xF), (TevAlphaOp)((flags2 >> 20) & 0xF) };
            AlphaMode = (TevMode)((flags2 >> 24) & 0xF);
            AlphaScale = (TevScale)((flags2 >> 28) & 0x3);
            AlphaSavePrevReg = ((flags2 >> 30) & 0x1) == 1;

            ConstColors = reader.ReadUInt32();
        }

        public void Write(FileWriter writer)
        {
            writer.Write(flags1);
            writer.Write(flags2);
        }
    }
}
