using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.BinaryData;
using System.IO;
using System.IO.Compression;
using OpenTK;
using System.Runtime.InteropServices;

namespace Toolbox.Library.IO
{
    public class FileReader : BinaryDataReader
    {
        public FileReader(Stream stream, bool leaveOpen = false)
            : base(stream, Encoding.ASCII, leaveOpen)
        {
            this.Position = 0;
        }

        public FileReader(string fileName, bool leaveOpen = false)
             : this(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), leaveOpen)
        {
            this.Position = 0;
        }
        public FileReader(byte[] data)
             : this(new MemoryStream(data))
        {
            this.Position = 0;
        }

        public bool IsBigEndian => ByteOrder == ByteOrder.BigEndian;

        //Checks signature (no stream advancement)
        public bool CheckSignature(int length, string Identifier, long position = 0)
        {
            if (Position + length + position >= BaseStream.Length || position < 0)
                return false;

            Position = position;
            string signature = ReadString(length, Encoding.ASCII);

            //Reset position
            Position = 0;

            return signature == Identifier;
        }

        //From kuriimu https://github.com/IcySon55/Kuriimu/blob/master/src/Kontract/IO/BinaryReaderX.cs#L40
        public T ReadStruct<T>() => ReadBytes(Marshal.SizeOf<T>()).BytesToStruct<T>(ByteOrder == ByteOrder.BigEndian);
        public List<T> ReadMultipleStructs<T>(int count) => Enumerable.Range(0, count).Select(_ => ReadStruct<T>()).ToList();
        public List<T> ReadMultipleStructs<T>(uint count) => Enumerable.Range(0, (int)count).Select(_ => ReadStruct<T>()).ToList();

        public bool CheckSignature(uint Identifier, long position = 0)
        {
            if (Position + 4 >= BaseStream.Length || position < 0 || position + 4 >= BaseStream.Length)
                return false;

            Position = position;
            uint signature = ReadUInt32();

            //Reset position
            Position = 0;

            return signature == Identifier;
        }

        public int ReadInt32(int position)
        {
            long origin = this.Position;

            SeekBegin(position);
            int value = ReadInt32();

            SeekBegin(origin + sizeof(int));
            return value;
        }

        public uint ReadUInt32(int position)
        {
            long origin = this.Position;

            SeekBegin(position);
            uint value = ReadUInt32();

            SeekBegin(origin + sizeof(uint));
            return value;
        }

        public string ReadNameOffset(bool IsRelative, Type OffsetType, bool ReadNameLength = false, bool IsNameLengthShort = false)
        {
            long pos = Position;
            long offset = 0;

            if (OffsetType == typeof(long))
                offset = ReadInt64();
            if (OffsetType == typeof(ulong))
                offset = (long)ReadUInt64();
            if (OffsetType == typeof(uint))
                offset = ReadUInt32();
            if (OffsetType == typeof(int))
                offset = ReadInt32();

            if (IsRelative && offset != 0)
                offset += pos;

            if (offset != 0)
            {
                using (TemporarySeek(offset, SeekOrigin.Begin))
                {
                    uint NameLength = 0;
                    if (ReadNameLength)
                    {
                        if (IsNameLengthShort)
                            NameLength = ReadUInt16();
                        else
                            NameLength = ReadUInt32();
                    }

                    return ReadString(BinaryStringFormat.ZeroTerminated);
                }
            }
            else
                return "";
        }

        public List<string> ReadNameOffsets(uint Count, bool IsRelative, Type OffsetType, bool ReadNameLength = false)
        {
            List<string> Names = new List<string>();
            for (int i = 0; i < Count; i++)
                Names.Add(ReadNameOffset(IsRelative, OffsetType, ReadNameLength));

            return Names;
        }

        public string ReadString(int length, bool removeSpaces)
        {
            return ReadString(length).Replace("\0", string.Empty);
        }

        public string ReadZeroTerminatedString(Encoding encoding = null)
        {
            return ReadString(BinaryStringFormat.ZeroTerminated, encoding ?? Encoding);
        }

        public string[] ReadZeroTerminatedStrings(uint count, Encoding encoding = null)
        {
            string[] str = new string[count];
            for (int i = 0; i < count; i++)
                str[i] = ReadString(BinaryStringFormat.ZeroTerminated, encoding ?? Encoding);
            return str;
        }

