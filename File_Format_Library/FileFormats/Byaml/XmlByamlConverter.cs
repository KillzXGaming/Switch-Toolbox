using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Syroot.BinaryData;
using Syroot.Maths;
using ByamlExt.Byaml;
using System.Globalization;

namespace FirstPlugin
{
    //From https://gitlab.com/Syroot/NintenTools.Byaml/-/blob/master/src/Syroot.NintenTools.Byaml/XmlConverter.cs
    //This allows xml to with with yamlconv and it looks better using attributes for properties
    //Todo this needs to support reference nodes
    public class XmlByamlConverter
    {
        private static readonly XNamespace _yamlconvNs = "yamlconv";

        public static string ToXML(BymlFileData data)
        {
            XDocument doc = ToXDocument(data.RootNode, data);
            using (StringWriter writer = new StringWriter())
            {
                doc.Save(writer);
                return writer.ToString();
            }
        }

        public static BymlFileData FromXML(string text) {
            return FromXDocument(XDocument.Parse(text));
        }

        internal static BymlFileData FromXDocument(XDocument xDocument)
        {
            if (xDocument == null)
                throw new ArgumentNullException(nameof(xDocument));
            if (xDocument.Root?.Name != "yaml")
                throw new ArgumentException("Incompatible XML data. A \"yaml\" root element is required.");

            // Retrieve information from root node.
            BymlFileData data = new BymlFileData();
            switch (xDocument.Root.Attribute(_yamlconvNs + "endianness")?.Value)
            {
                case "big": data.byteOrder = ByteOrder.BigEndian; break;
                case "little": data.byteOrder = ByteOrder.LittleEndian; break;
                default: data.byteOrder = ByteOrder.LittleEndian; break;
            }
            data.Version = UInt16.TryParse(xDocument.Root.Attribute(_yamlconvNs + "version")?.Value,
                out ushort version) ? version : (ushort)1;

            // Generate the root node.
            data.RootNode = ReadNode(xDocument.Root);
            return data;
        }

        private static dynamic ReadNode(XElement node)
        {
            dynamic convertValue(string value)
            {
                if (value == "null")
                    return null;
                if (value == string.Empty)
                    return "";
                else if (value == "true")
                    return true;
                else if (value == "false")
                    return false;
                else if (value.EndsWith("f"))
                    return Single.Parse(value.Substring(0, value.Length - "f".Length), CultureInfo.InvariantCulture);
                else if (value.EndsWith("u"))
                    return UInt32.Parse(value.Substring(0, value.Length - "u".Length), CultureInfo.InvariantCulture);
                else if (value.EndsWith("i64"))
                    return Int64.Parse(value.Substring(0, value.Length - "i64".Length), CultureInfo.InvariantCulture);
                else if (value.EndsWith("u64"))
                    return UInt64.Parse(value.Substring(0, value.Length - "u64".Length), CultureInfo.InvariantCulture);
                else if (value.EndsWith("d"))
                    return Double.Parse(value.Substring(0, value.Length - "d".Length), CultureInfo.InvariantCulture);
                else
                    return Int32.Parse(value, CultureInfo.InvariantCulture);
            }

            string convertString() => node.Value.ToString();

            List<ByamlPathPoint> convertPath()
            {
                List<ByamlPathPoint> path = new List<ByamlPathPoint>();
                foreach (XElement pathPoint in node.Elements("point"))
                    path.Add(convertPathPoint(pathPoint));
                return path;
            }

            ByamlPathPoint convertPathPoint(XElement pathPoint)
            {
                return new ByamlPathPoint
                {
                    Position = new Vector3F(
                        convertValue(pathPoint.Attribute("x")?.Value ?? "0f"),
                        convertValue(pathPoint.Attribute("y")?.Value ?? "0f"),
                        convertValue(pathPoint.Attribute("z")?.Value ?? "0f")),
                    Normal = new Vector3F(
                        convertValue(pathPoint.Attribute("nx")?.Value ?? "0f"),
                        convertValue(pathPoint.Attribute("ny")?.Value ?? "0f"),
                        convertValue(pathPoint.Attribute("nz")?.Value ?? "0f")),
                    Unknown = convertValue(pathPoint.Attribute("val")?.Value ?? "0")
                };
            }

            Dictionary<string, dynamic> convertDictionary()
            {
                Dictionary<string, dynamic> dictionary = new Dictionary<string, dynamic>();
                foreach (XElement element in node.Elements())
                    dictionary.Add(XmlConvert.DecodeName(element.Name.ToString()), ReadNode(element));
                // Only keep non-namespaced attributes for now to filter out yamlconv and xml(ns) ones.
                foreach (XAttribute attribute in node.Attributes().Where(x => x.Name.Namespace == XNamespace.None))
                    dictionary.Add(XmlConvert.DecodeName(attribute.Name.ToString()), convertValue(attribute.Value));
                return dictionary;
            }

            List<dynamic> convertArray()
            {
                List<dynamic> array = new List<dynamic>();
                foreach (XElement element in node.Elements("value"))
                    array.Add(ReadNode(element));
                return array;
            }


            // Detecting the special "type" attribute like this is unsafe as it could also be a dictionary with a "type"
            // key. Yamlconv should have namespaced its attribute to safely identify it.
            switch (node.Attributes("type").SingleOrDefault()?.Value)
            {
                // TODO: Add null support. Can null be set for value types?
                // TODO: Add reference support. Use Element with encoded XPath.
                case null when node.HasAttributes || node.HasElements: return convertDictionary();
                case null: return convertValue(node.Value);
                case "array": return convertArray();
                case "path": return convertPath();
                case "string": return convertString();
                default: throw new ByamlException("Unknown XML contents.");
            }
        }

