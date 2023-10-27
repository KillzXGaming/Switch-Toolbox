using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Toolbox.Library;
using System.IO;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Drawing;
using FirstPlugin.Forms;
using LibEveryFileExplorer.GFX;
using System.Drawing.Imaging;

namespace FirstPlugin
{
    public class BXFNT : IFileFormat, IEditor<BffntEditor>, IConvertableTextFormat
    {
        public FileType FileType { get; set; } = FileType.Font;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Cafe Font", "CTR Font", "Revolution Font", "Revolution Archived Font" };
        public string[] Extension { get; set; } = new string[] { "*.bffnt", "*.bcfnt", "*.brfnt", "*.brfna" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                return reader.CheckSignature(4, "FFNT") ||
                       reader.CheckSignature(4, "CFNT") ||
                       reader.CheckSignature(4, "RFNT") ||
                       reader.CheckSignature(4, "TNFR") ||
                       reader.CheckSignature(4, "RFNA");
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public FFNT bffnt;

        public BffntEditor OpenForm()
        {
            BffntEditor form = new BffntEditor();
            form.Text = "Font Editor";
            form.Dock = DockStyle.Fill;
            return form;
        }

        public void FillEditor(UserControl control)
        {
            ((BffntEditor)control).LoadFontFile(this);
        }


        #region Text Converter Interface
        public TextFileType TextFileType => TextFileType.Yaml;
        public bool CanConvertBack => false;

        public string ConvertToString()
        {
            return BxfntYamlConverter.ToYaml(bffnt);
        }

        public void ConvertFromString(string text)
        {
        }

        #endregion

        public void Load(System.IO.Stream stream)
        {
            PluginRuntime.BxfntFiles.Add(this);

            CanSave = true;

            bffnt = new FFNT();
            bffnt.Read(new FileReader(stream));

            TGLP tglp = bffnt.FontSection.TextureGlyph;

            if (tglp.SheetDataList.Count > 0)
            {
                if (bffnt.Platform == FFNT.PlatformType.NX)
                {
                    var bntx = STFileLoader.OpenFileFormat(
                        new MemoryStream(Utils.CombineByteArray(tglp.SheetDataList.ToArray())), "Sheet_0");
                    if (bntx != null)
                    {
                        tglp.BinaryTextureFile = (BNTX)bntx;
                    }
                }
                else if (bffnt.Platform == FFNT.PlatformType.Cafe)
                {
                    for (int s = 0; s < tglp.SheetDataList.Count; s++)
                    {
                        var surface = new Gx2ImageBlock();
                        surface.Text = $"Sheet_{s}";
                        surface.Load(tglp, s);
                        tglp.Textures.Add(surface);
                    }
                }
                else if (bffnt.Platform == FFNT.PlatformType.Ctr)
                {
                    for (int s = 0; s < tglp.SheetDataList.Count; s++)
                    {
                        var surface = new CtrImageBlock();
                        surface.Text = $"Sheet_{s}";
                        surface.Load(tglp, s);
                        surface.GetBitmap().Save($"Image{s}.png");
                        tglp.Textures.Add(surface);
                    }
                }
                else
                {
                    for (int s = 0; s < tglp.SheetDataList.Count; s++)
                    {
                        var surface = new RevImageBlock();
                        surface.Text = $"Sheet_{s}";
                        surface.Load(tglp, s);
                        surface.GetBitmap().Save($"Image{s}.png");
                        tglp.Textures.Add(surface);
                    }
                }
            }

            int i = 0;
            foreach (byte[] texture in tglp.SheetDataList)
            {
                //   BNTX file = (BNTX)STFileLoader.OpenFileFormat("Sheet" + i++, texture);
                //  Nodes.Add(file);
            }
        }

        public BitmapFont GetBitmapFont(bool UseChannelComp = false)
        {
            return bffnt.GetBitmapFont(UseChannelComp);
        }

        public Bitmap GetBitmap(string text, bool reversewh, LayoutBXLYT.BasePane pane)
        {
            return bffnt.GetBitmap(text, reversewh, pane);
        }

        public void Unload()
        {
            PluginRuntime.BxfntFiles.Remove(this);
        }

        public void Save(System.IO.Stream stream)
        {
            bffnt.Write(new FileWriter(stream));
        }


        public class SheetEntry : TreeNodeCustom
        {
            public SheetEntry()
            {
                ImageKey = "fileBlank";
                SelectedImageKey = "fileBlank";

                ContextMenu = new ContextMenu();
                MenuItem export = new MenuItem("Export");
                ContextMenu.MenuItems.Add(export);
                export.Click += Export;

            }
            public byte[] data;

            public override void OnClick(TreeView treeview)
            {

            }

            private void Export(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = Text;
                sfd.DefaultExt = "bntx";
                sfd.Filter = "Supported Formats|*.bntx;|" +
                             "All files(*.*)|*.*";


                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, data);
                }
            }
        }
    }

    public class FFNT
    {
        public string Signature;

        public ushort BOM;
        public ushort HeaderSize;
        public uint Version { get; set; }

        public GLGR GlyphGroup { get; set; }
        public FINF FontSection { get; set; }
        public FontKerningTable KerningTable { get; set; }

        public PlatformType Platform { get; set; } = PlatformType.Cafe;

        public bool IsWiiLE => Signature == "TNFR";

        public enum PlatformType
        {
            Wii,
            Ctr,
            Cafe,
            NX,
        }

        public void Read(FileReader reader)
        {
            reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            Signature = reader.ReadString(4, Encoding.ASCII);
            if (Signature != "FFNT" && Signature != "CFNT" && Signature != "RFNT" && Signature != "TNFR" && Signature != "RFNA")
                throw new Exception($"Invalid signature {Signature}! Expected FFNT or CFNT or RFNT.");

            BOM = reader.ReadUInt16();
            reader.CheckByteOrderMark(BOM);

            if (Signature == "TNFR")
                reader.ReverseMagic = true;

            //Parse header first and check the version
            //Brfnt uses a slightly different header structure
            if (Signature == "RFNT" || Signature == "TNFR" || Signature == "RFNA")
            {
                Version = reader.ReadUInt16();
                uint FileSize = reader.ReadUInt32();
                HeaderSize = reader.ReadUInt16();
                ushort BlockCount = reader.ReadUInt16();
            }
            else
            {
                HeaderSize = reader.ReadUInt16();
                Version = reader.ReadUInt32();
                uint FileSize = reader.ReadUInt16();
                ushort BlockCount = reader.ReadUInt16();
                ushort Padding = reader.ReadUInt16();
            }

            //Check platform based on version, magic, and endianness
            if (reader.ByteOrder == Syroot.BinaryData.ByteOrder.LittleEndian)
            {
                if (Version >= 0x04010000)
                    Platform = PlatformType.NX;
                else
                    Platform = PlatformType.Ctr;
            }
            else
                Platform = PlatformType.Cafe;

            if (Signature == "CFNT")
                Platform = PlatformType.Ctr;
            if (Signature == "RFNT" || Signature == "TNFR" || Signature == "RFNA")
                Platform = PlatformType.Wii;

            Console.WriteLine($"Platform {Platform}");

            reader.Seek(HeaderSize, SeekOrigin.Begin);
            if (Signature == "RFNA")
            {
                GlyphGroup = new GLGR();
                GlyphGroup.Read(reader);
                // It's needed to take off 22 because of the included header length in SectionSize.
                reader.Seek(GlyphGroup.SectionSize - 0x16, SeekOrigin.Current);
            }
            FontSection = new FINF();
            if (GlyphGroup != null)
            {
                FontSection.GlyphGroup = GlyphGroup;
            }
            FontSection.Read(reader, this);

            //Check for any unread blocks
            reader.Seek(HeaderSize, SeekOrigin.Begin);
            while (!reader.EndOfStream)
            {
                long BlockStart = reader.Position;

                string BlockSignature = reader.ReadSignature(4);
                uint BlockSize = reader.ReadUInt32();

                switch (BlockSignature)
                {
                    case "FFNT":
                    case "FFNA":
                    case "FCPX":
                    case "CWDH":
                    case "CGLP":
                    case "CMAP":
                    case "TGLP":
                    case "FINF":
                        break;
                    case "KRNG":
                        KerningTable = new FontKerningTable();
                        KerningTable.Read(reader, this, BlockSize);
                        break;
                    case "GLGR":
                    case "HTGL":
                        break;
                    default:
                        throw new Exception("Unknown block found! " + BlockSignature);
                }

                reader.SeekBegin(BlockStart + BlockSize);
            }
        }

        internal int BlockCounter = 0;
        public void Write(FileWriter writer)
        {
            writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            BlockCounter = 1;

            writer.WriteSignature(Signature);
            writer.Write(BOM);
            writer.CheckByteOrderMark(BOM);

            if (Signature == "TNFR")
                writer.ReverseMagic = true;

            long _ofsFileSize;
            long _ofsBlockNum;
            if (Platform == PlatformType.Wii)
            {
                writer.Write((ushort)Version);
                _ofsFileSize = writer.Position;
                writer.Write(uint.MaxValue);
                writer.Write(HeaderSize);
                _ofsBlockNum = writer.Position;
                writer.Write((ushort)0); //BlockCount
            }
            else
            {
                writer.Write(HeaderSize);
                writer.Write(Version);
                _ofsFileSize = writer.Position;
                writer.Write(uint.MaxValue);
                _ofsBlockNum = writer.Position;
                writer.Write((ushort)0); //BlockCount
                writer.Write((ushort)0);
            }

            writer.SeekBegin(HeaderSize);
            FontSection.Write(writer, this);
            if (KerningTable != null)
            {
                BlockCounter++;
                KerningTable.Write(writer, this);
            }

            //Save Block Count
            using (writer.TemporarySeek(_ofsBlockNum, SeekOrigin.Begin))
            {
                writer.Write((ushort)(BlockCounter + 1));
            }

            //Save File size
            using (writer.TemporarySeek(_ofsFileSize, SeekOrigin.Begin))
            {
                writer.Write((uint)(writer.BaseStream.Length));
            }
        }

        private string CheckSignature(FileReader reader)
        {
            string Signature = reader.ReadString(4, Encoding.ASCII);
            reader.Seek(-4, SeekOrigin.Current);
            return Signature;
        }

        private BitmapFont bitmapFont;
        public Bitmap GetBitmap(string text, bool reversewh, LayoutBXLYT.BasePane pane)
        {
            var FontInfo = FontSection;
            var TextureGlyph = FontInfo.TextureGlyph;

            var textPane = (LayoutBXLYT.ITextPane)pane;

            int fontWidth = (int)textPane.FontSize.X;
            int fontHeight = (int)textPane.FontSize.Y;
            if (textPane.FontSize.X > 2)
            {
                fontWidth = (int)textPane.FontSize.X - 2;
                fontHeight = (int)textPane.FontSize.Y - 2;
            }

            float XScale = (fontWidth / TextureGlyph.CellWidth);
            float YScale = (fontHeight / TextureGlyph.CellWidth);
            float height = (TextureGlyph.SheetHeight - 2) / TextureGlyph.ColumnCount;

            /*   int pos = 0;
               for (int i = 0; i < text.Length; i++)
               {
                   char character = text[i];

                   int charWidth = (int)FontInfo.DefaultCharWidth;
                   int glyphWidth = (int)FontInfo.DefaultGlyphWidth;
                   int leftWidth = (int)FontInfo.DefaultLeftWidth;

                   if (FontInfo.CodeMapDictionary.ContainsKey(character))
                   {
                       var idx = FontInfo.CodeMapDictionary[character];
                       if (idx == 0xFFFF) continue;
                       var charWidthInfo = GetCharWidthInfoByIndex(FontInfo, (ushort)idx);

                       charWidth = charWidthInfo.CharWidth;
                       glyphWidth = charWidthInfo.GlyphWidth;
                       leftWidth = charWidthInfo.Left;
                   }


                 /*  Bitmap b = new Bitmap(width, height);
                   using (Graphics g = Graphics.FromImage(b))
                   {
                       g.DrawImage();
                   }
               }*/

            if (bitmapFont == null)
                bitmapFont = GetBitmapFont(true);

            return bitmapFont.PrintToBitmap(text, new BitmapFont.FontRenderSettings()
            {
                TopColor = textPane.FontTopColor.Color,
                BottomColor = textPane.FontBottomColor.Color,
                CharSpacing = (int)textPane.CharacterSpace,
                XScale = (textPane.FontSize.X / TextureGlyph.CellWidth),
                YScale = (textPane.FontSize.Y / TextureGlyph.CellHeight),
                LineSpacing = (int)textPane.LineSpace,
            });
        }

        public BitmapFont GetBitmapFont(bool UseChannelComp = false)
        {
            var FontInfo = FontSection;
            var TextureGlyph = FontInfo.TextureGlyph;

            BitmapFont f = new BitmapFont();
            f.LineHeight = FontInfo.LineFeed;
            Bitmap[] Chars = new Bitmap[TextureGlyph.ColumnCount * TextureGlyph.RowCount * TextureGlyph.SheetCount];

            float realcellwidth = TextureGlyph.CellWidth + 1;
            float realcellheight = TextureGlyph.CellHeight + 1;

            int j = 0;
            for (int sheet = 0; sheet < TextureGlyph.SheetCount; sheet++)
            {
                Bitmap SheetBM = TextureGlyph.GetImageSheet(sheet).GetBitmap();

                if (UseChannelComp)
                    SheetBM = TextureGlyph.GetImageSheet(sheet).GetComponentBitmap(SheetBM, true);

                if (Platform >= PlatformType.Cafe)
                    SheetBM.RotateFlip(RotateFlipType.RotateNoneFlipY);
                BitmapData bd = SheetBM.LockBits(new Rectangle(0, 0, SheetBM.Width, SheetBM.Height),
                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                for (int y = 0; y < TextureGlyph.ColumnCount; y++)
                {
                    for (int x = 0; x < TextureGlyph.RowCount; x++)
                    {
                        Bitmap b = new Bitmap(TextureGlyph.CellWidth, TextureGlyph.CellHeight);
                        BitmapData bd2 = b.LockBits(new Rectangle(0, 0, b.Width, b.Height),
                            ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

                        for (int y2 = 0; y2 < TextureGlyph.CellHeight; y2++)
                        {
                            for (int x2 = 0; x2 < TextureGlyph.CellWidth; x2++)
                            {
                                Marshal.WriteInt32(bd2.Scan0, y2 * bd2.Stride + x2 * 4,
                                    Marshal.ReadInt32(bd.Scan0, (int)(y * realcellheight + y2 + 1) *
                                    bd.Stride + (int)(x * realcellwidth + x2 + 1) * 4));
                            }
                        }
                        b.UnlockBits(bd2);
                        Chars[j++] = b;
                    }
                }
                SheetBM.UnlockBits(bd);
            }

            foreach (var charMap in FontInfo.CodeMapDictionary)
            {
                var idx = charMap.Value;
                if (idx == 0xFFFF) continue;
                var info = GetCharWidthInfoByIndex(FontInfo, (ushort)idx);

                f.Characters.Add(charMap.Key, new BitmapFont.Character(Chars[idx], info.Left, info.GlyphWidth, info.CharWidth));
            }

            return f;
        }

        private CharacterWidthEntry GetCharWidthInfoByIndex(FINF fontInfo, UInt16 Index)
        {
            foreach (var v in fontInfo.CharacterWidths)
            {
                if (Index < v.StartIndex || Index > v.EndIndex) continue;
                return v.WidthEntries[Index - v.StartIndex];
            }
            return null;
        }
    }
}
