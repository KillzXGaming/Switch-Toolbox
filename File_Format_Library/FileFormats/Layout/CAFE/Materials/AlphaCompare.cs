using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class AlphaCompare
    {
        public byte CompareMode { get; set; }
        public uint Value { get; set; }

        public AlphaCompare(FileReader reader, BFLYT.Header header)
        {
            CompareMode = reader.ReadByte();
            reader.ReadBytes(0x3);
            Value = reader.ReadUInt32();
        }

        public void Write(FileWriter writer)
        {
            writer.Write(CompareMode);
            writer.Seek(3);
            writer.Write(Value);
        }
    }
}
