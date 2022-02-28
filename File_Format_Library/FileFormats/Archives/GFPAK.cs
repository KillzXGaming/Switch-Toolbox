using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using System.IO;
using Toolbox.Library.IO;
using Toolbox.Library.Animations;
using FirstPlugin.FileFormats.Hashes;

namespace FirstPlugin
{
    public class GFPAK : TreeNodeFile, IArchiveFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Graphic Package" };
        public string[] Extension { get; set; } = new string[] { "*.gfpak" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        static bool shownOodleError = false;

        public Dictionary<string, string> CategoryLookup
        {
            get {
                return new Dictionary<string, string>()
                {
                    { ".bnsh_vsh", "VertexShaders" },
                    { ".bnsh_fsh", "FragmentShaders" },
                    { ".bnsh", "Shaders" },
                    { ".bntx", "Textures" },
                    { ".gfbmdl", "Models" },
                    { ".gfbanm", "Animations" },
                    { ".gfbanmcfg", "AnimationConfigs" },
                    { ".gfbpokecfg", "PokemonConfigs" },
                };
            }
        }

        private string FindMatch(byte[] f)
        {
            if (f.Matches("SARC")) return ".szs";
            else if (f.Matches("Yaz")) return ".szs";
            else if (f.Matches("YB") || f.Matches("BY")) return ".byaml";
            else if (f.Matches("FRES")) return ".bfres";
            else if (f.Matches("Gfx2")) return ".gtx";
            else if (f.Matches("FLYT")) return ".bflyt";
            else if (f.Matches("CLAN")) return ".bclan";
            else if (f.Matches("CLYT")) return ".bclyt";
            else if (f.Matches("FLIM")) return ".bclim";
            else if (f.Matches("FLAN")) return ".bflan";
            else if (f.Matches("FSEQ")) return ".bfseq";
            else if (f.Matches("VFXB")) return ".ptcl";
            else if (f.Matches("AAHS")) return ".sharc";
            else if (f.Matches("BAHS")) return ".sharcb";
            else if (f.Matches("BNTX")) return ".bntx";
            else if (f.Matches("BNSH")) return ".bnsh";
            else if (f.Matches("FSHA")) return ".bfsha";
            else if (f.Matches("FFNT")) return ".bffnt";
            else if (f.Matches("CFNT")) return ".bcfnt";
            else if (f.Matches("CSTM")) return ".bcstm";
            else if (f.Matches("FSTM")) return ".bfstm";
            else if (f.Matches("STM")) return ".bstm";
            else if (f.Matches("CWAV")) return ".bcwav";
            else if (f.Matches("FWAV")) return ".bfwav";
            else if (f.Matches("CTPK")) return ".ctpk";
            else if (f.Matches("CGFX")) return ".bcres";
            else if (f.Matches("AAMP")) return ".aamp";
            else if (f.Matches("MsgStdBn")) return ".msbt";
            else if (f.Matches("MsgPrjBn")) return ".msbp";
            else if (f.Matches(0x00000004)) return ".gfbanm";
            else if (f.Matches(0x00000014)) return ".gfbanm";
            else if (f.Matches(0x00000018)) return ".gfbanmcfg";
            else if (f.Matches(0x00000020)) return ".gfbmdl";
            else if (f.Matches(0x00000044)) return ".gfbpokecfg";
            else return "";
        }

        //For BNTX, BNSH, etc
        private string GetBinaryHeaderName(byte[] Data)
        {
            using (var reader = new FileReader(Data))
            {
                reader.Seek(0x10, SeekOrigin.Begin);
                uint NameOffset = reader.ReadUInt32();

                reader.Seek(NameOffset, SeekOrigin.Begin);
                return reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);
            }
        }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(8, "GFLXPACK");
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

        public List<FileEntry> files = new List<FileEntry>();
        public IEnumerable<ArchiveFileInfo> Files => files;

