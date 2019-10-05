using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.Maths;
using Toolbox.Library.IO;
using Toolbox.Library.Animations;
using Toolbox.Library;
using WeifenLuo.WinFormsUI.Docking;
using System.ComponentModel;

namespace LayoutBXLYT
{
    public class BasePane : SectionCommon
    {
        [Browsable(false)]
        public PaneAnimController animController = new PaneAnimController();

        [Browsable(false)]
        public TreeNodeCustom NodeWrapper;

        [DisplayName("Is Visible"), CategoryAttribute("Flags")]
        public virtual bool Visible { get; set; }
   
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

        public bool IsNullPane
        {
            get
            {
                if (this is IPicturePane)
                    return false;
                else if (this is IWindowPane)
                    return false;
                else if (this is ITextPane)
                    return false;
                else if (this is IBoundryPane)
                    return false;
             else   if (this is IPartPane)
                    return false;

                return true;
            }
        }

        public CustomRectangle TransformParent(CustomRectangle rect)
        {
            return rect.GetTransformedRectangle(Parent, Translate, Scale);
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

        public void TransformRectangle(LayoutViewer.PickAction pickAction, float pickMouseX, float pickMouseY)
        {
            float posX = Translate.X;
            float posY = Translate.Y;
            float posZ = Translate.Z;

            Console.WriteLine("pickMouseX " + pickMouseX);

            switch (pickAction)
            {
                case LayoutViewer.PickAction.DragLeft:
                    Width += pickMouseX;
                    posX = Translate.X - (pickMouseX / 2);
                    break;
                case LayoutViewer.PickAction.DragRight:
                    Width -= pickMouseX;
                    posX = Translate.X - (pickMouseX / 2);
                    break;
                case LayoutViewer.PickAction.DragTop:
                    Height -= pickMouseY;
                    posY = Translate.Y - (pickMouseY / 2);
                    break;
                case LayoutViewer.PickAction.DragBottom:
                    Height += pickMouseY;
                    posY = Translate.Y - (pickMouseY / 2);
                    break;
                case LayoutViewer.PickAction.DragTopLeft:
                    Width += pickMouseX;
                    Height -= pickMouseY;
                    posX = Translate.X - (pickMouseX / 2);
                    posY = Translate.Y - (pickMouseY / 2);
                    break;
                case LayoutViewer.PickAction.DragBottomLeft:
                    Width += pickMouseX;
                    Height += pickMouseY;
                    posX = Translate.X - (pickMouseX / 2);
                    posY = Translate.Y - (pickMouseY / 2);
                    break;
                case LayoutViewer.PickAction.DragBottomRight:
                    Width -= pickMouseX;
                    Height += pickMouseY;
                    posX = Translate.X - (pickMouseX / 2);
                    posY = Translate.Y - (pickMouseY / 2);
                    break;
                case LayoutViewer.PickAction.DragTopRight:
                    Width -= pickMouseX;
                    Height -= pickMouseY;
                    posX = Translate.X - (pickMouseX / 2);
                    posY = Translate.Y - (pickMouseY / 2);
                    break;
            }

            Translate = new Vector3F(posX, posY, posZ);
        }

        public CustomRectangle CreateRectangle()
        {
            //Do origin transforms
            var transformed = new Vector4();
            int paneWidth = (int)Width;
            int paneHeight = (int)Height;
            if (animController.PaneSRT.ContainsKey(LPATarget.SizeX))
                paneWidth = (int)animController.PaneSRT[LPATarget.SizeX];
            if (animController.PaneSRT.ContainsKey(LPATarget.SizeY))
                paneHeight = (int)animController.PaneSRT[LPATarget.SizeY];

            transformed = TransformOrientation(paneWidth, paneHeight, originX, originY);
            var parentTransform = ParentOriginTransform(transformed);

            return new CustomRectangle(
                parentTransform.X,
                parentTransform.Y,
                parentTransform.Z,
                parentTransform.W);
        }

        public static float MixColors(params float[] c)
        {
            float a = c[0];
            for (int i = 1; i < c.Length; i++)
            {
                a *= c[i];
            }
            for (int i = 1; i < c.Length; i++)
            {
                a /= 255f;
            }
            return a / 255f;
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
            var rect = CreateRectangle();
            var transformed = rect.GetTransformedRectangle(Parent, Translate, Scale);

            if ((X > transformed.LeftPoint) && (X < transformed.RightPoint) &&
                (Y > transformed.BottomPoint) && (Y < transformed.TopPoint))
                return true;
            else
                return false;
        }

        public BasePane SearchPane(string name)
        {
            if (Name == name)
                return this;

            foreach (var child in Childern)
            {
                var matchPane = child.SearchPane(name);
                if (matchPane != null) return matchPane;
            }

            return null;
        }
    }

