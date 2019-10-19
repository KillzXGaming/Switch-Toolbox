using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Toolbox.Library.Animations;

namespace Toolbox.Library.Forms
{
    public class KeyedAnimTimeline : TimeLine
    {
        public EventHandler OnNodeSelected;

        public string GroupTarget = "";
        public bool DisplayKeys = true;

        private STAnimation activeAnimation;
        public STAnimation ActiveAnimation
        {
            get { return activeAnimation; }
            set
            {
                activeAnimation = value;
                PopulateTree();
            }
        }

        protected static int lineHeight = TextRenderer.MeasureText("§", font).Height;

        protected int scrollY = 0;

        private int trackCount = 0;

        private TreeView NodeTree;
        private Splitter splitter;

        private ImageList imgList = new ImageList();

        public KeyedAnimTimeline()
        {
            imgList = new ImageList()
            {
                ImageSize = new Size(24, 24),
                ColorDepth = ColorDepth.Depth32Bit,
            };
        }

        public void ReloadEditorView()
        {
            imgList.Images.Add("AnimationGroup", Properties.Resources.AnimationGroup);
            imgList.Images.Add("AnimationTrack", Properties.Resources.AnimationTrack);
            imgList.Images.Add("AnimationTrackR", Properties.Resources.AnimationTrackR);
            imgList.Images.Add("AnimationTrackG", Properties.Resources.AnimationTrackG);
            imgList.Images.Add("AnimationTrackB", Properties.Resources.AnimationTrackB);
            imgList.Images.Add("AnimationTrackA", Properties.Resources.AnimationTrackA);
            imgList.Images.Add("AnimationTrackX", Properties.Resources.AnimationTrackX);
            imgList.Images.Add("AnimationTrackY", Properties.Resources.AnimationTrackY);
            imgList.Images.Add("AnimationTrackZ", Properties.Resources.AnimationTrackZ);
            imgList.Images.Add("AnimationTrackW", Properties.Resources.AnimationTrackW);

            NodeTree = new TreeView();
            NodeTree.ImageList = imgList;
            NodeTree.Dock = DockStyle.Left;
            //    NodeTree.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left;
            NodeTree.Height = this.Height;
            NodeTree.Width = 300;
            NodeTree.BackColor = FormThemes.BaseTheme.FormBackColor;
            NodeTree.ForeColor = FormThemes.BaseTheme.FormForeColor;
            NodeTree.AfterSelect += NodeTree_OnAfterNodeSelect;
            NodeTree.BeforeExpand += NodeTree_OnExpand;
            NodeTree.AfterExpand += NodeTree_OnExpand;
            NodeTree.BeforeCollapse += NodeTree_OnExpand;
            NodeTree.AfterCollapse += NodeTree_OnExpand;

            Controls.Add(NodeTree);
            splitter = new Splitter();
            splitter.Dock = DockStyle.Left;
            splitter.LocationChanged += SpitterLocationChanged;
            Controls.Add(splitter);

            margin = NodeTree.Width;
        }

        private void PopulateTree()
        {
            if (ActiveAnimation == null || NodeTree == null || !DisplayKeys) return;

            NodeTree.Nodes.Clear();
            foreach (var group in ActiveAnimation.AnimGroups)
            {
                if (GroupTarget != string.Empty && group.Name != GroupTarget)
                    continue;

                if (group.Category != string.Empty)
                {
                    var categoryNode = AddCategoryNode(NodeTree.Nodes, group.Category);
                    AddGroup(categoryNode.Nodes, group);
                }
                else
                    AddGroup(NodeTree.Nodes, group);
            }
        }

        private TreeNode AddCategoryNode(TreeNodeCollection nodes, string name)
        {
            foreach (TreeNode node in nodes)
                if (node.Text == name) return node;

            var newNode = new TreeNode(name);
            NodeTree.Nodes.Add(newNode);
            return newNode;
        }

