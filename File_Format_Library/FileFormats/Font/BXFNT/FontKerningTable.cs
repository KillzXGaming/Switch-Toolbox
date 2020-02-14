using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    //Kerning Table
    //https://github.com/dnasdw/3dsfont/blob/4ead538d225d5d05929dce9d736bec91a6158052/src/bffnt/ResourceFormat.h
    public class FontKerningTable
    {
        private byte[] Data;

        public KerningFirstTable FirstTable { get; set; }

        public void Read(FileReader reader, FFNT Header, uint size)
        {
            Data = reader.ReadBytes((int)(size - 8));

            /*      if (Header.Platform == FFNT.PlatformType.NX)
                  {
                      ushort FirstWordCount = reader.ReadUInt16();
                      ushort padding = reader.ReadUInt16();

                      FirstTable = new KerningFirstTable();
                      FirstTable.Read(reader, Header);
                  }
                  else
                  {
                      ushort FirstWordCount = reader.ReadUInt16();

                      FirstTable = new KerningFirstTable();
                      FirstTable.Read(reader, Header);
                  }*/
        }

        public void Write(FileWriter writer, FFNT Header)
        {
            long pos = writer.Position;
            writer.WriteSignature("KRNG");
            writer.Write(uint.MaxValue);
            writer.Write(Data);
            writer.WriteSectionSizeU32(pos + 4, pos, writer.Position);
        }
    }

    public class KerningFirstTable
    {
        public uint FirstWordCount { get; set; }
        public uint Offset { get; set; }

        public void Read(FileReader reader, FFNT Header)
        {
            if (Header.Platform == FFNT.PlatformType.NX)
            {
                uint FirstWordCount = reader.ReadUInt32();
                uint Offset = reader.ReadUInt32();
            }
            else
            {
                uint FirstWordCount = reader.ReadUInt16();
                uint Offset = reader.ReadUInt16();
            }
        }
    }
}
