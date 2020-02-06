using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AampLibraryCSharp;
using Syroot.Maths;
using Toolbox.Library;
using Toolbox.Library.Forms;

namespace FirstPlugin.Forms
{
    public partial class AampEditor : AampEditorBase
    {
        public AampEditor(AAMP aamp, bool IsSaveDialog) : base(aamp, IsSaveDialog)
        {
            treeView1.Nodes.Add(aamp.FileName);
            LoadFile(aamp.aampFile, treeView1.Nodes[0]);
        }

        public void LoadFile(AampFile aampFile, TreeNode parentNode)
        {
            LoadChildNodes(aampFile.RootNode, parentNode);
        }

        public override void TreeView_AfterSelect()
        {
            var node = treeView1.SelectedNode;

            if (node.Tag != null)
            {
                if (node.Tag is ParamObject) {
                    LoadObjectDataList((ParamObject)node.Tag);
                }
            }
        }

        public override void AddParamEntry(TreeNode parentNode)
        {
            if (parentNode.Tag != null && parentNode.Tag is ParamObject)
            {
                ParamEntry entry = new ParamEntry();
                entry.ParamType = ParamType.Float;
                entry.HashString = "NewEntry";
                entry.Value = 0;

                ListViewItem item = new ListViewItem();
                SetListItemParamObject(entry, item);

                OpenNewParamEditor(entry,(ParamObject)parentNode.Tag, item);
            }
        }

        public override void RenameParamEntry(ListViewItem SelectedItem)
        {
            if (SelectedItem.Tag != null && SelectedItem.Tag is ParamEntry)
            {
                RenameDialog dialog = new RenameDialog();
                dialog.SetString(SelectedItem.Text);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string NewString = dialog.textBox1.Text;

                    ((ParamEntry)SelectedItem.Tag).HashString = NewString;
                }
            }
        }

        public override void OnEditorClick(ListViewItem SelectedItem)
        {
            if (SelectedItem.Tag != null && SelectedItem.Tag is ParamEntry)
            {
                OpenEditor((ParamEntry)SelectedItem.Tag, SelectedItem);
            }
        }

        private void LoadObjectDataList(ParamObject paramObj)
        {
            listViewCustom1.Items.Clear();
            foreach (var entry in paramObj.paramEntries)
            {
                ListViewItem item = new ListViewItem(entry.HashString);
                SetListItemParamObject(entry, item);
                listViewCustom1.Items.Add(item);
            }
        }

        private void SetListItemParamObject(ParamEntry entry, ListViewItem item)
        {
            item.SubItems.Clear();
            item.Text = entry.HashString;
            item.Tag = entry;
            item.UseItemStyleForSubItems = false;
            item.SubItems.Add(entry.ParamType.ToString());
            string ValueText = "";

            System.Drawing.Color color = System.Drawing.Color.Empty;

            switch (entry.ParamType)
            {
                case ParamType.Boolean:
                case ParamType.Float:
                case ParamType.Int:
                case ParamType.Uint:
                    ValueText = $"{entry.Value}";
                    break;
                case ParamType.String64:
                case ParamType.String32:
                case ParamType.String256:
                case ParamType.StringRef:
                    ValueText = $"{((AampLibraryCSharp.StringEntry)entry.Value).ToString()}";
                    break;
                case ParamType.Vector2F:
                    var vec2 = (Vector2F)entry.Value;
                    ValueText = $"{vec2.X} {vec2.Y}";
                    break;
                case ParamType.Vector3F:
                    var vec3 = (Vector3F)entry.Value;
                    ValueText = $"{vec3.X} {vec3.Y} {vec3.Z}";
                    break;
                case ParamType.Vector4F:
                    var vec4 = (Vector4F)entry.Value;
                    ValueText = $"{vec4.X} {vec4.Y} {vec4.Z} {vec4.W}";
                    break;
                case ParamType.Color4F:
                    var col = (Vector4F)entry.Value;
                    ValueText = $"{col.X} {col.Y} {col.Z} {col.W}";

                    int ImageIndex = Images.Count;

                    color = System.Drawing.Color.FromArgb(
                    EditBox.FloatToIntClamp(col.W),
                    EditBox.FloatToIntClamp(col.X),
                    EditBox.FloatToIntClamp(col.Y),
                    EditBox.FloatToIntClamp(col.Z));
                    break;
                default:
                    break;
            }

            item.SubItems.Add(ValueText);

            if (color != System.Drawing.Color.Empty)
                item.SubItems[2].BackColor = color;
        }

        public override void OnEntryDeletion(object obj, TreeNode objNode)
        {
            if (obj is ParamEntry)
            {
                var paramObjectParent = (ParamObject)objNode.Tag;

                var entryList = new List<ParamEntry>();
                for (int i = 0; i < paramObjectParent.paramEntries.Length; i++)
                    entryList.Add(paramObjectParent.paramEntries[i]);

                entryList.Remove((ParamEntry)obj);

                paramObjectParent.paramEntries = entryList.ToArray();
            }
        }

        public void LoadChildNodes(ParamList paramList, TreeNode parentNode)
        {
            TreeNode newNode = new TreeNode(paramList.HashString);
            newNode.Tag = paramList;

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
                SetObjNode(obj, newNode.Nodes["obj"]);
        }

        List<Bitmap> Images = new List<Bitmap>();

        void SetObjNode(ParamObject paramObj, TreeNode parentNode)
        {
            string name = paramObj.HashString;

            var objNode = new TreeNode(name);
            objNode.Tag = paramObj;
            parentNode.Nodes.Add(objNode);
        }

        private void OpenNewParamEditor(ParamEntry entry, ParamObject paramObject, ListViewItem SelectedItem)
        {
            EditBox editor = new EditBox();
            editor.LoadEntry(entry);
            editor.ToggleNameEditing(true);

            if (editor.ShowDialog() == DialogResult.OK)
            {
                editor.SaveEntry();
                SetListItemParamObject(entry, SelectedItem);
                listViewCustom1.Items.Add(SelectedItem);

                var entryList = new List<ParamEntry>();
                for (int i = 0; i < paramObject.paramEntries.Length; i++)
                    entryList.Add(paramObject.paramEntries[i]);

                entryList.Add(entry);
                paramObject.paramEntries = entryList.ToArray();
            }
        }

        private void OpenEditor(ParamEntry entry, ListViewItem SelectedItem)
        {
            EditBox editor = new EditBox();
            editor.LoadEntry(entry);
            editor.ToggleNameEditing(true);

            if (editor.ShowDialog() == DialogResult.OK)
            {
                editor.SaveEntry();
                SetListItemParamObject(entry, SelectedItem);
            }
        }
    }
}
