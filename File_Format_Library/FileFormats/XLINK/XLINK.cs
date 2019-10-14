using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace FirstPlugin
{
    public class XLINK : IEditor<TextEditor>, IFileFormat
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
            using (var reader = new Toolbox.Library.IO.FileReader(stream, true))
            {
                return reader.CheckSignature(4, "XLNK");
            }
        }

        public TextEditor OpenForm()
        {
            return new TextEditor();
        }

        public void FillEditor(UserControl control)
        {
            ((TextEditor)control).FillEditor(ToText());
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public string ToText()
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "  ",
            };


            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmldecl = doc.CreateXmlDeclaration("1.0", null, null);
            xmldecl.Encoding = "UTF-8";
            xmldecl.Standalone = "yes";

            var stringWriter = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(Header));
            XmlWriter output = XmlWriter.Create(stringWriter, settings);
            serializer.Serialize(output, header, ns);
            return stringWriter.ToString();
        }

        private Header header;

        public void Load(System.IO.Stream stream)
        {
            CanSave = false;

            header = new Header();
            header.Read(new FileReader(stream));

            var userData = new TreeNode("User Data");
        //    Nodes.Add(userData);

            var hashes = new TreeNode("Hashes");
            userData.Nodes.Add(hashes);

            foreach (var hash in header.UserDataTable.CRC32Hashes)
                hashes.Nodes.Add(new TreeNode(hash.ToString("x")));

            var paramDefines = new TreeNode("Param Defines");
       //     Nodes.Add(paramDefines);

            foreach (var param in header.ParamDefineTable.UserParams)
                paramDefines.Nodes.Add(param.Name);
            foreach (var param in header.ParamDefineTable.TriggerParams)
                paramDefines.Nodes.Add(param.Name);
            foreach (var param in header.ParamDefineTable.UserParams)
                paramDefines.Nodes.Add(param.Name);
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }


        //Documentation from https://github.com/Kinnay/Nintendo-File-Formats/wiki/XLINK-File-Format#header

        public class Header
        {
            public ushort ByteOrderMark;

            public UserDataTable UserDataTable;
            public ParamDefineTable ParamDefineTable;

            public List<ResourceAssetParamTable> ResourceAssetParamTables = new List<ResourceAssetParamTable>();
            public List<TriggerOverwriteParamTable> TriggerOverwriteParamTables = new List<TriggerOverwriteParamTable>();

            public List<LocalNameProperty> LocalNameProperties = new List<LocalNameProperty>();
            public List<LocalNameProperty> LocalNameEnumProperties = new List<LocalNameProperty>();

            internal uint nameTablePos;

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
                nameTablePos = reader.ReadUInt32();

                UserDataTable = new UserDataTable();
                UserDataTable.Read(reader, (int)numUser);

                ParamDefineTable = new ParamDefineTable();
                ParamDefineTable.Read(reader, this);

                for (int i = 0; i < numResAssetParam; i++)
                {
                    var resAssetsParam = new ResourceAssetParamTable();
                    resAssetsParam.Read(reader);
                    ResourceAssetParamTables.Add(resAssetsParam);
                }

                reader.SeekBegin(triggerOverwriteParamTablePos);
                for (int i = 0; i < numResTriggerOverwriteParam; i++)
                {
                    var triggerOverwriteParamTbl = new TriggerOverwriteParamTable();
                    triggerOverwriteParamTbl.Read(reader);
                    TriggerOverwriteParamTables.Add(triggerOverwriteParamTbl);
                }

                reader.SeekBegin(localPropertyNameRefTablePos);
                for (int i = 0; i < numLocalPropertyNameRefTable; i++)
                {
                    var localNameProp = new LocalNameProperty();
                    localNameProp.Read(reader, nameTablePos);
                    LocalNameProperties.Add(localNameProp);
                }

                for (int i = 0; i < numLocalPropertyEnumNameRefTable; i++)
                {
                    var localNameProp = new LocalNameProperty();
                    localNameProp.Read(reader, nameTablePos);
                    LocalNameEnumProperties.Add(localNameProp);
                }
            }
        }

        public class LocalNameProperty
        {
            [XmlAttribute]
            public string Name;

            public void Read(FileReader reader, uint nameTableOffset)
            {
                uint offset = reader.ReadUInt32();
                long pos = reader.Position;

                reader.SeekBegin(offset + nameTableOffset);
                Name = reader.ReadZeroTerminatedString(Encoding.GetEncoding(932));

                reader.SeekBegin(pos);
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

            public void Read(FileReader reader, Header header)
            {
                uint SectionSize = reader.ReadUInt32();
                uint numUserParams = reader.ReadUInt32();
                uint numAssetParams = reader.ReadUInt32();
                uint unknown = reader.ReadUInt32();
                uint numTriggerParams = reader.ReadUInt32();

                uint nameTblPos = (uint)reader.Position + ((numUserParams + numAssetParams + numTriggerParams) * 12);

                for (int i = 0; i < numUserParams; i++)
                {
                    var entry = new ParamDefineEntry();
                    entry.Read(reader, nameTblPos);
                    UserParams.Add(entry);
                }
                for (int i = 0; i < numAssetParams; i++)
                {
                    var entry = new ParamDefineEntry();
                    entry.Read(reader, nameTblPos);
                    AssetParams.Add(entry);
                }
                for (int i = 0; i < numTriggerParams; i++)
                {
                    var entry = new ParamDefineEntry();
                    entry.Read(reader, nameTblPos);
                    TriggerParams.Add(entry);
                }
            }
        }

        public class ParamDefineEntry
        {
            [XmlAttribute]
            public string Name { get; set; }

            [XmlAttribute]
            public uint Type { get; set; }

            [XmlElement("int", typeof(int))]
            [XmlElement("str", typeof(string))]
            [XmlElement("uint", typeof(uint))]
            [XmlElement("float", typeof(float))]
            public object DefaultValue { get; set; }

            public void Read(FileReader reader, uint nameTblPos)
            {
                long pos = reader.Position;

                uint NamePos = reader.ReadUInt32(); //Offset from string table
                Type = reader.ReadUInt32();

                Console.WriteLine("Type " + Type);

                if (Type == 0)
                {
                    uint defaultPos = reader.ReadUInt32();
                    reader.SeekBegin(nameTblPos + defaultPos);
                    DefaultValue = reader.ReadZeroTerminatedString();

                    Console.WriteLine("defaultPos " + defaultPos);
                }
                else if (Type == 1)
                    DefaultValue = reader.ReadSingle();
                else
                    DefaultValue = reader.ReadInt32();

                reader.SeekBegin(nameTblPos + NamePos);
                Name = reader.ReadZeroTerminatedString();

                Console.WriteLine("Name " + Name);

                reader.SeekBegin(pos + 12);
            }
        }

        public class ResourceAssetParamTable
        {
            public ulong Mask;

            public uint FirstReference;
            public uint SecondReference;
            public uint ThirdReference;

            public void Read(FileReader reader)
            {
                Mask = reader.ReadUInt64();
                if ((Mask & 1) != 0)
                    FirstReference = reader.ReadUInt32();
                if ((Mask & 2) != 0)
                    SecondReference = reader.ReadUInt32();
                if ((Mask & 4) != 0)
                    ThirdReference = reader.ReadUInt32();
            }
        }

        public class TriggerOverwriteParamTable
        {
            public uint Mask;

            public uint FirstReference;
            public uint SecondReference;
            public uint ThirdReference;

            public void Read(FileReader reader)
            {
                Mask = reader.ReadUInt32();
                if ((Mask & 1) != 0)
                    FirstReference = reader.ReadUInt32();
                if ((Mask & 2) != 0)
                    SecondReference = reader.ReadUInt32();
                if ((Mask & 4) != 0)
                    ThirdReference = reader.ReadUInt32();
            }
        }
    }
}
