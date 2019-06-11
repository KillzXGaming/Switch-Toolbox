using Syroot.BinaryData;
using System;
using System.IO;
using System.IO.Compression;
using OpenTK;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;

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
        public static void SaveFileFormat(IFileFormat FileFormat, string FileName, bool EnableDialog = true, string DetailsLog = "")
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

            DetailsLog += "\n" + SatisfyFileTables(FileName, FinalData,
                FileFormat.IFileInfo.DecompressedSize,
                FileFormat.IFileInfo.CompressedSize,
                  FileFormat.IFileInfo.FileIsCompressed);

            File.WriteAllBytes(FileName, FinalData);
            STSaveLogDialog.Show($"File has been saved to {FileName}", "Save Notification", DetailsLog);
            Cursor.Current = Cursors.Default;
        }

        private static string SatisfyFileTables(string FilePath, byte[] Data, uint DecompressedSize, uint CompressedSize, bool IsCompressed)
        {
            string FileLog = "";

            bool IsBotwFile = FilePath.IsSubPathOf(Runtime.BotwGamePath);
            bool IsTPHDFile = FilePath.IsSubPathOf(Runtime.TpGamePath);

            if (Runtime.ResourceTables.BotwTable && IsBotwFile)
            {
                string newFilePath = FilePath.Replace(Runtime.BotwGamePath, string.Empty).Remove(0, 1);
                newFilePath = newFilePath.Replace(".s", ".");
                string RealExtension = Path.GetExtension(newFilePath).Replace(".s", ".");

                string RstbPath = Path.Combine($"{Runtime.BotwGamePath}",
                    "System", "Resource", "ResourceSizeTable.product.srsizetable");

                RSTB BotwResourceTable = new RSTB();
                BotwResourceTable.LoadFile(RstbPath);

                //Create a backup first if one doesn't exist
                if (!File.Exists($"{RstbPath}.backup"))
                {
                    BotwResourceTable.Write(new FileWriter($"{RstbPath}.backup"));
                    File.WriteAllBytes($"{RstbPath}.backup", EveryFileExplorer.YAZ0.Compress($"{RstbPath}.backup"));
                }

                //Now apply the file table then save the table
                if (BotwResourceTable.IsInTable(newFilePath))
                    FileLog += $"File found in resource table! {newFilePath}";
                else
                    FileLog += $"File NOT found in resource table! {newFilePath}";

                BotwResourceTable.SetEntry(FilePath, Data);
                BotwResourceTable.Write(new FileWriter(RstbPath));
                File.WriteAllBytes(RstbPath, EveryFileExplorer.YAZ0.Compress(RstbPath));
            }

            if (Runtime.ResourceTables.TpTable && IsTPHDFile)
            {

            }

            return FileLog;
        }



        public static void SaveFileFormat(byte[] data, bool FileIsCompressed, int Alignment,
            CompressionType CompressionType, string FileName, bool EnableDialog = true, string DetailsLog = "")
        {
            Cursor.Current = Cursors.WaitCursor;
            byte[] FinalData = CompressFileFormat(data, FileIsCompressed, Alignment, CompressionType, FileName, EnableDialog);
            File.WriteAllBytes(FileName, FinalData);

            STSaveLogDialog.Show($"File has been saved to {FileName}", "Save Notification", DetailsLog);
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
