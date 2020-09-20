using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class TexCoordGen : BxlytTexCoordGen
    {
        public byte[] unkData;

        public TexCoordGen()
        {
            Matrix = TexGenMatrixType.Matrix2x4;
            Source = TexGenType.TextureCoord0;
            unkData = new byte[6];
        }

        public TexCoordGen(FileReader reader, BxlytHeader header)
        {
            Matrix = reader.ReadEnum<TexGenMatrixType>(false);
            Source = reader.ReadEnum<TexGenType>(false);
            if (header.VersionMajor >= 8)
                unkData = reader.ReadBytes(0xE);
            else
                unkData = reader.ReadBytes(6);
        }

        public void Write(FileWriter writer, BxlytHeader header)
        {
            writer.Write(Matrix, false);
            writer.Write(Source, false);

            int length = header.VersionMajor >= 8 ? 0xE : 6;
            for (int i = 0; i < length; i++)
                if (i < unkData.Length)
                    writer.Write(unkData[i]);
                else
                    writer.Write((byte)0);
        }
    }
}
