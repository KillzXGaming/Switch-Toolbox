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
using Bfres.Structs;

namespace FirstPlugin.Forms
{
    public partial class BoneIndexList : STForm
    {
        public BoneIndexList(string Title)
        {
            Text = Title;
            InitializeComponent();
        }

        public void LoadMatrixToIndexIndices(IList<ushort> Indices, FSKL fskl)
        {
            if (Indices == null)
                return;

            foreach (ushort index in Indices)
            {
                listViewCustom1.Items.Add(fskl.bones[fskl.Node_Array[index]].Text).SubItems.Add(fskl.Node_Array[index].ToString());
            }
        }

        public void LoadIndices(IList<ushort> Indices, FSKL fskl)
        {
            if (Indices == null)
                return;

            foreach (ushort index in Indices)
            {
                listViewCustom1.Items.Add(fskl.bones[index].Text).SubItems.Add(index.ToString());
            }
        }

        public void LoadIndices(List<ushort> Indices, FSKL fskl)
        {
            if (Indices == null)
                return;

            foreach (ushort index in Indices)
            {
                listViewCustom1.Items.Add(fskl.bones[index].Text).SubItems.Add(index.ToString());
            }
        }
    }
}
