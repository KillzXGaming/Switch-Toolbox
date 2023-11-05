using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library;
using Toolbox.Library.Forms;
using System.Windows.Forms;

namespace FirstPlugin
{
    public class NLG_NLOC : IEditor<TextEditor>, IFileFormat, IContextMenuNode, IConvertableTextFormat
    {
        public FileType FileType { get; set; } = FileType.Effect;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "NLG Localization Text" };
        public string[] Extension { get; set; } = new string[] { "*.nloc", "*.loc" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "NLOC");
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

        public TextEditor OpenForm()
        {
            return new TextEditor();
        }

        public void FillEditor(UserControl control)
        {
            ((TextEditor)control).FileFormat = this;
            ((TextEditor)control).FillEditor(ConvertToString());
        }

        #region Text Converter Interface
        public TextFileType TextFileType => TextFileType.Xml;
        public bool CanConvertBack => false;

        public string ConvertToString()
        {
            StringBuilder strBuilder = new StringBuilder();
            using (var textWriter = new System.IO.StringWriter(strBuilder))
            {
                textWriter.Write($"Language: {header.LanguageText}");
                for (int i = 0; i < header.Entries.Count; i++)
                {
                    textWriter.WriteLine($"ID: [{header.Entries[i].ID}]  [{header.Entries[i].Text}]");
                }
            }
            return strBuilder.ToString();
        }

        public void ConvertFromString(string text)
        {
        }

        #endregion

        private Header header;
        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            header = new Header();
            header.Read(new FileReader(stream));
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            return new ToolStripItem[]
            {
                 new ToolStripMenuItem("Save", null, SaveAction, Keys.Control | Keys.S),
            };
        }

        private void SaveAction(object sender, EventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = Utils.GetAllFilters(typeof(EFF));
            sfd.FileName = FileName;

            if (sfd.ShowDialog() == DialogResult.OK)
                STFileSaver.SaveFileFormat(this, sfd.FileName);
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream) {
            header.Write(new FileWriter(stream));
        }

        public class Header
        {
            public List<TextEntry> Entries = new List<TextEntry>();

            public uint LanguageID;

            public uint Unknown0x4;
            public uint Unknown0x10;

            public string LanguageText;

            public bool IsBigEndian = false;

            public void Read(FileReader reader)
            {
                reader.ReadSignature(4, "NLOC");
                Unknown0x4 = reader.ReadUInt32();
                if (Unknown0x4 != 1)
                {
                    IsBigEndian = true;
                    reader.SetByteOrder(true);
                }

                LanguageID = reader.ReadUInt32();
                uint numEntries = reader.ReadUInt32();
                Unknown0x10 = reader.ReadUInt32();

                var hashes = NLG_Common.HashNames;

                LanguageText = LanguageID.ToString("X");
                if (hashes.ContainsKey(LanguageID))
                    LanguageText = hashes[LanguageID];

                uint stringTableOfs = 0x14 + 8 * numEntries;


                reader.SeekBegin(stringTableOfs);
                List<string> ListStrings = new List<string>();
                for (int i = 0; i < numEntries; i++)
                    ListStrings.Add(reader.ReadUTF16String());

                reader.SeekBegin(0x14);
                for (int i = 0; i < numEntries; i++)
                {
                    TextEntry textEntry = new TextEntry();
                    Entries.Add(textEntry);

                    textEntry.ID = reader.ReadUInt32();
                    uint offset = reader.ReadUInt32();
                    textEntry.Text = ListStrings[i];
                }
            }

            public void Write(FileWriter writer)
            {
                writer.WriteSignature("NLOC");
                writer.SetByteOrder(IsBigEndian);
                writer.Write(Unknown0x4);
                writer.Write(LanguageID);
                writer.Write(Entries.Count);
                writer.Write(Unknown0x10);

                //Write empty data first
                writer.Write(new byte[8 * Entries.Count]);

                //Write string table
                List<uint> positions = new List<uint>();

                uint pos = 0;
                for (int i = 0; i < Entries.Count; i++)
                {
                    positions.Add(pos);

                    long startPos = writer.Position;

                    for (int j = 0; j < Entries[i].Text.Length; j++)
                        writer.Write(Entries[i].Text[j]);
                    writer.Align(4);

                    long endPos = writer.Position;

                    pos += (uint)(endPos - startPos);
                }

                writer.SeekBegin(0x14);
                for (int i = 0; i < Entries.Count; i++)
                {
                    writer.Write(Entries[i].ID);
                    writer.Write(positions[i]);
                }
            }
        }

        public class TextEntry
        {
            public string Text;
            public uint ID;
        }
    }
}
