using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using System.IO;
using BezelEngineArchive_Lib;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class BEA : TreeNodeFile, IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanAddFiles { get; set; } = true;
        public bool CanRenameFiles { get; set; } = true;
        public bool CanDeleteFiles { get; set; } = true;
        public bool CanReplaceFiles { get; set; } = true;

        public List<FileEntry> files = new List<FileEntry>();
        public IEnumerable<ArchiveFileInfo> Files => files;

        public void ClearFiles() { files.Clear(); }

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Bevel Engine Archive" };
        public string[] Extension { get; set; } = new string[] { "*.bea" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                return reader.CheckSignature(4, "SCNE");
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
            public STToolStripItem[] NewFileMenuExtensions => null;
            public STToolStripItem[] NewFromFileMenuExtensions => null;
            public STToolStripItem[] ToolsMenuExtensions => null;
            public STToolStripItem[] TitleBarExtensions => null;
            public STToolStripItem[] CompressionMenuExtensions => null;
            public STToolStripItem[] ExperimentalMenuExtensions => experimentalMenu;
            public STToolStripItem[] EditMenuExtensions => null;
            public ToolStripButton[] IconButtonMenuExtensions => null;

            STToolStripItem[] experimentalMenu = new STToolStripItem[1];
            public MenuExt()
            {
                experimentalMenu[0] = new STToolStripItem("BEA");

                STToolStripItem batchLUA = new STToolStripItem("Batch Extract LUA");
                batchLUA.Click += BatchExtractLUA;


                experimentalMenu[0].DropDownItems.Add(batchLUA);
            }

            private void BatchExtractLUA(object sender, EventArgs e)
            {
                FolderSelectDialog ofd = new FolderSelectDialog();

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = ofd.SelectedPath;

                    foreach (string file in Directory.GetFiles(folderPath))
                    {
                        Console.WriteLine(file);

                        if (Path.GetExtension(file) == ".bea")
                        {
                            BEA bea = STFileLoader.OpenFileFormat(file) as BEA;
                            foreach (FileEntry asset in bea.Files)
                            {
                                if (Path.GetExtension(asset.FileName) == ".lua")
                                {
                                    try
                                    {
                                        if (!String.IsNullOrWhiteSpace(Path.GetDirectoryName($"{folderPath}/{asset.FileName}"))) {
                                            if (!File.Exists(asset.FileName))
                                            {
                                                if (!Directory.Exists($"{folderPath}/{bea.FileName}")) {
                                                    Directory.CreateDirectory(Path.GetDirectoryName($"{folderPath}/{asset.FileName}"));
                                                }
                                            }
                                        }
                                        File.WriteAllBytes($"{folderPath}/{asset.FileName}", GetASSTData(asset));
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            bea.Unload();
                            GC.Collect();
                        }
                    }
                }
            }
        }

        public BezelEngineArchive beaFile;

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;
            beaFile = new BezelEngineArchive(stream);
            foreach (var file in beaFile.FileList.Values)
                files.Add(SetupFileEntry(file));
        }
        public void Unload()
        {
            this.files.Clear();
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            FileEntry file = new FileEntry(this);
            file.FileName = archiveFileInfo.FileName;
            file.CreateEntry(archiveFileInfo.FileData, true);
            files.Add(file);
            return true;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            files.Remove((FileEntry)archiveFileInfo);
            return true;
        }

        private void Save(object sender, EventArgs args)
        {
            Cursor.Current = Cursors.WaitCursor;
            List<IFileFormat> formats = new List<IFileFormat>();
            formats.Add(this);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(formats);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
            GC.Collect();
        }
        public void Save(System.IO.Stream stream)
        {
            beaFile.FileList.Clear();
            beaFile.FileDictionary.Clear();

            foreach (FileEntry node in Files)
            {
                node.SaveFileFormat();

                ASST asset = new ASST();
                asset.unk = node.unk1;
                asset.unk2 = node.unk2;
                asset.FileName = node.FileName;
                asset.FileData = node.CompressedData;
                asset.UncompressedSize = node.FileData.Length;
                beaFile.FileList.Add(asset.FileName, asset);
                beaFile.FileDictionary.Add(asset.FileName);
            }

            beaFile.Save(stream);
        }
    

        public bool Compressed;
        public class FolderEntry : TreeNode
        {
            public FolderEntry()
            {
                ImageKey = "folder";
                SelectedImageKey = "folder";
            }

            public FolderEntry(string Name)
            {
                Text = Name;
            }
        }
        public class FileEntry : ArchiveFileInfo
        {
            BEA ArchiveFile;
            public FileEntry(BEA bea)
            {
                ArchiveFile = bea;
            }

            private bool IsTexturesLoaded = false;
            public override IFileFormat OpenFile()
            {
                var FileFormat = base.OpenFile();
                bool IsModel = FileFormat is BFRES;

                if (IsModel && !IsTexturesLoaded)
                {
                    IsTexturesLoaded = true;
                    foreach (var file in ArchiveFile.Files)
                    {
                        if (Utils.GetExtension(file.FileName) == ".ftxb")
                        {
                            file.FileFormat = file.OpenFile();
                        }
                    }
                }

                return base.OpenFile();
            }

            public ushort unk1;
            public ushort unk2;
            public bool IsCompressed;

            public override byte[] FileData
            {
                get { return GetASSTData(this); }
                set { SetASST(this, value); }
            }

            public byte[] CompressedData;

            public void CreateEntry(byte[] data, bool compress)
            {
                IsCompressed = compress;
                unk1 = 2;
                unk2 = 12;
                FileData = data;
            }

            public override Dictionary<string, string> ExtensionImageKeyLookup
            {
                get
                {
                    return new Dictionary<string, string>()
                    {
                           { ".bntx", "bntx" },
                           { ".byml", "byaml" },
                           { ".byaml", "byaml" },
                           { ".bfres", "bfres" },
                           { ".sbfres", "bfres" },
                           { ".aamp", "aamp" },
                           { ".fmdb", "bfres" },
                           { ".fskb", "bfres" },
                           { ".ftsb", "bfres" },
                           { ".fvmb", "bfres" },
                           { ".fvbb", "bfres" },
                           { ".fspb", "bfres" },
                           { ".ftxb", "bntx" },
                           { ".fsnb", "bfres" },
                           { ".fmab", "bfres" },
                           { ".ftpb", "bfres" },
                    };
                }
            }
        }

        public static void ReplaceNode(TreeNode node, TreeNode replaceNode, TreeNode NewNode)
        {
            int index = node.Nodes.IndexOf(replaceNode);
            node.Nodes.RemoveAt(index);
            node.Nodes.Insert(index, NewNode);
        }

        public static byte[] GetASSTData(FileEntry entry)
        {
            if (entry.IsCompressed)
            {
                return Zstb.SDecompress(entry.CompressedData);
            }
            else
                return entry.CompressedData;
        }
        public static void SetASST(FileEntry fileEntry, byte[] data)
        {
            if (fileEntry.IsCompressed)
                fileEntry.CompressedData = Zstb.SCompress(data);
            else
                fileEntry.CompressedData = data;
        }

        public FileEntry SetupFileEntry(ASST asset)
        {
            FileEntry fileEntry = new FileEntry(this);
            fileEntry.FileName = asset.FileName;
            fileEntry.unk1 = asset.unk;
            fileEntry.unk2 = asset.unk2;
            fileEntry.IsCompressed = asset.IsCompressed;
            fileEntry.CompressedData = asset.FileData;
            return fileEntry;
        }
    }
}
