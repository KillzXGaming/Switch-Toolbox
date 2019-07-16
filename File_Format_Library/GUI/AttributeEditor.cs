using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bfres.Structs;
using Syroot.NintenTools.NSW.Bfres.GFX;

namespace FirstPlugin
{
    public partial class AttributeEditor : Form
    {
        public AttributeEditor()
        {
            InitializeComponent();

            foreach (var type in Enum.GetValues(typeof(AttribFormat)).Cast<AttribFormat>())
                formatCB.Items.Add(type);
        }
        private FMDL ActiveFMDL;
        private FSHP ActiveFSHP;
        private FSHP.VertexAttribute ActiveAttribute;

        public void LoadObjects(FMDL fmdl)
        {
            ActiveFMDL = fmdl;

            objectList.Items.Clear();
            foreach (var shape in fmdl.shapes)
            {
                ListViewItem item = new ListViewItem();
                item.Text = shape.Text;
                foreach (var attribute in shape.vertexAttributes)
                {
                    item.SubItems.Add(attribute.Name);
                }

                objectList.Items.Add(shape.Text);
            }
            objectList.Items[0].Selected = true;
            objectList.Select();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (objectList.SelectedItems.Count > 0)
            {
                ActiveFSHP = ActiveFMDL.shapes[objectList.SelectedIndices[0]];

                attributeCB.Items.Clear();
                foreach (var attribute in ActiveFSHP.vertexAttributes)
                {
                    attributeCB.Items.Add(attribute.Name);
                }
                if (attributeCB.Items.Count > 0)
                    attributeCB.SelectedIndex = 0;
            }
        }

        private void AttributeEditor_Load(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void attributeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (attributeCB.SelectedIndex >= 0)
            {
                string SelectedText = attributeCB.GetItemText(attributeCB.SelectedItem);

                hintLabel.Text = $"Hint: {SetHintLabel(SelectedText)}";

                foreach (var attribute in ActiveFSHP.vertexAttributes)
                    if (SelectedText == attribute.Name)
                        ActiveAttribute = attribute;

                if (ActiveAttribute != null)
                    formatCB.SelectedItem = ActiveAttribute.Format;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (formatCB.SelectedIndex < 0)
                return;

            string SelectedText = attributeCB.GetItemText(attributeCB.SelectedItem);

            DialogResult dialogResult = MessageBox.Show($"Are you sure you want to remove all {SelectedText} from" +
                $" FMDL {ActiveFMDL}? )", "", MessageBoxButtons.YesNo);


            if (dialogResult == DialogResult.Yes)
            {
                foreach (var shape in ActiveFMDL.shapes)
                {
                    foreach (var att in shape.vertexAttributes)
                    {
                        if (att.Name == SelectedText)
                        {
                            shape.vertexAttributes.Remove(att);
                            shape.SaveVertexBuffer(shape.IsWiiU);
                            if (shape.IsWiiU)
                                BfresWiiU.ReadShapesVertices(shape, shape.ShapeU, shape.VertexBufferU, ActiveFMDL);
                            else
                                BfresSwitch.ReadShapesVertices(shape, shape.Shape, shape.VertexBuffer, ActiveFMDL);

                            attributeCB.Items.Remove(att.Name);

                            if (attributeCB.Items.Count > 0)
                                attributeCB.SelectedIndex = 0;
                            break;
                        }
                    }
                }
                LoadObjects(ActiveFMDL);
                ActiveFMDL.UpdateVertexData();
            }
        }
        private string SetHintLabel(string attributeName)
        {
            switch (attributeName)
            {
                case "_p0": return "Position";
                case "_n0": return "Normal";
                case "_c0": return "Color";
                case "_u0": return "UV";
                case "_u1": return "UV Layer";
                case "_u2": return "UV Layer 2";
                case "_t0": return "Tangent";
                case "_b0": return "Bitangent";
                case "_w0": return "Weight";
                case "_i0": return "Index";
                case "_w1": return "Weight (Additional 4)";
                case "_i1": return "Index (Additional 4)";
                default: return "";
            }
        }

        private void formatCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (attributeCB.SelectedIndex >= 0)
            {
                ActiveAttribute.Format = (AttribFormat)formatCB.SelectedItem;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ActiveFSHP.vertexAttributes.Remove(ActiveAttribute);
            attributeCB.Items.Remove(ActiveAttribute.Name);

            if (attributeCB.Items.Count > 0)
                attributeCB.SelectedIndex = 0;
        }
    }
}
