using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Toolbox;
using System.Windows.Forms;
using SARCExt;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class SARC : IArchiveFile, IFileFormat, IContextMenuNode
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Sorted ARChive" };
        public string[] Extension { get; set; } = new string[] { "*.pack", "*.sarc", "*.bgenv", "*.sblarc", "*.sbactorpack", ".arc" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "SARC");
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
            public STToolStripItem[] NewFileMenuExtensions => newFileMenu;
            public STToolStripItem[] NewFromFileMenuExtensions => null;
            public STToolStripItem[] ToolsMenuExtensions => null;
            public STToolStripItem[] TitleBarExtensions => null;
            public STToolStripItem[] CompressionMenuExtensions => null;
            public STToolStripItem[] ExperimentalMenuExtensions => null;
            public STToolStripItem[] EditMenuExtensions => null;
            public ToolStripButton[] IconButtonMenuExtensions => null;

            STToolStripItem[] newFileMenu = new STToolStripItem[2];
            STToolStripItem[] newFromFileMenu = new STToolStripItem[1];

            public MenuExt()
            {
                newFileMenu[0] = new STToolStripItem("SARC (Wii U)", CreateNewBigEndianSarc);
                newFileMenu[1] = new STToolStripItem("SARC (Switch/3DS)", CreateNewLittleEndianSarc);
            }

            private void CreateNewBigEndianSarc(object sender, EventArgs e)
            {
                SARC sarc = new SARC();
                sarc.IFileInfo = new IFileInfo();
                sarc.FileName = "NewArchive.szs";
                CreateNewSARC(sarc, true);

                ObjectEditor editor = new ObjectEditor(sarc);
                editor.Text = "NewArchive.szs";
                LibraryGUI.CreateMdiWindow(editor);
            }

            private void CreateNewLittleEndianSarc(object sender, EventArgs e)
            {
                SARC sarc = new SARC();
                sarc.IFileInfo = new IFileInfo();
                sarc.FileName = "NewArchive.szs";
                CreateNewSARC(sarc, false);

                ObjectEditor editor = new ObjectEditor(sarc);
                editor.Text = "NewArchive.szs";
                LibraryGUI.CreateMdiWindow(editor);
            }
        }

        public bool CanAddFiles { get; set; } = true;
        public bool CanRenameFiles { get; set; } = true;
        public bool CanReplaceFiles { get; set; } = true;
        public bool CanDeleteFiles { get; set; } = true;

        public List<SarcEntry> files = new List<SarcEntry>();
        public IEnumerable<ArchiveFileInfo> Files => files;

        public void ClearFiles() { files.Clear(); }

        public SarcData sarcData;

        public static void CreateNewSARC(SARC sarc, bool IsBigEndian)
        {
            sarc.CanSave = true;
            sarc.IFileInfo.UseEditMenu = true;

            sarc.sarcData = new SarcData();
            sarc.sarcData.HashOnly = false;
            sarc.sarcData.Files = new Dictionary<string, byte[]>();

            if (IsBigEndian)
            sarc.sarcData.endianness = Syroot.BinaryData.ByteOrder.BigEndian;
            else
                sarc.sarcData.endianness = Syroot.BinaryData.ByteOrder.LittleEndian;
        }

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;
            IFileInfo.UseEditMenu = true;

            var SzsFiles = SARCExt.SARC.UnpackRamN(stream);
            sarcData = new SarcData();
            sarcData.HashOnly = SzsFiles.HashOnly;
            sarcData.Files = SzsFiles.Files;
            sarcData.endianness = GetByteOrder(stream);

            foreach (var file in SzsFiles.Files)
                files.Add(SetupFileEntry(file.Key, file.Value));

            sarcData.Files.Clear();
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            files.Add(new SarcEntry()
            {
                FileData = archiveFileInfo.FileData,
                FileName = archiveFileInfo.FileName,
            });
            return true;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            files.Remove((SarcEntry)archiveFileInfo);
            return true;
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Rename Actor Files (Odyssey)", null, RenameActors, Keys.Control | Keys.R));
            return Items.ToArray();
        }

        private void RenameActors(object sender, EventArgs args)
        {
            string ActorName = Path.GetFileNameWithoutExtension(FileName);

            RenameDialog dialog = new RenameDialog();
            dialog.SetString(ActorName);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string NewActorName = dialog.textBox1.Text;
                FileName = NewActorName + ".szs";

                foreach (var file in files)
                {
                    string NodeName = Path.GetFileNameWithoutExtension(file.FileName);
                    string ext = Utils.GetExtension(file.FileName);
                    if (NodeName == ActorName)
                    {
                        file.FileName = $"{NewActorName}{ext}";
                    }
                    else if (file.FileName.Contains("Attribute.byml"))
                    {
                        file.FileName = $"{NewActorName}Attribute.byml";
                    }
                }
            }
        }

        private void UnpackToFolder(object sender, EventArgs args)
        {

        }

        private void PackFromFolder(object sender, EventArgs args)
        {

        }

        public Syroot.BinaryData.ByteOrder GetByteOrder(System.IO.Stream data)
        {
            using (FileReader reader = new FileReader(data))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                reader.Seek(6);
                ushort bom = reader.ReadUInt16();
                reader.Close();
                reader.Dispose();

                if (bom == 0xFFFE)
                    return Syroot.BinaryData.ByteOrder.LittleEndian;
                else
                    return Syroot.BinaryData.ByteOrder.BigEndian;
            }
        }

        public void Unload()
        {
            foreach (var file in files)
                file.FileData = null;

            files.Clear();

            GC.SuppressFinalize(this);
        }

        IEnumerable<TreeNode> Collect(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                yield return node;

                bool IsNodeFile = node is IFileFormat;

                if (!IsNodeFile)
                {
                    //We don't need to save the child of IFIleFormats
                    //If opened the file should save it's own children
                    foreach (var child in Collect(node.Nodes))
                        yield return child;
                }
            }
        }
        public byte[] Save()
        {
            sarcData.Files.Clear();
            foreach (var file in files)
            {
                file.SaveFileFormat();
                sarcData.Files.Add(file.FileName, file.FileData);
            }

            Tuple<int, byte[]> sarc = SARCExt.SARC.PackN(sarcData);

            IFileInfo.Alignment = sarc.Item1;
            return sarc.Item2;
        }

        public class SarcEntry : ArchiveFileInfo
        {
            public SARC sarc; //Sarc file the entry is located in

            public SarcEntry()
            {

            }

            public override Dictionary<string, string> ExtensionImageKeyLookup
            {
                get
                {
                    return new Dictionary<string, string>()
                    {
                           { ".byml", "byaml" },
                           { ".byaml", "byaml" },
                           { ".bfres", "bfres" },
                           { ".sbfres", "bfres" },
                           { ".aamp", "aamp" },
                    };
                }
            }
        }

        public SarcEntry SetupFileEntry(string fullName, byte[] data)
        {
            SarcEntry sarcEntry = new SarcEntry();
            sarcEntry.FileName = fullName;
            sarcEntry.FileData = data;
            return sarcEntry;
        }
    }
}
