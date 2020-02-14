using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace LayoutBXLYT.GCBLO
{
    public class BloResource
    {
        public static string Read(FileReader reader, BLOHeader header)
        {
            var type = (bloResourceType)reader.ReadByte();
            byte length = reader.ReadByte();
            string name = reader.ReadString(length, true);
            Console.WriteLine($"{type} {length} {name}");

            if (!header.Resources.ContainsKey(name))
                header.Resources.Add(name, type);
            return name;
        }

        public static void Write(FileWriter writer, string value, BLOHeader header)
        {
            if (header.Resources.ContainsKey(value))
                writer.Write((byte)header.Resources[value]);
            else
                writer.Write((byte)bloResourceType.LocalArchive);

            writer.Write((byte)value.Length);
            writer.WriteString(value);
        }

        public enum bloResourceType
        {
            None,
            LocalDirectory,
            LocalArchive,
            Global,
        }
    }
}
