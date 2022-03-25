using Syroot.BinaryData;
using System.IO;
using System;
using System.IO.Compression;
using OpenTK;
using System.Windows.Forms;

namespace Toolbox.Library.IO
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
        public static TreeNode GetNodeFileFormat(string FileName, bool InArchive = false,
             bool LeaveStreamOpen = false, bool Compressed = false, ICompressionFormat CompressionFormat = null)
        {
            IFileFormat format = OpenFileFormat(FileName, LeaveStreamOpen, InArchive);

            if (format is TreeNode)
                return (TreeNode)format;
            else
                return null;
        }

        public static Type CheckFileFormatType(string FileName, Type[] FileTypes, byte[] data = null)
        {
            //Todo. Create a compression list like IFileFormat to decompress via an Identiy method
            data = CheckCompression(FileName, data);

            Stream stream;
            if (data != null)
                stream = new MemoryStream(data);
            else
                stream = File.OpenRead(FileName);

            foreach (IFileFormat fileFormat in FileManager.GetFileFormats())
            {
                fileFormat.FileName = Path.GetFileName(FileName);

                foreach (Type type in FileTypes)
                {
                    if (fileFormat.Identify(stream) && fileFormat.GetType() == type)
                        return type;
                }
            }

            return typeof(IFileFormat);
        }

        public static IFileFormat OpenFileFormat(string FileName, Type[] FileTypes, byte[] data = null)
        {
            //Todo. Create a compression list like IFileFormat to decompress via an Identiy method
            data = CheckCompression(FileName, data);

            Stream stream;
            if (data != null)
                stream = new MemoryStream(data);
            else
                stream = File.OpenRead(FileName);

            foreach (IFileFormat fileFormat in FileManager.GetFileFormats())
            {
                fileFormat.FileName = Path.GetFileName(FileName);

                foreach (Type type in FileTypes)
                {
                    if (fileFormat.Identify(stream) && fileFormat.GetType() == type)
                    {
                        fileFormat.IFileInfo = new IFileInfo();
                        fileFormat.FileName = Path.GetFileName(FileName);
                        fileFormat.FilePath = FileName;
                        return OpenFileFormat(stream, FileName);
                    }
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
                    return OpenFileFormat(stream, FileName);
            }

            return null;
        }

        private static byte[] CheckCompression(string FileName, byte[] data)
        {
            if (data != null)
            {
                using (var reader = new FileReader(data))
                {
                  return  DecompressData(reader, FileName, data);
                }
            }
            else
            {
                using (var reader = new FileReader(FileName))
                {
                    return DecompressData(reader, FileName, data);
                }
            }
        }

        private static byte[] DecompressData(FileReader reader, string FileName, byte[] data)
        {
            reader.ByteOrder = ByteOrder.BigEndian;
            reader.Position = 0;
            uint MagicHex = reader.ReadUInt32();
            string Magic = reader.ReadMagic(0, 4);
            reader.Position = 0;

            if (Magic == "Yaz0")
            {
                if (data != null)
                    return EveryFileExplorer.YAZ0.Decompress(data);
                else
                    return EveryFileExplorer.YAZ0.Decompress(FileName);
            }
            if (MagicHex == 0x28B52FFD || MagicHex == 0xFD2FB528)
            {
                if (data != null)
                    return  Zstb.SDecompress(data);
                else
                    return Zstb.SDecompress(File.ReadAllBytes(FileName));
            }

            return data;
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
        public static IFileFormat OpenFileFormat(string FileName, bool LeaveStreamOpen = false, bool InArchive = false,
            bool Compressed = false, ICompressionFormat CompressionFormat = null, uint DecompressedSize = 0, uint CompressedSize = 0)
        {
            return OpenFileFormat(File.OpenRead(FileName), FileName,  LeaveStreamOpen, InArchive,
                  Compressed, CompressionFormat, DecompressedSize, CompressedSize);
        }

        /// <summary>
        /// Gets the <see cref="IFileFormat"/> from a file or byte array. 
        /// </summary>
        /// <param name="FileName">The name of the file</param>
        /// <param name="data">The byte array of the data</param>
        /// <param name="InArchive">If the file is in an archive so it can be saved back</param>
        /// <param name="Compressed">If the file is being compressed or not</param>
        /// <param name="CompressionFormat">The type of <see cref="ICompressionFormat"/> being used</param>
        /// <returns></returns>
        public static IFileFormat OpenFileFormat(Stream stream, string FileName, bool LeaveStreamOpen = false, bool InArchive = false,
            bool Compressed = false, ICompressionFormat CompressionFormat = null, long DecompressedSize = 0, long CompressedSize = 0)
        {
            if (!Compressed)
                DecompressedSize = stream.Length;

            long streamStartPos = stream.Position;

            if (stream.Length < 8) return null;

            //Check all supported compression formats and decompress. Then loop back
            if (!Compressed)
            {
                foreach (ICompressionFormat compressionFormat in FileManager.GetCompressionFormats())
                {
                    stream.Position = streamStartPos;
                    if (compressionFormat.Identify(stream, FileName))
                    {
                        stream.Position = streamStartPos;

                        Stream decompStream = compressionFormat.Decompress(stream);
                        stream.Close();

                        CompressedSize = decompStream.Length;

                        return OpenFileFormat(decompStream, FileName, LeaveStreamOpen, InArchive,
                                true, compressionFormat, DecompressedSize, CompressedSize);
                    }
                }
            }

            stream.Position = streamStartPos;
            foreach (IFileFormat fileFormat in FileManager.GetFileFormats())
            {
                //Set the file name so we can check it's extension in the identifier. 
                //Most is by magic but some can be extension or name.

                fileFormat.FileName = Path.GetFileName(FileName);

                if (fileFormat.Identify(stream))
                {
                    fileFormat.IFileInfo = new IFileInfo();
                    fileFormat.IFileInfo.DecompressedSize = (uint)DecompressedSize;
                    fileFormat.IFileInfo.CompressedSize = (uint)CompressedSize;
                    return SetFileFormat(fileFormat, FileName, stream, LeaveStreamOpen, InArchive, Compressed, CompressionFormat);
                }
            }

            stream.Close();

            return null;
        }

        private static IFileFormat SetFileFormat(IFileFormat fileFormat, string FileName, Stream stream, bool LeaveStreamOpen = false, bool InArchive = false,
             bool Compressed = false, ICompressionFormat FileCompression = null)
        {
            fileFormat.IFileInfo.FileCompression = FileCompression;
            fileFormat.IFileInfo.FileIsCompressed = Compressed;
            fileFormat.FileName = Path.GetFileName(FileName);
            fileFormat.FilePath = FileName;
            fileFormat.IFileInfo.InArchive = InArchive;
            fileFormat.IFileInfo.FileIsCompressed = Compressed;
            fileFormat.Load(stream);
            //After file has been loaded and read, we'll dispose unless left open

            if (fileFormat is ILeaveOpenOnLoad) {
                LeaveStreamOpen = true;
            }

            if (!LeaveStreamOpen)
            {
                stream.Dispose();
                GC.SuppressFinalize(stream);
            }

            return fileFormat;
        }
    }
}
