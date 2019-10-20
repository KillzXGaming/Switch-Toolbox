using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirstPlugin.MuuntEditor
{
    public partial class MuuntPropertiesEditor : MuuntEditorDocker
    {
        private MuuntEditor ParentEditor;

        public MuuntPropertiesEditor(MuuntEditor editor)
        {
            InitializeComponent();
            ParentEditor = editor;
        }
    }
}
