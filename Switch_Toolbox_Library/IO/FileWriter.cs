using Syroot.BinaryData;
using System.IO;
using System.Text;

namespace Toolbox.Library.IO
{
    public class FileWriter : BinaryDataWriter
    {
        public void CheckByteOrderMark(uint ByteOrderMark)
        {
            if (ByteOrderMark == 0xFEFF)
                ByteOrder = ByteOrder.BigEndian;
            else
                ByteOrder = ByteOrder.LittleEndian;
        }

        public FileWriter(Stream stream, bool leaveOpen = false)
            : base(stream, Encoding.ASCII, leaveOpen)
        {
        }

        public FileWriter(string fileName)
             : this(new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
        }

        public FileWriter(byte[] data)
             : this(new MemoryStream(data))
        {
        }

        public void Write(Syroot.Maths.Vector2F v)
        {
            Write(v.X);
            Write(v.Y);
        }

        public void Write(Syroot.Maths.Vector3F v)
        {
            Write(v.X);
            Write(v.Y);
            Write(v.Z);
        }

        public void Write(Syroot.Maths.Vector4F v)
        {
            Write(v.X);
            Write(v.Y);
            Write(v.Z);
            Write(v.W);
        }

        public void WriteStruct<T>(T item) => Write(item.StructToBytes(ByteOrder == ByteOrder.BigEndian));

        public void WriteSignature(string value)
        {
            Write(Encoding.ASCII.GetBytes(value));
        }

        public void WriteString(string value)
        {
            Write(value, BinaryStringFormat.ZeroTerminated);
        }

        public void WriteUint64Offset(long target)
        {
            long pos = Position;
            using (TemporarySeek(target, SeekOrigin.Begin))
            {
                Write(pos);
            }
        }

        public void SetByteOrder(bool IsBigEndian)
        {
            if (IsBigEndian)
                ByteOrder = ByteOrder.BigEndian;
            else
                ByteOrder = ByteOrder.LittleEndian;
        }

        public void WriteString(string text, uint fixedSize)
        {
            long pos = Position;
            WriteString(text);
            Seek(pos + fixedSize);
        }

        //Writes the total size of a section as a uint. 
        public void WriteSectionSizeU32(long position, long startPosition, long endPosition)
        {
            using (TemporarySeek(position, System.IO.SeekOrigin.Begin))
            {
                Write((uint)endPosition - startPosition);
            }
        }

        //
        // RelativeOffsetPosition controls the relative position the offset starts at
        //
        public void WriteUint32Offset(long target, long RelativeOffsetPosition = 0) 
        {
            long pos = Position;
            using (TemporarySeek(target, SeekOrigin.Begin))
            {
                Write((uint)pos - (uint)RelativeOffsetPosition);
            }
        }

        public void SeekBegin(uint Offset) { Seek(Offset, SeekOrigin.Begin); }
        public void SeekBegin(int Offset) { Seek(Offset, SeekOrigin.Begin); }
        public void SeekBegin(long Offset) { Seek(Offset, SeekOrigin.Begin); }

        public void Write(OpenTK.Vector2 v)
        {
            Write(v.X);
            Write(v.Y);
        }
        public void Write(OpenTK.Vector3 v)
        {
            Write(v.X);
            Write(v.Y);
            Write(v.Z);
        }
        public void Write(OpenTK.Vector4 v)
        {
            Write(v.X);
            Write(v.Y);
            Write(v.Z);
            Write(v.W);
        }
    }
}
