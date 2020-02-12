using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class TevStage : BxlytTevStage
    {
        private byte colorFlags;
        private byte alphaFlags;

        public TevStage(FileReader reader, Header header)
        {
            colorFlags = reader.ReadByte();
            alphaFlags = reader.ReadByte();
            reader.ReadUInt16(); //padding

            ColorMode = (TevMode)colorFlags;
            AlphaMode = (TevMode)colorFlags;

            /*     TevSource srcRGB0 = (TevSource)Bit.ExtractBits8(colorFlags, 4, 0);
                 TevSource srcRGB1 = (TevSource)Bit.ExtractBits8(colorFlags, 4, 4);
                 TevSource srcRGB2 = (TevSource)Bit.ExtractBits8(colorFlags, 4, 8);
                 TevColorOp opRGB0 = (TevColorOp)Bit.ExtractBits8(colorFlags, 4, 12);
                 TevColorOp opRGB1 = (TevColorOp)Bit.ExtractBits8(colorFlags, 4, 16);
                 TevColorOp opRGB2 = (TevColorOp)Bit.ExtractBits8(colorFlags, 4, 20);

                 ColorSources = new TevSource[] { srcRGB0, srcRGB1, srcRGB2 };
                 ColorOperators = new TevColorOp[] { opRGB0, opRGB1, opRGB2 };
                 ColorMode = (TevMode)Bit.ExtractBits8(colorFlags, 4, 24);
                 ColorScale = (TevScale)Bit.ExtractBits8(colorFlags, 2, 28);
                 ColorSavePrevReg = Bit.ExtractBits8(colorFlags, 1, 30) == 1;

                 TevSource srcAlpha0 = (TevSource)Bit.ExtractBits8(alphaFlags, 4, 0);
                 TevSource srcAlpha1 = (TevSource)Bit.ExtractBits8(alphaFlags, 4, 4);
                 TevSource srcAlpha2 = (TevSource)Bit.ExtractBits8(alphaFlags, 4, 8);
                 TevAlphaOp opAlpha0 = (TevAlphaOp)Bit.ExtractBits8(alphaFlags, 4, 12);
                 TevAlphaOp opAlpha1 = (TevAlphaOp)Bit.ExtractBits8(alphaFlags, 4, 16);
                 TevAlphaOp opAlpha2 = (TevAlphaOp)Bit.ExtractBits8(alphaFlags, 4, 20);

                 AlphaSources = new TevSource[] { srcAlpha0, srcAlpha1, srcAlpha2 };
                 AlphaOperators = new TevAlphaOp[] { opAlpha0, opAlpha1, opAlpha2 };
                 AlphaMode = (TevMode)Bit.ExtractBits8(alphaFlags, 4, 24);
                 AlphaScale = (TevScale)Bit.ExtractBits8(alphaFlags, 2, 28);
                 AlphaSavePrevReg = Bit.ExtractBits8(alphaFlags, 1, 30) == 1;*/
        }

        public void Write(FileWriter writer)
        {
            writer.Write(colorFlags);
            writer.Write(alphaFlags);
            writer.Write((ushort)0);
        }
    }
}
