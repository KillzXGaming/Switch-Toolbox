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
    public class BFRESWrapper : STGenericWrapper
    {
        public override void OnClick(TreeView treeview)
        {
            base.OnClick(treeview);
        }

        public bool IsWiiU { get; set; }
      
        public bool SettingRemoveUnusedTextures
        {
            get
            {
                return ((ToolStripMenuItem)SettingsToolStrip.DropDownItems[0]).Checked;
            }
        }

        private ToolStripMenuItem SettingsToolStrip;

        public void LoadMenus(bool isWiiUBfres) {
            IsWiiU = isWiiUBfres;

            LoadFileMenus(true);

            SettingsToolStrip = new ToolStripMenuItem("Settings", null);
            SettingsToolStrip.DropDownItems.Add(new ToolStripMenuItem("Remove Unused Textures on Save", null, SettingBooleanAction));
            ContextMenuStrip.Items.Add(SettingsToolStrip);
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
