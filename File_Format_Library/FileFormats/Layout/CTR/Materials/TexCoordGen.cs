using Toolbox.Library.IO;

namespace LayoutBXLYT.CTR
{
    public class TexCoordGen : BxlytTexCoordGen
    {
        ushort Unknown { get; set; }

        public TexCoordGen(FileReader reader, BxlytHeader header)
        {
            Matrix = (TexGenMatrixType)reader.ReadByte();
            Source = (TexGenType)reader.ReadByte();
            Unknown = reader.ReadUInt16();
        }

        public void Write(FileWriter writer)
        {
            writer.Write((byte)Matrix);
            writer.Write((byte)Source);
            writer.Write(Unknown);
        }
    }
}
