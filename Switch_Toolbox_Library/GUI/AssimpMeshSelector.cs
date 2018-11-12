using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Assimp;
using Switch_Toolbox.Library.Rendering;
using OpenTK;

namespace Switch_Toolbox.Library
{
    public partial class AssimpMeshSelector : Form
    {
        public AssimpMeshSelector()
        {
            InitializeComponent();
        }
        AssimpData assimp;
        int index;
        public void LoadMeshes(AssimpData a, int i)
        {
            assimp = a;
            index = i;

            foreach (Mesh msh in assimp.scene.Meshes)
            {
                listView1.Items.Add(msh.Name);
            }
        }
        public GenericObject GetSelectedMesh()
        {
            assimp.processNode();
            foreach (Mesh msh in assimp.scene.Meshes)
            {
                if (msh.Name == listView1.SelectedItems[0].Text)
                    return assimp.CreateGenericObject(msh, index, Matrix4.Identity);
            }
            throw new Exception("This shouldn't happen???");
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void listView1_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(50, 50, 50)))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }
            using (SolidBrush foreBrush = new SolidBrush(Color.FromArgb(255, 255, 255)))
            {
                e.Graphics.DrawString(e.Header.Text, e.Font, foreBrush, e.Bounds);
            }
        }

        private void listView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
            if ((e.ItemIndex % 2) == 1)
            {
                e.Item.BackColor = Color.FromArgb(50, 50, 50);
                e.Item.UseItemStyleForSubItems = true;
            }
        }
    }
}
