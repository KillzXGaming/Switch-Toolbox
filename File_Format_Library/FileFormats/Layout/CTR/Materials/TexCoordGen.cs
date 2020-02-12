using Toolbox.Library.IO;

namespace LayoutBXLYT.CTR
{
    public class TexCoordGen
    {
        public MatrixType GenType { get; set; }
        public TextureGenerationType Source { get; set; }

        ushort Unknown { get; set; }

        public TexCoordGen(FileReader reader, BxlytHeader header)
        {
            GenType = (MatrixType)reader.ReadByte();
            Source = (TextureGenerationType)reader.ReadByte();
            Unknown = reader.ReadUInt16();
        }

        public void Write(FileWriter writer)
        {
            writer.Write((byte)GenType);
            writer.Write((byte)Source);
            writer.Write(Unknown);
        }

        public enum MatrixType : byte
        {
            Matrix2x4 = 0
        }

        public enum TextureGenerationType : byte
        {
            Tex0 = 0,
            Tex1 = 1,
            Tex2 = 2,
            Ortho = 3,
            PaneBased = 4,
            PerspectiveProj = 5
        }
    }
}
