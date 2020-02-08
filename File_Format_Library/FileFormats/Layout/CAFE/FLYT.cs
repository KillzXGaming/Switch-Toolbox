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
        public static BFLYT.Header FromXml(string text)
        {
            BFLYT.Header header = new BFLYT.Header();
            return header;
        }

        public static string ToXml(BFLYT.Header header)
        {

            return "";
        }
    }
}