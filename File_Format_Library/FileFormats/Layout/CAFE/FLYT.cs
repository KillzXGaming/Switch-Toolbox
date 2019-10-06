using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using LayoutBXLYT.Cafe;

namespace LayoutBXLYT
{
    public class FLYT
    {
        private static ColorRGBA WhiteRGBA => new ColorRGBA() { r = 255, g = 255, b = 255, a = 255 };
        private static ColorRGBA BlackRGBA => new ColorRGBA() { r = 0, g = 0, b = 0, a = 0 };

        public static BFLYT.Header FromXml(string text)
        {
            BFLYT.Header header = new BFLYT.Header();

            XmlSerializer serializer = new XmlSerializer(typeof(XmlRoot));
            XmlRoot flyt = (XmlRoot)serializer.Deserialize(new StringReader(text));
            var layout = flyt.body.lyt;

            //Do layout screen settings
            header.LayoutInfo = new BFLYT.LYT1();
            var layoutSize = ConvertVector2(layout.screenSetting.layoutSize);
            header.LayoutInfo.Name = "";
            header.LayoutInfo.Width = layoutSize.X;
            header.LayoutInfo.Height = layoutSize.Y;
            header.LayoutInfo.DrawFromCenter = layout.screenSetting.origin == "Normal";

            //Setup root pane
            header.RootPane = new BFLYT.PAN1();
            header.RootPane.Width = layoutSize.X;
            header.RootPane.Height = layoutSize.Y;
            header.RootPane.Name = layout.paneHierarchy.paneTree[0].name;

            //Go through each pane set
            Dictionary<string, BFLYT.PAN1> PaneSet = new Dictionary<string, BFLYT.PAN1>();
            foreach (var pane in layout.paneSet)
                PaneSet.Add(pane.name, ParsePane(pane, header));



            //Go through the tree
            foreach (var node in layout.paneHierarchy.paneTree)
                LoadTree(PaneSet, header.RootPane, node);

            header.RootGroup = new BFLYT.GRP1();
            return header;
        }

        private static void LoadTree(Dictionary<string, BFLYT.PAN1> PaneSet, BasePane parentPane, PaneTree tree)
        {
            foreach (var node in tree.paneTree)
            {
                var childPane = PaneSet[node.name];
                childPane.Parent = parentPane;
                parentPane.Childern.Add(childPane);

                LoadTree(PaneSet, childPane, node);
            }
        }

        private static BFLYT.PAN1 ParsePane(Pane pane, BFLYT.Header header)
        {
            BFLYT.PAN1 pan1 = new BFLYT.PAN1();
            /*  switch (pane.kind)
              {
                  case "Picture":
                      pan1 = new BFLYT.PIC1(header);
                      ParsePicturePane(ref pan1, (PicturePane)pane.Items[0],ref header);
                      break;
                  case "TextBox":
                      pan1 = new BFLYT.TXT1();
                      break;
                  case "Parts":
                      pan1 = new BFLYT.PRT1();
                      break;
                  case "Null":
                      pan1 = new BFLYT.PAN1();
                      break;
                  default:
                      throw new Exception("Unknown pane type found! " + pane.kind);
              }*/

            ParsePane(ref pan1, pane);
            return pan1;
        }

