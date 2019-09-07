using Toolbox.Library.IO;
using Toolbox.Library;

namespace LayoutBXLYT.Cafe
{
    public class FontShadowParameter
    {
        public STColor8 BlackColor { get; set; }
        public STColor8 WhiteColor { get; set; }

        public FontShadowParameter(FileReader reader, BFLYT.Header header)
        {
            BlackColor = reader.ReadColor8RGBA();
            WhiteColor = reader.ReadColor8RGBA();
        }

        public void Write(FileWriter writer)
        {
            writer.Write(BlackColor);
            writer.Write(WhiteColor);
        }
    }
}
