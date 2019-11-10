using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace Toolbox.Library
{
    public class ColladaWriter : IDisposable
    {
        private XmlTextWriter Writer;
        private DAE.ExportSettings Settings;
        private DAE.Version Version;

        private Dictionary<string, int> AttriubteIdList = new Dictionary<string, int>();

        public ColladaWriter(string fileName, DAE.ExportSettings settings)
        {
            Settings = settings;
            Version = settings.FileVersion;
            Writer = new XmlTextWriter(fileName, Encoding.UTF8)
            {
                Formatting = Formatting.Indented,
                Indentation = 2,
            };
        }

        public void WriteDAEHeader()
        {
            Writer.WriteStartDocument();
            Writer.WriteStartElement("COLLADA");
            Writer.WriteAttributeString("xmlns", "http://www.collada.org/2005/11/COLLADASchema");
            Writer.WriteAttributeString("version", $"{Version.Major}.{Version.Minor}.{Version.Micro}");
        }

        public void WriteAsset()
        {
            Writer.WriteStartElement("asset");
            Writer.WriteEndElement();
        }

        public void WriteFileSettings()
        {
            Writer.WriteStartElement("asset");
            {
                Writer.WriteStartElement("contributor");
                Writer.WriteElementString("authoring_tool", System.Windows.Forms.Application.ProductName);
                Writer.WriteEndElement();

                Writer.WriteStartElement("created");
                Writer.WriteString(DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture) + "Z");
                Writer.WriteEndElement();

                Writer.WriteStartElement("modified");
                Writer.WriteString(DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture) + "Z");
                Writer.WriteEndElement();

                Writer.WriteStartElement("unit");
                Writer.WriteAttributeString("meter", "0.01");
                Writer.WriteAttributeString("name", "centimeter");
                Writer.WriteEndElement();

                Writer.WriteStartElement("up_axis");
                Writer.WriteString("Y_UP");
                Writer.WriteEndElement();
            }
            Writer.WriteEndElement();
        }

        public void WriteLibraryImages(string[] textureNames)
        {
            Writer.WriteStartElement("library_images");
            for (int i = 0; i < textureNames?.Length; i++)
            {
                Writer.WriteStartElement("image");
                Writer.WriteAttributeString("id", textureNames[i]);
                Writer.WriteStartElement("init_from");
                Writer.WriteString($"{Settings.ImageFolder}{textureNames[i]}.{Settings.ImageExtension}");
                Writer.WriteEndElement();
                Writer.WriteEndElement();
            }
            Writer.WriteEndElement();
        }

        public void Dispose()
        {
            Writer.Close();
        }
    }
}
