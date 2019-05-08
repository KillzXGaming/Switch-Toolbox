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
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class GFPAK : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Graphic Package" };
        public string[] Extension { get; set; } = new string[] { "*.gfpak" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
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

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            Read(new FileReader(stream));

            Text = FileName;

            ContextMenuStrip = new STContextMenuStrip();
            ContextMenuStrip.Items.Add(new STToolStipMenuItem("Save", null, Save, Keys.Control | Keys.S));
            ContextMenuStrip.Items.Add(new STToolStripSeparator());
            ContextMenuStrip.Items.Add(new STToolStipMenuItem("Preview Window", null, PreviewWindow, Keys.Control | Keys.P));

            CanDelete = true;
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            MemoryStream mem = new MemoryStream();
            Write(new FileWriter(mem));
            return mem.ToArray();
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
            // Print each node recursively.  
            foreach (TreeNode tn in treeNode.Nodes)
            {
                PrintRecursive(tn);
            }
        }

        public ushort BOM;
        public uint Version;
        public List<FileEntry> files = new List<FileEntry>();
        public List<Folder> folders = new List<Folder>();

        public List<UInt64> hashes = new List<UInt64>();
        public List<HashIndex> hashIndices = new List<HashIndex>();

        public int version;
        public int FolderCount;

        public void Read(FileReader reader)
        {
            string Signature = reader.ReadString(8, Encoding.ASCII);
            if (Signature != "GFLXPACK")
                throw new Exception($"Invalid signature {Signature}! Expected GFLXPACK.");

            version = reader.ReadInt32();
            uint padding = reader.ReadUInt32();
            uint FileCount = reader.ReadUInt32();
            FolderCount = reader.ReadInt32();
            ulong FileInfoOffset = reader.ReadUInt64();
            ulong hashArrayPathsOffset = reader.ReadUInt64();
            ulong FolderArrayOffset = reader.ReadUInt64();

            reader.Seek((long)FolderArrayOffset, SeekOrigin.Begin);
            for (int i = 0; i < FolderCount; i++)
            {
                Folder folder = new Folder();
                folder.Read(reader);
                folders.Add(folder);
            }

            reader.Seek((long)hashArrayPathsOffset, SeekOrigin.Begin);
            for (int i = 0; i < FileCount; i++)
            {
                ulong hash = reader.ReadUInt64();
                hashes.Add(hash);
            }

            reader.Seek((long)FileInfoOffset, SeekOrigin.Begin);
            for (int i = 0; i < FileCount; i++)
            {
                FileEntry fileEntry = new FileEntry();
                fileEntry.Read(reader);
                fileEntry.Text = hashes[i].ToString();
                Nodes.Add(fileEntry);
                files.Add(fileEntry);
            }

            reader.Close();
            reader.Dispose();
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
            writer.Write(hashes);

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
            public uint FileCount;
            public uint unknown;

            public List<HashIndex> hashes = new List<HashIndex>();

            public void Read(FileReader reader)
            {
                hash = reader.ReadUInt64();
                FileCount = reader.ReadUInt32();
                unknown = reader.ReadUInt32();

                for (int f = 0; f < FileCount; f++)
                {
                    HashIndex hash = new HashIndex();
                    hash.Read(reader);
                    hashes.Add(hash);
                }
            }
            public void Write(FileWriter writer)
            {
                writer.Write(hash);
                writer.Write(FileCount);
                writer.Write(unknown);

                foreach (var hash in hashes)
                    hash.Write(writer);
            }
        }

        public class HashIndex
        {
            public ulong hash;
            public int Index;
            public uint unknown;

            public void Read(FileReader reader)
            {
                hash = reader.ReadUInt64();
                Index = reader.ReadInt32();
                unknown = reader.ReadUInt32(); //Always 0xCC?
            }
            public void Write(FileWriter writer)
            {
                writer.Write(hash);
                writer.Write(Index);
                writer.Write(unknown);
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
            public IFileFormat FileHandler;

            public uint CompressedFileSize;
            public uint padding;

            public void Read(FileReader reader)
            {
                unkown = reader.ReadUInt16(); //Usually 9?
                CompressionType = reader.ReadUInt16();
                uint DecompressedFileSize = reader.ReadUInt32();
                CompressedFileSize = reader.ReadUInt32();
                padding = reader.ReadUInt32();
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

            byte[] CompressedData;
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
                writer.Write(padding);
                DataOffset = writer.Position;
                writer.Write(0L);
            }
            public void WriteBlock(FileWriter writer)
            {
                writer.Align(16);
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
            public override void OnClick(TreeView treeView)
            {
                HexEditor editor = (HexEditor)LibraryGUI.Instance.GetActiveContent(typeof(HexEditor));
                if (editor == null)
                {
                    editor = new HexEditor();
                    LibraryGUI.Instance.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.LoadData(data);
            }

            public override void OnDoubleMouseClick(TreeView treeView)
            {
                FileHandler = STFileLoader.OpenFileFormat(Name, data,false, true, this);


                if (FileHandler != null && FileHandler is TreeNode)
                    ReplaceNode(this.Parent, this, (TreeNode)FileHandler);
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
        public static void ReplaceNode(TreeNode node, TreeNode replaceNode, TreeNode NewNode)
        {
            if (NewNode == null)
                return;

            int index = node.Nodes.IndexOf(replaceNode);
            node.Nodes.RemoveAt(index);
            node.Nodes.Insert(index, NewNode);
        }
    }
}
