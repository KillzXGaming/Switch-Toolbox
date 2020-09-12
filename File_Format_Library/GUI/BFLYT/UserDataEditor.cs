using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syroot.NintenTools.Bfres;

namespace LayoutBXLYT
{
    public partial class UserDataEditor : UserControl
    {
        public UserDataEditor()
        {
            InitializeComponent();

            listViewCustom1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        }

        BasePane activePane;
        UserData ActiveUserData;
        UserDataEntry SelectedEntry;

        public void Reset()
        {
            ActiveUserData = new UserData();
            SelectedEntry = null;
            listViewCustom1.Items.Clear();
        }

        public void LoadUserData(BasePane pane, UserData UserData)
        {
            activePane = pane;
            if (UserData != null)
            {
                listViewCustom1.Items.Clear();

                ActiveUserData = UserData;
                foreach (var item in ActiveUserData.Entries)
                    LoadUserData(item);
            }
        }

        private void LoadUserData(UserDataEntry item)
        {
            ListViewItem listItem = new ListViewItem();
            listItem.Text = item.Name;
            listItem.SubItems.Add(item.Type.ToString());

            string value = "";

            switch (item.Type)
            {
                case UserDataType.String:
                    if (item.GetString() != null)
                    {
                        value += $" {item.GetString()}";
                    }
                    break;
                case UserDataType.Float:
                    if (item.GetFloats() != null)
                    {
                        foreach (var val in item.GetFloats())
                            value += $" {val}";
                    }
                    break;
                case UserDataType.Int:
                    if (item.GetInts() != null)
                    {
                        foreach (var val in item.GetInts())
                            value += $" {val}";
                    }
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

                SelectedEntry = ActiveUserData.Entries[listViewCustom1.SelectedIndices[0]];
            }
            else
            {
                SelectedEntry = null;
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
            if (activePane == null) return;

            UserDataEntry userDataNew = ActiveUserData.CreateUserData();
            userDataNew.SetValue(new int[0]);
            SelectedEntry = userDataNew;
            bool IsEdited = EditData();
            if (IsEdited)
            {
                ActiveUserData.Edited = true;
                ActiveUserData.Entries.Add(userDataNew);
                LoadUserData(userDataNew);
            }
        }
        private bool EditData()
        {
            if (SelectedEntry != null)
            {
                UserDataParser parser = new UserDataParser();
                parser.UserDataName = SelectedEntry.Name;
                parser.Type = SelectedEntry.Type;

                switch (SelectedEntry.Type)
                {
                    case UserDataType.String:
                        if (SelectedEntry.GetString() != null)
                            parser.LoadValues(SelectedEntry.GetString());
                        break;
                    case UserDataType.Float:
                        if (SelectedEntry.GetString() != null)
                            parser.LoadValues(SelectedEntry.GetFloats());
                        break;
                    case UserDataType.Int:
                        if (SelectedEntry.GetInts() != null)
                            parser.LoadValues(SelectedEntry.GetInts());
                        break;
                }
                if (parser.ShowDialog() == DialogResult.OK)
                {
                    SelectedEntry.Name = parser.UserDataName;

                    if (parser.Type == UserDataType.Float)
                        SelectedEntry.SetValue(parser.GetFloats());
                    if (parser.Type == UserDataType.Int)
                        SelectedEntry.SetValue(parser.GetInts());
                    if (parser.Type == UserDataType.String)
                        SelectedEntry.SetValue(parser.GetStringASCII());

                    if (ActiveUserData == null) {
                        ActiveUserData = activePane.CreateUserData();
                    }

                    ActiveUserData.Edited = true;
                    LoadUserData(activePane, ActiveUserData);
                    return true;
                }
            }
            return false;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedIndices.Count > 0)
            {
                int index = listViewCustom1.SelectedIndices[0];
                listViewCustom1.Items.RemoveAt(index);

                if (ActiveUserData != null) {
                    ActiveUserData.Entries.RemoveAt(index);
                    ActiveUserData.Edited = true;
                }
            }
        }

        private void listViewCustom1_DoubleClick(object sender, EventArgs e)
        {
            EditData();
        }
    }
}
