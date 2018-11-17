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
using WeifenLuo.WinFormsUI.Docking;
using System.Text.RegularExpressions;
using Switch_Toolbox.Library.Rendering;
using Switch_Toolbox.Library.Forms;

namespace Switch_Toolbox.Library
{
    public partial class ObjectList : DockContent
    {
        Thread Thread;

        public ObjectList()
        {
            InitializeComponent();
            ApplyThumbnailSetting(Runtime.thumbnailSize);
    //        treeView1.Sort();
        }

        private void selectItem(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node is Animation)
            {
                string AnimName = e.Node.Text;
                AnimName = Regex.Match(AnimName, @"([A-Z][0-9][0-9])(.*)").Groups[0].ToString();
                if (AnimName.Length > 3)
                    AnimName = AnimName.Substring(3);

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
                AnimationPanel AnimationPanel = LoadAnimationPanel();

                if (AnimationPanel != null)
                {
                    AnimationPanel.CurrentAnimation = running;
                }
            }
        }
        public AnimationPanel LoadAnimationPanel()
        {
            Form form1 = Application.OpenForms[0];
            foreach (Control control in form1.Controls)
            {
                if (control is DockPanel)
                {                    
                    foreach (DockContent ctrl in ((DockPanel)control).Contents)
                    {
                        if (ctrl is AnimationPanel)
                        {
                          //  return (AnimationPanel)ctrl;
                        }
                    }
                }
            }
            return null;
        }

        private void ApplyThumbnailSetting(Runtime.ThumbnailSize size)
        {
            //Set default color
            smallToolStripMenuItem.BackColor = Color.FromArgb(33, 33, 33);
            mediumToolStripMenuItem.BackColor = Color.FromArgb(33, 33, 33);
            largeToolStripMenuItem.BackColor = Color.FromArgb(33, 33, 33);

            switch (size)
            {
                case Runtime.ThumbnailSize.Small:
                    treeView1.ImageHeight = 21;
                    treeView1.ImageWidth = 21;
                    smallToolStripMenuItem.BackColor = Color.FromArgb(80, 80, 80);
                    break;
                case Runtime.ThumbnailSize.Medium:
                    treeView1.ImageHeight = 27;
                    treeView1.ImageWidth = 27;
                    mediumToolStripMenuItem.BackColor = Color.FromArgb(80, 80, 80);
                    break;
                case Runtime.ThumbnailSize.Large:
                    treeView1.ImageHeight = 34;
                    treeView1.ImageWidth = 34;
                    largeToolStripMenuItem.BackColor = Color.FromArgb(80, 80, 80);
                    break;
            }
            treeView1.ReloadImages();
        }

        private void largeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Runtime.thumbnailSize = Runtime.ThumbnailSize.Large;
            ApplyThumbnailSetting(Runtime.thumbnailSize);
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Runtime.thumbnailSize = Runtime.ThumbnailSize.Medium;
            ApplyThumbnailSetting(Runtime.thumbnailSize);
        }

        private void smallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Runtime.thumbnailSize = Runtime.ThumbnailSize.Small;
            ApplyThumbnailSetting(Runtime.thumbnailSize);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            foreach (IFileFormat file in FileManager.GetFileFormats())
            {
                if (file.IFileInfo.IsActive)
                {
                    DialogResult dialogResult = MessageBox.Show("Are you sure you want to close this tab? Doing so may result in losing progress.", "", MessageBoxButtons.YesNo);
                    if (dialogResult != DialogResult.Yes)
                    {
                        e.Cancel = true;
                        base.OnFormClosing(e);
                    }
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode is TreeNodeCustom)
            {
                ((TreeNodeCustom)treeView1.SelectedNode).OnClick(treeView1);
            }
            if (treeView1.SelectedNode is STGenericObject)
            {
                ((STGenericObject)treeView1.SelectedNode).OnClick(treeView1);
            }
            if (treeView1.SelectedNode is STGenericMaterial)
            {
                ((STGenericMaterial)treeView1.SelectedNode).OnClick(treeView1);
            }

            Viewport.Instance.UpdateViewport();
        }

        private void ObjectList_DockStateChanged(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            Viewport.Instance.UpdateViewport();
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
            if (e.Button == MouseButtons.Left)
            {
                TreeViewHitTestInfo info = treeView1.HitTest(treeView1.PointToClient(Cursor.Position));
                if (info != null && info.Node is TreeNodeCustom)
                {
                    ((TreeNodeCustom)info.Node).OnMouseClick(treeView1);
                    treeView1.SelectedNode = info.Node;
                }
            }
        }
    }
}
