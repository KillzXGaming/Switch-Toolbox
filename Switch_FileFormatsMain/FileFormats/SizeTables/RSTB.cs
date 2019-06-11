using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.IO;

namespace FirstPlugin.FileFormats
{
    //Parsing based on wiki https://zeldamods.org/wiki/ResourceSizeTable.product.rsizetable
    //Functions from https://github.com/zeldamods/rstb/blob/master/rstb/rstb.py
    //Copyright 2018 leoetlino <leo@leolam.fr>
    //Licensed under GPLv2+
    // Copy of license can be found under Lib/Licenses

    public class RSTB
    {
        public Dictionary<string, uint> NameTables { get; set; }
        public Dictionary<uint, uint> Crc32Tables { get; set; }

        public void LoadFile(string FileName) {
            Read(new FileReader(FileName));
        }

        public RSTB()
        {
            Crc32Tables = new Dictionary<uint, uint>();
            NameTables = new Dictionary<string, uint>();
        }

        public void Read(FileReader reader)
        {
            reader.ReadSignature(4, "RSTB");
            uint Crc32TableCount = reader.ReadUInt32();
            uint NameTableCount = reader.ReadUInt32();

            for (int i = 0; i < Crc32TableCount; i++)
            {
                uint Crc32 = reader.ReadUInt32();
                uint Size = reader.ReadUInt32();
                Crc32Tables.Add(Crc32, Size);
            }

            for (int i = 0; i < NameTableCount; i++)
            {
                string Name = reader.ReadString(128);
                uint Size = reader.ReadUInt32();
                NameTables.Add(Name, Size);
            }
        }

        public void Write(FileWriter writer)
        {
            writer.WriteSignature("RSTB");
            writer.Write(Crc32Tables.Count);
            writer.Write(NameTables.Count);

            foreach (var table in Crc32Tables)
            {
                writer.Write(table.Key);
                writer.Write(table.Value);
            }

            foreach (var table in NameTables)
            {
                writer.Write(table.Key.ToByteArray(128));
                writer.Write(table.Value);
            }
        }

        public uint GetSize(string Name)
        {
            uint Crc32 = Name.EncodeCrc32();
            if (Crc32Tables.ContainsKey(Crc32))
                return Crc32Tables[Crc32];
            if (NameTables.ContainsKey(Name))
                return NameTables[Name];

            return 0;
        }

        public uint SetSize(string Name)
        {
            if (IsNeededInNameMap(Name))
            {
                if (Name.Length > 128)
                    throw new Exception("Name is too long! Must be smaller than 128 characters!");

            }

            uint Crc32 = Name.EncodeCrc32();
            if (Crc32Tables.ContainsKey(Crc32))
                return Crc32Tables[Crc32];
            if (NameTables.ContainsKey(Name))
                return NameTables[Name];

            return 0;
        }

        public int GetBufferSize()
        {
            return (4 + 4 + 4) + 8 * Crc32Tables.Count + (132 * NameTables.Count);
        }

        public bool IsInTable(string Name)
        {
            uint Crc32 = Name.EncodeCrc32();
            if (Crc32Tables.ContainsKey(Crc32) || NameTables.ContainsKey(Name))
                return true;

            return false;
        }

        public bool IsNeededInNameMap(string Name)
        {
            uint Crc32 = Name.EncodeCrc32();
            foreach (var existingName in NameTables.Keys)
                if (Crc32Tables.ContainsKey(existingName.EncodeCrc32()))
                    return true;
            return false;
        }
    }
}
