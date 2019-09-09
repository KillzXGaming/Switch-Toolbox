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
using System.ComponentModel;
using Syroot.Maths;
using SharpYaml.Serialization;
using FirstPlugin;
using static Toolbox.Library.IO.Bit;

namespace LayoutBXLYT
{
    public class BCLYT : IFileFormat, IEditorForm<LayoutEditor>, IConvertableTextFormat
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "CTR Layout (GUI)" };
        public string[] Extension { get; set; } = new string[] { "*.bclyt" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "CLYT");
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
            editor.LoadBxlyt(header);
            return editor;
        }

        public void FillEditor(Form control) {
            ((LayoutEditor)control).LoadBxlyt(header);
        }

        public Header header;
        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            header = new Header();
            header.Read(new FileReader(stream), this);
        }

        public List<BCLYT> GetLayouts()
        {
            List<BCLYT> animations = new List<BCLYT>();
            if (IFileInfo.ArchiveParent != null)
            {
                foreach (var file in IFileInfo.ArchiveParent.Files)
                {
                    if (Utils.GetExtension(file.FileName) == ".bclyt")
                    {
                        BCLYT bclyt = (BCLYT)file.OpenFile();
                        animations.Add(bclyt);
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
                    if (Utils.GetExtension(file.FileName) == ".bclim")
                    {
                        BCLIM bclim = (BCLIM)file.OpenFile();
                        file.FileFormat = bclim;
                        textures.Add(bclim.FileName, bclim);
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
            private const string Magic = "CLYT";

            private ushort ByteOrderMark;
            private ushort HeaderSize;

            public LYT1 LayoutInfo { get; set; }
            public TXL1 TextureList { get; set; }
            public MAT1 MaterialList { get; set; }
            public FNL1 FontList { get; set; }
            //   private List<SectionCommon> Sections;
            //  public List<PAN1> Panes = new List<PAN1>();

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

            public override List<string> Fonts
            {
                get { return FontList.Fonts; }
            }

            public override Dictionary<string, STGenericTexture> GetTextures
            {
                get { return ((BCLYT)FileInfo).GetTextures(); }
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

            public override List<BxlytMaterial> GetMaterials()
            {
                List<BxlytMaterial> materials = new List<BxlytMaterial>();
                if (MaterialList != null && MaterialList.Materials != null)
                {
                    for (int i = 0; i < MaterialList.Materials.Count; i++)
                        materials.Add(MaterialList.Materials[i]);
                }
                return materials;
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

            public void Read(FileReader reader, BCLYT bclyt)
            {
                IsBigEndian = reader.ByteOrder == Syroot.BinaryData.ByteOrder.BigEndian;

                LayoutInfo = new LYT1();
                TextureList = new TXL1();
                MaterialList = new MAT1();
                FontList = new FNL1();
                RootPane = new PAN1();
                RootGroup = new GRP1();

                FileInfo = bclyt;

                reader.SetByteOrder(true);
                reader.ReadSignature(4, Magic);
                ByteOrderMark = reader.ReadUInt16();
                reader.CheckByteOrderMark(ByteOrderMark);
                HeaderSize = reader.ReadUInt16();
                Version = reader.ReadUInt32();
                uint FileSize = reader.ReadUInt32();
                ushort sectionCount = reader.ReadUInt16();
                reader.ReadUInt16(); //Padding

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

                    SectionCommon section = new SectionCommon();
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
                            var textPanel = new TXT1(reader);

                            SetPane(textPanel, parentPane);
                            currentPane = textPanel;
                            break;
                        case "bnd1":
                            var boundsPanel = new BND1(reader);

                            SetPane(boundsPanel, parentPane);
                            currentPane = boundsPanel;
                            break;
                        case "prt1":
                            var partsPanel = new PRT1(reader);

                            SetPane(partsPanel, parentPane);
                            currentPane = partsPanel;
                            break;
                        case "wnd1":
                            var windowPanel = new WND1(reader);
                            SetPane(windowPanel, parentPane);
                            currentPane = windowPanel;
                            break;
                        case "cnt1":
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
                        case "usd1":
                            break;
                        //If the section is not supported store the raw bytes
                        default:
                            section.Data = reader.ReadBytes((int)SectionSize);
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

                writer.SetByteOrder(IsBigEndian);
                writer.WriteSignature(Magic);
                writer.Write(ByteOrderMark);
                writer.Write(HeaderSize);
                writer.Write(Version);
                writer.Write(uint.MaxValue); //Reserve space for file size later
                writer.Write(ushort.MaxValue); //Reserve space for section count later
                writer.Seek(2); //padding

                //Write the total file size
                using (writer.TemporarySeek(0x0C, System.IO.SeekOrigin.Begin))
                {
                    writer.Write((uint)writer.BaseStream.Length);
                }
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

            public TXT1(FileReader reader) : base(reader)
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

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                long pos = writer.Position;

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
            public WND1() : base()
            {

            }

            public WND1(FileReader reader) : base(reader)
            {

            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                base.Write(writer, header);
            }
        }

        public class BND1 : PAN1
        {
            public BND1() : base()
            {

            }

            public BND1(FileReader reader) : base(reader)
            {

            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                base.Write(writer, header);
            }
        }

        public class GRP1 : BasePane
        {
            public List<string> Panes { get; set; } = new List<string>();

            public GRP1() : base()
            {

            }

            public GRP1(FileReader reader, Header header)
            {
                Name = reader.ReadString(0x10, true);
                ushort numNodes = reader.ReadUInt16();
                reader.ReadUInt16();//padding

                for (int i = 0; i < numNodes; i++)
                    Panes.Add(reader.ReadString(0x10, true));
            }

            public override void Write(FileWriter writer, LayoutHeader header)
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
            public PRT1() : base()
            {

            }

            public PRT1(FileReader reader) : base(reader)
            {

            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                base.Write(writer, header);
            }
        }

        public class PIC1 : PAN1
        {
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

            private BCLYT.Header ParentLayout;

            public PIC1() : base() {
                ColorTopLeft = STColor8.White;
                ColorTopRight = STColor8.White;
                ColorBottomLeft = STColor8.White;
                ColorBottomRight = STColor8.White;
                MaterialIndex = 0;
                TexCoords = new TexCoord[1];
                TexCoords[0] = new TexCoord();
            }

            public PIC1(FileReader reader, BCLYT.Header header) : base(reader)
            {
                ParentLayout = header;

                ColorTopLeft = STColor8.FromBytes(reader.ReadBytes(4));
                ColorTopRight = STColor8.FromBytes(reader.ReadBytes(4));
                ColorBottomLeft = STColor8.FromBytes(reader.ReadBytes(4));
                ColorBottomRight = STColor8.FromBytes(reader.ReadBytes(4));
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

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                base.Write(writer, header);
                writer.Write(ColorTopLeft.ToBytes());
                writer.Write(ColorTopRight.ToBytes());
                writer.Write(ColorBottomLeft.ToBytes());
                writer.Write(ColorBottomRight.ToBytes());
                writer.Write(MaterialIndex);
                writer.Write(TexCoords != null ? TexCoords.Length : 0);
            }
        }

        public class PAN1 : BasePane
        {
            private byte _flags1;

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

            public override bool InfluenceAlpha
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

            public byte Origin { get; set; }

            public byte PartsScale { get; set; }

            public byte PaneMagFlags { get; set; }

            public string UserDataInfo { get; set; }

            public PAN1() : base()
            {

            }

            enum BCLYTOriginX : byte
            {
                Left = 0,
                Center = 1,
                Right = 2
            };

            enum BCLYTOriginY : byte
            {
                Top = 0,
                Center = 1,
                Bottom = 2
            };

            public override OriginX originX
            {
                get
                {
                    var bclytOriginX = (BCLYTOriginX)(Origin % 3);
                    if (bclytOriginX == BCLYTOriginX.Center)
                        return OriginX.Center;
                    else if (bclytOriginX == BCLYTOriginX.Left)
                        return OriginX.Left;
                    else
                        return OriginX.Right;
                }
            }

            public override OriginY originY
            {
                get
                {
                    var bclytOriginY = (BCLYTOriginY)(Origin / 3);
                    if (bclytOriginY == BCLYTOriginY.Center)
                        return OriginY.Center;
                    else if (bclytOriginY == BCLYTOriginY.Top)
                        return OriginY.Top;
                    else
                        return OriginY.Bottom;
                }
            }

            public PAN1(FileReader reader) : base()
            {
                _flags1 = reader.ReadByte();
                Origin = reader.ReadByte();
                Alpha = reader.ReadByte();
                PaneMagFlags = reader.ReadByte();
                Name = reader.ReadString(0x18, true);
                Translate = reader.ReadVec3SY();
                Rotate = reader.ReadVec3SY();
                Scale = reader.ReadVec2SY();
                Width = reader.ReadSingle();
                Height = reader.ReadSingle();
            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                writer.Write(_flags1);
                writer.Write(Alpha);
                writer.Write(PaneMagFlags);
                writer.WriteString(Name, 0x18);
                writer.Write(Translate);
                writer.Write(Rotate);
                writer.Write(Scale);
                writer.Write(Width);
                writer.Write(Height);
            }

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

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                writer.Write((ushort)Materials.Count);
                writer.Seek(2);
            }
        }

        public class Material : BxlytMaterial
        {
            public STColor8 TevColor { get; set; }
            public STColor8[] TevConstantColors { get; set; }

            public List<TextureRef> TextureMaps { get; set; }
            public List<TextureTransform> TextureTransforms { get; set; }

            private uint flags;

            private BCLYT.Header ParentLayout;
            public string GetTexture(int index)
            {
                if (TextureMaps[index].ID != -1)
                    return ParentLayout.TextureList.Textures[TextureMaps[index].ID];
                else
                    return "";
            }

            public Material()
            {
                TextureMaps = new List<TextureRef>();
                TextureTransforms = new List<TextureTransform>();
            }

            public Material(FileReader reader, Header header) : base()
            {
                ParentLayout = header;

                TextureMaps = new List<TextureRef>();
                TextureTransforms = new List<TextureTransform>();

                Name = reader.ReadString(20, true);
                TevColor = reader.ReadColor8RGBA();
                TevConstantColors = reader.ReadColor8sRGBA(6);
                flags = reader.ReadUInt32();

                uint texCount = flags & 3;
                uint mtxCount = (flags >> 2) & 3;
                uint texCoordGens = (flags >> 4) & 3;
                uint tevStages = (flags >> 6) & 7;

                for (int i = 0; i < texCount; i++)
                    TextureMaps.Add(new TextureRef(reader, header));

                for (int i = 0; i < mtxCount; i++)
                    TextureTransforms.Add(new TextureTransform(reader));
            }

            public void Write(FileWriter writer, Header header)
            {
                writer.WriteString(Name, 0x1C);
                writer.Write(TevColor);
                writer.Write(TevConstantColors);
                writer.Write(flags);

                for (int i = 0; i < TextureMaps.Count; i++)
                    TextureMaps[i].Write(writer);

                for (int i = 0; i < TextureTransforms.Count; i++)
                    TextureTransforms[i].Write(writer);
            }
        }

        public class TextureTransform
        {
            public Vector2F Translate;
            public float Rotate;
            public Vector2F Scale;

            public TextureTransform() { }

            public TextureTransform(FileReader reader)
            {
                Translate = reader.ReadVec2SY();
                Rotate = reader.ReadSingle();
                Scale = reader.ReadVec2SY();
            }

            public void Write(FileWriter writer)
            {
                writer.Write(Translate);
                writer.Write(Rotate);
                writer.Write(Scale);
            }
        }

        public class TextureRef : BxlytTextureRef
        {
            public short ID;
            byte flag1;
            byte flag2;

            public override WrapMode WrapModeU
            {
                get { return (WrapMode)(flag1 & 0x3); }
            }

            public override WrapMode WrapModeV
            {
                get { return (WrapMode)(flag2 & 0x3); }
            }

            public override FilterMode MinFilterMode
            {
                get { return (FilterMode)((flag1 >> 2) & 0x3); }
            }

            public override FilterMode MaxFilterMode
            {
                get { return (FilterMode)((flag2 >> 2) & 0x3); }
            }

            public TextureRef() {}

            public TextureRef(FileReader reader, Header header) {
                ID = reader.ReadInt16();
                flag1 = reader.ReadByte();
                flag2 = reader.ReadByte();

                if (header.Textures.Count > 0)
                    Name = header.Textures[ID];
            }

            public void Write(FileWriter writer)
            {
                writer.Write(ID);
                writer.Write(flag1);
                writer.Write(flag2);
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

            public override void Write(FileWriter writer, LayoutHeader header)
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

            public override void Write(FileWriter writer, LayoutHeader header)
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

            public float Width { get; set; }
            public float Height { get; set; }

            public LYT1()
            {
                DrawFromCenter = false;
                Width = 0;
                Height = 0;
            }

            public LYT1(FileReader reader)
            {
                DrawFromCenter = reader.ReadBoolean();
                reader.Seek(3); //padding
                Width = reader.ReadSingle();
                Height = reader.ReadSingle();
            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                writer.Write(DrawFromCenter);
                writer.Seek(3);
                writer.Write(Width);
                writer.Write(Height);
            }
        }
    }
}
