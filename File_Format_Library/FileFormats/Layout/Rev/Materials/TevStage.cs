using Toolbox.Library.IO;
using System.ComponentModel;

namespace LayoutBXLYT.Revolution
{
	//From https://github.com/Gericom/WiiLayoutEditor/blob/master/WiiLayoutEditor/IO/BRLYT.cs#L1201
	//Todo this needs major cleanup
	public class TevStage : BxlytTevStage
    {
		[Category("Fragment Sources")]
		public TexMapID TexMap { get; set; }
		[Category("Fragment Sources")]
		public TexCoordID TexCoord { get; set; }
		[Category("Fragment Sources")]
		public byte Color { get; set; }
		[Category("Fragment Sources")]
		public TevSwapSel RasSel { get; set; }
		[Category("Fragment Sources")]
		public TevSwapSel TexSel { get; set; }

		[Category("Color Output")]
		public TevKColorSel ColorConstantSel { get; set; }
		[Category("Color Output")]
		public ColorArg ColorA { get; set; }
		[Category("Color Output")]
		public ColorArg ColorB { get; set; }
		[Category("Color Output")]
		public ColorArg ColorC { get; set; }
		[Category("Color Output")]
		public ColorArg ColorD { get; set; }
		[Category("Color Output")]
		public Bias ColorBias { get; set; }
		[Category("Color Output")]
		public TevColorOp ColorOp { get; set; }
		[Category("Color Output")]
		public bool ColorClamp { get; set; }
		[Category("Color Output")]
		public TevScale ColorScale { get; set; }
		[Category("Color Output")]
		public TevColorRegID ColorRegID { get; set; }

		[Category("Alpha Output")]
		public TevKAlphaSel AlphaConstantSel { get; set; }
		[Category("Alpha Output")]
		public AlphaArg AlphaA { get; set; }
		[Category("Alpha Output")]
		public AlphaArg AlphaB { get; set; }
		[Category("Alpha Output")]
		public AlphaArg AlphaC { get; set; }
		[Category("Alpha Output")]
		public AlphaArg AlphaD { get; set; }
		[Category("Alpha Output")]
		public Bias AlphaBias { get; set; }
		[Category("Alpha Output")]
		public TevAlphaOp AlphaOp { get; set; }
		[Category("Alpha Output")]
		public bool AlphaClamp { get; set; }
		[Category("Alpha Output")]
		public TevScale AlphaScale { get; set; }
		[Category("Alpha Output")]
		public TevAlphaRegID AlphaRegID { get; set; }

		[Category("Indirect Texturing")]
		public IndTexFormat Format { get; set; }
		[Category("Indirect Texturing")]
		public byte TexID { get; set; }
		[Category("Indirect Texturing")]
		public Bias IndBias { get; set; }

		[Category("Indirect Texturing")]
		public IndTexMtxID Matrix { get; set; }
		[Category("Indirect Texturing")]
		public IndTexWrap WrapS { get; set; }
		[Category("Indirect Texturing")]
		public IndTexWrap WrapT { get; set; }
		[Category("Indirect Texturing")]
		public byte UsePreviousStage { get; set; }
		[Category("Indirect Texturing")]
		public byte UnmodifiedLOD { get; set; }
		[Category("Indirect Texturing")]
		public IndTexAlphaSel Alpha { get; set; }

		public TevStage()
		{
			TexCoord = TexCoordID.TexCoord0;
			Color = 255;
			TexMap = TexMapID.TexMap0;
			RasSel = TevSwapSel.Swap0;
			TexSel = TevSwapSel.Swap0;

			ColorA = ColorArg.Zero;
			ColorB = ColorArg.Zero;
			ColorC = ColorArg.Zero;
			ColorD = ColorArg.Zero;
			ColorOp = TevColorOp.Add;
			ColorBias = Bias.Zero;
			ColorScale = TevScale.MultiplyBy1;
			ColorClamp = true;
			ColorRegID = TevColorRegID.OutputColor;
			ColorConstantSel = TevKColorSel.Constant1_1;

			AlphaA = AlphaArg.Zero;
			AlphaB = AlphaArg.Zero;
			AlphaC = AlphaArg.Zero;
			AlphaD = AlphaArg.Zero;
			AlphaOp = TevAlphaOp.Add;
			AlphaBias = Bias.Zero;
			AlphaScale = TevScale.MultiplyBy1;
			AlphaClamp = true;
			AlphaRegID = TevAlphaRegID.OutputAlpha;
			AlphaConstantSel = TevKAlphaSel.Constant1_1;

			TexID = 0;
			Format = IndTexFormat.F_8_Bit_Offsets;
			IndBias = Bias.Zero;
			Matrix = IndTexMtxID.Matrix0;
			WrapS = IndTexWrap.NoWrap;
			WrapT = IndTexWrap.NoWrap;
			UsePreviousStage = 0;
			UnmodifiedLOD = 0;
			Alpha = IndTexAlphaSel.Off;
		}

