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
    public partial class MuuntObjectList : MuuntEditorDocker
    {
        private MuuntEditor ParentEditor;

        public MuuntObjectList(MuuntEditor editor)
        {
            InitializeComponent();
            ParentEditor = editor;
        }

        public void LoadObjects(List<ObjectGroup> groups)
        {
            for (int i = 0; i < groups.Count; i++)
            {
                stComboBox1.Items.Add(groups[i].Name);
            }
        }
    }
}
