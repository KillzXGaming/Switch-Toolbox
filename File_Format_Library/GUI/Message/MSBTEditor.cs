using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class MSBTEditor : UserControl, IFIleEditor
    {
        public bool ShowPreviewText = true;
        public bool ShowLabels = true;

        public List<IFileFormat> GetFileFormats()
        {
            return new List<IFileFormat>() { activeMessageFile };
        }

        public MSBTEditor()
        {
            InitializeComponent();

            listViewCustom1.HeaderStyle = ColumnHeaderStyle.None;
            listViewCustom1.FullRowSelect = true;
            listViewCustom1.CanResizeList = false;

            hexEditor1.EnableMenuBar = false;
        }

        MSBT activeMessageFile;

        public void LoadMSBT(MSBT msbt)
        {
            listViewCustom1.Items.Clear();

            activeMessageFile = msbt;

            if (msbt.header.Text2 != null)
            {
                if (ShowLabels)
                {
                    foreach (var lbl in msbt.header.Label1.Labels)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = lbl.Name;
                        item.Tag = msbt.header.Text2.TextData[(int)lbl.Index];
                        listViewCustom1.Items.Add(item);
                    }
                    listViewCustom1.Sorting = SortOrder.Ascending;
                    listViewCustom1.Sort();
                }
                else
                {
                    foreach (var text in msbt.header.Text2.TextData)
                    {
                        ListViewItem item = new ListViewItem();
                        string listText = text.GetTextLabel(ShowPreviewText, msbt.header.StringEncoding);

                        if (listText.Length > 25)
                            listText = $"{listText.Substring(0, 25)}......";

                        item.Text = listText;
                        item.Tag = text;
                        listViewCustom1.Items.Add(item);
                    }
                }
            }
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (activeMessageFile == null)
                return;

            if (listViewCustom1.SelectedItems.Count > 0)
            {
                var item = listViewCustom1.SelectedItems[0];
                if (item.Tag is MSBT.StringEntry)
                {
                    var msbtString = (MSBT.StringEntry)item.Tag;

                    editTextTB.Text = msbtString.GetText(activeMessageFile.header.StringEncoding);
                    originalTextTB.Text = msbtString.GetText(activeMessageFile.header.StringEncoding);
                    hexEditor1.LoadData(msbtString.Data);
                }
            }
        }
    }
}