    public class VertexColorHelper
    {
        public static System.Drawing.Color Mix(System.Drawing.Color colorA, System.Drawing.Color colorB, float value)
        {
            byte R = (byte)InterpolationHelper.Lerp(colorA.R, colorB.R, value);
            byte G = (byte)InterpolationHelper.Lerp(colorA.G, colorB.G, value);
            byte B = (byte)InterpolationHelper.Lerp(colorA.B, colorB.B, value);
            byte A = (byte)InterpolationHelper.Lerp(colorA.A, colorB.A, value);
            return System.Drawing.Color.FromArgb(A, R, G, B);
        }
    }


    public class PaneAnimController
    {
        public Dictionary<LPATarget, float> PaneSRT = new Dictionary<LPATarget, float>();
        public Dictionary<LVCTarget, float> PaneVertexColors = new Dictionary<LVCTarget, float>();
        public bool Visibile = true;

        public void ResetAnim()
        {
            Visibile = true;
            PaneSRT.Clear();
            PaneVertexColors.Clear();
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
        SizeY = 0x09,
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
        Image1 = 0x00,
        Image2 = 0x01, //Unsure if mutliple are used but just in case
        Image3 = 0x02,
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
        TextureColorBlendRatio,
        TexColor0Red,
        TexColor0Green,
        TexColor0Blue,
        TexColor0Alpha,
        TexColor1Red,
        TexColor1Green,
        TexColor1Blue,
        TexColor1Alpha,
        TexColor2Red,
        TexColor2Green,
        TexColor2Blue,
        TexColor2Alpha,
        TevKonstantColor0Red,
        TevKonstantColor0Green,
        TevKonstantColor0Blue,
        TevKonstantColor0Alpha,
        TevKonstantColor1Red,
        TevKonstantColor1Green,
        TevKonstantColor1Blue,
        TevKonstantColor1Alpha,
        TevKonstantColor2Red,
        TevKonstantColor2Green,
        TevKonstantColor2Blue,
        TevKonstantColor2Alpha,
    }

    public enum LIMTarget : byte
    {
        Rotation,
        ScaleU,
        ScaleV,
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

    public enum GfxAlphaFunction : byte
    {
        Never = 0,
        Less = 1,
        LessOrEqual = 2,
        Equal = 3,
        NotEqual = 4,
        GreaterOrEqual = 5,
        Greater = 6,
        Always = 7,
    }

    public enum TevMode : byte
    {
        Replace,
        Modulate,
        Add,
        AddSigned,
        Interpolate,
        Subtract,
        AddMultiplicate,
        MultiplcateAdd,
        Overlay,
        Indirect,
        BlendIndirect,
        EachIndirect,
    }


    public enum TevScale
    {
        Scale1,
        Scale2,
        Scale4
    }

