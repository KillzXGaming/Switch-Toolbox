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
                throw new System.NotImplementedException("Make sure to impliment a IFileInfo instance if a format is being created!");

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

            DetailsLog += "\n" + SatisfyFileTables(FileFormat, FileName, FinalData,
                FileFormat.IFileInfo.DecompressedSize,
                FileFormat.IFileInfo.CompressedSize,
                FileFormat.IFileInfo.FileIsCompressed);

            File.WriteAllBytes(FileName, FinalData);
            MessageBox.Show($"File has been saved to {FileName}", "Save Notification");

         //   STSaveLogDialog.Show($"File has been saved to {FileName}", "Save Notification", DetailsLog);
            Cursor.Current = Cursors.Default;
        }

        private static string SatisfyFileTables(IFileFormat FileFormat, string FilePath, byte[] Data, uint DecompressedSize, uint CompressedSize, bool IsCompressed)
        {
            string FileLog = "";

            bool IsBotwFile = FilePath.IsSubPathOf(Runtime.BotwGamePath);
            bool IsTPHDFile = FilePath.IsSubPathOf(Runtime.TpGamePath);

            if (Runtime.ResourceTables.BotwTable && IsBotwFile)
            {
                string newFilePath = FilePath.Replace(Runtime.BotwGamePath, string.Empty).Remove(0, 1);
                newFilePath = newFilePath.Replace(".s", ".");
                newFilePath = newFilePath.Replace( @"\", "/");

                string RealExtension = Path.GetExtension(newFilePath).Replace(".s", ".");

                string RstbPath = Path.Combine($"{Runtime.BotwGamePath}",
                    "System", "Resource", "ResourceSizeTable.product.srsizetable");

                RSTB BotwResourceTable = new RSTB();
                BotwResourceTable.LoadFile(RstbPath);

                //Create a backup first if one doesn't exist
                if (!File.Exists($"{RstbPath}.backup"))
                {
                    STConsole.WriteLine($"RSTB File found. Creating backup...");

                    BotwResourceTable.Write(new FileWriter($"{RstbPath}.backup"));
                    File.WriteAllBytes($"{RstbPath}.backup", EveryFileExplorer.YAZ0.Compress($"{RstbPath}.backup"));
                }

                //Now apply the file table then save the table
                if (BotwResourceTable.IsInTable(newFilePath))
                {
                    FileLog += $"File found in resource table! {newFilePath}";
                    STConsole.WriteLine(FileLog, 1);
                }
                else
                {
                    FileLog += $"File NOT found in resource table! {newFilePath}";
                    STConsole.WriteLine(FileLog, 0);

                }

                BotwResourceTable.SetEntry(newFilePath, Data);
                BotwResourceTable.Write(new FileWriter(RstbPath));
                File.WriteAllBytes(RstbPath, EveryFileExplorer.YAZ0.Compress(RstbPath));
            }

            if (Runtime.ResourceTables.TpTable && IsTPHDFile)
            {
                string newFilePath = FilePath.Replace(Runtime.TpGamePath, string.Empty).Remove(0, 1);
                newFilePath = newFilePath.Replace(@"\", "/");

                //Read the compressed tables and set the new sizes if paths match
                TPFileSizeTable CompressedFileTbl = new TPFileSizeTable();
                CompressedFileTbl.ReadCompressedTable(new FileReader($"{Runtime.TpGamePath}/FileSizeList.txt"));
                if (CompressedFileTbl.IsInFileSizeList(newFilePath))
                {
                    STConsole.WriteLine("Found matching path in File Size List table! " + newFilePath, 1);
                    CompressedFileTbl.SetFileSizeEntry(newFilePath, CompressedSize);
                }
                else
                    STConsole.WriteLine("Failed to find path in File Size List table! " + newFilePath, 0);

                //Read decompressed file sizes
                TPFileSizeTable DecompressedFileTbl = new TPFileSizeTable();
                DecompressedFileTbl.ReadDecompressedTable(new FileReader($"{Runtime.TpGamePath}/DecompressedSizeList.txt"));

                newFilePath = $"./DVDRoot/{newFilePath}";
                newFilePath = newFilePath.Replace(".gz", string.Empty);

                //Write the decompressed file size
                if (DecompressedFileTbl.IsInDecompressedFileSizeList(newFilePath))
                {
                    STConsole.WriteLine("Found matching path in File Size List table! " + newFilePath, 1);
                    DecompressedFileTbl.SetDecompressedFileSizeEntry(newFilePath, CompressedSize, DecompressedSize);
                }
                else
                    STConsole.WriteLine("Failed to find path in File Size List table! " + newFilePath, 0);

                //Check if archive type
                bool IsArchive = false;
                foreach (var inter in FileFormat.GetType().GetInterfaces())
                {
                    if (inter == typeof(IArchiveFile))
                        IsArchive = true;
                }


                //Write all the file sizes in the archive if it's an archive type
                //Note this seems uneeded atm
                //Todo store both compressed and decompressed sizes in archive info
                if (IsArchive)
                {
                    IArchiveFile Archive = (IArchiveFile)FileFormat;
                    foreach (var file in Archive.Files)
                    {
                        uint DecompressedArchiveFileSize = (uint)file.FileData.Length;
                        string ArchiveFilePath = $"/DVDRoot/{file.FileName}";

                        if (DecompressedFileTbl.IsInDecompressedFileSizeList(ArchiveFilePath))
                        {
                            STConsole.WriteLine("Found matching path in File Size List table! " + ArchiveFilePath, 1);
                            DecompressedFileTbl.SetDecompressedFileSizeEntry(ArchiveFilePath, DecompressedArchiveFileSize, DecompressedArchiveFileSize);
                        }
                        else
                            STConsole.WriteLine("Failed to find path in File Size List table! " + ArchiveFilePath, 0);
                    }
                }

                CompressedFileTbl.WriteCompressedTable(new FileWriter($"{Runtime.TpGamePath}/FileSizeList.txt"));
                DecompressedFileTbl.WriteDecompressedTable(new FileWriter($"{Runtime.TpGamePath}/DecompressedSizeList.txt"));
            }

            return FileLog;
        }



        public static void SaveFileFormat(byte[] data, bool FileIsCompressed, int Alignment,
            CompressionType CompressionType, string FileName, bool EnableDialog = true, string DetailsLog = "")
        {
            Cursor.Current = Cursors.WaitCursor;
            byte[] FinalData = CompressFileFormat(data, FileIsCompressed, Alignment, CompressionType, FileName, EnableDialog);
            File.WriteAllBytes(FileName, FinalData);

            MessageBox.Show($"File has been saved to {FileName}", "Save Notification");

            //   STSaveLogDialog.Show($"File has been saved to {FileName}", "Save Notification", DetailsLog);
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
