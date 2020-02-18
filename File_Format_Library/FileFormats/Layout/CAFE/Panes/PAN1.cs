using System;
using System.Linq;
using System.ComponentModel;
using Syroot.Maths;
using Toolbox.Library.IO;

namespace LayoutBXLYT.Cafe
{
    public class PAN1 : BasePane, IUserDataContainer
    {
        public override string Signature { get; } = "pan1";

        private byte _flags1;

        [DisplayName("Is Visible"), CategoryAttribute("Flags")]
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

        [DisplayName("Influence Alpha"), CategoryAttribute("Alpha")]
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

        [DisplayName("User Data"), CategoryAttribute("User Data")]
        public UserData UserData { get; set; }

        public PAN1() : base()
        {
            LoadDefaults();
        }

        public PAN1(BxlytHeader header, string name) : base()
        {
            LayoutFile = header;
            LoadDefaults();
            Name = name;
        }

        public override BasePane Copy()
        {
            PAN1 pan1 = new PAN1();
            pan1.Alpha = Alpha;
            pan1.DisplayInEditor = DisplayInEditor;
            pan1.Childern = Childern;
            pan1.Parent = Parent;
            pan1.Name = Name;
            pan1.LayoutFile = LayoutFile;
            pan1.NodeWrapper = NodeWrapper;
            pan1.originX = originX;
            pan1.originY = originY;
            pan1.ParentOriginX = ParentOriginX;
            pan1.ParentOriginY = ParentOriginY;
            pan1.Rotate = Rotate;
            pan1.Scale = Scale;
            pan1.Translate = Translate;
            pan1.Visible = Visible;
            pan1.Height = Height;
            pan1.Width = Width;
            pan1.UserData = UserData;
            pan1.UserDataInfo = UserDataInfo;
            pan1._flags1 = _flags1;
            return pan1;
        }

        public override UserData CreateUserData()
        {
            return new USD1();
        }

        public override void LoadDefaults()
        {
            base.LoadDefaults();

            UserDataInfo = "";
            UserData = null;
            PaneMagFlags = 0;
        }

        public PAN1(FileReader reader, BxlytHeader header) : base()
        {
            LayoutFile = header;
            _flags1 = reader.ReadByte();
            byte origin = reader.ReadByte();
            Alpha = reader.ReadByte();
            PaneMagFlags = reader.ReadByte();
            Name = reader.ReadString(0x18, true);
            UserDataInfo = reader.ReadString(0x8, true);
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
            UserData = new USD1();
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
            writer.WriteString(UserDataInfo, 0x8);
            writer.Write(Translate);
            writer.Write(Rotate);
            writer.Write(Scale);
            writer.Write(Width);
            writer.Write(Height);
        }

        [Browsable(false)]
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
