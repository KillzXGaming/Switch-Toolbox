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
            activeMessageFile = msbt;

            if (msbt.header.Text2 != null)
            {
                foreach (var text in msbt.header.Text2.TextData)
                {
                    string listText = text.GetTextLabel(ShowPreviewText, msbt.header.StringEncoding);

                    if (listText.Length > 25)
                        listText = $"{listText.Substring(0, 25)}......";

                    listViewCustom1.Items.Add(listText);
                }
            }
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (activeMessageFile == null)
                return;

            if (listViewCustom1.SelectedItems.Count > 0)
            {
                int index = listViewCustom1.SelectedIndices[0];

                var textSection = activeMessageFile.header.Text2;
                if (textSection != null)
                {
                    editTextTB.Text = textSection.TextData[index].GetText(activeMessageFile.header.StringEncoding);
                    originalTextTB.Text = textSection.OriginalTextData[index].GetText(activeMessageFile.header.StringEncoding);
                    hexEditor1.LoadData(textSection.TextData[index].Data);
                }
            }
        }
    }
}
