using Syroot.BinaryData;
using System.IO;
using System.IO.Compression;
using OpenTK;
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
        public static void SaveFileFormat(IFileFormat FileFormat, string FileName, bool EnableDialog = true)
        {
            //These always get created on loading a file,however not on creating a new file
            if (FileFormat.IFileInfo == null)
                throw new System.Exception("Make sure to impliment a IFileInfo instance if a format is being created!");

            Cursor.Current = Cursors.WaitCursor;
            FileFormat.FilePath = FileName;

            byte[] data = FileFormat.Save();
            FileFormat.IFileInfo.DecompressedSize = (uint)data.Length;

            byte[] FinalData = CompressFileFormat(data,
                FileFormat.IFileInfo.FileIsCompressed,
                FileFormat.IFileInfo.Alignment, 
                FileFormat.IFileInfo.CompressionType,
                FileName,
                EnableDialog);

            FileFormat.IFileInfo.CompressedSize = (uint)FinalData.Length;

            File.WriteAllBytes(FileName, FinalData);
            MessageBox.Show($"File has been saved to {FileName}");
            Cursor.Current = Cursors.Default;
        }

        public static void SaveFileFormat(byte[] data, bool FileIsCompressed, int Alignment,
            CompressionType CompressionType, string FileName, bool EnableDialog = true)
        {
            Cursor.Current = Cursors.WaitCursor;
            byte[] FinalData = CompressFileFormat(data, FileIsCompressed, Alignment, CompressionType, FileName, EnableDialog);
            File.WriteAllBytes(FileName, FinalData);
            MessageBox.Show($"File has been saved to {FileName}");
            Cursor.Current = Cursors.Default;
        }

        private static byte[] CompressFileFormat(byte[] data, bool FileIsCompressed, int Alignment,
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
                            return EveryFileExplorer.YAZ0.Compress(data, Runtime.Yaz0CompressionLevel, (uint)Alignment);
                        case CompressionType.Zstb:
                            return STLibraryCompression.ZSTD.Compress(data);
                        case CompressionType.Lz4:
                            return STLibraryCompression.Type_LZ4.Compress(data);
                        case CompressionType.Lz4f:
                            return STLibraryCompression.Type_LZ4F.Compress(data);
                        case CompressionType.Gzip:
                            return STLibraryCompression.GZIP.Compress(data);
                        case CompressionType.Zlib:
                            return STLibraryCompression.ZLIB.Compress(data, 2);
                        default:
                            MessageBox.Show($"Compression Type {CompressionType} not supported!!");
                            break;
                    }
                }
            }

            return data;
        }
    }

}
