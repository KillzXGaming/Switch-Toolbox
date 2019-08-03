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

                var Rooms = ReadRoomHeaders(reader, Version);
                foreach (var room in Rooms)
                    LoadRooms(room, this);

                //   ReadSceneHeaders(reader, Version);
            }
        }

        private void LoadRooms(RoomSetup roomSetup, TreeNode parentNode)
        {
            TreeNode RoomNode = new TreeNode("Room");
            parentNode.Nodes.Add(RoomNode);

            foreach (var mesh in roomSetup.Meshes)
                RoomNode.Nodes.Add(mesh);

            foreach (var room in roomSetup.SubSetups)
                LoadRooms(room, parentNode);
        }

        public class Scene
        {
            public List<RoomSetup> RoomSetups = new List<RoomSetup>();
            public List<EnvironmentSettings> EnvironmentSettings = new List<EnvironmentSettings>();
            public List<Actor> Doors = new List<Actor>();
            public List<string> Rooms = new List<string>();
        }


        private Scene ReadSceneHeaders(FileReader reader, GameVersion version)
        {
            Scene scene = new Scene();

            int offset = 0;
            long pos = reader.Position;
            while (true)
            {
                reader.SeekBegin(pos + offset);

                offset += 8;

                reader.SetByteOrder(true);
                uint cmd1 = reader.ReadUInt32();
                reader.SetByteOrder(false);
                uint cmd2 = reader.ReadUInt32();

                var cmdType = cmd1 >> 24;

                if (cmdType == (uint)HeaderCommands.End)
                    break;

                reader.SeekBegin(pos + cmd2);
                switch (cmdType)
                {
                    case (uint)HeaderCommands.EnvironmentSettings:
                        int numEnvironmentSettings = ((int)cmd1 >> 16) & 0xFF;
                        scene.EnvironmentSettings = ReadEnvironmentSettings(reader, version, numEnvironmentSettings);
                        break;
                    case (uint)HeaderCommands.DoorActor:
                        int numDoorActors = ((int)cmd1 >> 16) & 0xFF;
                        scene.Doors = ReadDoorActors(reader, version, numDoorActors);
                        break;
                    case (uint)HeaderCommands.Rooms:
                        int numRooms = ((int)cmd1 >> 16) & 0xFF;
                        scene.Rooms = ReadRooms(reader, version, numRooms);
                        break;
                    case (uint)HeaderCommands.SkyboxSettings:
                        break;
                }
            }

            return scene;
        }

        public List<EnvironmentSettings> ReadEnvironmentSettings(FileReader reader, GameVersion version, int numSettings)
        {
            List<EnvironmentSettings> settings = new List<EnvironmentSettings>();

            for (int i = 0; i < numSettings; i++)
            {
                EnvironmentSettings setting = new EnvironmentSettings();
                settings.Add(setting);
            }
            return settings;
        }

        public List<Actor> ReadDoorActors(FileReader reader, GameVersion version, int numActors)
        {
            List<Actor> actors = new List<Actor>();

            for (int i = 0; i < numActors; i++)
            {
                Actor actor = new Actor();
                actor.RoomFront = reader.ReadByte();
                actor.TransitionEffectFront = reader.ReadByte();
                actor.RoomBack = reader.ReadByte();
                actor.TransitionEffectBack = reader.ReadByte();
                actor.ActorID = reader.ReadUInt16();
                actor.PositionX = reader.ReadUInt16();
                actor.PositionY = reader.ReadUInt16();
                actor.PositionZ = reader.ReadUInt16();
                actor.RotationY = reader.ReadUInt16();
                actor.Variable = reader.ReadUInt16();
                actors.Add(actor);
            }
            return actors;
        }

        private List<string> ReadRooms(FileReader reader, GameVersion version, int numRooms)
        {
            List<string> rooms = new List<string>();
            var roomSize = version == GameVersion.OOT3D ? 0x44 : 0x34;

            long pos = reader.Position;
            for (int i = 0; i < numRooms; i++)
            {
                reader.SeekBegin(pos + (i * roomSize));
                rooms.Add(reader.ReadZeroTerminatedString());
            }
            return rooms;
        }

     /*   private List<RoomSetup> ReadRooms(FileReader reader, GameVersion version, int numRooms)
        {
            List<RoomSetup> rooms = new List<RoomSetup>();
            var roomSize = version == GameVersion.OOT3D ? 0x44 : 0x34;

            long pos = reader.Position;
            for (int i = 0; i < numRooms; i++)
            {
                reader.SeekBegin(pos + (i * roomSize));
                rooms.AddRange(ReadRoomHeaders(reader, version));
            }
            return rooms;
        }*/

        private List<RoomSetup> ReadRoomHeaders(FileReader reader, GameVersion version)
        {
            List<RoomSetup> roomSetups = new List<RoomSetup>();

            int offset = 0;

            long pos = reader.Position;
            while (true)
            {
                reader.SeekBegin(pos + offset);

                offset += 8;

                reader.SetByteOrder(true);
                uint cmd1 = reader.ReadUInt32();
                reader.SetByteOrder(false);
                uint cmd2 = reader.ReadUInt32();

                var cmdType = cmd1 >> 24;

                if (cmdType == (uint)HeaderCommands.End)
                    break;

                RoomSetup setup = new RoomSetup();
                roomSetups.Add(setup);

                Console.WriteLine((HeaderCommands)cmdType);
                Console.WriteLine("cmd2 " + cmd2 + " start " + pos);

                switch (cmdType)
                {
                    case (uint)HeaderCommands.MultiSetup:
                        {
                            int numSetups = ((int)cmd1 >> 16) & 0xFF;

                            reader.SeekBegin(pos + cmd2);
                            for (int i = 0; i < numSetups; i++)
                            {
                                uint setupOffset = reader.ReadUInt32();

                                if (setupOffset == 0)
                                    continue;

                                using (reader.TemporarySeek(pos + setupOffset, System.IO.SeekOrigin.Begin))
                                {
                                    var subsetups = ReadRoomHeaders(reader, version);
                                    setup.SubSetups.AddRange(subsetups);
                                }
                            }
                        }
                        break;
                    case (uint)HeaderCommands.Actor:
                        {
                            int numActors = ((int)cmd1 >> 16) & 0xFF;

                            reader.SeekBegin(pos + cmd2);
                            setup.Actors = ReadActors(reader, version, numActors);
                        }
                        break;
                    case (uint)HeaderCommands.Mesh:
                        {
                            reader.SeekBegin(pos + cmd2);
                            setup.Meshes = ReadMesh(reader);
                        }
                        break;
                }
            }

            return roomSetups;
        }

        private List<Actor> ReadActors(FileReader reader, GameVersion verion, int numActors)
        {
            List<Actor> actors = new List<Actor>();

            return actors;
        }

        private List<CMB> ReadMesh(FileReader reader )
        {
            List<CMB> Models = new List<CMB>();

            reader.SetByteOrder(true);
            uint flags = reader.ReadUInt32();
            reader.SetByteOrder(false);
            int meshType = ((int)flags >> 24);
            int numMeshes = ((int)flags >> 16) & 0xFF;
            int meshOffset = reader.ReadInt32();

            if (numMeshes == 0x00)
                return Models;

            //There should be 1 or 2 meshes, (opaque and transparent)
            if (numMeshes != 2 && numMeshes != 1)
                throw new Exception($"Unexpected mesh count {numMeshes}. Expected 1 or 2");

            if (meshType != 2)
                throw new Exception($"Unexpected mesh tye {meshType}. Expected 2");

            reader.SeekBegin(meshOffset + 32); //Relative to end of header
            uint magic = reader.ReadUInt32();
            uint fileSize = reader.ReadUInt32();

            CMB cmb = new CMB();
            cmb.IFileInfo = new IFileInfo();
            cmb.Load(new System.IO.MemoryStream(reader.getSection((uint)meshOffset + 32, fileSize)));
            Models.Add(cmb);

            return Models;
        }

        public void Unload()
        {

        }

        public byte[] Save()
        {
            return null;
        }

        public class RoomSetup
        {
            public List<Actor> Actors = new List<Actor>();
            public List<RoomSetup> SubSetups = new List<RoomSetup>();
            public List<CMB> Meshes = new List<CMB>();
        }

        public class EnvironmentSettings
        {

        }

        public class Actor
        {
            public byte RoomFront { get; set; }
            public byte TransitionEffectFront { get; set; }
            public byte RoomBack { get; set; }
            public byte TransitionEffectBack { get; set; }
            public ushort ActorID { get; set; }

            public ushort PositionX;
            public ushort PositionY;
            public ushort PositionZ;

            public ushort RotationY;

            public ushort Variable;
        }
    }
}
