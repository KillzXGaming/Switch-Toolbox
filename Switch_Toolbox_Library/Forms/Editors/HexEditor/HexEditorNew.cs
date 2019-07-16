using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace Toolbox.Library.Forms.Old
{
    public partial class HexEditor : STUserControl
    {
        FindOptions _findOptions = new FindOptions();
        ByteViewer ByteViewer;

        public HexEditor()
        {
            InitializeComponent();

            ByteViewer = new ByteViewer();
            ByteViewer.BackColor = FormThemes.BaseTheme.FormBackColor;
            ByteViewer.ForeColor = FormThemes.BaseTheme.FormForeColor;

            ByteViewer.Dock = DockStyle.Fill;
            stPanel1.Controls.Add(ByteViewer);
        }

        public override void OnControlClosing()
        {
            Cleanup();
            ByteViewer.Dispose();
        }

        private void Cleanup()
        {
         
        }


        public void LoadData(byte[] data)
        {
            Cleanup();
            ByteViewer.SetBytes(data);
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
        
        }
    }
}
