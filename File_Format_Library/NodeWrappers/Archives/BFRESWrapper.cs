using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Library.NodeWrappers;
using Toolbox.Library;
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
            SettingsToolStrip.DropDownItems.Add(new ToolStripMenuItem("Remove Unused Textures on Save", null, SettingBooleanAction)
            {
                Checked = SettingRemoveUnusedTextures,
            });
            Items.Add(SettingsToolStrip);
            return Items.ToArray();
        }

        protected void SettingBooleanAction(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                if (((ToolStripMenuItem)sender).Checked)
                    SettingRemoveUnusedTextures = false;
                else
                    SettingRemoveUnusedTextures = true;
            }
        }
    
    }
}
