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
    public partial class LayoutSaveDialog : STForm
    {
        private TreeNode AnimationFolder = new TreeNode("Animations");
        private TreeNode LayoutFolder = new TreeNode("Layouts");
        private TreeNode ShaderFolder = new TreeNode("Shaders");
        private TreeNode TexturesFolder = new TreeNode("Textures");

        public LayoutSaveDialog()
        {
            InitializeComponent();

            treeView1.BackColor = FormThemes.BaseTheme.FormBackColor;
            treeView1.ForeColor = FormThemes.BaseTheme.FormForeColor;
        }

        public void LoadAnimations(List<BxlanHeader> animationHeaders)
        {

        }

        public void LoadArchive(IArchiveFile archiveFile, string fileName)
        {
            TreeNode currentNode = new TreeNode();
            TreeNode root = new TreeNode(fileName);
            treeView1.Nodes.Add(root);
            foreach (var file in archiveFile.Files)
            {
                currentNode = null;

                string nodeString = file.FileName;

                var roots = nodeString.Split(new char[] { '/' },
                      StringSplitOptions.RemoveEmptyEntries);

                var parentNode = root;
                var sb = new System.Text.StringBuilder(fileName, nodeString.Length + fileName.Length);
                for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
                {
                    // Build the node name
                    var parentName = roots[rootIndex];
                    sb.Append("/");
                    sb.Append(parentName);
                    var nodeName = sb.ToString();

                    // Search for the node
                    var index = parentNode.Nodes.IndexOfKey(nodeName);
                    if (index == -1)
                    {
                        var folder = new TreeNode(parentName)
                        { Name = nodeName,Checked = true, };

                        if (rootIndex == roots.Length - 1)
                        {
                            var fileNode = new TreeNode(parentName)
                            { Name = nodeName, Checked = true, };

                            parentNode.Nodes.Add(fileNode);
                            parentNode = fileNode;
                        }
                        else
                        {
                            parentNode.Nodes.Add(folder);
                            parentNode = folder;
                        }
                    }
                    else
                        parentNode = parentNode.Nodes[index];
                }
            }


            root.ExpandAll();
        }

        public void LoadLayouts(List<BxlytHeader> layoutHeaders)
        {
            for (int i = 0; i < layoutHeaders.Count; i++)
            {
                AnimationFolder.Nodes.Add(new TreeNode(layoutHeaders[i].FileName));
            }
        }
    }
}
