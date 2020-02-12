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
        public static Header FromXml(string text)
        {
            Header header = new Header();
            return header;
        }

        public static string ToXml(Header header)
        {
            return "";
        }
    }
}