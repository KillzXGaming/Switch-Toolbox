using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class GRP1 : GroupPane
    {
        public GRP1() { }

        public GRP1(FileReader reader, BxlytHeader header)
        {
            LayoutFile = header;

            ushort numNodes = 0;
            if (header.VersionMajor >= 5)
            {
                Name = reader.ReadString(34, true);
                numNodes = reader.ReadUInt16();
            }
            else
            {
                Name = reader.ReadString(24, true); ;
                numNodes = reader.ReadUInt16();
                reader.Seek(2); //padding
            }

            for (int i = 0; i < numNodes; i++)
                Panes.Add(reader.ReadString(24, true));
        }

        public override void Write(FileWriter writer, LayoutHeader header)
        {
            if (header.Version >= 0x05020000)
            {
                writer.WriteString(Name, 34);
                writer.Write((ushort)Panes.Count);
            }
            else
            {
                writer.WriteString(Name, 24);
                writer.Write((ushort)Panes.Count);
                writer.Seek(2);
            }

            for (int i = 0; i < Panes.Count; i++)
                writer.WriteString(Panes[i], 24);
        }
    }
}
