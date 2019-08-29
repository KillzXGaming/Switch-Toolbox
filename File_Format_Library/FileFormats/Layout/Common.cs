using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.Maths;
using Toolbox.Library.IO;

namespace LayoutBXLYT
{
    public class BasePane : SectionCommon
    {
        public bool DisplayInEditor { get; set; } = true;

        public string Name { get; set; }

        public Vector3F Translate { get; set; }
        public Vector3F Rotate { get; set; }
        public Vector2F Scale { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public virtual OriginX originX { get; set; }
        public virtual OriginY originY { get; set; }
        public virtual OriginX ParentOriginX { get; set; }
        public virtual OriginY ParentOriginY { get; set; }

        public BasePane Parent { get; set; }

        public List<BasePane> Childern { get; set; } = new List<BasePane>();

        public bool HasChildern
        {
            get { return Childern.Count > 0; }
        }


        public CustomRectangle CreateRectangle()
        {
            int left = 0;
            int right = 0;
            int top = 0;
            int bottom = 0;

            //Do origin transforms
            var transformed = TransformOrientation((int)Width, (int)Height);

            //Now do parent transforms

            Vector2 ParentWH = new Vector2(0, 0);
            if (Parent != null && Parent is BasePane)
                ParentWH = new Vector2((int)Parent.Width, (int)Parent.Height);

            var transformedParent = TransformOrientation(ParentWH.X, ParentWH.Y);

            //  if (Parent != null)
            //      transformed -= transformedParent;

            return new CustomRectangle(
                transformed.X,
                transformed.Y,
                transformed.Z,
                transformed.W);
        }

        private Vector4 TransformOrientation(int Width, int Height)
        {
            int left = 0;
            int right = 0;
            int top = 0;
            int bottom = 0;

            if (originX == OriginX.Left)
                right = Width;
            else if (originX == OriginX.Right)
                left = -Width;
            else //To center
            {
                left = -Width / 2;
                right = Width / 2;
            }

            if (originY == OriginY.Top)
                bottom = Height;
            else if (originY == OriginY.Bottom)
                top = -Height;
            else //To center
            {
                top = -Height / 2;
                bottom = Height / 2;
            }

            return new Vector4(left, right, top, bottom);
        }
    }

    public enum OriginX : byte
    {
        Center = 0,
        Left = 1,
        Right = 2
    };

    public enum OriginY : byte
    {
        Center = 0,
        Top = 1,
        Bottom = 2
    };

    public class BxlytHeader
    {
        public BasePane RootPane { get; set; }

        internal uint Version;

        public string VersionFull
        {
            get
            {
                var major = Version >> 24;
                var minor = Version >> 16 & 0xFF;
                var micro = Version >> 8 & 0xFF;
                var micro2 = Version & 0xFF;
                return $"{major} {minor} {micro} {micro2}";
            }
        }

        public uint VersionMajor
        {
            get { return Version >> 24; }
        }

        public uint VersionMinor
        {
            get { return Version >> 16 & 0xFF; }
        }

        public uint VersionMicro
        {
            get { return Version >> 8 & 0xFF; }
        }

        public uint VersionMicro2
        {
            get { return Version & 0xFF; }
        }
    }

    public class SectionCommon
    {
        internal string Signature { get; set; }
        internal uint SectionSize { get; set; }

        internal byte[] Data { get; set; }

        public virtual void Write(FileWriter writer, BxlytHeader header)
        {
            writer.WriteSignature(Signature);
            if (Data != null)
            {
                writer.Write(Data.Length);
                writer.Write(Data);
            }
        }
    }

    public class CustomRectangle
    {
        public int LeftPoint;
        public int RightPoint;
        public int TopPoint;
        public int BottomPoint;

        public CustomRectangle(int left, int right, int top, int bottom)
        {
            LeftPoint = left;
            RightPoint = right;
            TopPoint = top;
            BottomPoint = bottom;
        }
    }
}
