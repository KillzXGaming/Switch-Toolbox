using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Library
{
    //A folder that can easily batch export multiple folders
    public class STTextureFolder : TreeNodeCustom, IContextMenuNode
    {
        public STTextureFolder()
        {
        }

        public STTextureFolder(string text)
        {
            Text = text;
        }

        public virtual ToolStripItem[] GetContextMenuItems()
        {
            List<ToolStripItem> Items = new List<ToolStripItem>();
            Items.Add(new ToolStripMenuItem("Export All", null, ExportAllAction, Keys.Control | Keys.E));
            return Items.ToArray();
        }

        private void ExportAllAction(object sender, EventArgs args) {
            ExportAll();
        }

        public virtual void ExportAll()
        {
            List<string> Formats = new List<string>();
            Formats.Add("Microsoft DDS (.dds)");
            Formats.Add("Portable Graphics Network (.png)");
            Formats.Add("Joint Photographic Experts Group (.jpg)");
            Formats.Add("Bitmap Image (.bmp)");
            Formats.Add("Tagged Image File Format (.tiff)");

            FolderSelectDialog sfd = new FolderSelectDialog();

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string folderPath = sfd.SelectedPath;

                BatchFormatExport form = new BatchFormatExport(Formats);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    foreach (STGenericTexture tex in Nodes)
                    {
                        if (form.Index == 0)
                            tex.SaveDDS(folderPath + '\\' + tex.Text + ".dds");
                        else if (form.Index == 1)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".png");
                        else if (form.Index == 2)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".jpg");
                        else if (form.Index == 3)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".bmp");
                        else if (form.Index == 4)
                            tex.SaveBitMap(folderPath + '\\' + tex.Text + ".tiff");
                    }
                }
            }
        }
    }

}
