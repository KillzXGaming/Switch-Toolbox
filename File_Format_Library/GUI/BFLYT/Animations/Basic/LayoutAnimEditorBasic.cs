using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Forms;
using Toolbox.Library;

namespace LayoutBXLYT
{
    public partial class LayoutAnimEditorBasic : UserControl
    {
        private BxlytHeader ParentLayout;
        private BxlanHeader ActiveAnim;
        public EventHandler OnPropertyChanged;

        public LayoutAnimEditorBasic()
        {
            InitializeComponent();

            treeView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            treeView1.ForeColor = FormThemes.BaseTheme.FormForeColor;
            splitContainer1.BackColor = FormThemes.BaseTheme.FormBackColor;
        }

        public void LoadAnim(BxlanHeader bxlan, BxlytHeader parentLayout = null)
        {
            ParentLayout = parentLayout;
            LoadAnimations(bxlan, new TreeNode(bxlan.FileName) { Tag = bxlan });
        }

        public void LoadAnimations(BxlanHeader bxlan, TreeNode root, bool LoadRoot = true)
        {
            ActiveAnim = bxlan;

            if (LoadRoot)
                treeView1.Nodes.Add(root);

            if (bxlan is BxlanHeader)
            {
                var header = bxlan as BxlanHeader;
                var pat1 = new TreeNode("Tag Info") { Tag = header.AnimationTag };
                var pai1 = new AnimInfoWrapper("Animation Info", ParentLayout) { Tag = header.AnimationInfo };

                for (int i = 0; i < header.AnimationInfo.Entries.Count; i++)
                    LayoutAnimTreeLoader.LoadAnimationEntry(header.AnimationInfo.Entries[i], pai1, treeView1);

                root.Nodes.Add(pat1);
                root.Nodes.Add(pai1);
            }
        }

        public void LoadAnimationEntry(BxlanHeader bxlan, BxlanPaiEntry entry)
        {
            ActiveAnim = bxlan;
            LayoutAnimTreeLoader.LoadAnimationEntry(entry, null, treeView1);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null) return;

            stPropertyGrid1.LoadProperty(e.Node.Tag, PropertyChanged);
        }

        private void PropertyChanged()
        {
            OnPropertyChanged?.Invoke(null, new EventArgs());

            //Enable saving if file is edited
            if (ActiveAnim != null)
                ActiveAnim.FileInfo.CanSave = true;
        }

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                if (e.Node is IContextMenuNode)
                {
                    STContextMenuStrip contextMenu = new STContextMenuStrip();
                    contextMenu.Items.AddRange(((IContextMenuNode)e.Node).GetContextMenuItems());
                    contextMenu.Show(Cursor.Position);
                }
            }
        }

        private void treeView1_KeyPress(object sender, KeyEventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode is IContextMenuNode)
            {
                IContextMenuNode node = (IContextMenuNode)treeView1.SelectedNode;

                var Items = node.GetContextMenuItems();
                foreach (ToolStripItem toolstrip in Items)
                {
                    if (toolstrip is ToolStripMenuItem)
                    {
                        if (((ToolStripMenuItem)toolstrip).ShortcutKeys == e.KeyData)
                            toolstrip.PerformClick();
                    }
                }
            }
        }
    }
}
