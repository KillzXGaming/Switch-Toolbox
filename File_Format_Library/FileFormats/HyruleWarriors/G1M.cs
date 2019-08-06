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
    public class G1M : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "G1M Model" };
        public string[] Extension { get; set; } = new string[] { "*.g1m" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "_M1G") || reader.CheckSignature(4, "G1M_");
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
            Read(new FileReader(stream));
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        public void Read(FileReader reader)
        {
            reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;
            string Magic = reader.ReadString(4);

            if (Magic == "_M1G")
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
            else
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

            uint Version = reader.ReadUInt32();
            uint FileSize = reader.ReadUInt32();
            uint firstChunkOffset = reader.ReadUInt32();
            uint padding = reader.ReadUInt32();
            uint numChunks = reader.ReadUInt32();

            switch (Version)
            {
                case 0x30303334:break;
                case 0x30303335: break;
                case 0x30303336: break;
                case 0x30303337: break;
                default: break;
            }

            reader.SeekBegin(firstChunkOffset);
            for (int i =0; i < numChunks; i++)
            {
                long chunkPos = reader.Position;
                string chunkMagic = reader.ReadString(4, Encoding.ASCII);
                uint version = reader.ReadUInt32();
                uint chunkSize = reader.ReadUInt32();

                if (chunkMagic == "G1MF")
                {

                }
                else if (chunkMagic == "G1MS")
                {

                }
                else if (chunkMagic == "G1MM")
                {

                }
                else if (chunkMagic == "G1MG")
                {

                }
                else if (chunkMagic == "COLL")
                {

                }
                else if (chunkMagic == "HAIR")
                {

                }
                else if (chunkMagic == "NUNO")
                {

                }
                else if (chunkMagic == "NUNS")
                {

                }
                else if (chunkMagic == "NUNV")
                {

                }
                else if (chunkMagic == "EXTR")
                {

                }
                else
                {

                }

                reader.SeekBegin(chunkPos + chunkSize);
            }
        }
    }
}
