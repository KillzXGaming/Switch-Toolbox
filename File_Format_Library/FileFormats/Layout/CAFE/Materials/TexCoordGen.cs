using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class TexCoordGen
    {
        public MatrixType GenType;
        public TextureGenerationType Source;

        byte[] unkData;

        public TexCoordGen(FileReader reader, BFLYT.Header header)
        {
            GenType = reader.ReadEnum<MatrixType>(false);
            Source = reader.ReadEnum<TextureGenerationType>(false);
            if (header.VersionMajor >= 8)
                unkData = reader.ReadBytes(0xE);
            else
                unkData = reader.ReadBytes(6);
        }

        public void Write(FileWriter writer)
        {
            writer.Write(GenType, false);
            writer.Write(Source, false);
            writer.Write(unkData);
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
