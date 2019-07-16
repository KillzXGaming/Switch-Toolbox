using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using Toolbox.Library.Forms;
using System.Windows.Forms;

namespace Toolbox.Library
{
    public class STConsole : RichTextBox
    {
        private static STConsole console;
        public static STConsole Instance
        {
            get
            {
                if (console == null || console.IsDisposed)
                {
                    console = new STConsole();
                    console.Text = ConsoleText;
                }

                return console;
            }
        }

        private static readonly uint MAX_TEXT_LINE = 1000;

        private static string consoleText;
        private static string ConsoleText
        {
            get
            {
                return consoleText;
            }
            set
            {
                //Reset the text depending on max size prevent memory issues
                if (value.Length > MAX_TEXT_LINE)
                    consoleText = "";
                else
                    consoleText = value;
            }
        }

        public STConsole()
        {
            BorderStyle = BorderStyle.None;
            Dock = DockStyle.Fill;

            BackColor = FormThemes.BaseTheme.ConsoleEditorBackColor;
            ForeColor = FormThemes.BaseTheme.TextForeColor;

            Multiline = true;
            ReadOnly = true;
        }

        public static void WriteLine(object line, int ColorKeyIndex)
        {
            if (ColorKeyIndex == 0)
                WriteLine(line.ToString(), Color.Red);
            else if (ColorKeyIndex == 1)
                WriteLine(line.ToString(), Color.Green);
            else
                WriteLine(line.ToString());
        }

        public static void WriteLine(string line, int ColorKeyIndex)
        {
            if (ColorKeyIndex == 0)
                WriteLine(line.ToString(), Color.Red);
            else if (ColorKeyIndex == 1)
                WriteLine(line.ToString(), Color.Green);
            else
                WriteLine(line.ToString());
        }

        public static void WriteLine(object line, Color? color = null)
        {
            WriteLine(line.ToString(), color);
        }

        public static void WriteLine(string Line, Color? color = null)
        {
            ConsoleText += $"{Line}\n";

            Color ForeColor = color ?? FormThemes.BaseTheme.TextForeColor;

            if (console != null && !console.Disposing && !console.IsDisposed)
                console.AppendTextData($"{Line}", ForeColor);
        }

        private void AppendTextData(string Text, Color color)
        {
            SelectionStart = TextLength;
            SelectionLength = 0;

            SelectionColor = color;
            AppendText($"{Text}\r\n");
            SelectionColor = ForeColor;
        }
    }
}
