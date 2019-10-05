using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FirstPlugin
{
    /// <summary>
    /// Collision from the KCL library turned into a mono script
    /// </summary>
    public class KclMonoscript
    {
        public class HeaderV2
        {

        }

        public class HeaderV1
        {

        }

        public HeaderV2 Header;

        public void ReadKCL(string fileName)
        {
            using (var reader = new StreamReader(File.OpenRead(fileName)))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.Contains("KCollisionV2Header "))
                    {
                        Header = new HeaderV2();

                    }
                }
            }
        }
    }
}
