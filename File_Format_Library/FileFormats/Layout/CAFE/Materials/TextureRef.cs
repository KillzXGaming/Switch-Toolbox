using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class TextureRef : BxlytTextureRef
    {
        byte flag1;
        byte flag2;

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

        public TextureRef(FileReader reader, BFLYT.Header header)
        {
            ID = reader.ReadInt16();
            flag1 = reader.ReadByte();
            flag2 = reader.ReadByte();

            if (header.Textures.Count > 0 && ID != -1)
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
