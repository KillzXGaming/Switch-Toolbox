using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class CWDH
    {
        public ushort StartIndex { get; set; }
        public ushort EndIndex { get; set; }

        public List<CharacterWidthEntry> WidthEntries = new List<CharacterWidthEntry>();

        public CWDH NextWidthSection { get; set; }

        public ushort EntryCount
        {
            get { return (ushort)(EndIndex - StartIndex + 1); }
        }

        public uint SectionSize;

        public void Read(FileReader reader, FFNT header, List<CWDH> CharacterWidths)
        {
            long pos = reader.Position;

            reader.ReadSignature(4, "CWDH");
            SectionSize = reader.ReadUInt32();
            StartIndex = reader.ReadUInt16();
            EndIndex = reader.ReadUInt16();
            uint NextWidthSectionOffset = reader.ReadUInt32();

            for (ushort i = StartIndex; i <= EndIndex; i++)
            {
                var entry = new CharacterWidthEntry();
                entry.Left = reader.ReadSByte();
                entry.GlyphWidth = reader.ReadByte();
                entry.CharWidth = reader.ReadByte();
                WidthEntries.Add(entry);
            }

            if (NextWidthSectionOffset != 0)
            {
                reader.SeekBegin((int)NextWidthSectionOffset - 8);
                NextWidthSection = new CWDH();
                NextWidthSection.Read(reader, header, CharacterWidths);
                CharacterWidths.Add(NextWidthSection);
            }
            else
                reader.SeekBegin(pos + SectionSize);
        }

        public void Write(FileWriter writer, FFNT Header)
        {
            Header.BlockCounter += 1;

            long pos = writer.Position;

            writer.WriteSignature("CWDH");
            writer.Write(uint.MaxValue); //Section Size
            writer.Write(StartIndex);
            writer.Write(EndIndex);

            long DataPos = writer.Position;
            writer.Write(0); //NextOffset

            for (int i = 0; i < WidthEntries.Count; i++)
            {
                writer.Write(WidthEntries[i].Left);
                writer.Write(WidthEntries[i].GlyphWidth);
                writer.Write(WidthEntries[i].CharWidth);
            }

            writer.Align(4);

            if (NextWidthSection != null)
            {
                writer.WriteUint32Offset(DataPos, -8);
                NextWidthSection.Write(writer, Header);
            }

            //Save section size
            long endPos = writer.Position;
            using (writer.TemporarySeek(pos + 4, SeekOrigin.Begin))
            {
                writer.Write((uint)(endPos - pos));
            }
        }
    }

    public class CharacterWidthEntry
    {
        public sbyte Left { get; set; }
        public byte GlyphWidth { get; set; }
        public byte CharWidth { get; set; }
    }
}
