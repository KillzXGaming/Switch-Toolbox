using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
    public static class StreamExtenstios
    {
        public static byte[] ToArray(this Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);

                return memoryStream.ToArray();
            }
        }
    }
}
