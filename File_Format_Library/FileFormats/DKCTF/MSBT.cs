using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Toolbox.Library;
using Toolbox.Library.IO;
using System.Windows.Forms;
using FirstPlugin.Forms;

namespace DKCTF
{
    public class MSBT : TreeNodeFile, IFileFormat
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
                return reader.CheckSignature(4, "MSBT", 20);
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
            using (var reader = new Toolbox.Library.IO.FileReader(stream))
            {
                reader.SetByteOrder(true);
                var header = reader.ReadStruct<MSBTHeader>();

                //parse the data 
                int index = 0;
                while (!reader.EndOfStream)
                {
                    CChunkDescriptor chunk = reader.ReadStruct<CChunkDescriptor>();
                    long startPos = reader.Position;

                    reader.SeekBegin(startPos + (int)chunk.DataOffset);
                    byte[] chunkData = reader.ReadBytes((int)chunk.DataSize);
                    Nodes.Add(new MessageEntry(chunkData, index++));
                }
            }
        }
        public void Unload()
        {

        }

        public class MessageEntry : TreeNodeCustom
        {
            FirstPlugin.MSBT msbt;

            public MessageEntry(byte[] data, int index)
            {
                Text = $"Message Entry {index}";

                var chunkFile = STFileLoader.OpenFileFormat(Text, data);
                if (chunkFile != null && chunkFile is FirstPlugin.MSBT)
                    msbt = (FirstPlugin.MSBT)chunkFile;
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
