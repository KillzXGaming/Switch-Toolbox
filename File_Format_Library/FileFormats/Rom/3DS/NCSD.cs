using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using CTR.NCCH;

namespace FirstPlugin
{
    //Documentation from https://www.3dbrew.org/wiki/NCSD
    public class NCSD : IArchiveFile, IFileFormat, ILeaveOpenOnLoad
    {
        private System.IO.Stream _stream = null;

        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "CTR Cart Image"};
        public string[] Extension { get; set; } = new string[] { "*.cci", "*.3ds" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "NCSD", 0x100);
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

        public List<RomfsFileEntry> files = new List<RomfsFileEntry>();
        public IEnumerable<ArchiveFileInfo> Files => files;
        public void ClearFiles() { files.Clear(); }

        public NCCH ncch;
        private CTR.NCSD.Header header;

        const int MediaUnitSize = 0x200;

        public void Load(System.IO.Stream stream)
        {
            _stream = stream;
            using (var reader = new FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                header = reader.ReadStruct<CTR.NCSD.Header>();

                for (int i = 0; i < header.Parts.Length; i++)
                {
                    if (header.Parts[i].Offset != 0)
                    {
                        string name;
                        if (PartNames.ContainsKey(i))
                            name = PartNames[i];
                        else
                            name = "Unknown.cfa";

                        //Load the cxi
                        if (i == 0)
                        {
                            ncch = new NCCH();
                            ncch.FileName = name;
                            ncch.IFileInfo = new IFileInfo();
                            ncch.IFileInfo.InArchive = true;

                            Console.WriteLine("Length NCCH " + reader.BaseStream.Length);

                            ncch.Load(new SubStream(stream,
                                header.Parts[i].Offset * MediaUnitSize, 
                                header.Parts[i].Size * MediaUnitSize));

                            files.AddRange(ncch.files);
                        }
                    }
                }
            }
        }

        public Dictionary<int, string> PartNames = new Dictionary<int, string>()
        {
            {0, "GameData.cxi" },
            {1, "EManual.cfa" },
            {2, "DLP.cfa" },
            {6, "FirmwareUpdate.cfa" },
            {7, "UpdateData.cfa" },
        };

        public void Unload()
        {
            _stream?.Dispose();
            _stream?.Close();
            _stream = null;
        }

        public void Save(System.IO.Stream stream)
        {
        }

        public bool AddFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }

        public bool DeleteFile(ArchiveFileInfo archiveFileInfo)
        {
            return false;
        }
    }
}
