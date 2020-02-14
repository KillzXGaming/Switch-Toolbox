using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library;
using FirstPlugin;

namespace LayoutBXLYT.GCBLO
{
    public class BLOHeader : BxlytHeader
    {
        public Dictionary<string, BloResource.bloResourceType> Resources = 
            new Dictionary<string, BloResource.bloResourceType>();

        private List<string> textureList = new List<string>();
        private List<string> fontList = new List<string>();
        private List<BxlytMaterial> materialList = new List<BxlytMaterial>();

        public override List<string> Textures
        {
            get { return textureList; }
        }

        public override List<string> Fonts
        {
            get { return fontList; }
        }

        public override List<BxlytMaterial> Materials
        {
            get { return materialList; }
        }

        public override Dictionary<string, STGenericTexture> GetTextures {
            get {
                Dictionary<string, STGenericTexture> textures = new Dictionary<string, STGenericTexture>();

                var archive = this.FileInfo.IFileInfo.ArchiveParent;
                foreach (var tex in Textures)
                {
                    if (archive != null)
                    {
                        foreach (var file in archive.Files)
                        {
                            string name = System.IO.Path.GetFileName(file.FileName);

                            Console.WriteLine($"{name} {tex} {tex.Contains(name)}");
                            if (tex.Contains(name))
                            {
                                var fileFormat = file.OpenFile();
                                if (fileFormat is BTI) {
                                    textures.Add(tex, (BTI)fileFormat);
                                }
                            }
                        }
                    }
                }
                return textures;
            }
        }

        public INF1 LayoutInfo;

        public void Read(FileReader reader, BLO bloFile)
        {
            FileInfo = bloFile;

            reader.SetByteOrder(true);
            reader.ReadSignature(4, "SCRN");
            string version = reader.ReadString(4, Encoding.ASCII);
            switch (version)
            {
                case "blo0": Version = 0; break;
                case "blo1": Version = 1; break;
                case "blo2": Version = 2; break;
                default:
                    throw new Exception("Unknown blo version! " + version);
            }
            uint fileSize = reader.ReadUInt32();
            uint numSections = reader.ReadUInt32();
            reader.Seek(0x10); //padding

            bool setRoot = false;
            bool setGroupRoot = false;

            BasePane currentPane = null;
            BasePane parentPane = null;
            this.RootGroup = new GroupPane();
            for (int i = 0; i < numSections; i++)
            {
                long pos = reader.Position;

                string Signature = reader.ReadString(4, Encoding.ASCII);
                uint SectionSize = reader.ReadUInt32();
                switch (Signature)
                {
                    case "INF1":
                        LayoutInfo = new INF1(reader, this);
                        break;
                    case "TEX1":
                        textureList = StringList.Read(reader, this);
                        break;
                    case "FNT1":
                        fontList = StringList.Read(reader, this);
                        break;
                    case "MAT1":
                        var mats = MAT1.ReadMaterials(reader, this);
                        foreach (var mat in mats)
                            materialList.Add(mat);
                        break;
                    case "PAN1":
                        {
                            var panel = new PAN1(reader, this);
                            AddPaneToTable(panel);
                            if (!setRoot)
                            {
                                RootPane = panel;
                                setRoot = true;
                            }

                            SetPane(panel, parentPane);
                            currentPane = panel;
                        }
                        break;
                    case "PAN2":
                        {
                            var panel = new PAN2(reader, this);
                            AddPaneToTable(panel);
                            if (!setRoot)
                            {
                                RootPane = panel;
                                setRoot = true;
                            }
                            SetPane(panel, parentPane);
                            currentPane = panel;
                        }
                        break;
                    case "PIC1":
                        {
                            var picturePanel = new PIC1(reader, this);
                            AddPaneToTable(picturePanel);

                            if (picturePanel.TextureName != string.Empty &&
                               !textureList.Contains(picturePanel.TextureName))
                            {
                                textureList.Add(picturePanel.TextureName);
                            }

                            SetPane(picturePanel, parentPane);
                            currentPane = picturePanel;
                            break;
                        }
                    case "PIC2":
                        {
                            reader.ReadUInt32(); //pan1 magic
                            reader.ReadUInt32(); //pan1 size
                            var picturePanel = new PIC2(reader, this);
                            AddPaneToTable(picturePanel);

                            SetPane(picturePanel, parentPane);
                            currentPane = picturePanel;
                            break;
                        }
                    case "WIN1":
                        var windowPane = new WIN1(reader, this);
                        AddPaneToTable(windowPane);

                        SetPane(windowPane, parentPane);
                        currentPane = windowPane;
                        break;
                    case "BGN1":
                        if (currentPane != null)
                            parentPane = currentPane;
                        break;
                    case "END1":
                        if (parentPane != null)
                            currentPane = parentPane;
                        parentPane = currentPane.Parent;
                        break;
                    case "EXT1":
                     //   reader.ReadUInt32();
                      //  reader.Align(0x20);
                        break;
                    default:
                        Console.WriteLine("Unknown section!" + Signature);
                        break;
                }

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

        }
    }
}
