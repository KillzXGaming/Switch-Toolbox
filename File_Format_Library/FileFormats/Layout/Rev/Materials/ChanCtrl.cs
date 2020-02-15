using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace LayoutBXLYT.Revolution
{
    public class ChanCtrl
    {
        public byte ColorMatSource { get; set; }
        public byte AlphaMatSource { get; set; }
        public byte Unknown1 { get; set; }
        public byte Unknown2 { get; set; }

        public ChanCtrl() {
            ColorMatSource = 1;
            AlphaMatSource = 1;
            Unknown1 = 0;
            Unknown2 = 0;
        }

        public bool HasDefaults() {
            return ColorMatSource == 1 &&
                   AlphaMatSource == 1 && 
                   Unknown1 == 0 &&
                   Unknown2 == 0;
        }

        public ChanCtrl(FileReader reader)
        {
            ColorMatSource = reader.ReadByte();
            AlphaMatSource = reader.ReadByte();
            Unknown1 = reader.ReadByte();
            Unknown2 = reader.ReadByte();
        }

        public void Write(FileWriter writer)
        {
            writer.Write(ColorMatSource);
            writer.Write(AlphaMatSource);
            writer.Write(Unknown1);
            writer.Write(Unknown2);
        }
    }
}
