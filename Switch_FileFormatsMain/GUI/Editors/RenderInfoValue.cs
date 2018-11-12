using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syroot.NintenTools.NSW.Bfres;
using Switch_Toolbox.Library;
using System.Reflection;

namespace FirstPlugin
{
    public partial class RenderInfoValueEditor : Form
    {
        public RenderInfoValueEditor()
        {
            InitializeComponent();

            if (!Runtime.IsDebugMode)
            {
                textBox1.ReadOnly = true;
                comboBox1.Enabled = false;
            }

            foreach (var type in Enum.GetValues(typeof(RenderInfoType)).Cast<RenderInfoType>())
                comboBox1.Items.Add(type);
        }
        public List<float> valueFloats = new List<float>();
        public List<string> valueStrings = new List<string>();
        public List<int> valueInts = new List<int>();

        public int ActiveItemIndex = 0;
        public void LoadValues(BFRESRender.BfresRenderInfo info)
        {
            textBox1.Text = info.Name;
            comboBox1.SelectedItem = info.Type;

            if (info.Type == RenderInfoType.Int32)
                valueInts = info.ValueInt.OfType<int>().ToList();
            if (info.Type == RenderInfoType.Single)
                valueFloats = info.ValueFloat.OfType<float>().ToList();
            if (info.Type == RenderInfoType.String)
                valueStrings = info.ValueString.OfType<string>().ToList();

            foreach (int i in valueInts)
                valuesListView.Items.Add(i.ToString());
            foreach (float f in valueFloats)
                valuesListView.Items.Add(f.ToString());
            foreach (string str in valueStrings)
                valuesListView.Items.Add(str);

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

            for (int i = 0; i < members.Length; i++ )
            {
                member = members[i];
                if (member.MemberType == MemberTypes.NestedType)
                {
                    enumType = Type.GetType(member.DeclaringType.FullName + "+" + member.Name);
                    if (enumType.IsEnum)
                    {
                        Console.WriteLine(member.Name);
                        if (member.Name == info.Name)
                        {
                            arrEnumNames = enumType.GetEnumNames();

                            foreach (string name in arrEnumNames)
                            {
                                renderInfoEnumsCB.Items.Add(name);
                            }
                        }
                    }
                }    
            }

    

            valuesListView.Items[0].Selected = true;
            valuesListView.Select();
        }
        private void SetEnums(Type enumType)
        {
            foreach (var type in Enum.GetValues(enumType))
                renderInfoEnumsCB.Items.Add(type);
        }
        public object SetNewValues()
        {
            if (valueInts.Count > 0)
               return valueInts.ToArray();
            else if (valueFloats.Count > 0)
                return valueFloats.ToArray();
            else if (valueStrings.Count > 0)
                return valueStrings.ToArray();
            else
                throw new Exception("No data found?");
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (valueInts.Count > 0)
                valueInts.RemoveAt(valueInts.Count - 1);
            if (valueFloats.Count > 0)
                valueFloats.RemoveAt(valueFloats.Count - 1);
            if (valueStrings.Count > 0)
                valueStrings.RemoveAt(valueStrings.Count - 1);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (valueInts.Count > 0)
            {
                valueInts.Add(0);
                valuesListView.Items.Add(0.ToString());
            }
            if (valueFloats.Count > 0)
            {
                valueFloats.Add(0);
                valuesListView.Items.Add(0.ToString());
            }
            if (valueStrings.Count > 0)
            {
                valueStrings.Add("");
                valuesListView.Items.Add("");
            }
        }

        private void valueBox_TextChanged(object sender, EventArgs e)
        {
            string val = valueBox.Text;

            var index = renderInfoEnumsCB.FindStringExact(val);
            if (index > -1)
            {
                renderInfoEnumsCB.SelectedIndex = index;
            }
            Console.WriteLine(index + " " + val);

            int ValueInt = 0;
            float ValueFloat = 0;

            if (valueInts.Count > 0)
            {
                bool IsInt = int.TryParse(val, out ValueInt);

                if (IsInt)
                {
                    valueInts[ActiveItemIndex] = ValueInt;
                }
                else
                {
                    MessageBox.Show("Value must be an int!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            if (valueFloats.Count > 0)
            {
                bool IsFloat = float.TryParse(val, out ValueFloat);

                if (IsFloat)
                {
                    valueFloats[ActiveItemIndex] = ValueFloat;
                }
                else
                {
                    MessageBox.Show("Value must be an float!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            if (valueStrings.Count > 0)
            {
                valueStrings[ActiveItemIndex] = val;
            }
            valuesListView.Items[ActiveItemIndex].Text = val;
        }

        private void valuesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (valuesListView.SelectedItems.Count > 0)
            {
                ActiveItemIndex = valuesListView.SelectedIndices[0];
                valueBox.Text = valuesListView.SelectedItems[0].Text;
            }
        }

        private void renderInfoEnums_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(renderInfoEnumsCB.SelectedIndex >= 0)
            {
                valueBox.Text = renderInfoEnumsCB.SelectedItem.ToString();
            }
        }
    }
}
