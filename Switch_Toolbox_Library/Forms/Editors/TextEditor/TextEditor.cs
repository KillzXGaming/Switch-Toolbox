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

namespace Toolbox.Library.Forms
{
    public partial class TextEditor : UserControl
    {
        FindReplace findReplaceDialog;

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

                    scintilla1.Styles[Style.Xml.XmlStart].ForeColor = Color.FromArgb(86, 156, 214);
                    scintilla1.Styles[Style.Xml.XmlEnd].ForeColor = Color.FromArgb(86, 156, 214);
                    scintilla1.Styles[Style.Xml.Default].ForeColor = Color.FromArgb(214, 157, 133);
                    scintilla1.Styles[Style.Xml.Comment].ForeColor = Color.FromArgb(87, 166, 74);
                    scintilla1.Styles[Style.Xml.Number].ForeColor = Color.FromArgb(214, 157, 133);


                    scintilla1.Styles[Style.Xml.DoubleString].ForeColor = Color.FromArgb(180, 180, 180);
                    scintilla1.Styles[Style.Xml.SingleString].ForeColor = Color.FromArgb(180, 180, 180);
                    scintilla1.Styles[Style.Xml.Tag].ForeColor = Color.FromArgb(214, 157, 133);
                    scintilla1.Styles[Style.Xml.TagEnd].ForeColor = Color.FromArgb(86, 156, 214);
                    scintilla1.Styles[Style.Xml.TagUnknown].ForeColor = Color.FromArgb(86, 156, 214);
                    scintilla1.Styles[Style.Xml.Attribute].ForeColor = Color.FromArgb(86, 156, 214);
                    scintilla1.Styles[Style.Xml.AttributeUnknown].ForeColor = Color.FromArgb(86, 156, 214);
                    scintilla1.Styles[Style.Xml.CData].ForeColor = Color.FromArgb(214, 157, 133);
                }
            }
        }

        public static Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        public TextEditor()
        {
            InitializeComponent();

            findReplaceDialog = new FindReplace();
            findReplaceDialog.Scintilla = scintilla1;
            findReplaceDialog.FindAllResults += MyFindReplace_FindAllResults;
            findReplaceDialog.KeyPressed += MyFindReplace_KeyPressed;

            findAllResultsPanel1.Scintilla = scintilla1;
            findAllResultsPanel1.Visible = false;
            findAllResultsPanel1.BackColor = FormThemes.BaseTheme.TextEditorBackColor;
            findAllResultsPanel1.ForeColor = FormThemes.BaseTheme.TextForeColor;
            findAllResultsPanel1.Scintilla.SetWhitespaceBackColor(true, Color.FromArgb(50, 50, 50));



            FillEditor("");
        }
        private Color BACK_COLOR = Color.FromArgb(30, 30, 30);
        private Color FORE_COLOR = Color.White;

        public string GetText()
        {
            return scintilla1.Text;
        }

        public void FillEditor(byte[] Data)
        {
            FillEditor(Encoding.Default.GetString(Data));
        }

        public void FillEditor(string Text)
        {
            InitSyntaxColoring();

            scintilla1.Text = Text;
            scintilla1.CaretLineBackColor = Color.DarkGray;
            scintilla1.CaretForeColor = FormThemes.BaseTheme.FormForeColor;
            scintilla1.SetSelectionBackColor(true, Color.FromArgb(38, 79, 120));
            scintilla1.SetSelectionForeColor(true, FormThemes.BaseTheme.FormForeColor);

            scintilla1.SetWhitespaceBackColor(true, Color.FromArgb(50,50,50));

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

            scintilla1.Styles[Style.LineNumber].ForeColor = Color.DarkCyan;

            scintilla1.Lexer = Lexer.Cpp;

            // Configure the CPP (C#) lexer styles
        }
        private void UpdateLineNumbers(int startingAtLine)
        {
            // Starting at the specified line index, update each
            // subsequent line margin text with a hex line number.
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
    }
}
