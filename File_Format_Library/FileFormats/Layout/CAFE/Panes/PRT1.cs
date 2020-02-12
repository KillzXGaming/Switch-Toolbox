using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Toolbox.Library;
using System.IO;
using Toolbox.Library.IO;
using FirstPlugin;

namespace LayoutBXLYT.Cafe
{
    public class PRT1 : PAN1, IPartPane
    {
        public override string Signature { get; } = "prt1";

        private bool hasSearchedParts = false;

        public PRT1() : base()
        {

        }

        public PRT1(Header header, string name) : base()
        {
            LoadDefaults();
            Name = name;

            layoutFile = header;

            MagnifyX = 1;
            MagnifyY = 1;

            LayoutFileName = "";

            Properties = new List<PartProperty>();
        }

        [DisplayName("Magnify X"), CategoryAttribute("Parts")]
        public float MagnifyX { get; set; }

        [DisplayName("Magnify Y"), CategoryAttribute("Parts")]
        public float MagnifyY { get; set; }

        [DisplayName("Properties"), CategoryAttribute("Parts")]
        public List<PartProperty> Properties { get; set; }

        private string layoutFilename;

        [DisplayName("External Layout File"), CategoryAttribute("Parts")]
        public string LayoutFileName
        {
            get { return layoutFilename; }
            set
            {
                layoutFilename = value;
                ExternalLayout = null;
                hasSearchedParts = false;
            }
        }

        private BFLYT ExternalLayout;

        public BasePane GetExternalPane()
        {
            if (hasSearchedParts || LayoutFileName == string.Empty) return null;

            ExternalLayout = layoutFile.PartsManager.TryGetLayout($"{LayoutFileName}.bflyt") as BFLYT;

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

                ExternalLayout.header.TextureManager = layoutFile.TextureManager;

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
                            try
                            {
                                var openedFile = STFileLoader.OpenFileFormat(file);
                                if (openedFile == null)
                                    continue;

                                openedFile.CanSave = false;
                                var bflan = openedFile as BXLAN;
                                layoutFile.PartsManager.AddAnimation(bflan.BxlanHeader);
                            }
                            catch
                            {

                            }
                        }
                        else if (Utils.GetExtension(file) == ".bflyt")
                        {
                            var openedFile = STFileLoader.OpenFileFormat(file);
                            if (openedFile == null) continue;

                            openedFile.CanSave = false;
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

            if (archiveFile is SARC)
            {
                if (((SARC)archiveFile).FileLookup.ContainsKey($"blyt/{LayoutFileName}.bflyt"))
                {
                    var entry = ((SARC)archiveFile).FileLookup[$"blyt/{LayoutFileName}.bflyt"];
                    var openedFile = entry.OpenFile();
                    if (openedFile is BFLYT)
                    {
                        layoutFile = openedFile as BFLYT;
                        layoutFile.IFileInfo = new IFileInfo();
                        layoutFile.IFileInfo.ArchiveParent = layoutFile.IFileInfo.ArchiveParent;
                        return;
                    }
                }
            }

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
                    try
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
                    catch
                    {

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

            //  for (int i = 0; i < Properties.Count; i++)
            //     Properties[i].WriteProperties(writer, header, startPos);

            //   for (int i = 0; i < Properties.Count; i++)
            //      Properties[i].WriteUserData(writer, header, startPos);

            for (int i = 0; i < Properties.Count; i++)
            {
                Properties[i].WriteProperties(writer, header, startPos);
                Properties[i].WriteUserData(writer, header, startPos);
                Properties[i].WritePaneInfo(writer, header, startPos);
            }
        }
    }

    public class PartProperty
    {
        public string Name { get; set; }

        public byte UsageFlag { get; set; }
        public byte BasicUsageFlag { get; set; }
        public byte MaterialUsageFlag { get; set; }

        public BasePane Property { get; set; }

        public USD1 UserData { get; set; }

        public byte[] PaneInfo { get; set; }

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
                        Property = new PIC1(reader, header);
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
                UserData = new USD1(reader, header);
            }
            if (panelInfoOffset != 0)
            {
                reader.SeekBegin(StartPosition + panelInfoOffset);
                PaneInfo = reader.ReadBytes(52);
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

        public void WriteUserData(FileWriter writer, LayoutHeader header, long startPos)
        {
            if (UserData != null)
            {
                writer.WriteUint32Offset(_ofsPos + 4, startPos);
                UserData.Write(writer, header);
            }
        }

        public void WritePaneInfo(FileWriter writer, LayoutHeader header, long startPos)
        {
            if (PaneInfo != null)
            {
                writer.WriteUint32Offset(_ofsPos + 8, startPos);
                writer.Write(PaneInfo);
            }
        }
    }

}
