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
            Formats.Add(typeof(Gzip));
            Formats.Add(typeof(lz4));
            Formats.Add(typeof(LZ4F));
            Formats.Add(typeof(LZSS));
            Formats.Add(typeof(YAY0));
            Formats.Add(typeof(Yaz0));
            Formats.Add(typeof(Zlib));
            Formats.Add(typeof(ZlibGZ));
            Formats.Add(typeof(Zstb));
            Formats.Add(typeof(Zstb_Kirby));
            Formats.Add(typeof(MtaCustomCmp));
            Formats.Add(typeof(LZ77));

            return Formats.ToArray();
        }
    }
}
