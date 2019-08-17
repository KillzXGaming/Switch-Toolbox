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
        public static void ExportToFile(this Stream stream, string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Write))
            {
                stream.CopyTo(fileStream);
            }
        }
    }
}
