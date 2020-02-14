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
    public class WIN1 : PAN1, IWindowPane
    {
        public BxlytHeader layoutHeader;

        public bool UseOneMaterialForAll
        {
            get { return Convert.ToBoolean(_flag & 1); }
            set { }
        }

        public bool UseVertexColorForAll
        {
            get { return Convert.ToBoolean((_flag >> 1) & 1); }
            set { }
        }

        public WindowKind WindowKind { get; set; }

        public bool NotDrawnContent
        {
            get { return (_flag & 8) == 16; }
            set { }
        }

        public ushort StretchLeft { get; set; }
        public ushort StretchRight { get; set; }
        public ushort StretchTop { get; set; }
        public ushort StretchBottm { get; set; }
        public ushort FrameElementLeft { get; set; }
        public ushort FrameElementRight { get; set; }
        public ushort FrameElementTop { get; set; }
        public ushort FrameElementBottm { get; set; }
        private byte _flag;

        private byte frameCount;
        public byte FrameCount
        {
            get { return frameCount; }
            set
            {
                frameCount = value;
            }
        }

        public System.Drawing.Color[] GetVertexColors()
        {
            return new System.Drawing.Color[4]
            {
                    Content.ColorTopLeft.Color,
                    Content.ColorTopRight.Color,
                    Content.ColorBottomLeft.Color,
                    Content.ColorBottomRight.Color,
            };
        }

        public void ReloadFrames()
        {
            SetFrames(layoutHeader);
        }

        public void SetFrames(BxlytHeader header)
        {

        }

        public void CopyWindows()
        {
            Content = (BxlytWindowContent)Content.Clone();
            for (int f = 0; f < WindowFrames.Count; f++)
                WindowFrames[f] = (BxlytWindowFrame)WindowFrames[f].Clone();
        }

        public BxlytWindowContent Content { get; set; }

        public List<BxlytWindowFrame> WindowFrames { get; set; }

        public string PaletteName { get; set; }

        public WIN1(FileReader reader, BLOHeader header) : base(reader, header)
        {
            byte numParams = reader.ReadByte();
            short transX = reader.ReadInt16();
            short transY = reader.ReadInt16();
            Width = reader.ReadInt16();
            Height = reader.ReadInt16();

            Translate = GetPosition() + new Syroot.Maths.Vector3F(transX, transY, 0);

            Content = new BxlytWindowContent(header);
            Content.Material = new Material();
            Content.Material.TextureMaps = new BxlytTextureRef[0];

            Console.WriteLine($"numParams {numParams}");
            FrameCount = 4;
            WindowKind = WindowKind.Around;

             numParams -= 5;
            if (numParams > 0) {
                WindowFrames = new List<BxlytWindowFrame>();
                for (int i = 0; i < 4; i++)
                {
                    string texName = BloResource.Read(reader, header);
                    Console.WriteLine($"texName {texName}");

                    var frame = new WindowFrame(texName);
                    WindowFrames.Add(frame);
                }
            }

     /*       if (numParams > 0) {
                PaletteName = BloResource.Read(reader, header);
                numParams--;
            }

            PaletteName = BloResource.Read(reader, header);
            byte src = reader.ReadByte();

            for (int i = 0; i < 4; i++)
            {
                var mirror = (byte)((src >> (6 - (i * 2))) & 0x3);
                if (mirror == 1)
                    WindowFrames[i].TextureFlip = WindowFrameTexFlip.FlipV;
            }*/
        }

        public void Write(FileWriter writer)
        {

        }
    }

    public class WindowFrame : BxlytWindowFrame
    {
        public STColor8 Color { get; set; }

        public WindowFrame(string name)
        {
            Material = new Material();
            Material.TextureMaps = new BxlytTextureRef[1];
            Material.TextureMaps[0] = new BxlytTextureRef()
            {
                Name = name,
            };
        }
    }
}
