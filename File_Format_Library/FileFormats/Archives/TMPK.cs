using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;

namespace FirstPlugin
{

    //Some code based on https://github.com/zeldamods/tmpk/blob/master/tmpk/tmpk.py
    // Copyright 2018 leoetlino <leo@leolam.fr>
    // Licensed under GPLv2+
    // Copy of license can be found under Lib/Licenses

    public class TMPK : IFileFormat, IArchiveFile
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "TMPK" };
        public string[] Extension { get; set; } = new string[] { "*.pack" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "TMPK");
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

        public List<FileInfo> files = new List<FileInfo>();

        public IEnumerable<ArchiveFileInfo> Files => files;

        public bool CanAddFiles { get; set; } = true;
        public bool CanRenameFiles { get; set; } = true;
        public bool CanReplaceFiles { get; set; } = true;
        public bool CanDeleteFiles { get; set; } = true;

        public Dictionary<long, byte[]> SavedDataEntries = new Dictionary<long, byte[]>();
        public Dictionary<long, string> SavedStringEntries = new Dictionary<long, string>();

        public uint Alignment;
        public static readonly uint DefaultAlignment = 4;

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                reader.ReadSignature(4, "TMPK");
                uint FileCount = reader.ReadUInt32();
                Alignment = reader.ReadUInt32();
                uint padding = reader.ReadUInt32();
                for (int i = 0; i < FileCount; i++)
                {
                    var info = new FileInfo(reader);
                    files.Add(info);
                }

                reader.Close();
                reader.Dispose();
            }
        }

        private void Save(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "pack";
            sfd.Filter = "Supported Formats|*.pack;";
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(this, sfd.FileName);
            }
        }

        private void SaveFile(FileWriter writer)
        {
            writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            writer.WriteSignature("TMPK");
            writer.Write(files.Count);
            writer.Write(Alignment);
            writer.Write(0);
            for (int i = 0; i < files.Count; i++)
            {
                files[i].SaveFileFormat();

                files[i]._posHeader = writer.Position;
                writer.Write(uint.MaxValue);
                writer.Write(uint.MaxValue);
                writer.Write(files[i].FileData.Length); //Padding
                writer.Write(0); //Padding
            }
            for (int i = 0; i < files.Count; i++)
            {
                writer.WriteUint32Offset(files[i]._posHeader);
                writer.Write(files[i].FileName, Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);
            }
            for (int i = 0; i < files.Count; i++)
            {
                SetAlignment(writer, files[i].FileName);
                writer.WriteUint32Offset(files[i]._posHeader + 4);

                writer.Write(files[i].FileData);
            }

            writer.Close();
            writer.Dispose();
        }

        private void SetAlignment(FileWriter writer, string FileName)
        {
            if (FileName.EndsWith(".gmx"))
                writer.Align(0x40);
            else if (FileName.EndsWith(".gtx"))
                writer.Align(0x2000);
            else
                writer.Write(DefaultAlignment);
        }

        public class FileInfo : ArchiveFileInfo
        {
            internal long _posHeader;

            public FileInfo()
            {

            }

            public static void ReplaceNode(TreeNode node, TreeNode replaceNode, TreeNode NewNode)
            {
                if (NewNode == null)
                    return;

                int index = node.Nodes.IndexOf(replaceNode);
                node.Nodes.RemoveAt(index);
                node.Nodes.Insert(index, NewNode);
            }

            public FileInfo(FileReader reader)
            {
                long pos = reader.Position;

                uint NameOffset = reader.ReadUInt32();
                uint FileOffset = reader.ReadUInt32();
                uint FileSize = reader.ReadUInt32();
                uint padding = reader.ReadUInt32();

                reader.Seek(NameOffset, System.IO.SeekOrigin.Begin);
                FileName = reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);

                reader.Seek(FileOffset, System.IO.SeekOrigin.Begin);
                FileData = reader.ReadBytes((int)FileSize);
                State = ArchiveFileState.Archived;

                reader.Seek(pos + 16, System.IO.SeekOrigin.Begin);
            }
        }    

        public void Unload()
        {

        }
        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
            SaveFile(new FileWriter(mem));
            return mem.ToArray();
        }


        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            files.Add(new FileInfo()
            {
                FileData = archiveFileInfo.FileData,
                FileName = archiveFileInfo.FileName,
            });

            return true;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {

            return true;
        }
    }
}
