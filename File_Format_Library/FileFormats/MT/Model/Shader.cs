using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Toolbox.Library;
using Toolbox.Library.IO;
using Toolbox.Library.Security.Cryptography;

namespace CafeLibrary.M2
{
    public class Shader
    {
        public Dictionary<uint, AttributeGroup> AttributeGroups { get; set; } = new Dictionary<uint, AttributeGroup>();

        public Shader() { }

        public Shader(Stream stream)
        {
            using (var reader = new FileReader(stream)) {
                Read(reader);
            }
        }

        public void Read(FileReader reader)
        {
            reader.ReadUInt32(); //MFX
            reader.Seek(8); //unks
            uint descCount = reader.ReadUInt32();
            uint stringTableOffset = reader.ReadUInt32();
            reader.ReadUInt32();

            long pos = reader.Position;
            for (int i = 0; i < descCount - 1; i++)
            {
                reader.SeekBegin(pos + (i * 4));
                uint ofs = reader.ReadUInt32();
                if (ofs == 0)
                    continue;

                reader.SeekBegin(ofs);

                Descriptor desc = new Descriptor();
                desc.Name = ReadName(reader, stringTableOffset);
                desc.Type = ReadName(reader, stringTableOffset);
                ushort DescType = reader.ReadUInt16();
                ushort MapLength = reader.ReadUInt16(); //Actual length is value / 2? Not sure
                ushort MapIndex = reader.ReadUInt16();
                ushort DescIndex = reader.ReadUInt16();
                reader.ReadUInt32();
                uint MapAddress = reader.ReadUInt32(); //Not sure what this address actually points to

                var crc = MT_Globals.crc32(desc.Name);
                var jcrc = MT_Globals.jamcrc(desc.Name);
                var mcrc = MT_Globals.mfxcrc(desc.Name, DescIndex);

 
                if (desc.Type == "__InputLayout")
                {
                    Console.WriteLine($"attribute {desc.Name} DescIndex {DescIndex}");

                   AttributeGroups.Add(mcrc, new AttributeGroup(reader, stringTableOffset)
                   {
                       Name = desc.Name,
                   });
                }
            }
            File.WriteAllText("Shader.json", JsonConvert.SerializeObject(this, Formatting.Indented));
        }
        private static string ReadName(FileReader reader, uint stringTableOffset)
        {
            uint ofs = reader.ReadUInt32();
            using (reader.TemporarySeek(stringTableOffset + ofs, SeekOrigin.Begin)) {
                return reader.ReadZeroTerminatedString();
            }
        }

        class Descriptor
        {
            public string Name;
            public string Type;
        }

        public class AttributeGroup
        {
            public string Name;

            public List<Attribute> Attributes { get; set; } = new List<Attribute>();

            public AttributeGroup() { }

            public AttributeGroup(FileReader reader, uint stringTableOffset)
            {
                ushort AttrCount = reader.ReadUInt16();
                byte Stride = reader.ReadByte(); 
                reader.ReadByte(); 
                reader.ReadUInt32();

                Stride *= 4;

                for (int j = 0; j < AttrCount; j++)
                {
                    string name = ReadName(reader, stringTableOffset);
                    uint Format = reader.ReadUInt32();

                    uint AttrIndex = (Format >> 0) & 0x3f; //Used when attribute is repeated usually
                    uint AttrFormat = (Format >> 6) & 0x1f; //See AttributeFormat enumerator
                    uint AttrElems = (Format >> 11) & 0x7F; //2 = 2D, 3 = 3D, 4 = 4D, ...
                    uint AttrOffset = (Format >> 24) & 0xff; //In Word Count (32-bits)
                    var instancing = (Format >> 31) & 0x1;

                    //Color
                    if (AttrFormat == 0xe || AttrFormat == 0xb || AttrFormat == 0xc)
                        AttrElems = 4;

                    AttrOffset *= 4;

                    this.Attributes.Add(new Attribute()
                    {
                        Name = name,
                        Format = (AttributeFormat)AttrFormat,
                        ElementCount = AttrElems,
                        Offset = AttrOffset,
                        Set = AttrIndex,
                    });
                }
            }

            public class Attribute
            {
                public string Name { get; set; }
                public AttributeFormat Format { get; set; }
                public uint Offset { get; set; }
                public uint ElementCount { get; set; }
                public uint Set { get; set; }
            }

            public enum AttributeFormat
            {
                Float = 1,
                HalfFloat = 2,
                UShort = 3,
                Short = 4,
                S16N = 5,
                U16N = 6,
                Sbyte = 7,
                Byte = 8,
                S8N = 9,
                U8N = 10,
                RGB = 13,
                RGBA = 14
            }
        }
    }
}
