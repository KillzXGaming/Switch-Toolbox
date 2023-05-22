using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolbox.Library.Compression.LZ77_wii_11_compresss.Exceptions
{
    /// <summary>
    /// An exception thrown by the compression or decompression function, indicating that the
    /// given input length was too large for the given input stream.
    /// </summary>
    public class StreamTooShortException : EndOfStreamException
    {
        /// <summary>
        /// Creates a new exception that indicates that the stream was shorter than the given input length.
        /// </summary>
        public StreamTooShortException()
            : base("The end of the stream was reached "
                 + "before the given amout of data was read.")
        { }
    }
}
