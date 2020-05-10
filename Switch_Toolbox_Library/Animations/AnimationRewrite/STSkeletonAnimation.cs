using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Toolbox.Library.Animations
{
    public class STSkeletonAnimation : STAnimation, IContextMenuNode
    {
        public virtual STSkeleton GetActiveSkeleton()
        {
            Viewport viewport = LibraryGUI.GetActiveViewport();
            if (viewport == null) return null;

            foreach (var drawable in viewport.scene.objects) {
                if (drawable is STSkeleton)
                    return (STSkeleton)drawable;
            }

            return null;
        }

        public ToolStripItem[] GetContextMenuItems()
        {
            bool hasBones = GetActiveSkeleton() != null;

            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Export Animation", null, ExportAction, Keys.Control | Keys.E)
            { Enabled = hasBones });

            return Items.ToArray();
        }

        private void ExportAction(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "SMD |*.smd;";
            sfd.DefaultExt = "smd";
            sfd.FileName = Name;
            if (sfd.ShowDialog() == DialogResult.OK) {
                SMD.Save(this, sfd.FileName);
            }
        }
    }
}
