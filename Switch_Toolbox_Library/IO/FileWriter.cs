using Syroot.BinaryData;
using System.IO;
using System.Text;

namespace Switch_Toolbox.Library.IO
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
    }
}