    public enum TevSource
    {
        Tex0 = 0,
        Tex1 = 1,
        Tex2 = 2,
        Tex3 = 3,
        Constant = 4,
        Primary = 5,
        Previous = 6,
        Register = 7
    }
    public enum TevColorOp
    {
        RGB = 0,
        InvRGB = 1,
        Alpha = 2,
        InvAlpha = 3,
        RRR = 4,
        InvRRR = 5,
        GGG = 6,
        InvGGG = 7,
        BBB = 8,
        InvBBB = 9
    }
    public enum TevAlphaOp
    {
        Alpha = 0,
        InvAlpha = 1,
        R = 2,
        InvR = 3,
        G = 4,
        InvG = 5,
        B = 6,
        InvB = 7
    }

    public interface IPicturePane
    {
        System.Drawing.Color[] GetVertexColors();
    }

    public interface IBoundryPane
    {

    }

    public interface ITextPane
    {
        string Text { get; set; }
        OriginX HorizontalAlignment { get; set; }
        OriginX VerticalAlignment { get; set; }

        ushort TextLength { get; set; }
        ushort MaxTextLength { get; set; }

        BxlytMaterial Material { get; set; }
        Toolbox.Library.Rendering.RenderableTex RenderableFont { get; set; }

        byte TextAlignment { get; set; }
        LineAlign LineAlignment { get; set; }

        float ItalicTilt { get; set; }

        STColor8 FontForeColor { get; set; }
        STColor8 FontBackColor { get; set; }
        Vector2F FontSize { get; set; }

        float CharacterSpace { get; set; }
        float LineSpace { get; set; }

        Vector2F ShadowXY { get; set; }
        Vector2F ShadowXYSize { get; set; }

        STColor8 ShadowForeColor { get; set; }
        STColor8 ShadowBackColor { get; set; }

        float ShadowItalic { get; set; }

        string TextBoxName { get; set; }

        bool PerCharTransform { get; set; }
        bool RestrictedTextLengthEnabled { get; set; }
        bool ShadowEnabled { get; set; }
        string FontName { get; set; }
    }

    public interface IPartPane
    {

    }

    public interface IWindowPane
    {
        System.Drawing.Color[] GetVertexColors();

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

    public class BXLAN
    {
        public BxlanHeader BxlanHeader;
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
        public WindowFrameTexFlip TextureFlip { get; set; }
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
        public PartsManager PartsManager = new PartsManager();

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
            PartsManager?.Dispose();
            FileInfo.Unload();
        }
    }

    public class BxlanHeader : LayoutHeader
    {
        public BxlanPAT1 AnimationTag;
        public BxlanPAI1 AnimationInfo;

        public virtual void Read(FileReader reader, BXLAN header)
        {

        }

        public virtual void Write(FileWriter writer)
        {

        }

        public LytAnimation GetGenericAnimation()
        {
            return animation;
        }

        private LytAnimation animation;
        public LytAnimation ToGenericAnimation(BxlytHeader parentLayout)
        {
            if (animation == null)
                animation = new LytAnimation(this, parentLayout);
            else
                animation.UpdateLayout(parentLayout);
            return animation;
        }
    }

    public class BxlanPAT1 : SectionCommon
    {
        [DisplayName("Name"), CategoryAttribute("Animation")]
        public string Name { get; set; }

        [DisplayName("Groups"), CategoryAttribute("Animation")]
        public List<string> Groups { get; set; }

        [DisplayName("Start"), CategoryAttribute("Frames")]
        public short StartFrame { get; set; }

        [DisplayName("End"), CategoryAttribute("Frames")]
        public short EndFrame { get; set; }

        [DisplayName("Animation Order"), CategoryAttribute("Parameters")]
        public ushort AnimationOrder { get; set; }

        [DisplayName("Child Binding"), CategoryAttribute("Parameters")]
        public bool ChildBinding { get; set; }
    }

    public class BxlanPAI1 : SectionCommon
    {
        public ushort FrameSize;
        public bool Loop;

        public List<string> Textures { get; set; }
        public List<BxlanPaiEntry> Entries = new List<BxlanPaiEntry>();
    }

    public class BxlanPaiEntry
    {
        [DisplayName("Name"), CategoryAttribute("Animation")]
        public string Name { get; set; }