        private static void ParsePane(ref BFLYT.PAN1 pan1, Pane pane)
        {
            var size = ConvertVector2(pane.size);
            string originX = pane.basePositionType.x;
            string originY = pane.basePositionType.y;
            string parentX = pane.parentRelativePositionType.x;
            string parentY = pane.parentRelativePositionType.y;

            pan1.Name = pane.name;
            pan1.Width = size.X;
            pan1.Height = size.Y;
            pan1.Scale = ConvertVector2(pane.scale);
            pan1.Translate = ConvertVector3(pane.translate);
            pan1.Rotate = ConvertVector3(pane.rotate);
            pan1.Visible = true;

            if (originX == "Center")
                pan1.originX = OriginX.Center;
            if (originX == "Left")
                pan1.originX = OriginX.Left;
            if (originX == "Right")
                pan1.originX = OriginX.Right;

            if (originY == "Top")
                pan1.originY = OriginY.Top;
            if (originY == "Center")
                pan1.originY = OriginY.Center;
            if (originY == "Bottom")
                pan1.originY = OriginY.Bottom;

            if (parentX == "Center")
                pan1.ParentOriginX = OriginX.Center;
            if (parentX == "Left")
                pan1.ParentOriginX = OriginX.Left;
            if (parentX == "Right")
                pan1.ParentOriginX = OriginX.Right;

            if (parentY == "Top")
                pan1.ParentOriginY = OriginY.Top;
            if (parentY == "Center")
                pan1.ParentOriginY = OriginY.Center;
            if (parentY == "Bottom")
                pan1.ParentOriginY = OriginY.Bottom;
        }

        private static void ParsePicturePane(ref BFLYT.PAN1 pan1, PicturePane pane, ref BFLYT.Header header)
        {
            var pic1 = (BFLYT.PIC1)pan1;

            var mat = new BFLYT.Material();
            pic1.MaterialIndex = AddMaterial(ref header, mat);
        }

        private static ushort AddMaterial(ref BFLYT.Header header, BFLYT.Material mat)
        {
            if (header.MaterialList == null)
                header.MaterialList = new BFLYT.MAT1();

            header.MaterialList.Materials.Add(mat);
            return (ushort)header.MaterialList.Materials.IndexOf(mat);
        }

        public static Syroot.Maths.Vector4F ConvertVector4(Vector4 value)
        {
            float X = 0, Y = 0, Z = 0, W = 0;
            float.TryParse(value.x, out X);
            float.TryParse(value.y, out Y);
            float.TryParse(value.z, out Z);
            float.TryParse(value.w, out W);
            return new Syroot.Maths.Vector4F(X, Y, Z, W);
        }

        public static Syroot.Maths.Vector3F ConvertVector3(Vector3 value)
        {
            float X = 0, Y = 0, Z = 0;
            float.TryParse(value.x, out X);
            float.TryParse(value.y, out Y);
            float.TryParse(value.z, out Z);
            return new Syroot.Maths.Vector3F(X, Y, Z);
        }

        public static Syroot.Maths.Vector2F ConvertVector2(Vector2 value)
        {
            float X = 0, Y = 0;
            float.TryParse(value.x, out X);
            float.TryParse(value.y, out Y);
            return new Syroot.Maths.Vector2F(X, Y);
        }