        private void AddGroup(TreeNodeCollection nodes, STAnimGroup group)
        {
            TreeNode groupNode = new TreeNode(group.Name)
            { Tag = group, ImageKey = "AnimationGroup", };
            nodes.Add(groupNode);

            foreach (var subGroup in group.SubAnimGroups)
                AddGroup(groupNode.Nodes, subGroup);

            foreach (var track in group.GetTracks())
            {
                if (!track.HasKeys)
                    continue;

                string imageKey = "AnimationTrack";
                if (track.Name.EndsWith("R")) imageKey = "AnimationTrackR";
                if (track.Name.EndsWith("G")) imageKey = "AnimationTrackG";
                if (track.Name.EndsWith("B")) imageKey = "AnimationTrackB";
                if (track.Name.EndsWith("A")) imageKey = "AnimationTrackA";
                if (track.Name.EndsWith("X")) imageKey = "AnimationTrackX";
                if (track.Name.EndsWith("Y")) imageKey = "AnimationTrackY";
                if (track.Name.EndsWith("Z")) imageKey = "AnimationTrackZ";
                if (track.Name.EndsWith("W")) imageKey = "AnimationTrackW";

                TreeNode trackNode = new TreeNode(track.Name)
                { Tag = track, ImageKey = imageKey, SelectedImageKey = imageKey, };
                groupNode.Nodes.Add(trackNode);
            }
        }

        private void NodeTree_OnExpand(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void NodeTree_OnAfterNodeSelect(object sender, TreeViewEventArgs e)
        {
            OnNodeSelected?.Invoke(sender, e);
            this.Invalidate();
        }

        private void SpitterLocationChanged(object sender, EventArgs e)
        {
            margin = splitter.Location.X;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!DisplayKeys)
            {
                base.OnPaint(e);
                return;
            }

            e.Graphics.FillRectangle(brush3, new Rectangle(0, 0, margin, Height));

            e.Graphics.SetClip(new Rectangle(0, barHeight, Width, Height - barHeight));

            bool v = false;
            int y = -scrollY - 10;
            trackCount = 0;

            if (ActiveAnimation != null)
            {
                foreach (var node in TreeViewExtensions.Collect(NodeTree.Nodes))
                {
                    if (node.Parent != null && node.Tag is STAnimationTrack && node.Parent.IsVisible && node.Parent.IsExpanded)
                    {
                        var track = node.Tag as STAnimationTrack;
                        if (!track.HasKeys)
                            continue;

                        for (int i = 1; i < track.KeyFrames.Count; i++)
                        {
                            int l = Math.Max(-20, (int)((
                                track.KeyFrames[i - 1].Frame
                                - frameLeft) * (Width - 40 - margin) / (frameRight - frameLeft)));
                            int r = (int)((
                                track.KeyFrames[i].Frame
                                - frameLeft) * (Width - 40 - margin) / (frameRight - frameLeft));

                            if (v = !v)
                            {
                                var keyColor = new SolidBrush(Color.Yellow);
                                switch (node.ImageKey)
                                {
                                    case "AnimationTrackR":
                                    case "AnimationTrackX":
                                        keyColor = new SolidBrush(Color.Red);
                                        break;
                                    case "AnimationTrackG":
                                    case "AnimationTrackY":
                                        keyColor = new SolidBrush(Color.Green);
                                        break;
                                    case "AnimationTrackB":
                                    case "AnimationTrackZ":
                                        keyColor = new SolidBrush(Color.Blue);
                                        break;
                                    case "AnimationTrackA":
                                    case "AnimationTrackW":
                                        keyColor = new SolidBrush(Color.Gray);
                                        break;
                                }

                                if (node.IsSelected)
                                    keyColor = new SolidBrush(Color.White);

                                e.Graphics.FillEllipse(keyColor, new Rectangle(l + margin + 20, barHeight + y, 5, lineHeight));
                            }
                        }
                    }

                    if (node.IsVisible)
                        y += NodeTree.ItemHeight;
                }
            }

            base.OnPaint(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.X < margin)
            {
                scrollY = Math.Max(Math.Min(trackCount * lineHeight + barHeight - Height, scrollY - e.Delta / 2), 0);
                Refresh();
            }
            else
                base.OnMouseWheel(e);
        }

        protected override void OnResize(EventArgs e)
        {
            scrollY = Math.Max(Math.Min(trackCount * lineHeight + barHeight - Height, scrollY), 0);
            base.OnResize(e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // KeyedAnimTimeline
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "KeyedAnimTimeline";
            this.ResumeLayout(false);

        }
    }
}
