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
            return header;
        }

        public static string ToXml(BFLAN.Header header)
        {
            return "";
        }
    }
}