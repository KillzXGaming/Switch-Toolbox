using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library;
using System.ComponentModel;
using Syroot.Maths;

namespace LayoutBXLYT.Revolution
{
    public class TXT1 : PAN1, ITextPane
    {
        public override string Signature { get; } = "txt1";

        public TXT1() : base()
        {

        }

        public List<string> GetFonts
        {
            get { return layoutFile.Fonts; }
        }

        [Browsable(false)]
        public Toolbox.Library.Rendering.RenderableTex RenderableFont { get; set; }

        [DisplayName("Horizontal Alignment"), CategoryAttribute("Font")]
        public OriginX HorizontalAlignment
        {
            get { return (OriginX)((TextAlignment >> 2) & 0x3); }
            set
            {
                TextAlignment &= unchecked((byte)(~0xC));
                TextAlignment |= (byte)((byte)(value) << 2);
            }
        }

        [DisplayName("Vertical Alignment"), CategoryAttribute("Font")]
        public OriginY VerticalAlignment
        {
            get { return (OriginY)((TextAlignment) & 0x3); }
            set
            {
                TextAlignment &= unchecked((byte)(~0x3));
                TextAlignment |= (byte)(value);
            }
        }

        [Browsable(false)]
        public ushort RestrictedLength
        {
            get
            { //Divide by 2 due to 2 characters taking up 2 bytes
              //Subtract 1 due to padding
                return (ushort)((TextLength / 2) - 1);
            }
            set
            {
                TextLength = (ushort)((value * 2) + 1);
            }
        }

        [Browsable(false)]
        public ushort TextLength { get; set; }
        [Browsable(false)]
        public ushort MaxTextLength { get; set; }
        [Browsable(false)]
        public ushort MaterialIndex { get; set; }
        [Browsable(false)]
        public ushort FontIndex { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytMaterial Material { get; set; }

        [DisplayName("Text Alignment"), CategoryAttribute("Font")]
        public byte TextAlignment { get; set; }

        [DisplayName("Line Alignment"), CategoryAttribute("Font")]
        public LineAlign LineAlignment { get; set; }

        [DisplayName("Italic Tilt"), CategoryAttribute("Font")]
        public float ItalicTilt { get; set; }

        [DisplayName("Top Color"), CategoryAttribute("Font")]
        public STColor8 FontTopColor { get; set; }

        [DisplayName("Bottom Color"), CategoryAttribute("Font")]
        public STColor8 FontBottomColor { get; set; }

        [DisplayName("Font Size"), CategoryAttribute("Font")]
        public Vector2F FontSize { get; set; }

        [DisplayName("Character Space"), CategoryAttribute("Font")]
        public float CharacterSpace { get; set; }

        [DisplayName("Line Space"), CategoryAttribute("Font")]
        public float LineSpace { get; set; }

        [DisplayName("Shadow Position"), CategoryAttribute("Shadows")]
        public Vector2F ShadowXY { get; set; }

        [DisplayName("Shadow Size"), CategoryAttribute("Shadows")]
        public Vector2F ShadowXYSize { get; set; }

        [DisplayName("Shadow Fore Color"), CategoryAttribute("Shadows")]
        public STColor8 ShadowForeColor { get; set; }

        [DisplayName("Shadow Back Color"), CategoryAttribute("Shadows")]
        public STColor8 ShadowBackColor { get; set; }

        [DisplayName("Shadow Italic"), CategoryAttribute("Shadows")]
        public float ShadowItalic { get; set; }

        [DisplayName("Text Box Name"), CategoryAttribute("Text Box")]
        public string TextBoxName { get; set; }

        private string text;

        [DisplayName("Text"), CategoryAttribute("Text Box")]
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                TextLength = (ushort)((text.Length * 2) + 2);
                UpdateTextRender();
            }
        }

        [DisplayName("Per Character Transform"), CategoryAttribute("Font")]
        public bool PerCharTransform
        {
            get { return (_flags & 0x10) != 0; }
            set { _flags = value ? (byte)(_flags | 0x10) : unchecked((byte)(_flags & (~0x10))); }
        }
        [DisplayName("Restricted Text Length"), CategoryAttribute("Font")]
        public bool RestrictedTextLengthEnabled
        {
            get { return (_flags & 0x2) != 0; }
            set { _flags = value ? (byte)(_flags | 0x2) : unchecked((byte)(_flags & (~0x2))); }
        }
        [DisplayName("Enable Shadows"), CategoryAttribute("Font")]
        public bool ShadowEnabled
        {
            get { return (_flags & 1) != 0; }
            set { _flags = value ? (byte)(_flags | 1) : unchecked((byte)(_flags & (~1))); }
        }

