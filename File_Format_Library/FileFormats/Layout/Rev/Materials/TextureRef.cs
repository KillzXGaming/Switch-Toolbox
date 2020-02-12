using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace LayoutBXLYT.Revolution
{
    public class TextureRef : BxlytTextureRef
    {
        public TextureRef() { }

        public TextureRef(FileReader reader, BxlytHeader header)
        {
            ID = reader.ReadInt16();
            WrapModeU = (WrapMode)reader.ReadByte();
            WrapModeV = (WrapMode)reader.ReadByte();
            MinFilterMode = FilterMode.Linear;
            MaxFilterMode = FilterMode.Linear;

            if (header.Textures.Count > 0)
                Name = header.Textures[ID];
        }

        public void Write(FileWriter writer)
        {
            writer.Write(ID);
            writer.Write((byte)WrapModeU);
            writer.Write((byte)WrapModeV);
        }
    }
}