        internal static XDocument ToXDocument(dynamic root, BymlFileData data)
        {
            XElement rootNode = SaveNode("yaml", root, false);

            List<XAttribute> attribs = rootNode.Attributes().ToList();
            attribs.Insert(0, new XAttribute(XNamespace.Xmlns + "yamlconv", "yamlconv"));
            attribs.Insert(1, new XAttribute(_yamlconvNs + "endianness",
                data.byteOrder == ByteOrder.LittleEndian ? "little" : "big"));
            attribs.Insert(2, new XAttribute(_yamlconvNs + "offsetCount", data.SupportPaths ? 4 : 3));
            attribs.Insert(3, new XAttribute(_yamlconvNs + "byamlVersion", data.Version));
            rootNode.Attributes().Remove();
            rootNode.Add(attribs);

            return new XDocument(new XDeclaration("1.0", "utf-8", null), rootNode);
        }

        private static XObject SaveNode(string name, dynamic node, bool isArrayElement)
        {
            XObject convertValue(object value)
             => isArrayElement ? new XElement(name, value) : (XObject)new XAttribute(name, value);

            XElement convertString(string stringNode)
                => new XElement(name, new XAttribute("type", "string"), stringNode);

            XElement convertPathPoint(ByamlPathPoint pathPointNode)
            {
                return new XElement("point",
                    new XAttribute("x", getSingleString(pathPointNode.Position.X)),
                    new XAttribute("y", getSingleString(pathPointNode.Position.Y)),
                    new XAttribute("z", getSingleString(pathPointNode.Position.Z)),
                    new XAttribute("nx", getSingleString(pathPointNode.Normal.X)),
                    new XAttribute("ny", getSingleString(pathPointNode.Normal.Y)),
                    new XAttribute("nz", getSingleString(pathPointNode.Normal.Z)),
                    new XAttribute("val", getUInt32String(pathPointNode.Unknown)));
            }

            XObject convertPath(List<ByamlPathPoint> pathNode)
            {
                XElement xElement = new XElement(name, new XAttribute("type", "path"));
                foreach (dynamic element in pathNode)
                    xElement.Add(SaveNode("point", element, true));
                return xElement;
            }

            XElement convertDictionary(IDictionary<string, dynamic> dictionaryNode)
            {
                XElement xElement = new XElement(name);
                foreach (KeyValuePair<string, dynamic> element in dictionaryNode.OrderBy(x => x.Key, StringComparer.Ordinal))
                    xElement.Add(SaveNode(element.Key, element.Value, false));
                return xElement;
            }

            XElement convertArray(IEnumerable<dynamic> arrayNode)
            {
                XElement xElement = new XElement(name, new XAttribute("type", "array"));
                foreach (dynamic element in arrayNode)
                    xElement.Add(SaveNode("value", element, true));
                return xElement;
            }

            string getBooleanString(Boolean value) => value ? "true" : "false";
            string getInt32String(Int32 value) => value.ToString(CultureInfo.InvariantCulture);
            string getSingleString(Single value) => value.ToString(CultureInfo.InvariantCulture) + "f";
            string getUInt32String(UInt32 value) => value.ToString(CultureInfo.InvariantCulture) + "u";
            string getInt64String(Int64 value) => value.ToString(CultureInfo.InvariantCulture) + "i64";
            string getUInt64String(UInt64 value) => value.ToString(CultureInfo.InvariantCulture) + "u64";
            string getDoubleString(Double value) => value.ToString(CultureInfo.InvariantCulture) + "d";

            if (node == null) return convertString("null");

            name = XmlConvert.EncodeName(name);
            if (node is bool) return convertValue(getBooleanString((bool)node));
            else if (node is int) return convertValue(getInt32String((int)node));
            else if (node is uint) return convertValue(getUInt32String((uint)node));
            else if (node is float) return convertValue(getSingleString((float)node));
            else if (node is UInt64) return convertValue(getUInt64String((UInt64)node));
            else if (node is Int64) return convertValue(getInt64String((Int64)node));
            else if (node is Double) return convertValue(getDoubleString((Double)node));
            else if (node is string) return convertString(node);
            else if (node is ByamlPathPoint) return convertPathPoint((ByamlPathPoint)node);
            else if (node is List<ByamlPathPoint>) return convertPath((List<ByamlPathPoint>)node);
            else if (node is IDictionary<string, dynamic>) return convertDictionary(node);
            else if (node is IEnumerable<dynamic>) return convertArray(node);
            else throw new Exception("Unsupported node type! " + node.GetType());
        }
    }
}
