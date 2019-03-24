using Syroot.BinaryData;
using System.IO;
using System.IO.Compression;
using OpenTK;
using K4os.Compression.LZ4.Streams;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace Switch_Toolbox.Library.IO
{
    public enum DataType
    {
        uint8,
        int8,
        uint16,
        int16,
        int32,
        uint32,
        int64,
        uint64,
    }



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
        public static void SaveFileFormat(IFileFormat FileFormat, string FileName, bool EnableDialog = true)
        {
            Cursor.Current = Cursors.WaitCursor;

            FileFormat.FilePath = FileName;

            SaveFileFormat(FileFormat.Save(), FileFormat.IFileInfo.FileIsCompressed, FileFormat.IFileInfo.Alignment,
                FileFormat.IFileInfo.CompressionType, FileName, EnableDialog);
        }

        public static void SaveFileFormat(byte[] data, bool FileIsCompressed, int Alignment,
            CompressionType CompressionType, string FileName, bool EnableDialog = true)
        {
            string extension = Path.GetExtension(FileName);

            if (extension == ".szs" || extension == ".sbfres")
            {
                FileIsCompressed = true;
                CompressionType = CompressionType.Yaz0;
            }
            if (extension == ".cmp")
            {
                FileIsCompressed = true;
                CompressionType = CompressionType.Lz4f;
            }

            if (EnableDialog && FileIsCompressed)
            {
                DialogResult save = MessageBox.Show($"Compress file with {CompressionType}?", "File Save", MessageBoxButtons.YesNo);

                if (save == DialogResult.Yes)
                {
                    switch (CompressionType)
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
                            MessageBox.Show($"Compression Type {CompressionType} not supported!!");
                            break;
                    }
                }
            }
            File.WriteAllBytes(FileName, data);
            MessageBox.Show($"File has been saved to {FileName}");
            Cursor.Current = Cursors.Default;
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
        public static TreeNode GetNodeFileFormat(string FileName, byte[] data = null, bool InArchive = false,
            TreeNode archiveNode = null, bool LeaveStreamOpen = false, bool Compressed = false, CompressionType CompType = 0)
        {
            IFileFormat format = OpenFileFormat(FileName, data, LeaveStreamOpen, InArchive, archiveNode);

            if (format is TreeNode)
                return (TreeNode)format;
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
        public static IFileFormat OpenFileFormat(string FileName, byte[] data = null, bool LeaveStreamOpen = false, bool InArchive = false,
            TreeNode archiveNode = null, bool Compressed = false, CompressionType CompType = 0)
        {
            FileReader fileReader;
            if (data != null)
                fileReader = new FileReader(data);
            else
                fileReader = new FileReader(FileName);

            if (fileReader.BaseStream.Length <= 4)
            {
                fileReader.Close();
                fileReader.Dispose();
                return null;
            }

            Cursor.Current = Cursors.WaitCursor;
            fileReader.ByteOrder = ByteOrder.BigEndian;
            uint MagicHex = fileReader.ReadUInt32();

            string Magic = fileReader.ReadMagic(0, 4);

            fileReader.Position = 0;

            if (Magic == "Yaz0")
            {
                if (data != null)
                    data = EveryFileExplorer.YAZ0.Decompress(data);
                else
                    data = EveryFileExplorer.YAZ0.Decompress(FileName);

                return OpenFileFormat(FileName, data, LeaveStreamOpen, InArchive, archiveNode, true, CompressionType.Yaz0);
            }
            if (Magic == "ZLIB")
            {
                if (data == null)
                    data = File.ReadAllBytes(FileName);

                data = STLibraryCompression.GZIP.Decompress(fileReader.getSection(64, data.Length - 64));
                return OpenFileFormat(FileName, data, LeaveStreamOpen, InArchive, archiveNode, true, CompressionType.Zlib);
            }
            if (MagicHex == 0x1f8b0808 && CompType == CompressionType.None)
            {
                if (data == null)
                    data = File.ReadAllBytes(FileName);

                data = STLibraryCompression.GZIP.Decompress(data);
                return OpenFileFormat(FileName, data, LeaveStreamOpen, InArchive, archiveNode, true, CompressionType.Gzip);
            }
            if (Path.GetExtension(FileName) == ".lz" && CompType == CompressionType.None)
            {
                if (data == null)
                    data = File.ReadAllBytes(FileName);

                data = STLibraryCompression.LZ77.Decompress(fileReader.getSection(16, data.Length - 16));
                return OpenFileFormat(FileName, data, LeaveStreamOpen, InArchive, archiveNode, true, CompressionType.Zlib);
            }
            if (Path.GetExtension(FileName) == ".cmp" && CompType == CompressionType.None)
            {
                if (data == null)
                    data = File.ReadAllBytes(FileName);

                fileReader.Position = 0;
                int OuSize = fileReader.ReadInt32();
                int InSize = data.Length - 4;
                data = STLibraryCompression.Type_LZ4F.Decompress(fileReader.getSection(4, InSize));

                return OpenFileFormat(FileName, data, InArchive, LeaveStreamOpen, archiveNode, true, CompressionType.Lz4f);
            }

            fileReader.Dispose();
            fileReader.Close();

            Stream stream;
            if (data != null)
                stream = new MemoryStream(data);
            else
                stream = File.OpenRead(FileName);

            foreach (IFileFormat fileFormat in FileManager.GetFileFormats())
            {
                //Set the file name so we can check it's extension in the identifier. 
                //Most is by magic but some can be extension or name.

                fileFormat.FileName = Path.GetFileName(FileName);
                fileFormat.IFileInfo = new IFileInfo();

                if (fileFormat.Identify(stream))
                {
                    return SetFileFormat(fileFormat, FileName, stream, LeaveStreamOpen, InArchive, archiveNode, Compressed, CompType);
                }
            }

            return null;
        }

        private static IFileFormat SetFileFormat(IFileFormat fileFormat, string FileName, Stream stream, bool LeaveStreamOpen = false, bool InArchive = false,
             TreeNode archiveNode = null, bool Compressed = false, CompressionType CompType = 0)
        {
            fileFormat.IFileInfo.CompressionType = CompType;
            fileFormat.IFileInfo.FileIsCompressed = Compressed;
            fileFormat.FileName = Path.GetFileName(FileName);
            fileFormat.FilePath = FileName;
            fileFormat.IFileInfo.InArchive = InArchive;
            fileFormat.IFileInfo.FileIsCompressed = Compressed;
            if (Compressed)
                fileFormat.IFileInfo.CompressionType = CompType;


            fileFormat.Load(stream);


            if (fileFormat is TreeNode)
            {
                if (archiveNode != null)
                {
                    ((TreeNode)fileFormat).Text = archiveNode.Text;
                    ((TreeNode)fileFormat).ImageKey = archiveNode.ImageKey;
                    ((TreeNode)fileFormat).SelectedImageKey = archiveNode.SelectedImageKey;
                }
            }
            //After file has been loaded and read, we'll dispose unless left open

            if (!LeaveStreamOpen)
            {
            //    stream.Close();
            //    stream.Dispose();
            }

            return fileFormat;
        }
    }
    public class STLibraryCompression
    {
        public static byte[] CompressFile(byte[] data, IFileFormat format)
        {
            int Alignment = 0;

            if (format.IFileInfo != null)
                Alignment = format.IFileInfo.Alignment;

            switch (format.IFileInfo.CompressionType)
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

        public class ZLIB
        {
            public static byte[] Decompress(byte[] b, uint DecompSize)
            {
                var output = new MemoryStream();
                using (var compressedStream = new MemoryStream(b))
                using (var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
                {
                    zipStream.CopyTo(output);
                    zipStream.Close();
                    output.Position = 0;
                    return output.ToArray();
                }
            }

            public static void CopyStream(System.IO.Stream input, System.IO.Stream output)
            {
                byte[] buffer = new byte[2000];
                int len;
                while ((len = input.Read(buffer, 0, 2000)) > 0)
                {
                    output.Write(buffer, 0, len);
                }
                output.Flush();
            }
        }

        public class LZ77
        {
            /// <summary>
            /// Decompresses LZ77-compressed data from the given input stream.
            /// </summary>
            /// <param name="input">The input stream to read from.</param>
            /// <returns>The decompressed data.</returns>
            public static byte[] Decompress(byte[] input)
            {
                BinaryReader reader = new BinaryReader(new MemoryStream(input));

                // Check LZ77 type.
             //   if (reader.ReadByte() != 0x10)
               //     throw new System.Exception("Input stream does not contain LZ77-compressed data.");

                // Read the size.
                int size = reader.ReadUInt16() | (reader.ReadByte() << 16);

                // Create output stream.
                MemoryStream output = new MemoryStream(size);

                // Begin decompression.
                while (output.Length < size)
                {
                    // Load flags for the next 8 blocks.
                    int flagByte = reader.ReadByte();

                    // Process the next 8 blocks.
                    for (int i = 0; i < 8; i++)
                    {
                        // Check if the block is compressed.
                        if ((flagByte & (0x80 >> i)) == 0)
                        {
                            // Uncompressed block; copy single byte.
                            output.WriteByte(reader.ReadByte());
                        }
                        else
                        {
                            // Compressed block; read block.
                            ushort block = reader.ReadUInt16();
                            // Get byte count.
                            int count = ((block >> 4) & 0xF) + 3;
                            // Get displacement.
                            int disp = ((block & 0xF) << 8) | ((block >> 8) & 0xFF);

                            // Save current position and copying position.
                            long outPos = output.Position;
                            long copyPos = output.Position - disp - 1;

                            // Copy all bytes.
                            for (int j = 0; j < count; j++)
                            {
                                // Read byte to be copied.
                                output.Position = copyPos++;
                                byte b = (byte)output.ReadByte();

                                // Write byte to be copied.
                                output.Position = outPos++;
                                output.WriteByte(b);
                            }
                        }

                        // If all data has been decompressed, stop.
                        if (output.Length >= size)
                        {
                            break;
                        }
                    }
                }

                output.Position = 0;
                return output.ToArray();
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
                var stream = new MemoryStream();
                using (var writer = new FileWriter(stream))
                {
                    writer.Write(data.Length);
                    byte[] buffer = LZ4.Frame.LZ4Frame.Compress(new MemoryStream(data), LZ4.Frame.LZ4MaxBlockSize.Auto, true, true, false, false, true);
                    writer.Write(buffer, 0, buffer.Length);
                }
                return stream.ToArray();
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


    public class FileExt
    {
        public static System.Drawing.Color[] ReadColors(int Count)
        {
            var colors = new System.Drawing.Color[Count];
            for (int i = 0; i < Count; i ++)
            {
                colors[i] = new System.Drawing.Color();
            }
            return colors;
        }

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
}
