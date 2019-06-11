using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using Switch_Toolbox.Library.Forms;
using System.Windows.Forms;

namespace Switch_Toolbox.Library
{
    public class STConsole : RichTextBox
    {
        private static STConsole console;
        public static STConsole Instance { get { return console == null || console.IsDisposed ? console = new STConsole() : console; } }

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
            if (console == null)
                return;

            if (ColorKeyIndex == 0)
                WriteLine(line.ToString(), Color.Red);
            else if (ColorKeyIndex == 1)
                WriteLine(line.ToString(), Color.Green);
            else
                WriteLine(line.ToString());
        }

        public static void WriteLine(string line, int ColorKeyIndex)
        {
            if (console == null)
                return;

            if (ColorKeyIndex == 0)
                WriteLine(line.ToString(), Color.Red);
            else if (ColorKeyIndex == 1)
                WriteLine(line.ToString(), Color.Green);
            else
                WriteLine(line.ToString());
        }

        public static void WriteLine(object line, Color? color = null)
        {
            if (console == null)
                return;

            WriteLine(line.ToString(), color);
        }

        public static void WriteLine(string Line, Color? color = null)
        {
            if (console == null)
                return;

            Color ForeColor = color ?? FormThemes.BaseTheme.TextForeColor;

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
