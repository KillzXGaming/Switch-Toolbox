using FirstPlugin.FileFormats.Hashes;
using FlatBuffers.TRPAK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    internal class TRPAK : TreeNodeFile, IArchiveFile, IFileFormat
    {
        public static bool shownOodleError;

        public bool CanAddFiles => false;
        public bool CanDeleteFiles => false;
        public bool CanRenameFiles => false;
        public bool CanReplaceFiles => false;
        public bool CanSave { get; set; } = false;

        public Dictionary<string, string> CategoryLookup
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { ".bntx", "Textures" },
                    { ".trmbf", "Models" },
                    { ".trmdl", "Models" },
                    { ".trmsh", "Models" },
                    { ".trmtr", "Models" },
                    { ".trskl", "Models" },
                };
            }
        }

        public string[] Description { get; set; } = new string[] { "tr Package" };
        public string[] Extension { get; set; } = new string[] { "*.trpak" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public List<ArchiveFileInfo> files { get; set; }
        public IEnumerable<ArchiveFileInfo> Files => files;
        public FileType FileType { get; set; } = FileType.Archive;
        public IFileInfo IFileInfo { get; set; }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            throw new NotImplementedException();
        }

        public void ClearFiles()
        {
            throw new NotImplementedException();
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            throw new NotImplementedException();
        }

        public bool Identify(Stream stream)
        {
            return Utils.GetExtension(FileName) == ".trpak";
        }

        public void Load(Stream stream)
        {
            GFPAKHashCache.EnsureHashCache();
            files = new List<ArchiveFileInfo>();
            FlatBuffers.TRPAK.TRPAK trpak = FlatBuffers.TRPAK.TRPAK.GetRootAsTRPAK(new FlatBuffers.ByteBuffer(stream.ToBytes()));
            if (trpak.FilesLength != trpak.HashesLength)
            {
                throw new Exception("not the same amount of Hashes and File Entries in Trpak Container");
            }
            List<string> paths = new List<string>();
            for (int i = 0; i < trpak.FilesLength; i++)
            {
                FlatBuffers.TRPAK.File? file = trpak.Files(i);
                ulong hash = trpak.Hashes(i);
                if (file.HasValue)
                {
                    ArchiveFileInfo AFI = new ArchiveFileInfo();
                    byte[] FileData = file.Value.GetDataArray();
                    if (file.Value.CompressionType == Compression.OODLE)
                    {
                        if (!shownOodleError && !System.IO.File.Exists($"{Runtime.ExecutableDir}\\oo2core_6_win64.dll"))
                        {
                            MessageBox.Show("'oo2core_6_win64.dll' not found in the executable folder! User must provide their own copy!");
                            shownOodleError = true;
                        }
                        byte[] FileDatadecompressed = Toolbox.Library.Compression.Oodle.Decompress(FileData, (long)file.Value.DecompressedSize);
                        FileData = FileDatadecompressed;
                    }
                    AFI.FileData = FileData;
                    AFI.Name = GetName(hash, FileData);
                    if (AFI.Name.Contains("/"))
                    {
                        string tmppath = System.IO.Path.GetDirectoryName(AFI.Name);
                        if (!paths.Contains(tmppath)) paths.Add(tmppath);
                    }
                    AFI.FileName = AFI.Name;
                    files.Add(AFI);
                }
            }
            if (paths.Count == 1)
            {
                string path = paths[0].Replace("\\", "/");
                foreach (var f in files)
                {
                    if (!f.Name.Contains("/"))
                    {
                        f.Name = Path.Combine(path, f.Name).Replace("\\", "/");
                        f.FileName = f.Name;
                    }
                }
            }

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
                    TreeNode folder = new GFPAK.QuickAccessFileFolder(folderName);
                    if (folderName == "Textures")
                        folder = new TextureFolder(this, "Textures");
                    if (folderName == "Models")
                        folder = new GFPAK.QuickAccessFileFolder("Models");

                    node.Nodes.Add(folder);
                    folders.Add(folderName, folder);
                }

                string name = Path.GetFileName(file.FileName);

                string imageKey = "fileBlank";
                switch (ext)
                {
                    case ".bntx": imageKey = "bntx"; break;
                    case ".trmdl": imageKey = "model"; break;
                    case ".trskl": imageKey = "bone"; break;
                    case ".trmbf": imageKey = "mesh"; break;
                    case ".trmsh": imageKey = "mesh"; break;
                    case ".trmtr": imageKey = "material"; break;
                }

                TreeNode fodlerNode = folders[folderName];
                fodlerNode.Nodes.Add(new QuickAccessFile(name)
                {
                    Tag = file,
                    ImageKey = imageKey,
                    SelectedImageKey = imageKey,
                });
            }

            GFPAKHashCache.WriteCache();
        }

        public void Save(Stream stream)
        {
            throw new NotImplementedException();
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

        private string FindMatch(byte[] f, string FileName)
        {
            foreach (IFileFormat fileFormat in FileManager.GetFileFormats())
            {
                fileFormat.FileName = FileName;

                if (fileFormat.Identify(new MemoryStream(f)))
                {
                    return fileFormat.Extension[0].Replace("*", "");
                }
            }
            return "";
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

        private string GetName(ulong fileHash, byte[] Data)
        {
            string fileHashName = GFPAKHashCache.GetHashName(fileHash) ?? "";
            string ext = FindMatch(Data, fileHashName);
            if ((ext == ".bntx" && fileHashName == "") || ext == ".bfres" || ext == ".bnsh" || ext == ".bfsha")
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

                return $"{fileName}";
            }
            else
            {
                if (fileHashName != "")
                {
                    return $"{fileHashName}{ext}";
                }
                else
                    return $"{fileHash.ToString("X")}{ext}";
            }
        }

        public class TextureFolder : TreeNodeCustom, IContextMenuNode
        {
            private IArchiveFile ArchiveFile;
            private bool HasExpanded = false;
            private List<STGenericTexture> Textures = new List<STGenericTexture>();

            public TextureFolder(IArchiveFile archive, string text)
            {
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

            public virtual ToolStripItem[] GetContextMenuItems()
            {
                List<ToolStripItem> Items = new List<ToolStripItem>();
                Items.Add(new ToolStripMenuItem("Export All", null, ExportAllAction, Keys.Control | Keys.E));
                return Items.ToArray();
            }

            public void LoadTextures()
            {
                if (HasExpanded) return;

                List<TreeNode> subNodes = new List<TreeNode>();

                foreach (TreeNode node in Nodes)
                {
                    var file = (ArchiveFileInfo)node.Tag;
                    if (file.FileFormat == null)
                        file.FileFormat = file.OpenFile();

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
                        subNodes.Add(texNode);
                        Textures.Add(tex);
                    }
                }

                Nodes.Clear();
                Nodes.AddRange(subNodes.ToArray());

                HasExpanded = true;
            }

            public override void OnExpand()
            {
                LoadTextures();
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

            private void OnTextureDeleted(object sender, EventArgs e)
            {
                var tex = (TextureData)sender;
                foreach (var file in ArchiveFile.Files)
                {
                    if (file.FileFormat != null && file.FileFormat is BNTX)
                    {
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
        }
    }
}