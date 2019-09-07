using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using LayoutBXLYT.Cafe;

namespace LayoutBXLYT
{
    public partial class LayoutTextDocked : LayoutDocked
    {
        public EventHandler TextCompiled;

        TextEditor editor;
        public LayoutTextDocked()
        {
            InitializeComponent();

            editor = new TextEditor();
            editor.Dock = DockStyle.Fill;
            Controls.Add(editor);

            TextCompiled += CompileLayout;
        }

        public BFLYT GetLayout()
        {
            return activeLayout;
        }

        public void Reset()
        {
            editor.FillEditor("");
        }

        private BFLYT activeLayout;
        public void LoadLayout(BFLYT bflyt)
        {
            activeLayout = bflyt;
            editor.AddContextMenu("Convert to BFLYT", TextCompiled);
            editor.FillEditor(bflyt.ConvertToString());
        }

        private void CompileLayout(object sender, EventArgs e)
        {
            try
            {
                activeLayout.ConvertFromString(editor.GetText());
            }
            catch (Exception ex)
            {
                STErrorDialog.Show("Failed to convert BFLYT! ", "Text Converter", ex.ToString());
            }
        }
    }
}
