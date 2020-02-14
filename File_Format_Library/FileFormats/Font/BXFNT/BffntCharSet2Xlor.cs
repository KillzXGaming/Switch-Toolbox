using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FirstPlugin
{
    public class BffntCharSet2Xlor
    {
        public static string ToXlor(BXFNT bffnt)
        {
            StringBuilder sb = new StringBuilder();
            using (var texWriter = new StringWriter(sb))
            {
                texWriter.WriteLine("<?xml version =\"1.0\" encoding=\"UTF-8\" ?>");
                texWriter.WriteLine("<!DOCTYPE letter-order SYSTEM \"letter-order.dtd\">");
                texWriter.WriteLine("\r");
                texWriter.WriteLine("<letter-order version=\"1.0\">");
                texWriter.WriteLine("	<head>");
                texWriter.WriteLine($"		<create user=\"\" date=\"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}T{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}\" />\r");
                texWriter.WriteLine($"		<title>{bffnt.FileName}</title>");
                texWriter.WriteLine("		<comment></comment>");
                texWriter.WriteLine("	</head>");
                texWriter.WriteLine("	<body>");
                texWriter.WriteLine("	<area width=\"16\" />");
                texWriter.WriteLine("	<order>");

                int row_pos = 0;
                foreach (var item in bffnt.bffnt.FontSection.CodeMapDictionary)
                {
                    ushort CharCode = (ushort)item.Key;
                    if (row_pos != 16)
                    {
                        if (CharCode != 0x20)
                            texWriter.Write($" &#x{CharCode.ToString("X4")};");

                        else
                            texWriter.Write("<sp/>    ");

                        row_pos++;
                    }
                    if (row_pos == 16)
                    {
                        texWriter.Write("\n");
                        row_pos = 0;
                    }
                }

                texWriter.WriteLine("\n");

            /*    foreach (var item in bffnt.bffnt.FontSection.CodeMapDictionary)
                {
                    if (row_pos != 16)
                    {
                        texWriter.Write(item.Key);
                        row_pos++;
                    }
                    if (row_pos == 16)
                    {
                        texWriter.Write("\n");
                        row_pos = 0;
                    }
                }*/


                texWriter.WriteLine("\n");
                texWriter.WriteLine("		</order>");
                texWriter.WriteLine("	</body>");
                texWriter.WriteLine("</letter-order>");

                return sb.ToString();
            }
        }
    }
}
