using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Toolbox.Library
{
    public interface ICompressionFormat
    {
        string[] Description { get; set; }
        string[] Extension { get; set; }

        bool Identify(Stream stream, string fileName);
        bool CanCompress { get; }

        Stream Decompress(Stream stream);
        Stream Compress(Stream stream);
    }
}
