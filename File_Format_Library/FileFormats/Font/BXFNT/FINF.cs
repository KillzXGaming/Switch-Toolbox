using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class FINF
    {
        public Dictionary<char, int> CodeMapDictionary = new Dictionary<char, int>();

        public uint Size;
        public FontType Type { get; set; }
        public byte Width { get; set; }
        public byte Height { get; set; }
        public byte Ascent { get; set; }
        public ushort LineFeed { get; set; }
        public ushort AlterCharIndex { get; set; }
        public byte DefaultLeftWidth { get; set; }
        public byte DefaultGlyphWidth { get; set; }
        public byte DefaultCharWidth { get; set; }
        public CharacterCode CharEncoding { get; set; }
        public GLGR GlyphGroup;
        public TGLP TextureGlyph;
        public CMAP CodeMap;
        public CWDH CharacterWidth;

        public List<CWDH> CharacterWidths { get; set; }
        public List<CMAP> CodeMaps { get; set; }

        public enum FontType : byte
        {
            Glyph = 1,
            Texture = 2,
            PackedTexture = 3,
        }

        public enum CharacterCode : byte
        {
            UTF8 = 0,
            Unicode = 1,
            ShiftJIS = 2,
            CP1252 = 3,
        }

        public void Read(FileReader reader, FFNT header)
        {
            CharacterWidths = new List<CWDH>();
            CodeMaps = new List<CMAP>();

            string Signature = reader.ReadSignature(4);
            if (Signature != "FINF")
                throw new Exception($"Invalid signature {Signature}! Expected FINF.");
            Size = reader.ReadUInt32();

            if (header.Platform <= FFNT.PlatformType.Ctr && header.Version < 0x04000000)
            {
                Type = reader.ReadEnum<FontType>(true);
                LineFeed = reader.ReadByte();
                AlterCharIndex = reader.ReadUInt16();
                DefaultLeftWidth = reader.ReadByte();
                DefaultGlyphWidth = reader.ReadByte();
                DefaultCharWidth = reader.ReadByte();
                CharEncoding = reader.ReadEnum<CharacterCode>(true);
                uint tglpOffset = reader.ReadUInt32();
                uint cwdhOffset = reader.ReadUInt32();
                uint cmapOffset = reader.ReadUInt32();

                Height = reader.ReadByte();
                Width = reader.ReadByte();
                Ascent = reader.ReadByte();
                reader.ReadByte(); //Padding

                //Add counter for TGLP
                //Note the other counters are inside sections due to recusive setup
                header.BlockCounter += 1;

                TextureGlyph = new TGLP();
                using (reader.TemporarySeek(tglpOffset - 8, SeekOrigin.Begin))
                    TextureGlyph.Read(reader, header);

                CharacterWidth = new CWDH();
                CharacterWidths.Add(CharacterWidth);
                using (reader.TemporarySeek(cwdhOffset - 8, SeekOrigin.Begin))
                    CharacterWidth.Read(reader, header, CharacterWidths);

                CodeMap = new CMAP();
                CodeMaps.Add(CodeMap);
                using (reader.TemporarySeek(cmapOffset - 8, SeekOrigin.Begin))
                    CodeMap.Read(reader, header, CodeMaps);

            }
            else
            {
                Type = reader.ReadEnum<FontType>(true);
                Height = reader.ReadByte();
                Width = reader.ReadByte();
                Ascent = reader.ReadByte();
                LineFeed = reader.ReadUInt16();
                AlterCharIndex = reader.ReadUInt16();
                DefaultLeftWidth = reader.ReadByte();
                DefaultGlyphWidth = reader.ReadByte();
                DefaultCharWidth = reader.ReadByte();
                CharEncoding = reader.ReadEnum<CharacterCode>(true);
                uint tglpOffset = reader.ReadUInt32();
                uint cwdhOffset = reader.ReadUInt32();
                uint cmapOffset = reader.ReadUInt32();

                //Add counter for TGLP
                //Note the other counters are inside sections due to recusive setup
                header.BlockCounter += 1;

                TextureGlyph = new TGLP();
                using (reader.TemporarySeek(tglpOffset - 8, SeekOrigin.Begin))
                    TextureGlyph.Read(reader, header);

                CharacterWidth = new CWDH();
                CharacterWidths.Add(CharacterWidth);
                using (reader.TemporarySeek(cwdhOffset - 8, SeekOrigin.Begin))
                    CharacterWidth.Read(reader, header, CharacterWidths);

                CodeMap = new CMAP();
                CodeMaps.Add(CodeMap);
                using (reader.TemporarySeek(cmapOffset - 8, SeekOrigin.Begin))
                    CodeMap.Read(reader, header, CodeMaps);

            }
        }

        public void Write(FileWriter writer, FFNT header)
        {
            long pos = writer.Position;

            writer.WriteSignature("FINF");
            writer.Write(uint.MaxValue);

            long _ofsTGLP = 0;
            long _ofsCWDH = 0;
            long _ofsCMAP = 0;
            if (header.Platform <= FFNT.PlatformType.Ctr && header.Version < 0x04000000)
            {
                writer.Write(Type, true);
                writer.Write((byte)LineFeed);
                writer.Write(AlterCharIndex);
                writer.Write(DefaultLeftWidth);
                writer.Write(DefaultGlyphWidth);
                writer.Write(DefaultCharWidth);
                writer.Write(CharEncoding, true);

                _ofsTGLP = writer.Position;
                writer.Write(uint.MaxValue);
                _ofsCWDH = writer.Position;
                writer.Write(uint.MaxValue);
                _ofsCMAP = writer.Position;
                writer.Write(uint.MaxValue);

                writer.Write(Height);
                writer.Write(Width);
                writer.Write(Ascent);
                writer.Write((byte)0);
            }
            else
            {
                writer.Write(Type, true);
                writer.Write(Height);
                writer.Write(Width);
                writer.Write(Ascent);
                writer.Write(LineFeed);
                writer.Write(AlterCharIndex);
                writer.Write(DefaultLeftWidth);
                writer.Write(DefaultGlyphWidth);
                writer.Write(DefaultCharWidth);
                writer.Write(CharEncoding, true);
                _ofsTGLP = writer.Position;
                writer.Write(uint.MaxValue);
                _ofsCWDH = writer.Position;
                writer.Write(uint.MaxValue);
                _ofsCMAP = writer.Position;
                writer.Write(uint.MaxValue);
            }


            //Save section size
            long endPos = writer.Position;
            using (writer.TemporarySeek(pos + 4, SeekOrigin.Begin))
            {
                writer.Write((uint)(endPos - pos));
            }

            //Save Texture Glyph
            writer.WriteUint32Offset(_ofsTGLP, -8);
            TextureGlyph.Write(writer, header);

            //Save Character Widths
            writer.WriteUint32Offset(_ofsCWDH, -8);
            CharacterWidth.Write(writer, header);

            //Save Code Maps
            writer.WriteUint32Offset(_ofsCMAP, -8);
            CodeMap.Write(writer, header);
        }

        public CharacterWidthEntry GetCharacterWidth(int index)
        {
            if (index == -1)
                return null;

            for (int i = 0; i < CharacterWidths.Count; i++)
            {
                if (CharacterWidths[i].StartIndex <= index && CharacterWidths[i].EndIndex >= index)
                {
                    int CharaIndex = index - CharacterWidths[i].StartIndex;
                    return CharacterWidths[i].WidthEntries[CharaIndex];
                }
            }

            throw new Exception("Failed to get valid character index!");
        }
    }
}
