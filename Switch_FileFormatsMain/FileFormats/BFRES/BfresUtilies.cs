using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class BfresUtilies
    {
        public static bool IsSubSectionSwitch(string FileName)
        {
            using (var reader = new FileReader(FileName))
            {
                reader.ByteOrder = Syroot.BinaryData.ByteOrder.BigEndian;

                reader.Seek(24, System.IO.SeekOrigin.Begin);
                return reader.ReadUInt32() == 1;
            }
        }
    }
}
