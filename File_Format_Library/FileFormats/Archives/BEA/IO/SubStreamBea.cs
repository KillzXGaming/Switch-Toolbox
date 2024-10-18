using System;
using System.IO;

namespace BezelEngineArchive_Lib
{
    public class SubStreamBea : Stream
    {
        Stream baseStream;
        readonly long length;
        readonly long baseOffset;
        public SubStreamBea(Stream baseStream, long offset, long length)
        {
            if (baseStream == null) throw new ArgumentNullException("baseStream");
            if (!baseStream.CanRead) throw new ArgumentException("baseStream.CanRead is false");
            if (!baseStream.CanSeek) throw new ArgumentException("baseStream.CanSeek is false");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset");
            if (offset + length > baseStream.Length) throw new ArgumentOutOfRangeException("length");

            this.baseStream = baseStream;
            this.length = length;
            baseOffset = offset;
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            baseStream.Position = baseOffset + offset + Position;
            int read = baseStream.Read(buffer, offset, (int)Math.Min(count, length - Position));
            Position += read;
            return read;
        }
        public override long Length => length;
        public override bool CanRead => true;
        public override bool CanWrite => false;
        public override bool CanSeek => true;
        public override long Position { get; set; }
        public override void Flush() => baseStream.Flush();

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin: return Position = offset;
                case SeekOrigin.Current: return Position += offset;
                case SeekOrigin.End: return Position = length + offset;
            }
            throw new ArgumentException("origin is invalid");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}