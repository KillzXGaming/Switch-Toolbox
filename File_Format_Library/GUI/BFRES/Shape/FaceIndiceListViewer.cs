using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class FaceIndiceListViewer : STForm
    {
        public FaceIndiceListViewer()
        {
            InitializeComponent();

            listViewCustom1.FullRowSelect = true;
            listViewCustom1.HeaderStyle = ColumnHeaderStyle.None;
        }

        public void LoadIndices(List<int> Indices)
        {
            listViewCustom1.Items.Clear();
            foreach (int face in Indices)
                listViewCustom1.Items.Add(face.ToString());
        }
    }
}
