using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Toolbox.Library.IO
{
    public class FileStreamStorage
    {
        public Stream CurrentStream;

        public byte[] ToBytes() {return CurrentStream.ToArray(); }

        public FileStreamStorage(Stream stream)
        {
            CurrentStream = stream;
        }
    }
}
