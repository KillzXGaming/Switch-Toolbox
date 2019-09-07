﻿using Toolbox.Library.IO;
using Syroot.Maths;

namespace LayoutBXLYT.Cafe
{
    public class TextureTransform
    {
        public Vector2F Translate;
        public float Rotate;
        public Vector2F Scale;

        public TextureTransform() { }

        public TextureTransform(FileReader reader)
        {
            Translate = reader.ReadVec2SY();
            Rotate = reader.ReadSingle();
            Scale = reader.ReadVec2SY();
        }

        public void Write(FileWriter writer)
        {
            writer.Write(Translate);
            writer.Write(Rotate);
            writer.Write(Scale);
        }
    }
}
