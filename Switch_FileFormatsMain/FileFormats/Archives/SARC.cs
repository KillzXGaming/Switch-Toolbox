using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Switch_Toolbox;
using System.Windows.Forms;
using SARCExt;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class SARC : IArchiveFile, IFileFormat
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
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "SARC");
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

        public bool CanAddFiles { get; set; } = true;
        public bool CanRenameFiles { get; set; } = true;
        public bool CanReplaceFiles { get; set; } = true;
        public bool CanDeleteFiles { get; set; } = true;

        public List<SarcEntry> files = new List<SarcEntry>();
        public IEnumerable<ArchiveFileInfo> Files => files;

        public SarcData sarcData;
        public string SarcHash;
        public void Load(System.IO.Stream stream)
        {
            CanSave = true;
            IFileInfo.UseEditMenu = true;

            var SzsFiles = SARCExt.SARC.UnpackRamN(stream);
            sarcData = new SarcData();
            sarcData.HashOnly = SzsFiles.HashOnly;
            sarcData.Files = SzsFiles.Files;
            sarcData.endianness = GetByteOrder(stream);
            SarcHash = Utils.GenerateUniqueHashID();

            foreach (var file in SzsFiles.Files)
                files.Add(SetupFileEntry(file.Key, file.Value));

            sarcData.Files.Clear();
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
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

            string ext = Path.GetExtension(fullName);
            string SarcEx = SARCExt.SARC.GuessFileExtension(data);
            return sarcEntry;
        }
    }
}
