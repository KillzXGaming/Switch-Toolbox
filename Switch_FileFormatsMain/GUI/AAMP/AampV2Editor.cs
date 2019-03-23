using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AampV2Library;
using Syroot.Maths;
using Switch_Toolbox.Library;

namespace FirstPlugin.Forms
{
    public partial class AampV2Editor : UserControl
    {
        public AampV2Editor()
        {
            InitializeComponent();
        }

        public void LoadFile(AampFile aampFile, TreeNode parentNode)
        {
            LoadChildNodes(aampFile.RootNode, parentNode);
        }

        public void LoadImages(TreeView treeView, TreeNode parentNode)
        {
            foreach (TreeNode nodes in TreeViewExtensions.Collect(parentNode.Nodes))
            {
                if (nodes is EditableEntry)
                {
                    if (((EditableEntry)nodes).entry.ParamType == ParamType.Color4F)
                    {
                        nodes.ImageIndex += treeView.ImageList.Images.Count;
                        nodes.SelectedImageIndex = nodes.ImageIndex;
                    }
                }
            }

            foreach (var image in Images)
                treeView.ImageList.Images.Add(image);
        }

        public void LoadChildNodes(ParamList paramList, TreeNode parentNode)
        {
            TreeNode newNode = new TreeNode(paramList.HashString);
            parentNode.Nodes.Add(newNode);

            //Add lists and objects if exits
            if (paramList.childParams.Length > 0)
                newNode.Nodes.Add("list", "Lists {}");
            if (paramList.paramObjects.Length > 0)
                newNode.Nodes.Add("obj", "Objects {}");

            //Add child nodes
            foreach (var child in paramList.childParams)
                LoadChildNodes(child, newNode.Nodes["list"]);

            //Add object nodes
            foreach (var obj in paramList.paramObjects)
                LoadObjectNodes(obj, newNode.Nodes["obj"]);
        }

        public void LoadObjectNodes(ParamObject paramObject, TreeNode parentNode)
        {
            TreeNode objNode = new TreeNode(paramObject.HashString);
            parentNode.Nodes.Add(objNode);

            foreach (var entry in paramObject.paramEntries)
            {
                EditableEntry entryNode = new EditableEntry($"{entry.HashString}");
                entryNode.entry = entry;
                objNode.Nodes.Add(entryNode);

                UpdateEntryNode(entryNode, entry);
            }
        }

        List<Bitmap> Images = new List<Bitmap>();

        public void UpdateEntryNode(EditableEntry node, ParamEntry entry)
        {
            node.ImageKey = "empty";
            node.SelectedImageKey = "empty";

            switch (entry.ParamType)
            {
                case ParamType.Boolean:
                case ParamType.Float:
                case ParamType.Int:
                case ParamType.Uint:
                case ParamType.String64:
                case ParamType.String32:
                case ParamType.String256:
                case ParamType.StringRef:
                    node.Text = $"{entry.HashString} {entry.Value}";
                    break;
                case ParamType.Vector2F:
                    var vec2 = (Vector2F)entry.Value;
                    node.Text = $"{entry.HashString} {vec2.X} {vec2.Y}";
                    break;
                case ParamType.Vector3F:
                    var vec3 = (Vector3F)entry.Value;
                    node.Text = $"{entry.HashString} {vec3.X} {vec3.Y} {vec3.Z}";
                    break;
                case ParamType.Vector4F:
                    var vec4 = (Vector4F)entry.Value;
                    node.Text = $"{entry.HashString} {vec4.X} {vec4.Y} {vec4.Z} {vec4.W}";
                    break;
                case ParamType.Color4F:
                    var col = (Vector4F)entry.Value;
                    node.Text = $"{entry.HashString} {col.X} {col.Y} {col.Z} {col.W}";

                    int ImageIndex = Images.Count;
                    node.ImageIndex = ImageIndex;

                    var color = System.Drawing.Color.FromArgb(
                    Utils.FloatToIntClamp(col.W),
                    Utils.FloatToIntClamp(col.X),
                    Utils.FloatToIntClamp(col.Y),
                    Utils.FloatToIntClamp(col.Z));

                    Bitmap bmp = new Bitmap(32, 32);
                    Graphics g = Graphics.FromImage(bmp);
                    g.Clear(color);

                    Images.Add(bmp);
                    break;
                default:
                    break;
            }
        }

        public class EditableEntry : TreeNode
        {
            public ParamEntry entry;
            public EditableEntry(string name)
            {
                Text = name;

                ContextMenu = new ContextMenu();
                ContextMenu.MenuItems.Add(new MenuItem("Edit", OpenEditor));
            }

            private void OpenEditor(object sender, EventArgs e)
            {
                MessageBox.Show("Editing v2 AAMP not supported yet. Only v1", "", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;

                EditBox editor = new EditBox();
                editor.LoadEntry(entry);

                if (editor.ShowDialog() == DialogResult.OK)
                {
                    editor.SaveEntry();

                    switch (entry.ParamType)
                    {
                        case ParamType.Boolean:
                        case ParamType.Float:
                        case ParamType.Int:
                        case ParamType.Uint:
                        case ParamType.String64:
                        case ParamType.String32:
                        case ParamType.String256:
                        case ParamType.StringRef:
                            Text = $"{entry.HashString} {entry.Value}";
                            break;
                        case ParamType.Vector2F:
                            var vec2 = (Vector2F)entry.Value;
                            Text = $"{entry.HashString} {vec2.X} {vec2.Y}";
                            break;
                        case ParamType.Vector3F:
                            var vec3 = (Vector3F)entry.Value;
                            Text = $"{entry.HashString} {vec3.X} {vec3.Y} {vec3.Z}";
                            break;
                        case ParamType.Vector4F:
                            var vec4 = (Vector4F)entry.Value;
                            Text = $"{entry.HashString} {vec4.X} {vec4.Y} {vec4.Z} {vec4.W}";
                            break;
                        case ParamType.Color4F:
                            var col = (Vector4F)entry.Value;
                            Text = $"{entry.HashString} {col.X} {col.Y} {col.Z} {col.W}";
                            break;
                    }
                }
            }
        }
    }
}
