using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace LayoutBXLYT.CTR
{
    public class TextureRef : BxlytTextureRef
    {
        public short ID;
        byte flag1 = 0x06;
        byte flag2 = 0x06;

        public override WrapMode WrapModeU
        {
            get { return (WrapMode)(flag1 & 0x3); }
        }

        public override WrapMode WrapModeV
        {
            get { return (WrapMode)(flag2 & 0x3); }
        }

        public override FilterMode MinFilterMode
        {
            get { return (FilterMode)((flag1 >> 2) & 0x3); }
        }

        public override FilterMode MaxFilterMode
        {
            get { return (FilterMode)((flag2 >> 2) & 0x3); }
        }

        public TextureRef() { }

        public TextureRef(FileReader reader, BxlytHeader header)
        {
            ID = reader.ReadInt16();
            flag1 = reader.ReadByte();
            flag2 = reader.ReadByte();

            if (header.Textures.Count > 0)
                Name = header.Textures[ID];
        }

        public void Write(FileWriter writer)
        {
            writer.Write(ID);
            writer.Write(flag1);
            writer.Write(flag2);
        }
    }
}
