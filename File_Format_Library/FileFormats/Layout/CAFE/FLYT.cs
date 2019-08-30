using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LayoutBXLYT
{
    public class FLYT
    {
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
                    basePane.name = pane.Name;
                    if (pane is BFLYT.PIC1)
                        basePane.kind = "Picture";
                    else if (pane is BFLYT.TXT1)
                        basePane.kind = "TextBox";
                    else if (pane is BFLYT.WND1)
                        basePane.kind = "Window";
                    else if (pane is BFLYT.PRT1)
                        basePane.kind = "Parts";
                    else
                        basePane.kind = "Null";

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

        public class Head
        {
            public Create create = new Create();
            public Title title = new Title();
            public Comment comment = new Comment();
            public Generator generator = new Generator();
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

        public class Body {
            public Layout lyt = new Layout();
        }

        public class Layout {
            [XmlArrayItem("pane")]
            public List<Pane> paneSet = new List<Pane>();
            public PaneHiearchy paneHierarchy = new PaneHiearchy();
            public Groups groupSet = new Groups();
            public ScreenSettings screenSetting = new ScreenSettings();
            public FontFile fontFile;
            public Metrics metrics = new Metrics();
            public UserData userData = new UserData();
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
