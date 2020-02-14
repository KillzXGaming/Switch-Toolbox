using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library;
using Syroot.Maths;

namespace LayoutBXLYT.GCBLO
{
    public class PIC2 : PAN2, IPicturePane
    {
        public TexCoord[] TexCoords { get; set; }
        public STColor8 ColorTopLeft { get; set; } = STColor8.White;
        public STColor8 ColorTopRight { get; set; } = STColor8.White;
        public STColor8 ColorBottomLeft { get; set; } = STColor8.White;
        public STColor8 ColorBottomRight { get; set; } = STColor8.White;

        public string TextureName { get; set; } = "";
        public string PaletteName { get; set; } = "";
        public byte Binding { get; set; }

        public BxlytMaterial Material { get; set; }

        public void CopyMaterial()
        {
            Material = (BxlytMaterial)Material.Clone();
        }

        public ushort MaterialIndex { get; set; }

        public PIC2(FileReader reader, BLOHeader header) : base(reader, header) {
            TexCoords = new TexCoord[1];
            TexCoords[0] = new TexCoord();

            ushort sectionSize = reader.ReadUInt16();
            Console.WriteLine($"PIC2 {sectionSize}");
            MaterialIndex = reader.ReadUInt16();
            Material = header.Materials[MaterialIndex];
            uint unk = reader.ReadUInt32();
            //These increase for each material
            ushort index1 = reader.ReadUInt16();
            ushort index2 = reader.ReadUInt16();
            ushort index3 = reader.ReadUInt16();
            ushort index4 = reader.ReadUInt16();

            //I think these are texture coordinates
            TexCoords[0].TopLeft = new Vector2F(
                reader.ReadInt16() / 256f,
                reader.ReadInt16() / 256f);
            TexCoords[0].TopRight = new Vector2F(
                reader.ReadInt16() / 256f,
                reader.ReadInt16() / 256f);
            TexCoords[0].BottomLeft = new Vector2F(
                reader.ReadInt16() / 256f,
                reader.ReadInt16() / 256f);
            TexCoords[0].BottomRight = new Vector2F(
                reader.ReadInt16() / 256f,
                reader.ReadInt16() / 256f);

            ColorTopLeft = STColor8.FromBytes(reader.ReadBytes(4));
            ColorTopRight = STColor8.FromBytes(reader.ReadBytes(4));
            ColorBottomLeft = STColor8.FromBytes(reader.ReadBytes(4));
            ColorBottomRight = STColor8.FromBytes(reader.ReadBytes(4));

        }

        public override void Write(FileWriter writer)
        {

        }
    }
}
