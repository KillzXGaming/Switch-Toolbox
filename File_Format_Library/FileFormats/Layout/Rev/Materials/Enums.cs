using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutBXLYT.Revolution
{

	//Thanks brawlbox. Layouts should work with these
	//https://github.com/libertyernie/brawltools/blob/40d7431b1a01ef4a0411cd69e51411bd581e93e2/BrawlLib/Wii/Graphics/Enum.cs
	public enum ColorArg : byte
	{
		OutputColor,//GX_CC_CPREV,
		OutputAlpha,//GX_CC_APREV,
		Color0,//GX_CC_C0,
		Alpha0,//GX_CC_A0,
		Color1,//GX_CC_C1,
		Alpha1,//GX_CC_A1,
		Color2,//GX_CC_C2,
		Alpha2,//GX_CC_A2,
		TextureColor,//GX_CC_TEXC,
		TextureAlpha,//GX_CC_TEXA,
		RasterColor,//GX_CC_RASC,
		RasterAlpha,//GX_CC_RASA,
		One,//GX_CC_ONE, //1
		Half,//GX_CC_HALF, //0.5
		ConstantColorSelection,//GX_CC_KONST,
		Zero//GX_CC_ZERO //0
	}

	public enum Bias
	{
		Zero,//GX_TB_ZERO,
		AddHalf,//GX_TB_ADDHALF,
		SubHalf//GX_TB_SUBHALF
	}

	public enum TevColorRegID
	{
		OutputColor,
		Color0,
		Color1,
		Color2,
	}

	public enum TevColorOp
	{
		Add = 0,
		Subtract = 1,

		CompR8Greater = 8,
		CompR8Equal = 9,
		CompGR16Greater = 10,
		CompGR16Equal = 11,
		CompBGR24Greater = 12,
		CompBGR24Equal = 13,
		CompRGB8Greater = 14,
		CompRGB8Equal = 15,

		//GX_TEV_COMP_A8_GT = GX_TEV_COMP_RGB8_GT, // for alpha channel
		//GX_TEV_COMP_A8_EQ = GX_TEV_COMP_RGB8_EQ  // for alpha channel
	}

	public enum TevAlphaRegID
	{
		OutputAlpha,
		Alpha0,
		Alpha1,
		Alpha2,
	}

	public enum AlphaArg
	{
		OutputAlpha,//GX_CA_APREV,
		Alpha0,//GX_CA_A0,
		Alpha1,//GX_CA_A1,
		Alpha2,//GX_CA_A2,
		TextureAlpha,//GX_CA_TEXA,
		RasterAlpha,//GX_CA_RASA,
		ConstantAlphaSelection,//GX_CA_KONST,
		Zero//GX_CA_ZERO //0
	}

	public enum TevAlphaOp
	{
		Add = 0,
		Subtract = 1,

		CompR8Greater = 8,
		CompR8Equal = 9,
		CompGR16Greater = 10,
		CompGR16Equal = 11,
		CompBGR24Greater = 12,
		CompBGR24Equal = 13,
		CompA8Greater = 14,
		CompA8Equal = 15,
	}

	public enum TevScale
	{
		MultiplyBy1,//GX_CS_SCALE_1,
		MultiplyBy2,//GX_CS_SCALE_2,
		MultiplyBy4,//GX_CS_SCALE_4,
		DivideBy2//GX_CS_DIVIDE_2
	}

	public enum TevKAlphaSel
	{
		Constant1_1/*GX_TEV_KASEL_8_8*/ = 0x00, //1.0f
		Constant7_8/*GX_TEV_KASEL_7_8*/ = 0x01, //0.875f
		Constant3_4/*GX_TEV_KASEL_6_8*/ = 0x02, //0.75f
		Constant5_8/*GX_TEV_KASEL_5_8*/ = 0x03, //0.625f
		Constant1_2/*GX_TEV_KASEL_4_8*/ = 0x04, //0.5f
		Constant3_8/*GX_TEV_KASEL_3_8*/ = 0x05, //0.375f
		Constant1_4/*GX_TEV_KASEL_2_8*/ = 0x06, //0.25f
		Constant1_8/*GX_TEV_KASEL_1_8*/ = 0x07, //0.125f

		//GX_TEV_KASEL_1    = GX_TEV_KASEL_8_8,
		//GX_TEV_KASEL_3_4  = GX_TEV_KASEL_6_8,
		//GX_TEV_KASEL_1_2  = GX_TEV_KASEL_4_8,
		//GX_TEV_KASEL_1_4  = GX_TEV_KASEL_2_8,

		ConstantColor0_Red/*GX_TEV_KASEL_K0_R*/ = 0x10,
		ConstantColor1_Red/*GX_TEV_KASEL_K1_R*/ = 0x11,
		ConstantColor2_Red/*GX_TEV_KASEL_K2_R*/ = 0x12,
		ConstantColor3_Red/*GX_TEV_KASEL_K3_R*/ = 0x13,
		ConstantColor0_Green/*GX_TEV_KASEL_K0_G*/ = 0x14,
		ConstantColor1_Green/*GX_TEV_KASEL_K1_G*/ = 0x15,
		ConstantColor2_Green/*GX_TEV_KASEL_K2_G*/ = 0x16,
		ConstantColor3_Green/*GX_TEV_KASEL_K3_G*/ = 0x17,
		ConstantColor0_Blue/*GX_TEV_KASEL_K0_B*/ = 0x18,
		ConstantColor1_Blue/*GX_TEV_KASEL_K1_B*/ = 0x19,
		ConstantColor2_Blue/*GX_TEV_KASEL_K2_B*/ = 0x1A,
		ConstantColor3_Blue/*GX_TEV_KASEL_K3_B*/ = 0x1B,
		ConstantColor0_Alpha/*GX_TEV_KASEL_K0_A*/ = 0x1C,
		ConstantColor1_Alpha/*GX_TEV_KASEL_K1_A*/ = 0x1D,
		ConstantColor2_Alpha/*GX_TEV_KASEL_K2_A*/ = 0x1E,
		ConstantColor3_Alpha/*GX_TEV_KASEL_K3_A*/ = 0x1F
	}

	public enum TevKColorSel
	{
		Constant1_1/*GX_TEV_KCSEL_8_8*/ = 0x00, //1.0f, 1.0f, 1.0f
		Constant7_8/*GX_TEV_KCSEL_7_8*/ = 0x01, //0.875f, 0.875f, 0.875f
		Constant3_4/*GX_TEV_KCSEL_6_8*/ = 0x02, //0.75f, 0.75f, 0.75f
		Constant5_8/*GX_TEV_KCSEL_5_8*/ = 0x03, //0.625f, 0.625f, 0.625f
		Constant1_2/*GX_TEV_KCSEL_4_8*/ = 0x04, //0.5f, 0.5f, 0.5f
		Constant3_8/*GX_TEV_KCSEL_3_8*/ = 0x05, //0.375f, 0.375f, 0.375f
		Constant1_4/*GX_TEV_KCSEL_2_8*/ = 0x06, //0.25f, 0.25f, 0.25f
		Constant1_8/*GX_TEV_KCSEL_1_8*/ = 0x07, //0.125f, 0.125f, 0.125f

		//GX_TEV_KCSEL_1    = GX_TEV_KCSEL_8_8,
		//GX_TEV_KCSEL_3_4  = GX_TEV_KCSEL_6_8,
		//GX_TEV_KCSEL_1_2  = GX_TEV_KCSEL_4_8,
		//GX_TEV_KCSEL_1_4  = GX_TEV_KCSEL_2_8,

		ConstantColor0_RGB/*GX_TEV_KCSEL_K0*/   = 0x0C,
		ConstantColor1_RGB/*GX_TEV_KCSEL_K1*/   = 0x0D,
		ConstantColor2_RGB/*GX_TEV_KCSEL_K2*/   = 0x0E,
		ConstantColor3_RGB/*GX_TEV_KCSEL_K3*/   = 0x0F,
		ConstantColor0_RRR/*GX_TEV_KCSEL_K0_R*/ = 0x10,
		ConstantColor1_RRR/*GX_TEV_KCSEL_K1_R*/ = 0x11,
		ConstantColor2_RRR/*GX_TEV_KCSEL_K2_R*/ = 0x12,
		ConstantColor3_RRR/*GX_TEV_KCSEL_K3_R*/ = 0x13,
		ConstantColor0_GGG/*GX_TEV_KCSEL_K0_G*/ = 0x14,
		ConstantColor1_GGG/*GX_TEV_KCSEL_K1_G*/ = 0x15,
		ConstantColor2_GGG/*GX_TEV_KCSEL_K2_G*/ = 0x16,
		ConstantColor3_GGG/*GX_TEV_KCSEL_K3_G*/ = 0x17,
		ConstantColor0_BBB/*GX_TEV_KCSEL_K0_B*/ = 0x18,
		ConstantColor1_BBB/*GX_TEV_KCSEL_K1_B*/ = 0x19,
		ConstantColor2_BBB/*GX_TEV_KCSEL_K2_B*/ = 0x1A,
		ConstantColor3_BBB/*GX_TEV_KCSEL_K3_B*/ = 0x1B,
		ConstantColor0_AAA/*GX_TEV_KCSEL_K0_A*/ = 0x1C,
		ConstantColor1_AAA/*GX_TEV_KCSEL_K1_A*/ = 0x1D,
		ConstantColor2_AAA/*GX_TEV_KCSEL_K2_A*/ = 0x1E,
		ConstantColor3_AAA/*GX_TEV_KCSEL_K3_A*/ = 0x1F
	}

	public enum TevSwapSel : ushort
	{
		Swap0,//GX_TEV_SWAP0 = 0,
		Swap1,//GX_TEV_SWAP1,
		Swap2,//GX_TEV_SWAP2,
		Swap3,//GX_TEV_SWAP3
	}

	public enum TexMapID
	{
		TexMap0,//GX_TEXMAP0,
		TexMap1,//GX_TEXMAP1,
		TexMap2,//GX_TEXMAP2,
		TexMap3,//GX_TEXMAP3,
		TexMap4,//GX_TEXMAP4,
		TexMap5,//GX_TEXMAP5,
		TexMap6,//GX_TEXMAP6,
		TexMap7,//GX_TEXMAP7,
				//GX_MAX_TEXMAP,

		//GX_TEXMAP_NULL = 0xff,
		//GX_TEX_DISABLE = 0x100	// mask : disables texture look up
	}

	public enum TexCoordID
	{
		TexCoord0,//GX_TEXCOORD0 = 0x0, // generated texture coordinate 0
		TexCoord1,//GX_TEXCOORD1, 		// generated texture coordinate 1
		TexCoord2,//GX_TEXCOORD2, 		// generated texture coordinate 2
		TexCoord3,//GX_TEXCOORD3, 		// generated texture coordinate 3
		TexCoord4,//GX_TEXCOORD4, 		// generated texture coordinate 4
		TexCoord5,//GX_TEXCOORD5, 		// generated texture coordinate 5
		TexCoord6,//GX_TEXCOORD6, 		// generated texture coordinate 6
		TexCoord7,//GX_TEXCOORD7, 		// generated texture coordinate 7
				  //GX_MAX_TEXCOORD = 8,
				  //GX_TEXCOORD_NULL = 0xff
	}

	public enum IndTexMtxID
	{
		NoMatrix,//GX_ITM_OFF,
		Matrix0,//GX_ITM_0,
		Matrix1,//GX_ITM_1,
		Matrix2,//GX_ITM_2,
		MatrixS0 = 5,//GX_ITM_S0 = 5,
		MatrixS1,//GX_ITM_S1,
		MatrixS2,//GX_ITM_S2,
		MatrixT0 = 9, //GX_ITM_T0 = 9,
		MatrixT1,//GX_ITM_T1,
		MatrixT2,//GX_ITM_T2
	}

	public enum IndTexWrap
	{
		NoWrap,//GX_ITW_OFF,		// no wrapping
		Wrap256,//GX_ITW_256,		// wrap 256
		Wrap128,//GX_ITW_128,		// wrap 128
		Wrap64,//GX_ITW_64, 		// wrap 64
		Wrap32,//GX_ITW_32, 		// wrap 32
		Wrap16,//GX_ITW_16, 		// wrap 16
		Wrap0,//GX_ITW_0, 		    // wrap 0
	}

	public enum IndTexScale
	{
		DivideBy1,//GX_ITS_1,		// Scale by 1.
		DivideBy2,//GX_ITS_2,		// Scale by 1/2.
		DivideBy4,//GX_ITS_4,		// Scale by 1/4.
		DivideBy8,//GX_ITS_8,		// Scale by 1/8.
		DivideBy16,//GX_ITS_16,		// Scale by 1/16.
		DivideBy32,//GX_ITS_32,		// Scale by 1/32.
		DivideBy64,//GX_ITS_64,		// Scale by 1/64.
		DivideBy128,//GX_ITS_128,   // Scale by 1/128.
		DivideBy256,//GX_ITS_256	// Scale by 1/256.
	}

	public enum IndTexFormat
	{
		F_8_Bit_Offsets,//GX_ITF_8,		// 8 bit texture offsets.
		F_5_Bit_Offsets,//GX_ITF_5,		// 5 bit texture offsets.
		F_4_Bit_Offsets,//GX_ITF_4,		// 4 bit texture offsets.
		F_3_Bit_Offsets,//GX_ITF_3		// 3 bit texture offsets.
	}

	public enum IndTexStageID
	{
		IndirectTexStg0,//GX_INDTEXSTAGE0,
		IndirectTexStg1,//GX_INDTEXSTAGE1,
		IndirectTexStg2,//GX_INDTEXSTAGE2,
		IndirectTexStg3//GX_INDTEXSTAGE3
	}

	public enum IndTexAlphaSel
	{
		Off,//GX_ITBA_OFF,
		S,//GX_ITBA_S,
		T,//GX_ITBA_T,
		U//GX_ITBA_U
	}



	public enum TexCoordGenTypes
	{
		GX_TG_MTX3x4 = 0,
		GX_TG_MTX2x4 = 1,
		GX_TG_BUMP0 = 2,
		GX_TG_BUMP1 = 3,
		GX_TG_BUMP2 = 4,
		GX_TG_BUMP3 = 5,
		GX_TG_BUMP4 = 6,
		GX_TG_BUMP5 = 7,
		GX_TG_BUMP6 = 8,
		GX_TG_BUMP7 = 9,
		GX_TG_SRTG = 0xA
	}
	public enum TexCoordGenSource
	{
		GX_TG_POS,
		GX_TG_NRM,
		GX_TG_BINRM,
		GX_TG_TANGENT,
		GX_TG_TEX0,
		GX_TG_TEX1,
		GX_TG_TEX2,
		GX_TG_TEX3,
		GX_TG_TEX4,
		GX_TG_TEX5,
		GX_TG_TEX6,
		GX_TG_TEX7,
		GX_TG_TEXCOORD0,
		GX_TG_TEXCOORD1,
		GX_TG_TEXCOORD2,
		GX_TG_TEXCOORD3,
		GX_TG_TEXCOORD4,
		GX_TG_TEXCOORD5,
		GX_TG_TEXCOORD6,
		GX_TG_COLOR0,
		GX_TG_COLOR1
	}
	public enum TexCoordGenMatrixSource
	{
		GX_PNMTX0,
		GX_PNMTX1,
		GX_PNMTX2,
		GX_PNMTX3,
		GX_PNMTX4,
		GX_PNMTX5,
		GX_PNMTX6,
		GX_PNMTX7,
		GX_PNMTX8,
		GX_PNMTX9,
		GX_TEXMTX0,
		GX_TEXMTX1,
		GX_TEXMTX2,
		GX_TEXMTX3,
		GX_TEXMTX4,
		GX_TEXMTX5,
		GX_TEXMTX6,
		GX_TEXMTX7,
		GX_TEXMTX8,
		GX_TEXMTX9,
		GX_IDENTITY,
		GX_DTTMTX0,
		GX_DTTMTX1,
		GX_DTTMTX2,
		GX_DTTMTX3,
		GX_DTTMTX4,
		GX_DTTMTX5,
		GX_DTTMTX6,
		GX_DTTMTX7,
		GX_DTTMTX8,
		GX_DTTMTX9,
		GX_DTTMTX10,
		GX_DTTMTX11,
		GX_DTTMTX12,
		GX_DTTMTX13,
		GX_DTTMTX14,
		GX_DTTMTX15,
		GX_DTTMTX16,
		GX_DTTMTX17,
		GX_DTTMTX18,
		GX_DTTMTX19,
		GX_DTTIDENTITY
	}

	public enum SwapChannel
	{
		Red,
		Green,
		Blue,
		Alpha
	}
}
