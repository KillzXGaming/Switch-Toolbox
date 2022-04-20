using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Toolbox.Library.Security.Cryptography;
using System.Runtime.InteropServices;
using Syroot.BinaryData;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Toolbox.Library.IO
{
    public static class IOExtensions
    {
        public static OpenTK.Vector2 ToTKVector2(this Syroot.Maths.Vector2F vec2) {
            return new OpenTK.Vector2(vec2.X, vec2.Y);
        }

        public static OpenTK.Vector3 ToTKVector3(this Syroot.Maths.Vector3F vec3) {
            return new OpenTK.Vector3(vec3.X, vec3.Y, vec3.Z);
        }

        public static OpenTK.Vector4 ToTKVector4(this Syroot.Maths.Vector4F vec4) {
            return new OpenTK.Vector4(vec4.X, vec4.Y, vec4.Z, vec4.W);
        }

        public static byte[] DeserializeToBytes<T>(this T structure) where T : struct
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(stream, structure);

                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                return stream.ToArray();
            }
        }

        public static T SerializeToStruct<T>(this byte[] buffer) where T : struct
        {
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                BinaryFormatter bf = new BinaryFormatter();
                var data = bf.Deserialize(stream);
                return (T)data;
            }
        }

        //Structs can be a bit faster and more memory efficent
        //From https://github.com/IcySon55/Kuriimu/blob/master/src/Kontract/IO/Extensions.cs
        //Read
        public static unsafe T BytesToStruct<T>(this byte[] buffer, bool isBigEndian = false, int offset = 0)
        {
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));

            Console.WriteLine("bytes to struct isBigEndian " + isBigEndian);
            AdjustBigEndianByteOrder(typeof(T), buffer, isBigEndian);

            fixed (byte* pBuffer = buffer) 
                return Marshal.PtrToStructure<T>((IntPtr)pBuffer + offset);
        }

        // Write
        public static unsafe byte[] StructToBytes<T>(this T item, bool isBigEndian)
        {
            var buffer = new byte[Marshal.SizeOf(typeof(T))];

            fixed (byte* pBuffer = buffer)
                Marshal.StructureToPtr(item, (IntPtr)pBuffer, false);

            AdjustBigEndianByteOrder(typeof(T), buffer, isBigEndian);

            return buffer;
        }

        //Adjust byte order for big endian
        private static void AdjustBigEndianByteOrder(Type type, byte[] buffer, bool isBigEndian, int startOffset = 0)
        {
            if (!isBigEndian)
                return;

            if (type.IsPrimitive)
            {
                if (type == typeof(short) || type == typeof(ushort) ||
                 type == typeof(int) || type == typeof(uint) ||
                 type == typeof(long) || type == typeof(ulong) ||
                  type == typeof(double) || type == typeof(float))
                {
                    Array.Reverse(buffer);
                    return;
                }
            }

            foreach (var field in type.GetFields())
            {
                var fieldType = field.FieldType;

                // Ignore static fields
                if (field.IsStatic) continue;

                if (fieldType.BaseType == typeof(Enum) && fieldType != typeof(ByteOrder))
                    fieldType = fieldType.GetFields()[0].FieldType;

                var offset = Marshal.OffsetOf(type, field.Name).ToInt32();
                // Enums
                if (fieldType.IsEnum)
                    fieldType = Enum.GetUnderlyingType(fieldType);

                // Check for sub-fields to recurse if necessary
                var subFields = fieldType.GetFields().Where(subField => subField.IsStatic == false).ToArray();
                var effectiveOffset = startOffset + offset;

                if (fieldType == typeof(short) || fieldType == typeof(ushort) ||
                    fieldType == typeof(int) || fieldType == typeof(uint) ||
                    fieldType == typeof(long) || fieldType == typeof(ulong) ||
                    fieldType == typeof(double) || fieldType == typeof(float))
                {
                    if (subFields.Length == 0)
                        Array.Reverse(buffer, effectiveOffset, Marshal.SizeOf(fieldType));
          
                }

                if (subFields.Length > 0)
                    AdjustBigEndianByteOrder(fieldType, buffer, isBigEndian, effectiveOffset);
            }
        }

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
            char[] _invalidFileNameChars = new char[41] { '\x00', '\x01', '\x02', '\x03', '\x04', '\x05', '\x06', '\x07',
            '\x08', '\x09', '\x0A', '\x0B', '\x0C', '\x0D', '\x0E', '\x0F', '\x10', '\x11', '\x12',
            '\x13', '\x14', '\x15', '\x16', '\x17', '\x18', '\x19', '\x1A', '\x1B', '\x1C', '\x1D',
            '\x1E', '\x1F', '\x22', '\x3C', '\x3E', '\x7C', ':', '*', '?', '\\', '/' };

            return string.Join("_", str.Split(_invalidFileNameChars));
        }

        public static string RemoveIllegaleFolderNameCharacters(this string str)
        {
            return string.Join("_", str.Split(Path.GetInvalidPathChars()));
        }

    }
}
