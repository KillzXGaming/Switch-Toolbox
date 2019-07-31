using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class SearchNodeForm : STForm
    {
        private TreeView treeView;

        private bool AutoSearch => chkAutoSearch.Checked;
        private bool MatchCase => chkMatchCase.Checked;
        private bool SearchSubNodes => chkSearchSubNodes.Checked;
        private bool UpdateDoubleClick => chkUpdateDoubleClick.Checked;
        
        private List<TreeNode> TreenodeLookup = new List<TreeNode>();

        public SearchNodeForm(TreeView tree)
        {
            InitializeComponent();

            treeView = tree;
            treeView.HideSelection = false;

            listViewCustom1.SmallImageList = TreeViewCustom.imgList;
            listViewCustom1.LargeImageList = TreeViewCustom.imgList;

            listViewCustom1.View = View.List;
            chkSearchSubNodes.Checked = true;
            chkAutoSearch.Checked = true;

            listViewModeCB.Items.Add(View.LargeIcon);
            listViewModeCB.Items.Add(View.List);
            listViewModeCB.Items.Add(View.SmallIcon);
            listViewModeCB.Items.Add(View.Tile);
            listViewModeCB.Items.Add(View.Details);

            listViewModeCB.SelectedIndex = 1;
        }

        private void searchBtn_Click(object sender, EventArgs e) {
            UpdateSearchResults(searchTB.Text);
        }

        private void searchTB_TextChanged(object sender, EventArgs e)
        {
            if (AutoSearch)
                UpdateSearchResults(searchTB.Text);
        }

        private int TotalNodeCount;
        private void UpdateSearchResults(string text)
        {
            if (text == string.Empty)
                return;

            TotalNodeCount = 0;

            listViewCustom1.BeginUpdate();
            listViewCustom1.Items.Clear();
            TreenodeLookup.Clear();

            foreach (TreeNode node in treeView.Nodes)
                RecurvsiveTreeNodeSearch(node, text);

            listViewCustom1.EndUpdate();

            lblFoundEntries.Text = $"Found Entries {TreenodeLookup.Count} of {TotalNodeCount}";
        }

        private void RecurvsiveTreeNodeSearch(TreeNode parentNode, string text)
        {
            bool HasText = false;

            if (MatchCase)
                HasText = parentNode.Text.IndexOf(text, StringComparison.Ordinal) >= 0;
            else
                HasText = parentNode.Text.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0;

            if (HasText)
            {
                listViewCustom1.Items.Add(parentNode.Text, parentNode.ImageKey);
                TreenodeLookup.Add(parentNode);
            }

            if (SearchSubNodes)
            {
                foreach (TreeNode node in parentNode.Nodes)
                    RecurvsiveTreeNodeSearch(node, text);
            }

            TotalNodeCount++;
        }

        private void listViewCustom1_DoubleClick(object sender, EventArgs e)
        {
            if (listViewCustom1.SelectedItems.Count > 0 && UpdateDoubleClick)
            {
                int index = listViewCustom1.SelectedIndices[0];

                treeView.SelectedNode = TreenodeLookup[index];
                treeView.Refresh();
            }
        }

        private void listViewModeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = listViewModeCB.SelectedItem;
            if (item != null && item is View)
            {
                listViewCustom1.View = (View)item;
                listViewCustom1.Refresh();
            }
        }

        private void chkMatchCase_CheckedChanged(object sender, EventArgs e) {
            UpdateSearchResults(searchTB.Text);
        }

        private void SearchNodeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            treeView.HideSelection = true;
        }

        private void listViewCustom1_SelectedIndexChanged(object sender, EventArgs e) {
            if (UpdateDoubleClick || listViewCustom1.SelectedItems.Count == 0)
                return;

            int index = listViewCustom1.SelectedIndices[0];
            treeView.SelectedNode = TreenodeLookup[index];
            treeView.Refresh();
        }
    }
}
