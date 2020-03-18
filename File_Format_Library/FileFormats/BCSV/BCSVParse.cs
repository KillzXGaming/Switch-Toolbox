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

        static Dictionary<uint, string> hashes = new Dictionary<uint, string>();
        public static Dictionary<uint, string> Hashes
        {
            get
            {
               // if (hashes.Count == 0)
                    //CalculateHashes();
                return hashes;
            }
        }

        public List<DataEntry> Entries = new List<DataEntry>();

        public void Read(FileReader reader)
        {
            uint numEntries = reader.ReadUInt32();
            uint entrySize = reader.ReadUInt32();
            ushort numFields = reader.ReadUInt16();
            byte flag1 = reader.ReadByte();
            byte flag2 = reader.ReadByte();
            if (flag1 == 1)
            {
                uint magic = reader.ReadUInt32();
                uint unk = reader.ReadUInt32(); //Always 100000
                reader.ReadUInt32();//0
                reader.ReadUInt32();//0
            }

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
                        type = DataType.Int16;
                    if (size == 4)
                        type = DataType.Int32;

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
                        case DataType.Int16:
                            value = reader.ReadInt16();
                            break;
                        case DataType.Int32:
                            value = reader.ReadInt32();
                            if (IsFloatValue((int)value))
                            {
                                reader.Seek(-4);
                                value = reader.ReadSingle();
                            }
                            break;
                        case DataType.String:
                            value = reader.ReadZeroTerminatedString(Encoding.UTF8);
                            break;
                    }

                    if (Hashes.ContainsKey(fields[f].Hash))
                        name = Hashes[fields[f].Hash];

                    entry.Fields.Add(name, value);
                }

                reader.SeekBegin(pos + entrySize);
            }
        }

        private bool IsFloatValue(int value) {
            return value.ToString().Length > 6;
        }

        public enum DataType
        {
            Byte,
            Int16,
            Int32,
            Int64,
            Float,
            String,
        }

        public void Write(FileWriter writer)
        {
            writer.Write(Entries.FirstOrDefault().Fields.Count);
        }

        private static void CalculateHashes()
        {

        }
    }
}
