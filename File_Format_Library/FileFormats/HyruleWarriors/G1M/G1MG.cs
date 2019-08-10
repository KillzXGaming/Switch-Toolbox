using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library;

namespace HyruleWarriors.G1M
{
    public class G1MG : G1MChunkCommon
    {
        private string Type { get; set; }

        public G1MG(FileReader reader)
        {
            Type = reader.ReadString(3, Encoding.ASCII);
            reader.ReadByte();//padding

            if (Type == "DX1" || Type == "NX_")
            {

            }
        }
    }
}
