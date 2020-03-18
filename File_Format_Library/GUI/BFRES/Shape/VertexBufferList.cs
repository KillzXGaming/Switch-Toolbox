using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syroot.NintenTools.NSW.Bfres;
using ResU = Syroot.NintenTools.Bfres;
using Bfres.Structs;
using Toolbox.Library.Rendering;

namespace FirstPlugin.Forms
{
    public partial class VertexBufferList : UserControl
    {
        public VertexBufferList()
        {
            InitializeComponent();

            btnView.Enabled = false;
            btnEdit.Enabled = false;
            attributeListView.FullRowSelect = true;
        }


        FSHP activeShape;

        public void LoadVertexBuffers(FSHP fshp, VertexBuffer vertexBuffer)
        {
            activeShape = fshp;

            attributeListView.Items.Clear();

            foreach (var att in vertexBuffer.Attributes)
            {
                ListViewItem item = new ListViewItem();
                item.Text = att.Name;
                item.SubItems.Add(GetAttributeHint(att.Name));
                item.SubItems.Add(att.Format.ToString());
                attributeListView.Items.Add(item);
            }
        }

        public void LoadVertexBuffers(FSHP fshp, ResU.VertexBuffer vertexBuffer)
        {
            activeShape = fshp;

            attributeListView.Items.Clear();

            foreach (var att in vertexBuffer.Attributes.Values)
            {
                ListViewItem item = new ListViewItem();
                item.Text = att.Name;
                item.SubItems.Add(GetAttributeHint(att.Name));
                item.SubItems.Add(att.Format.ToString());
                attributeListView.Items.Add(item);
            }
        }

        private int GetValueCount(string Name, int VertexSkinCount)
        {
            switch (Name)
            {
                case "_p0":
                case "_n0":
                    return 3;
                case "_u0":
                case "_u1":
                case "_u2":
                case "_u3":
                    return 2;
                case "_t0":
                case "_b0":
                case "_c0":
                    return 4;
                case "_w0":
                case "_i0":
                    return VertexSkinCount;
                default:
                    return 4;
            }
        }

        private string GetAttributeHint(string Name)
        {
            //The value used for multiple instances for the attribute
            var index = string.Concat(Name.ToArray().Reverse().TakeWhile(char.IsNumber).Reverse());

            if (Name == $"_p{index}")
                return $"Position {index}";
            if (Name == $"_n{index}")
                return $"Normal {index}";
            if (Name == $"_u{index}")
                return $"UV {index}";
            if (Name == $"_c{index}")
                return $"Color {index}";
            if (Name == $"_t{index}")
                return $"Tangent {index}";
            if (Name == $"_b{index}")
                return $"Bitangent {index}";
            if (Name == $"_w{index}")
                return $"Blend Weight {index}";
            if (Name == $"_i{index}")
                return $"Blend Index {index}";
            else
                return "";
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Removing a vertex buffer could potentially break the object. This cannot be undone. Are you sure you want to remove this?",
                "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var attriubte = attributeListView.SelectedItems[0];
                if (attriubte.Text == "_p0")
                {
                    MessageBox.Show("Cannot remove position attribute! You should remove a mesh instead or hide by materials.");
                    return;
                }

                for (int att = 0; att < activeShape.vertexAttributes.Count; att++)
                {
                    var CurrentAttribute = activeShape.vertexAttributes[att];

                    if (CurrentAttribute.Name == attriubte.Text)
                    {
                        var buffer = CurrentAttribute.BufferIndex;
                        activeShape.vertexAttributes.Remove(CurrentAttribute);

                        //Check if the index is no longer used for any attribute
                        if (!activeShape.vertexAttributes.Any(x => x.BufferIndex == buffer))
                        {
                            foreach (var attr in activeShape.vertexAttributes)
                                if (attr.BufferIndex > buffer)
                                    attr.BufferIndex -= 1;
                        }

                        attributeListView.Items.Remove(attriubte);
                    }
                }

                activeShape.SaveVertexBuffer(activeShape.GetResFileU() != null);
                activeShape.UpdateVertexData();
            }
        }

        private void stbtnView_Click(object sender, EventArgs e)
        {
            if (attributeListView.SelectedItems.Count <= 0)
                return;

            var attribute = attributeListView.SelectedItems[0];

            uint VertexID = 0;

            VertexAttributeDataList list = new VertexAttributeDataList();
            foreach (Vertex vtx in activeShape.vertices)
            {
                switch (attribute.Text)
                {
                    case "_p0":
                        list.AddVector3(vtx.pos, VertexID);
                        break;
                    case "_n0":
                        list.AddVector3(vtx.nrm, VertexID);
                        break;
                    case "_u0":
                        list.AddVector2(vtx.uv0, VertexID);
                        break;
                    case "_u1":
                        list.AddVector2(vtx.uv1, VertexID);
                        break;
                    case "_u2":
                        list.AddVector2(vtx.uv2, VertexID);
                        break;
                    case "_c0":
                        list.AddColor(vtx.col, VertexID);
                        break;
                    case "_t0":
                        list.AddVector4(vtx.tan, VertexID);
                        break;
                    case "_b0":
                        list.AddVector4(vtx.bitan, VertexID);
                        break;
                    case "_w0":
                        list.AddWeights(vtx.boneWeights, VertexID);
                        break;
                    case "_i0":
                        List<string> boneNames = new List<string>();
                        foreach (int id in vtx.boneIds)
                            boneNames.Add(activeShape.GetBoneNameFromIndex(activeShape.GetParentModel(), id));
                        list.AddBoneName(boneNames, vtx.boneIds, VertexID);
                        boneNames = null;
                        break;
                    case "_w1":
                        list.AddWeights(vtx.boneWeights, VertexID);
                        break;
                    case "_i1":
                        List<string> boneNames2 = new List<string>();
                        foreach (int id in vtx.boneIds)
                            boneNames2.Add(activeShape.GetBoneNameFromIndex(activeShape.GetParentModel(), id));
                        list.AddBoneName(boneNames2, vtx.boneIds, VertexID);
                        boneNames2 = null;
                        break;
                }
                VertexID++;
            }

            list.Show();
        }

        private void attributeListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (attributeListView.SelectedItems.Count > 0)
            {
                btnView.Enabled = true;
                btnEdit.Enabled = true;
            }
            else
            {
                btnView.Enabled = false;
                btnEdit.Enabled = false;
            }
        }

        private void UpdateList()
        {
            if (activeShape.IsWiiU)
                LoadVertexBuffers(activeShape, activeShape.VertexBufferU);
            else
                LoadVertexBuffers(activeShape, activeShape.VertexBuffer);
        }

        private void stButton1_Click(object sender, EventArgs e)
        {
            if (attributeListView.SelectedItems.Count <= 0)
                return;

            var attribute = attributeListView.SelectedItems[0];

            for (int att = 0; att < activeShape.vertexAttributes.Count; att++)
            {
                var CurrentAttribute = activeShape.vertexAttributes[att];

                if (CurrentAttribute.Name == attribute.Text)
                {

                    VertexBufferEncodeEditor encodeEditor = new VertexBufferEncodeEditor();
                    encodeEditor.LoadAttribute(CurrentAttribute);
                    if (encodeEditor.ShowDialog() == DialogResult.OK)
                    {
                        CurrentAttribute.Name = encodeEditor.activeAttribute.Name;
                        CurrentAttribute.Format = encodeEditor.activeAttribute.Format;
                        activeShape.SaveVertexBuffer(activeShape.GetResFileU() != null);
                        activeShape.UpdateVertexData();
                        UpdateList();
                    }
                }
            }
        }
    }
}
