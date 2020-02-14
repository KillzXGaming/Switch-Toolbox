using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Syroot.Maths;

namespace LayoutBXLYT.GCBLO
{
    public class PAN2 : PAN1
    {
        public PAN2(FileReader reader, BLOHeader header) : base()
        {
            long pos = reader.Position;

            ushort sectionSize = reader.ReadUInt16();
            ushort unk = reader.ReadUInt16();
            uint unk2 = reader.ReadUInt32();
            uint unk3 = reader.ReadUInt32();
            Name = reader.ReadString(0x0C, true);

            Width = reader.ReadSingle();
            Height = reader.ReadSingle();
            Left = reader.ReadSingle();
            Top = reader.ReadSingle();

            Translate = GetPosition();
            Scale = new Vector2F(1, 1);
            Rotate = new Vector3F(0, 0, 0);
            Alpha = 255;
            Visible = true;

            reader.SeekBegin(pos + sectionSize);
        }

        public override void Write(FileWriter writer)
        {

        }
    }
}