		public TevStage(FileReader reader, BxlytHeader header)
        {
			TexCoord = (TexCoordID)reader.ReadByte();
			Color = reader.ReadByte();

			ushort tmp16 = reader.ReadUInt16();
			TexMap = (TexMapID)(tmp16 & 0x1ff);
			RasSel = (TevSwapSel)((tmp16 & 0x7ff) >> 9);
			TexSel = (TevSwapSel)(tmp16 >> 11);

			byte tmp8 = reader.ReadByte();
			ColorA = (ColorArg)(tmp8 & 0xf);
			ColorB = (ColorArg)(tmp8 >> 4);
			tmp8 = reader.ReadByte();
			ColorC = (ColorArg)(tmp8 & 0xf);
			ColorD = (ColorArg)(tmp8 >> 4);
			tmp8 = reader.ReadByte();
			ColorOp = (TevColorOp)(tmp8 & 0xf);
			ColorBias = (Bias)((tmp8 & 0x3f) >> 4);
			ColorScale = (TevScale)(tmp8 >> 6);
			tmp8 = reader.ReadByte();
			ColorClamp = (tmp8 & 0x1) == 1;
			ColorRegID = (TevColorRegID)((tmp8 & 0x7) >> 1);
			ColorConstantSel = (TevKColorSel)(tmp8 >> 3);

			tmp8 = reader.ReadByte();
			AlphaA = (AlphaArg)(tmp8 & 0xf);
			AlphaB = (AlphaArg)(tmp8 >> 4);
			tmp8 = reader.ReadByte();
			AlphaC = (AlphaArg)(tmp8 & 0xf);
			AlphaD = (AlphaArg)(tmp8 >> 4);
			tmp8 = reader.ReadByte();
			AlphaOp = (TevAlphaOp)(tmp8 & 0xf);
			AlphaBias = (Bias)((tmp8 & 0x3f) >> 4);
			AlphaScale = (TevScale)(tmp8 >> 6);
			tmp8 = reader.ReadByte();
			AlphaClamp = (tmp8 & 0x1) == 1;
			AlphaRegID = (TevAlphaRegID)((tmp8 & 0x7) >> 1);
			AlphaConstantSel = (TevKAlphaSel)(tmp8 >> 3);

			tmp8 = reader.ReadByte();
			TexID = (byte)(tmp8 & 0x3);
			tmp8 = reader.ReadByte();
			IndBias = (Bias)(tmp8 & 0x7);
			Matrix = (IndTexMtxID)((tmp8 & 0x7F) >> 3);
			tmp8 = reader.ReadByte();
			WrapS = (IndTexWrap)(tmp8 & 0x7);
			WrapT = (IndTexWrap)((tmp8 & 0x3F) >> 3);
			tmp8 = reader.ReadByte();
			Format = (IndTexFormat)(tmp8 & 0x3);
			UsePreviousStage = (byte)((tmp8 & 0x7) >> 2);
			UnmodifiedLOD = (byte)((tmp8 & 0xF) >> 3);
			Alpha = (IndTexAlphaSel)((tmp8 & 0x3F) >> 4);
		}

        public void Write(FileWriter writer) {
			writer.Write((byte)TexCoord);
			writer.Write(Color);
			ushort tmp16 = 0;
			tmp16 |= (ushort)(((ushort)TexSel & 0x3F) << 11);
			tmp16 |= (ushort)(((ushort)RasSel & 0x7) << 9);
			tmp16 |= (ushort)(((ushort)TexMap & 0x1ff) << 0);
			writer.Write(tmp16);
			byte tmp8 = 0;
			tmp8 |= (byte)(((byte)ColorB & 0xf) << 4);
			tmp8 |= (byte)(((byte)ColorA & 0xf) << 0);
			writer.Write(tmp8);
			tmp8 = 0;
			tmp8 |= (byte)(((byte)ColorD & 0xf) << 4);
			tmp8 |= (byte)(((byte)ColorC & 0xf) << 0);
			writer.Write(tmp8);
			tmp8 = 0;
			tmp8 |= (byte)(((byte)ColorScale & 0x3) << 6);
			tmp8 |= (byte)(((byte)ColorBias & 0x3) << 4);
			tmp8 |= (byte)(((byte)ColorOp & 0xf) << 0);
			writer.Write(tmp8);
			tmp8 = 0;
			tmp8 |= (byte)(((byte)ColorConstantSel & 0x1F) << 3);
			tmp8 |= (byte)(((byte)ColorRegID & 0x7) << 1);
			tmp8 |= (byte)((ColorClamp ? 1 : 0) << 0);
			writer.Write(tmp8);

			tmp8 = 0;
			tmp8 |= (byte)(((byte)AlphaB & 0xf) << 4);
			tmp8 |= (byte)(((byte)AlphaA & 0xf) << 0);
			writer.Write(tmp8);
			tmp8 = 0;
			tmp8 |= (byte)(((byte)AlphaD & 0xf) << 4);
			tmp8 |= (byte)(((byte)AlphaC & 0xf) << 0);
			writer.Write(tmp8);
			tmp8 = 0;
			tmp8 |= (byte)(((byte)AlphaScale & 0x3) << 6);
			tmp8 |= (byte)(((byte)AlphaBias & 0x3) << 4);
			tmp8 |= (byte)(((byte)AlphaOp & 0xf) << 0);
			writer.Write(tmp8);
			tmp8 = 0;
			tmp8 |= (byte)(((byte)AlphaConstantSel & 0x1F) << 3);
			tmp8 |= (byte)(((byte)AlphaRegID & 0x7) << 1);
			tmp8 |= (byte)((AlphaClamp ? 1 : 0) << 0);
			writer.Write(tmp8);
			writer.Write((byte)(TexID & 0x3));
			tmp8 = 0;
			tmp8 |= (byte)(((byte)Matrix & 0x1F) << 3);
			tmp8 |= (byte)(((byte)IndBias & 0x7) << 0);
			writer.Write(tmp8);
			tmp8 = 0;
			tmp8 |= (byte)(((byte)WrapT & 0x7) << 3);
			tmp8 |= (byte)(((byte)WrapS & 0x7) << 0);
			writer.Write(tmp8);
			tmp8 = 0;
			tmp8 |= (byte)(((byte)Alpha & 0xF) << 4);
			tmp8 |= (byte)((UnmodifiedLOD & 0x1) << 3);
			tmp8 |= (byte)((UsePreviousStage & 0x1) << 2);
			tmp8 |= (byte)(((byte)Format & 0x3) << 0);
			writer.Write(tmp8);
		}
	}
}
