using Toolbox.Library.IO;

namespace LayoutBXLYT.Revolution
{
    public class TexCoordGenEntry
    {
		public TexCoordGenTypes Type { get; set; }
		public TexCoordGenSource Source { get; set; }
		public TexCoordGenMatrixSource MatrixSource { get; set; }
		public byte Unknown { get; set; }

		public TexCoordGenEntry()
		{
			Type = TexCoordGenTypes.GX_TG_MTX3x4;
			Source = TexCoordGenSource.GX_TG_TEX0;
			MatrixSource = TexCoordGenMatrixSource.GX_TEXMTX0;
			Unknown = 0;
		}

		public TexCoordGenEntry(FileReader reader)
        {
			Type = (TexCoordGenTypes)reader.ReadByte();
			Source = (TexCoordGenSource)reader.ReadByte();
			MatrixSource = (TexCoordGenMatrixSource)reader.ReadByte();
			Unknown = reader.ReadByte();
		}

        public void Write(FileWriter writer)
        {
			writer.Write((byte)Type);
			writer.Write((byte)Source);
			writer.Write((byte)MatrixSource);
			writer.Write(Unknown);
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
	}
}
