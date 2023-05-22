using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace Toolbox.Library.Compression.LZ77_wii_11_compresss.Utils
{
    /// <summary>
    /// Class for I/O-related utility methods.
    /// </summary>
    public static class IOUtils
    {

        #region byte[] <-> (u)int
        /// <summary>
        /// Returns a 4-byte unsigned integer as used on the NDS converted from four bytes
        /// at a specified position in a byte array.
        /// </summary>
        /// <param name="buffer">The source of the data.</param>
        /// <param name="offset">The location of the data in the source.</param>
        /// <returns>The indicated 4 bytes converted to uint</returns>
        public static uint ToNDSu32(byte[] buffer, int offset)
        {
            return (uint)(buffer[offset]
                        | (buffer[offset + 1] << 8)
                        | (buffer[offset + 2] << 16)
                        | (buffer[offset + 3] << 24));
        }


        /// <summary>
        /// Returns a 4-byte signed integer as used on the NDS converted from four bytes
        /// at a specified position in a byte array.
        /// </summary>
        /// <param name="buffer">The source of the data.</param>
        /// <param name="offset">The location of the data in the source.</param>
        /// <returns>The indicated 4 bytes converted to int</returns>
        public static int ToNDSs32(byte[] buffer, int offset)
        {
            return (int)(buffer[offset]
                        | (buffer[offset + 1] << 8)
                        | (buffer[offset + 2] << 16)
                        | (buffer[offset + 3] << 24));
        }


        /// <summary>
        /// Returns a 3-byte integer as used in the built-in compression
        /// formats in the DS, convrted from three bytes at a specified position in a byte array,
        /// </summary>
        /// <param name="buffer">The source of the data.</param>
        /// <param name="offset">The location of the data in the source.</param>
        /// <returns>The indicated 3 bytes converted to an integer.</returns>
        public static int ToNDSu24(byte[] buffer, int offset)
        {
            return (int)(buffer[offset]
                        | (buffer[offset + 1] << 8)
                        | (buffer[offset + 2] << 16));
        }
        #endregion
    }
}
