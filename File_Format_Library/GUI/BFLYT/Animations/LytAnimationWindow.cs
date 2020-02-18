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
    public partial class LytAnimationWindow : STForm
    {
        private LayoutEditor ParentEditor;
        public BasePane ActivePane;
        public BxlanHeader ActiveAnim;
        public EventHandler OnPropertyChanged;

        CurveEditor CurveEditor;

        public LytAnimationWindow()
        {
            InitializeComponent();

            treeView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            treeView1.ForeColor = FormThemes.BaseTheme.FormForeColor;

            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;
            CurveEditor = new CurveEditor();
            CurveEditor.Dock = DockStyle.Fill;
            tabPage2.Controls.Add(CurveEditor);
        }

        public List<BxlanHeader> GetAnimations() {
            return ParentEditor.AnimationFiles;
        }

        public void LoadPane(BasePane pane, LayoutEditor editor) {
            ActivePane = pane;
            ParentEditor = editor;

            animationCB.Items.Clear();
            if (pane == null) return; //No pane is selected then jut reset and return

            SearchActiveAnimations(pane);
        }

        class AnimationComboboxItem
        {
            public object Tag;
            public string Text;

            public AnimationComboboxItem(string text) {
                Text = text;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        private void SearchActiveAnimations(BasePane pane)
        {
            Console.WriteLine($"SearchActiveAnimations {pane.Name} {pane.LayoutFile == null}");

            if (pane.LayoutFile == null) return;

            var animations = GetAnimations();
            var material = pane.TryGetActiveMaterial();
            string matName = material != null ? material.Name : "";

            var archive = pane.LayoutFile.FileInfo.IFileInfo.ArchiveParent;

            if (archive != null)
            {
                foreach (var file in archive.Files)
                {
                    string ext = Utils.GetExtension(file.FileName);
                    bool isBxlan = ext == ".bflan" || ext == ".bclan" || ext == ".brlan";
                    if (isBxlan && !animations.Any(x => x.FileName == file.FileName))
                    {
                        if (BxlanHeader.ContainsEntry(file.FileData, new string[2] { pane.Name, matName }))
                            animationCB.Items.Add(new AnimationComboboxItem(file.FileName) { Tag = file });
                    }
                }
            }

            for (int i = 0; i < animations?.Count; i++)
            {
                if (animations[i].ContainsEntry(pane.Name) || animations[i].ContainsEntry(matName)) {
                    animationCB.Items.Add(new AnimationComboboxItem(animations[i].FileName) { Tag = animations[i] });
                }
            }

            if (animationCB.Items.Count > 0)
                animationCB.SelectedIndex = 0;
        }

        private void animationCB_SelectedIndexChanged(object sender, EventArgs e) {
            treeView1.Nodes.Clear();
            if (animationCB.SelectedIndex >= 0)
            {
                AnimationComboboxItem item = (AnimationComboboxItem)animationCB.SelectedItem;
                BxlanHeader bxlan = null;

                if (item.Tag is ArchiveFileInfo)
                {
                    var fileFormat = ((ArchiveFileInfo)item.Tag).OpenFile();
                    bxlan = ((BXLAN)fileFormat).BxlanHeader;
                }
                else
                {
                    bxlan = (BxlanHeader)item.Tag;
                }
                if (bxlan == null || ActiveAnim == bxlan) return;

                ActiveAnim = bxlan;

                for (int i = 0; i < bxlan.AnimationInfo.Entries.Count; i++)
                    if (bxlan.AnimationInfo.Entries[i].Name == ActivePane.Name)
                        PopulateTreeview(bxlan, bxlan.AnimationInfo.Entries[i]);
            }
        }

        private void PopulateTreeview(BxlanHeader anim, BxlanPaiEntry entry) {
            treeView1.Nodes.Clear();
            LayoutAnimTreeLoader.LoadAnimationEntry(entry, null, treeView1);
            treeView1.ExpandAll();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                treeView1.SelectedNode = e.Node;
                if (e.Node is IContextMenuNode)
                {
                    STContextMenuStrip contextMenu = new STContextMenuStrip();
                    contextMenu.Items.AddRange(((IContextMenuNode)e.Node).GetContextMenuItems());
                    contextMenu.Show(Cursor.Position);
                }
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag == null) return;

            CurveEditor.Tracks.Clear();

            if (ActiveAnim != null) {
                LytAnimation animation = ActiveAnim.ToGenericAnimation(ParentEditor.ActiveLayout);
                Console.WriteLine($"animation {animation != null}");
                if (animation != null)
                {
                    Console.WriteLine($"tag {e.Node.Tag}");

                    if (e.Node.Tag is BxlanPaiTag)
                    {
                        var target = (BxlanPaiTag)e.Node.Tag;
                        var track = animation.FindTarget(target, 0);
                        if (track != null) {
                            CurveEditor.Tracks.Add(track);
                        }
                    }
                }
            }
            CurveEditor.UpdateViewport();

            stPropertyGrid1.LoadProperty(e.Node.Tag, PropertyChanged);
        }

        private void PropertyChanged()
        {
            OnPropertyChanged?.Invoke(null, new EventArgs());

            //Enable saving if file is edited
            if (ActiveAnim != null)
                ActiveAnim.FileInfo.CanSave = true;
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e) {
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
