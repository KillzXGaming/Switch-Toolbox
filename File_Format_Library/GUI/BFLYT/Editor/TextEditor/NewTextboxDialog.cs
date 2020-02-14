using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;
using FirstPlugin;
using Toolbox.Library.IO;

namespace LayoutBXLYT
{ 
    public partial class NewTextboxDialog : STForm
    {
        public NewTextboxDialog()
        {
            InitializeComponent();

            CanResize = false;
        }

        public string GetText()
        {
            return stTextBox1.Text;
        }

        public string GetFont()
        {
            return (string)stComboBox1.SelectedItem;
        }

        public void LoadFonts(List<string> fonts)
        {
            stComboBox1.Items.Clear();
            for (int i = 0; i < fonts.Count; i++)
                stComboBox1.Items.Add(fonts[i]);

            if (stComboBox1.Items.Count > 0)
                stComboBox1.SelectedIndex = 0;
            else
                btnOk.Enabled = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Utils.GetAllFilters(new Type[]
            { typeof(BXFNT), typeof(BXFNT) });

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                stComboBox1.Items.Add(Path.GetFileName(ofd.FileName));
                stComboBox1.SelectedIndex = stComboBox1.Items.Count - 1;
                btnOk.Enabled = true;

                var file = STFileLoader.OpenFileFormat(ofd.FileName);
                if (file is BXFNT)
                {

                }
            }
        }
    }
}
