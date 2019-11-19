using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox;
using System.Windows.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;
using OpenTK;
using System.Reflection;

namespace FirstPlugin
{
    public class GFBANM : IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "GFBANM" };
        public string[] Extension { get; set; } = new string[] { "*.gfbanm" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            return Utils.GetExtension(FileName) == ".gfbanm";
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        Header header;
        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                header = new Header(reader);
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }

        public class Header
        {
            public Config config;
            public BoneList boneList;

            public Header(FileReader reader)
            {
                config = ParseSection<Config>(reader, this);
                boneList = ParseSection<BoneList>(reader, this);

                Console.WriteLine($"config NumKeyFrames {config.NumKeyFrames}");
                Console.WriteLine($"config FramesPerSecond {config.FramesPerSecond}");
            }
        }

        private static T ParseSection<T>(FileReader reader, Header header)
             where T : GFSection, new()
        {
            var offset = reader.ReadOffset(true, typeof(uint));
            reader.SeekBegin(offset);

            long origin = reader.Position;

            int layoutOffset = reader.ReadInt32();
            var dataOffset = reader.ReadOffset(true, typeof(uint));

            T section = new T();
            using (reader.TemporarySeek(origin - layoutOffset, System.IO.SeekOrigin.Begin))
            {
                ushort layoutSize = reader.ReadUInt16();
                ushort layoutStride = reader.ReadUInt16();

                List<ushort> pointers = new List<ushort>();
                uint looper = 4;
                while (looper < layoutSize) {
                    pointers.Add(reader.ReadUInt16());
                    looper += 2;
                }

                section.LayoutPointers = reader.ReadUInt16s((int)(layoutSize / 4));
            }

            using (reader.TemporarySeek(dataOffset, System.IO.SeekOrigin.Begin)) {
                section.Read(reader, header);
            }

            return section;
        }

        public class GFSection
        {
            public ushort LayoutSize { get; set; }
            public ushort LayoutStride { get; set; }

            public ushort[] LayoutPointers { get; set; }

            public void Read(FileReader reader, Header header)
            {
                long origin = reader.Position;

                PropertyInfo[] types = new PropertyInfo[(int)LayoutPointers?.Length];

                var sectionType = this.GetType();

                int index = 0;
                foreach (var prop in sectionType.GetProperties()) {
                    if (!Attribute.IsDefined(prop, typeof(FlatTableParse)))
                        continue;

                    types[index++] = prop;
                }

                for (int i = 0; i < LayoutPointers?.Length; i++)
                {
                    reader.SeekBegin(origin + LayoutPointers[i]);
                    if (types[i] != null)
                    {
                        var prop = types[i];
                        var propertyType = prop.PropertyType;

                        if (propertyType == typeof(uint))
                            prop.SetValue(this, reader.ReadUInt32());
                        else if (propertyType == typeof(int))
                            prop.SetValue(this, reader.ReadInt32());
                        else if(propertyType == typeof(byte))
                            prop.SetValue(this, reader.ReadByte());
                        else if(propertyType == typeof(sbyte))
                            prop.SetValue(this, reader.ReadSByte());
                        else if (propertyType == typeof(ushort))
                            prop.SetValue(this, reader.ReadUInt16());
                        else if (propertyType == typeof(short))
                            prop.SetValue(this, reader.ReadInt16());
                        else if (propertyType == typeof(Vector2))
                            prop.SetValue(this, new Vector2(
                                reader.ReadSingle(),
                                reader.ReadSingle())
                             );
                        else if (propertyType == typeof(Vector3))
                            prop.SetValue(this, new Vector3(
                                reader.ReadSingle(),
                                reader.ReadSingle(),
                                reader.ReadSingle())
                             );
                        else if (propertyType == typeof(Vector4))
                            prop.SetValue(this, new Vector4(
                                reader.ReadSingle(),
                                reader.ReadSingle(),
                                reader.ReadSingle(),
                                reader.ReadSingle())
                             );
                        else if (propertyType == typeof(GFSection))
                        {
                            var offset = reader.ReadOffset(true, typeof(uint));
                            reader.SeekBegin(offset);
                        }
                        else if (propertyType is IEnumerable<GFSection>) {

                        }
                    }
                }
            }
        }

        public class FlatTableParse : Attribute { }
        public class OffsetProperty : Attribute { }

        [OffsetProperty]
        public class Config : GFSection
        {
            [FlatTableParse]
            public int Unknown { get; set; }
            [FlatTableParse]
            public uint NumKeyFrames { get; set; }
            [FlatTableParse]
            public uint FramesPerSecond { get; set; }
        }

        [OffsetProperty]
        public class BoneList : GFSection
        {
            [FlatTableParse]
            public List<Bone> Bones = new List<Bone>();

            [FlatTableParse]
            public List<BoneDefaults> BoneDefaults = new List<BoneDefaults>();
        }

        public class Bone
        {
            [FlatTableParse, OffsetProperty]
            public string Name { get; set; }

            [FlatTableParse]
            public byte ScaleType { get; set; }
        }

        public class NammeOffset
        {
            [FlatTableParse, OffsetProperty]
            public string Name { get; set; }

            [FlatTableParse]
            public byte ScaleType { get; set; }
        }

        public class BoneDefaults
        {
            public int Unknown { get; set; }

            public Vector3 Scale { get; set; }
            public Vector4 Rotation { get; set; }
            public Vector3 Translation { get; set; }
        }

        public class TriggerList
        {
            public List<Trigger> Triggers = new List<Trigger>();
        }

        public class Trigger
        {
            public string Name { get; set; }
            public int StartFrame { get; set; }
            public int EndFrame { get; set; }
        }

        public class TriggerParameter
        {
            public string Name { get; set; }
            public byte Type { get; set; }

            //float, byte, int, string
            public object Value { get; set; }
        }
    }
}
