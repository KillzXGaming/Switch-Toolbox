using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.IO;
using CsvHelper;
using System.Windows.Forms;

namespace Switch_Toolbox.Library
{
    //Parsing based on wiki https://zeldamods.org/wiki/ResourceSizeTable.product.rsizetable
    //Functions from https://github.com/zeldamods/rstb/blob/master/rstb/rstb.py
    //Copyright 2018 leoetlino <leo@leolam.fr>
    //Licensed under GPLv2+
    // Copy of license can be found under Lib/Licenses
    public class RSTB
    {
        public bool IsBigEndian { get; set; } = true;
        public bool IsWiiU => IsBigEndian;

        public Dictionary<string, uint> NameTables { get; set; }
        public Dictionary<uint, uint> Crc32Tables { get; set; }

        private int ParseSize(string FilePath, byte[] Data = null, bool IsYaz0Compressed = false, bool Force = false)
        {
            var size = new RSTB.SizeCalculator().CalculateFileSize(FilePath, Data, IsWiiU, IsYaz0Compressed, Force);
            if (size == 0)
            {
                var result = MessageBox.Show("Error! Could not calculate size for resource entry! Do you want to remove it instead?", "Resource Table", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.OK)
                {
                    DeleteEntry(FilePath);
                }
            }
            return size;
        }

        public void SetEntry(string FileName, byte[] Data = null, bool IsYaz0Compressed = false, bool Force = false)
        {
            uint OldSize = GetSize(FileName);
            uint NewSize = (uint)ParseSize(FileName, Data, IsYaz0Compressed, Force);

            STConsole.WriteLine($"Setting RSTB Entry!");
            STConsole.WriteLine($"{FileName} OldSize {OldSize}");
            STConsole.WriteLine($"{FileName} NewSize {NewSize}");

            SetSize(FileName, NewSize);
        }

        public void LoadFile(string FileName) {
            Read(new FileReader(EveryFileExplorer.YAZ0.Decompress(FileName)));
        }

        //Hacky but no other way. Check if the name path count is over 10k
        //This shouldn't ever be the case, and little endian bom would read over that amount
        private bool CheckEndianness(FileReader reader)
        {
            reader.SetByteOrder(true);

            reader.Position = 8;
            bool IsBigEndian = reader.ReadUInt32() < 10000;
            reader.Position = 0;

            return IsBigEndian;
        }

        public RSTB()
        {
            Crc32Tables = new Dictionary<uint, uint>();
            NameTables = new Dictionary<string, uint>();
        }

        public void Read(FileReader reader)
        {
            IsBigEndian = CheckEndianness(reader);

            reader.SetByteOrder(IsBigEndian);
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

            reader.Close();
            reader.Dispose();
        }

        public void Write(FileWriter writer)
        {
            writer.SetByteOrder(IsBigEndian);
            writer.WriteSignature("RSTB");
            writer.Write(Crc32Tables.Count);
            writer.Write(NameTables.Count);

            var Crc32Sorted = Crc32Tables.OrderBy(x => x.Key);
            var NamesSorted = NameTables.OrderBy(x => x.Key);

            foreach (var table in Crc32Sorted)
            {
                writer.Write(table.Key);
                writer.Write(table.Value);
            }

            foreach (var table in NamesSorted)
            {
                writer.Write(table.Key.ToByteArray(128));
                writer.Write(table.Value);
            }

            writer.Close();
            writer.Dispose();
        }

        //Gets the size of the resource in the RSTB
        public uint GetSize(string Name)
        {
            uint Crc32 = Name.EncodeCrc32();
            if (Crc32Tables.ContainsKey(Crc32))
                return Crc32Tables[Crc32];
            if (NameTables.ContainsKey(Name))
                return NameTables[Name];

            return 0;
        }

        //Sets the size of the resource in the RSTB
        public void SetSize(string Name, uint Size)
        {
            if (IsNeededInNameMap(Name))
            {
                if (Name.Length > 128)
                    throw new Exception("Name is too long! Must be smaller than 128 characters!");

                NameTables[Name] = Size;
            }
            else
            {
                uint Crc32 = Name.EncodeCrc32();
                Crc32Tables[Crc32] = Size;
            }
        }