        public string ReadUTF16String()
        {
            List<byte> chars = new List<byte>();

            while (true)
            {
                ushort val = ReadUInt16();

                if (val == 0)
                {
                    return Encoding.ASCII.GetString(chars.ToArray());
                }
                else
                    chars.Add((byte)val); // casting to byte will remove the period, which is a part of UTF-16
            }
        }

        /// <summary>
        /// Checks the byte order mark to determine the endianness of the reader.
        /// </summary>
        /// <param name="ByteOrderMark">The byte order value being read. 0xFFFE = Little, 0xFEFF = Big. </param>
        /// <returns></returns>
        public void CheckByteOrderMark(uint ByteOrderMark)
        {
            SetByteOrder(ByteOrderMark == 0xFEFF);
        }

        public void SetByteOrder(bool IsBigEndian)
        {
            if (IsBigEndian)
                ByteOrder = ByteOrder.BigEndian;
            else
                ByteOrder = ByteOrder.LittleEndian;
        }

        public string ReadSignature(int length, string ExpectedSignature, bool TrimEnd = false)
        {
            string RealSignature =  ReadString(length, Encoding.ASCII);

            if (TrimEnd) RealSignature = RealSignature.TrimEnd(' ');

            if (RealSignature != ExpectedSignature)
                throw new Exception($"Invalid signature {RealSignature}! Expected {ExpectedSignature}.");

            return RealSignature;
        }

        public float ReadByteAsFloat()
        {
            return ReadByte() / 255.0f;
        }

        public float ReadHalfSingle()
        {
            return new Syroot.IOExtension.Half(ReadUInt16());
        }

        public Matrix4 ReadMatrix4(bool SwapRows = false)
        {
            Matrix4 mat4 = new Matrix4();
            if (SwapRows)
            {
                mat4.M11 = ReadSingle();
                mat4.M21 = ReadSingle();
                mat4.M31 = ReadSingle();
                mat4.M41 = ReadSingle();
                mat4.M12 = ReadSingle();
                mat4.M22 = ReadSingle();
                mat4.M32 = ReadSingle();
                mat4.M42 = ReadSingle();
                mat4.M13 = ReadSingle();
                mat4.M23 = ReadSingle();
                mat4.M33 = ReadSingle();
                mat4.M43 = ReadSingle();
                mat4.M14 = ReadSingle();
                mat4.M24 = ReadSingle();
                mat4.M34 = ReadSingle();
                mat4.M44 = ReadSingle();
            }
            else
            {
                mat4.M11 = ReadSingle();
                mat4.M12 = ReadSingle();
                mat4.M13 = ReadSingle();
                mat4.M14 = ReadSingle();
                mat4.M21 = ReadSingle();
                mat4.M22 = ReadSingle();
                mat4.M23 = ReadSingle();
                mat4.M24 = ReadSingle();
                mat4.M31 = ReadSingle();
                mat4.M32 = ReadSingle();
                mat4.M33 = ReadSingle();
                mat4.M34 = ReadSingle();
                mat4.M41 = ReadSingle();
                mat4.M42 = ReadSingle();
                mat4.M43 = ReadSingle();
                mat4.M44 = ReadSingle();
            }
            return mat4;
        }

        public void SeekBegin(uint Offset) { Seek(Offset, SeekOrigin.Begin); }
        public void SeekBegin(int Offset)  { Seek(Offset, SeekOrigin.Begin); }
        public void SeekBegin(long Offset) { Seek(Offset, SeekOrigin.Begin); }
        public void SeekBegin(ulong Offset) { Seek((long)Offset, SeekOrigin.Begin); }

        public long ReadOffset(bool IsRelative, Type OffsetType)
        {
            long pos = Position;
            long offset = 0;

            if (OffsetType == typeof(long))
                offset = ReadInt64();
            if (OffsetType == typeof(ulong))
                offset = (long)ReadUInt64();
            if (OffsetType == typeof(uint))
                offset = ReadUInt32();
            if (OffsetType == typeof(int))
                offset = ReadInt32();

            if (IsRelative && offset != 0)
                return pos + offset;
            else
                return offset;
        }

