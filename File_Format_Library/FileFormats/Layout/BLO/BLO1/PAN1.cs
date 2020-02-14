using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using Syroot.Maths;

namespace LayoutBXLYT.GCBLO
{
    public class PAN1 : BasePane
    {
        public float Left { get; set; }
        public float Top { get; set; }

        public short Angle { get; set; } = 0;

        public AnchorType Anchor { get; set; } = AnchorType.TopLeft;

        public PAN1() : base()
        {
        }

        public PAN1(FileReader reader, BLOHeader header) : base()
        {
            byte paramCount = reader.ReadByte();
            Visible = reader.ReadBoolean();
            reader.ReadUInt16();
            Name = reader.ReadString(4, true);

            Left = reader.ReadUInt16();
            Top = reader.ReadUInt16();
            Width = reader.ReadUInt16();
            Height = reader.ReadInt16();

            paramCount -= 6;
            if (paramCount > 0) {
                Angle = reader.ReadInt16();
                --paramCount;
            }
            if (paramCount > 0) {
                Anchor = (AnchorType)reader.ReadByte();
                --paramCount;
            }
            if (paramCount > 0) {
                Alpha = reader.ReadByte();
                --paramCount;
            }
            if (paramCount > 0) {
                InfluenceAlpha = reader.ReadBoolean();
                --paramCount;
            }
            Alpha = 255;

            Translate = GetPosition();
            Scale = new Vector2F(1, 1);
            Rotate = new Vector3F(0,0,0);
        }

        public Vector3F GetPosition()
        {
            float X = 0, Y = 0;
            switch ((byte)Anchor % 3)
            {
                case 0: X = 0; break;
                case 1: X = Left / 2; break;
                case 2: X = Left; break;
            }
            switch ((byte)Anchor / 3)
            {
                case 0: Y = 0; break;
                case 1: Y = Top / 2; break;
                case 2: Y = Top; break;
            }
            return new Vector3F(X,Y,0);
        }

        public virtual void Write(FileWriter writer)
        {
          
        }

        public enum AnchorType
        {
            TopLeft,
            TopMiddle,
            TopRight,

            CenterLeft,
            CenterMiddle,
            CenterRight,

            BottomLeft,
            BottomMiddle,
            BottomRight,
        }
    }
}
