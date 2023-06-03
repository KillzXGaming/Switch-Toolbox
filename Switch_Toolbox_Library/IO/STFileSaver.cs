using Syroot.BinaryData;
using System;
using System.IO;
using System.IO.Compression;
using OpenTK;
using System.Windows.Forms;
using Toolbox.Library.Forms;

namespace Toolbox.Library.IO
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

            string compressionLog = "";
            if (FileFormat.IFileInfo.FileIsCompressed || FileFormat.IFileInfo.InArchive
                || Path.GetExtension(FileName) == ".szs" || Path.GetExtension(FileName) == ".sbfres")
            {
                //Todo find more optmial way to handle memory with files in archives
                //Also make compression require streams
                var mem = new System.IO.MemoryStream();
                FileFormat.Save(mem);
                mem =  new System.IO.MemoryStream(mem.ToArray());

                FileFormat.IFileInfo.DecompressedSize = (uint)mem.Length;

                var finalStream = CompressFileFormat(
                    FileFormat.IFileInfo.FileCompression,
                    mem,
                    FileFormat.IFileInfo.FileIsCompressed,
                    FileFormat.IFileInfo.Alignment,
                    FileName,
                    EnableDialog);

                compressionLog = finalStream.Item2;
                Stream compressionStream = finalStream.Item1;

                FileFormat.IFileInfo.CompressedSize = (uint)compressionStream.Length;
                compressionStream.ExportToFile(FileName);

                DetailsLog += "\n" + SatisfyFileTables(FileFormat, FileName, compressionStream,
                                    FileFormat.IFileInfo.DecompressedSize,
                                    FileFormat.IFileInfo.CompressedSize,
                                    FileFormat.IFileInfo.FileIsCompressed);

                compressionStream.Flush();
                compressionStream.Close();
            }
            else
            {
                //Check if a stream is active and the file is beinng saved to the same opened file
                if (FileFormat is ISaveOpenedFileStream && FileFormat.FilePath == FileName && File.Exists(FileName))
                {
                    string savedPath = Path.GetDirectoryName(FileName);
                    string tempPath = Path.Combine(savedPath, "tempST.bin");

                    //Save a temporary file first to not disturb the opened file
                    using (var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        FileFormat.Save(fileStream);
                        FileFormat.Unload();

                        //After saving is done remove the existing file
                        File.Delete(FileName);

                        //Now move and rename our temp file to the new file path
                        File.Move(tempPath, FileName);

                        FileFormat.Load(File.OpenRead(FileName));

                        var activeForm = LibraryGUI.GetActiveForm();
                        if (activeForm != null && activeForm is ObjectEditor)
                            ((ObjectEditor)activeForm).ReloadArchiveFile(FileFormat);
                    }
                }
                else
                {
                    using (var fileStream = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                    {
                        FileFormat.Save(fileStream);
                    }
                }
            }

            if (EnableDialog)
            {
                if (compressionLog != string.Empty)
                    MessageBox.Show($"File has been saved to {FileName}. Compressed time: {compressionLog}", "Save Notification");
                else
                    MessageBox.Show($"File has been saved to {FileName}", "Save Notification");
            }

            //   STSaveLogDialog.Show($"File has been saved to {FileName}", "Save Notification", DetailsLog);
            Cursor.Current = Cursors.Default;
        }

        private static string SatisfyFileTables(IFileFormat FileFormat, string FilePath, Stream Data, uint DecompressedSize, uint CompressedSize, bool IsYaz0Compressed)
        {
            string FileLog = "";

            bool IsBotwFile = FilePath.IsSubPathOf(Runtime.BotwGamePath);
            bool IsTPHDFile = FilePath.IsSubPathOf(Runtime.TpGamePath);

            STConsole.WriteLine($"IsTPHDFile {IsTPHDFile}");

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

                BotwResourceTable.SetEntry(newFilePath, Data, IsYaz0Compressed);
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

                if (FileFormat == null)
                    return FileLog;

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
                /*   if (IsArchive)
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
                   }*/

                CompressedFileTbl.WriteCompressedTable(new FileWriter($"{Runtime.TpGamePath}/FileSizeList.txt"));
                DecompressedFileTbl.WriteDecompressedTable(new FileWriter($"{Runtime.TpGamePath}/DecompressedSizeList.txt"));
            }

            return FileLog;
        }



        public static void SaveFileFormat(byte[] data, bool FileIsCompressed, ICompressionFormat CompressionFormat,
            int Alignment, string FileName, bool EnableDialog = true, string DetailsLog = "")
        {
            uint DecompressedSize = (uint)data.Length;

            Cursor.Current = Cursors.WaitCursor;
            var compressedData = CompressFileFormat(CompressionFormat, new MemoryStream(data), FileIsCompressed, Alignment, FileName, EnableDialog);
            string compressionLog = compressedData.Item2;
            Stream FinalData = compressedData.Item1;

            FinalData.ExportToFile(FileName);

            uint CompressedSize = (uint)FinalData.Length;

            DetailsLog += "\n" + SatisfyFileTables(null, FileName, new MemoryStream(data),
                         DecompressedSize,
                         CompressedSize,
                         FileIsCompressed);

            FinalData.Flush();
            FinalData.Close();

            MessageBox.Show($"File has been saved to {FileName}", "Save Notification");

            //   STSaveLogDialog.Show($"File has been saved to {FileName}", "Save Notification", DetailsLog);
            Cursor.Current = Cursors.Default;
        }

        public static void BatchFileTable(string directory)
        {
            foreach (var file in Directory.GetDirectories(directory))
                BatchFileTable(file);

            foreach (var file in Directory.GetFiles(directory))
            {
                var fileStream = File.OpenRead(file);
                uint compLength = (uint)fileStream.Length;
                uint decompLength = (uint)fileStream.Length;
                bool yaz0 = false;
                foreach (ICompressionFormat compressionFormat in FileManager.GetCompressionFormats())
                {
                    fileStream.Position = 0;
                    if (compressionFormat.Identify(fileStream, file))
                    {
                        fileStream.Position = 0;
                        if (compressionFormat is Yaz0)
                            yaz0 = true;

                        var decomp = compressionFormat.Decompress(fileStream);
                        decompLength = (uint)decomp.Length;
                        decomp.Close();
                    }
                }

                SatisfyFileTables(null, file, File.OpenRead(file), compLength, decompLength, yaz0);
            }
        }

        private static Tuple<Stream, string> CompressFileFormat(ICompressionFormat compressionFormat, Stream data, bool FileIsCompressed, int Alignment,
              string FileName, bool EnableDialog = true)
        {
            string extension = Path.GetExtension(FileName);

            if (extension == ".szs" || extension == ".sbfres")
            {
                FileIsCompressed = true;
                compressionFormat = new Yaz0();
            }

            if (compressionFormat == null)
                return Tuple.Create(data, "");

            bool CompressFile = false;
            if (EnableDialog && FileIsCompressed)
            {
                if (Runtime.AlwaysCompressOnSave)
                    CompressFile = true;
                else
                {
                    DialogResult save = MessageBox.Show($"Compress file with {compressionFormat}?", "File Save", MessageBoxButtons.YesNo);
                    CompressFile = (save == DialogResult.Yes);
                }
            }
            else if (FileIsCompressed)
                CompressFile = true;

            Console.WriteLine($"FileIsCompressed {FileIsCompressed} CompressFile {CompressFile} CompressionType {compressionFormat}");

            if (CompressFile)
            {
                if (compressionFormat is Yaz0)
                    ((Yaz0)compressionFormat).Alignment = Alignment;

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                var comp = compressionFormat.Compress(data);
                sw.Stop();
                TimeSpan ts = sw.Elapsed;
                string message = string.Format("{0:D2}:{1:D2}:{2:D2}", ts.Minutes, ts.Seconds, ts.Milliseconds);
                Console.WriteLine($"Compression Time : {message}");
                return Tuple.Create(comp, message);
            }

            return Tuple.Create(data, "");
        }
    }

}
