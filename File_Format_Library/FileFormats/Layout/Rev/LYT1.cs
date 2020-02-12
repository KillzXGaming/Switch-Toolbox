using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;

namespace LayoutBXLYT.Revolution
{
    public class LYT1 : LayoutInfo
    {
        public LYT1()
        {
            DrawFromCenter = false;
            Width = 0;
            Height = 0;
        }

        public LYT1(FileReader reader)
        {
            DrawFromCenter = reader.ReadBoolean();
            reader.Seek(3); //padding
            Width = reader.ReadSingle();
            Height = reader.ReadSingle();
        }

        public override void Write(FileWriter writer, LayoutHeader header)
        {
            writer.Write(DrawFromCenter);
            writer.Seek(3);
            writer.Write(Width);
            writer.Write(Height);
        }
    }
}
