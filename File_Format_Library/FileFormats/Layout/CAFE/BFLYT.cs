using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
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
    public class BFLYT : IFileFormat, IEditorForm<LayoutEditor>,
        IEditorFormParameters, IConvertableTextFormat
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Cafe Layout (GUI)" };
        public string[] Extension { get; set; } = new string[] { "*.bflyt" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool KeepOpen { get; } = true;
        public EventHandler OnSave { get; set; }

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
        public bool CanConvertBack => true;

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
            header = FLYT.FromXml(text);
            header.FileInfo = this;
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

                        //Disable saving unless the file gets edited
                        //Prevents broken files if version is unsupported
                        bflan.CanSave = false;
                        animations.Add(bflan);
                    }
                }
            }
            return animations;
        }

        public Dictionary<string, STGenericTexture> GetTextures()
        {
            Dictionary<string, STGenericTexture> textures = new Dictionary<string, STGenericTexture>();

            if (File.Exists(FilePath))
            {
                string folder = Path.GetDirectoryName(FilePath);
                foreach (var file in Directory.GetFiles(folder))
                {
                    if (Utils.GetExtension(file) == ".bflim")
                    {
                        BFLIM bflim = (BFLIM)STFileLoader.OpenFileFormat(file);
                        if (!textures.ContainsKey(bflim.FileName))
                            textures.Add(bflim.FileName, bflim);
                    }
                    if (Utils.GetExtension(file) == ".bntx")
                    {
                        BNTX bntx = (BNTX)STFileLoader.OpenFileFormat(file);
                        foreach (var tex in bntx.Textures)
                        {
                            if (!textures.ContainsKey(tex.Key))
                                textures.Add(tex.Key, tex.Value);
                        }

                        string fileName = Path.GetFileName(file);
                        if (!header.TextureManager.BinaryContainers.ContainsKey(fileName))
                            header.TextureManager.BinaryContainers.Add(fileName, bntx);
                    }
                }
            }

            foreach (var archive in PluginRuntime.SarcArchives)
            {
                foreach (var file in archive.Files)
                {
                    if (Utils.GetExtension(file.FileName) == ".bntx")
                    {
                        BNTX bntx = (BNTX)file.OpenFile();
                        file.FileFormat = bntx;
                        foreach (var tex in bntx.Textures)
                            if (!textures.ContainsKey(tex.Key))
                                textures.Add(tex.Key, tex.Value);

                        if (!header.TextureManager.BinaryContainers.ContainsKey($"{archive.FileName}.bntx"))
                            header.TextureManager.BinaryContainers.Add($"{archive.FileName}.bntx", bntx);
                    }
                    if (Utils.GetExtension(file.FileName) == ".bflim")
                    {
                        BFLIM bflim = (BFLIM)file.OpenFile();
                        file.FileFormat = bflim;
                        if (!textures.ContainsKey(bflim.FileName))
                            textures.Add(bflim.FileName, bflim);
                    }
                }

                if (!header.TextureManager.ArchiveFile.ContainsKey(archive.FileName))
                    header.TextureManager.ArchiveFile.Add(archive.FileName, archive);
            }

            return textures;
        }

        public void Unload()
        {
            if (header != null)
            {
            }
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

            [Browsable(false)]
            public LYT1 LayoutInfo { get; set; }
            [Browsable(false)]
            public TXL1 TextureList { get; set; }
            [Browsable(false)]
            public MAT1 MaterialList { get; set; }
            [Browsable(false)]
            public FNL1 FontList { get; set; }
            [Browsable(false)]
            public CNT1 Container { get; set; }

            public override short AddMaterial(BxlytMaterial material, ushort index)
            {
                if (material == null) return -1;

                if (MaterialList.Materials.Count > index)
                    MaterialList.Materials.Insert(index, (Material)material);
                else
                    MaterialList.Materials.Add((Material)material);

                if (material.NodeWrapper == null)
                    material.NodeWrapper = new MatWrapper(material.Name)
                    {
                        Tag = material,
                        ImageKey = "material",
                        SelectedImageKey = "material",
                    };

                MaterialFolder.Nodes.Insert(index, material.NodeWrapper);

                return (short)MaterialList.Materials.IndexOf((Material)material);
            }

            public override int AddFont(string name)
            {
                if (!FontList.Fonts.Contains(name))
                    FontList.Fonts.Add(name);

                return FontList.Fonts.IndexOf(name);
            }

            /// <summary>
            /// Adds the given texture if not found. Returns the index of the texture
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public override int AddTexture(string name)
            {
                if (!TextureList.Textures.Contains(name))
                    TextureList.Textures.Add(name);

                return TextureList.Textures.IndexOf(name);
            }

            public override void RemoveTexture(string name)
            {
                if (!TextureList.Textures.Contains(name))
                    TextureList.Textures.Remove(name);
            }

            public override short AddMaterial(BxlytMaterial material)
            {
                if (material == null) return -1;

                if (!MaterialList.Materials.Contains((Material)material))
                    MaterialList.Materials.Add((Material)material);

                if (material.NodeWrapper == null)
                    material.NodeWrapper = new MatWrapper(material.Name)
                    {
                        Tag = material,
                        ImageKey = "material",
                        SelectedImageKey = "material",
                    };

                if (!MaterialFolder.Nodes.Contains(material.NodeWrapper))
                MaterialFolder.Nodes.Add(material.NodeWrapper);

                return (short)MaterialList.Materials.IndexOf((Material)material);
            }

            public override List<int> AddMaterial(List<BxlytMaterial> materials)
            {
                List<int> indices = new List<int>();
                foreach (var material in materials)
                    indices.Add(AddMaterial(material));
                return indices;
            }

            public override void TryRemoveMaterial(BxlytMaterial material)
            {
                if (material == null) return;
                material.RemoveNodeWrapper();

                if (MaterialList.Materials.Contains((Material)material))
                    MaterialList.Materials.Remove((Material)material);
            }

            public override void TryRemoveMaterial(List<BxlytMaterial> materials)
            {
                foreach (var material in materials)
                {
                    if (material == null) continue;
                    material.RemoveNodeWrapper();

                    if (MaterialList.Materials.Contains((Material)material))
                        MaterialList.Materials.Remove((Material)material);
                }
            }

            //As of now this should be empty but just for future proofing
            private List<SectionCommon> UnknownSections = new List<SectionCommon>();

            [Browsable(false)]
            public int TotalPaneCount()
            {
                int panes = GetPanes().Count;
                int grpPanes = GetGroupPanes().Count;
                return panes + grpPanes;
            }

            [Browsable(false)]
            public override List<string> Textures
            {
                get { return TextureList.Textures; }
            }

            [Browsable(false)]
            public override List<string> Fonts
            {
                get { return FontList.Fonts; }
            }

            [Browsable(false)]
            public override Dictionary<string, STGenericTexture> GetTextures
            {
                get { return ((BFLYT)FileInfo).GetTextures(); }
            }

            [Browsable(false)]
            public List<PAN1> GetPanes()
            {
                List<PAN1> panes = new List<PAN1>();
                GetPaneChildren(panes, (PAN1)RootPane);
                return panes;
            }

            [Browsable(false)]
            public List<GRP1> GetGroupPanes()
            {
                List<GRP1> panes = new List<GRP1>();
                GetGroupChildren(panes, (GRP1)RootGroup);
                return panes;
            }

            public override BxlytMaterial GetMaterial(ushort index) {
                return MaterialList.Materials[index];
            }

            public override BxlytMaterial CreateNewMaterial(string name) {
                return new Material(name, this);
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

            public Header()
            {
                LayoutInfo = new LYT1();
                TextureList = new TXL1();
                MaterialList = new MAT1();
                FontList = new FNL1();
                RootPane = new PAN1();
                RootGroup = new GRP1();

                VersionMajor = 8;
                VersionMinor = 0;
                VersionMicro = 0;
                VersionMicro2 = 0;
            }

            public void Read(FileReader reader, BFLYT bflyt)
            {
                PaneLookup.Clear();
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
                SetVersionInfo();
                uint FileSize = reader.ReadUInt32();
                ushort sectionCount = reader.ReadUInt16();
                reader.ReadUInt16(); //Padding

                IsBigEndian = reader.ByteOrder == Syroot.BinaryData.ByteOrder.BigEndian;

                if (!IsBigEndian)
                {
                    if (VersionMajor == 3)
                        TextureManager.Platform = TextureManager.PlatformType.ThreeDS;
                    else
                        TextureManager.Platform = TextureManager.PlatformType.Switch;
                }
                else
                    TextureManager.Platform = TextureManager.PlatformType.WiiU;

                TextureManager.LayoutFile = this;

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
                            var panel = new PAN1(reader, this);
                            AddPaneToTable(panel);
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
                            AddPaneToTable(picturePanel);

                            SetPane(picturePanel, parentPane);
                            currentPane = picturePanel;
                            break;
                        case "txt1":
                            var textPanel = new TXT1(reader, this);
                            AddPaneToTable(textPanel);

                            SetPane(textPanel, parentPane);
                            currentPane = textPanel;
                            break;
                        case "bnd1":
                            var boundsPanel = new BND1(reader, this);
                            AddPaneToTable(boundsPanel);

                            SetPane(boundsPanel, parentPane);
                            currentPane = boundsPanel;
                            break;
                        case "prt1":
                            var partsPanel = new PRT1(reader, this);
                            AddPaneToTable(partsPanel);

                            SetPane(partsPanel, parentPane);
                            currentPane = partsPanel;
                            break;
                        case "wnd1":
                            var windowPanel = new WND1(reader, this);
                            AddPaneToTable(windowPanel);

                            SetPane(windowPanel, parentPane);
                            currentPane = windowPanel;
                            break;
                        case "scr1":
                            var scissorPane = new SCR1(reader, this);
                            AddPaneToTable(scissorPane);

                            SetPane(scissorPane, parentPane);
                            currentPane = scissorPane;
                            break;
                        case "ali1":
                            var alignmentPane = new ALI1(reader, this);
                            AddPaneToTable(alignmentPane);

                            SetPane(alignmentPane, parentPane);
                            currentPane = alignmentPane;
                            break;
                        case "pas1":
                            if (currentPane != null)
                                parentPane = currentPane;
                            break;
                        case "pae1":
                            if (parentPane != null)
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
                            if (currentPane != null)
                                ((PAN1)currentPane).UserData = new USD1(reader, this);
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
                if (!IsBigEndian)
                    writer.Write((ushort)0xFFFE);
                else
                    writer.Write((ushort)0xFEFF);
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

            private void WritePanes(FileWriter writer, BasePane pane, LayoutHeader header, ref int sectionCount)
            {
                WriteSection(writer, pane.Signature, pane,() => pane.Write(writer, header));
                sectionCount++;

                if (pane is IUserDataContainer && ((IUserDataContainer)pane).UserData != null)
                {
                    var userData = ((IUserDataContainer)pane).UserData;
                    WriteSection(writer, "usd1", userData, () => userData.Write(writer, this));
                    sectionCount++;
                }

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

            private void WriteGroupPanes(FileWriter writer, BasePane pane, LayoutHeader header, ref int sectionCount)
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
        }

        public class CNT1 : SectionCommon
        {
            public string Name { get; set; }

            public CNT1(FileReader reader, Header header)
            {
                uint paneNamesOffset = 0;
                uint paneCount = 0;
                uint animCount = 0;
                uint controlUserNameOffset = 0;
                uint paneParamNamesOffset = 0;
                uint animParamNamesOffset = 0;

                if (header.VersionMajor < 3)
                {
                    paneNamesOffset = reader.ReadUInt32();
                    paneCount = reader.ReadUInt32();
                    animCount = reader.ReadUInt32();
                }
                else
                {
                    controlUserNameOffset = reader.ReadUInt32();
                    paneNamesOffset = reader.ReadUInt32();
                    paneCount = reader.ReadUInt16();
                    animCount = reader.ReadUInt16();
                    paneParamNamesOffset = reader.ReadUInt32();
                    animParamNamesOffset = reader.ReadUInt32();
                }

                Name = reader.ReadZeroTerminatedString();


            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {

            }
        }

        public class TXT1 : PAN1, ITextPane
        {
            private Header layoutFile;

            public override string Signature { get; } = "txt1";

            public List<string> GetFonts
            {
                get
                {
                    return layoutFile.Fonts;
                }
            }

            public TXT1() : base()
            {
         
            }

            public TXT1(BFLYT.Header header, string name) {
                LoadDefaults();
                Name = name;
                layoutFile = header;
                //Add new material
                var mat = new Material(this.Name, header);
                header.MaterialList.Materials.Add(mat);
                MaterialIndex = (ushort)header.MaterialList.Materials.IndexOf(mat);
                Material = mat;

                FontIndex = 0;
                FontName = "";
                TextLength = 4;
                MaxTextLength = 4;
                TextAlignment = 0;
                LineAlignment = LineAlign.Unspecified;
                ItalicTilt = 0;
                FontTopColor = STColor8.White;
                FontBottomColor = STColor8.White;
                FontSize = new Vector2F(92,101);
                CharacterSpace = 0;
                LineSpace = 0;
                ShadowXY = new Vector2F(1, -1);
                ShadowXYSize = new Vector2F(1,1);
                ShadowBackColor = STColor8.Black;
                ShadowForeColor = STColor8.Black;
                ShadowItalic = 0;
            }

            public void LoadFontData(FFNT bffnt)
            {
                FontSize = new Vector2F(
                    bffnt.FontSection.Width,
                    bffnt.FontSection.Height);
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
                get { //Divide by 2 due to 2 characters taking up 2 bytes
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

            private void UpdateTextRender()
            {
                if (RenderableFont == null) return;

                System.Drawing.Bitmap bitmap = null;
                foreach (var fontFile in FirstPlugin.PluginRuntime.BxfntFiles)
                {
                    if (Utils.CompareNoExtension(fontFile.Name, FontName))
                        bitmap = fontFile.GetBitmap(Text, false, this);
                }

                if (bitmap != null)
                    RenderableFont.UpdateFromBitmap(bitmap);
            }

            public TXT1(FileReader reader, Header header) : base(reader, header)
            {
                long startPos = reader.Position - 84;

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
                FontTopColor = STColor8.FromBytes(reader.ReadBytes(4));
                FontBottomColor = STColor8.FromBytes(reader.ReadBytes(4));
                FontSize = reader.ReadVec2SY();
                CharacterSpace = reader.ReadSingle();
                LineSpace = reader.ReadSingle();
                uint nameOffset = reader.ReadUInt32();
                ShadowXY = reader.ReadVec2SY();
                ShadowXYSize = reader.ReadVec2SY();
                ShadowForeColor = STColor8.FromBytes(reader.ReadBytes(4));
                ShadowBackColor = STColor8.FromBytes(reader.ReadBytes(4));
                ShadowItalic = reader.ReadSingle();

                if (MaterialIndex != ushort.MaxValue && header.MaterialList.Materials.Count > 0)
                    Material = header.MaterialList.Materials[MaterialIndex];
                
                if (FontIndex != ushort.MaxValue && header.FontList.Fonts.Count > 0)
                    FontName = header.FontList.Fonts[FontIndex];

                if (textOffset != 0)
                {
                    reader.SeekBegin(startPos + textOffset);

                    if (RestrictedTextLengthEnabled)
                        Text = reader.ReadZeroTerminatedString(Encoding.Unicode);
                    else
                        Text = reader.ReadZeroTerminatedString(Encoding.Unicode);
                }

                if (nameOffset != 0)
                {
                    reader.SeekBegin(startPos + nameOffset);
                    TextBoxName = reader.ReadZeroTerminatedString();
                }
            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                long pos = writer.Position - 8;

                var textLength = TextLength;
                if (!RestrictedTextLengthEnabled && text != null)
                    textLength = (ushort)((text.Length * 2) + 2);

                base.Write(writer, header);
                writer.Write(textLength);
                if (!RestrictedTextLengthEnabled)
                    writer.Write(textLength);
                else
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
                writer.Write(FontTopColor.ToBytes());
                writer.Write(FontBottomColor.ToBytes());
                writer.Write(FontSize);
                writer.Write(CharacterSpace);
                writer.Write(LineSpace);
                long _ofsNamePos = writer.Position;
                writer.Write(0);
                writer.Write(ShadowXY);
                writer.Write(ShadowXYSize);
                writer.Write(ShadowForeColor.ToBytes());
                writer.Write(ShadowBackColor.ToBytes());
                writer.Write(ShadowItalic);
                if (header.VersionMajor == 8)
                {
                    writer.Write(0);
                    writer.Write(0);
                }

                if (Text != null)
                {
                    writer.WriteUint32Offset(_ofsTextPos, pos);
                    if (RestrictedTextLengthEnabled)
                        writer.WriteString(Text, MaxTextLength, Encoding.Unicode);
                    else
                        writer.WriteString(Text, TextLength, Encoding.Unicode);

                    writer.Align(4);
                }

                if (TextBoxName != null)
                {
                    writer.WriteUint32Offset(_ofsNamePos, pos);
                    writer.WriteString(TextBoxName);
                }
            }
        }

        public class WND1 : PAN1, IWindowPane
        {
            public Header layoutHeader;

            public override string Signature { get; } = "wnd1";

            public WND1() : base()
            {

            }

            public WND1(Header header, string name)
            {
                layoutHeader = header;
                LoadDefaults();
                Name = name;

                Content = new WindowContent(header, this.Name);
                UseOneMaterialForAll = true;
                UseVertexColorForAll = true;
                WindowKind = WindowKind.Around;
                NotDrawnContent = false;

                StretchLeft = 0;
                StretchRight = 0;
                StretchTop = 0;
                StretchBottm = 0;

                Width = 70;
                Height = 80;

                FrameElementLeft = 16;
                FrameElementRight = 16;
                FrameElementTop = 16;
                FrameElementBottm = 16;
                FrameCount = 1;

                WindowFrames = new List<BxlytWindowFrame>();
                SetFrames(header);
            }

            public void ReloadFrames() {
                SetFrames(layoutHeader);
            }

            public void SetFrames(Header header)
            {
                if (WindowFrames == null)
                    WindowFrames = new List<BxlytWindowFrame>();

                switch (FrameCount)
                {
                    case 1:
                        if (WindowFrames.Count == 0)
                            WindowFrames.Add(new BxlytWindowFrame(header,$"{Name}_LT"));
                        break;
                    case 2:
                        if (WindowFrames.Count == 0)
                            WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_L"));
                        if (WindowFrames.Count == 1)
                            WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_R"));
                        break;
                    case 4:
                        if (WindowFrames.Count == 0)
                            WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_LT"));
                        if (WindowFrames.Count == 1)
                            WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_RT"));
                        if (WindowFrames.Count == 2)
                            WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_LB"));
                        if (WindowFrames.Count == 3)
                            WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_RB"));
                        break;
                    case 8:
                        if (WindowFrames.Count == 0)
                            WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_LT"));
                        if (WindowFrames.Count == 1)
                            WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_RT"));
                        if (WindowFrames.Count == 2)
                            WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_LB"));
                        if (WindowFrames.Count == 3)
                            WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_RB"));
                        if (WindowFrames.Count == 4)
                            WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_T"));
                        if (WindowFrames.Count == 5)
                            WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_B"));
                        if (WindowFrames.Count == 6)
                            WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_R"));
                        if (WindowFrames.Count == 7)
                            WindowFrames.Add(new BxlytWindowFrame(header, $"{Name}_L"));
                        break;
                }

                //Now search for invalid unused materials and remove them
                for (int i = 0; i < WindowFrames.Count; i++)
                {
                    if (i >= FrameCount)
                        layoutHeader.TryRemoveMaterial(WindowFrames[i].Material);
                    else if (!layoutHeader.MaterialList.ContainsMat((Material)WindowFrames[i].Material))
                        layoutHeader.AddMaterial(WindowFrames[i].Material);
                }
            }

            public bool UseOneMaterialForAll
            {
                get { return Convert.ToBoolean(_flag & 1); }
                set { }
            }

            public bool UseVertexColorForAll
            {
                get { return Convert.ToBoolean((_flag >> 1) & 1); }
                set { }
            }

            public WindowKind WindowKind { get; set; }
     
            public bool NotDrawnContent
            {
                get { return (_flag & 8) == 16; }
                set { }
            }

            public ushort StretchLeft { get; set; }
            public ushort StretchRight { get; set; }
            public ushort StretchTop { get; set; }
            public ushort StretchBottm { get; set; }
            public ushort FrameElementLeft { get; set; }
            public ushort FrameElementRight { get; set; }
            public ushort FrameElementTop { get; set; }
            public ushort FrameElementBottm { get; set; }
            private byte _flag;

            private byte frameCount;
            public byte FrameCount
            {
                get { return frameCount; }
                set
                {
                    frameCount = value;
                }
            }

            public System.Drawing.Color[] GetVertexColors()
            {
                return new System.Drawing.Color[4]
                {
                    Content.ColorTopLeft.Color,
                    Content.ColorTopRight.Color,
                    Content.ColorBottomLeft.Color,
                    Content.ColorBottomRight.Color,
                };
            }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public BxlytWindowContent Content { get; set; }

            [Browsable(false)]
            public List<BxlytWindowFrame> WindowFrames { get; set; }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public BxlytWindowFrame TopLeftFrame
            {
                get { return WindowFrames.Count >= 1 ? WindowFrames[0] : null; }
            }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public BxlytWindowFrame TopRightFrame
            {
                get { return WindowFrames.Count >= 2 ? WindowFrames[1] : null; }
            }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public BxlytWindowFrame BottomLeftFrame
            {
                get { return WindowFrames.Count >= 3 ? WindowFrames[2] : null; }
            }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public BxlytWindowFrame BottomRightFrame
            {
                get { return WindowFrames.Count >= 4 ? WindowFrames[3] : null; }
            }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public BxlytWindowFrame TopFrame
            {
                get { return WindowFrames.Count >= 5 ? WindowFrames[4] : null; }
            }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public BxlytWindowFrame BottomFrame
            {
                get { return WindowFrames.Count >= 6 ? WindowFrames[5] : null; }
            }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public BxlytWindowFrame LeftFrame
            {
                get { return WindowFrames.Count >= 7 ? WindowFrames[6] : null; }
            }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public BxlytWindowFrame RightFrame
            {
                get { return WindowFrames.Count >= 8 ? WindowFrames[7] : null; }
            }

            public WND1(FileReader reader, Header header) : base(reader, header)
            {
                layoutHeader = header;
                WindowFrames = new List<BxlytWindowFrame>();

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

                WindowKind = (WindowKind)((_flag >> 2) & 3);

                reader.SeekBegin(pos + contentOffset);
                Content = new WindowContent(reader, header);

                reader.SeekBegin(pos + frameOffsetTbl);

                var offsets = reader.ReadUInt32s(FrameCount);
                foreach (int offset in offsets)
                {
                    reader.SeekBegin(pos + offset);
                    WindowFrames.Add(new BxlytWindowFrame(reader, header));
                }
            }

            public override void Write(FileWriter writer, LayoutHeader header)
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
                ((WindowContent)Content).Write(writer);

                if (WindowFrames.Count > 0)
                {
                    writer.WriteUint32Offset(_ofsContentPos + 4, pos);
                    //Reserve space for frame offsets
                    long _ofsFramePos = writer.Position;
                    writer.Write(new uint[FrameCount]);
                    for (int i = 0; i < FrameCount; i++)
                    {
                        writer.WriteUint32Offset(_ofsFramePos + (i * 4), pos);
                        WindowFrames[i].Write(writer);
                    }
                }
            }

            public class WindowContent : BxlytWindowContent
            {
                private Header LayoutFile;

                public WindowContent(Header header, string name) {
                    LayoutFile = header;
                    ColorTopLeft = STColor8.White;
                    ColorTopRight = STColor8.White;
                    ColorBottomLeft = STColor8.White;
                    ColorBottomRight = STColor8.White;

                    TexCoords.Add(new TexCoord());

                    //Add new material
                    Material = new Material($"{name}_C", header);
                    MaterialIndex = (ushort)header.AddMaterial(Material);
                }

                public WindowContent(FileReader reader, Header header)
                {
                    LayoutFile = header;

                    ColorTopLeft = reader.ReadColor8RGBA();
                    ColorTopRight = reader.ReadColor8RGBA();
                    ColorBottomLeft = reader.ReadColor8RGBA();
                    ColorBottomRight = reader.ReadColor8RGBA();
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

                    Material = LayoutFile.MaterialList.Materials[MaterialIndex];
                }

                public void Write(FileWriter writer)
                {
                    writer.Write(ColorTopLeft);
                    writer.Write(ColorTopRight);
                    writer.Write(ColorBottomLeft);
                    writer.Write(ColorBottomRight);
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
        }

        public class ALI1 : PAN1
        {
            public override string Signature { get; } = "ali1";

            public ALI1() : base()
            {

            }

            public ALI1(FileReader reader, Header header) : base(reader, header)
            {

            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                base.Write(writer, header);
            }
        }

        public class SCR1 : PAN1
        {
            public override string Signature { get; } = "scr1";

            public SCR1() : base()
            {

            }

            public SCR1(FileReader reader, Header header) : base(reader, header)
            {

            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                base.Write(writer, header);
            }
        }

        public class BND1 : PAN1, IBoundryPane
        {
            public override string Signature { get; } = "bnd1";

            public BND1() : base() {
                LoadDefaults();
            }

            public BND1(Header header, string name) : base() {
                LoadDefaults();
                Name = name;
            }

            public BND1(FileReader reader, Header header) : base(reader, header)
            {

            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                base.Write(writer, header);
            }
        }

        public class GRP1 : BasePane
        {
            private Header LayoutFile;

            public override string Signature { get; } = "grp1";
 
            public List<string> Panes { get; set; } = new List<string>();

            private bool displayInEditor = true;
            public override bool DisplayInEditor
            {
                get { return displayInEditor; }
                set
                {
                    displayInEditor = value;
                    for (int i = 0; i < Panes.Count; i++)
                    {
                        var pane = SearchPane(Panes[i]);
                        Console.WriteLine($"searching {Panes[i]} {pane != null}");
                        if (pane != null)
                            pane.DisplayInEditor = value;
                    }
                }
            }

            private BasePane SearchPane(string name)
            {
                if (LayoutFile.PaneLookup.ContainsKey(name))
                    return LayoutFile.PaneLookup[name];
                return null;
            }

            public GRP1() : base()
            {

            }

            public GRP1(FileReader reader, Header header)
            {
                LayoutFile = header;

                ushort numNodes = 0;
                if (header.VersionMajor >= 5)
                {
                    Name = reader.ReadString(34, true);
                    numNodes = reader.ReadUInt16();
                }
                else
                {
                    Name = reader.ReadString(24, true);;
                    numNodes = reader.ReadUInt16();
                    reader.Seek(2); //padding
                }

                for (int i = 0; i < numNodes; i++)
                    Panes.Add(reader.ReadString(24, true));
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

        public class PRT1 : PAN1, IPartPane
        {
            public override string Signature { get; } = "prt1";

            private bool hasSearchedParts = false;

            public PRT1() : base()
            {

            }

            [DisplayName("Magnify X"), CategoryAttribute("Parts")]
            public float MagnifyX { get; set; }

            [DisplayName("Magnify Y"), CategoryAttribute("Parts")]
            public float MagnifyY { get; set; }

            [DisplayName("Properties"), CategoryAttribute("Parts")]
            public List<PartProperty> Properties { get; set; }

            [DisplayName("External Layout File"), CategoryAttribute("Parts")]
            public string LayoutFileName { get; set; }

            private BFLYT ExternalLayout;

            public BasePane GetExternalPane()
            {
                if (hasSearchedParts) return null;

                if (ExternalLayout == null)
                    ExternalLayout = SearchExternalFile();

                if (ExternalLayout == null)
                    return null;

                //Load all the part panes to the lookup table
                foreach (var pane in ExternalLayout.header.PaneLookup)
                    if (!layoutFile.PaneLookup.ContainsKey(pane.Key))
                        layoutFile.PaneLookup.Add(pane.Key, pane.Value);

                layoutFile.PartsManager.AddLayout(ExternalLayout.header);

                return ExternalLayout.header.RootPane;
            }

            //Get textures if possible from the external parts file
            public void UpdateTextureData(Dictionary<string, STGenericTexture> textures)
            {
                if (hasSearchedParts) return;

                if (ExternalLayout == null)
                {
                    ExternalLayout = SearchExternalFile();
                    if (ExternalLayout == null)
                        return;

                    var textureList = ExternalLayout.GetTextures();
                    foreach (var tex in textureList)
                        if (!textures.ContainsKey(tex.Key))
                            textures.Add(tex.Key, tex.Value);

                    textureList.Clear();
                }
            }

            private BFLYT SearchExternalFile()
            {
                hasSearchedParts = false;

                var fileFormat = layoutFile.FileInfo;

                string path = FileManager.GetSourcePath(fileFormat);
                //File is outside an archive so check the contents it is in
                if (File.Exists(path))
                {
                    string folder = Path.GetDirectoryName(path);
                    foreach (var file in Directory.GetFiles(folder))
                    {
                        if (file.Contains(LayoutFileName))
                        {
                            if (Utils.GetExtension(file) == ".szs")
                            {
                                var openedFile = STFileLoader.OpenFileFormat(file);
                                if (openedFile == null)
                                    continue;

                                layoutFile.PartsManager.AddArchive((IArchiveFile)openedFile);
                                BFLYT bflyt = null;
                                SearchArchive((IArchiveFile)openedFile, ref bflyt);
                                if (bflyt != null)
                                    return bflyt;
                            }
                            else if (Utils.GetExtension(file) == ".bflan")
                            {
                                var openedFile = STFileLoader.OpenFileFormat(file);
                                if (openedFile == null)
                                    continue;

                                var bflan = openedFile as BXLAN;
                                layoutFile.PartsManager.AddAnimation(bflan.BxlanHeader);
                            }
                            else if (Utils.GetExtension(file) == ".bflyt")
                            {
                                var openedFile = STFileLoader.OpenFileFormat(file);
                                if (openedFile == null) continue;

                                openedFile.IFileInfo = new IFileInfo();
                                openedFile.IFileInfo.ArchiveParent = fileFormat.IFileInfo.ArchiveParent;
                                return (BFLYT)openedFile;
                            }
                        }
                    }
                }

                for (int i = 0; i < PluginRuntime.SarcArchives.Count; i++)
                {
                    BFLYT bflyt = null;
                    SearchArchive(PluginRuntime.SarcArchives[i], ref bflyt);
                    if (bflyt != null)
                        return bflyt;
                }

                return null;
            }

            private void SearchArchive(IArchiveFile archiveFile, ref BFLYT layoutFile)
            {
                layoutFile = null;

                foreach (var file in archiveFile.Files)
                {
                    if (file.FileName.Contains(".lyarc"))
                    {
                        var openedFile = file.OpenFile();
                        if (openedFile is IArchiveFile)
                            SearchArchive((IArchiveFile)openedFile, ref layoutFile);
                    }
                    else if (file.FileName.Contains(LayoutFileName))
                    {
                        var openedFile = file.OpenFile();
                        if (openedFile is IArchiveFile)
                            SearchArchive((IArchiveFile)openedFile, ref layoutFile);
                        else if (openedFile is BFLYT)
                        {
                            Console.WriteLine("Part found! " + file.FileName);

                            layoutFile = openedFile as BFLYT;
                            layoutFile.IFileInfo = new IFileInfo();
                            layoutFile.IFileInfo.ArchiveParent = layoutFile.IFileInfo.ArchiveParent;
                            return;
                        }
                    }
                }
            }

            private Header layoutFile;
            public PRT1(FileReader reader, Header header) : base(reader, header)
            {
                layoutFile = header;

                Properties = new List<PartProperty>();
                StartPosition = reader.Position - 84;

                uint properyCount = reader.ReadUInt32();
                MagnifyX = reader.ReadSingle();
                MagnifyY = reader.ReadSingle();
                for (int i = 0; i < properyCount; i++)
                    Properties.Add(new PartProperty(reader, header, StartPosition));

                LayoutFileName = reader.ReadZeroTerminatedString();
            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                long startPos = writer.Position - 8;
                base.Write(writer, header);
                writer.Write(Properties.Count);
                writer.Write(MagnifyX);
                writer.Write(MagnifyY);

                for (int i = 0; i < Properties.Count; i++)
                    Properties[i].Write(writer, header, startPos);

                writer.WriteString(LayoutFileName);
                writer.Align(4);

                for (int i = 0; i < Properties.Count; i++)
                    Properties[i].WriteProperties(writer, header, startPos);
            }
        }

        public class PartProperty
        {
            public string Name { get; set; }

            public byte UsageFlag { get; set; }
            public byte BasicUsageFlag { get; set; }
            public byte MaterialUsageFlag { get; set; }

            public BasePane Property { get; set; }

            public PartProperty(FileReader reader, Header header, long StartPosition)
            {
                Name = reader.ReadString(0x18, true);
                UsageFlag = reader.ReadByte();
                BasicUsageFlag = reader.ReadByte();
                MaterialUsageFlag = reader.ReadByte();
                reader.ReadByte(); //padding
                uint propertyOffset = reader.ReadUInt32();
                uint userDataOffset = reader.ReadUInt32();
                uint panelInfoOffset = reader.ReadUInt32();

                long pos = reader.Position;

                if (propertyOffset != 0)
                {
                    reader.SeekBegin(StartPosition + propertyOffset);

                    long startPos = reader.Position;
                    string signtature = reader.ReadString(4, Encoding.ASCII);
                    uint sectionSize = reader.ReadUInt32();

                    Console.WriteLine($"signtature " + signtature);

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
                        default:
                            Console.WriteLine("Unknown section! " + signtature);
                            break;
                    }
                }
                if (userDataOffset != 0)
                {
                    reader.SeekBegin(StartPosition + userDataOffset);

                }
                if (panelInfoOffset != 0)
                {
                    reader.SeekBegin(StartPosition + panelInfoOffset);

                }

                reader.SeekBegin(pos);
            }

            private long _ofsPos;
            public void Write(FileWriter writer, LayoutHeader header, long startPos)
            {
                writer.WriteString(Name, 0x18);
                writer.Write(UsageFlag);
                writer.Write(BasicUsageFlag);
                writer.Write(MaterialUsageFlag);
                writer.Write((byte)0);
                //Reserve offset spaces
                _ofsPos = writer.Position;
                writer.Write(0); //Property Offset
                writer.Write(0); //External User Data
                writer.Write(0); //Panel Info Offset
            }

            public void WriteProperties(FileWriter writer, LayoutHeader header, long startPos)
            {
                if (Property != null)
                {
                    writer.WriteUint32Offset(_ofsPos, startPos);
                    Header.WriteSection(writer, Property.Signature, Property, () => Property.Write(writer, header));
                }
            }
        }

        public class USD1 : UserData
        {
            public USD1()
            {
                Entries = new List<UserDataEntry>();
            }

            public USD1(FileReader reader, Header header) : base()
            {
                long startPos = reader.Position - 8;

                ushort numEntries = reader.ReadUInt16();
                reader.ReadUInt16(); //padding

                for (int i = 0; i < numEntries; i++)
                    Entries.Add(new USD1Entry(reader, startPos, header));
            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                long startPos = writer.Position - 8;

                writer.Write((ushort)Entries.Count);
                writer.Write((ushort)0);

                long enryPos = writer.Position;
                for (int i = 0; i < Entries.Count; i++)
                    ((USD1Entry)Entries[i]).Write(writer, header);

                for (int i = 0; i < Entries.Count; i++)
                {
                    writer.WriteUint32Offset(Entries[i]._pos + 4, Entries[i]._pos);
                    switch (Entries[i].Type)
                    {
                        case UserDataType.String:
                            writer.WriteString(Entries[i].GetString());
                            break;
                        case UserDataType.Int:
                            writer.Write(Entries[i].GetInts());
                            break;
                        case UserDataType.Float:
                            writer.Write(Entries[i].GetFloats());
                            break;
                    }

                    writer.WriteUint32Offset(Entries[i]._pos, Entries[i]._pos);
                    writer.WriteString(Entries[i].Name);
                }
            }
        }

        public class USD1Entry : UserDataEntry
        {
            public USD1Entry(FileReader reader, long startPos, Header header)
            {
                long pos = reader.Position;

                uint nameOffset = reader.ReadUInt32();
                uint dataOffset = reader.ReadUInt32();
                ushort dataLength = reader.ReadUInt16();
                Type = reader.ReadEnum<UserDataType>(false);
                Unknown = reader.ReadByte();

                long datapos = reader.Position;

                if (nameOffset != 0)
                {
                    reader.SeekBegin(pos + nameOffset);
                    Name = reader.ReadZeroTerminatedString();
                }

                if (dataOffset != 0)
                {
                    reader.SeekBegin(pos + dataOffset);
                    switch (Type)
                    {
                        case UserDataType.String:
                            if (dataLength != 0)
                                data = reader.ReadString((int)dataLength);
                            else
                                data = reader.ReadZeroTerminatedString();
                            break;
                        case UserDataType.Int:
                            data = reader.ReadInt32s((int)dataLength);
                            break;
                        case UserDataType.Float:
                            data = reader.ReadSingles((int)dataLength);
                            break;
                    }
                }

                reader.SeekBegin(datapos);
            }

            public void Write(FileWriter writer, LayoutHeader header)
            {
                _pos = writer.Position;

                writer.Write(0); //nameOffset
                writer.Write(0); //dataOffset
                writer.Write((ushort)GetDataLength());
                writer.Write(Type, false);
                writer.Write(Unknown);
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
        }

        public class PIC1 : PAN1, IPicturePane
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

            public System.Drawing.Color[] GetVertexColors()
            {
                return new System.Drawing.Color[4]
                {
                    ColorTopLeft.Color,
                    ColorTopRight.Color,
                    ColorBottomLeft.Color,
                    ColorBottomRight.Color,
                };
            }

            [Browsable(false)]
            public ushort MaterialIndex { get; set; }

            [TypeConverter(typeof(ExpandableObjectConverter))]
            public BxlytMaterial Material { get; set; }

            [Browsable(false)]
            public string GetTexture(int index)
            {
                return ParentLayout.TextureList.Textures[Material.TextureMaps[index].ID];
            }

            private BFLYT.Header ParentLayout;

            public PIC1() : base() {
                LoadDefaults();
            }

            public PIC1(Header header, string name) : base() {
                LoadDefaults();
                Name = name;

                ParentLayout = header;

                ColorTopLeft = STColor8.White;
                ColorTopRight = STColor8.White;
                ColorBottomLeft = STColor8.White;
                ColorBottomRight = STColor8.White;
                TexCoords = new TexCoord[1];
                TexCoords[0] = new TexCoord();

                Material = new Material(name, header);
            }

            public PIC1(FileReader reader, BFLYT.Header header) : base(reader, header)
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

                Material = header.MaterialList.Materials[MaterialIndex];
            }

            public override void Write(FileWriter writer, LayoutHeader header)
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

        public class PAN1 : BasePane, IUserDataContainer
        {
            public override string Signature { get; } = "pan1";

            private byte _flags1;
            private byte origin;

            [DisplayName("Is Visible"), CategoryAttribute("Flags")]
            public override bool Visible
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

            [DisplayName("Origin X"), CategoryAttribute("Origin")]
            public override OriginX originX
            {
                get { return (OriginX)(origin & 3); }
                set
                {
                    origin &= unchecked((byte)(~0x3));
                    origin |= (byte)value;
                }
            }

            [DisplayName("Origin Y"), CategoryAttribute("Origin")]
            public override OriginY originY
            {
                get { return (OriginY)((origin >> 2) & 3); }
                set
                {
                    origin |= (byte)((byte)value << 2);
                    origin &= unchecked((byte)(~3));
                }
            }

            [DisplayName("Parent Origin X"), CategoryAttribute("Origin")]
            public override OriginX ParentOriginX
            {
                get { return (OriginX)((origin >> 4) & 3); }
                set
                {
                    origin &= unchecked((byte)(~3));
                    origin |= (byte)((byte)value << 4);
                }
            }

            [DisplayName("Parent Origin Y"), CategoryAttribute("Origin")]
            public override OriginY ParentOriginY
            {
                get { return (OriginY)((origin >> 6) & 3); }
                set
                {
                    origin &= unchecked((byte)(~3));
                    origin |= (byte)((byte)value << 6);
                }
            }

            [DisplayName("Parts Flag"), CategoryAttribute("Flags")]
            public byte PaneMagFlags { get; set; }

            [DisplayName("User Data Info"), CategoryAttribute("User Data")]
            public string UserDataInfo { get; set; }

            [DisplayName("User Data"), CategoryAttribute("User Data")]
            public UserData UserData { get; set; }

            public PAN1() : base() {
                LoadDefaults();
            }

            public PAN1(Header header, string name) : base() {
                LoadDefaults();
                Name = name;
            }

            public override BasePane Copy()
            {
                PAN1 pan1 = new PAN1();
                pan1.Alpha = Alpha;
                pan1.DisplayInEditor = DisplayInEditor;
                pan1.Childern = Childern;
                pan1.Parent = Parent;
                pan1.Name = Name;
                pan1.LayoutFile = LayoutFile;
                pan1.NodeWrapper = NodeWrapper;
                pan1.originX = originX;
                pan1.originY = originY;
                pan1.ParentOriginX = ParentOriginX;
                pan1.ParentOriginY = ParentOriginY;
                pan1.Rotate = Rotate;
                pan1.Scale = Scale;
                pan1.Translate = Translate;
                pan1.Visible = Visible;
                pan1.Height = Height;
                pan1.Width = Width;
                pan1.UserData = UserData;
                pan1.UserDataInfo = UserDataInfo;
                pan1._flags1 = _flags1;
                return pan1;
            }

            public override UserData CreateUserData()
            {
                return new USD1();
            }

            public void LoadDefaults()
            {
                Alpha = 255;
                PaneMagFlags = 0;
                Name = "";
                Translate = new Vector3F(0, 0, 0);
                Rotate = new Vector3F(0, 0, 0);
                Scale = new Vector2F(1, 1);
                Width = 30;
                Height = 40;
                UserDataInfo = "";
                UserData = null;

                originX = OriginX.Center;
                originY = OriginY.Center;
                ParentOriginX = OriginX.Center;
                ParentOriginY = OriginY.Center;
                InfluenceAlpha = false;
                Visible = true;
            }

            public PAN1(FileReader reader, BxlytHeader header) : base()
            {
                LayoutFile = header;
                _flags1 = reader.ReadByte();
                origin = reader.ReadByte();
                Alpha = reader.ReadByte();
                PaneMagFlags = reader.ReadByte();
                Name = reader.ReadString(0x18, true);
                UserDataInfo = reader.ReadString(0x8, true);
                Translate = reader.ReadVec3SY();
                Rotate = reader.ReadVec3SY();
                Scale = reader.ReadVec2SY();
                Width = reader.ReadSingle();
                Height = reader.ReadSingle();
            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                writer.Write(_flags1);
                writer.Write(origin);
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

            public bool ContainsMat(Material material)
            {
                var matches = Materials.Any(p => p.Name == material.Name);
                if (Materials.Contains(material) || matches)
                    return true;
                else
                    return false;
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
        public class Material : BxlytMaterial
        {
            private string name;
            [DisplayName("Name"), CategoryAttribute("General")]
            public override string Name
            {
                get { return name; }
                set
                {
                    name = value;
                    if (NodeWrapper != null)
                        NodeWrapper.Text = name;
                }
            }

            public override void AddTexture(string texture)
            {
                Console.WriteLine("TextureMaps AddTexture");

                int index = ParentLayout.AddTexture(texture);
                TextureRef textureRef = new TextureRef();
                textureRef.ID = (short)index;
                textureRef.Name = texture;
                TextureMaps = TextureMaps.AddToArray(textureRef);
                TextureTransforms = TextureTransforms.AddToArray(new TextureTransform());
            }

            [DisplayName("Thresholding Alpha Interpolation"), CategoryAttribute("Alpha")]
            public override bool ThresholdingAlphaInterpolation
            {
                get { return Convert.ToBoolean((flags >> 18) & 0x1); }
            }

            [DisplayName("Black Color"), CategoryAttribute("Color")]
            public override STColor8 BlackColor { get; set; }

            [DisplayName("White Color"), CategoryAttribute("Color")]
            public override STColor8 WhiteColor { get; set; }

            [DisplayName("Texture Coordinate Params"), CategoryAttribute("Texture")]
            public TexCoordGen[] TexCoords { get; set; }

            [DisplayName("Indirect Parameter"), CategoryAttribute("Texture")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public IndirectParameter IndParameter { get; set; }

            [DisplayName("Projection Texture Coord Parameters"), CategoryAttribute("Texture")]
            public ProjectionTexGenParam[] ProjTexGenParams { get; set; }

            [DisplayName("Font Shadow Parameters"), CategoryAttribute("Font")]
            [TypeConverter(typeof(ExpandableObjectConverter))]
            public FontShadowParameter FontShadowParameter { get; set; }

            private uint flags;
            private int unknown;

            public string GetTexture(int index)
            {
                if (TextureMaps[index].ID != -1)
                    return ((Header)ParentLayout).TextureList.Textures[TextureMaps[index].ID];
                else
                    return "";
            }

            public Material(string name, BxlytHeader header)
            {
                ParentLayout = header;
                Name = name;
                TextureMaps = new TextureRef[0];
                TextureTransforms = new TextureTransform[0];
                TexCoords = new TexCoordGen[0];
                TevStages = new TevStage[0];
                ProjTexGenParams = new ProjectionTexGenParam[0];

                BlackColor = new STColor8(0, 0, 0, 0);
                WhiteColor = STColor8.White;
            }

            public Material(FileReader reader, Header header) : base()
            {
                ParentLayout = header;

                Name = reader.ReadString(0x1C, true);
                Name = Name.Replace("\x01", string.Empty);
                Name = Name.Replace("\x04", string.Empty);

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

                Bit.ExtractBits(1, 2, 30);

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

                EnableAlphaCompare = hasAlphaCompare;
                EnableBlend = hasBlendMode;
                EnableBlendLogic = seperateBlendMode;
                EnableIndParams = hasIndParam;
                EnableFontShadowParams = hasFontShadowParam;

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
                    AlphaCompare = new BxlytAlphaCompare(reader, header);
                if (hasBlendMode)
                    BlendMode = new BxlytBlendMode(reader, header);
                if (seperateBlendMode)
                    BlendModeLogic = new BxlytBlendMode(reader, header);
                if (hasIndParam)
                    IndParameter = new IndirectParameter(reader, header);

                for (int i = 0; i < projTexGenParamCount; i++)
                    ProjTexGenParams[i] = new ProjectionTexGenParam(reader, header);

                if (hasFontShadowParam)
                    FontShadowParameter = new FontShadowParameter(reader, header);
            }

            public void Write(FileWriter writer, LayoutHeader header)
            {
                long flagPos = 0;
                writer.WriteString(Name, 0x1C);
                if (header.VersionMajor >= 8)
                {
                    flagPos = writer.Position;
                    writer.Write(flags);
                    writer.Write(unknown);
                    writer.Write(BlackColor);
                    writer.Write(WhiteColor);
                }
                else
                {
                    writer.Write(BlackColor);
                    writer.Write(WhiteColor);

                    flagPos = writer.Position;
                    writer.Write(flags);
                }

                flags = 0;
                for (int i = 0; i < TextureMaps.Length; i++)
                {
                    flags += Bit.BitInsert(1,1,2,30);
                    ((TextureRef)TextureMaps[i]).Write(writer);
                }

                for (int i = 0; i < TextureTransforms.Length; i++)
                {
                    flags += Bit.BitInsert(1, 1, 2, 28);
                    ((TextureTransform)TextureTransforms[i]).Write(writer);
                }

                for (int i = 0; i < TexCoords.Length; i++)
                {
                    flags += Bit.BitInsert(1, 1, 2, 26);
                    TexCoords[i].Write(writer);
                }

                for (int i = 0; i < TevStages.Length; i++)
                {
                    flags += Bit.BitInsert(1, 1, 2, 24);
                    ((TevStage)TevStages[i]).Write(writer);
                }

                if (AlphaCompare != null && EnableAlphaCompare)
                {
                    flags += Bit.BitInsert(1, 1, 2, 22);
                    AlphaCompare.Write(writer);
                }
                if (BlendMode != null && EnableBlend)
                {
                    flags += Bit.BitInsert(1, 1, 2, 20);
                    BlendMode.Write(writer);
                }
                if (BlendModeLogic != null && EnableBlendLogic)
                {
                    flags += Bit.BitInsert(1, 1, 2, 18);
                    BlendModeLogic.Write(writer);
                }
                if (IndParameter != null && EnableIndParams)
                {
                    flags += Bit.BitInsert(1, 1, 1, 17);
                    IndParameter.Write(writer);
                }

                for (int i = 0; i < ProjTexGenParams.Length; i++)
                {
                    flags += Bit.BitInsert(1, 1, 2, 15);
                    ProjTexGenParams[i].Write(writer);
                }

                if (FontShadowParameter != null && EnableFontShadowParams)
                {
                    flags += Bit.BitInsert(1, 1, 1, 14);
                    FontShadowParameter.Write(writer);
                }

                using (writer.TemporarySeek(flagPos, SeekOrigin.Begin))
                {
                    writer.Write(flags);
                }
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
                    Fonts.Add(reader.ReadZeroTerminatedString());
                }
            }

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                writer.Write((ushort)Fonts.Count);
                writer.Write((ushort)0);

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
                writer.Write((ushort)0);

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

            public override void Write(FileWriter writer, LayoutHeader header)
            {
                writer.Write(DrawFromCenter);
                writer.Seek(3);
                writer.Write(Width);
                writer.Write(Height);
                writer.Write(MaxPartsWidth);
                writer.Write(MaxPartsHeight);
                writer.WriteString(Name);
            }
        }
    }
}
