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
        TextEditor editor;
        public LayoutTextDocked()
        {
            InitializeComponent();

            editor = new TextEditor();
            editor.Dock = DockStyle.Fill;
            Controls.Add(editor);
        }

        public void Reset()
        {
            editor.FillEditor("");
        }

        public void LoadLayout(BFLYT bflyt)
        {
            editor.FillEditor(bflyt.ConvertToString());
        }
    }
}