        public static string ToXml(BFLYT.Header header)
        {
            XmlRoot root = new XmlRoot();
            root.head = new Head();
            root.body = new Body();
            root.body.lyt = new Layout();

            var generator = new Generator();
            root.head.generator = generator;
            generator.name = "ST";
            generator.version = "1.0"
                ;
            var create = new Create();
            root.head.create = create;
            create.date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");

            BinaryInfo info = new BinaryInfo();
            info.layout.name = header.LayoutInfo.Name;
            info.version.major = (byte)header.VersionMajor;
            info.version.minor = (byte)header.VersionMinor;
            info.version.micro = (byte)header.VersionMicro;
            info.version.micro2 = (byte)header.VersionMicro2;
            root.head.binaryInfo = info;

            var layout = new Layout();
            root.body.lyt = layout;

            List<Pane> panes = new List<Pane>();
            layout.paneSet = panes;

            layout.paneHierarchy.paneTree = new List<PaneTree>();
            var paneTreeRoot = new PaneTree() { name = header.RootPane.Name };
            LoadPaneTree(header.RootPane, paneTreeRoot);
            layout.paneHierarchy.paneTree.Add(paneTreeRoot);

            var screenSettings = new ScreenSettings();
            screenSettings.layoutSize.x = header.LayoutInfo.Width.ToString();
            screenSettings.layoutSize.y = header.LayoutInfo.Height.ToString();
            if (header.LayoutInfo.DrawFromCenter)
                screenSettings.origin = "Normal";
            else
                screenSettings.origin = "Classic";

            screenSettings.backGround.color.r = 169;
            screenSettings.backGround.color.g = 169;
            screenSettings.backGround.color.b = 169;
            screenSettings.backGround.color.a = 255;

            screenSettings.grid = new Grid();
            screenSettings.grid.color.r = 128;
            screenSettings.grid.color.g = 128;
            screenSettings.grid.color.b = 128;
            screenSettings.grid.color.a = 128;

            layout.screenSetting = screenSettings;

            layout.metrics.totalPaneCount = header.TotalPaneCount();

            int i = 0;
            foreach (var pane in header.GetPanes())
            {
                if (i > 0) //Skip root
                {
                    var basePane = new Pane();
                    if (pane is BFLYT.PIC1)
                    {
                        basePane.kind = "Picture";
                        basePane.Items = new object[1];
                        basePane.Items[0] = new PicturePane();
                    }
                    else if (pane is BFLYT.TXT1)
                        basePane.kind = "TextBox";
                    else if (pane is BFLYT.WND1)
                        basePane.kind = "Window";
                    else if (pane is BFLYT.PRT1)
                        basePane.kind = "Parts";
                    else
                        basePane.kind = "Null";

                    basePane.name = pane.Name;
                    basePane.translate.x = pane.Translate.X.ToString();
                    basePane.translate.y = pane.Translate.Y.ToString();
                    basePane.translate.z = pane.Translate.Z.ToString();
                    basePane.rotate.x = pane.Rotate.X.ToString();
                    basePane.rotate.y = pane.Rotate.Y.ToString();
                    basePane.rotate.z = pane.Rotate.Z.ToString();
                    basePane.scale.x = pane.Scale.X.ToString();
                    basePane.scale.y = pane.Scale.Y.ToString();
                    basePane.size.x = pane.Width.ToString();
                    basePane.size.y = pane.Height.ToString();
                    basePane.influencedAlpha = pane.InfluenceAlpha;
                    basePane.basePositionType.x = $"{pane.originX.ToString()}";
                    basePane.basePositionType.y = $"{pane.originY.ToString()}";
                    basePane.parentRelativePositionType.x = $"{pane.ParentOriginX}";
                    basePane.parentRelativePositionType.y = $"{pane.ParentOriginY}";

                    panes.Add(basePane);

                    if (pane is BFLYT.PIC1)
                    {
                        var pictureBflytPane = pane as BFLYT.PIC1;
                        var pictureBasePane = basePane.Items[0] as PicturePane;

                        pictureBasePane.vtxColRT = ToColorRGBA(pictureBflytPane.ColorTopRight);
                        pictureBasePane.vtxColLT = ToColorRGBA(pictureBflytPane.ColorTopLeft);
                        pictureBasePane.vtxColRB = ToColorRGBA(pictureBflytPane.ColorBottomRight);
                        pictureBasePane.vtxColLB = ToColorRGBA(pictureBflytPane.ColorBottomLeft);

                        foreach (var texCoord in pictureBflytPane.TexCoords)
                        {
                            var uvs = new TexCoord();
                            uvs.texLT.s = texCoord.TopLeft.X;
                            uvs.texLT.t = texCoord.TopLeft.Y;
                            uvs.texRT.s = texCoord.TopRight.X;
                            uvs.texRT.t = texCoord.TopRight.Y;
                            uvs.texLB.s = texCoord.BottomLeft.X;
                            uvs.texLB.t = texCoord.BottomLeft.Y;
                            uvs.texRB.s = texCoord.BottomRight.X;
                            uvs.texRB.t = texCoord.BottomRight.Y;
                            pictureBasePane.texCoord.Add(uvs);
                        }

                        pictureBasePane.material = SetMaterialData(pictureBflytPane.Material);
                        pictureBasePane.materialCtr = SetMaterialCtrData(pictureBflytPane.Material);
                    }
                }

                i++;
            }

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "  ",
            };


            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmldecl = doc.CreateXmlDeclaration("1.0", null, null);
            xmldecl.Encoding = "UTF-8";
            xmldecl.Standalone = "yes";

