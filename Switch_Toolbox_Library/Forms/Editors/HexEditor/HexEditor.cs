using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class HexEditor : STUserControl
    {
        FindOptions _findOptions = new FindOptions();
        public event EventHandler<byte[]> SaveData;

        public HexEditor()
        {
            InitializeComponent();

            hexBox1.BackColor = FormThemes.BaseTheme.FormBackColor;
            hexBox1.ForeColor = FormThemes.BaseTheme.FormForeColor;
            hexBox1.SelectionBackColor = FormThemes.BaseTheme.FormContextMenuSelectColor;
            hexBox1.SelectionForeColor = FormThemes.BaseTheme.FormForeColor;
            fixedBytesToolStripMenuItem.Checked = true;
            hexBox1.UseFixedBytesPerLine = true;
            hexBox1.ReadOnly = false;
        }

        public override void OnControlClosing()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            if (hexBox1.ByteProvider != null && !isStream)
            {
                hexBox1.ByteProvider.DeleteBytes(0, hexBox1.ByteProvider.Length);

                IDisposable byteProvider = hexBox1.ByteProvider as IDisposable;
                if (byteProvider != null)
                    byteProvider.Dispose();
                hexBox1.ByteProvider = null;
            }
        }

        public bool EnableMenuBar
        {
            set
            {
                if (value)
                    stContextMenuStrip1.Show();
                else
                    stContextMenuStrip1.Hide();
            }
            get
            {
                return stContextMenuStrip1.Visible;
            }
        }

        private bool isStream = false;

        public void LoadData(System.IO.Stream data)
        {
            isStream = true;

            IByteProvider provider = new DynamicFileByteProvider(data);
            hexBox1.ByteProvider = provider;
        }

        public void LoadData(byte[] data)
        {
            isStream = false;

            Cleanup();

            IByteProvider provider = new DynamicByteProvider(data);
            hexBox1.ByteProvider = provider;
            provider.Changed += ByteProvider_Changed;
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchHex searchHex = new SearchHex();
            searchHex.HexBox = hexBox1;
            searchHex.FindOptions = _findOptions;

            if (searchHex.ShowDialog() == DialogResult.OK)
            {
                FindOptions options = new FindOptions();
                options.MatchCase = searchHex.matchCase;
                options.Type = searchHex.findType;

                if (options.Type == FindType.Hex)
                    options.Hex = searchHex.findHex;
                else
                    options.Text = searchHex.findString;
            }
        }

        private void fixedBytesToolStripMenuItem_Click(object sender, EventArgs e) {
            hexBox1.UseFixedBytesPerLine = fixedBytesToolStripMenuItem.Checked;
            hexBox1.Refresh();
        }

        private void ByteProvider_Changed(object sender, EventArgs e)
        {
            var provider = hexBox1.ByteProvider;
            if (provider != null)
            {
                int length = (int)provider.Length;
                var data = new byte[length];
                for (var i = 0; i < length; ++i)
                {
                    data[i] = hexBox1.ByteProvider.ReadByte(i);
                }
                SaveData?.Invoke(this, data);
            }
        }
    }
}
