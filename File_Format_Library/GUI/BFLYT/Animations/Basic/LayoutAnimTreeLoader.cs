using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library;

namespace LayoutBXLYT
{
    public class LayoutAnimTreeLoader
    {
        public static void LoadAnimationEntry(BxlanPaiEntry entry, TreeNode root, TreeView treeView)
        {
            var nodeEntry = new GroupAnimWrapper(entry.Name) { Tag = entry };
            if (root != null)
                root.Nodes.Add(nodeEntry);
            else
                treeView.Nodes.Add(nodeEntry);

            for (int i = 0; i < entry.Tags.Count; i++)
            {
                var nodeTag = new GroupWrapper(entry.Tags[i].Type) { Tag = entry.Tags[i] };
                nodeEntry.Nodes.Add(nodeTag);
                for (int j = 0; j < entry.Tags[i].Entries.Count; j++)
                    LoadTagEntry(entry.Tags[i].Entries[j], nodeTag, j);
            }
        }

        private static void LoadTagEntry(BxlanPaiTagEntry entry, TreeNode root, int index)
        {
            var nodeEntry = new GroupTargetWrapper(entry.TargetName) { Tag = entry };
            root.Nodes.Add(nodeEntry);

            for (int i = 0; i < entry.KeyFrames.Count; i++)
            {
                var keyNode = new KeyNodeWrapper($"Key Frame {i}") { Tag = entry.KeyFrames[i] };
                nodeEntry.Nodes.Add(keyNode);
            }
        }
    }

    public class AnimInfoWrapper : TreeNode, IContextMenuNode
    {
        private BxlytHeader ParentLayout;
        public BxlanPAI1 bxlanPai => (BxlanPAI1)Tag;

        public AnimInfoWrapper(string text, BxlytHeader parentLayout)
        {
            Text = text;
            ParentLayout = parentLayout;
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Add Animation Group", null, AddGroup, Keys.Control | Keys.A));
            Items.Add(new ToolStripMenuItem("Clear Groups", null, ClearGroups, Keys.Control | Keys.C));
            return Items.ToArray();
        }

        private void AddGroup(object sender, EventArgs e)
        {
            AddAnimGroupDialog dlg = new AddAnimGroupDialog(bxlanPai, ParentLayout);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var entry = dlg.AddEntry();
                var nodeEntry = new GroupAnimWrapper(entry.Name) { Tag = entry };
                Nodes.Add(nodeEntry);
            }
        }

