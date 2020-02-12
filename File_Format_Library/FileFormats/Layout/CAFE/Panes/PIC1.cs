using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using System.ComponentModel;
using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class PIC1 : PAN1, IPicturePane
    {
        public override string Signature { get; } = "pic1";

        [DisplayName("Texture Coordinates"), CategoryAttribute("Texture")]
        public TexCoord[] TexCoords { get; set; }

        [DisplayName("Vertex Color (Top Left)"), CategoryAttribute("Color")]
        public STColor8 ColorTopLeft { get; set; }
        [DisplayName("Vertex Color (Top Right)"), CategoryAttribute("Color")]
        public STColor8 ColorTopRight { get; set; }
        [DisplayName("Vertex Color (Bottom Left)"), CategoryAttribute("Color")]
        public STColor8 ColorBottomLeft { get; set; }
        [DisplayName("Vertex Color (Bottom Right)"), CategoryAttribute("Color")]
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

        [Browsable(false)]
        public ushort MaterialIndex { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytMaterial Material { get; set; }

        [Browsable(false)]
        public string GetTexture(int index)
        {
            return ParentLayout.Textures[Material.TextureMaps[index].ID];
        }

        private Header ParentLayout;

        public PIC1() : base()
        {
            LoadDefaults();
        }

        public PIC1(Header header, string name) : base()
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
            Material = Material.Clone();
        }

        public PIC1(FileReader reader, Header header) : base(reader, header)
        {
            ParentLayout = header;

            ColorTopLeft = reader.ReadColor8RGBA();
            ColorTopRight = reader.ReadColor8RGBA();
            ColorBottomLeft = reader.ReadColor8RGBA();
            ColorBottomRight = reader.ReadColor8RGBA();
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
            writer.Write(ColorTopLeft);
            writer.Write(ColorTopRight);
            writer.Write(ColorBottomLeft);
            writer.Write(ColorBottomRight);
            writer.Write(MaterialIndex);
            writer.Write((byte)TexCoords.Length);
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
