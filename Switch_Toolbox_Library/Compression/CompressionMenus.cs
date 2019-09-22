using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            items.Add(new ToolStripMenuItem("Yaz0"));
            items.Add(new ToolStripMenuItem("Gzip"));
            items.Add(new ToolStripMenuItem("lZMA"));
            items.Add(new ToolStripMenuItem("lZ4"));
            items.Add(new ToolStripMenuItem("lZ4F"));
            items.Add(new ToolStripMenuItem("ZSTD"));
            items.Add(new ToolStripMenuItem("ZLIB"));
            items.Add(new ToolStripMenuItem("ZLIB_GZ (Hyrule Warriors)"));

            SetFunctions(items);
            return items;
        }
        private void SetFunctions(List<ToolStripMenuItem> items)
        {
            foreach (var item in items)
                item.DropDownItems.AddRange(FunctionItems());
        }
        private ToolStripItem[] FunctionItems()
        {
            var items = new List<ToolStripItem>();
            items.Add(new ToolStripMenuItem("Decompress", null, Decompress));
            items.Add(new ToolStripMenuItem("Compress", null, Compress));
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
            else if (Name == "lZMA")
                OpenFileForCompression(new LZMA(), Compress);
            else if (Name == "lZ4")
                OpenFileForCompression(new lz4(), Compress);
            else if (Name == "lZ4F")
                OpenFileForCompression(new LZ4F(), Compress);
            else if (Name == "ZSTD")
                OpenFileForCompression(new Zstb(), Compress);
            else if (Name == "ZLIB")
                OpenFileForCompression(new Zlib(), Compress);
            else if (Name.Contains("ZLIB_GZ"))
                OpenFileForCompression(new ZlibGZ(), Compress);
            else throw new Exception("Unimplimented Type! " + Name);
        }

        public void CompressData(ICompressionFormat CompressionFormat, Stream data)
        {
            try
            {
                SaveFileForCompression(true, data, CompressionFormat);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"File failed to compress with {CompressionFormat} compression! {ex.ToString()}");
            }
        }
        public void DecompressData(ICompressionFormat CompressionFormat, Stream data)
        {
            try
            {
                SaveFileForCompression(false, data, CompressionFormat);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"File not compressed with {CompressionFormat} compression! {ex.ToString()}");
            }
        }

        private void OpenFileForCompression(ICompressionFormat compressionFormat, bool Compress)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All files(*.*)|*.*";
            ofd.Multiselect = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                foreach (string file in ofd.FileNames)
                {
                    if (Compress)
                        CompressData(compressionFormat, File.OpenRead(ofd.FileName));
                    else
                        DecompressData(compressionFormat,  File.OpenRead(ofd.FileName));
                }
            }
        }

        private void SaveFileForCompression(bool Compress, Stream data, ICompressionFormat compressionFormat)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "All files(*.*)|*.*";

            Cursor.Current = Cursors.Default;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Stream stream;
                if (Compress)
                    stream = compressionFormat.Compress(data);
                else
                    stream = compressionFormat.Decompress(data);

                if (stream != null)
                {
                    stream.ExportToFile(sfd.FileName);
                    stream.Flush();
                    stream.Close();

                    MessageBox.Show($"File has been saved to {sfd.FileName}", "Save Notification");
                }
            }
        }
    }
}
