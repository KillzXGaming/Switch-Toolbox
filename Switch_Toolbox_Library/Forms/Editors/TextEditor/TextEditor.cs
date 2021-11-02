using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNET;
using ScintillaNET_FindReplaceDialog;
using Toolbox.Library.IO;

namespace Toolbox.Library.Forms
{
    public partial class TextEditor : UserControl, IFIleEditor
    {
        public IFileFormat FileFormat;

        public List<IFileFormat> GetFileFormats()
        {
            return new List<IFileFormat>() { FileFormat };
        }

        public EventHandler TextEditorChanged;

        public void BeforeFileSaved() { }

        FindReplace findReplaceDialog;

        public static Encoding TextEncoding = Encoding.Default;

        private void ResetTypes()
        {
            IsYAML = false;
            IsXML = false;
        }

        private bool isXML;
        public bool IsXML
        {
            get { return isXML; }
            set
            {
                isXML = value;

                if (isXML == true)
                {
                    scintilla1.Lexer = Lexer.Xml;

                    UpdateFolderMarkings();
                    scintilla1.SetProperty("fold.html", "1");

                    scintilla1.Styles[Style.Xml.XmlStart].ForeColor = Color.FromArgb(128, 128, 128);
                    scintilla1.Styles[Style.Xml.XmlEnd].ForeColor = Color.FromArgb(128, 128, 128);
                    scintilla1.Styles[Style.Xml.Default].ForeColor = Color.FromArgb(180, 180, 180);
                    scintilla1.Styles[Style.Xml.Comment].ForeColor = Color.FromArgb(87, 166, 74);
                    scintilla1.Styles[Style.Xml.Number].ForeColor = Color.FromArgb(180, 180, 180);
                    scintilla1.Styles[Style.Xml.DoubleString].ForeColor = Color.FromArgb(180, 180, 180);
                    scintilla1.Styles[Style.Xml.SingleString].ForeColor = Color.FromArgb(180, 180, 180);
                    scintilla1.Styles[Style.Xml.Tag].ForeColor = Color.FromArgb(86, 156, 214);
                    scintilla1.Styles[Style.Xml.TagEnd].ForeColor = Color.FromArgb(128, 128, 128);
                    scintilla1.Styles[Style.Xml.TagUnknown].ForeColor = Color.FromArgb(128, 128, 128);
                    scintilla1.Styles[Style.Xml.Attribute].ForeColor = Color.FromArgb(146, 202, 244);
                    scintilla1.Styles[Style.Xml.AttributeUnknown].ForeColor = Color.FromArgb(146, 202, 244);
                    scintilla1.Styles[Style.Xml.CData].ForeColor = Color.FromArgb(214, 157, 133);

                    scintilla1.SetFoldMarginColor(true, BACK_COLOR);
                    scintilla1.SetFoldMarginHighlightColor(true, BACK_COLOR);
                }
            }
        }

