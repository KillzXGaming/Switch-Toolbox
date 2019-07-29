using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.Windows.Forms; 

namespace FirstPlugin
{
    class NARC : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Nitro Archive (NARC)" };
        public string[] Extension { get; set; } = new string[] { "*.narc" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "NARC");
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

        public class FileEntry : TreeNodeCustom
        {
            public byte[] FileData { get; set; }
            public NARC.FileAllocationEntry entry;
            public FileImageBlock fileImage;

            public FileEntry(string Name)
            {
                Text = Name.Replace(" ", string.Empty).RemoveIllegaleFolderNameCharacters();

                if (Utils.HasExtension(Name, ".cbfmd") ||
                    Utils.HasExtension(Name, ".cbfa") ||
                    Utils.HasExtension(Name, ".cbfsa"))
                {
                    ImageKey = "bfres";
                    SelectedImageKey = "bfres";
                }
                if (Utils.HasExtension(Name, ".cbntx"))
                {
                    ImageKey = "bntx";
                    SelectedImageKey = "bntx";
                }

                ContextMenuStrip = new STContextMenuStrip();
                ContextMenuStrip.Items.Add(new STToolStipMenuItem("Export Raw Data", null, Export, Keys.Control | Keys.E));
            }
            private void Export(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = Text;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllBytes(sfd.FileName, DecompressBlock());
                }
            }

            public override void OnDoubleMouseClick(TreeView treeView)
            {
                TreeNode node = STFileLoader.GetNodeFileFormat(Text, DecompressBlock(), true, this);

                if (node != null)
                    ReplaceNode(this.Parent, this, node);
            }

            public static void ReplaceNode(TreeNode node, TreeNode replaceNode, TreeNode NewNode)
            {
                if (NewNode == null)
                    return;

                int index = node.Nodes.IndexOf(replaceNode);
                node.Nodes.RemoveAt(index);
                node.Nodes.Insert(index, NewNode);
            }

            public override void OnClick(TreeView treeView)
            {
                HexEditor editor = (HexEditor)LibraryGUI.GetActiveContent(typeof(HexEditor));
                if (editor == null)
                {
                    editor = new HexEditor();
                    LibraryGUI.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.LoadData(DecompressBlock());
            }

            public byte[] DecompressBlock()
            {
                byte[] data = GetBlock();

                var reader = new FileReader(data);
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                byte compType = reader.ReadByte();

                if (compType == 0x50)
                {
                    reader.Seek(4, System.IO.SeekOrigin.Begin);
                    uint decompSize = reader.ReadUInt32();
                    uint compSize = (uint)reader.BaseStream.Length - 8;

                    var comp = new STLibraryCompression.MTA_CUSTOM();
                    return comp.Decompress(data, decompSize);
                }
                else if (compType == 0x30 || compType == 0x20)
                {
                    uint decompSize = reader.ReadUInt32();
                    uint compSize = (uint)reader.BaseStream.Length - 14;

                    reader.SeekBegin(14);
                    bool IsGZIP = reader.ReadUInt16() == 0x1F8B;

                    byte[] filedata = reader.getSection(14, (int)compSize);
                    reader.Close();
                    reader.Dispose();

                    if (IsGZIP)
                        data = STLibraryCompression.GZIP.Decompress(filedata);
                    else
                        data = STLibraryCompression.ZLIB.Decompress(filedata);
                }

                return data;
            }

            public byte[] GetBlock()
            {
                return Utils.SubArray(fileImage.dataBlock, entry.StartOffset, entry.EndOffset);
            }
        }

        Header header;

        public List<FileEntry> FileEntries = new List<FileEntry>();

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            header = new Header(new FileReader(stream));
            var names = GetNames(header.FNTB);

