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
    public class BARSLIST : IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Audio;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Audio Archive List" };
        public string[] Extension { get; set; } = new string[] { "*.barslist" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "ARSL");
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

        public class BinaryData
        {
            public ushort Version;

            public void Read(FileReader reader)
            {
                string Signature = reader.ReadSignature(4, "ARSL");
                ushort bom = reader.ReadUInt16();
                reader.CheckByteOrderMark(bom);
                Version = reader.ReadUInt16();
                uint padding = reader.ReadUInt32();
                uint entryCount = reader.ReadUInt32();

            }
        }

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                BinaryData data = new BinaryData();

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
