using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Syroot.BinaryData;
using System.IO;

namespace FirstPlugin
{
    public class SAHT
    {
        public string FileName { get; set; }

        public SAHT(string fileName) {
            FileName  = fileName;
            Read(new FileReader(fileName));
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

            ToTextFile();
        }

        public void ToTextFile()
        {
            StringWriter writer = new StringWriter();

            foreach (var entry in HashEntries)
            {
                writer.WriteLine($"{entry.Name}={entry.Hash.ToString("x")}");
            }

            File.WriteAllText(FileName + ".txt", writer.ToString());
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
