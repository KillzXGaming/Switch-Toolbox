using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Syroot.Maths;
using Toolbox.Library.IO;

namespace LayoutBXLYT.CTR
{
    public class USD1 : UserData
    {
        public USD1()
        {
            Entries = new List<UserDataEntry>();
        }

        public override UserDataEntry CreateUserData() {
            return new USD1Entry();
        }

        public USD1(FileReader reader, BxlytHeader header) : base()
        {
            long startPos = reader.Position - 8;

            ushort numEntries = reader.ReadUInt16();
            reader.ReadUInt16(); //padding

            for (int i = 0; i < numEntries; i++)
                Entries.Add(new USD1Entry(reader, startPos, header));
        }

        public override void Write(FileWriter writer, LayoutHeader header)
        {
            long startPos = writer.Position - 8;

            writer.Write((ushort)Entries.Count);
            writer.Write((ushort)0);

            long enryPos = writer.Position;
            for (int i = 0; i < Entries.Count; i++)
                ((USD1Entry)Entries[i]).Write(writer, header);

            for (int i = 0; i < Entries.Count; i++)
            {
                writer.WriteUint32Offset(Entries[i]._pos + 4, Entries[i]._pos);
                switch (Entries[i].Type)
                {
                    case UserDataType.String:
                        writer.WriteString(Entries[i].GetString());
                        break;
                    case UserDataType.Int:
                        writer.Write(Entries[i].GetInts());
                        break;
                    case UserDataType.Float:
                        writer.Write(Entries[i].GetFloats());
                        break;
                }
            }

            //Write strings after
            for (int i = 0; i < Entries.Count; i++)
            {
                writer.WriteUint32Offset(Entries[i]._pos, Entries[i]._pos);
                writer.WriteString(Entries[i].Name);
            }
        }
    }

    public class USD1Entry : UserDataEntry
    {
        public USD1Entry() { }

        public USD1Entry(FileReader reader, long startPos, BxlytHeader header)
        {
            long pos = reader.Position;

            uint nameOffset = reader.ReadUInt32();
            uint dataOffset = reader.ReadUInt32();
            ushort dataLength = reader.ReadUInt16();
            Type = reader.ReadEnum<UserDataType>(false);
            Unknown = reader.ReadByte();

            long datapos = reader.Position;

            if (nameOffset != 0)
            {
                reader.SeekBegin(pos + nameOffset);
                Name = reader.ReadZeroTerminatedString();
            }

            if (dataOffset != 0)
            {
                reader.SeekBegin(pos + dataOffset);
                switch (Type)
                {
                    case UserDataType.String:
                        if (dataLength != 0)
                            data = reader.ReadString((int)dataLength);
                        else
                            data = reader.ReadZeroTerminatedString();
                        break;
                    case UserDataType.Int:
                        data = reader.ReadInt32s((int)dataLength);
                        break;
                    case UserDataType.Float:
                        data = reader.ReadSingles((int)dataLength);
                        break;
                }
            }

            reader.SeekBegin(datapos);
        }

        public void Write(FileWriter writer, LayoutHeader header)
        {
            _pos = writer.Position;

            writer.Write(0); //nameOffset
            writer.Write(0); //dataOffset
            writer.Write((ushort)GetDataLength());
            writer.Write(Type, false);
            writer.Write(Unknown);
        }

        private int GetDataLength()
        {
            if (data is string)
                return ((string)data).Length;
            else if (data is int[])
                return ((int[])data).Length;
            else if (data is float[])
                return ((float[])data).Length;
            return 0;
        }
    }
}
