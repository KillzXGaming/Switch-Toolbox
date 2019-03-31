using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.IO;
using Syroot.BinaryData;

namespace FirstPlugin
{
    public class SAHT
    {
        public SAHT(string FileName) {
            Read(new FileReader(FileName));
        }

        public List<HashEntry> HashEntries = new List<HashEntry>();

        private void Read(FileReader reader)
        {
            reader.ReadSignature(4, "SAHT");
            uint FileSize = reader.ReadUInt32();
            uint Offset = reader.ReadUInt32();
            uint EntryCount = reader.ReadUInt32();

            reader.Seek(Offset, System.IO.SeekOrigin.Begin);
            for (int i = 0; i <EntryCount; i++)
            {
                HashEntry entry = new HashEntry();
                entry.Read(reader);
                reader.Align(16);
                HashEntries.Add(entry);
            }

            foreach (var entry in HashEntries)
            {
                Console.WriteLine(entry.Hash + " " + entry.Name);
            }
        }

        public class HashEntry
        {
            public uint Hash { get; set; }
            public string Name { get; set; }

            public void Read(FileReader reader)
            {
                 Hash = reader.ReadUInt32();
                 Name = reader.ReadString(BinaryStringFormat.ZeroTerminated);
            }
        }
    }
}
