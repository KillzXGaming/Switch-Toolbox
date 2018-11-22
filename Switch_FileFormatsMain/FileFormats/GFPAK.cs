using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using System.IO;
using Switch_Toolbox.Library.IO;

namespace FirstPlugin
{
    public class GFPAK : IFileFormat
    {
        public bool CanSave { get; set; } = false;
        public bool FileIsEdited { get; set; } = false;
        public bool FileIsCompressed { get; set; } = false;
        public string[] Description { get; set; } = new string[] { "Graphic Package" };
        public string[] Extension { get; set; } = new string[] { "*.gfpak" };
        public string Magic { get; set; } = "GFLX";
        public CompressionType CompressionType { get; set; } = CompressionType.None;
        public byte[] Data { get; set; }
        public string FileName { get; set; }
        public TreeNodeFile EditorRoot { get; set; }
        public bool IsActive { get; set; } = false;
        public bool UseEditMenu { get; set; } = false;
        public int Alignment { get; set; } = 0;
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }
        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public void Load()
        {
            IsActive = true;
            EditorRoot = new GFLXPACK(FileName, this);
            IFileInfo = new IFileInfo();

            ((GFLXPACK)EditorRoot).Read(new FileReader(new MemoryStream(Data)), EditorRoot);

        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            MemoryStream mem = new MemoryStream();
            ((GFLXPACK)EditorRoot).Write(new FileWriter(mem));
            return mem.ToArray();
        }
    }


    public class GFLXPACK : TreeNodeFile
    {
        public GFLXPACK(string text, IFileFormat format)
        {
            Text = text;
            FileHandler = format;

            ContextMenu = new ContextMenu();
            MenuItem save = new MenuItem("Save");
            ContextMenu.MenuItems.Add(save);
            save.Click += Save;
            MenuItem previewFiles = new MenuItem("Preview Window");
            ContextMenu.MenuItems.Add(previewFiles);
            previewFiles.Click += PreviewWindow;
        }
        private void Save(object sender, EventArgs args)
        {
            List<IFileFormat> formats = new List<IFileFormat>();
            formats.Add(FileHandler);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(formats);
            sfd.FileName = FileHandler.FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                SaveCompressFile(FileHandler.Save(), sfd.FileName, FileHandler.IFileInfo.Alignment);
            }
        }
        private void SaveCompressFile(byte[] data, string FileName, int Alignment = 0, bool EnableDialog = true)
        {
            if (EnableDialog)
            {
                DialogResult save = MessageBox.Show("Compress file?", "File Save", MessageBoxButtons.YesNo);

                if (save == DialogResult.Yes)
                    data = EveryFileExplorer.YAZ0.Compress(data, 3, (uint)Alignment);
            }
            File.WriteAllBytes(FileName, data);
            MessageBox.Show($"File has been saved to {FileName}");
            Cursor.Current = Cursors.Default;
        }

        private void PreviewWindow(object sender, EventArgs args)
        {
            PreviewFormatList previewFormatList = new PreviewFormatList();

            if (previewFormatList.ShowDialog() == DialogResult.OK)
            {
                CallRecursive(TreeView);
                PreviewEditor previewWindow = new PreviewEditor();
                previewWindow.Show();
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
            if (treeNode is FileEntry)
            {
                FileEntry file = (FileEntry)treeNode;

                if (file.ImageKey == "bntx")
                    OpenFile(file.Name, file.data, file);

                if (file.ImageKey == "bntx")
                    Console.WriteLine(file.Name);
                //  if (file.ImageKey == "bfres")
                //   OpenFile(file.Name, GetASSTData(file.FullName), TreeView);
            }

            // Print each node recursively.  
            foreach (TreeNode tn in treeNode.Nodes)
            {
                PrintRecursive(tn);
            }
        }

        public ushort BOM;
        public uint Version;
        public List<FileEntry> files = new List<FileEntry>();
        public List<UInt64> hashes = new List<UInt64>();
        public List<HashIndex> hashIndices = new List<HashIndex>();

        public int version;
        public int FolderCount;

        public void Read(FileReader reader, TreeNode root)
        {
            string Signature = reader.ReadString(8, Encoding.ASCII);
            if (Signature != "GFLXPACK")
                throw new Exception($"Invalid signature {Signature}! Expected GFLXPACK.");

            version = reader.ReadInt32();
            uint padding = reader.ReadUInt32();
            uint FileCount = reader.ReadUInt32();
            FolderCount = reader.ReadInt32();
            ulong FileInfoOffset = reader.ReadUInt64();
            ulong hashArrayOffset = reader.ReadUInt64();
            ulong hashArrayIndexOffset = reader.ReadUInt64();

            reader.Seek((long)hashArrayOffset, SeekOrigin.Begin);
            for (int i = 0; i < FileCount; i++)
            {
                ulong hash = reader.ReadUInt64();
                hashes.Add(hash);
            }
            reader.Seek((long)hashArrayIndexOffset, SeekOrigin.Begin);
            for (int i = 0; i < FileCount; i++)
            {
                HashIndex hashindex = new HashIndex();
                hashindex.Read(reader);
                hashIndices.Add(hashindex);
            }

            reader.Seek((long)FileInfoOffset, SeekOrigin.Begin);
            for (int i = 0; i < FileCount; i++)
            {
                FileEntry fileEntry = new FileEntry();
                fileEntry.Read(reader);
                fileEntry.Text = hashes[i].ToString();
                root.Nodes.Add(fileEntry);
                files.Add(fileEntry);
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
            long HashIndexArrOffset = writer.Position;
            writer.Write(0L);
            //Now write all sections
            writer.WriteUint64Offset(HashArrayOffset);
            writer.Write(hashes);

            writer.WriteUint64Offset(HashIndexArrOffset);
            foreach (var hashIndx in hashIndices)
                hashIndx.Write(writer);

            writer.WriteUint64Offset(FileInfoOffset);
            foreach (var fileTbl in files)
                fileTbl.Write(writer);

            //Save data blocks
            foreach (var fileTbl in files)
            {
                fileTbl.WriteBlock(writer);
            }
        }
        public class HashIndex
        {
            public ulong hash;
            public int Index;
            public uint unkown;

            public void Read(FileReader reader)
            {
                hash = reader.ReadUInt64();
                Index = reader.ReadInt32();
                unkown = reader.ReadUInt32(); //Always 0xCC?
            }
            public void Write(FileWriter writer)
            {
                writer.Write(hash);
                writer.Write(Index);
                writer.Write(unkown);
            }
        }
        public class FileEntry : TreeNodeCustom
        {
            public FileEntry()
            {
                ImageKey = "fileBlank";
                SelectedImageKey = "fileBlank";

                ContextMenu = new ContextMenu();
                MenuItem export = new MenuItem("Export");
                ContextMenu.MenuItems.Add(export);
                export.Click += Export;

            }
            public uint unkown;
            public uint CompressionType;
            public byte[] data;
            private long DataOffset;
            private byte[] CompressedData;
            public IFileFormat FileHandler;

            public void Read(FileReader reader)
            {
                unkown = reader.ReadUInt16(); //Usually 9?
                CompressionType = reader.ReadUInt16();
                uint DecompressedFileSize = reader.ReadUInt32();
                uint CompressedFileSize = reader.ReadUInt32();
                uint padding = reader.ReadUInt32();
                ulong FileOffset = reader.ReadUInt64();

                using (reader.TemporarySeek((long)FileOffset, SeekOrigin.Begin))
                {
                    data = reader.ReadBytes((int)CompressedFileSize);
                    data = STLibraryCompression.Type_LZ4.Decompress(data, 0, (int)CompressedFileSize, (int)DecompressedFileSize);

                    string ext = SARCExt.SARC.GuessFileExtension(data);

                    if (ext == ".bntx")
                    {
                        ImageKey = "bntx";
                        SelectedImageKey = "bntx";
                    }
                    if (ext == ".byaml")
                    {
                        ImageKey = "byaml";
                        SelectedImageKey = "byaml";
                    }
                    if (ext == ".aamp")
                    {
                        ImageKey = "aamp";
                        SelectedImageKey = "aamp";
                    }
                    if (ext == ".lua")
                    {

                    }
                }
            }
            public void Write(FileWriter writer)
            {
                if (FileHandler != null && FileHandler.CanSave)
                {
                    data = FileHandler.Save();
                }

                CompressedData = Compress(data, CompressionType);

                writer.Write((ushort)unkown);
                writer.Write((ushort)CompressionType);
                writer.Write(data.Length);
                writer.Write(CompressedData.Length);
                writer.Write(0);
                DataOffset = writer.Position;
                writer.Write(0L);
            }
            public void WriteBlock(FileWriter writer)
            {
                writer.WriteUint64Offset(DataOffset);
                writer.Write(CompressedData);
            }
            public static byte[] Compress(byte[] data, uint Type)
            {
                if (Type == 2)
                {
                    return STLibraryCompression.Type_LZ4.Compress(data);
                }
                else
                    throw new Exception("Unkown compression type?");
            }
            public override void OnClick(TreeView treeview)
            {

            }
            public override void OnMouseLeftClick(TreeView treeView)
            {
                ReplaceNode(this.Parent, this, OpenFile(Name, data, this));
            }
            private void Export(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = Text;
                sfd.Filter = "All files(*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName, data);
                }
            }
        }
        public static TreeNode OpenFile(string FileName, byte[] data, FileEntry fileEntry, bool Compressed = false, CompressionType CompType = 0)
        {
            Cursor.Current = Cursors.WaitCursor;
            FileReader fileReader = new FileReader(data);
            string Magic4 = fileReader.ReadMagic(0, 4);
            string Magic2 = fileReader.ReadMagic(0, 2);
            if (Magic4 == "Yaz0")
            {
                data = EveryFileExplorer.YAZ0.Decompress(data);
                return OpenFile(FileName, data, fileEntry, true, (CompressionType)1);
            }
            if (Magic4 == "ZLIB")
            {
                data = FileReader.InflateZLIB(fileReader.getSection(64, data.Length - 64));
                return OpenFile(FileName, data, fileEntry, true, (CompressionType)2);
            }
            fileReader.Dispose();
            fileReader.Close();
            foreach (IFileFormat fileFormat in FileManager.GetFileFormats())
            {
                if (fileFormat.Magic == Magic4 || fileFormat.Magic == Magic2)
                {
                    fileFormat.CompressionType = CompType;
                    fileFormat.FileIsCompressed = Compressed;
                    fileFormat.Data = data;
                    fileFormat.Load();
                    fileFormat.FileName = Path.GetFileName(FileName);
                    fileFormat.FilePath = FileName;
                    fileFormat.IFileInfo = new IFileInfo();
                    fileFormat.IFileInfo.InArchive = true;
                    if (fileFormat.EditorRoot == null)
                        return null;
                    fileFormat.EditorRoot.ImageKey = fileEntry.ImageKey;
                    fileFormat.EditorRoot.SelectedImageKey = fileEntry.SelectedImageKey;
                    fileFormat.EditorRoot.Text = fileEntry.Text;
                    fileEntry.FileHandler = fileFormat;

                    return fileFormat.EditorRoot;
                }
                if (fileFormat.Magic == string.Empty)
                {
                    foreach (string str3 in fileFormat.Extension)
                    {
                        if (str3.Remove(0, 1) == Path.GetExtension(FileName))
                        {
                            fileFormat.Data = data;
                            fileFormat.Load();
                            fileFormat.FileName = Path.GetFileName(FileName);
                            fileFormat.FilePath = FileName;
                            fileFormat.IFileInfo = new IFileInfo();
                            fileFormat.IFileInfo.InArchive = true;
                            if (fileFormat.EditorRoot == null)
                                return null;
                            fileFormat.EditorRoot.ImageKey = fileEntry.ImageKey;
                            fileFormat.EditorRoot.SelectedImageKey = fileEntry.SelectedImageKey;
                            fileFormat.EditorRoot.Text = fileEntry.Text;
                            fileEntry.FileHandler = fileFormat;

                            return fileFormat.EditorRoot;
                        }
                    }
                }
            }
            return (TreeNode)null;
        }
        public static void ReplaceNode(TreeNode node, TreeNode replaceNode, TreeNode NewNode)
        {
            if (NewNode == null)
                return;

            int index = node.Nodes.IndexOf(replaceNode);
            node.Nodes.RemoveAt(index);
            node.Nodes.Insert(index, NewNode);
        }
    }


    public class FileUnk
    {
        public uint Size;
        public uint Type;
        public uint Width;
        public uint Height;
        public uint Ascend;
        public uint LineFeed;
        public uint AlterCharIndex;
        public uint DefaultLeftWidth;
        public uint DefaultGlyphWidth;
        public uint DefaultCharWidth;
        public uint CharEncoding;
        public TGLP tglp;

        public void Read(FileReader reader)
        {
           
        }
    }
}