            List<byte> Data = new List<byte>();
            for (ushort i = 0; i < header.FATB.FileCount; i++)
            {
                FileEntries.Add(new FileEntry(names[i])
                {
                    entry = header.FATB.FileEntries[i],
                    fileImage = header.FIMG,
                });
                Nodes.Add(FileEntries[i]);
            }
        }
        public List<string> GetNames(FileNameTable nameTable)
        {
            var names = new List<string>();
            foreach (var tblEntry in nameTable.entryNameTable)
            {
                if (tblEntry is EntryNameTableFileEntry)
                    names.Add(((EntryNameTableFileEntry)tblEntry).entryName);
            }
            return names;
        }

        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }

        //EFE for REing format https://github.com/Gericom/EveryFileExplorer/blob/f9f00d193c9608d71c9a23d9f3ab7e752f4ada2a/NDS/NitroSystem/FND/NARC.cs

       
        public class Header
        {
            public string Signature;
            public ushort ByteOrder;
            public uint FileSize;
            public uint Version;
            public ushort HeaderSize;
            public ushort DataBlocks;

            public FileAllocationTableBlock FATB;
            public FileNameTable FNTB;
            public FileImageBlock FIMG;

            public Header(FileReader reader)
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                reader.ReadSignature(4, "NARC");

                ByteOrder = reader.ReadUInt16();
                reader.CheckByteOrderMark(ByteOrder);

                Version = reader.ReadUInt16();
                FileSize = reader.ReadUInt32();
                HeaderSize = reader.ReadUInt16();
                DataBlocks = reader.ReadUInt16();

                FATB = new FileAllocationTableBlock(reader);
                FNTB = new FileNameTable(reader);
                FIMG = new FileImageBlock(reader);

            }
            public void Write(FileWriter writer)
            {
                writer.WriteSignature(Signature);
                writer.Write(ByteOrder);
                writer.Write(Version);
                writer.Write(FileSize);
                writer.Write(HeaderSize);
                writer.Write(DataBlocks);
            }
        }
        public class FileAllocationTableBlock
        {
            public string Signature;
            public uint Size;
            public ushort FileCount;
            public ushort Reserved;

            public List<FileAllocationEntry> FileEntries = new List<FileAllocationEntry>();

            public FileAllocationTableBlock(FileReader reader)
            {
                long startPos = reader.Position;

                reader.ReadSignature(4, "BTAF");

                Size = reader.ReadUInt32();
                FileCount = reader.ReadUInt16();
                Reserved = reader.ReadUInt16();
                for (int i = 0; i < FileCount; i++)
                    FileEntries.Add(new FileAllocationEntry(reader));

                reader.Seek(startPos + Size, System.IO.SeekOrigin.Begin);
            }
        }
        public class FileAllocationEntry
        {
            public uint StartOffset;
            public uint EndOffset;

            public FileAllocationEntry(FileReader reader)
            {
                StartOffset = reader.ReadUInt32();
                EndOffset = reader.ReadUInt32();
            }
            public void Write(FileWriter writer)
            {
                writer.Write(StartOffset);
                writer.Write(EndOffset);
            }
        }
        public class FileImageBlock
        {
            public string Signature;
            public uint Size;
            public byte[] dataBlock;

            public FileImageBlock(FileReader reader)
            {
                reader.ReadSignature(4, "GMIF");

                Size = reader.ReadUInt32();
                dataBlock = reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position));
            }
        }          
        public class FileNameTable
        {
            public string Signature;
            public uint Size;

            public List<DirectoryTableEntry> directoryTable = new List<DirectoryTableEntry>();
            public List<EntryNameTableEntry> entryNameTable = new List<EntryNameTableEntry>();

            public FileNameTable(FileReader reader)
            {
                long startPos = reader.BaseStream.Position;

                reader.ReadSignature(4, "BTNF");
                Size = reader.ReadUInt32();

                directoryTable.Add(new DirectoryTableEntry(reader));

                for (int i = 0; i < directoryTable[0].dirParentID - 1; i++)
                {
                    directoryTable.Add(new DirectoryTableEntry(reader));
                }
                entryNameTable = new List<EntryNameTableEntry>();
                int EndOfDirectory = 0;
                while (EndOfDirectory < directoryTable[0].dirParentID)
                {
                    byte entryNameLength = reader.ReadByte();
                    reader.BaseStream.Position--;
                    if (entryNameLength == 0)
                    {
                        entryNameTable.Add(new EntryNameTableEndOfDirectoryEntry(reader));
                        EndOfDirectory++;
                    }
                    else if (entryNameLength < 0x80) entryNameTable.Add(new EntryNameTableFileEntry(reader));
                    else entryNameTable.Add(new EntryNameTableDirectoryEntry(reader));
                }

                reader.BaseStream.Position = startPos + Size;
            }
        }

        public class EntryNameTableEndOfDirectoryEntry : EntryNameTableEntry
        {
            public EntryNameTableEndOfDirectoryEntry() { }
            public EntryNameTableEndOfDirectoryEntry(FileReader reader)
                : base(reader) { }
            public override void Write(FileWriter writer)
            {
                base.Write(writer);
            }
        }

        public class EntryNameTableDirectoryEntry : EntryNameTableEntry
        {
            public string entryName;
            public ushort directoryID;

            public EntryNameTableDirectoryEntry(FileReader reader) : base(reader)
            {
                entryName = reader.ReadString(entryNameLength & 0x7F, Encoding.ASCII);
                directoryID = reader.ReadUInt16();
            }
            public override void Write(FileWriter writer)
            {
                base.Write(writer);
                writer.Write(entryName, Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);
                writer.Write(directoryID);
            }
        }

        public class EntryNameTableFileEntry : EntryNameTableEntry
        {
            public string entryName;

            public EntryNameTableFileEntry(FileReader reader) : base(reader)
            {
                entryName = reader.ReadString(entryNameLength, Encoding.ASCII);
            }
            public override void Write(FileWriter writer)
            {
                writer.Write(entryName, Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);
            }
        }

        public class EntryNameTableEntry
        {
            public byte entryNameLength;

            protected EntryNameTableEntry() { }
            public EntryNameTableEntry(FileReader reader)
            {
                entryNameLength = reader.ReadByte();
            }
            public virtual void Write(FileWriter writer)
            {
                writer.Write(entryNameLength);
            }
        }

        public class DirectoryTableEntry
        {
            public uint dirEntryStart;
            public ushort dirEntryFileID;
            public ushort dirParentID;

            public DirectoryTableEntry() { }
            public DirectoryTableEntry(FileReader reader)
            {
                dirEntryStart = reader.ReadUInt32();
                dirEntryFileID = reader.ReadUInt16();
                dirParentID = reader.ReadUInt16();
            }
            public void Write(FileWriter writer)
            {
                writer.Write(dirEntryStart);
                writer.Write(dirEntryFileID);
                writer.Write(dirParentID);
            }
        }
    }
}
