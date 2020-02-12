using Toolbox.Library.IO;
using System;

namespace LayoutBXLYT
{
    public class ProjectionTexGenParam
    {
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }

        public bool IsFittingLayoutSize
        {
            get { return Convert.ToBoolean(flags & 0x1); }
        }

        public bool IsFittingPaneSize
        {
            get { return Convert.ToBoolean(flags & 0x2); }
        }

        public bool IsAdjustPRojectionSR
        {
            get { return Convert.ToBoolean(flags & 0x3); }
        }

        byte flags;

        public ProjectionTexGenParam(FileReader reader, BxlytHeader header)
        {
            PosX = reader.ReadSingle();
            PosY = reader.ReadSingle();
            ScaleX = reader.ReadSingle();
            ScaleY = reader.ReadSingle();
            flags = reader.ReadByte();
            reader.Seek(3);
        }

        public void Write(FileWriter writer)
        {
            writer.Write(PosX);
            writer.Write(PosY);
            writer.Write(ScaleX);
            writer.Write(ScaleY);
            writer.Write(flags);
            writer.Seek(3);
        }
    }
}
