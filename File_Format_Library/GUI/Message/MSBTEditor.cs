using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;
using Toolbox.Library.IO;

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

            editTextTB.TextChanged += TextChanged;

            Reload();
        }

        private void TextChanged(object sender, EventArgs args)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                var item = listViewCustom1.SelectedItems[0];
                if (item.Tag is MSBT.StringEntry)
                {
                    var msbtString = (MSBT.StringEntry)item.Tag;
                    msbtString.SetText(editTextTB.Text, activeMessageFile.header.StringEncoding); 
                    hexEditor1.LoadData(msbtString.Data);
                }
            }
        }

        private void Reload()
        {
            string[] fontSizes = { "8", "9", "10", "11", "12", "14", "16", "18",
                                   "20", "22", "24", "26", "28", "30", "36", "48", "72" };

            FontFamily[] fontFamilies = new InstalledFontCollection().Families;
            fontFamiltyCB.Items.Clear();
            fontFamiltyCB.Items.AddRange(fontFamilies.Select(o => o.Name).ToArray());
            fontFamiltyCB.SelectedText = Runtime.MessageEditor.FontFamily.Name;

            fontSizeCB.Items.Clear();
            fontSizeCB.Items.AddRange(fontSizes);
            fontSizeCB.SelectedText = Runtime.MessageEditor.FontSize.ToString();
        }

        MSBT activeMessageFile;

        public void LoadMSBT(MSBT msbt)
        {
            listViewCustom1.BeginUpdate();
            listViewCustom1.Items.Clear();

            activeMessageFile = msbt;

            if (msbt.header.Text2 != null)
            {
                if (ShowLabels && msbt.HasLabels)
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

            listViewCustom1.EndUpdate();
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
                    originalTextTB.Text = msbtString.GetOriginalText(activeMessageFile.header.StringEncoding);
                    hexEditor1.LoadData(msbtString.Data);
                }
            }
        }

        private void UpdateFont(Font font)
        {
            editTextTB.Font = font;
            originalTextTB.Font = font;
        }

        private void loadFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats|*.bfttf; *.ttf;*.otf|" +
                         "Binary Cafe True Type Font |*.bfttf|" +
                         "True Type Font |*.ttf|" +
                         "Open Type Font |*.otf|" +
                         "All files(*.*)|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (Utils.GetExtension(ofd.FileName) == ".bfttf")
                {
                    BFTTF bfttf = (BFTTF)STFileLoader.OpenFileFormat(ofd.FileName);
                    var font = bfttf.ToFont(editTextTB.Font.Size);
                    UpdateFont(font);
                    bfttf.Unload();
                }
                else if (Utils.GetExtension(ofd.FileName) == ".ttf" ||
                    Utils.GetExtension(ofd.FileName) == ".otf")
                {
                    PrivateFontCollection privateFonts = new PrivateFontCollection();
                    privateFonts.AddFontFile(ofd.FileName);
                    var font = privateFonts.Families[0];
                    UpdateFont(new Font(privateFonts.Families[0], editTextTB.Font.Size));
                }
            }
        }

        private void fontFamiltyCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadFont();
        }

        private void fontSizeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReloadFont();
        }

        private void ReloadFont()
        {
            float fontSize = 12;
            float.TryParse(fontSizeCB.Text, out fontSize);
            originalTextTB.Font = new Font(fontFamiltyCB.Text, fontSize);
            editTextTB.Font = new Font(fontFamiltyCB.Text, fontSize);

         //   Runtime.MessageEditor.FontFamily = fontFamiltyCB.SelectedText;
         //   Runtime.MessageEditor.FontSize = fontSize;
        }
    }
}
