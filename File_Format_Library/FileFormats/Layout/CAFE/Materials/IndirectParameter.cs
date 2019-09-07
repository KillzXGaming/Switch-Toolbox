using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class IndirectParameter
    {
        public float Rotation { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }

        public IndirectParameter(FileReader reader, BFLYT.Header header)
        {
            Rotation = reader.ReadSingle();
            ScaleX = reader.ReadSingle();
            ScaleY = reader.ReadSingle();
        }

        public void Write(FileWriter writer)
        {
            writer.Write(Rotation);
            writer.Write(ScaleX);
            writer.Write(ScaleY);
        }
    }
}