        private bool isJson;
        public bool IsJson
        {
            get
            {
                return isJson;
            }
            set
            {
                isJson = true;

                if (isJson)
                {
                    scintilla1.Styles[Style.Json.Default].ForeColor = Color.Silver;
                    scintilla1.Styles[Style.Json.BlockComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
                    scintilla1.Styles[Style.Json.LineComment].ForeColor = Color.FromArgb(0, 128, 0); // Green
                    scintilla1.Styles[Style.Json.Number].ForeColor = Color.Olive;
                    scintilla1.Styles[Style.Json.PropertyName].ForeColor = Color.Blue;
                    scintilla1.Styles[Style.Json.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
                    scintilla1.Styles[Style.Json.StringEol].BackColor = Color.Pink;
                    scintilla1.Styles[Style.Json.Operator].ForeColor = Color.Purple;
                    scintilla1.Lexer = Lexer.Json;

                    UpdateFolderMarkings();

                    scintilla1.SetFoldMarginColor(true, BACK_COLOR);
                    scintilla1.SetFoldMarginHighlightColor(true, BACK_COLOR);
                }
            }
        }

        private bool isYAML;
        public bool IsYAML
        {
            get
            {
                return IsYAML;
            }
            set
            {
                isYAML = value;

                if (isYAML == true)
                {
                    scintilla1.Lexer = (Lexer)48;

                    scintilla1.Styles[Style.Json.Default].ForeColor = Color.FromArgb(214, 157, 133);
                    scintilla1.Styles[Style.Json.BlockComment].ForeColor = Color.FromArgb(87, 166, 74); // Green
                    scintilla1.Styles[Style.Json.LineComment].ForeColor = Color.FromArgb(87, 166, 74); // Green
                    scintilla1.Styles[Style.Json.Number].ForeColor = Color.FromArgb(214, 157, 133);
                    scintilla1.Styles[Style.Json.PropertyName].ForeColor = Color.FromArgb(214, 157, 133);
                    scintilla1.Styles[Style.Json.String].ForeColor = Color.FromArgb(86, 156, 214);
                    scintilla1.Styles[Style.Json.StringEol].BackColor = Color.FromArgb(214, 157, 133);
                    scintilla1.Styles[Style.Json.Operator].ForeColor = Color.FromArgb(180, 180, 180);
                    scintilla1.Styles[Style.Json.Keyword].ForeColor = Color.FromArgb(146, 202, 244);
                    scintilla1.Styles[Style.Json.EscapeSequence].ForeColor = Color.FromArgb(146, 202, 244);

                    UpdateFolderMarkings();

               //     scintilla1.SetKeywords(0, "!aamp !io True False");
                 //   scintilla1.SetKeywords(1, "!color !vec2 !vec3 !vec4 !str32 !str64 !str128 !str256 !obj");
                }
            }
        }

        public void UpdateFolderMarkings()
        {
            // Enable folding
            scintilla1.SetProperty("fold", "1");
            scintilla1.SetProperty("fold.compact", "1");

            scintilla1.Margins[0].Width = 20;

            // Use Margin 2 for fold markers
            scintilla1.Margins[2].Type = MarginType.Symbol;
            scintilla1.Margins[2].Mask = Marker.MaskFolders;
            scintilla1.Margins[2].Sensitive = true;
            scintilla1.Margins[2].Width = 20;

            // Reset folder markers
            for (int i = Marker.FolderEnd; i <= Marker.FolderOpen; i++)
            {
                scintilla1.Markers[i].SetForeColor(BACK_COLOR);
                scintilla1.Markers[i].SetBackColor(Color.Gray);
            }

            scintilla1.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            scintilla1.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            scintilla1.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            scintilla1.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            scintilla1.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            scintilla1.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            scintilla1.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            scintilla1.AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change;

            scintilla1.SetFoldMarginColor(true, BACK_COLOR);
            scintilla1.SetFoldMarginHighlightColor(true, BACK_COLOR);

        }

        public static Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        public List<Encoding> EncodingOptions = new List<Encoding>();

        public TextEditor()
        {
            InitializeComponent();

            Dock = DockStyle.Fill;

            findReplaceDialog = new FindReplace();
            findReplaceDialog.Scintilla = scintilla1;
            findReplaceDialog.FindAllResults += MyFindReplace_FindAllResults;
            findReplaceDialog.KeyPressed += MyFindReplace_KeyPressed;

            findAllResultsPanel1.Scintilla = scintilla1;
            findAllResultsPanel1.Visible = false;
            findAllResultsPanel1.BackColor = FormThemes.BaseTheme.TextEditorBackColor;
            findAllResultsPanel1.ForeColor = FormThemes.BaseTheme.TextForeColor;
            findAllResultsPanel1.Scintilla.SetWhitespaceBackColor(true, Color.FromArgb(50, 50, 50));

            EncodingOptions.Add(Encoding.Default);
            EncodingOptions.Add(Encoding.GetEncoding("shift_jis"));

            foreach (var encoding in EncodingOptions) {
                var toolstrip = new ToolStripMenuItem(encoding.EncodingName, null, encodingMenuClick);
                encodingToolStripMenuItem.DropDownItems.Add(toolstrip);
            }

            ReloadEncodingMenuToggle();

            FillEditor("");
        }

        private void ReloadEncodingMenuToggle()
        {
            foreach (ToolStripMenuItem encoding in encodingToolStripMenuItem.DropDownItems)
            {
                if (encoding.Text == TextEncoding.EncodingName)
                    encoding.Checked = true;
                else
                    encoding.Checked = false;
            }
        }

        private Color BACK_COLOR = Color.FromArgb(30, 30, 30);
        private Color FORE_COLOR = Color.White;

        public void AddContextMenu(STToolStripItem menu)
        {
            foreach (ToolStripItem item in stContextMenuStrip1.Items)
                if (item.Text == menu.Text)
                    return;

            stContextMenuStrip1.Items.Add(menu);
        }

        public void AddContextMenu(string text, EventHandler handler)
        {
            foreach (ToolStripItem item in stContextMenuStrip1.Items)
                if (item.Text == text)
                    return;

            stContextMenuStrip1.Items.Add(text, null, handler);
        }

        public void ClearContextMenus(string[] filter)
        {
            var menuItemsRemove = new List<ToolStripMenuItem>();
            foreach (ToolStripMenuItem item in stContextMenuStrip1.Items)
                if (!filter.Contains(item.Text))
                    menuItemsRemove.Add(item);

            foreach (var item in menuItemsRemove)
                stContextMenuStrip1.Items.Remove(item);
        }

        public string GetText()
        {
            return scintilla1.Text;
        }

        private byte[] Data;
        public void FillEditor(System.IO.Stream data)
        {
            Data = data.ToBytes();
            ReloadText();
        }

        public void FillEditor(byte[] data)
        {
            Data = data;
            ReloadText();
        }

        public void FillEditor(string Text)
        {
            if (scintilla1 == null || scintilla1.IsDisposed || scintilla1.Disposing)
                return;

            InitSyntaxColoring();

            scintilla1.Text = Text;
            scintilla1.CaretLineBackColor = Color.DarkGray;
            scintilla1.CaretForeColor = FormThemes.BaseTheme.FormForeColor;
            scintilla1.SetSelectionBackColor(true, Color.FromArgb(38, 79, 120));
            scintilla1.SetSelectionForeColor(true, FormThemes.BaseTheme.FormForeColor);

            scintilla1.SetWhitespaceBackColor(true, Color.FromArgb(50,50,50));
            scintilla1.WrapMode = WrapMode.Word;
            wordWrapToolStripMenuItem.Checked = true;

            scintilla1.Margins[0].Type = MarginType.Number;
            scintilla1.Margins[0].Width = 35;

        }

        private void InitSyntaxColoring() {
            scintilla1.StyleResetDefault();
            scintilla1.Styles[Style.Default].Font = "Consolas";
            scintilla1.Styles[Style.Default].Size = 10;
            scintilla1.Styles[Style.Default].BackColor = BACK_COLOR;
            scintilla1.Styles[Style.Default].ForeColor = FORE_COLOR;
            scintilla1.StyleClearAll();

            scintilla1.Styles[Style.LineNumber].BackColor = BACK_COLOR;
            scintilla1.Styles[Style.LineNumber].ForeColor = FORE_COLOR;
            scintilla1.Styles[Style.IndentGuide].ForeColor = FORE_COLOR;
            scintilla1.Styles[Style.IndentGuide].BackColor = BACK_COLOR;
            scintilla1.Styles[Style.FoldDisplayText].BackColor = BACK_COLOR;
            scintilla1.Styles[Style.Default].BackColor = BACK_COLOR;
            scintilla1.Styles[Style.LineNumber].ForeColor = Color.DarkCyan;
            scintilla1.Styles[Style.BraceBad].BackColor = BACK_COLOR;
            scintilla1.Styles[Style.BraceLight].BackColor = BACK_COLOR;
            scintilla1.Styles[Style.CallTip].BackColor = BACK_COLOR;

            scintilla1.Lexer = Lexer.Cpp;

            // Configure the CPP (C#) lexer styles
        }
        private void UpdateLineNumbers(int startingAtLine)
        {
            for (int i = startingAtLine; i < scintilla1.Lines.Count; i++)
            {
                scintilla1.Lines[i].MarginStyle = Style.LineNumber;
                scintilla1.Lines[i].MarginText = i.ToString();
            }
        }


        private void scintilla1_Click(object sender, EventArgs e)
        {

        }

        private int maxLineNumberCharLength;
        private void scintilla1_TextChanged(object sender, EventArgs e)
        {
            TextEditorChanged?.Invoke(sender, e);

            // Did the number of characters in the line number display change?
            // i.e. nnn VS nn, or nnnn VS nn, etc...
            var maxLineNumberCharLength = scintilla1.Lines.Count.ToString().Length;
            if (maxLineNumberCharLength == this.maxLineNumberCharLength)
                return;

            // Calculate the width required to display the last line number
            // and include some padding for good measure.
            const int padding = 4;
            scintilla1.Margins[0].Width = scintilla1.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
            this.maxLineNumberCharLength = maxLineNumberCharLength;

            if (scintilla1.Lines.Count > 0)
            {
                if (scintilla1.Lines[0].Text.Contains("<?xml")) {
                    IsXML = true;
                }
            }
        }

        private void scintilla1_Insert(object sender, ModificationEventArgs e)
        {
            // Only update line numbers if the number of lines changed
            if (e.LinesAdded != 0)
                UpdateLineNumbers(scintilla1.LineFromPosition(e.Position));
        }

        private void scintilla1_Delete(object sender, ModificationEventArgs e)
        {
            // Only update line numbers if the number of lines changed
            if (e.LinesAdded != 0)
                UpdateLineNumbers(scintilla1.LineFromPosition(e.Position));
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            findReplaceDialog.ShowFind();
        }

        private void MyFindReplace_FindAllResults(object sender, FindResultsEventArgs FindAllResults)
        {
            findAllResultsPanel1.Visible = true;

            // Pass on find results
            findAllResultsPanel1.UpdateFindAllResults(FindAllResults.FindReplace, FindAllResults.FindAllResults);
        }

        private void MyFindReplace_KeyPressed(object sender, KeyEventArgs e)
        {
            scintilla1_KeyDown(sender, e);
        }

        private void scintilla1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.F)
            {
                findReplaceDialog.ShowFind();

                e.SuppressKeyPress = true;
            }
            else if (e.Shift && e.KeyCode == Keys.F3)
            {
                findReplaceDialog.Window.FindPrevious();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.F3)
            {
                findReplaceDialog.Window.FindNext();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.H)
            {
                findReplaceDialog.ShowReplace();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.I)
            {
                findReplaceDialog.ShowIncrementalSearch();
                e.SuppressKeyPress = true;
            }
            else if (e.Control && e.KeyCode == Keys.G)
            {
                GoTo MyGoTo = new GoTo((Scintilla)sender);
                MyGoTo.ShowGoToDialog();
                e.SuppressKeyPress = true;
            }
        }

        private void scintilla1_Enter(object sender, EventArgs e)
        {
            findReplaceDialog.Scintilla = (Scintilla)sender;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "txt"; 

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(sfd.FileName, scintilla1.Text);
            }
        }

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (wordWrapToolStripMenuItem.Checked)
                scintilla1.WrapMode = WrapMode.Word;
            else
                scintilla1.WrapMode = WrapMode.None;
        }

        private void encodingMenuClick(object sender, EventArgs e)
        {
            var menu = (ToolStripMenuItem)sender;
            var index = encodingToolStripMenuItem.DropDownItems.IndexOf(menu);
            if (index != -1)
            {
                TextEncoding = EncodingOptions[index];
                ReloadEncodingMenuToggle();
                ReloadText();
            }
        }
        
        private void ReloadText() {
            FillEditor(TextEncoding.GetString(Data));
        }
    }
}
