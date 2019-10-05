using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FirstPlugin
{
    public class ESET
    {
        public static string ToXml(PTCL.Header header)
        {
            var root = new EmitterSetData();

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
            XmlSerializer serializer = new XmlSerializer(typeof(EmitterSetData));
            XmlWriter output = XmlWriter.Create(stringWriter, settings);
            serializer.Serialize(output, root, ns);
            return stringWriter.ToString();
        }

        [XmlRootAttribute("nw4f_layout")]
        public class EmitterSetData
        {
            [XmlAttribute]
            public string version = "1.0.0.8";

            [XmlAttribute]
            public bool UseXmlDocSerializer = true;

            public bool EnableConvert = true;

            public EmitterSetBasic EmitterSetBasicData = new EmitterSetBasic();
            public EmitterSetUserData EmitterSetUserData = new EmitterSetUserData();
            public EmitterList EmitterList = new EmitterList();
        }

        public class EmitterSetBasic
        {
            public Comment Comment;
            public int LabelColor = 0;
        }

        public class EmitterSetUserData
        {
     
        }

        public class Comment
        {

        }

        public class EmitterList
        {
            public List<EmitterData> EmitterData = new List<EmitterData>();
        }

        public class EmitterData
        {
            public bool EnableConvert = true;
            public string Name;

            public EmitterColorData EmitterColorData;
        }

        public class EmitterColorData
        {
            public ParticleColor ParticleColor;
            public EmitterColor EmitterColor;
        }

        public class ParticleColor
        {

        }

        public class EmitterColor
        {
            public int Color0BehaviorType = 0;
            public int Color1BehaviorType = 0;
            public int Alpha0BehaviorType = 0;
            public int Alpha1BehaviorType = 0;
        }
    }
}
