using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class AlphaCompare
    {
        public GfxAlphaFunction CompareMode { get; set; }
        public float Value { get; set; }

        public AlphaCompare(FileReader reader, BFLYT.Header header)
        {
            CompareMode = reader.ReadEnum<GfxAlphaFunction>(false);
            reader.ReadBytes(0x3);
            Value = reader.ReadSingle();
        }

        public void Write(FileWriter writer)
        {
            writer.Write(CompareMode, false);
            writer.Seek(3);
            writer.Write(Value);
        }
    }
}