        public void ClearFiles() { files.Clear(); folders.Clear(); }

        public bool CanAddFiles { get; set; } = true;
        public bool CanRenameFiles { get; set; } = false;
        public bool CanReplaceFiles { get; set; } = true;
        public bool CanDeleteFiles { get; set; } = true;

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            Read(new FileReader(stream));

            TreeNode node = new QuickAccessFolder(this, "Quick access");
            Nodes.Add(node);
            Dictionary<string, TreeNode> folders = new Dictionary<string, TreeNode>();
            foreach (var file in files)
            {
                string ext = Utils.GetExtension(file.FileName);
                string folderName = "Other";
                if (CategoryLookup.ContainsKey(ext))
                    folderName = CategoryLookup[ext];

                if (!folders.ContainsKey(folderName))
                {
                    TreeNode folder = new QuickAccessFileFolder(folderName);
                    if (folderName == "Textures")
                        folder = new TextureFolder(this, "Textures");
                    if (folderName == "Models")
                        folder = new ModelFolder("Models");
                    if (folderName == "Animations")
                        folder = new AnimationFolder("Animations");

                    node.Nodes.Add(folder);
                    folders.Add(folderName, folder);
                }

                string name = Path.GetFileName(file.FileName).Split('[').FirstOrDefault();

                string imageKey = "fileBlank";
                switch (ext)
                {
                    case ".bntx": imageKey = "bntx"; break;
                    case ".gfbmdl": imageKey = "model"; break;
                }

                TreeNode fodlerNode = folders[folderName];
                fodlerNode.Nodes.Add(new QuickAccessFile(name)
                {
                    Tag = file,
                    ImageKey = imageKey,
                    SelectedImageKey = imageKey,
                });
            }
        }

        public class QuickAccessFileFolder : TreeNodeCustom
        {
            private bool HasExpanded = false;

            public QuickAccessFileFolder(string text) {
                Text = text;
            }

            public override void OnExpand()
            {
                if (HasExpanded) return;

                List<TreeNode> files = new List<TreeNode>();
                foreach (TreeNode node in Nodes)
                {
                    var file = (ArchiveFileInfo)node.Tag;

                    try
                    {
                        if (file.FileFormat == null)
                            file.FileFormat = file.OpenFile();
                    }
                    catch
                    {
                        files.Add(node);
                        continue;
                    }

                    var fileNode = file.FileFormat as TreeNode;
                    if (fileNode != null)
                    {
                        fileNode.Tag = file;
                        fileNode.ImageKey = "fileBlank";
                        fileNode.SelectedImageKey = "fileBlank";
                        fileNode.Tag = file;
                        fileNode.Text = node.Text;

                        files.Add(fileNode);
                    }
                    else
                        files.Add(node);
                }

                Nodes.Clear();
                Nodes.AddRange(files.ToArray());

                HasExpanded = true;
            }
        }

        public class ModelFolder : TreeNodeCustom
        {
            private bool HasExpanded = false;

            public ModelFolder(string text) {
                Text = text;
            }

            public override void OnExpand()
            {
                if (HasExpanded) return;

                List<GFBMDL> models = new List<GFBMDL>();
                foreach (TreeNode node in Nodes)
                {
                    var file = (ArchiveFileInfo)node.Tag;
                    if (file.FileFormat == null)
                        file.FileFormat = file.OpenFile();

                    var model = file.FileFormat as GFBMDL;
                    if (model != null) {
                        model.Tag = file;
                        model.Text = node.Text;
                        if (Utils.GetExtension(model.Text) != ".gfbmdl")
                            model.Text += ".gfbmdl";

                        model.ImageKey = "model";
                        model.SelectedImageKey = "model";
                        models.Add(model);
                    }
                }

                Nodes.Clear();
                Nodes.AddRange(models.ToArray());

                HasExpanded = true;
            }
        }

        public class AnimationFolder : QuickAccessFileFolder, IContextMenuNode
        {
            public AnimationFolder(string text) : base(text) {
            }

            public virtual ToolStripItem[] GetContextMenuItems()
            {
                List<ToolStripItem> Items = new List<ToolStripItem>();
                Items.Add(new ToolStripMenuItem("Export All", null, ExportAllAction, Keys.Control | Keys.E));
                return Items.ToArray();
            }

            private void ExportAllAction(object sender, EventArgs args)
            {
                OnExpand();

                List<string> Formats = new List<string>();
                Formats.Add("SMD (.smd)");
                Formats.Add("SEANIM (.seanim)");

                FolderSelectDialog sfd = new FolderSelectDialog();

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = sfd.SelectedPath;

                    BatchFormatExport form = new BatchFormatExport(Formats);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        foreach (TreeNode node in Nodes)
                        {
                            Console.WriteLine($"node {node}");
                            if (!(node is IAnimationContainer))
                                continue;

                            var anim = ((IAnimationContainer)node).AnimationController;
                            string name = Path.GetFileNameWithoutExtension(node.Text);

                            if (form.Index == 0)
                                SMD.Save((STSkeletonAnimation)anim, $"{folderPath}/{name}.smd");
                            if (form.Index == 1)
                                SEANIM.Save((STSkeletonAnimation)anim, $"{folderPath}/{name}.seanim");
                        }
                    }
                }
            }
        }

        public class TextureSubFolder : TreeNodeCustom
        {

        }

        public class TextureFolder : TreeNodeCustom, IContextMenuNode
        {
            private bool HasExpanded = false;

            private IArchiveFile ArchiveFile;

            private List<STGenericTexture> Textures = new List<STGenericTexture>();

            public TextureFolder(IArchiveFile archive, string text) {
                ArchiveFile = archive;
                Text = text;
            }

            public void AddTexture(string fileName)
            {
                BNTX bntx = BNTX.CreateBNTXFromTexture(fileName);
                var mem = new MemoryStream();
                bntx.Save(mem);

                string filePath = fileName;

                ArchiveFile.AddFile(new ArchiveFileInfo()
                {
                    FileData = mem.ToArray(),
                    FileFormat = bntx,
                    FileName = filePath,
                });
            }

            private void OnTextureDeleted(object sender, EventArgs e)
            {
                var tex = (TextureData)sender;
                foreach (var file in ArchiveFile.Files)
                {
                    if (file.FileFormat != null && file.FileFormat is BNTX) {
                        var bntx = (BNTX)file.FileFormat;
                        if (bntx.Textures.ContainsKey(tex.Text))
                        {
                            bntx.RemoveTexture(tex);
                            bntx.Unload();
                            ArchiveFile.DeleteFile(file);
                            Nodes.RemoveByKey(tex.Text);
                        }
                    }
                }
            }

            public virtual ToolStripItem[] GetContextMenuItems()
            {
                List<ToolStripItem> Items = new List<ToolStripItem>();
                Items.Add(new ToolStripMenuItem("Export All", null, ExportAllAction, Keys.Control | Keys.E));
                Items.Add(new ToolStripMenuItem("Replace Textures (From Folder)", null, ReplaceAllAction, Keys.Control | Keys.R));
                return Items.ToArray();
            }

            private void ReplaceAllAction(object sender, EventArgs args) {
                LoadTextures();

                List<BNTX> bntxFiles = new List<BNTX>();
                foreach (TextureData node in Textures)
                    bntxFiles.Add(node.ParentBNTX);

                if (bntxFiles.Count > 0)
                    BNTX.ReplaceAll(bntxFiles.ToArray());
            }

            private void ExportAllAction(object sender, EventArgs args)
            {
                LoadTextures();

                List<string> Formats = new List<string>();
                Formats.Add("Microsoft DDS (.dds)");
                Formats.Add("Portable Graphics Network (.png)");
                Formats.Add("Joint Photographic Experts Group (.jpg)");
                Formats.Add("Bitmap Image (.bmp)");
                Formats.Add("Tagged Image File Format (.tiff)");

                FolderSelectDialog sfd = new FolderSelectDialog();

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = sfd.SelectedPath;

                    BatchFormatExport form = new BatchFormatExport(Formats);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        foreach (STGenericTexture tex in Textures)
                        {
                            if (form.Index == 0)
                                tex.SaveDDS(folderPath + '\\' + tex.Text + ".dds");
                            else if (form.Index == 1)
                                tex.SaveBitMap(folderPath + '\\' + tex.Text + ".png");
                            else if (form.Index == 2)
                                tex.SaveBitMap(folderPath + '\\' + tex.Text + ".jpg");
                            else if (form.Index == 3)
                                tex.SaveBitMap(folderPath + '\\' + tex.Text + ".bmp");
                            else if (form.Index == 4)
                                tex.SaveBitMap(folderPath + '\\' + tex.Text + ".tiff");
                        }
                    }
                }
            }

            public override void OnExpand() {
                LoadTextures();
            }

            public void LoadTextures()
            {
                if (HasExpanded) return;

                Dictionary<string, TreeNode> folders = new Dictionary<string, TreeNode>();

                //Create a folder lookup
                foreach (TreeNode node in Nodes) {
                    var file = (FileEntry)node.Tag;

                    string folder = file.FolderHash.Parent.hash.ToString();
                    if (!folders.ContainsKey(folder)) {
                        folders.Add(folder, new TreeNode($"{folders.Count}"));
                    }
                }

                List<TreeNode> subNodes = new List<TreeNode>();

                if (folders.Count > 1) {
                    foreach (var node in folders.Values)
                        subNodes.Add(node);
                }

                foreach (TreeNode node in Nodes)
                {
                    var file = (FileEntry)node.Tag;
                    if (file.FileFormat == null)
                        file.FileFormat = file.OpenFile();

                    string folder = file.FolderHash.Parent.hash.ToString();

                    BNTX bntx = file.FileFormat as BNTX;
                    foreach (var tex in bntx.Textures.Values)
                    {
                        tex.OnTextureDeleted += OnTextureDeleted;
                        //Set tree key for deletion
                        tex.Name = tex.Text;
                        tex.Tag = file;
                        var texNode = new TreeNode(tex.Text);
                        texNode.Tag = tex;
                        texNode.ImageKey = tex.ImageKey;
                        texNode.SelectedImageKey = tex.SelectedImageKey;

                        if (folders.Count > 1)
                            folders[folder].Nodes.Add(texNode);
                        else
                            subNodes.Add(texNode);
                        Textures.Add(tex);
                    }
                }

                Nodes.Clear();
                Nodes.AddRange(subNodes.ToArray());

                HasExpanded = true;
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

            GC.SuppressFinalize(this);
        }

        public void Save(System.IO.Stream stream)
        {
            if (version == 0x1000 && !File.Exists($"{Runtime.ExecutableDir}\\oo2core_6_win64.dll"))
            {
                MessageBox.Show("It is necessary to have 'oo2core_6_win64.dll' in the executable folder.");
                return;
            }
            Write(new FileWriter(stream));
        }

        private void Save(object sender, EventArgs args)
        {
            List<IFileFormat> formats = new List<IFileFormat>();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(formats);
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        private void CallRecursive(TreeView treeView)
        {
            // Print each node recursively.  
            TreeNodeCollection nodes = treeView.Nodes;
            foreach (TreeNode n in nodes)
            {
                PrintRecursive(n);
            }
        }
        private void PrintRecursive(TreeNode treeNode)
        {
            // Print each node recursively.  
            foreach (TreeNode tn in treeNode.Nodes)
            {
                PrintRecursive(tn);
            }
        }

        public ushort BOM;
        public uint Version;
        public List<Folder> folders = new List<Folder>();

        public int version;
        public int FolderCount;

        public void Read(FileReader reader)
        {
            string Signature = reader.ReadString(8, Encoding.ASCII);
            if (Signature != "GFLXPACK")
                throw new Exception($"Invalid signature {Signature}! Expected GFLXPACK.");

            GFPAKHashCache.EnsureHashCache();

            version = reader.ReadInt32();
            uint padding = reader.ReadUInt32();
            uint FileCount = reader.ReadUInt32();
            FolderCount = reader.ReadInt32();
            ulong FileInfoOffset = reader.ReadUInt64();
            ulong hashArrayPathsOffset = reader.ReadUInt64();
            ulong FolderArrayOffset = reader.ReadUInt64();

            reader.Seek((long)FolderArrayOffset, SeekOrigin.Begin);

            List<UInt64> hashes = new List<UInt64>();

            List<HashIndex> FolderFiles = new List<HashIndex>();
            for (int i = 0; i < FolderCount; i++)
            {
                Folder folder = new Folder();
                folder.Read(reader);
                folders.Add(folder);

                foreach (var hash in folder.hashes)
                    FolderFiles.Add(hash);
            }

            reader.Seek((long)hashArrayPathsOffset, SeekOrigin.Begin);
            for (int i = 0; i < FileCount; i++)
            {
                ulong hash = reader.ReadUInt64();
                hashes.Add(hash);
            }

            GeneratePokeStrings();

            reader.Seek((long)FileInfoOffset, SeekOrigin.Begin);
            for (int i = 0; i < FileCount; i++)
            {
                FileEntry fileEntry = new FileEntry(this);

                fileEntry.Read(reader);
                string Extension = FindMatch(fileEntry.FileData);
                if (Extension.EndsWith("gfbanmcfg") && version != 0x1000)
                {
                    GFBANMCFG cfg = new GFBANMCFG();
                    cfg.Load(new MemoryStream(fileEntry.FileData));
                    GenerateAnmCfgStrings(cfg);
                }

                files.Add(fileEntry);
            }

            for (int i = 0; i < FileCount; i++)
            {
                FileEntry fileEntry = files[i];

                for (int f = 0; f < FolderFiles.Count; f++)
                    if (FolderFiles[f].Index == i)
                        fileEntry.FolderHash = FolderFiles[f];

                var dir = fileEntry.FolderHash.Parent;

                fileEntry.FileName = GetString(hashes[i], fileEntry.FolderHash, fileEntry.FileData);
                fileEntry.FilePathHash = hashes[i];
            }

            GFPAKHashCache.WriteCache();
        }

        private void GenerateAnmCfgStrings(GFBANMCFG cfg)
        {
            foreach (GFBANMCFG.Animation a in cfg.Config.Animations)
            {
                GFPAKHashCache.PutHash(a.FileName);
            }
        }

        private void GeneratePokeStrings()
        {
            //Also check file name just in case
            if (FileName.Contains("pm"))
            {
                string baseName = FileName.Substring(0, 12);

                GFPAKHashCache.GeneratePokeStringsFromFile(baseName);
            }
        }

        private string GetString(ulong fullHash, HashIndex fileHashIndex, byte[] Data)
        {
            var folderHash = fileHashIndex.Parent.hash;
            var fileHash = fileHashIndex.hash;

            bool hasFolderHash = false;

            string folder = "";
            string folderHashName = GFPAKHashCache.GetHashName(folderHash);
            if (folderHashName != null) {
                hasFolderHash = true;
                folder = $"{folderHashName}/";
            }

            if (!hasFolderHash)
                folder = $"{folderHash.ToString("X")}/";



            string ext = FindMatch(Data);
            if (ext == ".bntx" || ext == ".bfres" || ext == ".bnsh" || ext == ".bfsha")
            {
                string fileName = GetBinaryHeaderName(Data);
                //Check for matches for shaders
                if (ext == ".bnsh")
                {
                    if (FNV64A1.Calculate($"{fileName}.bnsh_fsh") == fileHash)
                        fileName = $"{fileName}.bnsh_fsh";
                    else if (FNV64A1.Calculate($"{fileName}.bnsh_vsh") == fileHash)
                        fileName = $"{fileName}.bnsh_vsh";
                }
                else
                    fileName = $"{fileName}{ext}";

                if (hasFolderHash)
                    return $"{folder}{fileName}";
                else
                    return $"{folder}{fileName}[FullHash={fullHash.ToString("X")}]{ext}";
            }
            else
            {
                string fileHashName = GFPAKHashCache.GetHashName(fileHash);
                if (fileHashName != null)
                {
                    if (hasFolderHash)
                        return $"{folder}{fileHashName}";
                    else
                        return $"{folder}{fileHashName}[FullHash={fullHash.ToString("X")}]{ext}";
                }
                else
                    return $"{folder}{fileHash.ToString("X")}[FullHash={fullHash.ToString("X")}]{ext}";
            }
        }



        public void Write(FileWriter writer)
        {
            writer.WriteSignature("GFLXPACK");
            writer.Write(version);
            writer.Write(0);
            writer.Write(files.Count);
            writer.Write(FolderCount);
            long FileInfoOffset = writer.Position;
            writer.Write(0L);
            long HashArrayOffset = writer.Position;
            writer.Write(0L);
            long folderArrOffset = writer.Position;

            //Reserve space for folder offsets
            for (int f = 0; f < FolderCount; f++)
                writer.Write(0L);

            //Now write all sections
            writer.WriteUint64Offset(HashArrayOffset);
            foreach (var fileTbl in files)
                writer.Write(fileTbl.FilePathHash);

            //Save folder sections
            List<long> FolderSectionPositions = new List<long>();
            foreach (var folder in folders)
            {
                FolderSectionPositions.Add(writer.Position);
                folder.Write(writer);
            }
            //Write the folder offsets back
            using (writer.TemporarySeek(folderArrOffset, SeekOrigin.Begin))
            {
                foreach (long offset in FolderSectionPositions)
                    writer.Write(offset);
            }

            //Now file data
            writer.WriteUint64Offset(FileInfoOffset);
            foreach (var fileTbl in files)
                fileTbl.Write(writer);

            //Save data blocks
            foreach (var fileTbl in files)
            {
                fileTbl.WriteBlock(writer);
            }

            writer.Align(16);
        }

        public class Folder
        {
            public ulong hash;
            public uint FileCount => (uint)hashes.Count;
            public uint Padding = 0xCC;

            public List<HashIndex> hashes = new List<HashIndex>();

            public void Read(FileReader reader)
            {
                hash = reader.ReadUInt64();
                uint fileCount = reader.ReadUInt32();
                Padding = reader.ReadUInt32();

                for (int f = 0; f < fileCount; f++)
                {
                    HashIndex hash = new HashIndex();
                    hash.Read(reader, this);
                    hashes.Add(hash);
                }
            }
            public void Write(FileWriter writer)
            {
                writer.Write(hash);
                writer.Write(FileCount);
                writer.Write(Padding);

                foreach (var hash in hashes)
                    hash.Write(writer);
            }
        }

        public class HashIndex
        {
            public ulong hash;
            public int Index;
            public uint Padding = 0xCC;

            public Folder Parent { get; set; }

            public void Read(FileReader reader, Folder parent)
            {
                Parent = parent;
                hash = reader.ReadUInt64();
                Index = reader.ReadInt32();
                Padding = reader.ReadUInt32(); //Always 0xCC?
            }
            public void Write(FileWriter writer)
            {
                writer.Write(hash);
                writer.Write(Index);
                writer.Write(Padding);
            }
        }
        public class FileEntry : ArchiveFileInfo
        {
            public HashIndex FolderHash;

            public UInt64 FilePathHash;

            public ushort Level = 9;
            public CompressionType Type = CompressionType.Lz4;
            private long DataOffset;

            public uint CompressedFileSize;
            public uint Padding = 0xCC;

            private IArchiveFile ArchiveFile;

            public FileEntry(IArchiveFile archiveFile) {
                ArchiveFile = archiveFile;
            }

            private bool IsTexturesLoaded = false;
            public override IFileFormat OpenFile()
            {
                var FileFormat = base.OpenFile();
                bool IsModel = FileFormat is GFBMDL;

                if (IsModel && !IsTexturesLoaded)
                {
                    IsTexturesLoaded = true;
                    foreach (var file in ArchiveFile.Files)
                    {
                        if (Utils.GetExtension(file.FileName) == ".bntx")
                        {
                            file.FileFormat = file.OpenFile();
                        }
                    }
                }


                return base.OpenFile();
            }

            public void Read(FileReader reader)
            {
                Level = reader.ReadUInt16(); //Usually 9?
                Type = reader.ReadEnum<CompressionType>(false);
                uint DecompressedFileSize = reader.ReadUInt32();
                CompressedFileSize = reader.ReadUInt32();
                Padding = reader.ReadUInt32();
                ulong FileOffset = reader.ReadUInt64();

                if (Type == CompressionType.Oodle)
                {
                    if (!shownOodleError && !File.Exists($"{Runtime.ExecutableDir}\\oo2core_6_win64.dll"))
                    {
                        MessageBox.Show("'oo2core_6_win64.dll' not found in the executable folder! User must provide their own copy!");
                        shownOodleError = true;
                    }
                }

                using (reader.TemporarySeek((long)FileOffset, SeekOrigin.Begin))
                {
                    if (Type == CompressionType.Lz4)
                    {
                        FileData = reader.ReadBytes((int)CompressedFileSize);
                        FileData = STLibraryCompression.Type_LZ4.Decompress(FileData, 0, (int)CompressedFileSize, (int)DecompressedFileSize);
                    }
                    else if (Type == CompressionType.None)
                        FileData = reader.ReadBytes((int)DecompressedFileSize);
                    else if (Type == CompressionType.Oodle && !shownOodleError)
                    {
                        FileData = reader.ReadBytes((int)CompressedFileSize); 
                        FileData = STLibraryCompression.Type_Oodle.Decompress(FileData, (int)DecompressedFileSize);
                    }
                    else
                        FileData = reader.ReadBytes((int)CompressedFileSize);
                }
            }

            byte[] CompressedData;
            public void Write(FileWriter writer)
            {
                this.SaveFileFormat();

                CompressedData = Compress(FileData, Type);

                writer.Write(Level);
                writer.Write(Type, true);
                writer.Write(FileData.Length);
                writer.Write(CompressedData.Length);
                writer.Write(Padding);
                DataOffset = writer.Position;
                writer.Write(0L);
            }
            public void WriteBlock(FileWriter writer)
            {
                writer.Align(16);
                writer.WriteUint64Offset(DataOffset);
                writer.Write(CompressedData);
            }
            public static byte[] Compress(byte[] data, CompressionType Type)
            {
                if (Type == CompressionType.Lz4)
                    return STLibraryCompression.Type_LZ4.Compress(data);
                else if (Type == CompressionType.None)
                    return data;
                else if (Type == CompressionType.Zlib)
                    return STLibraryCompression.ZLIB.Compress(data);
                else if (Type == CompressionType.Oodle)
                    return STLibraryCompression.Type_Oodle.Compress(data, Toolbox.Library.Compression.Oodle.OodleLZ_Compressor.OodleLZ_Compressor_Kraken,
                        Toolbox.Library.Compression.Oodle.OodleLZ_CompressionLevel.OodleLZ_CompressionLevel_Optimal2);
                else 
                    return data;
            }

            public enum CompressionType : ushort
            {
                None = 0,
                Zlib = 1,
                Lz4 = 2,
                Oodle = 3,
            }
        }

        public static void ReplaceNode(TreeNode node, TreeNode replaceNode, TreeNode NewNode)
        {
            if (NewNode == null)
                return;

            int index = node.Nodes.IndexOf(replaceNode);
            node.Nodes.RemoveAt(index);
            node.Nodes.Insert(index, NewNode);
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            //First we need to determine the paths
            string fullPath = archiveFileInfo.FileName.Replace("\\", "/");
            string filePath = Path.GetFileName(fullPath);
            string filePathNoExt = Path.GetFileNameWithoutExtension(fullPath);
            string directoryPath = Path.GetDirectoryName(fullPath).Replace("\\", "/");

            ulong fullPathHash = 0;
            ulong directoryHash = 0;
            ulong fileHash = 0;

            //Calculate hashes for each one

            //Check for full path hashes
            if (fullPath.Contains("[FullHash="))
            {
                string HashString = fullPath.Split('=').LastOrDefault().Replace("]", string.Empty);
                HashString = Path.GetFileNameWithoutExtension(HashString);

                filePath = filePath.Split('[').FirstOrDefault();

                TryParseHex(HashString, out fullPathHash);
            }

            ulong hash = 0;
            bool isDirectoryHash = TryParseHex(directoryPath, out hash);
            bool isFileHash = TryParseHex(filePath, out hash);

            if (isFileHash)
                TryParseHex(filePath, out fileHash);
            else
                fileHash = FNV64A1.Calculate(filePath);

            if (isDirectoryHash)
                TryParseHex(directoryPath, out directoryHash);
            else
                directoryHash = FNV64A1.Calculate($"{directoryPath}/");

            if (!isFileHash && !isDirectoryHash)
                fullPathHash = FNV64A1.Calculate(fullPath);

            var folder = folders.FirstOrDefault(x => x.hash == directoryHash);

            Console.WriteLine($"{fullPath} FilePathHash {fullPathHash}");
            Console.WriteLine($"{directoryPath} FolderHash {directoryHash} directoryHash {directoryHash}");
            Console.WriteLine($"{filePath} fileHash {fileHash} isFileHash {isFileHash}");

            if (folder != null)
            {
                folder.hashes.Add(new HashIndex()
                {
                    hash = fileHash,
                    Parent = folder,
                    Index = files.Count,
                });
            }
            else
            {
                folder = new Folder();
                folder.hash = directoryHash;
                folder.hashes.Add(new HashIndex()
                {
                    hash = fileHash,
                    Parent = folder,
                    Index = files.Count,
                });
                folders.Add(folder);
            }

            files.Add(new FileEntry(this)
            {
                FilePathHash = fullPathHash,
                FolderHash = folder.hashes.LastOrDefault(),
                FileData = archiveFileInfo.FileData,
                FileDataStream = archiveFileInfo.FileDataStream,
                FileName = archiveFileInfo.FileName,
            });

            return true;
        }

        private static bool TryParseHex(string str, out ulong value)
        {
            return ulong.TryParse(str, System.Globalization.NumberStyles.HexNumber, null, out value);
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            int removeindex = files.IndexOf((FileEntry)archiveFileInfo);
            foreach (var folder in folders)
            {
                for (int f = 0; f < folder.FileCount; f++)
                    if (folder.hashes[f].Index == removeindex)
                        folder.hashes.RemoveAt(f);
            }

            files.Remove((FileEntry)archiveFileInfo);
            RegenerateFileIndices();

            return true;
        }

        private void RegenerateFileIndices()
        {
            foreach (var folder in folders)
            {
                int index = 0;
                foreach (FileEntry file in files)
                {
                    for (int f = 0; f < folder.FileCount; f++)
                    {
                        if (file.FolderHash == folder.hashes[f])
                            folder.hashes[f].Index = index;
                    }

                    index++;
                }
            }
        }
    }
}
