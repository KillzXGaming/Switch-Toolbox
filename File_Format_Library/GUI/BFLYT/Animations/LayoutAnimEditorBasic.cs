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

namespace LayoutBXLYT
{
    public partial class LayoutAnimEditorBasic : LayoutDocked
    {
        private BxlanHeader ActiveAnim;
        public EventHandler OnPropertyChanged;

        public LayoutAnimEditorBasic()
        {
            InitializeComponent();

            treeView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            treeView1.ForeColor = FormThemes.BaseTheme.FormForeColor;
            splitContainer1.BackColor = FormThemes.BaseTheme.FormBackColor;
        }

        public void LoadAnim(BxlanHeader bxlan)
        {
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
                var pai1 = new TreeNode("Animation Info") { Tag = header.AnimationInfo };

                for (int i = 0; i < header.AnimationInfo.Entries.Count; i++)
                    LoadAnimationEntry(header.AnimationInfo.Entries[i], pai1);

                root.Nodes.Add(pat1);
                root.Nodes.Add(pai1);
            }
        }

        private void LoadAnimationEntry(BxlanPaiEntry entry, TreeNode root)
        {
            var nodeEntry = new TreeNode(entry.Name) { Tag = entry };
            root.Nodes.Add(nodeEntry);

            for (int i = 0; i < entry.Tags.Count; i++)
            {
                var nodeTag = new TreeNode(entry.Tags[i].Type) { Tag = entry.Tags[i] };
                nodeEntry.Nodes.Add(nodeTag);
                for (int j = 0; j < entry.Tags[i].Entries.Count; j++)
                    LoadTagEntry(entry.Tags[i].Entries[j], nodeTag, j);
            }
        }

        private void LoadTagEntry(BxlanPaiTagEntry entry, TreeNode root, int index)
        {
            var nodeEntry = new TreeNode(entry.TargetName) { Tag = entry };
            root.Nodes.Add(nodeEntry);

            for (int i = 0; i < entry.KeyFrames.Count; i++)
            {
                var keyNode = new TreeNode($"Key Frame {i}") { Tag = entry.KeyFrames[i] };
                nodeEntry.Nodes.Add(keyNode);
            }
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
    }
}
