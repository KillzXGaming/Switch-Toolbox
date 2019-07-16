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
using Toolbox.Library;
using System.Reflection;

namespace FirstPlugin
{
    public partial class RenderInfoDataEditor : STForm
    {
        public RenderInfoDataEditor()
        {
            InitializeComponent();

            valueTB.BackColor = FormThemes.BaseTheme.FormBackColor;
            valueTB.ForeColor = FormThemes.BaseTheme.FormForeColor;

            typeCB.Items.Add("String");
            typeCB.Items.Add("Single");
            typeCB.Items.Add("Int32");
        }

        public string RenderInfoName
        {
            set {
                nameTB.Text = value;
            }
            get {
                return nameTB.Text;
            }
        }

        public string FormatType
        {
            set
            {
                typeCB.SelectedItem = value;
            }
            get
            {
                return typeCB.GetItemText(typeCB.SelectedItem);
            }
        }

        public void LoadValues(string[] strings)
        {
            foreach (var str in strings)
                valueTB.Text += $"{str}\n";
        }
        public void LoadValues(float[] floats)
        {
            foreach (var str in floats)
                valueTB.Text += $"{str}\n";
        }
        public void LoadValues(int[] ints)
        {
            foreach (var str in ints)
                valueTB.Text += $"{str}\n";
        }
        public void LoadValues(byte[] bytes)
        {
            foreach (var str in bytes)
                valueTB.Text += $"{str}\n";
        }

        public void LoadPresets()
        {
            Type enumType;
            FieldInfo field;
            MemberInfo member;
            object enumValue;
            string[] arrEnumNames;
            MemberInfo[] members;

            if (Runtime.activeGame == Runtime.ActiveGame.SMO)
                members = typeof(RenderInfoEnums.SMO).GetMembers();
            else if (Runtime.activeGame == Runtime.ActiveGame.ARMs)
                members = typeof(RenderInfoEnums.ARMS).GetMembers();
            else if (Runtime.activeGame == Runtime.ActiveGame.BOTW)
                members = typeof(RenderInfoEnums.BOTW).GetMembers();
            else if (Runtime.activeGame == Runtime.ActiveGame.Splatoon2)
                members = typeof(RenderInfoEnums.Splatoon2).GetMembers();
            else
                members = typeof(RenderInfoEnums.MK8D).GetMembers();

            for (int i = 0; i < members.Length; i++)
            {
                member = members[i];
                if (member.MemberType == MemberTypes.NestedType)
                {
                    enumType = Type.GetType(member.DeclaringType.FullName + "+" + member.Name);
                    if (enumType.IsEnum)
                    {
                        Console.WriteLine(member.Name);
                        if (member.Name == RenderInfoName)
                        {
                            arrEnumNames = enumType.GetEnumNames();

                            foreach (string name in arrEnumNames)
                            {
                                presetCB.Items.Add(name);
                            }
                        }
                    }
                }
            }

            if (presetCB.Items.Count > 0)
            {
                presetCB.SelectedIndex = presetCB.FindStringExact(valueTB.Text);
            }
        }

        public float[] GetFloats()
        {
            List<float> values = new List<float>();

            int curLine = 0;
            foreach (string line in valueTB.Lines)
            {
                if (line == string.Empty)
                    continue;

                float valResult;
                bool sucess = float.TryParse(line, out valResult);

                if (!sucess)
                    throw new Exception($"Failed to parse float at line {curLine}");

                values.Add(valResult);

                curLine++;
            }
            if (values.Count == 0)
                values.Add(0);

            return values.ToArray();
        }

        public int[] GetInts()
        {
            List<int> values = new List<int>();

            int curLine = 0;
            foreach (string line in valueTB.Lines)
            {
                if (line == string.Empty)
                    continue;

                int valResult;
                bool sucess = int.TryParse(line, out valResult);

                if (!sucess)
                    throw new Exception($"Failed to parse int at line {curLine}");

                values.Add(valResult);

                curLine++;
            }
            if (values.Count == 0)
                values.Add(0);

            return values.ToArray();
        }

        public string[] GetStrings()
        {
            List<string> values = new List<string>();

            int curLine = 0;
            foreach (string line in valueTB.Lines)
            {
                if (line == string.Empty)
                    continue;

                values.Add(line);

                curLine++;
            }
            if (values.Count == 0)
                values.Add("");

            return values.ToArray();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!CheckParser())
                return;

            if (RenderInfoName == string.Empty)
            {
                MessageBox.Show("Name parameter not set!", Application.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                DialogResult = DialogResult.None;
            }
            else
            {
                DialogResult = DialogResult.OK;
            }
        }
        private bool CheckParser()
        {
            bool CanParse = true;

            float valSingle;
            int valInt;
            byte valByte;

            string Error = "";

            int curLine = 0;
            foreach (var line in valueTB.Lines)
            {
                bool Success = true;

                if (FormatType == "WString")
                {

                }
                else if (FormatType == "String")
                {

                }
                else if (line == string.Empty) //Don't parse empty lines, instead we'll skip those
                {

                }
                else if (FormatType == "Single")
                    Success = float.TryParse(line, out valSingle);
                else if (FormatType == "Int32")
                    Success = int.TryParse(line, out valInt);
                

                if (!Success)
                {
                    CanParse = false;

                    Error += $"Invalid data type at line {curLine}.\n";
                }
                curLine++;
            }
            if (CanParse == false)
            {
                STErrorDialog.Show($"Data must be type of {FormatType}","User Data", Error);
            }

            return CanParse;
        }

        private void valueTB_TextChanged(object sender, EventArgs e)
        {
            int index = presetCB.FindStringExact(valueTB.Text);
            if (index >= 0)
                presetCB.SelectedIndex = index;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (presetCB.SelectedIndex >= 0)
            {
                valueTB.Text = presetCB.SelectedItem.ToString();
            }
        }
    }
}
