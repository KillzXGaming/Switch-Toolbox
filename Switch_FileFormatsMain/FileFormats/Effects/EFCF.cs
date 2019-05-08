using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using FirstPlugin.Forms;

namespace FirstPlugin
{
    public class EFCF : IFileFormat, IEditor<EffectTableEditor>
    {
        public FileType FileType { get; set; } = FileType.Effect;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Effect Table" };
        public string[] Extension { get; set; } = new string[] { "*.efc" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                if (reader.CheckSignature(4, "EFCF") ||
                     reader.CheckSignature(4, "EFCC"))
                    return true;
                else
                    return false;
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

        public Header EfcHeader;

        public EffectTableEditor OpenForm()
        {
            bool IsDialog = IFileInfo != null && IFileInfo.InArchive;

            var form = new EffectTableEditor();
            form.Text = FileName;
            form.Dock = DockStyle.Fill;
            form.LoadEffectFile(this);

            return form;
        }

        public void Load(System.IO.Stream stream)
        {
            CanSave = true;

            EfcHeader = new Header();
            EfcHeader.Read(new FileReader(stream));
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            var mem = new System.IO.MemoryStream();
            EfcHeader.Write(new FileWriter(mem));
            return mem.ToArray();
        }

        public class Header
        {
            public string Magic { get; set; }
            public uint Version { get; set; }

            public List<Entry> Entries = new List<Entry>();
            public List<string> StringEntries = new List<string>();

            public void Read(FileReader reader)
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                Magic = reader.ReadString(4, Encoding.ASCII);
                if (Magic == "EFCC")
                    reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;

                uint Version = reader.ReadUInt32();
                ushort EntryCount = reader.ReadUInt16();
                ushort StringCount = reader.ReadUInt16();
                ushort SlotSpecificDataCount = reader.ReadUInt16();
                ushort Padding = reader.ReadUInt16();

                for (int i = 0; i < EntryCount; i++)
                {
                    Entry entry = new Entry();
                    entry.Read(reader);
                    Entries.Add(entry);
                }
                for (int i = 0; i < StringCount; i++)
                {
                    StringEntries.Add(reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated));
                    reader.Align(2);
                }
            }

            public void Write(FileWriter writer)
            {
                writer.WriteSignature(Magic);
                if (Magic == "EFCC")
                    writer.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                else
                    writer.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                writer.Write(Version);
                writer.Write((ushort)Entries.Count);
                writer.Write((ushort)StringEntries.Count);
                writer.Write((ushort)0);

                for (int i = 0; i < Entries.Count; i++)
                {
                    Entries[i].Write(writer);
                }
                for (int i = 0; i < StringEntries.Count; i++)
                {
                    writer.Write(StringEntries[i]);
                    writer.Align(2);
                }
            }
        }

        public class Entry
        {
            public uint ActiveTime { get; set; }
            public int PtclStringSlot { get; set; }
            public int StringBankSlot { get; set; }
            public int SlotSpecificPtclData { get; set; }

            public void Read(FileReader reader)
            {
                ActiveTime = reader.ReadUInt32();
                PtclStringSlot = reader.ReadInt32() - 1;
                StringBankSlot = reader.ReadInt32() - 1;
                SlotSpecificPtclData = reader.ReadInt32();
            }

            public void Write(FileWriter writer)
            {
                writer.Write(ActiveTime);
                writer.Write(PtclStringSlot + 1);
                writer.Write(StringBankSlot + 1);
                writer.Write(SlotSpecificPtclData);
            }
        }
    }
}