        private void ClearGroups(object sender, EventArgs e)
        {
            bxlanPai.Entries.Clear();
            Nodes.Clear();
        }
    }

    public class GroupAnimWrapper : TreeNode, IContextMenuNode
    {
        public BxlanPAI1 AnimInfo => (BxlanPAI1)Parent.Tag;

        public BxlanPaiEntry PaiEntry => (BxlanPaiEntry)Tag;

        public GroupAnimWrapper(string text)
        {
            Text = text;
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Add Animation Group", null, AddGroup, Keys.Control | Keys.A));
            Items.Add(new ToolStripMenuItem("Remove Group", null, RemoveGroup, Keys.Delete));
            Items.Add(new ToolStripMenuItem("Clear Groups", null, ClearGroups, Keys.Control | Keys.C));
            return Items.ToArray();
        }

        private void RemoveGroup(object sender, EventArgs e)
        {
            AnimInfo.Entries.Remove(PaiEntry);
            Parent.Nodes.Remove(this);
        }

        private void AddGroup(object sender, EventArgs e)
        {
            AddGroupTypeDialog dlg = new AddGroupTypeDialog(PaiEntry);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var tag = dlg.AddEntry();
                var nodeEntry = new GroupWrapper(tag.Type) { Tag = tag };
                Nodes.Add(nodeEntry);
            }
        }

        private void ClearGroups(object sender, EventArgs e)
        {
            PaiEntry.Tags.Clear();
            Nodes.Clear();
        }
    }

    public class GroupWrapper : TreeNode, IContextMenuNode
    {
        public BxlanPaiEntry ParentPaiEntry => (BxlanPaiEntry)Parent.Tag;

        public BxlanPaiTag GroupTag => (BxlanPaiTag)Tag;

        public GroupWrapper(string text)
        {
            Text = text;
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Add Target", null, AddTarget, Keys.Control | Keys.A));
            Items.Add(new ToolStripMenuItem("Remove Group", null, RemoveGroup, Keys.Delete));
            Items.Add(new ToolStripMenuItem("Clear Targets", null, ClearTargets, Keys.Control | Keys.C));
            return Items.ToArray();
        }

        private void RemoveGroup(object sender, EventArgs e)
        {
            ParentPaiEntry.Tags.Remove(GroupTag);
            Parent.Nodes.Remove(this);
        }

        private void AddTarget(object sender, EventArgs e)
        {
            AddGroupTargetDialog dlg = new AddGroupTargetDialog();
            bool canLoad = dlg.LoadTag(GroupTag);
            if (dlg.ShowDialog() == DialogResult.OK && canLoad)
            {
                BxlanPaiTagEntry target = dlg.GetGroupTarget();
                target.Index = (byte)GroupTag.Entries.Count;
                GroupTag.Entries.Add(target);

                var nodeEntry = new GroupTargetWrapper(target.TargetName) { Tag = target };
                Nodes.Add(nodeEntry);
            }
        }

        private void ClearTargets(object sender, EventArgs e)
        {
            GroupTag.Entries.Clear();
            Nodes.Clear();
        }
    }


    public class GroupTargetWrapper : TreeNode, IContextMenuNode
    {
        public BxlanPaiTag GroupTag => (BxlanPaiTag)Parent.Tag;

        public BxlanPaiTagEntry TypeTag => (BxlanPaiTagEntry)Tag;

        public GroupTargetWrapper(string text)
        {
            Text = text;
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Add Keyframe", null, AddKey, Keys.Control | Keys.A));
            Items.Add(new ToolStripMenuItem("Remove Target", null, RemoveTarget, Keys.Delete));
            Items.Add(new ToolStripMenuItem("Clear Keys", null, RemoveKeys, Keys.Control | Keys.C));
            return Items.ToArray();
        }

        private void AddKey(object sender, EventArgs e)
        {
            float frame = 0;
            if (TypeTag.KeyFrames.Count > 0)
                frame = TypeTag.KeyFrames.Max(k => k.Frame) + 1;

            var keyFrame = new KeyFrame(frame);
            var keyNode = new KeyNodeWrapper($"Key Frame {TypeTag.KeyFrames.Count}")
            { Tag = keyFrame };

            TypeTag.KeyFrames.Add(keyFrame);
            Nodes.Add(keyNode);
        }

        private void RemoveKeys(object sender, EventArgs e)
        {
            TypeTag.KeyFrames.Clear();
            Nodes.Clear();
        }

        private void RemoveTarget(object sender, EventArgs e)
        {
            GroupTag.Entries.Remove(TypeTag);
            Parent.Nodes.Remove(this);
        }

        public void UpdateKeys(TreeNode removedNode)
        {
            int index = 0;
            foreach (TreeNode node in Nodes)
            {
                if (node == removedNode)
                    continue;

                node.Text = $"Key Frame {index++}";
            }
        }
    }

    public class KeyNodeWrapper : TreeNode, IContextMenuNode
    {
        public BxlanPaiTagEntry TypeTag => (BxlanPaiTagEntry)Parent.Tag;

        public KeyFrame KeyFrame => (KeyFrame)Tag;

        public KeyNodeWrapper(string text)
        {
            Text = text;
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Remove Key", null, RemoveKey, Keys.Delete));
            return Items.ToArray();
        }

        private void RemoveKey(object sender, EventArgs e)
        {
            TypeTag.KeyFrames.Remove(KeyFrame);
            ((GroupTargetWrapper)Parent).UpdateKeys(this);
            Parent.Nodes.Remove(this);
        }
    }
}
