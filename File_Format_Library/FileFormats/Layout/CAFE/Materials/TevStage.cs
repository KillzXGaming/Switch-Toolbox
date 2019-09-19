using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class TevStage
    {
        public TevMode RGBMode { get; set; }
        public TevMode AlphaMode { get; set; }
        public ushort unk { get; set; }

        public TevStage(FileReader reader, BFLYT.Header header)
        {
            RGBMode = (TevMode)reader.ReadByte();
            AlphaMode = (TevMode)reader.ReadByte();
            unk = reader.ReadUInt16();
        }

        public void Write(FileWriter writer)
        {
            writer.Write(RGBMode, false);
            writer.Write(AlphaMode, false);
            writer.Write(unk);
        }
    }
}
