using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace Switch_Toolbox.Library.IO
{
    public static class IOExtensions
    {
        //https://github.com/exelix11/EditorCore/blob/872d210f85ec0409f8a6ac3a12fc162aaf4cd90c/EditorCoreCommon/CustomClasses.cs#L367
        public static bool Matches(this byte[] arr, string magic) =>
                    arr.Matches(0, magic.ToCharArray());
        public static bool Matches(this byte[] arr, uint magic) =>
            arr.Matches(0, BitConverter.GetBytes(magic));

        public static bool Matches(this byte[] arr, uint startIndex, params byte[] magic)
        {
            if (arr.Length < magic.Length + startIndex) return false;
            for (uint i = 0; i < magic.Length; i++)
            {
                if (arr[i + startIndex] != magic[i]) return false;
            }
            return true;
        }

        public static byte[] ToByteArray(this string str, int length)
        {
            return Encoding.ASCII.GetBytes(str.PadRight(length, ' '));
        }

        public static uint EncodeCrc32(this string str)
        {
            return Crc32.Compute(str);
        }

        public static bool Matches(this byte[] arr, uint startIndex, params char[] magic)
        {
            if (arr.Length < magic.Length + startIndex) return false;
            for (uint i = 0; i < magic.Length; i++)
            {
                if (arr[i + startIndex] != magic[i]) return false;
            }
            return true;
        }

        public static uint Reverse(this uint x)
        {
            // swap adjacent 16-bit blocks
            x = (x >> 16) | (x << 16);
            // swap adjacent 8-bit blocks
            return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
        }

        //https://stackoverflow.com/questions/2230826/remove-invalid-disallowed-bad-characters-from-filename-or-directory-folder/12800424#12800424
        public static string RemoveIllegaleFileNameCharacters(this string str)
        {
            return string.Join("_", str.Split(Path.GetInvalidFileNameChars()));
        }

        public static string RemoveIllegaleFolderNameCharacters(this string str)
        {
            return string.Join("_", str.Split(Path.GetInvalidPathChars()));
        }

    }
}
