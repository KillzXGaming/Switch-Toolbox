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
        public BxlytHeader LayoutFile;

        [Browsable(false)]
        public PaneAnimController animController = new PaneAnimController();

        [Browsable(false)]
        public TreeNodeCustom NodeWrapper;

        [DisplayName("Is Visible"), CategoryAttribute("Flags")]
        public virtual bool Visible { get; set; }

        [Browsable(false)]
        public bool IsRoot
        {
            get { return Parent == null; }
        }

        [Browsable(false)]
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

        private string name;

        [DisplayName("Name"), CategoryAttribute("Pane")]
        public string Name
        {
            get { return name; }
            set
            {
                //Adjust necessary parameters if the user changes the name
                if (name != null && LayoutFile != null)
                {
                    //Adjust name table entry
                    if (LayoutFile != null && LayoutFile.PaneLookup.ContainsKey(name))
                        LayoutFile.PaneLookup.Remove(name);

                    //Adjust material reference
                    if (this is IPicturePane)
                        ((IPicturePane)this).Material.SetName(name, value);
                    else if (this is ITextPane)
                        ((ITextPane)this).Material.SetName(name, value);
                    else if (this is IWindowPane)
                    {
                        var wnd = this as IWindowPane;
                        wnd.Content?.Material?.SetName(name, value);

                        if (wnd.WindowFrames != null)
                        {
                            foreach (var frame in wnd.WindowFrames)
                            {
                                if (frame.Material == null) continue;
                                frame.Material.SetName(name, value);
                            }
                        }
                    }
                }

                name = value;

                if (LayoutFile != null && !LayoutFile.PaneLookup.ContainsKey(name))
                    LayoutFile.PaneLookup.Add(name, this);
            }
        }

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

        public Vector3F GetTranslation()
        {
            var posX = Translate.X;
            var posY = Translate.Y;
            var posZ = Translate.Z;
            if (animController.PaneSRT.ContainsKey(LPATarget.TranslateX))
                posX = animController.PaneSRT[LPATarget.TranslateX];
            if (animController.PaneSRT.ContainsKey(LPATarget.TranslateY))
                posY = animController.PaneSRT[LPATarget.TranslateY];
            if (animController.PaneSRT.ContainsKey(LPATarget.TranslateZ))
                posZ = animController.PaneSRT[LPATarget.TranslateZ];
            return new Vector3F(posX, posY, posZ);
        }

        public Vector3F GetRotation()
        {
            var rotX = Rotate.X;
            var rotY = Rotate.Y;
            var rotZ = Rotate.Z;
            if (animController.PaneSRT.ContainsKey(LPATarget.RotateX))
                rotX = animController.PaneSRT[LPATarget.RotateX];
            if (animController.PaneSRT.ContainsKey(LPATarget.RotateY))
                rotY = animController.PaneSRT[LPATarget.RotateY];
            if (animController.PaneSRT.ContainsKey(LPATarget.RotateZ))
                rotZ = animController.PaneSRT[LPATarget.RotateZ];
            return new Vector3F(rotX, rotY, rotZ);
        }

        public Vector2F GetScale()
        {
            var scaX = Scale.X;
            var scaY = Scale.Y;
            if (animController.PaneSRT.ContainsKey(LPATarget.ScaleX))
                scaX = animController.PaneSRT[LPATarget.ScaleX];
            if (animController.PaneSRT.ContainsKey(LPATarget.ScaleY))
                scaY = animController.PaneSRT[LPATarget.ScaleY];
            return new Vector2F(scaX, scaY);
        }

        public Vector2F GetSize()
        {
            var scaX = Width;
            var scaY = Height;
            if (animController.PaneSRT.ContainsKey(LPATarget.SizeX))
                scaX = animController.PaneSRT[LPATarget.SizeX];
            if (animController.PaneSRT.ContainsKey(LPATarget.SizeY))
                scaY = animController.PaneSRT[LPATarget.SizeY];
            return new Vector2F(scaX, scaY);
        }

        public BasePane()
        {
            originX = OriginX.Center;
            originY = OriginY.Center;
            ParentOriginX = OriginX.Center;
            ParentOriginY = OriginY.Center;
        }

        public virtual BasePane Copy()
        {
            return new BasePane();
        }

        public virtual UserData CreateUserData()
        {
            return new UserData();
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
                else if (this is IPartPane)
                    return false;

                return true;
            }
        }

        public void ResetParentTransform(BasePane newParent)
        {
            //We need to get the difference in the parent transform and remove it to the current transform of this pane
            var transform = GetParentTransform(Parent);
            var newParentTransform = GetParentTransform(newParent);
            Translate += transform;
            Translate -= newParentTransform;

            Console.WriteLine($"newParentTransform {newParentTransform.X} {newParentTransform.Y} {newParentTransform.Z}");
            Console.WriteLine($"transform {transform.X} {transform.Y} {transform.Z}");
        }

        private Vector3F GetParentTransform(BasePane parent) {
            return GetParentTransform(parent.Translate);
        }

        private Vector3F GetParentTransform(Vector3F translate)
        {
            if (Parent != null)
                translate = Parent.Translate + Parent.GetParentTransform(translate);

            return translate;

        }

        public void KeepChildrenTransform(float newTransX, float newTransY)
        {
            Vector2F distance = new Vector2F(newTransX - Translate.X, newTransY - Translate.Y);
            KeepChildrenTransform(distance, newTransX, newTransY);
        }

        private void KeepChildrenTransform(Vector2F distance, float newTransX, float newTransY)
        {
            if (HasChildern)
            {
                foreach (var child in Childern)
                {
                    child.Translate -= new Vector3F(distance.X, distance.Y, 0);
                    child.KeepChildrenTransform(child.Translate.X, child.Translate.Y);
                }
            }
        }

        public CustomRectangle TransformParent(CustomRectangle rect)
        {
            return rect.GetTransformedRectangle(Parent, GetTranslation(),GetRotation(), GetScale());
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

        public void TransformRectangle(LayoutViewer.PickAction pickAction, CustomRectangle selectionBox, float pickMouseX, float pickMouseY)
        {
            CustomRectangle currentRectangle = this.CreateRectangle();

            currentRectangle = currentRectangle.GetTransformedRectangle(Parent, GetTranslation(), GetRotation(), GetScale());

            //The selection box can have mutlile panes selected in one box
            //Scaling the edges will have different distances
            float distanceLeft = selectionBox.LeftPoint - currentRectangle.LeftPoint;
            float distanceRight = selectionBox.RightPoint - currentRectangle.RightPoint;
            float distanceTop = selectionBox.TopPoint - currentRectangle.TopPoint;
            float distanceBottom = selectionBox.BottomPoint - currentRectangle.BottomPoint;

            Console.WriteLine("distanceLeft " + distanceLeft);
            Console.WriteLine("distanceRight " + distanceRight);
            Console.WriteLine("distanceTop " + distanceTop);
            Console.WriteLine("distanceBottom " + distanceBottom);


            float posX = Translate.X;
            float posY = Translate.Y;
            float posZ = Translate.Z;


            float pickWidth = pickMouseX;
            float pickHeight = pickMouseY;

            switch (pickAction)
            {
                case LayoutViewer.PickAction.DragLeft:
                    Width += pickMouseX;

                    if (originX == OriginX.Left)
                        posX = Translate.X - pickMouseX;
                    else if (originX == OriginX.Right)
                        posX = Translate.X + pickMouseX;
                    else
                        posX = Translate.X - (pickMouseX / 2);
                    break;
                case LayoutViewer.PickAction.DragRight:

                    if (originX == OriginX.Right)
                    {
                        Width -= pickMouseX;
                        posX = Translate.X + pickMouseX;
                    }
                    else if (originX == OriginX.Left)
                    {
                        Width -= pickMouseX;
                        posX = Translate.X + pickMouseX;
                    }
                    else
                    {
                        Width -= pickMouseX;
                        posX = Translate.X - (pickMouseX / 2);
                    }
                    break;
                case LayoutViewer.PickAction.DragTop:
                    if (originY == OriginY.Top)
                    {
                        Height -= pickMouseY;
                        posY = Translate.Y - pickMouseY;
                    }
                    else if (originY == OriginY.Bottom)
                    {
                        Height -= pickMouseY;
                        posY = Translate.Y - pickMouseY;
                    }
                    else
                    {
                        Height -= pickMouseY;
                        posY = Translate.Y - (pickMouseY / 2);
                    }
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

            if (!Runtime.LayoutEditor.TransformChidlren)
                KeepChildrenTransform(posX, posY);

            Translate = new Vector3F(posX, posY, posZ);
        }

        private Vector2F SetOrientation(float pickWidth, float pickHeight)
        {
            float posX = 0;
            float posY = 0;
            switch (originX)
            {
                case OriginX.Left:
                    Width += pickWidth;
                    posX = pickWidth;
                    break;
                default:
                    Width += pickWidth;
                    posX = pickWidth / 2;
                    break;
            }
            switch (originY)
            {
                case OriginY.Top:
                    Height += pickHeight;
                    posY = pickHeight;
                    break;
                default:
                    Height += pickHeight;
                    posY = pickHeight / 2;
                    break;
            }
            return new Vector2F(posX, posY);
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

        public bool IsHit(CustomRectangle selectionBox)
        {
            var rect = CreateRectangle();
            var transformed = rect.GetTransformedRectangle(Parent, GetTranslation(), GetRotation(), GetScale());
      
            if ((selectionBox.LeftPoint < transformed.RightPoint) &&
                (selectionBox.RightPoint > transformed.LeftPoint) &&
                (selectionBox.TopPoint > transformed.BottomPoint) &&
                (selectionBox.BottomPoint < transformed.TopPoint))
                return true;
            else
                return false;
        }

        public bool IsHit(int X, int Y)
        {
            var rect = CreateRectangle();
            var transformed = rect.GetTransformedRectangle(Parent, GetTranslation(), GetRotation(), GetScale());

            bool isInBetweenX = (X > transformed.LeftPoint) && (X < transformed.RightPoint) ||
                                (X < transformed.LeftPoint) && (X > transformed.RightPoint);

            bool isInBetweenY = (Y > transformed.BottomPoint) && (Y < transformed.TopPoint) ||
                                (Y < transformed.BottomPoint) && (Y > transformed.TopPoint);

            if (isInBetweenX && isInBetweenY)
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

    public class BxlytAlphaCompare
    {
        public GfxAlphaFunction CompareMode { get; set; }
        public float Value { get; set; }

        public BxlytAlphaCompare()
        {
            CompareMode = GfxAlphaFunction.Always;
            Value = 0;
        }

        public BxlytAlphaCompare(FileReader reader, BxlytHeader header)
        {
            CompareMode = reader.ReadEnum<GfxAlphaFunction>(false);
            reader.ReadBytes(0x3);
            Value = reader.ReadSingle();
        }

        public void Write(FileWriter writer)
        {
            writer.Write(CompareMode, false);
            writer.Seek(3);
            writer.Write(Value);
        }
    }

    public class BxlytBlendMode
    {
        public GX2BlendOp BlendOp { get; set; }
        public GX2BlendFactor SourceFactor { get; set; }
        public GX2BlendFactor DestFactor { get; set; }
        public GX2LogicOp LogicOp { get; set; }

        public BxlytBlendMode()
        {
            BlendOp = GX2BlendOp.Add;
            SourceFactor = GX2BlendFactor.SourceAlpha;
            DestFactor = GX2BlendFactor.SourceInvAlpha;
            LogicOp = GX2LogicOp.Disable;
        }

        public BxlytBlendMode(FileReader reader, BxlytHeader header)
        {
            BlendOp = (GX2BlendOp)reader.ReadByte();
            SourceFactor = (GX2BlendFactor)reader.ReadByte();
            DestFactor = (GX2BlendFactor)reader.ReadByte();
            LogicOp = (GX2LogicOp)reader.ReadByte();
        }

        public void Write(FileWriter writer)
        {
            writer.Write(BlendOp, false);
            writer.Write(SourceFactor, false);
            writer.Write(DestFactor, false);
            writer.Write(LogicOp, false);
        }

        public enum GX2BlendFactor : byte
        {
            Factor0 = 0,
            Factor1 = 1,
            DestColor = 2,
            DestInvColor = 3,
            SourceAlpha = 4,
            SourceInvAlpha = 5,
            DestAlpha = 6,
            DestInvAlpha = 7,
            SourceColor = 8,
            SourceInvColor = 9
        }

        public enum GX2BlendOp : byte
        {
            Disable = 0,
            Add = 1,
            Subtract = 2,
            ReverseSubtract = 3,
            SelectMin = 4,
            SelectMax = 5
        }

        public enum GX2LogicOp : byte
        {
            Disable = 0,
            NoOp = 1,
            Clear = 2,
            Set = 3,
            Copy = 4,
            InvCopy = 5,
            Inv = 6,
            And = 7,
            Nand = 8,
            Or = 9,
            Nor = 10,
            Xor = 11,
            Equiv = 12,
            RevAnd = 13,
            InvAd = 14,
            RevOr = 15,
            InvOr = 16
        }
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
        ushort MaterialIndex { get; set; }

        BxlytMaterial Material { get; set; }

        TexCoord[] TexCoords { get; set; }

        STColor8 ColorTopLeft { get; set; }
        STColor8 ColorTopRight { get; set; }
        STColor8 ColorBottomLeft { get; set; }
        STColor8 ColorBottomRight { get; set; }
    }

    public interface IBoundryPane
    {

    }

    public interface ITextPane
    {
        ushort MaterialIndex { get; set; }

        string Text { get; set; }
        OriginX HorizontalAlignment { get; set; }
        OriginY VerticalAlignment { get; set; }

        List<string> GetFonts { get; }

        ushort TextLength { get; set; }
        ushort MaxTextLength { get; set; }
        ushort RestrictedLength { get; }

        BxlytMaterial Material { get; set; }
        Toolbox.Library.Rendering.RenderableTex RenderableFont { get; set; }

        byte TextAlignment { get; set; }
        LineAlign LineAlignment { get; set; }

        float ItalicTilt { get; set; }

        STColor8 FontTopColor { get; set; }
        STColor8 FontBottomColor { get; set; }
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
        ushort FontIndex { get; set; }
    }

    public interface IPartPane
    {
        string LayoutFileName { get; set; }
    }

    public interface IWindowPane
    {
        System.Drawing.Color[] GetVertexColors();

        void ReloadFrames();
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

        private BxlytHeader LayoutFile;

        public BxlytWindowContent(BxlytHeader header, string name)
        {
            LayoutFile = header;
            ColorTopLeft = STColor8.White;
            ColorTopRight = STColor8.White;
            ColorBottomLeft = STColor8.White;
            ColorBottomRight = STColor8.White;

            TexCoords.Add(new TexCoord());

            //Add new material
            Material = header.CreateNewMaterial($"{name}_C");
            MaterialIndex = (ushort)header.AddMaterial(Material);
        }

        public BxlytWindowContent(FileReader reader, BxlytHeader header)
        {
            LayoutFile = header;

            ColorTopLeft = reader.ReadColor8RGBA();
            ColorTopRight = reader.ReadColor8RGBA();
            ColorBottomLeft = reader.ReadColor8RGBA();
            ColorBottomRight = reader.ReadColor8RGBA();
            MaterialIndex = reader.ReadUInt16();
            byte UVCount = reader.ReadByte();
            reader.ReadByte(); //padding

            for (int i = 0; i < UVCount; i++)
                TexCoords.Add(new TexCoord()
                {
                    TopLeft = reader.ReadVec2SY(),
                    TopRight = reader.ReadVec2SY(),
                    BottomLeft = reader.ReadVec2SY(),
                    BottomRight = reader.ReadVec2SY(),
                });

            Material = LayoutFile.GetMaterial(MaterialIndex);
        }

        public void Write(FileWriter writer)
        {
            writer.Write(ColorTopLeft);
            writer.Write(ColorTopRight);
            writer.Write(ColorBottomLeft);
            writer.Write(ColorBottomRight);
            writer.Write(MaterialIndex);
            writer.Write((byte)TexCoords.Count);
            writer.Write((byte)0);
            foreach (var texCoord in TexCoords)
            {
                writer.Write(texCoord.TopLeft);
                writer.Write(texCoord.TopRight);
                writer.Write(texCoord.BottomLeft);
                writer.Write(texCoord.BottomRight);
            }
        }
    }

    public class BxlytWindowFrame
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytMaterial Material { get; set; }

        public ushort MaterialIndex;
        public WindowFrameTexFlip TextureFlip { get; set; }

        public BxlytWindowFrame(BxlytHeader header, string materialName)
        {
            TextureFlip = WindowFrameTexFlip.None;

            var mat = header.CreateNewMaterial(materialName);
            MaterialIndex = (ushort)header.AddMaterial(mat);
            Material = mat;
        }

        public BxlytWindowFrame(FileReader reader, BxlytHeader header)
        {
            MaterialIndex = reader.ReadUInt16();
            TextureFlip = (WindowFrameTexFlip)reader.ReadByte();
            reader.ReadByte(); //padding

            Material = header.GetMaterial(MaterialIndex);
        }

        public void Write(FileWriter writer)
        {
            writer.Write(MaterialIndex);
            writer.Write(TextureFlip, false);
            writer.Write((byte)0);
        }
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
        public TextureManager TextureManager = new TextureManager();

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
            TextureManager?.Dispose();
            FileInfo.Unload();
        }
    }

    public class BxlanHeader : LayoutHeader
    {
        public BxlanPAT1 AnimationTag;
        public BxlanPAI1 AnimationInfo;

        public BxlanPaiEntry TryGetTag(string name)
        {
            for (int i = 0; i < AnimationInfo.Entries?.Count; i++)
            {
                if (AnimationInfo.Entries[i].Name == name)
                    return AnimationInfo.Entries[i];
            }
            return null;
        }

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
        public System.Windows.Forms.TreeNode MaterialFolder;

        [Browsable(false)]
        public System.Windows.Forms.TreeNode TextureFolder;

        [Browsable(false)]
        public System.Windows.Forms.TreeNode FontFolder;

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

        public virtual short AddMaterial(BxlytMaterial material, ushort index) { return -1; }
        public virtual short AddMaterial(BxlytMaterial material) { return -1; }
        public virtual List<int> AddMaterial(List<BxlytMaterial> materials) { return new List<int>(); }

        public virtual void TryRemoveMaterial(BxlytMaterial material) { }
        public virtual void TryRemoveMaterial(List<BxlytMaterial> materials) { }

        public virtual BxlytMaterial CreateNewMaterial(string name)
        {
            return new BxlytMaterial();
        }

        public virtual BxlytMaterial GetMaterial(ushort index)
        {
            return new BxlytMaterial();
        }

        public void RemoveTextureReferences(string texture)
        {
            foreach (var mat in GetMaterials())
                mat.TryRemoveTexture(texture);
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

        public virtual int AddFont(string name)
        {
            return -1;
        }

        public virtual void RemoveTexture(string name)
        {

        }

        public virtual int AddTexture(string name)
        {
            return -1;
        }

        public void AddPaneToTable(BasePane pane)
        {
            if (!PaneLookup.ContainsKey(pane.Name))
                PaneLookup.Add(pane.Name, pane);
        }

        public void AddPane(BasePane pane, string parent)
        {
            if (PaneLookup.ContainsKey(parent))
                AddPane(pane, PaneLookup[parent]);
            else
                throw new Exception($"Failed to find parent pane! [{parent}]");
        }

        public void AddPane(BasePane pane, BasePane parent)
        {
            if (!PaneLookup.ContainsKey(pane.Name))
                PaneLookup.Add(pane.Name, pane);

            pane.Parent = parent;
            parent.Childern.Add(pane);
            parent.NodeWrapper.Nodes.Add(pane.NodeWrapper);

            Console.WriteLine("newpane " + pane.Name + " " + pane.Parent.Name);
        }

        public void RemovePanes(List<BasePane> panes, BasePane rootPane)
        {
            Console.WriteLine("RemovePanes num " + panes.Count);

            for (int i = 0; i < panes.Count; i++)
            {
                Console.WriteLine($"RemovePanes {panes[i].Name} {panes[i]}");

                //We need to remove any materials that the material referenced
                if (panes[i] is IPicturePane)
                    TryRemoveMaterial(((IPicturePane)panes[i]).Material);
                if (panes[i] is ITextPane)
                    TryRemoveMaterial(((ITextPane)panes[i]).Material);
                if (panes[i] is IWindowPane)
                {
                    var wnd = panes[i] as IWindowPane;

                    List<BxlytMaterial> materials = new List<BxlytMaterial>();
                    var matC = wnd.Content.Material;
                    materials.Add(matC);
                    foreach (var windowFrame in wnd.WindowFrames)
                        materials.Add(windowFrame.Material);

                    TryRemoveMaterial(materials);
                    materials.Clear();
                }
            }

            List<BasePane> topMostPanes = new List<BasePane>();
            GetTopMostPanes(panes, topMostPanes, rootPane);

            foreach (var pane in topMostPanes)
            {
                pane.Parent.NodeWrapper.Nodes.Remove(pane.NodeWrapper);
                pane.Parent.Childern.Remove(pane);
            }

            for (int i = 0; i < panes.Count; i++)
            {
                if (PaneLookup.ContainsKey(panes[i].Name))
                    PaneLookup.Remove(panes[i].Name);
            }
        }

        //Loop through each pane in the heiarchy until it finds the first set of panes
        //The topmost panes are only going to be removed for adding with redo to be easier
        private void GetTopMostPanes(List<BasePane> panes, List<BasePane> topMost, BasePane root)
        {
            foreach (var child in root.Childern)
            {
                if (panes.Contains(child))
                    topMost.Add(child);
            }

            if (topMost.Count == 0)
            {
                foreach (var child in root.Childern)
                    GetTopMostPanes(panes, topMost, child);
            }
        }
    }

    public class BxlytMaterial
    {
        //Setup some enable booleans
        //These determine wether to switch to default values or not
        //While i could null out the instances of each, it's best to keep those intact
        //incase the settings get renabled, which keeps the previous data

        [Browsable(false)]
        public bool EnableAlphaCompare { get; set; }

        [Browsable(false)]
        public bool EnableBlend { get; set; }

        [Browsable(false)]
        public bool EnableBlendLogic { get; set; }

        [Browsable(false)]
        public bool EnableIndParams { get; set; }

        [Browsable(false)]
        public bool EnableFontShadowParams { get; set; }

        [Browsable(false)]
        public TreeNodeCustom NodeWrapper;

        public bool TryRemoveTexture(string name)
        {
            int removeIndex = -1;
            for (int i = 0; i < TextureMaps?.Length; i++)
            {
                if (TextureMaps[i].Name == name)
                    removeIndex = i;
            }

            if (removeIndex != -1)
            {
                TextureMaps = TextureMaps.RemoveAt(removeIndex);
                return true;
            }

            return false;
        }

        public void RemoveNodeWrapper()
        {
            if (NodeWrapper.Parent != null)
            {
                var parent = NodeWrapper.Parent;
                parent.Nodes.Remove(NodeWrapper);
            }
        }

        public virtual void AddTexture(string texture)
        {
            int index = ParentLayout.AddTexture(texture);
            BxlytTextureRef textureRef = new BxlytTextureRef();
            textureRef.ID = (short)index;
            textureRef.Name = texture;
            TextureMaps = TextureMaps.AddToArray(textureRef);
            TextureTransforms = TextureTransforms.AddToArray(new BxlytTextureTransform());
        }

        [DisplayName("Texture Transforms"), CategoryAttribute("Texture")]
        public BxlytTextureTransform[] TextureTransforms { get; set; }

        [Browsable(false)]
        public MaterialAnimController animController = new MaterialAnimController();

        [DisplayName("Name"), CategoryAttribute("General")]
        public virtual string Name { get; set; }

        [Browsable(false)]
        public BxlytHeader ParentLayout;

        public void SetName(string oldName, string newName)
        {
            if (Name == null) return;

            Name = Name.Replace(oldName, newName);
        }

        [DisplayName("Black Color"), CategoryAttribute("Color")]
        public virtual STColor8 WhiteColor { get; set; } = STColor8.White;

        [DisplayName("White Color"), CategoryAttribute("Color")]
        public virtual STColor8 BlackColor { get; set; } = STColor8.Black;

        [DisplayName("Blend Mode"), CategoryAttribute("Blend")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytBlendMode BlendMode { get; set; }

        [DisplayName("Blend Mode Logic"), CategoryAttribute("Blend")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytBlendMode BlendModeLogic { get; set; }

        [DisplayName("Alpha Compare"), CategoryAttribute("Alpha")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public BxlytAlphaCompare AlphaCompare { get; set; }

        [DisplayName("Thresholding Alpha Interpolation"), CategoryAttribute("Alpha")]
        public virtual bool ThresholdingAlphaInterpolation { get; set; }

        [Browsable(false)]
        public virtual BxlytShader Shader { get; set; }

        [DisplayName("Texture Maps"), CategoryAttribute("Texture")]
        public BxlytTextureRef[] TextureMaps { get; set; }

        [DisplayName("Tev Stages"), CategoryAttribute("Tev")]
        public BxlytTevStage[] TevStages { get; set; }
    }

    public class BxlytTevStage
    {
        public TevMode ColorMode { get; set; }
        public TevMode AlphaMode { get; set; }
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

        public BxlytTextureTransform()
        {
            Translate = new Vector2F(0,0);
            Scale = new Vector2F(1, 1);
            Rotate = 0;
        }
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
            var topLeft = RotateZPoint(LeftPoint, TopPoint, rotate);
            var bottomRight = RotateZPoint(RightPoint, BottomPoint, rotate);

            return new CustomRectangle(
                (int)topLeft.X,
                (int)bottomRight.X,
                (int)topLeft.Y,
                (int)bottomRight.Y);
        }

        private OpenTK.Vector2 RotateZPoint(float p1, float p2,  float rotate)
        {
            double angleInRadians = rotate * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            var centerPoint = new OpenTK.Vector2(0,0);

            return new OpenTK.Vector2(
                (int)(p1 * cosTheta - p2 * sinTheta),
                (int)(p1 * sinTheta + p2 * cosTheta));

            return new OpenTK.Vector2(
                (int)
            (cosTheta * (p1 - centerPoint.X) -
            sinTheta * (p2 - centerPoint.Y) + centerPoint.X),
                (int)
            (sinTheta * (p1 - centerPoint.X) +
            cosTheta * (p2 - centerPoint.Y) + centerPoint.Y));
        }

        public CustomRectangle GetTransformedRectangle(BasePane parent, Vector3F Transform, Vector3F Rotate, Vector2F scale)
        {
            var rect = this.RotateZ(Rotate.Z);
            rect = new CustomRectangle(
                (int)(((rect.LeftPoint * scale.X) + Transform.X)),
                (int)(((rect.RightPoint * scale.X) + Transform.X)),
                (int)(((rect.TopPoint * scale.Y) + Transform.Y)),
                (int)(((rect.BottomPoint * scale.Y) + Transform.Y)));

            if (parent != null)
                return parent.TransformParent(rect);
            else
                return rect;
        }

        public int Width
        {
            get { return LeftPoint - RightPoint; }
        }

        public int Height
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
