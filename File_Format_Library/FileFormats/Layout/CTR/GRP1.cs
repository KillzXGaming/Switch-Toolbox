using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace LayoutBXLYT.CTR
{
    public class GRP1 : GroupPane
    {
        public GRP1() : base()
        {

        }

        public GRP1(FileReader reader, BxlytHeader header)
        {
            LayoutFile = header;

            Name = reader.ReadString(0x10, true); ;
            int numNodes = reader.ReadUInt16();
            reader.Seek(2); //padding

            for (int i = 0; i < numNodes; i++)
                Panes.Add(reader.ReadString(0x10, true));
        }

        public override void Write(FileWriter writer, LayoutHeader header)
        {
            writer.WriteString(Name, 0x10);
            writer.Write((ushort)Panes.Count);
            writer.Seek(2);

            for (int i = 0; i < Panes.Count; i++)
                writer.WriteString(Panes[i], 0x10);
        }
    }

}
