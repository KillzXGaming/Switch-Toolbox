using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Toolbox.Library.IO
{
    public static class StreamExport
    {
        public static byte[] ToBytes(this Stream stream)
        {
            using (var reader = new FileReader(stream, true))
            {
                return reader.ReadBytes((int)stream.Length);
            }

            using (var memStream = new MemoryStream())
            {
                stream.CopyTo(memStream);
                return memStream.ToArray();
            }
        }

        public static void ExportToFile(this Stream stream, string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
            {
                stream.Position = 0;
                stream.CopyTo(fileStream);
            }
        }
    }
}
