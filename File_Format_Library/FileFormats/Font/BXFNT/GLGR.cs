using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class GLGR
    {
        public uint SectionSize;
        public uint SheetSize;
        public uint HeaderSize = 0x20;
        public ushort GlyphsPerSheet;
        public ushort SetCount;
        public ushort SheetCount;
        public ushort CWDHCount;
        public ushort CMAPCount;

        public void Read(FileReader reader)
        {
            string Signature = reader.ReadString(4, Encoding.ASCII);
            if (Signature != "GLGR")
            {
                throw new Exception($"Invalid signature {Signature}! Expected GLGR.");
            }
            SectionSize = reader.ReadUInt32();
            SheetSize = reader.ReadUInt32();
            GlyphsPerSheet = reader.ReadUInt16();
            SetCount = reader.ReadUInt16();
            SheetCount = reader.ReadUInt16();
            CWDHCount = reader.ReadUInt16();
            CMAPCount = reader.ReadUInt16();
        }
    }
}
