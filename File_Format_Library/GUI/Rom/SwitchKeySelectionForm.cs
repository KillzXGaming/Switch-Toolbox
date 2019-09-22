using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using LibHac;
using Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class SwitchKeySelectionForm : STForm
    {
        public SwitchKeySelectionForm()
        {
            InitializeComponent();
        }

        public static Keyset ShowKeySelector()
        {
            if (Runtime.SwitchKeys.HasKeys())
                return ExternalKeys.ReadKeyFile(Runtime.SwitchKeys.ProdKeys, Runtime.SwitchKeys.TitleKeys);

            SwitchKeySelectionForm selectorKeys = new SwitchKeySelectionForm();
            if (selectorKeys.ShowDialog() == DialogResult.OK)
            {
                var Keys = ExternalKeys.ReadKeyFile(Runtime.SwitchKeys.ProdKeys, Runtime.SwitchKeys.TitleKeys);
                return Keys;
            }

            return null;
        }

        private void setProdKeyPath_Click(object sender, EventArgs e)
        {
            string ProdKeyPath = GetOpenedFileName();
            TextBoxProdKeyPath.Text = ProdKeyPath;

            if (File.Exists(ProdKeyPath))
            {
                TextBoxProdKeyPath.BackColor = FormThemes.BaseTheme.FormBackColor;
                Runtime.SwitchKeys.ProdKeys = ProdKeyPath;
            }
            else
                TextBoxProdKeyPath.BackColor = Color.Red;

            CheckKeys();
        }

        private void setTitleKeyPath_Click(object sender, EventArgs e)
        {
            string TitleKeyPath = GetOpenedFileName();
            TextBoxTitleKey.Text = TitleKeyPath;

            if (File.Exists(TitleKeyPath))
            { 
                TextBoxTitleKey.BackColor = FormThemes.BaseTheme.FormBackColor;
                Runtime.SwitchKeys.TitleKeys = TitleKeyPath;
            }
            else
                TextBoxTitleKey.BackColor = Color.Red;

            CheckKeys();
        }

        private void UpdateTextBoxes(string titlePath, string prodPath)
        {
            if (File.Exists(titlePath))
            {
                TextBoxTitleKey.Text = titlePath;
                TextBoxTitleKey.BackColor = FormThemes.BaseTheme.FormBackColor;
                Runtime.SwitchKeys.TitleKeys = titlePath;
            }
            else
                TextBoxTitleKey.BackColor = Color.Red;

            if (File.Exists(prodPath))
            {
                TextBoxTitleKey.Text = prodPath;
                TextBoxProdKeyPath.BackColor = FormThemes.BaseTheme.FormBackColor;
                Runtime.SwitchKeys.ProdKeys = prodPath;
            }
            else
                TextBoxProdKeyPath.BackColor = Color.Red;

            CheckKeys();
        }

        private void CheckKeys()
        {
            if (File.Exists(Runtime.SwitchKeys.ProdKeys) && File.Exists(Runtime.SwitchKeys.TitleKeys))
                btnOk.Enabled = true;
            else
                btnOk.Enabled = false;
        }

        private string GetOpenedFileName()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
                return ofd.FileName;
            else return null;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                var Keys = ExternalKeys.ReadKeyFile(Runtime.SwitchKeys.ProdKeys, Runtime.SwitchKeys.TitleKeys);
                Toolbox.Library.Config.Save();
            }
            catch
            {
                DialogResult = DialogResult.Ignore;
            }
        }

        private void stCheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (stCheckBox1.Checked)
            {
                TextBoxProdKeyPath.Text = Path.Combine(Runtime.SwitchKeys.SwitchFolder, "prod.keys");
                TextBoxTitleKey.Text = Path.Combine(Runtime.SwitchKeys.SwitchFolder, "title.keys");

            }
            else
            {
                TextBoxProdKeyPath.Text = Runtime.SwitchKeys.ProdKeys;
                TextBoxTitleKey.Text = Runtime.SwitchKeys.TitleKeys;
            }

            UpdateTextBoxes(TextBoxTitleKey.Text , TextBoxProdKeyPath.Text);
        }
    }
}
