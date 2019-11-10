using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library;
using Toolbox.Library.IO;

namespace FirstPlugin.NLG
{
    public class StrikersSAnim : TreeNodeFile,  IFileFormat
    {
        public FileType FileType { get; set; } = FileType.Animation;

        public bool CanSave { get; set; }
        public string[] Description { get; set; } = new string[] { "Striker Skeleton Animation" };
        public string[] Extension { get; set; } = new string[] { "*.sanim" };
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public IFileInfo IFileInfo { get; set; }

        public bool Identify(System.IO.Stream stream)
        {
            return Utils.GetExtension(FileName) == ".sanim";
        }

        public Type[] Types
        {
            get
            {
                List<Type> types = new List<Type>();
                return types.ToArray();
            }
        }

        public enum SectionMagic : uint
        {

        }

        Dictionary<SectionMagic, SectionHeader> SectionLookup = new Dictionary<SectionMagic, SectionHeader>();

        public void Load(System.IO.Stream stream)
        {
            using (var reader = new FileReader(stream))
            {
                ushort fileFlags = reader.ReadUInt16();
                ushort fileMagic = reader.ReadUInt16();
                uint fileSize = reader.ReadUInt32();

                reader.SetByteOrder(true);
                while (!reader.EndOfStream)
                {
                    ushort flags = reader.ReadUInt16();
                    ushort magic = reader.ReadUInt16();
                    uint sectionSize = reader.ReadUInt32();

                    long pos = reader.Position;
                    Console.WriteLine($"Magic {(SectionMagic)magic} sectionSize {sectionSize}");

                    if (!SectionLookup.ContainsKey((SectionMagic)magic))
                        SectionLookup.Add((SectionMagic)magic, new SectionHeader(sectionSize, pos));

                    //This section will skip sub sections so don't do that
                    reader.SeekBegin(pos + sectionSize);

                    Nodes.Add(magic.ToString("X"));

                    reader.Align(4);
                }
            }
        }

        public class SectionHeader
        {
            public long Position;
            public uint Size;

            public SectionHeader(uint size, long pos)
            {
                Size = size;
                Position = pos;
            }
        }

        public void Unload()
        {

        }

        public void Save(System.IO.Stream stream)
        {
        }
    }
}
