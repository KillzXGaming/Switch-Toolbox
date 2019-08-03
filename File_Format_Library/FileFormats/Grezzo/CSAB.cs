using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Animations;

namespace FirstPlugin
{
    public class CSAB : Animation, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Animation;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "CTR Skeletal Animation Binary" };
        public string[] Extension { get; set; } = new string[] { "*.csab" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "csab");
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

            ImageKey = "";
        }
        public void Unload()
        {

        }

        public byte[] Save()
        {
            return null;
        }

        public enum GameVersion
        {
            OOT3D,
            MM3D,
            LM3DS,
        }

        public class Header
        {
            public GameVersion Version;

            public void Read(FileReader reader)
            {
                reader.CheckSignature(4, "csab");
                uint FileSize = reader.ReadUInt32();
                uint versionNum = reader.ReadUInt32();
                if (versionNum == 5)
                    Version = GameVersion.MM3D;
                else if (versionNum == 3)
                    Version = GameVersion.OOT3D;
                else
                    Version = GameVersion.LM3DS;

                uint padding = reader.ReadUInt32(); //Unsure
                if (Version >= GameVersion.MM3D)
                {
                    uint unknown = reader.ReadUInt32();//0x42200000
                    uint unknown2 = reader.ReadUInt32();//0x42200000
                    uint unknown3 = reader.ReadUInt32();//0x42200000
                }
                uint numAnimations = reader.ReadUInt32(); //Unsure
                uint location = reader.ReadUInt32(); //Unsure
                uint unknown4 = reader.ReadUInt32();//0x00
                uint unknown5 = reader.ReadUInt32();//0x00
                uint unknown6 = reader.ReadUInt32();//0x00
                uint unknown7 = reader.ReadUInt32();//0x00
                uint duration = reader.ReadUInt32();
                uint nodeCount = reader.ReadUInt32();
                uint boneCount = reader.ReadUInt32();
                if (nodeCount != boneCount) throw new Exception("Unexpected bone and node count!");

                ushort[] BoneIndexTable = reader.ReadUInt16s((int)boneCount);
                for (int i = 0; i < boneCount; i++)
                {

                }
            }
        }
    }
}
