using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library
{
    public class FormatList
    {
        //File formats accessable from this library
        public static Type[] GetFileFormats()
        {
            List<Type> Formats = new List<Type>();
            Formats.Add(typeof(DDS));
            Formats.Add(typeof(ASTC));
            Formats.Add(typeof(TGA));
            Formats.Add(typeof(TPFileSizeTable));
            return Formats.ToArray();
        }

        //Compression formats accessable from this library
        public static Type[] GetCompressionFormats()
        {
            List<Type> Formats = new List<Type>();
            Formats.Add(typeof(LZSS));
            return Formats.ToArray();
        }
    }
}
