using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;

namespace Toolbox.Library.IO
{
    public class CompressionMenus
    {
        public static List<ToolStripMenuItem> GetMenuItems()
        {
            CompressionMenus menus = new CompressionMenus();
            return menus.MenuItems();
        }

        private List<ToolStripMenuItem> MenuItems()
        {
            var items = new List<ToolStripMenuItem>();
            items.Add(CreateMenu("Yaz0"));
            items.Add(CreateMenu("Gzip"));
            items.Add(CreateMenu("LZ77 (Wii Type 11)"));
            items.Add(CreateMenu("LZSS", false));
            items.Add(CreateMenu("lZMA"));
            items.Add(CreateMenu("lZ4"));
            items.Add(CreateMenu("lZ4F"));
            items.Add(CreateMenu("ZSTD"));
            items.Add(CreateMenu("ZSTD (Kirby)"));
            items.Add(CreateMenu("ZLIB"));
            items.Add(CreateMenu("ZLIB_GZ (Hyrule Warriors)"));
            return items;
        }

        private ToolStripMenuItem CreateMenu(string text, bool canCompress = true)
        {
            var item = new ToolStripMenuItem(text);
            item.DropDownItems.AddRange(FunctionItems(canCompress));
            return item;
        }

        private ToolStripItem[] FunctionItems(bool canCompress)
        {
            var items = new List<ToolStripItem>();
            items.Add(new ToolStripMenuItem("Decompress", null, Decompress));
            items.Add(new ToolStripMenuItem("Compress", null, Compress) { Enabled = canCompress });
            return items.ToArray();
        }

        private void Decompress(object sender, EventArgs e)
        {
            ToolStripMenuItem curMenu = sender as ToolStripMenuItem;
            ToolStrip toolStrip = curMenu.GetCurrentParent();

            var itemCheck = (curMenu.GetCurrentParent() as ToolStripDropDown).OwnerItem;

            SetToolStripFunctions(itemCheck.Text, false);
        }

        private void Compress(object sender, EventArgs e)
        {
            ToolStripMenuItem curMenu = sender as ToolStripMenuItem;
            ToolStrip toolStrip = curMenu.GetCurrentParent();

            var itemCheck = (curMenu.GetCurrentParent() as ToolStripDropDown).OwnerItem;

            SetToolStripFunctions(itemCheck.Text, true);
        }
        private void SetToolStripFunctions(string Name, bool Compress)
        {
            if (Name == "Yaz0")
                OpenFileForCompression(new Yaz0(), Compress);
            else if (Name == "Gzip")
                OpenFileForCompression(new Gzip(), Compress);
            else if (Name.Contains("LZSS"))
                OpenFileForCompression(new LZSS(), Compress);
            else if (Name == "lZMA")
                OpenFileForCompression(new LZMA(), Compress);
            else if (Name == "lZ4")
                OpenFileForCompression(new lz4(), Compress);
            else if (Name == "lZ4F")
                OpenFileForCompression(new LZ4F(), Compress);
            else if (Name == "ZSTD")
                OpenFileForCompression(new Zstb(), Compress);
            else if (Name == "ZSTD (Kirby)")
                OpenFileForCompression(new Zstb_Kirby(), Compress);
            else if (Name == "ZLIB")
                OpenFileForCompression(new Zlib(), Compress);
            else if (Name.Contains("ZLIB_GZ"))
                OpenFileForCompression(new ZlibGZ(), Compress);
            else if (Name == "LZ77 (Wii Type 11)")
                OpenFileForCompression(new LZ77(), Compress);

            else throw new Exception("Unimplimented Type! " + Name);
        }

        private void OpenFileForCompression(ICompressionFormat compressionFormat, bool Compress)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All files(*.*)|*.*";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                SaveFileForCompression(Compress, ofd.FileNames, compressionFormat);
            }
        }

        private void SaveFileForCompression(bool Compress, string[] fileNames, ICompressionFormat compressionFormat)
        {
            if (fileNames.Length == 0)
                return;

            string ext = Compress ? ".comp" : "";
            if (compressionFormat.Extension.Length > 0 && Compress)
                ext = compressionFormat.Extension[0].Replace("*", string.Empty);

            List<string> failedFiles = new List<string>();
            if (fileNames.Length > 1)
            {
                FolderSelectDialog ofd = new FolderSelectDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in fileNames)
                    {
                        string name = Path.GetFileName(file);
                        name = name.Count(c => c == '.') > 1 && !Compress ? name.Remove(name.LastIndexOf('.')) : name;
                        using (var data = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            try
                            {
                                Stream stream;
                                if (Compress)
                                    stream = compressionFormat.Compress(data);
                                else
                                {
                                    compressionFormat.Identify(data, file);
                                    stream = compressionFormat.Decompress(data);
                                }

                                if (stream != null)
                                {
                                    stream.ExportToFile($"{ofd.SelectedPath}/{name}{ext}");
                                    stream.Flush();
                                    stream.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                failedFiles.Add($"{file} \n\n {ex} \n\n");
                            }
                        }
                    }

                    if (failedFiles.Count > 0)
                    {
                        string action = Compress ? "compress" : "decompress";
                        STErrorDialog.Show($"Some files failed to {action}! See detail list of failed files.", "Switch Toolbox",
                            string.Join("\n", failedFiles.ToArray()));
                    }
                    else
                        MessageBox.Show("Files batched successfully!");
                }
            }
            else
            {
                SaveFileDialog sfd = new SaveFileDialog();
                string name = Path.GetFileName(fileNames[0]);
                sfd.FileName = name + ext;
                sfd.Filter = "All files(*.*)|*.*";

                Cursor.Current = Cursors.Default;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var data = new FileStream(fileNames[0], FileMode.Open, FileAccess.Read))
                        {
                            Stream stream;
                            if (Compress)
                                stream = compressionFormat.Compress(data);
                            else
                            {
                                compressionFormat.Identify(data, fileNames[0]);
                                stream = compressionFormat.Decompress(data);
                            }

                            if (stream != null)
                            {
                                stream.ExportToFile(sfd.FileName);
                                stream.Flush();
                                stream.Close();

                                MessageBox.Show($"File has been saved to {sfd.FileName}", "Save Notification");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string action = Compress ? "compress" : "decompress";
                        STErrorDialog.Show($"Failed to {action}! See details for info.", "Switch Toolbox", ex.ToString());
                    }
                }
            }
        }
    }
}
