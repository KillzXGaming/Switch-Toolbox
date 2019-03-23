using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;

namespace FirstPlugin
{
    public partial class VertexAttributeDataList : Form
    {
        public VertexAttributeDataList()
        {
            InitializeComponent();
        }
        public void AddVector2(Vector2 value, uint VertexID)
        {
            ListViewItem item = new ListViewItem();
            item.Text = VertexID.ToString();
            item.SubItems.Add(value.X.ToString());
            item.SubItems.Add(value.Y.ToString());
            listViewCustom1.Items.Add(item);
        }
        public void AddVector3(Vector3 value, uint VertexID)
        {
            ListViewItem item = new ListViewItem();
            item.Text = VertexID.ToString();
            item.SubItems.Add(value.X.ToString());
            item.SubItems.Add(value.Y.ToString());
            item.SubItems.Add(value.Z.ToString());
            listViewCustom1.Items.Add(item);
        }
        public void AddVector4(Vector4 value, uint VertexID)
        {
            ListViewItem item = new ListViewItem();
            item.Text = VertexID.ToString();
            item.SubItems.Add(value.X.ToString());
            item.SubItems.Add(value.Y.ToString());
            item.SubItems.Add(value.Z.ToString());
            item.SubItems.Add(value.W.ToString());
            listViewCustom1.Items.Add(item);
        }
        public void AddColor(Vector4 value, uint VertexID)
        {
            Color SetColor = Color.White;

            int someIntX = (int)Math.Ceiling(value.X * 255);
            int someIntY = (int)Math.Ceiling(value.Y * 255);
            int someIntZ = (int)Math.Ceiling(value.Z * 255);
            int someIntW = (int)Math.Ceiling(value.W * 255);

            SetColor = Color.FromArgb(
        255,
        someIntX,
        someIntY,
        someIntZ
        );

            columnHeader2.Text = "R";
            columnHeader3.Text = "G";
            columnHeader4.Text = "B";
            columnHeader5.Text = "A";

            ListViewItem item = new ListViewItem();
            item.BackColor = SetColor;
            item.Text = VertexID.ToString();
            item.SubItems.Add(value.X.ToString());
            item.SubItems.Add(value.Y.ToString());
            item.SubItems.Add(value.Z.ToString());
            item.SubItems.Add(value.W.ToString());
            listViewCustom1.Items.Add(item);
        }
        public void AddBoneName(List<string> value, uint VertexID)
        {
            ListViewItem item = new ListViewItem();
            item.Text = VertexID.ToString();

            columnHeader2.Text = "Bone 1";
            columnHeader3.Text = "Bone 2";
            columnHeader4.Text = "Bone 3";
            columnHeader5.Text = "Bone 4";

            if (value.Count > 1)
                item.SubItems.Add(value[0].ToString());
            if (value.Count > 2)
                item.SubItems.Add(value[1].ToString());
            if (value.Count > 3)
                item.SubItems.Add(value[2].ToString());
            if (value.Count > 4)
                item.SubItems.Add(value[3].ToString());

            listViewCustom1.Items.Add(item);
        }
        public void AddWeights(List<float> value, uint VertexID)
        {
            ListViewItem item = new ListViewItem();
            item.Text = VertexID.ToString();

            if (value.Count > 1)
                item.SubItems.Add(value[0].ToString());
            if (value.Count > 2)
                item.SubItems.Add(value[1].ToString());
            if (value.Count > 3)
                item.SubItems.Add(value[2].ToString());
            if (value.Count > 4)
                item.SubItems.Add(value[3].ToString());

            listViewCustom1.Items.Add(item);
        }
    }
}
