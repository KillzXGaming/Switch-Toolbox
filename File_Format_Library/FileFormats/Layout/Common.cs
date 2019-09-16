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
        public bool IsRoot = false;

        public bool ParentIsRoot
        {
            get { return Parent != null && Parent.IsRoot; }
        }

        internal RenderablePane renderablePane;

        [DisplayName("Alpha"), CategoryAttribute("Alpha")]
        public byte Alpha { get; set; }

        [DisplayName("Influence Alpha"), CategoryAttribute("Alpha")]
        public virtual bool InfluenceAlpha { get; set; }

        [Browsable(false)]
        public virtual bool DisplayInEditor { get; set; } = true;

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

        [DisplayName("Height"), CategoryAttribute("Pane")]
        public float Height { get; set; }

        [DisplayName("Origin X"), CategoryAttribute("Origin")]
        public virtual OriginX originX { get; set; }

        [DisplayName("Origin X"), CategoryAttribute("Origin")]
        public virtual OriginY originY { get; set; }

        [DisplayName("Parent Origin X"), CategoryAttribute("Origin")]
        public virtual OriginX ParentOriginX { get; set; }

        [DisplayName("Parent Origin Y"), CategoryAttribute("Origin")]
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

        [Browsable(false)]
        public CustomRectangle Rectangle
        {
            get
            {
                UpdateRectangle();
                return rectangle;
            }
        }

        private void UpdateRectangle()
        {
            rectangle = CreateRectangle();
        }

        public CustomRectangle CreateRectangle(uint width, uint height)
        {
            //Do origin transforms
            var transformed = TransformOrientation((int)width, (int)height, originX, originY);
            var parentTransform = ParentOriginTransform(transformed);

            return new CustomRectangle(
                parentTransform.X,
                parentTransform.Y,
                parentTransform.Z,
                parentTransform.W);
        }

        public CustomRectangle CreateRectangle()
        {
            //Do origin transforms
            var transformed = TransformOrientation((int)Width, (int)Height, originX, originY);
            var parentTransform = ParentOriginTransform(transformed);

            return new CustomRectangle(
                parentTransform.X,
                parentTransform.Y,
                parentTransform.Z,
                parentTransform.W);
        }

        //Get the previous transform from the parent origin
        private Vector4 ParentOriginTransform(Vector4 points)
        {
            //Dont shift the root or the first child of the root
            //The parent setting shouldn't be set, but it doesn't hurt to do this
            if (IsRoot || ParentIsRoot || Parent == null)
                return points;

            var transformedPosition = TransformOrientationPosition((int)Parent.Width, (int)Parent.Height, ParentOriginX, ParentOriginY);
            var transformed = ShiftRectangle(transformedPosition, points);
            if (Parent != null)
                return Parent.ParentOriginTransform(transformed);

            return transformed;
        }

        private static Vector4 ShiftRectangle(Vector2 position, Vector4 points)
        {
            int left = points[0] + position.X;
            int right = points[1] + position.X;
            int top = points[2] + position.Y;
            int bottom = points[3] + position.Y;
            return new Vector4(left, right, top, bottom);
        }

        private static Vector2 TransformOrientationPosition(int Width, int Height, OriginX originX, OriginY originY)
        {
            int x = 0;
            int y = 0;

            if (originX == OriginX.Left)
                x = -(Width / 2);
            else if (originX == OriginX.Right)
                x = (Width / 2);

            if (originY == OriginY.Top)
                y = Height / 2;
            else if (originY == OriginY.Bottom)
                y = -(Height / 2);

            return new Vector2(x, y);
        }

        private static Vector4 TransformOrientation(int Width, int Height, OriginX originX, OriginY originY)
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
                bottom = -Height;
            else if (originY == OriginY.Bottom)
                top = Height;
            else //To center
            {
                top = Height / 2;
                bottom = -Height / 2;
            }

            return new Vector4(left, right, top, bottom);
        }

        public bool IsHit(int X, int Y)
        {
            if ((X > Translate.X) && (X < Translate.X + Width) &&
                (Y > Translate.Y) && (Y < Translate.Y + Height))
                return true;
            else
                return false;
        }
    }

    public enum FilterMode
    {
        Near = 0,
        Linear = 1
    }

    public enum WrapMode
    {
        Clamp = 0,
        Repeat = 1,
        Mirror = 2
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

    public interface IUserDataContainer
    {
        UserData UserData { get; set; }
    }

    public class BxlytTextureRef
    {
        public short ID { get; set; }

        public string Name { get; set; }

        public virtual WrapMode WrapModeU { get; set; }
        public virtual WrapMode WrapModeV { get; set; }
        public virtual FilterMode MinFilterMode { get; set; }
        public virtual FilterMode MaxFilterMode { get; set; }
    }

    public class UserData : SectionCommon
    {
        public List<UserDataEntry> Entries { get; set; }

        public UserData()
        {
            Entries = new List<UserDataEntry>();
        }

        public override void Write(FileWriter writer, LayoutHeader header)
        {
        }
    }

    public class UserDataEntry
    {
        public string Name { get; set; }
        public UserDataType Type { get; set; }
        public byte Unknown { get; set; }

        public object data;

        public string GetString()
        {
            return (string)data;
        }

        public float[] GetFloats()
        {
            return (float[])data;
        }

        public int[] GetInts()
        {
            return (int[])data;
        }

        public void SetValue(string value)
        {
            data = value;
            Type = UserDataType.String;
        }

        public void SetValue(float[] value)
        {
            data = value;
            Type = UserDataType.Float;
        }

        public void SetValue(int[] value)
        {
            data = value;
            Type = UserDataType.Int;
        }

        internal long _pos;
    }

    public enum UserDataType : byte
    {
        String,
        Int,
        Float,
    }

    public enum AnimationTarget : byte
    {
        Pane = 0,
        Material = 1
    }

    public enum KeyType : byte
    {
        Uin16 = 1,
        Float = 2,
    }

    public enum LPATarget : byte
    {
        TranslateX = 0x00,
        TranslateY = 0x01,
        TranslateZ = 0x02,
        RotateX = 0x03,
        RotateY = 0x04,
        RotateZ = 0x05,
        ScaleX = 0x06,
        ScaleY = 0x07,
        SizeX = 0x08,
        SizeZ = 0x09,
    }

    public enum LTSTarget : byte
    {
        TranslateS = 0x00,
        TranslateT = 0x01,
        Rotate = 0x02,
        ScaleS = 0x03,
        ScaleT = 0x04,
    }

    public enum LVITarget : byte
    {
        Visibility = 0x00,
    }

    public enum LVCTarget : byte
    {
        LeftTopRed = 0x00,
        LeftTopGreen = 0x01,
        LeftTopBlue = 0x02,
        LeftTopAlpha = 0x03,

        RightTopRed = 0x04,
        RightTopGreen = 0x05,
        RightTopBlue = 0x06,
        RightTopAlpha = 0x07,

        LeftBottomRed = 0x08,
        LeftBottomGreen = 0x09,
        LeftBottomBlue = 0x0A,
        LeftBottomAlpha = 0x0B,

        RightBottomRed = 0x0C,
        RightBottomGreen = 0x0D,
        RightBottomBlue = 0x0E,
        RightBottomAlpha = 0x0F,

        PaneAlpha = 0x10,
    }

    public enum LTPTarget : byte
    {
        Image = 0x00,
    }

    public enum LMCTarget : byte
    {
        BlackColorRed,
        BlackColorGreen,
        BlackColorBlue,
        BlackColorAlpha,
        WhiteColorRed,
        WhiteColorGreen,
        WhiteColorBlue,
        WhiteColorAlpha,
    }

    public enum LFSTarget : byte
    {
        FontShadowBlackColorRed,
        FontShadowBlackColorGreen,
        FontShadowBlackColorBlue,
        FontShadowBlackColorAlpha,
        FontShadowWhiteColorRed,
        FontShadowWhiteColorGreen,
        FontShadowWhiteColorBlue,
        FontShadowWhiteColorAlpha,
    }

    public enum LCTTarget : byte
    {
        FontShadowBlackColorRed,
        FontShadowBlackColorGreen,
        FontShadowBlackColorBlue,
        FontShadowBlackColorAlpha,
        FontShadowWhiteColorRed,
        FontShadowWhiteColorGreen,
        FontShadowWhiteColorBlue,
        FontShadowWhiteColorAlpha,
    }

    public enum WindowKind
    {
        Around = 0,
        Horizontal = 1,
        HorizontalNoContent = 2
    }

    public enum WindowFrameTexFlip : byte
    {
        None = 0,
        FlipH = 1,
        FlipV = 2,
        Rotate90 = 3,
        Rotate180 = 4,
        Rotate270 = 5
    }

    public interface IWindowPane
    {
        bool UseOneMaterialForAll { get; set; }
        bool UseVertexColorForAll { get; set; }
        WindowKind WindowKind { get; set; }
        bool NotDrawnContent { get; set; }
        ushort StretchLeft { get; set; }
        ushort StretchRight { get; set; }
        ushort StretchTop { get; set; }
        ushort StretchBottm { get; set; }
        ushort FrameElementLeft { get; set; }
        ushort FrameElementRight { get; set; }
        ushort FrameElementTop { get; set; }
        ushort FrameElementBottm { get; set; }
        byte FrameCount { get; set; }

        BxlytWindowContent Content { get; set; }

        [Browsable(false)]
        List<BxlytWindowFrame> WindowFrames { get; set; }
    }

    public class BxlytWindowContent
    {
        public STColor8 ColorTopLeft { get; set; }
        public STColor8 ColorTopRight { get; set; }
        public STColor8 ColorBottomLeft { get; set; }
        public STColor8 ColorBottomRight { get; set; }

        public ushort MaterialIndex { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public virtual BxlytMaterial Material { get; set; }

        public List<TexCoord> TexCoords = new List<TexCoord>();
    }

    public class BxlytWindowFrame
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytMaterial Material { get; set; }

        public ushort MaterialIndex;
        public WindowFrameTexFlip TextureFlip;
    }

    public class TexCoord
    {
        public Vector2F TopLeft { get; set; }
        public Vector2F TopRight { get; set; }
        public Vector2F BottomLeft { get; set; }
        public Vector2F BottomRight { get; set; }

        public TexCoord()
        {
            TopLeft = new Vector2F(0, 0);
            TopRight = new Vector2F(1, 0);
            BottomLeft = new Vector2F(0, 1);
            BottomRight = new Vector2F(1, 1);
        }
    }

    public class LayoutHeader : IDisposable
    {
        [Browsable(false)]
        public string FileName
        {
            get { return FileInfo.FileName; }
        }

        [DisplayName("Use Big Endian"), CategoryAttribute("File Settings")]
        public bool IsBigEndian { get; set; }

        [Browsable(false)]
        internal IFileFormat FileInfo;

        [Browsable(false)]
        internal uint Version;

        [DisplayName("Version"), CategoryAttribute("File Settings")]
        public string VersionFull
        {
            get
            {
                return $"{VersionMajor},{VersionMinor},{VersionMicro},{VersionMicro2}";
            }
        }

        [RefreshProperties(RefreshProperties.All)]
        public uint VersionMajor { get; set; }
        [RefreshProperties(RefreshProperties.All)]
        public uint VersionMinor { get; set; }
        [RefreshProperties(RefreshProperties.All)]
        public uint VersionMicro { get; set; }
        [RefreshProperties(RefreshProperties.All)]
        public uint VersionMicro2 { get; set; }

        internal void SetVersionInfo()
        {
            VersionMajor = Version >> 24;
            VersionMinor = Version >> 16 & 0xFF;
            VersionMicro = Version >> 8 & 0xFF;
            VersionMicro2 = Version & 0xFF;
        }

        internal uint SaveVersion()
        {
            return VersionMajor << 24 | VersionMinor << 16 | VersionMicro << 8 | VersionMicro2;
        }

        public static void WriteSection(FileWriter writer, string magic, SectionCommon section, Action WriteMethod = null)
        {
            long startPos = writer.Position;
            writer.WriteSignature(magic);
            writer.Write(uint.MaxValue);
            WriteMethod?.Invoke();
            writer.Align(4);

            long endPos = writer.Position;

            using (writer.TemporarySeek(startPos + 4, System.IO.SeekOrigin.Begin))
            {
                writer.Write((uint)(endPos - startPos));
            }
        }

        public void Dispose()
        {
            FileInfo.Unload();
        }
    }

    public class BxlanHeader : LayoutHeader
    {

    }

    public class BxlytHeader : LayoutHeader
    {
        [Browsable(false)]
        public BasePane RootPane { get; set; }

        [Browsable(false)]
        public BasePane RootGroup { get; set; }

        [Browsable(false)]
        public virtual Dictionary<string, STGenericTexture> GetTextures { get; }

        [Browsable(false)]
        public virtual List<string> Textures { get; }

        [Browsable(false)]
        public virtual List<string> Fonts { get; }

        [Browsable(false)]
        public virtual List<BxlytMaterial> GetMaterials()
        {
            return new List<BxlytMaterial>();
        }
    }

    public class BxlytMaterial
    {
        [DisplayName("Name"), CategoryAttribute("General")]
        public virtual string Name { get; set; }

        [Browsable(false)]
        public virtual BxlytShader Shader { get; set; }

        [DisplayName("Texture Maps"), CategoryAttribute("Texture")]
        public BxlytTextureRef[] TextureMaps { get; set; }
    }

    public class SectionCommon
    {
        [Browsable(false)]
        public virtual string Signature { get; }

        internal uint SectionSize { get; set; }
        internal long StartPosition { get; set; }
        internal byte[] Data { get; set; }

        public SectionCommon()
        {

        }

        public SectionCommon(string signature)
        {
            Signature = signature;
        }

        public virtual void Write(FileWriter writer, LayoutHeader header)
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
