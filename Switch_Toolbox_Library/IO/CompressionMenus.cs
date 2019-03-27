using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Switch_Toolbox.Library.IO
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
            items.Add(new ToolStripMenuItem("lZ4"));
            items.Add(new ToolStripMenuItem("lZ4F"));
            items.Add(new ToolStripMenuItem("ZSTD"));

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
                OpenFileForCompression(CompressionType.Yaz0, Compress);
            else if (Name == "Gzip")
                OpenFileForCompression(CompressionType.Gzip, Compress);
            else if (Name == "lZ4")
                OpenFileForCompression(CompressionType.Lz4, Compress);
            else if (Name == "lZ4F")
                OpenFileForCompression(CompressionType.Lz4f, Compress);
            else if (Name == "ZSTD")
                OpenFileForCompression(CompressionType.Zstb, Compress);
            else throw new Exception("Unimplimented Type! " + Name);
        }

        public void CompressData(CompressionType CompressionType, byte[] data)
        {
            switch (CompressionType)
            {
                case CompressionType.Yaz0:
                    SaveFileForCompression(EveryFileExplorer.YAZ0.Compress(data, Runtime.Yaz0CompressionLevel));
                    break;
                case CompressionType.Zlib:
                    break;
                case CompressionType.Gzip:
                    SaveFileForCompression(STLibraryCompression.GZIP.Compress(data));
                    break;
                case CompressionType.Zstb:
                    SaveFileForCompression(STLibraryCompression.ZSTD.Compress(data));
                    break;
                case CompressionType.Lz4f:
                    SaveFileForCompression(STLibraryCompression.Type_LZ4F.Compress(data));
                    break;
                case CompressionType.Lz4:
                    SaveFileForCompression(STLibraryCompression.Type_LZ4.Compress(data));
                    break;
            }
        }
        public void DecompressData(CompressionType CompressionType, byte[] data)
        {
            try
            {
                switch (CompressionType)
                {
                    case CompressionType.Yaz0:
                        SaveFileForCompression(EveryFileExplorer.YAZ0.Decompress(data));
                        break;
                    case CompressionType.Zlib:
                        break;
                    case CompressionType.Gzip:
                        SaveFileForCompression(STLibraryCompression.GZIP.Decompress(data));
                        break;
                    case CompressionType.Zstb:
                        SaveFileForCompression(STLibraryCompression.ZSTD.Decompress(data));
                        break;
                    case CompressionType.Lz4f:
                        using (var reader = new FileReader(data))
                        {
                            reader.Position = 0;
                            int OuSize = reader.ReadInt32();
                            int InSize = data.Length - 4;
                            SaveFileForCompression(STLibraryCompression.Type_LZ4F.Decompress(reader.getSection(4, InSize)));
                        }
                        break;
                    case CompressionType.Lz4:
                        SaveFileForCompression(STLibraryCompression.Type_LZ4.Decompress(data));
                        break;
                }
            }
            catch
            {
                MessageBox.Show($"File not compressed with {CompressionType} compression!");
            }
        }

        private void OpenFileForCompression(CompressionType compressionType, bool Compress)
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
                        CompressData(compressionType, File.ReadAllBytes(ofd.FileName));
                    else
                        DecompressData(compressionType, File.ReadAllBytes(ofd.FileName));
                }
            }
        }

        private void SaveFileForCompression(byte[] data)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "All files(*.*)|*.*";

            Cursor.Current = Cursors.Default;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                STFileSaver.SaveFileFormat(data, true, 0, CompressionType.None, sfd.FileName, false);
            }
        }
    }
}
