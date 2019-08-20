using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;
using System.IO;
using System.Windows.Forms;
using FirstPlugin.Forms;

namespace DKCTF
{
    public class MSBT : TreeNodeFile, IFileFormat, ILeaveOpenOnLoad
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "MSBT (DKCTF)" };
        public string[] Extension { get; set; } = new string[] { "*.MSBT" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            if (stream.Length <= 24)
                return false;

            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                bool IsForm = reader.CheckSignature(4, "RFRM");
                bool FormType = reader.CheckSignature(4, "MSBT", 20);

                return IsForm && FormType;
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

        private Stream _stream;
        public void Load(System.IO.Stream stream)
        {
            _stream = stream;
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.SetByteOrder(true);
                var header = reader.ReadStruct<MSBTHeader>();

                Text = FileName;

                //parse the data 
                int index = 0;
                while (!reader.EndOfStream)
                {
                    CChunkDescriptor chunk = reader.ReadStruct<CChunkDescriptor>();
                    long startPos = reader.Position;

                    reader.SeekBegin(startPos + (int)chunk.DataOffset);
                    var subStream = new SubStream(_stream, reader.Position, (long)chunk.DataSize);
                    Nodes.Add(new MessageEntry(subStream, index++, chunk.ChunkType));

                    reader.SeekBegin(startPos + (int)chunk.DataOffset + (int)chunk.DataSize);
                }
            }
        }
        public void Unload()
        {

        }

        public class MessageEntry : TreeNodeCustom, IContextMenuNode
        {
            FirstPlugin.MSBT msbt;

            private Stream stream;
            public MessageEntry(Stream data, int index, string type)
            {
                stream = data;

                Text = $"{type}.msbt";

                var chunkFile = STFileLoader.OpenFileFormat(data, Text);
                if (chunkFile != null && chunkFile is FirstPlugin.MSBT)
                    msbt = (FirstPlugin.MSBT)chunkFile;
            }

            public ToolStripItem[] GetContextMenuItems()
            {
                List<ToolStripItem> Items = new List<ToolStripItem>();
                Items.Add(new STToolStipMenuItem("Export", null, ExportAction, Keys.Control | Keys.E));
                return Items.ToArray();
            }

            private void ExportAction(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = Utils.GetAllFilters(typeof(MSBT));
                sfd.FileName = Text;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    stream.ExportToFile(sfd.FileName);
                }
            }

            public override void OnClick(TreeView treeview)
            {
                if (msbt != null)
                {
                    MSBTEditor editor = (MSBTEditor)LibraryGUI.GetActiveContent(typeof(MSBTEditor));
                    if (editor == null)
                    {
                        editor = new MSBTEditor();
                        editor.Dock = DockStyle.Fill;
                        LibraryGUI.LoadEditor(editor);
                    }

                    editor.LoadMSBT(msbt);
                }
            }
        }

        public void Save(System.IO.Stream stream)
        {
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class MSBTHeader
    {
        CFormDescriptor PackForm;
    }
}
