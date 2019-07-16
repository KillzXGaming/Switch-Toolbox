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
using ResNX = Syroot.NintenTools.NSW.Bfres;
using ResBNTX = Syroot.NintenTools.NSW.Bntx;

namespace FirstPlugin
{
    public partial class UserDataEditor : UserControl
    {
        public UserDataEditor()
        {
            InitializeComponent();

            listViewCustom1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
        }

        ResDict<UserData> userData;
        IList<ResNX.UserData> userDataNX;

        UserData SelectedDataU;
        ResNX.UserData SelectedDataNX;

        bool IsWiiU = false;
        public void LoadUserData(IList<ResNX.UserData> UserDataList)
        {
            listViewCustom1.Items.Clear();

            userDataNX = UserDataList;
            foreach (var item in userDataNX)
                LoadUserData(item);
        }

        //Convert the user data from bntx
        public void LoadUserData(IList<ResBNTX.UserData> UserDataList)
        {
            userDataNX = new List<ResNX.UserData>();

            listViewCustom1.Items.Clear();

            foreach (var item in UserDataList)
            {
                var userData = new ResNX.UserData();

                userData.Name = item.Name;
                if (item.Type == ResBNTX.UserDataType.Byte)
                    userData.SetValue(item.GetValueByteArray());
                if (item.Type == ResBNTX.UserDataType.String)
                    userData.SetValue(item.GetValueStringArray());
                if (item.Type == ResBNTX.UserDataType.WString)
                    userData.SetValue(item.GetValueStringArray());
                if (item.Type == ResBNTX.UserDataType.Single)
                    userData.SetValue(item.GetValueSingleArray());
                if (item.Type == ResBNTX.UserDataType.Int32)
                    userData.SetValue(item.GetValueInt32Array());

                userDataNX.Add(userData);
                LoadUserData(userData);
            }
        }
        
        public void LoadUserData(ResDict<UserData> UserDataList)
        {
            listViewCustom1.Items.Clear();

            IsWiiU = true;

            userData = UserDataList;

            foreach (var item in userData.Values)
                LoadUserData(item);
        }

        private void LoadUserData(ResNX.UserData item)
        {
            ListViewItem listItem = new ListViewItem();
            listItem.Text = item.Name;
            listItem.SubItems.Add(item.Type.ToString());

            string value = "";

            switch (item.Type)
            {
                case ResNX.UserDataType.WString:
                case ResNX.UserDataType.String:
                    if (item.GetValueStringArray() != null)
                    {
                        foreach (var val in item.GetValueStringArray())
                            value += $" {val}";
                    }
                    break;
                case ResNX.UserDataType.Single:
                    if (item.GetValueSingleArray() != null)
                    {
                        foreach (var val in item.GetValueSingleArray())
                            value += $" {val}";
                    }
                    break;
                case ResNX.UserDataType.Int32:
                    if (item.GetValueInt32Array() != null)
                    {
                        foreach (var val in item.GetValueInt32Array())
                            value += $" {val}";
                    }
                    break;
                case ResNX.UserDataType.Byte:
                    if (item.GetValueByteArray() != null)
                    {
                        foreach (var val in item.GetValueByteArray())
                            value += $" {val}";
                    }
                    break;
            }
            listItem.SubItems.Add(value);

            listViewCustom1.Items.Add(listItem);
        }