        [DisplayName("Target"), CategoryAttribute("Animation")]
        public AnimationTarget Target { get; set; }

        public List<BxlanPaiTag> Tags = new List<BxlanPaiTag>();
    }

    public class BxlanPaiTag
    {
        public BxlanPaiTag()
        {

        }

        public BxlanPaiTag(string tag)
        {
            Tag = tag;
        }

        public List<BxlanPaiTagEntry> Entries = new List<BxlanPaiTagEntry>();

        public string Tag;

        public string Type
        {
            get { return TypeDefine.ContainsKey(Tag) ? TypeDefine[Tag] : Tag; }
        }

        public Dictionary<string, string> TypeDefine = new Dictionary<string, string>()
            {
                {"FLPA","PaneSRT" },
                {"FLVI","Visibility" },
                {"FLTS","TextureSRT" },
                {"FLVC","VertexColor" },
                {"FLMC","MaterialColor" },
                {"FLTP","TexturePattern" },
                {"FLIM","IndTextureSRT" },
                {"FLAC","AlphaTest" },
                {"FLCT","FontShadow" },
                {"FLCC","PerCharacterTransformCurve" },

                {"RLPA","PaneSRT" },
                {"RLVI","Visibility" },
                {"RLTS","TextureSRT" },
                {"RLVC","VertexColor" },
                {"RLMC","MaterialColor" },
                {"RLTP","TexturePattern" },
                {"RLIM","IndTextureSRT" },
                {"RLAC","AlphaTest" },
                {"RLCT","FontShadow" },
                {"RLCC","PerCharacterTransformCurve" },

                {"CLPA","PaneSRT" },
                {"CLVI","Visibility" },
                {"CLTS","TextureSRT" },
                {"CLVC","VertexColor" },
                {"CLMC","MaterialColor" },
                {"CLTP","TexturePattern" },
                {"CLIM","IndTextureSRT" },
                {"CLAC","AlphaTest" },
                {"CLCT","FontShadow" },
                {"CLCC","PerCharacterTransformCurve" },
            };
    }


    public class LPATagEntry : BxlanPaiTagEntry
    {
        public override string TargetName => Target.ToString();
        [DisplayName("Target"), CategoryAttribute("Tag")]
        public LPATarget Target
        {
            get { return (LPATarget)AnimationTarget; }
            set { AnimationTarget = (byte)value; }
        }

        public LPATagEntry(FileReader reader, BxlanHeader header) : base(reader, header) { }
    }

    public class LTSTagEntry : BxlanPaiTagEntry
    {
        public override string TargetName => Target.ToString();
        [DisplayName("Target"), CategoryAttribute("Tag")]
        public LTSTarget Target
        {
            get { return (LTSTarget)AnimationTarget; }
            set { AnimationTarget = (byte)value; }
        }

        public LTSTagEntry(FileReader reader, BxlanHeader header) : base(reader, header) { }
    }

    public class LVITagEntry : BxlanPaiTagEntry
    {
        public override string TargetName => Target.ToString();
        [DisplayName("Target"), CategoryAttribute("Tag")]
        public LVITarget Target
        {
            get { return (LVITarget)AnimationTarget; }
            set { AnimationTarget = (byte)value; }
        }

        public LVITagEntry(FileReader reader, BxlanHeader header) : base(reader, header) { }
    }

    public class LVCTagEntry : BxlanPaiTagEntry
    {
        public override string TargetName => Target.ToString();
        [DisplayName("Target"), CategoryAttribute("Tag")]
        public LVCTarget Target
        {
            get { return (LVCTarget)AnimationTarget; }
            set { AnimationTarget = (byte)value; }
        }

        public LVCTagEntry(FileReader reader, BxlanHeader header) : base(reader, header) { }
    }

    public class LMCTagEntry : BxlanPaiTagEntry
    {
        public override string TargetName => Target.ToString();
        [DisplayName("Target"), CategoryAttribute("Tag")]
        public LMCTarget Target
        {
            get { return (LMCTarget)AnimationTarget; }
            set { AnimationTarget = (byte)value; }
        }