        public string LoadString(bool IsRelative, Type OffsetType, Encoding encoding = null, uint ReadStringLength = 0)
        {
            long pos = Position;

            long offset = 0;
            int size = 0;

            if (OffsetType == typeof(long))
                offset = ReadInt64();
            if (OffsetType == typeof(ulong))
                offset = (long)ReadUInt64();
            if (OffsetType == typeof(uint))
                offset = ReadUInt32();
            if (OffsetType == typeof(int))
                offset = ReadInt32();

            if (offset == 0) return string.Empty;

            if (IsRelative)
                offset = offset + pos;

            encoding = encoding ?? Encoding;
            using (TemporarySeek(offset, SeekOrigin.Begin))
            {
                //Read the size of the string if set
                uint stringLength = 0;

                if (ReadStringLength == 2)
                    stringLength = ReadUInt16();
                if (ReadStringLength == 4)
                    stringLength = ReadUInt32();

                return ReadString(BinaryStringFormat.ZeroTerminated, encoding);
            }
        }

        public STColor8[] ReadColor8sRGBA(int count)
        {
            STColor8[] colors = new STColor8[count];
            for (int i = 0; i < count; i++)
                colors[i] = STColor8.FromBytes(ReadBytes(4));

            return colors;
        }

        public STColor8 ReadColor8RGBA()
        {
            return STColor8.FromBytes(ReadBytes(4));
        }

        public STColor16[] ReadColor16sRGBA(int count)
        {
            STColor16[] colors = new STColor16[count];
            for (int i = 0; i < count; i++)
                colors[i] = STColor16.FromShorts(ReadUInt16s(4));

            return colors;
        }

        public STColor16 ReadColor16RGBA()
        {
            return STColor16.FromShorts(ReadUInt16s(4));
        }

        public STColor[] ReadColorsRGBA(int count)
        {
            STColor[] colors = new STColor[count];
            for (int i = 0; i < count; i++)
                colors[i] =  STColor.FromFloats(ReadSingles(4));

            return colors;
        }

        public STColor ReadColorRGBA()
        {
            return STColor.FromFloats(ReadSingles(4));
        }

        public static byte[] DeflateZLIB(byte[] i)
        {
            MemoryStream output = new MemoryStream();
            output.WriteByte(0x78);
            output.WriteByte(0x9C);
            using (DeflateStream dstream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dstream.Write(i, 0, i.Length);
            }
            return output.ToArray();
        }

        public byte[] getSection(uint offset, uint size)
        {
            Position = offset;
            return ReadBytes((int)size);
        }

        public byte[] getSection(int offset, int size)
        {
            Position = offset;
            return ReadBytes(size);
        }

        public Vector4 ReadVec4()
        {
            return new Vector4(ReadSingle(), ReadSingle(), ReadSingle(), ReadSingle());
        }
        public Vector3 ReadVec3()
        {
            return new Vector3(ReadSingle(), ReadSingle(), ReadSingle());
        }
        public Syroot.Maths.Vector3F ReadVec3SY()
        {
            return new Syroot.Maths.Vector3F(ReadSingle(), ReadSingle(), ReadSingle());
        }
        public Vector2 ReadVec2()
        {
            return new Vector2(ReadSingle(), ReadSingle());
        }
        public Syroot.Maths.Vector2F ReadVec2SY()
        {
            return new Syroot.Maths.Vector2F(ReadSingle(), ReadSingle());
        }
        public static byte[] InflateZLIB(byte[] i)
        {
            var stream = new MemoryStream();
            var ms = new MemoryStream(i);
            ms.ReadByte();
            ms.ReadByte();
            var zlibStream = new DeflateStream(ms, CompressionMode.Decompress);
            byte[] buffer = new byte[4095];
            while (true)
            {
                int size = zlibStream.Read(buffer, 0, buffer.Length);
                if (size > 0)
                    stream.Write(buffer, 0, buffer.Length);
                else
                    break;
            }
            zlibStream.Close();
            return stream.ToArray();
        }
        public string ReadMagic(int Offset, int Length)
        {
            Seek(Offset, SeekOrigin.Begin);
            return ReadString(Length);
        }
    }
}
