using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using Toolbox.Library;
using Toolbox.Library.IO;
using System.ComponentModel;

namespace LayoutBXLYT.Cafe
{
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

        public USD1 UserData { get; set; }

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

        public override BasePane CreateNewNullPane(string name) {
            return new PAN1(this, name);
        }

        public override BasePane CreateNewTextPane(string name) {
            return new TXT1(this, name);
        }

        public override BasePane CreateNewPicturePane(string name) {
            return new PIC1(this, name);
        }

        public override BasePane CreateNewWindowPane(string name) {
            return new WND1(this, name);
        }

        public override BasePane CreateNewBoundryPane(string name) {
            return new BND1(this, name);
        }

        public override BasePane CreateNewPartPane(string name) {
            return new PRT1(this, name);
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
            if (TextureList.Textures.Contains(name))
                TextureList.Textures.Remove(name);

            RemoveTextureReferences(name);
        }

        public override short AddMaterial(BxlytMaterial material)
        {
            if (material == null) return -1;

            if (!MaterialList.Materials.Contains(material))
                MaterialList.Materials.Add(material);

            if (material.NodeWrapper == null)
                material.NodeWrapper = new MatWrapper(material.Name)
                {
                    Tag = material,
                    ImageKey = "material",
                    SelectedImageKey = "material",
                };

            if (!MaterialFolder.Nodes.Contains(material.NodeWrapper))
                MaterialFolder.Nodes.Add(material.NodeWrapper);

            return (short)MaterialList.Materials.IndexOf(material);
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

            if (MaterialList.Materials.Contains(material))
                MaterialList.Materials.Remove(material);
        }

        public override void TryRemoveMaterial(List<BxlytMaterial> materials)
        {
            foreach (var material in materials)
            {
                if (material == null) continue;
                material.RemoveNodeWrapper();

                if (MaterialList.Materials.Contains(material))
                    MaterialList.Materials.Remove(material);
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
        public override List<BxlytMaterial> Materials
        {
            get { return MaterialList.Materials; }
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

        public override BxlytMaterial GetMaterial(ushort index)
        {
            return MaterialList.Materials[index];
        }

        public override BxlytMaterial CreateNewMaterial(string name)
        {
            return new Material(name, this);
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
            UserData = new USD1();
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

            GroupPane currentGroupPane = null;
            GroupPane parentGroupPane = null;

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
                        TextureList = new TXL1(reader, this);
                        break;
                    case "fnl1":
                        FontList = new FNL1(reader, this);
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
                        long dataPos = reader.Position;

                        if (currentPane != null)
                        {
                            ((PAN1)currentPane).UserData = new USD1(reader, this);

                            reader.SeekBegin(dataPos);
                            ((PAN1)currentPane).UserData.Data = reader.ReadBytes((int)SectionSize - 8);
                        }
                        else
                        {
                            //User data before panes
                            UserData = new USD1(reader, this);

                            reader.SeekBegin(dataPos);
                            UserData.Data = reader.ReadBytes((int)SectionSize - 8);
                        }
                        break;
                    //If the section is not supported store the raw bytes
                    default:
                        section.Data = reader.ReadBytes((int)SectionSize - 8);
                        UnknownSections.Add(section);
                        Console.WriteLine("Unknown section!" + Signature);
                        break;
                }

                section.SectionSize = SectionSize;
                reader.SeekBegin(pos + SectionSize);
            }
        }

        private void SetPane(GroupPane pane, GroupPane parentPane)
        {
            if (parentPane != null)
            {
                parentPane.Childern.Add(pane);
                pane.Parent = parentPane;
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
            RecalculateMaterialReferences();

            Version = VersionMajor << 24 | VersionMinor << 16 | VersionMicro << 8 | VersionMicro2;

            foreach (var pane in PaneLookup.Values)
            {
                if (pane is PIC1)
                    ((PIC1)pane).MaterialIndex = (ushort)MaterialList.Materials.IndexOf(((PIC1)pane).Material);
            }

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

            WriteSection(writer, "lyt1", LayoutInfo, () => LayoutInfo.Write(writer, this));

            if (UserData != null && UserData.Entries?.Count > 0)
            {
                WriteSection(writer, "usd1", UserData, () => UserData.Write(writer, this));
                sectionCount++;
            }

            if (TextureList != null && TextureList.Textures.Count > 0)
            {
                WriteSection(writer, "txl1", TextureList, () => TextureList.Write(writer, this));
                sectionCount++;
            }
            if (FontList != null && FontList.Fonts.Count > 0)
            {
                WriteSection(writer, "fnl1", FontList, () => FontList.Write(writer, this));
                sectionCount++;
            }
            if (MaterialList != null && MaterialList.Materials.Count > 0)
            {
                WriteSection(writer, "mat1", MaterialList, () => MaterialList.Write(writer, this));
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
            WriteSection(writer, pane.Signature, pane, () => pane.Write(writer, header));
            sectionCount++;

            if (pane is IUserDataContainer && ((IUserDataContainer)pane).UserData != null &&
                ((IUserDataContainer)pane).UserData.Entries.Count > 0)
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

        private void WriteGroupPanes(FileWriter writer, GroupPane pane, LayoutHeader header, ref int sectionCount)
        {
            WriteSection(writer, pane.Signature, pane, () => pane.Write(writer, header));
            sectionCount++;

            if (pane.HasChildern)
            {
                sectionCount += 2;

                //Write start of children section
                WriteSection(writer, "grs1", null);

                foreach (GroupPane child in pane.Childern)
                    WriteGroupPanes(writer, child, header, ref sectionCount);

                //Write pae1 of children section
                WriteSection(writer, "gre1", null);
            }
        }
    }
}
