using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.Forms;
using Toolbox.Library.IO;
using FirstPlugin.Forms;
using Syroot.Maths;
using SharpYaml.Serialization;
using FirstPlugin;
using System.ComponentModel;

namespace LayoutBXLYT.Cafe
{
    public class BFLYT : IFileFormat, IEditorForm<LayoutEditor>, IConvertableTextFormat
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Cafe Layout (GUI)" };
        public string[] Extension { get; set; } = new string[] { "*.bflyt" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "FLYT");
            }
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                types.Add(typeof(MenuExt));
                return types.ToArray();
            }
        }

        class MenuExt : IFileMenuExtension
        {
            public STToolStripItem[] NewFileMenuExtensions => newFileExt;
            public STToolStripItem[] NewFromFileMenuExtensions => null;
            public STToolStripItem[] ToolsMenuExtensions => null;
            public STToolStripItem[] TitleBarExtensions => null;
            public STToolStripItem[] CompressionMenuExtensions => null;
            public STToolStripItem[] ExperimentalMenuExtensions => null;
            public STToolStripItem[] EditMenuExtensions => null;
            public ToolStripButton[] IconButtonMenuExtensions => null;

            STToolStripItem[] newFileExt = new STToolStripItem[1];

            public MenuExt()
            {
                newFileExt[0] = new STToolStripItem("Layout Editor");
                newFileExt[0].Click += LoadNewLayoutEditor;
            }

            private void LoadNewLayoutEditor(object sender, EventArgs e)
            {
                LayoutEditor editor = new LayoutEditor();
                editor.Show();
            }
        }

        #region Text Converter Interface
        public TextFileType TextFileType => TextFileType.Xml;
        public bool CanConvertBack => false;

        public string ConvertToString()
        {
            var serializerSettings = new SerializerSettings()
            {
                //  EmitTags = false
            };

            serializerSettings.DefaultStyle = SharpYaml.YamlStyle.Any;
            serializerSettings.ComparerForKeySorting = null;
            serializerSettings.RegisterTagMapping("Header", typeof(Header));

            return FLYT.ToXml(header);

            var serializer = new Serializer(serializerSettings);
            string yaml = serializer.Serialize(header, typeof(Header));
            return yaml;
        }

        public void ConvertFromString(string text)
        {
        }

        #endregion

        public LayoutEditor OpenForm()
        {
            LayoutEditor editor = new LayoutEditor();
            editor.Dock = DockStyle.Fill;
            editor.LoadBxlyt(header, FileName);
            return editor;
        }

        public void FillEditor(Form control) {
            ((LayoutEditor)control).LoadBxlyt(header, FileName);
        }

        public Header header;
        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            header = new Header();
            header.Read(new FileReader(stream), this);
        }

        public List<GTXFile> GetShadersGTX()
        {
            List<GTXFile> shaders = new List<GTXFile>();
            if (IFileInfo.ArchiveParent != null)
            {
                foreach (var file in IFileInfo.ArchiveParent.Files)
                {
                    if (Utils.GetExtension(file.FileName) == ".gsh")
                    {
                        GTXFile bnsh = (GTXFile)file.OpenFile();
                        shaders.Add(bnsh);
                    }
                }
            }
            return shaders;
        }

        public List<BNSH> GetShadersNX()
        {
            List<BNSH> shaders = new List<BNSH>();
            if (IFileInfo.ArchiveParent != null)
            {
                foreach (var file in IFileInfo.ArchiveParent.Files)
                {
                    if (Utils.GetExtension(file.FileName) == ".bnsh")
                    {
                        BNSH bnsh = (BNSH)file.OpenFile();
                        shaders.Add(bnsh);
                    }
                }
            }
            return shaders;
        }

        public List<BFLYT> GetLayouts()
        {
            List<BFLYT> animations = new List<BFLYT>();
            if (IFileInfo.ArchiveParent != null)
            {
                foreach (var file in IFileInfo.ArchiveParent.Files)
                {
                    if (Utils.GetExtension(file.FileName) == ".bflyt")
                    {
                        BFLYT bflyt = (BFLYT)file.OpenFile();
                        animations.Add(bflyt);
                    }
                }
            }
            return animations;
        }

        public List<BFLAN> GetAnimations()
        {
            List<BFLAN> animations = new List<BFLAN>();
            if (IFileInfo.ArchiveParent != null)
            {
                foreach (var file in IFileInfo.ArchiveParent.Files)
                {
                    if (Utils.GetExtension(file.FileName) == ".bflan")
                    {
                        BFLAN bflan = (BFLAN)file.OpenFile();
                        animations.Add(bflan);
                    }
                }
            }
            return animations;
        }

        public Dictionary<string, STGenericTexture> GetTextures()
        {
            Dictionary<string, STGenericTexture> textures = new Dictionary<string, STGenericTexture>();
            if (IFileInfo.ArchiveParent != null)
            {
                foreach (var file in IFileInfo.ArchiveParent.Files)
                {
                    if (Utils.GetExtension(file.FileName) == ".bntx")
                    {
                        BNTX bntx = (BNTX)file.OpenFile();
                        file.FileFormat = bntx;
                        foreach (var tex in bntx.Textures)
                            textures.Add(tex.Key, tex.Value);
                    }
                    else if (Utils.GetExtension(file.FileName) == ".bflim")
                    {
                        BFLIM bflim = (BFLIM)file.OpenFile();
                        file.FileFormat = bflim;
                        textures.Add(bflim.FileName, bflim);
                    }
                }
            }

            return textures;
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream) {
            header.Write(new FileWriter(stream));
        }

        //Thanks to SwitchThemes for flags, and enums
        //https://github.com/FuryBaguette/SwitchLayoutEditor/tree/master/SwitchThemesCommon
        public class Header : BxlytHeader, IDisposable
        {
            private const string Magic = "FLYT";

            private ushort ByteOrderMark;
            private ushort HeaderSize;

            public LYT1 LayoutInfo { get; set; }
            public TXL1 TextureList { get; set; }
            public MAT1 MaterialList { get; set; }
            public FNL1 FontList { get; set; }
            public CNT1 Container { get; set; }
            public USD1 UserData { get; set; }

            //As of now this should be empty but just for future proofing
            private List<SectionCommon> UnknownSections = new List<SectionCommon>();

            public int TotalPaneCount()
            {
                int panes = GetPanes().Count;
                int grpPanes = GetGroupPanes().Count;
                return panes + grpPanes;
            }

            public override List<string> Textures
            {
                get { return TextureList.Textures; }
            }

            public override Dictionary<string, STGenericTexture> GetTextures
            {
                get { return ((BFLYT)FileInfo).GetTextures(); }
            }

            public List<PAN1> GetPanes()
            {
                List<PAN1> panes = new List<PAN1>();
                GetPaneChildren(panes, (PAN1)RootPane);
                return panes;
            }

            public List<GRP1> GetGroupPanes()
            {
                List<GRP1> panes = new List<GRP1>();
                GetGroupChildren(panes, (GRP1)RootGroup);
                return panes;
            }

            private void GetPaneChildren(List<PAN1> panes, PAN1 root)
            {
                panes.Add(root);
                foreach (var pane in root.Childern)
                    GetPaneChildren(panes, (PAN1)pane);
            }

            private void GetGroupChildren(List<GRP1> panes, GRP1 root)
            {
                panes.Add(root);
                foreach (var pane in root.Childern)
                    GetGroupChildren(panes, (GRP1)pane);
            }

            public void Read(FileReader reader, BFLYT bflyt)
            {
                LayoutInfo = new LYT1();
                TextureList = new TXL1();
                MaterialList = new MAT1();
                FontList = new FNL1();
                RootPane = new PAN1();
                RootGroup = new GRP1();

                FileInfo = bflyt;

                reader.SetByteOrder(true);
                reader.ReadSignature(4, Magic);
                ByteOrderMark = reader.ReadUInt16();
                reader.CheckByteOrderMark(ByteOrderMark);
                HeaderSize = reader.ReadUInt16();
                Version = reader.ReadUInt32();
                uint FileSize = reader.ReadUInt32();
                ushort sectionCount = reader.ReadUInt16();
                reader.ReadUInt16(); //Padding

                IsBigEndian = reader.ByteOrder == Syroot.BinaryData.ByteOrder.BigEndian;

                bool setRoot = false;
                bool setGroupRoot = false;

                BasePane currentPane = null;
                BasePane parentPane = null;

                BasePane currentGroupPane = null;
                BasePane parentGroupPane = null;

                reader.SeekBegin(HeaderSize);
                for (int i = 0; i < sectionCount; i++)
                {
                    long pos = reader.Position;

                    string Signature = reader.ReadString(4, Encoding.ASCII);
                    uint SectionSize = reader.ReadUInt32();

                    SectionCommon section = new SectionCommon(Signature);
                    switch (Signature)
                    {
                        case "lyt1":
                            LayoutInfo = new LYT1(reader);
                            break;
                        case "txl1":
                            TextureList = new TXL1(reader);
                            break;
                        case "fnl1":
                            FontList = new FNL1(reader);
                            break;
                        case "mat1":
                            MaterialList = new MAT1(reader, this);
                            break;
                        case "pan1":
                            var panel = new PAN1(reader);
                            if (!setRoot)
                            {
                                RootPane = panel;
                                setRoot = true;
                            }

                            SetPane(panel, parentPane);
                            currentPane = panel;
                            break;
                        case "pic1":
                            var picturePanel = new PIC1(reader, this);

                            SetPane(picturePanel, parentPane);
                            currentPane = picturePanel;
                            break;
                        case "txt1":
                            var textPanel = new TXT1(reader, this);

                            SetPane(textPanel, parentPane);
                            currentPane = textPanel;
                            break;
                        case "bnd1":
                            var boundsPanel = new BND1(reader, this);

                            SetPane(boundsPanel, parentPane);
                            currentPane = boundsPanel;
                            break;
                        case "prt1":
                            var partsPanel = new PRT1(reader, this);

                            SetPane(partsPanel, parentPane);
                            currentPane = partsPanel;
                            break;
                        case "wnd1":
                            var windowPanel = new WND1(reader, this);
                            SetPane(windowPanel, parentPane);
                            currentPane = windowPanel;
                            break;
                        case "pas1":
                            if (currentPane != null)
                                parentPane = currentPane;
                            break;
                        case "pae1":
                            currentPane = parentPane;
                            parentPane = currentPane.Parent;
                            break;
                        case "grp1":
                            var groupPanel = new GRP1(reader, this);

                            if (!setGroupRoot)
                            {
                                RootGroup = groupPanel;
                                setGroupRoot = true;
                            }

                            SetPane(groupPanel, parentGroupPane);
                            currentGroupPane = groupPanel;
                            break;
                        case "grs1":
                            if (currentGroupPane != null)
                                parentGroupPane = currentGroupPane;
                            break;
                        case "gre1":
                            currentGroupPane = parentGroupPane;
                            parentGroupPane = currentGroupPane.Parent;
                            break;
                     /*   case "cnt1":
                            Container = new CNT1(reader, this);
                            break;*/
                        case "usd1":
                            UserData = new USD1(reader, this);
                            break;
                        //If the section is not supported store the raw bytes
                        default:
                            section.Data = reader.ReadBytes((int)SectionSize - 8);
                            UnknownSections.Add(section);
                            break;
                    }

                    section.SectionSize = SectionSize;
                    reader.SeekBegin(pos + SectionSize);
                }
            }

            private void SetPane(BasePane pane, BasePane parentPane)
            {
                if (parentPane != null)
                {
                    parentPane.Childern.Add(pane);
                    pane.Parent = parentPane;
                }
            }

            public void Write(FileWriter writer)
            {
                Version = VersionMajor << 24 | VersionMinor << 16 | VersionMicro << 8 | VersionMicro2;

                writer.SetByteOrder(true);
                writer.WriteSignature(Magic);
                writer.Write(ByteOrderMark);
                writer.SetByteOrder(IsBigEndian);
                writer.Write(HeaderSize);
                writer.Write(Version);
                writer.Write(uint.MaxValue); //Reserve space for file size later
                writer.Write(ushort.MaxValue); //Reserve space for section count later
                writer.Seek(2); //padding

                int sectionCount = 1;

                WriteSection(writer, "lyt1", LayoutInfo,() => LayoutInfo.Write(writer, this));

                if (TextureList != null && TextureList.Textures.Count > 0)
                {
                    WriteSection(writer, "txl1", TextureList,() => TextureList.Write(writer, this));
                    sectionCount++;
                }
                if (FontList != null && FontList.Fonts.Count > 0)
                {
                    WriteSection(writer, "fnl1", FontList,() => FontList.Write(writer, this));
                    sectionCount++;
                }
                if (MaterialList != null && MaterialList.Materials.Count > 0)
                {
                    WriteSection(writer, "mat1", MaterialList,() => MaterialList.Write(writer, this));
                    sectionCount++;
                }

                WritePanes(writer, RootPane, this, ref sectionCount);
                WriteGroupPanes(writer, RootGroup, this, ref sectionCount);

                if (UserData != null)
                {
                    WriteSection(writer, "usd1", UserData, () => UserData.Write(writer, this));
                    sectionCount++;
                }

                foreach (var section in UnknownSections)
                {
                    WriteSection(writer, section.Signature, section, () => section.Write(writer, this));
                    sectionCount++;
                }

                //Write the total section count
                using (writer.TemporarySeek(0x10, System.IO.SeekOrigin.Begin))
                {
                    writer.Write((ushort)sectionCount);
                }

                //Write the total file size
                using (writer.TemporarySeek(0x0C, System.IO.SeekOrigin.Begin))
                {
                    writer.Write((uint)writer.BaseStream.Length);
                }
            }

            private void WritePanes(FileWriter writer, BasePane pane, BxlytHeader header, ref int sectionCount)
            {
                WriteSection(writer, pane.Signature, pane,() => pane.Write(writer, header));
                sectionCount++;

                if (pane.HasChildern)
                {
                    sectionCount += 2;

                    //Write start of children section
                    WriteSection(writer, "pas1", null);

                    foreach (var child in pane.Childern)
                        WritePanes(writer, child, header, ref sectionCount);

                    //Write pae1 of children section
                    WriteSection(writer, "pae1", null);
                }
            }

            private void WriteGroupPanes(FileWriter writer, BasePane pane, BxlytHeader header, ref int sectionCount)
            {
                WriteSection(writer, pane.Signature, pane, () => pane.Write(writer, header));
                sectionCount++;

                if (pane.HasChildern)
                {
                    sectionCount += 2;

                    //Write start of children section
                    WriteSection(writer, "grs1", null);

                    foreach (var child in pane.Childern)
                        WriteGroupPanes(writer, child, header, ref sectionCount);

                    //Write pae1 of children section
                    WriteSection(writer, "gre1", null);
                }
            }

            private void WriteSection(FileWriter writer, string magic, SectionCommon section, Action WriteMethod = null)
            {
                long startPos = writer.Position;
                writer.WriteSignature(magic);
                writer.Write(uint.MaxValue);
                WriteMethod?.Invoke();

                long endPos = writer.Position;

                using (writer.TemporarySeek(startPos + 4, System.IO.SeekOrigin.Begin))
                {
                    writer.Write((uint)(endPos - startPos));
                }

            }
        }

        public class CNT1 : SectionCommon
        {
            public CNT1(FileReader reader, Header header)
            {

            }

            public override void Write(FileWriter writer, BxlytHeader header)
            {

            }
        }

        public class TexCoord
        {
            public Vector2F TopLeft { get; set; }
            public Vector2F TopRight { get; set; }
            public Vector2F BottomLeft { get; set; }
            public Vector2F BottomRight { get; set; }

            public TexCoord()
            {
                TopLeft = new Vector2F(0, 0);
				TopRight = new Vector2F(1, 0);
				BottomLeft = new Vector2F(0, 1);
				BottomRight = new Vector2F(1, 1);
            }
        }

        public class TXT1 : PAN1
        {
            public override string Signature { get; } = "txt1";

            public TXT1() : base()
            {
         
            }

            public string Text { get; set; }

            public OriginX HorizontalAlignment
            {
                get { return (OriginX)((TextAlignment >> 2) & 0x3); }
                set
                {
                    TextAlignment &= unchecked((byte)(~0xC));
                    TextAlignment |= (byte)((byte)(value) << 2);
                }
            }

            public OriginX VerticalAlignment
            {
                get { return (OriginX)((TextAlignment) & 0x3); }
                set
                {
                    TextAlignment &= unchecked((byte)(~0x3));
                    TextAlignment |= (byte)(value);
                }
            }

            public ushort TextLength { get; set; }
            public ushort MaxTextLength { get; set; }
            public ushort MaterialIndex { get; set; }
            public ushort FontIndex { get; set; }

            public byte TextAlignment { get; set; }
            public LineAlign LineAlignment { get; set; }
        
            public float ItalicTilt { get; set; }

            public STColor8 FontForeColor { get; set; }
            public STColor8 FontBackColor { get; set; }
            public Vector2F FontSize { get; set; }

            public float CharacterSpace { get; set; }
            public float LineSpace { get; set; }

            public Vector2F ShadowXY { get; set; }
            public Vector2F ShadowXYSize { get; set; }

            public STColor8 ShadowForeColor { get; set; }
            public STColor8 ShadowBackColor { get; set; }

            public float ShadowItalic { get; set; }

            public bool PerCharTransform
            {
                get { return (_flags & 0x10) != 0; }
                set { _flags = value ? (byte)(_flags | 0x10) : unchecked((byte)(_flags & (~0x10))); }
            }
            public bool RestrictedTextLengthEnabled
            {
                get { return (_flags & 0x2) != 0; }
                set { _flags = value ? (byte)(_flags | 0x2) : unchecked((byte)(_flags & (~0x2))); }
            }
            public bool ShadowEnabled
            {
                get { return (_flags & 1) != 0; }
                set { _flags = value ? (byte)(_flags | 1) : unchecked((byte)(_flags & (~1))); }
            }

            private byte _flags;

            public TXT1(FileReader reader, Header header) : base(reader)
            {
                TextLength = reader.ReadUInt16();
                MaxTextLength = reader.ReadUInt16();
                MaterialIndex = reader.ReadUInt16();
                FontIndex = reader.ReadUInt16();
                TextAlignment = reader.ReadByte();
                LineAlignment = (LineAlign)reader.ReadByte();
                _flags = reader.ReadByte();
                reader.Seek(1); //padding
                ItalicTilt = reader.ReadSingle();
                uint textOffset = reader.ReadUInt32();
                FontForeColor = STColor8.FromBytes(reader.ReadBytes(4));
                FontBackColor = STColor8.FromBytes(reader.ReadBytes(4));
                FontSize = reader.ReadVec2SY();
                CharacterSpace = reader.ReadSingle();
                LineSpace = reader.ReadSingle();
                ShadowXY = reader.ReadVec2SY();
                ShadowXYSize = reader.ReadVec2SY();
                ShadowForeColor = STColor8.FromBytes(reader.ReadBytes(4));
                ShadowBackColor = STColor8.FromBytes(reader.ReadBytes(4));
                ShadowItalic = reader.ReadSingle();

                if (RestrictedTextLengthEnabled)
                    Text = reader.ReadString(MaxTextLength);
                else
                    Text = reader.ReadString(TextLength);
            }

            public override void Write(FileWriter writer, BxlytHeader header)
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
                writer.Write(ItalicTilt);
                long _ofsTextPos = writer.Position;
                writer.Write(0); //text offset
                writer.Write(FontForeColor.ToBytes());
                writer.Write(FontBackColor.ToBytes());
                writer.Write(FontSize);
                writer.Write(CharacterSpace);
                writer.Write(LineSpace);
                writer.Write(ShadowXY);
                writer.Write(ShadowXYSize);
                writer.Write(ShadowForeColor.ToBytes());
                writer.Write(ShadowBackColor.ToBytes());
                writer.Write(ShadowItalic);

                writer.WriteUint32Offset(_ofsTextPos, pos);
                if (RestrictedTextLengthEnabled)
                    writer.WriteString(Text, MaxTextLength);
                else
                    writer.WriteString(Text, TextLength);
            }

            public enum BorderType : byte
            {
                Standard = 0,
                DeleteBorder = 1,
                RenderTwoCycles = 2,
            };

            public enum LineAlign : byte
            {
                Unspecified = 0,
                Left = 1,
                Center = 2,
                Right = 3,
            };
        }

        public class WND1 : PAN1
        {
            public override string Signature { get; } = "wnd1";

            public WND1() : base()
            {

            }

            public ushort StretchLeft;
            public ushort StretchRight;
            public ushort StretchTop;
            public ushort StretchBottm;
            public ushort FrameElementLeft;
            public ushort FrameElementRight;
            public ushort FrameElementTop;
            public ushort FrameElementBottm;
            public byte FrameCount;
            private byte _flag;


            public WindowContent Content { get; set; }

            public List<WindowFrame> WindowFrames = new List<WindowFrame>();

            public WND1(FileReader reader, Header header) : base(reader)
            {
                long pos = reader.Position - 0x54;

                StretchLeft = reader.ReadUInt16();
                StretchRight = reader.ReadUInt16();
                StretchTop = reader.ReadUInt16();
                StretchBottm = reader.ReadUInt16();
                FrameElementLeft = reader.ReadUInt16();
                FrameElementRight = reader.ReadUInt16();
                FrameElementTop = reader.ReadUInt16();
                FrameElementBottm = reader.ReadUInt16();
                FrameCount = reader.ReadByte();
                _flag = reader.ReadByte();
                reader.ReadUInt16();//padding
                uint contentOffset = reader.ReadUInt32();
                uint frameOffsetTbl = reader.ReadUInt32();

                reader.SeekBegin(pos + contentOffset);
                Content = new WindowContent(reader);

                reader.SeekBegin(pos + frameOffsetTbl);

                var offsets = reader.ReadUInt32s(FrameCount);
                foreach (int offset in offsets)
                {
                    reader.SeekBegin(pos + offset);
                    WindowFrames.Add(new WindowFrame(reader));
                }
            }

            public override void Write(FileWriter writer, BxlytHeader header)
            {
                long pos = writer.Position - 8;

                base.Write(writer, header);
                writer.Write(StretchLeft);
                writer.Write(StretchRight);
                writer.Write(StretchTop);
                writer.Write(StretchBottm);
                writer.Write(FrameElementLeft);
                writer.Write(FrameElementRight);
                writer.Write(FrameElementTop);
                writer.Write(FrameElementBottm);
                writer.Write(FrameCount);
                writer.Write(_flag);
                writer.Write((ushort)0);

                long _ofsContentPos = writer.Position;
                writer.Write(0);
                writer.Write(0);

                writer.WriteUint32Offset(_ofsContentPos, pos);
                Content.Write(writer);

                if (WindowFrames.Count > 0)
                {
                    writer.WriteUint32Offset(_ofsContentPos + 4, pos);
                    //Reserve space for frame offsets
                    long _ofsFramePos = writer.Position;
                    writer.Write(new uint[WindowFrames.Count]);
                    for (int i = 0; i < WindowFrames.Count; i++)
                    {
                        writer.WriteUint32Offset(_ofsFramePos + (i * 4), pos);
                        WindowFrames[i].Write(writer);
                    }
                }
            }

            public class WindowContent
            {
                public STColor8 TopLeftColor { get; set; }
                public STColor8 TopRightColor { get; set; }
                public STColor8 BottomLeftColor { get; set; }
                public STColor8 BottomRightColor { get; set; }

                public ushort MaterialIndex { get; set; }

                public List<TexCoord> TexCoords = new List<TexCoord>();

                public WindowContent(FileReader reader)
                {
                    TopLeftColor = reader.ReadColor8RGBA();
                    TopRightColor = reader.ReadColor8RGBA();
                    BottomLeftColor = reader.ReadColor8RGBA();
                    BottomRightColor = reader.ReadColor8RGBA();
                    MaterialIndex = reader.ReadUInt16();
                    byte UVCount = reader.ReadByte();
                    reader.ReadByte(); //padding

                    for (int i = 0; i < UVCount; i++)
                        TexCoords.Add(new TexCoord()
                        {
                            TopLeft = reader.ReadVec2SY(),
                            TopRight = reader.ReadVec2SY(),
                            BottomLeft = reader.ReadVec2SY(),
                            BottomRight = reader.ReadVec2SY(),
                        });
                }

                public void Write(FileWriter writer)
                {
                    writer.Write(TopLeftColor);
                    writer.Write(TopRightColor);
                    writer.Write(BottomLeftColor);
                    writer.Write(BottomRightColor);
                    writer.Write(MaterialIndex);
                    writer.Write((byte)TexCoords.Count);
                    writer.Write((byte)0);
                    foreach (var texCoord in TexCoords)
                    {
                        writer.Write(texCoord.TopLeft);
                        writer.Write(texCoord.TopRight);
                        writer.Write(texCoord.BottomLeft);
                        writer.Write(texCoord.BottomRight);
                    }
                }
            }

            public class WindowFrame
            {
                public ushort MaterialIndex;
                public byte Flip;

                public WindowFrame(FileReader reader)
                {
                    MaterialIndex = reader.ReadUInt16();
                    Flip = reader.ReadByte();
                    reader.ReadByte(); //padding
                }

                public void Write(FileWriter writer)
                {
                    writer.Write(MaterialIndex);
                    writer.Write(Flip);
                    writer.Write((byte)0);
                }
            }
        }

        public class BND1 : PAN1
        {
            public override string Signature { get; } = "bnd1";

            public BND1() : base()
            {

            }

            public BND1(FileReader reader, Header header) : base(reader)
            {

            }

            public override void Write(FileWriter writer, BxlytHeader header)
            {
                base.Write(writer, header);
            }
        }

        public class GRP1 : BasePane
        {
            public override string Signature { get; } = "grp1";
 
            public List<string> Panes { get; set; } = new List<string>();

            public GRP1() : base()
            {

            }

            public GRP1(FileReader reader, Header header)
            {
                ushort numNodes = 0;
                if (header.VersionMajor >= 5)
                {
                    Name = reader.ReadString(34).Replace("\0", string.Empty);
                    numNodes = reader.ReadUInt16();
                }
                else
                {
                    Name = reader.ReadString(24).Replace("\0", string.Empty);
                    numNodes = reader.ReadUInt16();
                    reader.Seek(2); //padding
                }

                for (int i = 0; i < numNodes; i++)
                    Panes.Add(reader.ReadString(24));
            }

            public override void Write(FileWriter writer, BxlytHeader header)
            {
                if (header.Version >= 0x05020000)
                {
                    writer.WriteString(Name, 34);
                    writer.Write((ushort)Panes.Count);
                }
                else
                {
                    writer.WriteString(Name, 24);
                    writer.Write((ushort)Panes.Count);
                    writer.Seek(2);
                }

                for (int i = 0; i < Panes.Count; i++)
                    writer.WriteString(Panes[i], 24);
            }
        }

        public class PRT1 : PAN1
        {
            public override string Signature { get; } = "prt1";

            public PRT1() : base()
            {

            }

            public float MagnifyX { get; set; }
            public float MagnifyY { get; set; }

            public List<PartProperty> Properties = new List<PartProperty>();

            public PRT1(FileReader reader, Header header) : base(reader)
            {
                uint properyCount = reader.ReadUInt32();
                MagnifyX = reader.ReadSingle();
                MagnifyY = reader.ReadSingle();
                for (int i = 0; i < properyCount; i++)
                    Properties.Add(new PartProperty(reader, header, this));
            }

            public override void Write(FileWriter writer, BxlytHeader header)
            {
                long startPos = writer.Position;
                base.Write(writer, header);
                writer.Write(Properties.Count);
                writer.Write(MagnifyX);
                writer.Write(MagnifyY);

                for (int i = 0; i < Properties.Count; i++)
                    Properties[i].Write(writer, header, startPos);
            }
        }

        public class PartProperty
        {
            public string Name { get; set; }

            public byte UsageFlag { get; set; }
            public byte BasicUsageFlag { get; set; }
            public byte MaterialUsageFlag { get; set; }

            public BasePane Property { get; set; }

            public PartProperty(FileReader reader, Header header, PRT1 prt1)
            {
                Name = reader.ReadString(0x18).Replace("\0", string.Empty);
                UsageFlag = reader.ReadByte();
                BasicUsageFlag = reader.ReadByte();
                MaterialUsageFlag = reader.ReadByte();
                reader.ReadByte(); //padding
                uint propertyOffset = reader.ReadUInt32();
                uint userDataOffset = reader.ReadUInt32();
                uint panelInfoOffset = reader.ReadUInt32();

                if (propertyOffset != 0)
                {
                    reader.SeekBegin(prt1.StartPosition + propertyOffset);

                    long startPos = reader.Position;
                    string signtature = reader.ReadString(4, Encoding.ASCII);
                    uint sectionSize = reader.ReadUInt32();

                    switch (signtature)
                    {
                        case "pic1":
                            Property = new PIC1(reader,header);
                            break;
                        case "txt1":
                            Property = new TXT1(reader, header);
                            break;
                        case "wnd1":
                            Property = new WND1(reader, header);
                            break;
                        case "bnd1":
                            Property = new BND1(reader, header);
                            break;
                        case "prt1":
                            Property = new PRT1(reader, header);
                            break;
                    }
                }
                if (userDataOffset != 0)
                {
                    reader.SeekBegin(prt1.StartPosition + userDataOffset);

                }
                if (panelInfoOffset != 0)
                {
                    reader.SeekBegin(prt1.StartPosition + panelInfoOffset);

                }
            }

            public void Write(FileWriter writer, BxlytHeader header, long startPos)
            {
                writer.WriteString(Name, 0x18);
                writer.Write(UsageFlag);
                writer.Write(BasicUsageFlag);
                writer.Write(MaterialUsageFlag);
                writer.Write((byte)0);
                //Reserve offset spaces
                long _ofsPos = writer.Position;
                writer.Write(0); //Property Offset
                writer.Write(0); //External User Data
                writer.Write(0); //Panel Info Offset

                if (Property != null)
                {
                    writer.WriteUint32Offset(_ofsPos);
                    Property.Write(writer, header);
                }
            }
        }

        public class USD1 : SectionCommon
        {
            public List<UserDataEntry> Entries = new List<UserDataEntry>();

            public USD1(FileReader reader, Header header)
            {
                long startPos = reader.Position - 8;

                ushort numEntries = reader.ReadUInt16();
                reader.ReadUInt16(); //padding

                for (int i = 0; i < numEntries; i++)
                    Entries.Add(new UserDataEntry(reader, startPos, header));
            }

            public override void Write(FileWriter writer, BxlytHeader header)
            {
                long startPos = writer.Position - 8;

                writer.Write((ushort)Entries.Count);
                writer.Write((ushort)0);

                for (int i = 0; i < Entries.Count; i++)
                    Entries[i].Write(writer, startPos, header);
            }
        }

        public class UserDataEntry
        {
            public string Name { get; set; }
            public DataType Type { get; private set; }
            public byte Unknown { get; set; }

            private object data;

            public string GetString()
            {
                return (string)data;
            }

            public float[] GetFloats()
            {
                return (float[])data;
            }

            public int[] GetInts()
            {
                return (int[])data;
            }

            public void SetStrings(string[] value)
            {
                data = value;
                Type = DataType.String;
            }

            public void GetFloats(float [] value)
            {
                data = value;
                Type = DataType.Float;
            }

            public void GetInts(int[] value)
            {
                data = value;
                Type = DataType.Int;
            }

            public UserDataEntry(FileReader reader, long startPos, Header header)
            {
                long pos = reader.Position;

                uint nameOffset = reader.ReadUInt32();
                uint dataOffset = reader.ReadUInt32();
                uint dataLength = reader.ReadUInt16();
                Type = reader.ReadEnum<DataType>(false);
                Unknown = reader.ReadByte();

                reader.SeekBegin(pos + nameOffset);
                Name = reader.ReadZeroTerminatedString();

                reader.SeekBegin(pos + dataOffset);
                switch (Type)
                {
                    case DataType.String:
                        data = reader.ReadString((int)dataLength);
                        break;
                    case DataType.Int:
                        data = reader.ReadInt32s((int)dataLength);
                        break;
                    case DataType.Float:
                        data = reader.ReadSingles((int)dataLength);
                        break;
                }
            }

            public void Write(FileWriter writer, long startPos, BxlytHeader header)
            {
                long pos = writer.Position;

                writer.Write(0); //nameOffset
                writer.Write(0); //dataOffset
                writer.Write(GetDataLength());
                writer.Write(Type, false);
                writer.Write(Unknown);

                writer.WriteUint32Offset(pos + 4, pos);
                switch (Type)
                {
                    case DataType.String:
                        writer.Write((string)data);
                        break;
                    case DataType.Int:
                        writer.Write((int[])data);
                        break;
                    case DataType.Float:
                        writer.Write((float[])data);
                        break;
                }

                writer.WriteUint32Offset(pos, pos);
                writer.WriteString(Name);
                writer.Align(4);
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

            public enum DataType : byte
            {
                String,
                Int,
                Float,
            }
        }

        public class PIC1 : PAN1
        {
            public override string Signature { get; } = "pic1";

            [DisplayName("Texture Coordinates"), CategoryAttribute("Texture")]
            public TexCoord[] TexCoords { get; set; }

            [DisplayName("Vertex Color (Top Left)"), CategoryAttribute("Color")]
            public STColor8 ColorTopLeft { get; set; }
            [DisplayName("Vertex Color (Top Right)"), CategoryAttribute("Color")]
            public STColor8 ColorTopRight { get; set; }
            [DisplayName("Vertex Color (Bottom Left)"), CategoryAttribute("Color")]
            public STColor8 ColorBottomLeft { get; set; }
            [DisplayName("Vertex Color (Bottom Right)"), CategoryAttribute("Color")]
            public STColor8 ColorBottomRight { get; set; }

            [Browsable(false)]
            public ushort MaterialIndex { get; set; }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public Material Material
            {
                get
                {
                    return ParentLayout.MaterialList.Materials[MaterialIndex];
                }
            }

            [Browsable(false)]
            public string GetTexture(int index)
            {
                return ParentLayout.TextureList.Textures[Material.TextureMaps[index].ID];
            }

            private BFLYT.Header ParentLayout;

            public PIC1() : base() {
                ColorTopLeft = STColor8.White;
                ColorTopRight = STColor8.White;
                ColorBottomLeft = STColor8.White;
                ColorBottomRight = STColor8.White;
                MaterialIndex = 0;
                TexCoords = new TexCoord[1];
                TexCoords[0] = new TexCoord();
            }

            public PIC1(FileReader reader, BFLYT.Header header) : base(reader)
            {
                ParentLayout = header;

                ColorTopLeft = reader.ReadColor8RGBA();
                ColorTopRight = reader.ReadColor8RGBA();
                ColorBottomLeft = reader.ReadColor8RGBA();
                ColorBottomRight = reader.ReadColor8RGBA();
                MaterialIndex = reader.ReadUInt16();
                byte numUVs = reader.ReadByte();
                reader.Seek(1); //padding

                TexCoords = new TexCoord[numUVs];
                for (int i = 0; i < numUVs; i++)
                {
                    TexCoords[i] = new TexCoord()
                    {
                        TopLeft = reader.ReadVec2SY(),
                        TopRight = reader.ReadVec2SY(),
                        BottomLeft = reader.ReadVec2SY(),
                        BottomRight = reader.ReadVec2SY(),
                    };
                }
            }

            public override void Write(FileWriter writer, BxlytHeader header)
            {
                base.Write(writer, header);
                writer.Write(ColorTopLeft);
                writer.Write(ColorTopRight);
                writer.Write(ColorBottomLeft);
                writer.Write(ColorBottomRight);
                writer.Write(MaterialIndex);
                writer.Write((byte)TexCoords.Length);
                writer.Write((byte)0);

                for (int i = 0; i < TexCoords.Length; i++)
                {
                    writer.Write(TexCoords[i].TopLeft);
                    writer.Write(TexCoords[i].TopRight);
                    writer.Write(TexCoords[i].BottomLeft);
                    writer.Write(TexCoords[i].BottomRight);
                }
            }
        }

        public class PAN1 : BasePane
        {
            public override string Signature { get; } = "pan1";

            private byte _flags1;
            private byte _flags2;

            [DisplayName("Is Visible"), CategoryAttribute("Flags")]
            public bool Visible
            {
                get { return (_flags1 & 0x1) == 0x1; }
                set {
                    if (value)
                        _flags1 |= 0x1;
                    else
                        _flags1 &= 0xFE; 
                }
            }

            [DisplayName("Influence Alpha"), CategoryAttribute("Alpha")]
            public bool InfluenceAlpha
            {
                get { return (_flags1 & 0x2) == 0x2; }
                set
                {
                    if (value)
                        _flags1 |= 0x2;
                    else
                        _flags1 &= 0xFD;
                }
            }

            [DisplayName("Alpha"), CategoryAttribute("Alpha")]
            public byte Alpha { get; set; }

            [DisplayName("Origin X"), CategoryAttribute("Origin")]
            public OriginX originX
            {
                get { return (OriginX)((_flags2 & 0xC0) >> 6); }
                set
                {
                    _flags2 &= unchecked((byte)(~0xC0));
                    _flags2 |= (byte)((byte)value << 6);
                }
            }

            [DisplayName("Origin Y"), CategoryAttribute("Origin")]
            public OriginY originY
            {
                get { return (OriginY)((_flags2 & 0x30) >> 4); }
                set
                {
                    _flags2 &= unchecked((byte)(~0x30));
                    _flags2 |= (byte)((byte)value << 4);
                }
            }

            [Browsable(false)]
            public OriginX ParentOriginX
            {
                get { return (OriginX)((_flags2 & 0xC) >> 2); }
                set
                {
                    _flags2 &= unchecked((byte)(~0xC));
                    _flags2 |= (byte)((byte)value << 2);
                }
            }

            [Browsable(false)]
            public OriginY ParentOriginY
            {
                get { return (OriginY)((_flags2 & 0x3)); }
                set
                {
                    _flags2 &= unchecked((byte)(~0x3));
                    _flags2 |= (byte)value;
                }
            }

            [DisplayName("Parts Flag"), CategoryAttribute("Flags")]
            public byte PaneMagFlags { get; set; }

            [DisplayName("User Data"), CategoryAttribute("User Data")]
            public string UserDataInfo { get; set; }

            public PAN1() : base()
            {

            }

            public PAN1(FileReader reader) : base()
            {
                _flags1 = reader.ReadByte();
                _flags2 = reader.ReadByte();
                Alpha = reader.ReadByte();
                PaneMagFlags = reader.ReadByte();
                Name = reader.ReadString(0x18).Replace("\0", string.Empty);
                UserDataInfo = reader.ReadString(0x8).Replace("\0", string.Empty);
                Translate = reader.ReadVec3SY();
                Rotate = reader.ReadVec3SY();
                Scale = reader.ReadVec2SY();
                Width = reader.ReadSingle();
                Height = reader.ReadSingle();
            }

            public override void Write(FileWriter writer, BxlytHeader header)
            {
                writer.Write(_flags1);
                writer.Write(_flags2);
                writer.Write(Alpha);
                writer.Write(PaneMagFlags);
                writer.WriteString(Name, 0x18);
                writer.WriteString(UserDataInfo, 0x8);
                writer.Write(Translate);
                writer.Write(Rotate);
                writer.Write(Scale);
                writer.Write(Width);
                writer.Write(Height);
            }

            [Browsable(false)]
            public bool ParentVisibility
            {
                get
                {
                    if (Scale.X == 0 || Scale.Y == 0)
                        return false;
                    if (!Visible)
                        return false;
                    if (Parent != null && Parent is PAN1)
                    {
                        return ((PAN1)Parent).ParentVisibility && Visible;
                    }
                    return true;
                }
            }
        }

        public class MAT1 : SectionCommon
        {
            public List<Material> Materials { get; set; }

            public MAT1() {
                Materials = new List<Material>();
            }

            public MAT1(FileReader reader, Header header) : base()
            {
                Materials = new List<Material>();

                long pos = reader.Position;

                ushort numMats = reader.ReadUInt16();
                reader.Seek(2); //padding

                uint[] offsets = reader.ReadUInt32s(numMats);
                for (int i = 0; i < numMats; i++)
                {
                    reader.SeekBegin(pos + offsets[i] - 8);
                    Materials.Add(new Material(reader, header));
                }
            }

            public override void Write(FileWriter writer, BxlytHeader header)
            {
                long pos = writer.Position - 8;

                writer.Write((ushort)Materials.Count);
                writer.Seek(2);

                long _ofsPos = writer.Position;
                //Fill empty spaces for offsets later
                writer.Write(new uint[Materials.Count]);

                //Save offsets and strings
                for (int i = 0; i < Materials.Count; i++)
                {
                    writer.WriteUint32Offset(_ofsPos + (i * 4), pos);
                    Materials[i].Write(writer, header);
                    writer.Align(4);
                }
            }
        }

        //Thanks to shibbs for the material info
        //https://github.com/shibbo/flyte/blob/master/flyte/lyt/common/MAT1.cs
        public class Material
        {
            [DisplayName("Name"), CategoryAttribute("General")]
            public string Name { get; set; }

            [DisplayName("Black Color"), CategoryAttribute("Color")]
            public STColor8 BlackColor { get; set; }

            [DisplayName("White Color"), CategoryAttribute("Color")]
            public STColor8 WhiteColor { get; set; }

            [DisplayName("Texture Maps"), CategoryAttribute("Texture")]
            public TextureRef[] TextureMaps { get; set; }

            [DisplayName("Texture Transforms"), CategoryAttribute("Texture")]
            public TextureTransform[] TextureTransforms { get; set; }

            [DisplayName("Texture Coordinate Params"), CategoryAttribute("Texture")]
            public TexCoordGen[] TexCoords { get; set; }

            [DisplayName("Tev Stages"), CategoryAttribute("Tev")]
            public TevStage[] TevStages { get; set; }

            [DisplayName("Alpha Compare"), CategoryAttribute("Alpha")]
            public AlphaCompare AlphaCompare { get; set; }

            [DisplayName("Blend Mode"), CategoryAttribute("Blend")]
            public BlendMode BlendMode { get; set; }

            [DisplayName("Blend Mode Logic"), CategoryAttribute("Blend")]
            public BlendMode BlendModeLogic { get; set; }

            [DisplayName("Indirect Parameter"), CategoryAttribute("Texture")]
            public IndirectParameter IndParameter { get; set; }

            [DisplayName("Projection Texture Coord Parameters"), CategoryAttribute("Texture")]
            public ProjectionTexGenParam[] ProjTexGenParams { get; set; }

            [DisplayName("Font Shadow Parameters"), CategoryAttribute("Font")]
            public FontShadowParameter FontShadowParameter { get; set; }

            private uint flags;
            private int unknown;

            private BFLYT.Header ParentLayout;
            public string GetTexture(int index)
            {
                if (TextureMaps[index].ID != -1)
                    return ParentLayout.TextureList.Textures[TextureMaps[index].ID];
                else
                    return "";
            }

            public Material()
            {
                TextureMaps = new TextureRef[0];
                TextureTransforms = new TextureTransform[0];
            }

            public Material(FileReader reader, Header header) : base()
            {
                ParentLayout = header;

                Name = reader.ReadString(0x1C).Replace("\0", string.Empty);
                if (header.VersionMajor >= 8)
                {
                    flags = reader.ReadUInt32();
                    unknown = reader.ReadInt32();
                    BlackColor = STColor8.FromBytes(reader.ReadBytes(4));
                    WhiteColor = STColor8.FromBytes(reader.ReadBytes(4));
                }
                else
                {
                    BlackColor = STColor8.FromBytes(reader.ReadBytes(4));
                    WhiteColor = STColor8.FromBytes(reader.ReadBytes(4));
                    flags = reader.ReadUInt32();
                }

                uint texCount = Convert.ToUInt32(flags & 3);
                uint mtxCount = Convert.ToUInt32(flags >> 2) & 3;
                uint texCoordGenCount = Convert.ToUInt32(flags >> 4) & 3;
                uint tevStageCount = Convert.ToUInt32(flags >> 6) & 0x7;
                var hasAlphaCompare = Convert.ToBoolean((flags >> 9) & 0x1);
                var hasBlendMode = Convert.ToBoolean((flags >> 10) & 0x1);
                var useTextureOnly = Convert.ToBoolean((flags >> 11) & 0x1);
                var seperateBlendMode = Convert.ToBoolean((flags >> 12) & 0x1);
                var hasIndParam = Convert.ToBoolean((flags >> 14) & 0x1);
                var projTexGenParamCount = Convert.ToUInt32((flags >> 15) & 0x3);
                var hasFontShadowParam = Convert.ToBoolean((flags >> 17) & 0x1);
                var thresholdingAlphaInterpolation = Convert.ToBoolean((flags >> 18) & 0x1);

                TextureMaps = new TextureRef[texCount];
                TextureTransforms = new TextureTransform[mtxCount];
                TexCoords = new TexCoordGen[texCoordGenCount];
                TevStages = new TevStage[tevStageCount];
                ProjTexGenParams = new ProjectionTexGenParam[projTexGenParamCount];

                for (int i = 0; i < texCount; i++)
                    TextureMaps[i] = new TextureRef(reader, header);

                for (int i = 0; i < mtxCount; i++)
                    TextureTransforms[i] = new TextureTransform(reader);

                for (int i = 0; i < texCoordGenCount; i++)
                    TexCoords[i] = new TexCoordGen(reader, header);

                for (int i = 0; i < tevStageCount; i++)
                    TevStages[i] = new TevStage(reader, header);

                if (hasAlphaCompare)
                    AlphaCompare = new AlphaCompare(reader, header);
                if (hasBlendMode)
                    BlendMode = new BlendMode(reader, header);
                if (seperateBlendMode)
                    BlendModeLogic = new BlendMode(reader, header);
                if (hasIndParam)
                    IndParameter = new IndirectParameter(reader, header);

                for (int i = 0; i < projTexGenParamCount; i++)
                    ProjTexGenParams[i] = new ProjectionTexGenParam(reader, header);

                if (hasFontShadowParam)
                    FontShadowParameter = new FontShadowParameter(reader, header);
            }

            public void Write(FileWriter writer, BxlytHeader header)
            {
                writer.WriteString(Name, 0x1C);
                if (header.VersionMajor >= 8)
                {
                    writer.Write(flags);
                    writer.Write(unknown);
                    writer.Write(BlackColor);
                    writer.Write(WhiteColor);
                }
                else
                {
                    writer.Write(BlackColor);
                    writer.Write(WhiteColor);
                    writer.Write(flags);
                }

                for (int i = 0; i < TextureMaps.Length; i++)
                    TextureMaps[i].Write(writer);

                for (int i = 0; i < TextureTransforms.Length; i++)
                    TextureTransforms[i].Write(writer);

                for (int i = 0; i < TexCoords.Length; i++)
                    TexCoords[i].Write(writer);

                for (int i = 0; i < TevStages.Length; i++)
                    TevStages[i].Write(writer);

                if (AlphaCompare != null)
                    AlphaCompare.Write(writer);
                if (BlendMode != null)
                    BlendMode.Write(writer);
                if (BlendModeLogic != null)
                    BlendModeLogic.Write(writer);
                if (IndParameter != null)
                    IndParameter.Write(writer);

                for (int i = 0; i < ProjTexGenParams.Length; i++)
                    ProjTexGenParams[i].Write(writer);

                if (FontShadowParameter != null)
                    FontShadowParameter.Write(writer);
            }
        }

        public class FNL1 : SectionCommon
        {
            public List<string> Fonts { get; set; }

            public FNL1()
            {
                Fonts = new List<string>();
            }

            public FNL1(FileReader reader) : base()
            {
                Fonts = new List<string>();

                ushort numFonts = reader.ReadUInt16();
                reader.Seek(2); //padding

                long pos = reader.Position;

                uint[] offsets = reader.ReadUInt32s(numFonts);
                for (int i = 0; i < offsets.Length; i++)
                {
                    reader.SeekBegin(offsets[i] + pos);
                }
            }

            public override void Write(FileWriter writer, BxlytHeader header)
            {
                writer.Write((ushort)Fonts.Count);
                writer.Seek(2);

                //Fill empty spaces for offsets later
                long pos = writer.Position;
                writer.Write(new uint[Fonts.Count]);

                //Save offsets and strings
                for (int i = 0; i < Fonts.Count; i++)
                {
                    writer.WriteUint32Offset(pos + (i * 4), pos);
                    writer.WriteString(Fonts[i]);
                }
            }
        }

        public class TXL1 : SectionCommon
        {
            public List<string> Textures { get; set; }

            public TXL1()
            {
                Textures = new List<string>();
            }

            public TXL1(FileReader reader) : base()
            {
                Textures = new List<string>();

                ushort numTextures = reader.ReadUInt16();
                reader.Seek(2); //padding

                long pos = reader.Position;

                uint[] offsets = reader.ReadUInt32s(numTextures);
                for (int i = 0; i < offsets.Length; i++)
                {
                    reader.SeekBegin(offsets[i] + pos);
                    Textures.Add(reader.ReadZeroTerminatedString());
                }
            }

            public override void Write(FileWriter writer, BxlytHeader header)
            {
                writer.Write((ushort)Textures.Count);
                writer.Seek(2);

                //Fill empty spaces for offsets later
                long pos = writer.Position;
                writer.Write(new uint[Textures.Count]);

                //Save offsets and strings
                for (int i = 0; i < Textures.Count; i++)
                {
                    writer.WriteUint32Offset(pos + (i * 4), pos);
                    writer.WriteString(Textures[i]);
                }
            }
        }

        public class LYT1 : SectionCommon
        {
            public bool DrawFromCenter { get; set; }

            [DisplayName("Canvas Width"), CategoryAttribute("Layout")]
            public float Width { get; set; }

            [DisplayName("Canvas Height"), CategoryAttribute("Layout")]
            public float Height { get; set; }

            [DisplayName("Max Parts Width"), CategoryAttribute("Layout")]
            public float MaxPartsWidth { get; set; }

            [DisplayName("Max Parts Height"), CategoryAttribute("Layout")]
            public float MaxPartsHeight { get; set; }

            [DisplayName("Layout Name"), CategoryAttribute("Layout")]
            public string Name { get; set; }

            public LYT1()
            {
                DrawFromCenter = false;
                Width = 0;
                Height = 0;
                MaxPartsWidth = 0;
                MaxPartsHeight = 0;
                Name = "";
            }

            public LYT1(FileReader reader)
            {
                DrawFromCenter = reader.ReadBoolean();
                reader.Seek(3); //padding
                Width = reader.ReadSingle();
                Height = reader.ReadSingle();
                MaxPartsWidth = reader.ReadSingle();
                MaxPartsHeight = reader.ReadSingle();
                Name = reader.ReadZeroTerminatedString();
            }

            public override void Write(FileWriter writer, BxlytHeader header)
            {
                writer.Write(DrawFromCenter);
                writer.Seek(3);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(MaxPartsWidth);
                writer.Write(MaxPartsHeight);
                writer.WriteString(Name);
                writer.Align(4);
            }
        }
    }
}
