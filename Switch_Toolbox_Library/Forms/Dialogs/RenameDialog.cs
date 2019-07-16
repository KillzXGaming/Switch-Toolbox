using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace Toolbox.Library.Forms
{
    public partial class RenameDialog : Form
    {
        public RenameDialog()
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;
            button2.Select();
        }
        public void SetString(string Default)
        {
            textBox1.Text = Default;
            button2.Select();
        }
        //If used as a treeview filter
        public TreeViewCustom treeView;
        public TreeViewCustom _fieldsTreeCache;

        public void LoadTree(TreeViewCustom t)
        {
            treeView = t;

            _fieldsTreeCache = new TreeViewCustom();
            foreach (TreeNode originalNode in treeView.Nodes)
            {
                TreeNode newNode = new TreeNode(originalNode.Text);
                newNode.Tag = originalNode.Tag;
                _fieldsTreeCache.Nodes.Add(newNode);
                IterateTreeNodes(originalNode, newNode, _fieldsTreeCache);
            }
        }
        private void IterateTreeNodes(TreeNode originalNode, TreeNode rootNode, TreeView treeView2)
        {
            foreach (TreeNode childNode in originalNode.Nodes)
            {
                TreeNode newNode = new TreeNode(childNode.Text);
                newNode.Tag = childNode.Tag;
                treeView2.SelectedNode = rootNode;
                treeView2.SelectedNode.Nodes.Add(newNode);
                IterateTreeNodes(childNode, newNode, treeView2);
            }
        }
        private void RenameDialog_Load(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (treeView != null)
            {
                if (textBox1.Text == String.Empty)
                {
                    Console.WriteLine("Resetting tree");

                    treeView.Nodes.Clear();
                    foreach (TreeNode originalNode in _fieldsTreeCache.Nodes)
                    {
                        TreeNode newNode = new TreeNode(originalNode.Text);
                        newNode.Tag = originalNode.Tag;
                        treeView.Nodes.Add(newNode);
                        IterateTreeNodes(originalNode, newNode, treeView);
                    }
                }

                //blocks repainting tree till all objects loaded
                this.treeView.BeginUpdate();
                this.treeView.Nodes.Clear();
                if (this.textBox1.Text != string.Empty)
                {
                    foreach (TreeNode _parentNode in _fieldsTreeCache.Nodes)
                    {
                        foreach (TreeNode _childNode in _parentNode.Nodes)
                        {
                            if (_childNode.Text.StartsWith(this.textBox1.Text))
                            {
                                this.treeView.Nodes.Add((TreeNode)_childNode.Clone());
                            }
                        }
                    }
                }
                else
                {
                    foreach (TreeNode _node in this._fieldsTreeCache.Nodes)
                    {
                        treeView.Nodes.Add((TreeNode)_node.Clone());
                    }
                }
                //enables redrawing tree after all objects have been added
                this.treeView.EndUpdate();
            }
        }
    }
}