        public void DeleteEntry(string FileName)
        {
            if (!IsInTable(FileName))
                MessageBox.Show("File not in table! Could not remove entry! " + FileName);

            uint Crc32 = FileName.EncodeCrc32();
            if (Crc32Tables.ContainsKey(Crc32))
                Crc32Tables.Remove(Crc32);
            if (NameTables.ContainsKey(FileName))
                NameTables.Remove(FileName);
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

        public class FactoryParsers
        {
            public int ParseWiiU(byte[] FileData) { return 0;}
            public int ParseNX(byte[] FileData) { return 0; }
        }

        public class SizeCalculator
        {
            public Dictionary<string, FactoryParsers> FactoryParsers = new Dictionary<string, FactoryParsers>();
            public Dictionary<string, Factory> FactoryInfo = new Dictionary<string, Factory>();

            public class Factory
            {
                public int Size_NX { get; set; }
                public int Size_WiiU { get; set; }
                public int Alignment { get; set; }
                public int Parse_Size_NX { get; set; }
                public int Parse_Size_WiiU { get; set; }

                public float Multiplier { get; set; }
                public int Constant { get; set; }
                public bool IsComplex { get; set; }
            }

            public SizeCalculator()
            {
                var reader = new System.IO.StreamReader(new System.IO.MemoryStream(Properties.Resources.resource_factory_info), true);
                using (var csv = new CsvReader(reader, new CsvHelper.Configuration.Configuration() { Delimiter = "\t" }))
                {
                    csv.Read();
                    csv.ReadHeader();
                    while (csv.Read())
                    {
                        //Parse table entries
                        var name = csv.GetField("name");
                        var size_nx = csv.GetField("size_nx");
                        var size_wiiu = csv.GetField("size_wiiu");
                        var alignment = csv.GetField("alignment");
                        var constant = csv.GetField("constant");
                        var parse_size_nx = csv.GetField("parse_size_nx");
                        var parse_size_wiiu = csv.GetField("parse_size_wiiu");
                        var multiplier = csv.GetField("multiplier");
                        var otherExtensions = csv.GetField("other_extensions").Split(',');

                        foreach (var extension in otherExtensions)
                            Console.WriteLine("other_extensions " + extension);

                        //Add new factory to dictionary for extension data
                        var factory = new Factory();
                        ParseObject(size_nx, factory.Size_NX);
                        ParseObject(size_wiiu, factory.Size_WiiU);
                        ParseObject(alignment, factory.Alignment);
                        ParseObject(constant, factory.Constant);
                        ParseObject(parse_size_nx, factory.Parse_Size_NX);
                        ParseObject(parse_size_wiiu, factory.Parse_Size_WiiU);
                        ParseObject(multiplier, factory.Multiplier);

                        FactoryInfo.Add(name, factory);
                        foreach (var extension in otherExtensions)
                        {
                            if (extension != string.Empty)
                                FactoryInfo.Add(extension.Strip(), factory);
                        }
                    }
                }
            }

            private static void ParseObject(string value, uint output) {
                uint.TryParse(value, out output);
            }

            private static void ParseObject(string value, float output) {
                float.TryParse(value, out output);
            }

            public int CalculateFileSize(string FileName, byte[] Data, bool IsWiiU,bool IsYaz0Compressed, bool Force)
            {
                return CalculateFileSizeByExtension(FileName, Data, IsWiiU, System.IO.Path.GetExtension(FileName), IsYaz0Compressed, Force);
            }

            private int CalculateFileSizeByExtension(string FileName, byte[] Data, bool WiiU, string Ext, bool IsYaz0Compressed, bool Force = false)
            {
                int Size = 0;
                if (System.IO.File.Exists(FileName))
                {
                    if (Ext.StartsWith("s") || IsYaz0Compressed)
                    {
                        using (var reader = new FileReader(FileName))
                        {
                            reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                            reader.Seek(4, System.IO.SeekOrigin.Begin);
                            Size = reader.ReadInt32();
                        }
                    }
                    else
                        Size = (int)new System.IO.FileInfo(FileName).Length;
                }
                else
                    Size = Data.Length;

                byte[] FileData = Data;

                Size = (Size + 31) & -32;
                string ActualExt = Ext.Replace(".s", ".").Remove(0,1);
                Factory Info = FactoryInfo[ActualExt];
                if (Info.IsComplex)
                {
                    if (!FactoryParsers.ContainsKey(ActualExt) && !Force)
                        return 0;
                    if (System.IO.File.Exists(FileName))
                        FileData = EveryFileExplorer.YAZ0.Decompress(FileName);
                    else if (Data != null)
                        FileData = EveryFileExplorer.YAZ0.Decompress(Data);
                }

                if (WiiU)
                {
                    Size += 0xe4;
                    Size += Info.Size_WiiU;

                    if (!FactoryParsers.ContainsKey(ActualExt))
                        Size += 0;
                    else if (Info.IsComplex)
                        Size += FactoryParsers[ActualExt].ParseWiiU(FileData);
                    else
                        Size += Info.Parse_Size_WiiU;

                    if (ActualExt == "beventpack")
                        Size += 0xe0;
                    if (ActualExt == "bfevfl")
                        Size += 0x58;
                }
                else
                {
                    Size += 0x168;
                    Size += Info.Size_NX;

                    if (!FactoryParsers.ContainsKey(ActualExt))
                        Size += 0;
                    else if (Info.IsComplex)
                        Size += FactoryParsers[ActualExt].ParseNX(FileData);
                    else
                        Size += Info.Parse_Size_NX;
                }

                return Size;
            }
        }
    }
}
