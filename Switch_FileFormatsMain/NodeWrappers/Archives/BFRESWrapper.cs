using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Switch_Toolbox.Library.NodeWrappers;
using Switch_Toolbox.Library;
using System.Windows.Forms;
using ResU = Syroot.NintenTools.Bfres;
using ResNX = Syroot.NintenTools.NSW.Bfres;
using Bfres.Structs;

namespace FirstPlugin.NodeWrappers
{
    public class BFRESWrapper : STGenericWrapper, IContextMenuNode
    {
        public override void OnClick(TreeView treeview)
        {
            base.OnClick(treeview);
        }

        public bool IsWiiU { get; set; }
        public bool SettingRemoveUnusedTextures;

        private ToolStripMenuItem SettingsToolStrip;

        public void LoadMenus(bool isWiiUBfres) {
            IsWiiU = isWiiUBfres;
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.AddRange(base.GetContextMenuItems());
            SettingsToolStrip = new ToolStripMenuItem("Settings", null);
            SettingsToolStrip.DropDownItems.Add(new ToolStripMenuItem("Remove Unused Textures on Save", null, SettingBooleanAction));
            Items.Add(SettingsToolStrip);
            return Items.ToArray();
        }

        public override void Delete()
        {
            var editor = LibraryGUI.Instance.GetObjectEditor();
            if (editor != null)
            {
                editor.RemoveFile(this);
                editor.ResetControls();
            }
        }

        protected void SettingBooleanAction(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                if (((ToolStripMenuItem)sender).Checked)
                    ((ToolStripMenuItem)sender).Checked = false;
                else
                    ((ToolStripMenuItem)sender).Checked = true;
            }
        }
    
    }
}
