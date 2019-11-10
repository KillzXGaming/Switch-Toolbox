using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace FirstPlugin.LuigisMansion.DarkMoon
{
    public class ChunkEntry
    {
        public uint Unknown1;
        public uint ChunkOffset;
        public DataType ChunkType;
        public uint ChunkSubCount;
        public uint Unknown3;
    }

    public class ChunkSubEntry
    {
        public SubDataType ChunkType;
        public byte BlockIndex;
        public uint ChunkSize;
        public uint ChunkOffset;
    }

    //Table consists of 2 chunk entry lists that define how the .data reads sections
    public class LM2_ChunkTable
    {
        private const int ChunkInfoIdenfier = 0x2001301;

        //I am uncertain how these chunk lists work. There is first a list with an identifier and one extra unknown
        //The second list can contain the same entries as the other list, however it may include more chunks 
        //Example, the first list may have image headers, while the second include both image headers and image blocks
        public List<ChunkEntry> ChunkEntries = new List<ChunkEntry>();
        public List<ChunkSubEntry> ChunkSubEntries = new List<ChunkSubEntry>();

        public void Read(FileReader tableReader)
        {
            tableReader.SetByteOrder(false);

            //Each data type has chunk info. There is no counter so keep checking idenfier
            while (tableReader.ReadUInt32() == ChunkInfoIdenfier)
            {
                ChunkEntry entry = new ChunkEntry();
                ChunkEntries.Add(entry);

                entry.Unknown1 = tableReader.ReadUInt32(); //8
                entry.ChunkOffset = tableReader.ReadUInt32(); //The chunk offset in the file. Relative to the first chunk position in the file
                entry.ChunkType = tableReader.ReadEnum<DataType>(false); //The type of chunk. 0x8701B5 for example for texture info
                byte unk = tableReader.ReadByte();
                byte chunkFlags = tableReader.ReadByte();
                // entry.ChunkSubCount = tableReader.ReadUInt32(); //Uncertain about this. 2 for textures (info + block). Some sections however use large numbers.
                var flags = tableReader.ReadByte(); //Uncertain about this. 2 for textures (info + block). Some sections however use large numbers.
                entry.ChunkSubCount = flags;
                tableReader.ReadByte();
                tableReader.ReadByte();
                tableReader.ReadByte();

                //This increases by 2 each chunk info, however the starting value is not 0
                //Note the last entry does not have this
                entry.Unknown3 = tableReader.ReadUInt32();
            }

            if (ChunkEntries.Count > 0)
                ChunkEntries.LastOrDefault<ChunkEntry>().Unknown3 = 0;

            tableReader.Position -= 4; //Seek 4 back as the last entry lacks unkown 4
                                  //Check the chunk types
                                  //This time it includes more chunks (like image blocks)

            //Read to the end of the file as the rest of the table are types, offsets, and an unknown value
            while (!tableReader.EndOfStream && tableReader.Position <= tableReader.BaseStream.Length - 12)
            {
                ChunkSubEntry subEntry = new ChunkSubEntry();
                subEntry.ChunkType = tableReader.ReadEnum<SubDataType>(false); //The type of chunk. 0x8701B5 for example for texture info
                byte unk = tableReader.ReadByte();
                byte chunkFlags = tableReader.ReadByte();

                byte blockFlag = (byte)(chunkFlags >> 4);

                if (blockFlag < 8)
                    subEntry.BlockIndex = blockFlag;
                subEntry.ChunkSize = tableReader.ReadUInt32(); 
                subEntry.ChunkOffset = tableReader.ReadUInt32(); 
                ChunkSubEntries.Add(subEntry);
            }
        }
    }
}
