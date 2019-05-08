using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Forms;
using LibHac;
using LibHac.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;

namespace FirstPlugin
{

    public class NSP : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Rom;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "NSP" };
        public string[] Extension { get; set; } = new string[] { "*.nsp" };
        public string FileName { get; set; }
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

        Nca Control { get; set; }
        RomfsNodeWrapper romfsWrapper;

        public bool Identify(System.IO.Stream stream)
        {
            return Utils.HasExtension(FileName, ".nsp");
        }

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            string homeFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string KeyFile = Path.Combine(homeFolder, ".switch", "prod.keys");
            string TitleKeyFile = Path.Combine(homeFolder, ".switch", "title.keys");

            var Keys = ExternalKeys.ReadKeyFile(KeyFile, TitleKeyFile);

            Stream Input;

            var Pfs = new Pfs(stream.AsStorage());
            var CnmtNca = new Nca(Keys, Pfs.OpenFile(Pfs.Files.FirstOrDefault(s => s.Name.Contains(".cnmt.nca"))), false);
            var CnmtPfs = new Pfs(CnmtNca.OpenSection(0, false, IntegrityCheckLevel.None, true));
            var Cnmt = new Cnmt(CnmtPfs.OpenFile(CnmtPfs.Files[0]).AsStream());
            var Program = Cnmt.ContentEntries.FirstOrDefault(c => c.Type == CnmtContentType.Program);
            var CtrlEntry = Cnmt.ContentEntries.FirstOrDefault(c => c.Type == CnmtContentType.Control);
            if (CtrlEntry != null)
                Control = new Nca(Keys, Pfs.OpenFile($"{CtrlEntry.NcaId.ToHexString().ToLower()}.nca"), false);
            Input = Pfs.OpenFile($"{Program.NcaId.ToHexString().ToLower()}.nca").AsStream();

            var Nca = new Nca(Keys, Input.AsStorage(), true);

            Romfs romfs = new Romfs(
                     Nca.OpenSection(Nca.Sections.FirstOrDefault
                            (s => s?.Type == SectionType.Romfs || s?.Type == SectionType.Bktr)
                            .SectionNum, false, IntegrityCheckLevel.None, true));

            romfsWrapper = new RomfsNodeWrapper();
            romfsWrapper.romfs = romfs;
            romfsWrapper.FillTreeNodes(Nodes, romfs.RootDir);
        }
 
        public void Unload()
        {
            if (romfsWrapper != null)
            {
                if (romfsWrapper.romfs != null)
                {
                    romfsWrapper.romfs.FileDict.Clear();
                    romfsWrapper.romfs.Files.Clear();
                    romfsWrapper.romfs = null;
                }
            }
            Nodes.Clear();

            Control = null;
        }
        public byte[] Save()
        {
            return null;
        }
    }
}
