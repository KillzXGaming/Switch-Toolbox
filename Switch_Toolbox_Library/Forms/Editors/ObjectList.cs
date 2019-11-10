using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Toolbox.Library.Rendering;
using Toolbox.Library.Forms;
using Toolbox.Library.Animations;

namespace Toolbox.Library
{
    public partial class ObjectList : UserControl
    {
        private static ObjectList _instance;
        public static ObjectList Instance { get { return _instance == null ? _instance = new ObjectList() : _instance; } }

        Thread Thread;

        public ObjectList()
        {
            InitializeComponent();
        }

        private void selectItem(object sender, TreeNodeMouseClickEventArgs e)
        {
            Viewport viewport = LibraryGUI.GetActiveViewport();

            if (viewport == null)
                return;

            //The new animation player
            if (e.Node.Tag != null && e.Node.Tag is STAnimation)
            {
                var anim = e.Node.Tag as STAnimation;

                if (LibraryGUI.GetAnimationPanel() != null)
                {
                    Console.WriteLine("running" + anim.Name);
                    LibraryGUI.GetAnimationPanel().CurrentSTAnimation = anim;
                }
            }

            //The old animation player (will be removed soon)
            if (e.Node is Animation)
            {
                string AnimName = e.Node.Text;
                AnimName = Regex.Match(AnimName, @"([A-Z][0-9][0-9])(.*)").Groups[0].ToString();
                if (AnimName.Length > 3)
                    AnimName = AnimName.Substring(3);

                Console.WriteLine("AnimName " + AnimName);

                Animation running = new Animation(AnimName);
                running.ReplaceMe((Animation)e.Node);
                running.Tag = e.Node;

                Queue<TreeNode> NodeQueue = new Queue<TreeNode>();
                foreach (TreeNode n in treeView1.Nodes)
                {
                    NodeQueue.Enqueue(n);
                }
                while (NodeQueue.Count > 0)
                {
                    try
                    {
                        TreeNode n = NodeQueue.Dequeue();
                        string NodeName = Regex.Match(n.Text, @"([A-Z][0-9][0-9])(.*)").Groups[0].ToString();
                        if (NodeName.Length <= 3)
                            Console.WriteLine(NodeName);
                        else
                            NodeName = NodeName.Substring(3);
                        if (n is Animation)
                        {
                            if (n == e.Node)
                                continue;
                            if (NodeName.Equals(AnimName))
                            {
                                running.Children.Add(n);
                            }
                        }
                        if (n is AnimationGroupNode)
                        {
                            foreach (TreeNode tn in n.Nodes)
                                NodeQueue.Enqueue(tn);
                        }
                    }
                    catch
                    {

                    }
                }

                if (LibraryGUI.GetAnimationPanel() != null)
                {
                    Console.WriteLine("running" + running.Text);
                    LibraryGUI.GetAnimationPanel().CurrentAnimation = running;
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode is TreeNodeCustom)
            {
                ((TreeNodeCustom)treeView1.SelectedNode).OnClick(treeView1);
            }
            else if (treeView1.SelectedNode is STGenericObject)
            {
                ((STGenericObject)treeView1.SelectedNode).OnClick(treeView1);
            }
            else if (treeView1.SelectedNode is STGenericMaterial)
            {
                ((STGenericMaterial)treeView1.SelectedNode).OnClick(treeView1);
            }
           // Viewport.Instance.UpdateViewport();
        }

        private void ObjectList_DockStateChanged(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            LibraryGUI.UpdateViewport();

            if (e.Node is STGenericModel)
            {
                CheckChildNodes(e.Node, e.Node.Checked);
            }
        }
        private void CheckChildNodes(TreeNode node, bool IsChecked)
        {
            foreach (TreeNode n in node.Nodes)
            {
                n.Checked = IsChecked;
                if (n.Nodes.Count > 0)
                {
                    CheckChildNodes(n, IsChecked);
                }
            }
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            return;

            RenameDialog search = new RenameDialog();
            search.Text = "Search";
            search.LoadTree(treeView1);
            if (search.ShowDialog() == DialogResult.OK)
            {
                string NameLookup = search.textBox1.Text;
            }
        }

        private void ObjectList_Load(object sender, EventArgs e)
        {
            // this.treeView1.DrawMode = TreeViewDrawMode.OwnerDrawText;
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            TreeViewHitTestInfo info = treeView1.HitTest(treeView1.PointToClient(Cursor.Position));
            if (info != null && info.Node is TreeNodeCustom)
            {
                if (e.Button == MouseButtons.Left)
                    ((TreeNodeCustom)info.Node).OnMouseLeftClick(treeView1);
                else if (e.Button == MouseButtons.Right)
                    ((TreeNodeCustom)info.Node).OnMouseRightClick(treeView1);

                treeView1.SelectedNode = info.Node;
            }
        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (treeView1.SelectedNode is TreeNodeCustom)
            {
                ((TreeNodeCustom)treeView1.SelectedNode).OnDoubleMouseClick(treeView1);
            }
        }
        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = true;

            if (e.Node is STGenericObject || e.Node is STGenericModel)
            {

            }
            else
                TreeViewExtensions.HideCheckBox(e.Node);
        }

    }
}
