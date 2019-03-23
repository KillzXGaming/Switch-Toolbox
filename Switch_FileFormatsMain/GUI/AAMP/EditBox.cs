using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AampV1Library;
using Aampv2 = AampV2Library;
using Syroot.Maths;

namespace FirstPlugin.Forms
{
    public partial class EditBox : Form
    {
        public EditBox()
        {
            InitializeComponent();

            foreach (var type in Enum.GetValues(typeof(ParamType)).Cast<ParamType>())
                typeCB.Items.Add(type);
        }

        ParamEntry paramEntry;
        Aampv2.ParamEntry paramEntryV2;

        public void LoadEntry(Aampv2.ParamEntry entry)
        {
            paramEntryV2 = entry;
            nameTB.Text = entry.HashString;
            typeCB.SelectedItem = entry.ParamType;

            dataTB.Text = entry.Value.ToString();

            switch (paramEntry.ParamType)
            {
                case ParamType.Vector2F:
                    var vec2 = (Vector2F)entry.Value;
                    dataTB.Text = $"{vec2.X} {vec2.Y}";
                    break;
                case ParamType.Vector3F:
                    var vec3 = (Vector3F)entry.Value;
                    dataTB.Text = $"{vec3.X} {vec3.Y} {vec3.Z}";
                    break;
                case ParamType.Vector4F:
                case ParamType.Color4F:
                    var vec4 = (Vector4F)entry.Value;
                    dataTB.Text = $"{vec4.X} {vec4.Y} {vec4.Z} {vec4.W}";
                    break;
            }
        }

        public void LoadEntry(ParamEntry entry)
        {
            paramEntry = entry;
            nameTB.Text = entry.HashString;
            typeCB.SelectedItem = entry.ParamType;

            dataTB.Text = entry.Value.ToString();

            switch (paramEntry.ParamType)
            {
                case ParamType.Vector2F:
                    var vec2 = (Vector2F)entry.Value;
                    dataTB.Text = $"{vec2.X} {vec2.Y}";
                    break;
                case ParamType.Vector3F:
                    var vec3 = (Vector3F)entry.Value;
                    dataTB.Text = $"{vec3.X} {vec3.Y} {vec3.Z}";
                    break;
                case ParamType.Vector4F:
                case ParamType.Color4F:
                    var vec4 = (Vector4F)entry.Value;
                    dataTB.Text = $"{vec4.X} {vec4.Y} {vec4.Z} {vec4.W}";
                    break;
            }
        }

        public void SaveEntry()
        {
            paramEntry.ParamType = (ParamType)typeCB.SelectedItem;

            switch (paramEntry.ParamType)
            {
                case ParamType.Boolean:
                    bool value = false;
                    bool.TryParse(dataTB.Text, out value);
                    paramEntry.Value = value;
                    break;
                case ParamType.Float:
                    float valueF = 0;
                    float.TryParse(dataTB.Text, out valueF);
                    paramEntry.Value = valueF;
                    break;
                case ParamType.Int:
                    int valueInt = 0;
                    int.TryParse(dataTB.Text, out valueInt);
                    paramEntry.Value = valueInt;
                    break;
                case ParamType.Vector2F:
                    var values2F = dataTB.Text.Split(' ');
                    if (values2F.Length != 2)
                    {

                    }
                    else
                    {
                        float x, y;
                        float.TryParse(values2F[0], out x);
                        float.TryParse(values2F[1], out y);
                        paramEntry.Value = new Vector2F(x, y);
                    }
                    break;
                case ParamType.Vector3F:
                    var values3F = dataTB.Text.Split(' ');
                    if (values3F.Length != 3)
                    {

                    }
                    else
                    {
                        float x, y, z;
                        float.TryParse(values3F[0], out x);
                        float.TryParse(values3F[1], out y);
                        float.TryParse(values3F[2], out z);
                        paramEntry.Value = new Vector3F(x, y, z);
                    }
                    break;
                case ParamType.Vector4F:
                case ParamType.Color4F:
                    var values = dataTB.Text.Split(' ');
                    if (values.Length != 4)
                    {

                    }
                    else
                    {
                        float x, y, z, w;
                        float.TryParse(values[0], out x);
                        float.TryParse(values[1], out y);
                        float.TryParse(values[2], out z);
                        float.TryParse(values[3], out w);
                        paramEntry.Value = new Vector4F(x,y,z,w);
                    }
                    break;
                case ParamType.Uint:
                    uint valueUInt = 0;
                    uint.TryParse(dataTB.Text, out valueUInt);
                    paramEntry.Value = valueUInt;
                    break;
                case ParamType.String64:
                case ParamType.String32:
                case ParamType.String256:
                case ParamType.StringRef:
                    paramEntry.Value = dataTB.Text;
                    break;
            }
        }

        private void dataTB_TextChanged(object sender, EventArgs e)
        {
            switch (paramEntry.ParamType)
            {
                case ParamType.Color4F:
                    var values = dataTB.Text.Split(' ');
                    if (values.Length == 4)
                    {
                        float x, y, z, w;
                        float.TryParse(values[0], out x);
                        float.TryParse(values[1], out y);
                        float.TryParse(values[2], out z);
                        float.TryParse(values[3], out w);

                        pictureBox1.BackColor = System.Drawing.Color.FromArgb(
                            FloatToIntClamp(w), FloatToIntClamp(x), FloatToIntClamp(y), FloatToIntClamp(z));
                    }
                    break;
            }
        }

        public static int FloatToIntClamp(float r)
        {
            return Clamp((int)(r * 255), 0, 255);
        }

        public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (paramEntry.ParamType == ParamType.Color4F)
            {
                ColorDialog dlg = new ColorDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    paramEntry.Value = new Vector4F(
                        dlg.Color.R / 255.0f,
                        dlg.Color.G / 255.0f,
                        dlg.Color.B / 255.0f,
                        dlg.Color.A / 255.0f);

                    var vec4 = (Vector4F)paramEntry.Value;
                    dataTB.Text = $"{vec4.X} {vec4.Y} {vec4.Z} {vec4.W}";
                }
            }
        }
    }
}
