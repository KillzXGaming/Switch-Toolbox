using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using LayoutBXLYT.GCBLO;

namespace LayoutBXLYT
{
    public class BLO : IFileFormat, IEditorForm<LayoutEditor>
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "GC Layout File" };
        public string[] Extension { get; set; } = new string[] { "*.bl0" };
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
                return reader.CheckSignature(4, "SCRN");
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

        public LayoutEditor OpenForm()
        {
            LayoutEditor editor = new LayoutEditor();
            editor.Dock = DockStyle.Fill;
            editor.LoadBxlyt(Header);
            return editor;
        }

        public void FillEditor(Form control)
        {
            ((LayoutEditor)control).LoadBxlyt(Header);
        }

        public BLOHeader Header;

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream)) {
                Header = new BLOHeader();
                Header.Read(reader, this);
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }
    }
}
