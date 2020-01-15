using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace FirstPlugin
{
    public class SharcCommon
    {
        public static void WriteSectionSize(FileWriter writer, long startPos)
        {
            long endPos = writer.Position;
            using (writer.TemporarySeek(startPos, System.IO.SeekOrigin.Begin)) {
                writer.Write((uint)(endPos - startPos));
            };
        }
    }
}
