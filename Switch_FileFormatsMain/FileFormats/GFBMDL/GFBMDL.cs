using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox;
using System.Windows.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;

namespace FirstPlugin
{
    public class GFBMDL : TreeNodeFile, IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Model;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Graphic Model" };
        public string[] Extension { get; set; } = new string[] { "*.gfbmdl" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            using (var reader = new Switch_Toolbox.Library.IO.FileReader(stream, true))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                bool IsMatch = reader.ReadUInt32() == 0x20000000;
                reader.Position = 0;

                return IsMatch;
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
            var mem = new System.IO.MemoryStream();
            header.Write(new FileWriter(mem));
            return mem.ToArray();
        }

        public class Header
        {
            public uint Version { get; set; }
            public float[] Boundings { get; set; }
            public List<string> TextureMaps = new List<string>();
            public List<string> Materials = new List<string>();

            public void Read(FileReader reader)
            {
                reader.SetByteOrder(false);

                Version = reader.ReadUInt32();
                Boundings = reader.ReadSingles(9);
                long TextureOffset = reader.ReadOffset(true, typeof(uint));
                long MaterialOffset = reader.ReadOffset(true, typeof(uint));
                long UnknownOffset = reader.ReadOffset(true, typeof(uint));
                long Unknown2Offset = reader.ReadOffset(true, typeof(uint));
                long ShaderOffset = reader.ReadOffset(true, typeof(uint));
                long VisGroupOffset = reader.ReadOffset(true, typeof(uint));
                long BoneDataOffset = reader.ReadOffset(true, typeof(uint));

                if (TextureOffset != 0)
                {
                    reader.Seek(TextureOffset, SeekOrigin.Begin);
                    uint Count = reader.ReadUInt32();
                    TextureMaps = reader.ReadNameOffsets(Count, true, typeof(uint), true);
                }

                foreach (var tex in TextureMaps)
                    Console.WriteLine("TEXNAME " + tex);

                if (MaterialOffset != 0)
                {
                    reader.Seek(MaterialOffset, SeekOrigin.Begin);
                    uint Count = reader.ReadUInt32();
                    Materials = reader.ReadNameOffsets(Count, true, typeof(uint));
                }
            }

            public void Write(FileWriter writer)
            {
                writer.Write(Version);
            }
        }

        public class Bone
        {

        }

        public class Material
        {

        }
    }
}
