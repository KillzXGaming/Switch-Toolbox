using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class FileTableViewTPHD : STUserControl
    {
        public FileTableViewTPHD()
        {
            InitializeComponent();
        }

        private TPFileSizeTable FileTable;
        private bool IsDecompressTable;
        public void LoadTable(TPFileSizeTable FileSizeTable, bool isDecompressTable)
        {
            FileTable = FileSizeTable;
            IsDecompressTable = isDecompressTable;

            listViewCustom1.BeginUpdate();
            if (IsDecompressTable)
            {
                foreach (var file in FileSizeTable.DecompressedFileSizes)
                {
                    var entry = file.Value;
                    listViewCustom1.Items.Add(file.Key).SubItems.AddRange(new ListViewItem.ListViewSubItem[]
                    {
                           new ListViewItem.ListViewSubItem(){Text= entry.CompressedSize.ToString() },
                           new ListViewItem.ListViewSubItem(){Text= entry.DecompressedSize.ToString() },
                           new ListViewItem.ListViewSubItem(){Text= entry.Precentage.ToString() },
                    });
                }
            }
            else
            {
                foreach (var file in FileSizeTable.FileSizes)
                {
                    listViewCustom1.Items.Add(file.Key).SubItems.Add(file.Value.ToString());
                }
            }
            listViewCustom1.EndUpdate();
        }
    }
}
