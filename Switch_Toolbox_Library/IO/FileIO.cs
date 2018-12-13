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
using System.Windows.Forms;

namespace Switch_Toolbox.Library.IO
{
    public class STFileSaver
    {
        /// <summary>
        /// Saves the <see cref="IFileFormat"/> as a file from the given <param name="FileName">
        /// </summary>
        /// <param name="IFileFormat">The format instance of the file being saved</param>
        /// <param name="FileName">The name of the file</param>
        /// <param name="Alignment">The Alignment used for compression. Used for Yaz0 compression type. </param>
        /// <param name="EnableDialog">Toggle for showing compression dialog</param>
        /// <returns></returns>
        public static void SaveFileFormat(IFileFormat FileFormat, string FileName, int Alignment = 0, bool EnableDialog = true)
        {
            Cursor.Current = Cursors.WaitCursor;

            byte[] data = FileFormat.Save();
            if (EnableDialog && FileFormat.FileIsCompressed)
            {
                DialogResult save = MessageBox.Show($"Compress file with {FileFormat.CompressionType}?", "File Save", MessageBoxButtons.YesNo);

                if (save == DialogResult.Yes)
                {
                    switch (FileFormat.CompressionType)
                    {
                        case CompressionType.Yaz0:
                            data = EveryFileExplorer.YAZ0.Compress(data, Runtime.Yaz0CompressionLevel, (uint)Alignment);
                            break;
                        case CompressionType.Zstb:
                            data = STLibraryCompression.ZSTD.Compress(data);
                            break;
                        case CompressionType.Lz4:
                            data = STLibraryCompression.Type_LZ4.Compress(data);
                            break;
                        case CompressionType.Lz4f:
                            data = STLibraryCompression.Type_LZ4.Compress(data);
                            break;
                        case CompressionType.Gzip:
                            data = STLibraryCompression.GZIP.Compress(data);
                            break;
                        default:
                            MessageBox.Show($"Compression Type {FileFormat.CompressionType} not supported!!");
                            break;
                    }
                }
            }
            File.WriteAllBytes(FileName, data);
            Cursor.Current = Cursors.Default;
            MessageBox.Show($"File has been saved to {FileName}");
        }
    }
    public class STFileLoader
    {
        /// <summary>
        /// Gets the <see cref="TreeNodeFile"/> from a file or byte array. 
        /// </summary>
        /// <param name="FileName">The name of the file</param>
        /// <param name="data">The byte array of the data</param>
        /// <param name="InArchive">If the file is in an archive so it can be saved back</param>
        /// <param name="archiveNode">The node being replaced from an archive</param>
        /// <param name="ArchiveHash">The unique hash from an archive for saving</param>
        /// <param name="Compressed">If the file is being compressed or not</param>
        /// <param name="CompType">The type of <see cref="CompressionType"/> being used</param>
        /// <returns></returns>
        public static TreeNodeFile GetNodeFileFormat(string FileName, byte[] data = null, bool InArchive = false,
            string ArchiveHash = "", TreeNode archiveNode = null, bool Compressed = false, CompressionType CompType = 0)
        {
            if (data == null)
                data = File.ReadAllBytes(FileName);

            IFileFormat format = OpenFileFormat(FileName, data, InArchive, ArchiveHash, archiveNode);

            if (format is TreeNode)
                return (TreeNodeFile)format;
            else
                return null;
        }
        /// <summary>
        /// Gets the <see cref="IFileFormat"/> from a file or byte array. 
        /// </summary>
        /// <param name="FileName">The name of the file</param>
        /// <param name="data">The byte array of the data</param>
        /// <param name="InArchive">If the file is in an archive so it can be saved back</param>
        /// <param name="archiveNode">The node being replaced from an archive</param>
        /// <param name="Compressed">If the file is being compressed or not</param>
        /// <param name="CompType">The type of <see cref="CompressionType"/> being used</param>
        /// <returns></returns>
        public static IFileFormat OpenFileFormat(string FileName, byte[] data = null, bool InArchive = false,
            string ArchiveHash = "", TreeNode archiveNode = null, bool Compressed = false, CompressionType CompType = 0)
        {
            if (data == null)
                data = File.ReadAllBytes(FileName);

            Cursor.Current = Cursors.WaitCursor;
            FileReader fileReader = new FileReader(data);
            string Magic4 = fileReader.ReadMagic(0, 4);
            string Magic2 = fileReader.ReadMagic(0, 2);
            if (Magic4 == "Yaz0")
            {
                data = EveryFileExplorer.YAZ0.Decompress(data);
                return OpenFileFormat(FileName, data, InArchive, ArchiveHash, archiveNode, true, CompressionType.Yaz0);
            }
            if (Magic4 == "ZLIB")
            {
                data = FileReader.InflateZLIB(fileReader.getSection(64, data.Length - 64));
                return OpenFileFormat(FileName, data, InArchive, ArchiveHash, archiveNode, true, CompressionType.Zlib);
            }
            fileReader.Dispose();
            fileReader.Close();
            foreach (IFileFormat fileFormat in FileManager.GetFileFormats())
            {
                if (fileFormat.Magic == Magic4 || fileFormat.Magic == Magic2)
                {
                    fileFormat.CompressionType = CompType;
                    fileFormat.FileIsCompressed = Compressed;
                    fileFormat.Data = data;
                    fileFormat.Load();
                    fileFormat.FileName = Path.GetFileName(FileName);
                    fileFormat.FilePath = FileName;
                    fileFormat.IFileInfo = new IFileInfo();
                    fileFormat.IFileInfo.InArchive = InArchive;
                    fileFormat.IFileInfo.ArchiveHash = ArchiveHash;
                    fileFormat.FileIsCompressed = Compressed;
                    if (Compressed)
                        fileFormat.CompressionType = CompType;

                    if (fileFormat is TreeNode && archiveNode != null)
                    {
                        ((TreeNode)fileFormat).Text = archiveNode.Text;
                        ((TreeNode)fileFormat).ImageKey = archiveNode.ImageKey;
                        ((TreeNode)fileFormat).SelectedImageKey = archiveNode.SelectedImageKey;
                    }
                    return fileFormat;
                }
                if (fileFormat.Magic == string.Empty)
                {
                    foreach (string str3 in fileFormat.Extension)
                    {
                        if (str3.Remove(0, 1) == Path.GetExtension(FileName))
                        {
                            fileFormat.Data = data;
                            fileFormat.Load();
                            fileFormat.FileName = Path.GetFileName(FileName);
                            fileFormat.FilePath = FileName;
                            fileFormat.IFileInfo = new IFileInfo();
                            fileFormat.IFileInfo.InArchive = true;
                            fileFormat.IFileInfo.ArchiveHash = ArchiveHash;

                            if (fileFormat is TreeNode)
                            {
                                ((TreeNode)fileFormat).Text = archiveNode.Text;
                                ((TreeNode)fileFormat).ImageKey = archiveNode.ImageKey;
                                ((TreeNode)fileFormat).SelectedImageKey = archiveNode.SelectedImageKey;
                            }
                            return fileFormat;
                        }
                    }
                }
            }
            return null;
        }
    }

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

        public class ZSTD
        {
            public static byte[] Decompress(byte[] b)
            {
                using (var decompressor = new ZstdNet.Decompressor())
                {
                    return decompressor.Unwrap(b);
                }
            }
            public static byte[] Decompress(byte[] b, int MaxDecompressedSize)
            {
                using (var decompressor = new ZstdNet.Decompressor())
                {
                    return decompressor.Unwrap(b, MaxDecompressedSize);
                }
            }
            public static byte[] Compress(byte[] b)
            {
                using (var compressor = new ZstdNet.Compressor())
                {
                    return compressor.Wrap(b);
                }
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
        public void WriteUint32Offset(long target)
        {
            long pos = Position;
            using (TemporarySeek(target, SeekOrigin.Begin))
            {
                Write((uint)pos);
            }
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
