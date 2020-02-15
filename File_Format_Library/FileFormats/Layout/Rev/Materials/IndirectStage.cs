using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace LayoutBXLYT.Revolution
{
    public class IndirectStage
    {
        public byte TexCoord { get; set; }
        public byte TexMap { get; set; }
        public byte ScaleS { get; set; }
        public byte ScaleT { get; set; }

        public IndirectStage(FileReader reader)
        {
            TexCoord = reader.ReadByte();
            TexMap = reader.ReadByte();
            ScaleS = reader.ReadByte();
            ScaleT = reader.ReadByte();
        }

        public void Write(FileWriter writer)
        {
            writer.Write(TexCoord);
            writer.Write(TexMap);
            writer.Write(ScaleS);
            writer.Write(ScaleT);
        }
    }
}
