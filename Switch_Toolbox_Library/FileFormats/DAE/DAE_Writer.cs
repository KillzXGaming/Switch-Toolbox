using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Toolbox.Library
{
    public class ColladaWriter : IDisposable
    {
        private XmlTextWriter Writer;
        private DAE.ExportSettings Settings;
        private DAE.Version Version;

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

        public void WriteHeader()
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

        public static void WriteSectionAsset()
        {

        }

        public void Dispose()
        {
            Writer.Close();
        }
    }
}
