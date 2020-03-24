using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public interface CharMapping { }

    public class CMAPIndexTable : CharMapping
    {
        public short[] Table { get; set; }
    }

    public class CMAPDirect : CharMapping
    {
        public ushort Offset { get; set; }
    }

    public class CMAPScanMapping : CharMapping
    {
        public uint[] Codes { get; set; }
        public short[] Indexes { get; set; }
    }

    public class CMAP
    {
        public uint SectionSize;

        public char CharacterCodeBegin { get; set; }
        public char CharacterCodeEnd { get; set; }

        public Mapping MappingMethod { get; set; }

        private ushort Padding;

        public CharMapping MappingData;

        public enum Mapping : ushort
        {
            Direct,
            Table,
            Scan,
        }

        public ushort GetIndexFromCode(ushort code)
        {
            if (code < CharacterCodeBegin || code > CharacterCodeEnd) return 0xFFFF;

            switch (MappingMethod)
            {
                case Mapping.Direct:
                    return (UInt16)(code - CharacterCodeBegin + ((CMAPDirect)MappingData).Offset);
                case Mapping.Table:
                    return (ushort)((CMAPIndexTable)MappingData).Table[code - CharacterCodeBegin];
                case Mapping.Scan:
                    if (!((CMAPScanMapping)MappingData).Codes.Contains(code)) return 0xFFFF;
                    else
                    {
                        var codes = ((CMAPScanMapping)MappingData).Codes;
                        var index = Array.FindIndex(codes, map => map == code);

                        return (ushort)((CMAPScanMapping)MappingData).Indexes[index];
                    }
            }

            return 0xFFFF;
        }

        public CMAP NextCodeMapSection { get; set; }

        public static void GenerateCMAP(FileReader reader, FFNT header)
        {
            var fontSection = header.FontSection;
            var cmap = new CMAP();



            fontSection.CodeMap = cmap;
        }

        public void Read(FileReader reader, FFNT header, List<CMAP> CodeMaps)
        {
            uint CodeBegin = 0;
            uint CodeEnd = 0;

            long pos = reader.Position;

            reader.ReadSignature(4, "CMAP");
            SectionSize = reader.ReadUInt32();
            if (header.Platform == FFNT.PlatformType.NX)
            {
                CodeBegin = reader.ReadUInt32();
                CodeEnd = reader.ReadUInt32();
                MappingMethod = reader.ReadEnum<Mapping>(true);
                Padding = reader.ReadUInt16();
            }
            else
            {
                CodeBegin = reader.ReadUInt16();
                CodeEnd = reader.ReadUInt16();
                MappingMethod = reader.ReadEnum<Mapping>(true);
                Padding = reader.ReadUInt16();
            }

            CharacterCodeBegin = (char)CodeBegin;
            CharacterCodeEnd = (char)CodeEnd;

            uint NextMapOffset = reader.ReadUInt32();

        //Mapping methods from
        https://github.com/IcySon55/Kuriimu/blob/f670c2719affc1eaef8b4c40e40985881247acc7/src/Cetera/Font/BFFNT.cs#L211
            switch (MappingMethod)
            {
                case Mapping.Direct:
                    var charOffset = reader.ReadUInt16();
                    for (char i = CharacterCodeBegin; i <= CharacterCodeEnd; i++)
                    {
                        int idx = i - CharacterCodeBegin + charOffset;
                        header.FontSection.CodeMapDictionary[i] = idx < ushort.MaxValue ? idx : 0;

                        Console.WriteLine($"direct {i} {idx}");
                    }

                    MappingData = new CMAPDirect();
                    ((CMAPDirect)MappingData).Offset = charOffset;
                    break;
                case Mapping.Table:
                    List<short> table = new List<short>();
                    for (char i = CharacterCodeBegin; i <= CharacterCodeEnd; i++)
                    {
                        short idx = reader.ReadInt16();
                        if (idx != -1) header.FontSection.CodeMapDictionary[i] = idx;

                        Console.WriteLine($"table {i} {idx}");

                        table.Add(idx);
                    }

                    MappingData = new CMAPIndexTable();
                    ((CMAPIndexTable)MappingData).Table = table.ToArray();
                    break;
                case Mapping.Scan:
                    var CharEntryCount = reader.ReadUInt16();

                    if (header.Platform == FFNT.PlatformType.NX)
                        reader.ReadUInt16(); //Padding

                    uint[] codes = new uint[CharEntryCount];
                    short[] indexes = new short[CharEntryCount];

                    for (int i = 0; i < CharEntryCount; i++)
                    {
                        if (header.Platform == FFNT.PlatformType.NX)
                        {
                            uint charCode = reader.ReadUInt32();
                            short index = reader.ReadInt16();
                            short padding = reader.ReadInt16();
                            if (index != -1) header.FontSection.CodeMapDictionary[(char)charCode] = index;

                            codes[i] = charCode;
                            indexes[i] = index;
                        }
                        else
                        {
                            ushort charCode = reader.ReadUInt16();
                            short index = reader.ReadInt16();
                            if (index != -1) header.FontSection.CodeMapDictionary[(char)charCode] = index;

                            Console.WriteLine($"scan {i} {(char)charCode} {index}");

                            codes[i] = charCode;
                            indexes[i] = index;
                        }
                    }

                    MappingData = new CMAPScanMapping();
                    ((CMAPScanMapping)MappingData).Codes = codes;
                    ((CMAPScanMapping)MappingData).Indexes = indexes;
                    break;
            }

            if (NextMapOffset != 0)
            {
                reader.SeekBegin(NextMapOffset - 8);
                NextCodeMapSection = new CMAP();
                NextCodeMapSection.Read(reader, header, CodeMaps);
                CodeMaps.Add(NextCodeMapSection);
            }
            else
                reader.SeekBegin(pos + SectionSize);
        }

        public void Write(FileWriter writer, FFNT Header)
        {
            Header.BlockCounter += 1;

            long pos = writer.Position;

            writer.WriteSignature("CMAP");
            writer.Write(uint.MaxValue); //Section Size
            if (Header.Platform == FFNT.PlatformType.NX)
            {
                writer.Write((uint)CharacterCodeBegin);
                writer.Write((uint)CharacterCodeEnd);
            }
            else
            {
                writer.Write((ushort)CharacterCodeBegin);
                writer.Write((ushort)CharacterCodeEnd);
            }

            writer.Write(MappingMethod, true);
            writer.Seek(2);

            long DataPos = writer.Position;
            writer.Write(0); //Next Section Offset

            //Write the data
            switch (MappingMethod)
            {
                case Mapping.Direct:
                    writer.Write(((CMAPDirect)MappingData).Offset);
                    break;
                case Mapping.Table:
                    for (int i = 0; i < ((CMAPIndexTable)MappingData).Table.Length; i++)
                    {
                        writer.Write(((CMAPIndexTable)MappingData).Table[i]);
                    }
                    break;
                case Mapping.Scan:
                    writer.Write((ushort)((CMAPScanMapping)MappingData).Codes.Length);
                    if (Header.Platform == FFNT.PlatformType.NX)
                        writer.Seek(2); //Padding

                    for (int i = 0; i < ((CMAPScanMapping)MappingData).Codes.Length; i++)
                    {
                        if (Header.Platform == FFNT.PlatformType.NX)
                        {
                            writer.Write((uint)((CMAPScanMapping)MappingData).Codes[i]);
                            writer.Write(((CMAPScanMapping)MappingData).Indexes[i]);
                            writer.Write((ushort)0); //Padding
                        }
                        else
                        {
                            writer.Write((ushort)((CMAPScanMapping)MappingData).Codes[i]);
                            writer.Write(((CMAPScanMapping)MappingData).Indexes[i]);
                        }
                    }
                    break;
            }
            writer.AlignBytes(4); //Padding

            //Save section size
            long endPos = writer.Position;
            using (writer.TemporarySeek(pos + 4, SeekOrigin.Begin))
            {
                writer.Write((uint)(endPos - pos));
            }

            if (NextCodeMapSection != null)
            {
                writer.WriteUint32Offset(DataPos, -8);
                NextCodeMapSection.Write(writer, Header);
            }
        }

        //From https://github.com/dnasdw/3dsfont/blob/79e6f4ab6676d82fdcd6c0f79d9b0d7a343f82b5/src/bcfnt2charset/bcfnt2charset.cpp#L3
        //Todo add the rest of the encoding types
        public char CodeToU16Code(FINF.CharacterCode characterCode, ushort code)
        {
            if (code < 0x20)
            {
                return (char)0;
            }

            switch (characterCode)
            {
                case FINF.CharacterCode.Unicode:
                    return (char)code;
            }

            return (char)code;
        }
    }
}
