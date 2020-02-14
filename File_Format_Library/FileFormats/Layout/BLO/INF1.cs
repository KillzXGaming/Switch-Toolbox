using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Toolbox.Library;

namespace LayoutBXLYT.GCBLO
{
    public class INF1 : LayoutInfo
    {
        public uint Unknown { get; set; }

        public STColor8 TintColor { get; set; }

        public INF1()
        {
            Unknown = 0;
            Width = 0;
            Height = 0;
            TintColor = STColor8.White;
        }

        public INF1(FileReader reader, BLOHeader header)
        {
            Unknown = reader.ReadUInt32();
            Width = reader.ReadUInt16();
            Height = reader.ReadInt16();
            TintColor = reader.ReadColor8RGBA();
        }

        public void Write(FileWriter writer)
        {
            writer.Write(Unknown);
            writer.Write((ushort)Width);
            writer.Write((ushort)Height);
            writer.Write(TintColor);
        }
    }
}
