using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library;
using System.Windows.Forms;

namespace FirstPlugin
{
    public class XLINK : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Effect;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Effect Link" };
        public string[] Extension { get; set; } = new string[] { "*.bslnk", "*.belnk" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "XLNK");
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
            CanSave = false;

            Text = FileName;

            Header Header = new Header();
            Header.Read(new FileReader(stream));

            var userData = new TreeNode("User Data");
            Nodes.Add(userData);

            var hashes = new TreeNode("Hashes");
            userData.Nodes.Add(hashes);

            foreach (var hash in Header.UserDataTable.CRC32Hashes)
                hashes.Nodes.Add(new TreeNode(hash.ToString("x")));

            var paramDefines = new TreeNode("Param Defines");
            Nodes.Add(paramDefines);

            foreach (var param in Header.ParamDefineTable.UserParams)
                paramDefines.Nodes.Add(param.Name);
            foreach (var param in Header.ParamDefineTable.TriggerParams)
                paramDefines.Nodes.Add(param.Name);
            foreach (var param in Header.ParamDefineTable.UserParams)
                paramDefines.Nodes.Add(param.Name);
        }

        public void Unload()
        {

        }
        public byte[] Save()
        {
            return null;
        }


        //Documentation from https://github.com/Kinnay/Nintendo-File-Formats/wiki/XLINK-File-Format#header

        public class Header
        {
            public ushort ByteOrderMark;

            public UserDataTable UserDataTable;
            public ParamDefineTable ParamDefineTable;

            public void Read(FileReader reader)
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.LittleEndian;
                reader.ReadSignature(4, "XLNK");
                uint FileSize = reader.ReadUInt32();
                uint Version = reader.ReadUInt32();
                uint numResParam = reader.ReadUInt32();
                uint numResAssetParam = reader.ReadUInt32();
                uint numResTriggerOverwriteParam = reader.ReadUInt32();
                uint triggerOverwriteParamTablePos = reader.ReadUInt32();
                uint localPropertyNameRefTablePos = reader.ReadUInt32();
                uint numLocalPropertyNameRefTable = reader.ReadUInt32();
                uint numLocalPropertyEnumNameRefTable = reader.ReadUInt32();
                uint numDirectValueTable = reader.ReadUInt32();
                uint numRandomTable = reader.ReadUInt32();
                uint numCurveTable = reader.ReadUInt32();
                uint numCurvePointTable = reader.ReadUInt32();
                uint exRegionPos = reader.ReadUInt32();
                uint numUser = reader.ReadUInt32();
                uint conditionTablePos = reader.ReadUInt32();
                uint nameTablePos = reader.ReadUInt32();

                UserDataTable = new UserDataTable();
                UserDataTable.Read(reader, (int)numUser);

                ParamDefineTable = new ParamDefineTable();
                ParamDefineTable.Read(reader);
            }
        }

        public class UserDataTable
        {
            public uint[] CRC32Hashes;
            public uint[] ExRegionOffsets;

            public void Read(FileReader reader, int EntryCount)
            {
                CRC32Hashes = reader.ReadUInt32s(EntryCount);
                ExRegionOffsets = reader.ReadUInt32s(EntryCount);
            }
        }

        public class ParamDefineTable
        {
            public List<ParamDefineEntry> UserParams = new List<ParamDefineEntry>();
            public List<ParamDefineEntry> AssetParams = new List<ParamDefineEntry>();
            public List<ParamDefineEntry> TriggerParams = new List<ParamDefineEntry>();

            public void Read(FileReader reader)
            {
                uint SectionSize = reader.ReadUInt32();
                uint numUserParams = reader.ReadUInt32();
                uint numAssetParams = reader.ReadUInt32();
                uint unknown = reader.ReadUInt32();
                uint numTriggerParams = reader.ReadUInt32();

                for (int i = 0; i < numUserParams; i++)
                {
                    var entry = new ParamDefineEntry();
                    entry.Read(reader);
                    UserParams.Add(entry);
                }
                for (int i = 0; i < numAssetParams; i++)
                {
                    var entry = new ParamDefineEntry();
                    entry.Read(reader);
                    AssetParams.Add(entry);
                }
                for (int i = 0; i < numTriggerParams; i++)
                {
                    var entry = new ParamDefineEntry();
                    entry.Read(reader);
                    TriggerParams.Add(entry);
                }

                List<byte> StringTable = new List<byte>();

                long StringTablePosition = reader.Position;

                foreach (var param in UserParams)
                    param.ReadString(reader, StringTablePosition);

                foreach (var param in AssetParams)
                    param.ReadString(reader, StringTablePosition);

                foreach (var param in TriggerParams)
                    param.ReadString(reader, StringTablePosition);
            }
        }

        public class ParamDefineEntry
        {
            internal uint NamePos;

            public string Name { get; set; }
            public uint Type { get; set; }
            public byte[] DefaultValue { get; set; }

            public void Read(FileReader reader)
            {
                NamePos = reader.ReadUInt32(); //Offset from string table
                Type = reader.ReadUInt32();
                DefaultValue = reader.ReadBytes(4);
            }

            public void ReadString(FileReader reader, long TablePosition)
            {
                Console.WriteLine("NamePos " + NamePos);
                reader.Position = TablePosition + NamePos;
                Name = reader.ReadString(Syroot.BinaryData.BinaryStringFormat.ZeroTerminated);

                Console.WriteLine("Name " + Name);
            }
        }
    }
}
