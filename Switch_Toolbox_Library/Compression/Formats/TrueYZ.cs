/*
  TrueYZ - Byte-exact Yaz0 compression library

  This file reconstructs the Yaz0 (SZS) encoder used by Nintendo EAD/EPD
  from the Nintendo 3DS era onwards. It is a zlib-derived lazy-matching
  deflate front end (hash chains, longest_match, one-token lookahead)
  driving a Yaz0 token writer instead of a Huffman coder.

  Because the entire purpose of this file is to reproduce another encoder's
  output exactly, the match-finder constants and the shape of the search are
  not free parameters. Several of them differ from stock zlib, and each such
  divergence is called out where it appears. Changing any of them silently
  produces valid Yaz0 that no longer matches the reference bytes.

  This software is released under the MIT License.
  See LICENSE file for details.
 https://github.com/aboood40091/TrueYZ/blob/master/LICENSE
*/

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Yaz0Test
{
    public static class TrueYZ
    {
        private const string LibraryName = "trueyz";
        private const int Yaz0HeaderSize = 16;

        static TrueYZ()
        {
        }

        public static bool CanUse()
        {
            return File.Exists(LibraryName + ".dll");
        }

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int trueyz_compress(
            [In] byte[] input, int length, [Out] byte[] output);

        [DllImport(LibraryName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int trueyz_compress_align(
            [In] byte[] input, int length, [Out] byte[] output, int alignment);

        public static byte[] Compress(byte[] input, int alignment = 0)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            byte[] output = new byte[TRUEYZ_BOUND(input.Length)];
            int size = alignment > 0 ? trueyz_compress_align(input, input.Length, output, alignment)
                                     : trueyz_compress(input, input.Length, output);

            if (size <= 0)
                throw new Exception("trueyz compression failed.");

            Array.Resize(ref output, size);
            return output;
        }

        private static long TRUEYZ_BOUND(int length)
             => Yaz0HeaderSize + (long)length + (length / 8) + 1;
    }
}
