using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class TevStage
    {
        public byte RGBMode { get; set; }
        public byte AlphaMode { get; set; }
        public ushort unk { get; set; }

        public TevStage(FileReader reader, BFLYT.Header header)
        {
            RGBMode = reader.ReadByte();
            AlphaMode = reader.ReadByte();
            unk = reader.ReadUInt16();
        }

        public void Write(FileWriter writer)
        {
            writer.Write(RGBMode);
            writer.Write(AlphaMode);
            writer.Write(unk);
        }
    }
}
