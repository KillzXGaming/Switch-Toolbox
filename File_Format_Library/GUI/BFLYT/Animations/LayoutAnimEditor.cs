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
using Toolbox.Library.Animations;

namespace LayoutBXLYT
{
    public partial class LayoutAnimEditor : LayoutDocked
    {
        public EventHandler OnNodeSelected;

        private LytAnimation ActiveAnimation;
        private LytAnimGroup ActiveGroup;

        public LayoutAnimEditor()
        {
            InitializeComponent();

            BackColor = FormThemes.BaseTheme.FormBackColor;
            ForeColor = FormThemes.BaseTheme.FormBackColor;
            stTabControl1.myBackColor = FormThemes.BaseTheme.FormBackColor;

            numericUpDownFloat1.DecimalPlaces = 0;

            OnNodeSelected += treeViewCustom1_AfterSelect;
        }

        public void LoadFile(LytAnimation lytAnimation)
        {
            ActiveAnimation = lytAnimation;
            var bxlan = lytAnimation.BxlanAnimation;

            stComboBox1.BeginUpdate();
            stComboBox1.Items.Clear();
            foreach (var group in lytAnimation.AnimGroups)
            {
                stComboBox1.Items.Add(group.Name);
            }
            stComboBox1.EndUpdate();

            //   keyedAnimTimeline1.AddAnimation(lytAnimation, true);
            //  keyedAnimTimeline1.OnAnimationFrameAdvance += AnimationFrameAdvance;
        }

        public void OnAnimationPlaying()
        {
            numericUpDownFloat1.Value = (decimal)ActiveAnimation.Frame;

            if (ActiveGroup != null)
                LoadAnimationGroup(ActiveGroup);
        }

        private TreeNode LoadAnimNode(LytAnimGroup group)
        {
            var node = new TreeNode(group.Name) { Tag = group, };
            foreach (var subGroup in group.SubAnimGroups)
            {
                var subNode = new TreeNode(subGroup.Name) { Tag = subGroup, };
                node.Nodes.Add(subNode);
                foreach (LytAnimTrack track in subGroup.GetTracks())
                {
                    if (!track.HasKeys)
                        continue;

                    var trackNode = new TreeNode(track.Name) { Tag = track, };
                    subNode.Nodes.Add(trackNode);

                    foreach (LytKeyFrame keyFrame in track.KeyFrames)
                    {
                        var keyNode = new TreeNode($"Key Frame {keyFrame.Frame}") { Tag = keyFrame, };
                        trackNode.Nodes.Add(keyNode);
                    }
                }
            }
            return node;
        }

        private void Reset()
        {
            paneSRTPanel?.Dispose();
            paneSRTPanel = null;
            paneVisPanel?.Dispose();
            paneVisPanel = null;
            vtxcPanel?.Dispose();
            vtxcPanel = null;
        }

        private void AnimationFrameAdvance(object sender, EventArgs e)
        {
            if (ActiveGroup == null) return;

            LoadAnimationGroup(ActiveGroup);
        }

        private void treeViewCustom1_AfterSelect(object sender, EventArgs ea)
        {
            if (ea is TreeViewEventArgs)
            {
                var e = (TreeViewEventArgs)ea;

                if (e.Node.Tag == null) return;

                stPropertyGrid1.LoadProperty(e.Node.Tag);

                stComboBox1.SelectedItem = e.Node.Text;

                if (e.Node.Tag is LytKeyFrame)
                {
                    var keyFrame = e.Node.Tag as LytKeyFrame;
                    //   miniAnimationPlayer1.UpdateFrame(keyFrame.Frame + ActiveAnimation.StartFrame);
                }

                if (e.Node.Tag is LytAnimGroup)
                {
                    LoadAnimationGroup((LytAnimGroup)e.Node.Tag);
                }
            }
        }

        LytKeyPanePanel paneSRTPanel;
        LytKeyVisibiltyPane paneVisPanel;
        LytKeyVtxColorPanel vtxcPanel;

        private void LoadAnimationGroup(LytAnimGroup entry)
        {
            ActiveGroup = entry;

            float frame = ActiveAnimation.Frame;
            float startFrame = ActiveAnimation.StartFrame;

            Console.WriteLine("frame " + frame);

            if (paneSRTPanel == null || paneSRTPanel.IsDisposed || paneSRTPanel.Disposing)
            {
                stFlowLayoutPanel1.Controls.Clear();
                paneSRTPanel = new LytKeyPanePanel();
                paneVisPanel = new LytKeyVisibiltyPane();

                AddDropPanel(paneSRTPanel, "Pane SRT");
                AddDropPanel(paneVisPanel, "Pane Visibilty");
            }
            if (entry.Target == AnimationTarget.Pane)
            {
                BasePane pane = null;

                //If pane cannot be found use a new one and fill default values
                if (ActiveAnimation.parentLayout == null || !ActiveAnimation.parentLayout.PaneLookup.ContainsKey(ActiveGroup.Name))
                    pane = new BasePane();
                else
                    pane = ActiveAnimation.parentLayout.PaneLookup[ActiveGroup.Name];

                paneSRTPanel.SetSRT(frame, startFrame, pane, ActiveGroup.SubAnimGroups);
                paneVisPanel.SetPane(frame, startFrame, pane, ActiveGroup.SubAnimGroups);

                if (pane is IPicturePane || pane is IWindowPane)
                {
                    if (vtxcPanel == null || vtxcPanel.IsDisposed || vtxcPanel.Disposing)
                    {
                        vtxcPanel = new LytKeyVtxColorPanel();
                        AddDropPanel(vtxcPanel, "Vertex Colors");
                    }


                    Color[] colors = new Color[4];
                    colors[0] = Color.White;
                    colors[1] = Color.White;
                    colors[2] = Color.White;
                    colors[3] = Color.White;

                    if (pane is IPicturePane)
                    {
                        colors[0] = ((IPicturePane)pane).ColorTopRight.Color;
                        colors[1] = ((IPicturePane)pane).ColorTopLeft.Color;
                        colors[2] = ((IPicturePane)pane).ColorBottomRight.Color;
                        colors[3] = ((IPicturePane)pane).ColorBottomLeft.Color;
                    }
                    if (pane is IWindowPane)
                        colors = ((IWindowPane)pane).GetVertexColors();

                    vtxcPanel.SetColors(colors, startFrame, frame, ActiveGroup);
                }
                else
                {
                    if (stFlowLayoutPanel1.Controls.ContainsKey("Vertex Colors"))
                        stFlowLayoutPanel1.Controls.RemoveByKey("Vertex Colors");
                    vtxcPanel?.Dispose();
                    vtxcPanel = null;
                }
            }
            else if (entry.Target == AnimationTarget.Material)
            {
            }
            else
            {
            }
        }

        private void AddDropPanel(Control panel, string text)
        {
            var dropDownPanel = new STDropDownPanel();
            dropDownPanel.Name = text;
            panel.Dock = DockStyle.Fill;
            dropDownPanel.PanelName = text;
            dropDownPanel.Height = panel.Height;
            dropDownPanel.Controls.Add(panel);
            stFlowLayoutPanel1.Controls.Add(dropDownPanel);
        }

        private void treeViewCustom1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
