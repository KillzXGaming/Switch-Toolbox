using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using OpenTK;

namespace HyruleWarriors.G1M
{
    public class NUNO
    {
        public List<NUNOType0303Struct> NUNO0303StructList = new List<NUNOType0303Struct>();

        public class NUNOType0303Struct
        {
            public int Unknown { get; set; }
            public int BoneParentID { get; set; }
            public Vector4[] Points { get; set; }
            public NUNOInfluence[] Influences { get; set; }

            public int Unknown2 { get; set; }
            public int Unknown3 { get; set; }
            public int Unknown4 { get; set; }
        }

        public struct NUNOInfluence
        {
            public int P1 { get; set; }
            public int P2 { get; set; }
            public int P3 { get; set; }
            public int P4 { get; set; }
            public float P5 { get; set; }
            public float P6 { get; set; }
        }

        public NUNO(FileReader reader, uint version)
        {
            uint numChunks = reader.ReadUInt32();
            for (int i = 0; i < numChunks; i++)
            {
                long pos = reader.Position;
                uint subchunkMagic = reader.ReadUInt32();
                uint subchunkSize = reader.ReadUInt32();
                uint subChunkCount = reader.ReadUInt32();
                for (int j = 0; j < subChunkCount; j++)
                {
                    switch(subchunkMagic)
                    {
                        case 0x00030001:
                            NUNO0303StructList.Add(ParseNUNOSection0301(reader, version));
                            break;
                        case 0x00030002:
                            ParseNUNOSection0302(reader, version);
                            break;
                        case 0x00030003:
                            NUNO0303StructList.Add(ParseNUNOSection0303(reader, version));
                            break;
                        case 0x00030004:
                            ParseNUNOSection0304(reader, version);
                            break;
                        default:
                            Console.WriteLine("Unknown chunk! " + subchunkMagic.ToString());
                            break;
                    }
                }
                reader.SeekBegin(pos + subchunkSize);
            }
        }

        public NUNOType0303Struct ParseNUNOSection0301(FileReader reader, uint version)
        {
            var nuno301 = new NUNOType0303Struct();
            nuno301.Unknown = reader.ReadInt32();
            uint numControlPoints = reader.ReadUInt32();
            uint unkCount = reader.ReadUInt32();
            nuno301.Unknown2 = reader.ReadInt32();
            nuno301.Unknown3 = reader.ReadInt32();
            nuno301.Unknown4 = reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadInt32();
            nuno301.BoneParentID = reader.ReadInt32();

            reader.ReadBytes(0x40);
            if (version >= 0x30303235)
                reader.ReadBytes(0x10);

            nuno301.Influences = new NUNOInfluence[(int)numControlPoints];
            nuno301.Points = new Vector4[(int)numControlPoints];
            for (int i = 0; i < numControlPoints; i++)
                nuno301.Points[i] = reader.ReadVec4();

            for (int i = 0; i < numControlPoints; i++) {
                var influence = new NUNOInfluence();
                influence.P1 = reader.ReadInt32();
                influence.P2 = reader.ReadInt32();
                influence.P3 = reader.ReadInt32();
                influence.P4 = reader.ReadInt32();
                influence.P5 = reader.ReadSingle();
                influence.P6 = reader.ReadSingle();
                nuno301.Influences[i] = influence;
            }

            reader.Seek(48 * unkCount);
            reader.Seek(4 * nuno301.Unknown2);
            reader.Seek(4 * nuno301.Unknown3);
            reader.Seek(4 * nuno301.Unknown4);

            return nuno301;
        }

        public void ParseNUNOSection0302(FileReader reader, uint version)
        {

        }

        public NUNOType0303Struct ParseNUNOSection0303(FileReader reader, uint version)
        {
            var nuno301 = new NUNOType0303Struct();
            nuno301.Unknown = reader.ReadInt32();
            uint numControlPoints = reader.ReadUInt32();
            uint unkCount = reader.ReadUInt32();
            nuno301.Unknown2 = reader.ReadInt32();
            nuno301.BoneParentID = reader.ReadInt32();
            nuno301.Unknown3 = reader.ReadInt32();

            reader.ReadBytes(0xB0);
            if (version >= 0x30303235)
                reader.ReadBytes(0x10);

            nuno301.Influences = new NUNOInfluence[(int)numControlPoints];
            nuno301.Points = new Vector4[(int)numControlPoints];
            for (int i = 0; i < numControlPoints; i++)
                nuno301.Points[i] = reader.ReadVec4();

            for (int i = 0; i < numControlPoints; i++)
            {
                var influence = new NUNOInfluence();
                influence.P1 = reader.ReadInt32();
                influence.P2 = reader.ReadInt32();
                influence.P3 = reader.ReadInt32();
                influence.P4 = reader.ReadInt32();
                influence.P5 = reader.ReadSingle();
                influence.P6 = reader.ReadSingle();
                nuno301.Influences[i] = influence;
            }

            reader.Seek(48 * unkCount);
            reader.Seek(4 * nuno301.Unknown2);
            reader.Seek(8 * nuno301.Unknown3);

            return nuno301;
        }

        public void ParseNUNOSection0304(FileReader reader, uint version)
        {

        }
    }
}
