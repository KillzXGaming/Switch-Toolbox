using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Switch_Toolbox.Library.Forms;
using Switch_Toolbox.Library;
using Switch_Toolbox.Library.IO;
using Switch_Toolbox.Library.Animations;
using System.Text.RegularExpressions;
using Bfres.Structs;

namespace FirstPlugin.Forms
{
    public partial class AnimationLoader : STForm
    {
        public AnimationLoader()
        {
            InitializeComponent();

            animTypeCB.Items.Add("Skeletal Animation");
            animTypeCB.Items.Add("Material Animation");
            animTypeCB.Items.Add("Bone Visibilty Animation");
            animTypeCB.Items.Add("Scene Animation");
            animTypeCB.Items.Add("Shape Animation");
        }

        public void LoadFile(string fileName)
        {

        }

        public BFRES bfres;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            List<IFileFormat> formats = new List<IFileFormat>();
            formats.Add(new BFRES());
            ofd.Filter = Utils.GetAllFilters(formats);

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var format = STFileLoader.OpenFileFormat(ofd.FileName);

                if (format != null)
                {
                    if (format is SARC)
                    {
                        foreach (SARC.SarcEntry fileNode in ((SARC)format).Nodes)
                        {
                            if (fileNode.ImageKey == "bfres")
                            {
                                var sarcFile = STFileLoader.OpenFileFormat(fileNode.FullName, fileNode.Data, false, true);
                                bfres = (BFRES)sarcFile;
                            }
                        }
                    }
                    else if (format is BFRES)
                    {
                        bfres = (BFRES)format;
                    }
                    else
                        throw new Exception("Failed to load bfres file. ");
                }
            }
        }

        private void LoadAnimations(TreeNodeCollection nodes)
        {
            string cbText = animTypeCB.GetItemText(animTypeCB.SelectedItem);
            animTreeView.Nodes.Clear();

            foreach (var node in nodes)
            {
                if (node is BFRESGroupNode)
                {
                    BFRESGroupNode group = (BFRESGroupNode)node;

                    switch (group.Type)
                    {
                        case BRESGroupType.SkeletalAnim:
                        case BRESGroupType.MaterialAnim:
                        case BRESGroupType.BoneVisAnim:
                        case BRESGroupType.ShapeAnim:
                        case BRESGroupType.SceneAnim:
                        case BRESGroupType.MatVisAnim:
                        case BRESGroupType.ShaderParamAnim:
                        case BRESGroupType.ColorAnim:
                        case BRESGroupType.TexSrtAnim:
                        case BRESGroupType.TexPatAnim:
                            AddAnims(group);
                            break;
                    }
                }
            }
        }

        private void AddAnims(TreeNode node)
        {
            foreach (TreeNode anim in node.Nodes)
            {
                animTreeView.Nodes.Add(anim);
            }
        }

        private void selectItem(object sender, TreeNodeMouseClickEventArgs e)
        {
            Viewport viewport = LibraryGUI.Instance.GetActiveViewport();

            if (viewport == null)
                return;

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
                foreach (TreeNode n in animTreeView.Nodes)
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

                if (LibraryGUI.Instance.GetAnimationPanel() != null)
                {
                    Console.WriteLine("running" + running.Text);
                    LibraryGUI.Instance.GetAnimationPanel().CurrentAnimation = running;
                }
            }
        }

        private void animTypeCB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (animTypeCB.SelectedIndex >= 0)
            {
                if (bfres != null)
                {
                    LoadAnimations(bfres.Nodes);
                }
            }
        }

        private void animTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}
