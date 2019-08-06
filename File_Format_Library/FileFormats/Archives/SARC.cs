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
using Toolbox.Library.Security.Cryptography;

namespace FirstPlugin
{
    public class SARC : IArchiveFile, IFileFormat, IContextMenuNode
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Sead (hashed) Archived Resource" };
        public string[] Extension { get; set; } = new string[] { "*.pack", "*.sarc", "*.bgenv", "*.sbfarc", "*.sblarc", "*.sbactorpack", ".arc" };
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
            {
                string fileName = file.Key;
                string Hash = string.Empty;
                if (SzsFiles.HashOnly)
                {
                    fileName = SARCExt.SARC.TryGetNameFromHashTable(fileName);
                    Hash = file.Key;
                }
                files.Add(SetupFileEntry(fileName, file.Value, Hash));
            }

            sarcData.Files.Clear();

        //    stream.Close();
         //   stream.Dispose();
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

        static uint NameHash(string name)
        {
            uint result = 0;
            for (int i = 0; i < name.Length; i++)
            {
                result = name[i] + result * 0x00000065;
            }
            return result;
        }

        public void Save(System.IO.Stream stream)
        {
            sarcData.Files.Clear();
            foreach (var file in files)
            {
                Console.WriteLine("sarc file name " + file.FileName);
                file.SaveFileFormat();

                if (sarcData.HashOnly)
                {
                    sarcData.Files.Add(file.HashName, file.FileData);

                    /*
                    uint hash = 0;
                    bool IsHash = uint.TryParse(file.FileName, out hash);
                    if (IsHash && file.FileName.Length == 8)
                    {
                        sarcData.Files.Add(file.FileName, file.FileData);
                    }
                    else
                    {
                        string Hash = Crc32.Compute(file.FileName).ToString();
                        sarcData.Files.Add(Hash, file.FileData);
                    }*/
                }
                else
                    sarcData.Files.Add(file.FileName, file.FileData);
            }

            Tuple<int, byte[]> sarc = SARCExt.SARC.PackN(sarcData);

            IFileInfo.Alignment = sarc.Item1;

            using (var writer = new FileWriter(stream)) {
                writer.Write(sarc.Item2);
            }
        }

        public class SarcEntry : ArchiveFileInfo
        {
            public SARC sarc; //Sarc file the entry is located in

            public string HashName;

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
                           { ".bflim", "texture" },
                           { ".aamp", "aamp" },
                    };
                }
            }
        }

        public SarcEntry SetupFileEntry(string fullName, byte[] data, string HashName)
        {
            SarcEntry sarcEntry = new SarcEntry();
            sarcEntry.FileName = fullName;
            sarcEntry.FileData = data;
            sarcEntry.HashName = HashName;
            return sarcEntry;
        }
    }
}
