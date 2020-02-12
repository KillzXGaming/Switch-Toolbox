using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;
using System.ComponentModel;

namespace LayoutBXLYT.CTR
{
    public class PIC1 : PAN1, IPicturePane
    {
        public override string Signature { get; } = "pic1";

        public TexCoord[] TexCoords { get; set; }

        public STColor8 ColorTopLeft { get; set; }
        public STColor8 ColorTopRight { get; set; }
        public STColor8 ColorBottomLeft { get; set; }
        public STColor8 ColorBottomRight { get; set; }

        public System.Drawing.Color[] GetVertexColors()
        {
            return new System.Drawing.Color[4]
            {
                    ColorTopLeft.Color,
                    ColorTopRight.Color,
                    ColorBottomLeft.Color,
                    ColorBottomRight.Color,
            };
        }

        public ushort MaterialIndex { get; set; }

        public string GetTexture(int index)
        {
            return ParentLayout.Textures[Material.TextureMaps[index].ID];
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytMaterial Material { get; set; }

        private BxlytHeader ParentLayout;

        public PIC1() : base()
        {
            LoadDefaults();
        }

        public PIC1(BxlytHeader header, string name) : base()
        {
            LoadDefaults();
            Name = name;

            ParentLayout = header;

            ColorTopLeft = STColor8.White;
            ColorTopRight = STColor8.White;
            ColorBottomLeft = STColor8.White;
            ColorBottomRight = STColor8.White;
            TexCoords = new TexCoord[1];
            TexCoords[0] = new TexCoord();

            Material = new Material(name, header);
        }

        public void CopyMaterial()
        {
            Material = (BxlytMaterial)Material.Clone();
        }

        public PIC1(FileReader reader, BCLYT.Header header) : base(reader, header)
        {
            ParentLayout = header;

            ColorTopLeft = STColor8.FromBytes(reader.ReadBytes(4));
            ColorTopRight = STColor8.FromBytes(reader.ReadBytes(4));
            ColorBottomLeft = STColor8.FromBytes(reader.ReadBytes(4));
            ColorBottomRight = STColor8.FromBytes(reader.ReadBytes(4));
            MaterialIndex = reader.ReadUInt16();
            byte numUVs = reader.ReadByte();
            reader.Seek(1); //padding

            TexCoords = new TexCoord[numUVs];
            for (int i = 0; i < numUVs; i++)
            {
                TexCoords[i] = new TexCoord()
                {
                    TopLeft = reader.ReadVec2SY(),
                    TopRight = reader.ReadVec2SY(),
                    BottomLeft = reader.ReadVec2SY(),
                    BottomRight = reader.ReadVec2SY(),
                };
            }

            Material = header.MaterialList.Materials[MaterialIndex];
        }

        public override void Write(FileWriter writer, LayoutHeader header)
        {
            base.Write(writer, header);
            writer.Write(ColorTopLeft.ToBytes());
            writer.Write(ColorTopRight.ToBytes());
            writer.Write(ColorBottomLeft.ToBytes());
            writer.Write(ColorBottomRight.ToBytes());
            writer.Write(MaterialIndex);
            writer.Write(TexCoords != null ? (byte)TexCoords.Length : (byte)0);
            writer.Write((byte)0);
            for (int i = 0; i < TexCoords.Length; i++)
            {
                writer.Write(TexCoords[i].TopLeft);
                writer.Write(TexCoords[i].TopRight);
                writer.Write(TexCoords[i].BottomLeft);
                writer.Write(TexCoords[i].BottomRight);
            }
        }
    }
}