        public LMCTagEntry(FileReader reader, BxlanHeader header) : base(reader, header) { }
    }

    public class LTPTagEntry : BxlanPaiTagEntry
    {
        public override string TargetName => Target.ToString();
        [DisplayName("Target"), CategoryAttribute("Tag")]
        public LTPTarget Target
        {
            get { return (LTPTarget)AnimationTarget; }
            set { AnimationTarget = (byte)value; }
        }

        public LTPTagEntry(FileReader reader, BxlanHeader header) : base(reader, header) { }
    }

    public class BxlanPaiTagEntry
    {
        [Browsable(false)]
        public virtual string TargetName
        {
            get { return AnimationTarget.ToString(); }
        }

        public byte AnimationTarget;

        [DisplayName("Index"), CategoryAttribute("Tag")]
        public byte Index { get; set; }

        [DisplayName("Curve Type"), CategoryAttribute("Tag")]
        public CurveType CurveType { get; set; }

        public List<KeyFrame> KeyFrames = new List<KeyFrame>();


        public BxlanPaiTagEntry(FileReader reader, BxlanHeader header)
        {
            long startPos = reader.Position;
            Index = reader.ReadByte();
            AnimationTarget = reader.ReadByte();
            CurveType = reader.ReadEnum<CurveType>(true);
            reader.ReadByte(); //padding
            var KeyFrameCount = reader.ReadUInt16();
            reader.ReadUInt16(); //Padding
            uint keyFrameOffset = reader.ReadUInt32();

            reader.SeekBegin(startPos + keyFrameOffset);
            for (int i = 0; i < KeyFrameCount; i++)
                KeyFrames.Add(new KeyFrame(reader, CurveType));
        }

        public void Write(FileWriter writer, LayoutHeader header)
        {
            long startPos = writer.Position;

            writer.Write(Index);
            writer.Write(AnimationTarget);
            writer.Write(CurveType, true);
            writer.Write((byte)0); //padding
            writer.Write((ushort)KeyFrames.Count);
            writer.Write((ushort)0); //padding
            writer.Write(0); //key offset

            if (KeyFrames.Count > 0)
            {
                writer.WriteUint32Offset(startPos + 8, startPos);
                for (int i = 0; i < KeyFrames.Count; i++)
                    KeyFrames[i].Write(writer, CurveType);
            }
        }
    }

    public enum BorderType : byte
    {
        Standard = 0,
        DeleteBorder = 1,
        RenderTwoCycles = 2,
    };

    public enum LineAlign : byte
    {
        Unspecified = 0,
        Left = 1,
        Center = 2,
        Right = 3,
    };

    public enum CurveType : byte
    {
        Constant,
        Step,
        Hermite,
    }

    public class KeyFrame
    {
        [DisplayName("Slope"), CategoryAttribute("Key Frame")]
        public float Slope { get; set; }
        [DisplayName("Frame"), CategoryAttribute("Key Frame")]
        public float Frame { get; set; }
        [DisplayName("Value"), CategoryAttribute("Key Frame")]
        public float Value { get; set; }

        public KeyFrame(FileReader reader, CurveType curveType)
        {
            switch (curveType)
            {
                case CurveType.Hermite:
                    Frame = reader.ReadSingle();
                    Value = reader.ReadSingle();
                    Slope = reader.ReadSingle();
                    break;
                default:
                    Frame = reader.ReadSingle();
                    Value = reader.ReadInt16();
                    reader.ReadInt16(); //padding
                    break;
            }
        }

        public void Write(FileWriter writer, CurveType curveType)
        {
            switch (curveType)
            {
                case CurveType.Hermite:
                    writer.Write(Frame);
                    writer.Write(Value);
                    writer.Write(Slope);
                    break;
                default:
                    writer.Write(Frame);
                    writer.Write((ushort)Value);
                    writer.Write((ushort)0);
                    break;
            }
        }
    }

