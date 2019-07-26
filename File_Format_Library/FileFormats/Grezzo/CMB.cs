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
    public class CMB : IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Layout;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "CMB" };
        public string[] Extension { get; set; } = new string[] { "*.cmb" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "cmb ");
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

        public Header header;

        public void Load(System.IO.Stream stream)
        {
            header = new Header();
            header.Read(new FileReader(stream));
        }
        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }

        public enum CMBVersion
        {
            OOT3DS,
            MM3DS,
            LM3DS,
        }

        public class Header
        {
            public string Name { get; set; }

            public CMBVersion Version;

            public void Read(FileReader reader)
            {
                string magic = reader.ReadSignature(4, "cmb ");
                uint FileSize = reader.ReadUInt32();
                uint ChunkCount = reader.ReadUInt32();
                uint Unknown = reader.ReadUInt32();

                Name = reader.ReadString(0x10).TrimEnd('\0');
                    
                //Check the chunk count used by the game
                if (ChunkCount == 0x0F)
                    Version = CMBVersion.LM3DS;
                else if (ChunkCount == 0x0A)
                    Version = CMBVersion.MM3DS;
                else if (ChunkCount == 0x06)
                    Version = CMBVersion.OOT3DS;
                else
                    throw new Exception("Unexpected chunk count! " + ChunkCount);

                int chunkIdx = 0x04;
            }
        }
    }
}
