using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Toolbox.Library
{
    public class XmlDoc
    {
        public static void AddAttribute(XmlDocument doc, string name, string[] values, XmlNode node)
        {
            XmlAttribute att = doc.CreateAttribute(name);
            foreach (string value in values) {
                att.Value = value;
                node.Attributes.Append(att);
            }
        }

        static public string Beautify(XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }

        public static string DocumentToString(XmlDocument doc)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;

            using (var stringWriter = new StringWriter())
            using (XmlWriter xmlTextWriter = XmlWriter.Create(stringWriter, settings))
            {
                doc.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }

        static string RemoveInvalidXmlChars(string text)
        {
            var validXmlChars = text.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();
            return new string(validXmlChars);
        }

        public static void AddAttribute(XmlDocument doc, string name, string value, XmlNode node)
        {
            name = RemoveInvalidXmlChars(name);
            value = RemoveInvalidXmlChars(value);

            XmlAttribute att = doc.CreateAttribute(name);
            att.Value = value;
            node.Attributes.Append(att);
        }
    }
}
