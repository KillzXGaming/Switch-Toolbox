using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class BFLYT : IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Cafe Layout (GUI)" };
        public string[] Extension { get; set; } = new string[] { "*.bflyt" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "FLYT");
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
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }

        public class Header
        {
            private const string Magic = "FLYT";

            private ushort ByteOrderMark;
            private ushort HeaderSize;

            public uint Version;

            public void Read(FileReader reader)
            {
                reader.ReadSignature(4, Magic);
                ByteOrderMark = reader.ReadUInt16();
                reader.CheckByteOrderMark(ByteOrderMark);
                HeaderSize = reader.ReadUInt16();
                Version = reader.ReadUInt32();
                uint FileSize = reader.ReadUInt32();
                ushort sectionCount = reader.ReadUInt16();
                reader.ReadUInt16(); //Padding

                reader.SeekBegin(HeaderSize);
                for (int i = 0; i < sectionCount; i++)
                {
                    long pos = reader.Position;

                    string Signature = reader.ReadString(4, Encoding.ASCII);
                    uint SectionSize = reader.ReadUInt32();

                    SectionCommon section = new SectionCommon();

                    switch (Signature)
                    {
                        //If the section is not supported store the raw bytes
                        default:
                            section.Data = reader.ReadBytes((int)SectionSize);
                            break;
                    }

                    section.Signature = Signature;
                    section.SectionSize = SectionSize;

                    reader.SeekBegin(pos + SectionSize + 0x10);
                    reader.Align(16);
                }
            }

            public void Write(FileWriter writer)
            {
                writer.WriteSignature(Magic);
                writer.Write(ByteOrderMark);
                writer.Write(HeaderSize);
                writer.Write(Version);
                writer.Write(uint.MaxValue); //Reserve space for file size later
                writer.Write(ushort.MaxValue); //Reserve space for section count later
                writer.Seek(2); //padding




                //Write the total file size
                using (writer.TemporarySeek(0x0C, System.IO.SeekOrigin.Begin))
                {
                    writer.Write((uint)writer.BaseStream.Length);
                }
            }
        }

        public class LYT1 : SectionCommon
        {
        }

        public class SectionCommon
        {
            internal string Signature { get; set; }
            internal uint SectionSize { get; set; }

            internal byte[] Data { get; set; }

            public void Write(FileWriter writer)
            {
                writer.WriteSignature(Signature);
                if (Data != null)
                {
                    writer.Write(Data.Length);
                    writer.Write(Data);
                }
            }
        }
    }
}