    public class BxlytHeader : LayoutHeader
    {
        [Browsable(false)]
        public Dictionary<string, BasePane> PaneLookup = new Dictionary<string, BasePane>();

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

        public BxlytMaterial SearchMaterial(string name)
        {
            var materials = GetMaterials();
            for (int i = 0; i < materials.Count; i++)
            {
                if (materials[i].Name == name)
                    return materials[i];
            }
            return null;
        }

        public void AddPaneToTable(BasePane pane)
        {
            if (!PaneLookup.ContainsKey(pane.Name))
                PaneLookup.Add(pane.Name, pane);
        }


        public void AddPane(BasePane pane, BasePane parent)
        {
            if (parent == null) return;

            if (!PaneLookup.ContainsKey(pane.Name))
                PaneLookup.Add(pane.Name, pane);

            parent.Childern.Add(pane);
            parent.NodeWrapper.Nodes.Add(pane.NodeWrapper);
        }

        public void RemovePanes(List<BasePane> panes)
        {
            for (int i = 0; i < panes.Count; i++)
            {
                if (PaneLookup.ContainsKey(panes[i].Name))
                    PaneLookup.Remove(panes[i].Name);

                if (panes[i].Parent == null)
                    continue;

                var parent = panes[i].Parent;
                parent.Childern.Remove(panes[i]);

                parent.NodeWrapper.Nodes.Remove(panes[i].NodeWrapper);
            }
        }
    }

    public class BxlytMaterial
    {
        [Browsable(false)]
        public MaterialAnimController animController = new MaterialAnimController();

        [DisplayName("Name"), CategoryAttribute("General")]
        public virtual string Name { get; set; }

        [DisplayName("Thresholding Alpha Interpolation"), CategoryAttribute("Alpha")]
        public virtual bool ThresholdingAlphaInterpolation { get; set; }

        [Browsable(false)]
        public virtual BxlytShader Shader { get; set; }

        [DisplayName("Texture Maps"), CategoryAttribute("Texture")]
        public BxlytTextureRef[] TextureMaps { get; set; }
    }

    public class MaterialAnimController
    {
        public Dictionary<LMCTarget, float> MaterialColors = new Dictionary<LMCTarget, float>();
        public Dictionary<LTPTarget, string> TexturePatterns = new Dictionary<LTPTarget, string>();
        public Dictionary<LTSTarget, float> TextureSRTS = new Dictionary<LTSTarget, float>();
        public Dictionary<LIMTarget, float> IndTextureSRTS = new Dictionary<LIMTarget, float>();

        public void ResetAnim()
        {
            MaterialColors.Clear();
            TexturePatterns.Clear();
            TextureSRTS.Clear();
            IndTextureSRTS.Clear();
        }
    }

    public class BxlytTextureTransform
    {
        public Vector2F Translate { get; set; }
        public float Rotate { get; set; }
        public Vector2F Scale { get; set; }
    }

    public class BxlytIndTextureTransform
    {
        public float Rotation { get; set; }
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }
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

        public CustomRectangle RotateZ(float rotate)
        {
            return new CustomRectangle(
                LeftPoint,
                RightPoint,
                TopPoint,
                BottomPoint);
        }

        public CustomRectangle GetTransformedRectangle(BasePane parent, Vector3F Transform, Vector2F Scale)
        {
            var rect = new CustomRectangle(
                (int)(LeftPoint + Transform.X * Scale.X),
                (int)(RightPoint + Transform.X * Scale.X),
                (int)(TopPoint + Transform.Y * Scale.Y),
                (int)(BottomPoint + Transform.Y * Scale.Y));

            if (parent != null)
                return parent.TransformParent(rect);
            else
                return rect;
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

    public class LayoutControlDocked : Toolbox.Library.Forms.STUserControl
    {
        public DockContent DockContent;

        public DockPane Pane => DockContent?.Pane;
    }

    public class LayoutDocked : DockContent
    {

    }
}
