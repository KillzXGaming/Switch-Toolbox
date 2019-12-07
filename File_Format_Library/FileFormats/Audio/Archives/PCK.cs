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
    public class PCK : TreeNodeFile, IFileFormat, ILeaveOpenOnLoad
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

        public uint Version = 1;
        public uint Flags;

        public List<LanguageEntry> Languages = new List<LanguageEntry>();
        public List<BankEntry> Banks = new List<BankEntry>();
        public List<AudioEntry> Sounds = new List<AudioEntry>();

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;
            CanSave = false;

            using (var reader = new FileReader(stream, true))
            {
                reader.ReadSignature(4, "AKPK");
                //Total size of language, banks, and sound headers
                uint HeaderSize = reader.ReadUInt32();
                Flags = reader.ReadUInt32();
                uint LanguageSize = reader.ReadUInt32();
                uint BanksSize = reader.ReadUInt32();
                uint SoundsSize = reader.ReadUInt32();

                uint SectionSizes = LanguageSize + BanksSize + SoundsSize + 0x10;
                if (SectionSizes == HeaderSize)
                    Version = 1;
                else {
                    Version = 2;
                    uint Section4 = reader.ReadUInt32();
                }

                //Parse sections

                //Parse language
                long LanguagePos = reader.Position;
                uint LanguageCount = reader.ReadUInt32();
                for (int i = 0; i < LanguageCount; i++)
                {
                    var lang = new LanguageEntry();
                    uint nameOffset = reader.ReadUInt32(); ;
                    lang.ID = reader.ReadUInt32();
                    Languages.Add(lang);

                    using (reader.TemporarySeek(LanguagePos + nameOffset, System.IO.SeekOrigin.Begin)) {
                        lang.Name = reader.ReadZeroTerminatedString();
                    }
                }

                reader.SeekBegin(LanguagePos + LanguageSize);

                //Parse bansk
                long banksPos = reader.Position;
                uint BanksCount = reader.ReadUInt32();
                for (int i = 0; i < BanksCount; i++)
                    Banks.Add(new BankEntry());
                reader.SeekBegin(banksPos + BanksSize);

                //Parse sounds
                long soundPos = reader.Position;
                uint SoundsCount = reader.ReadUInt32();
                Console.WriteLine($"SoundsCount {SoundsCount}");
                for (int i = 0; i < SoundsCount; i++)
                {
                    var entry = new AudioEntry();
                    Nodes.Add(entry);
                    Sounds.Add(entry);

                    entry.HashID = reader.ReadUInt32();
                    entry.Alignment = reader.ReadUInt32();
                    uint size = reader.ReadUInt32();
                    uint offset = reader.ReadUInt32();
                    entry.LanguageID = reader.ReadUInt32();

                    entry.Text = entry.HashID.ToString("X") + ".wem";
                    entry.AudioData = new SubStream(reader.BaseStream, offset * entry.Alignment, size);
                }

                reader.SeekBegin(soundPos + SoundsSize);
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
            using (var writer = new FileWriter(stream)) {
                writer.WriteSignature("AKPK");
                writer.Write(uint.MaxValue); //reserve total header size for later
                writer.Write(Flags);
                long sectionSizesPos = writer.Position;
                writer.Write(uint.MaxValue); //reserve language header size for later
                writer.Write(uint.MaxValue); //reserve bank header size for later
                writer.Write(uint.MaxValue); //reserve sound header size for later
                if (Version >= 2)
                    writer.Write(4);

                long langPos = writer.Position;

                writer.Write(Languages.Count);
                for (int i = 0; i < Languages.Count; i++)
                {
                    writer.Write(0);
                    writer.Write(Languages[i].ID);
                }
                //Write the strings after
                for (int i = 0; i < Languages.Count; i++)
                {
                    writer.WriteUint32Offset(langPos + 4 + (i * 8), langPos);
                    writer.Write(Languages[i].Name);
                }

                //Save language section size
                writer.WriteSectionSizeU32(sectionSizesPos, langPos, writer.Position);


                long banksPos = writer.Position;

                //I don't know any files that use banks yet but todo
                writer.Write(Banks.Count);
                for (int i = 0; i < Banks.Count; i++)
                {
        
                }

                //Save banks section size
                writer.WriteSectionSizeU32(sectionSizesPos + 4, banksPos, writer.Position);


                long soundsPos = writer.Position;

                writer.Write(Sounds.Count);
                for (int i = 0; i < Sounds.Count; i++)
                {
                    writer.Write(Sounds[i].HashID);
                    writer.Write(Sounds[i].Alignment);
                    writer.Write((uint)Sounds[i].AudioData.Length);
                    writer.Write(uint.MaxValue);
                    writer.Write(Sounds[i].LanguageID);
                }

                //Save sounds section size
                writer.WriteSectionSizeU32(sectionSizesPos + 8, soundsPos, writer.Position);

                //Save unknown section as empty
                if (Version >= 2)
                    writer.Write(0);

                //Save sound data
                for (int i = 0; i < Sounds.Count; i++)
                {
                    writer.WriteUint32Offset((soundsPos + 4) + 12 + (i * 20));
                    writer.Write(Sounds[i].AudioData.ToBytes());
                }
            }
        }

        public class LanguageEntry
        {
            public uint ID;
            public string Name;
        }

        public class BankEntry
        {

        }

        public class AudioEntry : TreeNodeCustom, IContextMenuNode
        {
            public AudioEntry()
            {
                ImageKey = "bfwav";
                SelectedImageKey = "bfwav";
            }

            public uint HashID { get; set; }
            public uint Alignment { get; set; }
            public uint LanguageID { get; set; }

            public System.IO.Stream AudioData { get; set; }

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

                if (sfd.ShowDialog() == DialogResult.OK) {
                    AudioData.ExportToFile(sfd.FileName);
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
