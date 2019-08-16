using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace HedgehogLibrary
{
    public class HeaderCommon
    {
        public const uint Signature = 0x133054A;

        public Node Root;

        public void Read(FileReader reader)
        {
            //Not sure but i've seen this used for checking game version
            ushort version = reader.ReadUInt16(); 
            if (version == 0x80)
            {
                ushort fileSize = reader.ReadUInt16();
                uint signature = reader.ReadUInt32();
                uint offsetTable = reader.ReadUInt32();
                uint offsetTableEntryCount = reader.ReadUInt32();

                while (true)
                {
                    ushort sectionFlag = reader.ReadUInt16();
                    ushort sectionAddress = reader.ReadUInt16();
                    uint sectionValue = reader.ReadUInt32();
                    string sectionName = reader.ReadString(8, Encoding.ASCII);

                    if (sectionName == "Contexts")
                        break;
                }
            }
        }

        public class Node
        {

        }
    }
}
