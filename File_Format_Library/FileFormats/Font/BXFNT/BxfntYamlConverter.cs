using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SharpYaml.Serialization;

namespace FirstPlugin
{
    public class BxfntYamlConverter
    {
        public static string ToYaml(FFNT header)
        {
            YamlMappingNode mapping = new YamlMappingNode();
            mapping.Add("Platform", header.Platform.ToString());
            mapping.Add("Version", header.Version.ToString("X"));
            mapping.Add("FontInfo", SaveFontInfo(header.FontSection));
            mapping.Add("KerningTable", SaveKerningTable(header.KerningTable));

            var doc = new YamlDocument(mapping);

            YamlStream stream = new YamlStream(doc);
            var buffer = new StringBuilder();
            using (var writer = new StringWriter(buffer)) {
                stream.Save(writer, true);
                return writer.ToString();
            }
        }

        private static YamlMappingNode SaveFontInfo(FINF fontInfo)
        {
            YamlMappingNode mapping = new YamlMappingNode();
            mapping.Add("Type", fontInfo.Type.ToString());
            mapping.Add("Font_Width", fontInfo.Width.ToString());
            mapping.Add("Font_Height", fontInfo.Height.ToString());
            mapping.Add("Line_Feed", fontInfo.LineFeed.ToString());
            mapping.Add("Ascent", fontInfo.Ascent.ToString());
            mapping.Add("AlterCharIndex", fontInfo.AlterCharIndex.ToString());
            mapping.Add("DefaultCharWidth", fontInfo.DefaultCharWidth.ToString());
            mapping.Add("DefaultGlyphWidth", fontInfo.DefaultGlyphWidth.ToString());
            mapping.Add("DefaultLeftWidth", fontInfo.DefaultLeftWidth.ToString());
            mapping.Add("Texture_Glyph", SaveTextureGlyph(fontInfo.TextureGlyph));
            mapping.Add("Characters", SaveCharacterMaps(fontInfo));
            mapping.Add("Character Widths", SaveCharacterWidths(fontInfo));
            return mapping;
        }

        private static YamlMappingNode SaveTextureGlyph(TGLP texInfo)
        {
            YamlMappingNode mapping = new YamlMappingNode();
            mapping.Add("Cell_Height", texInfo.CellHeight.ToString());
            mapping.Add("Cell_Width", texInfo.CellWidth.ToString());
            mapping.Add("Format", texInfo.Format.ToString());
            mapping.Add("BaseLinePos", texInfo.BaseLinePos.ToString());
            mapping.Add("MaxCharWidth", texInfo.MaxCharWidth.ToString());
            mapping.Add("Sheet_Height", texInfo.SheetHeight.ToString());
            mapping.Add("Sheet_Width", texInfo.SheetWidth.ToString());
            mapping.Add("RowCount", texInfo.RowCount.ToString());
            mapping.Add("LinesCount", texInfo.LinesCount.ToString());
            return mapping;
        }

        private static YamlSequenceNode SaveCharacterMaps(FINF fontInfo)
        {
            YamlSequenceNode node = new YamlSequenceNode();
           // node.Style = SharpYaml.YamlStyle.Flow;
            foreach (var character in fontInfo.CodeMapDictionary.Keys)
            {
                YamlMappingNode mapping = new YamlMappingNode();
                mapping.Add($"0x{((ushort)character).ToString("X4")}", character.ToString());
                node.Add(mapping);
            }
            
            return node;
        }

        private static YamlSequenceNode SaveCharacterWidths(FINF fontInfo)
        {
            YamlSequenceNode node = new YamlSequenceNode();
            foreach (var character in fontInfo.CodeMapDictionary)
            {
                YamlMappingNode mapping = new YamlMappingNode();
                mapping.Style = SharpYaml.YamlStyle.Flow;
                if (character.Value != -1) {
                    var width = fontInfo.GetCharacterWidth(character.Value);
                    mapping.Add($"0x{((ushort)character.Key).ToString("X4")}", SaveCharacterWidth(width));
                }
                node.Add(mapping);
            }
            return node;
        }

        private static YamlSequenceNode SaveCharacterWidth(CharacterWidthEntry table)
        {
            YamlSequenceNode node = new YamlSequenceNode();
            node.Style = SharpYaml.YamlStyle.Flow;
            node.Add(NewMappingNode("CharWidth", table.CharWidth.ToString()));
            node.Add(NewMappingNode("GlyphWidth", table.GlyphWidth.ToString()));
            node.Add(NewMappingNode("Left", table.Left.ToString()));
            return node;
        }

        private static YamlMappingNode NewMappingNode(string key, string value)
        {
            return new YamlMappingNode(new YamlScalarNode(key), new YamlScalarNode(value));
        }

        private static YamlMappingNode SaveKerningTable(FontKerningTable table)
        {
            YamlMappingNode mapping = new YamlMappingNode();
            if (table == null)
                return mapping;


            return mapping;
        }
    }
}
