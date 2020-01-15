using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SPICA.Formats.CtrH3D;

namespace FirstPlugin.CtrLibrary.Forms
{
    public partial class BCHUserDataEditor : UserControl
    {
        public BCHUserDataEditor()
        {
            InitializeComponent();

            listViewCustom1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        }

        H3DMetaDataValue SelectedData;

        H3DMetaData MetaData;

        bool IsWiiU = false;
        public void LoadUserData(H3DMetaData metaData)
        {
            listViewCustom1.Items.Clear();

            MetaData = metaData;

            if (metaData == null) return;

            foreach (var item in metaData)
                LoadUserData(item);
        }     

        private void LoadUserData(H3DMetaDataValue item)
        {
            ListViewItem listItem = new ListViewItem();
            listItem.Text = item.Name;
            listItem.SubItems.Add(item.Type.ToString());

            string value = "";

            switch (item.Type)
            {
                case H3DMetaDataType.ASCIIString:
                case H3DMetaDataType.UnicodeString:
                    foreach (var val in item.Values)
                        value += $" {val.ToString()}";
                    break;
                case H3DMetaDataType.Single:
                    foreach (float val in item.Values)
                        value += $" {val.ToString()}";
                    break;
                case H3DMetaDataType.Integer:
                    foreach (int val in item.Values)
                        value += $" {val.ToString()}";
                    break;
            }
            listItem.SubItems.Add(value);

            listViewCustom1.Items.Add(listItem);
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0)
            {
                btnScrolDown.Enabled = true;
                btnScrollUp.Enabled = true;
                btnEdit.Enabled = true;
                btnRemove.Enabled = true;

                SelectedData = MetaData[listViewCustom1.SelectedIndices[0]];
            }
            else
            {
                SelectedData = null;
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (MetaData == null) MetaData = new H3DMetaData();

            H3DMetaDataValue userDataNew = new H3DMetaDataValue();
            userDataNew.Type = H3DMetaDataType.Integer;
            userDataNew.SetValue(new int[0]);
            SelectedData = userDataNew;
            bool IsEdited = EditData();

            if (IsEdited)
            {
                MetaData.Add(SelectedData);
                LoadUserData(SelectedData);
            }
        }
        private bool EditData()
        {
            UserDataParser parser = new UserDataParser();
            parser.UserDataName = SelectedData.Name;
            parser.Type = SelectedData.Type.ToString();

            switch (SelectedData.Type)
            {
                case H3DMetaDataType.UnicodeString:
                case H3DMetaDataType.ASCIIString:
                    parser.LoadValues(SelectedData.GetValueStringArray());
                    break;
                case H3DMetaDataType.Single:
                    parser.LoadValues(SelectedData.GetValueFloatArray());
                    break;
                case H3DMetaDataType.Integer:
                    parser.LoadValues(SelectedData.GetValueIntArray());
                    break;
            }
            if (parser.ShowDialog() == DialogResult.OK)
            {
                SelectedData.Name = parser.UserDataName;

                if (parser.Type == "Single")
                    SelectedData.SetValue(parser.GetFloats());
                if (parser.Type == "Int32")
                    SelectedData.SetValue(parser.GetInts());
                if (parser.Type == "String")
                    SelectedData.SetValue(parser.GetStringASCII());
                if (parser.Type == "WString")
                    SelectedData.SetValue(parser.GetStringUnicode());
                return true;
            }

            return false;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (MetaData == null) return;

            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                int index = listViewCustom1.SelectedIndices[0];
                listViewCustom1.Items.RemoveAt(index);

                if (MetaData != null && index < MetaData.Count)
                    MetaData.Remove(MetaData[index]);
            }
        }

        private void listViewCustom1_DoubleClick(object sender, EventArgs e)
        {
            EditData();
        }
    }
}
