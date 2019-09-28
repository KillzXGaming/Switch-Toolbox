using Toolbox.Library.IO;
using Syroot.Maths;

namespace LayoutBXLYT.Cafe
{
    public class TextureTransform : BxlytTextureTransform
    {
        public TextureTransform() { }

        public TextureTransform(FileReader reader)
        {
            Translate = reader.ReadVec2SY();
            Rotate = reader.ReadSingle();
            Scale = reader.ReadVec2SY();
        }

        public void Write(FileWriter writer)
        {
            writer.Write(Translate);
            writer.Write(Rotate);
            writer.Write(Scale);
        }
    }
}
