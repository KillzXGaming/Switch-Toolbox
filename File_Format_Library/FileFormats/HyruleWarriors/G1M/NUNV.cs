using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using OpenTK;

namespace HyruleWarriors.G1M
{
    public class NUNV
    {
        public List<NUNO.NUNOType0303Struct> NUNV0303StructList = new List<NUNO.NUNOType0303Struct>();

        public NUNV(FileReader reader, uint version)
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
                    switch (subchunkMagic)
                    {
                        case 0x00050001:
                            NUNV0303StructList.Add(ParseNUVOSection0501(reader, version));
                            break;
                        default:
                            Console.WriteLine("Unknown chunk! " + subchunkMagic.ToString());
                            break;
                    }
                }
                reader.SeekBegin(pos + subchunkSize);
            }
        }

        public NUNO.NUNOType0303Struct ParseNUVOSection0501(FileReader reader, uint version)
        {
            var nuno501 = new NUNO.NUNOType0303Struct();
            nuno501.Unknown = reader.ReadInt32();
            uint numControlPoints = reader.ReadUInt32();
            uint unkCount = reader.ReadUInt32();
            nuno501.Unknown2 = reader.ReadInt32();
            nuno501.BoneParentID = reader.ReadInt32();

            reader.ReadBytes(0x50);
            if (version >= 0x30303131)
                reader.ReadBytes(0x10);

            nuno501.Influences = new NUNO.NUNOInfluence[(int)numControlPoints];
            nuno501.Points = new Vector4[(int)numControlPoints];
            for (int i = 0; i < numControlPoints; i++)
                nuno501.Points[i] = reader.ReadVec4();

            for (int i = 0; i < numControlPoints; i++)
            {
                var influence = new NUNO.NUNOInfluence();
                influence.P1 = reader.ReadInt32();
                influence.P2 = reader.ReadInt32();
                influence.P3 = reader.ReadInt32();
                influence.P4 = reader.ReadInt32();
                influence.P5 = reader.ReadSingle();
                influence.P6 = reader.ReadSingle();
                nuno501.Influences[i] = influence;
            }

            reader.Seek(48 * unkCount);
            reader.Seek(4 * nuno501.Unknown2);

            return nuno501;
        }
    }
}
