using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Forms;

namespace FirstPlugin
{
    public class NLG_PCK : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Audio Archive" };
        public string[] Extension { get; set; } = new string[] { "*.pck" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "AKPK");
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

        public List<AudioEntry> Entries = new List<AudioEntry>();

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            using (var reader = new FileReader(stream))
            {
                uint magic = reader.ReadUInt32();
                uint audioInfoSize = reader.ReadUInt32();

                reader.SeekBegin(48);
                uint numEntries = reader.ReadUInt32();

                uint audioOffset = audioInfoSize + 12;
                for (int i = 0; i < numEntries; i++)
                {
                    AudioEntry entry = new AudioEntry();
                    entry.ImageKey = "bfwav";
                    entry.SelectedImageKey = "bfwav";
                    Nodes.Add(entry);

                    entry.HashID = reader.ReadUInt32();
                    uint alignment = reader.ReadUInt32(); 
                    uint AudioSize = reader.ReadUInt32();
                    uint unk = reader.ReadUInt32();
                    reader.ReadUInt32(); //0

                    entry.Text = entry.HashID.ToString("X") + ".wav";

                    using (reader.TemporarySeek(audioOffset, System.IO.SeekOrigin.Begin))
                    {
                        reader.Align((int)alignment);
                        entry.AudioData = reader.ReadBytes((int)AudioSize);
                        audioOffset = (uint)reader.Position;
                    }
                }
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        public class AudioEntry : TreeNodeCustom, IContextMenuNode
        {
            public uint HashID { get; set; }

            public byte[] AudioData { get; set; }

            public ToolStripItem[] GetContextMenuItems()
            {
                List<ToolStripItem> Items = new List<ToolStripItem>();
                Items.Add(new ToolStripMenuItem("Export", null, ExportAction, Keys.Control | Keys.E));
                return Items.ToArray();
            }

            private void ExportAction(object sender, EventArgs args)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = Text;
                sfd.Filter = "Raw Data (*.*)|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    System.IO.File.WriteAllBytes(sfd.FileName, AudioData);
                }
            }

            public override void OnClick(TreeView treeview)
            {
                HexEditor editor = (HexEditor)LibraryGUI.GetActiveContent(typeof(HexEditor));
                if (editor == null)
                {
                    editor = new HexEditor();
                    LibraryGUI.LoadEditor(editor);
                }
                editor.Text = Text;
                editor.Dock = DockStyle.Fill;
                editor.LoadData(AudioData);
            }
        }
    }
}
