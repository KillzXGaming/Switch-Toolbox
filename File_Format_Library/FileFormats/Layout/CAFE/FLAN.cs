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
    public class FLAN
    {
        public static BFLAN.Header FromXml(string text)
        {
            BFLAN.Header header = new BFLAN.Header();

            XmlSerializer serializer = new XmlSerializer(typeof(XmlRoot));
            XmlRoot flyt = (XmlRoot)serializer.Deserialize(new StringReader(text));
          
            return header;
        }

        public static string ToXml(BFLAN.Header header)
        {
            XmlRoot root = new XmlRoot();
            root.head = new Head();
            root.body = new Body();

            var generator = new Generator();
            root.head.generator = generator;
            generator.name = "ST";
            generator.version = "1.0"
                ;
            var create = new Create();
            root.head.create = create;
            create.date = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss");

            BinaryInfo info = new BinaryInfo();
            info.layout.name = header.AnimationTag.Name;
            info.version.major = (byte)header.VersionMajor;
            info.version.minor = (byte)header.VersionMinor;
            info.version.micro = (byte)header.VersionMicro;
            info.version.micro2 = (byte)header.VersionMicro2;
            root.head.binaryInfo = info;

            AnimTag tag = new AnimTag();
            AnimInfo animInfo = new AnimInfo();

            if (header.AnimationInfo.Loop)
                tag.animLoop = AnimLoopType.Loop;

            tag.descendingBind = header.AnimationTag.ChildBinding;
            tag.name = header.AnimationTag.Name;
            tag.fileName = header.AnimationTag.Name;
            tag.startFrame = header.AnimationTag.StartFrame;
            tag.endFrame = header.AnimationTag.EndFrame;
            tag.group = new Group[header.AnimationTag.Groups.Count];
            for (int i =0; i < header.AnimationTag.Groups.Count; i++) {
                tag.group[i] = new Group();
                tag.group[i].name = header.AnimationTag.Groups[i];
            }

            root.body.animTag[0] = tag;
            root.body.lan[0] = animInfo;

            var bflanInfo = header.AnimationInfo;
            var animContent = new AnimContent();
            animInfo.animContent[0] = animContent;
            animInfo.startFrame = bflanInfo.FrameSize;



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
            [XmlArrayItem]
            public AnimTag[] animTag = new AnimTag[1];

            [XmlArrayItem]
            public AnimInfo[] lan = new AnimInfo[1];
        }

        public class AnimTag
        {
            [XmlAttribute]
            public string name = "";

            [XmlAttribute]
            public int startFrame = 0;

            [XmlAttribute]
            public int endFrame = 0;

            public AnimLoopType animLoop = AnimLoopType.OneTime;

            [XmlAttribute]
            public string fileName = "";

            [XmlAttribute]
            public bool descendingBind = false;

            [XmlArrayItem]
            public Group[] group;
        }

        public enum AnimLoopType
        {
            Loop,
            OneTime,
        }

        public class Group
        {
            [XmlAttribute]
            public string name = "";
        }

        public class AnimInfo
        {
            [XmlAttribute]
            public AnimType animType;

            [XmlAttribute]
            public int startFrame = 0;

            [XmlAttribute]
            public int endFrame = 0;

            [XmlAttribute]
            public int convertStartFrame = 0;

            [XmlAttribute]
            public int convertEndFrame = 0;

            [XmlArrayItem]
            public AnimContent[] animContent;
        }

        public enum AnimType
        {
            PaneSRT,
            VertexColor,
            MaterialColor,
            TextureSRT,
        }

        public class AnimContent
        {
            [XmlAttribute]
            public string name = "";
        }
    }
}