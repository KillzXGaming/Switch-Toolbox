using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class TexCoordGen : BxlytTexCoordGen
    {
        byte[] unkData;

        public TexCoordGen(FileReader reader, BxlytHeader header)
        {
            Matrix = reader.ReadEnum<TexGenMatrixType>(false);
            Source = reader.ReadEnum<TexGenType>(false);
            if (header.VersionMajor >= 8)
                unkData = reader.ReadBytes(0xE);
            else
                unkData = reader.ReadBytes(6);
        }

        public void Write(FileWriter writer)
        {
            writer.Write(Matrix, false);
            writer.Write(Source, false);
            writer.Write(unkData);
        }
    }
}
