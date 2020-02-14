using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace LayoutBXLYT.GCBLO
{
    public class StringList
    {
        public static List<string> Read(FileReader reader, BLOHeader header)
        {
            List<string> values = new List<string>();

            ushort count = reader.ReadUInt16();
            if (count == 0) return values;

            ushort unk = reader.ReadUInt16(); //0xFFFF
            uint headerSize = reader.ReadUInt32(); //0x10

            long startpos = reader.Position;

            ushort count2 = reader.ReadUInt16(); //Same as count
            ushort[] offsets = reader.ReadUInt16s((int)count);
            for (int i = 0; i < count; i++)
            {
                reader.SeekBegin(startpos + offsets[i]);
                values.Add(BloResource.Read(reader, header));
            }
            return values;
        }

        public static void Write(List<string> values, FileWriter writer, BLOHeader header)
        {
            writer.Write(values.Count);
            writer.Write((ushort)0xFFFF);
            writer.Write((ushort)0x10);

            long startpos = writer.Position;
            writer.Write(values.Count);
            writer.Write(new ushort[values.Count]);
            for (int i = 0; i < values.Count; i++)
            {
                writer.WriteUint16Offset(startpos + (i * 4), startpos);
                BloResource.Write(writer, values[i], header); 
            }
        }
    }
}
