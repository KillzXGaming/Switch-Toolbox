using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    
    public class STToolStripSeparator : ToolStripSeparator
    {
        public STToolStripSeparator()
        {
            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;
        }
    }
    public class STToolStipMenuItem : ToolStripMenuItem
    {
        public STToolStipMenuItem()
        {
            LoadTheme();
        }

        private void LoadTheme()
        {
            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormForeColor;
        }

        public STToolStipMenuItem(string text)
        {
            LoadTheme();

            Text = text;
        }

        public STToolStipMenuItem(string text, Image image, EventHandler onClick)
        {
            LoadTheme();

            Text = text;
            Image = image;
            Click += new EventHandler(onClick);
        }

        public STToolStipMenuItem(string text, Image image, EventHandler onClick, Keys shortcutKeys)
        {
            LoadTheme();

            Text = text;
            Image = image;
            Click += new EventHandler(onClick);
            ShortcutKeys = shortcutKeys;
        }
    }
}
