using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AampLibraryCSharp;
using Syroot.Maths;

namespace FirstPlugin.Forms
{
    public partial class EditBox : Form
    {
        public EditBox()
        {
            InitializeComponent();
        }

        public void ToggleNameEditing(bool CanEdit)
        {
            nameTB.Enabled = CanEdit;
        }

        ParamEntry paramEntry;
        public void LoadEntry(ParamEntry entry)
        {
            typeCB.Items.Clear();

            foreach (var type in Enum.GetValues(typeof(ParamType)).Cast<ParamType>())
                typeCB.Items.Add(type);

            paramEntry = entry;
            nameTB.Text = entry.HashString;
            typeCB.SelectedItem = entry.ParamType;

            dataTB.Text = entry.Value.ToString();

            switch (entry.ParamType)
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
                case ParamType.BufferBinary:
                    dataTB.Text = string.Join(",", (byte[])entry.Value);
                    break;
                case ParamType.BufferFloat:
                    dataTB.Text = string.Join(",", (float[])entry.Value);
                    break;
                case ParamType.BufferInt:
                    dataTB.Text = string.Join(",", (int[])entry.Value);
                    break;
                case ParamType.BufferUint:
                    dataTB.Text = string.Join(",", (uint[])entry.Value);
                    break;
            }
        }

        public void SaveEntry()
        {
            if (paramEntry != null)
            {
                paramEntry.ParamType = (ParamType)typeCB.SelectedItem;

                if (nameTB.Enabled)
                    paramEntry.HashString = nameTB.Text;

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
                    case ParamType.BufferUint:
                        {
                            var strings = dataTB.Text.Split(',');
                            var array = new uint[strings.Length];
                            for (int i = 0; i < array.Length; i++)
                                array[i] = uint.Parse(strings[i]);
                            paramEntry.Value = array;
                        }
                        break;
                    case ParamType.BufferInt:
                        {
                            var strings = dataTB.Text.Split(',');
                            var array = new int[strings.Length];
                            for (int i = 0; i < array.Length; i++)
                                array[i] = int.Parse(strings[i]);
                            paramEntry.Value = array;
                        }
                        break;
                    case ParamType.BufferFloat:
                        {
                            var strings = dataTB.Text.Split(',');
                            var array = new float[strings.Length];
                            for (int i = 0; i < array.Length; i++)
                                array[i] = float.Parse(strings[i]);
                            paramEntry.Value = array;
                        }
                        break;
                    case ParamType.BufferBinary:
                        {
                            var strings = dataTB.Text.Split(',');
                            var array = new byte[strings.Length];
                            for (int i = 0; i < array.Length; i++)
                                array[i] = byte.Parse(strings[i]);
                            paramEntry.Value = array;
                        }
                        break;
                    case ParamType.Vector2F:
                        var values2F = dataTB.Text.Split(' ');
                        if (values2F.Length == 2) {
                            float x, y;
                            float.TryParse(values2F[0], out x);
                            float.TryParse(values2F[1], out y);
                            paramEntry.Value = new Vector2F(x, y);
                        }
                        else
                            throw new Exception("Invalid amount of values. Type requires 2 values.");
                        break;
                    case ParamType.Vector3F:
                        var values3F = dataTB.Text.Split(' ');
                        if (values3F.Length == 3) {
                            float x, y, z;
                            float.TryParse(values3F[0], out x);
                            float.TryParse(values3F[1], out y);
                            float.TryParse(values3F[2], out z);
                            paramEntry.Value = new Vector3F(x, y, z);
                        }
                        else
                            throw new Exception("Invalid amount of values. Type requires 3 values.");
                        break;
                    case ParamType.Vector4F:
                    case ParamType.Color4F:
                        var values = dataTB.Text.Split(' ');
                        if (values.Length == 4)
                        {
                            float x, y, z, w;
                            float.TryParse(values[0], out x);
                            float.TryParse(values[1], out y);
                            float.TryParse(values[2], out z);
                            float.TryParse(values[3], out w);
                            paramEntry.Value = new Vector4F(x, y, z, w);
                        }
                        else
                            throw new Exception("Invalid amount of values. Type requires 4 values.");
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
                        paramEntry.Value = new AampLibraryCSharp.StringEntry(dataTB.Text);
                        break;
                }
            }
        }

        private void dataTB_TextChanged(object sender, EventArgs e)
        {
            switch (paramEntry.ParamType)
            {
                case ParamType.Color4F:
                    UpdatePictureboxColor();
                    break;
            }
        }

        private void UpdatePictureboxColor()
        {
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
            if (paramEntry != null)
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



        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.nameTB = new System.Windows.Forms.TextBox();
            this.typeCB = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.dataTB = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // nameTB
            // 
            this.nameTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nameTB.Enabled = false;
            this.nameTB.Location = new System.Drawing.Point(76, 12);
            this.nameTB.Name = "nameTB";
            this.nameTB.Size = new System.Drawing.Size(220, 20);
            this.nameTB.TabIndex = 0;
            // 
            // typeCB
            // 
            this.typeCB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.typeCB.FormattingEnabled = true;
            this.typeCB.Location = new System.Drawing.Point(76, 38);
            this.typeCB.Name = "typeCB";
            this.typeCB.Size = new System.Drawing.Size(220, 21);
            this.typeCB.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Type:";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(275, 187);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // dataTB
            // 
            this.dataTB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataTB.Location = new System.Drawing.Point(12, 74);
            this.dataTB.Multiline = true;
            this.dataTB.Name = "dataTB";
            this.dataTB.Size = new System.Drawing.Size(346, 107);
            this.dataTB.TabIndex = 6;
            this.dataTB.TextChanged += new System.EventHandler(this.dataTB_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(302, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(56, 50);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // EditBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 216);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.dataTB);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.typeCB);
            this.Controls.Add(this.nameTB);
            this.Name = "EditBox";
            this.Text = "EditBox";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nameTB;
        private System.Windows.Forms.ComboBox typeCB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox dataTB;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
