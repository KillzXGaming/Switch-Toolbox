using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syroot.BinaryData;
using System.IO;
using System.IO.Compression;
using OpenTK;
using K4os.Compression.LZ4.Streams;

namespace Switch_Toolbox.Library.IO
{
    public class STLibraryCompression
    {
        public static byte[] CompressFile(byte[] data, IFileFormat format)
        {
            int Alignment = 0;

            if (format.IFileInfo != null)
                Alignment = format.IFileInfo.Alignment;

            switch (format.CompressionType)
            {
                case CompressionType.Yaz0:
                    return EveryFileExplorer.YAZ0.Compress(data, 3, (uint)Alignment);
                case CompressionType.None:
                    return data;
                default:
                    return data;
            }
        }

        public class GZIP
        {
            public static byte[] Decompress(byte[] b)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    using (GZipStream gzip = new GZipStream(new MemoryStream(b), CompressionMode.Decompress))
                    {
                        gzip.CopyTo(mem);
                        mem.Write(b, 0, b.Length);
                    }
                    return mem.ToArray();
                }
            }

            public static byte[] Compress(byte[] b)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    using (GZipStream gzip = new GZipStream(mem,
                        CompressionMode.Compress, true))
                    {
                        gzip.Write(b, 0, b.Length);
                    }
                    return mem.ToArray();
                }
            }
        }
        public class Type_LZ4F
        {
            public static byte[] Decompress(byte[] data)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    using (var source = LZ4Stream.Decode(new MemoryStream(data)))
                    {
                        source.CopyTo(mem);
                        mem.Write(data, 0, data.Length);
                    }
                    return mem.ToArray();
                }
            }
            public static byte[] Compress(byte[] data)
            {
                LZ4EncoderSettings settings = new LZ4EncoderSettings();
                settings.ChainBlocks = false;
         //       settings.BlockSize = K4os.Compression.LZ4.Internal.Mem.M1;

                using (MemoryStream mem = new MemoryStream())
                {
                    var encodeSettings = new LZ4EncoderSettings();
                    using (var source = LZ4Stream.Encode(mem, settings))
                    {
                        source.Write(data, 0, data.Length);

                        var newMem = new MemoryStream();
                        BinaryWriter writer = new BinaryWriter(newMem);
                        writer.Write((uint)data.Length);
                        writer.Write(mem.ToArray());
                        writer.Write((uint)973407368);
                        return newMem.ToArray();
                    }
                }
            }
        }
        public class Type_LZ4
        {
            public static byte[] Decompress(byte[] data, int inputOffset, int InputLength, int decompressedSize)
            {
                return LZ4.LZ4Codec.Decode(data, inputOffset, InputLength, decompressedSize);
            }
            public static byte[] Decompress(byte[] data)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    using (var source = LZ4Stream.Decode(new MemoryStream(data)))
                    {
                        source.CopyTo(mem);
                        mem.Write(data, 0, data.Length);
                    }
                    return mem.ToArray();
                }
            }
            public static byte[] Compress(byte[] data, int inputOffset = 0)
            {
                return LZ4.LZ4Codec.Encode(data, inputOffset, data.Length);
            }
        }
    }

    public class FileWriter : BinaryDataWriter
    {
        public FileWriter(Stream stream, bool leaveOpen = false)
            : base(stream, Encoding.ASCII, leaveOpen)
        {
        }

        public FileWriter(string fileName)
             : this(new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Write))
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
    }
    public class FileExt
    {
        public static Vector2 ToVec2(Syroot.Maths.Vector2F v)
        {
            return new Vector2(v.X, v.Y);
        }
        public static Vector3 ToVec3(Syroot.Maths.Vector3F v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
        public static Vector4 ToVec4(Syroot.Maths.Vector4F v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }
        public static Vector2 ToVec2(float[] v)
        {
            return new Vector2(v[0], v[1]);
        }
        public static Vector3 ToVec3(float[] v)
        {
            return new Vector3(v[0], v[1], v[2]);
        }
        public static Vector4 ToVec4(float[] v)
        {
            return new Vector4(v[0], v[1], v[2], v[3]);
        }


        public static string DataToString(Syroot.Maths.Vector2F v)
        {
            return $"{v.X},{v.Y}";
        }
        public static string DataToString(Syroot.Maths.Vector3F v)
        {
            return $"{v.X},{v.Y},{v.Z}";
        }
        public static string DataToString(Syroot.Maths.Vector4F v)
        {
            return $"{v.X},{v.Y},{v.Z} {v.W}";
        }
    }
    public class FileReader : BinaryDataReader
    {
        public FileReader(Stream stream, bool leaveOpen = false)
            : base(stream, Encoding.ASCII, leaveOpen)
        {
        }

        public FileReader(string fileName)
             : this(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
        }
        public FileReader(byte[] data)
             : this(new MemoryStream(data))
        {
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
