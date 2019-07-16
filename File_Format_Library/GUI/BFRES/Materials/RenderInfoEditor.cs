using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bfres.Structs;
using Toolbox.Library.Forms;
using Syroot.NintenTools.NSW.Bfres;


namespace FirstPlugin.Forms
{
    public partial class RenderInfoEditor : UserControl
    {
        public RenderInfoEditor()
        {
            InitializeComponent();

            renderInfoListView.ListViewItemSorter = new ListSorter();
        }

        FMAT material;
        BfresRenderInfo activeRenderInfo;

        public void InitializeRenderInfoList(FMAT mat)
        {
            material = mat;

            renderInfoListView.BeginUpdate();
            renderInfoListView.Items.Clear();

            foreach (var rnd in material.renderinfo)
                renderInfoListView.Items.Add(CreateRenderInfoItem(rnd));

            renderInfoListView.FullRowSelect = true;
            renderInfoListView.EndUpdate();
        }
        private ListViewItem CreateRenderInfoItem(BfresRenderInfo rnd)
        {
            ListViewItem item = new ListViewItem();
            item.Text = rnd.Name;

            string Value = "";
            switch (rnd.Type)
            {
                case RenderInfoType.Int32:
                    Value = string.Join(",", rnd.ValueInt);
                    break;
                case RenderInfoType.Single:
                    Value = string.Join(",", rnd.ValueFloat);
                    break;
                case RenderInfoType.String:
                    Value = string.Join(",", rnd.ValueString);
                    break;
            }
            item.SubItems.Add(Value);
            item.SubItems.Add(rnd.Type.ToString());
            return item;
        }


        private void btnAddRenderInfo_Click(object sender, EventArgs e)
        {
            BfresRenderInfo info = new BfresRenderInfo();
            activeRenderInfo = info;
            activeRenderInfo.Type = RenderInfoType.Int32;
            activeRenderInfo.ValueInt = new int[0];
            bool IsEdited = EditData();

            if (IsEdited)
            {
                material.renderinfo.Add(activeRenderInfo);
                InitializeRenderInfoList(material);
            }
        }
        private bool EditData()
        {
            if (activeRenderInfo == null)
                return false;

            RenderInfoDataEditor editor = new RenderInfoDataEditor();
            editor.RenderInfoName = activeRenderInfo.Name;
            editor.FormatType = activeRenderInfo.Type.ToString();
            editor.LoadPresets();

            switch (activeRenderInfo.Type)
            {
                case RenderInfoType.String:
                    editor.LoadValues(activeRenderInfo.ValueString);
                    break;
                case RenderInfoType.Single:
                    editor.LoadValues(activeRenderInfo.ValueFloat);
                    break;
                case RenderInfoType.Int32:
                    editor.LoadValues(activeRenderInfo.ValueInt);
                    break;
            }
            if (editor.ShowDialog() == DialogResult.OK)
            {
                activeRenderInfo.Name = editor.RenderInfoName;

                if (editor.FormatType == "Single")
                {
                    activeRenderInfo.Type = RenderInfoType.Single;
                    activeRenderInfo.ValueFloat = editor.GetFloats();
                }
                if (editor.FormatType == "Int32")
                {
                    activeRenderInfo.Type = RenderInfoType.Int32;
                    activeRenderInfo.ValueInt = editor.GetInts();
                }
                if (editor.FormatType == "String")
                {
                    activeRenderInfo.Type = RenderInfoType.String;
                    activeRenderInfo.ValueString = editor.GetStrings();
                }
                InitializeRenderInfoList(material);

                return true;
            }
            return false;
        }

        private void btnRemoveRenderInfo_Click(object sender, EventArgs e)
        {
            if (renderInfoListView.SelectedItems.Count > 0)
            {
               var result = MessageBox.Show($"Are you sure you want to remove {renderInfoListView.SelectedItems[0].Text}? This could potentially break things!",
               "Render Info Editor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.OK)
                {
                    material.renderinfo.RemoveAt(renderInfoListView.SelectedIndices[0]);
                    renderInfoListView.Items.RemoveAt(renderInfoListView.SelectedIndices[0]);
                }
            }
        }

        private void renderInfoListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (renderInfoListView.SelectedItems.Count > 0)
            {
                btnRemove.Enabled = true;
            }
            else
                btnRemove.Enabled = false;
        }

        private void renderInfoListView_DoubleClick(object sender, EventArgs e)
        {
            EditData();
        }

        private void renderInfoListView_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (renderInfoListView.SelectedItems.Count > 0)
            {
                btnScrolDown.Enabled = true;
                btnScrollUp.Enabled = true;
                btnEdit.Enabled = true;
                btnRemove.Enabled = true;

                foreach (var info in material.renderinfo)
                {
                    if (info.Name == renderInfoListView.SelectedItems[0].Text)
                        activeRenderInfo = info;
                }
            }
            else
            {
                activeRenderInfo = null;
                btnScrolDown.Enabled = false;
                btnScrollUp.Enabled = false;
                btnEdit.Enabled = false;
                btnRemove.Enabled = false;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditData();
        }
    }
}
