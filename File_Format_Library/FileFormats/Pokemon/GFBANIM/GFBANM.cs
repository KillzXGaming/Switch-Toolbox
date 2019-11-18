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

namespace FirstPlugin
{
    public class GFBANM : IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "MDL" };
        public string[] Extension { get; set; } = new string[] { "*.mdl" };
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

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {

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

            public void Read(FileReader reader)
            {
                reader.ReadUInt32();
                config = ParseSection<Config>(reader, this);
            }
        }

        private static T ParseSection<T>(FileReader reader, Header header)
             where T : GFSection, new()
        {
            var layoutOffset = reader.ReadOffset(true, typeof(uint));
            var dataOffset = reader.ReadOffset(true, typeof(uint));

            T section = new T();
            using (reader.TemporarySeek(layoutOffset, System.IO.SeekOrigin.Begin))
            {
                ushort layoutSize = reader.ReadUInt16();
                ushort layoutStride = reader.ReadUInt16();
                section.LayoutPointers = reader.ReadUInt16s((int)(layoutSize / 2));
            }

            using (reader.TemporarySeek(dataOffset, System.IO.SeekOrigin.Begin))
            {
                section.Read(reader, header);
            }

            return section;
        }

        public class GFSection
        {
            public ushort[] LayoutPointers { get; set; }

            public virtual void Read(FileReader reader, Header header)
            {

            }
        }

        public class Config : GFSection
        {
            public int Unknown { get; set; }
            public uint NumKeyFrames { get; set; }
            public uint FramesPerSecond { get; set; }
        }

        public class BoneList : GFSection
        {
            public List<Bone> Bones = new List<Bone>();
            public List<BoneDefaults> BoneDefaults = new List<BoneDefaults>();
        }

        public class Bone
        {
            public string Name { get; set; }
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
