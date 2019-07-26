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
    //Information on this file from noclip
    //https://github.com/magcius/noclip.website/blob/master/src/oot3d/zsi.ts
    public class ZSI : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Archive;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Zelda Scene Information (OOT3D/MM3D)" };
        public string[] Extension { get; set; } = new string[] { "*.zsi" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool CanAddFiles { get; set; }
        public bool CanRenameFiles { get; set; }
        public bool CanReplaceFiles { get; set; }
        public bool CanDeleteFiles { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "ZSI\x01") || reader.CheckSignature(4, "ZSI\x09");
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

        public GameVersion Version;

        public enum GameVersion
        {
            OOT3D,
            MM3D, 
        }

        public enum HeaderCommands : uint
        {
            Actor = 0x01,
            Collision = 0x03,
            Rooms = 0x04,
            Mesh = 0x0A,
            DoorActor = 0x0E,
            SkyboxSettings = 0x11,
            End = 0x14,
            MultiSetup = 0x18,
            EnvironmentSettings = 0x0F,
        }

        public void Load(System.IO.Stream stream)
        {
            Text = FileName;

            using (var reader = new FileReader(stream))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                string Signature = reader.ReadString(4, Encoding.ASCII);
                switch (Signature)
                {
                    case "ZSI\x01":
                        Version = GameVersion.OOT3D;
                        break;
                    default:
                        Version = GameVersion.MM3D;
                        break;
                }

                string CodeName = reader.ReadString(0x0C);

                ReadScene(reader);
            }
        }

        private void ReadScene(FileReader reader)
        {
            while (true)
            {
                reader.SetByteOrder(true);
                uint cmd1 = reader.ReadUInt32();
                reader.SetByteOrder(false);
                uint cmd2 = reader.ReadUInt32();

                reader.Seek(0x08);

                var cmdType = cmd1 >> 24;

                if (cmdType == (uint)HeaderCommands.End)
                    break;

                switch (cmdType)
                {
                    case (uint)HeaderCommands.EnvironmentSettings:
                        {
                            Nodes.Add("EnvironmentSettings");
                        }
                        break;
                    case (uint)HeaderCommands.DoorActor:
                        {
                            Nodes.Add("DoorActor");
                        }
                        break;
                    case (uint)HeaderCommands.Rooms:
                        {
                            Nodes.Add("Rooms");
                        }
                        break;
                    case (uint)HeaderCommands.SkyboxSettings:
                        {
                            Nodes.Add("SkyboxSettings");
                        }
                        break;
                }
            }
        }

        public void Unload()
        {

        }

        public byte[] Save()
        {
            return null;
        }
    }
}
