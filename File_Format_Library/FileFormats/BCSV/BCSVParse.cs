using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class BCSVParse
    {
        public class Field
        {
            public uint Hash { get; set; }
            public uint Offset { get; set; }

            public object Value;
        }

        public class DataEntry
        {
            public Dictionary<string, object> Fields;
        }

        public List<DataEntry> Entries = new List<DataEntry>();

        public void Read(FileReader reader)
        {
            uint numEntries = reader.ReadUInt32();
            uint entrySize = reader.ReadUInt32();
            ushort numFields = reader.ReadUInt16();
            ushort version = reader.ReadUInt16();
            uint magic = reader.ReadUInt32();
            uint unk = reader.ReadUInt32(); //Always 100000
            reader.ReadUInt32();//0
            reader.ReadUInt32();//0

            Field[] fields = new Field[numFields];
            for (int i = 0; i < numFields; i++) {
                fields[i] = new Field()
                {
                    Hash = reader.ReadUInt32(),
                    Offset = reader.ReadUInt32(),
                };
            }
            for (int i = 0; i < numEntries; i++)
            {
                DataEntry entry = new DataEntry();
                Entries.Add(entry);
                entry.Fields = new Dictionary<string, object>();

                long pos = reader.Position;
                for (int f = 0; f < fields.Length; f++)
                {
                    DataType type = DataType.String;
                    uint size = entrySize - fields[f].Offset;
                    if (f < fields.Length - 1) {
                        size = fields[f + 1].Offset - fields[f].Offset;
                    }
                    if (size == 1)
                        type = DataType.Byte;
                    if (size == 2)
                        type = DataType.Uint16;
                    if (size == 4)
                        type = DataType.Uint32;
                    if (size == 32)
                        type = DataType.Uint32;


                    reader.SeekBegin(pos + fields[f].Offset);
                    object value = 0;
                    string name = fields[f].Hash.ToString("x");
                    switch (type)
                    {
                        case DataType.Byte:
                            value = reader.ReadByte();
                            break;
                        case DataType.Float:
                            value = reader.ReadSingle();
                            break;
                        case DataType.Uint16:
                            value = reader.ReadUInt16();
                            break;
                        case DataType.Uint32:
                            value = reader.ReadUInt32();
                            break;
                        case DataType.String:
                            value = reader.ReadZeroTerminatedString(Encoding.UTF8);
                            break;
                    }

                    entry.Fields.Add(name, value);
                }

                reader.SeekBegin(pos + entrySize);
            }
        }

        public enum DataType
        {
            Byte,
            Uint16,
            Uint32,
            Uint64,
            Float,
            String,
        }

        public void Write(FileWriter writer)
        {

        }



        private static Dictionary<uint, string> hashes = new Dictionary<uint, string>();
        public static Dictionary<uint, string> Hashes
        {
            get {
                if (hashes.Count == 0)
                    CalculateHashes();
                return hashes; }
        }

        public static void CalculateHashes()
        {
            if (hashes.Count > 0)
                return;

            foreach (string hashStr in Properties.Resources.ACNH.Split('\n'))
            {
                string HashString = hashStr.TrimEnd();

                uint hash = (uint)Toolbox.Library.Security.Cryptography.Crc32.Compute(HashString);
                if (!hashes.ContainsKey(hash))
                    hashes.Add(hash, HashString);
            }

            foreach (var hash in AampLibraryCSharp.Hashes.hashName)
                if (!hashes.ContainsKey(hash.Key))
                    hashes.Add(hash.Key, hash.Value);
        }

        static uint NameHash(string name)
        {
            uint result = 0;
            for (int i = 0; i < name.Length; i++)
            {
                result = name[i] + result * 0x00000065;
            }
            return result;
        }

        public static uint stringToHash(string name)
        {
            int hash = 0;
            for (int i = 0; i < name.Length; i++)
            {
                hash *= 0x1F;
                hash += name[i];
            }

            return (uint)hash;
        }
    }
}
