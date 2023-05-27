using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Bfres.Structs;
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class ShaderOptionsEditor : UserControl
    {
        private bool FilterDefaults => chkFilterDefaults.Checked;

        public ShaderOptionsEditor()
        {
            InitializeComponent();

            shaderOptionsListView.ListViewItemSorter = new ListSorter();
            shaderOptionsListView.FullRowSelect = true;
            shaderOptionsListView.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.HeaderSize);

            foreach (BrightIdeasSoftware.OLVColumn column in shaderOptionsListView.Columns)
            {
                var headerstyle = new BrightIdeasSoftware.HeaderFormatStyle();
                headerstyle.SetBackColor(FormThemes.BaseTheme.FormBackColor);
                headerstyle.SetForeColor(FormThemes.BaseTheme.FormForeColor);
                column.HeaderFormatStyle = headerstyle;
            }
            shaderOptionsListView.BorderStyle = BorderStyle.None;
        }

        FMAT material;
        Thread Thread;

        public void InitializeShaderOptionList(FMAT mat)
        {
            material = mat;

            List<Options> options = new List<Options>();

            shaderOptionsListView.BeginUpdate();
            shaderOptionsListView.Items.Clear();
            shaderOptionsListView.BackColor = FormThemes.BaseTheme.FormBackColor;
            shaderOptionsListView.ForeColor = FormThemes.BaseTheme.FormForeColor;

            foreach (var option in material.shaderassign.options)
            {
                if (FilterDefaults && option.Value == "<Default Value>")
                    continue;

                options.Add(new Options() { Name = option.Key, Value = option.Value });
            }

            shaderOptionsListView.SetObjects(options);
            options.Clear();
            shaderOptionsListView.FillLastColumnSpace(true);
            shaderOptionsListView.EndUpdate();
        }

        public class Options
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        private void shaderOptionsListView_DoubleClick(object sender, EventArgs e)
        {
            int ActiveIndex = shaderOptionsListView.SelectedIndices[0];
            if (shaderOptionsListView.SelectedObject != null)
            {
                var option = (Options)shaderOptionsListView.SelectedObject;

                var edtior = new ShaderOptionsEditBox();
                edtior.LoadOption(option.Name, option.Value);

                if (edtior.ShowDialog() == DialogResult.OK)
                {
                    option.Name = edtior.textBoxName.Text;
                    option.Value = edtior.textBoxValue.Text;

                    material.shaderassign.options[option.Name] = option.Value;

                    shaderOptionsListView.RefreshObject(option);
                }
            }
        }

        private void shaderOptionsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (shaderOptionsListView.SelectedObject != null)
            {
                btnScrolDown.Enabled = true;
                btnScrollUp.Enabled = true;
                btnEdit.Enabled = true;
                btnRemove.Enabled = true;
            }
            else
            {
                btnScrolDown.Enabled = false;
                btnScrollUp.Enabled = false;
                btnEdit.Enabled = false;
                btnRemove.Enabled = false;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("NOTE! Shader Options are link by shaders. These are not possible to edit yet, do you want to continue?", "Material Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
            if (result == DialogResult.OK)
            {
                var edtior = new ShaderOptionsEditBox();
                edtior.LoadOption("", "");

                if (edtior.ShowDialog() == DialogResult.OK)
                {
                    shaderOptionsListView.AddObject(new Options()
                    {
                        Name = edtior.textBoxName.Text,
                        Value = edtior.textBoxValue.Text,
                    });
                    material.shaderassign.options.Add(edtior.textBoxName.Text, edtior.textBoxValue.Text);
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("NOTE! Shader Options are link by shaders. These are not possible to edit yet, do you want to continue?", "Material Editor", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
            if (result == DialogResult.OK)
            {
                if (shaderOptionsListView.SelectedObject != null)
                {
                    var option = (Options)shaderOptionsListView.SelectedObject;

                    material.shaderassign.options.Remove(option.Name);
                    shaderOptionsListView.RemoveObject(option);
                }
            }
        }

        private void chkFilterDefaults_CheckedChanged(object sender, EventArgs e)
        {
            InitializeShaderOptionList(material);
        }
    }
}
