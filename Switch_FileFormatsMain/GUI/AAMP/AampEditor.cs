using ByamlExt.Byaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syroot.BinaryData;
using EditorCore;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library;
using ByamlExt;

namespace FirstPlugin.Forms
{
    public partial class AampEditorBase : STForm, IFIleEditor
    {
        public AAMP AampFile;

        public List<IFileFormat> GetFileFormats()
        {
            return new List<IFileFormat>() { AampFile };
        }

        public AampEditorBase(AAMP aamp, bool IsSaveDialog)
        {
            InitializeComponent();

            treeView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            treeView1.ForeColor = FormThemes.BaseTheme.FormForeColor;

            AampFile = aamp;

            if (AampFile.aampFileV1 != null)
            {
                Text = $"{AampFile.FileName} Type [{AampFile.aampFileV1.EffectType}]";
            }
            else
            {
                Text = $"{AampFile.FileName} Type [{AampFile.aampFileV2.EffectType}]";
            }

            if (!IsSaveDialog)
            {
                stButton1.Visible = false;
                stButton2.Visible = false;
                stPanel1.Dock = DockStyle.Fill;
            }
        }

        private void CopyNode_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(treeView1.SelectedNode.Text);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sav = new SaveFileDialog() { FileName = AampFile.FileName, Filter = "Parameter Archive | *.aamp" };
            if (sav.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(sav.FileName, AampFile.Save());
            }
        }

        private void editValueNodeMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count <= 0)
                return;

            OnEditorClick(listViewCustom1.SelectedItems[0]);
        }

        private void ResetValues()
        {
            if (treeView1.SelectedNode == null)
                return;

            listViewCustom1.Items.Clear();

            var targetNodeCollection = treeView1.SelectedNode.Nodes;

            dynamic target = treeView1.SelectedNode.Tag;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            ResetValues();
            TreeView_AfterSelect();
        }

        private void addNodeToolStripMenuItem_Click(object sender, EventArgs e) {
            if (treeView1.SelectedNode == null)
                return;

            AddParamEntry(treeView1.SelectedNode);
        }

        public virtual void OnEditorClick(ListViewItem SelectedItem) { }
        public virtual void TreeView_AfterSelect() { }
        public virtual void AddParamEntry(TreeNode parent) { }
        public virtual void RenameParamEntry(ListViewItem SelectedItem) { }
        public virtual void OnEntryDeletion(object target, TreeNode parent) { }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count <= 0 && treeView1.SelectedNode != null) 
                return;

            var result = MessageBox.Show("Are you sure you want to remove this entry? This cannot be undone!",
                $"Entry {listViewCustom1.SelectedItems[0].Text}", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                OnEntryDeletion(listViewCustom1.SelectedItems[0].Tag, treeView1.SelectedNode);

                int index = listViewCustom1.Items.IndexOf(listViewCustom1.SelectedItems[0]);
                listViewCustom1.Items.RemoveAt(index);
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e) {
            if (listViewCustom1.SelectedItems.Count <= 0)
                return;

            RenameParamEntry(listViewCustom1.SelectedItems[0]);
        }

        private void deleteNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                return;
            }
        }

        private void contentContainer_Paint(object sender, PaintEventArgs e)
        {

        }

        private void listViewCustom1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point pt = listViewCustom1.PointToScreen(e.Location);
                stContextMenuStrip1.Show(pt);
            }
        }

        private void stContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}
