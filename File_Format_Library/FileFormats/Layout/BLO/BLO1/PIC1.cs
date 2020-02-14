using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library;
using Syroot.BinaryData;

namespace LayoutBXLYT.GCBLO
{
    public class PIC1 : PAN1, IPicturePane
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

        public void CopyMaterial() {
            Material = (BxlytMaterial)Material.Clone();
        }

        public ushort MaterialIndex { get; set; }

        public PIC1(FileReader reader, BLOHeader header) : base(reader, header)
        {
            byte numParams = reader.ReadByte();
            if (numParams > 0) {
                TextureName = BloResource.Read(reader, header);
                numParams--;
            }
            if (numParams > 0) {
                PaletteName = BloResource.Read(reader, header);
                numParams--;
            }
            if (numParams > 0) {
                Binding = reader.ReadByte();
                numParams--;
            }

            Material = new Material();

            if (TextureName == string.Empty)
                Material.TextureMaps = new BxlytTextureRef[0];
            else
            {
                Material.TextureMaps = new BxlytTextureRef[1];
                Material.TextureMaps[0] = new BxlytTextureRef()
                {
                    Name = TextureName,
                };
            }


            TexCoords = new TexCoord[1];
            TexCoords[0] = new TexCoord();
        }

        public void Write(FileWriter writer)
        {

        }
    }
}