        [DisplayName("Font Name"), CategoryAttribute("Font")]
        public string FontName { get; set; }

        private byte _flags;
        private BxlytHeader layoutFile;

        public void UpdateTextRender()
        {
            if (RenderableFont == null) return;

            System.Drawing.Bitmap bitmap = null;
            foreach (var fontFile in FirstPlugin.PluginRuntime.BxfntFiles)
            {
                if (Utils.CompareNoExtension(fontFile.FileName, FontName))
                    bitmap = fontFile.GetBitmap(Text, false, this);
            }

            if (bitmap != null)
                RenderableFont.UpdateFromBitmap(bitmap);
        }

        public void CopyMaterial()
        {
            Material = Material.Clone();
        }

        public TXT1(BRLYT.Header header, string name)
        {
            LoadDefaults();
            Name = name;
            layoutFile = header;
            //Add new material
            var mat = new Material(this.Name, header);
            header.MaterialList.Materials.Add(mat);
            MaterialIndex = (ushort)header.MaterialList.Materials.IndexOf(mat);
            Material = mat;

            Text = Encoding.Unicode.GetString(new byte[0]);

            FontIndex = 0;
            FontName = "";
            TextLength = 4;
            MaxTextLength = 4;
            TextAlignment = 0;
            LineAlignment = LineAlign.Unspecified;
            ItalicTilt = 0;
            FontTopColor = STColor8.White;
            FontBottomColor = STColor8.White;
            FontSize = new Vector2F(92, 101);
            CharacterSpace = 0;
            LineSpace = 0;
            ShadowXY = new Vector2F(1, -1);
            ShadowXYSize = new Vector2F(1, 1);
            ShadowBackColor = STColor8.Black;
            ShadowForeColor = STColor8.Black;
            ShadowItalic = 0;
        }

        public TXT1(FileReader reader, BxlytHeader header) : base(reader, header)
        {
            layoutFile = header;

            TextLength = reader.ReadUInt16();
            MaxTextLength = reader.ReadUInt16();
            MaterialIndex = reader.ReadUInt16();
            FontIndex = reader.ReadUInt16();
            TextAlignment = reader.ReadByte();
            LineAlignment = (LineAlign)reader.ReadByte();
            _flags = reader.ReadByte();
            reader.Seek(1); //padding
            uint textOffset = reader.ReadUInt32();
            FontTopColor = STColor8.FromBytes(reader.ReadBytes(4));
            FontBottomColor = STColor8.FromBytes(reader.ReadBytes(4));
            FontSize = reader.ReadVec2SY();
            CharacterSpace = reader.ReadSingle();
            LineSpace = reader.ReadSingle();

            if (MaterialIndex != ushort.MaxValue && header.Materials.Count > 0)
                Material = header.Materials[MaterialIndex];

            if (FontIndex != ushort.MaxValue && header.Fonts.Count > 0)
                FontName = header.Fonts[FontIndex];

            if (RestrictedTextLengthEnabled)
                text = reader.ReadZeroTerminatedString(Encoding.Unicode);
            else
                text = reader.ReadZeroTerminatedString(Encoding.Unicode);
        }

        public override void Write(FileWriter writer, LayoutHeader header)
        {
            long pos = writer.Position - 8;

            base.Write(writer, header);
            writer.Write(TextLength);
            writer.Write(MaxTextLength);
            writer.Write(MaterialIndex);
            writer.Write(FontIndex);
            writer.Write(TextAlignment);
            writer.Write(LineAlignment, false);
            writer.Write(_flags);
            writer.Seek(1);
            long _ofsTextPos = writer.Position;
            writer.Write(0); //text offset
            writer.Write(FontTopColor.ToBytes());
            writer.Write(FontBottomColor.ToBytes());
            writer.Write(FontSize);
            writer.Write(CharacterSpace);
            writer.Write(LineSpace);

            if (Text != null)
            {
                Encoding encoding = Encoding.Unicode;
                if (writer.ByteOrder == Syroot.BinaryData.ByteOrder.BigEndian)
                    encoding = Encoding.BigEndianUnicode;

                writer.WriteUint32Offset(_ofsTextPos, pos);
                if (RestrictedTextLengthEnabled)
                    writer.WriteString(Text, MaxTextLength, encoding);
                else
                    writer.WriteString(Text, TextLength, encoding);
            }
        }

        public enum BorderType : byte
        {
            Standard = 0,
            DeleteBorder = 1,
            RenderTwoCycles = 2,
        };
    }
}