            var stringWriter = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(XmlRoot));
            XmlWriter output = XmlWriter.Create(stringWriter, settings);
            serializer.Serialize(output, root, ns);
            return stringWriter.ToString();
        }

        private static MaterialCtr SetMaterialCtrData(BFLYT.Material material)
        {
            MaterialCtr mat = new MaterialCtr();
            mat.name = material.Name;
            mat.tevColReg = BlackRGBA;
            mat.tevConstReg = new ColorRGBA[6];
            mat.tevConstReg[0] = WhiteRGBA;
            mat.tevConstReg[1] = WhiteRGBA;
            mat.tevConstReg[2] = WhiteRGBA;
            mat.tevConstReg[3] = WhiteRGBA;
            mat.tevConstReg[4] = WhiteRGBA;
            mat.tevConstReg[5] = WhiteRGBA;
            mat.texMatrix = new List<TexMatrix>();

            if (material.TextureMaps != null)
            {
                for (int i = 0; i < material.TextureMaps.Length; i++)
                {
                    var texMap = new TexMap();
                    texMap.imageName = material.TextureMaps[i].Name;
                    texMap.wrap_s = material.TextureMaps[i].WrapModeU.ToString();
                    texMap.wrap_t = material.TextureMaps[i].WrapModeU.ToString();
                    mat.texMap.Add(texMap);
                }
            }

            mat.blendMode = new BlendMode();
            mat.blendMode.blendOp = "Add";
            mat.blendMode.srcFactor = "SrcAlpha";
            mat.blendMode.dstFactor = "InvSrcAlpha";

            if (material.TextureTransforms != null)
            {
                for (int i = 0; i < material.TextureTransforms.Length; i++)
                {
                    var texMatrix = new TexMatrix();
                    texMatrix.rotate = material.TextureTransforms[i].Rotate;
                    texMatrix.translate.x = material.TextureTransforms[i].Translate.X.ToString();
                    texMatrix.translate.y = material.TextureTransforms[i].Translate.Y.ToString();
                    texMatrix.scale.x = material.TextureTransforms[i].Scale.X.ToString();
                    texMatrix.scale.y = material.TextureTransforms[i].Scale.Y.ToString();
                    mat.texMatrix.Add(texMatrix);
                }
            }
            else
                mat.texMatrix.Add(new TexMatrix());


            if (material.TextureTransforms != null)
            {
                for (int i = 0; i < material.TexCoords.Length; i++)
                {
                    var texCoordGen = new TexCoordGen();
                    texCoordGen.srcParam = material.TexCoords[i].Source.ToString();
                    mat.texCoordGen.Add(texCoordGen);
                }
            }
            else
                mat.texCoordGen.Add(new TexCoordGen());

            if (material.BlendMode != null)
            {
                mat.blendMode.blendOp = BlendOperations[material.BlendMode.BlendOp];
                mat.blendMode.dstFactor = BlendFactors[material.BlendMode.DestFactor];
                mat.blendMode.srcFactor = BlendFactors[material.BlendMode.SourceFactor];
            }

            if (material.AlphaCompare != null)
            {
                mat.alphaCompare.comp = ((CompareMode)material.AlphaCompare.CompareMode).ToString();
                mat.alphaCompare.reference = material.AlphaCompare.Value;
            }

            mat.tevStageNum = 1;

            return mat;
        }

        enum CompareMode
        {
            Never = 0,
            Less = 1,
            LEqual = 2,
            Equal = 3,
            NEqual = 4,
            GEqual = 5,
            Greater = 6,
            Always = 7,
        }

        private static Dictionary<Cafe.BlendMode.GX2LogicOp, string> LogicalOperations = new Dictionary<Cafe.BlendMode.GX2LogicOp, string>()
        {
            {Cafe.BlendMode.GX2LogicOp.Disable,  "None" },
            {Cafe.BlendMode.GX2LogicOp.NoOp,  "NoOp" },
            {Cafe.BlendMode.GX2LogicOp.Clear,  "Clear" },
            {Cafe.BlendMode.GX2LogicOp.Set,  "Set" },
            {Cafe.BlendMode.GX2LogicOp.Copy,  "Copy" },
            {Cafe.BlendMode.GX2LogicOp.InvCopy,  "InvCopy" },
            {Cafe.BlendMode.GX2LogicOp.Inv,  "Inv" },
            {Cafe.BlendMode.GX2LogicOp.And,  "And" },
            {Cafe.BlendMode.GX2LogicOp.Nand,  "Nand" },
            {Cafe.BlendMode.GX2LogicOp.Or,  "Or" },
            {Cafe.BlendMode.GX2LogicOp.Xor,  "Xor" },
            {Cafe.BlendMode.GX2LogicOp.Equiv,  "Equiv" },
            {Cafe.BlendMode.GX2LogicOp.RevAnd,  "RevAnd" },
            {Cafe.BlendMode.GX2LogicOp.RevOr,  "RevOr" },
            {Cafe.BlendMode.GX2LogicOp.InvOr,  "InvOr" },
        };

        private static Dictionary<Cafe.BlendMode.GX2BlendOp, string> BlendOperations = new Dictionary<Cafe.BlendMode.GX2BlendOp, string>()
        {
            {Cafe.BlendMode.GX2BlendOp.Disable,  "None" },
            {Cafe.BlendMode.GX2BlendOp.Add,  "Add" },
            {Cafe.BlendMode.GX2BlendOp.Subtract,  "Subtract" },
            {Cafe.BlendMode.GX2BlendOp.ReverseSubtract,  "ReverseSubtract" },
            {Cafe.BlendMode.GX2BlendOp.SelectMin,  "SelectMin" },
            {Cafe.BlendMode.GX2BlendOp.SelectMax,  "SelectMax" },
        };

        private static Dictionary<Cafe.BlendMode.GX2BlendFactor, string> BlendFactors = new Dictionary<Cafe.BlendMode.GX2BlendFactor, string>()
        {
            {Cafe.BlendMode.GX2BlendFactor.Factor0,  "V0" },
            {Cafe.BlendMode.GX2BlendFactor.Factor1,  "V1_0" },
            {Cafe.BlendMode.GX2BlendFactor.DestColor,  "DstClr" },
            {Cafe.BlendMode.GX2BlendFactor.DestInvColor,  "InvDstClr" },
            {Cafe.BlendMode.GX2BlendFactor.SourceAlpha,  "SrcAlpha" },
            {Cafe.BlendMode.GX2BlendFactor.SourceInvAlpha,  "InvSrcAlpha" },
            {Cafe.BlendMode.GX2BlendFactor.DestAlpha,  "DstAlpha" },
            {Cafe.BlendMode.GX2BlendFactor.DestInvAlpha,  "InvDstAlpha" },
            {Cafe.BlendMode.GX2BlendFactor.SourceColor,  "SrcClr" },
            {Cafe.BlendMode.GX2BlendFactor.SourceInvColor,  "InvSrcClr" },

        };

        private static Material SetMaterialData(BFLYT.Material material)
        {
            Material mat = new Material();
            mat.name = material.Name;
            mat.blackColor = ToColorRGB(material.BlackColor);
            mat.whiteColor = ToColorRGB(material.WhiteColor);
            if (material.IndParameter != null)
            {
                mat.hsbAdjustment.hOffset = material.IndParameter.Rotation;
                mat.hsbAdjustment.bScale = material.IndParameter.ScaleX;
                mat.hsbAdjustment.sScale = material.IndParameter.ScaleY;
            }

            if (material.TextureMaps != null)
            {
                for (int i = 0; i < material.TextureMaps.Length; i++)
                {
                    var texMap = new TexMap();
                    texMap.imageName = material.TextureMaps[i].Name;
                    texMap.wrap_s = material.TextureMaps[i].WrapModeU.ToString();
                    texMap.wrap_t = material.TextureMaps[i].WrapModeU.ToString();
                    mat.texMap.Add(texMap);
                }
            }

            if (material.TextureTransforms != null)
            {
                for (int i = 0; i < material.TextureTransforms.Length; i++)
                {
                    var texMatrix = new TexMatrix();
                    texMatrix.rotate = material.TextureTransforms[i].Rotate;
                    texMatrix.translate.x = material.TextureTransforms[i].Translate.X.ToString();
                    texMatrix.translate.y = material.TextureTransforms[i].Translate.Y.ToString();
                    texMatrix.scale.x = material.TextureTransforms[i].Scale.X.ToString();
                    texMatrix.scale.y = material.TextureTransforms[i].Scale.Y.ToString();
                    mat.texMatrix.Add(texMatrix);
                }
            }
            else
                mat.texMatrix.Add(new TexMatrix());

            if (material.TextureTransforms != null)
            {
                for (int i = 0; i < material.TexCoords.Length; i++)
                {
                    var texCoordGen = new TexCoordGen();
                    texCoordGen.srcParam = material.TexCoords[i].Source.ToString();
                    mat.texCoordGen.Add(texCoordGen);
                }
            }
            else
                mat.texCoordGen.Add(new TexCoordGen());

            return mat;
        }

        private static ColorRGB ToColorRGB(Toolbox.Library.STColor8 color)
        {
            ColorRGB col = new ColorRGB();
            col.r = color.R;
            col.g = color.G;
            col.b = color.B;
            return col;
        }

        private static ColorRGBA ToColorRGBA(Toolbox.Library.STColor8 color)
        {
            ColorRGBA col = new ColorRGBA();
            col.r = color.R;
            col.g = color.G;
            col.b = color.B;
            col.a = color.A;
            return col;
        }

        private static void LoadPaneTree(BasePane parentPane, PaneTree paneEntry)
        {
            if (parentPane.Childern != null && parentPane.Childern.Count > 0)
                paneEntry.paneTree = new List<PaneTree>();
            foreach (var child in parentPane.Childern)
            {
                var paneTreeChild = new PaneTree() { name = child.Name };
                paneEntry.paneTree.Add(paneTreeChild);
                LoadPaneTree(child, paneTreeChild);
            }
        }

        [XmlRootAttribute("nw4f_layout")]
        public class XmlRoot
        {
            [XmlAttribute]
            public string version = "1.5.16";

            public Head head = new Head();
            public Body body = new Body();
        }

        public class BinaryInfo
        {
            public BinaryLayout layout = new BinaryLayout();
            public BinaryVersion version = new BinaryVersion();
        }

        public class BinaryLayout
        {
            [XmlAttribute]
            public string name = "";
        }

        public class BinaryVersion
        {
            [XmlAttribute]
            public byte major;

            [XmlAttribute]
            public byte minor;

            [XmlAttribute]
            public byte micro;

            [XmlAttribute]
            public byte micro2;
        }

        public class Head
        {
            public Create create = new Create();
            public Title title = new Title();
            public Comment comment = new Comment();
            public Generator generator = new Generator();
            public BinaryInfo binaryInfo = new BinaryInfo();
        }

        public class Comment
        {

        }

        public class Title
        {

        }

        public class Create
        {
            [XmlAttribute]
            public string user = "";

            [XmlAttribute]
            public string host = "";

            [XmlAttribute]
            public string date = "";

            [XmlAttribute]
            public string source = "";
        }

        public class Generator
        {
            [XmlAttribute]
            public string name = "";

            [XmlAttribute]
            public string version = "";
        }

        public class Body
        {
            public Layout lyt = new Layout();
        }

        public class Layout
        {
            [XmlArrayItem("pane")]
            public List<Pane> paneSet = new List<Pane>();
            public PaneHiearchy paneHierarchy = new PaneHiearchy();
            public Groups groupSet = new Groups();
            public ScreenSettings screenSetting = new ScreenSettings();
            public FontFile[] fontFile = new FontFile[0];
            public TextureFile[] textureFilenew = new TextureFile[0];
            public Metrics metrics = new Metrics();
            public UserData userData = new UserData();
        }

        public class PicturePane
        {
            [XmlAttribute]
            public bool detailSetting = true;

            public ColorRGBA vtxColLT = new ColorRGBA();
            public ColorRGBA vtxColRT = new ColorRGBA();
            public ColorRGBA vtxColLB = new ColorRGBA();
            public ColorRGBA vtxColRB = new ColorRGBA();
            public List<TexCoord> texCoord = new List<TexCoord>();
            public Material material = new Material();
            public MaterialCtr materialCtr = new MaterialCtr();
        }

        public class TexCoord
        {
            public TexCoordST texLT = new TexCoordST();
            public TexCoordST texRT = new TexCoordST();
            public TexCoordST texLB = new TexCoordST();
            public TexCoordST texRB = new TexCoordST();
        }

        public class Material
        {
            [XmlAttribute]
            public string name = "";

            public ColorRGB blackColor = new ColorRGB();
            public ColorRGB whiteColor = new ColorRGB();

            public HsbAdjustment hsbAdjustment = new HsbAdjustment();
            public List<TexMap> texMap = new List<TexMap>();
            public List<TexMatrix> texMatrix = new List<TexMatrix>();
            public List<TexCoordGen> texCoordGen = new List<TexCoordGen>();
            public TexStage textureStage = new TexStage();
        }

        public class MaterialCtr
        {
            [XmlAttribute]
            public string name = "";

            [XmlAttribute]
            public int tevStageNum = 0;

            [XmlAttribute]
            public bool useDefaultBlendSettings = true;

            [XmlAttribute]
            public bool useDefaultAlphaTestSettings = true;

            public ColorRGBA tevColReg = new ColorRGBA();

            [XmlArrayItem("tevConstReg")]
            public ColorRGBA[] tevConstReg;

            public List<TexMap> texMap = new List<TexMap>();
            public List<TexMatrix> texMatrix = new List<TexMatrix>();
            public List<TexCoordGen> texCoordGen = new List<TexCoordGen>();

            public AlphaCompare alphaCompare = new AlphaCompare();
            public BlendMode blendMode = new BlendMode();
            public BlendModeAlpha blendModeAlpha = new BlendModeAlpha();
        }

        public class AlphaCompare
        {
            [XmlAttribute]
            public string comp = "Always";

            [XmlAttribute("ref")]
            public float reference = 0;
        }

        public class BlendMode
        {
            [XmlAttribute]
            public string type = "Blend";

            [XmlAttribute]
            public string srcFactor = "SrcAlpha";

            [XmlAttribute]
            public string dstFactor = "InvSrcAlpha";

            [XmlAttribute]
            public string blendOp = "Add";
        }

        public class BlendModeAlpha
        {
            [XmlAttribute]
            public string type = "None";
        }

        public class HsbAdjustment
        {
            [XmlAttribute]
            public float hOffset = 0;

            [XmlAttribute]
            public float sScale = 1;

            [XmlAttribute]
            public float bScale = 1;
        }

        public class TexMap
        {
            [XmlAttribute]
            public string imageName = "";

            [XmlAttribute]
            public string wrap_s = "Clamp";

            [XmlAttribute]
            public string wrap_t = "Clamp";
        }

        public class TexCoordGen
        {
            [XmlAttribute]
            public string srcParam = "Tex0";

            public Vector2 projectionScale = new Vector2() { x = "1", y = "1" };
            public Vector2 projectionTrans = new Vector2() { x = "0", y = "0" };
        }

        public class TexStage
        {
            [XmlAttribute]
            public string texMap = "0";
        }

        public class TexMatrix
        {
            [XmlAttribute]
            public float rotate = 0;

            public Vector2 scale = new Vector2();
            public Vector2 translate = new Vector2();
        }

        public class TextureFile
        {
            [XmlAttribute]
            public string imagePath = "";

            [XmlAttribute]
            public string format = "";
        }


        public class FontFile
        {
            [XmlAttribute]
            public string path = "";

            [XmlAttribute]
            public string name = "";
        }

        public class Pane
        {
            [XmlAttribute]
            public string kind = "";

            [XmlAttribute]
            public string name = "";

            [XmlAttribute]
            public bool influencedAlpha;

            public Comment comment = new Comment();
            public Vector2 basePositionType = new Vector2();
            public Vector2 parentRelativePositionType = new Vector2();
            public Vector3 translate = new Vector3();
            public Vector3 rotate = new Vector3();
            public Vector2 scale = new Vector2();
            public Vector2 size = new Vector2();

            private object[] items;

            [XmlElement("picture", typeof(PicturePane))]
            public object[] Items
            {
                get { return items; }
                set { items = value; }
            }

            public UserDataPane userData = new UserDataPane();
        }

        public class UserDataPane
        {
            [XmlElement("string")]
            public XmlString UserString = new XmlString();
        }

        public class XmlString
        {
            [XmlAttribute]
            public string name = "";
        }

        public class TextboxPane : Pane
        {

        }

        public class PaneHiearchy
        {
            public List<PaneTree> paneTree = new List<PaneTree>();
        }

        public class PaneTree
        {
            [XmlAttribute]
            public string name = "";

            [XmlArrayItem("paneTree")]
            public List<PaneTree> paneTree;
        }

        public class Groups
        {

        }

        public class ScreenSettings
        {
            [XmlAttribute]
            public string origin = "Normal";

            public Vector2 layoutSize = new Vector2();
            public Background backGround = new Background();
            public Grid grid = new Grid();
        }

        public class Background
        {
            public ColorRGBA color = new ColorRGBA();
        }

        public class Grid
        {
            [XmlAttribute]
            public float thickLineInterval = 40;

            [XmlAttribute]
            public float thinDivisionNum = 4;

            [XmlAttribute]
            public bool visible = true;

            [XmlAttribute]
            public string moveMethod = "None";

            public ColorRGBA color = new ColorRGBA();
        }

        public class Metrics
        {
            public int totalPaneCount = 0;
            public int totalPixelCount = 0;
        }

        public class UserData
        {

        }

        public class TexCoordST
        {
            [XmlAttribute]
            public float s = 0;

            [XmlAttribute]
            public float t = 0;
        }

        public class Vector2
        {
            [XmlAttribute]
            public string x = "";

            [XmlAttribute]
            public string y = "";
        }

        public class Vector3
        {
            [XmlAttribute]
            public string x = "";

            [XmlAttribute]
            public string y = "";

            [XmlAttribute]
            public string z = "";
        }

        public class Vector4
        {
            [XmlAttribute]
            public string x = "";

            [XmlAttribute]
            public string y = "";

            [XmlAttribute]
            public string z = "";

            [XmlAttribute]
            public string w = "";
        }

        public class ColorRGB
        {
            [XmlAttribute]
            public int r = 0;

            [XmlAttribute]
            public int g = 0;

            [XmlAttribute]
            public int b = 0;
        }

        public class ColorRGBA
        {
            [XmlAttribute]
            public int r = 0;

            [XmlAttribute]
            public int g = 0;

            [XmlAttribute]
            public int b = 0;

            [XmlAttribute]
            public int a = 0;
        }
    }
}