        private void LoadUserData(UserData item)
        {
            ListViewItem listItem = new ListViewItem();
            listItem.Text = item.Name;
            listItem.SubItems.Add(item.Type.ToString());

            string value = "";

            switch (item.Type)
            {
                case UserDataType.WString:
                case UserDataType.String:
                    foreach (var val in item.GetValueStringArray())
                        value += $" {val}";
                    break;
                case UserDataType.Single:
                    foreach (var val in item.GetValueSingleArray())
                        value += $" {val}";
                    break;
                case UserDataType.Int32:
                    foreach (var val in item.GetValueInt32Array())
                        value += $" {val}";
                    break;
                case UserDataType.Byte:
                    foreach (var val in item.GetValueByteArray())
                        value += $" {val}";
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

                if (IsWiiU)
                    SelectedDataU = userData[listViewCustom1.SelectedIndices[0]];
                else
                    SelectedDataNX = userDataNX[listViewCustom1.SelectedIndices[0]];
            }
            else
            {
                SelectedDataU = null;
                SelectedDataNX = null;
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
            if (IsWiiU)
            {
                UserData userDataNew = new UserData();
                userDataNew.SetValue(new int[0]);
                SelectedDataU = userDataNew;
                bool IsEdited = EditData();

                if (IsEdited)
                {
                    userData.Add(SelectedDataU.Name, SelectedDataU);
                    LoadUserData(SelectedDataU);
                }
            }
            else
            {
                ResNX.UserData userDataNew = new ResNX.UserData();
                userDataNew.SetValue(new int[0]);
                SelectedDataNX = userDataNew;
                bool IsEdited = EditData();

                if (IsEdited)
                {
                    userDataNX.Add(SelectedDataNX);
                    LoadUserData(SelectedDataNX);
                }
            }
        }
        private bool EditData()
        {
            if (SelectedDataU != null)
            {
                UserDataParser parser = new UserDataParser();
                parser.UserDataName = SelectedDataU.Name;
                parser.Type = SelectedDataU.Type.ToString();

                switch (SelectedDataU.Type)
                {
                    case UserDataType.WString:
                    case UserDataType.String:
                        if (SelectedDataU.GetValueStringArray() != null)
                            parser.LoadValues(SelectedDataU.GetValueStringArray());
                        break;
                    case UserDataType.Single:
                        if (SelectedDataU.GetValueSingleArray() != null)
                            parser.LoadValues(SelectedDataU.GetValueSingleArray());
                        break;
                    case UserDataType.Int32:
                        if (SelectedDataU.GetValueInt32Array() != null)
                            parser.LoadValues(SelectedDataU.GetValueInt32Array());
                        break;
                    case UserDataType.Byte:
                        if (SelectedDataU.GetValueByteArray() != null)
                            parser.LoadValues(SelectedDataU.GetValueByteArray());
                        break;
                }
                if (parser.ShowDialog() == DialogResult.OK)
                {
                    SelectedDataU.Name = parser.UserDataName;

                    if (parser.Type == "Byte")
                        SelectedDataU.SetValue(parser.GetBytes());
                    if (parser.Type == "Single")
                        SelectedDataU.SetValue(parser.GetFloats());
                    if (parser.Type == "Int32")
                        SelectedDataU.SetValue(parser.GetInts());
                    if (parser.Type == "String")
                        SelectedDataU.SetValue(parser.GetStringASCII());
                    if (parser.Type == "WString")
                        SelectedDataU.SetValue(parser.GetStringUnicode(), true);

                    LoadUserData(userData);
                    return true;
                }
            }
            else if (SelectedDataNX != null)
            {
                UserDataParser parser = new UserDataParser();
                parser.UserDataName = SelectedDataNX.Name;
                parser.Type = SelectedDataNX.Type.ToString();

                switch (SelectedDataNX.Type)
                {
                    case ResNX.UserDataType.WString:
                    case ResNX.UserDataType.String:
                        if (SelectedDataNX.GetValueStringArray() != null)
                            parser.LoadValues(SelectedDataNX.GetValueStringArray());
                        break;
                    case ResNX.UserDataType.Single:
                        if (SelectedDataNX.GetValueSingleArray() != null)
                            parser.LoadValues(SelectedDataNX.GetValueSingleArray());
                        break;
                    case ResNX.UserDataType.Int32:
                        if (SelectedDataNX.GetValueInt32Array() != null)
                            parser.LoadValues(SelectedDataNX.GetValueInt32Array());
                        break;
                    case ResNX.UserDataType.Byte:
                        if (SelectedDataNX.GetValueByteArray() != null)
                            parser.LoadValues(SelectedDataNX.GetValueByteArray());
                        break;
                }
                if (parser.ShowDialog() == DialogResult.OK)
                {
                    SelectedDataNX.Name = parser.UserDataName;

                    if (parser.Type == "Byte")
                        SelectedDataNX.SetValue(parser.GetBytes());
                    if (parser.Type == "Single")
                        SelectedDataNX.SetValue(parser.GetFloats());
                    if (parser.Type == "Int32")
                        SelectedDataNX.SetValue(parser.GetInts());
                    if (parser.Type == "String")
                        SelectedDataNX.SetValue(parser.GetStringASCII());
                    if (parser.Type == "WString")
                        SelectedDataNX.SetValue(parser.GetStringUnicode());

                    LoadUserData(userDataNX);
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

                if (userData != null)
                    userData.RemoveAt(index);
                if (userDataNX != null)
                    userDataNX.RemoveAt(index);
            }
        }

        private void listViewCustom1_DoubleClick(object sender, EventArgs e)
        {
            EditData();
        }
    }
}
