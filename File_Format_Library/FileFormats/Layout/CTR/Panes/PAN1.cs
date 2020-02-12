using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.IO;
using System.ComponentModel;

namespace LayoutBXLYT.CTR
{
    public class PAN1 : BasePane, IUserDataContainer
    {
        public override string Signature { get; } = "pan1";

        private byte _flags1;

        public override bool Visible
        {
            get { return (_flags1 & 0x1) == 0x1; }
            set
            {
                if (value)
                    _flags1 |= 0x1;
                else
                    _flags1 &= 0xFE;
            }
        }

        public override bool InfluenceAlpha
        {
            get { return (_flags1 & 0x2) == 0x2; }
            set
            {
                if (value)
                    _flags1 |= 0x2;
                else
                    _flags1 &= 0xFD;
            }
        }

        public byte PartsScale { get; set; }

        public byte PaneMagFlags { get; set; }

        [DisplayName("User Data"), CategoryAttribute("User Data")]
        public UserData UserData { get; set; }

        public PAN1() : base()
        {

        }

        public PAN1(BxlytHeader header, string name) : base() {
            LoadDefaults();
            Name = name;
        }

        public override UserData CreateUserData()
        {
            return new USD1();
        }

        public override void LoadDefaults()
        {
            base.LoadDefaults();

            UserData = null;
            PaneMagFlags = 0;
        }

        public PAN1(FileReader reader, BxlytHeader header) : base()
        {
            _flags1 = reader.ReadByte();
            byte origin = reader.ReadByte();
            Alpha = reader.ReadByte();
            PaneMagFlags = reader.ReadByte();
            Name = reader.ReadString(0x18, true);
            Translate = reader.ReadVec3SY();
            Rotate = reader.ReadVec3SY();
            Scale = reader.ReadVec2SY();
            Width = reader.ReadSingle();
            Height = reader.ReadSingle();

            int mainorigin = origin % 16;
            int parentorigin = origin / 16;

            originX = (OriginX)(mainorigin % 4);
            originY = (OriginY)(mainorigin / 4);
            ParentOriginX = (OriginX)(parentorigin % 4);
            ParentOriginY = (OriginY)(parentorigin / 4);
        }

        public override void Write(FileWriter writer, LayoutHeader header)
        {
            int originL = (int)originX;
            int originH = (int)originY * 4;
            int originPL = (int)ParentOriginX;
            int originPH = (int)ParentOriginY * 4;
            byte parentorigin = (byte)((originPL + originPH) * 16);
            byte origin = (byte)(originL + originH + parentorigin);

            writer.Write(_flags1);
            writer.Write(origin);
            writer.Write(Alpha);
            writer.Write(PaneMagFlags);
            writer.WriteString(Name, 0x18);
            writer.Write(Translate);
            writer.Write(Rotate);
            writer.Write(Scale);
            writer.Write(Width);
            writer.Write(Height);
        }

        public bool ParentVisibility
        {
            get
            {
                if (Scale.X == 0 || Scale.Y == 0)
                    return false;
                if (!Visible)
                    return false;
                if (Parent != null && Parent is PAN1)
                {
                    return ((PAN1)Parent).ParentVisibility && Visible;
                }
                return true;
            }
        }
    }

}
