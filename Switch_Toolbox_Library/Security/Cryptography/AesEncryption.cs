using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Toolbox.Library.Security.Cryptography
{
    public static class AesEncryption
    {
        private static byte[] keyBytes;
        private static byte[] IvBytes;

        static AesEncryption()
        {
            keyBytes = UTF8Encoding.UTF8.GetBytes("000000000");
            IvBytes = UTF8Encoding.UTF8.GetBytes("000000000");
        }

        public static void SetKey(string key)
        {
            keyBytes = UTF8Encoding.UTF8.GetBytes(key);
        }

        public static void SetIV(string key)
        {
            IvBytes = UTF8Encoding.UTF8.GetBytes(key);
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static string DecodeAndDecrypt(string cipherText)
        {
            string DecodeAndDecrypt = AesDecrypt(StringToByteArray(cipherText));
            return (DecodeAndDecrypt);
        }

        public static string EncryptAndEncode(string plaintext)
        {
            return ByteArrayToHexString(AesEncrypt(plaintext));
        }

        public static string AesDecrypt(Byte[] inputBytes)
        {
            var cryptoTransform = new AesManaged().CreateDecryptor(keyBytes, IvBytes);
            return Encoding.UTF8.GetString(TransformBlocks(cryptoTransform, inputBytes));
        }

        public static byte[] AesEncrypt(string inputText) => AesEncrypt(UTF8Encoding.UTF8.GetBytes(inputText));

        public static byte[] AesEncrypt(byte[] inputBytes)
        {
            var cryptoTransform = new AesManaged().CreateEncryptor(keyBytes, IvBytes);
            return TransformBlocks(cryptoTransform, inputBytes);
        }

        static byte[] TransformBlocks(ICryptoTransform cryptoTransform, byte[] input)
        {
            byte[] result = new byte[input.Length];

            int num = 0;
            while (num < input.Length)
            {
                cryptoTransform.TransformBlock(input, num, 16, result, num);
                num += 16;
            }
            while (result[0] == (byte)0)
                result = ((IEnumerable<byte>)result).Skip<byte>(1).ToArray<byte>();
            return result;
        }
    }
}
