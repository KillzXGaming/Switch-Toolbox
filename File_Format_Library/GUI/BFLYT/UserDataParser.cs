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

namespace LayoutBXLYT
{
    public partial class UserDataParser : STForm
    {
        public UserDataParser()
        {
            InitializeComponent();

            valueTB.BackColor = FormThemes.BaseTheme.FormBackColor;
            valueTB.ForeColor = FormThemes.BaseTheme.FormForeColor;

            typeCB.Items.Add(UserDataType.String);
            typeCB.Items.Add(UserDataType.Float);
            typeCB.Items.Add(UserDataType.Int);
        }

        public string UserDataName
        {
            set {
                nameTB.Text = value;
            }
            get {
                return nameTB.Text;
            }
        }

        public UserDataType Type
        {
            set
            {
                typeCB.SelectedItem = value;
            }
            get
            {
                return (UserDataType)typeCB.SelectedItem;
            }
        }

        public void LoadValues(string strings)
        {
            valueTB.Text += $"{strings}";
            typeCB.SelectedItem = UserDataType.String;
        }
        public void LoadValues(float[] floats)
        {
            foreach (var str in floats)
                valueTB.Text += $"{str}\n";

            typeCB.SelectedItem = UserDataType.Float;
        }
        public void LoadValues(int[] ints)
        {
            foreach (var str in ints)
                valueTB.Text += $"{str}\n";

            typeCB.SelectedItem = UserDataType.Int;
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

        public byte[] GetBytes()
        {
            List<byte> values = new List<byte>();

            int curLine = 0;
            foreach (string line in valueTB.Lines)
            {
                if (line == string.Empty)
                    continue;

                byte valResult;
                bool sucess = byte.TryParse(line, out valResult);

                if (!sucess)
                    throw new Exception($"Failed to parse byte at line {curLine}");

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

        public string[] GetStringUnicode()
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

        public string GetStringASCII()
        {
            string values = "";

            int curLine = 0;
            foreach (string line in valueTB.Lines)
            {
                if (line == string.Empty)
                    continue;

                values += line;
                curLine++;
            }

            return values;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!CheckParser())
                return;

            if (UserDataName == string.Empty)
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

                if (Type == UserDataType.String)
                {

                }
                else if (line == string.Empty) //Don't parse empty lines, instead we'll skip those
                {

                }
                else if (Type == UserDataType.Float)
                    Success = float.TryParse(line, out valSingle);
                else if (Type == UserDataType.Int)
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
                STErrorDialog.Show($"Data must be type of {Type}","User Data", Error);
            }

            return CanParse;
        }

        private void valueTB_TextChanged(object sender, EventArgs e)
        {
      
        }

        private void contentContainer_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
