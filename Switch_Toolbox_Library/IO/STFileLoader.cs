using Syroot.BinaryData;
using System.IO;
using System;
using System.IO.Compression;
using OpenTK;
using System.Windows.Forms;

namespace Switch_Toolbox.Library.IO
{
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

        public static IFileFormat OpenFileFormat(string FileName, Type[] FileTypes, byte[] data = null)
        {
            CheckCompression(FileName, data);

            Stream stream;
            if (data != null)
                stream = new MemoryStream(data);
            else
                stream = File.OpenRead(FileName);

            foreach (IFileFormat fileFormat in FileManager.GetFileFormats())
            {
                fileFormat.FileName = Path.GetFileName(FileName);
                fileFormat.IFileInfo = new IFileInfo();

                foreach (Type type in FileTypes)
                {
                    if (fileFormat.Identify(stream) && fileFormat.GetType() == type)
                        return OpenFileFormat(FileName, data);
                }
            }
            
            return null;
        }

        public static IFileFormat OpenFileFormat(string FileName, Type FileType, byte[] data = null)
        {
            CheckCompression(FileName, data);

            Stream stream;
            if (data != null)
                stream = new MemoryStream(data);
            else
                stream = File.OpenRead(FileName);

            foreach (IFileFormat fileFormat in FileManager.GetFileFormats())
            {
                if (fileFormat.GetType() == FileType)
                    return OpenFileFormat(FileName, data);
            }

            return null;
        }

        private static void CheckCompression(string FileName, byte[] data)
        {
            if (data != null)
            {
                using (var reader = new FileReader(data))
                {
                    DecompressData(reader, FileName, data);
                }
            }
            else
            {
                using (var reader = new FileReader(FileName))
                {
                    DecompressData(reader, FileName, data);
                }
            }
        }

        private static void DecompressData(FileReader reader, string FileName, byte[] data)
        {
            reader.ByteOrder = ByteOrder.BigEndian;
            uint MagicHex = reader.ReadUInt32();
            string Magic = reader.ReadMagic(0, 4);
            reader.Position = 0;

            if (Magic == "Yaz0")
            {
                if (data != null)
                    data = EveryFileExplorer.YAZ0.Decompress(data);
                else
                    data = EveryFileExplorer.YAZ0.Decompress(FileName);
            }
            if (MagicHex == 0x28B52FFD || MagicHex == 0xFD2FB528)
            {
                if (data != null)
                    data = STLibraryCompression.ZSTD.Decompress(data);
                else
                    data = STLibraryCompression.ZSTD.Decompress(File.ReadAllBytes(FileName));
            }
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
            TreeNode archiveNode = null, bool Compressed = false, CompressionType CompType = 0, uint DecompressedSize = 0, uint CompressedSize = 0)
        {
            uint DecompressedFileSize = 0;
            uint CompressedFileSize = 0;

            FileReader fileReader;
            if (data != null)
                fileReader = new FileReader(data);
            else
                fileReader = new FileReader(FileName);

            if (CompType == CompressionType.None)
                DecompressedFileSize = (uint)fileReader.BaseStream.Length;
            if (CompType != CompressionType.None)
                CompressedFileSize = (uint)fileReader.BaseStream.Length;

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
            ushort MagicHex2 = fileReader.ReadUInt16();

            if (Magic == "Yaz0")
            {
                if (data != null)
                    data = EveryFileExplorer.YAZ0.Decompress(data);
                else
                    data = EveryFileExplorer.YAZ0.Decompress(FileName);

                return OpenFileFormat(FileName, data, LeaveStreamOpen, InArchive, archiveNode, true,
                    CompressionType.Yaz0, DecompressedFileSize, CompressedFileSize);
            }
            if (MagicHex == 0x28B52FFD || MagicHex == 0xFD2FB528)
            {
                if (data != null)
                    data = STLibraryCompression.ZSTD.Decompress(fileReader.getSection(0, data.Length));
                else
                    data = STLibraryCompression.ZSTD.Decompress(File.ReadAllBytes(FileName));

                return OpenFileFormat(FileName, data, LeaveStreamOpen, InArchive, archiveNode, true,
                    CompressionType.Zstb, DecompressedFileSize, CompressedFileSize);
            }
            if (MagicHex2 == 0x789C || MagicHex2 == 0x78DA || Path.GetExtension(FileName) == ".z" && CompType == CompressionType.None)
            {
                if (data == null)
                    data = File.ReadAllBytes(FileName);

                data = STLibraryCompression.ZLIB.Decompress(data);
                return OpenFileFormat(FileName, data, LeaveStreamOpen, InArchive, archiveNode, true,
                    CompressionType.Zlib, DecompressedFileSize, CompressedFileSize);
            }
            if (Path.GetExtension(FileName) == ".carc" && CompType == CompressionType.None)
            {
                if (data == null)
                    data = File.ReadAllBytes(FileName);

                data = STLibraryCompression.ZLIB.Decompress(fileReader.getSection(0x10, data.Length - 0x10));
                return OpenFileFormat(FileName, data, LeaveStreamOpen, InArchive, archiveNode, true,
                    CompressionType.Zlib, DecompressedFileSize, CompressedFileSize);
            }
            if (Magic == "ZLIB")
            {
                if (data == null)
                    data = File.ReadAllBytes(FileName);

                data = STLibraryCompression.GZIP.Decompress(fileReader.getSection(64, data.Length - 64));
                return OpenFileFormat(FileName, data, LeaveStreamOpen, InArchive, archiveNode, true,
                    CompressionType.Zlib, DecompressedFileSize, CompressedFileSize);
            }
            if (MagicHex == 0x1f8b0808 || MagicHex2 == 0x1f8b && CompType == CompressionType.None)
            {
                if (data == null)
                    data = File.ReadAllBytes(FileName);

                data = STLibraryCompression.GZIP.Decompress(data);
                return OpenFileFormat(FileName, data, LeaveStreamOpen, InArchive, archiveNode, true,
                    CompressionType.Gzip, DecompressedFileSize, CompressedFileSize);
            }
            if (MagicHex == 0x184D2204)
            {
                if (data == null)
                    data = File.ReadAllBytes(FileName);

                data = STLibraryCompression.Type_LZ4.Decompress(data);
                return OpenFileFormat(FileName, data, LeaveStreamOpen, InArchive, archiveNode, true,
                    CompressionType.Lz4, DecompressedFileSize, CompressedFileSize);
            }
            if (Path.GetExtension(FileName) == ".lz" && CompType == CompressionType.None)
            {
                if (data == null)
                    data = File.ReadAllBytes(FileName);

                data = STLibraryCompression.LZ77.Decompress(fileReader.getSection(16, data.Length - 16));
                return OpenFileFormat(FileName, data, LeaveStreamOpen, InArchive, archiveNode, true,
                    CompressionType.Zlib, DecompressedFileSize, CompressedFileSize);
            }
            if (Path.GetExtension(FileName) == ".cmp" && CompType == CompressionType.None)
            {
                if (data == null)
                    data = File.ReadAllBytes(FileName);

                fileReader.Position = 0;
                int OuSize = fileReader.ReadInt32();
                int InSize = data.Length - 4;
                data = STLibraryCompression.Type_LZ4F.Decompress(fileReader.getSection(4, InSize));

                return OpenFileFormat(FileName, data, InArchive, LeaveStreamOpen, archiveNode, true,
                    CompressionType.Lz4f, DecompressedFileSize, CompressedFileSize);
            }

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
                    fileFormat.IFileInfo.DecompressedSize = DecompressedFileSize;
                    fileFormat.IFileInfo.CompressedSize = CompressedFileSize;
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
}
