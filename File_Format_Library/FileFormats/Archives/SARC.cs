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
        public string[] Extension { get; set; } = new string[] { "*.pack", "*.sbeventpack", "*.sarc", "*.bgenv", "*.sbfarc", "*.sblarc", "*.sbactorpack", ".arc" };
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

        public Dictionary<string, SarcEntry> FileLookup = new Dictionary<string, SarcEntry>();

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

            PluginRuntime.SarcArchives.Add(this);

            FileLookup.Clear();

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
                var entry = SetupFileEntry(fileName, file.Value, Hash);
                files.Add(entry);
                FileLookup.Add(fileName, entry);

                if (SzsFiles.Files.Count == 1)
                    entry.OpenFileFormatOnLoad = true;
            }

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

                foreach (var file in files) {
                    file.FileName = file.FileName.Replace(ActorName, NewActorName);
                    file.UpdateWrapper();
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
            using (FileReader reader = new FileReader(data, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
                reader.Seek(6);
                ushort bom = reader.ReadUInt16();
                reader.Position = 0;

                if (bom == 0xFFFE)
                    return Syroot.BinaryData.ByteOrder.LittleEndian;
                else
                    return Syroot.BinaryData.ByteOrder.BigEndian;
            }
        }

        public void Unload()
        {
            foreach (var file in files)
            {
                if (file.FileFormat != null)
                    file.FileFormat.Unload();

                file.FileData = null;
            }

            files.Clear();

            if (PluginRuntime.SarcArchives.Contains(this))
                PluginRuntime.SarcArchives.Remove(this);

            GC.SuppressFinalize(this);
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
                    sarcData.Files.Add(file.HashName, file.FileData);
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

            public string OriginalFileName;

            private bool NameChanged
            {
                get { return OriginalFileName != FileName; }
            }



            private string hashName;
            public string HashName
            {
                get
                {
                    if (hashName == null || NameChanged)
                        hashName = NameHash(FileName).ToString("X8");

                    return hashName;
                }
                set
                {
                    hashName = value;
                }
            }

            public string TryGetHash(List<string> names, string folder)
            {
                //Name is not hashed so return
                if (!FileName.Contains(HashName))
                    return FileName;

                for (int i = 0; i < names?.Count; i++)
                {
                    uint hash = StringHashToUint(hashName);
                    Console.WriteLine($"{FileName} {hash} {names[i]} {NameHash(names[i])}");
                    if (hash == NameHash($"{folder}/{names[i]}"))
                        return names[i];
                }

                return FileName;
            }

            public bool IsHashMatch(string fileName)
            {
                uint hash = StringHashToUint(hashName);
                return hash == NameHash(FileName);
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

            static uint StringHashToUint(string name)
            {
                if (name.Contains("."))
                    name = name.Split('.')[0];
                if (name.Length != 8) throw new Exception("Invalid hash length");
                return Convert.ToUInt32(name, 16);
            }

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
            sarcEntry.OriginalFileName = fullName;
            sarcEntry.FileName = fullName;
            sarcEntry.FileData = data;
            sarcEntry.HashName = HashName;
            return sarcEntry;
        }
    }
}
