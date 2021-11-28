using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class SearchNodePanel : STUserControl
    {
        private TreeView treeView;

        private bool AutoSearch => chkAutoSearch.Checked;
        private bool MatchCase => chkMatchCase.Checked;
        private bool SearchSubNodes => chkSearchSubNodes.Checked;
        private bool UpdateDoubleClick => chkUpdateDoubleClick.Checked;
        private bool OpenDoubleClick => chkOpenWithDoubleClick.Checked;
        private bool AllowWildcards => chkAllowWildcards.Checked;
        
        private List<TreeNode> TreenodeLookup = new List<TreeNode>();

        public SearchNodePanel(TreeView tree)
        {
            InitializeComponent();

            treeView = tree;
            treeView.HideSelection = false;

            listViewCustom1.SmallImageList = tree.ImageList;
            listViewCustom1.LargeImageList = tree.ImageList;

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
                RecursiveTreeNodeSearch(node, text);

            listViewCustom1.EndUpdate();

            lblFoundEntries.Text = $"Found Entries {TreenodeLookup.Count} of {TotalNodeCount}";
        }

        private static String WildCardToRegular(String query)
        {
            return Regex.Escape(query).Replace("\\?", ".").Replace("\\*", ".*")+".*";
        }

        private void RecursiveTreeNodeSearch(TreeNode parentNode, string text)
        {
            bool HasText = Regex.IsMatch(parentNode.Text, AllowWildcards ? WildCardToRegular(text) : text, MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase);

            if (HasText)
            {
                if (parentNode.ImageKey != null && parentNode.ImageKey != string.Empty)
                {
                    listViewCustom1.Items.Add(new ListViewItem()
                    {
                        Text = parentNode.Text,
                        ImageKey = parentNode.ImageKey,
                    });
                }
                else
                {
                    listViewCustom1.Items.Add(new ListViewItem()
                    {
                        Text = parentNode.Text,
                        ImageIndex = parentNode.ImageIndex,
                    });
                }

                TreenodeLookup.Add(parentNode);
            }

            if (SearchSubNodes)
            {
                foreach (TreeNode node in parentNode.Nodes)
                    RecursiveTreeNodeSearch(node, text);
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
            if (listViewCustom1.SelectedItems.Count > 0 && OpenDoubleClick)
            {
                int index = listViewCustom1.SelectedIndices[0];
                if (TreenodeLookup[index] is TreeNodeCustom)
                    ((TreeNodeCustom)TreenodeLookup[index]).OnDoubleMouseClick(treeView);
                if (TreenodeLookup[index].Tag != null && TreenodeLookup[index].Tag is TreeNodeCustom)
                    ((TreeNodeCustom)TreenodeLookup[index].Tag).OnDoubleMouseClick(treeView);
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

        public override void OnControlClosing()
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

        private void chkAllowWildcards_CheckedChanged(object sender, EventArgs e) {
            UpdateSearchResults(searchTB.Text);
        }
    }
}
