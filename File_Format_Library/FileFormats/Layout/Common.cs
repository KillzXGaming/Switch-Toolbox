using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.Maths;
using Toolbox.Library.IO;
using Toolbox.Library;
using WeifenLuo.WinFormsUI.Docking;
using System.ComponentModel;

namespace LayoutBXLYT
{
    public class BasePane : SectionCommon
    {
        [DisplayName("Alpha"), CategoryAttribute("Alpha")]
        public byte Alpha { get; set; }

        [DisplayName("Influence Alpha"), CategoryAttribute("Alpha")]
        public virtual bool InfluenceAlpha { get; set; }

        [Browsable(false)]
        public bool DisplayInEditor { get; set; } = true;

        [DisplayName("Name"), CategoryAttribute("Pane")]
        public string Name { get; set; }

        [DisplayName("Translate"), CategoryAttribute("Pane")]
        public Vector3F Translate { get; set; }

        [DisplayName("Rotate"), CategoryAttribute("Pane")]
        public Vector3F Rotate { get; set; }

        [DisplayName("Scale"), CategoryAttribute("Pane")]
        public Vector2F Scale { get; set; }

        [DisplayName("Width"), CategoryAttribute("Pane")]
        public float Width { get; set; }

        [DisplayName("Width"), CategoryAttribute("Pane")]
        public float Height { get; set; }

        [DisplayName("Origin X"), CategoryAttribute("Origin")]
        public virtual OriginX originX { get; set; }

        [DisplayName("Origin X"), CategoryAttribute("Origin")]
        public virtual OriginY originY { get; set; }

        [Browsable(false)]
        public virtual OriginX ParentOriginX { get; set; }

        [Browsable(false)]
        public virtual OriginY ParentOriginY { get; set; }

        [Browsable(false)]
        public BasePane Parent { get; set; }

        [Browsable(false)]
        public List<BasePane> Childern { get; set; } = new List<BasePane>();

        [Browsable(false)]
        public bool HasChildern
        {
            get { return Childern.Count > 0; }
        }

        public BasePane()
        {
            originX = OriginX.Center;
            originY = OriginY.Center;
            ParentOriginX = OriginX.Center;
            ParentOriginY = OriginY.Center;
        }

        private CustomRectangle rectangle;
        public CustomRectangle Rectangle
        {
            get
            {
                if (rectangle == null)
                    UpdateRectangle();
                return rectangle;
            }
        }

        private void UpdateRectangle() {
            rectangle = CreateRectangle();
        }

        public CustomRectangle CreateRectangle()
        {
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

        public bool IsHit(int X, int Y)
        {
            if ((X > Rectangle.X) && (X < Rectangle.X + Rectangle.Width) &&
                (Y > Rectangle.Y) && (Y < Rectangle.Y + Rectangle.Height))
                return true;
            else
                return false;
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

    public class BxlytHeader : IDisposable
    {
        public string FileName
        {
            get { return FileInfo.FileName; }
        }

        public bool IsBigEndian { get; set; }

        internal IFileFormat FileInfo;

        public BasePane RootPane { get; set; }

        public BasePane RootGroup { get; set; }

        public virtual Dictionary<string, STGenericTexture> GetTextures { get; }

        public virtual List<string> Textures { get; }

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

        public void Dispose()
        {
            FileInfo.Unload();
        }
    }

    public class SectionCommon 
    {
        public virtual string Signature { get; }
        public uint SectionSize { get; set; }
        public long StartPosition { get; set; }

        internal byte[] Data { get; set; }

        public SectionCommon()
        {

        }

        public SectionCommon(string signature)
        {
            Signature = signature;
        }

        public virtual void Write(FileWriter writer, BxlytHeader header)
        {
            if (Data != null)
                writer.Write(Data);
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

        public float X
        {
            get { return Width / 2; }
        }

        public float Y
        {
            get { return Height / 2; }
        }

        public float Width
        {
            get { return LeftPoint - RightPoint; }
        }

        public float Height
        {
            get { return TopPoint - BottomPoint; }
        }
    }

    public class LayoutDocked : DockContent
    {

    }
}
