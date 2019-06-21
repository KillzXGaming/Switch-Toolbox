using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.BinaryData;
using System.IO;
using System.IO.Compression;
using OpenTK;

namespace Switch_Toolbox.Library.IO
{
    public class FileReader : BinaryDataReader
    {
        public FileReader(Stream stream, bool leaveOpen = false)
            : base(stream, Encoding.ASCII, leaveOpen)
        {
            this.Position = 0;
        }

        public FileReader(string fileName)
             : this(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            this.Position = 0;
        }
        public FileReader(byte[] data)
             : this(new MemoryStream(data))
        {
            this.Position = 0;
        }

        //Checks signature (no stream advancement)
        public bool CheckSignature(int length, string Identifier, long position = 0)
        {
            if (Position + length >= BaseStream.Length || position < 0)
                return false;

            Position = position;
            string signature = ReadString(length, Encoding.ASCII);

            //Reset position
            Position = 0;

            return signature == Identifier;
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

        public string ReadZeroTerminatedString()
        {
            return ReadString(BinaryStringFormat.ZeroTerminated);
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

        public string ReadSignature(int length, string ExpectedSignature)
        {
            string RealSignature = ReadString(length, Encoding.ASCII);

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

        public void SeekBegin(uint Offset) { Seek(Offset, SeekOrigin.Begin); }
        public void SeekBegin(int Offset)  { Seek(Offset, SeekOrigin.Begin); }
        public void SeekBegin(long Offset) { Seek(Offset, SeekOrigin.Begin); }

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

            if (offset == 0) return null;

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
        public byte[] getSection(int offset, int size)
        {
            Seek(offset, SeekOrigin.Begin);
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
