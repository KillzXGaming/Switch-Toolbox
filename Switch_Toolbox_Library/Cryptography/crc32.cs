// Copyright(c) 2014 Mads Breusch Klinkby. All rights reserved.
// Licensed under the MIT License; you may not use this file except in compliance with the License.
// You may obtain a copy of the License at https://opensource.org/licenses/MIT
// Published at https://github.com/klinkby/klinkby.checksum

using System.Text;

namespace System.Security.Cryptography
{
    /// <summary>
    /// Computes a CRC32 checksum.
    /// </summary>
    /// <remarks>Based on <see cref="http://sanity-free.org/12/crc32_implementation_in_csharp.html"/></remarks>
    public static class Crc32
    {
        readonly static uint[] Table = CreateTable();

        static Crc32()
        { }

        /// <summary>
        /// Compute the checksum of a UTF8 text.
        /// </summary>
        /// <param name="text">Text to calculate</param>
        /// <returns>Checksum</returns>
        public static uint Compute(string text)
        {
            return Compute(text, Encoding.UTF8);
        }

        /// <summary>
        /// Compute the checksum of a text using a specific encoding.
        /// </summary>
        /// <param name="text">Text to calculate</param>
        /// <param name="encoding">Text encoding</param>
        /// <returns>Checksum</returns>
        public static uint Compute(string text, Encoding encoding)
        {
            if (string.IsNullOrEmpty(text)) return 0;
            byte[] bytes = encoding.GetBytes(text);
            return Compute(bytes);
        }

        /// <summary>
        /// Compute the checksum of a binary buffer.
        /// </summary>
        /// <param name="bytes">Buffer to calculate</param>
        /// <returns></returns>
        public static uint Compute(byte[] bytes)
        {
            uint crc = 0xffffffff;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(((crc) & 0xff) ^ bytes[i]);
                crc = (crc >> 8) ^ Table[index];
            }

            return unchecked((~crc));
        }

        static uint[] CreateTable()
        {
            const uint poly = 0xedb88320;
            var table = new uint[256];
            uint temp = 0;
            for (uint i = 0; i < table.Length; ++i)
            {
                temp = i;
                for (int j = 8; j > 0; --j)
                {
                    if ((temp & 1) == 1)
                    {
                        temp = (uint)((temp >> 1) ^ poly);
                    }
                    else
                    {
                        temp >>= 1;
                    }
                }

                table[i] = temp;
            }

            return table;
        }
    }
}